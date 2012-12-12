using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopDown.Core.ManagingCountries;
using Aims.Core;

namespace TopDown.Core.ManagingTaxonomies
{
    /// <summary>
    /// Creates a deep copy of a taxonomy.
    /// </summary>
    public class TaxonomyCopier
    {
        public Taxonomy Copy(Taxonomy taxonomy)
        {
            var result = new Taxonomy(taxonomy.Id);
            var residents = taxonomy.GetResidents();
            foreach (var resident in residents)
            {
                var copied =  this.CopyOnceResolved(resident);
                result.RegisterResident(copied);
            }
            return result;
        }

        private ITaxonomyResident CopyOnceResolved(ITaxonomyResident resident)
        {
            var resolver = new CopyOnceResolved_ITaxonomyResidentResolver(this);
            resident.Accept(resolver);
            return resolver.Result;
        }

        private class CopyOnceResolved_ITaxonomyResidentResolver : ITaxonomyResidentResolver
        {
            private TaxonomyCopier copier;

            public CopyOnceResolved_ITaxonomyResidentResolver(TaxonomyCopier copier)
            {
                this.copier = copier;
            }

            public ITaxonomyResident Result { get; private set; }

            public void Resolve(RegionNode node)
            {
                this.Result = this.copier.Copy(node);
            }

            public void Resolve(OtherNode node)
            {
                this.Result = this.copier.Copy(node);
            }

            public void Resolve(BasketRegionNode node)
            {
                this.Result = this.copier.Copy(node);
            }
        }

        public RegionNode Copy(RegionNode node)
        {
            var result = new RegionNode(node.Name);
            var residents = node.GetResidents();
            foreach (var resident in residents)
            {
                var copied = this.CopyOnceResolved(resident);
                result.RegisterResident(resident);
            }
            return result;
        }

        public IRegionNodeResident CopyOnceResolved(IRegionNodeResident resident)
        {
            var resolver = new CopyOnceResolved_IRegionNodeResidentResolver(this);
            resident.Accept(resolver);
            return resolver.Result;
        }

        private class CopyOnceResolved_IRegionNodeResidentResolver : IRegionNodeResidentResolver
        {
            private TaxonomyCopier copier;

            public CopyOnceResolved_IRegionNodeResidentResolver(TaxonomyCopier copier)
            {
                this.copier = copier;
            }

            public IRegionNodeResident Result { get; private set; }

            public void Resolve(RegionNode node)
            {
                this.Result = this.copier.Copy(node);
            }

            public void Resolve(BasketRegionNode node)
            {
                this.Result = this.copier.Copy(node);
            }

            public void Resolve(BasketCountryNode node)
            {
                this.Result = this.copier.Copy(node);
            }
        }

        public OtherNode Copy(OtherNode node)
        {
            var result = new OtherNode();
            var basketCountries = node.GetBasketCountries();
            foreach (var basketCountry in basketCountries)
            {
                var copied = this.Copy(basketCountry);
                result.RegisterResident(copied);
            }
            return result;
        }

        public BasketRegionNode Copy(BasketRegionNode node)
        {
            var result = new BasketRegionNode(node.Basket);
            return result;
        }

        public Country CopyCountry(Country country)
        {
            var result = new Country(country.IsoCode, country.Name);
            return result;
        }

        public BasketCountryNode Copy(BasketCountryNode node)
        {
            var result = new BasketCountryNode(node.Basket);
            return result;
        }
    }
}
