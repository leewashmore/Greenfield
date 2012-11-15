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
using GreenField.DashBoardModule.ViewModels.Targeting;
using GreenField.Gadgets.ViewModels.Targeting.BroadGlobalActive;

namespace GreenField.DashboardModule.Views
{
    /// <summary>
    /// Panel (title + content placeholder).
    /// </summary>
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
            IDBInteractivity dbInteractivity
        )
        {
            InitializeComponent();

            this.eventAgregator = eventAggregator;
            this.logger = logger;
            this.dbInteractivity = dbInteractivity;

            this.eventAgregator.GetEvent<DashboardGadgetLoad>().Subscribe(HandleDashboardGadgetLoad);
        }

        public void HandleDashboardGadgetLoad(DashboardGadgetPayload payload)
        {
            DashboardGadgetParam param = new DashboardGadgetParam()
            {
                DashboardGadgetPayload = payload,
                DBInteractivity = this.dbInteractivity,
                EventAggregator = this.eventAgregator,
                LoggerFacade = this.logger
            };

            var editorViewModel = new EditorViewModel(param);
            var pickerViewModel = new PickerViewModel(param.DBInteractivity);
            var rootModel = new RootViewModel(
                pickerViewModel,
                editorViewModel
            );
            
            this.DataContext = rootModel;
        }

        public Boolean IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            /*ViewBaseUserControl control = (ViewBaseUserControl)this.placeholder.Content;
            if (control != null)
            {
                control.IsActive = false;
            }*/
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            /*
            ViewBaseUserControl control = (ViewBaseUserControl)this.placeholder.Content;
            if (control != null)
            {
                control.IsActive = true;
            }*/
        }
    }
}
