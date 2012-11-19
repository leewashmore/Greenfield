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
using Microsoft.Practices.Prism.Events;
using GreenField.Targeting.Only;

namespace GreenField.Targeting.App
{
    [Export]
    public class ShellViewModel : NotificationObject
    {
        private IRegionManager regionManager;
        private IEventAggregator eventAggregator;
        private ILoggerFacade logger;

        [ImportingConstructor]
        public ShellViewModel(
            IRegionManager regionManager,
            ILoggerFacade logger,
            IEventAggregator eventAggregator
        )
        {
            this.eventAggregator = eventAggregator;
            this.regionManager = regionManager;
            this.logger = logger;
        }

        public void Run(GlobalSettings settings)
        {
            this.regionManager.RequestNavigate("MainRegion", new Uri("ViewDashboardTargetingBroadGlobalActive", UriKind.Relative));
            this.eventAggregator.GetEvent<Only.BroadGlobalActive.RunEvent>().Publish(settings.BgaSettings);
        }
    }
}
