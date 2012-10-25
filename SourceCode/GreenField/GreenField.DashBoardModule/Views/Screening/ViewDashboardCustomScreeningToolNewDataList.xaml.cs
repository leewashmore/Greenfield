using System.ComponentModel.Composition;
using System.Windows.Controls;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Regions;
using GreenField.Common;
using GreenField.Common.Helper;
using GreenField.Gadgets.Helpers;
using GreenField.Gadgets.ViewModels;
using GreenField.Gadgets.Views;
using GreenField.ServiceCaller;

namespace GreenField.DashBoardModule.Views.Screening
{
    [Export]
    public partial class ViewDashboardCustomScreeningToolNewDataList : UserControl, INavigationAware
    {
        #region Fields
        private IEventAggregator _eventAggregator;
        private ILoggerFacade _logger;
        private IDBInteractivity _dBInteractivity;
        private IRegionManager _regionManager;
        #endregion

        [ImportingConstructor]
        public ViewDashboardCustomScreeningToolNewDataList(ILoggerFacade logger, IEventAggregator eventAggregator,
            IDBInteractivity dbInteractivity, IRegionManager regionManager)
        {
            InitializeComponent();
            _eventAggregator = eventAggregator;
            _logger = logger;
            _dBInteractivity = dbInteractivity;
            _regionManager = regionManager;
            _eventAggregator.GetEvent<DashboardGadgetLoad>().Subscribe(HandleDashboardGadgetLoad);

            this.tbHeader.Text = GadgetNames.CUSTOM_SCREENING_TOOL;

        }

        public void HandleDashboardGadgetLoad(DashboardGadgetPayload payload)
        {
            if (this.cctrDashboardContent.Content != null)
                return;

            DashboardGadgetParam param = new DashboardGadgetParam()
            {
                DashboardGadgetPayload = payload,
                DBInteractivity = _dBInteractivity,
                EventAggregator = _eventAggregator,
                LoggerFacade = _logger,
                RegionManager = _regionManager
            };

            this.cctrDashboardContent.Content = new ViewCSTDataFieldSelector(new ViewModelCSTDataFieldSelector(param));

        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            ViewBaseUserControl control = (ViewBaseUserControl)cctrDashboardContent.Content;
            if (control != null)
            {
                control.IsActive = false;
            }
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            ViewBaseUserControl control = (ViewBaseUserControl)cctrDashboardContent.Content;
            if (control != null)
            {
                control.IsActive = true;
            }
        }
    }
}
