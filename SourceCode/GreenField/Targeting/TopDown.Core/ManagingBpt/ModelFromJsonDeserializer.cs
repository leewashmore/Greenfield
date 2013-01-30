using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopDown.Core.ManagingBaskets;
using TopDown.Core.ManagingCountries;
using TopDown.Core.Persisting;
using TopDown.Core.ManagingSecurities;
using TopDown.Core.ManagingPortfolios;
using TopDown.Core.ManagingTargetingTypes;
using Aims.Core;

namespace TopDown.Core.ManagingBpt
{
    public class ModelFromJsonDeserializer
    {
        private ExpressionPicker picker;
#warning DOn't need these
        private ModelBuilder modelBuilder;
        private GlobeTraverser traverser;
#warning DOn't need these
        private ExpressionFromJsonDeserializer expressionDeserializer;

        public ModelFromJsonDeserializer(
            ExpressionPicker picker,
            ModelBuilder modelBuilder,
            GlobeTraverser traverser,
            ExpressionFromJsonDeserializer expressionDeserializer
        )
        {
            this.picker = picker;
            this.modelBuilder = modelBuilder;
            this.traverser = traverser;
            this.expressionDeserializer = expressionDeserializer;
        }


        public RootModel DeserializeRoot(
            JsonReader reader,
            BasketRepository basketRepository,
            SecurityRepository securityRepository,
            PortfolioRepository portfolioRepository,
            TargetingTypeRepository targetingTypeRepository
        )
        {
            return reader.Read(delegate
            {
                var residents = new List<IGlobeResident>();
                var globe = this.modelBuilder.CreateGlobeModel(residents);

                var computations = this.modelBuilder.CreateComputations(globe, this.traverser);

                var targetingTypeId = reader.ReadAsInt32(JsonNames.TargetingTypeId);
                var portfolioId = reader.ReadAsString(JsonNames.PortfolioId);
                var benchamrkDate = reader.ReadAsDatetime(JsonNames.BenchmarkDate);

                var latestChangesets = reader.Read(JsonNames.LatestChangesets, delegate
                {
                    return new
                    {
                        TargetingTypeBasketBaseValue = reader.Read(JsonNames.TargetingTypeBasketBaseValue, delegate
                        {
                            return this.DeserializeTargetingTypeBasketBaseValueChangeset(reader);
                        }),
                        TargetingTypeBasketPortfolioTarget = reader.Read(JsonNames.TargetingTypeBasketPortfolioTarget, delegate
                        {
                            return this.DeserializeTargetingTypeBasketPortfolioTargetChangeset(reader);
                        }),
                        PortfolioSecurityTargetOverlay = reader.Read(JsonNames.PortfolioSecurityTargetOverlay, delegate
                        {
                            return this.DeserializePortfolioSecurityTargetOverlayChangeset(reader);
                        }),
                        PortfolioSecurityTarget = reader.Read(JsonNames.PortfolioSecurityTarget, delegate
                        {
                            return this.DeserializePortfolioSecurityTargetChangeset(reader);
                        })
                    };
                });

                reader.Read(JsonNames.Root, delegate
                {
                    reader.ReadArray(JsonNames.Residents, delegate
                    {
                        var resident = this.DeserializeBreakdownResidentModelOnceResolver(basketRepository, reader, computations);
                        residents.Add(resident);
                    });
                });

                var factors = reader.Read(JsonNames.Overlay, delegate
                {
                    return this.DeserializeOverlay(reader, securityRepository, portfolioRepository);
                });

                var cash = this.modelBuilder.CreateCash(computations);
                var portfolioScaledTotal = this.modelBuilder.CreateAddExpression(cash.PortfolioScaled, globe.PortfolioScaled);
                var targetingType = targetingTypeRepository.GetTargetingType(targetingTypeId);
                var portfolio = portfolioRepository.GetBroadGlobalActivePortfolio(portfolioId);
                var trueExposureGrandTotal = this.modelBuilder.CreateAddExpression(cash.TrueExposure, globe.TrueExposure);
                var trueActiveGrandTotal = this.modelBuilder.CreateAddExpression(cash.TrueActive, globe.TrueActive);

                var result = new RootModel(
                    targetingType,
                    portfolio,
                    latestChangesets.TargetingTypeBasketBaseValue,
                    latestChangesets.TargetingTypeBasketPortfolioTarget,
                    latestChangesets.PortfolioSecurityTargetOverlay,
                    latestChangesets.PortfolioSecurityTarget,
                    globe,
                    cash,
                    factors,
                    portfolioScaledTotal,
                    trueExposureGrandTotal,
                    trueActiveGrandTotal,
                    benchamrkDate,
                    false
                );
                return result;
            });
        }





#warning Same serialization/deserializetion routines are used for each changeset that does roundtrip to the client.

        protected TargetingTypeBasketPortfolioTargetChangesetInfo DeserializeTargetingTypeBasketPortfolioTargetChangeset(JsonReader reader)
        {
            var result = new TargetingTypeBasketPortfolioTargetChangesetInfo(
                reader.ReadAsInt32(JsonNames.Id),
                reader.ReadAsString(JsonNames.Username),
                reader.ReadAsDatetime(JsonNames.Timestamp),
                reader.ReadAsInt32(JsonNames.CalcualtionId)
            );
            return result;
        }

        protected BuPortfolioSecurityTargetChangesetInfo DeserializePortfolioSecurityTargetChangeset(JsonReader reader)
        {
            var result = new BuPortfolioSecurityTargetChangesetInfo(
                reader.ReadAsInt32(JsonNames.Id),
                reader.ReadAsString(JsonNames.Username),
                reader.ReadAsDatetime(JsonNames.Timestamp),
                reader.ReadAsInt32(JsonNames.CalcualtionId)
            );
            return result;
        }

        protected TargetingTypeBasketBaseValueChangesetInfo DeserializeTargetingTypeBasketBaseValueChangeset(JsonReader reader)
        {
            var result = new TargetingTypeBasketBaseValueChangesetInfo(
                reader.ReadAsInt32(JsonNames.Id),
                reader.ReadAsString(JsonNames.Username),
                reader.ReadAsDatetime(JsonNames.Timestamp),
                reader.ReadAsInt32(JsonNames.CalcualtionId)
            );
            return result;
        }

        protected BgaPortfolioSecurityFactorChangesetInfo DeserializePortfolioSecurityTargetOverlayChangeset(JsonReader reader)
        {
            var result = new BgaPortfolioSecurityFactorChangesetInfo(
                reader.ReadAsInt32(JsonNames.Id),
                reader.ReadAsString(JsonNames.Username),
                reader.ReadAsDatetime(JsonNames.Timestamp),
                reader.ReadAsInt32(JsonNames.CalcualtionId)
            );
            return result;
        }

        protected Overlaying.RootModel DeserializeOverlay(
            JsonReader reader,
            SecurityRepository securityRepository,
            PortfolioRepository portfolioRepository
        )
        {
            var items = reader.ReadArray(JsonNames.Items, delegate
            {
                return this.DeserializeOverlayItem(
                    reader,
                    securityRepository,
                    portfolioRepository
                );
            });

            var result = new Overlaying.RootModel(items);
            return result;
        }

        protected Overlaying.ItemModel DeserializeOverlayItem(
            JsonReader reader,
            SecurityRepository securityRepository,
            PortfolioRepository portfolioRepository
        )
        {
            var portfolioId = reader.ReadAsString(JsonNames.PortfolioId);
            var bottomUpPortfolio = portfolioRepository.GetBottomUpPortfolio(portfolioId);

            var expression = this.modelBuilder.OverlayModelBuilder.CreateOverlayFactorExpression(bottomUpPortfolio.Name);
            reader.Read(JsonNames.OverlayFactor, delegate
            {
                this.expressionDeserializer.PopulateEditableExpression(reader, expression);
            });

            var result = new Overlaying.ItemModel(bottomUpPortfolio, expression);
            return result;
        }



        protected BasketRegionModel DeserializeBasketRegionModel(BasketRepository basketRepository, JsonReader reader, Computations computations)
        {
            var basketId = reader.ReadAsInt32(JsonNames.BasketId);
            var basket = basketRepository.GetBasket(basketId).AsRegionBasket();

			var baseExpression = this.modelBuilder.CreateBaseExpression();
            reader.Read(JsonNames.Base, delegate
            {
                this.expressionDeserializer.PopulateEditableExpression(reader, baseExpression);
            });

            var portfolioAdjustmentExpression = this.modelBuilder.CreatePortfolioAdjustmentExpression();
            reader.Read(JsonNames.PortfolioAdjustment, delegate
            {
                this.expressionDeserializer.PopulateEditableExpression(reader, portfolioAdjustmentExpression);
            });

            var countries = reader.ReadArray(JsonNames.Countries, delegate
            {
                return this.DeserializeCountryModel(reader);
            });

            var result = this.modelBuilder.CreateBasketRegionModel(
                basket,
                countries,
                computations,
                baseExpression,
                portfolioAdjustmentExpression
            );

            return result;
        }

        protected BasketCountryModel DeserializeBasketCountryModel(BasketRepository basketRepository, JsonReader reader, Computations computations)
        {
            var basketId = reader.ReadAsInt32(JsonNames.BasketId);

            var country = reader.Read(JsonNames.Country, delegate
            {
                return this.DeserializeCountry(reader);
            });

            var basket = basketRepository.GetBasket(basketId).AsCountryBasket();
            var result = this.modelBuilder.CreateBasketCountryModel(
                basket,
                computations,
                this.modelBuilder.CreateBaseExpression(),
                this.modelBuilder.CreatePortfolioAdjustmentExpression()
            );

            reader.Read(JsonNames.Benchmark, delegate
            {
                this.expressionDeserializer.PopulateUnchangeableExpression(reader, result.Benchmark);
            });

            reader.Read(JsonNames.Base, delegate
            {
                this.expressionDeserializer.PopulateEditableExpression(reader, result.Base);
            });

            reader.Read(JsonNames.Overlay, delegate
            {
                this.expressionDeserializer.PopulateUnchangeableExpression(reader, result.Overlay);
            });

            reader.Read(JsonNames.PortfolioAdjustment, delegate
            {
                this.expressionDeserializer.PopulateEditableExpression(reader, result.PortfolioAdjustment);
            });

            return result;
        }

        protected UnsavedBasketCountryModel DeserializeUnsavedBasketCountryModel(JsonReader reader, Computations computations)
        {
            //var basketId = reader.ReadAsInt32(JsonNames.BasketId);

            var country = reader.Read(JsonNames.Country, delegate
            {
                return this.DeserializeCountry(reader);
            });

            //var basket = basketRepository.GetBasket(basketId).AsCountryBasket();
            var result = this.modelBuilder.CreateUnsavedBasketModel(
                country,
                computations,
                this.modelBuilder.CreateBaseExpression(),
                this.modelBuilder.CreatePortfolioAdjustmentExpression()
            );

            reader.Read(JsonNames.Benchmark, delegate
            {
                this.expressionDeserializer.PopulateUnchangeableExpression(reader, result.Benchmark);
            });

            reader.Read(JsonNames.Base, delegate
            {
                this.expressionDeserializer.PopulateEditableExpression(reader, result.Base);
            });

            reader.Read(JsonNames.Overlay, delegate
            {
                this.expressionDeserializer.PopulateUnchangeableExpression(reader, result.Overlay);
            });

            reader.Read(JsonNames.PortfolioAdjustment, delegate
            {
                this.expressionDeserializer.PopulateEditableExpression(reader, result.PortfolioAdjustment);
            });

            return result;
        }



        protected RegionModel DeserializeRegionModel(BasketRepository basketRepository, JsonReader reader, Computations computations)
        {
            var name = reader.ReadAsString("name");

            var residents = new List<IRegionModelResident>();
            reader.ReadArray(JsonNames.Residents, delegate
            {
                var resident = this.DeserializeRegionModelResidentOnceResolved(basketRepository, reader, computations);
                residents.Add(resident);
            });

            var result = this.modelBuilder.CreateRegionModel(
                name,
                computations.BaseActiveFormula,
                residents
            );
            return result;
        }

        protected OtherModel DeserializeOtherModel(BasketRepository basketRepository, JsonReader reader, Computations computations)
        {
            var basketCountries = new List<BasketCountryModel>();
            var unsavedBasketCountries = new List<UnsavedBasketCountryModel>();
            

            reader.ReadArray(JsonNames.BasketCountries, delegate
            {
                reader.ReadAsString(JsonNames.Discriminator);
                var basketCountry = this.DeserializeBasketCountryModel(basketRepository, reader, computations);
                basketCountries.Add(basketCountry);
            });

            reader.ReadArray(JsonNames.UnsavedBasketCountries, delegate
            {
                reader.ReadAsString(JsonNames.Discriminator);
                var unsavedBasketCountry = this.DeserializeUnsavedBasketCountryModel(reader, computations);
                unsavedBasketCountries.Add(unsavedBasketCountry);
            });

            var result = this.modelBuilder.CreateOtherModel(
                basketCountries,
                unsavedBasketCountries
            );
            return result;
        }

        protected CountryModel DeserializeCountryModel(JsonReader reader)
        {
            var country = reader.Read(JsonNames.Country, delegate
            {
                return this.DeserializeCountry(reader);
            });

			var result = this.modelBuilder.CreateCountryModel(country);

            reader.Read(JsonNames.Benchmark, delegate
            {
                result.Benchmark.InitialValue = reader.ReadAsDecimal(JsonNames.Value);
            });

            reader.Read(JsonNames.Overlay, delegate
            {
                result.Overlay.InitialValue = reader.ReadAsDecimal(JsonNames.Value);
            });

            return result;
        }

        protected Country DeserializeCountry(JsonReader reader)
        {
            var isoCode = reader.ReadAsString(JsonNames.IsoCode);
            var name = reader.ReadAsString(JsonNames.Name);
            var country = new Country(isoCode, name);
            return country;
        }



        protected IGlobeResident DeserializeBreakdownResidentModelOnceResolver(BasketRepository basketRepository, JsonReader reader, Computations computations)
        {
            IGlobeResident resident;
            var discriminator = reader.ReadAsString(JsonNames.Discriminator);
            if (discriminator == JsonNames.BasketRegion)
            {
                resident = this.DeserializeBasketRegionModel(basketRepository, reader, computations);
            }
            else if (discriminator == JsonNames.Region)
            {
                resident = this.DeserializeRegionModel(basketRepository, reader, computations);
            }
            else if (discriminator == JsonNames.Other)
            {
                resident = this.DeserializeOtherModel(basketRepository, reader, computations);
            }
            else
            {
                throw new ApplicationException("Unexpected discriminator \"" + discriminator + "\".");
            }
            return resident;
        }

        protected IRegionModelResident DeserializeRegionModelResidentOnceResolved(BasketRepository basketRepository, JsonReader reader, Computations computations)
        {
            IRegionModelResident result;
            var discriminator = reader.ReadAsString(JsonNames.Discriminator);

            if (discriminator == JsonNames.BasketRegion)
            {
                result = this.DeserializeBasketRegionModel(basketRepository, reader, computations);
            }
            else if (discriminator == JsonNames.BasketCountry)
            {
                result = this.DeserializeBasketCountryModel(basketRepository, reader, computations);
            }
            else if (discriminator == JsonNames.Region)
            {
                result = this.DeserializeRegionModel(basketRepository, reader, computations);
            }
            else
            {
                throw new ApplicationException("Unexpected discriminator \"" + discriminator + "\".");
            }

            return result;
        }
    }
}
