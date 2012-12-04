using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopDown.Core.Persisting;
using System.Diagnostics;

namespace TopDown.Core.ManagingBpst
{
    public class BasketSecurityPortfolioTargetRepositoryManager
    {
        public const String BasketSecurityPortfolioTargetRepositoryStorageKey = "BasketSecurityPortfolioTargetRepository";
        private IStorage<BasketSecurityPortfolioTargetRepository> storage;
        
        [DebuggerStepThrough]
        public BasketSecurityPortfolioTargetRepositoryManager(
            IStorage<BasketSecurityPortfolioTargetRepository> storage
        )
        {
            this.storage = storage;
        }

        public BasketSecurityPortfolioTargetRepository CreateRepository(
            IEnumerable<BasketPortfolioSecurityTargetInfo> targetInfos
        )
        {
            var result = new BasketSecurityPortfolioTargetRepository(targetInfos);
            return result;
        }

        public BasketSecurityPortfolioTargetRepository CreateRepository(
            Int32 basketId,
            IEnumerable<String> broadGlobalActivePortfolioIds,
            IDataManager manager
        )
        {
            var targetInfos = manager.GetBasketProtfolioSecurityTargets(basketId, broadGlobalActivePortfolioIds);
            var result = this.CreateRepository(targetInfos);
            return result;
        }

        public BasketSecurityPortfolioTargetRepository CreateRepository(IDataManager manager)
        {
            var targets = manager.GetBasketProtfolioSecurityTargets();
            var result = new BasketSecurityPortfolioTargetRepository(targets);
            return result;
        }


        public void DropRepository()
        {
            this.storage[BasketSecurityPortfolioTargetRepositoryStorageKey] = null;
        }

        public BasketSecurityPortfolioTargetRepository ClaimRepository(IDataManager manager)
        {
            return this.storage.Claim(BasketSecurityPortfolioTargetRepositoryStorageKey, delegate
            {
                return this.CreateRepository(manager);
            });
        }

    }
}
