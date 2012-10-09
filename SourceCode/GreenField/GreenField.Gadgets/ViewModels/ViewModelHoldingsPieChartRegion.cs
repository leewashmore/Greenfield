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
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.ViewModel;
using GreenField.ServiceCaller.BenchmarkHoldingsDefinitions;
using GreenField.Common;
using GreenField.DataContracts;
using GreenField.ServiceCaller;

namespace GreenField.Gadgets.ViewModels
{
    /// <summary>
    /// Class that provides the interaction of the view model with the Service caller and the View.
    /// </summary>    
    public class ViewModelHoldingsPieChartRegion : NotificationObject
    {
        #region PrivateMembers
        /// <summary>
        /// private member object of the IEventAggregator for event aggregation
        /// </summary>
        private IEventAggregator eventAggregator;
        /// <summary>
        /// private member object of the IDBInteractivity for interaction with the Service Caller
        /// </summary>
        private IDBInteractivity dbInteractivity;
        /// <summary>
        /// private member object of ILoggerFacade for logging
        /// </summary>
        private ILoggerFacade logger;
        /// <summary>
        /// private member object of the PortfolioSelectionData class for storing Fund Selection Data
        /// </summary>
        private PortfolioSelectionData portfolioSelectionData;
        /// <summary>
        /// Private member containing the Key Value Pair
        /// </summary>
        private FilterSelectionData holdingDataFilter;
        /// <summary>
        /// Private member to store info about look thru enabled or not
        /// </summary>
        private bool lookThruEnabled;

        #endregion
        
        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="param">MEF Eventaggrigator instance</param>
        public ViewModelHoldingsPieChartRegion(DashboardGadgetParam param)
        {
            dbInteractivity = param.DBInteractivity;
            logger = param.LoggerFacade;
            eventAggregator = param.EventAggregator;
            portfolioSelectionData = param.DashboardGadgetPayload.PortfolioSelectionData;
            EffectiveDate = param.DashboardGadgetPayload.EffectiveDate;
            holdingDataFilter = param.DashboardGadgetPayload.FilterSelectionData;
            lookThruEnabled = param.DashboardGadgetPayload.IsLookThruEnabled;

            if (EffectiveDate != null && portfolioSelectionData != null && holdingDataFilter != null && IsActive)
            {
                dbInteractivity.RetrieveHoldingsPercentageDataForRegion(portfolioSelectionData, Convert.ToDateTime(EffectiveDate), holdingDataFilter.Filtertype, holdingDataFilter.FilterValues,lookThruEnabled, RetrieveHoldingsPercentageDataForRegionCallbackMethod);
            }
            if (eventAggregator != null)
            {
                eventAggregator.GetEvent<PortfolioReferenceSetEvent>().Subscribe(HandleFundReferenceSetEvent);
                eventAggregator.GetEvent<EffectiveDateReferenceSetEvent>().Subscribe(HandleEffectiveDateReferenceSetEvent);
                eventAggregator.GetEvent<HoldingFilterReferenceSetEvent>().Subscribe(HandleFilterReferenceSetEvent);
                eventAggregator.GetEvent<LookThruFilterReferenceSetEvent>().Subscribe(HandleLookThruReferenceSetEvent);
            }
            if (EffectiveDate != null && portfolioSelectionData != null && holdingDataFilter == null)
            {
                dbInteractivity.RetrieveHoldingsPercentageDataForRegion(portfolioSelectionData, Convert.ToDateTime(EffectiveDate), "Show Everything", " ",lookThruEnabled, RetrieveHoldingsPercentageDataForRegionCallbackMethod);
            }           
        }
        #endregion

        #region Properties
        #region UI Fields    

        /// <summary>
        /// Collection that contains the holdings data to be binded to the region chart
        /// </summary>
        private ObservableCollection<HoldingsPercentageData> holdingsPercentageInfoForRegion;
        public ObservableCollection<HoldingsPercentageData> HoldingsPercentageInfoForRegion
        {
            get { return holdingsPercentageInfoForRegion; }
            set
            {
                holdingsPercentageInfoForRegion = value;
                RaisePropertyChanged(() => this.HoldingsPercentageInfoForRegion);
            }
        }
        /// <summary>
        /// Effective date appended by as of
        /// </summary>
        private String effectiveDateString;
        public String EffectiveDateString
        {
            get
            {
                return effectiveDateString;
            }
            set
            {
                effectiveDateString = value;
                RaisePropertyChanged(() => this.EffectiveDateString);
            }
        }
        /// <summary>
        ///Effective Date as selected by the user 
        /// </summary>
        private DateTime? effectiveDate;
        public DateTime? EffectiveDate
        {
            get { return effectiveDate; }
            set
            {
                if (effectiveDate != value)
                {
                    effectiveDate = value;
                    EffectiveDateString = Convert.ToDateTime(EffectiveDate).ToLongDateString();

                    if (portfolioSelectionData != null && EffectiveDate != null && holdingDataFilter != null)
                    {
                        dbInteractivity.RetrieveHoldingsPercentageDataForRegion(portfolioSelectionData, Convert.ToDateTime(EffectiveDate), holdingDataFilter.Filtertype, holdingDataFilter.FilterValues,lookThruEnabled, RetrieveHoldingsPercentageDataForRegionCallbackMethod);
                    }
                    RaisePropertyChanged(() => this.EffectiveDate);
                }
            }
        }

        /// <summary>
        /// Property that stores the benchmark name
        /// </summary>
        private String benchmarkName;
        public String BenchmarkName
        {
            get { return benchmarkName; }
            set 
            {
                benchmarkName = value;
                RaisePropertyChanged(() => this.BenchmarkName);
            }
        }


        #endregion

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
                if (EffectiveDate != null && portfolioSelectionData != null && holdingDataFilter != null && isActive)
                {
                    BeginWebServiceCall(portfolioSelectionData, Convert.ToDateTime(EffectiveDate), holdingDataFilter.Filtertype, holdingDataFilter.FilterValues, lookThruEnabled);
                }
                if (EffectiveDate != null && portfolioSelectionData != null && holdingDataFilter == null && isActive)
                {
                    BeginWebServiceCall(portfolioSelectionData, Convert.ToDateTime(EffectiveDate), "Show Everything", " ", lookThruEnabled);
                }
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Event for the notification of Data Load Completion
        /// </summary>
        public event DataRetrievalProgressIndicatorEventHandler holdingsPieChartForRegionDataLoadedEvent;
        #endregion

        #region Event Handlers
        /// <summary>
        /// Assigns UI Field Properties based on Selected Effective Date
        /// </summary>
        /// <param name="effectiveDate">Effectice date as selected by the user</param>
        public void HandleEffectiveDateReferenceSetEvent(DateTime effectiveDate)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (effectiveDate != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, effectiveDate, 1);
                    EffectiveDate = effectiveDate;
                    if (EffectiveDate != null && portfolioSelectionData != null && holdingDataFilter != null && IsActive)
                    {                        
                        BeginWebServiceCall(portfolioSelectionData, Convert.ToDateTime(EffectiveDate), holdingDataFilter.Filtertype, holdingDataFilter.FilterValues, lookThruEnabled);
                    }
                    if (EffectiveDate != null && portfolioSelectionData != null && holdingDataFilter == null && IsActive)
                    {                        
                        BeginWebServiceCall(portfolioSelectionData, Convert.ToDateTime(EffectiveDate), "Show Everything", " ", lookThruEnabled);
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
        /// Assigns UI Field Properties based on Fund reference
        /// </summary>
        /// <param name="PortfolioSelectionData">Object of PortfolioSelectionData Class</param>
        public void HandleFundReferenceSetEvent(PortfolioSelectionData portSelectionData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (portSelectionData != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, portSelectionData, 1);
                    portfolioSelectionData = portSelectionData;
                    if (EffectiveDate != null && portfolioSelectionData != null && holdingDataFilter != null && IsActive)
                    {                        
                        BeginWebServiceCall(portfolioSelectionData, Convert.ToDateTime(EffectiveDate), holdingDataFilter.Filtertype, holdingDataFilter.FilterValues, lookThruEnabled);
                    }
                    if (EffectiveDate != null && portfolioSelectionData != null && holdingDataFilter == null && IsActive)
                    {
                        if (null != holdingsPieChartForRegionDataLoadedEvent)
                        {
                            holdingsPieChartForRegionDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
                        }
                        BeginWebServiceCall(portfolioSelectionData, Convert.ToDateTime(EffectiveDate), "Show Everything", " ", lookThruEnabled);
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
        /// Assigns UI Field Properties based on Holding Filter reference
        /// </summary>
        /// <param name="filterSelectionData">Key value pais consisting of the Filter Type and Filter Value selected by the user </param>
        public void HandleFilterReferenceSetEvent(FilterSelectionData filterSelectionData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (filterSelectionData != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, filterSelectionData, 1);
                    holdingDataFilter = filterSelectionData;
                    if (EffectiveDate != null && portfolioSelectionData != null && holdingDataFilter != null && IsActive)
                    {
                        if (null != holdingsPieChartForRegionDataLoadedEvent)
                        {
                            holdingsPieChartForRegionDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
                        }
                        BeginWebServiceCall(portfolioSelectionData, Convert.ToDateTime(EffectiveDate), holdingDataFilter.Filtertype, holdingDataFilter.FilterValues, lookThruEnabled);
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
        /// Event Handler for LookThru Status
        /// </summary>
        /// <param name="enableLookThru">True: LookThru Enabled/False: LookThru Disabled</param>
        public void HandleLookThruReferenceSetEvent(bool enableLookThru)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {           
                    Logging.LogMethodParameter(logger, methodNamespace, enableLookThru, 1);
                    lookThruEnabled = enableLookThru;
                    if (EffectiveDate != null && portfolioSelectionData != null && holdingDataFilter != null && IsActive)
                    {                        
                        BeginWebServiceCall(portfolioSelectionData, Convert.ToDateTime(EffectiveDate), holdingDataFilter.Filtertype, holdingDataFilter.FilterValues, lookThruEnabled);
                    }
                    if (EffectiveDate != null && portfolioSelectionData != null && holdingDataFilter == null && IsActive)
                    {                        
                        BeginWebServiceCall(portfolioSelectionData, Convert.ToDateTime(EffectiveDate), "Show Everything", " ", lookThruEnabled);
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
        /// Calling Web services through dbInteractivity
        /// </summary>
        /// <param name="portSelectionData"></param>
        /// <param name="effectiveDate"></param>
        /// <param name="filterType"></param>
        /// <param name="filterValue"></param>
        /// <param name="enableLookThru"></param>
        private void BeginWebServiceCall(PortfolioSelectionData portSelectionData, DateTime effectiveDate, String filterType, String filterValue, bool enableLookThru)
        {
            if (null != holdingsPieChartForRegionDataLoadedEvent)
            {
                holdingsPieChartForRegionDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
            }
            dbInteractivity.RetrieveHoldingsPercentageDataForRegion(portSelectionData, effectiveDate, filterType, filterValue, enableLookThru, RetrieveHoldingsPercentageDataForRegionCallbackMethod);
        }
    
        #endregion

        #region Callback Methods       
        /// <summary>
        /// Callback method that assigns value to the HoldingsPercentageInfoForRegion property
        /// </summary>
        /// <param name="result">contains the holdings data for the region  pie chart</param>
        public void RetrieveHoldingsPercentageDataForRegionCallbackMethod(List<HoldingsPercentageData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null && result.Count > 0)
                {
                    HoldingsPercentageInfoForRegion = new ObservableCollection<HoldingsPercentageData>(result);
                    BenchmarkName = result[0].BenchmarkName;
                    if (null != holdingsPieChartForRegionDataLoadedEvent)
                    {
                        holdingsPieChartForRegionDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
                    }
                }
                else
                {
                    HoldingsPercentageInfoForRegion = new ObservableCollection<HoldingsPercentageData>();
                    BenchmarkName = "";
                    Logging.LogMethodParameterNull(logger, methodNamespace, 1);
                    holdingsPieChartForRegionDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
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

        #region EventUnSubscribe
        /// <summary>
        /// Method that disposes the events
        /// </summary>
        public void Dispose()
        {
            eventAggregator.GetEvent<PortfolioReferenceSetEvent>().Unsubscribe(HandleFundReferenceSetEvent);
            eventAggregator.GetEvent<EffectiveDateReferenceSetEvent>().Unsubscribe(HandleEffectiveDateReferenceSetEvent);
            eventAggregator.GetEvent<HoldingFilterReferenceSetEvent>().Unsubscribe(HandleFilterReferenceSetEvent);
            eventAggregator.GetEvent<LookThruFilterReferenceSetEvent>().Unsubscribe(HandleLookThruReferenceSetEvent);
        }
        #endregion
    }
}
