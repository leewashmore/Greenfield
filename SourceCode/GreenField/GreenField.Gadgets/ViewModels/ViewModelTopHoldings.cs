using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using GreenField.Common;
using GreenField.DataContracts;
using GreenField.ServiceCaller;

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
        private IEventAggregator eventAggregator;
        private IDBInteractivity dbInteractivity;
        private ILoggerFacade logger;
        private IRegionManager regionManager;

        /// <summary>
        /// DashboardGadgetPayLoad fields
        /// </summary>
        private PortfolioSelectionData portfolioSelectionDataInfo;

        /// <summary>
        /// Private member to store info about including or excluding cash securities
        /// </summary>
        private bool  isExCashSecurity;

        /// <summary>
        /// Private member to store info about look thru enabled or not
        /// </summary>
        private bool  lookThruEnabled;

        /// <summary>
        /// IsActive is true when parent control is displayed on UI
        /// </summary>
        private bool  isActive;
        public bool IsActive
        {
            get { return  isActive; }
            set
            {
                if ( isActive != value)
                {
                     isActive = value;
                    if ((portfolioSelectionDataInfo != null) && (EffectiveDate != null) &&  isActive)
                    {
                        dbInteractivity.RetrieveTopHoldingsData(portfolioSelectionDataInfo, Convert.ToDateTime( effectiveDate),  isExCashSecurity,  lookThruEnabled,
                            RetrieveTopHoldingsDataCallbackMethod);
                        BusyIndicatorStatus = true;
                    }
                }
            }
        }

        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="param">DashBoardGadgetParam</param>
        public ViewModelTopHoldings(DashboardGadgetParam param)
        {
            eventAggregator = param.EventAggregator;
            dbInteractivity = param.DBInteractivity;
            logger = param.LoggerFacade;
            regionManager = param.RegionManager;
            portfolioSelectionDataInfo = param.DashboardGadgetPayload.PortfolioSelectionData;
            isExCashSecurity = param.DashboardGadgetPayload.IsExCashSecurityData;
            EffectiveDate = param.DashboardGadgetPayload.EffectiveDate;
            lookThruEnabled = param.DashboardGadgetPayload.IsLookThruEnabled;
            if ((portfolioSelectionDataInfo != null) && (EffectiveDate != null) && IsActive)
            {
                dbInteractivity.RetrieveTopHoldingsData(portfolioSelectionDataInfo, Convert.ToDateTime( effectiveDate),  isExCashSecurity, lookThruEnabled,
                    RetrieveTopHoldingsDataCallbackMethod);
                BusyIndicatorStatus = true;
            }

            if (eventAggregator != null)
            {
                eventAggregator.GetEvent<PortfolioReferenceSetEvent>().Subscribe(HandlePortfolioReferenceSet);
                eventAggregator.GetEvent<EffectiveDateReferenceSetEvent>().Subscribe(HandleEffectiveDateSet);
                eventAggregator.GetEvent<ExCashSecuritySetEvent>().Subscribe(HandleExCashSecuritySetEvent);
                eventAggregator.GetEvent<LookThruFilterReferenceSetEvent>().Subscribe(HandleLookThruReferenceSetEvent);
            }
        }
        #endregion

        #region Properties
        #region UI Fields
        /// <summary>
        /// contains data to be displayed in this gadget
        /// </summary>
        private ObservableCollection<TopHoldingsData>  topHoldingsInfo;
        public ObservableCollection<TopHoldingsData> TopHoldingsInfo
        {
            get { return  topHoldingsInfo; }
            set
            {
                if ( topHoldingsInfo != value)
                {
                     topHoldingsInfo = value;
                    RaisePropertyChanged(() => this.TopHoldingsInfo);
                }
            }
        }

        /// <summary>
        /// property to contain effective date value from EffectiveDate Datepicker
        /// </summary>
        private DateTime?  effectiveDate;
        public DateTime? EffectiveDate
        {
            get { return  effectiveDate; }
            set
            {
                if ( effectiveDate != value)
                {
                     effectiveDate = value;
                    RaisePropertyChanged(() => EffectiveDate);
                }
            }
        }

        /// <summary>
        /// property to contain status value for busy indicator of the gadget
        /// </summary>
        private bool  busyIndicatorStatus;
        public bool BusyIndicatorStatus
        {
            get { return  busyIndicatorStatus; }
            set
            {
                if ( busyIndicatorStatus != value)
                {
                     busyIndicatorStatus = value;
                    RaisePropertyChanged(() => BusyIndicatorStatus);
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
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (portfolioSelectionData != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, portfolioSelectionData, 1);
                    portfolioSelectionDataInfo = portfolioSelectionData;
                    if ((portfolioSelectionDataInfo != null) && (EffectiveDate != null) && IsActive)
                    {
                        dbInteractivity.RetrieveTopHoldingsData(portfolioSelectionDataInfo, Convert.ToDateTime( effectiveDate),  isExCashSecurity,  lookThruEnabled,
                            RetrieveTopHoldingsDataCallbackMethod);
                        BusyIndicatorStatus = true;
                    }

                    else
                    {
                        Logging.LogMethodParameterNull(logger, methodNamespace, 1);
                    }
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
        /// Event Handler to subscribed event 'EffectiveDateSet'
        /// </summary>
        /// <param name="effectiveDate">DateTime</param>
        public void HandleEffectiveDateSet(DateTime effectiveDateInfo)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (effectiveDateInfo != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, effectiveDate, 1);
                    EffectiveDate = effectiveDateInfo;
                    if ((portfolioSelectionDataInfo != null) && (EffectiveDate != null) && IsActive)
                    {
                        dbInteractivity.RetrieveTopHoldingsData(portfolioSelectionDataInfo, Convert.ToDateTime( effectiveDate),  isExCashSecurity,  lookThruEnabled,
                            RetrieveTopHoldingsDataCallbackMethod);
                        BusyIndicatorStatus = true;
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
        /// Event Handler to Check for Cash Securities
        /// </summary>
        /// <param name="isExCashSec"></param>
        public void HandleExCashSecuritySetEvent(bool isExCashSec)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                Logging.LogMethodParameter(logger, methodNamespace, isExCashSec, 1);
                if ( isExCashSecurity != isExCashSec)
                {
                     isExCashSecurity = isExCashSec;

                    if ((portfolioSelectionDataInfo != null) && (EffectiveDate != null) && IsActive)
                    {
                        dbInteractivity.RetrieveTopHoldingsData(portfolioSelectionDataInfo, Convert.ToDateTime( effectiveDate),  isExCashSecurity,  lookThruEnabled, 
                            RetrieveTopHoldingsDataCallbackMethod);
                        BusyIndicatorStatus = true;
                    }
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
                if ( lookThruEnabled != enableLookThru)
                {
                     lookThruEnabled = enableLookThru;

                    if ((portfolioSelectionDataInfo != null) && (EffectiveDate != null) && IsActive)
                    {
                        dbInteractivity.RetrieveTopHoldingsData(portfolioSelectionDataInfo, Convert.ToDateTime( effectiveDate),  isExCashSecurity,  lookThruEnabled, 
                            RetrieveTopHoldingsDataCallbackMethod);
                        BusyIndicatorStatus = true;
                    }
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

        #region ICommand Methods
        /// <summary>
        /// method to navigate user to another gadget on click of a button
        /// </summary>
        /// <param name="param"></param>
        private void DetailsCommandMethod(object param)
        {
            Logging.LogBeginMethod(logger,String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name));
            try 
	        {
                regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardPortfolioHoldings", UriKind.Relative));		
	        }
	         catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name));
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
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (topHoldingsData != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, topHoldingsData, 1);
                    TopHoldingsInfo = new ObservableCollection<TopHoldingsData>(topHoldingsData);
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

        #region Dispose Method
        /// <summary>
        /// method to dispose all subscribed events
        /// </summary>
        public void Dispose()
        {
            eventAggregator.GetEvent<PortfolioReferenceSetEvent>().Unsubscribe(HandlePortfolioReferenceSet);
            eventAggregator.GetEvent<EffectiveDateReferenceSetEvent>().Unsubscribe(HandleEffectiveDateSet);
            eventAggregator.GetEvent<ExCashSecuritySetEvent>().Unsubscribe(HandleExCashSecuritySetEvent);
            eventAggregator.GetEvent<LookThruFilterReferenceSetEvent>().Unsubscribe(HandleLookThruReferenceSetEvent);
        }
        #endregion
    }
}
