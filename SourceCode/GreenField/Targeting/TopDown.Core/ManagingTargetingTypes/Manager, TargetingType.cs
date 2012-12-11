using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using TopDown.Core.Sql;
using TopDown.Core.Persisting;
using TopDown.Core.ManagingTaxonomies;
using Aims.Core;

namespace TopDown.Core.ManagingTargetingTypes
{
    public class TargetingTypeManager
    {
        public const String TargetingTypeRepositoryStorageKey = "TargetingTypeRepository";
        public const String TargetingTypeGroupRepositoryStorageKey = "TargetingTypeGroupRepository";

        private InfoDeserializer deserializer;
        private IStorage<TargetingTypeRepository> targetingTypeRepositoryStorage;
        private IStorage<TargetingTypeGroupRepository> targetingTypeGroupRepositoryStorage;

        [DebuggerStepThrough]
        public TargetingTypeManager(
            InfoDeserializer deserializer,
            IStorage<TargetingTypeRepository> targetingTypeRepositoryStorage,
            IStorage<TargetingTypeGroupRepository> targetingTypeGroupRepositoryStorage
        )
        {
            this.deserializer = deserializer;
            this.targetingTypeRepositoryStorage = targetingTypeRepositoryStorage;
            this.targetingTypeGroupRepositoryStorage = targetingTypeGroupRepositoryStorage;
        }

        public TargetingTypeRepository ClaimTargetingTypeRepository(
			IOnDamand<IDataManager> ondemandManager,
			Func<TaxonomyRepository> ondemandTaxonomyRepository,
			Func<PortfolioRepository> ondemandPortfolioRepository
		)
        {
            return this.targetingTypeRepositoryStorage.Claim(TargetingTypeRepositoryStorageKey, delegate
            {
                return this.CreateTargetingTypeRepository(
					ondemandManager.Claim(),
					ondemandTaxonomyRepository(),
					ondemandPortfolioRepository()
				);
            });
        }


        public TargetingTypeRepository ClaimTargetingTypeRepository(
           IDataManager manager,
           Func<TaxonomyRepository> ondemandTaxonomyRepository,
           Func<PortfolioRepository> ondemandPortfolioRepository
       )
        {
            return this.targetingTypeRepositoryStorage.Claim(TargetingTypeRepositoryStorageKey, delegate
            {
                return this.CreateTargetingTypeRepository(
                    manager,
                    ondemandTaxonomyRepository(),
                    ondemandPortfolioRepository()
                );
            });
        }
		
		
		public TargetingTypeRepository ClaimTargetingTypeRepository(
			IOnDamand<IDataManager> ondemandManager,
			TaxonomyRepository taxonomyRepository,
			PortfolioRepository portfolioRepository
		)
        {
            return this.targetingTypeRepositoryStorage.Claim(TargetingTypeRepositoryStorageKey, delegate
            {
                return this.CreateTargetingTypeRepository(
					ondemandManager.Claim(),
					taxonomyRepository,
					portfolioRepository
				);
            });
        }
        protected TargetingTypeRepository CreateTargetingTypeRepository(
			IDataManager manager,
			TaxonomyRepository taxonomyRepository,
			PortfolioRepository portfolioRepository
		)
        {
            var targetingTypeInfos = manager.GetAllTargetingTypes();
            var whateverPortfolioCompositionInfos = manager.GetAllTargetingTypePortfolio();
            var targetingTypes = targetingTypeInfos
                .Select(x => this.deserializer.DeserializeToTargetingType(
					x, taxonomyRepository,
					whateverPortfolioCompositionInfos,
					portfolioRepository
				));

            var result = new TargetingTypeRepository(targetingTypes);
            return result;
        }

		public TargetingTypeGroupRepository ClaimTargetingTypeGroupRepository(IDataManager manager, Func<TargetingTypeRepository> ondemandTargetingTypeRepository)
		{
			return this.targetingTypeGroupRepositoryStorage.Claim(TargetingTypeGroupRepositoryStorageKey, delegate
			{
				return this.CreateTargetingTypeGroupRepository(manager, ondemandTargetingTypeRepository());
			});
		}

		public TargetingTypeGroupRepository ClaimTargetingTypeGroupRepository(IOnDamand<IDataManager> ondemandManager, Func<TargetingTypeRepository> ondemandTargetingTypeRepository)
        {
            return this.targetingTypeGroupRepositoryStorage.Claim(TargetingTypeGroupRepositoryStorageKey, delegate
            {
                return this.CreateTargetingTypeGroupRepository(ondemandManager.Claim(), ondemandTargetingTypeRepository());
            });
        }

        protected TargetingTypeGroupRepository CreateTargetingTypeGroupRepository(IDataManager manager, TargetingTypeRepository targetingTypeRepository)
        {
            var targetingTypeGroupInfos = manager.GetAllTargetingTypeGroups();
            var result = new TargetingTypeGroupRepository(targetingTypeGroupInfos, targetingTypeRepository);
            return result;
        }

        public void DropTargetingTypeRepository()
        {
            this.targetingTypeRepositoryStorage[TargetingTypeRepositoryStorageKey] = null;
        }
        public void DropTargetingTypeGroupRepository()
        {
            this.targetingTypeGroupRepositoryStorage[TargetingTypeGroupRepositoryStorageKey] = null;
        }


       
    }
}
