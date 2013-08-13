using System.ComponentModel.Composition;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Regions;
using Telerik.Windows.Controls;
using GreenField.Common;
using GreenField.Common.Helper;
using GreenField.Gadgets.Helpers;
using GreenField.Gadgets.ViewModels;
using GreenField.Gadgets.Views;
using GreenField.ServiceCaller;

namespace GreenField.DashboardModule.Views
{
    /// <summary>
    /// Code behind for ViewDashboardPortfolioSnapshot
    /// </summary>
    [Export]
    public partial class ViewDashboardCompanySnapshotSummary : UserControl, INavigationAware
    {
        #region Fields
        /// <summary>
        /// MEF event aggreagator instance
        /// </summary>
        private IEventAggregator eventAggregator;
        /// <summary>
        /// Logging instance
        /// </summary>
        private ILoggerFacade logger;
        /// <summary>
        /// Service caller instance
        /// </summary>
        private IDBInteractivity dBInteractivity;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger">ILoggerFacade</param>
        /// <param name="eventAggregator">IEventAggregator</param>
        /// <param name="dbInteractivity">IDBInteractivity</param>        
        [ImportingConstructor]
        public ViewDashboardCompanySnapshotSummary(ILoggerFacade logger1, IEventAggregator eventAggregator1,
            IDBInteractivity dbInteractivity)
        {
            InitializeComponent();
            eventAggregator = eventAggregator1;
            logger = logger1;
            dBInteractivity = dbInteractivity;
            eventAggregator.GetEvent<DashboardGadgetLoad>().Subscribe(HandleDashboardGadgetLoad);
        }
        #endregion

        #region Event Handler
        /// <summary>
        /// DashboardGadgetLoad Event Handler
        /// </summary>
        /// <param name="payload">DashboardGadgetPayload</param>
        public void HandleDashboardGadgetLoad(DashboardGadgetPayload payload)
        {
            if (this.rtvDashboard.Items.Count > 0)
            {
                return;
            }
            DashboardGadgetParam param = new DashboardGadgetParam()
            {
                DashboardGadgetPayload = payload,
                DBInteractivity = dBInteractivity,
                EventAggregator = eventAggregator,
                LoggerFacade = logger
            };

            //Company OverView Gadget
            this.rtvDashboard.Items.Add(new RadTileViewItem
            {
                Header = new Telerik.Windows.Controls.HeaderedContentControl
                {
                    Content = GadgetNames.SECURITY_OVERVIEW,
                    Foreground =
                        new SolidColorBrush(Colors.Black),
                    FontSize = 12,
                    FontFamily = new FontFamily("Arial")
                },
                Content = new ViewSecurityOverview(new ViewModelSecurityOverview(param))
            });

            //Fair Value Gadget
            this.rtvDashboard.Items.Add(new RadTileViewItem
            {
                Header = new Telerik.Windows.Controls.HeaderedContentControl
                {
                    Content =
                        GadgetNames.PORTFOLIO_CONSTRUCTION_FAIR_VALUE_COMPOSITION_SUMMARY,
                    Foreground = new SolidColorBrush(Colors.Black),
                    FontSize = 12,
                    FontFamily = new FontFamily("Arial")
                },
                Content = new ViewFairValueCompositionSummary(new ViewModelFairValueCompositionSummary(param))
            });


            //Holdings and Positioning Gadget
            this.rtvDashboard.Items.Add(new RadTileViewItem
            {
                Header = new Telerik.Windows.Controls.HeaderedContentControl
                {
                    Content = GadgetNames.INTERNAL_RESEARCH_PRICING_DETAILED,
                    Foreground = new SolidColorBrush(Colors.Black),
                    FontSize = 12,
                    FontFamily = new FontFamily("Arial")
                },
                Content = new ViewCompositeFund(new ViewModelCompositeFund(param))
            });

            //Summary Financials and Valuations Gadget
            this.rtvDashboard.Items.Add(new RadTileViewItem
            {
                Header = new Telerik.Windows.Controls.HeaderedContentControl
                {
                    Content = GadgetNames.GADGET_WITH_PERIOD_COLUMNS_COA_SPECIFIC,
                    Foreground = new SolidColorBrush(Colors.Black),
                    FontSize = 12,
                    FontFamily = new FontFamily("Arial")
                },
                RestoredHeight = 635,
                Content = new ViewCOASpecific(new ViewModelCOASpecific(param))
            });

            //Trade History Gadget
            this.rtvDashboard.Items.Add(new RadTileViewItem
            {
                Header = new Telerik.Windows.Controls.HeaderedContentControl
                {
                    Content = GadgetNames.HOLDINGS_CHART_EXTENTION,
                    Foreground = new SolidColorBrush(Colors.Black),
                    FontSize = 12,
                    FontFamily = new FontFamily("Arial")
                },
                RestoredHeight = 300,
                Content = new ViewSlice1ChartExtension(new ViewModelSlice1ChartExtension(param))
            });

            //Comparison with Consensus Gadget
            this.rtvDashboard.Items.Add(new RadTileViewItem
            {
                Header = new Telerik.Windows.Controls.HeaderedContentControl
                {
                    Content =
                        GadgetNames.INTERNAL_RESEARCH_CONSESUS_ESTIMATE_SUMMARY,
                    Foreground = new SolidColorBrush(Colors.Black),
                    FontSize = 12,
                    FontFamily = new FontFamily("Arial")
                },
                RestoredHeight = 220,
                Content = new ViewConsensusEstimateSummary(new ViewModelConsensusEstimateSummary(param))
            });


            //Relative Performance Gadget
            this.rtvDashboard.Items.Add(new RadTileViewItem
            {
                Header = new Telerik.Windows.Controls.HeaderedContentControl
                {
                    Content = GadgetNames.BENCHMARK_RELATIVE_PERFORMANCE,
                    Foreground = new SolidColorBrush(Colors.Black),
                    FontSize = 12,
                    FontFamily = new FontFamily("Arial")
                },
                RestoredHeight = 200,
                Content = new ViewRelativePerformanceUI(new ViewModelRelativePerformanceUI(param))
            });

            //Market Data Gadget
            this.rtvDashboard.Items.Add(new RadTileViewItem
            {
                Header = new Telerik.Windows.Controls.HeaderedContentControl
                {
                    Content = GadgetNames.EXTERNAL_RESEARCH_BASIC_DATA,
                    Foreground = new SolidColorBrush(Colors.Black),
                    FontSize = 12,
                    FontFamily = new FontFamily("Arial")
                },
                Content = new ViewBasicData(new ViewModelBasicData(param))
            });
        }
        #endregion

        #region INavigationAware methods
        /// <summary>
        /// Returns true if satisfies requisite condition
        /// </summary>
        /// <param name="navigationContext">NavigationContext</param>
        /// <returns>True/False</returns>
        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        /// <summary>
        /// Executed on navigation from this view
        /// </summary>
        /// <param name="navigationContext">NavigationContext</param>
        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            SetIsActiveOnDahsboardItems(false);
        }

        /// <summary>
        /// Executed on navigation to this view
        /// </summary>
        /// <param name="navigationContext">NavigationContext</param>
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            SetIsActiveOnDahsboardItems(true);
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Set IsActive property on Dashboard content
        /// </summary>
        /// <param name="value">IsActive value</param>
        private void SetIsActiveOnDahsboardItems(bool value)
        {
            int a = rtvDashboard.Items.Count;
            foreach (RadTileViewItem item in this.rtvDashboard.Items)
            {
                ViewBaseUserControl control = (ViewBaseUserControl)item.Content;
                if (control != null)
                {
                    control.IsActive = value;
                }
            }
        }
        #endregion
    }
}
