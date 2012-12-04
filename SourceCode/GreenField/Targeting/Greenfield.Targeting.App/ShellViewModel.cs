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
using GreenField.Targeting.Controls;
using Microsoft.Practices.Prism.Commands;

namespace GreenField.Targeting.App
{
    [Export]
    public class ShellViewModel : NotificationObject
    {
        public const String MainRegionName = "MainRegion";

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

            this.InitializeCommands();
        }

        private void InitializeCommands()
        {
            this.NavigateToTargetingBroadGlobalActiveCommand = new DelegateCommand(delegate
            {
				this.regionManager.RequestNavigate(MainRegionName, typeof(Controls.BroadGlobalActive.RootView).FullName);
            });

            this.NavigateToTargetingBottomUpCommand = new DelegateCommand(delegate
            {
				this.regionManager.RequestNavigate(MainRegionName, typeof(Controls.BottomUp.RootView).FullName);
            });

            this.NavigateToTargetingBasketTargetsCommand = new DelegateCommand(delegate
            {
				this.regionManager.RequestNavigate(MainRegionName, typeof(Controls.BasketTargets.RootView).FullName);
            });
        }

        // Navigation commands
        public ICommand NavigateToTargetingBroadGlobalActiveCommand { get; private set; }
        public ICommand NavigateToTargetingBottomUpCommand { get; private set; }
        public ICommand NavigateToTargetingBasketTargetsCommand { get; private set; }

        /// <summary>
        /// Begins execution of the application.
        /// </summary>
        public void Start()
        {
            //this.NavigateToTargetingBottomUpCommand.Execute(new Object());
            this.NavigateToTargetingBasketTargetsCommand.Execute(new Object());
			//this.NavigateToTargetingBroadGlobalActiveCommand.Execute(new Object());
            //this.regionManager.RequestNavigate(MainRegionName, typeof(Controls.SecurityPickerDemoView).FullName);
            //this.regionManager.RequestNavigate(MainRegionName, typeof(Controls.CommentDemoView).FullName);
            //this.regionManager.RequestNavigate(MainRegionName, typeof(WatermarkLabs).FullName);
        }
    }
}
