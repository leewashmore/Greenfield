using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TopDown.Core.ManagingTaxonomies
{
    public class TaxonomyTraverser
    {
        public IEnumerable<INode> TraverseTaxonomy(Taxonomy taxonomy)
        {
            var residents = taxonomy.GetResidents();
            foreach (var resident in residents)
            {
                var nodes = this.TraverseOnceResolved(resident);
                foreach (var node in nodes)
                {
                    yield return node;
                }
            }
        }

        private IEnumerable<INode> TraverseOnceResolved(ITaxonomyResident resident)
        {
            var resolver = new TraverseOnceResolved_ITaxonomyResidentResolver(this);
            resident.Accept(resolver);
            return resolver.Result;
        }

        private class TraverseOnceResolved_ITaxonomyResidentResolver : ITaxonomyResidentResolver
        {
            private TaxonomyTraverser traverser;

            public TraverseOnceResolved_ITaxonomyResidentResolver(TaxonomyTraverser traverser)
            {
                this.traverser = traverser;
            }

            public IEnumerable<INode> Result { get; private set; }

            public void Resolve(RegionNode node)
            {
                this.Result = this.traverser.TraverseRegion(node);
            }

            public void Resolve(OtherNode node)
            {
                this.Result = this.traverser.TraverseOther(node);
            }

            public void Resolve(BasketRegionNode node)
            {
                this.Result = this.traverser.TraverseBasketRegion(node);
            }
        }

        public IEnumerable<INode> TraverseRegion(RegionNode regionNode)
        {
            yield return regionNode;
            var residents = regionNode.GetResidents();
            foreach (var resident in residents)
            {
                var nodes = this.TraverseOnceResolved(resident);
                foreach (var node in nodes)
                {
                    yield return node;
                }
            }
        }

        public IEnumerable<INode> TraverseOnceResolved(IRegionNodeResident resident)
        {
            var resolver = new TraverseOnceResolved_IRegionNodeResidentResolver(this);
            resident.Accept(resolver);
            return resolver.Result;
        }

        private class TraverseOnceResolved_IRegionNodeResidentResolver : IRegionNodeResidentResolver
        {
            private TaxonomyTraverser traverser;

            public TraverseOnceResolved_IRegionNodeResidentResolver(TaxonomyTraverser traverser)
            {
                this.traverser = traverser;
            }

            public IEnumerable<INode> Result { get; private set; }

            public void Resolve(RegionNode node)
            {
                this.Result = this.traverser.TraverseRegion(node);
            }

            public void Resolve(BasketRegionNode node)
            {
                this.Result = this.traverser.TraverseBasketRegion(node);
            }

            public void Resolve(BasketCountryNode node)
            {
                this.Result = this.traverser.TraverseBasketCountry(node);
            }
        }

        public IEnumerable<INode> TraverseOther(OtherNode otherNode)
        {
            yield return otherNode;
            var residents = otherNode.GetBasketCountries();
            foreach (var resident in residents)
            {
                var nodes = this.TraverseOnceResolved(resident);
                foreach (var node in nodes)
                {
                    yield return node;
                }
            }
        }

        public IEnumerable<INode> TraverseBasketCountry(BasketCountryNode basketCountryNode)
        {
            yield return basketCountryNode;
        }

        public IEnumerable<INode> TraverseBasketRegion(BasketRegionNode basketRegionNode)
        {
            yield return basketRegionNode;
        }
    }
}
