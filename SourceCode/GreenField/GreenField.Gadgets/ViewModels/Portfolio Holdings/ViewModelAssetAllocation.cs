using System;
using System.Collections.Generic;
using System.Windows;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.ViewModel;
using GreenField.Gadgets.Helpers;
using GreenField.DataContracts;
using GreenField.ServiceCaller;
using GreenField.Common;

namespace GreenField.Gadgets.ViewModels
{
    /// <summary>
    /// View Model for Asset Allocation Gadget
    /// </summary>
    public class ViewModelAssetAllocation : NotificationObject
    {
        #region Fields

        /// <summary>
        /// Event Aggregator
        /// </summary>
        private IEventAggregator eventAggregator;

        /// <summary>
        /// Instance of Service Caller Class
        /// </summary>
        private IDBInteractivity dbInteractivity;

        /// <summary>
        /// Instance of LoggerFacade
        /// </summary>
        private ILoggerFacade logger;

        /// <summary>
        /// Details of Selected Portfolio
        /// </summary>
        private PortfolioSelectionData portfolioSelectionData;

        /// <summary>
        /// Selected Date
        /// </summary>
        private DateTime? effectiveDate;

        /// <summary>
        /// Filter for checking LookThru
        /// </summary>
        private bool enableLookThru = false;

        /// <summary>
        /// IsActive is true when parent control is displayed on UI
        /// </summary>
        private bool isActive;
        public bool IsActive
        {
            get
            {
                return isActive;
            }
            set
            {
                isActive = value;
                if (effectiveDate != null && portfolioSelectionData != null && isActive)
                {
                    BusyIndicatorStatus = true;
                    dbInteractivity.RetrieveAssetAllocationData(portfolioSelectionData, Convert.ToDateTime(effectiveDate), enableLookThru, ExcludeCashSecurities, RetrieveAssetAllocationDataCallbackMethod);
                }
            }
        }


        #endregion

        #region Constructor

        /// <summary>
        /// Constructor of the View Model
        /// </summary>
        /// <param name="param">DashboardGadgetParam: Contains instances of MEF singletons</param>
        public ViewModelAssetAllocation(DashboardGadgetParam param)
        {
            eventAggregator = param.EventAggregator;
            dbInteractivity = param.DBInteractivity;
            logger = param.LoggerFacade;
            enableLookThru = param.DashboardGadgetPayload.IsLookThruEnabled;
            portfolioSelectionData = param.DashboardGadgetPayload.PortfolioSelectionData;

            effectiveDate = param.DashboardGadgetPayload.EffectiveDate;
            ExcludeCashSecurities = param.DashboardGadgetPayload.IsExCashSecurityData;
            if ((portfolioSelectionData != null) && (effectiveDate != null))
            {
                dbInteractivity.RetrieveAssetAllocationData(portfolioSelectionData, Convert.ToDateTime(effectiveDate), enableLookThru, ExcludeCashSecurities, RetrieveAssetAllocationDataCallbackMethod);
                BusyIndicatorStatus = true;
            }
            if (eventAggregator != null)
            {
                eventAggregator.GetEvent<PortfolioReferenceSetEvent>().Subscribe(HandleFundReferenceSet);
                eventAggregator.GetEvent<EffectiveDateReferenceSetEvent>().Subscribe(HandleEffectiveDateSet);
                eventAggregator.GetEvent<LookThruFilterReferenceSetEvent>().Subscribe(HandleLookThruReferenceSet);
                eventAggregator.GetEvent<ExCashSecuritySetEvent>().Subscribe(HandleExCashSecuritySetEvent);
            }
        }

        #endregion

        #region PropertiesDeclaration

        #region UI Fields

        /// <summary>
        /// Collection of AssetAllocationData bound to the DataGrid
        /// </summary>
        private RangeObservableCollection<AssetAllocationData> assetAllocationData;
        public RangeObservableCollection<AssetAllocationData> AssetAllocationData
        {
            get
            {
                if (assetAllocationData == null)
                    assetAllocationData = new RangeObservableCollection<AssetAllocationData>();
                return assetAllocationData;
            }
            set
            {
                if (assetAllocationData != value)
                {
                    assetAllocationData = value;
                    RaisePropertyChanged(() => this.AssetAllocationData);
                }
            }
        }

        /// <summary>
        /// Busy Indicator Status
        /// </summary>
        private bool busyIndicatorStatus;
        public bool BusyIndicatorStatus
        {
            get
            {
                return busyIndicatorStatus;
            }
            set
            {
                busyIndicatorStatus = value;
                this.RaisePropertyChanged(() => this.BusyIndicatorStatus);
            }
        }

        /// <summary>
        /// Check to include Cash Securities
        /// </summary>
        private bool excludeCashSecurities;
        public bool ExcludeCashSecurities
        {
            get
            {
                return excludeCashSecurities;
            }
            set
            {
                excludeCashSecurities = value;
                this.RaisePropertyChanged(() => this.ExcludeCashSecurities);
            }
        }

        #endregion

        #endregion

        #region Event Handlers

        /// <summary>
        /// Handle Fund Change Event
        /// </summary>
        /// <param name="portfolioSelectionData">Details of Selected Portfolio</param>
        public void HandleFundReferenceSet(PortfolioSelectionData portfolioSelectionData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);

            try
            {
                if (portfolioSelectionData != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, portfolioSelectionData, 1);
                    this.portfolioSelectionData = portfolioSelectionData;
                    if (effectiveDate != null && portfolioSelectionData != null && IsActive)
                    {
                        BusyIndicatorStatus = true;
                        dbInteractivity.RetrieveAssetAllocationData(portfolioSelectionData, Convert.ToDateTime(effectiveDate), enableLookThru, ExcludeCashSecurities, RetrieveAssetAllocationDataCallbackMethod);
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
        /// Handle Date Change Event
        /// </summary>
        /// <param name="effectiveDate">Effective Date</param>
        public void HandleEffectiveDateSet(DateTime effectiveDate)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (effectiveDate != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, effectiveDate, 1);
                    this.effectiveDate = effectiveDate;
                    if (effectiveDate != null && portfolioSelectionData != null && IsActive)
                    {
                        BusyIndicatorStatus = true;
                        dbInteractivity.RetrieveAssetAllocationData(portfolioSelectionData, Convert.ToDateTime(effectiveDate), enableLookThru, ExcludeCashSecurities, RetrieveAssetAllocationDataCallbackMethod);
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
        /// Handle the LookThru Event set
        /// </summary>
        /// <param name="enableLookThru">LookThruEnabled/Disabled</param>
        public void HandleLookThruReferenceSet(bool enableLookThru)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                Logging.LogMethodParameter(logger, methodNamespace, enableLookThru, 1);
                this.enableLookThru = enableLookThru;
                if (effectiveDate != null && portfolioSelectionData != null && IsActive)
                {
                    BusyIndicatorStatus = true;
                    dbInteractivity.RetrieveAssetAllocationData(portfolioSelectionData, Convert.ToDateTime(effectiveDate), enableLookThru, ExcludeCashSecurities, RetrieveAssetAllocationDataCallbackMethod);
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
        /// Handle the ExcludeCash Event
        /// </summary>
        /// <param name="excludeCash">Exclude/Include Cash Securities</param>
        public void HandleExCashSecuritySetEvent(bool excludeCash)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {

                Logging.LogMethodParameter(logger, methodNamespace, excludeCash, 1);
                ExcludeCashSecurities = excludeCash;
                if (effectiveDate != null && portfolioSelectionData != null && IsActive)
                {
                    BusyIndicatorStatus = true;
                    dbInteractivity.RetrieveAssetAllocationData(portfolioSelectionData, Convert.ToDateTime(effectiveDate), enableLookThru, ExcludeCashSecurities, RetrieveAssetAllocationDataCallbackMethod);
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

        #region Callback Methods

        /// <summary>
        /// Callback Method for AssetAllocationData
        /// </summary>
        /// <param name="assetAllocationData">Returns Collection of AssetAllocationData</param>
        private void RetrieveAssetAllocationDataCallbackMethod(List<AssetAllocationData> assetAllocationData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (assetAllocationData != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, assetAllocationData, 1);
                    AssetAllocationData.Clear();
                    AssetAllocationData.AddRange(assetAllocationData);
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
            finally
            {
                BusyIndicatorStatus = false;
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }

        #endregion

        #region EventUnsubscribe

        /// <summary>
        /// Method to Kill Events & Event Subscribers
        /// </summary>
        public void Dispose()
        {
            eventAggregator.GetEvent<PortfolioReferenceSetEvent>().Unsubscribe(HandleFundReferenceSet);
            eventAggregator.GetEvent<EffectiveDateReferenceSetEvent>().Unsubscribe(HandleEffectiveDateSet);
        }

        #endregion

    }
}
