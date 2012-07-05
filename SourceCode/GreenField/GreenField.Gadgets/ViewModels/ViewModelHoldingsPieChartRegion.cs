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
using GreenField.ServiceCaller.BenchmarkHoldingsDefinitions;
using GreenField.Common;
using Microsoft.Practices.Prism.ViewModel;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using GreenField.DataContracts;

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
        /// Private member containing the Key Value Pair
        /// </summary>
        private FilterSelectionData _holdingDataFilter;

        /// <summary>
        /// Private member to store info about look thru enabled or not
        /// </summary>
        private bool _lookThruEnabled;

        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="param">MEF Eventaggrigator instance</param>
        public ViewModelHoldingsPieChartRegion(DashboardGadgetParam param)
        {

            _dbInteractivity = param.DBInteractivity;
            _logger = param.LoggerFacade;
            _eventAggregator = param.EventAggregator;
            _PortfolioSelectionData = param.DashboardGadgetPayload.PortfolioSelectionData;
            EffectiveDate = param.DashboardGadgetPayload.EffectiveDate;
            _holdingDataFilter = param.DashboardGadgetPayload.FilterSelectionData;
            _lookThruEnabled = param.DashboardGadgetPayload.IsLookThruEnabled;

            if (EffectiveDate != null && _PortfolioSelectionData != null && _holdingDataFilter != null && IsActive)
            {
                _dbInteractivity.RetrieveHoldingsPercentageDataForRegion(_PortfolioSelectionData, Convert.ToDateTime(EffectiveDate), _holdingDataFilter.Filtertype, _holdingDataFilter.FilterValues,_lookThruEnabled, RetrieveHoldingsPercentageDataForRegionCallbackMethod);
            }

            if (_eventAggregator != null)
            {
                _eventAggregator.GetEvent<PortfolioReferenceSetEvent>().Subscribe(HandleFundReferenceSetEvent);
                _eventAggregator.GetEvent<EffectiveDateReferenceSetEvent>().Subscribe(HandleEffectiveDateReferenceSetEvent);
                _eventAggregator.GetEvent<HoldingFilterReferenceSetEvent>().Subscribe(HandleFilterReferenceSetEvent);
                _eventAggregator.GetEvent<LookThruFilterReferenceSetEvent>().Subscribe(HandleLookThruReferenceSetEvent);
            }

            if (EffectiveDate != null && _PortfolioSelectionData != null && _holdingDataFilter == null)
            {
                _dbInteractivity.RetrieveHoldingsPercentageDataForRegion(_PortfolioSelectionData, Convert.ToDateTime(EffectiveDate), "Show Everything", " ",_lookThruEnabled, RetrieveHoldingsPercentageDataForRegionCallbackMethod);
            }

           
        }
        #endregion

        #region Properties
        #region UI Fields

      

        /// <summary>
        /// Collection that contains the holdings data to be binded to the region chart
        /// </summary>
        private ObservableCollection<HoldingsPercentageData> _holdingsPercentageInfoForRegion;
        public ObservableCollection<HoldingsPercentageData> HoldingsPercentageInfoForRegion
        {
            get { return _holdingsPercentageInfoForRegion; }
            set
            {
                _holdingsPercentageInfoForRegion = value;
                RaisePropertyChanged(() => this.HoldingsPercentageInfoForRegion);
            }
        }
        /// <summary>
        /// Effective date appended by as of
        /// </summary>
        private String _effectiveDateString;
        public String EffectiveDateString
        {
            get
            {
                return _effectiveDateString;
            }

            set
            {
                _effectiveDateString = value;
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

                    if (_PortfolioSelectionData != null && EffectiveDate != null && _holdingDataFilter != null)
                    {
                        _dbInteractivity.RetrieveHoldingsPercentageDataForRegion(_PortfolioSelectionData, Convert.ToDateTime(EffectiveDate), _holdingDataFilter.Filtertype, _holdingDataFilter.FilterValues,_lookThruEnabled, RetrieveHoldingsPercentageDataForRegionCallbackMethod);
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
                if (EffectiveDate != null && _PortfolioSelectionData != null && _holdingDataFilter != null && _isActive)
                {
                    BeginWebServiceCall(_PortfolioSelectionData, Convert.ToDateTime(EffectiveDate), _holdingDataFilter.Filtertype, _holdingDataFilter.FilterValues, _lookThruEnabled);
                }

                if (EffectiveDate != null && _PortfolioSelectionData != null && _holdingDataFilter == null && _isActive)
                {
                    BeginWebServiceCall(_PortfolioSelectionData, Convert.ToDateTime(EffectiveDate), "Show Everything", " ", _lookThruEnabled);
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
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (effectiveDate != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, effectiveDate, 1);
                    EffectiveDate = effectiveDate;
                    if (EffectiveDate != null && _PortfolioSelectionData != null && _holdingDataFilter != null && IsActive)
                    {                        
                        BeginWebServiceCall(_PortfolioSelectionData, Convert.ToDateTime(EffectiveDate), _holdingDataFilter.Filtertype, _holdingDataFilter.FilterValues, _lookThruEnabled);
                    }

                    if (EffectiveDate != null && _PortfolioSelectionData != null && _holdingDataFilter == null && IsActive)
                    {                        
                        BeginWebServiceCall(_PortfolioSelectionData, Convert.ToDateTime(EffectiveDate), "Show Everything", " ", _lookThruEnabled);
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
        /// Assigns UI Field Properties based on Fund reference
        /// </summary>
        /// <param name="PortfolioSelectionData">Object of PortfolioSelectionData Class</param>
        public void HandleFundReferenceSetEvent(PortfolioSelectionData PortfolioSelectionData)
        {

            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (PortfolioSelectionData != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, PortfolioSelectionData, 1);
                    _PortfolioSelectionData = PortfolioSelectionData;
                    if (EffectiveDate != null && _PortfolioSelectionData != null && _holdingDataFilter != null && IsActive)
                    {                        
                        BeginWebServiceCall(_PortfolioSelectionData, Convert.ToDateTime(EffectiveDate), _holdingDataFilter.Filtertype, _holdingDataFilter.FilterValues, _lookThruEnabled);
                    }

                    if (EffectiveDate != null && _PortfolioSelectionData != null && _holdingDataFilter == null && IsActive)
                    {
                        if (null != holdingsPieChartForRegionDataLoadedEvent)
                            holdingsPieChartForRegionDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
                        BeginWebServiceCall(_PortfolioSelectionData, Convert.ToDateTime(EffectiveDate), "Show Everything", " ", _lookThruEnabled);
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
        /// Assigns UI Field Properties based on Holding Filter reference
        /// </summary>
        /// <param name="filterSelectionData">Key value pais consisting of the Filter Type and Filter Value selected by the user </param>
        public void HandleFilterReferenceSetEvent(FilterSelectionData filterSelectionData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (filterSelectionData != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, filterSelectionData, 1);
                    _holdingDataFilter = filterSelectionData;
                    if (EffectiveDate != null && _PortfolioSelectionData != null && _holdingDataFilter != null && IsActive)
                    {
                        if (null != holdingsPieChartForRegionDataLoadedEvent)
                            holdingsPieChartForRegionDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
                        BeginWebServiceCall(_PortfolioSelectionData, Convert.ToDateTime(EffectiveDate), _holdingDataFilter.Filtertype, _holdingDataFilter.FilterValues, _lookThruEnabled);
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
        /// Event Handler for LookThru Status
        /// </summary>
        /// <param name="enableLookThru">True: LookThru Enabled/False: LookThru Disabled</param>
        public void HandleLookThruReferenceSetEvent(bool enableLookThru)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
               
                    Logging.LogMethodParameter(_logger, methodNamespace, enableLookThru, 1);
                    _lookThruEnabled = enableLookThru;

                    if (EffectiveDate != null && _PortfolioSelectionData != null && _holdingDataFilter != null && IsActive)
                    {                        
                        BeginWebServiceCall(_PortfolioSelectionData, Convert.ToDateTime(EffectiveDate), _holdingDataFilter.Filtertype, _holdingDataFilter.FilterValues, _lookThruEnabled);
                    }

                    if (EffectiveDate != null && _PortfolioSelectionData != null && _holdingDataFilter == null && IsActive)
                    {                        
                        BeginWebServiceCall(_PortfolioSelectionData, Convert.ToDateTime(EffectiveDate), "Show Everything", " ", _lookThruEnabled);
                    }
                
               
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        private void BeginWebServiceCall(PortfolioSelectionData PortfolioSelectionData, DateTime effectiveDate, String filterType, String filterValue
            , bool enableLookThru)
        {
            if (null != holdingsPieChartForRegionDataLoadedEvent)
                holdingsPieChartForRegionDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
            _dbInteractivity.RetrieveHoldingsPercentageDataForRegion(PortfolioSelectionData, effectiveDate, filterType, filterValue, enableLookThru, RetrieveHoldingsPercentageDataForRegionCallbackMethod);
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
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null && result.Count > 0)
                {
                    HoldingsPercentageInfoForRegion = new ObservableCollection<HoldingsPercentageData>(result);
                    BenchmarkName = result[0].BenchmarkName;
                    if (null != holdingsPieChartForRegionDataLoadedEvent)
                        holdingsPieChartForRegionDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
                }
                else
                {
                    HoldingsPercentageInfoForRegion = new ObservableCollection<HoldingsPercentageData>(result);
                    BenchmarkName = "";
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                    holdingsPieChartForRegionDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
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
            _eventAggregator.GetEvent<PortfolioReferenceSetEvent>().Unsubscribe(HandleFundReferenceSetEvent);
            _eventAggregator.GetEvent<EffectiveDateReferenceSetEvent>().Unsubscribe(HandleEffectiveDateReferenceSetEvent);
            _eventAggregator.GetEvent<HoldingFilterReferenceSetEvent>().Unsubscribe(HandleFilterReferenceSetEvent);
            _eventAggregator.GetEvent<LookThruFilterReferenceSetEvent>().Unsubscribe(HandleLookThruReferenceSetEvent);
        }

        #endregion


    }
}
