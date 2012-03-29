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
using GreenField.Module.Views;
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
                    DashboardGadgetPayLoad.EntitySelectionData = SecurityReferenceData.Where(entity => entity.ShortName == ((value.Category == "Ticker") ? value.Header : value.Detail)).First();
                    _eventAggregator.GetEvent<SecurityReferenceSetEvent>().Publish(DashboardGadgetPayLoad.EntitySelectionData);
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
                    DashboardGadgetPayLoad.FundSelectionData = FundReferenceData.Where(entity => entity.Name == value.Detail).First();
                    _eventAggregator.GetEvent<FundReferenceSetEvent>().Publish(DashboardGadgetPayLoad.FundSelectionData);
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
                    DashboardGadgetPayLoad.BenchmarkSelectionData = BenchmarkReferenceData.Where(entity => entity.Name == value.Header).First();
                    _eventAggregator.GetEvent<BenchmarkReferenceSetEvent>().Publish(DashboardGadgetPayLoad.BenchmarkSelectionData);
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
                    DashboardGadgetPayLoad.EffectiveDate = value;
                    _eventAggregator.GetEvent<EffectiveDateSet>().Publish(DashboardGadgetPayLoad.EffectiveDate);
                }
            }
        }

        private DashboardGadgetPayLoad _dashboardGadgetPayLoad;

        public DashboardGadgetPayLoad DashboardGadgetPayLoad
        {
            get
            {
                if (_dashboardGadgetPayLoad == null)
                    _dashboardGadgetPayLoad = new DashboardGadgetPayLoad();
                return _dashboardGadgetPayLoad; 
            }
            set { _dashboardGadgetPayLoad = value; }
        }        

        #endregion

        # region ICommand
        public ICommand LogOutCommand
        {
            get { return new DelegateCommand<object>(LogOutCommandMethod); }
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

        public ICommand GadgetHoldingsCommand
        {
            get
            {
                return new DelegateCommand<object>(GadgetHoldingsCommandMethod);
            }
        }

        public ICommand GadgetPerformanceCommand
        {
            get
            {
                return new DelegateCommand<object>(GadgetPerformanceCommandMethod);
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

        public ICommand GadgetRelativePerformaceCommand
        {
            get
            {
                return new DelegateCommand<object>(GadgetRelativePerformaceCommandMethod);
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
                _eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(DashboardGadgetPayLoad);
                _regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashBoard", UriKind.Relative));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name));
        }

        private void GadgetHoldingsCommandMethod(object param)
        {
            _eventAggregator.GetEvent<DashBoardTileViewItemAdded>().Publish
                (new DashBoardTileViewItemInfo
                {
                    DashBoardTileHeader = "Holdings Data",
                    DashBoardTileObject = new HoldingsView()
                });
        }

        private void GadgetPerformanceCommandMethod(object param)
        {
            _eventAggregator.GetEvent<DashBoardTileViewItemAdded>().Publish
                (new DashBoardTileViewItemInfo
                {
                    DashBoardTileHeader = "Performance Data",
                    DashBoardTileObject = new PerformanceView()
                });
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
                _eventAggregator.GetEvent<DashBoardTileViewItemAdded>().Publish
                       (new DashBoardTileViewItemInfo
                       {
                           DashBoardTileHeader = GadgetNames.SECURITY_OVERVIEW,
                           DashBoardTileObject = new ViewSecurityOverview(new ViewModelSecurityOverview(GetDashBoardGadgetParam()))
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
                _eventAggregator.GetEvent<DashBoardTileViewItemAdded>().Publish
                       (new DashBoardTileViewItemInfo
                       {
                           DashBoardTileHeader = GadgetNames.PRICING,
                           DashBoardTileObject = new ViewClosingPriceChart(new ViewModelClosingPriceChart(GetDashBoardGadgetParam()))
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
                _eventAggregator.GetEvent<DashBoardTileViewItemAdded>().Publish
                        (new DashBoardTileViewItemInfo
                        {
                            DashBoardTileHeader = GadgetNames.UNREALIZED_GAINLOSS,
                            DashBoardTileObject = new ViewUnrealizedGainLoss(new ViewModelUnrealizedGainLoss(GetDashBoardGadgetParam()))
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
                _eventAggregator.GetEvent<DashBoardTileViewItemAdded>().Publish
                        (new DashBoardTileViewItemInfo
                        {
                            DashBoardTileHeader = GadgetNames.REGION_BREAKDOWN,
                            DashBoardTileObject = new ViewRegionBreakdown(new ViewModelRegionBreakDown(GetDashBoardGadgetParam()))
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
                _eventAggregator.GetEvent<DashBoardTileViewItemAdded>().Publish
                        (new DashBoardTileViewItemInfo
                        {
                            DashBoardTileHeader = GadgetNames.SECTOR_BREAKDOWN,
                            DashBoardTileObject = new ViewSectorBreakdown(new ViewModelSectorBreakDown(GetDashBoardGadgetParam()))
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
                _eventAggregator.GetEvent<DashBoardTileViewItemAdded>().Publish
                        (new DashBoardTileViewItemInfo
                        {
                            DashBoardTileHeader = GadgetNames.INDEX_CONSTITUENTS,
                            DashBoardTileObject = new ViewIndexConstituents(new ViewModelIndexConstituents(GetDashBoardGadgetParam()))
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
                _eventAggregator.GetEvent<DashBoardTileViewItemAdded>().Publish
                        (new DashBoardTileViewItemInfo
                        {
                            DashBoardTileHeader = GadgetNames.MARKET_CAPITALIZATION,
                            DashBoardTileObject = new ViewMarketCapitalization(new ViewModelMarketCapitalization(GetDashBoardGadgetParam()))
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
                _eventAggregator.GetEvent<DashBoardTileViewItemAdded>().Publish
                        (new DashBoardTileViewItemInfo
                        {
                            DashBoardTileHeader = GadgetNames.TOP_HOLDINGS,
                            DashBoardTileObject = new ViewTopHoldings(new ViewModelTopHoldings(GetDashBoardGadgetParam()))
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
                _eventAggregator.GetEvent<DashBoardTileViewItemAdded>().Publish
                        (new DashBoardTileViewItemInfo
                        {
                            DashBoardTileHeader = GadgetNames.ASSET_ALLOCATION,
                            DashBoardTileObject = new ViewAssetAllocation(new ViewModelAssetAllocation(GetDashBoardGadgetParam()))
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
                _eventAggregator.GetEvent<DashBoardTileViewItemAdded>().Publish
                        (new DashBoardTileViewItemInfo
                        {
                            DashBoardTileHeader = GadgetNames.HOLDINGS_PIECHART,
                            DashBoardTileObject = new ViewHoldingsPieChart(new ViewModelHoldingsPieChart(GetDashBoardGadgetParam()))
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
                _eventAggregator.GetEvent<DashBoardTileViewItemAdded>().Publish
                        (new DashBoardTileViewItemInfo
                        {
                            DashBoardTileHeader = GadgetNames.PORTFOLIO_RISK_RETURNS,
                            DashBoardTileObject = new ViewPortfolioRiskReturns(new ViewModelPortfolioRiskReturns(GetDashBoardGadgetParam()))
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
                _eventAggregator.GetEvent<DashBoardTileViewItemAdded>().Publish
                        (new DashBoardTileViewItemInfo
                        {
                            DashBoardTileHeader = GadgetNames.TOP_BENCHMARK_SECURITIES,
                            DashBoardTileObject = new ViewTopBenchmarkSecurities(new ViewModelTopBenchmarkSecurities(GetDashBoardGadgetParam()))
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
        private void GadgetRelativePerformaceCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                _eventAggregator.GetEvent<DashBoardTileViewItemAdded>().Publish
                        (new DashBoardTileViewItemInfo
                        {
                            DashBoardTileHeader = GadgetNames.RELATIVE_PERFORMANCE,
                            DashBoardTileObject = new ViewRelativePerformance(new ViewModelRelativePerformance(GetDashBoardGadgetParam()))
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
                _eventAggregator.GetEvent<DashBoardTileViewItemAdded>().Publish
                        (new DashBoardTileViewItemInfo
                        {
                            DashBoardTileHeader = GadgetNames.TOP_CONTRIBUTOR,
                            DashBoardTileObject = new ViewTopContributor(new ViewModelTopContributor(GetDashBoardGadgetParam()))
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
                _eventAggregator.GetEvent<DashBoardTileViewItemAdded>().Publish
                        (new DashBoardTileViewItemInfo
                        {
                            DashBoardTileHeader = GadgetNames.TOP_DETRACTOR,
                            DashBoardTileObject = new ViewTopDetractor(new ViewModelTopDetractor(GetDashBoardGadgetParam()))
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
                _eventAggregator.GetEvent<DashBoardTileViewItemAdded>().Publish
                        (new DashBoardTileViewItemInfo
                        {
                            DashBoardTileHeader = GadgetNames.CONTRIBUTOR_DETRACTOR,
                            DashBoardTileObject = new ViewContributorDetractor(new ViewModelContributorDetractor(GetDashBoardGadgetParam()))
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
                    SecurityReferenceData = new ObservableCollection<EntitySelectionData>(result.Where(e => e.Type == EntityTypes.SECURITY));
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
                        PropertyName = "Category",
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
        /// Get DashBoardGadgetParam object
        /// </summary>
        /// <returns>DashBoardGadgetParam</returns>
        private DashBoardGadgetParam GetDashBoardGadgetParam()
        {
            DashBoardGadgetParam param;
            Logging.LogBeginMethod(_logger, String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name));
            try
            {
                param = new DashBoardGadgetParam()
                    {
                        DBInteractivity = _dbInteractivity,
                        EventAggregator = _eventAggregator,
                        LoggerFacade = _logger,
                        DashboardGadgetPayLoad = DashboardGadgetPayLoad
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
