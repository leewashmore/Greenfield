using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aims.Core.Persisting;
using System.Diagnostics;

namespace Aims.Core
{
    public class IssuerManager
    {
        public const String IssuerRepositoryStorageKey = "IssuerRepository";
        private IStorage<IssuerRepository> issuerRepositoryStorage;
        private IMonitor monitor;

        [DebuggerStepThrough]
        public IssuerManager(IMonitor monitor, IStorage<IssuerRepository> issuerRepositoryStorage)
        {
            this.monitor = monitor;
            this.issuerRepositoryStorage = issuerRepositoryStorage;
        }

        public IssuerRepository ClaimIssuerRepository(IDataManager manager)
        {
            return this.issuerRepositoryStorage.Claim(IssuerRepositoryStorageKey, delegate
            {
                return this.CreateIssuerRepository(manager);
            });
        }

        public IssuerRepository ClaimIssuerRepository(IOnDamand<IDataManager> ondemandManager)
        {
            return this.issuerRepositoryStorage.Claim(IssuerRepositoryStorageKey, delegate
            {
                var manager = ondemandManager.Claim();
                return this.CreateIssuerRepository(manager);
            });
        }

        public IssuerRepository ClaimIssuerRepository(IEnumerable<SecurityInfo> securities)
        {
            return this.issuerRepositoryStorage.Claim(IssuerRepositoryStorageKey, delegate
            {
                return this.CreateIssuerRepository(securities);
            });
        }

        public IssuerRepository CreateIssuerRepository(IDataManager manager)
        {
            var securities = manager.GetAllSecurities();
            var repository = CreateIssuerRepository(securities);
            return repository;
        }

        public IssuerRepository CreateIssuerRepository(IEnumerable<SecurityInfo> securities)
        {
            var repository = new IssuerRepository(this.monitor, securities);
            return repository;
        }

        public void DropIssuerRepository()
        {
            this.issuerRepositoryStorage[IssuerRepositoryStorageKey] = null;
        }
    }
}
