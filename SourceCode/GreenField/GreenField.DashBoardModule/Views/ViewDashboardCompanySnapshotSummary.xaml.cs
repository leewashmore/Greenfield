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

namespace GreenField.DashboardModule.Views
{
    [Export]
    public partial class ViewDashboardCompanySnapshotSummary : UserControl
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
                RestoredHeight = 400,
                Header = GadgetNames.SECURITY_OVERVIEW,
                Content = new ViewSecurityOverview(new ViewModelSecurityOverview(param))
            });

            this.rtvDashboard.Items.Add(new RadTileViewItem
            {
                RestoredHeight = 400,
                Header = GadgetNames.EXTERNAL_RESEARCH_PRICING,
                Content = null
            });

            this.rtvDashboard.Items.Add(new RadTileViewItem
            {
                RestoredHeight = 400,
                Header = GadgetNames.INTERNAL_RESEARCH_PRICING_DETAILED,
                Content = null
            });

            this.rtvDashboard.Items.Add(new RadTileViewItem
            {
                RestoredHeight = 400,
                Header = GadgetNames.EXTERNAL_RESEARCH_VALUATIONS,
                Content = null
            });

            this.rtvDashboard.Items.Add(new RadTileViewItem
            {
                RestoredHeight = 400,
                Header = GadgetNames.HOLDINGS_CHART_EXTENTION,
                Content = null
            });

            this.rtvDashboard.Items.Add(new RadTileViewItem
            {
                RestoredHeight = 400,
                Header = GadgetNames.EXTERNAL_RESEARCH_GROWTH,
                Content = null
            });

            this.rtvDashboard.Items.Add(new RadTileViewItem
            {
                RestoredHeight = 400,
                Header = GadgetNames.BENCHMARK_RELATIVE_PERFORMANCE,
                Content = null
            });

            this.rtvDashboard.Items.Add(new RadTileViewItem
            {
                RestoredHeight = 400,
                Header = GadgetNames.EXTERNAL_RESEARCH_MARGINS,
                Content = null
            });

            this.rtvDashboard.Items.Add(new RadTileViewItem
            {
                RestoredHeight = 400,
                Header = GadgetNames.EXTERNAL_RESEARCH_BASIC_DATA,
                Content = null
            });

            this.rtvDashboard.Items.Add(new RadTileViewItem
            {
                RestoredHeight = 400,
                Header = GadgetNames.EXTERNAL_RESEARCH_LEVERAGE_CAPITAL_FINANCIAL_STRENGTH,
                Content = null
            });

            this.rtvDashboard.Items.Add(new RadTileViewItem
            {
                RestoredHeight = 400,
                Header = GadgetNames.PORTFOLIO_CONSTRUCTION_FAIR_VALUE_COMPOSITION,
                Content = null
            });

            this.rtvDashboard.Items.Add(new RadTileViewItem
            {
                RestoredHeight = 400,
                Header = GadgetNames.EXTERNAL_RESEARCH_ASSET_QUALITY_CASH_FLOW,
                Content = null
            });

            this.rtvDashboard.Items.Add(new RadTileViewItem
            {
                RestoredHeight = 400,
                Header = GadgetNames.INTERNAL_RESEARCH_VALUATIONS_DETAILED,
                Content = null
            });

            this.rtvDashboard.Items.Add(new RadTileViewItem
            {
                RestoredHeight = 400,
                Header = GadgetNames.EXTERNAL_RESEARCH_PROFITABILITY,
                Content = null
            });
        }
        
    }
}
