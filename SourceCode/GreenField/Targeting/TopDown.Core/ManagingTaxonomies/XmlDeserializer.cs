using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using TopDown.Core.Xml;
using System.Diagnostics;
using TopDown.Core.ManagingBaskets;
using TopDown.Core.ManagingCountries;

namespace TopDown.Core.ManagingTaxonomies
{
	public class XmlDeserializer
	{
		public const String RootElementName = "taxonomy";
		public const String BasketRegionElementName = "basket-region";
		public const String BasketCountryElementName = "basket-country";
		public const String UnsavedBasketCountryElementName = "unsaved-basket-country";
		public const String RegionElementName = "region";
		public const String CountryElementName = "country";
		public const String OtherElementName = "other";

		public void DeserializeTaxonomy(XmlReader reader, BasketRepository basketRepository, CountryRepository countryRepository, Taxonomy taxonomy)
		{
			var document = new DocumentElement(reader);
			var rootElement = document.LockOn("taxonomy");
			this.ReadRoot(rootElement, basketRepository, countryRepository, taxonomy);
		}

		protected void ReadRoot(IElement rootElement, BasketRepository basketRepository, CountryRepository countryRepository, Taxonomy taxonomy)
		{
			var expectedElements = new String[] { RegionElementName, BasketRegionElementName, OtherElementName };
			var someElementOpt = rootElement.TryMultiLockOn(expectedElements);
			while (someElementOpt != null)
			{
				ITaxonomyResident resident;
				switch (someElementOpt.Name)
				{
					case RegionElementName:
						{
							resident = this.ReadRegionNode(someElementOpt, basketRepository);
							break;
						}
					case BasketRegionElementName:
						{
							resident = this.ReadBasketRegionNode(someElementOpt, basketRepository);
							break;
						}
					case OtherElementName:
						{
							resident = this.ReadOtherNode(someElementOpt, basketRepository, countryRepository);
							break;
						}
					default:
						{
							throw new ApplicationException();
						}
				}
				taxonomy.RegisterResident(resident);

				someElementOpt = someElementOpt.ReleaseAndTryMultiLockOnNext(someElementOpt.Name, expectedElements);
			}
		}

		protected OtherNode ReadOtherNode(IElement element, BasketRepository basketRepository, CountryRepository countryRepository)
		{
			var result = new OtherNode();
			if (!element.IsAtomic)
			{
                var someElementOpt = element.TryLockOn(BasketCountryElementName);
				while (someElementOpt != null)
				{
                    var basketCountry = this.ReadBasketCountryNode(someElementOpt, basketRepository);
                    result.RegisterResident(basketCountry);
					someElementOpt = someElementOpt.ReleaseAndTryLockOnNext(someElementOpt.Name);
				}
			}
			return result;
		}

		protected RegionNode ReadRegionNode(IElement element, BasketRepository basketRepository)
		{
			var name = element.ReadAttributeAsNotEmptyString("name");
			var result = new RegionNode(name);

			var expectedElements = new String[] { RegionElementName, BasketCountryElementName, BasketRegionElementName };
			var someElementOpt = element.TryMultiLockOn(expectedElements);
			while (someElementOpt != null)
			{
				IRegionNodeResident resident;
				switch (someElementOpt.Name)
				{
					case RegionElementName:
						{
							resident = this.ReadRegionNode(someElementOpt, basketRepository);
							break;
						}
					case BasketCountryElementName:
						{
							resident = this.ReadBasketCountryNode(someElementOpt, basketRepository);
							break;
						}
					case BasketRegionElementName:
						{
							resident = this.ReadBasketRegionNode(someElementOpt, basketRepository);
							break;
						}
					default:
						{
							throw new ApplicationException();
						}
				}
				result.RegisterResident(resident);
				someElementOpt = someElementOpt.ReleaseAndTryMultiLockOnNext(someElementOpt.Name, expectedElements);
			}
			return result;
		}

		protected BasketRegionNode ReadBasketRegionNode(IElement element, BasketRepository basketRepository)
		{
			var basketId = element.ReadAttributeAsInt32("basketId");
            var basket = basketRepository.GetBasket(basketId).AsRegionBasket();
			var result = new BasketRegionNode(basket);
			return result;
		}

		protected BasketCountryNode ReadBasketCountryNode(IElement element, BasketRepository basketRepository)
		{
			var basketId = element.ReadAttributeAsInt32("basketId");
			var basket = basketRepository.GetBasket(basketId).AsCountryBasket();
			var result = new BasketCountryNode(basket);
			return result;
		}
	}
}
