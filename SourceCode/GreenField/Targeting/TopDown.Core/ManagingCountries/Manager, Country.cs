using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using TopDown.Core.Persisting;
using TopDown.Core.Sql;

namespace TopDown.Core.ManagingCountries
{
	public class CountryManager
	{
		public const String CountryRepositoryStorageKey = "CountryRepository";
		private IStorage<CountryRepository> countryRepositoryStorage;
		
		[DebuggerStepThrough]
		public CountryManager(IStorage<CountryRepository> countryRepositoryStorage)
		{
			this.countryRepositoryStorage = countryRepositoryStorage;
		}

        public CountryRepository ClaimCountryRepository(IOnDamand<IDataManager> ondemandManager)
        {
            return this.countryRepositoryStorage.Claim(CountryRepositoryStorageKey, delegate
            {
                var manager = ondemandManager.Claim();
                return this.CreateCountryRepository(manager);
            });
        }
		public CountryRepository ClaimCountryRepository(IDataManager manager)
		{
			return this.countryRepositoryStorage.Claim(CountryRepositoryStorageKey, delegate
			{
				return this.CreateCountryRepository(manager);
			});
		}
		protected CountryRepository CreateCountryRepository(IDataManager manager)
		{
            var countryInfos = manager.GetAllCountries();
            return new CountryRepository(countryInfos);
		}

        public void DropCountryRepository()
        {
            this.countryRepositoryStorage[CountryRepositoryStorageKey] = null;
        }

		
	}
}
