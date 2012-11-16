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
using System.Diagnostics;
using GreenField.ServiceCaller.TargetingDefinitions;

namespace GreenField.Gadgets.ViewModels.Targeting.BroadGlobalActive
{
    public class DefaultExpandCollapseStateSetter
    {
        private ModelTraverser traverser;

        [DebuggerStepThrough]
        public DefaultExpandCollapseStateSetter(ModelTraverser traverser)
        {
            this.traverser = traverser;
        }

        public void SetDefaultCollapseExpandState(GlobeModel model)
        {
            model.IsExpanded = true;
            var residents = this.traverser.Traverse(model);
            foreach (var resident in residents)
            {
                this.SetStateOnceResolved(resident);
            }
        }

        private void SetStateOnceResolved(IGlobeResident model)
        {
            var resolver = new SetStateOnceResolved_IGlobeResidentResolver(this);
            model.Accept(resolver);
        }

        private class SetStateOnceResolved_IGlobeResidentResolver : IGlobeResidentResolver
        {
            private DefaultExpandCollapseStateSetter setter;

            public SetStateOnceResolved_IGlobeResidentResolver(DefaultExpandCollapseStateSetter setter)
            {
                this.setter = setter;
            }

            public void Resolve(BasketCountryModel model)
            {
                // do nothing
            }

            public void Resolve(BasketRegionModel model)
            {
                model.IsExpanded = false;
            }

            public void Resolve(OtherModel model)
            {
                model.IsExpanded = true;
            }

            public void Resolve(RegionModel model)
            {
                model.IsExpanded = true;
            }

            public void Resolve(UnsavedBasketCountryModel model)
            {
                // do nothing
            }

            public void Resolve(BgaCountryModel model)
            {
                // do nothing
            }
        }
    }
}
