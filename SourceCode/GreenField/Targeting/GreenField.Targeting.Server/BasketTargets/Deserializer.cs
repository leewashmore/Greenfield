using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Core = TopDown.Core.ManagingBpst;
using TopDown.Core.Persisting;
using TopDown.Core.ManagingBaskets;

namespace GreenField.Targeting.Server.BasketTargets
{
    public class Deserializer
    {
        private Server.Deserializer deserializer;
        private Core.ModelBuilder modelBuilder;
        private Core.BenchmarkInitializer benchmarkInitializer;

        [DebuggerStepThrough]
        public Deserializer(
            Server.Deserializer deserializer,
            Core.ModelBuilder modelBuilder,
            Core.BenchmarkInitializer benchmarkInitializer
        )
        {
            this.deserializer = deserializer;
            this.modelBuilder = modelBuilder;
            this.benchmarkInitializer = benchmarkInitializer;
        }

        public Core.RootModel DeserializeRoot(RootModel model)
        {
            var targetingTypeGroup = this.deserializer.DeserializeTargetingTypeGroup(model.TargetingTypeGroup);
            var benchmarkRepository = this.deserializer.ClaimBenchmarkRepository(model.BenchmarkDate);
            var basket = this.deserializer.DeserializeBasket(model.Basket.Id);
            var securities = this.DeserializeSecurities(model.Securities).ToList();
            if (model.SecurityToBeAddedOpt != null)
            {
                var security = this.DeserializeAdditionalSecurity(
                    basket,
                    model.SecurityToBeAddedOpt,
                    targetingTypeGroup,
                    benchmarkRepository
                );
                securities.Add(security);
            }
            var baseTotalExpression = this.modelBuilder.CreateBaseTotalExpression(securities);
            var benchmarkTotalExpression = this.modelBuilder.CreateBenchmarkTotalExpression(securities);
            var baseActiveTotalExpression = this.modelBuilder.CreateBaseActiveTotalExpression(securities);
            var core = new Core.CoreModel(
                targetingTypeGroup,
                basket,
                this.DeserializePortfolios(model.Portfolios, securities),
                securities,
                baseTotalExpression,
                benchmarkTotalExpression,
                baseActiveTotalExpression
            );

            var result = new Core.RootModel(
                this.DeserializeTargetingTypeGroupBasketSecurityBaseValueChangeset(model.LatestBaseChangeset),
                this.DeserializeBasketPortfolioSecurityTargetChangesetInfo(model.LatestPortfolioTargetChangeset),
                core,
                model.BenchmarkDate
            );
            return result;
        }

        private Core.SecurityModel DeserializeAdditionalSecurity(
            IBasket basket,
            Aims.Data.Server.SecurityModel securityModel,
            TopDown.Core.ManagingTargetingTypes.TargetingTypeGroup targetingTypeGroup,
            TopDown.Core.ManagingBenchmarks.BenchmarkRepository benchmarkRepository
        )
        {
                var baseExpression = this.modelBuilder.CreateBaseExpression();
                var benchmarkExpression = this.modelBuilder.CreateBenchmarkExpression();
                var baseActiveExpression = this.modelBuilder.CreateBaseActiveExpression(baseExpression, benchmarkExpression);
                var result = new Core.SecurityModel(
                    this.deserializer.DeserializeSecurity(securityModel),
                    baseExpression,
                    benchmarkExpression,
                    targetingTypeGroup.GetBgaPortfolios().Select(bgaPortfolio => new Core.PortfolioTargetModel(
                        bgaPortfolio,
                        this.modelBuilder.CreatePortfolioTargetExpression(bgaPortfolio.Name))
                    ).ToList(),
                    baseActiveExpression
                );
                if (!String.IsNullOrWhiteSpace(targetingTypeGroup.BenchmarkIdOpt))
                {
                    this.benchmarkInitializer.InitializeSecurity(basket, targetingTypeGroup.BenchmarkIdOpt, result, benchmarkRepository);
                }
            return result;
        }

        protected IEnumerable<Core.PortfolioModel> DeserializePortfolios(IEnumerable<PortfolioModel> models, IEnumerable<Core.SecurityModel> securities)
        {
            var result = models.Select(x => this.DeserializePortfolio(x, securities)).ToList();
            return result;
        }

        protected Core.PortfolioModel DeserializePortfolio(PortfolioModel model, IEnumerable<Core.SecurityModel> securities)
        {
            var broadGlobalActiveProfolio = this.deserializer.DeserializeBroadGlobalActivePorfolio(model.BroadGlobalActivePortfolio);
            var portfolioTargetTotalExpression = this.modelBuilder.CreatePortfolioTargetTotalExpression(broadGlobalActiveProfolio, securities);
            var result = new Core.PortfolioModel(
                broadGlobalActiveProfolio,
                portfolioTargetTotalExpression
            );
            return result;
        }

        protected IEnumerable<Core.SecurityModel> DeserializeSecurities(IEnumerable<SecurityModel> models)
        {
            var result = models.Select(x => this.DeserializeSecurity(x)).ToList();
            return result;
        }

        protected Core.SecurityModel DeserializeSecurity(SecurityModel model)
        {
            var baseExpression = this.modelBuilder.CreateBaseExpression();
            this.deserializer.PopulateEditableExpression(baseExpression, model.Base);
            var benchmarkExpression = this.modelBuilder.CreateBenchmarkExpression();
            this.deserializer.PopulateUnchangableExpression(benchmarkExpression, model.Benchmark);

            var portfolioTargets = this.DeserializePortfolioTargets(model.PortfolioTargets);
            var baseActiveExpression = this.modelBuilder.CreateBaseActiveExpression(baseExpression, benchmarkExpression);
            var result = new Core.SecurityModel(
                this.deserializer.DeserializeSecurity(model.Security),
                baseExpression,
                benchmarkExpression,
                portfolioTargets,
                baseActiveExpression
            );
            return result;
        }

        protected IEnumerable<Core.PortfolioTargetModel> DeserializePortfolioTargets(IEnumerable<PortfolioTargetModel> models)
        {
            var result = models.Select(x => this.DeserializePortfolioTarget(x)).ToList();
            return result;
        }

        private Core.PortfolioTargetModel DeserializePortfolioTarget(PortfolioTargetModel model)
        {
            var broadGlobalActivePorfolio = this.deserializer.DeserializeBroadGlobalActivePorfolio(model.BroadGlobalActivePortfolio);
            var porfolioTargetExpression = this.modelBuilder.CreatePortfolioTargetExpression(broadGlobalActivePorfolio.Name);
            this.deserializer.PopulateEditableExpression(porfolioTargetExpression, model.PortfolioTarget);
            var result = new Core.PortfolioTargetModel(broadGlobalActivePorfolio, porfolioTargetExpression);
            return result;
        }

        protected BasketPortfolioSecurityTargetChangesetInfo DeserializeBasketPortfolioSecurityTargetChangesetInfo(ChangesetModel model)
        {
            var result = new BasketPortfolioSecurityTargetChangesetInfo(
                model.Id,
                model.Username,
                model.Timestamp,
                model.CalculationId
            );
            return result;
        }

        protected TargetingTypeGroupBasketSecurityBaseValueChangesetInfo DeserializeTargetingTypeGroupBasketSecurityBaseValueChangeset(ChangesetModel model)
        {
            var result = new TargetingTypeGroupBasketSecurityBaseValueChangesetInfo(
                model.Id,
                model.Username,
                model.Timestamp,
                model.CalculationId
            );
            return result;
        }
    }
}
