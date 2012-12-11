using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Aims.Core.Persisting;

namespace Aims.Core
{
    public class SecurityManager
    {
        public const String SecurityManagerCacheKey = "SecurityManager";
        private IStorage<SecurityRepository> securityRepositoryStorage;
        private IMonitor monitor;

        public SecurityManager(
            IStorage<SecurityRepository> securityRepositoryStorage,
            IMonitor monitor
        )
        {
            this.securityRepositoryStorage = securityRepositoryStorage;
            this.monitor = monitor;
        }

        public SecurityRepository ClaimSecurityRepository(
            IOnDemand<IDataManager> ondemandManager,
            Func<CountryRepository> ondemandCountryRepository
        )
        {
            return this.securityRepositoryStorage.Claim(SecurityManagerCacheKey, delegate
            {
                var securities = ondemandManager.Claim().GetAllSecurities();
                var repository = this.CreateSecurityRepository(ondemandCountryRepository, securities);
                return repository;
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
                var repository = new SecurityRepository(
                    this.monitor,
                    securities,
                    ondemandCountryRepository()
                );
                return repository;
            });
        }

        public SecurityRepository ClaimSecurityRepository(
            Func<IEnumerable<SecurityInfo>> ondemandSecurities,
            Func<CountryRepository> ondemandCountryRepository)
        {
            return this.securityRepositoryStorage.Claim(SecurityManagerCacheKey, delegate
            {
                var securities = ondemandSecurities();
                var countryRepository = ondemandCountryRepository();
                var repository = new SecurityRepository(
                    this.monitor,
                    securities,
                    countryRepository
                );
                return repository;
            });
        }

        public SecurityRepository CreateSecurityRepository(
            Func<CountryRepository> ondemandCountryRepository,
            IEnumerable<SecurityInfo> securities)
        {
            var repository = new SecurityRepository(
                this.monitor,
                securities,
                ondemandCountryRepository()
            );
            return repository;
        }


        public void DropSecurityRepository()
        {
            this.securityRepositoryStorage[SecurityManagerCacheKey] = null;
        }
    }
}
