using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.ViewModel;
using GreenField.Common;
using GreenField.DataContracts;
using GreenField.ServiceCaller;

namespace GreenField.Gadgets.ViewModels
{
    /// <summary>
    /// View model for ViewContributorDetractor class
    /// </summary>
    public class ViewModelContributorDetractor : NotificationObject
    {
         #region Fields
        /// <summary>
        /// MEF Singletons
        /// </summary>
        private IEventAggregator eventAggregator;
        private IDBInteractivity dbInteractivity;
        private ILoggerFacade logger;
             
        /// <summary>
        /// DashboardGadgetPayLoad fields
        /// </summary>
        PortfolioSelectionData portfolioSelectionDataInfo;

        //To check that grid is not re populated for same values when Relative Performance Matrix is clicked
        RelativePerformanceGridCellData checkValue = new RelativePerformanceGridCellData();
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="param">DashboardGadgetparam</param>
        public ViewModelContributorDetractor(DashboardGadgetParam param)
        {
            eventAggregator = param.EventAggregator;
            dbInteractivity = param.DBInteractivity;
            logger = param.LoggerFacade;
            portfolioSelectionDataInfo = param.DashboardGadgetPayload.PortfolioSelectionData;
            EffectiveDate = param.DashboardGadgetPayload.EffectiveDate;
            periodInfo = param.DashboardGadgetPayload.PeriodSelectionData;

            if (EffectiveDate != null && portfolioSelectionDataInfo != null && Period != null && IsActive)
            {
                dbInteractivity.RetrieveRelativePerformanceSecurityData(portfolioSelectionDataInfo, Convert.ToDateTime(effectiveDateInfo),periodInfo, 
                    RetrieveRelativePerformanceSecurityDataCallbackMethod);
                BusyIndicatorStatus = true;
            }

            if (eventAggregator != null)
            {
                eventAggregator.GetEvent<PortfolioReferenceSetEvent>().Subscribe(HandlePortfolioReferenceSet);
                eventAggregator.GetEvent<EffectiveDateReferenceSetEvent>().Subscribe(HandleEffectiveDateSet);
                eventAggregator.GetEvent<PeriodReferenceSetEvent>().Subscribe(HandlePeriodReferenceSet);
                eventAggregator.GetEvent<RelativePerformanceGridClickEvent>().Subscribe(HandleRelativePerformanceGridClickEvent);
            }           
        } 
        #endregion

        #region Properties        
        #region UI Fields

        /// <summary>
        /// IsActive is true when parent control is displayed on UI
        /// </summary>
        private bool isActive;
        public bool IsActive
        {
            get{ return isActive; }
            set
            {
                if (isActive != value)
                {
                    isActive = value;
                    if (EffectiveDate != null && portfolioSelectionDataInfo != null && Period != null && isActive)
                    {
                        dbInteractivity.RetrieveRelativePerformanceSecurityData(portfolioSelectionDataInfo, Convert.ToDateTime(effectiveDateInfo), periodInfo, 
                            RetrieveRelativePerformanceSecurityDataCallbackMethod);
                        BusyIndicatorStatus = true;
                    }
                }
            }
        }
        #endregion

        /// <summary>
        /// Effective date selected
        /// </summary>
        private DateTime? effectiveDateInfo;
        public DateTime? EffectiveDate
        {
            get { return effectiveDateInfo; }
            set
            {
                if (effectiveDateInfo != value)
                {
                    effectiveDateInfo = value;
                    RaisePropertyChanged(() => EffectiveDate);
                }
            }
        }

        /// <summary>
        /// Period selected
        /// </summary>
        private string periodInfo;
        public string Period
        {
            get { return periodInfo; }
            set
            {
                if (periodInfo != value)
                {
                    periodInfo = value;
                    RaisePropertyChanged(() => Period);
                }
            }
        }        

        /// <summary>
        /// contains all data to be displayed in the gadget.
        /// </summary>
        private ObservableCollection<RelativePerformanceSecurityData> contributorDetractorInfo;
        public ObservableCollection<RelativePerformanceSecurityData> ContributorDetractorInfo
        {
            get { return contributorDetractorInfo; }
            set 
            {
                if (contributorDetractorInfo != value)
                {
                    contributorDetractorInfo = value;
                    RaisePropertyChanged(() => ContributorDetractorInfo);

                }
            }
        }

        /// <summary>
        /// property to contain status value for busy indicator of the gadget
        /// </summary>
        private bool busyIndicatorStatus;
        public bool BusyIndicatorStatus
        {
            get { return busyIndicatorStatus; }
            set
            {
                if (busyIndicatorStatus != value)
                {
                    busyIndicatorStatus = value;
                    RaisePropertyChanged(() => BusyIndicatorStatus);
                }
            }
        }
        #endregion  

        #region Event Handlers
        /// <summary>
        /// Event Handler to subscribed event 'FundReferenceSetEvent'
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
                    if (EffectiveDate != null && portfolioSelectionDataInfo != null && Period != null && IsActive)
                    {
                        dbInteractivity.RetrieveRelativePerformanceSecurityData(portfolioSelectionDataInfo, Convert.ToDateTime(effectiveDateInfo),periodInfo, 
                            RetrieveRelativePerformanceSecurityDataCallbackMethod);
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
        /// Event Handler to subscribed event 'EffectiveDateSet'
        /// </summary>
        /// <param name="effectiveDate">DateTime</param>
        public void HandleEffectiveDateSet(DateTime effectiveDate)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (effectiveDate != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, effectiveDate, 1);
                    EffectiveDate = effectiveDate;
                    if (EffectiveDate != null && portfolioSelectionDataInfo != null && Period != null && IsActive)
                    {
                        dbInteractivity.RetrieveRelativePerformanceSecurityData(portfolioSelectionDataInfo, Convert.ToDateTime(effectiveDateInfo),periodInfo,
                            RetrieveRelativePerformanceSecurityDataCallbackMethod);
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
        /// Event Handler to subscribed event 'PeriodReferenceSetEvent'
        /// </summary>
        /// <param name="period"></param>
        public void HandlePeriodReferenceSet(string period)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (period != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, period, 1);
                    Period = period;
                    if (EffectiveDate != null && portfolioSelectionDataInfo != null && Period != null && IsActive)
                    {
                       dbInteractivity.RetrieveRelativePerformanceSecurityData(portfolioSelectionDataInfo, Convert.ToDateTime(effectiveDateInfo), periodInfo,
                           RetrieveRelativePerformanceSecurityDataCallbackMethod);
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
        /// Event Handler to subscribed event 'RelativePerformanceGridClickEvent'
        /// </summary>
        /// <param name="relativePerformanceGridCellData">RelativePerformanceGridCellData</param>
        public void HandleRelativePerformanceGridClickEvent(RelativePerformanceGridCellData relativePerformanceGridCellData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (relativePerformanceGridCellData != null)
                {
                    if (checkValue.CountryID != relativePerformanceGridCellData.CountryID || checkValue.SectorID != relativePerformanceGridCellData.SectorID)
                    {
                        checkValue = relativePerformanceGridCellData;
                        Logging.LogMethodParameter(logger, methodNamespace, relativePerformanceGridCellData, 1);
                        if (EffectiveDate != null && portfolioSelectionDataInfo != null && IsActive)
                        {
                            dbInteractivity.RetrieveRelativePerformanceSecurityData(portfolioSelectionDataInfo, Convert.ToDateTime(effectiveDateInfo),
                                periodInfo, RetrieveRelativePerformanceSecurityDataCallbackMethod, relativePerformanceGridCellData.CountryID,
                                relativePerformanceGridCellData.SectorID);
                            BusyIndicatorStatus = true;
                        }
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
        #endregion

        #region Callback Methods

        /// <summary>
        /// Callback method for RetrieveRelativePerformanceSecurityData Service call
        /// </summary>
        /// <param name="result">RelativePerformanceSecurityData Collection</param>
        public void RetrieveRelativePerformanceSecurityDataCallbackMethod(List<RelativePerformanceSecurityData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    ContributorDetractorInfo = new ObservableCollection<RelativePerformanceSecurityData>(result);
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
            eventAggregator.GetEvent<PeriodReferenceSetEvent>().Unsubscribe(HandlePeriodReferenceSet);
            eventAggregator.GetEvent<RelativePerformanceGridClickEvent>().Unsubscribe(HandleRelativePerformanceGridClickEvent);
        }
        #endregion
    }
}
