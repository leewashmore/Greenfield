using System;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.MefExtensions.Modularity;
using Microsoft.Practices.Prism.Regions;
using System.ComponentModel.Composition;
using GreenField.Module.Views;


namespace GreenField.Module
{
    [ModuleExport(typeof(Module))]
    public class Module : IModule
    {
        IRegionManager _regionManager;

        [ImportingConstructor]
        public Module(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void Initialize()
        {
            _regionManager.RegisterViewWithRegion("MainRegion", typeof(Home));
            _regionManager.RegisterViewWithRegion("MainRegion", typeof(ConsensusEstimateView));
            _regionManager.RegisterViewWithRegion("MainRegion", typeof(DetailedEstimateView));
            _regionManager.RegisterViewWithRegion("MainRegion", typeof(HoldingsView));
            _regionManager.RegisterViewWithRegion("MainRegion", typeof(AggregatedDataView));
            _regionManager.RegisterViewWithRegion("MainRegion", typeof(PerformanceView));
            _regionManager.RegisterViewWithRegion("MainRegion", typeof(ReferenceView));            
        }
    }
}
