using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using TopDown.Core.Persisting;
using TopDown.Core.ManagingSecurities;
using TopDown.Core.ManagingTargetingTypes;
using System.IO;
using System.Data.SqlClient;
using TopDown.Core.ManagingBaskets;
using TopDown.Core.ManagingPortfolios;
using Aims.Expressions;
using TopDown.Core.ManagingBenchmarks;
using TopDown.Core.ManagingCalculations;
using Aims.Core;

namespace TopDown.Core.ManagingBpst
{
    public class ModelManager
    {
        private ModelToJsonSerializer modelSerializer;
        private ModelFromJsonDeserializer modelDeserializer;
        private ModelBuilder modelBuilder;
        private ModelApplier modelApplier;
        private ModelValidator modelValidator;
        private BenchmarkInitializer benchmarkInitializer;
        private ModelChangeDetector modelChangeDetector;
        private RepositoryManager repositoryManager;

        [DebuggerStepThrough]
        public ModelManager(
            ModelToJsonSerializer modelSerializer,
            ModelBuilder modelBuilder,
            ModelFromJsonDeserializer modelDeserializer,
            ModelApplier modelApplier,
            ModelValidator modelValidator,
            BenchmarkInitializer benchmarkInitializer,
            ModelChangeDetector modelChangeDetector,
            RepositoryManager repositoryManager
        )
        {
            this.modelSerializer = modelSerializer;
            this.modelBuilder = modelBuilder;
            this.modelDeserializer = modelDeserializer;
            this.modelApplier = modelApplier;
            this.modelValidator = modelValidator;
            this.benchmarkInitializer = benchmarkInitializer;
            this.modelChangeDetector = modelChangeDetector;
            this.repositoryManager = repositoryManager;
        }

        
        public String SerializeToJson(RootModel root, CalculationTicket ticket)
        {
            var builder = new StringBuilder();
            using (var writer = new JsonWriter(builder.ToJsonTextWriter()))
            {
                writer.Write(delegate
                {
                    this.modelSerializer.SerializeRoot(root, ticket, writer);
                    writer.Write(this.modelChangeDetector.HasChanged(root), JsonNames.Unsaved);
                });
            }
            return builder.ToString();
        }

        
        public RootModel GetRootModel(
            Int32 targetingTypeGroupId,
            Int32 basketId,
            IDataManager manager
        )
        {
            var targetingTypeGroupRepository = this.repositoryManager.ClaimTargetingTypeGroupRepository(manager);
            var targetingTypeGroup = targetingTypeGroupRepository.GetTargetingTypeGroup(targetingTypeGroupId);

            var basketRepository = this.repositoryManager.ClaimBasketRepository(manager);
            var basket = basketRepository.GetBasket(basketId);

            var broadGlobalActivePortfolioIds = targetingTypeGroup.GetTargetingTypes()
                .SelectMany(x => x.BroadGlobalActivePortfolios)
                .Select(x => x.Id).Distinct();

            var ttgbsbvRepository = this.repositoryManager.CreateTargetingTypeGroupBasketSecurityBaseValueRepository(
                targetingTypeGroup,
                basket,
                manager
            );
            var bpstRepository = this.repositoryManager.CreateBasketPortfolioSecurityTargetRepository(
                basket.Id,
                broadGlobalActivePortfolioIds,
                manager
            );
			var securityRepository = this.repositoryManager.ClaimSecurityRepository(manager);
			var portfolioRepository = this.repositoryManager.ClaimPortfolioRepository(manager);
            var benchmarkDate = manager.GetLastestDateWhichBenchmarkDataIsAvialableOn();

            var core = this.GetCoreModel(
                targetingTypeGroup,
                basket,
				securityRepository,
                ttgbsbvRepository,
                bpstRepository,
				portfolioRepository
            );

            var latestBaseChangeset = manager.GetLatestTargetingTypeGroupBasketSecurityBaseValueChangeset();
            var latestPortfolioTargetChangeset = manager.GetLatestBasketPortfolioSecurityTargetChangeset();
            var result = new RootModel(
                latestBaseChangeset,
                latestPortfolioTargetChangeset,
                core,
                benchmarkDate
            );
            var benchmarkRepository = this.repositoryManager.ClaimBenchmarkRepository(manager, benchmarkDate);
            this.benchmarkInitializer.InitializeCore(result.Core, benchmarkRepository);

            return result;
        }

        
        public CoreModel GetCoreModel(
            TargetingTypeGroup targetingTypeGroup,
            IBasket basket,
            SecurityRepository securityRepository,
            TargetingTypeGroupBasketSecurityBaseValueRepository ttgbsbvRepository,
            BasketSecurityPortfolioTargetRepository bpstRepository,
			PortfolioRepository portfolioRepository
        )
        {

			var bgaPortfolios = targetingTypeGroup.GetBgaPortfolios();

            // both base values and target values can contribute to the list of securities
            var securityIds = ttgbsbvRepository
                .GetBaseValues(targetingTypeGroup, basket).Select(x => x.SecurityId)
                .Union(bpstRepository.GetTargets(basket).Select(x => x.SecurityId));

            var securityModels = new List<SecurityModel>();
            foreach (var securityId in securityIds)
            {
                var security = securityRepository.GetSecurity(securityId);
                var securityModel = this.CreateSecurity(
					targetingTypeGroup,
                    basket,
                    security,
                    bgaPortfolios,
                    ttgbsbvRepository,
                    bpstRepository,
					portfolioRepository
                );
                securityModels.Add(securityModel);
            }

            var portfolios = bgaPortfolios.Select(portfolio => {
				return new PortfolioModel(
					portfolio,
					this.modelBuilder.CreatePortfolioTargetTotalExpression(portfolio, securityModels)
				);
			}
            ).ToArray();

            var baseTotalExpression = this.modelBuilder.CreateBaseTotalExpression(securityModels);
            var benchmarkTotalExpression = this.modelBuilder.CreateBenchmarkTotalExpression(securityModels);
            var baseActiveTotalExpression = this.modelBuilder.CreateBaseActiveTotalExpression(securityModels);
            var core = new CoreModel(
                targetingTypeGroup,
                basket,
                portfolios,
                securityModels,
                baseTotalExpression,
                benchmarkTotalExpression,
                baseActiveTotalExpression
            );
            return core;
        }

        
        public SecurityModel CreateSecurity(
			TargetingTypeGroup targetingTypeGroup,
            IBasket basket,
            ISecurity security,
            IEnumerable<BroadGlobalActivePortfolio> bgaPortfolios,
            TargetingTypeGroupBasketSecurityBaseValueRepository ttgbsbvRepository,
            BasketSecurityPortfolioTargetRepository bsptRepository,
			PortfolioRepository portfolioRepository
        )
        {
            var portfolioTargets = new List<PortfolioTargetModel>();

            var targetsOfSecurity = bsptRepository.GetTargets(basket, security);
            var targetsByPortfolio = targetsOfSecurity.ToDictionary(x => x.PortfolioId);
			foreach (var bgaPortfolio in bgaPortfolios)
            {
				var portfolioTargetExpression = this.modelBuilder.CreatePortfolioTargetExpression(bgaPortfolio.Name);
				var portfolioModel = new PortfolioTargetModel(bgaPortfolio, portfolioTargetExpression);

                BasketPortfolioSecurityTargetInfo target;
				if (targetsByPortfolio.TryGetValue(bgaPortfolio.Id, out target))
                {
                    // there is a target for the given portfolio
                    portfolioModel.Target.InitialValue = target.Target;
                }

                portfolioTargets.Add(portfolioModel);
            }

            var baseExpression = this.modelBuilder.CreateBaseExpression();

			var baseInfoOpt = ttgbsbvRepository.TryGetBaseValue(targetingTypeGroup, basket, security);
            if (baseInfoOpt != null)
            {
                // there is a base value for this security already defined
                baseExpression.InitialValue = baseInfoOpt.BaseValue;
            }
            else
            {
                // it will have a default base value which is null)
            }

            var benchmarkExpression = this.modelBuilder.CreateBenchmarkExpression();
            var baseActiveExpression = this.modelBuilder.CreateBaseActiveExpression(baseExpression, benchmarkExpression);
            var securityModel = new SecurityModel(
                security,
                baseExpression,
                benchmarkExpression,
                portfolioTargets,
                baseActiveExpression
            );
            return securityModel;
        }

        
        public RootModel DeserializeFromJson(
            String bpstModelAsJson,
            SecurityRepository securityRepository,
            TargetingTypeGroupRepository targetingTypeGroupRepository,
            BasketRepository basketRepository,
			PortfolioRepository portfolioRepository,
            IOnDemand<IDataManager> ondemandManager
        )
        {
            using (var reader = new JsonReader(new Newtonsoft.Json.JsonTextReader(new StringReader(bpstModelAsJson))))
            {
                var result = reader.Read(delegate
                {
                    return this.modelDeserializer.DeserializeRoot(
                        reader,
                        securityRepository,
                        targetingTypeGroupRepository,
                        basketRepository,
						portfolioRepository,
						this.repositoryManager,
                        ondemandManager
                    );
                });
                return result;
            }
        }

        
        public IEnumerable<IValidationIssue> ApplyIfValid(RootModel model, String username, SqlConnection connection, CalculationTicket ticket, ref CalculationInfo calculationInfo)
        {
            var issues = this.Validate(model, ticket);
            if (issues.Any()) return issues;
            try
            {
                this.modelApplier.Apply(model, username, connection, ref calculationInfo);
                return No.ValidationIssues;
            }
            catch (ValidationException exception)
            {
                return new IValidationIssue[] { exception.Issue };
            }
        }

        public IEnumerable<IValidationIssue> Validate(RootModel model, CalculationTicket ticket)
        {
            var issues = this.modelValidator.ValidateRoot(model, ticket);
            return issues;
        }
		public IEnumerable<IValidationIssue> Validate(CoreModel model, CalculationTicket ticket)
		{
			var issues = this.modelValidator.ValidateCore(model, ticket);
			return issues;
		}
    }
}
