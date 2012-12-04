using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopDown.Core.Persisting;
using System.Diagnostics;
using System.Xml;
using System.IO;
using TopDown.Core.ManagingBaskets;

namespace TopDown.Core.ManagingTaxonomies
{
	/// <summary>
	/// Restores a Taxonomy object from its persistent representation.
	/// </summary>
	public class TaxonomyDeserializer
	{
		private TaxonomyXmlDeserializer xmlDeserializer;
		
		[DebuggerStepThrough]
		public TaxonomyDeserializer(TaxonomyXmlDeserializer xmlDeserializer)
		{
			this.xmlDeserializer = xmlDeserializer;
		}

		public Taxonomy Deserialize(BasketRepository basketRepository, TaxonomyInfo taxonomyInfo)
		{
			var result = new Taxonomy(taxonomyInfo.Id);
			var xml = taxonomyInfo.DefinitionXml;
			using (var reader = XmlReader.Create(new StringReader(xml)))
			{
				this.xmlDeserializer.ReadTaxonomy(basketRepository, reader, result);
			}
			return result;
		}
	}
}
