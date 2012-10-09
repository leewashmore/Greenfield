using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Greenfield.Gadgets.Helpers;
using Greenfield.Gadgets.Models;
using GreenField.Common;
using GreenField.DataContracts;
using GreenField.Gadgets.Helpers;
using GreenField.Gadgets.Models;
using GreenField.ServiceCaller;
using GreenField.ServiceCaller.DCFDefinitions;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.ViewModel;

namespace GreenField.Gadgets.ViewModels
{
    /// <summary>
    /// View-Model for DCF Gadgets
    /// </summary>
    public class ViewModelDCF : NotificationObject
    {
        #region PrivateVariables

        /// <summary>
        /// Event Aggregator
        /// </summary>
        private IEventAggregator eventAggregator;

        /// <summary>
        /// Instance of Service Caller Class
        /// </summary>
        private IDBInteractivity dbInteractivity;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="param"></param>
        public ViewModelDCF(DashboardGadgetParam param)
        {
            eventAggregator = param.EventAggregator;
            dbInteractivity = param.DBInteractivity;
            logger = param.LoggerFacade;
            EntitySelectionData = param.DashboardGadgetPayload.EntitySelectionData;
            if (eventAggregator != null)
            {
                eventAggregator.GetEvent<SecurityReferenceSetEvent>().Subscribe(HandleSecurityReferenceSetEvent);
            }
            if (EntitySelectionData != null)
            {
                HandleSecurityReferenceSetEvent(EntitySelectionData);
            }
        }

        #endregion

        #region PropertyDeclaration

        #region SelectedSecurity

        /// <summary>
        /// Selected Security
        /// </summary>
        private EntitySelectionData entitySelectionData;
        public EntitySelectionData EntitySelectionData
        {
            get
            {
                return entitySelectionData;
            }
            set
            {
                entitySelectionData = value;
                this.RaisePropertyChanged(() => this.EntitySelectionData);
            }
        }

        /// <summary>
        /// Country of Selected Security
        /// </summary>
        private string countryName;
        public string CountryName
        {
            get { return countryName; }
            set
            {
                countryName = value;
                this.RaisePropertyChanged(() => this.CountryName);
            }
        }


        #endregion

        #region Dashboard

        /// <summary>
        /// Bool to check whether the Current Dashboard is Selected or Not
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
                if (isActive != value)
                {
                    isActive = value;
                    if (value)
                    {
                        if (EntitySelectionData != null)
                        {
                            dbInteractivity.RetrieveDCFCurrentPrice(EntitySelectionData, RetrieveCurrentPriceDataCallbackMethod);
                            BusyIndicatorNotification(true, "Fetching Data for Selected Security");
                        }
                    }
                    this.RaisePropertyChanged(() => this.IsActive);
                }
            }
        }

        #endregion

        #region Busy Indicator

        /// <summary>
        /// Busy Indicator Status
        /// </summary>
        private bool busyIndicatorIsBusy;
        public bool BusyIndicatorIsBusy
        {
            get { return busyIndicatorIsBusy; }
            set
            {
                busyIndicatorIsBusy = value;
                RaisePropertyChanged(() => this.BusyIndicatorIsBusy);
            }
        }

        /// <summary>
        /// Busy Indicator Content
        /// </summary>
        private string busyIndicatorContent;
        public string BusyIndicatorContent
        {
            get { return busyIndicatorContent; }
            set
            {
                busyIndicatorContent = value;
                RaisePropertyChanged(() => this.BusyIndicatorContent);
            }
        }

        #endregion

        #region DataGrid

        #region TerminalValueCalculations

        /// <summary>
        /// List of Type TerminalValueCalculationsData
        /// </summary>
        private List<DCFTerminalValueCalculationsData> terminalValueCalculationsData;
        public List<DCFTerminalValueCalculationsData> TerminalValueCalculationsData
        {
            get
            {
                if (terminalValueCalculationsData == null)
                {
                    terminalValueCalculationsData = new List<DCFTerminalValueCalculationsData>();
                }
                return terminalValueCalculationsData;
            }
            set
            {
                terminalValueCalculationsData = value;
                this.RaisePropertyChanged(() => this.TerminalValueCalculationsData);
            }
        }

        /// <summary>
        /// List of type TerminalValueCalculationsDisplayData to show in the Grid
        /// </summary>
        private RangeObservableCollection<DCFDisplayData> terminalValueCalculationsDisplayData;
        public RangeObservableCollection<DCFDisplayData> TerminalValueCalculationsDisplayData
        {
            get
            {
                if (terminalValueCalculationsDisplayData == null)
                {
                    terminalValueCalculationsDisplayData = new RangeObservableCollection<DCFDisplayData>();
                }
                return terminalValueCalculationsDisplayData;
            }
            set
            {
                terminalValueCalculationsDisplayData = value;
                this.RaisePropertyChanged(() => this.TerminalValueCalculationsDisplayData);
            }
        }

        /// <summary>
        /// FreeCashFlow for Year9
        /// </summary>
        private decimal freeCashFlowY9;
        public decimal FreeCashFlowY9
        {
            get
            {
                return freeCashFlowY9;
            }
            set
            {
                freeCashFlowY9 = value;
            }
        }

        #endregion

        #region Analysis Summary

        /// <summary>
        /// Collection of type DCFAnalysisSummaryData bound to the Data-Grid
        /// </summary>
        private RangeObservableCollection<DCFAnalysisSummaryData> analysisSummaryData;
        public RangeObservableCollection<DCFAnalysisSummaryData> AnalysisSummaryData
        {
            get
            {
                if (analysisSummaryData == null)
                {
                    analysisSummaryData = new RangeObservableCollection<DCFAnalysisSummaryData>();
                }
                return analysisSummaryData;
            }
            set
            {
                analysisSummaryData = value;
                this.RaisePropertyChanged(() => this.AnalysisSummaryData);
            }
        }

        /// <summary>
        /// Default Display Data
        /// </summary>
        private RangeObservableCollection<DCFDisplayData> analysisSummaryDisplayData;
        public RangeObservableCollection<DCFDisplayData> AnalysisSummaryDisplayData
        {
            get
            {
                if (analysisSummaryDisplayData == null)
                {
                    analysisSummaryDisplayData = SetDefaultAnalysisDisplayData();
                }
                return analysisSummaryDisplayData;
            }
            set
            {
                this.analysisSummaryDisplayData = value;
                this.RaisePropertyChanged(() => this.AnalysisSummaryDisplayData);
            }
        }

        /// <summary>
        /// Stock Specific Discount
        /// </summary>
        private decimal? stockSpecificDiscount = 0;
        public decimal? StockSpecificDiscount
        {
            get
            {
                return stockSpecificDiscount;
            }
            set
            {
                stockSpecificDiscount = value;
                if (AnalysisSummaryData.Count == 0)
                {
                    if (AnalysisSummaryDisplayData.Where(a => a.PropertyName == "Stock Specific Discount").FirstOrDefault() != null)
                    {
                        AnalysisSummaryDisplayData.Where(a => a.PropertyName == "Stock Specific Discount").FirstOrDefault().Value = Convert.ToString(value) + "%";
                    }
                    this.RaisePropertyChanged(() => this.AnalysisSummaryDisplayData);
                }
                else
                {
                    SetAnalysisSummaryDisplayData();
                }
                this.RaisePropertyChanged(() => this.StockSpecificDiscount);
            }
        }

        #endregion

        #region DCFSummary

        /// <summary>
        /// Data Returned from Service
        /// </summary>
        private List<DCFSummaryData> summaryData;
        public List<DCFSummaryData> SummaryData
        {
            get
            {
                if (summaryData == null)
                {
                    summaryData = new List<DCFSummaryData>();
                }
                return summaryData;
            }
            set
            {
                summaryData = value;
                this.RaisePropertyChanged(() => this.SummaryData);
            }
        }

        /// <summary>
        /// Summary Display Data
        /// </summary>
        private RangeObservableCollection<DCFDisplayData> summaryDisplayData;
        public RangeObservableCollection<DCFDisplayData> SummaryDisplayData
        {
            get
            {
                if (summaryDisplayData == null)
                {
                    summaryDisplayData = new RangeObservableCollection<DCFDisplayData>();
                }
                return summaryDisplayData;
            }
            set
            {
                summaryDisplayData = value;
                this.RaisePropertyChanged(() => this.SummaryDisplayData);
            }
        }


        #endregion

        #region Sensitivity

        /// <summary>
        /// Data displayed in Sensitivity Grid
        /// </summary>
        private RangeObservableCollection<SensitivityData> sensitivityDisplayData;
        public RangeObservableCollection<SensitivityData> SensitivityDisplayData
        {
            get
            {
                return sensitivityDisplayData;
            }
            set
            {
                sensitivityDisplayData = value;
                this.RaisePropertyChanged(() => this.SensitivityDisplayData);
            }
        }


        #endregion

        #region SensitivityBPS

        /// <summary>
        /// Collection bound to Gadget SensitivityBVPS
        /// </summary>
        private RangeObservableCollection<SensitivityData> sensitivityBPS;
        public RangeObservableCollection<SensitivityData> SensitivityBPS
        {
            get
            {
                if (sensitivityBPS == null)
                {
                    sensitivityBPS = new RangeObservableCollection<SensitivityData>();
                }
                return sensitivityBPS;
            }
            set
            {
                sensitivityBPS = value;
                this.RaisePropertyChanged(() => this.SensitivityBPS);
            }
        }

        /// <summary>
        /// Value of FWD EPS
        /// </summary>
        private decimal fwdEPS = 0;
        public decimal FWDEPS
        {
            get
            {
                return fwdEPS;
            }
            set
            {
                fwdEPS = value;
                if (value != 0)
                {
                    if (SensitivityValues.Count != 0)
                    {
                        List<SensitivityData> data = ListUtils.GetDeepCopy<SensitivityData>(SensitivityValues.ToList());
                        GenerateSensitivityEPSData(data);
                    }
                }
                this.RaisePropertyChanged(() => this.FWDEPS);
            }
        }


        #endregion

        #region SensitivityBVPS

        /// <summary>
        /// Collection Bound to SensitivityBVPS grid
        /// </summary>
        private RangeObservableCollection<SensitivityData> sensivityBVPS;
        public RangeObservableCollection<SensitivityData> SensitivityBVPS
        {
            get
            {
                if (sensivityBVPS == null)
                {
                    sensivityBVPS = new RangeObservableCollection<SensitivityData>();
                }
                return sensivityBVPS;
            }
            set
            {
                sensivityBVPS = value;
                this.RaisePropertyChanged(() => this.SensitivityBVPS);
            }
        }

        /// <summary>
        /// Value of FWD BVPS
        /// </summary>
        private decimal fwdBVPS = 0;
        public decimal FWDBVPS
        {
            get
            {
                return fwdBVPS;
            }
            set
            {
                fwdBVPS = value;
                if (value != 0)
                {
                    if (SensitivityValues.Count != 0)
                    {
                        List<SensitivityData> data = ListUtils.GetDeepCopy<SensitivityData>(SensitivityValues.ToList());
                        GenerateSensitivityBVPSData(data);
                    }
                }
                this.RaisePropertyChanged(() => this.FWDBVPS);
            }
        }

        #endregion

        #endregion

        #region LoggerFacade

        /// <summary>
        /// Public property for LoggerFacade _logger
        /// </summary>
        private ILoggerFacade logger;
        public ILoggerFacade Logger
        {
            get
            {
                return logger;
            }
            set
            {
                logger = value;
            }
        }

        #endregion

        #region Calculations

        /// <summary>
        /// TerminalGrowthRate from FreeCashFlows
        /// </summary>
        private decimal? terminalGrowthRate;
        public decimal? TerminalGrowthRate
        {
            get
            {
                return terminalGrowthRate;
            }
            set
            {
                terminalGrowthRate = value;
                this.RaisePropertyChanged(() => this.TerminalGrowthRate);
            }
        }

        /// <summary>
        /// OverRide the value of MinorityInvestments
        /// </summary>
        private decimal? minorityInvestments;
        public decimal? MinorityInvestments
        {
            get
            {
                return minorityInvestments;
            }
            set
            {
                minorityInvestments = value;
                if (SummaryDisplayData.Count != 0)
                {
                    SetAnalysisSummaryDisplayData();
                }
                this.RaisePropertyChanged(() => this.MinorityInvestments);
            }
        }


        /// <summary>
        /// Yearly Calculated Data
        /// </summary>
        private List<DCFCashFlowData> yearlyCalculatedData;
        public List<DCFCashFlowData> YearlyCalculatedData
        {
            get
            {
                if (yearlyCalculatedData == null)
                {
                    yearlyCalculatedData = new List<DCFCashFlowData>();
                }
                return yearlyCalculatedData;
            }
            set
            {
                yearlyCalculatedData = value;
                this.RaisePropertyChanged(() => this.YearlyCalculatedData);
            }
        }

        /// <summary>
        /// Value of WACC
        /// </summary>
        private decimal WACCp;
        public decimal WACC
        {
            get
            {
                return WACCp;
            }
            set
            {
                WACCp = value;
                if (WACCp != null)
                {
                    YearlyCalculatedData = CalculateYearlyData(YearlyCalculatedData, value);
                }
                this.RaisePropertyChanged(() => this.WACC);
            }
        }

        /// <summary>
        /// True when WACC is less then TGR
        /// </summary>
        private bool WACCLessTGRp;
        public bool WACCLessTGR
        {
            get
            {
                return WACCLessTGRp;
            }
            set
            {
                WACCLessTGRp = value;
                this.RaisePropertyChanged(() => this.WACCLessTGR);
            }
        }

        #region Sensitivity

        /// <summary>
        /// Calculation Parameters used for DCF calculations
        /// </summary>
        private DCFCalculationParameters calculationParameters;
        public DCFCalculationParameters CalculationParameters
        {
            get
            {
                if (calculationParameters == null)
                {
                    calculationParameters = new DCFCalculationParameters();
                }
                return calculationParameters;
            }
            set
            {
                calculationParameters = value;
                this.RaisePropertyChanged(() => this.CalculationParameters);
            }
        }

        #region Statistics

        /// <summary>
        /// Max Share Value
        /// </summary>
        private string maxShareVal;
        public string MaxShareVal
        {
            get { return maxShareVal; }
            set
            {
                maxShareVal = value;
                this.RaisePropertyChanged(() => this.MaxShareVal);
            }
        }

        /// <summary>
        /// Min Share Value
        /// </summary>
        private string minShareVal;
        public string MinShareVal
        {
            get { return minShareVal; }
            set
            {
                minShareVal = value;
                this.RaisePropertyChanged(() => this.MinShareVal);
            }
        }

        /// <summary>
        /// Max Share Value
        /// </summary>
        private string avgShareVal;
        public string AvgShareVal
        {
            get { return avgShareVal; }
            set
            {
                avgShareVal = value;
                this.RaisePropertyChanged(() => this.AvgShareVal);
            }
        }

        /// <summary>
        /// Avg Share Value
        /// </summary>
        private string maxUpside;
        public string MaxUpside
        {
            get { return maxUpside; }
            set
            {
                maxUpside = value;
                this.RaisePropertyChanged(() => this.MaxUpside);
            }
        }

        /// <summary>
        /// Avg Share Value
        /// </summary>
        private string minUpside;
        public string MinUpside
        {
            get { return minUpside; }
            set
            {
                minUpside = value;
                this.RaisePropertyChanged(() => this.MinUpside);
            }
        }

        /// <summary>
        /// Avg Share Value
        /// </summary>
        private string avgUpside;
        public string AvgUpside
        {
            get { return avgUpside; }
            set
            {
                avgUpside = value;
                this.RaisePropertyChanged(() => this.AvgUpside);
            }
        }
        
        #endregion

        #region StatisticsEPS

        /// <summary>
        /// Max Share Value
        /// </summary>
        private string maxEPSShareVal;
        public string MaxEPSShareVal
        {
            get { return maxEPSShareVal; }
            set
            {
                maxEPSShareVal = value;
                this.RaisePropertyChanged(() => this.MaxEPSShareVal);
            }
        }

        /// <summary>
        /// Min Share Value
        /// </summary>
        private string minEPSShareVal;
        public string MinEPSShareVal
        {
            get { return minEPSShareVal; }
            set
            {
                minEPSShareVal = value;
                this.RaisePropertyChanged(() => this.MinEPSShareVal);
            }
        }

        /// <summary>
        /// Max Share Value
        /// </summary>
        private string avgEPSShareVal;
        public string AvgEPSShareVal
        {
            get { return avgEPSShareVal; }
            set
            {
                avgEPSShareVal = value;
                this.RaisePropertyChanged(() => this.AvgEPSShareVal);
            }
        }

        /// <summary>
        /// Avg Share Value
        /// </summary>
        private string maxEPSUpside;
        public string MaxEPSUpside
        {
            get { return maxEPSUpside; }
            set
            {
                maxEPSUpside = value;
                this.RaisePropertyChanged(() => this.MaxEPSUpside);
            }
        }

        /// <summary>
        /// Avg Share Value
        /// </summary>
        private string minEPSUpside;
        public string MinEPSUpside
        {
            get { return minEPSUpside; }
            set
            {
                minEPSUpside = value;
                this.RaisePropertyChanged(() => this.MinEPSUpside);
            }
        }

        /// <summary>
        /// Avg Share Value
        /// </summary>
        private string avgEPSUpside;
        public string AvgEPSUpside
        {
            get { return avgEPSUpside; }
            set
            {
                avgEPSUpside = value;
                this.RaisePropertyChanged(() => this.AvgEPSUpside);
            }
        }       

        #endregion

        #region StatisticsBVPS

        /// <summary>
        /// Max Share Value
        /// </summary>
        private string maxBVPSShareVal;
        public string MaxBVPSShareVal
        {
            get { return maxBVPSShareVal; }
            set
            {
                maxBVPSShareVal = value;
                this.RaisePropertyChanged(() => this.MaxBVPSShareVal);
            }
        }

        /// <summary>
        /// Min Share Value
        /// </summary>
        private string minBVPSShareVal;
        public string MinBVPSShareVal
        {
            get { return minBVPSShareVal; }
            set
            {
                minBVPSShareVal = value;
                this.RaisePropertyChanged(() => this.MinBVPSShareVal);
            }
        }

        /// <summary>
        /// Max Share Value
        /// </summary>
        private string avgBVPSShareVal;
        public string AvgBVPSShareVal
        {
            get { return avgBVPSShareVal; }
            set
            {
                avgBVPSShareVal = value;
                this.RaisePropertyChanged(() => this.AvgBVPSShareVal);
            }
        }

        /// <summary>
        /// Avg Share Value
        /// </summary>
        private string maxBVPSUpside;
        public string MaxBVPSUpside
        {
            get { return maxBVPSUpside; }
            set
            {
                maxBVPSUpside = value;
                this.RaisePropertyChanged(() => this.MaxBVPSUpside);
            }
        }

        /// <summary>
        /// Avg Share Value
        /// </summary>
        private string minBVPSUpside;
        public string MinBVPSUpside
        {
            get { return minBVPSUpside; }
            set
            {
                minBVPSUpside = value;
                this.RaisePropertyChanged(() => this.MinBVPSUpside);
            }
        }

        /// <summary>
        /// Avg Share Value
        /// </summary>
        private string avgBVPSUpside;
        public string AvgBVPSUpside
        {
            get { return avgBVPSUpside; }
            set
            {
                avgBVPSUpside = value;
                this.RaisePropertyChanged(() => this.AvgBVPSUpside);
            }
        }




        #endregion

        /// <summary>
        /// Property to Store the Values of Sensitivity
        /// </summary>
        private RangeObservableCollection<SensitivityData> sensitivityValues;
        public RangeObservableCollection<SensitivityData> SensitivityValues
        {
            get
            {
                if (sensitivityValues == null)
                {
                    sensitivityValues = new RangeObservableCollection<SensitivityData>();
                }
                return sensitivityValues;
            }
            set
            {
                sensitivityValues = value;
                this.RaisePropertyChanged(() => this.SensitivityValues);
            }
        }

        /// <summary>
        /// DCF Value per Share
        /// </summary>
        private decimal? dcfValuePerShare;
        public decimal? DCFValuePerShare
        {
            get
            {
                return dcfValuePerShare;
            }
            set
            {
                dcfValuePerShare = value;
                this.RaisePropertyChanged(() => this.DCFValuePerShare);
            }
        }

        #endregion

        /// <summary>
        /// Number of Shares
        /// </summary>
        private decimal numberOfShares;
        public decimal NumberOfShares
        {
            get
            {
                return numberOfShares;
            }
            set
            {
                numberOfShares = value;
                this.RaisePropertyChanged(() => this.NumberOfShares);
            }
        }

        /// <summary>
        /// TerminalValuePresent
        /// </summary>
        private decimal? terminalValuePresent;
        public decimal? TerminalValuePresent
        {
            get
            {
                return terminalValuePresent;
            }
            set
            {
                terminalValuePresent = value;
                this.RaisePropertyChanged(() => this.TerminalValuePresent);
            }
        }

        /// <summary>
        /// TerminalValueNominal
        /// </summary>
        private decimal? terminalValueNominal;
        public decimal? TerminalValueNominal
        {
            get
            {
                return terminalValueNominal;
            }
            set
            {
                terminalValueNominal = value;
                this.RaisePropertyChanged(() => this.TerminalValueNominal);
            }
        }

        #region Summary

        /// <summary>
        /// Current Market Price
        /// </summary>
        private decimal? currentMarketPrice;
        public decimal? CurrentMarketPrice
        {
            get
            {
                return currentMarketPrice;
            }
            set
            {
                currentMarketPrice = value;
                this.RaisePropertyChanged(() => this.CurrentMarketPrice);
            }
        }

        /// <summary>
        /// Yearly Calculated Data
        /// </summary>
        private List<DCFCashFlowData> yearlySummaryCalculatedData;
        public List<DCFCashFlowData> YearlySummaryCalculatedData
        {
            get
            {
                return yearlyCalculatedData;
            }
            set
            {
                yearlyCalculatedData = value;
                this.RaisePropertyChanged(() => this.YearlySummaryCalculatedData);
            }
        }

        /// <summary>
        /// Present Value Explicit Forecast- DCF Summary
        /// </summary>
        private decimal presentValueExplicitForecast;
        public decimal PresentValueExplicitForecast
        {
            get { return presentValueExplicitForecast; }
            set
            {
                presentValueExplicitForecast = value;
                this.RaisePropertyChanged(() => this.PresentValueExplicitForecast);
            }
        }
                
        #endregion
        
        #endregion

        #region Fair Value

        /// <summary>
        /// Fair value Data
        /// </summary>
        private List<PERIOD_FINANCIALS> fairValueData;
        public List<PERIOD_FINANCIALS> FairValueData
        {
            get
            {
                if (fairValueData == null)
                {
                    fairValueData = new List<PERIOD_FINANCIALS>();
                }
                return fairValueData;
            }
            set
            {
                fairValueData = value;
                this.RaisePropertyChanged(() => this.FairValueData);
            }
        }

        #endregion

        #region ICommand

        /// <summary>
        /// Insert Fair Values
        /// </summary>
        public ICommand InsertFairValues
        {
            get
            {
                return new DelegateCommand<object>(InsertFairValuesCommandMethod);
            }
        }

        #endregion

        #endregion

        #region ICommandMethods

        /// <summary>
        /// Insert Fair Values in Table
        /// </summary>
        /// <param name="param"></param>
        private void InsertFairValuesCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                DeleteExistingFairValues();
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
        }

        #endregion

        #region EventHandlers

        /// <summary>
        /// Event Handler to subscribed event 'SecurityReferenceSet'
        /// </summary>
        /// <param name="securityReferenceData">SecurityReferenceData</param>
        public void HandleSecurityReferenceSetEvent(EntitySelectionData entitySelectionData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                //argument null exception
                if (entitySelectionData != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, entitySelectionData, 1);
                    EntitySelectionData = entitySelectionData;
                    StockSpecificDiscount = 0;
                    SensitivityDisplayData = new RangeObservableCollection<SensitivityData>();
                    SensitivityBVPS = new RangeObservableCollection<SensitivityData>();
                    SensitivityBPS = new RangeObservableCollection<SensitivityData>();
                    FWDEPS = 0;
                    FWDBVPS = 0;
                    if (IsActive)
                    {
                        if (IsActive && EntitySelectionData != null)
                        {
                            dbInteractivity.RetrieveDCFCurrentPrice(entitySelectionData, RetrieveCurrentPriceDataCallbackMethod);
                            dbInteractivity.FetchDCFCountryName(entitySelectionData, RetrieveCountryNameCallbackMethod);
                            BusyIndicatorNotification(true, "Fetching Data for Selected Security");
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
        }

        #endregion

        #region CallbackMethods

        /// <summary>
        /// Consensus Estimate Data callback Method
        /// </summary>
        /// <param name="result"></param>
        public void RetrieveDCFTerminalValueCalculationsDataCallbackMethod(List<DCFTerminalValueCalculationsData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result, 1);
                    TerminalValueCalculationsData = result;
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
                BusyIndicatorNotification();
                dbInteractivity.RetrieveDCFSummaryData(EntitySelectionData, RetrieveDCFSummaryDataCallbackMethod);
                BusyIndicatorNotification(true, "Fetching Data for DCF Summary Data");
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }

        /// <summary>
        /// Consensus Estimate Data callback Method
        /// </summary>
        /// <param name="result"></param>
        public void RetrieveDCFCashFlowYearlyDataCallbackMethod(List<DCFCashFlowData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result, 1);
                    YearlyCalculatedData = result;
                    if (WACC != null)
                    {
                        YearlyCalculatedData = CalculateYearlyData(YearlyCalculatedData, WACC);
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
                BusyIndicatorNotification();
                dbInteractivity.RetrieveDCFTerminalValueCalculationsData(EntitySelectionData, RetrieveDCFTerminalValueCalculationsDataCallbackMethod);
                BusyIndicatorNotification(true, "Fetching Data for Terminal Value Calculations");
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }

        /// <summary>
        /// Consensus Estimate Data callback Method
        /// </summary>
        /// <param name="result"></param>
        public void RetrieveDCFAnalysisDataCallbackMethod(List<DCFAnalysisSummaryData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result, 1);
                    AnalysisSummaryData.Clear();
                    AnalysisSummaryData.AddRange(result);
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
                BusyIndicatorNotification();
                dbInteractivity.RetrieveCashFlows(EntitySelectionData, RetrieveDCFCashFlowYearlyDataCallbackMethod);
                BusyIndicatorNotification(true, "Fetching Cash Flow Data for Current Security");
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }

        /// <summary>
        /// Consensus Estimate Data callback Method
        /// </summary>
        /// <param name="result"></param>
        public void RetrieveDCFSummaryDataCallbackMethod(List<DCFSummaryData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result, 1);
                    SummaryData = result;
                    MinorityInvestments = SummaryData.Select(a => a.FVMinorities).FirstOrDefault();
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
                BusyIndicatorNotification();
                SetAnalysisSummaryDisplayData();
                dbInteractivity.RetrieveDCFFairValueData(EntitySelectionData, DCFFairValueCallbackMethod);
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }

        /// <summary>
        /// Consensus Estimate Data callback Method
        /// </summary>
        /// <param name="result"></param>
        public void RetrieveCurrentPriceDataCallbackMethod(decimal? result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result, 1);
                    CurrentMarketPrice = result;
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
                BusyIndicatorNotification();
                dbInteractivity.RetrieveDCFAnalysisData(EntitySelectionData, RetrieveDCFAnalysisDataCallbackMethod);
                BusyIndicatorNotification(true, "Fetching Data for Analysis Summary");
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }

        /// <summary>
        /// Fair-Value callback method
        /// </summary>
        /// <param name="fairValueData">Collection of Period_Financials</param>
        public void DCFFairValueCallbackMethod(List<PERIOD_FINANCIALS> fairValueData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (fairValueData != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, fairValueData, 1);
                    FairValueData = fairValueData;
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
        /// Store EPS fair value Callback Method
        /// </summary>
        /// <param name="result"></param>
        public void StoreEPSFairValueCallbackMethod(bool result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result)
                {
                    Prompt.ShowDialog("Values of P_E Uploaded Successfully");
                }
                else
                {
                    Prompt.ShowDialog("There was some problem in storing the Values of P_E");
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
        /// Store BVPS fair value Callback Method
        /// </summary>
        /// <param name="result"></param>
        public void StoreBVPSFairValueCallbackMethod(bool result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result)
                {
                    Prompt.ShowDialog("Values of P_BV Uploaded Successfully");
                }
                else
                {
                    Prompt.ShowDialog("There was some problem in storing the Values of P_BV");
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
        /// Fetch Country name callback method
        /// </summary>
        /// <param name="result">Country name</param>
        public void RetrieveCountryNameCallbackMethod(string result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result, 1);
                    CountryName = result;
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
        /// Delete Existing DCF Fair value data from the table for selected Security
        /// </summary>
        /// <param name="result">Result of the Operation</param>
        public void DeleteDCFFairValueCompleted(bool result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != false)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result, 1);
                    StoreEPSValue();
                    StoreBVPSValue();
                }
                else
                {
                    Logging.LogMethodParameterNull(logger, methodNamespace, 1);
                    Prompt.ShowDialog("There was some problem storing the DCF data");
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

        #region HelperMethods

        /// <summary>
        /// Busy Indicator Notification
        /// </summary>
        /// <param name="showBusyIndicator">Busy Indicator Running/Stopped: True/False</param>
        /// <param name="message">Message in Busy Indicator</param>
        public void BusyIndicatorNotification(bool showBusyIndicator = false, String message = null)
        {
            if (message != null)
            {
                BusyIndicatorContent = message;
            }
            BusyIndicatorIsBusy = showBusyIndicator;
        }

        /// <summary>
        /// Convert Data to Pivotted Form
        /// </summary>
        /// <param name="data">Set the Display Data</param>
        public void SetTerminalValueCalculationsDisplayData()
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                List<DCFDisplayData> result = new List<DCFDisplayData>();

                decimal cashFlow2020 = Convert.ToDecimal(YearlyCalculatedData.Where(a => a.PERIOD_YEAR == (DateTime.Today.AddYears(8).Year)).
                    Select(a => a.FREE_CASH_FLOW).FirstOrDefault());
                decimal sustainableROIC = Convert.ToDecimal(TerminalValueCalculationsData.Select(a => a.SustainableROIC).FirstOrDefault());
                decimal sustainableDPR = Convert.ToDecimal(TerminalValueCalculationsData.Select(a => a.SustainableDividendPayoutRatio).FirstOrDefault());
                decimal longTermNominalGDPGrowth = Convert.ToDecimal(TerminalValueCalculationsData.Select(a => a.LongTermNominalGDPGrowth).FirstOrDefault());
                decimal TGR;
                decimal terminalValueNominal = 0;
                decimal terminalValuePresent = 0;
                decimal discountingFactorY10 = Convert.ToDecimal(YearlyCalculatedData.Where(a => a.PERIOD_YEAR == (DateTime.Today.AddYears(9).Year)).
                    Select(a => a.DISCOUNTING_FACTOR).FirstOrDefault());
                TGR = (Math.Min(sustainableROIC * (Convert.ToDecimal(Convert.ToDecimal(1.0) - sustainableDPR)), longTermNominalGDPGrowth));

                result.Add(new DCFDisplayData() { PropertyName = "Cash Flow in 2020", Value = Math.Round(cashFlow2020, 1).ToString("N1") });
                result.Add(new DCFDisplayData() { PropertyName = "Sustainable ROIC", Value = Math.Round
                    (Convert.ToDecimal(sustainableROIC * Convert.ToDecimal(100)), 1).ToString() + " %" });
                result.Add(new DCFDisplayData() { PropertyName = "Sustainable Dividend Payout Ratio", Value = Math.Round
                    (Convert.ToDecimal(sustainableDPR * Convert.ToDecimal(100)), 1).ToString() + " %" });
                result.Add(new DCFDisplayData() { PropertyName = "Long-term Nominal GDP Growth", Value = Math.Round
                    (Convert.ToDecimal(longTermNominalGDPGrowth * Convert.ToDecimal(100)), 1).ToString() + " %" });
                result.Add(new DCFDisplayData() { PropertyName = "Terminal Growth Rate", Value = Math.Round
                    (Convert.ToDecimal(TGR * Convert.ToDecimal(100)), 1).ToString() + " %" });

                if (WACC < TGR)
                {
                    WACCLessTGR = true;
                    result.Add(new DCFDisplayData() { PropertyName = "Terminal Value (nominal)", Value = "WACC<TGR" });
                    result.Add(new DCFDisplayData() { PropertyName = "Terminal Value (present)", Value = "WACC<TGR" });
                }
                else
                {
                    WACCLessTGR = false;
                    terminalValueNominal = Convert.ToDecimal(DCFCalculations.CalculateNominalTerminalValue(WACC, TGR, cashFlow2020));
                    terminalValuePresent = Convert.ToDecimal(DCFCalculations.CalculatePresentTerminalValue(terminalValueNominal, discountingFactorY10));
                    result.Add(new DCFDisplayData() { PropertyName = "Terminal Value (nominal)", Value = Math.Round(terminalValueNominal, 0).ToString("N0") });
                    result.Add(new DCFDisplayData() { PropertyName = "Terminal Value (present)", Value = Math.Round(terminalValuePresent, 0).ToString("N0") });
                }

                TerminalValueCalculationsDisplayData.Clear();
                TerminalValueCalculationsDisplayData.AddRange(result);
                TerminalValuePresent = terminalValuePresent;
                CalculationParameters.Year9CashFlow = (Convert.ToDecimal(YearlyCalculatedData.Where(a => a.PERIOD_YEAR == (DateTime.Today.AddYears(8).Year)).
                    Select(a => a.FREE_CASH_FLOW).FirstOrDefault()));
                CalculationParameters.TerminalGrowthRate = TGR;
                CalculationParameters.Year10DiscountingFactor = (Convert.ToDecimal(YearlyCalculatedData.Where(a => a.PERIOD_YEAR == (DateTime.Today.AddYears(9).Year)).
                    Select(a => a.DISCOUNTING_FACTOR).FirstOrDefault()));
                CalculationParameters.CurrentMarketPrice = Convert.ToDecimal(CurrentMarketPrice);
                SetSummaryDisplayData();
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
        }

        /// <summary>
        /// Calculate Discounting Factor/PresentValues for 10 year period
        /// </summary>
        public List<DCFCashFlowData> CalculateYearlyData(List<DCFCashFlowData> periodData, decimal WACC)
        {
            try
            {
                DateTime currentDate = DateTime.Today;
                List<DateTime> endDates = new List<DateTime>();
                foreach (DCFCashFlowData item in periodData)
                {
                    DateTime endDate = new DateTime(item.PERIOD_YEAR, 12, 31);
                    TimeSpan timeSpan = endDate - currentDate;
                    item.DISCOUNTING_FACTOR = Convert.ToDecimal(Math.Pow(Convert.ToDouble(1 + WACC), Convert.ToDouble(timeSpan.Days / 365)));
                    if (item.DISCOUNTING_FACTOR != 0)
                    {
                        item.AMOUNT = item.FREE_CASH_FLOW / item.DISCOUNTING_FACTOR;
                    }
                    else
                    {
                        item.AMOUNT = 0;
                    }
                }
                PresentValueExplicitForecast = periodData.Select(a => Convert.ToDecimal(a.AMOUNT)).Sum();
                return periodData;
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
                return null;
            }
        }

        /// <summary>
        /// Convert Data to Pivotted Form
        /// </summary>
        /// <param name="data"></param>
        public void SetAnalysisSummaryDisplayData()
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                RangeObservableCollection<DCFDisplayData> result = new RangeObservableCollection<DCFDisplayData>();
                decimal costOfEquity;
                decimal weightOfEquity;
                decimal costOfDebt;
                decimal resultWACC;
                NumberFormatInfo provider = new NumberFormatInfo();

                result.Add(new DCFDisplayData() { PropertyName = "Market Risk Premium", Value = Convert.ToString(Math.Round
                    ((Convert.ToDecimal(AnalysisSummaryData.Select(a => a.MarketRiskPremium).FirstOrDefault() * 100)), 1)) + "%" });
                result.Add(new DCFDisplayData() { PropertyName = "Beta (*)", Value = Convert.ToString(Math.Round
                    (Convert.ToDecimal(AnalysisSummaryData.Select(a => a.Beta).FirstOrDefault()), 2)) });
                result.Add(new DCFDisplayData() { PropertyName = "Risk Free Rate", Value = Convert.ToString(Math.Round
                    (Convert.ToDecimal(AnalysisSummaryData.Select(a => a.RiskFreeRate).FirstOrDefault() * 100), 1)) + " %" });
                result.Add(new DCFDisplayData()
                {
                    PropertyName = "Stock Specific Discount",
                    Value = Convert.ToString(Math.Round(Convert.ToDecimal(StockSpecificDiscount), 1)) + "%"
                });

                result.Add(new DCFDisplayData() { PropertyName = "Marginal Tax Rate", Value = Convert.ToString(Math.Round
                    (Convert.ToDecimal(AnalysisSummaryData.Select(a => a.MarginalTaxRate).FirstOrDefault()), 1)) + "%" });


                costOfEquity = Convert.ToDecimal(AnalysisSummaryData.Select(a => a.Beta).FirstOrDefault()) *
                    Convert.ToDecimal(AnalysisSummaryData.Select(a => a.MarketRiskPremium).FirstOrDefault()) + 
                    Convert.ToDecimal(AnalysisSummaryData.Select(a => a.RiskFreeRate).FirstOrDefault()) +
                    Convert.ToDecimal(StockSpecificDiscount / Convert.ToDecimal(100));
                result.Add(new DCFDisplayData()
                {
                    PropertyName = "Cost of Equity",
                    Value = Convert.ToString(Math.Round(Convert.ToDecimal(costOfEquity * 100), 1)) + "%"
                });

                costOfDebt = Convert.ToDecimal(Math.Round(Convert.ToDecimal(AnalysisSummaryData.Select(a => a.CostOfDebt).FirstOrDefault() / 100), 4));
                result.Add(new DCFDisplayData() { PropertyName = "Cost of Debt", Value = Convert.ToString(Math.Round
                    (Convert.ToDecimal(costOfDebt) * 100, 1)) + "%" });
                result.Add(new DCFDisplayData() { PropertyName = "Market Cap", Value = (Math.Round
                    (Convert.ToDecimal(AnalysisSummaryData.Select(a => a.MarketCap).FirstOrDefault()), 0)).ToString("N1") });
                result.Add(new DCFDisplayData() { PropertyName = "Gross Debt", Value = Math.Round
                    (Convert.ToDecimal(AnalysisSummaryData.Select(a => a.GrossDebt).FirstOrDefault()), 0).ToString("N1") });
                if ((Convert.ToDecimal(Convert.ToDecimal(AnalysisSummaryData.Select(a => a.MarketCap).FirstOrDefault()) 
                    + Convert.ToDecimal(AnalysisSummaryData.Select(a => a.GrossDebt).FirstOrDefault())) == 0))
                {
                    weightOfEquity = 0;
                    resultWACC = 0;
                }
                else
                {
                    weightOfEquity = Convert.ToDecimal(AnalysisSummaryData.Select(a => a.MarketCap).FirstOrDefault()) / 
                        (Convert.ToDecimal(AnalysisSummaryData.Select(a => a.MarketCap).FirstOrDefault()) + 
                        Convert.ToDecimal(AnalysisSummaryData.Select(a => a.GrossDebt).FirstOrDefault()));
                    resultWACC = (weightOfEquity * costOfEquity) + ((1 - weightOfEquity) * ((costOfDebt) *
                        (1 - Convert.ToDecimal(AnalysisSummaryData.Select(a => a.MarginalTaxRate).FirstOrDefault()) / 100M)));
                }
                result.Add(new DCFDisplayData() { PropertyName = "Weight of Equity", Value = Convert.ToString(Math.Round
                    (Convert.ToDecimal(weightOfEquity * 100), 1)) + "%" });
                result.Add(new DCFDisplayData() { PropertyName = "WACC", Value = Convert.ToString(Math.Round
                    (Convert.ToDecimal(resultWACC * 100), 1)) + "%" });
                result.Add(new DCFDisplayData() { PropertyName = "Date", Value = (DateTime.Today.ToShortDateString()) });
                result.Add(new DCFDisplayData() { PropertyName = "Market Price", Value = Math.Round
                    (Convert.ToDecimal(CurrentMarketPrice), 2).ToString("N1") });
                AnalysisSummaryDisplayData = result;
                this.RaisePropertyChanged(() => this.AnalysisSummaryDisplayData);
                this.WACC = resultWACC;

                CalculationParameters.CostOfEquity = costOfEquity;
                CalculationParameters.CostOfDebt = costOfDebt;
                CalculationParameters.MarketCap = Convert.ToDecimal(AnalysisSummaryData.Select(a => a.MarketCap).FirstOrDefault());
                CalculationParameters.MarginalTaxRate = Convert.ToDecimal(AnalysisSummaryData.Select(a => a.MarginalTaxRate).FirstOrDefault()) / 100M;
                CalculationParameters.GrossDebtA = Convert.ToDecimal(AnalysisSummaryData.Select(a => a.GrossDebt).FirstOrDefault());
                SetTerminalValueCalculationsDisplayData();
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
        }

        /// <summary>
        /// Set Default Display Data
        /// </summary>
        /// <returns></returns>
        private RangeObservableCollection<DCFDisplayData> SetDefaultAnalysisDisplayData()
        {
            RangeObservableCollection<DCFDisplayData> result = new RangeObservableCollection<DCFDisplayData>();
            result.Add(new DCFDisplayData() { PropertyName = "Market Risk Premium" });
            result.Add(new DCFDisplayData() { PropertyName = "Beta (*)" });
            result.Add(new DCFDisplayData() { PropertyName = "Risk Free Rate" });
            result.Add(new DCFDisplayData() { PropertyName = "Stock Specific Discount" });
            result.Add(new DCFDisplayData() { PropertyName = "Marginal Tax Rate" });
            result.Add(new DCFDisplayData() { PropertyName = "Cost of Equity" });
            result.Add(new DCFDisplayData() { PropertyName = "Cost of Debt" });
            result.Add(new DCFDisplayData() { PropertyName = "Market Cap" });
            result.Add(new DCFDisplayData() { PropertyName = "Gross Debt" });
            result.Add(new DCFDisplayData() { PropertyName = "Weight of Equity" });
            result.Add(new DCFDisplayData() { PropertyName = "WACC" });
            result.Add(new DCFDisplayData() { PropertyName = "Date" });
            result.Add(new DCFDisplayData() { PropertyName = "Market Price" });
            return result;
        }

        /// <summary>
        /// Convert Data to Pivotted Form
        /// </summary>
        /// <param name="data">Set the Display Data</param>
        public void SetSummaryDisplayData()
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                List<DCFDisplayData> result = new List<DCFDisplayData>();
                decimal DCFValuePerShare;
                decimal FVMinorities;
                decimal totalEnterpriseValue = DCFCalculations.CalculateTotalEnterpriseValue(PresentValueExplicitForecast, Convert.ToDecimal(TerminalValuePresent));
                decimal cash = SummaryData.Select(a => a.Cash).FirstOrDefault();
                decimal FVInvestments = SummaryData.Select(a => a.FVInvestments).FirstOrDefault();
                decimal grossDebt = SummaryData.Select(a => a.GrossDebt).FirstOrDefault();
                FVMinorities = Convert.ToDecimal(MinorityInvestments);
                decimal equityValue = DCFCalculations.CalculateEquityValue(totalEnterpriseValue, cash, FVInvestments, grossDebt, FVMinorities);
                decimal numberOfShares = SummaryData.Select(a => a.NumberOfShares).FirstOrDefault();
                NumberOfShares = numberOfShares;
                if (numberOfShares != 0)
                {
                    DCFValuePerShare = DCFCalculations.CalculateDCFValuePerShare(equityValue, numberOfShares);
                }
                else
                {
                    DCFValuePerShare = 0;
                }
                decimal upsideDownside = DCFCalculations.CalculateUpsideValue(DCFValuePerShare, Convert.ToDecimal(CurrentMarketPrice));

                result.Add(new DCFDisplayData() { PropertyName = "Present Value of Explicit Forecast", Value = Math.Round
                    (Convert.ToDecimal(PresentValueExplicitForecast), 1).ToString("N1") });
                if (!WACCLessTGR)
                {
                    result.Add(new DCFDisplayData() { PropertyName = "Terminal Value", Value = Math.Round
                        (Convert.ToDecimal(TerminalValuePresent), 1).ToString("N1") });
                    result.Add(new DCFDisplayData() { PropertyName = "Total Enterprise Value", Value = Convert.ToString(Math.Round
                        (Convert.ToDecimal(totalEnterpriseValue), 1)) });
                }
                else
                {
                    result.Add(new DCFDisplayData() { PropertyName = "Terminal Value", Value = "WACC<TGR" });
                    result.Add(new DCFDisplayData() { PropertyName = "Total Enterprise Value", Value = "WACC<TGR" });
                }
                result.Add(new DCFDisplayData() { PropertyName = "(+) Cash", Value = Math.Round(Convert.ToDecimal(cash), 1).ToString("N1") });
                result.Add(new DCFDisplayData() { PropertyName = "(+) FV of Investments & Associates", Value = Math.Round
                    (Convert.ToDecimal(FVInvestments), 1).ToString("N1") });
                result.Add(new DCFDisplayData() { PropertyName = "(-) Gross Debt", Value = Math.Round
                    (Convert.ToDecimal(grossDebt), 1).ToString("N1") });
                result.Add(new DCFDisplayData() { PropertyName = "(-) FV of Minorities", Value = Math.Round
                    (Convert.ToDecimal(FVMinorities), 1).ToString("N1") });
                if (!WACCLessTGR)
                {
                    result.Add(new DCFDisplayData() { PropertyName = "Equity Value", Value = Math.Round
                        (Convert.ToDecimal(equityValue), 1).ToString("N1") });
                }
                else
                {
                    result.Add(new DCFDisplayData() { PropertyName = "Equity Value", Value = "WACC<TGR" });
                }
                result.Add(new DCFDisplayData() { PropertyName = "Number of Shares", Value = Math.Round
                    (Convert.ToDecimal(numberOfShares), 2).ToString("N1") });
                if (!WACCLessTGR)
                {
                    result.Add(new DCFDisplayData() { PropertyName = "DCF Value Per Share", Value = Convert.ToString(Math.Round
                        (Convert.ToDecimal(DCFValuePerShare), 2)) });
                    result.Add(new DCFDisplayData() { PropertyName = "Upside/Downside", Value = Convert.ToString(Math.Round
                        (Convert.ToDecimal(upsideDownside * 100), 1)) + "%" });
                }
                else
                {
                    result.Add(new DCFDisplayData() { PropertyName = "DCF Value Per Share", Value = "WACC<TGR" });
                    result.Add(new DCFDisplayData() { PropertyName = "Upside/Downside", Value = "WACC<TGR" });
                }
                SummaryDisplayData.Clear();
                SummaryDisplayData.AddRange(result);
                this.RaisePropertyChanged(() => this.SummaryData);

                CalculationParameters.Cash = cash;
                CalculationParameters.FutureValueOfInvestments = FVInvestments;
                CalculationParameters.FutureValueOfMinorties = FVMinorities;
                CalculationParameters.NumberOfShares = numberOfShares;
                CalculationParameters.PresentValueOfForecast = PresentValueExplicitForecast;
                CalculationParameters.GrossDebt = grossDebt;
                if (!WACCLessTGR)
                {
                    GenerateSensitivityDisplayData();
                }
                else
                {
                    ClearSensitivityGrids();
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
        }

        /// <summary>
        /// Generate Data bound for Sensitivity Gadget
        /// </summary>
        public void GenerateSensitivityDisplayData()
        {
            try
            {
                decimal costOfEquity = CalculationParameters.CostOfEquity;
                decimal terminalValueGrowth = CalculationParameters.TerminalGrowthRate;
                List<decimal> upSideValues = new List<decimal>();
                List<decimal> TGR = new List<decimal>();
                SensitivityDisplayData = new RangeObservableCollection<SensitivityData>();
                SensitivityData costOfEquityValues = SetFirstRow(CalculationParameters.CostOfEquity);
                if (costOfEquityValues != null)
                {
                    SensitivityDisplayData.Add(costOfEquityValues);
                }
                Dictionary<int, decimal> VPS = new Dictionary<int, decimal>();
                DCFValue result = new DCFValue();
                CalculationParameters.TerminalGrowthRate = Convert.ToDecimal((CalculationParameters.TerminalGrowthRate)) - Convert.ToDecimal(5.0 / 1000.0);
                for (int i = 0; i < 5; i++)
                {
                    CalculationParameters.CostOfEquity = Convert.ToDecimal((CalculationParameters.CostOfEquity)) - Convert.ToDecimal(5.0 / 1000.0);
                    SensitivityData data = new SensitivityData();
                    for (int j = 0; j < 5; j++)
                    {
                        result = new DCFValue();
                        result = DCFCalculations.CalculateDCFValue(CalculationParameters);
                        TGR.Add(result.DCFValuePerShare);
                        VPS.Add(j, result.DCFValuePerShare);
                        if (i == 2 && j == 2)
                        {
                            DCFValuePerShare = result.DCFValuePerShare;
                        }
                        upSideValues.Add(result.UpsideValue);
                        CalculationParameters.CostOfEquity = CalculationParameters.CostOfEquity + Convert.ToDecimal(25.0 / 10000.0);
                    }
                    data.C1 = (i + 1).ToString();
                    data.C2 = Math.Round((CalculationParameters.TerminalGrowthRate * 100), 2).ToString() + "%";
                    if (VPS.ContainsKey(0))
                    {
                        data.C3 = Convert.ToString(Math.Round(Convert.ToDecimal(VPS.Where(a => a.Key == 0).Select(a => a.Value).FirstOrDefault()), 2));
                    }
                    if (VPS.ContainsKey(1))
                    {
                        data.C4 = Convert.ToString(Math.Round(Convert.ToDecimal(VPS.Where(a => a.Key == 1).Select(a => a.Value).FirstOrDefault()), 2));
                    }
                    if (VPS.ContainsKey(2))
                    {
                        data.C5 = Convert.ToString(Math.Round(Convert.ToDecimal(VPS.Where(a => a.Key == 2).Select(a => a.Value).FirstOrDefault()), 2));
                    }
                    if (VPS.ContainsKey(3))
                    {
                        data.C6 = Convert.ToString(Math.Round(Convert.ToDecimal(VPS.Where(a => a.Key == 3).Select(a => a.Value).FirstOrDefault()), 2));
                    }
                    if (VPS.ContainsKey(4))
                    {
                        data.C7 = Convert.ToString(Math.Round(Convert.ToDecimal(VPS.Where(a => a.Key == 4).Select(a => a.Value).FirstOrDefault()), 2));
                    }
                    VPS = new Dictionary<int, decimal>();
                    SensitivityDisplayData.Add(data);
                    CalculationParameters.CostOfEquity = costOfEquity;
                    CalculationParameters.TerminalGrowthRate = CalculationParameters.TerminalGrowthRate + Convert.ToDecimal(25.0 / 10000.0);
                }
                MaxShareVal = Convert.ToString(Math.Round(TGR.Select(a => a).Max(), 2));
                MinShareVal = Convert.ToString((Math.Round(TGR.Select(a => a).Min(), 2)));
                AvgShareVal = Convert.ToString((Math.Round(TGR.Select(a => a).Average(), 2)));

                MaxUpside = Convert.ToString((Math.Round(upSideValues.Select(a => a).Max() * 100, 2))) + " %";
                MinUpside = Convert.ToString((Math.Round(upSideValues.Select(a => a).Min() * 100, 2))) + " %";
                AvgUpside = Convert.ToString((Math.Round(upSideValues.Select(a => a).Average() * 100, 2))) + " %";
                CalculationParameters.TerminalGrowthRate = terminalValueGrowth;
                SensitivityValues = SensitivityDisplayData;
                RangeObservableCollection<SensitivityData> dataL = new RangeObservableCollection<SensitivityData>();
                dataL.AddRange(SensitivityDisplayData.ToList());
                List<SensitivityData> dataEPS = ListUtils.GetDeepCopy<SensitivityData>(SensitivityValues.ToList());
                if (FWDEPS != 0)
                {
                    GenerateSensitivityEPSData(dataEPS);
                }
                if (FWDBVPS != 0)
                {
                    GenerateSensitivityBVPSData(dataEPS);
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
        }

        /// <summary>
        /// Generate Data for Display in SensitivityBVPS
        /// </summary>
        /// <param name="sensitivityData">Collection of Sensitivity Data from Sensitivity Grid</param>
        private void GenerateSensitivityBVPSData(List<SensitivityData> sensitivityData)
        {
            try
            {
                RangeObservableCollection<SensitivityData> dataBVPS = new RangeObservableCollection<SensitivityData>();
                foreach (SensitivityData item in sensitivityData)
                {
                    dataBVPS.Add(item);
                }
                dataBVPS.RemoveAt(0);
                char[] redundantData = new char[] { '%' };
                List<decimal> BVPS = new List<decimal>();
                if (FWDBVPS == 0)
                {
                    throw new Exception("FWD BVPS cannot be 0");
                }
                foreach (SensitivityData item in dataBVPS)
                {
                    item.C1 = "";
                    item.C2 = "";
                    item.C3 = item.C3.Trim(redundantData);
                    item.C4 = item.C4.Trim(redundantData);
                    item.C5 = item.C5.Trim(redundantData);
                    item.C6 = item.C6.Trim(redundantData);
                    item.C7 = item.C7.Trim(redundantData);
                }
                int i = 1;
                foreach (SensitivityData item in dataBVPS)
                {
                    item.C1 = i.ToString();
                    item.C2 = Math.Round(((CalculationParameters.TerminalGrowthRate - (0.005M - (i - 1) * (0.0025M))) * 100), 2).ToString() + "%";
                    item.C3 = Convert.ToString(Math.Round(Convert.ToDecimal((Convert.ToDecimal(item.C3) / FWDBVPS)), 2));
                    item.C4 = Convert.ToString(Math.Round(Convert.ToDecimal((Convert.ToDecimal(item.C4) / FWDBVPS)), 2));
                    item.C5 = Convert.ToString(Math.Round(Convert.ToDecimal((Convert.ToDecimal(item.C5) / FWDBVPS)), 2));
                    item.C6 = Convert.ToString(Math.Round(Convert.ToDecimal((Convert.ToDecimal(item.C6) / FWDBVPS)), 2));
                    item.C7 = Convert.ToString(Math.Round(Convert.ToDecimal((Convert.ToDecimal(item.C7) / FWDBVPS)), 2));
                    BVPS.Add(Convert.ToDecimal(item.C3));
                    BVPS.Add(Convert.ToDecimal(item.C4));
                    BVPS.Add(Convert.ToDecimal(item.C5));
                    BVPS.Add(Convert.ToDecimal(item.C6));
                    BVPS.Add(Convert.ToDecimal(item.C7));
                    i++;
                }
                SensitivityBVPS.Clear();
                SensitivityData costOfEquityValues = SetFirstRow(CalculationParameters.CostOfEquity);
                if (costOfEquityValues != null)
                {
                    SensitivityBVPS.Add(costOfEquityValues);
                }
                SensitivityBVPS.AddRange(dataBVPS.ToList());

                MaxBVPSShareVal = Convert.ToString(Math.Round(BVPS.Max(), 2));
                MinBVPSShareVal = Convert.ToString(Math.Round(BVPS.Min(), 2));
                AvgBVPSShareVal = Convert.ToString(Math.Round(BVPS.Average(), 2));
                if (CurrentMarketPrice != 0)
                {
                    MaxBVPSUpside = Convert.ToString(Math.Round(Convert.ToDecimal(Convert.ToDecimal(MaxBVPSShareVal) / CurrentMarketPrice - Convert.ToDecimal(1)) * 100, 2)) + "%";
                    MinBVPSUpside = Convert.ToString(Math.Round(Convert.ToDecimal(Convert.ToDecimal(MinBVPSShareVal) / CurrentMarketPrice - Convert.ToDecimal(1)) * 100, 2)) + "%";
                    AvgBVPSUpside = Convert.ToString(Math.Round(Convert.ToDecimal(Convert.ToDecimal(AvgBVPSShareVal) / CurrentMarketPrice - Convert.ToDecimal(1)) * 100, 2)) + "%";
                }
                else
                {
                    MaxBVPSUpside = " ";
                    MinBVPSUpside = " ";
                    AvgBVPSUpside = " ";
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
        }

        /// <summary>
        /// Generate data for Sensitivity EPS
        /// </summary>
        private void GenerateSensitivityEPSData(List<SensitivityData> sensitivityData)
        {
            try
            {
                RangeObservableCollection<SensitivityData> dataEPS = new RangeObservableCollection<SensitivityData>();
                foreach (SensitivityData item in sensitivityData)
                {
                    dataEPS.Add(item);
                }
                dataEPS.RemoveAt(0);
                char[] redundantData = new char[] { '%' };
                if (FWDEPS == 0)
                {
                    throw new Exception("FWD EPS cannot be 0");
                } 
                List<decimal> EPS = new List<decimal>();
                foreach (SensitivityData item in dataEPS)
                {
                    item.C1 = "";
                    item.C2 = "";

                    item.C3 = item.C3.Trim(redundantData);
                    item.C4 = item.C4.Trim(redundantData);
                    item.C5 = item.C5.Trim(redundantData);
                    item.C6 = item.C6.Trim(redundantData);
                    item.C7 = item.C7.Trim(redundantData);
                }
                int i = 1;
                foreach (SensitivityData item in dataEPS)
                {
                    item.C1 = i.ToString();
                    item.C2 = Math.Round(((CalculationParameters.TerminalGrowthRate - (0.005M - (i - 1) * (0.0025M))) * 100), 2).ToString() + "%";
                    item.C3 = Convert.ToString(Math.Round(Convert.ToDecimal((Convert.ToDecimal(item.C3) / FWDEPS)), 2));
                    item.C4 = Convert.ToString(Math.Round(Convert.ToDecimal((Convert.ToDecimal(item.C4) / FWDEPS)), 2));
                    item.C5 = Convert.ToString(Math.Round(Convert.ToDecimal((Convert.ToDecimal(item.C5) / FWDEPS)), 2));
                    item.C6 = Convert.ToString(Math.Round(Convert.ToDecimal((Convert.ToDecimal(item.C6) / FWDEPS)), 2));
                    item.C7 = Convert.ToString(Math.Round(Convert.ToDecimal((Convert.ToDecimal(item.C7) / FWDEPS)), 2));
                    EPS.Add(Convert.ToDecimal(item.C3));
                    EPS.Add(Convert.ToDecimal(item.C4));
                    EPS.Add(Convert.ToDecimal(item.C5));
                    EPS.Add(Convert.ToDecimal(item.C6));
                    EPS.Add(Convert.ToDecimal(item.C7));
                    i++;
                }
                SensitivityBPS.Clear();
                SensitivityData costOfEquityValues = SetFirstRow(CalculationParameters.CostOfEquity);
                if (costOfEquityValues != null)
                {
                    SensitivityBPS.Add(costOfEquityValues);
                }

                SensitivityBPS.AddRange(dataEPS.ToList());
                MaxEPSShareVal = Convert.ToString(Math.Round(EPS.Max(), 2));
                MinEPSShareVal = Convert.ToString(Math.Round(EPS.Min(), 2));
                AvgEPSShareVal = Convert.ToString(Math.Round(EPS.Average(), 2));

                if (CurrentMarketPrice != 0)
                {
                    MaxEPSUpside = Convert.ToString(Math.Round(Convert.ToDecimal(Convert.ToDecimal(MaxEPSShareVal) / CurrentMarketPrice - Convert.ToDecimal(1)) * 100, 2)) + "%";
                    MinEPSUpside = Convert.ToString(Math.Round(Convert.ToDecimal(Convert.ToDecimal(MinEPSShareVal) / CurrentMarketPrice - Convert.ToDecimal(1)) * 100, 2)) + "%";
                    AvgEPSUpside = Convert.ToString(Math.Round(Convert.ToDecimal(Convert.ToDecimal(AvgEPSShareVal) / CurrentMarketPrice - Convert.ToDecimal(1)) * 100, 2)) + "%";
                }
                else
                {
                    MaxEPSUpside = " ";
                    MinEPSUpside = " ";
                    AvgEPSUpside = " ";
                }
                dataEPS.Clear();
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
        }

        /// <summary>
        /// Method to generate Intial Row in DCF Sensitivity
        /// </summary>
        /// <param name="costOfEquity">Cost of Equity</param>
        /// <returns>Instance of SensitivityData</returns>
        private SensitivityData SetFirstRow(decimal costOfEquity)
        {
            try
            {
                SensitivityData result = new SensitivityData();
                decimal initialDiff = 0.005M;
                decimal step = 0.0025M;
                decimal initialCostOfEquity = costOfEquity - initialDiff;
                result.C1 = "T.G.R.";
                result.C2 = "";
                result.C3 = Math.Round(((initialCostOfEquity) * 100), 2).ToString() + "%";
                result.C4 = Math.Round(((initialCostOfEquity + (1 * step)) * 100), 2).ToString() + "%";
                result.C5 = Math.Round(((initialCostOfEquity + (2 * step)) * 100), 2).ToString() + "%";
                result.C6 = Math.Round(((initialCostOfEquity + (3 * step)) * 100), 2).ToString() + "%";
                result.C7 = Math.Round(((initialCostOfEquity + (4 * step)) * 100), 2).ToString() + "%";
                return result;
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
                return null;
            }
        }

        /// <summary>
        /// Clear Sensitivity Grids when WACC is less then TGR
        /// </summary>
        private void ClearSensitivityGrids()
        {
            SensitivityValues = new RangeObservableCollection<SensitivityData>();
            SensitivityBVPS = new RangeObservableCollection<SensitivityData>();
            SensitivityBPS = new RangeObservableCollection<SensitivityData>();
            MaxShareVal = string.Empty;
            MinShareVal = string.Empty;
            AvgShareVal = string.Empty;
            MaxUpside = string.Empty;
            MinUpside = string.Empty;
            AvgUpside = string.Empty;
            MaxBVPSShareVal = string.Empty;
            MinBVPSShareVal = string.Empty;
            AvgBVPSShareVal = string.Empty;
            MaxBVPSUpside = string.Empty;
            MinBVPSUpside = string.Empty;
            AvgBVPSUpside = string.Empty;
            MaxEPSShareVal = string.Empty;
            MinEPSShareVal = string.Empty;
            AvgEPSShareVal = string.Empty;
            MaxEPSUpside = string.Empty;
            MinEPSUpside = string.Empty;
            AvgEPSUpside = string.Empty;
        }

        #region FairValue

        /// <summary>
        /// Store EPS Value
        /// </summary>
        private void StoreBVPSValue()
        {
            try
            {
                decimal? nCurrentPBV;
                decimal? upside;
                if (FWDBVPS != 0)
                {
                    if (DCFValuePerShare != null)
                    {
                        if (FairValueData != null)
                        {
                            if (FairValueData.Count != 0)
                            {
                                nCurrentPBV = FairValueData.Where(a => a.DATA_ID == 188).Select(a => a.AMOUNT).FirstOrDefault();
                            }
                            else
                            {
                                nCurrentPBV = 0;
                            }
                        }
                        else
                        {
                            nCurrentPBV = 0;
                        }
                        decimal? FV_Sell = DCFValuePerShare / FWDBVPS;
                        if (nCurrentPBV != 0)
                        {
                            upside = DCFValuePerShare / nCurrentPBV - 1M;
                        }
                        else
                        {
                            upside = 0;
                        }
                        dbInteractivity.InsertDCFFairValueData(EntitySelectionData, "DCF_PBV", 188, 0, FV_Sell, nCurrentPBV, upside, DateTime.Now, StoreEPSFairValueCallbackMethod);
                    }
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
        }

        /// <summary>
        /// Store EPS Value
        /// </summary>
        private void StoreEPSValue()
        {
            try
            {
                decimal? nCurrentPE;
                decimal? upside;
                if (FWDEPS != 0)
                {
                    if (DCFValuePerShare != null)
                    {
                        if (FairValueData != null)
                        {
                            if (FairValueData.Count != 0)
                            {
                                nCurrentPE = FairValueData.Where(a => a.DATA_ID == 187).Select(a => a.AMOUNT).FirstOrDefault();
                            }
                            else
                            {
                                nCurrentPE = 0;
                            }
                        }
                        else
                        {
                            nCurrentPE = null;
                        }
                        decimal? FV_Sell = DCFValuePerShare / FWDEPS;
                        if (nCurrentPE != 0)
                        {
                            upside = DCFValuePerShare / nCurrentPE - 1M;
                        }
                        else
                        {
                            upside = 0;
                        }
                        dbInteractivity.InsertDCFFairValueData(EntitySelectionData, "DCF_PE", 187, 0, FV_Sell, nCurrentPE, upside, DateTime.Now, StoreBVPSFairValueCallbackMethod);
                    }
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
        }

        /// <summary>
        /// Delete Existing Fair Values Data
        /// </summary>
        private void DeleteExistingFairValues()
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                dbInteractivity.DeleteDCFFairValue(EntitySelectionData, DeleteDCFFairValueCompleted);
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
        }

        #endregion

        #endregion
    }
}