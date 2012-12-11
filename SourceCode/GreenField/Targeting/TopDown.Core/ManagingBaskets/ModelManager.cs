using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using TopDown.Core.Persisting;
using TopDown.Core.ManagingCountries;
using TopDown.Core.ManagingSecurities;
using Aims.Core;

namespace TopDown.Core.ManagingBaskets
{
	public class BasketManager
	{
		public const String BasketRepositoryStorageKey = "BasketRepository";
		private IStorage<BasketRepository> basketRepositoryStorage;
		private XmlDeserializer xmlDeserializer;
		private BasketSecurityRelationshipInvestigator basketSecurityRelationshipInvestigator;


		[DebuggerStepThrough]
		public BasketManager(
			IStorage<BasketRepository> basketRepositoryStorage,
			XmlDeserializer xmlDeserializer,
			BasketSecurityRelationshipInvestigator basketSecurityRelationshipInvestigator
		)
		{
			this.xmlDeserializer = xmlDeserializer;
			this.basketRepositoryStorage = basketRepositoryStorage;
			this.basketSecurityRelationshipInvestigator = basketSecurityRelationshipInvestigator;
		}

		public BasketRepository ClaimBasketRepository(
			IOnDemand<IDataManager> ondemandManager,
			Func<CountryRepository> ondemandCountryRepository,
			IMonitor monitor
		)
		{
			return this.basketRepositoryStorage.Claim(BasketRepositoryStorageKey, delegate
			{
				return this.CreateBasketRepository(
					ondemandManager.Claim(),
					ondemandCountryRepository(),
					monitor
				);
			});
		}

        public BasketRepository ClaimBasketRepository(IDataManager manager, Func<CountryRepository> ondemandCountryRepository, IMonitor monitor)
        {
            return this.basketRepositoryStorage.Claim(BasketRepositoryStorageKey, delegate
            {
                return this.CreateBasketRepository(
                    manager,
                    ondemandCountryRepository(),
                    monitor
                );
            });
        }

		public BasketRepository ClaimBasketRepository(
			IOnDemand<IDataManager> ondemandManager,
			CountryRepository countryRepository,
			IMonitor monitor
		)
		{
			return this.basketRepositoryStorage.Claim(BasketRepositoryStorageKey, delegate
			{
				var manager = ondemandManager.Claim();
				return this.CreateBasketRepository(
					manager,
					countryRepository,
					monitor
				);
			});
		}

        

		protected BasketRepository CreateBasketRepository(IDataManager manager, CountryRepository countryRepository, IMonitor monitor)
		{
			var basketInfos = manager.GetAllBaskets();
			var countryBasketInfos = manager.GetAllCountryBaskets().ToDictionary(x => x.Id);
			var regionBasketInfos = manager.GetAllRegionBaskets().ToDictionary(x => x.Id);

			var baskets = new List<IBasket>();
			foreach (var basketInfo in basketInfos)
			{
				var basketOpt = monitor.DefaultIfFails<IBasket>("Creating a basket of the \"" + basketInfo.Type + "\" type and \"" + basketInfo.Id + "\" ID.", delegate
				{
					switch (basketInfo.Type)
					{
						case "country":
						{
							CountryBasketInfo countryBasketInfo;
							if (countryBasketInfos.TryGetValue(basketInfo.Id, out countryBasketInfo))
							{
								var country = countryRepository.GetCountry(countryBasketInfo.IsoCountryCode);
								return new CountryBasket(basketInfo.Id, country);
							}
							else
							{
								throw new ApplicationException("There is no country basket with the \"" + basketInfo.Id + "\" ID.");
							}
						}
						case "region":
						{
							RegionBasketInfo regionBasketInfo;
							if (regionBasketInfos.TryGetValue(basketInfo.Id, out regionBasketInfo))
							{
								var result = this.xmlDeserializer.ReadBasket(basketInfo.Id, regionBasketInfo.DefinitionXml, countryRepository);
								return result;
							}
							else
							{
								throw new ApplicationException("There is no region basket with the \"" + basketInfo.Id + "\" ID.");
							}
						}
						default:
						{
							throw new ApplicationException("Unexpected basket type \"" + basketInfo.Type + "\".");
						}
					}
				});
				if (basketOpt == null) continue;
				baskets.Add(basketOpt);
			}


			var repository = new BasketRepository(baskets);
			return repository;
		}

		public void DropBasketRespoitory()
		{
			this.basketRepositoryStorage[BasketRepositoryStorageKey] = null;
		}

		public Boolean IsSecurityFromBasket(ISecurity security, IBasket basket)
		{
			var value = this.basketSecurityRelationshipInvestigator.IsSecurityFromBasketOnceResolved(security, basket);
			return value;
		}

        
    }
}
