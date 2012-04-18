using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel.Composition;
using GreenField.ServiceCaller;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Logging;
using GreenField.Common.Helper;
using Telerik.Windows.Controls;
using System.Reflection;
using GreenField.DashboardModule.Helpers;
using GreenField.Common;
using GreenField.DashBoardModule.Helpers;
using GreenField.Gadgets.Views;
using GreenField.Gadgets.ViewModels;

namespace GreenField.DashboardModule.Views
{
    [Export]
    public partial class ViewDashboardPortfolioPerformanceRelativePerformance : UserControl
    {
        #region Fields
        private IEventAggregator _eventAggregator;
        private ILoggerFacade _logger;
        private IDBInteractivity _dBInteractivity;
        #endregion

        [ImportingConstructor]
        public ViewDashboardPortfolioPerformanceRelativePerformance(ILoggerFacade logger, IEventAggregator eventAggregator,
            IDBInteractivity dbInteractivity)
        {
            InitializeComponent();

            _eventAggregator = eventAggregator;
            _logger = logger;    
            _dBInteractivity = dbInteractivity;

            _eventAggregator.GetEvent<DashboardGadgetLoad>().Subscribe(HandleDashboardGadgetLoad);
            
        }

        public void HandleDashboardGadgetLoad(DashboardGadgetPayload payload)
        {
            if (this.rtvDashboard.Items.Count > 0)
                return;

            DashboardGadgetParam param = new DashboardGadgetParam()
            {
                DashboardGadgetPayload = payload,
                DBInteractivity = _dBInteractivity,
                EventAggregator = _eventAggregator,
                LoggerFacade = _logger
            };

            this.rtvDashboard.Items.Add(new RadTileViewItem
            {
                Header = new Telerik.Windows.Controls.HeaderedContentControl { Content = GadgetNames.PERFORMANCE_RELATIVE_PERFORMANCE, Foreground = new SolidColorBrush(Colors.White), FontSize = 8, FontFamily = new FontFamily("Arial") },
                RestoredHeight = 400,
                Content = new ViewRelativePerformance(new ViewModelRelativePerformance(param))
            });

            this.rtvDashboard.Items.Add(new RadTileViewItem
            {
                RestoredHeight = 400,
                Header = new Telerik.Windows.Controls.HeaderedContentControl { Content = GadgetNames.PERFORMANCE_COUNTRY_ACTIVE_POSITION, Foreground = new SolidColorBrush(Colors.White), FontSize = 8, FontFamily = new FontFamily("Arial") },
                Content = new ViewRelativePerformanceCountryActivePosition(new ViewModelRelativePerformanceCountryActivePosition(param))
            });
                        
            this.rtvDashboard.Items.Add(new RadTileViewItem
            {
                RestoredHeight = 400,
                Header = new Telerik.Windows.Controls.HeaderedContentControl { Content = GadgetNames.PERFORMANCE_SECURITY_ACTIVE_POSITION, Foreground = new SolidColorBrush(Colors.White), FontSize = 8, FontFamily = new FontFamily("Arial") },
                Content = new ViewRelativePerformanceSectorActivePosition(new ViewModelRelativePerformanceSectorActivePosition(param))
            });

            this.rtvDashboard.Items.Add(new RadTileViewItem
            {
                RestoredHeight = 400,
                Header = new Telerik.Windows.Controls.HeaderedContentControl { Content = GadgetNames.PERFORMANCE_SECTOR_ACTIVE_POSITION, Foreground = new SolidColorBrush(Colors.White), FontSize = 8, FontFamily = new FontFamily("Arial") },
                Content = new ViewRelativePerformanceSecurityActivePosition(new ViewModelRelativePerformanceSecurityActivePosition(param))
            });

            this.rtvDashboard.Items.Add(new RadTileViewItem
            {
                RestoredHeight = 400,
                Header = new Telerik.Windows.Controls.HeaderedContentControl { Content = GadgetNames.PERFORMANCE_CONTRIBUTOR_DETRACTOR, Foreground = new SolidColorBrush(Colors.White), FontSize = 8, FontFamily = new FontFamily("Arial") },
                Content = new ViewContributorDetractor(new ViewModelContributorDetractor(param))
            });
        }
    }
}
