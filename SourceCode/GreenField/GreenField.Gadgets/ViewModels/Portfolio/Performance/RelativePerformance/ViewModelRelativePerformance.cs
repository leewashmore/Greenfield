using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
    /// View model for ViewRelativePerformance class
    /// </summary>
    public class ViewModelRelativePerformance : NotificationObject
    {
        #region Fields
        //MEF Singletons
        public IEventAggregator eventAggregator;
        public IDBInteractivity dbInteractivity;
        public ILoggerFacade logger;

        //Selection Data
        public PortfolioSelectionData portfolioSelectionDataInfo;

        //Gadget Data
        private List<RelativePerformanceSectorData> relativePerformanceSectorInfo;
        private List<RelativePerformanceData> relativePerformanceInfo;
        
        #endregion

        #region Properties
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
        /// IsActive is true when parent control is displayed on UI
        /// </summary>
        private bool isActive;
        public bool IsActive
        {
            get { return isActive; }
            set
            {
                if (isActive != value)
                {
                    isActive = value;
                    if (effectiveDateInfo != null && portfolioSelectionDataInfo != null && Period != null && isActive)
                    {
                        dbInteractivity.RetrieveRelativePerformanceSectorData(portfolioSelectionDataInfo, Convert.ToDateTime(effectiveDateInfo),
                            RetrieveRelativePerformanceSectorDataCallbackMethod);
                        BusyIndicatorStatus = true;
                    }
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

        /// <summary>
        /// Contains security level data to be displayed in grids shown on toggling
        /// </summary>
        private ObservableCollection<RelativePerformanceSecurityData> securityDetails;
        public ObservableCollection<RelativePerformanceSecurityData> SecurityDetails
        {
            get { return securityDetails; }
            set
            {
                if (securityDetails != value)
                {
                    securityDetails = value;
                    RaisePropertyChanged(() => SecurityDetails);
                }
            }
        }

        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="param">DashboardGadgetParam</param>
        public ViewModelRelativePerformance(DashboardGadgetParam param)
        {
            //MEF singleton initialization
            eventAggregator = param.EventAggregator;
            dbInteractivity = param.DBInteractivity;
            logger = param.LoggerFacade;

            //selection data initialization
            portfolioSelectionDataInfo = param.DashboardGadgetPayload.PortfolioSelectionData;
            EffectiveDate = param.DashboardGadgetPayload.EffectiveDate;
            periodInfo = param.DashboardGadgetPayload.PeriodSelectionData;

            //service call to retrieve sector data relating fund selection data/ benchmark selection data and effective date
            if (effectiveDateInfo != null && portfolioSelectionDataInfo != null && Period != null && IsActive)
            {
                dbInteractivity.RetrieveRelativePerformanceSectorData(portfolioSelectionDataInfo, Convert.ToDateTime(effectiveDateInfo),
                    RetrieveRelativePerformanceSectorDataCallbackMethod);
                BusyIndicatorStatus = true;
            }

            if (eventAggregator != null)
            {
                eventAggregator.GetEvent<PortfolioReferenceSetEvent>().Subscribe(HandlePortfolioReferenceSet);
                eventAggregator.GetEvent<EffectiveDateReferenceSetEvent>().Subscribe(HandleEffectiveDateSet);
                eventAggregator.GetEvent<PeriodReferenceSetEvent>().Subscribe(HandlePeriodReferenceSet);
                eventAggregator.GetEvent<RelativePerformanceGridCountrySectorClickEvent>().Subscribe(HandleRelativePerformanceGridCountrySectorClickEvent);
            }
        } 
        #endregion        

        #region Events
        /// <summary>
        /// Event handling for relative performance gadget grid building
        /// </summary>
        public event RelativePerformanceGridBuildEventHandler RelativePerformanceGridBuildEvent;

        /// <summary>
        /// Event handling for building grid shown on click over sector name in relative performance gadget grid  
        /// </summary>
        public event RelativePerformanceToggledSectorGridBuildEventHandler RelativePerformanceToggledSectorGridBuildEvent;
              
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
                    if (EffectiveDate != null && portfolioSelectionDataInfo != null && periodInfo != null && IsActive)
                    {
                        dbInteractivity.RetrieveRelativePerformanceSectorData(portfolioSelectionDataInfo, Convert.ToDateTime(effectiveDateInfo), 
                            RetrieveRelativePerformanceSectorDataCallbackMethod);
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
        /// <param name="effectiveDate"></param>
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
                    if (EffectiveDate != null && portfolioSelectionDataInfo != null && periodInfo != null && IsActive)
                    {
                        dbInteractivity.RetrieveRelativePerformanceSectorData(portfolioSelectionDataInfo,Convert.ToDateTime(effectiveDateInfo),
                            RetrieveRelativePerformanceSectorDataCallbackMethod);
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
                    if (EffectiveDate != null && portfolioSelectionDataInfo != null && periodInfo != null && IsActive)
                    {
                        dbInteractivity.RetrieveRelativePerformanceSectorData(portfolioSelectionDataInfo, Convert.ToDateTime(effectiveDateInfo),
                            RetrieveRelativePerformanceSectorDataCallbackMethod);
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
        /// Event Handler to subscribed event 'RelativePerformanceGridCountrySectorClickEvent'
        /// </summary>
        /// <param name="relativePerformanceGridCellData">RelativePerformanceGridCellData</param>
        public void HandleRelativePerformanceGridCountrySectorClickEvent(RelativePerformanceGridCellData relativePerformanceGridCellData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (relativePerformanceGridCellData != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, relativePerformanceGridCellData, 1);
                    if (EffectiveDate != null && portfolioSelectionDataInfo != null)
                    {
                        if (relativePerformanceGridCellData.SectorID == null)
                        {
                            dbInteractivity.RetrieveRelativePerformanceSecurityData(portfolioSelectionDataInfo, Convert.ToDateTime(effectiveDateInfo), periodInfo,
                           RetrieveRelativePerformanceSecurityDataCallbackMethod, relativePerformanceGridCellData.CountryID, relativePerformanceGridCellData.SectorID);
                        }
                        else if (relativePerformanceGridCellData.CountryID == null)
                        {
                          dbInteractivity.RetrieveRelativePerformanceSecurityData(portfolioSelectionDataInfo, Convert.ToDateTime(effectiveDateInfo), periodInfo,
                          RetrieveRelativePerformanceSecurityDataCallbackMethod, relativePerformanceGridCellData.CountryID, relativePerformanceGridCellData.SectorID);
                        }
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

        #endregion

        #region Callback Methods
        /// <summary>
        /// Callback method for RetrieveRelativePerformanceSectorData Service call
        /// </summary>
        /// <param name="result">RelativePerformanceSectorData Collection</param>
        private void RetrieveRelativePerformanceSectorDataCallbackMethod(List<RelativePerformanceSectorData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result, 1);
                    relativePerformanceSectorInfo = result;
                    dbInteractivity.RetrieveRelativePerformanceData(portfolioSelectionDataInfo, Convert.ToDateTime(effectiveDateInfo),periodInfo,
                        RetrieveRelativePerformanceDataCallbackMethod);                    
                }
                else
                {
                    BusyIndicatorStatus = false;
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
        /// Callback method for RetrieveRelativePerformanceData Service call
        /// </summary>
        /// <param name="result">RelativePerformanceData Collection</param>
        private void RetrieveRelativePerformanceDataCallbackMethod(List<RelativePerformanceData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result, 1);
                    relativePerformanceInfo = result;

                    dbInteractivity.RetrieveRelativePerformanceSecurityData(portfolioSelectionDataInfo, Convert.ToDateTime(effectiveDateInfo)
                        , periodInfo, RetrieveRelativePerformanceSecurityDataTopAlphaCallbackMethod);
                }
                else
                {
                    BusyIndicatorStatus = false;
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
        /// Callback method for RetrieveRelativePerformanceSecurityData Service call
        /// </summary>
        /// <param name="result">RelativePerformanceSecurityData Collection</param>
        public void RetrieveRelativePerformanceSecurityDataTopAlphaCallbackMethod(List<RelativePerformanceSecurityData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    RelativePerformanceGridBuildEvent.Invoke(new RelativePerformanceGridBuildEventArgs()
                    {
                        RelativePerformanceSectorInfo = relativePerformanceSectorInfo,
                        RelativePerformanceInfo = relativePerformanceInfo,
                        RelativePerformanceSecurityInfo = result
                    });
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
                    SecurityDetails = new ObservableCollection<RelativePerformanceSecurityData>(result);
                    RelativePerformanceToggledSectorGridBuildEvent.Invoke(new RelativePerformanceToggledSectorGridBuildEventArgs()
                    {
                        RelativePerformanceCountryNameInfo = SecurityDetails.OrderBy(r => r.SecurityCountryId).Select(r => r.SecurityCountryId).Distinct().ToList(),
                        RelativePerformanceSecurityInfo = SecurityDetails.ToList()
                    });
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
            eventAggregator.GetEvent<RelativePerformanceGridCountrySectorClickEvent>().Unsubscribe(HandleRelativePerformanceGridCountrySectorClickEvent);
        }
        #endregion
    }
}
