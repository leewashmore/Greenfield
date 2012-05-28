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
    public partial class ViewDashboardCompanyEstimatesConsensus : UserControl
    {
        #region Fields
        private IEventAggregator _eventAggregator;
        private ILoggerFacade _logger;
        private IDBInteractivity _dBInteractivity;
        #endregion

        [ImportingConstructor]
        public ViewDashboardCompanyEstimatesConsensus(ILoggerFacade logger, IEventAggregator eventAggregator,
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
                RestoredHeight = 50,
                Header = GadgetNames.EXTERNAL_RESEARCH_CONSENSUS_OVERVIEW,
                Content = null
            });

            this.rtvDashboard.Items.Add(new RadTileViewItem
            {
                RestoredHeight = 50,
                Header = GadgetNames.EXTERNAL_RESEARCH_CONSENSUS_RECOMMENDATION,
                Content = null
            });

            this.rtvDashboard.Items.Add(new RadTileViewItem
            {
                RestoredHeight = 50,
                Header = GadgetNames.EXTERNAL_RESEARCH_CONSENSUS_TARGET_PRICE,
                Content = null
            });

            this.rtvDashboard.Items.Add(new RadTileViewItem
            {
                RestoredHeight = 50,
                Header = GadgetNames.EXTERNAL_RESEARCH_CONSENSUS_MEDIAN_ESTIMATES,
                Content = null
            });

            this.rtvDashboard.Items.Add(new RadTileViewItem
            {
                RestoredHeight = 50,
                Header = GadgetNames.EXTERNAL_RESEARCH_CONSENSUS_VALUATIONS,
                Content = null
            });
        }
    }
}
