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
using Microsoft.Practices.Prism.Regions;
using GreenField.Gadgets.Helpers;

namespace GreenField.DashboardModule.Views
{
    [Export]
    public partial class ViewDashboardCompanyChartingValuation : UserControl, INavigationAware
    {
        #region Fields
        private IEventAggregator _eventAggregator;
        private ILoggerFacade _logger;
        private IDBInteractivity _dBInteractivity;
        #endregion

        [ImportingConstructor]
        public ViewDashboardCompanyChartingValuation(ILoggerFacade logger, IEventAggregator eventAggregator,
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
                Header = new Telerik.Windows.Controls.HeaderedContentControl
                {
                    Content = GadgetNames.EXTERNAL_RESEARCH_HISTORICAL_VALUATION_CHART_PREVENUE,
                    Foreground = new SolidColorBrush(Colors.Black),
                    FontSize =
                        12,
                    FontFamily = new FontFamily("Arial")
                },
                RestoredHeight = 300,
                Content = new ViewPRevenue(new ViewModelPRevenue(param)) 
                
            });
            this.rtvDashboard.Items.Add(new RadTileViewItem
            {
                Header = new Telerik.Windows.Controls.HeaderedContentControl
                {
                    Content = GadgetNames.EXTERNAL_RESEARCH_HISTORICAL_VALUATION_CHART_EVEBITDA,
                    Foreground = new SolidColorBrush(Colors.Black),
                    FontSize =
                        12,
                    FontFamily = new FontFamily("Arial")
                },
                RestoredHeight = 300,
                Content = new ViewEVEBITDA(new ViewModelEVEBITDA(param)) 
                
            });
            this.rtvDashboard.Items.Add(new RadTileViewItem
            {
                Header = new Telerik.Windows.Controls.HeaderedContentControl
                {
                    Content = GadgetNames.EXTERNAL_RESEARCH_HISTORICAL_VALUATION_CHART_PCE,
                    Foreground = new SolidColorBrush(Colors.Black),
                    FontSize =
                        12,
                    FontFamily = new FontFamily("Arial")
                },
                RestoredHeight = 300,
                Content = new ViewPCE(new ViewModelPCE(param)) 
                
            });
            this.rtvDashboard.Items.Add(new RadTileViewItem
            {
                Header = new Telerik.Windows.Controls.HeaderedContentControl
                {
                    Content = GadgetNames.EXTERNAL_RESEARCH_HISTORICAL_VALUATION_CHART_PE,
                    Foreground = new SolidColorBrush(Colors.Black),
                    FontSize =
                        12,
                    FontFamily = new FontFamily("Arial")
                },
                RestoredHeight = 300,
                Content = new ViewPE(new ViewModelPE(param))
                
            });
            this.rtvDashboard.Items.Add(new RadTileViewItem
            {
                Header = new Telerik.Windows.Controls.HeaderedContentControl
                {
                    Content = GadgetNames.EXTERNAL_RESEARCH_HISTORICAL_VALUATION_CHART_PBV,
                    Foreground = new SolidColorBrush(Colors.Black),
                    FontSize =
                        12,
                    FontFamily = new FontFamily("Arial")
                },
                RestoredHeight = 300,
                Content = new ViewPBV(new ViewModelPBV(param))
                
            });
            this.rtvDashboard.Items.Add(new RadTileViewItem
            {
                Header = new Telerik.Windows.Controls.HeaderedContentControl
                {
                    Content = GadgetNames.EXTERNAL_RESEARCH_HISTORICAL_VALUATION_CHART_FCFYield,
                    Foreground = new SolidColorBrush(Colors.Black),
                    FontSize =
                        12,
                    FontFamily = new FontFamily("Arial")
                },
                RestoredHeight = 300,
                Content = new ViewFCFYield(new ViewModelFCFYield(param))
                
            });
            this.rtvDashboard.Items.Add(new RadTileViewItem
            {
                Header = new Telerik.Windows.Controls.HeaderedContentControl
                {
                    Content = GadgetNames.EXTERNAL_RESEARCH_HISTORICAL_VALUATION_CHART_DividendYield,
                    Foreground = new SolidColorBrush(Colors.Black),
                    FontSize =
                        12,
                    FontFamily = new FontFamily("Arial")
                },
                RestoredHeight = 300,
                Content = new ViewDividendYield(new ViewModelDividendYield(param)) 
                
            });
        }

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
