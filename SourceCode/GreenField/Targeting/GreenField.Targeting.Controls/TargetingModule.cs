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
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Regions;
using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.Modularity;

namespace GreenField.Targeting.Controls
{
    [ModuleExport(typeof(TargetingModule))]
    public class TargetingModule : IModule
    {
        public const String MainRegionName = "MainRegion";
        private IRegionManager regionManager;

        [ImportingConstructor]
        public TargetingModule(IRegionManager regionManager)
        {
            this.regionManager = regionManager;
        }

        public void Initialize()
        {
            this.regionManager.RegisterViewWithRegion(MainRegionName, typeof(BasketTargets.RootView));
            this.regionManager.RegisterViewWithRegion(MainRegionName, typeof(BottomUp.RootView));
            this.regionManager.RegisterViewWithRegion(MainRegionName, typeof(BroadGlobalActive.RootView));
        }
    }
}
