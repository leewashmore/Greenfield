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
using Microsoft.Practices.Prism.ViewModel;
using System.Collections.Generic;
using GreenField.ServiceCaller.BenchmarkHoldingsDefinitions;
using GreenField.Gadgets.Helpers;
using GreenField.DataContracts;

namespace GreenField.Gadgets.ViewModels
{
    /// <summary>
    /// View Model for Asset Allocation Gadget
    /// </summary>
    public class ViewModelAssetAllocation : NotificationObject
    {
        //MEF Singletons
        #region Fields

        /// <summary>
        /// Event Aggregator
        /// </summary>
        private IEventAggregator _eventAggregator;

        /// <summary>
        /// Instance of Service Caller Class
        /// </summary>
        private IDBInteractivity _dbInteractivity;

        /// <summary>
        /// Instance of LoggerFacade
        /// </summary>
        private ILoggerFacade _logger;

        /// <summary>
        /// Details of Selected Portfolio
        /// </summary>
        private PortfolioSelectionData _portfolioSelectionData;

        /// <summary>
        /// Selected Date
        /// </summary>
        private DateTime? _effectiveDate;

        /// <summary>
        /// Filter for checking LookThru
        /// </summary>
        private bool _enableLookThru = false;

        /// <summary>
        /// IsActive is true when parent control is displayed on UI
        /// </summary>
        public bool IsActive { get; set; }


        #endregion

        #region Constructor

        /// <summary>
        /// Constructor of the View Model
        /// </summary>
        /// <param name="param"></param>
        public ViewModelAssetAllocation(DashboardGadgetParam param)
        {
            _eventAggregator = param.EventAggregator;
            _dbInteractivity = param.DBInteractivity;
            _logger = param.LoggerFacade;
            _enableLookThru = param.DashboardGadgetPayload.IsLookThruEnabled;
            _portfolioSelectionData = param.DashboardGadgetPayload.PortfolioSelectionData;

            _effectiveDate = param.DashboardGadgetPayload.EffectiveDate;
            ExcludeCashSecurities = param.DashboardGadgetPayload.IsExCashSecurityData;
            if ((_portfolioSelectionData != null) && (_effectiveDate != null))
            {
                _dbInteractivity.RetrieveAssetAllocationData(_portfolioSelectionData, Convert.ToDateTime(_effectiveDate), _enableLookThru, ExcludeCashSecurities, RetrieveAssetAllocationDataCallbackMethod);
                BusyIndicatorStatus = true;
            }
            if (_eventAggregator != null)
            {
                _eventAggregator.GetEvent<PortfolioReferenceSetEvent>().Subscribe(HandleFundReferenceSet);
                _eventAggregator.GetEvent<EffectiveDateReferenceSetEvent>().Subscribe(HandleEffectiveDateSet);
                _eventAggregator.GetEvent<LookThruFilterReferenceSetEvent>().Subscribe(HandleLookThruReferenceSet);
                _eventAggregator.GetEvent<ExCashSecuritySetEvent>().Subscribe(HandleExCashSecuritySetEvent);
            }
        }

        #endregion

        #region PropertiesDeclaration

        #region UI Fields

        /// <summary>
        /// Collection of AssetAllocationData bound to the DataGrid
        /// </summary>
        private RangeObservableCollection<AssetAllocationData> _assetAllocationData;
        public RangeObservableCollection<AssetAllocationData> AssetAllocationData
        {
            get
            {
                if (_assetAllocationData == null)
                    _assetAllocationData = new RangeObservableCollection<AssetAllocationData>();
                return _assetAllocationData;
            }
            set
            {
                if (_assetAllocationData != value)
                {
                    _assetAllocationData = value;
                    RaisePropertyChanged(() => this.AssetAllocationData);
                }
            }
        }

        /// <summary>
        /// Busy Indicator Status
        /// </summary>
        private bool _busyIndicatorStatus;
        public bool BusyIndicatorStatus
        {
            get
            {
                return _busyIndicatorStatus;
            }
            set
            {
                _busyIndicatorStatus = value;
                this.RaisePropertyChanged(() => this.BusyIndicatorStatus);
            }
        }

        /// <summary>
        /// Check to include Cash Securities
        /// </summary>
        private bool _excludeCashSecurities;
        public bool ExcludeCashSecurities
        {
            get
            {
                return _excludeCashSecurities;
            }
            set
            {
                _excludeCashSecurities = value;
                this.RaisePropertyChanged(() => this.ExcludeCashSecurities);
            }
        }

        #endregion
        #endregion

        #region Events

        /// <summary>
        /// Data Retrieval Progress Indicator
        /// </summary>
        public event DataRetrievalProgressIndicatorEventHandler AssetAllocationDataLoadedEvent;

        #endregion

        #region Event Handlers

        /// <summary>
        /// Handle Fund Change Event
        /// </summary>
        /// <param name="PortfolioSelectionData">Details of Selected Portfolio</param>
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
                    if (_effectiveDate != null && _portfolioSelectionData != null && IsActive)
                    {
                        BusyIndicatorStatus = true;
                        _dbInteractivity.RetrieveAssetAllocationData(_portfolioSelectionData, Convert.ToDateTime(_effectiveDate), _enableLookThru, ExcludeCashSecurities, RetrieveAssetAllocationDataCallbackMethod);
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
        /// Handle Date Change Event
        /// </summary>
        /// <param name="effectiveDate">Effective Date</param>
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
                    if (_effectiveDate != null && _portfolioSelectionData != null && IsActive)
                    {
                        BusyIndicatorStatus = true;
                        _dbInteractivity.RetrieveAssetAllocationData(_portfolioSelectionData, Convert.ToDateTime(_effectiveDate), _enableLookThru, ExcludeCashSecurities, RetrieveAssetAllocationDataCallbackMethod);
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
        /// Handle the LookThru Event set
        /// </summary>
        /// <param name="enableLookThru">LookThruEnabled/Disabled</param>
        public void HandleLookThruReferenceSet(bool enableLookThru)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {

                Logging.LogMethodParameter(_logger, methodNamespace, enableLookThru, 1);
                _enableLookThru = enableLookThru;
                if (_effectiveDate != null && _portfolioSelectionData != null && IsActive)
                {
                    BusyIndicatorStatus = true;
                    _dbInteractivity.RetrieveAssetAllocationData(_portfolioSelectionData, Convert.ToDateTime(_effectiveDate), _enableLookThru, ExcludeCashSecurities, RetrieveAssetAllocationDataCallbackMethod);
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
        /// Handle the ExcludeCash Event
        /// </summary>
        /// <param name="excludeCash">Exclude/Include Cash Securities</param>
        public void HandleExCashSecuritySetEvent(bool excludeCash)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {

                Logging.LogMethodParameter(_logger, methodNamespace, excludeCash, 1);
                ExcludeCashSecurities = excludeCash;
                if (_effectiveDate != null && _portfolioSelectionData != null && IsActive)
                {
                    BusyIndicatorStatus = true;
                    _dbInteractivity.RetrieveAssetAllocationData(_portfolioSelectionData, Convert.ToDateTime(_effectiveDate), _enableLookThru, ExcludeCashSecurities, RetrieveAssetAllocationDataCallbackMethod);
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
        /// Callback Method for AssetAllocationData
        /// </summary>
        /// <param name="assetAllocationData">Returns Collection of AssetAllocationData</param>
        private void RetrieveAssetAllocationDataCallbackMethod(List<AssetAllocationData> assetAllocationData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (assetAllocationData != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, assetAllocationData, 1);
                    AssetAllocationData.Clear();
                    AssetAllocationData.AddRange(assetAllocationData);
                }
                else
                {
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                }
                if (null != AssetAllocationDataLoadedEvent)
                    AssetAllocationDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            finally
            {
                BusyIndicatorStatus = false;
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        #endregion

        #region EventUnsubscribe

        /// <summary>
        /// Method to Kill Events & Event Subscribers
        /// </summary>
        public void Dispose()
        {
            _eventAggregator.GetEvent<PortfolioReferenceSetEvent>().Unsubscribe(HandleFundReferenceSet);
            _eventAggregator.GetEvent<EffectiveDateReferenceSetEvent>().Unsubscribe(HandleEffectiveDateSet);
        }

        #endregion

    }
}
