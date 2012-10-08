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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.ViewModel;
using GreenField.ServiceCaller;
using GreenField.ServiceCaller.SecurityReferenceDefinitions;
using GreenField.Common;
using GreenField.ServiceCaller.BenchmarkHoldingsDefinitions;
using GreenField.DataContracts;

namespace GreenField.Gadgets.ViewModels
{
    /// <summary>
    /// Class that provides the interaction of the view model with the Service caller and the View.
    /// </summary>
    public class ViewModelHoldingsPieChart : NotificationObject
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
        public ViewModelHoldingsPieChart(DashboardGadgetParam param)
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
                dbInteractivity.RetrieveHoldingsPercentageData(portfolioSelectionData, Convert.ToDateTime(EffectiveDate), holdingDataFilter.Filtertype, holdingDataFilter.FilterValues, lookThruEnabled, RetrieveHoldingsPercentageDataCallbackMethod);
            }
            if (EffectiveDate != null && portfolioSelectionData != null && holdingDataFilter == null && IsActive)
            {
                dbInteractivity.RetrieveHoldingsPercentageData(portfolioSelectionData, Convert.ToDateTime(EffectiveDate), "Show Everything", " ", lookThruEnabled, RetrieveHoldingsPercentageDataCallbackMethod);
            }
            if (eventAggregator != null)
            {
                eventAggregator.GetEvent<PortfolioReferenceSetEvent>().Subscribe(HandleFundReferenceSetEvent);
                eventAggregator.GetEvent<EffectiveDateReferenceSetEvent>().Subscribe(HandleEffectiveDateReferenceSetEvent);
                eventAggregator.GetEvent<HoldingFilterReferenceSetEvent>().Subscribe(HandleFilterReferenceSetEvent);
                eventAggregator.GetEvent<LookThruFilterReferenceSetEvent>().Subscribe(HandleLookThruReferenceSetEvent);
            }
        }
        #endregion

       #region Properties
        #region UI Fields

        /// <summary>
        /// Collection that contains the holdings data to be binded to the sector chart
        /// </summary>
        private ObservableCollection<HoldingsPercentageData> holdingsPercentageInfo;
        public ObservableCollection<HoldingsPercentageData> HoldingsPercentageInfo
        {
            get { return holdingsPercentageInfo; }
            set
            {
                holdingsPercentageInfo = value;
                RaisePropertyChanged(()=> this.HoldingsPercentageInfo);
            }
        }
        /// <summary>
        /// Effective date appended 
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
        private DateTime? _effectiveDate;
        public DateTime? EffectiveDate
        {
            get { return _effectiveDate; }
            set
            {
                if (_effectiveDate != value)
                {
                    _effectiveDate = value;
                    EffectiveDateString = Convert.ToDateTime(EffectiveDate).ToLongDateString();
                    if (portfolioSelectionData != null && EffectiveDate != null && holdingDataFilter != null)
                    {
                        dbInteractivity.RetrieveHoldingsPercentageData(portfolioSelectionData, Convert.ToDateTime(EffectiveDate), holdingDataFilter.Filtertype, holdingDataFilter.FilterValues, lookThruEnabled, RetrieveHoldingsPercentageDataCallbackMethod);

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
        public event DataRetrievalProgressIndicatorEventHandler holdingsPieChartDataLoadedEvent;
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
                        BeginWebServiceCall(portSelectionData, Convert.ToDateTime(EffectiveDate), holdingDataFilter.Filtertype, holdingDataFilter.FilterValues, lookThruEnabled);
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
        /// Assigns UI Field Properties based on Holding Filter reference
        /// </summary>
        /// <param name="filterSelectionData">Key value pais consisting of the Filter Type and Filter Value selected by the user </param>
        public void HandleFilterReferenceSetEvent(FilterSelectionData filterSelectionData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (filterSelectionData !=null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, filterSelectionData, 1);
                    holdingDataFilter = filterSelectionData;
                    if (EffectiveDate != null && portfolioSelectionData != null && holdingDataFilter.Filtertype != null && holdingDataFilter.FilterValues != null && IsActive)
                    {
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
        /// Calls the web service method through dbinteractivity
        /// </summary>
        /// <param name="PortfolioSelectionData">Portfolio selected by the user</param>
        /// <param name="effectiveDate">Effective Date selected by the user</param>
        /// <param name="filterType">Filter Type selected by the user</param>
        /// <param name="fileterValue">Filter Value selected by the user</param>
        /// <param name="enableLookThru">Look Through check box value</param>
        private void BeginWebServiceCall(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate, String filterType, String fileterValue
            , bool enableLookThru)
        {
            if (null != holdingsPieChartDataLoadedEvent)
                holdingsPieChartDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
            dbInteractivity.RetrieveHoldingsPercentageData(portfolioSelectionData, effectiveDate, filterType, fileterValue, enableLookThru, RetrieveHoldingsPercentageDataCallbackMethod);
        }
        #endregion

       #region Callback Methods
        /// <summary>
        /// Callback method that assigns value to the HoldingsPercentageInfo property
        /// </summary>
        /// <param name="result">contains the holdings data for the sector pie chart</param>
        public void RetrieveHoldingsPercentageDataCallbackMethod(List<HoldingsPercentageData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try 
            {
                if (result != null && result.Count > 0)
                {
                    HoldingsPercentageInfo = new ObservableCollection<HoldingsPercentageData>(result);
                    BenchmarkName = result[0].BenchmarkName;
                    if (null != holdingsPieChartDataLoadedEvent)
                        holdingsPieChartDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
                }
                else
                {
                    HoldingsPercentageInfo = new ObservableCollection<HoldingsPercentageData>();
                    BenchmarkName = "";
                    Logging.LogMethodParameterNull(logger, methodNamespace, 1);
                    holdingsPieChartDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
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
