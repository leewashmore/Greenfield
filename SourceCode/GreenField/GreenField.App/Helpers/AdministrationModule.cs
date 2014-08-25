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
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using System.ComponentModel.Composition;
using GreenField.AdministrationModule.Views;
using GreenField.Common;

namespace GreenField.AdministrationModule
{
    [ModuleExport(typeof(AdministrationModule))]
    public class AdministrationModule : IModule
    {
        IRegionManager _regionManager;

        [ImportingConstructor]
        public AdministrationModule(IRegionManager regionManager)
        {
            try
            {
                _regionManager = regionManager;
            }
            catch (Exception) { }
        }

        public void Initialize()
        {
            try
            {
                _regionManager.RegisterViewWithRegion(RegionNames.MAIN_REGION, typeof(Home));
                _regionManager.RegisterViewWithRegion(RegionNames.MAIN_REGION, typeof(ViewManageUsers));
                _regionManager.RegisterViewWithRegion(RegionNames.MAIN_REGION, typeof(ViewManageRoles));
            }
            catch (Exception) { }
        }
    }

}
