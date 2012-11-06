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
using System.ComponentModel;
using System.Collections.Generic;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Logging;
using Telerik.Windows.Controls;
using GreenField.ServiceCaller.SecurityReferenceDefinitions;
using GreenField.Common;
using GreenField.ServiceCaller;
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
        protected const string dbfExtension = "dbf";
        /// <summary>
        /// Region for Heat Map
        /// </summary>
        private string region;
        /// <summary>
        /// Uri for Shape file
        /// </summary>
        private Uri shapefileSourceUri;
        /// <summary>
        /// Uri for Data Source file
        /// </summary>
        private Uri shapefileDataSourceUri;       
        /// <summary>
        /// private member object of the IEventAggregator for event aggregation
        /// </summary>
        public IEventAggregator eventAggregator;
        /// <summary>
        /// private member object of the IDBInteractivity for interaction with the Service Caller
        /// </summary>
        private IDBInteractivity dbInteractivity = new DBInteractivity();
        /// <summary>
        /// private member object of ILoggerFacade for logging
        /// </summary>
        private ILoggerFacade logger;
        /// <summary>
        /// private member object of the PortfolioSelectionData class for storing Portfolio Selection Data
        /// </summary>
        private PortfolioSelectionData portfolioSelectionData;
         /// <summary>
        /// Effective Date 
       /// </summary>
        private DateTime? effectiveDate;
        /// <summary>
        /// Selected Period
        /// </summary>
        private String selectedPeriod;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor of the class that initializes various objects
        /// </summary>
        /// <param name="param">MEF Eventaggrigator instance</param>
        public ViewModelHeatMap(DashboardGadgetParam param)
        {
            eventAggregator = param.EventAggregator;
            dbInteractivity = param.DBInteractivity;
            logger = param.LoggerFacade;
            portfolioSelectionData = param.DashboardGadgetPayload.PortfolioSelectionData;          
            effectiveDate = param.DashboardGadgetPayload.EffectiveDate;
            selectedPeriod = param.DashboardGadgetPayload.PeriodSelectionData;
            this.ShapefileSourceUri = new Uri(string.Format(ShapeRelativeUriFormat, "world", ShapeExtension), UriKind.Relative);
            this.ShapefileDataSourceUri = new Uri(string.Format(ShapeRelativeUriFormat, "world", dbfExtension), UriKind.Relative);
            if (effectiveDate != null && portfolioSelectionData != null && selectedPeriod != null && IsActive)
            {
                dbInteractivity.RetrieveHeatMapData(portfolioSelectionData, Convert.ToDateTime(effectiveDate),selectedPeriod, RetrieveHeatMapDataCallbackMethod);
            }
            if (eventAggregator != null)
            {
                eventAggregator.GetEvent<PortfolioReferenceSetEvent>().Subscribe(HandleFundReferenceSet, false);
                eventAggregator.GetEvent<EffectiveDateReferenceSetEvent>().Subscribe(HandleEffectiveDateSet, false);
                eventAggregator.GetEvent<PeriodReferenceSetEvent>().Subscribe(HandlePeriodReferenceSet, false);
            } 
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
                return this.shapefileSourceUri;
            }
            set
            {
                if (this.shapefileSourceUri != value)
                {
                    this.shapefileSourceUri = value;
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
                return this.shapefileDataSourceUri;
            }
            set
            {
                if (this.shapefileDataSourceUri != value)
                {
                    this.shapefileDataSourceUri = value;
                    this.RaisePropertyChanged("ShapefileDataSourceUri");
                }
            }
        }        
        /// <summary>
        /// Collection that stores the Heat Map Data
        /// </summary>
        private List<HeatMapData> heatMapInfo;
        public List<HeatMapData> HeatMapInfo
        {
            get
            {
                if (heatMapInfo == null)
                {
                    if (portfolioSelectionData != null && effectiveDate != null && selectedPeriod != null)
                    {
                        dbInteractivity.RetrieveHeatMapData(portfolioSelectionData, Convert.ToDateTime(effectiveDate), selectedPeriod, RetrieveHeatMapDataCallbackMethod);
                    }                  
                }
                return heatMapInfo;
            }
            set
            {
                if (heatMapInfo != value)
                {
                    heatMapInfo = value;
                    this.RaisePropertyChanged("HeatMapInfo");
                }
            }
        }
        private bool isActive;
        /// <summary>
        /// IsActive is true when parent control is displayed on UI
        /// </summary>
        public bool IsActive
        {
            get
            {
                return isActive;
            }
            set
            {
                isActive = value;
                if (portfolioSelectionData != null && effectiveDate != null && selectedPeriod != null && isActive)
                {
                    BeginWebServiceCall(portfolioSelectionData, Convert.ToDateTime(effectiveDate), selectedPeriod);
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
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null && result.Count > 0)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result, 1);
                    HeatMapInfo = result;
                    if (null != HeatMapDataLoadedEvent)
                    {
                        HeatMapDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
                    }
                    RetrieveHeatMapDataCompletedEvent(new RetrieveHeatMapDataCompleteEventArgs() { HeatMapInfo = result });
                }                
                else
                {
                    HeatMapInfo = result;
                    Logging.LogMethodParameterNull(logger, methodNamespace, 1);
                    if (null != HeatMapDataLoadedEvent)
                    {
                        HeatMapDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
                    }
                    RetrieveHeatMapDataCompletedEvent(new RetrieveHeatMapDataCompleteEventArgs() { HeatMapInfo = result });
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);               
        }
        #endregion

        #region Event
        /// <summary>
        /// Event for the notification of Data Load Completion
        /// </summary>
        public event DataRetrievalProgressIndicatorEventHandler HeatMapDataLoadedEvent;
         /// <summary>
        /// Event for the Retrieval of Data 
        /// </summary>
        public event RetrieveHeatMapDataCompleteEventHandler RetrieveHeatMapDataCompletedEvent;      
        #endregion

        #region Event Handlers
        /// <summary>
        /// Assigns UI Field Properties based on Portfolio reference
        /// </summary>
        /// <param name="portfSelectionData">Object of PortfolioSelectionData class containg the Portfolio Selection Data </param>
        public void HandleFundReferenceSet(PortfolioSelectionData portfSelectionData)
        {

            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (portfSelectionData != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, portfSelectionData, 1);
                    portfolioSelectionData = portfSelectionData;
                    if (portfSelectionData != null && effectiveDate != null && selectedPeriod != null && IsActive)
                    {
                        BeginWebServiceCall(portfolioSelectionData, Convert.ToDateTime(effectiveDate), selectedPeriod);
                    }
                }
                else
                {
                    Logging.LogMethodParameterNull(logger, methodNamespace, 1);
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }

        /// <summary>
        /// Assigns UI Field Properties based on Effective Date
        /// </summary>
        /// <param name="effectDate">Effective Date selected by the user</param>
        public void HandleEffectiveDateSet(DateTime effectDate)
        {

            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (effectDate != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, effectDate, 1);
                    effectiveDate = effectDate;
                    if (portfolioSelectionData != null && effectiveDate != null && selectedPeriod != null && IsActive)
                    {
                        BeginWebServiceCall(portfolioSelectionData, Convert.ToDateTime(effectiveDate), selectedPeriod);
                    }
                }
                else
                {
                    Logging.LogMethodParameterNull(logger, methodNamespace, 1);
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }

        /// <summary>
        /// Assigns UI Field Properties based on Period
        /// </summary>
        /// <param name="selectedPeriodType">Period selected by the user</param>
        public void HandlePeriodReferenceSet(String selectedPeriodType)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (selectedPeriodType != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, selectedPeriodType, 1);
                    selectedPeriod = selectedPeriodType;
                    if (portfolioSelectionData != null && effectiveDate != null && selectedPeriod != null && IsActive)
                    {
                        BeginWebServiceCall(portfolioSelectionData, Convert.ToDateTime(effectiveDate), selectedPeriod);
                    }
                }
                else
                {
                    Logging.LogMethodParameterNull(logger, methodNamespace, 1);
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);
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
            if (portfolioSelectionData != null && effectiveDate != null && selectedPeriod != null)
            {
                if (null != HeatMapDataLoadedEvent)
                    HeatMapDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
                dbInteractivity.RetrieveHeatMapData(PortfolioSelectionData, effectiveDate, selectedPeriodType, RetrieveHeatMapDataCallbackMethod);          
            }
        }
        #endregion

        #region EventUnSubscribe
        /// <summary>
        /// Method that disposes the events
        /// </summary>
        public void Dispose()
        {
            eventAggregator.GetEvent<PortfolioReferenceSetEvent>().Unsubscribe(HandleFundReferenceSet);
            eventAggregator.GetEvent<EffectiveDateReferenceSetEvent>().Unsubscribe(HandleEffectiveDateSet);
            eventAggregator.GetEvent<PeriodReferenceSetEvent>().Unsubscribe(HandlePeriodReferenceSet);
        }
        #endregion
    }
}
