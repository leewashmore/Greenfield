using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Practices.Prism.ViewModel;
using Telerik.Windows.Controls;
using System.Collections.Generic;
using GreenField.ServiceCaller.SecurityReferenceDefinitions;
using GreenField.Common;
using Microsoft.Practices.Prism.Events;
using GreenField.ServiceCaller;
using Microsoft.Practices.Prism.Logging;
using System.ComponentModel;
using GreenField.ServiceCaller.BenchmarkHoldingsDefinitions;
using GreenField.DataContracts;

namespace GreenField.Gadgets.ViewModels
{
    public class ViewModelHeatMap : NotificationObject
    {
        #region Fields
        /// <summary>
        /// Relative Uri
        /// </summary>
        protected const string ShapeRelativeUriFormat = "DataSources/Geospatial/{0}.{1}";
        /// <summary>
        /// Extension for shape file
        /// </summary>
        protected const string ShapeExtension = "shp";
        /// <summary>
        /// Extension for Data source file
        /// </summary>
        protected const string DbfExtension = "dbf";

        private string _region ;
        /// <summary>
        /// Uri for Shape file
        /// </summary>
        private Uri _shapefileSourceUri;
        /// <summary>
        /// Uri for Data Source file
        /// </summary>
        private Uri _shapefileDataSourceUri;
       
        /// <summary>
        /// private member object of the IEventAggregator for event aggregation
        /// </summary>
        public IEventAggregator _eventAggregator;

        /// <summary>
        /// private member object of the IDBInteractivity for interaction with the Service Caller
        /// </summary>
        private IDBInteractivity _dbInteractivity = new DBInteractivity();

        /// <summary>
        /// private member object of ILoggerFacade for logging
        /// </summary>
        private ILoggerFacade _logger;

        /// <summary>
        /// private member object of the PortfolioSelectionData class for storing Portfolio Selection Data
        /// </summary>
        private PortfolioSelectionData _PortfolioSelectionData;
         /// <summary>
        /// Effective Date 
       /// </summary>
        private DateTime? _effectiveDate;

        private String _selectedPeriod;
        #endregion

        #region Constructor

        /// <summary>
        /// Constructor of the class that initializes various objects
        /// </summary>
        /// <param name="param">MEF Eventaggrigator instance</param>
        public ViewModelHeatMap(DashboardGadgetParam param)
        {
            _eventAggregator = param.EventAggregator;
            _dbInteractivity = param.DBInteractivity;
            _logger = param.LoggerFacade;
            _PortfolioSelectionData = param.DashboardGadgetPayload.PortfolioSelectionData;          
            _effectiveDate = param.DashboardGadgetPayload.EffectiveDate;
            _selectedPeriod = param.DashboardGadgetPayload.PeriodSelectionData;
            this.ShapefileSourceUri = new Uri(string.Format(ShapeRelativeUriFormat, "world", ShapeExtension), UriKind.Relative);
            this.ShapefileDataSourceUri = new Uri(string.Format(ShapeRelativeUriFormat, "world", DbfExtension), UriKind.Relative);
            if (_effectiveDate != null && _PortfolioSelectionData != null && _selectedPeriod != null && IsActive)
            {
                _dbInteractivity.RetrieveHeatMapData(_PortfolioSelectionData, Convert.ToDateTime(_effectiveDate),_selectedPeriod, RetrieveHeatMapDataCallbackMethod);
            }
            if (_eventAggregator != null)
            {
                _eventAggregator.GetEvent<PortfolioReferenceSetEvent>().Subscribe(HandleFundReferenceSet, false);
                _eventAggregator.GetEvent<EffectiveDateReferenceSetEvent>().Subscribe(HandleEffectiveDateSet, false);
                _eventAggregator.GetEvent<PeriodReferenceSetEvent>().Subscribe(HandlePeriodReferenceSet, false);
            }  
            //_dbInteractivity.RetrieveHeatMapData(_PortfolioSelectionData, Convert.ToDateTime(_effectiveDate), RetrieveHeatMapDataCallbackMethod);
        }

        #endregion      
       
        #region Properties
        /// <summary>
        /// Property that stores the Uri for Shape file Source
        /// </summary>
        public Uri ShapefileSourceUri
        {
            get
            {
                return this._shapefileSourceUri;
            }
            set
            {
                if (this._shapefileSourceUri != value)
                {
                    this._shapefileSourceUri = value;
                    this.RaisePropertyChanged("ShapefileSourceUri");
                }
            }
        }
        /// <summary>
        /// Property that stores the Uri for Data Source file
        /// </summary>
        public Uri ShapefileDataSourceUri
        {
            get
            {
                return this._shapefileDataSourceUri;
            }
            set
            {
                if (this._shapefileDataSourceUri != value)
                {
                    this._shapefileDataSourceUri = value;
                    this.RaisePropertyChanged("ShapefileDataSourceUri");
                }
            }
        }        
        /// <summary>
        /// Collection that stores the Heat Map Data
        /// </summary>
        private List<HeatMapData> _heatMapInfo;
        public List<HeatMapData> HeatMapInfo
        {
            get

            {
                if (_heatMapInfo == null)
                {
                    if (_PortfolioSelectionData != null && _effectiveDate != null && _selectedPeriod != null)
                    {
                        _dbInteractivity.RetrieveHeatMapData(_PortfolioSelectionData, Convert.ToDateTime(_effectiveDate),_selectedPeriod, RetrieveHeatMapDataCallbackMethod);
                    }
                  
                }
                return _heatMapInfo;
            }
            set
            {
                if (_heatMapInfo != value)
                {
                    _heatMapInfo = value;
                    this.RaisePropertyChanged("HeatMapInfo");
                }
            }
        }

        private bool _isActive;
        /// <summary>
        /// IsActive is true when parent control is displayed on UI
        /// </summary>
        public bool IsActive
        {
            get
            {
                return _isActive;
            }
            set
            {
                _isActive = value;
                if (_PortfolioSelectionData != null && _effectiveDate != null && _selectedPeriod != null && _isActive)
                {
                    BeginWebServiceCall(_PortfolioSelectionData, Convert.ToDateTime(_effectiveDate), _selectedPeriod);
                }
            }
        }
        #endregion

        #region CallbackMethods
        /// <summary>
        /// Retrieves Heat Map Data from the Web Service
        /// </summary>
        /// <param name="result">List of the type of HeatMapData</param>
        void RetrieveHeatMapDataCallbackMethod(List<HeatMapData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);

            try
            {
                if (result != null && result.Count > 0)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    HeatMapInfo = result;
                    if (null != heatMapDataLoadedEvent)
                        heatMapDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
                    RetrieveHeatMapDataCompletedEvent(new RetrieveHeatMapDataCompleteEventArgs() { HeatMapInfo = result });
                }
                
                else
                {
                    HeatMapInfo = result;
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                    if (null != heatMapDataLoadedEvent)
                        heatMapDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
               
        }
        #endregion

        #region Event
        /// <summary>
        /// Event for the notification of Data Load Completion
        /// </summary>
        public event DataRetrievalProgressIndicatorEventHandler heatMapDataLoadedEvent;
         /// <summary>
        /// Event for the Retrieval of Data 
        /// </summary>
        public event RetrieveHeatMapDataCompleteEventHandler RetrieveHeatMapDataCompletedEvent;      
        #endregion

        #region Event Handlers
        /// <summary>
        /// Assigns UI Field Properties based on Portfolio reference
        /// </summary>
        /// <param name="PortfolioSelectionData">Object of PortfolioSelectionData class containg the Portfolio Selection Data </param>
        public void HandleFundReferenceSet(PortfolioSelectionData PortfolioSelectionData)
        {

            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (PortfolioSelectionData != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, PortfolioSelectionData, 1);
                    _PortfolioSelectionData = PortfolioSelectionData;
                    if (PortfolioSelectionData != null && _effectiveDate != null && _selectedPeriod != null && IsActive)
                    {
                        BeginWebServiceCall(_PortfolioSelectionData, Convert.ToDateTime(_effectiveDate), _selectedPeriod);
                    }
                }
                else
                {
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        /// <summary>
        /// Assigns UI Field Properties based on Effective Date
        /// </summary>
        /// <param name="effectiveDate">Effective Date selected by the user</param>
        public void HandleEffectiveDateSet(DateTime effectiveDate)
        {

            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (effectiveDate != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, effectiveDate, 1);
                    _effectiveDate = effectiveDate;
                    if (_PortfolioSelectionData != null && _effectiveDate != null && _selectedPeriod != null && IsActive)
                    {
                        BeginWebServiceCall(_PortfolioSelectionData, Convert.ToDateTime(_effectiveDate), _selectedPeriod);
                    }
                }
                else
                {
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);

        }

        /// <summary>
        /// Assigns UI Field Properties based on Period
        /// </summary>
        /// <param name="selectedPeriodType">Period selected by the user</param>
        public void HandlePeriodReferenceSet(String selectedPeriodType)
        {

            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (selectedPeriodType != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, selectedPeriodType, 1);
                    _selectedPeriod = selectedPeriodType;
                    if (_PortfolioSelectionData != null && _effectiveDate != null && _selectedPeriod != null && IsActive)
                    {
                        BeginWebServiceCall(_PortfolioSelectionData, Convert.ToDateTime(_effectiveDate), _selectedPeriod);
                    }
                }
                else
                {
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);

        }

        /// <summary>
        /// Calls the webservice to retrieve the data
        /// </summary>
        /// <param name="PortfolioSelectionData">Selected Portfolio</param>
        /// <param name="effectiveDate">Selected Effective Date</param>
        /// <param name="selectedPeriodType">Selected Period Type</param>
        private void BeginWebServiceCall(PortfolioSelectionData PortfolioSelectionData, DateTime effectiveDate,
            String selectedPeriodType)
        {
            if (_PortfolioSelectionData != null && _effectiveDate != null && _selectedPeriod != null)
            {
                if (null != heatMapDataLoadedEvent)
                    heatMapDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
                _dbInteractivity.RetrieveHeatMapData(PortfolioSelectionData, effectiveDate, selectedPeriodType, RetrieveHeatMapDataCallbackMethod);
                //SelectedPeriod = selectedPeriodType;
            }
        }

        #endregion

        #region EventUnSubscribe
        /// <summary>
        /// Method that disposes the events
        /// </summary>
        public void Dispose()
        {
            _eventAggregator.GetEvent<PortfolioReferenceSetEvent>().Unsubscribe(HandleFundReferenceSet);
            _eventAggregator.GetEvent<EffectiveDateReferenceSetEvent>().Unsubscribe(HandleEffectiveDateSet);
            _eventAggregator.GetEvent<PeriodReferenceSetEvent>().Unsubscribe(HandlePeriodReferenceSet);
        }

        #endregion
    }
}
