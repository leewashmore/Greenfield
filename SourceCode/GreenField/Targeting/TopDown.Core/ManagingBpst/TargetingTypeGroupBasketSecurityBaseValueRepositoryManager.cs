using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopDown.Core.Persisting;
using TopDown.Core.ManagingTargetingTypes;
using TopDown.Core.ManagingBaskets;

namespace TopDown.Core.ManagingBpst
{
    public class TargetingTypeGroupBasketSecurityBaseValueRepositoryManager
    {
        public const String StorageKey = "TargetingTypeGroupBasketSecurityBaseValueRepository";
        private IStorage<TargetingTypeGroupBasketSecurityBaseValueRepository> storage;
        
        public TargetingTypeGroupBasketSecurityBaseValueRepositoryManager(
            IStorage<TargetingTypeGroupBasketSecurityBaseValueRepository> storage
        )
        {
            this.storage = storage;
        }

        public void DropRepository()
        {
            this.storage[StorageKey] = null;
        }

        public TargetingTypeGroupBasketSecurityBaseValueRepository ClaimRepository(IDataManager manager)
        {
            return this.storage.Claim(StorageKey, delegate
            {
                return this.CreateRepository(manager);
            });
        }

        public TargetingTypeGroupBasketSecurityBaseValueRepository CreateRepository(TargetingTypeGroup targetingTypeGroup, IBasket basket, IDataManager manager)
        {
            var baseValues = manager.GetTargetingTypeGroupBasketSecurityBaseValues(targetingTypeGroup.Id, basket.Id);
            var result = new TargetingTypeGroupBasketSecurityBaseValueRepository(baseValues);
            return result;
        }

        public TargetingTypeGroupBasketSecurityBaseValueRepository CreateRepository(IDataManager manager)
        {
            var baseValues = manager.GetTargetingTypeGroupBasketSecurityBaseValues();
            var result = new TargetingTypeGroupBasketSecurityBaseValueRepository(baseValues);
            return result;
        }
    }
}
