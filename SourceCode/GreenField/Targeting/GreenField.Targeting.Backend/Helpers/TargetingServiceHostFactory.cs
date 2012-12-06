using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel.Activation;
using System.ServiceModel;
using TopDown.Core;
using System.Web.Caching;
using System.Diagnostics;
using TopDown.Core.Persisting;
using TopDown.Core.ManagingCountries;
using TopDown.Core.ManagingBaskets;
using TopDown.Core.ManagingSecurities;
using TopDown.Core.ManagingCalculations;
using TopDown.Core.Sql;
using TopDown.Core.ManagingTargetingTypes;
using TopDown.Core.ManagingTaxonomies;
using TopDown.Core.ManagingBenchmarks;
using TopDown.Core.ManagingBpt;
using TopDown.Core.Overlaying;
using TopDown.Core.ManagingPst;
using TopDown.Core.Gadgets.PortfolioPicker;

namespace GreenField.Targeting.Backend.Helpers
{
    public class TargetingServiceHostFactory : ServiceHostFactory
    {
        private readonly GreenField.Targeting.Server.FacadeSettings settings;

        public TargetingServiceHostFactory()
        {
            this.settings = CreateFacadeSettings(
                ConfigurationSettings.AimsConnectionString,
                ConfigurationSettings.ShouldDropRepositoriesOnEachReload
            );
        }

        protected override ServiceHost CreateServiceHost(Type serviceType,
            Uri[] baseAddresses)
        {
            var serviceHost = new TargetingServiceHost(
                this.settings,
                serviceType,
                baseAddresses
            );
            return serviceHost;
        }

        private class CacheStorage<TValue> : IStorage<TValue>
            where TValue : class
        {
            private Cache cache;
            public CacheStorage(Cache cache)
            {
                this.cache = cache;
            }

            public TValue this[String key]
            {
                [DebuggerStepThrough]
                get
                {
                    var result = this.cache.Get(key) as TValue;
                    return result;
                }
                [DebuggerStepThrough]
                set
                {
                    if (value == null)
                    {
                        this.cache.Remove(key);
                    }
                    else
                    {
                        this.cache.Insert(key, value);
                    }
                }
            }
        }

        protected static GreenField.Targeting.Server.FacadeSettings CreateFacadeSettings(String connectionString, Boolean shouldDropRepositories)
        {
            try
            {
                return CreateFacadeSettingsUnsafe(connectionString, shouldDropRepositories);
            }
            catch (Exception exception)
            {
                throw new ApplicationException("Unable to create a facade for targeting operations. Reason: " + exception.Message, exception);
            }
        }

        private static GreenField.Targeting.Server.FacadeSettings CreateFacadeSettingsUnsafe(String connectionString, Boolean shouldDropRepositories)
        {
            var infoCopier = new InfoCopier();
            var cache = HttpContext.Current.Cache;
            var countryRepositoryStorage = new CacheStorage<CountryRepository>(cache);
            var countrySerializer = new CountryToJsonSerializer();
            var countryManager = new CountryManager(countryRepositoryStorage);
            var basketRenderer = new BasketRenderer();
            var securityRepositoryCache = new CacheStorage<SecurityRepository>(cache);
            var calculationRequester = new CalculationRequester();
            var monitor = new Monitor();
            var securitySerializer = new SecurityToJsonSerializer(countrySerializer);
            var securityManager = new SecurityManager(securityRepositoryCache, securitySerializer, monitor);

            IDataManagerFactory dataManagerFactory = new FakeDataManagerFactory();
            var connectionFactory = new SqlConnectionFactory(connectionString);
            var portfolioRepositoryCache = new CacheStorage<TopDown.Core.ManagingPortfolios.PortfolioRepository>(cache);
            var portfolioSerialzer = new TopDown.Core.ManagingPortfolios.PortfolioToJsonSerializer(securitySerializer);
            var portfolioManager = new TopDown.Core.ManagingPortfolios.PortfolioManager(
                portfolioRepositoryCache,
                portfolioSerialzer
            );

            var targetingTypeManager = new TargetingTypeManager(
                new TopDown.Core.ManagingTargetingTypes.InfoDeserializer(),
                new CacheStorage<TargetingTypeRepository>(cache),
                new CacheStorage<TargetingTypeGroupRepository>(cache)
            );
            var taxonomyManager = new TaxonomyManager(
                new CacheStorage<TaxonomyRepository>(cache),
                new TopDown.Core.ManagingTaxonomies.InfoDeserializer(
                    new TopDown.Core.ManagingTaxonomies.XmlDeserializer()
                )
            );

            var basketRepositoryStorage = new CacheStorage<BasketRepository>(cache);
            var basketManager = new BasketManager(
                basketRepositoryStorage,
                new TopDown.Core.ManagingBaskets.XmlDeserializer(),
                new BasketSecurityRelationshipInvestigator()
            );


            var benchmarkRepositoryStorage = new CacheStorage<BenchmarkRepository>(cache);
            var benchmarkManager = new BenchmarkManager(benchmarkRepositoryStorage);

            var portfolioSecurityTargetRepositoryCache = new CacheStorage<TopDown.Core.ManagingPst.PortfolioSecurityTargetRepository>(cache);
            var portfolioSecurityTargetRepositoryManager = new TopDown.Core.ManagingPst.RepositoryManager(
                infoCopier,
                portfolioSecurityTargetRepositoryCache
            );

            var bpstCache = new CacheStorage<TopDown.Core.ManagingBpst.BasketSecurityPortfolioTargetRepository>(cache);
            var bpstManager = new TopDown.Core.ManagingBpst.BasketSecurityPortfolioTargetRepositoryManager(bpstCache);

            var ttgbsbvrCache = new CacheStorage<TopDown.Core.ManagingBpst.TargetingTypeGroupBasketSecurityBaseValueRepository>(cache);
            var ttgbsbvrManager = new TopDown.Core.ManagingBpst.TargetingTypeGroupBasketSecurityBaseValueRepositoryManager(ttgbsbvrCache);

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
                ttgbsbvrManager
            );

            if (shouldDropRepositories)
            {
                repositoryManager.DropEverything();
            }

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
            var pstModelChangeDetector = new TopDown.Core.ManagingPst.ModelChangeDetector(
                    new TopDown.Core.ManagingPst.ModelExpressionTraverser()
                );
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
                pstModelChangeDetector,
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
            var bpstModelChangeDetector =new TopDown.Core.ManagingBpst.ModelChangeDetector(new TopDown.Core.ManagingBpst.ModelExpressionTraverser());
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
                bpstModelChangeDetector,
                repositoryManager
            );

            var validationManager = new ValidationManager(validationSerializer);

            var hopper = new TopDown.Core.ManagingCalculations.Hopper(
                repositoryManager,
                bptManager,
                bpstModelManager,
                basketRenderer
            );

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
                hopper
            );


            var serializer = new Server.Serializer();
            var bgaSerializer = new GreenField.Targeting.Server.BroadGlobalActive.Serializer(
                serializer,
                bptChangeDetector
            );

            var deserializer = new Server.Deserializer(connectionFactory, dataManagerFactory, repositoryManager);
            var bgaDeserializer = new GreenField.Targeting.Server.BroadGlobalActive.Deserializer(
                deserializer,
                bptModelBuilder,
                globeTraverser
            );

            var btSerializer = new GreenField.Targeting.Server.BasketTargets.Serializer(serializer, bpstModelChangeDetector);
            var btDeserializer = new GreenField.Targeting.Server.BasketTargets.Deserializer(deserializer, bpstModelBuilder, bpstBenchmarkInitializer);


            var buSerializer = new GreenField.Targeting.Server.BottomUp.Serializer(serializer, pstModelChangeDetector);
            var buDeserializer = new GreenField.Targeting.Server.BottomUp.Deserializer(deserializer, pstModelBuilder);

            var result = new GreenField.Targeting.Server.FacadeSettings(
                facade,
                serializer,
                bgaSerializer,
                bgaDeserializer,
                btSerializer,
                btDeserializer,
                buSerializer,
                buDeserializer,
                ConfigurationSettings.ShouldDropRepositoriesOnEachReload
            );

            return result;
        }
    }
}