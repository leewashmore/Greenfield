using System;
using System.Windows;
using System.Windows.Input;
using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Prism.Commands;
using System.Windows.Browser;
using GreenField.Common;
using GreenField.ServiceCaller;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Logging;
using GreenField.Gadgets.Views;
using System.Windows.Data;
using System.Collections.ObjectModel;
using GreenField.ServiceCaller.SecurityReferenceDefinitions;
using GreenField.App.Models;
using GreenField.Gadgets.ViewModels;
using GreenField.Common.Helper;
using GreenField.App.Helpers;
using GreenField.ServiceCaller.BenchmarkHoldingsPerformanceDefinitions;

namespace GreenField.App.ViewModel
{
    /// <summary>
    /// View model class for Shell
    /// </summary>
    [Export]
    public class ViewModelShell : NotificationObject
    {
        #region Fields
        /// <summary>
        /// MEF Singletons
        /// </summary>
        private IRegionManager _regionManager;
        private IManageSessions _manageSessions;
        private ILoggerFacade _logger;
        private IEventAggregator _eventAggregator;
        private IDBInteractivity _dbInteractivity;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="regionManager">Prism IRegionManager</param>
        /// <param name="manageSessions">Service IManageSessions</param>
        /// <param name="logger">Service ILoggerFacade</param>
        /// <param name="eventAggregator">Prism IEventAggregator</param>
        /// <param name="dbInteractivity">Service IDBInteractivity</param>
        [ImportingConstructor]
        public ViewModelShell(IRegionManager regionManager, IManageSessions manageSessions,
            ILoggerFacade logger, IEventAggregator eventAggregator, IDBInteractivity dbInteractivity)
        {
            _logger = logger;
            _regionManager = regionManager;
            _manageSessions = manageSessions;
            _eventAggregator = eventAggregator;
            _dbInteractivity = dbInteractivity;


            if (_manageSessions != null)
            {
                try
                {
                    _manageSessions.GetSession((session) =>
                            {
                                if (session != null)
                                {
                                    SessionManager.SESSION = session;
                                    UserName = SessionManager.SESSION.UserName;
                                    Logging.LogSessionStart(_logger);
                                    if (_dbInteractivity != null)
                                    {
                                        _dbInteractivity.RetrieveEntitySelectionData(RetrieveEntitySelectionDataCallbackMethod);
                                        _dbInteractivity.RetrievePortfolioSelectionData(RetrievePortfolioSelectionDataCallbackMethod);
                                        //_dbInteractivity.RetrieveBenchmarkSelectionData(RetrieveBenchmarkSelectionDataCallBackMethod);
                                    }
                                }
                            });
                }
                catch (Exception ex)
                {
                    string StackTrace = Logging.StackTraceToString(ex);
                    MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + StackTrace, "Exception", MessageBoxButton.OK);
                    Logging.LogException(_logger, ex);
                }
            }

        }
        #endregion

        # region Properties

        #region UI Fields
        /// <summary>
        /// Property binding UserName TextBlock
        /// </summary>
        private string _userName;
        public string UserName
        {
            get
            {
                if (_userName == null)
                {
                    _userName = SessionManager.SESSION != null ? SessionManager.SESSION.UserName : null;
                }
                return _userName;
            }
            set
            {
                if (_userName != value)
                    _userName = value;
                RaisePropertyChanged(() => this.UserName);
            }
        }

        #region Payload
        /// <summary>
        /// Stores payload to be published through aggregate events
        /// </summary>
        private DashboardGadgetPayload _selectorPayload;
        public DashboardGadgetPayload SelectorPayload
        {
            get
            {
                if (_selectorPayload == null)
                    _selectorPayload = new DashboardGadgetPayload();
                return _selectorPayload;
            }
            set
            {
                _selectorPayload = value;
            }
        }
        #endregion

        #region ToolBox
        #region Security Selector
        /// <summary>
        /// Stores the list of EntitySelectionData for all entity Types
        /// </summary>
        private List<EntitySelectionData> _entitySelectionInfo;
        public List<EntitySelectionData> EntitySelectionInfo
        {
            get { return _entitySelectionInfo; }
            set
            {
                _entitySelectionInfo = value;
                RaisePropertyChanged(() => this.EntitySelectionInfo);

                SecuritySelectorInfo = value
                    .Where(record => record.Type == EntityType.SECURITY)
                    .ToList();
            }
        }

        /// <summary>
        /// Stores the list of EntitySelectionData for entity type - SECURITY
        /// </summary>
        private List<EntitySelectionData> _securitySelectorInfo;
        public List<EntitySelectionData> SecuritySelectorInfo
        {
            get { return _securitySelectorInfo; }
            set
            {
                _securitySelectorInfo = value;
                RaisePropertyChanged(() => this.SecuritySelectorInfo);
            }
        }

        /// <summary>
        /// Stores selected security - Publishes SecurityReferenceSetEvent on set event
        /// </summary>
        private EntitySelectionData _selectedSecurityInfo;
        public EntitySelectionData SelectedSecurityInfo
        {
            get { return _selectedSecurityInfo; }
            set
            {
                if (_selectedSecurityInfo != value)
                {
                    _selectedSecurityInfo = value;
                    RaisePropertyChanged(() => this.SelectedSecurityInfo);
                    if (value != null)
                    {
                        SelectorPayload.EntitySelectionData = value;
                        _eventAggregator.GetEvent<SecurityReferenceSetEvent>().Publish(value);
                    }
                }
            }
        }

        /// <summary>
        /// Stores search text entered by user - Refines SecuritySelectionInfo based on the text entered
        /// </summary>
        private string _securitySearchText;
        public string SecuritySearchText
        {
            get { return _securitySearchText; }
            set
            {
                _securitySearchText = value;
                RaisePropertyChanged(() => this.SecuritySearchText);
                if (value != String.Empty && EntitySelectionInfo != null)
                    SecuritySelectorInfo = EntitySelectionInfo
                                .Where(record => record.LongName.ToLower().Contains(value.ToLower())
                                    || record.ShortName.ToLower().Contains(value.ToLower())
                                    || record.InstrumentID.ToLower().Contains(value.ToLower()))
                                .ToList();
                else
                    SecuritySelectorInfo = EntitySelectionInfo;
            }
        }

        /// <summary>
        /// Stores visibility property of the security selector
        /// </summary>
        private Visibility _securitySelectorVisibility = Visibility.Collapsed;
        public Visibility SecuritySelectorVisibility
        {
            get { return _securitySelectorVisibility; }
            set
            {
                _securitySelectorVisibility = value;
                RaisePropertyChanged(() => this.SecuritySelectorVisibility);
                if (value == Visibility.Visible && EntitySelectionInfo == null)
                {
                    _dbInteractivity.RetrieveEntitySelectionData(RetrieveEntitySelectionDataCallbackMethod);
                }
            }
        }
        #endregion

        #region Portfolio Selector
        /// <summary>
        /// Stores the list of PortfolioSelectionData for all portfolios
        /// </summary>
        private List<PortfolioSelectionData> _portfolioSelectionInfo;
        public List<PortfolioSelectionData> PortfolioSelectionInfo
        {
            get { return _portfolioSelectionInfo; }
            set
            {
                _portfolioSelectionInfo = value;
                RaisePropertyChanged(() => this.PortfolioSelectionInfo);
                PortfolioSelectorInfo = value;
            }
        }

        /// <summary>
        /// Stores the list of PortfolioSelectionData for selector
        /// </summary>
        private List<PortfolioSelectionData> _portfolioSelectorInfo;
        public List<PortfolioSelectionData> PortfolioSelectorInfo
        {
            get { return _portfolioSelectorInfo; }
            set
            {
                _portfolioSelectorInfo = value;
                RaisePropertyChanged(() => this.PortfolioSelectorInfo);
            }
        }

        /// <summary>
        /// Stores selected portfolio - Publishes PortfolioReferenceSetEvent on set event
        /// </summary>
        private PortfolioSelectionData _selectedPortfolioInfo;
        public PortfolioSelectionData SelectedPortfolioInfo
        {
            get { return _selectedPortfolioInfo; }
            set
            {
                if (_selectedPortfolioInfo != value)
                {
                    _selectedPortfolioInfo = value;
                    RaisePropertyChanged(() => this.SelectedPortfolioInfo);
                    if (value != null)
                    {
                        SelectorPayload.PortfolioSelectionData = value;
                        _eventAggregator.GetEvent<PortfolioReferenceSetEvent>().Publish(value);
                    }
                }
            }
        }

        /// <summary>
        /// Stores search text entered by user - Refines PortfolioSelectionInfo based on the text entered
        /// </summary>
        private string _portfolioSearchText;
        public string PortfolioSearchText
        {
            get { return _portfolioSearchText; }
            set
            {
                _portfolioSearchText = value;
                RaisePropertyChanged(() => this.PortfolioSearchText);
                if (value != String.Empty && PortfolioSelectionInfo != null)
                    PortfolioSelectorInfo = PortfolioSelectionInfo
                                .Where(record => record.PortfolioId.ToLower().Contains(value.ToLower()))
                                .ToList();
                else
                    PortfolioSelectorInfo = PortfolioSelectionInfo;
            }
        }

        /// <summary>
        /// Stores visibility property of the portfolio selector
        /// </summary>
        private Visibility _portfolioSelectorVisibility = Visibility.Collapsed;
        public Visibility PortfolioSelectorVisibility
        {
            get { return _portfolioSelectorVisibility; }
            set
            {
                _portfolioSelectorVisibility = value;
                RaisePropertyChanged(() => this.PortfolioSelectorVisibility);
                if (value == Visibility.Visible && PortfolioSelectionInfo == null)
                {
                    _dbInteractivity.RetrievePortfolioSelectionData(RetrievePortfolioSelectionDataCallbackMethod);
                }
            }
        }
        #endregion

        #region Effective Date Selector
        /// <summary>
        /// Stores selected effective date - Publishes EffectiveDateReferenceSetEvent on set event
        /// </summary>
        private DateTime? _selectedEffectiveDateInfo;
        public DateTime? SelectedEffectiveDateInfo
        {
            get { return _selectedEffectiveDateInfo; }
            set
            {
                _selectedEffectiveDateInfo = value;
                RaisePropertyChanged(() => this.SelectedEffectiveDateInfo);
                if (value != null)
                {
                    SelectorPayload.EffectiveDate = Convert.ToDateTime(value);
                    _eventAggregator.GetEvent<EffectiveDateReferenceSetEvent>().Publish(Convert.ToDateTime(value));
                }
            }
        }

        /// <summary>
        /// Stores visibility property of the effective date selector
        /// </summary>
        private Visibility _effectiveDateSelectorVisibility = Visibility.Collapsed;
        public Visibility EffectiveDateSelectorVisibility
        {
            get { return _effectiveDateSelectorVisibility; }
            set
            {
                _effectiveDateSelectorVisibility = value;
                RaisePropertyChanged(() => this.EffectiveDateSelectorVisibility);
                if (value == Visibility.Collapsed)
                {
                    SelectedEffectiveDateInfo = null;
                }
            }
        }
        #endregion

        #region Period Selector
        /// <summary>
        /// Stores selected period start date
        /// </summary>
        private DateTime? _selectedPeriodStartDateInfo;
        public DateTime? SelectedPeriodStartDateInfo
        {
            get { return _selectedPeriodStartDateInfo; }
            set
            {
                _selectedPeriodStartDateInfo = value;
                RaisePropertyChanged(() => this.SelectedPeriodStartDateInfo);
                if (value != null)
                {
                    if (SelectorPayload.PeriodSelectionData == null)
                    {
                        SelectorPayload.PeriodSelectionData = new PeriodSelectionData();
                    }
                    SelectorPayload.PeriodSelectionData.PeriodStartDate = Convert.ToDateTime(value);
                    if (SelectorPayload.PeriodSelectionData.PeriodEndDate != null)
                    {
                        _eventAggregator.GetEvent<PeriodReferenceSetEvent>().Publish(SelectorPayload.PeriodSelectionData);
                    }
                }
            }
        }

        /// <summary>
        /// Stores selected period end date - Publishes PeriodReferenceSetEvent on set event
        /// </summary>
        private DateTime? _selectedPeriodEndDateInfo;
        public DateTime? SelectedPeriodEndDateInfo
        {
            get { return _selectedPeriodEndDateInfo; }
            set
            {
                _selectedPeriodEndDateInfo = value;
                RaisePropertyChanged(() => this.SelectedPeriodStartDateInfo);
                if (value != null)
                {
                    if (SelectorPayload.PeriodSelectionData == null)
                    {
                        SelectorPayload.PeriodSelectionData = new PeriodSelectionData();
                    }
                    SelectorPayload.PeriodSelectionData.PeriodEndDate = Convert.ToDateTime(value);
                    if (SelectorPayload.PeriodSelectionData.PeriodStartDate != null)
                    {
                        _eventAggregator.GetEvent<PeriodReferenceSetEvent>().Publish(SelectorPayload.PeriodSelectionData);
                    }
                }
            }
        }

        /// <summary>
        /// Stores visibility property of the period selector
        /// </summary>
        private Visibility _periodSelectorVisibility = Visibility.Collapsed;
        public Visibility PeriodSelectorVisibility
        {
            get { return _periodSelectorVisibility; }
            set
            {
                _periodSelectorVisibility = value;
                RaisePropertyChanged(() => this.PeriodSelectorVisibility);
                if (value == Visibility.Collapsed)
                {
                    SelectedPeriodStartDateInfo = null;
                    SelectedPeriodEndDateInfo = null;
                }
            }
        }
        #endregion

        //#region Country Selector
        ///// <summary>
        ///// Stores visibility property of the country selector
        ///// </summary>
        //private Visibility _countrySelectorVisibility = Visibility.Collapsed;
        //public Visibility CountrySelectorVisibility
        //{
        //    get { return _countrySelectorVisibility; }
        //    set
        //    {
        //        _countrySelectorVisibility = value;
        //        RaisePropertyChanged(() => this.CountrySelectorVisibility);
        //    }
        //}
        //#endregion

        //#region Sector Selector
        ///// <summary>
        ///// Stores visibility property of the sector selector
        ///// </summary>
        //private Visibility _sectorSelectorVisibility = Visibility.Collapsed;
        //public Visibility SectorSelectorVisibility
        //{
        //    get { return _sectorSelectorVisibility; }
        //    set
        //    {
        //        _sectorSelectorVisibility = value;
        //        RaisePropertyChanged(() => this.SectorSelectorVisibility);
        //    }
        //}
        //#endregion

        //#region Industry Selector
        ///// <summary>
        ///// Stores visibility property of the industry selector
        ///// </summary>
        //private Visibility _industrySelectorVisibility = Visibility.Collapsed;
        //public Visibility IndustrySelectorVisibility
        //{
        //    get { return _industrySelectorVisibility; }
        //    set
        //    {
        //        _industrySelectorVisibility = value;
        //        RaisePropertyChanged(() => this.IndustrySelectorVisibility);
        //    }
        //}
        //#endregion

        //#region Region Selector
        ///// <summary>
        ///// Stores visibility property of the region selector
        ///// </summary>
        //private Visibility _regionSelectorVisibility = Visibility.Collapsed;
        //public Visibility RegionSelectorVisibility
        //{
        //    get { return _regionSelectorVisibility; }
        //    set
        //    {
        //        _regionSelectorVisibility = value;
        //        RaisePropertyChanged(() => this.RegionSelectorVisibility);
        //    }
        //}
        //#endregion

        #region Snapshot Selector
        /// <summary>
        /// Stores the list of MarketSnapshotSelectionData for user
        /// </summary>
        private List<MarketSnapshotSelectionData> _marketSnapshotSelectionInfo;
        public List<MarketSnapshotSelectionData> MarketSnapshotSelectionInfo
        {
            get { return _marketSnapshotSelectionInfo; }
            set
            {
                if (_marketSnapshotSelectionInfo != value)
                {
                    _marketSnapshotSelectionInfo = value;
                    RaisePropertyChanged(() => MarketSnapshotSelectionInfo);
                    MarketSnapshotSelectorInfo = value;
                }
            }
        }

        /// <summary>
        /// Stores the list of MarketSnapshotSelectionData for selector
        /// </summary>
        private List<MarketSnapshotSelectionData> _marketSnapshotSelectorInfo;
        public List<MarketSnapshotSelectionData> MarketSnapshotSelectorInfo
        {
            get { return _marketSnapshotSelectorInfo; }
            set
            {
                if (_marketSnapshotSelectorInfo != value)
                {
                    _marketSnapshotSelectorInfo = value;
                    RaisePropertyChanged(() => MarketSnapshotSelectorInfo);
                }
            }
        }

        /// <summary>
        /// Stores selected snapshot - Publishes MarketPerformanceSnapshotReferenceSetEvent on set event
        /// </summary>
        private MarketSnapshotSelectionData _selectedMarketSnapshotSelectionInfo;
        public MarketSnapshotSelectionData SelectedMarketSnapshotSelectionInfo
        {
            get { return _selectedMarketSnapshotSelectionInfo; }
            set
            {
                if (_selectedMarketSnapshotSelectionInfo != value)
                {
                    _selectedMarketSnapshotSelectionInfo = value;
                    RaisePropertyChanged(() => SelectedMarketSnapshotSelectionInfo);
                    RaisePropertyChanged(() => this.MarketSnapshotSaveCommand);
                    RaisePropertyChanged(() => this.MarketSnapshotRemoveCommand);
                    if (value != null)
                    {
                        SelectorPayload.MarketSnapshotSelectionData = value;
                        _eventAggregator.GetEvent<MarketPerformanceSnapshotReferenceSetEvent>().Publish(value);
                    }

                }
            }
        }

        /// <summary>
        /// Stores search text entered by user - Refines MarketSnapshotSelectionInfo based on the text entered
        /// </summary>
        private string _marketPerformanceSnapshotSearchText;
        public string MarketPerformanceSnapshotSearchText
        {
            get { return _marketPerformanceSnapshotSearchText; }
            set
            {
                _marketPerformanceSnapshotSearchText = value;
                RaisePropertyChanged(() => this.MarketPerformanceSnapshotSearchText);
                if (value != String.Empty && MarketSnapshotSelectionInfo != null)
                    MarketSnapshotSelectorInfo = MarketSnapshotSelectionInfo
                                .Where(record => record.SnapshotName.ToLower().Contains(value.ToLower())).ToList();
                else
                    MarketSnapshotSelectorInfo = MarketSnapshotSelectionInfo;
            }
        }

        /// <summary>
        /// Stores visibility property of the snapshot selector
        /// </summary>
        private Visibility _snapshotSelectorVisibility = Visibility.Collapsed;
        public Visibility SnapshotSelectorVisibility
        {
            get { return _snapshotSelectorVisibility; }
            set
            {
                _snapshotSelectorVisibility = value;
                RaisePropertyChanged(() => this.SnapshotSelectorVisibility);
                if (value == Visibility.Visible && MarketSnapshotSelectionInfo == null)
                {
                    if (SessionManager.SESSION != null)
                    {
                        _dbInteractivity.RetrieveMarketSnapshotSelectionData(SessionManager.SESSION.UserName, RetrieveMarketSnapshotSelectionDataCallbackMethod);
                    }
                    else
                    {
                        _manageSessions.GetSession((session) =>
                            {
                                SessionManager.SESSION = session;
                                _dbInteractivity.RetrieveMarketSnapshotSelectionData(SessionManager.SESSION.UserName, RetrieveMarketSnapshotSelectionDataCallbackMethod);
                            });
                    }
                }
            }
        }
        #endregion
        #endregion
        #endregion

        # region ICommand
        public ICommand LogOutCommand
        {
            get { return new DelegateCommand<object>(LogOutCommandMethod); }
        }

        public ICommand MyDashboardCommand
        {
            get
            {
                return new DelegateCommand<object>(MyDashboardCommandMethod);
            }
        }

        public ICommand GadgetSecurityOverviewCommand
        {
            get
            {
                return new DelegateCommand<object>(GadgetSecurityOverviewCommandMethod);
            }
        }

        public ICommand GadgetPricingCommand
        {
            get
            {
                return new DelegateCommand<object>(GadgetPricingCommandMethod);
            }
        }

        public ICommand GadgetTheoreticalUnrealizedGainLossCommand
        {
            get
            {
                return new DelegateCommand<object>(GadgetTheoreticalUnrealizedGainLossCommandMethod);
            }
        }

        public ICommand GadgetRegionBreakdownCommand
        {
            get
            {
                return new DelegateCommand<object>(GadgetRegionBreakdownCommandMethod);
            }
        }

        public ICommand GadgetSectorBreakdownCommand
        {
            get
            {
                return new DelegateCommand<object>(GadgetSectorBreakdownCommandMethod);
            }
        }

        public ICommand GadgetIndexConstituentsCommand
        {
            get
            {
                return new DelegateCommand<object>(GadgetIndexConstituentsCommandMethod);
            }
        }

        public ICommand GadgetMarketCapitalizationCommand
        {
            get
            {
                return new DelegateCommand<object>(GadgetMarketCapitalizationCommandMethod);
            }
        }

        public ICommand GadgetTopHoldingsCommand
        {
            get
            {
                return new DelegateCommand<object>(GadgetTopHoldingsCommandMethod);
            }
        }

        public ICommand GadgetAssetAllocationCommand
        {
            get
            {
                return new DelegateCommand<object>(GadgetAssetAllocationCommandMethod);
            }
        }

        public ICommand GadgetHoldingsPieChartCommand
        {
            get
            {
                return new DelegateCommand<object>(GadgetHoldingsPieChartCommandMethod);
            }
        }

        public ICommand GadgetPortfolioRiskReturnsCommand
        {
            get
            {
                return new DelegateCommand<object>(GadgetPortfolioRiskReturnsCommandMethod);
            }
        }

        public ICommand GadgetTopBenchmarkSecuritiesCommand
        {
            get
            {
                return new DelegateCommand<object>(GadgetTopBenchmarkSecuritiesCommandMethod);
            }
        }

        public ICommand GadgetRelativePerformanceCommand
        {
            get
            {
                return new DelegateCommand<object>(GadgetRelativePerformanceCommandMethod);
            }
        }

        public ICommand GadgetCountryActivePositionCommand
        {
            get
            {
                return new DelegateCommand<object>(GadgetCountryActivePositionCommandMethod);
            }
        }

        public ICommand GadgetSectorActivePositionCommand
        {
            get
            {
                return new DelegateCommand<object>(GadgetSectorActivePositionCommandMethod);
            }
        }

        public ICommand GadgetSecurityActivePositionCommand
        {
            get
            {
                return new DelegateCommand<object>(GadgetSecurityActivePositionCommandMethod);
            }
        }

        public ICommand GadgetTopContributorCommand
        {
            get
            {
                return new DelegateCommand<object>(GadgetTopContributorCommandMethod);
            }
        }

        public ICommand GadgetTopDetractorCommand
        {
            get
            {
                return new DelegateCommand<object>(GadgetTopDetractorCommandMethod);
            }
        }

        public ICommand GadgetContributorDetractorCommand
        {
            get
            {
                return new DelegateCommand<object>(GadgetContributorDetractorCommandMethod);
            }
        }

        public ICommand GadgetSaveCommand
        {
            get
            {
                return new DelegateCommand<object>(GadgetSaveCommandMethod);
            }
        }

        public ICommand RelativePerformanceCommand
        {
            get
            {
                return new DelegateCommand<object>(RelativePerformanceCommandMethod);
            }
        }

        public ICommand PortfolioDetailsCommand
        {
            get
            {
                return new DelegateCommand<object>(PortfolioDetailsCommandMethod);
            }
        }

        public ICommand PerformanceGraphCommand
        {
            get
            {
                return new DelegateCommand<object>(PerformanceGraphCommandMethod);
            }
        }

        public ICommand PerformanceGridCommand
        {
            get
            {
                return new DelegateCommand<object>(PerformanceGridCommandMethod);
            }
        }

        public ICommand AttributionCommand
        {
            get
            {
                return new DelegateCommand<object>(AttributionCommandMethod);
            }
        }

        public ICommand HeatMapCommand
        {
            get
            {
                return new DelegateCommand<object>(HeatMapCommandMethod);
            }
        }

        #region Dashboard
        #region Company
        #region Snapshot
        public ICommand DashboardCompanySnapshotSummaryCommand
        {
            get { return new DelegateCommand<object>(DashboardCompanySnapshotSummaryCommandMethod); }
        }

        public ICommand DashboardCompanySnapshotCompanyProfileCommand
        {
            get { return new DelegateCommand<object>(DashboardCompanySnapshotCompanyProfileCommandMethod); }
        }

        public ICommand DashboardCompanySnapshotTearSheetCommand
        {
            get { return new DelegateCommand<object>(DashboardCompanySnapshotTearSheetCommandMethod); }
        }
        #endregion

        #region Financials
        public ICommand DashboardCompanyFinancialsSummaryCommand
        {
            get { return new DelegateCommand<object>(DashboardCompanyFinancialsSummaryCommandMethod); }
        }

        public ICommand DashboardCompanyFinancialsIncomeStatementCommand
        {
            get { return new DelegateCommand<object>(DashboardCompanyFinancialsIncomeStatementCommandMethod); }
        }

        public ICommand DashboardCompanyFinancialsBalanceSheetCommand
        {
            get { return new DelegateCommand<object>(DashboardCompanyFinancialsBalanceSheetCommandMethod); }
        }

        public ICommand DashboardCompanyFinancialsCashFlowCommand
        {
            get { return new DelegateCommand<object>(DashboardCompanyFinancialsCashFlowCommandMethod); }
        }

        public ICommand DashboardCompanyFinancialsFinStatCommand
        {
            get { return new DelegateCommand<object>(DashboardCompanyFinancialsFinStatCommandMethod); }
        }

        public ICommand DashboardCompanyFinancialsPeerComparisonCommand
        {
            get { return new DelegateCommand<object>(DashboardCompanyFinancialsPeerComparisonCommandMethod); }
        }
        #endregion

        #region Estimates
        public ICommand DashboardCompanyEstimatesConsensusCommand
        {
            get { return new DelegateCommand<object>(DashboardCompanyEstimatesConsensusCommandMethod); }
        }

        public ICommand DashboardCompanyEstimatesDetailedCommand
        {
            get { return new DelegateCommand<object>(DashboardCompanyEstimatesDetailedCommandMethod); }
        }

        public ICommand DashboardCompanyEstimatesComparisonCommand
        {
            get { return new DelegateCommand<object>(DashboardCompanyEstimatesComparisonCommandMethod); }
        }
        #endregion

        #region Valuation
        public ICommand DashboardCompanyValuationFairValueCommand
        {
            get { return new DelegateCommand<object>(DashboardCompanyValuationFairValueCommandMethod); }
        }

        public ICommand DashboardCompanyValuationDiscountedCashFlowCommand
        {
            get { return new DelegateCommand<object>(DashboardCompanyValuationDiscountedCashFlowCommandMethod); }
        }
        #endregion

        #region Documents
        public ICommand DashboardCompanyDocumentsCommand
        {
            get { return new DelegateCommand<object>(DashboardCompanyDocumentsCommandMethod); }
        }
        #endregion

        #region Charting
        public ICommand DashboardCompanyChartingClosingPriceCommand
        {
            get { return new DelegateCommand<object>(DashboardCompanyChartingClosingPriceCommandMethod); }
        }

        public ICommand DashboardCompanyChartingUnrealizedGainCommand
        {
            get { return new DelegateCommand<object>(DashboardCompanyChartingUnrealizedGainCommandMethod); }
        }

        public ICommand DashboardCompanyChartingContextCommand
        {
            get { return new DelegateCommand<object>(DashboardCompanyChartingContextCommandMethod); }
        }

        public ICommand DashboardCompanyChartingValuationCommand
        {
            get { return new DelegateCommand<object>(DashboardCompanyChartingValuationCommandMethod); }
        }
        #endregion

        #region Corporate Governance
        public ICommand DashboardCompanyCorporateGovernanceQuestionnaireCommand
        {
            get { return new DelegateCommand<object>(DashboardCompanyCorporateGovernanceQuestionnaireCommandMethod); }
        }

        public ICommand DashboardCompanyCorporateGovernanceReportCommand
        {
            get { return new DelegateCommand<object>(DashboardCompanyCorporateGovernanceReportCommandMethod); }
        }
        #endregion
        #endregion

        #region Markets
        #region Snapshot
        public ICommand DashboardMarketsSnapshotSummaryCommand
        {
            get { return new DelegateCommand<object>(DashboardMarketsSnapshotSummaryCommandMethod); }
        }

        public ICommand DashboardMarketsSnapshotMarketPerformanceCommand
        {
            get { return new DelegateCommand<object>(DashboardMarketsSnapshotMarketPerformanceCommandMethod); }
        }

        public ICommand DashboardMarketsSnapshotInternalModelValuationCommand
        {
            get { return new DelegateCommand<object>(DashboardMarketsSnapshotInternalModelValuationCommandMethod); }
        }
        #endregion

        #region MacroEconomic
        public ICommand DashboardMarketsMacroEconomicsEMSummaryCommand
        {
            get { return new DelegateCommand<object>(DashboardMarketsMacroEconomicsEMSummaryCommandMethod); }
        }

        public ICommand DashboardMarketsMacroEconomicsCountrySummaryCommand
        {
            get { return new DelegateCommand<object>(DashboardMarketsMacroEconomicsCountrySummaryCommandMethod); }
        }
        #endregion

        #region Commodities
        public ICommand DashboardMarketsCommoditiesSummaryCommand
        {
            get { return new DelegateCommand<object>(DashboardMarketsCommoditiesSummaryCommandMethod); }
        }
        #endregion
        #endregion

        #region Portfolio
        #region Snapshot
        public ICommand DashboardPortfolioSnapshotCommand
        {
            get { return new DelegateCommand<object>(DashboardPortfolioSnapshotCommandMethod); }
        }
        #endregion

        #region Holdings
        public ICommand DashboardPortfolioHoldingsCommand
        {
            get { return new DelegateCommand<object>(DashboardPortfolioHoldingsCommandMethod); }
        }
        #endregion

        #region Performance
        public ICommand DashboardPortfolioPerformanceSummaryCommand
        {
            get { return new DelegateCommand<object>(DashboardPortfolioPerformanceSummaryCommandMethod); }
        }

        public ICommand DashboardPortfolioPerformanceAttributionCommand
        {
            get { return new DelegateCommand<object>(DashboardPortfolioPerformanceAttributionCommandMethod); }
        }

        public ICommand DashboardPortfolioPerformanceRelativePerformanceCommand
        {
            get { return new DelegateCommand<object>(DashboardPortfolioPerformanceRelativePerformanceCommandMethod); }
        }

        #endregion

        #region Benchmark
        public ICommand DashboardPortfolioBenchmarkSummaryCommand
        {
            get { return new DelegateCommand<object>(DashboardPortfolioBenchmarkSummaryCommandMethod); }
        }

        public ICommand DashboardPortfolioBenchmarkComponentsCommand
        {
            get { return new DelegateCommand<object>(DashboardPortfolioBenchmarkComponentsCommandMethod); }
        }
        #endregion
        #endregion
        #endregion

        #region ToolBox
        public ICommand MarketSnapshotSaveCommand
        {
            get
            {
                return new DelegateCommand<object>(MarketSnapshotSaveCommandMethod, MarketSnapshotSaveCommandValidationMethod);
            }
        }

        public ICommand MarketSnapshotSaveAsCommand
        {
            get
            {
                return new DelegateCommand<object>(MarketSnapshotSaveAsCommandMethod);
            }
        }

        public ICommand MarketSnapshotAddCommand
        {
            get
            {
                return new DelegateCommand<object>(MarketSnapshotAddCommandMethod);
            }
        }

        public ICommand MarketSnapshotRemoveCommand
        {
            get
            {
                return new DelegateCommand<object>(MarketSnapshotRemoveCommandMethod, MarketSnapshotRemoveCommandValidationMethod);
            }
        }
        #endregion
        #endregion
        #endregion

        #region ICommand Methods

        /// <summary>
        /// LogoutCommand Execution Method - Navigate to Login Page
        /// </summary>
        /// <param name="param">SenderInfo</param>
        private void LogOutCommandMethod(object param)
        {
            try
            {
                HtmlPage.Window.Navigate(new Uri(@"Login.aspx", UriKind.Relative));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
        }

        #region Dashboard
        #region Company
        #region Snapshot
        private void DashboardCompanySnapshotSummaryCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                _eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(SelectorPayload);
                ToolBoxSelecter.SetToolBoxItemVisibility(DashboardCategoryType.COMPANY_SNAPSHOT_SUMMARY);
                UpdateToolBoxSelectorVisibility();
                _regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardCompanySnapshotSummary", UriKind.Relative));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        private void DashboardCompanySnapshotCompanyProfileCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                _eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(SelectorPayload);
                ToolBoxSelecter.SetToolBoxItemVisibility(DashboardCategoryType.COMPANY_SNAPSHOT_COMPANY_PROFILE);
                UpdateToolBoxSelectorVisibility();
                _regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardCompanySnapshotCompanyProfile", UriKind.Relative));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        private void DashboardCompanySnapshotTearSheetCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                _eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(SelectorPayload);
                ToolBoxSelecter.SetToolBoxItemVisibility(DashboardCategoryType.COMPANY_SNAPSHOT_TEAR_SHEET);
                UpdateToolBoxSelectorVisibility();
                _regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardCompanySnapshotTearSheet", UriKind.Relative));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }
        #endregion

        #region Financials
        private void DashboardCompanyFinancialsSummaryCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                _eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(SelectorPayload);
                ToolBoxSelecter.SetToolBoxItemVisibility(DashboardCategoryType.COMPANY_FINANCIALS_SUMMARY);
                UpdateToolBoxSelectorVisibility();
                _regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardCompanyFinancialsSummary", UriKind.Relative));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        private void DashboardCompanyFinancialsIncomeStatementCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                _eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(SelectorPayload);
                ToolBoxSelecter.SetToolBoxItemVisibility(DashboardCategoryType.COMPANY_FINANCIALS_INCOME_STATEMENT);
                UpdateToolBoxSelectorVisibility();
                _regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardCompanyFinancialsIncomeStatement", UriKind.Relative));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        private void DashboardCompanyFinancialsBalanceSheetCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                _eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(SelectorPayload);
                ToolBoxSelecter.SetToolBoxItemVisibility(DashboardCategoryType.COMPANY_FINANCIALS_BALANCE_SHEET);
                UpdateToolBoxSelectorVisibility();
                _regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardCompanyFinancialsBalanceSheet", UriKind.Relative));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        private void DashboardCompanyFinancialsCashFlowCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                _eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(SelectorPayload);
                ToolBoxSelecter.SetToolBoxItemVisibility(DashboardCategoryType.COMPANY_FINANCIALS_CASH_FLOW);
                UpdateToolBoxSelectorVisibility();
                _regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardCompanyFinancialsCashFlow", UriKind.Relative));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        private void DashboardCompanyFinancialsFinStatCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                _eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(SelectorPayload);
                ToolBoxSelecter.SetToolBoxItemVisibility(DashboardCategoryType.COMPANY_FINANCIALS_FINSTAT);
                UpdateToolBoxSelectorVisibility();
                _regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardCompanyFinancialsFinStat", UriKind.Relative));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        private void DashboardCompanyFinancialsPeerComparisonCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                _eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(SelectorPayload);
                ToolBoxSelecter.SetToolBoxItemVisibility(DashboardCategoryType.COMPANY_FINANCIALS_PEER_COMPARISON);
                UpdateToolBoxSelectorVisibility();
                _regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardCompanyFinancialsPeerComparison", UriKind.Relative));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }
        #endregion

        #region Estimates
        private void DashboardCompanyEstimatesConsensusCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                _eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(SelectorPayload);
                ToolBoxSelecter.SetToolBoxItemVisibility(DashboardCategoryType.COMPANY_ESTIMATES_CONSENSUS);
                UpdateToolBoxSelectorVisibility();
                _regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardCompanyEstimatesConsensus", UriKind.Relative));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        private void DashboardCompanyEstimatesDetailedCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                _eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(SelectorPayload);
                ToolBoxSelecter.SetToolBoxItemVisibility(DashboardCategoryType.COMPANY_ESTIMATES_DETAILED);
                UpdateToolBoxSelectorVisibility();
                _regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardCompanyEstimatesDetailed", UriKind.Relative));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        private void DashboardCompanyEstimatesComparisonCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                _eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(SelectorPayload);
                ToolBoxSelecter.SetToolBoxItemVisibility(DashboardCategoryType.COMPANY_ESTIMATES_COMPARISON);
                UpdateToolBoxSelectorVisibility();
                _regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardCompanyEstimatesComparison", UriKind.Relative));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }
        #endregion

        #region Valuation
        private void DashboardCompanyValuationFairValueCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                _eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(SelectorPayload);
                ToolBoxSelecter.SetToolBoxItemVisibility(DashboardCategoryType.COMPANY_VALUATION_FAIR_VALUE);
                UpdateToolBoxSelectorVisibility();
                _regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardCompanyValuationFairValue", UriKind.Relative));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        private void DashboardCompanyValuationDiscountedCashFlowCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                _eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(SelectorPayload);
                ToolBoxSelecter.SetToolBoxItemVisibility(DashboardCategoryType.COMPANY_VALUATION_DCF);
                UpdateToolBoxSelectorVisibility();
                _regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardCompanyValuationDiscountedCashFlow", UriKind.Relative));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }
        #endregion

        #region Documents
        private void DashboardCompanyDocumentsCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                _eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(SelectorPayload);
                ToolBoxSelecter.SetToolBoxItemVisibility(DashboardCategoryType.COMPANY_DOCUMENTS);
                UpdateToolBoxSelectorVisibility();
                _regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardCompanyDocuments", UriKind.Relative));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }
        #endregion

        #region Charting
        private void DashboardCompanyChartingClosingPriceCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                _eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(SelectorPayload);
                ToolBoxSelecter.SetToolBoxItemVisibility(DashboardCategoryType.COMPANY_CHARTING_PRICE_COMPARISON);
                UpdateToolBoxSelectorVisibility();
                _regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardCompanyChartingClosingPrice", UriKind.Relative));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        private void DashboardCompanyChartingUnrealizedGainCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                _eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(SelectorPayload);
                ToolBoxSelecter.SetToolBoxItemVisibility(DashboardCategoryType.COMPANY_CHARTING_UNREALIZED_GAIN_LOSS);
                UpdateToolBoxSelectorVisibility();
                _regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardCompanyChartingUnrealizedGainLoss", UriKind.Relative));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        private void DashboardCompanyChartingContextCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                _eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(SelectorPayload);
                ToolBoxSelecter.SetToolBoxItemVisibility(DashboardCategoryType.COMPANY_CHARTING_CONTEXT);
                UpdateToolBoxSelectorVisibility();
                _regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardCompanyChartingContext", UriKind.Relative));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        private void DashboardCompanyChartingValuationCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                _eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(SelectorPayload);
                ToolBoxSelecter.SetToolBoxItemVisibility(DashboardCategoryType.COMPANY_CHARTING_VALUATION);
                UpdateToolBoxSelectorVisibility();
                _regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardCompanyChartingValuation", UriKind.Relative));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }
        #endregion

        #region Corporate Governance
        private void DashboardCompanyCorporateGovernanceQuestionnaireCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                _eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(SelectorPayload);
                ToolBoxSelecter.SetToolBoxItemVisibility(DashboardCategoryType.COMPANY_CORPORATE_GOVERNANCE_QUESTIONNAIRE);
                UpdateToolBoxSelectorVisibility();
                _regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardCompanyCorporateGovernanceQuestionnaire", UriKind.Relative));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        private void DashboardCompanyCorporateGovernanceReportCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                _eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(SelectorPayload);
                ToolBoxSelecter.SetToolBoxItemVisibility(DashboardCategoryType.COMPANY_CORPORATE_GOVERNANCE_REPORT);
                UpdateToolBoxSelectorVisibility();
                _regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardCompanyCorporateGovernanceReport", UriKind.Relative));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }
        #endregion
        #endregion

        #region Markets
        #region Snapshot
        private void DashboardMarketsSnapshotSummaryCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                _eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(SelectorPayload);
                ToolBoxSelecter.SetToolBoxItemVisibility(DashboardCategoryType.MARKETS_SNAPSHOT_SUMMARY);
                UpdateToolBoxSelectorVisibility();
                _regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardMarketsSnapshotSummary", UriKind.Relative));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        private void DashboardMarketsSnapshotMarketPerformanceCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                _eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(SelectorPayload);
                ToolBoxSelecter.SetToolBoxItemVisibility(DashboardCategoryType.MARKETS_SNAPSHOT_MARKET_PERFORMANCE);
                UpdateToolBoxSelectorVisibility();
                _regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardMarketsSnapshotMarketPerformance", UriKind.Relative));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        private void DashboardMarketsSnapshotInternalModelValuationCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                _eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(SelectorPayload);
                ToolBoxSelecter.SetToolBoxItemVisibility(DashboardCategoryType.MARKETS_SNAPSHOT_INTERNAL_MODEL_VALUATION);
                UpdateToolBoxSelectorVisibility();
                _regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardMarketsSnapshotInternalModelValuation", UriKind.Relative));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }
        #endregion

        #region MacroEconomic
        private void DashboardMarketsMacroEconomicsEMSummaryCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                _eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(SelectorPayload);
                ToolBoxSelecter.SetToolBoxItemVisibility(DashboardCategoryType.MARKETS_MACROECONOMIC_EM_SUMMARY);
                UpdateToolBoxSelectorVisibility();
                _regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardMarketsMacroEconomicsEMSummary", UriKind.Relative));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        private void DashboardMarketsMacroEconomicsCountrySummaryCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                _eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(SelectorPayload);
                ToolBoxSelecter.SetToolBoxItemVisibility(DashboardCategoryType.MARKETS_MACROECONOMIC_COUNTRY_SUMMARY);
                UpdateToolBoxSelectorVisibility();
                _regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardMarketsMacroEconomicsCountrySummary", UriKind.Relative));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }
        #endregion

        #region Commodities
        private void DashboardMarketsCommoditiesSummaryCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                _eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(SelectorPayload);
                ToolBoxSelecter.SetToolBoxItemVisibility(DashboardCategoryType.MARKETS_COMMODITIES_SUMMARY);
                UpdateToolBoxSelectorVisibility();
                _regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardMarketsCommoditiesSummary", UriKind.Relative));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }
        #endregion
        #endregion

        #region Portfolio
        #region Snapshot
        private void DashboardPortfolioSnapshotCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                _eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(SelectorPayload);
                ToolBoxSelecter.SetToolBoxItemVisibility(DashboardCategoryType.PORTFOLIO_SNAPSHOT);
                UpdateToolBoxSelectorVisibility();
                _regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardPortfolioSnapshot", UriKind.Relative));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }
        #endregion

        #region Holdings
        private void DashboardPortfolioHoldingsCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                _eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(SelectorPayload);
                ToolBoxSelecter.SetToolBoxItemVisibility(DashboardCategoryType.PORTFOLIO_HOLDINGS);
                UpdateToolBoxSelectorVisibility();
                _regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardPortfolioHoldings", UriKind.Relative));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }
        #endregion

        #region Performance
        private void DashboardPortfolioPerformanceSummaryCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                _eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(SelectorPayload);
                ToolBoxSelecter.SetToolBoxItemVisibility(DashboardCategoryType.PORTFOLIO_PERFORMANCE_SUMMARY);
                UpdateToolBoxSelectorVisibility();
                _regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardPortfolioPerformanceSummary", UriKind.Relative));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        private void DashboardPortfolioPerformanceAttributionCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                _eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(SelectorPayload);
                ToolBoxSelecter.SetToolBoxItemVisibility(DashboardCategoryType.PORTFOLIO_PERFORMANCE_ATTRIBUTION);
                UpdateToolBoxSelectorVisibility();
                _regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardPortfolioPerformanceAttribution", UriKind.Relative));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        private void DashboardPortfolioPerformanceRelativePerformanceCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                _eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(SelectorPayload);
                ToolBoxSelecter.SetToolBoxItemVisibility(DashboardCategoryType.PORTFOLIO_PERFORMANCE_RELATIVE_PERFORMANCE);
                UpdateToolBoxSelectorVisibility();
                _regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardPortfolioPerformanceRelativePerformance", UriKind.Relative));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }
        #endregion

        #region Benchmark
        private void DashboardPortfolioBenchmarkSummaryCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                _eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(SelectorPayload);
                ToolBoxSelecter.SetToolBoxItemVisibility(DashboardCategoryType.PORTFOLIO_BENCHMARK_SUMMARY);
                UpdateToolBoxSelectorVisibility();
                _regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardPortfolioBenchmarkSummary", UriKind.Relative));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        private void DashboardPortfolioBenchmarkComponentsCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                _eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(SelectorPayload);
                ToolBoxSelecter.SetToolBoxItemVisibility(DashboardCategoryType.PORTFOLIO_BENCHMARK_COMPOSITION);
                UpdateToolBoxSelectorVisibility();
                _regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardPortfolioBenchmarkComponents", UriKind.Relative));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }
        #endregion
        #endregion
        #endregion

        #region ToolBox
        /// MarketSnapshotAddCommand execution method - creates new market performance snapshot
        /// </summary>
        /// <param name="param">sender info</param>
        private void MarketSnapshotAddCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                _eventAggregator.GetEvent<MarketPerformanceSnapshotActionEvent>().Publish(MarketPerformanceSnapshotActionType.SNAPSHOT_ADD);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        /// <summary>
        /// MarketSnapshotSaveCommand Validation Method
        /// </summary>
        /// <param name="param">sender info</param>
        /// <returns>True/False</returns>
        private bool MarketSnapshotSaveCommandValidationMethod(object param)
        {
            return SelectedMarketSnapshotSelectionInfo != null;
        }            

        /// <summary>
        /// MarketSnapshotSaveCommand execution method - saves changes in existing market performance snapshot
        /// </summary>
        /// <param name="param">sender info</param>
        private void MarketSnapshotSaveCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                _eventAggregator.GetEvent<MarketPerformanceSnapshotActionEvent>().Publish(MarketPerformanceSnapshotActionType.SNAPSHOT_SAVE);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        /// <summary>
        /// MarketSnapshotSaveAsCommand execution method - saves existing market performance snapshot by new name
        /// </summary>
        /// <param name="param">sender info</param>
        private void MarketSnapshotSaveAsCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                _eventAggregator.GetEvent<MarketPerformanceSnapshotActionEvent>().Publish(MarketPerformanceSnapshotActionType.SNAPSHOT_SAVE_AS);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        /// <summary>
        /// MarketSnapshotRemoveCommand Validation Method
        /// </summary>
        /// <param name="param">sender info</param>
        /// <returns>True/False</returns>
        private bool MarketSnapshotRemoveCommandValidationMethod(object param)
        {
            return SelectedMarketSnapshotSelectionInfo != null;
        }

        /// <summary>
        /// MarketSnapshotRemoveCommand execution method - deletes existing market performance snapshot
        /// </summary>
        /// <param name="param">sender info</param>
        private void MarketSnapshotRemoveCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                _eventAggregator.GetEvent<MarketPerformanceSnapshotActionEvent>().Publish(MarketPerformanceSnapshotActionType.SNAPSHOT_REMOVE);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        #endregion
        private void UserManagementCommandMethod(object param)
        {
            Logging.LogBeginMethod(_logger, String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name));
            try
            {
                _regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewManageUsers", UriKind.Relative));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "NullReferenceException", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name));
        }

        private void RoleManagementCommandMethod(object param)
        {
            Logging.LogBeginMethod(_logger, String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name));
            try
            {
                _regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewManageRoles", UriKind.Relative));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "NullReferenceException", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name));
        }

        private void DailyMorningSnapshotCommandMethod(object param)
        {
            _regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewMorningSnapshot", UriKind.Relative));
        }

        /// <summary>
        /// MyDashboardCommand Execution Method - Opens Dashboard 
        /// </summary>
        /// <param name="param"></param>
        private void MyDashboardCommandMethod(object param)
        {
            Logging.LogBeginMethod(_logger, String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name));
            try
            {
                _eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(SelectorPayload);
                _regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboard", UriKind.Relative));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name));
        }

        /// <summary>
        /// GadgetSecurityOverviewCommand Execution Method - Add Gadget - SECURITY_OVERVIEW
        /// </summary>
        /// <param name="param">SenderInfo</param>
        private void GadgetSecurityOverviewCommandMethod(object param)
        {
            Logging.LogBeginMethod(_logger, String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name));
            try
            {
                _eventAggregator.GetEvent<DashboardTileViewItemAdded>().Publish
                       (new DashboardTileViewItemInfo
                       {
                           DashboardTileHeader = GadgetNames.SECURITY_OVERVIEW,
                           DashboardTileObject = new ViewSecurityOverview(new ViewModelSecurityOverview(GetDashboardGadgetParam()))
                       });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name));
        }

        /// <summary>
        /// GadgetPricingCommand Execution Method - Add Gadget - PRICING
        /// </summary>
        /// <param name="param">SenderInfo</param>
        private void GadgetPricingCommandMethod(object param)
        {
            Logging.LogBeginMethod(_logger, String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name));
            try
            {
                _eventAggregator.GetEvent<DashboardTileViewItemAdded>().Publish
                       (new DashboardTileViewItemInfo
                       {
                           DashboardTileHeader = GadgetNames.SECURITY_REFERENCE_PRICE_COMPARISON,
                           DashboardTileObject = new ViewClosingPriceChart(new ViewModelClosingPriceChart(GetDashboardGadgetParam()))
                       });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name));
        }

        /// <summary>
        /// GadgetTheoreticalUnrealizedGainLoss Execution Method - Add Gadget - UNREALIZED_GAINLOSS
        /// </summary>
        /// <param name="param">SenderInfo</param>
        private void GadgetTheoreticalUnrealizedGainLossCommandMethod(object param)
        {
            Logging.LogBeginMethod(_logger, String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name));
            try
            {
                _eventAggregator.GetEvent<DashboardTileViewItemAdded>().Publish
                        (new DashboardTileViewItemInfo
                        {
                            DashboardTileHeader = GadgetNames.SECURITY_REFERENCE_UNREALIZED_GAIN_LOSS,
                            DashboardTileObject = new ViewUnrealizedGainLoss(new ViewModelUnrealizedGainLoss(GetDashboardGadgetParam()))
                        });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name));
        }

        /// <summary>
        /// GadgetRegionBreakdownCommand Execution Method - Add Gadget - REGION_BREAKDOWN
        /// </summary>
        /// <param name="param">SenderInfo</param>
        private void GadgetRegionBreakdownCommandMethod(object param)
        {
            Logging.LogBeginMethod(_logger, String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name));
            try
            {
                _eventAggregator.GetEvent<DashboardTileViewItemAdded>().Publish
                        (new DashboardTileViewItemInfo
                        {
                            DashboardTileHeader = GadgetNames.HOLDINGS_REGION_BREAKDOWN,
                            DashboardTileObject = new ViewRegionBreakdown(new ViewModelRegionBreakdown(GetDashboardGadgetParam()))
                        });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name));
        }

        /// <summary>
        /// GadgetSectorBreakdownCommand Execution Method - Add Gadget - SECTOR_BREAKDOWN
        /// </summary>
        /// <param name="param">SenderInfo</param>
        private void GadgetSectorBreakdownCommandMethod(object param)
        {
            Logging.LogBeginMethod(_logger, String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name));
            try
            {
                _eventAggregator.GetEvent<DashboardTileViewItemAdded>().Publish
                        (new DashboardTileViewItemInfo
                        {
                            DashboardTileHeader = GadgetNames.HOLDINGS_SECTOR_BREAKDOWN,
                            DashboardTileObject = new ViewSectorBreakdown(new ViewModelSectorBreakdown(GetDashboardGadgetParam()))
                        });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name));
        }

        /// <summary>
        /// GadgetIndexConstituentsCommand Execution Method - Add Gadget - INDEX_CONSTITUENTS
        /// </summary>
        /// <param name="param">SenderInfo</param>
        private void GadgetIndexConstituentsCommandMethod(object param)
        {
            Logging.LogBeginMethod(_logger, String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name));
            try
            {
                _eventAggregator.GetEvent<DashboardTileViewItemAdded>().Publish
                        (new DashboardTileViewItemInfo
                        {
                            DashboardTileHeader = GadgetNames.BENCHMARK_INDEX_CONSTITUENTS,
                            DashboardTileObject = new ViewIndexConstituents(new ViewModelIndexConstituents(GetDashboardGadgetParam()))
                        });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name));
        }

        /// <summary>
        /// GadgetMarketCapitalizationCommand Execution Method - Add Gadget - MARKET_CAPITALIZATION
        /// </summary>
        /// <param name="param">SenderInfo</param>
        private void GadgetMarketCapitalizationCommandMethod(object param)
        {
            Logging.LogBeginMethod(_logger, String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name));
            try
            {
                _eventAggregator.GetEvent<DashboardTileViewItemAdded>().Publish
                        (new DashboardTileViewItemInfo
                        {
                            DashboardTileHeader = GadgetNames.HOLDINGS_MARKET_CAPITALIZATION,
                            DashboardTileObject = new ViewMarketCapitalization(new ViewModelMarketCapitalization(GetDashboardGadgetParam()))
                        });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name));
        }

        /// <summary>
        /// GadgetTopHoldingsCommand Execution Method - Add Gadget - TOP_HOLDINGS
        /// </summary>
        /// <param name="param">SenderInfo</param>
        private void GadgetTopHoldingsCommandMethod(object param)
        {
            Logging.LogBeginMethod(_logger, String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name));
            try
            {
                _eventAggregator.GetEvent<DashboardTileViewItemAdded>().Publish
                        (new DashboardTileViewItemInfo
                        {
                            DashboardTileHeader = GadgetNames.HOLDINGS_TOP_TEN_HOLDINGS,
                            DashboardTileObject = new ViewTopHoldings(new ViewModelTopHoldings(GetDashboardGadgetParam()))
                        });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name));
        }

        /// <summary>
        /// GadgetTopHoldingsCommand Execution Method - Add Gadget - ASSET_ALLOCATION
        /// </summary>
        /// <param name="param">SenderInfo</param>
        private void GadgetAssetAllocationCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                _eventAggregator.GetEvent<DashboardTileViewItemAdded>().Publish
                        (new DashboardTileViewItemInfo
                        {
                            DashboardTileHeader = GadgetNames.HOLDINGS_ASSET_ALLOCATION,
                            DashboardTileObject = new ViewAssetAllocation(new ViewModelAssetAllocation(GetDashboardGadgetParam()))
                        });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        /// <summary>
        /// GadgetHoldingsPieChartCommand Execution Method - Add Gadget - HOLDINGS_PIECHART
        /// </summary>
        /// <param name="param">SenderInfo</param>
        private void GadgetHoldingsPieChartCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                _eventAggregator.GetEvent<DashboardTileViewItemAdded>().Publish
                        (new DashboardTileViewItemInfo
                        {
                            DashboardTileHeader = GadgetNames.BENCHMARK_HOLDINGS_SECTOR_PIECHART,
                            DashboardTileObject = new ViewHoldingsPieChart(new ViewModelHoldingsPieChart(GetDashboardGadgetParam()))
                        });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        /// <summary>
        /// GadgetPortfolioRiskReturnsCommand Execution Method - Add Gadget - PORTFOLIO_RISK_RETURNS
        /// </summary>
        /// <param name="param">SenderInfo</param>
        private void GadgetPortfolioRiskReturnsCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                _eventAggregator.GetEvent<DashboardTileViewItemAdded>().Publish
                        (new DashboardTileViewItemInfo
                        {
                            DashboardTileHeader = GadgetNames.HOLDINGS_RISK_RETURN,
                            DashboardTileObject = new ViewPortfolioRiskReturns(new ViewModelPortfolioRiskReturns(GetDashboardGadgetParam()))
                        });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        /// <summary>
        /// GadgetTopBenchmarkSecuritiesCommand Execution Method - Add Gadget - TOP_BENCHMARK_SECURITIES
        /// </summary>
        /// <param name="param">SenderInfo</param>
        private void GadgetTopBenchmarkSecuritiesCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                _eventAggregator.GetEvent<DashboardTileViewItemAdded>().Publish
                        (new DashboardTileViewItemInfo
                        {
                            DashboardTileHeader = GadgetNames.BENCHMARK_TOP_TEN_CONSTITUENTS,
                            DashboardTileObject = new ViewTopBenchmarkSecurities(new ViewModelTopBenchmarkSecurities(GetDashboardGadgetParam()))
                        });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        /// <summary>
        /// GadgetRelativePerformaceCommand Execution Method - Add Gadget - RELATIVE_PERFORMANCE
        /// </summary>
        /// <param name="param">SenderInfo</param>
        private void GadgetRelativePerformanceCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                _eventAggregator.GetEvent<DashboardTileViewItemAdded>().Publish
                        (new DashboardTileViewItemInfo
                        {
                            DashboardTileHeader = GadgetNames.PERFORMANCE_RELATIVE_PERFORMANCE,
                            DashboardTileObject = new ViewRelativePerformance(new ViewModelRelativePerformance(GetDashboardGadgetParam()))
                        });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        /// <summary>
        /// GadgetCountryActivePositionCommand Execution Method - Add Gadget - COUNTRY_ACTIVE_POSITION
        /// </summary>
        /// <param name="param">SenderInfo</param>
        private void GadgetCountryActivePositionCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                _eventAggregator.GetEvent<DashboardTileViewItemAdded>().Publish
                        (new DashboardTileViewItemInfo
                        {
                            DashboardTileHeader = GadgetNames.PERFORMANCE_COUNTRY_ACTIVE_POSITION,
                            DashboardTileObject = new ViewRelativePerformanceCountryActivePosition(new ViewModelRelativePerformanceCountryActivePosition(GetDashboardGadgetParam()))
                        });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        /// <summary>
        /// GadgetSectorActivePositionCommand Execution Method - Add Gadget - SECTOR_ACTIVE_POSITION
        /// </summary>
        /// <param name="param">SenderInfo</param>
        private void GadgetSectorActivePositionCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                _eventAggregator.GetEvent<DashboardTileViewItemAdded>().Publish
                        (new DashboardTileViewItemInfo
                        {
                            DashboardTileHeader = GadgetNames.PERFORMANCE_SECTOR_ACTIVE_POSITION,
                            DashboardTileObject = new ViewRelativePerformanceSectorActivePosition(new ViewModelRelativePerformanceSectorActivePosition(GetDashboardGadgetParam()))
                        });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        /// <summary>
        /// GadgetSecurityActivePositionCommand Execution Method - Add Gadget - SECURITY_ACTIVE_POSITION
        /// </summary>
        /// <param name="param">SenderInfo</param>
        private void GadgetSecurityActivePositionCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                _eventAggregator.GetEvent<DashboardTileViewItemAdded>().Publish
                        (new DashboardTileViewItemInfo
                        {
                            DashboardTileHeader = GadgetNames.PERFORMANCE_SECTOR_ACTIVE_POSITION,
                            DashboardTileObject = new ViewRelativePerformanceSecurityActivePosition(new ViewModelRelativePerformanceSecurityActivePosition(GetDashboardGadgetParam()))
                        });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        /// <summary>
        /// GadgetTopContributorCommand Execution Method - Add Gadget - TOP_CONTRIBUTOR
        /// </summary>
        /// <param name="param">SenderInfo</param>
        private void GadgetTopContributorCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                _eventAggregator.GetEvent<DashboardTileViewItemAdded>().Publish
                        (new DashboardTileViewItemInfo
                        {
                            DashboardTileHeader = GadgetNames.PERFORMANCE_TOP_CONTRIBUTOR,
                            DashboardTileObject = new ViewTopContributor(new ViewModelTopContributor(GetDashboardGadgetParam()))
                        });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        /// <summary>
        /// GadgetTopDetractorCommand Execution Method - Add Gadget - TOP_DETRACTOR
        /// </summary>
        /// <param name="param">SenderInfo</param>
        private void GadgetTopDetractorCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                _eventAggregator.GetEvent<DashboardTileViewItemAdded>().Publish
                        (new DashboardTileViewItemInfo
                        {
                            DashboardTileHeader = GadgetNames.PERFORMANCE_TOP_DETRACTOR,
                            DashboardTileObject = new ViewTopDetractor(new ViewModelTopDetractor(GetDashboardGadgetParam()))
                        });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        /// <summary>
        /// GadgetContributorDetractorCommand Execution Method - Add Gadget - CONTRIBUTOR_DETRACTOR
        /// </summary>
        /// <param name="param">SenderInfo</param>
        private void GadgetContributorDetractorCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                _eventAggregator.GetEvent<DashboardTileViewItemAdded>().Publish
                        (new DashboardTileViewItemInfo
                        {
                            DashboardTileHeader = GadgetNames.PERFORMANCE_CONTRIBUTOR_DETRACTOR,
                            DashboardTileObject = new ViewContributorDetractor(new ViewModelContributorDetractor(GetDashboardGadgetParam()))
                        });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        /// <summary>
        /// GadgetSaveCommand Execution Method - Save Dashboard Preference
        /// </summary>
        /// <param name="param">SenderInfo</param>
        private void GadgetSaveCommandMethod(object param)
        {
            Logging.LogBeginMethod(_logger, String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name));
            try
            {
                _eventAggregator.GetEvent<DashboardGadgetSave>().Publish(null);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name));
        }

        private void RelativePerformanceCommandMethod(object param)
        {
            _regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewRelativePerformance", UriKind.Relative));
        }

        private void PortfolioDetailsCommandMethod(object param)
        {
            _regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewPortfolioDetails", UriKind.Relative));
        }





        private void PerformanceGraphCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                _eventAggregator.GetEvent<DashboardTileViewItemAdded>().Publish
                        (new DashboardTileViewItemInfo
                        {
                            DashboardTileHeader = GadgetNames.PERFORMANCE_GRAPH,
                            DashboardTileObject = new ViewPerformanceGadget(new ViewModelPerformanceGadget(GetDashboardGadgetParam()))
                        });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        private void PerformanceGridCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                _eventAggregator.GetEvent<DashboardTileViewItemAdded>().Publish
                        (new DashboardTileViewItemInfo
                        {
                            DashboardTileHeader = GadgetNames.PERFORMANCE_GRID,
                            DashboardTileObject = new ViewPerformanceGrid(new ViewModelPerformanceGrid(GetDashboardGadgetParam()))
                        });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        private void HeatMapCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                _eventAggregator.GetEvent<DashboardTileViewItemAdded>().Publish
                        (new DashboardTileViewItemInfo
                        {
                            DashboardTileHeader = GadgetNames.PERFORMANCE_HEAT_MAP,
                            DashboardTileObject = new ViewHeatMap(new ViewModelHeatMap(GetDashboardGadgetParam()))
                        });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }



        private void AttributionCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                _eventAggregator.GetEvent<DashboardTileViewItemAdded>().Publish
                        (new DashboardTileViewItemInfo
                        {
                            DashboardTileHeader = GadgetNames.PERFORMANCE_ATTRIBUTION,
                            DashboardTileObject = new ViewAttribution(new ViewModelAttribution(GetDashboardGadgetParam()))
                        });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }
        #endregion

        #region Callback Methods
        /// <summary>
        /// RetrieveEntitySelectionData Callback Method
        /// </summary>
        /// <param name="result">List of EntitySelectionData objects</param>
        private void RetrieveEntitySelectionDataCallbackMethod(List<EntitySelectionData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);

            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result.ToString(), 1);
                    EntitySelectionInfo = result
                            .OrderBy(t => t.LongName)
                            .ToList();
                }
                else
                {
                    MessageBox.Show("Message: Argument Null\nStackTrace: " + methodNamespace + ":result", "ArgumentNullDebug", MessageBoxButton.OK);
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        /// <summary>
        /// RetrievePortfolioSelectionData Callback Method
        /// </summary>
        /// <param name="result">List of PortfolioSelectionData objects</param>
        private void RetrievePortfolioSelectionDataCallbackMethod(List<PortfolioSelectionData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);

            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result.ToString(), 1);

                    //FundReference = new CollectionViewSource();
                    //FundReferenceData = new ObservableCollection<PortfolioSelectionData>(result);
                    //FundReferenceSource = new ObservableCollection<GroupSelectionData>();

                    //foreach (PortfolioSelectionData item in FundReferenceData)
                    //{
                    //    FundReferenceSource.Add(new GroupSelectionData()
                    //    {
                    //        Category = item.Category,
                    //        Header = item.Name,
                    //        Detail = item.Name
                    //    });
                    //}
                    //FundReference.GroupDescriptions.Add(new PropertyGroupDescription("Category"));
                    //FundReference.SortDescriptions.Add(new System.ComponentModel.SortDescription
                    //{
                    //    PropertyName = "Category",
                    //    Direction = System.ComponentModel.ListSortDirection.Ascending
                    //});
                    //FundReference.Source = FundReferenceSource;

                    PortfolioSelectionInfo = result.OrderBy(o => o.PortfolioId).ToList();

                }
                else
                {
                    MessageBox.Show("Message: Argument Null\nStackTrace: " + methodNamespace + ":result", "ArgumentNullDebug", MessageBoxButton.OK);
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        /// <summary>
        /// RetrieveMarketSnapshotSelectionData Callback Method
        /// </summary>
        /// <param name="result">List of MarketSnapshotSelectionData objects</param>
        private void RetrieveMarketSnapshotSelectionDataCallbackMethod(List<MarketSnapshotSelectionData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result.ToString(), 1);
                    try
                    {
                        MarketSnapshotSelectionInfo = result;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                        Logging.LogException(_logger, ex);
                    }
                }
                else
                {
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        #endregion

        #region Helper Methods
        /// <summary>
        /// Get DashboardGadgetParam object
        /// </summary>
        /// <returns>DashboardGadgetParam</returns>
        private DashboardGadgetParam GetDashboardGadgetParam()
        {
            DashboardGadgetParam param;
            Logging.LogBeginMethod(_logger, String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name));
            try
            {
                param = new DashboardGadgetParam()
                    {
                        DBInteractivity = _dbInteractivity,
                        EventAggregator = _eventAggregator,
                        LoggerFacade = _logger,
                        DashboardGadgetPayload = SelectorPayload
                    };
            }
            catch (Exception ex)
            {
                param = null;
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "NullReferenceException", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }

            Logging.LogEndMethod(_logger, String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name));
            return param;
        }

        /// <summary>
        /// Updates the visibility properties of selectors based on the ToolBoxItemVisibility static class values
        /// </summary>
        private void UpdateToolBoxSelectorVisibility()
        {
            SecuritySelectorVisibility = ToolBoxItemVisibility.SECURITY_SELECTOR_VISIBILITY;
            PortfolioSelectorVisibility = ToolBoxItemVisibility.PORTFOLIO_SELECTOR_VISIBILITY;
            EffectiveDateSelectorVisibility = ToolBoxItemVisibility.EFFECTIVE_DATE_SELECTOR_VISIBILITY;
            PeriodSelectorVisibility = ToolBoxItemVisibility.PERIOD_SELECTOR_VISIBILITY;
            //CountrySelectorVisibility = ToolBoxItemVisibility.COUNTRY_SELECTOR_VISIBILITY;
            //SectorSelectorVisibility = ToolBoxItemVisibility.SECTOR_SELECTOR_VISIBILITY;
            //IndustrySelectorVisibility = ToolBoxItemVisibility.INDUSTRY_SELECTOR_VISIBILITY;
            //RegionSelectorVisibility = ToolBoxItemVisibility.REGION_SELECTOR_VISIBILITY;
            SnapshotSelectorVisibility = ToolBoxItemVisibility.SNAPSHOT_SELECTOR_VISIBILITY;
        }

        #endregion

    }
}
