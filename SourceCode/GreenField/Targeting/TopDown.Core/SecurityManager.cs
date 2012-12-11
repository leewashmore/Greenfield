using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using TopDown.Core.Persisting;
using TopDown.Core.Sql;
using TopDown.Core.ManagingCountries;
using Aims.Core;

namespace TopDown.Core.ManagingSecurities
{
	public class SecurityManager
	{
		public const String SecurityManagerCacheKey = "SecurityManager";
        private IStorage<SecurityRepository> securityRepositoryStorage;
		private SecurityToJsonSerializer securitySerializer;
		private IMonitor monitor;

		public SecurityManager(
			IStorage<SecurityRepository> securityRepositoryStorage,
			SecurityToJsonSerializer securitySerializer,
			IMonitor monitor
		)
		{
            this.securityRepositoryStorage = securityRepositoryStorage;
			this.securitySerializer = securitySerializer;
			this.monitor = monitor;
		}

        public SecurityRepository ClaimSecurityRepository(
			IOnDamand<IDataManager> ondemandManager,
			Func<CountryRepository> ondemandCountryRepository
		)
		{
            return this.securityRepositoryStorage.Claim(SecurityManagerCacheKey, delegate
            {
                var securities = ondemandManager.Claim().GetAllSecurities();
				return new SecurityRepository(
					this.monitor,
					securities,
					ondemandCountryRepository()
				);
            });
		}

        public SecurityRepository ClaimSecurityRepository(
			IDataManager manager,
			Func<CountryRepository> ondemandCountryRepository
		)
        {
            return this.securityRepositoryStorage.Claim(SecurityManagerCacheKey, delegate
            {
                var securities = manager.GetAllSecurities();
				return new SecurityRepository(
					this.monitor,
					securities,
					ondemandCountryRepository()
				);
            });
        }
    }
}
