using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopDown.Core.ManagingTaxonomies;
using TopDown.Core.Persisting;
using TopDown.Core.ManagingBenchmarks;
using TopDown.Core.ManagingBaskets;
using System.IO;
using TopDown.Core.ManagingCountries;
using System.Data.SqlClient;
using TopDown.Core.ManagingTargetingTypes;
using TopDown.Core.ManagingSecurities;
using TopDown.Core.ManagingPortfolios;
using Aims.Expressions;
using TopDown.Core.ManagingCalculations;
using Aims.Core;
using Aims.Core.Sql;

namespace TopDown.Core.ManagingBpt
{
    public class ModelManager
    {
        private TaxonomyToModelTransformer taxonomyTransformer;
        private BaseValueInitializer baseValueInitializer;
        private BenchmarkValueInitializer benchmarkValueInitializer;
        private ModelToJsonSerializer breakdownSerializer;
        private ModelFromJsonDeserializer breakdownDeserializer;
        internal protected GlobeTraverser Traverser { get; private set; }
        private ModelBuilder modelBuilder;
        private RepositoryManager repositoryManager;
        private Overlaying.OverlayManager overlayManager;
        private MissingCountriesDetector countriesDetector;
        private ModelApplier modelApplier;
        private PortfolioAdjustmentInitializer portfolioAdjustmentInitializer;
        private OverlayInitializer overlayInitializer;
        private ModelChangeDetector changeDetector;

        public ModelManager(
            GlobeTraverser traverser,
            ModelBuilder modelBuilder,
            TaxonomyToModelTransformer taxonomyToModelTransformer,
            BaseValueInitializer baseValueInitializer,
            BenchmarkValueInitializer benchmarkValueInitializer,
            OverlayInitializer overlayInitializer,
            PortfolioAdjustmentInitializer portfolioAdjustmentInitializer,
            ModelToJsonSerializer breakdownSerializer,
            ModelFromJsonDeserializer breakdownDeserializer,
            RepositoryManager repositoryManager,
            Overlaying.OverlayManager overlayManager,
            MissingCountriesDetector countriesDetector,
            ModelApplier modelApplier,
            ModelChangeDetector changeDetector
        )
        {
            this.Traverser = traverser;
            this.modelBuilder = modelBuilder;
            this.taxonomyTransformer = taxonomyToModelTransformer;
            this.baseValueInitializer = baseValueInitializer;
            this.benchmarkValueInitializer = benchmarkValueInitializer;
            this.overlayInitializer = overlayInitializer;
            this.portfolioAdjustmentInitializer = portfolioAdjustmentInitializer;
            this.breakdownSerializer = breakdownSerializer;
            this.breakdownDeserializer = breakdownDeserializer;
            this.repositoryManager = repositoryManager;
            this.overlayManager = overlayManager;
            this.countriesDetector = countriesDetector;
            this.modelApplier = modelApplier;
            this.changeDetector = changeDetector;
        }


        public ManagingBpt.RootModel GetRootModel(
            Int32 targetingTypeId,
            String portfolioId,
            Boolean shouldBenchmarksBeInitialized,
            IDataManager manager
        )
        {
            ManagingBpt.RootModel result;
            // getting targeting, taxonomy, base values, overlays data from the database

            var countryRepository = this.repositoryManager.ClaimCountryRepository(manager);
            var targetingTypeRepository = this.repositoryManager.ClaimTargetingTypeRepository(manager);
            var targetingType = targetingTypeRepository.GetTargetingType(targetingTypeId);

            var baseValueResolver = this.CreateBaseValueResolver(manager, targetingType.Id);
            var securityRepository = this.repositoryManager.ClaimSecurityRepository(manager);
            var portfolioRepository = this.repositoryManager.ClaimPortfolioRepository(manager);

            var overlayModel = this.overlayManager.GetOverlayModel(
                targetingType,
                portfolioId,
                portfolioRepository,
                securityRepository,
                manager
            );

            var latestTtbbvChangesetInfo = manager.GetLatestTargetingTypeBasketBaseValueChangeset();
            var latestTtbptChangesetInfo = manager.GetLatestTargetingTypeBasketPortfolioTargetChangeset();
            var latestPstoChangesetInfo = manager.GetLatestBgaPortfolioSecurityFactorChangeset();
            var latestPstChangesetInfo = manager.GetLatestPortfolioSecurityTargetChangeSet();

            var portfolioSecurityTargetRepository = this.repositoryManager.ClaimPortfolioSecurityTargetRepository(latestPstChangesetInfo, manager);

            var residents = new List<IGlobeResident>();
            var globe = this.modelBuilder.CreateGlobeModel(residents);
            var computations = this.modelBuilder.CreateComputations(globe, this.Traverser);
            foreach (var node in targetingType.Taxonomy.GetResidents())
            {
                var resident = this.taxonomyTransformer.CreateModelOnceResolved(computations, node);
                residents.Add(resident);
            }

            var cash = this.modelBuilder.CreateCash(computations);
            var portfolioScaledGrandTotal = this.modelBuilder.CreateAddExpression(cash.PortfolioScaled, globe.PortfolioScaled);
            var trueExposureGrandTotal = this.modelBuilder.CreateAddExpression(cash.TrueExposure, globe.TrueExposure);
            var trueActiveGrandTotal = this.modelBuilder.CreateAddExpression(cash.TrueActive, globe.TrueActive);
            var portfolio = portfolioRepository.GetBroadGlobalActivePortfolio(portfolioId);
            var benchmarkDate = manager.GetLastestDateWhichBenchmarkDataIsAvialableOn();
            
            result = new RootModel(
                targetingType,
                portfolio,
                latestTtbbvChangesetInfo,
                latestTtbptChangesetInfo,
                latestPstoChangesetInfo,
                latestPstChangesetInfo,
                globe,
                cash,
                overlayModel,
                portfolioScaledGrandTotal,
                trueExposureGrandTotal,
                trueActiveGrandTotal,
                benchmarkDate
            );

            // populating the base and portfolio adjustment columsn first as they don't depend on potentially missing countries
            this.baseValueInitializer.Initialize(result, baseValueResolver);
            this.portfolioAdjustmentInitializer.InitializePortfolioAdjustments(result, manager);

            // in order to proceed we need to make sure all missing countries are there
            IEnumerable<BenchmarkSumByIsoInfo> benchmarks = new BenchmarkSumByIsoInfo[] { };

            // here is the deal with shouldBenchmarksBeInitialized:
            // when we are editing broad-global-avive composition we need benchmakrs for reference and in order to make sure all countries are considered
            // but when the hopper process  runs we don't need additional counties from bechmakr that can mess around and prevent the hopper from running
            // so this is why we need this switch
            if (shouldBenchmarksBeInitialized)
            {
                var benchmarkRepository = this.repositoryManager.ClaimBenchmarkRepository(manager, benchmarkDate);
                benchmarks = benchmarkRepository.GetByBenchmarkId(targetingType.BenchmarkIdOpt);
            }

            this.RegisterMissingCountriesIfAny(
                result,
                computations,
                countryRepository,
                targetingType,
                securityRepository,
                portfolioRepository,
                portfolioSecurityTargetRepository,
                benchmarks
            );

            // now we can initialize columns that require all missing countries to be in the model
            this.InitializeOverlay(
                result,
                securityRepository,
                portfolioRepository,
                portfolioSecurityTargetRepository
            );
            this.benchmarkValueInitializer.Initialize(result, new BenchmarkValueResolver(benchmarks));

            return result;
        }

        protected void RegisterMissingCountriesIfAny(ManagingBpt.RootModel result, Computations computations, CountryRepository countryRepository, TargetingType targetingType, SecurityRepository securityRepository, PortfolioRepository portfolioRepository, ManagingPst.PortfolioSecurityTargetRepository portfolioSecurityTargetRepository, IEnumerable<BenchmarkSumByIsoInfo> benchmarks)
        {
            // figuring out missing ISO country codes (if any) from benchmarks and overlays
            var missingCountriesIsoCodes = this.countriesDetector.FindMissingCountries(
                targetingType.Taxonomy,
                benchmarks,
                result.Factors.Items.Where(x => x.OverlayFactor.EditedValue.HasValue).Select(x => x.BottomUpPortfolio.Name),
                portfolioSecurityTargetRepository,
                portfolioRepository,
                securityRepository
            );

            if (missingCountriesIsoCodes.Any())
            {
                List<string> clearedMissingCountriesIsoCode;
                var traverser = new GlobeTraverser();
                var otherModel = traverser.TraverseGlobe(result.Globe).Where(m => m.TryAsOther() != null).FirstOrDefault();
                if (otherModel != null)
                {
                    clearedMissingCountriesIsoCode = new List<string>();
                    var codesInModel = otherModel.TryAsOther().UnsavedBasketCountries.Select(b => b.Country.IsoCode).Union(otherModel.TryAsOther().BasketCountries.Select(b => b.Basket.Country.IsoCode));
                    clearedMissingCountriesIsoCode.AddRange(codesInModel.Except(missingCountriesIsoCodes));
                }
                else
                {
                    clearedMissingCountriesIsoCode = new List<string>(missingCountriesIsoCodes);
                }

                if (clearedMissingCountriesIsoCode.Any())
                {
                    // there are some missing ISO country codes
                    // we need to resolve them to countries
                    // and put these countries to the taxonomy
                    var missingCountries = clearedMissingCountriesIsoCode.Select(x => countryRepository.GetCountry(x));
                    this.RegisterMissingCountries(result, missingCountries, computations);
                }
            }
        }

        protected void InitializeOverlay(
            RootModel root,
            SecurityRepository securityRepository,
            PortfolioRepository portfolioRepository,
            ManagingPst.PortfolioSecurityTargetRepository bottomUpPortfolioSecurityTargetRepository
        )
        {
            this.overlayInitializer.Initialize(
                root,
                securityRepository,
                portfolioRepository,
                bottomUpPortfolioSecurityTargetRepository
            );
        }

        public BaseValueResolver CreateBaseValueResolver(IDataManager manager, Int32 targetingId)
        {
            var targetingBasketBaseValues = manager.GetTargetingTypeBasketBaseValues(targetingId);
            var valueResolver = new BaseValueResolver(targetingBasketBaseValues);
            return valueResolver;
        }

        public String SerializeToJson(RootModel root, CalculationTicket ticket)
        {
            var builder = new StringBuilder();

            using (var writer = new JsonWriter(builder.ToJsonTextWriter()))
            {
                writer.Write(delegate
                {
                    this.breakdownSerializer.SerializeRoot(writer, root, ticket);

                    writer.Write(this.changeDetector.HasChanged(root), JsonNames.Unsaved);
                });
            }

            var result = builder.ToString();
            return result;
        }

        public RootModel DeserializeFromJson(String bptAsJson,
            BasketRepository basketRepository,
            SecurityRepository securityRepository,
            PortfolioRepository portfolioRepository,
            TargetingTypeRepository targetingTypeRepository
        )
        {
            using (var reader = new JsonReader(new Newtonsoft.Json.JsonTextReader(new StringReader(bptAsJson))))
            {
                var result = this.breakdownDeserializer.DeserializeRoot(
                    reader,
                    basketRepository,
                    securityRepository,
                    portfolioRepository,
                    targetingTypeRepository
                );

                
                return result;
            }
        }

        public void RegisterMissingCountries(RootModel root, IEnumerable<Country> missingCountries, Computations computations)
        {
            if (!missingCountries.Any()) return;        // hey! there's nothing to do
            var other = this.ClaimOtherModel(root);
            foreach (var country in missingCountries)
            {
				var baseExpression = this.modelBuilder.CreateBaseExpression();
				var portfolioAdjustmentExpression = this.modelBuilder.CreatePortfolioAdjustmentExpression();
                var unsavedBasketCountry = this.modelBuilder.CreateUnsavedBasketModel(
					country,
					computations,
					baseExpression,
					portfolioAdjustmentExpression
				);
                other.UnsavedBasketCountries.Add(unsavedBasketCountry);
            }
        }

        protected OtherModel ClaimOtherModel(RootModel root)
        {
            var models = this.Traverser.TraverseGlobe(root.Globe);
            var otherNodeOpt = models.Select(x => x.TryAsOther()).Where(x => x != null).SingleOrDefault();
            if (otherNodeOpt == null)
            {
                otherNodeOpt = this.modelBuilder.CreateOtherModel(
                    new List<BasketCountryModel>(),
                    new List<UnsavedBasketCountryModel>()
                );
                root.Globe.Residents.Add(otherNodeOpt);
            }
            return otherNodeOpt;
        }

        public IEnumerable<IValidationIssue> ApplyIfValid(
            RootModel root,
            String username,
            SqlConnection connection,
            TargetingTypeRepository targetingTypeRepository,
            CalculationTicket ticket,
            ref CalculationInfo info
        )
        {
            var targetingType = targetingTypeRepository.GetTargetingType(root.TargetingType.Id);
            var issues = this.modelApplier.ApplyIfValid(
                root,
                targetingType.Taxonomy,
                this.repositoryManager,
                username,
                connection,
                ticket,
                ref info
            );
            
            return issues;
        }

        public IEnumerable<IValidationIssue> Validate(RootModel root, CalculationTicket ticket)
        {
            var issues = this.modelApplier.ValidateModel(root, ticket);
            return issues;
        }


        public void RecalculateRootModel(
            RootModel root,
            ISqlConnectionFactory connectionFactory,
            IDataManagerFactory dataManagerFactory,
            CalculationTicket ticket
        )
        {
            // everything is recalculated automatically but overlays in case they were changed
#warning it's a big question whether overlays need to be recalculated every time
#warning we need to see if somebody has changed PST composition since we opened for editing but haven't yet saved

            SecurityRepository securityRepository;
            PortfolioRepository portfolioRepository;
            ManagingPst.PortfolioSecurityTargetRepository portfolioSecurityTargetRepository;
            using (var ondemandManager = new OnDemandDataManager(connectionFactory, dataManagerFactory))
            {
                securityRepository = this.repositoryManager.ClaimSecurityRepository(ondemandManager);
                portfolioRepository = this.repositoryManager.ClaimPortfolioRepository(ondemandManager);
                portfolioSecurityTargetRepository = this.repositoryManager.ClaimPortfolioSecurityTargetRepository(
                    root.LatestPstChangeset,
                    ondemandManager.Claim()
                );

                
                // in order to proceed we need to make sure all missing countries are there
                IEnumerable<BenchmarkSumByIsoInfo> benchmarks = new BenchmarkSumByIsoInfo[] { };
                var computations = this.modelBuilder.CreateComputations(root.Globe, this.Traverser);
                var countryRepository = this.repositoryManager.ClaimCountryRepository(ondemandManager);
                var targetingTypeRepository = this.repositoryManager.ClaimTargetingTypeRepository(ondemandManager);
                var targetingType = targetingTypeRepository.GetTargetingType(root.TargetingType.Id);
                this.RegisterMissingCountriesIfAny(
                    root,
                    computations,
                    countryRepository,
                    targetingType,
                    securityRepository,
                    portfolioRepository,
                    portfolioSecurityTargetRepository,
                    benchmarks
                );

            }

            

            this.InitializeOverlay(
                root,
                securityRepository,
                portfolioRepository,
                portfolioSecurityTargetRepository
            );
        }


    }
}
