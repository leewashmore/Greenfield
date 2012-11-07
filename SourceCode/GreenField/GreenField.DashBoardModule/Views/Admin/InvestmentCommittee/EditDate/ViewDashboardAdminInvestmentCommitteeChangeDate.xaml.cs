using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Regions;
using GreenField.Common;
using GreenField.Common.Helper;
using GreenField.Gadgets.Helpers;
using GreenField.Gadgets.ViewModels;
using GreenField.Gadgets.Views;
using GreenField.ServiceCaller;


namespace GreenField.DashboardModule.Views
{
    /// <summary>
    /// Code behind for ViewDashboardAdminInvestmentCommitteeChangeDate
    /// </summary>
    [Export]
    public partial class ViewDashboardAdminInvestmentCommitteeChangeDate : ViewBaseUserControl, INavigationAware
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
        /// <summary>
        /// Region Manager
        /// </summary>
        private IRegionManager regionManager;
        private ViewPresentations view;
        private ViewModelPresentations viewModel;
        private ViewModelICPresentationNew viewModelNew;
        private ViewICPresentationNew viewNew;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger1">ILoggerFacade</param>
        /// <param name="eventAggregator1">IEventAggregator</param>
        /// <param name="dbInteractivity1">IDBInteractivity</param>
        /// <param name="regionManager1">IRegionManager</param>
        [ImportingConstructor]
        public ViewDashboardAdminInvestmentCommitteeChangeDate(ILoggerFacade logger1, IEventAggregator eventAggregator1,
            IDBInteractivity dbInteractivity1, IRegionManager regionManager1)
        {
            InitializeComponent();
            eventAggregator = eventAggregator1;
            logger = logger1;
            dBInteractivity = dbInteractivity1;
            regionManager = regionManager1;
            eventAggregator.GetEvent<DashboardGadgetLoad>().Subscribe(HandleDashboardGadgetLoad);
            this.tbHeader.Text = GadgetNames.ADMIN_CHANGE_DATE;
        }
        #endregion

        #region Event Handler
        /// <summary>
        /// DashboardGadgetLoad Event Handler
        /// </summary>
        /// <param name="payload">DashboardGadgetPayload</param>
        public void HandleDashboardGadgetLoad(DashboardGadgetPayload payload)
        {
            if (this.cctrDashboardContent.Content != null)
            {
                return;
            }
            DashboardGadgetParam param = new DashboardGadgetParam()
            {
                DashboardGadgetPayload = payload,
                DBInteractivity = dBInteractivity,
                EventAggregator = eventAggregator,
                LoggerFacade = logger,
                RegionManager = regionManager
            };          
            this.cctrDashboardContent.Content = new ViewMeetingConfigurationSchedule(new ViewModelMeetingConfigSchedule(param));
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
            ViewBaseUserControl control = (ViewBaseUserControl)cctrDashboardContent.Content;
            if (control != null)
            {
                control.IsActive = false;
            }
        }

        /// <summary>
        /// Executed on navigation to this view
        /// </summary>
        /// <param name="navigationContext">NavigationContext</param>
        public void OnNavigatedTo(NavigationContext navigationContext)
        {         
            ViewBaseUserControl control = (ViewBaseUserControl)cctrDashboardContent.Content;
            if (control != null)
            {
                control.IsActive = true;
            }
        }
        #endregion
    }
}
