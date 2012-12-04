using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopDown.Core.ManagingCountries;
using System.Xml;
using System.IO;
using TopDown.Core.Xml;

namespace TopDown.Core.ManagingBaskets
{
    /// <summary>
    /// Determines the specialization of the basket from its XML and reads it.
    /// </summary>
    public class XmlDeserializer
    {
        public const String RegionBasket = "region-basket";

        public IBasket ReadBasket(Int32 basketId, String definitionXml, CountryRepository countryRepository)
        {
            IBasket result;
            using (var reader = XmlReader.Create(new StringReader(definitionXml)))
            {
                var document = new DocumentElement(reader);
                var expectedElementNames = new String[] { RegionBasket };
                var someElement = document.MultilockOn(expectedElementNames);

                switch (someElement.Name)
                {
                    case RegionBasket:
                    {
                        result = this.ReadRegionBasket(someElement, basketId, countryRepository);
                        break;
                    }
                    default:
                    {
                        throw new ApplicationException("Anthough \"" + someElement.Name + "\" is an expected XML element of some basket type, there is no handler for reading it.");
                    }
                }
            }
            return result;
        }

        public RegionBasket ReadRegionBasket(IElement regionBasketElement, Int32 basketId, CountryRepository countryRepository)
        {
            var name = regionBasketElement.ReadAttributeAsNotEmptyString("name");
            var isoCodes = regionBasketElement.ReadAttributeAsNotEmptyString("iso-codes");
            var isoCodesDivided = isoCodes.Split(',').Select(x => x.Trim()).Where(x => !String.IsNullOrWhiteSpace(x));
            if (!isoCodesDivided.Any()) throw new ApplicationException("Region basket with the \"" + basketId + "\" doesn't have anything in its list of countries: " + isoCodes + ".");
            return new RegionBasket(basketId, name, isoCodesDivided.Select(x => countryRepository.GetCountry(x)));
        }
    }
}
