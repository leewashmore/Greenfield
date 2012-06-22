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
using System.Collections.ObjectModel;
using GreenField.Common;
using Microsoft.Practices.Prism.ViewModel;
using System.Collections.Generic;
using GreenField.ServiceCaller.BenchmarkHoldingsDefinitions;
using Microsoft.Practices.Prism.Commands;
using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.Regions;
using GreenField.DataContracts;

namespace GreenField.Gadgets.ViewModels
{
    /// <summary>
    /// View model for ViewTopHoldings class
    /// </summary>
    public class ViewModelTopHoldings : NotificationObject
    {
        #region Fields
        /// <summary>
        /// MEF Singletons
        /// </summary>
        private IEventAggregator _eventAggregator;
        private IDBInteractivity _dbInteractivity;
        private ILoggerFacade _logger;
        private IRegionManager _regionManager;

        /// <summary>
        /// DashboardGadgetPayLoad fields
        /// </summary>
        private PortfolioSelectionData _portfolioSelectionData;

        /// <summary>
        /// Private member to store info about including or excluding cash securities
        /// </summary>
        private bool _isExCashSecurity;

        /// <summary>
        /// Private member to store info about look thru enabled or not
        /// </summary>
        private bool _lookThruEnabled;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="param">DashBoardGadgetParam</param>
        public ViewModelTopHoldings(DashboardGadgetParam param)
        {
            _eventAggregator = param.EventAggregator;
            _dbInteractivity = param.DBInteractivity;
            _logger = param.LoggerFacade;
            _regionManager = param.RegionManager;

            _portfolioSelectionData = param.DashboardGadgetPayload.PortfolioSelectionData;
            _isExCashSecurity = param.DashboardGadgetPayload.IsExCashSecurityData;
            EffectiveDate = param.DashboardGadgetPayload.EffectiveDate;
            _lookThruEnabled = param.DashboardGadgetPayload.IsLookThruEnabled;
            if ((_portfolioSelectionData != null) && (EffectiveDate != null))
            {
                _dbInteractivity.RetrieveTopHoldingsData(_portfolioSelectionData, Convert.ToDateTime(_effectiveDate), _isExCashSecurity,_lookThruEnabled, RetrieveTopHoldingsDataCallbackMethod);
            }

            if (_eventAggregator != null)
            {
                _eventAggregator.GetEvent<PortfolioReferenceSetEvent>().Subscribe(HandlePortfolioReferenceSet);
                _eventAggregator.GetEvent<EffectiveDateReferenceSetEvent>().Subscribe(HandleEffectiveDateSet);
                _eventAggregator.GetEvent<ExCashSecuritySetEvent>().Subscribe(HandleExCashSecuritySetEvent);
                _eventAggregator.GetEvent<LookThruFilterReferenceSetEvent>().Subscribe(HandleLookThruReferenceSetEvent);
            }
        }
        #endregion

        #region Properties
        #region UI Fields
        /// <summary>
        /// contains data to be displayed in this gadget
        /// </summary>
        private ObservableCollection<TopHoldingsData> _topHoldingsInfo;
        public ObservableCollection<TopHoldingsData> TopHoldingsInfo
        {
            get { return _topHoldingsInfo; }
            set
            {
                if (_topHoldingsInfo != value)
                {
                    _topHoldingsInfo = value;
                    RaisePropertyChanged(() => this.TopHoldingsInfo);
                }
            }
        }

        /// <summary>
        /// property to contain effective date value from EffectiveDate Datepicker
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
                    RaisePropertyChanged(() => EffectiveDate);
                }
            }
        }
               
        #endregion

        #region ICommand

        public ICommand DetailsCommand
        {
            get { return new DelegateCommand<object>(DetailsCommandMethod); }
        }
        #endregion
        #endregion

        #region Event Handlers
        /// <summary>
        /// Event Handler to subscribed event 'PortfolioReferenceSetEvent'
        /// </summary>
        /// <param name="portfolioSelectionData">PortfolioSelectionData</param>
        public void HandlePortfolioReferenceSet(PortfolioSelectionData portfolioSelectionData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);

            try
            {
                if (portfolioSelectionData != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, portfolioSelectionData, 1);
                    _portfolioSelectionData = portfolioSelectionData;
                    if ((_portfolioSelectionData != null) && (EffectiveDate != null))
                    {
                        _dbInteractivity.RetrieveTopHoldingsData(_portfolioSelectionData, Convert.ToDateTime(_effectiveDate), _isExCashSecurity, _lookThruEnabled, RetrieveTopHoldingsDataCallbackMethod);
                        if (TopHoldingsDataLoadedEvent != null)
                            TopHoldingsDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
                    }

                    else
                    {
                        Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                    }
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
        /// Event Handler to subscribed event 'EffectiveDateSet'
        /// </summary>
        /// <param name="effectiveDate">DateTime</param>
        public void HandleEffectiveDateSet(DateTime effectiveDate)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (effectiveDate != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, effectiveDate, 1);
                    EffectiveDate = effectiveDate;
                    if ((_portfolioSelectionData != null) && (EffectiveDate != null))
                    {
                        _dbInteractivity.RetrieveTopHoldingsData(_portfolioSelectionData, Convert.ToDateTime(_effectiveDate), _isExCashSecurity, _lookThruEnabled, RetrieveTopHoldingsDataCallbackMethod);
                        if (TopHoldingsDataLoadedEvent != null)
                            TopHoldingsDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
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
        /// Event Handler to Check for Cash Securities
        /// </summary>
        /// <param name="isExCashSec"></param>
        public void HandleExCashSecuritySetEvent(bool isExCashSec)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                Logging.LogMethodParameter(_logger, methodNamespace, isExCashSec, 1);
                if (isExCashSec != null)
                {
                    _isExCashSecurity = isExCashSec;

                    if ((_portfolioSelectionData != null) && (EffectiveDate != null))
                    {
                        _dbInteractivity.RetrieveTopHoldingsData(_portfolioSelectionData, Convert.ToDateTime(_effectiveDate), _isExCashSecurity, _lookThruEnabled, RetrieveTopHoldingsDataCallbackMethod);
                        if (TopHoldingsDataLoadedEvent != null)
                            TopHoldingsDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
                    }
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

                if ((_portfolioSelectionData != null) && (EffectiveDate != null))
                {
                    _dbInteractivity.RetrieveTopHoldingsData(_portfolioSelectionData, Convert.ToDateTime(_effectiveDate), _isExCashSecurity, _lookThruEnabled, RetrieveTopHoldingsDataCallbackMethod);
                    if (TopHoldingsDataLoadedEvent != null)
                        TopHoldingsDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
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

        #region ICommand Methods

        private void DetailsCommandMethod(object param)
        {
            Logging.LogBeginMethod(_logger,String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name));
            try 
	        {
                _regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardPortfolioHoldings", UriKind.Relative));

		
	        }
	         catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name));
        }
        #endregion

        #region Callback Methods
        /// <summary>
        /// callback method for RetrieveTopHoldingsData service call
        /// </summary>
        /// <param name="topHoldingsData">TopHoldingsData collection</param>
        private void RetrieveTopHoldingsDataCallbackMethod(List<TopHoldingsData> topHoldingsData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (topHoldingsData != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, topHoldingsData, 1);
                    TopHoldingsInfo = new ObservableCollection<TopHoldingsData>(topHoldingsData);
                }
                else
                {
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                }
                if (TopHoldingsDataLoadedEvent != null)
                    TopHoldingsDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
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
        /// event to handle data retrieval progress indicator
        /// </summary>
        public event DataRetrievalProgressIndicatorEventHandler TopHoldingsDataLoadedEvent;

        #endregion

        #region Dispose Method
        /// <summary>
        /// method to dispose all subscribed events
        /// </summary>
        public void Dispose()
        {
            _eventAggregator.GetEvent<PortfolioReferenceSetEvent>().Unsubscribe(HandlePortfolioReferenceSet);
            _eventAggregator.GetEvent<EffectiveDateReferenceSetEvent>().Unsubscribe(HandleEffectiveDateSet);
            _eventAggregator.GetEvent<ExCashSecuritySetEvent>().Unsubscribe(HandleExCashSecuritySetEvent);
            _eventAggregator.GetEvent<LookThruFilterReferenceSetEvent>().Unsubscribe(HandleLookThruReferenceSetEvent);
        }
        #endregion
    }


}
