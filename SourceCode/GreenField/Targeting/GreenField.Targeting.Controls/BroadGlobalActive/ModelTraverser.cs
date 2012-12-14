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
using System.Collections.Generic;
using TopDown.FacingServer.Backend.Targeting;

namespace GreenField.Targeting.Controls.BroadGlobalActive
{
    public class ModelTraverser
    {

        public IEnumerable<IGlobeResident> Traverse(BgaRootModel model)
        {
            model.InitializeWhenDeSerializationIsDone();
            var result = new List<IGlobeResident>();

            this.TraverseGlobe(model.Globe, result);

            var cashLine = new CashLineModel(model.Cash.Base, model.Cash.Scaled);
            cashLine.Parent = model.Globe;
            result.Add(cashLine);

            var globe = model.Globe;

            var totalLine = new TotalLineModel(
                model.GrandTotal,
                globe.Benchmark,
                globe.Overlay,
                globe.PortfolioAdjustment,
                globe.PortfolioScaled,
                globe.TrueExposure,
                globe.TrueActive
            );
            totalLine.Parent = model.Globe;
            result.Add(totalLine);
            
            return result;
        }

        public IEnumerable<IGlobeResident> Traverse(GlobeModel model)
        {
            var result = new List<IGlobeResident>();
            this.TraverseGlobe(model, result);
            return result;
        }

        protected void TraverseGlobe(GlobeModel model, ICollection<IGlobeResident> result)
        {
            foreach (var resident in model.Residents)
            {
                resident.Parent = model;
                this.TraverseResidentOnceResolved(resident, result);
            }
        }

        protected void TraverseResidentOnceResolved(IGlobeResident resident, ICollection<IGlobeResident> result)
        {
            var resolver = new TraverseResidentOnceResolved_IGlobeResidentResolver(this, result);
            resident.Accept(resolver);
        }

        private class TraverseResidentOnceResolved_IGlobeResidentResolver : IGlobeResidentResolver
        {
            private ModelTraverser traverser;
            private ICollection<IGlobeResident> result;

            public TraverseResidentOnceResolved_IGlobeResidentResolver(ModelTraverser traverser, ICollection<IGlobeResident> result)
            {
                this.traverser = traverser;
                this.result = result;
            }

            public void Resolve(BasketCountryModel model)
            {
                this.traverser.TraverseBasketCountry(model, this.result);
            }

            public void Resolve(BasketRegionModel model)
            {
                this.traverser.TraverseBasketRegion(model, this.result);
            }

            public void Resolve(OtherModel model)
            {
                this.traverser.TraverseOther(model, this.result);
            }

            public void Resolve(RegionModel model)
            {
                this.traverser.TraverseRegion(model, this.result);
            }

            public void Resolve(UnsavedBasketCountryModel model)
            {
                this.traverser.TraverseUnsavedBasketCountry(model, this.result);
            }

            public void Resolve(BgaCountryModel model)
            {
                this.traverser.TraverseCountry(model, this.result);
            }

            public void Resolve(CashLineModel model)
            {
                throw new InvalidCastException("Not expeceted here.");
            }

            public void Resolve(TotalLineModel model)
            {
                throw new InvalidCastException("Not expeceted here.");
            }
        }

        protected void TraverseBasketCountry(BasketCountryModel model, ICollection<IGlobeResident> result)
        {
            result.Add(model);
        }

        protected void TraverseBasketRegion(BasketRegionModel model, ICollection<IGlobeResident> result)
        {
            result.Add(model);
            foreach (var resident in model.Countries)
            {
                resident.Parent = model;
                this.TraverseCountry(resident, result);
            }
        }

        protected void TraverseOther(OtherModel model, ICollection<IGlobeResident> result)
        {
            result.Add(model);
            foreach (var resident in model.BasketCountries)
            {
                resident.Parent = model;
                this.TraverseBasketCountry(resident, result);
            }
        }

        protected void TraverseRegion(RegionModel model, ICollection<IGlobeResident> result)
        {
            result.Add(model);
            foreach (var resident in model.Residents)
            {
                resident.Parent = model;
                this.TraverseResidentOnceResolved(resident, result);
            }
        }

        protected void TraverseUnsavedBasketCountry(UnsavedBasketCountryModel model, ICollection<IGlobeResident> result)
        {
            result.Add(model);
        }

        protected void TraverseCountry(BgaCountryModel model, ICollection<IGlobeResident> result)
        {
            result.Add(model);
        }
    }
}
