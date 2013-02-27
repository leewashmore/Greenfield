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
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Logging;
using GreenField.ServiceCaller.SecurityReferenceDefinitions;
using GreenField.Common;
using GreenField.ServiceCaller.BenchmarkHoldingsDefinitions;
using GreenField.Gadgets.Models;
using GreenField.DataContracts;
using GreenField.Web.Helpers;
using GreenField.ServiceCaller;

namespace GreenField.Gadgets.ViewModels
{
    /// <summary>
    /// Class that provides the interaction of the view model with the Service caller and the View.
    /// </summary>
    public class ViewModelAttribution : NotificationObject
    {
        #region PrivateMembers
        /// <summary>
        /// private member object of the IEventAggregator for event aggregation
        /// </summary>
        private IEventAggregator eventAggregator;

        /// <summary>
        /// private member object of the IDBInteractivity for interaction with the Service Caller
        /// </summary>
        private IDBInteractivity dbInteractivity;

        /// <summary>
        /// private member object of ILoggerFacade for logging
        /// </summary>
        private ILoggerFacade logger;

        /// <summary>
        /// private member object of the PortfolioSelectionData class for storing Fund Selection Data
        /// </summary>
        private PortfolioSelectionData portfolioSelectionData;

        /// <summary>
        /// Stores Effective Date selected by the user
        /// </summary>
        private DateTime? effectiveDate;
        /// <summary>
        /// stores Node Name Filter selected by the user 
        /// </summary>
        private String nodeName;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor of the class that initializes various objects
        /// </summary>
        /// <param name="param">MEF Eventaggrigator instance</param>
        public ViewModelAttribution(DashboardGadgetParam param)
        {
            effectiveDate = param.DashboardGadgetPayload.EffectiveDate;
            dbInteractivity = param.DBInteractivity;
            logger = param.LoggerFacade;
            selectedPeriod = param.DashboardGadgetPayload.PeriodSelectionData;
            eventAggregator = param.EventAggregator;
            nodeName = param.DashboardGadgetPayload.NodeNameSelectionData;
            portfolioSelectionData = param.DashboardGadgetPayload.PortfolioSelectionData;
            if (effectiveDate != null && portfolioSelectionData != null && selectedPeriod != null && IsActive && nodeName != null)
            {
                dbInteractivity.RetrieveAttributionData(portfolioSelectionData, Convert.ToDateTime(effectiveDate), nodeName, RetrieveAttributionDataCallBackMethod);
            }
            if (eventAggregator != null)
            {
                eventAggregator.GetEvent<PortfolioReferenceSetEvent>().Subscribe(HandleFundReferenceSet, false);
                eventAggregator.GetEvent<EffectiveDateReferenceSetEvent>().Subscribe(HandleEffectiveDateSet, false);
                eventAggregator.GetEvent<PeriodReferenceSetEvent>().Subscribe(HandlePeriodReferenceSet, false);
                eventAggregator.GetEvent<NodeNameReferenceSetEvent>().Subscribe(HandleNodeNameReferenceSet, false);
            }
        }
        #endregion

        #region Properties
        #region UI Fields

        /// <summary>
        /// Collection that contains all the data for propertyName particular portfolio and date
        /// </summary>
        private List<AttributionData> attributionDataInfo;
        public List<AttributionData> AttributionDataInfo
        {
            get
            {
                if (attributionDataInfo == null)
                    attributionDataInfo = new List<AttributionData>();
                return attributionDataInfo;
            }
            set
            {
                if (attributionDataInfo != value)
                {
                    attributionDataInfo = value;
                    RaisePropertyChanged(() => this.AttributionDataInfo);
                }
            }
        }
        /// <summary>
        /// The Period selected by the user.
        /// </summary>
        private String selectedPeriod;
        public String SelectedPeriod
        {
            get
            { return selectedPeriod; }

            set
            {
                selectedPeriod = value;
                RaisePropertyChanged(() => this.SelectedPeriod);
                if (AttributionDataInfo != null && AttributionDataInfo.Count > 0)
                {
                    bool isValidData;
                    switch (value)
                    {
                        case "1D":
                            List<PeriodAttributeData> resultd = new List<PeriodAttributeData>();
                            for (int i = 0; i < AttributionDataInfo.Count; i++)
                            {
                                if (AttributionDataInfo[i].PorRcAvgWgt1d == 0 && AttributionDataInfo[i].Bm1RcAvgWgt1d == 0)
                                {
                                    continue;
                                }
                                if (AttributionDataInfo[i].PorRcAvgWgt1d == null)
                                {
                                    AttributionDataInfo[i].PorRcAvgWgt1d = 0;
                                }
                                if (AttributionDataInfo[i].Bm1RcAvgWgt1d == null)
                                {
                                    AttributionDataInfo[i].Bm1RcAvgWgt1d = 0;
                                }
                                PeriodAttributeData entry = new PeriodAttributeData();
                                entry.Country = AttributionDataInfo[i].Country;
                                entry.CountryName = AttributionDataInfo[i].CountryName;
                                entry.BenchmarkWeight = AttributionDataInfo[i].Bm1RcAvgWgt1d;
                                entry.PortfolioWeight = AttributionDataInfo[i].PorRcAvgWgt1d;
                                entry.PortfolioReturn = AttributionDataInfo[i].FPorAshRcCtn1d;
                                entry.BenchmarkReturn = AttributionDataInfo[i].BM1_RC_TWR_1D;
                                entry.AssetAllocation = AttributionDataInfo[i].FBm1AshAssetAlloc1d;
                                entry.StockSelectionTotal = AttributionDataInfo[i].FBm1AshSecSelec1d;
                                entry.TotalValueAdd = entry.AssetAllocation + entry.StockSelectionTotal;
                                resultd.Add(entry);
                            }
                            PeriodAttributionInfo = resultd;
                            if (null != AttributionDataLoadedEvent)
                            {
                                AttributionDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
                            }
                            break;
                        case "1W":
                            List<PeriodAttributeData> result = new List<PeriodAttributeData>();
                            for (int i = 0; i < AttributionDataInfo.Count; i++)
                            {
                                if (AttributionDataInfo[i].PorRcAvgWgt1w == 0 && AttributionDataInfo[i].Bm1RcAvgWgt1w == 0)
                                {
                                    continue;
                                }
                                if (AttributionDataInfo[i].PorRcAvgWgt1w == null)
                                {
                                    AttributionDataInfo[i].PorRcAvgWgt1w = 0;
                                }
                                if (AttributionDataInfo[i].Bm1RcAvgWgt1w == null)
                                {
                                    AttributionDataInfo[i].Bm1RcAvgWgt1w = 0;
                                }
                                PeriodAttributeData entry = new PeriodAttributeData();
                                entry.Country = AttributionDataInfo[i].Country;
                                entry.CountryName = AttributionDataInfo[i].CountryName;
                                entry.BenchmarkWeight = AttributionDataInfo[i].Bm1RcAvgWgt1w;
                                entry.PortfolioWeight = AttributionDataInfo[i].PorRcAvgWgt1w;
                                entry.PortfolioReturn = AttributionDataInfo[i].FPorAshRcCtn1w;
                                entry.BenchmarkReturn = AttributionDataInfo[i].BM1_RC_TWR_1W;
                                entry.AssetAllocation = AttributionDataInfo[i].FBm1AshAssetAlloc1w;
                                entry.StockSelectionTotal = AttributionDataInfo[i].FBm1AshSecSelec1w;
                                entry.TotalValueAdd = entry.AssetAllocation + entry.StockSelectionTotal;
                                result.Add(entry);
                            }
                            PeriodAttributionInfo = result;
                            if (null != AttributionDataLoadedEvent)
                            {
                                AttributionDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
                            }
                            break;
                        case "MTD":
                            List<PeriodAttributeData> resultMtd = new List<PeriodAttributeData>();
                            isValidData = InceptionDateCheck(selectedPeriod, Convert.ToDateTime(AttributionDataInfo[0].EffectiveDate), AttributionDataInfo);
                            if (!isValidData)
                            {
                                PeriodAttributionInfo = new List<PeriodAttributeData>();
                            }
                            else
                            {
                                for (int i = 0; i < AttributionDataInfo.Count; i++)
                                {
                                    if (AttributionDataInfo[i].PorRcAvgWgtMtd == 0 && AttributionDataInfo[i].Bm1RcAvgWgtMtd == 0)
                                    {
                                        continue;
                                    }
                                    if (AttributionDataInfo[i].PorRcAvgWgtMtd == null)
                                    {
                                        AttributionDataInfo[i].PorRcAvgWgtMtd = 0;
                                    }
                                    if (AttributionDataInfo[i].Bm1RcAvgWgtMtd == null)
                                    {
                                        AttributionDataInfo[i].Bm1RcAvgWgtMtd = 0;
                                    }
                                    PeriodAttributeData entry = new PeriodAttributeData();
                                    entry.Country = AttributionDataInfo[i].Country;
                                    entry.CountryName = AttributionDataInfo[i].CountryName;
                                    entry.BenchmarkWeight = AttributionDataInfo[i].Bm1RcAvgWgtMtd;
                                    entry.PortfolioWeight = AttributionDataInfo[i].PorRcAvgWgtMtd;
                                    entry.PortfolioReturn = AttributionDataInfo[i].FPorAshRcCtnMtd;
                                    entry.BenchmarkReturn = AttributionDataInfo[i].BM1_RC_TWR_MTD;
                                    entry.AssetAllocation = AttributionDataInfo[i].FBm1AshAssetAllocMtd;
                                    entry.StockSelectionTotal = AttributionDataInfo[i].FBm1AshSecSelecMtd;
                                    entry.TotalValueAdd = entry.AssetAllocation + entry.StockSelectionTotal;
                                    resultMtd.Add(entry);
                                }
                                PeriodAttributionInfo = resultMtd;
                            }
                            if (null != AttributionDataLoadedEvent)
                            {
                                AttributionDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
                            }
                            break;
                        case "QTD":
                            List<PeriodAttributeData> resultQtd = new List<PeriodAttributeData>();
                            isValidData = InceptionDateCheck(selectedPeriod, Convert.ToDateTime(AttributionDataInfo[0].EffectiveDate), AttributionDataInfo);
                            if (!isValidData)
                            {
                                PeriodAttributionInfo = new List<PeriodAttributeData>();
                            }
                            else
                            {
                                for (int i = 0; i < AttributionDataInfo.Count; i++)
                                {
                                    if (AttributionDataInfo[i].PorRcAvgWgtQtd == 0 && AttributionDataInfo[i].Bm1RcAvgWgtQtd == 0)
                                    {
                                        continue;
                                    }
                                    if (AttributionDataInfo[i].PorRcAvgWgtQtd == null)
                                    {
                                        AttributionDataInfo[i].PorRcAvgWgtQtd = 0;
                                    }
                                    if (AttributionDataInfo[i].Bm1RcAvgWgtQtd == null)
                                    {
                                        AttributionDataInfo[i].Bm1RcAvgWgtQtd = 0;
                                    }
                                    PeriodAttributeData entry = new PeriodAttributeData();
                                    entry.Country = AttributionDataInfo[i].Country;
                                    entry.CountryName = AttributionDataInfo[i].CountryName;
                                    entry.BenchmarkWeight = AttributionDataInfo[i].Bm1RcAvgWgtQtd;
                                    entry.PortfolioWeight = AttributionDataInfo[i].PorRcAvgWgtQtd;
                                    entry.PortfolioReturn = AttributionDataInfo[i].FPorAshRcCtnQtd;
                                    entry.BenchmarkReturn = AttributionDataInfo[i].BM1_RC_TWR_QTD;
                                    entry.AssetAllocation = AttributionDataInfo[i].FBm1AshAssetAllocQtd;
                                    entry.StockSelectionTotal = AttributionDataInfo[i].FBm1AshSecSelecQtd;
                                    entry.TotalValueAdd = entry.AssetAllocation + entry.StockSelectionTotal;
                                    resultQtd.Add(entry);
                                }
                                PeriodAttributionInfo = resultQtd;
                            }
                            if (null != AttributionDataLoadedEvent)
                            {
                                AttributionDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
                            }
                            break;
                        case "YTD":
                            List<PeriodAttributeData> resultYTD = new List<PeriodAttributeData>();
                            isValidData = InceptionDateCheck(selectedPeriod, Convert.ToDateTime(AttributionDataInfo[0].EffectiveDate), AttributionDataInfo);
                            if (!isValidData)
                            {
                                PeriodAttributionInfo = new List<PeriodAttributeData>();
                            }
                            else
                            {
                                for (int i = 0; i < AttributionDataInfo.Count; i++)
                                {
                                    if (AttributionDataInfo[i].PorRcAvgWgtYtd == 0 && AttributionDataInfo[i].Bm1RcAvgWgtYtd == 0)
                                    {
                                        continue;
                                    }
                                    if (AttributionDataInfo[i].PorRcAvgWgtYtd == null)
                                    {
                                        AttributionDataInfo[i].PorRcAvgWgtYtd = 0;
                                    }
                                    if (AttributionDataInfo[i].Bm1RcAvgWgtYtd == null)
                                    {
                                        AttributionDataInfo[i].Bm1RcAvgWgtYtd = 0;
                                    }
                                    PeriodAttributeData entry = new PeriodAttributeData();
                                    entry.Country = AttributionDataInfo[i].Country;
                                    entry.CountryName = AttributionDataInfo[i].CountryName;
                                    entry.BenchmarkWeight = AttributionDataInfo[i].Bm1RcAvgWgtYtd;
                                    entry.PortfolioWeight = AttributionDataInfo[i].PorRcAvgWgtYtd;
                                    entry.PortfolioReturn = AttributionDataInfo[i].FPorAshRcCtnYtd;
                                    entry.BenchmarkReturn = AttributionDataInfo[i].BM1_RC_TWR_YTD;
                                    entry.AssetAllocation = AttributionDataInfo[i].FBm1AshAssetAllocYtd;
                                    entry.StockSelectionTotal = AttributionDataInfo[i].FBm1AshSecSelecYtd;
                                    entry.TotalValueAdd = entry.AssetAllocation + entry.StockSelectionTotal;
                                    resultYTD.Add(entry);
                                }
                                PeriodAttributionInfo = resultYTD;
                            }
                            if (null != AttributionDataLoadedEvent)
                            {
                                AttributionDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
                            }
                            break;
                        case "1Y":
                            List<PeriodAttributeData> result1Y = new List<PeriodAttributeData>();
                            isValidData = InceptionDateCheck(selectedPeriod, Convert.ToDateTime(AttributionDataInfo[0].EffectiveDate), AttributionDataInfo);
                            if (!isValidData)
                            {
                                PeriodAttributionInfo = new List<PeriodAttributeData>();
                            }
                            else
                            {
                                for (int i = 0; i < AttributionDataInfo.Count; i++)
                                {
                                    if (AttributionDataInfo[i].PorRcAvgWgt1y == 0 && AttributionDataInfo[i].Bm1RcAvgWgt1y == 0)
                                    {
                                        continue;
                                    }
                                    if (AttributionDataInfo[i].PorRcAvgWgt1y == null)
                                    {
                                        AttributionDataInfo[i].PorRcAvgWgt1y = 0;
                                    }
                                    if (AttributionDataInfo[i].Bm1RcAvgWgt1y == null)
                                    {
                                        AttributionDataInfo[i].Bm1RcAvgWgt1y = 0;
                                    }
                                    PeriodAttributeData entry = new PeriodAttributeData();
                                    entry.Country = AttributionDataInfo[i].Country;
                                    entry.CountryName = AttributionDataInfo[i].CountryName;
                                    entry.BenchmarkWeight = AttributionDataInfo[i].Bm1RcAvgWgt1y;
                                    entry.PortfolioWeight = AttributionDataInfo[i].PorRcAvgWgt1y;
                                    entry.PortfolioReturn = AttributionDataInfo[i].FPorAshRcCtn1y;
                                    entry.BenchmarkReturn = AttributionDataInfo[i].BM1_RC_TWR_1Y;
                                    entry.AssetAllocation = AttributionDataInfo[i].FBm1AshAssetAlloc1y;
                                    entry.StockSelectionTotal = AttributionDataInfo[i].FBm1AshSecSelec1y;
                                    entry.TotalValueAdd = entry.AssetAllocation + entry.StockSelectionTotal;
                                    result1Y.Add(entry);
                                }
                                PeriodAttributionInfo = result1Y;
                            }
                            if (null != AttributionDataLoadedEvent)
                            {
                                AttributionDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
                            }
                            break;
                        default:
                            List<PeriodAttributeData> result10Y = new List<PeriodAttributeData>();
                            PeriodAttributionInfo = result10Y;
                            if (null != AttributionDataLoadedEvent)
                            {
                                AttributionDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
                            }
                            break;
                    }
                }
                else
                {
                    PeriodAttributionInfo = new List<PeriodAttributeData>();
                    if (null != AttributionDataLoadedEvent)
                    {
                        AttributionDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
                    }
                }
            }
        }

        /// <summary>
        /// Collection binded to the Attribution Grid
        /// </summary>
        private List<PeriodAttributeData> periodAttributionInfo;
        public List<PeriodAttributeData> PeriodAttributionInfo
        {
            get
            {
                if (periodAttributionInfo == null)
                {
                    periodAttributionInfo = new List<PeriodAttributeData>();
                }
                return periodAttributionInfo;
            }
            set
            {
                if (periodAttributionInfo != value)
                {
                    periodAttributionInfo = value;
                    RaisePropertyChanged(() => this.PeriodAttributionInfo);
                }
            }
        }

        #endregion

        private bool isActive;
        /// <summary>
        /// IsActive is true when parent control is displayed on UI
        /// </summary>
        public bool IsActive
        {
            get
            { return isActive; }
            set
            {
                isActive = value;
                if (portfolioSelectionData != null && effectiveDate != null && selectedPeriod != null && isActive && nodeName != null)
                {
                    BeginWebServiceCall(portfolioSelectionData, Convert.ToDateTime(effectiveDate), nodeName);
                }
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Event for the notification of Data Load Completion
        /// </summary>
        public event DataRetrievalProgressIndicatorEventHandler AttributionDataLoadedEvent;
        #endregion

        #region Event Handlers
        /// <summary>
        /// Assigns UI Field Properties based on Fund reference
        /// </summary>
        /// <param name="portSelectionData">Object of PortfolioSelectionData class containg the Fund Selection Data </param>
        public void HandleFundReferenceSet(PortfolioSelectionData portSelectionData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (portSelectionData != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, portSelectionData, 1);
                    portfolioSelectionData = portSelectionData;
                    if (portSelectionData != null && effectiveDate != null && selectedPeriod != null && IsActive && nodeName != null)
                    {
                        BeginWebServiceCall(portfolioSelectionData, Convert.ToDateTime(effectiveDate), nodeName);
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
        /// Assigns UI Field Properties based on Effective Date
        /// </summary>
        /// <param name="effectDate">Effective Date selected by the user</param>
        public void HandleEffectiveDateSet(DateTime effectDate)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (effectDate != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, effectDate, 1);
                    effectiveDate = effectDate;
                    if (portfolioSelectionData != null && effectiveDate != null && selectedPeriod != null && IsActive && nodeName != null)
                    {
                        BeginWebServiceCall(portfolioSelectionData, Convert.ToDateTime(effectiveDate), nodeName);
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
        /// Assigns UI Field Properties based on Period
        /// </summary>
        /// <param name="selectedPeriodType">Period selected by the user</param>
        public void HandlePeriodReferenceSet(String selectedPeriodType)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (selectedPeriodType != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, selectedPeriodType, 1);
                    selectedPeriod = selectedPeriodType;
                    if (portfolioSelectionData != null && effectiveDate != null && selectedPeriod != null && nodeName != null)
                    {
                        if (AttributionDataInfo.Count == 0 && IsActive)
                        {
                            BeginWebServiceCall(portfolioSelectionData, Convert.ToDateTime(effectiveDate), nodeName); 
                        }
                        else
                        {
                            if (null != AttributionDataLoadedEvent)
                            {
                                AttributionDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
                            }
                            SelectedPeriod = selectedPeriodType;
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

        /// <summary>
        /// Assigns UI Field Properties based on Node Name Selected
        /// </summary>
        /// <param name="selectedNodeType">Node Name selected by the user</param>
        public void HandleNodeNameReferenceSet(String selectedNodeType)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (selectedNodeType != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, selectedNodeType, 1);
                    nodeName = selectedNodeType;
                    if (portfolioSelectionData != null && effectiveDate != null && selectedPeriod != null && IsActive && nodeName != null)
                    {
                        BeginWebServiceCall(portfolioSelectionData, Convert.ToDateTime(effectiveDate), nodeName);
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
        /// Calling Web services through dbInteractivity
        /// </summary>
        /// <param name="portfolioSelectionData"></param>
        /// <param name="effectiveDate"></param>
        /// <param name="nodeName"></param>
        private void BeginWebServiceCall(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate, String nodeName)
        {
            if (null != AttributionDataLoadedEvent)
            {
                AttributionDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
            }
            dbInteractivity.RetrieveAttributionData(portfolioSelectionData, effectiveDate, nodeName, RetrieveAttributionDataCallBackMethod);
        }
        #endregion

        #region CallbackMethods
        /// <summary>
        /// Plots the grid after getting the resulting collection
        /// </summary>
        /// <param name="result">Contains the Attribution Data</param>
        private void RetrieveAttributionDataCallBackMethod(List<AttributionData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null && result.Count > 0)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result, 1);
                    AttributionDataInfo = result;
                    SelectedPeriod = selectedPeriod;
                    if (null != AttributionDataLoadedEvent)
                    {
                        AttributionDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
                    }
                }
                else
                {
                    AttributionDataInfo = new List<AttributionData>();
                    SelectedPeriod = selectedPeriod;
                    Logging.LogMethodParameterNull(logger, methodNamespace, 1);
                    if (null != AttributionDataLoadedEvent)
                        AttributionDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
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

        #region HelperMethod
        /// <summary>
        /// method to check inception date
        /// </summary>
        /// <param name="period"></param>
        /// <param name="selectedDate"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool InceptionDateCheck(string period, DateTime selectedDate, List<AttributionData> data)
        {
            System.Globalization.DateTimeFormatInfo dateInfo = new System.Globalization.DateTimeFormatInfo();
            dateInfo.ShortDatePattern = "dd/MM/yyyy";
            DateTime portfolioInceptionDate = Convert.ToDateTime(data[0].PorInceptionDate, dateInfo);
            bool isValid = false;
            isValid = InceptionDateChecker.ValidateInceptionDate(selectedPeriod, portfolioInceptionDate, Convert.ToDateTime(data[0].EffectiveDate));
            return isValid;
        }

        #endregion

        #region EventUnSubscribe
        /// <summary>
        /// Method that disposes the events
        /// </summary>
        public void Dispose()
        {
            eventAggregator.GetEvent<PortfolioReferenceSetEvent>().Unsubscribe(HandleFundReferenceSet);
            eventAggregator.GetEvent<EffectiveDateReferenceSetEvent>().Unsubscribe(HandleEffectiveDateSet);
            eventAggregator.GetEvent<PeriodReferenceSetEvent>().Unsubscribe(HandlePeriodReferenceSet);
            eventAggregator.GetEvent<NodeNameReferenceSetEvent>().Unsubscribe(HandleNodeNameReferenceSet);
        }
        #endregion
    }
}
