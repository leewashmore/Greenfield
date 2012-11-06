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
using System.Collections.ObjectModel;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.ViewModel;
using GreenField.ServiceCaller.SecurityReferenceDefinitions;
using GreenField.Common;
using GreenField.Common.Helper;
using GreenField.ServiceCaller.BenchmarkHoldingsDefinitions;
using GreenField.DataContracts;
using GreenField.Gadgets.Models;
using GreenField.Web.Helpers;
using GreenField.ServiceCaller;

namespace GreenField.Gadgets.ViewModels
{
    /// <summary>
    /// Class that provides the interaction of the view model with the Service caller and the View.
    /// </summary>
    public class ViewModelPortfolioRiskReturns : NotificationObject
    {
        #region Fields
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
        /// Contains the effective date
        /// </summary>
        private DateTime? effectiveDate;
        #endregion

        #region Constructor

        // <summary>
        /// Constructor of the class that initializes various objects
        /// </summary>
        /// <param name="param">MEF Eventaggrigator instance</param>
        public ViewModelPortfolioRiskReturns(DashboardGadgetParam param)
        {
            dbInteractivity = param.DBInteractivity;
            logger = param.LoggerFacade;
            eventAggregator = param.EventAggregator;
            portfolioSelectionData = param.DashboardGadgetPayload.PortfolioSelectionData;
            effectiveDate = param.DashboardGadgetPayload.EffectiveDate;
            if (eventAggregator != null)
            {
                eventAggregator.GetEvent<PortfolioReferenceSetEvent>().Subscribe(HandleFundReferenceSet);
                eventAggregator.GetEvent<EffectiveDateReferenceSetEvent>().Subscribe(HandleEffectiveDateSet);
            }
            if (effectiveDate != null && portfolioSelectionData != null && selectedPeriod != null && IsActive)
            {
                dbInteractivity.RetrievePortfolioRiskReturnData(portfolioSelectionData, Convert.ToDateTime(effectiveDate),
                    RetrievePortfolioRiskReturnDataCallbackMethod);
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Consists of whole Portfolio Data
        /// </summary>
        private List<PortfolioRiskReturnData> portfolioRiskReturnInfo;
        public List<PortfolioRiskReturnData> PortfolioRiskReturnInfo
        {
            get
            { return portfolioRiskReturnInfo; }
            set
            {
                portfolioRiskReturnInfo = value;
                RaisePropertyChanged(() => this.PortfolioRiskReturnInfo);
            }
        }

        /// <summary>
        /// Property binded to the grid
        /// </summary>
        private List<PerformancePeriodData> portfolioRiskReturnPeriodInfo;
        public List<PerformancePeriodData> PortfolioRiskReturnPeriodInfo
        {
            get
            {
                return portfolioRiskReturnPeriodInfo;
            }
            set
            {
                portfolioRiskReturnPeriodInfo = value;
                RaisePropertyChanged(() => this.PortfolioRiskReturnPeriodInfo);
            }
        }

        /// <summary>
        /// List of Periods for this gadget
        /// </summary>
        public List<String> PeriodInfo
        {
            get
            {
                { return new List<String> { "1Y", "3Y", "5Y", "10Y", "SI" }; }
            }
        }

        /// <summary>
        /// The Period selected by the user.
        /// </summary>
        private String selectedPeriod = "1Y";
        public String SelectedPeriod
        {
            get
            { return selectedPeriod; }
            set
            {
                selectedPeriod = value;
                RaisePropertyChanged(() => this.SelectedPeriod);
                if (PortfolioRiskReturnInfo != null && PortfolioRiskReturnInfo.Count > 0)
                {
                    bool isValidData;
                    switch (value)
                    {
                        case "1Y":
                            List<PerformancePeriodData> result = new List<PerformancePeriodData>();
                            PerformancePeriodData entry = new PerformancePeriodData();
                            isValidData = InceptionDateCheck(selectedPeriod, Convert.ToDateTime(PortfolioRiskReturnInfo[0].EffectiveDate), PortfolioRiskReturnInfo);
                            if (!isValidData)
                            {
                                PortfolioRiskReturnPeriodInfo = new List<PerformancePeriodData>();
                            }
                            else
                            {
                                entry.DataPointName = PortfolioRiskReturnInfo[0].DataPointName;
                                entry.BenchmarkValue = PortfolioRiskReturnInfo[0].BenchMarkValue1;
                                entry.PortfolioValue = PortfolioRiskReturnInfo[0].PortfolioValue1;
                                result.Add(entry);
                                entry = new PerformancePeriodData();
                                entry.DataPointName = PortfolioRiskReturnInfo[1].DataPointName;
                                entry.BenchmarkValue = PortfolioRiskReturnInfo[1].BenchMarkValue1;
                                entry.PortfolioValue = PortfolioRiskReturnInfo[1].PortfolioValue1;
                                result.Add(entry);
                                entry = new PerformancePeriodData();
                                entry.DataPointName = PortfolioRiskReturnInfo[2].DataPointName;
                                entry.BenchmarkValue = PortfolioRiskReturnInfo[2].BenchMarkValue1;
                                entry.PortfolioValue = PortfolioRiskReturnInfo[2].PortfolioValue1;
                                result.Add(entry);
                                entry = new PerformancePeriodData();
                                entry.DataPointName = PortfolioRiskReturnInfo[3].DataPointName;
                                entry.BenchmarkValue = PortfolioRiskReturnInfo[3].BenchMarkValue1;
                                entry.PortfolioValue = PortfolioRiskReturnInfo[3].PortfolioValue1;
                                result.Add(entry);
                                entry = new PerformancePeriodData();
                                entry.DataPointName = PortfolioRiskReturnInfo[4].DataPointName;
                                entry.BenchmarkValue = PortfolioRiskReturnInfo[4].BenchMarkValue1;
                                entry.PortfolioValue = PortfolioRiskReturnInfo[4].PortfolioValue1;
                                result.Add(entry);
                                entry = new PerformancePeriodData();
                                entry.DataPointName = PortfolioRiskReturnInfo[5].DataPointName;
                                entry.BenchmarkValue = PortfolioRiskReturnInfo[5].BenchMarkValue1;
                                entry.PortfolioValue = PortfolioRiskReturnInfo[5].PortfolioValue1;
                                result.Add(entry);
                                entry = new PerformancePeriodData();
                                entry.DataPointName = PortfolioRiskReturnInfo[6].DataPointName;
                                entry.BenchmarkValue = PortfolioRiskReturnInfo[6].BenchMarkValue1;
                                entry.PortfolioValue = PortfolioRiskReturnInfo[6].PortfolioValue1;
                                result.Add(entry);
                                entry = new PerformancePeriodData();
                                entry.DataPointName = PortfolioRiskReturnInfo[7].DataPointName;
                                entry.BenchmarkValue = PortfolioRiskReturnInfo[7].BenchMarkValue1;
                                entry.PortfolioValue = PortfolioRiskReturnInfo[7].PortfolioValue1;
                                result.Add(entry);
                                PortfolioRiskReturnPeriodInfo = result;
                            }
                            if (null != PortfolioRiskReturnDataLoadedEvent)
                            {
                                PortfolioRiskReturnDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
                            }
                            break;
                        case "3Y":
                            List<PerformancePeriodData> result1 = new List<PerformancePeriodData>();
                            PerformancePeriodData entry1 = new PerformancePeriodData();
                            isValidData = InceptionDateCheck(selectedPeriod, Convert.ToDateTime(PortfolioRiskReturnInfo[0].EffectiveDate), PortfolioRiskReturnInfo);
                            if (!isValidData)
                            {
                                PortfolioRiskReturnPeriodInfo = new List<PerformancePeriodData>();
                            }
                            else
                            {
                                entry1.DataPointName = PortfolioRiskReturnInfo[0].DataPointName;
                                entry1.BenchmarkValue = PortfolioRiskReturnInfo[0].BenchMarkValue2;
                                entry1.PortfolioValue = PortfolioRiskReturnInfo[0].PortfolioValue2;
                                result1.Add(entry1);
                                entry1 = new PerformancePeriodData();
                                entry1.DataPointName = PortfolioRiskReturnInfo[1].DataPointName;
                                entry1.BenchmarkValue = PortfolioRiskReturnInfo[1].BenchMarkValue2;
                                entry1.PortfolioValue = PortfolioRiskReturnInfo[1].PortfolioValue2;
                                result1.Add(entry1);
                                entry1 = new PerformancePeriodData();
                                entry1.DataPointName = PortfolioRiskReturnInfo[2].DataPointName;
                                entry1.BenchmarkValue = PortfolioRiskReturnInfo[2].BenchMarkValue2;
                                entry1.PortfolioValue = PortfolioRiskReturnInfo[2].PortfolioValue2;
                                result1.Add(entry1);
                                entry1 = new PerformancePeriodData();
                                entry1.DataPointName = PortfolioRiskReturnInfo[3].DataPointName;
                                entry1.BenchmarkValue = PortfolioRiskReturnInfo[3].BenchMarkValue2;
                                entry1.PortfolioValue = PortfolioRiskReturnInfo[3].PortfolioValue2;
                                result1.Add(entry1);
                                entry1 = new PerformancePeriodData();
                                entry1.DataPointName = PortfolioRiskReturnInfo[4].DataPointName;
                                entry1.BenchmarkValue = PortfolioRiskReturnInfo[4].BenchMarkValue2;
                                entry1.PortfolioValue = PortfolioRiskReturnInfo[4].PortfolioValue2;
                                result1.Add(entry1);
                                entry1 = new PerformancePeriodData();
                                entry1.DataPointName = PortfolioRiskReturnInfo[5].DataPointName;
                                entry1.BenchmarkValue = PortfolioRiskReturnInfo[5].BenchMarkValue2;
                                entry1.PortfolioValue = PortfolioRiskReturnInfo[5].PortfolioValue2;
                                result1.Add(entry1);
                                entry1 = new PerformancePeriodData();
                                entry1.DataPointName = PortfolioRiskReturnInfo[6].DataPointName;
                                entry1.BenchmarkValue = PortfolioRiskReturnInfo[6].BenchMarkValue2;
                                entry1.PortfolioValue = PortfolioRiskReturnInfo[6].PortfolioValue2;
                                result1.Add(entry1);
                                entry1 = new PerformancePeriodData();
                                entry1.DataPointName = PortfolioRiskReturnInfo[7].DataPointName;
                                entry1.BenchmarkValue = PortfolioRiskReturnInfo[7].BenchMarkValue2;
                                entry1.PortfolioValue = PortfolioRiskReturnInfo[7].PortfolioValue2;
                                result1.Add(entry1);
                                PortfolioRiskReturnPeriodInfo = result1;
                            }
                            if (null != PortfolioRiskReturnDataLoadedEvent)
                            {
                                PortfolioRiskReturnDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
                            }
                            break;
                        case "5Y":
                            List<PerformancePeriodData> result2 = new List<PerformancePeriodData>();
                            PerformancePeriodData entry2 = new PerformancePeriodData();
                            isValidData = InceptionDateCheck(selectedPeriod, Convert.ToDateTime(PortfolioRiskReturnInfo[0].EffectiveDate), PortfolioRiskReturnInfo);
                            if (!isValidData)
                            {
                                PortfolioRiskReturnPeriodInfo = new List<PerformancePeriodData>();
                            }
                            else
                            {
                                entry2.DataPointName = PortfolioRiskReturnInfo[0].DataPointName;
                                entry2.BenchmarkValue = PortfolioRiskReturnInfo[0].BenchMarkValue3;
                                entry2.PortfolioValue = PortfolioRiskReturnInfo[0].PortfolioValue3;
                                result2.Add(entry2);
                                entry2 = new PerformancePeriodData();
                                entry2.DataPointName = PortfolioRiskReturnInfo[1].DataPointName;
                                entry2.BenchmarkValue = PortfolioRiskReturnInfo[1].BenchMarkValue3;
                                entry2.PortfolioValue = PortfolioRiskReturnInfo[1].PortfolioValue3;
                                result2.Add(entry2);
                                entry2 = new PerformancePeriodData();
                                entry2.DataPointName = PortfolioRiskReturnInfo[2].DataPointName;
                                entry2.BenchmarkValue = PortfolioRiskReturnInfo[2].BenchMarkValue3;
                                entry2.PortfolioValue = PortfolioRiskReturnInfo[2].PortfolioValue3;
                                result2.Add(entry2);
                                entry2 = new PerformancePeriodData();
                                entry2.DataPointName = PortfolioRiskReturnInfo[3].DataPointName;
                                entry2.BenchmarkValue = PortfolioRiskReturnInfo[3].BenchMarkValue3;
                                entry2.PortfolioValue = PortfolioRiskReturnInfo[3].PortfolioValue3;
                                result2.Add(entry2);
                                entry2 = new PerformancePeriodData();
                                entry2.DataPointName = PortfolioRiskReturnInfo[4].DataPointName;
                                entry2.BenchmarkValue = PortfolioRiskReturnInfo[4].BenchMarkValue3;
                                entry2.PortfolioValue = PortfolioRiskReturnInfo[4].PortfolioValue3;
                                result2.Add(entry2);
                                entry2 = new PerformancePeriodData();
                                entry2.DataPointName = PortfolioRiskReturnInfo[5].DataPointName;
                                entry2.BenchmarkValue = PortfolioRiskReturnInfo[5].BenchMarkValue3;
                                entry2.PortfolioValue = PortfolioRiskReturnInfo[5].PortfolioValue3;
                                result2.Add(entry2);
                                entry2 = new PerformancePeriodData();
                                entry2.DataPointName = PortfolioRiskReturnInfo[6].DataPointName;
                                entry2.BenchmarkValue = PortfolioRiskReturnInfo[6].BenchMarkValue3;
                                entry2.PortfolioValue = PortfolioRiskReturnInfo[6].PortfolioValue3;
                                result2.Add(entry2);
                                entry2 = new PerformancePeriodData();
                                entry2.DataPointName = PortfolioRiskReturnInfo[7].DataPointName;
                                entry2.BenchmarkValue = PortfolioRiskReturnInfo[7].BenchMarkValue3;
                                entry2.PortfolioValue = PortfolioRiskReturnInfo[7].PortfolioValue3;
                                result2.Add(entry2);
                                PortfolioRiskReturnPeriodInfo = result2;
                            }
                            if (null != PortfolioRiskReturnDataLoadedEvent)
                            {
                                PortfolioRiskReturnDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
                            }
                            break;
                        case "10Y":
                            List<PerformancePeriodData> result3 = new List<PerformancePeriodData>();
                            PerformancePeriodData entry3 = new PerformancePeriodData();
                            isValidData = InceptionDateCheck(selectedPeriod, Convert.ToDateTime(PortfolioRiskReturnInfo[0].EffectiveDate), PortfolioRiskReturnInfo);
                            if (!isValidData)
                            {
                                PortfolioRiskReturnPeriodInfo = new List<PerformancePeriodData>();
                            }
                            else
                            {
                                entry3.DataPointName = PortfolioRiskReturnInfo[0].DataPointName;
                                entry3.BenchmarkValue = PortfolioRiskReturnInfo[0].BenchMarkValue4;
                                entry3.PortfolioValue = PortfolioRiskReturnInfo[0].PortfolioValue4;
                                result3.Add(entry3);
                                entry3 = new PerformancePeriodData();
                                entry3.DataPointName = PortfolioRiskReturnInfo[1].DataPointName;
                                entry3.BenchmarkValue = PortfolioRiskReturnInfo[1].BenchMarkValue4;
                                entry3.PortfolioValue = PortfolioRiskReturnInfo[1].PortfolioValue4;
                                result3.Add(entry3);
                                entry3 = new PerformancePeriodData();
                                entry3.DataPointName = PortfolioRiskReturnInfo[2].DataPointName;
                                entry3.BenchmarkValue = PortfolioRiskReturnInfo[2].BenchMarkValue4;
                                entry3.PortfolioValue = PortfolioRiskReturnInfo[2].PortfolioValue4;
                                result3.Add(entry3);
                                entry3 = new PerformancePeriodData();
                                entry3.DataPointName = PortfolioRiskReturnInfo[3].DataPointName;
                                entry3.BenchmarkValue = PortfolioRiskReturnInfo[3].BenchMarkValue4;
                                entry3.PortfolioValue = PortfolioRiskReturnInfo[3].PortfolioValue4;
                                result3.Add(entry3);
                                entry3 = new PerformancePeriodData();
                                entry3.DataPointName = PortfolioRiskReturnInfo[4].DataPointName;
                                entry3.BenchmarkValue = PortfolioRiskReturnInfo[4].BenchMarkValue4;
                                entry3.PortfolioValue = PortfolioRiskReturnInfo[4].PortfolioValue4;
                                result3.Add(entry3);
                                entry3 = new PerformancePeriodData();
                                entry3.DataPointName = PortfolioRiskReturnInfo[5].DataPointName;
                                entry3.BenchmarkValue = PortfolioRiskReturnInfo[5].BenchMarkValue4;
                                entry3.PortfolioValue = PortfolioRiskReturnInfo[5].PortfolioValue4;
                                result3.Add(entry3);
                                entry3 = new PerformancePeriodData();
                                entry3.DataPointName = PortfolioRiskReturnInfo[6].DataPointName;
                                entry3.BenchmarkValue = PortfolioRiskReturnInfo[6].BenchMarkValue4;
                                entry3.PortfolioValue = PortfolioRiskReturnInfo[6].PortfolioValue4;
                                result3.Add(entry3);
                                entry3 = new PerformancePeriodData();
                                entry3.DataPointName = PortfolioRiskReturnInfo[7].DataPointName;
                                entry3.BenchmarkValue = PortfolioRiskReturnInfo[7].BenchMarkValue4;
                                entry3.PortfolioValue = PortfolioRiskReturnInfo[7].PortfolioValue4;
                                result3.Add(entry3);
                                PortfolioRiskReturnPeriodInfo = result3;
                            }
                            if (null != PortfolioRiskReturnDataLoadedEvent)
                            {
                                PortfolioRiskReturnDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
                            }
                            break;
                        case "SI":
                            List<PerformancePeriodData> result4 = new List<PerformancePeriodData>();
                            PerformancePeriodData entry4 = new PerformancePeriodData();
                            isValidData = InceptionDateCheck(selectedPeriod, Convert.ToDateTime(PortfolioRiskReturnInfo[0].EffectiveDate), PortfolioRiskReturnInfo);
                            if (!isValidData)
                            {
                                PortfolioRiskReturnPeriodInfo = new List<PerformancePeriodData>();
                            }
                            else
                            {
                                entry4.DataPointName = PortfolioRiskReturnInfo[0].DataPointName;
                                entry4.BenchmarkValue = PortfolioRiskReturnInfo[0].BenchMarkValue5;
                                entry4.PortfolioValue = PortfolioRiskReturnInfo[0].PortfolioValue5;
                                result4.Add(entry4);
                                entry4 = new PerformancePeriodData();
                                entry4.DataPointName = PortfolioRiskReturnInfo[1].DataPointName;
                                entry4.BenchmarkValue = PortfolioRiskReturnInfo[1].BenchMarkValue5;
                                entry4.PortfolioValue = PortfolioRiskReturnInfo[1].PortfolioValue5;
                                result4.Add(entry4);
                                entry4 = new PerformancePeriodData();
                                entry4.DataPointName = PortfolioRiskReturnInfo[2].DataPointName;
                                entry4.BenchmarkValue = PortfolioRiskReturnInfo[2].BenchMarkValue5;
                                entry4.PortfolioValue = PortfolioRiskReturnInfo[2].PortfolioValue5;
                                result4.Add(entry4);
                                entry4 = new PerformancePeriodData();
                                entry4.DataPointName = PortfolioRiskReturnInfo[3].DataPointName;
                                entry4.BenchmarkValue = PortfolioRiskReturnInfo[3].BenchMarkValue5;
                                entry4.PortfolioValue = PortfolioRiskReturnInfo[3].PortfolioValue5;
                                result4.Add(entry4);
                                entry4 = new PerformancePeriodData();
                                entry4.DataPointName = PortfolioRiskReturnInfo[4].DataPointName;
                                entry4.BenchmarkValue = PortfolioRiskReturnInfo[4].BenchMarkValue5;
                                entry4.PortfolioValue = PortfolioRiskReturnInfo[4].PortfolioValue5;
                                result4.Add(entry4);
                                entry4 = new PerformancePeriodData();
                                entry4.DataPointName = PortfolioRiskReturnInfo[5].DataPointName;
                                entry4.BenchmarkValue = PortfolioRiskReturnInfo[5].BenchMarkValue5;
                                entry4.PortfolioValue = PortfolioRiskReturnInfo[5].PortfolioValue5;
                                result4.Add(entry4);
                                entry4 = new PerformancePeriodData();
                                entry4.DataPointName = PortfolioRiskReturnInfo[6].DataPointName;
                                entry4.BenchmarkValue = PortfolioRiskReturnInfo[6].BenchMarkValue5;
                                entry4.PortfolioValue = PortfolioRiskReturnInfo[6].PortfolioValue5;
                                result4.Add(entry4);
                                entry4 = new PerformancePeriodData();
                                entry4.DataPointName = PortfolioRiskReturnInfo[7].DataPointName;
                                entry4.BenchmarkValue = PortfolioRiskReturnInfo[7].BenchMarkValue5;
                                entry4.PortfolioValue = PortfolioRiskReturnInfo[7].PortfolioValue5;
                                result4.Add(entry4);
                                PortfolioRiskReturnPeriodInfo = result4;
                            }
                            if (null != PortfolioRiskReturnDataLoadedEvent)
                            {
                                PortfolioRiskReturnDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
                            }
                            break;
                        default:
                            List<PerformancePeriodData> result5 = new List<PerformancePeriodData>();
                            PortfolioRiskReturnPeriodInfo = result5;
                            if (null != PortfolioRiskReturnDataLoadedEvent)
                            {
                                PortfolioRiskReturnDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
                            }
                            break;
                    }
                }
                else
                {
                    List<PerformancePeriodData> result6 = new List<PerformancePeriodData>();
                    PortfolioRiskReturnPeriodInfo = result6;
                    if (null != PortfolioRiskReturnDataLoadedEvent)
                    {
                        PortfolioRiskReturnDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
                    }
                }
            }
        }

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
                if (effectiveDate != null && portfolioSelectionData != null && selectedPeriod != null && isActive)
                {
                    BeginWebServiceCall(portfolioSelectionData, Convert.ToDateTime(effectiveDate));
                }
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Event for the notification of Data Load Completion
        /// </summary>
        public event DataRetrievalProgressIndicatorEventHandler PortfolioRiskReturnDataLoadedEvent;
        #endregion

        #region Event Handlers
        /// <summary>
        /// Assigns UI Field Properties based on Fund reference
        /// </summary>
        /// <param name="portfSelectionData">Object of PortfolioSelectionData Class containing Fund data</param>
        public void HandleFundReferenceSet(PortfolioSelectionData portfSelectionData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (portfSelectionData != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, portfSelectionData, 1);
                    portfolioSelectionData = portfSelectionData;
                    if (effectiveDate != null && portfSelectionData != null && selectedPeriod != null && IsActive)
                    {
                        BeginWebServiceCall(portfolioSelectionData, Convert.ToDateTime(effectiveDate));
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
        /// <param name="effectDate"></param>
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
                    if (effectiveDate != null && portfolioSelectionData != null && selectedPeriod != null && IsActive)
                    {
                        BeginWebServiceCall(portfolioSelectionData, Convert.ToDateTime(effectiveDate));
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
        /// Calls Web services through dbInteractivity
        /// </summary>
        /// <param name="portfolioSelectionData"></param>
        /// <param name="effectiveDate"></param>
        private void BeginWebServiceCall(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate)
        {
            if (null != PortfolioRiskReturnDataLoadedEvent)
                PortfolioRiskReturnDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
            dbInteractivity.RetrievePortfolioRiskReturnData(portfolioSelectionData, Convert.ToDateTime(effectiveDate), RetrievePortfolioRiskReturnDataCallbackMethod);
        }
        #endregion

        #region Callback Methods

        /// <summary>
        /// Plots the result in the grid after getting the resulting collection
        /// </summary>
        /// <param name="result">Contains the Portfolio Risk Return Data</param>
        public void RetrievePortfolioRiskReturnDataCallbackMethod(List<PortfolioRiskReturnData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null && result.Count > 0)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result, 1);
                    PortfolioRiskReturnInfo = result;
                    SelectedPeriod = selectedPeriod;
                    if (null != PortfolioRiskReturnDataLoadedEvent)
                    {
                        PortfolioRiskReturnDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
                    }
                }
                else
                {
                    PortfolioRiskReturnInfo = new List<PortfolioRiskReturnData>();
                    SelectedPeriod = selectedPeriod;
                    Logging.LogMethodParameterNull(logger, methodNamespace, 1);
                    PortfolioRiskReturnDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
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
        public bool InceptionDateCheck(string period, DateTime selectedDate,List<PortfolioRiskReturnData> data)
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
        }

        #endregion
    }
}
