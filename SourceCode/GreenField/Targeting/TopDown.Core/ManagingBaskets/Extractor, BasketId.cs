using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopDown.Core.ManagingTargetingTypes;
using TopDown.Core.ManagingTaxonomies;

namespace TopDown.Core.ManagingBaskets
{
    public class BasketExtractor
    {
        private TaxonomyTraverser taxonomyTraverser;

        public BasketExtractor(TaxonomyTraverser taxonomyTraverser)
        {
            this.taxonomyTraverser = taxonomyTraverser;
        }

        public IEnumerable<IBasket> ExtractBaskets(TargetingTypeGroup targetingTypeGroup)
        {
            return targetingTypeGroup.GetTargetingTypes().SelectMany(x => this.ExtractBaskets(x)).Distinct();
        }

        public IEnumerable<IBasket> ExtractBaskets(TargetingType targetingType)
        {
            return this.ExtractBasket(targetingType.Taxonomy);
        }

        public IEnumerable<IBasket> ExtractBasket(Taxonomy taxonomy)
        {
            var nodes = this.taxonomyTraverser.TraverseTaxonomy(taxonomy);
            return nodes.Select(x => this.TryExtractBasketOnceResolved(x)).Where(x => x != null);
        }

        public IBasket TryExtractBasketOnceResolved(INode node)
        {
            var resolver = new ExtractBasketsIdsOnceResolved_INodeResolver(this);
            node.Accept(resolver);
            return resolver.ResultOpt;
        }

        private class ExtractBasketsIdsOnceResolved_INodeResolver : INodeResolver
        {
            private BasketExtractor extractor;

            public ExtractBasketsIdsOnceResolved_INodeResolver(BasketExtractor extractor)
            {
                this.extractor = extractor;
            }

            public IBasket ResultOpt { get; private set; }

            public void Resolve(BasketCountryNode node)
            {
                this.ResultOpt = node.Basket;
            }

            public void Resolve(OtherNode node)
            {
                this.ResultOpt = null;
            }

            public void Resolve(RegionNode node)
            {
                this.ResultOpt = null;
            }

            public void Resolve(BasketRegionNode node)
            {
                this.ResultOpt = node.Basket;
            }
        }
    }
}
