using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopDown.Core.Persisting;
using System.Xml;
using System.IO;
using TopDown.Core.ManagingBaskets;
using TopDown.Core.ManagingCountries;
using Aims.Core;

namespace TopDown.Core.ManagingTaxonomies
{
    public class TaxonomyManager
    {
        public const String TaxonomyRepositoryStorageKey = "TaxonomyRepository";

        private IStorage<TaxonomyRepository> taxonomyRepositoryStorage;
        private InfoDeserializer deserializer;

        public TaxonomyManager(
            IStorage<TaxonomyRepository> taxonomyRepositoryStorage,
            InfoDeserializer deserializer
        )
        {
            this.taxonomyRepositoryStorage = taxonomyRepositoryStorage;
            this.deserializer = deserializer;
        }

        public TaxonomyRepository ClaimTaxonomyRepository(
			IOnDemand<IDataManager> ondemandManager,
			Func<BasketRepository> ondemandBasketRepository,
			Func<CountryRepository> ondemandCountryRepository
		)
        {
            return this.taxonomyRepositoryStorage.Claim(TaxonomyRepositoryStorageKey, delegate
            {
				return this.CreateTaxonomyRepository(
					ondemandManager.Claim(),
					ondemandBasketRepository(),
					ondemandCountryRepository()
				);
            });
        }

        public TaxonomyRepository ClaimTaxonomyRepository(
            IDataManager manager,
            Func<BasketRepository> ondemandBasketRepository,
            Func<CountryRepository> ondemandCountryRepository
        )
        {
            return this.taxonomyRepositoryStorage.Claim(TaxonomyRepositoryStorageKey, delegate
            {
                return this.CreateTaxonomyRepository(
                    manager,
                    ondemandBasketRepository(),
                    ondemandCountryRepository()
                );
            });
        }

        public TaxonomyRepository ClaimTaxonomyRepository(
			IOnDemand<IDataManager> ondemandManager,
			BasketRepository basketRepository,
			CountryRepository countryRepository
		)
        {
            return this.taxonomyRepositoryStorage.Claim(TaxonomyRepositoryStorageKey, delegate
            {
				return this.CreateTaxonomyRepository(ondemandManager.Claim(), basketRepository, countryRepository);
            });
        }

        private TaxonomyRepository CreateTaxonomyRepository(
			IDataManager manager,
			BasketRepository basketRepository,
			CountryRepository countryRepository
		)
        {
            var taxonomyInfos = manager.GetAllTaxonomies();
			var taxonomies = taxonomyInfos.Select(x => this.deserializer.DeserializeTaxonomy(x, basketRepository, countryRepository));
            var result = new TaxonomyRepository(taxonomies);
            return result;
        }

		public void DropRepository()
		{
			this.taxonomyRepositoryStorage[TaxonomyRepositoryStorageKey] = null;
		}
	}
}
