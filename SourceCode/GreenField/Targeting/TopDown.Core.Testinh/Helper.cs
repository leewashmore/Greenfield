using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopDown.Core.Persisting;
using TopDown.Core.ManagingCountries;
using TopDown.Core.ManagingSecurities;
using TopDown.Core.ManagingCalculations;
using TopDown.Core.ManagingBaskets;
using Aims.Core.Sql;
using TopDown.Core.ManagingTargetingTypes;
using TopDown.Core.ManagingTaxonomies;
using TopDown.Core.ManagingBenchmarks;
using TopDown.Core.ManagingBpt;
using TopDown.Core.Overlaying;
using TopDown.Core.ManagingPst;
using TopDown.Core.Gadgets.PortfolioPicker;
using Aims.Core;

namespace TopDown.Core.Testing
{
	public class Helper
	{
		public static Facade CreateFacade(String connectionString)
		{
			var infoCopier = new InfoCopier();
			var countryRepositoryStorage = new InMemoryStorage<CountryRepository>();
			var countrySerializer = new CountryToJsonSerializer();
			var countryManager = new CountryManager(countryRepositoryStorage);
			var basketRenderer = new BasketRenderer();
			var securityRepositoryCache = new InMemoryStorage<SecurityRepository>();
			var calculationRequester = new CalculationRequester();
			var monitor = new Monitor();
			var securitySerializer = new SecurityToJsonSerializer(countrySerializer);
			var securityManager = new SecurityManager(securityRepositoryCache, monitor);

			IDataManagerFactory dataManagerFactory = new FakeDataManagerFactory();
            var connectionFactory = new SqlConnectionFactory(connectionString);
			var portfolioRepositoryCache = new InMemoryStorage<PortfolioRepository>();
			var portfolioSerialzer = new TopDown.Core.ManagingPortfolios.PortfolioToJsonSerializer(securitySerializer);
			var portfolioManager = new TopDown.Core.ManagingPortfolios.PortfolioManager(
				portfolioRepositoryCache,
				portfolioSerialzer
			);

			var targetingTypeManager = new TargetingTypeManager(
				new TopDown.Core.ManagingTargetingTypes.InfoDeserializer(),
				new InMemoryStorage<TargetingTypeRepository>(),
				new InMemoryStorage<TargetingTypeGroupRepository>()
			);
			var taxonomyManager = new TaxonomyManager(
				new InMemoryStorage<TaxonomyRepository>(),
				new TopDown.Core.ManagingTaxonomies.InfoDeserializer(
					new TopDown.Core.ManagingTaxonomies.XmlDeserializer()
				)
			);

			var basketRepositoryStorage = new InMemoryStorage<BasketRepository>();
			var basketManager = new BasketManager(
				basketRepositoryStorage,
				new TopDown.Core.ManagingBaskets.XmlDeserializer(),
				new BasketSecurityRelationshipInvestigator()
			);


			var benchmarkRepositoryStorage = new InMemoryStorage<BenchmarkRepository>();
			var benchmarkManager = new BenchmarkManager(benchmarkRepositoryStorage);

			var portfolioSecurityTargetRepositoryCache = new InMemoryStorage<TopDown.Core.ManagingPst.PortfolioSecurityTargetRepository>();
			var portfolioSecurityTargetRepositoryManager = new TopDown.Core.ManagingPst.RepositoryManager(
				infoCopier,
				portfolioSecurityTargetRepositoryCache
			);

			var bpstCache = new InMemoryStorage<TopDown.Core.ManagingBpst.BasketSecurityPortfolioTargetRepository>();
			var bpstManager = new TopDown.Core.ManagingBpst.BasketSecurityPortfolioTargetRepositoryManager(bpstCache);

			var ttgbsbvrCache = new InMemoryStorage<TopDown.Core.ManagingBpst.TargetingTypeGroupBasketSecurityBaseValueRepository>();
			var ttgbsbvrManager = new TopDown.Core.ManagingBpst.TargetingTypeGroupBasketSecurityBaseValueRepositoryManager(ttgbsbvrCache);

            var issuerRepositoryStorage = new InMemoryStorage<IssuerRepository>();
            var issuerManager = new IssuerManager(monitor, issuerRepositoryStorage);


			var repositoryManager = new TopDown.Core.RepositoryManager(
				monitor,
				basketManager,
				targetingTypeManager,
				countryManager,
				taxonomyManager,
				securityManager,
				portfolioManager,
				benchmarkManager,
				portfolioSecurityTargetRepositoryManager,
				bpstManager,
				ttgbsbvrManager,
                issuerManager
			);

			repositoryManager.DropEverything();

			var validationSerializer = new TopDown.Core.ValidationIssueToJsonSerializer();
			var expressionSerializer = new ExpressionToJsonSerializer(validationSerializer);
			var expressionDeserializer = new ExpressionFromJsonDeserializer();
			var defaultBreakdownValues = TopDown.Core.ManagingBpt.DefaultValues.CreateDefaultValues();
			var picker = new ExpressionPicker();
			var commonParts = new CommonParts();
			var overlayModelBuilder = new TopDown.Core.Overlaying.ModelBuilder(null, commonParts);
			var overlayManager = new OverlayManager(overlayModelBuilder);
			var bptModelBuilder = new TopDown.Core.ManagingBpt.ModelBuilder(
				picker,
				commonParts,
				defaultBreakdownValues,
				overlayModelBuilder
			);

			var globeTraverser = new GlobeTraverser();
			var taxonomyTraverser = new TaxonomyTraverser();
			var taxonomyToModelTransformer = new TaxonomyToModelTransformer(picker, bptModelBuilder, globeTraverser);
			var countriesDetector = new MissingCountriesDetector(
				new UnknownCountryIsoCodesDetector(),
				new TopDown.Core.ManagingTaxonomies.CountryIsoCodesExtractor(taxonomyTraverser),
				new TopDown.Core.Overlaying.CombinedCountryIsoCodesExtractor(new TopDown.Core.Overlaying.CountryIsoCodesExtractor()),
				new TopDown.Core.ManagingBenchmarks.CountryIsoCodesExtractor()
			);
			var modelToTaxonomyTransformer = new ModelToTaxonomyTransformer();
			var bptModelApplier = new TopDown.Core.ManagingBpt.ModelApplier(
				new TopDown.Core.ManagingBpt.ChangingBt.ChangesetApplier(dataManagerFactory, modelToTaxonomyTransformer),
				new TopDown.Core.ManagingBpt.ChangingBt.ModelToChangesetTransformer(globeTraverser),
				new TopDown.Core.ManagingBpt.ChangingPsto.ChangesetApplier(),
				new TopDown.Core.ManagingBpt.ChangingPsto.ModelToChangesetTransformer(),
				new TopDown.Core.ManagingBpt.ChangingTtbbv.ChangesetApplier(),
				new TopDown.Core.ManagingBpt.ChangingTtbbv.ModelToChangesetTransformer(globeTraverser),
				new TopDown.Core.ManagingBpt.ChangingTtbpt.ChangesetApplier(),
				new TopDown.Core.ManagingBpt.ChangingTtbpt.ModelToChangesetTransformer(globeTraverser),
				new TopDown.Core.ManagingBpt.ModelValidator(globeTraverser),
				dataManagerFactory,
				calculationRequester
			);

			var targetsFlattener = new TargetsFlattener(infoCopier);
			var bptChangeDetector = new TopDown.Core.ManagingBpt.ModelChangeDetector(
					new TopDown.Core.ManagingBpt.ModelExpressionTraverser(globeTraverser)
			);
			var bptManager = new TopDown.Core.ManagingBpt.ModelManager(
				globeTraverser,
				bptModelBuilder,
				taxonomyToModelTransformer,
				new BaseValueInitializer(globeTraverser),
				new BenchmarkValueInitializer(globeTraverser),
				new OverlayInitializer(globeTraverser, targetsFlattener),
				new PortfolioAdjustmentInitializer(globeTraverser),
				new TopDown.Core.ManagingBpt.ModelToJsonSerializer(expressionSerializer, portfolioSerialzer),
				new TopDown.Core.ManagingBpt.ModelFromJsonDeserializer(
					picker,
					bptModelBuilder,
					globeTraverser,
					expressionDeserializer
				),
				repositoryManager,
				overlayManager,
				countriesDetector,
				bptModelApplier,
				bptChangeDetector
			);

			var pstModelToChangeMapper = new TopDown.Core.ManagingPst.ModelToChangesetTransformer();
			var pstChangeApplier = new TopDown.Core.ManagingPst.ChangesetApplier();
			var pstModelBuilder = new TopDown.Core.ManagingPst.ModelBuilder(null, commonParts);
            var pstModelValidator = new TopDown.Core.ManagingPst.ModelValidator();
			var pstManager = new PstManager(
				pstChangeApplier,
                pstModelValidator,
				pstModelToChangeMapper,
				new TopDown.Core.ManagingPst.ModelFromJsonDeserializer(
					pstModelBuilder,
					expressionDeserializer
				),
				pstModelBuilder,
				portfolioSecurityTargetRepositoryManager,
				new TopDown.Core.ManagingPst.ModelChangeDetector(
					new TopDown.Core.ManagingPst.ModelExpressionTraverser()
				),
				dataManagerFactory,
				calculationRequester,
				new TopDown.Core.ManagingPst.ModelToJsonSerializer(expressionSerializer, securitySerializer)
			);


			var portfiolioPickerManager = new ProtfolioPickerManager(
				new TopDown.Core.Gadgets.PortfolioPicker.ModelToJsonSerializer()
			);

			var basketPickerManager = new TopDown.Core.Gadgets.BasketPicker.ModelManager(
				new TopDown.Core.Gadgets.BasketPicker.ModelBuilder(
					new BasketExtractor(taxonomyTraverser),
					new BasketRenderer()
				),
				new TopDown.Core.Gadgets.BasketPicker.ModelToJsonSerializer()
			);

			var bpstModelBuilder = new TopDown.Core.ManagingBpst.ModelBuilder(
			   TopDown.Core.ManagingBpst.DefaultValues.CreateDefaultValues(),
				commonParts
			);
			var bpstBenchmarkInitializer = new TopDown.Core.ManagingBpst.BenchmarkInitializer();
			var bpstModelValidator = new TopDown.Core.ManagingBpst.ModelValidator();
			var bpstModelManager = new TopDown.Core.ManagingBpst.ModelManager(
				new TopDown.Core.ManagingBpst.ModelToJsonSerializer(expressionSerializer, securitySerializer),
				bpstModelBuilder,
				new TopDown.Core.ManagingBpst.ModelFromJsonDeserializer(
					expressionDeserializer,
					bpstModelBuilder,
					bpstBenchmarkInitializer
				),
				new TopDown.Core.ManagingBpst.ModelApplier(
					dataManagerFactory,
					new TopDown.Core.ManagingBpst.ChangingTtgbsbv.ChangesetApplier(),
					new TopDown.Core.ManagingBpst.ChangingTtgbsbv.ModelToChangesetTransformer(),
					new TopDown.Core.ManagingBpst.ChangingBpst.PortfolioTargetChangesetApplier(),
					new TopDown.Core.ManagingBpst.ChangingBpst.ModelToChangesetTransformter(),
					calculationRequester,
					bpstModelValidator,
					repositoryManager
				),
				bpstModelValidator,
				bpstBenchmarkInitializer,
				new TopDown.Core.ManagingBpst.ModelChangeDetector(new TopDown.Core.ManagingBpst.ModelExpressionTraverser()),
				repositoryManager
			);

			var validationManager = new ValidationManager(validationSerializer);

			var hopper = new TopDown.Core.ManagingCalculations.Hopper(
				repositoryManager,
				bptManager,
				bpstModelManager,
				basketRenderer
			);

            var commentManager = new Core.ManagingComments.CommentManager();

			var facade = new Facade(
				connectionFactory,
				dataManagerFactory,
				repositoryManager,
				bptManager,
				picker,
				commonParts,
				pstManager,
				basketManager,
				portfiolioPickerManager,
				basketPickerManager,
				bpstModelManager,
				portfolioManager,
				hopper,
                commentManager
			);

			return facade;
		}
	}
}
