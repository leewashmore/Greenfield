using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TopDown.Web.Helpers;
using TopDown.Core.Persisting;
using TopDown.Core;
using TopDown.Core.ManagingBpt;
using Aims.Core.Sql;
using TopDown.Core.Overlaying;
using TopDown.Core.ManagingSecurities;
using TopDown.Core.ManagingPst;
using TopDown.Core.ManagingTaxonomies;
using TopDown.Core.ManagingBaskets;
using TopDown.Core.ManagingCountries;
using TopDown.Core.ManagingTargetingTypes;
using TopDown.Core.Gadgets.PortfolioPicker;
using TopDown.Core.Gadgets.BasketPicker;
using TopDown.Core.ManagingBenchmarks;
using Aims.Core;

namespace TopDown.Web.Controllers
{
	public abstract class ControllerBase : Controller
	{
		[Obsolete("This is a HACK!")]
		public const String Username = "bykova";
		//public String Username { get { return this.HttpContext.User.Identity.Name; } }
		public const String JsonMimeType = "application/json";
		private ExceptionToJsonSerializer exceptionSerializer;

		public ControllerBase()
		{
			this.exceptionSerializer = new Core.ExceptionToJsonSerializer();
		}

		protected Facade CreateFacade(Boolean shouldDrop, IMonitor monitor)
		{
			var infoCopier = new InfoCopier();
			var cache = this.HttpContext.Cache;
			var countryRepositoryStorage = new CacheStorage<CountryRepository>(this.HttpContext.Cache);
			var countrySerializer = new CountryToJsonSerializer();
			var countryManager = new CountryManager(countryRepositoryStorage);
			var basketRenderer = new BasketRenderer();
			var securityRepositoryCache = new CacheStorage<SecurityRepository>(cache);
			var calculationRequester = new Core.ManagingCalculations.CalculationRequester();

			var securitySerializer = new SecurityToJsonSerializer(countrySerializer);
			var securityManager = new SecurityManager(securityRepositoryCache, monitor);
			var settings = Settings.CreateFromConfiguration();

			IDataManagerFactory dataManagerFactory = new FakeDataManagerFactory();
			var connectionFactory = new SqlConnectionFactory(settings.ConnectionString);
			var portfolioRepositoryCache = new CacheStorage<PortfolioRepository>(cache);
			var portfolioSerialzer = new Core.ManagingPortfolios.PortfolioToJsonSerializer(securitySerializer);
			var portfolioManager = new Core.ManagingPortfolios.PortfolioManager(
				portfolioRepositoryCache,
				portfolioSerialzer
			);

            var issuerRepositoryStorage = new CacheStorage<IssuerRepository>(cache);
            var issuerManager = new IssuerManager(monitor, issuerRepositoryStorage);

			var targetingTypeManager = new TargetingTypeManager(
				new Core.ManagingTargetingTypes.InfoDeserializer(),
				new CacheStorage<TargetingTypeRepository>(cache),
				new CacheStorage<TargetingTypeGroupRepository>(cache)
			);
			var taxonomyManager = new TaxonomyManager(
				new CacheStorage<TaxonomyRepository>(cache),
				new Core.ManagingTaxonomies.InfoDeserializer(
					new Core.ManagingTaxonomies.XmlDeserializer()
				)
			);

			var basketRepositoryStorage = new CacheStorage<BasketRepository>(this.HttpContext.Cache);
			var basketManager = new BasketManager(
				basketRepositoryStorage,
				new Core.ManagingBaskets.XmlDeserializer(),
				new BasketSecurityRelationshipInvestigator()
			);


			var benchmarkRepositoryStorage = new CacheStorage<BenchmarkRepository>(this.HttpContext.Cache);
			var benchmarkManager = new BenchmarkManager(benchmarkRepositoryStorage);

			var portfolioSecurityTargetRepositoryCache = new CacheStorage<Core.ManagingPst.PortfolioSecurityTargetRepository>(cache);
			var portfolioSecurityTargetRepositoryManager = new Core.ManagingPst.RepositoryManager(
				infoCopier,
				portfolioSecurityTargetRepositoryCache
			);

			var bpstCache = new CacheStorage<Core.ManagingBpst.BasketSecurityPortfolioTargetRepository>(cache);
			var bpstManager = new Core.ManagingBpst.BasketSecurityPortfolioTargetRepositoryManager(bpstCache);

			var ttgbsbvrCache = new CacheStorage<Core.ManagingBpst.TargetingTypeGroupBasketSecurityBaseValueRepository>(cache);
			var ttgbsbvrManager = new Core.ManagingBpst.TargetingTypeGroupBasketSecurityBaseValueRepositoryManager(ttgbsbvrCache);
            
			var repositoryManager = new Core.RepositoryManager(
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

			if (shouldDrop)
			{
				repositoryManager.DropEverything();
			}

			var validationSerializer = new Core.ValidationIssueToJsonSerializer();
			var expressionSerializer = new ExpressionToJsonSerializer(validationSerializer);
			var expressionDeserializer = new ExpressionFromJsonDeserializer();
			var defaultBreakdownValues = TopDown.Core.ManagingBpt.DefaultValues.CreateDefaultValues();
			var picker = new ExpressionPicker();
			var commonParts = new CommonParts();
			var overlayModelBuilder = new Core.Overlaying.ModelBuilder(null, commonParts);
			var overlayManager = new OverlayManager(overlayModelBuilder);
			var bptModelBuilder = new Core.ManagingBpt.ModelBuilder(
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
				new Core.ManagingTaxonomies.CountryIsoCodesExtractor(taxonomyTraverser),
				new Core.Overlaying.CombinedCountryIsoCodesExtractor(new Core.Overlaying.CountryIsoCodesExtractor()),
				new Core.ManagingBenchmarks.CountryIsoCodesExtractor()
			);
			var modelToTaxonomyTransformer = new ModelToTaxonomyTransformer();
			var bptModelApplier = new Core.ManagingBpt.ModelApplier(
				new Core.ManagingBpt.ChangingBt.ChangesetApplier(dataManagerFactory, modelToTaxonomyTransformer),
				new Core.ManagingBpt.ChangingBt.ModelToChangesetTransformer(globeTraverser),
				new Core.ManagingBpt.ChangingPsto.ChangesetApplier(),
				new Core.ManagingBpt.ChangingPsto.ModelToChangesetTransformer(),
				new Core.ManagingBpt.ChangingTtbbv.ChangesetApplier(),
				new Core.ManagingBpt.ChangingTtbbv.ModelToChangesetTransformer(globeTraverser),
				new Core.ManagingBpt.ChangingTtbpt.ChangesetApplier(),
				new Core.ManagingBpt.ChangingTtbpt.ModelToChangesetTransformer(globeTraverser),
				new Core.ManagingBpt.ModelValidator(globeTraverser),
				dataManagerFactory,
				calculationRequester
			);

			var targetsFlattener = new TargetsFlattener(infoCopier);
			var bptManager = new Core.ManagingBpt.ModelManager(
				globeTraverser,
				bptModelBuilder,
				taxonomyToModelTransformer,
				new BaseValueInitializer(globeTraverser),
				new BenchmarkValueInitializer(globeTraverser),
				new OverlayInitializer(globeTraverser, targetsFlattener),
				new PortfolioAdjustmentInitializer(globeTraverser),
				new Core.ManagingBpt.ModelToJsonSerializer(expressionSerializer, portfolioSerialzer),
				new Core.ManagingBpt.ModelFromJsonDeserializer(
					picker,
					bptModelBuilder,
					globeTraverser,
					expressionDeserializer
				),
				repositoryManager,
				overlayManager,
				countriesDetector,
				bptModelApplier,
				new Core.ManagingBpt.ModelChangeDetector(
					new Core.ManagingBpt.ModelExpressionTraverser(globeTraverser)
				)
			);

			var pstModelToChangeMapper = new ModelToChangesetTransformer();
			var pstChangeApplier = new ChangesetApplier();
			var pstModelBuilder = new Core.ManagingPst.ModelBuilder(null, commonParts);
            var pstModelValidator = new TopDown.Core.ManagingPst.ModelValidator();
			var pstManager = new PstManager(
				pstChangeApplier,
                pstModelValidator,
				pstModelToChangeMapper,
				new Core.ManagingPst.ModelFromJsonDeserializer(
					pstModelBuilder,
					expressionDeserializer
				),
				pstModelBuilder,
				portfolioSecurityTargetRepositoryManager,
				new Core.ManagingPst.ModelChangeDetector(
					new Core.ManagingPst.ModelExpressionTraverser()
				),
				dataManagerFactory,
				calculationRequester,
				new Core.ManagingPst.ModelToJsonSerializer(expressionSerializer, securitySerializer)
			);


			var portfiolioPickerManager = new ProtfolioPickerManager(
				new Core.Gadgets.PortfolioPicker.ModelToJsonSerializer()
			);

			var basketPickerManager = new Core.Gadgets.BasketPicker.ModelManager(
				new Core.Gadgets.BasketPicker.ModelBuilder(
					new BasketExtractor(taxonomyTraverser),
					new BasketRenderer()
				),
				new Core.Gadgets.BasketPicker.ModelToJsonSerializer()
			);

			var bpstModelBuilder = new Core.ManagingBpst.ModelBuilder(
				Core.ManagingBpst.DefaultValues.CreateDefaultValues(),
				commonParts
			);
			var bpstBenchmarkInitializer = new Core.ManagingBpst.BenchmarkInitializer();
			var bpstModelValidator = new Core.ManagingBpst.ModelValidator();
			var bpstModelManager = new Core.ManagingBpst.ModelManager(
				new Core.ManagingBpst.ModelToJsonSerializer(expressionSerializer, securitySerializer),
				bpstModelBuilder,
				new Core.ManagingBpst.ModelFromJsonDeserializer(
					expressionDeserializer,
					bpstModelBuilder,
					bpstBenchmarkInitializer
				),
				new Core.ManagingBpst.ModelApplier(
					dataManagerFactory,
					new Core.ManagingBpst.ChangingTtgbsbv.ChangesetApplier(),
					new Core.ManagingBpst.ChangingTtgbsbv.ModelToChangesetTransformer(),
					new Core.ManagingBpst.ChangingBpst.PortfolioTargetChangesetApplier(),
					new Core.ManagingBpst.ChangingBpst.ModelToChangesetTransformter(),
					calculationRequester,
					bpstModelValidator,
					repositoryManager
				),
				bpstModelValidator,
				bpstBenchmarkInitializer,
				new Core.ManagingBpst.ModelChangeDetector(new Core.ManagingBpst.ModelExpressionTraverser()),
				repositoryManager
			);

			var validationManager = new ValidationManager(validationSerializer);

			var hopper = new Core.ManagingCalculations.Hopper(
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

		protected IMonitor CreateMonitor()
		{
			var monitor = new Monitor();
			return monitor;
		}

		protected JsonFacade CreateJsonFacade(Boolean shouldDrop)
		{
			var monitor = this.CreateMonitor();
			var facade = CreateFacade(shouldDrop, monitor);
			var securityToJsonSerializer = new SecurityToJsonSerializer(new CountryToJsonSerializer());
			var result = new JsonFacade(
				facade,
				new ValidationManager(new ValidationIssueToJsonSerializer()),
				securityToJsonSerializer
			);
			return result;
		}

		protected ActionResult WatchAndWrap(Func<String> handler)
		{
			String json;
			try
			{
				json = "{ \"data\": " + handler() + " }";
			}
			catch (Exception exception)
			{
				json = "{ \"error\": " + this.exceptionSerializer.RenderToJson(
					exception,
					new ExceptionToJsonSerializerOptions(true, true, true)
				) + " }";
			}
			return this.Content(json, JsonMimeType);
		}

		protected Boolean ShouldDrop(String what)
		{
			if (what == "Drop")
			{
				return true;
			}
			else if (what == "Keep")
			{
				return false;
			}
			else
			{
                return false;
				throw new ApplicationException("Unexpected mode \"" + what + "\".");
			}
		}
	}
}
