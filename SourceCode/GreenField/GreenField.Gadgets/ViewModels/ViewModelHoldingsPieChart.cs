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
using GreenField.ServiceCaller;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Events;
using System.Collections.Generic;
using GreenField.ServiceCaller.SecurityReferenceDefinitions;
using Microsoft.Practices.Prism.ViewModel;
using GreenField.Common;
using System.Collections.ObjectModel;
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
        public ViewModelHoldingsPieChart(DashboardGadgetParam param)
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
                _dbInteractivity.RetrieveHoldingsPercentageData(_PortfolioSelectionData, Convert.ToDateTime(EffectiveDate), _holdingDataFilter.Filtertype, _holdingDataFilter.FilterValues,_lookThruEnabled, RetrieveHoldingsPercentageDataCallbackMethod);
            }

            if (EffectiveDate != null && _PortfolioSelectionData != null && _holdingDataFilter == null && IsActive)
            {
                _dbInteractivity.RetrieveHoldingsPercentageData(_PortfolioSelectionData, Convert.ToDateTime(EffectiveDate),"Show Everything"," ",_lookThruEnabled, RetrieveHoldingsPercentageDataCallbackMethod);
            }

            if (_eventAggregator != null)
            {
                _eventAggregator.GetEvent<PortfolioReferenceSetEvent>().Subscribe(HandleFundReferenceSetEvent);
                _eventAggregator.GetEvent<EffectiveDateReferenceSetEvent>().Subscribe(HandleEffectiveDateReferenceSetEvent);
                _eventAggregator.GetEvent<HoldingFilterReferenceSetEvent>().Subscribe(HandleFilterReferenceSetEvent);
                _eventAggregator.GetEvent<LookThruFilterReferenceSetEvent>().Subscribe(HandleLookThruReferenceSetEvent);
            }           

        }
        #endregion

       #region Properties
        #region UI Fields

        /// <summary>
        /// Collection that contains the holdings data to be binded to the sector chart
        /// </summary>
        private ObservableCollection<HoldingsPercentageData> _holdingsPercentageInfo;
        public ObservableCollection<HoldingsPercentageData> HoldingsPercentageInfo
        {
            get { return _holdingsPercentageInfo; }
            set
            {
                _holdingsPercentageInfo = value;
                RaisePropertyChanged(()=> this.HoldingsPercentageInfo);
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
                    if (_PortfolioSelectionData != null && EffectiveDate != null && _holdingDataFilter !=null)
                    {
                        _dbInteractivity.RetrieveHoldingsPercentageData(_PortfolioSelectionData, Convert.ToDateTime(EffectiveDate), _holdingDataFilter.Filtertype, _holdingDataFilter.FilterValues,_lookThruEnabled, RetrieveHoldingsPercentageDataCallbackMethod);

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
                        BeginWebServiceCall(PortfolioSelectionData, Convert.ToDateTime(EffectiveDate), _holdingDataFilter.Filtertype, _holdingDataFilter.FilterValues, _lookThruEnabled);
                    }

                    if (EffectiveDate != null && _PortfolioSelectionData != null && _holdingDataFilter == null && IsActive)
                    {                        
                        BeginWebServiceCall(_PortfolioSelectionData, Convert.ToDateTime(EffectiveDate), "Show Everything", " ",_lookThruEnabled);
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
                if (filterSelectionData !=null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, filterSelectionData, 1);
                    _holdingDataFilter = filterSelectionData;
                    if (EffectiveDate != null && _PortfolioSelectionData != null && _holdingDataFilter.Filtertype != null && _holdingDataFilter.FilterValues != null && IsActive)
                    {                        
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

        private void BeginWebServiceCall(PortfolioSelectionData PortfolioSelectionData, DateTime effectiveDate, String filterType, String fileterValue
            , bool enableLookThru)
        {
            if (null != holdingsPieChartDataLoadedEvent)
                holdingsPieChartDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
            _dbInteractivity.RetrieveHoldingsPercentageData(PortfolioSelectionData, effectiveDate, filterType, fileterValue, enableLookThru, RetrieveHoldingsPercentageDataCallbackMethod);
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
            Logging.LogBeginMethod(_logger, methodNamespace);
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
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                    holdingsPieChartDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
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
