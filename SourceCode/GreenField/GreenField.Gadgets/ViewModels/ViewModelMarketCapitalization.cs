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
using GreenField.ServiceCaller.SecurityReferenceDefinitions;
using Microsoft.Practices.Prism.Logging;
using GreenField.ServiceCaller;
using Microsoft.Practices.Prism.Events;
using GreenField.Common;
using Microsoft.Practices.Prism.ViewModel;
using GreenField.Common.Helper;
using GreenField.ServiceCaller.BenchmarkHoldingsDefinitions;
using System.Collections.Generic;
using GreenField.DataContracts;
using System.Linq;

namespace GreenField.Gadgets.ViewModels
{
    /// <summary>
    /// Class that provides the interaction of the view model with the Service caller and the View.
    /// </summary>
    public class ViewModelMarketCapitalization : NotificationObject
    {
        #region Fields
        //MEF Singletons

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
        private PortfolioSelectionData _portfolioSelectionData;

        /// <summary>
        /// Private member containing the Key Value Pair
        /// </summary>
        private FilterSelectionData _mktCapDataFilter;

        /// <summary>
        /// Private member to store mkt cap data
        /// </summary>
        private MarketCapitalizationData _marketCapitalizationInfo;

        /// <summary>
        /// Private member to store effective date
        /// </summary>
        private DateTime? _effectiveDate;

        /// <summary>
        /// Private member to store info about including or excluding cash securities
        /// </summary>
        private bool _isExCashSecurity = false;

        /// <summary>
        /// Private member to enable or disble lookthru 
        /// </summary>

        private bool _lookThruEnabled = false;

        /// <summary>
        /// Private member to store market cap gadget visibilty
        /// </summary>
        private Visibility _marketCapGadgetVisibility = Visibility.Collapsed;


        #endregion

        #region Constructor
        public ViewModelMarketCapitalization(DashboardGadgetParam param)
        {
            _eventAggregator = param.EventAggregator;
            _dbInteractivity = param.DBInteractivity;
            _logger = param.LoggerFacade;

            _portfolioSelectionData = param.DashboardGadgetPayload.PortfolioSelectionData;
            //_benchmarkSelectionData = param.DashboardGadgetPayload.BenchmarkSelectionData;
            _effectiveDate = param.DashboardGadgetPayload.EffectiveDate;
            _mktCapDataFilter = param.DashboardGadgetPayload.FilterSelectionData;
            IsExCashSecurity = param.DashboardGadgetPayload.IsExCashSecurityData;
            _lookThruEnabled = param.DashboardGadgetPayload.IsLookThruEnabled;

            //if (_effectiveDate != null && _PortfolioSelectionData != null && _benchmarkSelectionData != null)
            //{
            //    _dbInteractivity.RetrieveMarketCapitalizationData(_PortfolioSelectionData, _benchmarkSelectionData, _effectiveDate, RetrieveMarketCapitalizationDataCallbackMethod);
            //}
            if (_effectiveDate != null && _portfolioSelectionData != null && IsActive)// && _mktCapDataFilter != null)
            {
                CallingWebMethod();
                //_dbInteractivity.RetrieveMarketCapitalizationData(PortfolioSelectionData, Convert.ToDateTime(EffectiveDate), _mktCapDataFilter.Filtertype, _mktCapDataFilter.FilterValues, IsExCashSecurity, _lookThruEnabled, RetrieveMarketCapitalizationDataCallbackMethod);
            }
            if (_eventAggregator != null)
            {
                _eventAggregator.GetEvent<PortfolioReferenceSetEvent>().Subscribe(HandleFundReferenceSet);
                //_eventAggregator.GetEvent<BenchmarkReferenceSetEvent>().Subscribe(HandleBenchmarkReferenceSet);
                _eventAggregator.GetEvent<EffectiveDateReferenceSetEvent>().Subscribe(HandleEffectiveDateSet);
                _eventAggregator.GetEvent<HoldingFilterReferenceSetEvent>().Subscribe(HandleFilterReferenceSetEvent);
                _eventAggregator.GetEvent<ExCashSecuritySetEvent>().Subscribe(HandleExCashSecuritySetEvent);
                _eventAggregator.GetEvent<LookThruFilterReferenceSetEvent>().Subscribe(HandleLookThruReferenceSetEvent);
            }
        }
        #endregion

        #region Properties
        #region UI Fields

        /// <summary>
        /// Stores data for Mkt Cap grid
        /// </summary>
        public MarketCapitalizationData MarketCapitalizationInfo
        {
            get { return _marketCapitalizationInfo; }
            set
            {
                if (_marketCapitalizationInfo != value)
                {
                    _marketCapitalizationInfo = value;
                    RaisePropertyChanged(() => this.MarketCapitalizationInfo);
                }
            }
        }

       /// <summary>
       /// Stores Market cap gadget visibility 
       /// </summary>
            
        public Visibility MarketCapGadgetVisibility
        {
            get
            {
                return _marketCapGadgetVisibility; 
            }
            set
            {
                _marketCapGadgetVisibility = value;
                RaisePropertyChanged(() => this.MarketCapGadgetVisibility);                
            }
        }
        
        /// <summary>
        /// Portfolio selected by user
        /// </summary>       
        public PortfolioSelectionData PortfolioSelectionData
        {
            get { return _portfolioSelectionData; }
            set
            {
                if (_portfolioSelectionData != value)
                {
                    _portfolioSelectionData = value;
                    RaisePropertyChanged(() => PortfolioSelectionData);
                }
            }
        }

        /// <summary>
        ///Effective Date as selected by the user 
        /// </summary>       
        public DateTime? EffectiveDate
        {
            get
            {
                return _effectiveDate;
            }
            set
            {
                if (_effectiveDate != value)
                {

                    _effectiveDate = value;

                    //if (_portfolioSelectionData != null && EffectiveDate != null )
                    //{
                    //    _dbInteractivity.RetrieveMarketCapitalizationData(PortfolioSelectionData, Convert.ToDateTime(EffectiveDate), _mktCapDataFilter.Filtertype, _mktCapDataFilter.FilterValues, IsExCashSecurity, RetrieveMarketCapitalizationDataCallbackMethod);

                    //}

                    RaisePropertyChanged(() => this.EffectiveDate);
                }
            }
        }

        /// <summary>
        /// Stores securities including or excluding cash type
        /// </summary>        
        public bool IsExCashSecurity
        {
            get { return _isExCashSecurity; }
            set
            {
                if (_isExCashSecurity != value)
                {
                    _isExCashSecurity = value;
                    //if (_effectiveDate != null && _portfolioSelectionData != null)// && _mktCapDataFilter != null)
                    //{
                    //    _dbInteractivity.RetrieveMarketCapitalizationData(PortfolioSelectionData, Convert.ToDateTime(EffectiveDate), _mktCapDataFilter.Filtertype, _mktCapDataFilter.FilterValues, IsExCashSecurity, RetrieveMarketCapitalizationDataCallbackMethod);

                    //}

                    RaisePropertyChanged(() => this.EffectiveDate);
                }
            }
        }
        /// <summary>
        /// Check to include LookThru or Not
        /// </summary>
        private bool _enableLookThru;
        public bool EnableLookThru
        {
            get { return _enableLookThru; }
            set
            {
                _enableLookThru = value;
                this.RaisePropertyChanged(() => this.EnableLookThru);
            }
        }
        /// <summary>
        /// IsActive is true when parent control is displayed on UI
        /// </summary>
        private bool _isActive;
        public bool IsActive 
        { 
            get{ return _isActive; }
            set {
                _isActive = value;               
                if (_effectiveDate != null && _portfolioSelectionData != null && _isActive)// && _mktCapDataFilter != null)
                {
                    CallingWebMethod();
                    //if (MarketCapitalizationDataLoadEvent != null)
                    //    MarketCapitalizationDataLoadEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
                    //if (_mktCapDataFilter != null)
                    //    _dbInteractivity.RetrieveMarketCapitalizationData(PortfolioSelectionData, Convert.ToDateTime(EffectiveDate), _mktCapDataFilter.Filtertype, _mktCapDataFilter.FilterValues, IsExCashSecurity, EnableLookThru, RetrieveMarketCapitalizationDataCallbackMethod);
                    //else
                    //    _dbInteractivity.RetrieveMarketCapitalizationData(PortfolioSelectionData, Convert.ToDateTime(EffectiveDate), null, null, IsExCashSecurity, EnableLookThru, RetrieveMarketCapitalizationDataCallbackMethod);
                    
                }
            }
        }

        #endregion
        #endregion

        #region Event
        /// <summary>
        /// event to handle data retrieval progress indicator
        /// </summary>
        public event DataRetrievalProgressIndicatorEventHandler MarketCapitalizationDataLoadEvent;

        #endregion

        #region Event Handlers

        /// <summary>
        /// Assigns UI Field Properties based on Fund reference
        /// </summary>
        /// <param name="PortfolioSelectionData">Object of PortfolioSelectionData Class</param>
        public void HandleFundReferenceSet(PortfolioSelectionData PortfolioSelectionData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);

            try
            {
                if (PortfolioSelectionData != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, PortfolioSelectionData, 1);
                    _portfolioSelectionData = PortfolioSelectionData;
                    if (_effectiveDate != null && _portfolioSelectionData != null && IsActive)// && _mktCapDataFilter != null)
                    {
                        CallingWebMethod();
                        //if (MarketCapitalizationDataLoadEvent != null)
                        //    MarketCapitalizationDataLoadEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
                        //if (_mktCapDataFilter != null)
                        //    _dbInteractivity.RetrieveMarketCapitalizationData(PortfolioSelectionData, Convert.ToDateTime(EffectiveDate), _mktCapDataFilter.Filtertype, _mktCapDataFilter.FilterValues, IsExCashSecurity, EnableLookThru, RetrieveMarketCapitalizationDataCallbackMethod);
                        //else
                        //    _dbInteractivity.RetrieveMarketCapitalizationData(PortfolioSelectionData, Convert.ToDateTime(EffectiveDate), null, null, IsExCashSecurity, EnableLookThru, RetrieveMarketCapitalizationDataCallbackMethod);
                        
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
        /// Assigns UI Field Properties based on Selected Effective Date
        /// </summary>
        /// <param name="effectiveDate">Effectice date as selected by the user</param>
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
                    if (_effectiveDate != null && _portfolioSelectionData != null && IsActive)// && _mktCapDataFilter != null)
                    {
                        CallingWebMethod();
                        //if (MarketCapitalizationDataLoadEvent != null)
                        //    MarketCapitalizationDataLoadEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
                        //if (_mktCapDataFilter != null)
                        //    _dbInteractivity.RetrieveMarketCapitalizationData(PortfolioSelectionData, Convert.ToDateTime(EffectiveDate), _mktCapDataFilter.Filtertype, _mktCapDataFilter.FilterValues, IsExCashSecurity, EnableLookThru, RetrieveMarketCapitalizationDataCallbackMethod);
                        //else
                        //    _dbInteractivity.RetrieveMarketCapitalizationData(PortfolioSelectionData, Convert.ToDateTime(EffectiveDate), null, null, IsExCashSecurity, EnableLookThru, RetrieveMarketCapitalizationDataCallbackMethod);
                        
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
        /// <param name="filterSelectionData">Key value pairs consisting of the Filter Type and Filter Value selected by the user </param>
        public void HandleFilterReferenceSetEvent(FilterSelectionData filterSelectionData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (filterSelectionData != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, filterSelectionData, 1);
                    _mktCapDataFilter = filterSelectionData;
                    if (MarketCapitalizationDataLoadEvent != null)
                            MarketCapitalizationDataLoadEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
                    if (_effectiveDate != null && _portfolioSelectionData != null && _mktCapDataFilter != null && IsActive)
                    {
                        CallingWebMethod();
                        //_dbInteractivity.RetrieveMarketCapitalizationData(PortfolioSelectionData, Convert.ToDateTime(EffectiveDate), _mktCapDataFilter.Filtertype, _mktCapDataFilter.FilterValues, IsExCashSecurity,  EnableLookThru, RetrieveMarketCapitalizationDataCallbackMethod);
                        
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

        #endregion

        #region Callback Methods

        /// <summary>
        /// Callback method that assigns value to the MarketCapitalizationInfo property
        /// </summary>
        /// <param name="result">contains the market capitalization data </param>
        private void RetrieveMarketCapitalizationDataCallbackMethod(List<MarketCapitalizationData> marketCapitalizationData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                MarketCapitalizationInfo = null;
                MarketCapGadgetVisibility = Visibility.Collapsed;
                if (marketCapitalizationData != null && marketCapitalizationData.Count > 0)
                {
                    MarketCapGadgetVisibility = Visibility.Visible;
                    Logging.LogMethodParameter(_logger, methodNamespace, marketCapitalizationData, 1);
                    MarketCapitalizationInfo = marketCapitalizationData.FirstOrDefault();
                    this.RaisePropertyChanged(() => this.MarketCapitalizationInfo);
                }
                else
                {
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                }
                if (MarketCapitalizationDataLoadEvent != null)
                    MarketCapitalizationDataLoadEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        public void HandleExCashSecuritySetEvent(bool isExCashSec)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {               
                    Logging.LogMethodParameter(_logger, methodNamespace, isExCashSec, 1);
                    IsExCashSecurity = isExCashSec;
                    if (_effectiveDate != null && _portfolioSelectionData != null && IsActive)//&& _mktCapDataFilter != null)
                    {
                        CallingWebMethod();
                        //if (MarketCapitalizationDataLoadEvent != null)
                        //    MarketCapitalizationDataLoadEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
                        //    if(_mktCapDataFilter != null)
                        //        _dbInteractivity.RetrieveMarketCapitalizationData(PortfolioSelectionData, Convert.ToDateTime(EffectiveDate), _mktCapDataFilter.Filtertype, _mktCapDataFilter.FilterValues, IsExCashSecurity, EnableLookThru, RetrieveMarketCapitalizationDataCallbackMethod);
                        //    else
                        //        _dbInteractivity.RetrieveMarketCapitalizationData(PortfolioSelectionData, Convert.ToDateTime(EffectiveDate), null, null, IsExCashSecurity, EnableLookThru, RetrieveMarketCapitalizationDataCallbackMethod);

                            
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
                EnableLookThru = enableLookThru;

                if (_effectiveDate != null && _portfolioSelectionData != null && IsActive)//&& _mktCapDataFilter != null)
                {
                    CallingWebMethod();
                    //if (MarketCapitalizationDataLoadEvent != null)
                    //    MarketCapitalizationDataLoadEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
                    //if (_mktCapDataFilter != null)
                    //    _dbInteractivity.RetrieveMarketCapitalizationData(PortfolioSelectionData, Convert.ToDateTime(EffectiveDate), _mktCapDataFilter.Filtertype, _mktCapDataFilter.FilterValues, IsExCashSecurity, EnableLookThru, RetrieveMarketCapitalizationDataCallbackMethod);
                    //else
                    //    _dbInteractivity.RetrieveMarketCapitalizationData(PortfolioSelectionData, Convert.ToDateTime(EffectiveDate), null, null, IsExCashSecurity, EnableLookThru, RetrieveMarketCapitalizationDataCallbackMethod);

                    
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

        #region SERVICE CALL METOHD
        /// <summary>
        /// Calls web service method
        /// </summary>
        private void CallingWebMethod()
        {
            if (MarketCapitalizationDataLoadEvent != null)
                MarketCapitalizationDataLoadEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
            if (_mktCapDataFilter != null)
                _dbInteractivity.RetrieveMarketCapitalizationData(PortfolioSelectionData, Convert.ToDateTime(EffectiveDate), _mktCapDataFilter.Filtertype, _mktCapDataFilter.FilterValues, IsExCashSecurity, EnableLookThru, RetrieveMarketCapitalizationDataCallbackMethod);
            else
                _dbInteractivity.RetrieveMarketCapitalizationData(PortfolioSelectionData, Convert.ToDateTime(EffectiveDate), null, null, IsExCashSecurity, EnableLookThru, RetrieveMarketCapitalizationDataCallbackMethod);

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
            _eventAggregator.GetEvent<MarketCapitalizationSetEvent>().Unsubscribe(HandleFilterReferenceSetEvent);
            _eventAggregator.GetEvent<ExCashSecuritySetEvent>().Unsubscribe(HandleExCashSecuritySetEvent);
            _eventAggregator.GetEvent<LookThruFilterReferenceSetEvent>().Unsubscribe(HandleLookThruReferenceSetEvent);
        }

        #endregion
    }
}
