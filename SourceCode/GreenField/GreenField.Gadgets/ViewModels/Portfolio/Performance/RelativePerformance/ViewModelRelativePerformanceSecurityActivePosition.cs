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
    /// class for model for RelativePerformanceSecurityActivePosition
    /// </summary>
    public class ViewModelRelativePerformanceSecurityActivePosition : NotificationObject
    {
        #region Fields
        //MEF Singletons
        private IEventAggregator eventAggregator;
        private IDBInteractivity dbInteractivity;
        private ILoggerFacade logger;

        //Selection Data
       PortfolioSelectionData portfolioSelectionDataInfo;

       //to check that grid is not re populated for same values when Relative Performance Matrix is clicked
        RelativePerformanceGridCellData checkValue = new RelativePerformanceGridCellData();
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
       /// <param name="param">DashboardGadgetParam</param>
        public ViewModelRelativePerformanceSecurityActivePosition(DashboardGadgetParam param)
        {
            eventAggregator = param.EventAggregator;
            dbInteractivity = param.DBInteractivity;
            logger = param.LoggerFacade;

           portfolioSelectionDataInfo = param.DashboardGadgetPayload.PortfolioSelectionData;
           EffectiveDate = param.DashboardGadgetPayload.EffectiveDate;
           Period = param.DashboardGadgetPayload.PeriodSelectionData;

           if (EffectiveDate != null && portfolioSelectionDataInfo != null && Period != null && IsActive)
           {
               dbInteractivity.RetrieveRelativePerformanceSecurityActivePositionData(portfolioSelectionDataInfo, Convert.ToDateTime(effectiveDateInfo),
                   periodInfo, RetrieveRelativePerformanceSecurityActivePositionDataCallbackMethod);
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
        /// data received from view
        /// </summary>
        private List<RelativePerformanceActivePositionData> relativePerformanceActivePositionOrigInfo;
        public List<RelativePerformanceActivePositionData> RelativePerformanceActivePositionOrigInfo
        {
            get
            {
                if (relativePerformanceActivePositionOrigInfo == null)
                {
                    relativePerformanceActivePositionOrigInfo = new List<RelativePerformanceActivePositionData>();
                }
                return relativePerformanceActivePositionOrigInfo;
            }
            set
            {
                if (relativePerformanceActivePositionOrigInfo != value)
                {
                    relativePerformanceActivePositionOrigInfo = value;
                    UpdateRelativePerformanceActivePositionInfo(value, Convert.ToBoolean(DisplayIssuerIsChecked));
                }
            }
        }

        /// <summary>
        /// Contains data to be displayed in the gadget
        /// </summary>
        private ObservableCollection<RelativePerformanceActivePositionData> relativePerformanceActivePositionInfo;
        public ObservableCollection<RelativePerformanceActivePositionData> RelativePerformanceActivePositionInfo
        {
            get
            {
                if (relativePerformanceActivePositionInfo == null)
                {
                    relativePerformanceActivePositionInfo = new ObservableCollection<RelativePerformanceActivePositionData>();
                }
                return relativePerformanceActivePositionInfo; 
            }
            set
            {
                if (relativePerformanceActivePositionInfo != value)
                {
                    relativePerformanceActivePositionInfo = value;
                    RaisePropertyChanged(() => this.RelativePerformanceActivePositionInfo);
                }
            }
        }

        /// <summary>
        /// contains value for display issuer checkbox
        /// </summary>
        private bool? displayIssuerIsChecked = false;
        public bool? DisplayIssuerIsChecked
        {
            get { return displayIssuerIsChecked; }
            set
            {
                if (displayIssuerIsChecked != value)
                {
                    displayIssuerIsChecked = value;
                    RaisePropertyChanged(() => this.DisplayIssuerIsChecked);
                    UpdateRelativePerformanceActivePositionInfo(RelativePerformanceActivePositionOrigInfo, Convert.ToBoolean(value));
                }
            }
        }        

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
                        dbInteractivity.RetrieveRelativePerformanceSecurityActivePositionData(portfolioSelectionDataInfo, Convert.ToDateTime(effectiveDateInfo), 
                            periodInfo, RetrieveRelativePerformanceSecurityActivePositionDataCallbackMethod);
                        BusyIndicatorStatus = true;
                    }
                }
            }
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
                    if (EffectiveDate != null && portfolioSelectionDataInfo != null && Period != null && IsActive)
                    {
                        dbInteractivity.RetrieveRelativePerformanceSecurityActivePositionData(portfolioSelectionDataInfo, Convert.ToDateTime(effectiveDateInfo), 
                            periodInfo, RetrieveRelativePerformanceSecurityActivePositionDataCallbackMethod);
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
                    if (EffectiveDate != null && portfolioSelectionDataInfo != null && Period != null && IsActive)
                    {
                        dbInteractivity.RetrieveRelativePerformanceSecurityActivePositionData(portfolioSelectionDataInfo, Convert.ToDateTime(effectiveDateInfo),
                            periodInfo, RetrieveRelativePerformanceSecurityActivePositionDataCallbackMethod);
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
                       dbInteractivity.RetrieveRelativePerformanceSecurityActivePositionData(portfolioSelectionDataInfo, Convert.ToDateTime(effectiveDateInfo),
                           periodInfo, RetrieveRelativePerformanceSecurityActivePositionDataCallbackMethod);
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
        public void HandleRelativePerformanceGridClickEvent(RelativePerformanceGridCellData filter)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (filter != null)
                {
                    if (checkValue.CountryID != filter.CountryID || checkValue.SectorID != filter.SectorID)
                    {
                        checkValue = filter;
                        Logging.LogMethodParameter(logger, methodNamespace, filter, 1);
                        if (effectiveDateInfo != null && portfolioSelectionDataInfo != null && Period != null && IsActive)
                        {
                            dbInteractivity.RetrieveRelativePerformanceSecurityActivePositionData(portfolioSelectionDataInfo, Convert.ToDateTime(effectiveDateInfo),
                                periodInfo, RetrieveRelativePerformanceSecurityActivePositionDataCallbackMethod, filter.CountryID, filter.SectorID);
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
        /// Callback method for RetrieveRelativePerformanceSecurityActivePositionData Service call
        /// </summary>
        /// <param name="result">RelativePerformanceActivePositionData Collection</param>
        private void RetrieveRelativePerformanceSecurityActivePositionDataCallbackMethod(List<RelativePerformanceActivePositionData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result, 1);
                    RelativePerformanceActivePositionOrigInfo = result;
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

        #region Helper Method
        /// <summary>
        /// update display data based on diaplsy issuer checkbox
        /// </summary>
        /// <param name="value"></param>
        /// <param name="issuerFilter"></param>
        private void UpdateRelativePerformanceActivePositionInfo(List<RelativePerformanceActivePositionData> value, bool issuerFilter = false)
        {
            if (issuerFilter)
            {
                List<RelativePerformanceActivePositionData> groupedData = new List<RelativePerformanceActivePositionData>();
                List<String> distinctIssuers = value.Select(record => record.EntityGroup).Distinct().ToList();
                foreach (String item in distinctIssuers)
                {
                    Decimal? aggMarketValue = 0;
                    Decimal? aggFundWeight = 0;
                    Decimal? aggBenchmarkWeight = 0;

                    foreach (RelativePerformanceActivePositionData data in value.Where(g => g.EntityGroup == item))
                    {
                        aggMarketValue += data.MarketValue == null ? 0 : data.MarketValue;
                        aggFundWeight += data.FundWeight == null ? 0 : data.FundWeight;
                        aggBenchmarkWeight += data.BenchmarkWeight == null ? 0 : data.BenchmarkWeight;
                    }

                    groupedData.Add(new RelativePerformanceActivePositionData()
                    {
                        Entity = item,
                        EntityGroup = null,
                        MarketValue = aggMarketValue,
                        FundWeight = aggFundWeight,
                        BenchmarkWeight = aggBenchmarkWeight,
                        ActivePosition = aggFundWeight - aggBenchmarkWeight
                    });

                    RelativePerformanceActivePositionInfo = new ObservableCollection<RelativePerformanceActivePositionData>(groupedData);
                }
            }
            else
            {
                RelativePerformanceActivePositionInfo = new ObservableCollection<RelativePerformanceActivePositionData>(value);
            }
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
