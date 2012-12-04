using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopDown.Core.ManagingSecurities;
using System.Diagnostics;
using TopDown.Core.Persisting;
using TopDown.Core.ManagingPortfolios;
using TopDown.Core.ManagingBenchmarks;

namespace TopDown.Core.ManagingBpst
{
	public class ModelFromJsonDeserializer
	{
		private ExpressionFromJsonDeserializer expressionDeserializer;
		private ModelBuilder modelBuilder;
		private RepositoryManager repositoryManager;
		private BenchmarkInitializer benchmarkInitializer;

		[DebuggerStepThrough]
		public ModelFromJsonDeserializer(
			ExpressionFromJsonDeserializer expressionDeserializer,
			ModelBuilder modelBuilder,
			BenchmarkInitializer benchmarkInitializer
		)
		{
			this.modelBuilder = modelBuilder;
			this.expressionDeserializer = expressionDeserializer;
			this.benchmarkInitializer = benchmarkInitializer;
		}

		public RootModel DeserializeRoot(
			JsonReader reader,
			SecurityRepository securityRepository,
			ManagingTargetingTypes.TargetingTypeGroupRepository targetingTypeGroupRepository,
			ManagingBaskets.BasketRepository basketRepository,
			PortfolioRepository portfolioRepository,
			BenchmarkRepository benchmarkRepository
		)
		{

			var latestBaseChangeset = reader.Read(JsonNames.LatestBaseChangeset, delegate
			{
				return this.DeserializeLatestBaseChangeset(reader);
			});

			var latestPortfolioTargetChangeset = reader.Read(JsonNames.LatestPortfolioTargetChangeset, delegate
			{
				return this.DeserializeLatestPortfolioTargetChangeset(reader);
			});

			var targetingTypeGroupId = reader.ReadAsInt32(JsonNames.TargetingTypeGroupId);
			var targetingTypeGroup = targetingTypeGroupRepository.GetTargetingTypeGroup(targetingTypeGroupId);

			var basketId = reader.ReadAsInt32(JsonNames.BasketId);
			var basket = basketRepository.GetBasket(basketId);

			var securityModels = reader.ReadArray("securities", delegate
			{
				var securityModel = this.DeserializeSecurity(
					reader,
					securityRepository,
					portfolioRepository
				);
				return securityModel;
			}).ToList();

			// adding a new security if any
			var securityIdTobeAddedOpt = reader.ReadAsNullableString(JsonNames.SecurityIdTobeAdded);
			if (securityIdTobeAddedOpt != null)
			{
				var broadGlobalActivePortfolios = targetingTypeGroup.GetBgaPortfolios();
				var security = securityRepository.GetSecurity(securityIdTobeAddedOpt);
				var baseExpression = this.modelBuilder.CreateBaseExpression();
				var benchmarkExpression = this.modelBuilder.CreateBenchmarkExpression();
				var securityModel = new SecurityModel(
					security,
					baseExpression,
					benchmarkExpression,
					broadGlobalActivePortfolios.Select(bgaPortfolio => new PortfolioTargetModel(
						bgaPortfolio,
						this.modelBuilder.CreatePortfolioTargetExpression(bgaPortfolio.Name)
					)).ToArray()
				);
				this.benchmarkInitializer.InitializeSecurity(securityModel, benchmarkRepository);
				securityModels.Add(securityModel);
			}


			var portfolios = reader.ReadArray(JsonNames.Portfolios, delegate
			{
				var portfolioId = reader.ReadAsString(JsonNames.Id);
				var portfolio = portfolioRepository.GetBroadGlobalActivePortfolio(portfolioId);
				var portfolioTargetTotalExpression = this.modelBuilder.CreatePortfolioTargetTotalExpression(
					portfolio,
					securityModels
				);
				return new PortfolioModel(
					portfolio,
					portfolioTargetTotalExpression
				);
			});

			var baseTotalExpression = this.modelBuilder.CreateBaseTotalExpression(securityModels);


			//var targetingTypeGroup = targetingTypeGroupRepository.Get
			var core = new CoreModel(
				targetingTypeGroup,
				basket,
				portfolios,
				securityModels,
				baseTotalExpression
			);



			var result = new RootModel(
				latestBaseChangeset,
				latestPortfolioTargetChangeset,
				core
			);
			return result;
		}

		public BasketPortfolioSecurityTargetChangesetInfo DeserializeLatestPortfolioTargetChangeset(JsonReader reader)
		{
			var result = new BasketPortfolioSecurityTargetChangesetInfo(
				reader.ReadAsInt32(JsonNames.Id),
				reader.ReadAsString(JsonNames.Username),
				reader.ReadAsDatetime(JsonNames.Timestamp),
				reader.ReadAsInt32(JsonNames.CalcualtionId)
			);
			return result;
		}

		public TargetingTypeGroupBasketSecurityBaseValueChangesetInfo DeserializeLatestBaseChangeset(JsonReader reader)
		{
			var result = new TargetingTypeGroupBasketSecurityBaseValueChangesetInfo(
				reader.ReadAsInt32(JsonNames.Id),
				reader.ReadAsString(JsonNames.Username),
				reader.ReadAsDatetime(JsonNames.Timestamp),
				reader.ReadAsInt32(JsonNames.CalcualtionId)
			);
			return result;
		}

		public SecurityModel DeserializeSecurity(
			JsonReader reader,
			SecurityRepository securityRepository,
			PortfolioRepository portfolioRepository
		)
		{
			var securityId = reader.ReadAsString(JsonNames.SecurityId);
			var security = securityRepository.GetSecurity(securityId);

			var baseExpression = this.modelBuilder.CreateBaseExpression();
			reader.Read(JsonNames.Base, delegate
			{
				this.expressionDeserializer.PopulateEditableExpression(reader, baseExpression);
			});

			var benchmarkExpression = this.modelBuilder.CreateBenchmarkExpression();
			reader.Read(JsonNames.Benchmark, delegate
			{
				this.expressionDeserializer.PopulateUnchangeableExpression(reader, benchmarkExpression);
			});

			var portfolioTargets = reader.ReadArray(JsonNames.PortfolioTargets, delegate
			{
				return this.DeserializePortfolioTarget(reader, portfolioRepository);
			});

			var result = new SecurityModel(
				security,
				baseExpression,
				benchmarkExpression,
				portfolioTargets
			);
			return result;
		}

		public PortfolioTargetModel DeserializePortfolioTarget(JsonReader reader, PortfolioRepository portfolioRepository)
		{
			var portfolioId = reader.ReadAsString(JsonNames.PortfolioId);
			var bgaPortfolio = portfolioRepository.GetBroadGlobalActivePortfolio(portfolioId);
			var portfolioTargetExpression = this.modelBuilder.CreatePortfolioTargetExpression(bgaPortfolio.Name);
			reader.Read(JsonNames.Target, delegate
			{
				this.expressionDeserializer.PopulateEditableExpression(reader, portfolioTargetExpression);
			});

			var result = new PortfolioTargetModel(bgaPortfolio, portfolioTargetExpression);
			return result;
		}
	}
}
