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
using Microsoft.Practices.Prism.Events;
using GreenField.ServiceCaller;
using Microsoft.Practices.Prism.Logging;
using GreenField.ServiceCaller.SecurityReferenceDefinitions;
using GreenField.Common;
using System.Collections.Generic;
using Microsoft.Practices.Prism.ViewModel;
using GreenField.ServiceCaller.BenchmarkHoldingsDefinitions;
using GreenField.DataContracts;

namespace GreenField.Gadgets.ViewModels
{
    /// <summary>
    /// Class that provides the interaction of the view model with the Service caller and the View.
    /// </summary>
    public class ViewModelPerformanceGrid : NotificationObject
    {
        #region PrivateMembers

        /// <summary>
        /// private member object of the IEventAggregator for event aggregation
        /// </summary>
        private IEventAggregator _eventAggregator;

        /// <summary>
        /// private member object of the IDBInteractivity for interaction with the Service Caller
        /// </summary>
        private IDBInteractivity _dbInteractivity;

        /// <summary>
        /// private member object of ILoggerFacade for logging
        /// </summary>
        private ILoggerFacade _logger;

        /// <summary>
        /// private member object of the PortfolioSelectionData class for storing Fund Selection Data
        /// </summary>
        private PortfolioSelectionData _PortfolioSelectionData;

        /// <summary>
        /// Stores Effective Date selected by the user
        /// </summary>
        private DateTime? _effectiveDate;

        private String _country;



        #endregion

        #region Constructor

        /// <summary>
        /// Constructor of the class that initializes various objects
        /// </summary>
        /// <param name="param">MEF Eventaggrigator instance</param>
        public ViewModelPerformanceGrid(DashboardGadgetParam param)
        {
            _effectiveDate = param.DashboardGadgetPayload.EffectiveDate;
            _dbInteractivity = param.DBInteractivity;
            _logger = param.LoggerFacade;
            _eventAggregator = param.EventAggregator;
            _country = param.DashboardGadgetPayload.HeatMapCountryData;
            _PortfolioSelectionData = param.DashboardGadgetPayload.PortfolioSelectionData;

            if (_effectiveDate != null && _PortfolioSelectionData != null && IsActive)
            {
                _dbInteractivity.RetrievePerformanceGridData(_PortfolioSelectionData, Convert.ToDateTime(_effectiveDate),"NoFiltering", RetrievePerformanceGridDataCallbackMethod);
            }          
            if (_eventAggregator != null)
            {
                _eventAggregator.GetEvent<PortfolioReferenceSetEvent>().Subscribe(HandlePortfolioReferenceSet, false);
                _eventAggregator.GetEvent<EffectiveDateReferenceSetEvent>().Subscribe(HandleEffectiveDateSet, false);
                _eventAggregator.GetEvent<HeatMapClickEvent>().Subscribe(HandleCountrySelectionDataSet, false);
                
            }  
        }
        #endregion

        #region Properties
        #region UI Fields

        /// <summary>
        /// Collection binded to the Grid
        /// </summary>
        private List<PerformanceGridData> _performanceGridInfo;
        public List<PerformanceGridData> PerformanceInfo
        {
            get
            {
                return _performanceGridInfo;
            }
            set
            {
                if (_performanceGridInfo != value)
                {
                    _performanceGridInfo = value;
                    RaisePropertyChanged(() => this.PerformanceInfo);
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
                if (_PortfolioSelectionData != null && _effectiveDate != null && _isActive)
                {
                    if (_country != null)
                        BeginWebServiceCall(_PortfolioSelectionData, Convert.ToDateTime(_effectiveDate), _country);
                    else
                        BeginWebServiceCall(_PortfolioSelectionData, Convert.ToDateTime(_effectiveDate), "NoFiltering");
                }
            }
        }


        #endregion
        #endregion

        #region Events
        /// <summary>
        /// Event for the notification of Data Load Completion
        /// </summary>
        public event DataRetrievalProgressIndicatorEventHandler performanceGridDataLoadedEvent;
        #endregion

        #region Event Handlers
        /// <summary>
        /// Assigns UI Field Properties based on Fund reference
        /// </summary>
        /// <param name="PortfolioSelectionData">Object of PortfolioSelectionData Class</param>
        public void HandlePortfolioReferenceSet(PortfolioSelectionData PortfolioSelectionData)
        {

            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (PortfolioSelectionData != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, PortfolioSelectionData, 1);
                    _PortfolioSelectionData = PortfolioSelectionData;
                    if (_effectiveDate != null && _PortfolioSelectionData != null && IsActive)
                    {                        
                        BeginWebServiceCall(PortfolioSelectionData, Convert.ToDateTime(_effectiveDate), "NoFiltering");
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
        /// Assigns UI Field Properties based on effectiveDate
        /// </summary>
        /// <param name="effectiveDate">effective Date selected by the user</param>
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
                    if (_PortfolioSelectionData != null && _effectiveDate != null && IsActive)
                    {                        
                        BeginWebServiceCall(_PortfolioSelectionData, Convert.ToDateTime(_effectiveDate), "NoFiltering");
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
        /// Assigns UI Field Properties based on Country
        /// </summary>
        /// <param name="country">Country selected by the user from the heat map</param>
        public void HandleCountrySelectionDataSet(String country)
        {

            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (country != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, country, 1);
                    _country = country;
                    if (_PortfolioSelectionData != null && _effectiveDate != null && _country !=null && IsActive)
                    {                        
                        BeginWebServiceCall(_PortfolioSelectionData, Convert.ToDateTime(_effectiveDate), country);
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

        private void BeginWebServiceCall(PortfolioSelectionData PortfolioSelectionData, DateTime effectiveDate, String country)
        {
            if (null != performanceGridDataLoadedEvent)
                performanceGridDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
            _dbInteractivity.RetrievePerformanceGridData(PortfolioSelectionData, effectiveDate, country, RetrievePerformanceGridDataCallbackMethod);
        }

        #endregion

        #region CallbackMethods
        /// <summary>
        /// Plots the result on grid after getting the resulting collection
        /// </summary>
        /// <param name="result">Contains the Performance Grid Data</param>
        private void RetrievePerformanceGridDataCallbackMethod(List<PerformanceGridData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null && result.Count > 0)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    PerformanceInfo = result; 
                        if (null != performanceGridDataLoadedEvent)
                            performanceGridDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
                }
                else
                {
                    PerformanceInfo = result;   
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                    performanceGridDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
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

        #region EventUnSubscribe
        /// <summary>
        /// Method that disposes the events
        /// </summary>
        public void Dispose()
        {
            _eventAggregator.GetEvent<PortfolioReferenceSetEvent>().Unsubscribe(HandlePortfolioReferenceSet);
            _eventAggregator.GetEvent<EffectiveDateReferenceSetEvent>().Unsubscribe(HandleEffectiveDateSet);            
            _eventAggregator.GetEvent<HeatMapClickEvent>().Unsubscribe(HandleCountrySelectionDataSet);

        }

        #endregion

    }
}
