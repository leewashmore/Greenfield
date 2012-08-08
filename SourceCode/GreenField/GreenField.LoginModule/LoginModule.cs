
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using System.ComponentModel.Composition;
using GreenField.Common;
using GreenField.LoginModule.Views;
using System;

namespace GreenField.LoginModule
{
    [ModuleExport(typeof(LoginModule))]
    public class LoginModule : IModule
    {
        private IRegionManager _regionManager;

        [ImportingConstructor]
        public LoginModule(IRegionManager regionManager)
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
                _regionManager.RegisterViewWithRegion(RegionNames.MAIN_REGION, typeof(ViewLoginForm));
                _regionManager.RegisterViewWithRegion(RegionNames.MAIN_REGION, typeof(ViewRegisterForm));
                _regionManager.RegisterViewWithRegion(RegionNames.MAIN_REGION, typeof(ViewPasswordChangeForm));
                _regionManager.RegisterViewWithRegion(RegionNames.MAIN_REGION, typeof(ViewPasswordResetForm));
                _regionManager.RegisterViewWithRegion(RegionNames.MAIN_REGION, typeof(ViewNotifications));
            }
            catch (Exception) { }
        }
    }
}
