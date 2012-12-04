using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopDown.Core.ManagingBpt;
using TopDown.Core.ManagingTaxonomies;

namespace TopDown.Core.ManagingBpt
{
    public class ModelToTaxonomyTransformer
    {
        /// <summary>
        /// Populates the taxonomy with the information pulled from the breakdown.
        /// </summary>
        public void Populate(Taxonomy taxonomy, GlobeModel root)
        {
            foreach (var resident in root.Residents)
            {
                var node = this.CreateNodeOnceResolved(resident);
                taxonomy.RegisterResident(node);
            }
        }

        private ITaxonomyResident CreateNodeOnceResolved(IGlobeResident resident)
        {
            var resolver = new CreateNodeOnceResolved_IBreakdownModelResidentResolver(this);
            resident.Accept(resolver);
            return resolver.Result;
        }

        private class CreateNodeOnceResolved_IBreakdownModelResidentResolver : IGlobeResidentResolver
        {
            private ModelToTaxonomyTransformer builder;

            public CreateNodeOnceResolved_IBreakdownModelResidentResolver(ModelToTaxonomyTransformer builder)
            {
                this.builder = builder;
            }

            public ITaxonomyResident Result { get; private set; }

            public void Resolve(RegionModel model)
            {
                this.Result = this.builder.CreateNode(model);
            }

            public void Resolve(OtherModel model)
            {
                this.Result = this.builder.CreateNode(model);
            }

            public void Resolve(BasketRegionModel model)
            {
                this.Result = this.builder.CreateBasketRegion(model);
            }
        }

        public RegionNode CreateNode(RegionModel regionModel)
        {
            var result = new RegionNode(regionModel.Name);
            foreach (var residentModel in regionModel.Residents)
            {
                var node = this.CreateNodeOnceResolved(residentModel);
                result.RegisterResident(node);
            }
            return result;
        }

        public IRegionNodeResident CreateNodeOnceResolved(IRegionModelResident residentModel)
        {
            var resolver = new CreateNodeOnceResolved_IRegionModelResidentResolver(this);
            residentModel.Accept(resolver);
            return resolver.Result;
        }

        private class CreateNodeOnceResolved_IRegionModelResidentResolver : IRegionModelResidentResolver
        {
            private ModelToTaxonomyTransformer builder;

            public CreateNodeOnceResolved_IRegionModelResidentResolver(ModelToTaxonomyTransformer builder)
            {
                this.builder = builder;
            }

            public IRegionNodeResident Result { get; private set; }

            public void Resolve(BasketCountryModel model)
            {
                this.Result = this.builder.CreateBasketCountry(model);
            }

            public void Resolve(BasketRegionModel model)
            {
                this.Result = this.builder.CreateBasketRegion(model);
            }

            public void Resolve(RegionModel model)
            {
                this.Result = this.builder.CreateNode(model);
            }
        }

        public OtherNode CreateNode(OtherModel otherModel)
        {
            if (otherModel.UnsavedBasketCountries.Any())
            {
                throw new ApplicationException("Unable to create a taxonomy because the model has unsaved countries.");
            }

            var result = new OtherNode();
            foreach (var basketCountry in otherModel.BasketCountries)
            {
                var node = this.CreateBasketCountry(basketCountry);
                result.RegisterResident(node);
            }
            return result;
        }

        public BasketCountryNode CreateBasketCountry(BasketCountryModel model)
        {
            var result = new BasketCountryNode(model.Basket);
            return result;
        }

        public BasketRegionNode CreateBasketRegion(BasketRegionModel model)
        {
            var result = new BasketRegionNode(model.Basket);
            return result;
        }
    }
}
