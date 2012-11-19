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
using Microsoft.Practices.Prism.MefExtensions;
using System.ComponentModel.Composition.Hosting;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.Logging;
using GreenField.Targeting.Only;

namespace GreenField.Targeting.App
{
    public class Bootstrapper : MefBootstrapper
    {
        protected override void ConfigureAggregateCatalog()
        {
            base.ConfigureAggregateCatalog();

            this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(Bootstrapper).Assembly));
            this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(GreenField.Targeting.Only.GlobalSettings).Assembly));
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

            var targetingSettings = this.CreateTargetingSettings();
            shell.DataContextSource.Run(targetingSettings);
        }

        private Targeting.Only.GlobalSettings CreateTargetingSettings()
        {
            var benchmarkDate = new DateTime(2012, 10, 17);
            var clientFactory = new DefaultClientFactory();
            var modelTraverser = new Only.BroadGlobalActive.ModelTraverser();
            
            var bgaSettings = new Targeting.Only.BroadGlobalActive.Settings(
                clientFactory,
                modelTraverser,
                new Only.BroadGlobalActive.DefaultExpandCollapseStateSetter(modelTraverser),
                benchmarkDate
            );

            var settings = new Targeting.Only.GlobalSettings(bgaSettings);
            return settings;
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();
            this.Container.ComposeExportedValue<ILoggerFacade>(this.Logger);
        }

        
    }
}
