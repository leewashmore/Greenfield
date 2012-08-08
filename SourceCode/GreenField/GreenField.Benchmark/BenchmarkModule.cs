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
using GreenField.Common;
using GreenField.Benchmark.Views;

namespace GreenField.Benchmark
{
    [ModuleExport(typeof(BenchmarkModule))]
    public class BenchmarkModule : IModule
    {
        IRegionManager _regionManager;

        [ImportingConstructor]
        public BenchmarkModule(IRegionManager regionManager)
        {
            try
            {
                _regionManager = regionManager;
            }
            catch (Exception)
            {
            
            }
        }

        public void Initialize()
        {
            try
            {
                _regionManager.RegisterViewWithRegion(RegionNames.MAIN_REGION, typeof(ViewMorningSnapshot));
            }
            catch (Exception)
            {
            
            }
        }
    }
}