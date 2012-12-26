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
using Microsoft.Practices.Prism.MefExtensions;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Regions;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.Modularity;
using GreenField.IssuerShares.Controls;

namespace GreenField.IssuerShares.App
{
    public class Bootstrapper : MefBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            var shell = this.Container.GetExportedValue<ShellView>();
            return shell;
        }

        protected override void ConfigureModuleCatalog()
        {
            base.ConfigureModuleCatalog();
        }

        protected override void ConfigureAggregateCatalog()
        {
            base.ConfigureAggregateCatalog();
            var catalogs = this.AggregateCatalog.Catalogs;
            catalogs.Add(new AssemblyCatalog(typeof(Bootstrapper).Assembly));
            catalogs.Add(new AssemblyCatalog(typeof(Controls.Module).Assembly));
        }

        protected override IRegionBehaviorFactory ConfigureDefaultRegionBehaviors()
        {
            var factory = base.ConfigureDefaultRegionBehaviors();
            return factory;
        }

        protected override void InitializeShell()
        {
            base.InitializeShell();
            var root = (UIElement)this.Shell;
            Application.Current.RootVisual = root;
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();
            this.Container.ComposeExportedValue(this.Logger);

            var clientFactory = new DefaultClientFactory();
            this.Container.ComposeExportedValue<IClientFactory>(clientFactory);
            
        }
    }
}
