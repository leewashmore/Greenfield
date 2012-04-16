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
using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Prism.Commands;
using System.Windows.Browser;
using GreenField.Common;
using GreenField.ServiceCaller;
using Microsoft.Practices.Prism.Modularity;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Logging;
using GreenField.Gadgets.Views;
using System.Windows.Data;
using System.Collections.ObjectModel;
using GreenField.ServiceCaller.ProxyDataDefinitions;
using GreenField.App.Models;
using GreenField.Gadgets.ViewModels;
using GreenField.Common.Helper;

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
                                        _dbInteractivity.RetrieveEntitySelectionData(RetrieveEntitySelectionDataCallBackMethod);
                                        _dbInteractivity.RetrieveFundSelectionData(RetrieveFundSelectionDataCallBackMethod);
                                        _dbInteractivity.RetrieveBenchmarkSelectionData(RetrieveBenchmarkSelectionDataCallBackMethod);
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

        private CollectionViewSource _securityReference;
        public CollectionViewSource SecurityReference
        {
            get { return _securityReference; }
            set
            {
                _securityReference = value;
                RaisePropertyChanged(() => this.SecurityReference);                
            }
        }

        private CollectionViewSource _fundReference;
        public CollectionViewSource FundReference
        {
            get { return _fundReference; }
            set
            {
                _fundReference = value;
                RaisePropertyChanged(() => this.FundReference);
            }
        }

        private CollectionViewSource _benchmarkReference;
        public CollectionViewSource BenchmarkReference
        {
            get { return _benchmarkReference; }
            set
            {
                _benchmarkReference = value;
                RaisePropertyChanged(() => this.BenchmarkReference);
            }
        }

        public ObservableCollection<EntitySelectionData> SecurityReferenceData { get; set; }
        public ObservableCollection<FundSelectionData> FundReferenceData { get; set; }
        public ObservableCollection<BenchmarkSelectionData> BenchmarkReferenceData { get; set; }

        

        public ObservableCollection<GroupSelectionData> SecurityReferenceSource { get; set; }
        public ObservableCollection<GroupSelectionData> FundReferenceSource { get; set; }
        public ObservableCollection<GroupSelectionData> BenchmarkReferenceSource { get; set; }
        
        private string _securityEnteredText;
        public string SecurityEnteredText
        {
            get { return _securityEnteredText; }
            set
            {
                _securityEnteredText = value;
                RaisePropertyChanged(() => this.SecurityEnteredText);
                if (value != String.Empty)
                    SecurityReference.Source = SecurityReferenceSource.Where(o => o.Header.ToLower().Contains(value.ToLower()));
                else
                    SecurityReference.Source = SecurityReferenceSource;
            }
        }

        private string _fundEnteredText;
        public string FundEnteredText
        {
            get { return _fundEnteredText; }
            set
            {
                _fundEnteredText = value;
                RaisePropertyChanged(() => this.FundEnteredText);
                if (value != String.Empty)
                    FundReference.Source = FundReferenceSource.Where(o => o.Header.ToLower().Contains(value.ToLower()));
                else
                    FundReference.Source = FundReferenceSource;
            }
        }

        private string _benchmarkEnteredText;
        public string BenchmarkEnteredText
        {
            get { return _benchmarkEnteredText; }
            set
            {
                _benchmarkEnteredText = value;
                RaisePropertyChanged(() => this.BenchmarkEnteredText);
                if (value != String.Empty)
                    BenchmarkReference.Source = BenchmarkReferenceSource.Where(o => o.Header.ToLower().Contains(value.ToLower()));
                else
                    BenchmarkReference.Source = BenchmarkReferenceSource;
            }
        }

        private GroupSelectionData _selectedSecurityReference;
        public GroupSelectionData SelectedSecurityReference
        {
            get { return _selectedSecurityReference; }
            set
            {
                _selectedSecurityReference = value;
                RaisePropertyChanged(()=> this.SelectedSecurityReference);
                if (value != null)
                {
                    DashboardGadgetPayload.EntitySelectionData = SecurityReferenceData.Where(entity => entity.ShortName == ((value.Category == "Ticker") ? value.Header : value.Detail)).First();
                    _eventAggregator.GetEvent<SecurityReferenceSetEvent>().Publish(DashboardGadgetPayload.EntitySelectionData);
                }
            }
        }

        private GroupSelectionData _selectedFundReference;
        public GroupSelectionData SelectedFundReference
        {
            get { return _selectedFundReference; }
            set
            {
                _selectedFundReference = value;
                RaisePropertyChanged(() => this.SelectedFundReference);
                if (value != null)
                {
                    DashboardGadgetPayload.FundSelectionData = FundReferenceData.Where(entity => entity.Name == value.Detail).First();
                    _eventAggregator.GetEvent<FundReferenceSetEvent>().Publish(DashboardGadgetPayload.FundSelectionData);
                }
            }
        }

        private GroupSelectionData _selectedBenchmarkReference;
        public GroupSelectionData SelectedBenchmarkReference
        {
            get { return _selectedBenchmarkReference; }
            set
            {
                _selectedBenchmarkReference = value;
                RaisePropertyChanged(() => this.SelectedBenchmarkReference);
                if (value != null)
                {
                    DashboardGadgetPayload.BenchmarkSelectionData = BenchmarkReferenceData.Where(entity => entity.Name == value.Header).First();
                    _eventAggregator.GetEvent<BenchmarkReferenceSetEvent>().Publish(DashboardGadgetPayload.BenchmarkSelectionData);
                }
            }
        }

        private DateTime _selectedEffectiveDateReference = DateTime.Today;
        public DateTime SelectedEffectiveDateReference
        {
            get 
            {
                return _selectedEffectiveDateReference; 
            }
            set 
            {
                if (_selectedEffectiveDateReference != value)
                {
                    _selectedEffectiveDateReference = value;
                    RaisePropertyChanged(() => this.SelectedEffectiveDateReference);
                    DashboardGadgetPayload.EffectiveDate = value;
                    _eventAggregator.GetEvent<EffectiveDateSet>().Publish(DashboardGadgetPayload.EffectiveDate);
                }
            }
        }

        private DashboardGadgetPayload _dashboardGadgetPayload;
        public DashboardGadgetPayload DashboardGadgetPayload
        {
            get
            {
                if (_dashboardGadgetPayload == null)
                    _dashboardGadgetPayload = new DashboardGadgetPayload();
                return _dashboardGadgetPayload; 
            }
            set 
            {
                _dashboardGadgetPayload = value;                
            }
        }        

        #endregion

        # region ICommand
        public ICommand LogOutCommand
        {
            get { return new DelegateCommand<object>(LogOutCommandMethod); }
        }

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

        public ICommand DashboardCompanyValuationFairValueCommand
        {
            get { return new DelegateCommand<object>(DashboardCompanyValuationFairValueCommandMethod); }
        }

        public ICommand DashboardCompanyValuationDiscountedCashFlowCommand
        {
            get { return new DelegateCommand<object>(DashboardCompanyValuationDiscountedCashFlowCommandMethod); }
        }

        public ICommand DashboardCompanyDocumentsCommand
        {
            get { return new DelegateCommand<object>(DashboardCompanyDocumentsCommandMethod); }
        }

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

        public ICommand DashboardCompanyCorporateGovernanceQuestionnaireCommand
        {
            get { return new DelegateCommand<object>(DashboardCompanyCorporateGovernanceQuestionnaireCommandMethod); }
        }

        public ICommand DashboardCompanyCorporateGovernanceReportCommand
        {
            get { return new DelegateCommand<object>(DashboardCompanyCorporateGovernanceReportCommandMethod); }
        }

        

        public ICommand DetailedEstimateCommand
        {
            get { return new DelegateCommand<object>(DetailedEstimateCommandMethod); }
        }

        public ICommand ConsensusEstimateCommand
        {
            get { return new DelegateCommand<object>(ConsensusEstimateCommandMethod); }
        }

        public ICommand HoldingsCommand
        {
            get { return new DelegateCommand<object>(HoldingsCommandMethod); }
        }

        public ICommand PerformanceCommand
        {
            get { return new DelegateCommand<object>(PerformanceCommandMethod); }
        }

        public ICommand ReferenceCommand
        {
            get { return new DelegateCommand<object>(ReferenceCommandMethod); }
        }

        public ICommand AggregateDataCommand
        {
            get { return new DelegateCommand<object>(AggregateDataCommandMethod); }
        }

        public ICommand UserManagementCommand
        {
            get { return new DelegateCommand<object>(UserManagementCommandMethod); }
        }

        public ICommand RoleManagementCommand
        {
            get { return new DelegateCommand<object>(RoleManagementCommandMethod); }
        }

        public ICommand DailyMorningSnapshotCommand
        {
            get { return new DelegateCommand<object>(DailyMorningSnapshotCommandMethod); }
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
        
        private void DashboardCompanySnapshotSummaryCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                _eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(DashboardGadgetPayload);
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
                _eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(DashboardGadgetPayload);
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
                _eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(DashboardGadgetPayload);
                _regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardCompanySnapshotTearSheet", UriKind.Relative));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        private void DashboardCompanyFinancialsSummaryCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                _eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(DashboardGadgetPayload);
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
                _eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(DashboardGadgetPayload);
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
                _eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(DashboardGadgetPayload);
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
                _eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(DashboardGadgetPayload);
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
                _eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(DashboardGadgetPayload);
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
                _eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(DashboardGadgetPayload);
                _regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardCompanyFinancialsPeerComparison", UriKind.Relative));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        private void DashboardCompanyEstimatesConsensusCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                _eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(DashboardGadgetPayload);
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
                _eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(DashboardGadgetPayload);
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
                _eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(DashboardGadgetPayload);
                _regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardCompanyEstimatesComparison", UriKind.Relative));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        private void DashboardCompanyValuationFairValueCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                _eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(DashboardGadgetPayload);
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
                _eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(DashboardGadgetPayload);
                _regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardCompanyValuationDiscountedCashFlow", UriKind.Relative));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        private void DashboardCompanyDocumentsCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                _eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(DashboardGadgetPayload);
                _regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardCompanyDocuments", UriKind.Relative));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        private void DashboardCompanyChartingClosingPriceCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                _eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(DashboardGadgetPayload);
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
                _eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(DashboardGadgetPayload);
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
                _eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(DashboardGadgetPayload);
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
                _eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(DashboardGadgetPayload);
                _regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardCompanyChartingValuation", UriKind.Relative));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        private void DashboardCompanyCorporateGovernanceQuestionnaireCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                _eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(DashboardGadgetPayload);
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
                _eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(DashboardGadgetPayload);
                _regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardCompanyCorporateGovernanceReport", UriKind.Relative));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }



        

        private void DetailedEstimateCommandMethod(object param)
        {
            try
            {
                _regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("DetailedEstimateView", UriKind.Relative));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "NullReferenceException", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
        }

        private void ConsensusEstimateCommandMethod(object param)
        {
            try
            {
                _regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ConsensusEstimateView", UriKind.Relative));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "NullReferenceException", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
        }

        private void HoldingsCommandMethod(object param)
        {
            try
            {
                _regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("HoldingsView", UriKind.Relative));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "NullReferenceException", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
        }

        private void PerformanceCommandMethod(object param)
        {
            try
            {
                _regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("PerformanceView", UriKind.Relative));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "NullReferenceException", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
        }

        private void ReferenceCommandMethod(object param)
        {
            try
            {
                _regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ReferenceView", UriKind.Relative));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "NullReferenceException", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
        }

        private void AggregateDataCommandMethod(object param)
        {
            try
            {
                _regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("AggregatedDataView", UriKind.Relative));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "NullReferenceException", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
        }

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
                _eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(DashboardGadgetPayload);
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
                            DashboardTileHeader = GadgetNames.REGION_BREAKDOWN,
                            DashboardTileObject = new ViewRegionBreakdown(new ViewModelRegionBreakDown(GetDashboardGadgetParam()))
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
                            DashboardTileHeader = GadgetNames.SECTOR_BREAKDOWN,
                            DashboardTileObject = new ViewSectorBreakdown(new ViewModelSectorBreakDown(GetDashboardGadgetParam()))
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
                            DashboardTileHeader = GadgetNames.INDEX_CONSTITUENTS,
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
                            DashboardTileHeader = GadgetNames.MARKET_CAPITALIZATION,
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
                            DashboardTileHeader = GadgetNames.TOP_HOLDINGS,
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
                            DashboardTileHeader = GadgetNames.ASSET_ALLOCATION,
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
                            DashboardTileHeader = GadgetNames.HOLDINGS_PIECHART,
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
                            DashboardTileHeader = GadgetNames.PORTFOLIO_RISK_RETURNS,
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
                            DashboardTileHeader = GadgetNames.TOP_BENCHMARK_SECURITIES,
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
                            DashboardTileHeader = GadgetNames.BENCHMARK_RELATIVE_PERFORMANCE,
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
                            DashboardTileHeader = GadgetNames.COUNTRY_ACTIVE_POSITION,
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
                            DashboardTileHeader = GadgetNames.SECTOR_ACTIVE_POSITION,
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
                            DashboardTileHeader = GadgetNames.SECTOR_ACTIVE_POSITION,
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
                            DashboardTileHeader = GadgetNames.TOP_CONTRIBUTOR,
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
                            DashboardTileHeader = GadgetNames.TOP_DETRACTOR,
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
                            DashboardTileHeader = GadgetNames.CONTRIBUTOR_DETRACTOR,
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


        private void AttributionCommandMethod(object param) 
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                _eventAggregator.GetEvent<DashboardTileViewItemAdded>().Publish
                        (new DashboardTileViewItemInfo
                        {
                            DashboardTileHeader = GadgetNames.ATTRIBUTION,
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
        private void RetrieveEntitySelectionDataCallBackMethod(List<EntitySelectionData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);

            if (result != null)
            {
                Logging.LogMethodParameter(_logger, methodNamespace, result.ToString(), 1);
                try
                {
                    SecurityReference = new CollectionViewSource();
                    SecurityReferenceData = new ObservableCollection<EntitySelectionData>(result.Where(e => e.Type == EntityTypes.SECURITY).OrderBy(t=> t.LongName));
                    List<EntitySelectionData> a = SecurityReferenceData.ToList();
                    SecurityReferenceSource = new ObservableCollection<GroupSelectionData>();

                    foreach (EntitySelectionData item in SecurityReferenceData)
                    {
                        GroupSelectionData TickerHeaderSecuritySelectionData = new GroupSelectionData()
                        {
                            Category = "Ticker",
                            Header = item.ShortName,
                            Detail = item.LongName
                        };

                        GroupSelectionData IssueNameHeaderSecuritySelectionData = new GroupSelectionData()
                        {
                            Category = "Issuer Name",
                            Header = item.LongName,
                            Detail = item.ShortName
                        };

                        SecurityReferenceSource.Add(TickerHeaderSecuritySelectionData);
                        SecurityReferenceSource.Add(IssueNameHeaderSecuritySelectionData);
                    }

                    SecurityReference.GroupDescriptions.Add(new PropertyGroupDescription("Category"));
                    SecurityReference.SortDescriptions.Add(new System.ComponentModel.SortDescription
                    {
                        PropertyName = "Header",
                        Direction = System.ComponentModel.ListSortDirection.Ascending
                    });
                    SecurityReference.Source = SecurityReferenceSource;
                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                    Logging.LogException(_logger, ex);
                }
            }
            else
            {
                MessageBox.Show("Message: Argument Null\nStackTrace: " + methodNamespace + ":result", "ArgumentNullDebug", MessageBoxButton.OK);
                Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
            }


            Logging.LogEndMethod(_logger, methodNamespace);
        }

        private void RetrieveFundSelectionDataCallBackMethod(List<FundSelectionData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);

            if (result != null)
            {
                Logging.LogMethodParameter(_logger, methodNamespace, result.ToString(), 1);
                try
                {
                    FundReference = new CollectionViewSource();
                    FundReferenceData = new ObservableCollection<FundSelectionData>(result);
                    FundReferenceSource = new ObservableCollection<GroupSelectionData>();

                    foreach (FundSelectionData item in FundReferenceData)
                    {
                        FundReferenceSource.Add(new GroupSelectionData()
                        {
                            Category = item.Category,
                            Header = item.Name,
                            Detail = item.Name
                        });
                    }

                    FundReference.GroupDescriptions.Add(new PropertyGroupDescription("Category"));
                    FundReference.SortDescriptions.Add(new System.ComponentModel.SortDescription
                    {
                        PropertyName = "Category",
                        Direction = System.ComponentModel.ListSortDirection.Ascending
                    });
                    FundReference.Source = FundReferenceSource;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                    Logging.LogException(_logger, ex);
                }
            }
            else
            {
                MessageBox.Show("Message: Argument Null\nStackTrace: " + methodNamespace + ":result", "ArgumentNullDebug", MessageBoxButton.OK);
                Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
            }


            Logging.LogEndMethod(_logger, methodNamespace);
        }

        private void RetrieveBenchmarkSelectionDataCallBackMethod(List<BenchmarkSelectionData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);

            if (result != null)
            {
                Logging.LogMethodParameter(_logger, methodNamespace, result.ToString(), 1);
                try
                {
                    BenchmarkReference = new CollectionViewSource();
                    BenchmarkReferenceData = new ObservableCollection<BenchmarkSelectionData>(result);
                    BenchmarkReferenceSource = new ObservableCollection<GroupSelectionData>();

                    foreach (BenchmarkSelectionData item in BenchmarkReferenceData)
                    {
                        BenchmarkReferenceSource.Add(new GroupSelectionData()
                        {
                            Category = "Group",
                            Header = item.Name,
                            Detail = item.Name
                        });
                    }

                    BenchmarkReference.GroupDescriptions.Add(new PropertyGroupDescription("Category"));
                    BenchmarkReference.SortDescriptions.Add(new System.ComponentModel.SortDescription
                    {
                        PropertyName = "Category",
                        Direction = System.ComponentModel.ListSortDirection.Ascending
                    });
                    BenchmarkReference.Source = BenchmarkReferenceSource;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                    Logging.LogException(_logger, ex);
                }
            }
            else
            {
                MessageBox.Show("Message: Argument Null\nStackTrace: " + methodNamespace + ":result", "ArgumentNullDebug", MessageBoxButton.OK);
                Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
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
                        DashboardGadgetPayload = DashboardGadgetPayload
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
        #endregion


    }
}
