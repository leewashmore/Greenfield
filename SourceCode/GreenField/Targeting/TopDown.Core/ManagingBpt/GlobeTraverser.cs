using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TopDown.Core.ManagingBpt
{
    public class GlobeTraverser
    {
        public IEnumerable<IModel> TraverseGlobe(GlobeModel root)
        {
            var residents = root.Residents;
            foreach (var resident in residents)
            {
                var models = this.TraverseOnceResolved(resident);
                foreach (var model in models)
                {
                    yield return model;
                }
            }
        }

        protected IEnumerable<IModel> TraverseOnceResolved(IGlobeResident resident)
        {
            var resolver = new TraverseOnceResolved_IBreakdownModelResidentResolver(this);
            resident.Accept(resolver);
            return resolver.Result;
        }

        private class TraverseOnceResolved_IBreakdownModelResidentResolver : IGlobeResidentResolver
        {
            private GlobeTraverser traverser;

            public TraverseOnceResolved_IBreakdownModelResidentResolver(GlobeTraverser traverser)
            {
                this.traverser = traverser;
            }

            public IEnumerable<IModel> Result { get; private set; }

            public void Resolve(RegionModel model)
            {
                this.Result = this.traverser.TraverseRegion(model);
            }

            public void Resolve(OtherModel model)
            {
                this.Result = this.traverser.TraverseOther(model);
            }

            public void Resolve(BasketRegionModel model)
            {
                this.Result = this.traverser.TraverseBasketRegion(model);
            }
        }

        protected IEnumerable<IModel> TraverseRegion(RegionModel regionModel)
        {
            yield return regionModel;
            foreach (var resident in regionModel.Residents)
            {
                var models = this.TraverseOnceResolved(resident);
                foreach (var model in models)
                {
                    yield return model;
                }
            }
        }

        protected IEnumerable<IModel> TraverseOnceResolved(IRegionModelResident resident)
        {
            var resolver = new TraverseOnceResolved_IRegionModelResidentResolver(this);
            resident.Accept(resolver);
            return resolver.Result;
        }

        private class TraverseOnceResolved_IRegionModelResidentResolver : IRegionModelResidentResolver
        {
            private GlobeTraverser traverser;

            public TraverseOnceResolved_IRegionModelResidentResolver(GlobeTraverser traverser)
            {
                this.traverser = traverser;
            }

            public IEnumerable<IModel> Result { get; private set; }

            public void Resolve(BasketCountryModel model)
            {
                this.Result = this.traverser.TraverseBasketCountry(model);
            }

            public void Resolve(BasketRegionModel model)
            {
                this.Result = this.traverser.TraverseBasketRegion(model);
            }

            public void Resolve(RegionModel model)
            {
                this.Result = this.traverser.TraverseRegion(model);
            }
        }

        protected IEnumerable<IModel> TraverseOther(OtherModel otherModel)
        {
            yield return otherModel;
            foreach (var basketCountry in otherModel.BasketCountries)
            {
                var models = this.TraverseBasketCountry(basketCountry);
                foreach (var model in models)
                {
                    yield return model;
                }
            }

            foreach (var unsavedBasketCountry in otherModel.UnsavedBasketCountries)
            {
                var models = this.TraverseUnsavedBasketCountry(unsavedBasketCountry);
                foreach (var model in models)
                {
                    yield return model;
                }
            }

        }

        protected IEnumerable<IModel> TraverseBasketCountry(BasketCountryModel basketCountryModel)
        {
            yield return basketCountryModel;
        }

        protected IEnumerable<IModel> TraverseBasketRegion(BasketRegionModel basketRegionModel)
        {
            yield return basketRegionModel;
            var countryModels = basketRegionModel.Countries;
            foreach (var countryModel in countryModels)
            {
                var models = this.TraverseCountry(countryModel);
                foreach (var model in models)
                {
                    yield return model;
                }
            }
        }

        protected IEnumerable<IModel> TraverseCountry(CountryModel countryModel)
        {
            yield return countryModel;
        }

        protected IEnumerable<IModel> TraverseUnsavedBasketCountry(UnsavedBasketCountryModel unsavedBasketCountryModel)
        {
            yield return unsavedBasketCountryModel;
        }
    }
}
