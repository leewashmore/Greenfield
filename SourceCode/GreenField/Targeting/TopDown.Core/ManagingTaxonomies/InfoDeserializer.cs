using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopDown.Core.Persisting;
using System.IO;
using System.Xml;
using TopDown.Core.ManagingBaskets;
using TopDown.Core.ManagingCountries;
using Aims.Core;

namespace TopDown.Core.ManagingTaxonomies
{
    public class InfoDeserializer
    {
        private XmlDeserializer xmlDeserializer;

        public InfoDeserializer(XmlDeserializer xmlDeserializer)
        {
            this.xmlDeserializer = xmlDeserializer;
        }

        public Taxonomy DeserializeTaxonomy(
			TaxonomyInfo taxonomyInfo,
			BasketRepository basketRepository,
			CountryRepository countryRepository
		)
        {
            var taxonomy = new Taxonomy(taxonomyInfo.Id);
            using (var reader = XmlReader.Create(new StringReader(taxonomyInfo.DefinitionXml)))
            {
				this.xmlDeserializer.DeserializeTaxonomy(reader, basketRepository, countryRepository, taxonomy);
            }
            return taxonomy;
        }
    }
}
