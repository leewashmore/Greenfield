using System;
using System.Windows;
using Microsoft.Practices.Prism.MefExtensions;
using Microsoft.Practices.Prism.Regions;
using System.ComponentModel.Composition.Hosting;
using GreenField.ServiceCaller;
using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Modularity;


namespace GreenField.Login.App
{
    public class Bootstrapper : MefBootstrapper
    {
        [Import]
        public ILogger _logger { get; set; }

        protected override void ConfigureAggregateCatalog()
        {
            base.ConfigureAggregateCatalog();

            this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(Bootstrapper).Assembly));
            this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(GreenField.LoginModule.LoginModule).Assembly));            
            this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(GreenField.ServiceCaller.ManageLogins).Assembly));
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
            Application.Current.RootVisual = (UIElement)this.Shell;
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();
            this.Container.ComposeExportedValue<ILoggerFacade>(this._logger);
        }
    }
}
