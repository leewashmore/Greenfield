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
using GreenField.Gadgets.Views;
using GreenField.Gadgets.ViewModels;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Logging;
using GreenField.ServiceCaller;
using GreenField.Common;
using GreenField.Common.Helper;
using Microsoft.Practices.Prism.Regions;
using GreenField.Gadgets.Helpers;
using GreenField.DataContracts;

namespace GreenField.DashboardModule.Views
{
    [Export]
    public partial class ViewDashboardInvestmentCommitteeMeetingMinutes : ViewBaseUserControl, INavigationAware
    {
        #region Fields
        private IEventAggregator _eventAggregator;
        private ILoggerFacade _logger;
        private IDBInteractivity _dBInteractivity;
        private IRegionManager _regionManager;
        private ViewModelPresentationMeetingMinutes _viewModel;
        private ViewPresentationMeetingMinutes _view;
        #endregion

        [ImportingConstructor]
        public ViewDashboardInvestmentCommitteeMeetingMinutes(ILoggerFacade logger, IEventAggregator eventAggregator,
            IDBInteractivity dbInteractivity, IRegionManager regionManager)
        {
            InitializeComponent();

            _eventAggregator = eventAggregator;
            _logger = logger;
            _dBInteractivity = dbInteractivity;
            _regionManager = regionManager;

            _eventAggregator.GetEvent<DashboardGadgetLoad>().Subscribe(HandleDashboardGadgetLoad);
            this.tbHeader.Text = GadgetNames.ICPRESENTATION_MEETING_MINUTES;
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

            //_viewModel = new ViewModelPresentationMeetingMinutes(param);
            //_view = new ViewPresentationMeetingMinutes(_viewModel);
            this.cctrDashboardContent.Content = null;// _view;
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
                _viewModel.Initialize();
            }
        }
    }
}
