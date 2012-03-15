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
using GreenField.DashBoardModule.Views;
using GreenField.Common;
using Microsoft.Practices.Prism.Modularity;

namespace GreenField.DashBoardModule
{
    [ModuleExport(typeof(DashBoardModule))]
    public class DashBoardModule : IModule
    {
        IRegionManager _regionManager;

        [ImportingConstructor]
        public DashBoardModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void Initialize()
        {
            _regionManager.RegisterViewWithRegion(RegionNames.MAIN_REGION, typeof(ViewDashBoard));            
        }
    }
}
