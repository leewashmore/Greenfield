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
using GreenField.Targeting.Controls;

namespace GreenField.Targeting.App
{
    public class Bootstrapper : MefBootstrapper
    {
        protected override void ConfigureAggregateCatalog()
        {
            base.ConfigureAggregateCatalog();

            this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(Bootstrapper).Assembly));
			this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(Controls.GlobalSettings).Assembly));
            
            // 
            
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
            shell.DataContextSource.Start();
        }

        // 3. Add known instance to the container so that they can be resolved later.
        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();
            // adding instances to the container
            // these very instances are going to be used to resolve interfaces and other shit
            // this is a logger
            this.Container.ComposeExportedValue<ILoggerFacade>(this.Logger);

            // this is the settings object that is used to initialize the targeting viewmodels
            var settings = this.CreateTargetingSettings();
			this.Container.ComposeExportedValue<Controls.BroadGlobalActive.Settings>(settings.BgaSettings);
            this.Container.ComposeExportedValue<Controls.IClientFactory>(settings.ClientFactory);
            this.Container.ComposeExportedValue<Controls.BottomUp.Settings>(settings.BuSettings);
            this.Container.ComposeExportedValue<Controls.BasketTargets.Settings>(settings.BtSettings);
        }

        private Controls.GlobalSettings CreateTargetingSettings()
        {
            var benchmarkDate = new DateTime(2012, 10, 17);
            var clientFactory = new DefaultClientFactory();
			var modelTraverser = new Controls.BroadGlobalActive.ModelTraverser();

			var bgaSettings = new Controls.BroadGlobalActive.Settings(
                clientFactory,
                modelTraverser,
				new Controls.BroadGlobalActive.DefaultExpandCollapseStateSetter(modelTraverser),
                benchmarkDate
            );

            var buSettings = new Controls.BottomUp.Settings(clientFactory);
            var btSettings = new Controls.BasketTargets.Settings(clientFactory, benchmarkDate);

			var settings = new Controls.GlobalSettings(
                clientFactory,
                bgaSettings,
                buSettings,
                btSettings
            );
            return settings;
        }
    }
}
