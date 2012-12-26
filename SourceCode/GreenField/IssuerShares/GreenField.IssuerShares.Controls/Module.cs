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
using Microsoft.Practices.Prism.Modularity;
using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.MefExtensions.Modularity;

namespace GreenField.IssuerShares.Controls
{
    [ModuleExport(typeof(Module))]
    public class Module : IModule
    {
        public const String MainRegion = "MainRegion";

        private IRegionManager regionManager;

        [ImportingConstructor]
        public Module(IRegionManager regionManager)
        {
            this.regionManager = regionManager;
        }

        public void Initialize()
        {
            this.regionManager.RegisterViewWithRegion(MainRegion, typeof(InitialView));
            this.regionManager.RegisterViewWithRegion(MainRegion, typeof(RootView));
        }
    }
}
