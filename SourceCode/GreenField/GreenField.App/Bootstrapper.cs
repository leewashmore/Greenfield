using System;
using System.Windows;
using Microsoft.Practices.Prism.MefExtensions;
using Microsoft.Practices.Prism.Regions;
using System.ComponentModel.Composition.Hosting;
using GreenField.ServiceCaller;
using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Modularity;
using TargetingModule = GreenField.Targeting.Controls;
using System.Windows.Threading;

namespace GreenField.App
{
    public class Bootstrapper : MefBootstrapper
    {
        [Import]
        public ILogger Logger { get; set; }

        protected override void ConfigureAggregateCatalog()
        {
            base.ConfigureAggregateCatalog();

            this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(Bootstrapper).Assembly));
            this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(GreenField.AdministrationModule.AdministrationModule).Assembly));
            this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(GreenField.Targeting.Controls.TargetingModule).Assembly));
            this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(GreenField.IssuerShares.Controls.Module).Assembly));
            this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(GreenField.ServiceCaller.DBInteractivity).Assembly));
            this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(GreenField.DashboardModule.DashboardModule).Assembly));
        }

        protected override IRegionBehaviorFactory ConfigureDefaultRegionBehaviors()
        {
            var factory = base.ConfigureDefaultRegionBehaviors(); 
            return factory;
        }

        protected override DependencyObject CreateShell()
        {
            return this.Container.GetExportedValue<Shell>();
        }

        protected override void InitializeShell()
        {
            base.InitializeShell();
            var shell = (Shell)this.Shell;
            Application.Current.RootVisual = (UIElement)this.Shell;
            
            this.Container.ComposeExportedValue<Dispatcher>(shell.Dispatcher);

        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();
            this.Container.ComposeExportedValue<ILoggerFacade>(this.Logger);
            this.InitializeEntitiesForTargetingModule();
            this.InitializeEntitiesForIssuerSharesModule();
        }

        private void InitializeEntitiesForIssuerSharesModule()
        {
            var clientFactory = new GreenField.IssuerShares.Controls.DefaultClientFactory();
            this.Container.ComposeExportedValue<GreenField.IssuerShares.Controls.IClientFactory>(clientFactory);
        }

        private void InitializeEntitiesForTargetingModule()
        {
            var settings = this.CreateTargetingModuleSettings();
            this.Container.ComposeExportedValue<TargetingModule.BroadGlobalActive.Settings>(settings.BgaSettings);
            this.Container.ComposeExportedValue<TargetingModule.IClientFactory>(settings.ClientFactory);
            this.Container.ComposeExportedValue<TargetingModule.BottomUp.Settings>(settings.BuSettings);
            this.Container.ComposeExportedValue<TargetingModule.BasketTargets.Settings>(settings.BtSettings);
            
        }

        private TargetingModule.GlobalSettings CreateTargetingModuleSettings()
        {
            var clientFactory = new TargetingModule.DefaultClientFactory();
            var modelTraverser = new TargetingModule.BroadGlobalActive.ModelTraverser();

            var bgaSettings = new TargetingModule.BroadGlobalActive.Settings(
                clientFactory,
                modelTraverser,
                new TargetingModule.BroadGlobalActive.DefaultExpandCollapseStateSetter(modelTraverser)
            );

            var buSettings = new TargetingModule.BottomUp.Settings(clientFactory);
            var btSettings = new TargetingModule.BasketTargets.Settings(clientFactory);

            var settings = new TargetingModule.GlobalSettings(
                clientFactory,
                bgaSettings,
                buSettings,
                btSettings
            );
            return settings;
        }

    }
}
