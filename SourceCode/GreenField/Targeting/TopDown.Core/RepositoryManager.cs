using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using TopDown.Core.Persisting;
using TopDown.Core.Gadgets.PortfolioPicker;
using TopDown.Core.ManagingTargetingTypes;
using TopDown.Core.ManagingBaskets;
using Aims.Core;

namespace TopDown.Core
{
    public class RepositoryManager : Aims.Core.RepositoryManager
    {
        private IMonitor monitor;
        private ManagingBaskets.BasketManager basketManager;
        private ManagingTargetingTypes.TargetingTypeManager targetingTypeManager;
        private ManagingTaxonomies.TaxonomyManager taxonomyManager;
        private ManagingBenchmarks.BenchmarkManager benchmarkManager;
        private ManagingPst.RepositoryManager portfolioSecurityTargerManager;
        private ManagingBpst.BasketSecurityPortfolioTargetRepositoryManager bpstManager;
        private ManagingBpst.TargetingTypeGroupBasketSecurityBaseValueRepositoryManager ttgbsbvrManager;

        [DebuggerStepThrough]
        public RepositoryManager(
            IMonitor monitor,
            ManagingBaskets.BasketManager basketManager,
            ManagingTargetingTypes.TargetingTypeManager targetingTypeManager,
            CountryManager countryManager,
            ManagingTaxonomies.TaxonomyManager taxonomyManager,
            SecurityManager securityManager,
            PortfolioManager portfolioManager,
            ManagingBenchmarks.BenchmarkManager benchmarkManager,
            ManagingPst.RepositoryManager portfolioSecurityTargerManager,
            ManagingBpst.BasketSecurityPortfolioTargetRepositoryManager bpstManager,
            ManagingBpst.TargetingTypeGroupBasketSecurityBaseValueRepositoryManager ttgbsbvrManager,
            IssuerManager issuerManager
        )
            : base(monitor, countryManager, securityManager, portfolioManager, issuerManager)
        {
            this.monitor = monitor;
            this.taxonomyManager = taxonomyManager;
            this.targetingTypeManager = targetingTypeManager;
            this.basketManager = basketManager;
            this.benchmarkManager = benchmarkManager;
            this.portfolioSecurityTargerManager = portfolioSecurityTargerManager;
            this.bpstManager = bpstManager;
            this.ttgbsbvrManager = ttgbsbvrManager;
        }


        public ManagingBaskets.BasketRepository ClaimBasketRepository(IOnDemand<IDataManager> ondemandManager)
        {
            return this.basketManager.ClaimBasketRepository(ondemandManager, delegate
            {
                return this.ClaimCountryRepository(ondemandManager);
            },
            monitor);
        }
        public ManagingBaskets.BasketRepository ClaimBasketRepository(IDataManager manager)
        {
            return this.basketManager.ClaimBasketRepository(manager, delegate
            {
                return this.ClaimCountryRepository(manager);
            },
            monitor);
        }

        public ManagingTargetingTypes.TargetingTypeGroupRepository ClaimTargetingTypeGroupRepository(IOnDemand<IDataManager> ondemandManager)
        {
            return this.targetingTypeManager.ClaimTargetingTypeGroupRepository(ondemandManager, delegate
            {
                return this.ClaimTargetingTypeRepository(ondemandManager);
            });
        }
        public ManagingTargetingTypes.TargetingTypeGroupRepository ClaimTargetingTypeGroupRepository(IDataManager manager)
        {
            return this.targetingTypeManager.ClaimTargetingTypeGroupRepository(manager, delegate
            {
                return this.ClaimTargetingTypeRepository(manager);
            });
        }

        public ManagingTargetingTypes.TargetingTypeRepository ClaimTargetingTypeRepository(IOnDemand<IDataManager> ondemandManager)
        {
            return this.targetingTypeManager.ClaimTargetingTypeRepository(ondemandManager, delegate
            {
                return this.ClaimTaxonomyRepository(ondemandManager);
            },
            delegate
            {
                return this.ClaimPortfolioRepository(ondemandManager);
            });
        }
        public ManagingTargetingTypes.TargetingTypeRepository ClaimTargetingTypeRepository(IDataManager manager)
        {
            return this.targetingTypeManager.ClaimTargetingTypeRepository(manager, delegate
            {
                return this.ClaimTaxonomyRepository(manager);
            },
            delegate
            {
                return this.ClaimPortfolioRepository(manager);
            });
        }



        public ManagingTaxonomies.TaxonomyRepository ClaimTaxonomyRepository(IOnDemand<IDataManager> ondemandManager)
        {
            return this.taxonomyManager.ClaimTaxonomyRepository(ondemandManager, delegate
            {
                return this.ClaimBasketRepository(ondemandManager);
            },
            delegate
            {
                return this.ClaimCountryRepository(ondemandManager);
            });
        }
        public ManagingTaxonomies.TaxonomyRepository ClaimTaxonomyRepository(IDataManager manager)
        {
            return this.taxonomyManager.ClaimTaxonomyRepository(manager, delegate
            {
                return this.ClaimBasketRepository(manager);
            },
            delegate
            {
                return this.ClaimCountryRepository(manager);
            });
        }


        public ManagingBenchmarks.BenchmarkRepository ClaimBenchmarkRepository(IOnDemand<IDataManager> ondemandManager, DateTime benchmarkDate)
        {
            return this.benchmarkManager.ClaimBenchmarkRepository(ondemandManager, benchmarkDate);

        }
        public ManagingBenchmarks.BenchmarkRepository ClaimBenchmarkRepository(IDataManager manager, DateTime benchmarkDate)
        {
            return this.benchmarkManager.ClaimBenchmarkRepository(manager, benchmarkDate);
        }


        public ManagingPst.PortfolioSecurityTargetRepository ClaimPortfolioSecurityTargetRepository(
            BuPortfolioSecurityTargetChangesetInfo latestPstChangesetInfo,
            IDataManager manager
        )
        {
            var result = this.portfolioSecurityTargerManager.ClaimRepository(
                latestPstChangesetInfo,
                manager
            );
            return result;
        }

        public ManagingBpst.BasketSecurityPortfolioTargetRepository CreateBasketPortfolioSecurityTargetRepository(
            Int32 basketId,
            IEnumerable<String> broadGlobalActivePortfolioIds,
            IDataManager manager
        )
        {
            var result = this.bpstManager.CreateRepository(
                basketId,
                broadGlobalActivePortfolioIds,
                manager
            );
            return result;
        }
        public ManagingBpst.BasketSecurityPortfolioTargetRepository CliamBasketPortfolioSecurityTargetRepository(
            IDataManager manager
        )
        {
            var result = this.bpstManager.ClaimRepository(manager);
            return result;
        }

        public ManagingBpst.TargetingTypeGroupBasketSecurityBaseValueRepository CreateTargetingTypeGroupBasketSecurityBaseValueRepository(
            TargetingTypeGroup targetingTypeGroup,
            IBasket basket,
            IDataManager manager
        )
        {
            var result = this.ttgbsbvrManager.CreateRepository(
                targetingTypeGroup,
                basket,
                manager
            );
            return result;
        }

        public ManagingBpst.TargetingTypeGroupBasketSecurityBaseValueRepository ClaimTargetingTypeGroupBasketSecurityBaseValueRepository(IDataManager manager)
        {
            var result = this.ttgbsbvrManager.ClaimRepository(manager);
            return result;
        }



        private void DropTaxonomyRepository()
        {
            this.taxonomyManager.DropRepository();
        }
        public void DropTargetingTypeRepository()
        {
            this.targetingTypeManager.DropTargetingTypeRepository();
        }
        public void DropTargetingTypeGroupRepository()
        {
            this.targetingTypeManager.DropTargetingTypeGroupRepository();
        }
        public void DropBasketRespoitory()
        {
            this.basketManager.DropBasketRespoitory();
        }
        public void DropPortfolioSecurityTargetRepository()
        {
            this.portfolioSecurityTargerManager.DropRepository();
        }
        public void DropBenchmarkRepository()
        {
            this.benchmarkManager.DropRepository();
        }
        public void DropBasketSecurityPortfolioTargetRepository()
        {
            this.bpstManager.DropRepository();
        }
        public void DropTargetingTypeGroupBasketSecurityBaseValueRepository()
        {
            this.ttgbsbvrManager.DropRepository();
        }

        public override void DropEverything()
        {
            base.DropEverything();
            this.DropTaxonomyRepository();
            this.DropTargetingTypeRepository();
            this.DropTargetingTypeGroupRepository();
            this.DropCountryRepository();
            this.DropBasketRespoitory();
            this.DropPortfolioRepository();
            this.DropPortfolioSecurityTargetRepository();
            this.DropBenchmarkRepository();
            this.DropBasketSecurityPortfolioTargetRepository();
            this.DropTargetingTypeGroupBasketSecurityBaseValueRepository();
        }



    }
}
