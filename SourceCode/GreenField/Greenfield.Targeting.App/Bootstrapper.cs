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
using System.ComponentModel.Composition;
using GreenField.ServiceCaller;
using Microsoft.Practices.Prism.MefExtensions;
using System.ComponentModel.Composition.Hosting;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.Logging;

namespace GreenField.Targeting.App
{
    public class Bootstrapper : MefBootstrapper
    {
        [Import]
        public new ILogger Logger { get; set; }

        protected override void ConfigureAggregateCatalog()
        {
            base.ConfigureAggregateCatalog();

            this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(Bootstrapper).Assembly));
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

        // 1. Create a shell
        protected override void InitializeShell()
        {
            base.InitializeShell();
            var shell = (Shell) this.Shell;
            Application.Current.RootVisual = shell;
        }

        // 2. Run
        protected override void InitializeModules()
        {
            base.InitializeModules();
            var shell = (Shell)this.Shell;
            shell.DataContextSource.Run();
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();
            this.Container.ComposeExportedValue<ILoggerFacade>(this.Logger);
        }

        
    }
}
