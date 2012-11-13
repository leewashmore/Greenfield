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

namespace GreenField.DashboardModule.Views
{
    [Export]
    public partial class ViewDashboardTargetingBroadGlobalActive : UserControl, INavigationAware
    {
        private IEventAggregator eventAgregator;
        private ILoggerFacade logger;
        private IDBInteractivity dbInteractivity;

        [ImportingConstructor]
        public ViewDashboardTargetingBroadGlobalActive(
            ILoggerFacade logger,
            IEventAggregator eventAggregator,
            IDBInteractivity dbInteractivity)
        {
            InitializeComponent();

            this.eventAgregator = eventAggregator;
            this.logger = logger;
            this.dbInteractivity = dbInteractivity;

            this.eventAgregator.GetEvent<DashboardGadgetLoad>().Subscribe(HandleDashboardGadgetLoad);
            //this.tbHeader.Text = GadgetNames.MODELS_FX_MACRO_ECONOMICS_INTERNAL_MODELS_EVALUATION_REPORT;
        }

        public void HandleDashboardGadgetLoad(DashboardGadgetPayload payload)
        {
            if (this.cctrDashboardContent.Content != null)
                return;

            DashboardGadgetParam param = new DashboardGadgetParam()
            {
                DashboardGadgetPayload = payload,
                DBInteractivity = this.dbInteractivity,
                EventAggregator = this.eventAgregator,
                LoggerFacade = this.logger
            };

            var viewModel = new ViewModelTargetingBroadGlobalActive(param);
            var view = new ViewTargetingBroadGlobalActive();
            this.cctrDashboardContent.Content = view;
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
