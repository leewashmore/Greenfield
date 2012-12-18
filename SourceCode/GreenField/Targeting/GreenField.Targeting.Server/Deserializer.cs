using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopDown.Core;
using System.Diagnostics;
using Aims.Core.Sql;
using TopDown.Core.Persisting;
using TopDown.Core.ManagingTargetingTypes;
using TopDown.Core.ManagingPortfolios;
using TopDown.Core.ManagingBaskets;
using TopDown.Core.ManagingCountries;
using TopDown.Core.ManagingSecurities;
using TopDown.Core.ManagingBenchmarks;
using Aims.Core;
using Aims.Data.Server;

namespace GreenField.Targeting.Server
{
    public class Deserializer : Aims.Data.Server.Deserializer<IDataManager>
    {
        private TopDown.Core.RepositoryManager repositoryManager;
        private ISqlConnectionFactory connectionFactory;
        private IDataManagerFactory dataManagerFactory;

        [DebuggerStepThrough]
        public Deserializer(
            ISqlConnectionFactory connectionFactory,
            IDataManagerFactory dataManagerFactory,
            TopDown.Core.RepositoryManager repositoryManager
        )
            : base(connectionFactory, dataManagerFactory, repositoryManager)
        {
            this.connectionFactory = connectionFactory;
            this.dataManagerFactory = dataManagerFactory;
            this.repositoryManager = repositoryManager;
        }


        public TargetingType DeserializeTargetingType(TargetingTypeModel model)
        {
            TargetingTypeRepository targetingTypeRepository;
            using (var ondemandManager = this.CreateOnDemandDataManager())
            {
                targetingTypeRepository = this.repositoryManager.ClaimTargetingTypeRepository(ondemandManager);
            }
            var result = targetingTypeRepository.GetTargetingType(model.Id);
            return result;
        }

        public BroadGlobalActivePortfolio DeserializeBroadGlobalActivePorfolio(BroadGlobalActivePortfolioModel model)
        {
            PortfolioRepository portfolioRepository;
            using (var ondemandManager = this.CreateOnDemandDataManager())
            {
                portfolioRepository = this.repositoryManager.ClaimPortfolioRepository(ondemandManager);
            }
            var result = portfolioRepository.GetBroadGlobalActivePortfolio(model.Id);
            return result;
        }

        public CountryBasket DeserializeCountryBasket(CountryBasketModel model)
        {
            var basket = DeserializeBasket(model.Id);
            var result = basket.AsCountryBasket();
            return result;
        }

        public IBasket DeserializeBasket(Int32 basketId)
        {
            BasketRepository basketRepository;
            using (var ondemandManager = this.CreateOnDemandDataManager())
            {
                basketRepository = this.repositoryManager.ClaimBasketRepository(ondemandManager);
            }
            var basket = basketRepository.GetBasket(basketId);
            return basket;
        }

        public RegionBasket DeserializeRegionBasket(RegionBasketModel model)
        {
            var basket = DeserializeBasket(model.Id);
            var result = basket.AsRegionBasket();
            return result;
        }

        public void PopulateEditableExpression(Aims.Expressions.EditableExpression expression, EditableExpressionModel model)
        {
            expression.InitialValue = model.InitialValue;
            expression.EditedValue = model.EditedValue;
            expression.Comment = model.Comment;
            expression.LastOneModified = model.IsLastEdited;
        }

        public void PopulateUnchangableExpression(Aims.Expressions.UnchangableExpression<Decimal> expression, ExpressionModel model)
        {
            expression.InitialValue = model.Value;
        }


        public Country DeserializeCountry(CountryModel model)
        {
            CountryRepository countryRepository;
            using (var ondemandManager = this.CreateOnDemandDataManager())
            {
                countryRepository = this.repositoryManager.ClaimCountryRepository(ondemandManager);
            }
            var result = countryRepository.GetCountry(model.IsoCode);
            return result;
        }


        public BottomUpPortfolio DeserializeBottomUpPortfolio(BottomUpPortfolioModel model)
        {
            PortfolioRepository portfolioRepository;
            using (var ondemandManager = this.CreateOnDemandDataManager())
            {
                portfolioRepository = this.repositoryManager.ClaimPortfolioRepository(ondemandManager);
            }
            var result = portfolioRepository.GetBottomUpPortfolio(model.Id);
            return result;
        }

        public ISecurity DeserializeSecurity(SecurityModel model)
        {
            SecurityRepository securityRepository;
            using (var ondemandManager = this.CreateOnDemandDataManager())
            {
                securityRepository = this.repositoryManager.ClaimSecurityRepository(ondemandManager);
            }
            var security = securityRepository.GetSecurity(model.Id);
            return security;
        }

        public TargetingTypeGroup DeserializeTargetingTypeGroup(TargetingTypeGroupModel model)
        {
            TargetingTypeGroupRepository targetingTypeGroupRepository;
            using (var ondemandManager = this.CreateOnDemandDataManager())
            {
                targetingTypeGroupRepository = this.repositoryManager.ClaimTargetingTypeGroupRepository(ondemandManager);
            }
            var targetingTypeGroup = targetingTypeGroupRepository.GetTargetingTypeGroup(model.Id);
            return targetingTypeGroup;
        }

        public BenchmarkRepository ClaimBenchmarkRepository(DateTime benchmarkDate)
        {
            BenchmarkRepository benchmarkRepository;
            using (var ondemandManager = this.CreateOnDemandDataManager())
            {
                benchmarkRepository = this.repositoryManager.ClaimBenchmarkRepository(ondemandManager, benchmarkDate);
            }
            return benchmarkRepository;
        }
    }
}
