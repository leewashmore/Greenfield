using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using TopDown.Core.Xml;

namespace TopDown.Core.ManagingTaxonomies
{
    /// <summary>
    /// Writes a taxonomy into XML.
    /// </summary>
    public class TaxonomyToXmlWriter
    {
        public const String NamespaceUri = "urn:TopDown.Core";
        public void Write(Taxonomy taxonomy, XmlWriter writer)
        {
            writer.WriteElement("taxonomy", NamespaceUri, delegate
            {
                var residents = taxonomy.GetResidents();
                foreach (var resident in residents)
                {
                    this.WriteOnceResolved(resident, writer);
                }
            });
        }

        public void WriteOnceResolved(ITaxonomyResident resident, XmlWriter writer)
        {
            var resolver = new WriteOnceResolved_ITaxonomyResidentResolver(this, writer);
            resident.Accept(resolver);
        }

        private class WriteOnceResolved_ITaxonomyResidentResolver : ITaxonomyResidentResolver
        {
            private TaxonomyToXmlWriter parent;
            private XmlWriter writer;

            public WriteOnceResolved_ITaxonomyResidentResolver(TaxonomyToXmlWriter parent, XmlWriter writer)
            {
                this.parent = parent;
                this.writer = writer;
            }

            public void Resolve(RegionNode node)
            {
                this.parent.Write(node, this.writer);
            }

            public void Resolve(OtherNode node)
            {
                this.parent.Write(node, this.writer);
            }


            public void Resolve(BasketRegionNode node)
            {
                this.parent.Write(node, this.writer);
            }
        }

        public void Write(RegionNode node, XmlWriter writer)
        {
            writer.WriteElement("region", delegate
            {
                writer.WriteAttribute("name", node.Name);
                var residents = node.GetResidents();
                foreach (var resident in residents)
                {
                    this.WriteOnceResolved(resident, writer);
                }
            });
        }

        public void WriteOnceResolved(IRegionNodeResident resident, XmlWriter writer)
        {
            var resolver = new WriteOnceResolved_IRegionNodeResidentResolver(this, writer);
            resident.Accept(resolver);
        }

        private class WriteOnceResolved_IRegionNodeResidentResolver : IRegionNodeResidentResolver
        {
            private XmlWriter writer;
            private TaxonomyToXmlWriter parent;

            public WriteOnceResolved_IRegionNodeResidentResolver(TaxonomyToXmlWriter parent, XmlWriter writer)
            {
                this.parent = parent;
                this.writer = writer;
            }

            public void Resolve(RegionNode node)
            {
                this.parent.Write(node, writer);
            }

            public void Resolve(BasketRegionNode node)
            {
                this.parent.Write(node, writer);
            }

            public void Resolve(BasketCountryNode node)
            {
                this.parent.Write(node, writer);
            }
        }

        public void Write(OtherNode node, XmlWriter writer)
        {
            writer.WriteElement("other", delegate
            {
                var residents = node.GetBasketCountries();
                foreach (var resident in residents)
                {
                    this.WriteOnceResolved(resident, writer);
                }
            });
        }

        public void Write(BasketRegionNode node, XmlWriter writer)
        {
            writer.WriteElement("basket-region", delegate
            {
                writer.WriteAttribute("basketId", node.Basket.Id);
            });
        }

        public void Write(BasketCountryNode node, XmlWriter writer)
        {
            writer.WriteElement("basket-country", delegate
            {
                writer.WriteAttribute("basketId", node.Basket.Id);
            });
        }
    }
}
