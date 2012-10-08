using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.ViewModel;
using GreenField.Common;
using GreenField.DataContracts;
using GreenField.Gadgets.Models;
using GreenField.ServiceCaller;

namespace GreenField.Gadgets.ViewModels
{
    /// <summary>
    /// View Model for Risk Index Exposures
    /// </summary>
    public class ViewModelRiskIndexExposures : NotificationObject
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
        private PortfolioSelectionData portfolioSelectionDataInfo;

        /// <summary>
        /// Private member containing the Key Value Pair
        /// </summary>
        private FilterSelectionData holdingDataFilter;

        /// <summary>
        /// Private member to store info about including or excluding cash securities
        /// </summary>
        private bool isExCashSecurity;

        /// <summary>
        /// Private member to store info about look thru enabled or not
        /// </summary>
        private bool lookThruEnabled;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="param">DashBoardGadgetParam</param>
        public ViewModelRiskIndexExposures(DashboardGadgetParam param)
        {
            eventAggregator = param.EventAggregator;
            dbInteractivity = param.DBInteractivity;
            logger = param.LoggerFacade;
            portfolioSelectionDataInfo = param.DashboardGadgetPayload.PortfolioSelectionData;
            isExCashSecurity = param.DashboardGadgetPayload.IsExCashSecurityData;
            EffectiveDate = param.DashboardGadgetPayload.EffectiveDate;
            holdingDataFilter = param.DashboardGadgetPayload.FilterSelectionData;
            lookThruEnabled = param.DashboardGadgetPayload.IsLookThruEnabled;

            if ((portfolioSelectionDataInfo != null) && (EffectiveDate != null) && holdingDataFilter != null && IsActive)
            {
                dbInteractivity.RetrieveRiskIndexExposuresData(portfolioSelectionDataInfo, Convert.ToDateTime(effectiveDateInfo), isExCashSecurity, lookThruEnabled, 
                                                        holdingDataFilter.Filtertype, holdingDataFilter.FilterValues, RetrieveRiskIndexExposuresDataCallbackMethod);
                BusyIndicatorStatus = true;
            }
            else if ((portfolioSelectionDataInfo != null) && (EffectiveDate != null) && holdingDataFilter == null && IsActive)
            {
                dbInteractivity.RetrieveRiskIndexExposuresData(portfolioSelectionDataInfo, Convert.ToDateTime(effectiveDateInfo), isExCashSecurity, lookThruEnabled,
                                                        "Show Everything", " ", RetrieveRiskIndexExposuresDataCallbackMethod);
                BusyIndicatorStatus = true;
            }

            if (eventAggregator != null)
            {
                eventAggregator.GetEvent<PortfolioReferenceSetEvent>().Subscribe(HandlePortfolioReferenceSet);
                eventAggregator.GetEvent<EffectiveDateReferenceSetEvent>().Subscribe(HandleEffectiveDateSet);
                eventAggregator.GetEvent<ExCashSecuritySetEvent>().Subscribe(HandleExCashSecuritySetEvent);
                eventAggregator.GetEvent<LookThruFilterReferenceSetEvent>().Subscribe(HandleLookThruReferenceSetEvent);
                eventAggregator.GetEvent<HoldingFilterReferenceSetEvent>().Subscribe(HandleFilterReferenceSetEvent);
            }
        } 
        #endregion

        #region Properties
        #region UI Fields
        /// <summary>
        /// property to contain effective date value from EffectiveDate Datepicker
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
        /// contains data to be displayed in this gadget
        /// </summary>
        private ObservableCollection<RiskIndexExposuresData> riskIndexExposuresInfo;
        public ObservableCollection<RiskIndexExposuresData> RiskIndexExposuresInfo
        {
            get { return riskIndexExposuresInfo; }
            set
            {
                if (riskIndexExposuresInfo != value)
                {
                    riskIndexExposuresInfo = value;
                    RaisePropertyChanged(() => this.RiskIndexExposuresInfo);
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
        /// property to conatin data to show in chart
        /// </summary>
        private ObservableCollection<RiskIndexExposuresChartData> riskIndexExposuresChartInfo;
        public ObservableCollection<RiskIndexExposuresChartData> RiskIndexExposuresChartInfo
        {
            get { return riskIndexExposuresChartInfo; }
            set 
            {
                if (riskIndexExposuresChartInfo != value)
                {
                    riskIndexExposuresChartInfo = value;
                    RaisePropertyChanged(() => RiskIndexExposuresChartInfo);
                }
            }
        }

        /// <summary>
        /// Minimum Value for X-Axis of Chart
        /// </summary>
        private decimal axisXMinValue;
        public decimal AxisXMinValue
        {
            get { return axisXMinValue; }
            set
            {
                axisXMinValue = value;
                this.RaisePropertyChanged(() => this.AxisXMinValue);
            }
        }

        /// <summary>
        /// Maximum Value for X-Axis of Chart
        /// </summary>
        private decimal axisXMaxValue;
        public decimal AxisXMaxValue
        {
            get { return axisXMaxValue; }
            set
            {
                axisXMaxValue = value;
                this.RaisePropertyChanged(() => this.AxisXMaxValue);
            }
        }

        /// <summary>
        /// Step size of XAxis of Chart
        /// </summary>
        private int axisXStep;
        public int AxisXStep
        {
            get { return axisXStep; }
            set{ axisXStep = value; }
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
                    if ((portfolioSelectionDataInfo != null) && (EffectiveDate != null) && holdingDataFilter != null && isActive)
                    {
                        dbInteractivity.RetrieveRiskIndexExposuresData(portfolioSelectionDataInfo, Convert.ToDateTime(effectiveDateInfo), isExCashSecurity, 
                            lookThruEnabled, holdingDataFilter.Filtertype, holdingDataFilter.FilterValues, RetrieveRiskIndexExposuresDataCallbackMethod);
                        BusyIndicatorStatus = true;
                    }
                    else if ((portfolioSelectionDataInfo != null) && (EffectiveDate != null) && holdingDataFilter == null && isActive)
                    {
                        dbInteractivity.RetrieveRiskIndexExposuresData(portfolioSelectionDataInfo, Convert.ToDateTime(effectiveDateInfo), isExCashSecurity, 
                            lookThruEnabled, "Show Everything", " ", RetrieveRiskIndexExposuresDataCallbackMethod);
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
                    if ((portfolioSelectionDataInfo != null) && (EffectiveDate != null) && holdingDataFilter != null && IsActive)
                    {
                        dbInteractivity.RetrieveRiskIndexExposuresData(portfolioSelectionDataInfo, Convert.ToDateTime(effectiveDateInfo), isExCashSecurity,
                            lookThruEnabled, holdingDataFilter.Filtertype, holdingDataFilter.FilterValues, RetrieveRiskIndexExposuresDataCallbackMethod);
                        BusyIndicatorStatus = true;
                    }
                    else if ((portfolioSelectionDataInfo != null) && (EffectiveDate != null) && holdingDataFilter == null && IsActive)
                    {
                        dbInteractivity.RetrieveRiskIndexExposuresData(portfolioSelectionDataInfo, Convert.ToDateTime(effectiveDateInfo), isExCashSecurity, 
                            lookThruEnabled, "Show Everything", " ", RetrieveRiskIndexExposuresDataCallbackMethod);
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
                    if ((portfolioSelectionDataInfo != null) && (EffectiveDate != null) && holdingDataFilter != null && IsActive)
                    {
                        dbInteractivity.RetrieveRiskIndexExposuresData(portfolioSelectionDataInfo, Convert.ToDateTime(effectiveDateInfo), isExCashSecurity,
                            lookThruEnabled, holdingDataFilter.Filtertype, holdingDataFilter.FilterValues, RetrieveRiskIndexExposuresDataCallbackMethod);
                        BusyIndicatorStatus = true;
                    }
                    else if ((portfolioSelectionDataInfo != null) && (EffectiveDate != null) && holdingDataFilter == null && IsActive)
                    {
                        dbInteractivity.RetrieveRiskIndexExposuresData(portfolioSelectionDataInfo, Convert.ToDateTime(effectiveDateInfo), isExCashSecurity,
                            lookThruEnabled, "Show Everything", " ", RetrieveRiskIndexExposuresDataCallbackMethod);
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
                if (isExCashSecurity != isExCashSec)
                {
                    isExCashSecurity = isExCashSec;
                    if ((portfolioSelectionDataInfo != null) && (EffectiveDate != null) && holdingDataFilter != null && IsActive)
                    {
                        dbInteractivity.RetrieveRiskIndexExposuresData(portfolioSelectionDataInfo, Convert.ToDateTime(effectiveDateInfo), isExCashSecurity,
                            lookThruEnabled, holdingDataFilter.Filtertype, holdingDataFilter.FilterValues, RetrieveRiskIndexExposuresDataCallbackMethod);
                        BusyIndicatorStatus = true;
                    }
                    else if ((portfolioSelectionDataInfo != null) && (EffectiveDate != null) && holdingDataFilter == null && IsActive)
                    {
                        dbInteractivity.RetrieveRiskIndexExposuresData(portfolioSelectionDataInfo, Convert.ToDateTime(effectiveDateInfo), isExCashSecurity,
                            lookThruEnabled, "Show Everything", " ", RetrieveRiskIndexExposuresDataCallbackMethod);
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
                lookThruEnabled = enableLookThru;

                if ((portfolioSelectionDataInfo != null) && (EffectiveDate != null) && holdingDataFilter != null && IsActive)
                {
                    dbInteractivity.RetrieveRiskIndexExposuresData(portfolioSelectionDataInfo, Convert.ToDateTime(effectiveDateInfo), isExCashSecurity,
                        lookThruEnabled, holdingDataFilter.Filtertype, holdingDataFilter.FilterValues, RetrieveRiskIndexExposuresDataCallbackMethod);
                    BusyIndicatorStatus = true;
                }
                else if ((portfolioSelectionDataInfo != null) && (EffectiveDate != null) && holdingDataFilter == null && IsActive)
                {
                    dbInteractivity.RetrieveRiskIndexExposuresData(portfolioSelectionDataInfo, Convert.ToDateTime(effectiveDateInfo), isExCashSecurity,
                        lookThruEnabled, "Show Everything", " ", RetrieveRiskIndexExposuresDataCallbackMethod);
                    BusyIndicatorStatus = true;
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
        /// Assigns UI Field Properties based on Holding Filter reference
        /// </summary>
        /// <param name="filterSelectionData">Key value pair consisting of the Filter Type and Filter Value selected by the user </param>
        public void HandleFilterReferenceSetEvent(FilterSelectionData filterSelectionData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (filterSelectionData != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, filterSelectionData, 1);
                    holdingDataFilter = filterSelectionData;
                    if (EffectiveDate != null && portfolioSelectionDataInfo != null && holdingDataFilter != null && IsActive)
                    {
                        dbInteractivity.RetrieveRiskIndexExposuresData(portfolioSelectionDataInfo, Convert.ToDateTime(effectiveDateInfo), isExCashSecurity, 
                            lookThruEnabled, holdingDataFilter.Filtertype, holdingDataFilter.FilterValues, RetrieveRiskIndexExposuresDataCallbackMethod);
                        BusyIndicatorStatus = true;
                    }
                    else if ((portfolioSelectionDataInfo != null) && (EffectiveDate != null) && holdingDataFilter == null && IsActive)
                    {
                        dbInteractivity.RetrieveRiskIndexExposuresData(portfolioSelectionDataInfo, Convert.ToDateTime(effectiveDateInfo), isExCashSecurity,
                            lookThruEnabled, "Show Everything", " ", RetrieveRiskIndexExposuresDataCallbackMethod);
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
        /// callback method for RetrieveRiskIndexExposuresData service call
        /// </summary>
        /// <param name="riskIndexExposuresData">RiskIndexExposuresData collection</param>
        private void RetrieveRiskIndexExposuresDataCallbackMethod(List<RiskIndexExposuresData> riskIndexExposuresData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (riskIndexExposuresData != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, riskIndexExposuresData, 1);
                    RiskIndexExposuresInfo = new ObservableCollection<RiskIndexExposuresData>(riskIndexExposuresData);                   
                    RiskIndexExposuresChartInfo = new ObservableCollection<RiskIndexExposuresChartData>();
                     
                     foreach (RiskIndexExposuresData item in RiskIndexExposuresInfo)
                     {                        
                         RiskIndexExposuresChartInfo.Add(new RiskIndexExposuresChartData()
                         {
                             EntityName = item.EntityType,
                             Descriptor = "Momentum",
                             Value = item.Momentum
                         });
                         RiskIndexExposuresChartInfo.Add(new RiskIndexExposuresChartData()
                         {
                             EntityName = item.EntityType,
                             Descriptor = "Volatility",
                             Value = item.Volatility
                         });
                         RiskIndexExposuresChartInfo.Add(new RiskIndexExposuresChartData()
                         {
                             EntityName = item.EntityType,
                             Descriptor = "Value",
                             Value = item.Value
                         });
                         RiskIndexExposuresChartInfo.Add(new RiskIndexExposuresChartData()
                         {
                             EntityName = item.EntityType,
                             Descriptor = "Size",
                             Value = item.Size
                         });
                         RiskIndexExposuresChartInfo.Add(new RiskIndexExposuresChartData()
                         {
                             EntityName = item.EntityType,
                             Descriptor = "SizeNonLinear",
                             Value = item.SizeNonLinear
                         });
                         RiskIndexExposuresChartInfo.Add(new RiskIndexExposuresChartData()
                         {
                             EntityName = item.EntityType,
                             Descriptor = "Growth",
                             Value = item.Growth
                         });
                         RiskIndexExposuresChartInfo.Add(new RiskIndexExposuresChartData()
                         {
                             EntityName = item.EntityType,
                             Descriptor = "Liquidity",
                             Value = item.Liquidity
                         });
                         RiskIndexExposuresChartInfo.Add(new RiskIndexExposuresChartData()
                         {
                             EntityName = item.EntityType,
                             Descriptor = "Leverage",
                             Value = item.Leverage
                         }); 
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
            eventAggregator.GetEvent<HoldingFilterReferenceSetEvent>().Unsubscribe(HandleFilterReferenceSetEvent);
        }
        #endregion
    }
}
