using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopDown.Core.ManagingTaxonomies;
using System.Diagnostics;

namespace TopDown.Core.ManagingTaxonomies
{
    public class CountryIsoCodesExtractor
    {
        private TaxonomyTraverser traverser;
        
        [DebuggerStepThrough]
        public CountryIsoCodesExtractor(TaxonomyTraverser traverser)
        {
            this.traverser = traverser;
        }

        public IEnumerable<String> ExtractCodes(Taxonomy taxonomy)
        {
            return this.traverser.TraverseTaxonomy(taxonomy).SelectMany(x => this.ExtractIsoCodes(x));
        }

        protected IEnumerable<String> ExtractIsoCodes(INode node)
        {
            var resolver = new TryExtractIsoCode_INodeResolver(this);
            node.Accept(resolver);
            return resolver.Result;
        }

        private class TryExtractIsoCode_INodeResolver : INodeResolver
        {
            private CountryIsoCodesExtractor extractor;

            public TryExtractIsoCode_INodeResolver(CountryIsoCodesExtractor extractor)
            {
                this.extractor = extractor;
            }
            
            public IEnumerable<String> Result { get; private set; }

            public void Resolve(BasketCountryNode basketCountry)
            {
                this.Result = new String[] { basketCountry.Basket.Country.IsoCode };
            }

            public void Resolve(OtherNode other)
            {
                this.Result = No.IsoCodes;                
            }

            public void Resolve(RegionNode region)
            {
                this.Result = No.IsoCodes;
            }

            public void Resolve(BasketRegionNode basketRegion)
            {
                this.Result = basketRegion.Basket.Countries.Select(x => x.IsoCode);
            }
        }

    }
}
