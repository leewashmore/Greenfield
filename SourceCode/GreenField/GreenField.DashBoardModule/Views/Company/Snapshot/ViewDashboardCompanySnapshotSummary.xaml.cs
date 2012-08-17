using System;
using System.Collections.Generic;
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
    public partial class ViewDashboardCompanySnapshotSummary : UserControl, INavigationAware
    {
        #region Fields
        private IEventAggregator _eventAggregator;
        private ILoggerFacade _logger;
        private IDBInteractivity _dBInteractivity;
        #endregion

        [ImportingConstructor]
        public ViewDashboardCompanySnapshotSummary(ILoggerFacade logger, IEventAggregator eventAggregator,
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
                Header = new Telerik.Windows.Controls.HeaderedContentControl { Content = GadgetNames.SECURITY_OVERVIEW, Foreground = new SolidColorBrush(Colors.Black), FontSize = 12, FontFamily = new FontFamily("Arial") },
                Content = new ViewSecurityOverview(new ViewModelSecurityOverview(param))
            });

            this.rtvDashboard.Items.Add(new RadTileViewItem
            {
                Header = new Telerik.Windows.Controls.HeaderedContentControl { Content = GadgetNames.EXTERNAL_RESEARCH_PRICING, Foreground = new SolidColorBrush(Colors.Black), FontSize = 12, FontFamily = new FontFamily("Arial") },
                Content = null
            });

            this.rtvDashboard.Items.Add(new RadTileViewItem
            {
                Header = new Telerik.Windows.Controls.HeaderedContentControl { Content = GadgetNames.INTERNAL_RESEARCH_PRICING_DETAILED, Foreground = new SolidColorBrush(Colors.Black), FontSize = 12, FontFamily = new FontFamily("Arial") },
                Content = null
            });

            this.rtvDashboard.Items.Add(new RadTileViewItem
            {

                Header = new Telerik.Windows.Controls.HeaderedContentControl { Content = GadgetNames.EXTERNAL_RESEARCH_VALUATIONS, Foreground = new SolidColorBrush(Colors.Black), FontSize = 12, FontFamily = new FontFamily("Arial") },
                Content = null
            });

            this.rtvDashboard.Items.Add(new RadTileViewItem
            {

                Header = new Telerik.Windows.Controls.HeaderedContentControl
                {
                    Content = GadgetNames.HOLDINGS_CHART_EXTENTION,
                    Foreground = new SolidColorBrush(Colors.Black),
                    FontSize =
                        12,
                    FontFamily = new FontFamily("Arial")
                },
                RestoredHeight = 300,
                Content = new ViewSlice1ChartExtension(new ViewModelSlice1ChartExtension(param))
            });

            this.rtvDashboard.Items.Add(new RadTileViewItem
            {

                Header = new Telerik.Windows.Controls.HeaderedContentControl { Content = GadgetNames.EXTERNAL_RESEARCH_GROWTH, 
                    Foreground = new SolidColorBrush(Colors.Black), FontSize = 12, FontFamily = new FontFamily("Arial") },
                Content = null
            });

            this.rtvDashboard.Items.Add(new RadTileViewItem
            {

                Header = new Telerik.Windows.Controls.HeaderedContentControl { Content = GadgetNames.BENCHMARK_RELATIVE_PERFORMANCE, 
                    Foreground = new SolidColorBrush(Colors.Black), FontSize = 12, FontFamily = new FontFamily("Arial") },
                RestoredHeight = 200,
                Content = new ViewRelativePerformanceUI(new ViewModelRelativePeroformanceUI(param))
            });

            this.rtvDashboard.Items.Add(new RadTileViewItem
            {

                Header = new Telerik.Windows.Controls.HeaderedContentControl { Content = GadgetNames.EXTERNAL_RESEARCH_MARGINS, Foreground = new SolidColorBrush(Colors.Black), FontSize = 12, FontFamily = new FontFamily("Arial") },
                Content = null
            });

            this.rtvDashboard.Items.Add(new RadTileViewItem
            {

                Header = new Telerik.Windows.Controls.HeaderedContentControl { Content = GadgetNames.EXTERNAL_RESEARCH_BASIC_DATA,
                    Foreground = new SolidColorBrush(Colors.Black), FontSize = 12, FontFamily = new FontFamily("Arial") },
                RestoredHeight = 300,
                Content = null
                //new ViewBasicData(new ViewModelBasicData(param))
            });

            this.rtvDashboard.Items.Add(new RadTileViewItem
            {

                Header = new Telerik.Windows.Controls.HeaderedContentControl { Content = GadgetNames.EXTERNAL_RESEARCH_LEVERAGE_CAPITAL_FINANCIAL_STRENGTH, 
                    Foreground = new SolidColorBrush(Colors.Black), FontSize = 12, FontFamily = new FontFamily("Arial") },
                Content = null
            });

            this.rtvDashboard.Items.Add(new RadTileViewItem
            {

                Header = new Telerik.Windows.Controls.HeaderedContentControl { Content = GadgetNames.PORTFOLIO_CONSTRUCTION_FAIR_VALUE_COMPOSITION_SUMMARY,
                    Foreground = new SolidColorBrush(Colors.Black), FontSize = 12, FontFamily = new FontFamily("Arial") },
                Content = null
            });

            this.rtvDashboard.Items.Add(new RadTileViewItem
            {

                Header = new Telerik.Windows.Controls.HeaderedContentControl { Content = GadgetNames.EXTERNAL_RESEARCH_ASSET_QUALITY_CASH_FLOW,
                    Foreground = new SolidColorBrush(Colors.Black), FontSize = 12, FontFamily = new FontFamily("Arial") },
                Content = null
            });

            this.rtvDashboard.Items.Add(new RadTileViewItem
            {

                Header = new Telerik.Windows.Controls.HeaderedContentControl { Content = GadgetNames.INTERNAL_RESEARCH_VALUATIONS_DETAILED,
                    Foreground = new SolidColorBrush(Colors.Black), FontSize = 12, FontFamily = new FontFamily("Arial") },
                Content = null
            });

            this.rtvDashboard.Items.Add(new RadTileViewItem
            {

                Header = new Telerik.Windows.Controls.HeaderedContentControl { Content = GadgetNames.EXTERNAL_RESEARCH_PROFITABILITY,
                    Foreground = new SolidColorBrush(Colors.Black), FontSize = 12, FontFamily = new FontFamily("Arial") },
                Content = null
            });
            this.rtvDashboard.Items.Add(new RadTileViewItem
            {

                Header = new Telerik.Windows.Controls.HeaderedContentControl { Content = GadgetNames.INTERNAL_RESEARCH_CONSESUS_ESTIMATE_SUMMARY,
                    Foreground = new SolidColorBrush(Colors.Black), FontSize = 12, FontFamily = new FontFamily("Arial") },
                RestoredHeight = 220,
                Content = new ViewConsensusEstimateSummary(new ViewModelConsensusEstimateSummary(param))
            });

            this.rtvDashboard.Items.Add(new RadTileViewItem
            {

                Header = new Telerik.Windows.Controls.HeaderedContentControl { Content = GadgetNames.GADGET_WITH_PERIOD_COLUMNS_COA_SPECIFIC,
                    Foreground = new SolidColorBrush(Colors.Black), FontSize = 12, FontFamily = new FontFamily("Arial") },
                RestoredHeight = 300,
                Content =  new ViewCOASpecific(new ViewModelCOASpecific(param))
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
            int a = rtvDashboard.Items.Count;
            foreach (RadTileViewItem item in this.rtvDashboard.Items)
            {
                ViewBaseUserControl control = (ViewBaseUserControl)item.Content;
                if (control != null)
                    control.IsActive = value;
            }

        }
    }
}
