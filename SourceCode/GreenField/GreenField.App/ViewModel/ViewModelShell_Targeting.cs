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
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using GreenField.Common;
using TargetingModule = GreenField.Targeting.Controls;
namespace GreenField.App.ViewModel
{
    // this piece of the ViewModelShell class is related solely to targeting
    public partial class ViewModelShell
    {
        // navigation commands that bound to the main menu and lead to targeting screens

        public DelegateCommand TargetingBroadGlobalActiveCommand { get; private set; }
        public DelegateCommand TargetingBasketTargetsCommand { get; set; }
        public DelegateCommand TargetingBottomUpCommand { get; set; }

        // this method is called from the ViewModelSheel constructor
        private void InitializeTargetingCommands()
        {
            this.TargetingBroadGlobalActiveCommand = new DelegateCommand(this.NavigateToTargetingBroadGlobalActiveCommand);
            this.TargetingBasketTargetsCommand = new DelegateCommand(this.NavigateToTargetingBasketTargetsCommand);
            this.TargetingBottomUpCommand = new DelegateCommand(this.NavigateToTargetingBottomUpCommand);
        }

        // navigate to targeting views respectively

        public void NavigateToTargetingBroadGlobalActiveCommand()
        {
            this.regionManager.RequestNavigate(RegionNames.MAIN_REGION, typeof(TargetingModule.BroadGlobalActive.RootView).FullName);
        }

        public void NavigateToTargetingBasketTargetsCommand()
        {
            this.regionManager.RequestNavigate(RegionNames.MAIN_REGION, typeof(TargetingModule.BasketTargets.RootView).FullName);
        }

        public void NavigateToTargetingBottomUpCommand()
        {
            this.regionManager.RequestNavigate(RegionNames.MAIN_REGION, typeof(TargetingModule.BottomUp.RootView).FullName);
        }
    }
}
