using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Windows;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Logging;
using GreenField.Gadgets.Helpers;
using GreenField.Common;
using GreenField.DataContracts;
using GreenField.ServiceCaller;

namespace GreenField.Gadgets.ViewModels
{
    /// <summary>
    /// View-Model for PortfolioDetailsUI
    /// </summary>
    public class ViewModelPortfolioDetails : NotificationObject
    {
        #region PrivateFields

        /// <summary>
        /// Event Aggregator
        /// </summary>
        private IEventAggregator eventAggregator;

        /// <summary>
        /// Instance of IDBInteractivity
        /// </summary>
        private IDBInteractivity dbInteractivity;
        
        /// <summary>
        /// Logger Facade
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

        /// <summary>
        /// Selected portfolio
        /// </summary>
        private PortfolioSelectionData portfolioSelectionData;
        
        /// <summary>
        /// Selected Effective Date
        /// </summary>
        private DateTime? effectiveDate;

        /// <summary>
        /// Look-Thru status
        /// </summary>
        private bool lookThruEnabled = false;
        
        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dbInteractivity">Instance of service caller class</param>
        /// <param name="eventAggregator"></param>
        /// <param name="logger">Instance of LoggerFacade</param>
        public ViewModelPortfolioDetails(DashboardGadgetParam param)
        {
            this.dbInteractivity = param.DBInteractivity;
            this.eventAggregator = param.EventAggregator;
            this.logger = param.LoggerFacade;
            portfolioSelectionData = param.DashboardGadgetPayload.PortfolioSelectionData;
            SelectedPortfolioId = portfolioSelectionData;
            effectiveDate = param.DashboardGadgetPayload.EffectiveDate;
            ExcludeCashSecurities = param.DashboardGadgetPayload.IsExCashSecurityData;
            lookThruEnabled = param.DashboardGadgetPayload.IsLookThruEnabled;

            if (eventAggregator != null && effectiveDate != null && SelectedPortfolioId != null && IsActive)
            {
                BusyIndicatorStatus = true;
                dbInteractivity.RetrievePortfolioDetailsData(SelectedPortfolioId, Convert.ToDateTime(effectiveDate), lookThruEnabled, ExcludeCashSecurities, false, RetrievePortfolioDetailsDataCallbackMethod);
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

        #region PropertiesDeclaration

        /// <summary>
        /// The Portfolio selected from the top menu Portfolio Selector Control
        /// </summary>
        private PortfolioSelectionData selectedPortfolioId;
        public PortfolioSelectionData SelectedPortfolioId
        {
            get
            {
                return selectedPortfolioId;
            }
            set
            {
                selectedPortfolioId = value;
                CheckFilterApplied = 1;
                this.RaisePropertyChanged(() => this.SelectedPortfolioDetailsData);
            }
        }

        /// <summary>
        /// Collection Containing the Data to be shown in the Grid
        /// </summary>
        private RangeObservableCollection<PortfolioDetailsData> selectedPortfolioDetailsData = new RangeObservableCollection<PortfolioDetailsData>();
        public RangeObservableCollection<PortfolioDetailsData> SelectedPortfolioDetailsData
        {
            get
            {
                return selectedPortfolioDetailsData;
            }
            set
            {
                selectedPortfolioDetailsData = value;
                this.RaisePropertyChanged(() => this.SelectedPortfolioDetailsData);
            }
        }

       

        /// <summary>
        /// 
        /// </summary>
        private RangeObservableCollection<PortfolioDetailsData> groupedFilteredPortfolioDetailsData;
        public RangeObservableCollection<PortfolioDetailsData> GroupedFilteredPortfolioDetailsData
        {
            get
            {
                return groupedFilteredPortfolioDetailsData;
            }
            set
            {
                if (value != null)
                {
                    groupedFilteredPortfolioDetailsData = value;
                    if (value.Count > 0)
                    {
                        ReturnGroupedColumnData();
                    }
                    this.RaisePropertyChanged(() => this.GroupedFilteredPortfolioDetailsData);
                }
            }
        }

        /// <summary>
        /// Base column for grouping
        /// </summary>
        private string groupingColumn;
        public string GroupingColumn
        {
            get
            {
                return groupingColumn;
            }
            set
            {
                groupingColumn = value;
                this.RaisePropertyChanged(() => this.GroupingColumn);
            }
        }


        /// <summary>
        /// Base Portfolio Data Collection
        /// </summary>
        private RangeObservableCollection<PortfolioDetailsData> basePortfolioData;
        public RangeObservableCollection<PortfolioDetailsData> BasePortfolioData
        {
            get
            {
                return basePortfolioData;
            }
            set
            {
                basePortfolioData = value;
                this.RaisePropertyChanged(() => this.BasePortfolioData);
            }
        }

        /// <summary>
        /// Bool to check whether to get Data of Benchmark
        /// </summary>
        private bool getBenchmarkData;
        public bool GetBenchmarkData
        {
            get
            {
                return getBenchmarkData;
            }
            set
            {
                getBenchmarkData = value;
                if (SelectedPortfolioId != null && effectiveDate != null && IsActive)
                {
                    BusyIndicatorStatus = true;
                    CheckFilterApplied = 1;
                    RetrievePortfolioDetailsData(SelectedPortfolioId, Convert.ToDateTime(effectiveDate), GetBenchmarkData, RetrievePortfolioDetailsDataCallbackMethod);
                }
                this.RaisePropertyChanged(() => this.GetBenchmarkData);
            }
        }

        /// <summary>
        /// Bool to check whether to group holdings into one record or not
        /// </summary>
        private bool isHoldingsGrouped;
        public bool IsHoldingsGrouped
        {
            get
            {
                return isHoldingsGrouped;
            }
            set
            {
                isHoldingsGrouped = value;
                if (EnableLookThru)
                {
                    if (isHoldingsGrouped)
                    {
                        if (groupedData != null)
                        {
                            SelectedPortfolioDetailsData = new RangeObservableCollection<PortfolioDetailsData>(groupedData);
                        }
                    }
                    else
                    {
                        if (initialData != null)
                            SelectedPortfolioDetailsData = new RangeObservableCollection<PortfolioDetailsData>(initialData);
                    }
                }
                this.RaisePropertyChanged(() => this.IsHoldingsGrouped);
            }
        }

        /// <summary>
        /// Collection of all Benchmark Names
        /// </summary>
        private ObservableCollection<string> benchmarkNamesData;
        public ObservableCollection<string> BenchmarkNamesData
        {
            get
            {
                return benchmarkNamesData;
            }
            set
            {
                benchmarkNamesData = value;
                this.RaisePropertyChanged(() => this.BenchmarkNamesData);
            }
        }

        /// <summary>
        /// Property Bind to the BenchmarkSelectionComboBox
        /// </summary>
        private string selectedBenchmark;
        public string SelectedBenchmark
        {
            get
            {
                return selectedBenchmark;
            }
            set
            {
                selectedBenchmark = value;
                this.RaisePropertyChanged(() => this.SelectedBenchmark);
            }
        }

        /// <summary>
        /// Selected date from the application toolbar.
        /// </summary>
        private DateTime selectedDate = DateTime.Today;
        public DateTime SelectedDate
        {
            get
            {
                return selectedDate;
            }
            set
            {
                selectedDate = value;
                this.RaisePropertyChanged(() => this.SelectedDate);
            }
        }

        /// <summary>
        /// Check whether applying/removing grouping
        /// </summary>
        private bool groupingStatus = false;
        public bool GroupingStatus
        {
            get
            {
                return groupingStatus;
            }
            set
            {
                groupingStatus = value;
                this.RaisePropertyChanged(() => this.GroupingStatus);
            }
        }

        /// <summary>
        /// Busy Indicator for Portfolio Details
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
                CheckFilterApplied = 1;
                this.RaisePropertyChanged(() => this.ExcludeCashSecurities);
            }
        }

        /// <summary>
        /// Check to include LookThru or Not
        /// </summary>
        private bool enableLookThru;
        public bool EnableLookThru
        {
            get { return enableLookThru; }
            set
            {
                enableLookThru = value;
                CheckFilterApplied = 1;
                this.RaisePropertyChanged(() => this.EnableLookThru);
            }
        }

        /// <summary>
        /// Filter Descriptor in Grid
        /// </summary>
        private string filterDescriptor = " ";
        public string FilterDescriptor
        {
            get
            {
                return filterDescriptor;
            }
            set
            {
                filterDescriptor = value;
                this.RaisePropertyChanged(() => this.FilterDescriptor);
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
                if (SelectedPortfolioId != null && effectiveDate != null && isActive)
                {
                    RetrievePortfolioDetailsData(SelectedPortfolioId, Convert.ToDateTime(effectiveDate), GetBenchmarkData, RetrievePortfolioDetailsDataCallbackMethod);
                }
            }
        }

        /// <summary>
        /// Get Benchmark Securities
        /// </summary>
        private bool getBenchmarkCheck;
        public bool GetBenchmarkCheck
        {
            get
            {
                return getBenchmarkCheck;
            }
            set
            {
                getBenchmarkCheck = value;

                this.RaisePropertyChanged(() => this.GetBenchmarkCheck);
            }
        }

        /// <summary>
        /// Filter Unique Value
        /// </summary>
        private string filterUniqueValue;
        public string FilterUniqueValue
        {
            get
            {
                return filterUniqueValue;
            }
            set
            {
                filterUniqueValue = value;
                this.RaisePropertyChanged(() => this.FilterUniqueValue);
            }
        }

        /// <summary>
        /// Property to check if Filter is applied
        /// </summary>
        private int checkFilterApplied;
        public int CheckFilterApplied
        {
            get
            {
                return checkFilterApplied;
            }
            set
            {
                checkFilterApplied = value;
                this.RaisePropertyChanged(() => this.CheckFilterApplied);
            }
        }

        #endregion

        #region CallbackMethods

        private List<PortfolioDetailsData> initialData;
        private List<PortfolioDetailsData> groupedData;

        /// <summary>
        /// CallBack Method for Retrieving Portfolio Names
        /// </summary>
        /// <param name="result"></param>
        private void RetrievePortfolioDetailsDataCallbackMethod(List<PortfolioDetailsData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result, 1);
                    initialData = CalculatePortfolioValues(result).OrderBy(a => a.ActivePosition).ThenBy(a => a.PortfolioWeight).ToList();
                    if (this.EnableLookThru)
                    {
                        groupedData = GetGroupedPortfolios(SelectedPortfolioId.PortfolioId, initialData);
                        if (isHoldingsGrouped)
                        {
                            SelectedPortfolioDetailsData = new RangeObservableCollection<PortfolioDetailsData>(groupedData);
                        }
                        else
                        {
                            SelectedPortfolioDetailsData = new RangeObservableCollection<PortfolioDetailsData>(initialData);
                        }
                    }
                    else
                    {
                        SelectedPortfolioDetailsData = new RangeObservableCollection<PortfolioDetailsData>(initialData);
                    }
                    
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

        private List<PortfolioDetailsData> GetGroupedPortfolios(string portfolioId, List<PortfolioDetailsData> list)
        {
            var result = new List<PortfolioDetailsData>();
            var query = from d in list
                        group d by d.AsecSecShortName into grp
                        select grp;
            var groups = query.ToList();

            foreach (var group in groups)
            {
                var main = group.Where(x => x.PortfolioPath == portfolioId).FirstOrDefault();
                if (main == null || group.Count() == 1)
                {
                    result.AddRange(group.AsEnumerable());
                }
                else
                {
                    var holding = new PortfolioDetailsData
                    {
                        A_Sec_Instr_Type = group.First().A_Sec_Instr_Type,
                        ActivePosition = group.Sum(x => x.ActivePosition ?? 0.0m),
                        AsecSecShortName = group.Key,
                        AshEmmModelWeight = group.Sum(x => x.AshEmmModelWeight ?? 0.0m),
                        BalanceNominal = group.Sum(x => x.BalanceNominal ?? 0.0m),
                        BenchmarkWeight = main.BenchmarkWeight,
                        DirtyValuePC = group.Sum(x => x.DirtyValuePC ?? 0.0m),
                        ForwardEB_EBITDA = main.ForwardEB_EBITDA,
                        ForwardPE = main.ForwardPE,
                        ForwardPBV = main.ForwardPBV,
                        FreecashFlowMargin = main.FreecashFlowMargin,
                        FromDate = main.FromDate,
                        IndustryName = main.IndustryName,
                        IsoCountryCode = main.IsoCountryCode,
                        IssueName = main.IssueName,
                        IssuerId = main.IssuerId,
                        MarketCap = main.MarketCap,
                        MarketCapUSD = main.MarketCapUSD,
                        NetDebtEquity = main.NetDebtEquity,
                        NetIncomeGrowthCurrentYear = main.NetIncomeGrowthCurrentYear,
                        NetIncomeGrowthNextYear = main.NetIncomeGrowthNextYear,
                        PfcHoldingPortfolio = String.Join(", ", group.Select(x => x.PfcHoldingPortfolio ).ToArray()),
                        PortfolioDirtyValuePC = group.Sum(x => x.PortfolioDirtyValuePC),
                        PortfolioPath = null,
                        PortfolioWeight = group.Sum(x => x.PortfolioWeight ?? 0.0m),
                        ProprietaryRegionCode = main.ProprietaryRegionCode,
                        ReAshEmmModelWeight = group.Sum(x => x.ReAshEmmModelWeight ?? 0.0m),
                        RePortfolioWeight = group.Sum(x => x.RePortfolioWeight ?? 0.0m),
                        ReBenchmarkWeight = main.ReBenchmarkWeight,
                        RevenueGrowthCurrentYear = main.RevenueGrowthCurrentYear,
                        RevenueGrowthNextYear = main.RevenueGrowthNextYear,
                        ROE = main.ROE,
                        SectorName = main.SectorName,
                        SecurityId = main.SecurityId,
                        SecurityThemeCode = main.SecurityThemeCode,
                        SecurityType = main.SecurityType,
                        SubIndustryName = main.SubIndustryName,
                        Ticker = main.Ticker,
                        TradingCurrency = main.TradingCurrency,
                        Type = main.Type,
                        Upside = main.Upside,
                        IsExpanded = true,
                        Children = group.ToList()

                    };
                    result.Add(holding);
                }
            }

            return result;
        }

        #endregion

        #region HelperMethods

        /// <summary>
        /// Service call to Retrieve the Details for propertyName Portfolio
        /// </summary>
        /// <param name="objPortfolioId">PortfolioName</param>
        private void RetrievePortfolioDetailsData(PortfolioSelectionData objPortfolioId, DateTime objSelectedDate, bool objgetBenchmark, Action<List<PortfolioDetailsData>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (objPortfolioId != null && objSelectedDate != null && dbInteractivity != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, objSelectedDate, 1);
                    Logging.LogMethodParameter(logger, methodNamespace, objPortfolioId, 1);
                    dbInteractivity.RetrievePortfolioDetailsData(objPortfolioId, objSelectedDate, EnableLookThru, ExcludeCashSecurities, GetBenchmarkData, callback);
                    BusyIndicatorStatus = true;
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
        /// Helper methods to Re-Calculate Weights while Grouping
        /// </summary>
        /// <param name="objGroupingColumnName">Column on the basis of which Grouping is Done</param>
        private void ReturnGroupedColumnData()
        {
            try
            {
                if (GroupedFilteredPortfolioDetailsData != null)
                {
                    BasePortfolioData = GroupedFilteredPortfolioDetailsData;

                    decimal? sumBenchmarkWeight = (from p in BasePortfolioData
                                                   select p.BenchmarkWeight).ToList().Sum();
                    decimal? sumDirtyValuePC = (from p in BasePortfolioData
                                                select p.DirtyValuePC).ToList().Sum();
                    decimal? sumAshEmmModelWeight = (from p in BasePortfolioData
                                                     select p.AshEmmModelWeight).ToList().Sum();

                    foreach (PortfolioDetailsData data in SelectedPortfolioDetailsData)
                    {
                        if (sumDirtyValuePC != 0)
                            data.RePortfolioWeight = data.DirtyValuePC / sumDirtyValuePC * 100;

                        if (sumBenchmarkWeight != 0)
                            data.ReBenchmarkWeight = data.BenchmarkWeight / sumBenchmarkWeight * 100;

                        if (sumAshEmmModelWeight != 0)
                            data.ReAshEmmModelWeight = data.AshEmmModelWeight / sumAshEmmModelWeight * 100;

                        data.ActivePosition = Convert.ToDecimal(data.RePortfolioWeight) - Convert.ToDecimal(data.ReBenchmarkWeight);
                    }

                    List<PortfolioDetailsData> collection = new List<PortfolioDetailsData>(SelectedPortfolioDetailsData);
                    SelectedPortfolioDetailsData = new RangeObservableCollection<PortfolioDetailsData>
                        (collection.OrderBy(a => a.ActivePosition).ThenBy(a => a.PortfolioWeight).ToList());
                    collection.Clear();
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
        }

        /// <summary>
        /// Calculations for Portfolio Details UI
        /// </summary>
        /// <param name="portfolioDetailsData">Collection of Portfolio Details Data</param>
        /// <returns>Collection of PortfolioDetailsData</returns>
        private List<PortfolioDetailsData> CalculatePortfolioValues(List<PortfolioDetailsData> portfolioDetailsData)
        {
            try
            {
                if (portfolioDetailsData == null)
                    return new List<PortfolioDetailsData>();
                if (portfolioDetailsData.Count == 0)
                    return new List<PortfolioDetailsData>();

                decimal sumDirtyValuePC = 0;
                decimal sumModelWeight = 0;

                sumDirtyValuePC = portfolioDetailsData.Sum(a => Convert.ToDecimal(a.DirtyValuePC));
                sumModelWeight = portfolioDetailsData.Sum(a => Convert.ToDecimal(a.AshEmmModelWeight));

                if (sumDirtyValuePC == 0 && sumModelWeight == 0)
                    return portfolioDetailsData;

                foreach (PortfolioDetailsData item in portfolioDetailsData)
                {
                    if (sumDirtyValuePC != 0)
                    {
                        item.PortfolioWeight = item.DirtyValuePC / sumDirtyValuePC * 100;
                        item.RePortfolioWeight = item.PortfolioWeight;
                        item.ActivePosition = item.PortfolioWeight - item.BenchmarkWeight;
                    }
                    else
                    {
                        item.RePortfolioWeight = 0;
                        item.ActivePosition = 0;
                    }

                    if (sumModelWeight != 0)
                    {
                        item.AshEmmModelWeight = item.AshEmmModelWeight / sumModelWeight * 100;
                        item.ReAshEmmModelWeight = item.AshEmmModelWeight;
                    }
                    else
                    {
                        item.ReAshEmmModelWeight = 0;
                    }

                    item.ReBenchmarkWeight = item.BenchmarkWeight;
                }
                if (portfolioDetailsData != null)
                {
                    return portfolioDetailsData;
                }
                else
                {
                    return new List<PortfolioDetailsData>();
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
                return new List<PortfolioDetailsData>();
            }
        }

        #endregion

        #region EventHandlers

        /// <summary>
        /// Event handler for FundSelection changed Event
        /// </summary>
        /// <param name="portfolioSelectionData"></param>
        public void HandlePortfolioReferenceSet(PortfolioSelectionData portfolioSelectionData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                //arguement null exception
                if (portfolioSelectionData != null)
                {
                    SelectedPortfolioId = portfolioSelectionData;
                    if (SelectedPortfolioId != null && effectiveDate != null && IsActive)
                    {
                        BusyIndicatorStatus = true;
                        RetrievePortfolioDetailsData(SelectedPortfolioId, Convert.ToDateTime(effectiveDate), GetBenchmarkData, RetrievePortfolioDetailsDataCallbackMethod);
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
                    if (effectiveDate != null && SelectedPortfolioId != null && IsActive)
                    {
                        BusyIndicatorStatus = true;
                        dbInteractivity.RetrievePortfolioDetailsData(SelectedPortfolioId, Convert.ToDateTime(effectiveDate), EnableLookThru, ExcludeCashSecurities, false, RetrievePortfolioDetailsDataCallbackMethod);
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
                ExcludeCashSecurities = isExCashSec;

                if (dbInteractivity != null && SelectedPortfolioId != null && effectiveDate != null && IsActive)
                {
                    BusyIndicatorStatus = true;
                    RetrievePortfolioDetailsData(SelectedPortfolioId, Convert.ToDateTime(effectiveDate), GetBenchmarkData, RetrievePortfolioDetailsDataCallbackMethod);
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
                EnableLookThru = enableLookThru;

                if (dbInteractivity != null && SelectedPortfolioId != null && effectiveDate != null && IsActive)
                {
                    BusyIndicatorStatus = true;
                    RetrievePortfolioDetailsData(SelectedPortfolioId, Convert.ToDateTime(effectiveDate), GetBenchmarkData, RetrievePortfolioDetailsDataCallbackMethod);
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

        #region EventUnsubscribe

        /// <summary>
        /// Unsubscribe Events
        /// </summary>
        public void Dispose()
        {
            if (eventAggregator != null)
            {
                eventAggregator.GetEvent<PortfolioReferenceSetEvent>().Unsubscribe(HandlePortfolioReferenceSet);
                eventAggregator.GetEvent<EffectiveDateReferenceSetEvent>().Unsubscribe(HandleEffectiveDateSet);
                eventAggregator.GetEvent<ExCashSecuritySetEvent>().Unsubscribe(HandleExCashSecuritySetEvent);
                eventAggregator.GetEvent<LookThruFilterReferenceSetEvent>().Unsubscribe(HandleLookThruReferenceSetEvent);
            }
        }

        #endregion
    }
}