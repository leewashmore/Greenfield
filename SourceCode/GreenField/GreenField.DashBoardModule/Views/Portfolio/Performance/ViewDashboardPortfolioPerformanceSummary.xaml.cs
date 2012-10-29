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
    /// Code behind for ViewDashboardPortfolioPerformanceSummary
    /// </summary>
    [Export]
    public partial class ViewDashboardPortfolioPerformanceSummary : UserControl, INavigationAware
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
        private IDBInteractivity dbInteractivity;

        /// <summary>
        /// Region Manager
        /// </summary>
        private IRegionManager regionManager;
        #endregion

        public bool IsActive { get; set; }

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger">ILoggerFacade</param>
        /// <param name="eventAggregator">IEventAggregator</param>
        /// <param name="dbInteractivity">IDBInteractivity</param>
        [ImportingConstructor]
        public ViewDashboardPortfolioPerformanceSummary(ILoggerFacade logger, IEventAggregator eventAggregator,
            IDBInteractivity dbInteractivity, IRegionManager regionManager)
        {
            InitializeComponent();
            this.eventAggregator = eventAggregator;
            this.logger = logger;
            this.dbInteractivity = dbInteractivity;
            this.regionManager = regionManager;
            //event subscription
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
                DBInteractivity = dbInteractivity,
                EventAggregator = eventAggregator,
                LoggerFacade = logger,
                RegionManager = regionManager
            };
            this.rtvDashboard.Items.Add(new RadTileViewItem
            {
                Header = new Telerik.Windows.Controls.HeaderedContentControl
                { 
                    Content = GadgetNames.PERFORMANCE_HEAT_MAP, 
                    Foreground = new SolidColorBrush(Colors.Black),
                    FontSize = 12, 
                    FontFamily = new FontFamily("Arial") 
                },
                RestoredHeight = 300,
                Content = new ViewHeatMap(new ViewModelHeatMap(param))                
            });

            this.rtvDashboard.Items.Add(new RadTileViewItem
            {
                Header = new Telerik.Windows.Controls.HeaderedContentControl 
                { 
                    Content = GadgetNames.PERFORMANCE_GRAPH, 
                    Foreground = new SolidColorBrush(Colors.Black), 
                    FontSize = 12, 
                    FontFamily = new FontFamily("Arial") 
                },
                RestoredHeight = 300,
                Content = new ViewPerformanceGadget(new ViewModelPerformanceGadget(param))
            });

            this.rtvDashboard.Items.Add(new RadTileViewItem
            {
                Header = new Telerik.Windows.Controls.HeaderedContentControl 
                { 
                    Content = GadgetNames.PERFORMANCE_GRID, 
                    Foreground = new SolidColorBrush(Colors.Black), 
                    FontSize = 12, 
                    FontFamily = new FontFamily("Arial") 
                },
                RestoredHeight = 150,
                Content = new ViewPerformanceGrid(new ViewModelPerformanceGrid(param))
            });
        }
        #endregion

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            SetIsActiveOnDahsboardItems(false);
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            SetIsActiveOnDahsboardItems(true);
        }

        private void SetIsActiveOnDahsboardItems(bool value)
        {
            foreach (RadTileViewItem item in this.rtvDashboard.Items)
            {
                ViewBaseUserControl control = (ViewBaseUserControl)item.Content;
                if (control != null)
                    control.IsActive = value;
            }
        }
    }
}
