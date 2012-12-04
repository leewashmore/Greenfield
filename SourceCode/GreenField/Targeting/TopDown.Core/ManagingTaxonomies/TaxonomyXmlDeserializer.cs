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

	public class TaxonomyXmlDeserializer
	{
		public const String RootElementName = "taxonomy";
		public const String BasketRegionElementName = "basket-region";
		public const String BasketCountryElementName = "basket-country";
		public const String UnsavedBasketCountryElementName = "unsaved-basket-country";
		public const String RegionElementName = "region";
		public const String CountryElementName = "country";
		public const String OtherElementName = "other";


		public void ReadTaxonomy(BasketRepository basketRepository, XmlReader reader, Taxonomy taxonomy)
		{
			var document = new DocumentElement(reader);
			var rootElement = document.LockOn("taxonomy");
			this.ReadRoot(basketRepository, rootElement, taxonomy);
		}

		protected void ReadRoot(BasketRepository basketRepository, IElement rootElement, Taxonomy taxonomy)
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
							resident = this.ReadRegionNode(basketRepository, someElementOpt);
							break;
						}
					case BasketRegionElementName:
						{
							resident = this.ReadBasketRegionNode(basketRepository, someElementOpt);
							break;
						}
					case OtherElementName:
						{
							resident = this.ReadOtherNode(basketRepository, someElementOpt);
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

		protected OtherNode ReadOtherNode(BasketRepository basketRepository, IElement element)
		{
			var expectedElementNames = new String[] { BasketCountryElementName, UnsavedBasketCountryElementName };
			var result = new OtherNode();
			if (!element.IsAtomic)
			{
				var someElementOpt = element.TryMultiLockOn(expectedElementNames);
				while (someElementOpt != null)
				{
					IOtherNodeResident resident;
					switch (someElementOpt.Name)
					{

						case BasketCountryElementName:
							{
								var basketCountry = this.ReadBasketCountryNode(basketRepository, someElementOpt);
								resident = basketCountry;
								break;
							}
						case UnsavedBasketCountryElementName:
							{
								var unsavedBasketCountry = this.ReadUnsavedBasketCountry(someElementOpt);
								resident = unsavedBasketCountry;
								break;
							}
						default: throw new ApplicationException("Unexpected element name: " + someElementOpt.Name);
					}

					result.RegisterResident(resident);
					someElementOpt = someElementOpt.ReleaseAndTryMultiLockOnNext(someElementOpt.Name, expectedElementNames);
				}
			}
			return result;
		}

		protected RegionNode ReadRegionNode(BasketRepository basketRepository, IElement element)
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
							resident = this.ReadRegionNode(basketRepository, someElementOpt);
							break;
						}
					case BasketCountryElementName:
						{
							resident = this.ReadBasketCountryNode(basketRepository, someElementOpt);
							break;
						}
					case BasketRegionElementName:
						{
							resident = this.ReadBasketRegionNode(basketRepository, someElementOpt);
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

		protected BasketRegionNode ReadBasketRegionNode(BasketRepository basketRepository, IElement element)
		{
			var basketId = element.ReadAttributeAsInt32("basketId");
            var basket = basketRepository.GetBasket(basketId).AsRegionBasket();
			var name = element.ReadAttributeAsNotEmptyString("name");
			var result = new BasketRegionNode(basket);
			return result;
		}

		protected BasketCountryNode ReadBasketCountryNode(BasketRepository basketRepository, IElement element)
		{
			var basketId = element.ReadAttributeAsInt32("basketId");
			var basket = basketRepository.GetBasket(basketId).AsCountryBasket();
			var result = new BasketCountryNode(basket);
			return result;
		}

		protected UnsavedBasketCountryNode ReadUnsavedBasketCountry(IElement element)
		{
			var name = element.ReadAttributeAsNotEmptyString("name");
			var isoCode = element.ReadAttributeAsNotEmptyString("iso-code");
			var country = new Country(isoCode, name);
			var result = new UnsavedBasketCountryNode(country);
			return result;
		}
	}
}
