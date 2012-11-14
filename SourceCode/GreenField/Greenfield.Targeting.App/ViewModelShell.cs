using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Practices.Prism.ViewModel;
using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Regions;
using GreenField.ServiceCaller;
using Microsoft.Practices.Prism.Events;
using GreenField.DashboardModule.Views;
using GreenField.Common;
using GreenField.Common.Helper;

namespace GreenField.Targeting.App
{
    [Export]
    public class ViewModelShell : NotificationObject
    {
        private IRegionManager regionManager;
        private IEventAggregator eventAggregator;
        
        [ImportingConstructor]
        public ViewModelShell(
            IRegionManager regionManager,
            IManageSessions manageSessions,
            ILoggerFacade logger,
            IEventAggregator eventAggregator,
            IDBInteractivity dbInteractivity
        )
        {
            this.eventAggregator = eventAggregator;
            this.regionManager = regionManager;
        }

        public void Run()
        {
            var payload = new DashboardGadgetPayload();
            this.regionManager.RequestNavigate("MainRegion", new Uri("ViewDashboardTargetingBroadGlobalActive", UriKind.Relative));
            this.eventAggregator.GetEvent<DashboardGadgetLoad>().Publish(payload);
        }
    }
}
