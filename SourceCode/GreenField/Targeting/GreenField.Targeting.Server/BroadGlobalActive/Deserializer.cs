using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using TopDown.Core.Persisting;
using Core = TopDown.Core;

namespace GreenField.Targeting.Server.BroadGlobalActive
{
    public class Deserializer
    {
        private Server.Deserializer deserializer;
        private Core.ManagingBpt.ModelBuilder modelBuilder;
        private Core.ManagingBpt.GlobeTraverser globeTraverser;

        [DebuggerStepThrough]
        public Deserializer(
            Server.Deserializer deserializer,
            Core.ManagingBpt.ModelBuilder modelBuilder,
            Core.ManagingBpt.GlobeTraverser globeTraverser
        )
        {
            this.deserializer = deserializer;
            this.modelBuilder = modelBuilder;
            this.globeTraverser = globeTraverser;
        }

        public Core.ManagingBpt.RootModel DeserializeRoot(RootModel model)
        {
            var residents = new List<Core.ManagingBpt.IGlobeResident>();
            var globe = this.modelBuilder.CreateGlobeModel(residents);
            var computations = this.modelBuilder.CreateComputations(globe, this.globeTraverser);
            residents.AddRange(this.DeserializeGlobeResidents(model.Globe.Residents, computations));

			var factors = this.DeserializeOverlayFactors(model.Factors);
            var cash = this.modelBuilder.CreateCash(computations);
            var portfolioScaledGrandTotalExpression = this.modelBuilder.CreateAddExpression(cash.PortfolioScaled, globe.PortfolioScaled);
            var trueExposureGrandTotal = this.modelBuilder.CreateAddExpression(cash.TrueExposure, globe.TrueExposure);
            var trueActiveGrandTotal = this.modelBuilder.CreateAddExpression(cash.TrueActive, globe.TrueActive);
            var result = new Core.ManagingBpt.RootModel(
                this.deserializer.DeserializeTargetingType(model.TargetingType),
                this.deserializer.DeserializeBroadGlobalActivePorfolio(model.BroadGlobalActiveProtfolio),
                this.DeserializeTargetingTypeBasketBaseValueChangesetInfo(model.LatestTtbbvChangeset),
                this.DeserializeTargetingTypeBasketPortfolioTargetChangesetInfo(model.LatestTtbptChangeset),
                this.DeserializeBgaPortfolioSecurityFactorChangesetInfo(model.LatestBgapsfChangeset),
                this.DeserializeBuPortfolioSecurityTargetChangesetInfo(model.LatestBupstChangeset),
                globe,
                cash,
				factors,
				portfolioScaledGrandTotalExpression,
                trueExposureGrandTotal,
                trueActiveGrandTotal,
                model.BenchmarkDate,
                model.IsUserPermittedToSave
            );
            return result;
        }

		protected Core.Overlaying.RootModel DeserializeOverlayFactors(FactorModel model)
		{
			var items = model.Items.Select(x => this.DeserializeOverlayFactorsItem(x)).ToList();
			var result = new Core.Overlaying.RootModel(items);
			return result;
		}

		protected Core.Overlaying.ItemModel DeserializeOverlayFactorsItem(FactorItemModel model)
		{
			var bottomUpPortfolio = this.deserializer.DeserializeBottomUpPortfolio(model.BottomUpPortfolio);
			var overlayFactorExpression = this.modelBuilder.OverlayModelBuilder.CreateOverlayFactorExpression(bottomUpPortfolio.Name);
            this.deserializer.PopulateEditableExpression(overlayFactorExpression, model.OverlayFactor);
			var result = new Core.Overlaying.ItemModel(bottomUpPortfolio, overlayFactorExpression);
			return result;
		}

        protected IEnumerable<Core.ManagingBpt.IGlobeResident> DeserializeGlobeResidents(List<GlobeResident> models, Core.ManagingBpt.Computations computations)
        {
            var result = models.Select(x => this.DeserializeGlobeResidentOnceResolved(x, computations)).ToArray();
            return result;
        }

        protected Core.ManagingBpt.IGlobeResident DeserializeGlobeResidentOnceResolved(GlobeResident model, Core.ManagingBpt.Computations computations)
        {
            var resolver = new DeserializeGlobeResidentOnceResolved_IGlobeResidentResolver(this, computations);
            model.Accept(resolver);
            return resolver.Result;
        }

        private class DeserializeGlobeResidentOnceResolved_IGlobeResidentResolver : IGlobeResidentResolver
        {
            private Deserializer deserializer;
            private Core.ManagingBpt.Computations computations;

            public DeserializeGlobeResidentOnceResolved_IGlobeResidentResolver(Deserializer deserializer, Core.ManagingBpt.Computations computations)
            {
                this.deserializer = deserializer;
                this.computations = computations;
            }

            public Core.ManagingBpt.IGlobeResident Result { get; private set; }

            public void Resolve(RegionModel model)
            {
                this.Result = this.deserializer.DeserializeRegion(model, this.computations);
            }

            public void Resolve(BasketCountryModel model)
            {
                throw new InvalidOperationException();
            }

            public void Resolve(BasketRegionModel model)
            {
                this.Result = this.deserializer.DeserializeBasketRegion(model, this.computations);
            }

            public void Resolve(CountryModel model)
            {
                throw new InvalidOperationException();
            }

            public void Resolve(UnsavedBasketCountryModel model)
            {
                throw new InvalidOperationException();
            }

            public void Resolve(OtherModel model)
            {
                this.Result = this.deserializer.DeserializeOther(model, this.computations);
            }
        }

        protected BuPortfolioSecurityTargetChangesetInfo DeserializeBuPortfolioSecurityTargetChangesetInfo(ChangesetModel model)
        {
            var result = new BuPortfolioSecurityTargetChangesetInfo(model.Id, model.Username, model.Timestamp, model.CalculationId);
            return result;
        }

        protected BgaPortfolioSecurityFactorChangesetInfo DeserializeBgaPortfolioSecurityFactorChangesetInfo(ChangesetModel model)
        {
            var result = new BgaPortfolioSecurityFactorChangesetInfo(model.Id, model.Username, model.Timestamp, model.CalculationId);
            return result;
        }

        protected TargetingTypeBasketPortfolioTargetChangesetInfo DeserializeTargetingTypeBasketPortfolioTargetChangesetInfo(ChangesetModel model)
        {
            var result = new TargetingTypeBasketPortfolioTargetChangesetInfo(model.Id, model.Username, model.Timestamp, model.CalculationId);
            return result;
        }

        protected TargetingTypeBasketBaseValueChangesetInfo DeserializeTargetingTypeBasketBaseValueChangesetInfo(ChangesetModel model)
        {
            var result = new TargetingTypeBasketBaseValueChangesetInfo(model.Id, model.Username, model.Timestamp, model.CalculationId);
            return result;
        }

        protected Core.ManagingBpt.RegionModel DeserializeRegion(RegionModel model, Core.ManagingBpt.Computations computations)
        {
            var residents = this.DeserializeRegionResidents(model.Residents, computations);
            var result = this.modelBuilder.CreateRegionModel(
                model.Name,
                computations.BaseActiveFormula,
                residents
            );
            return result;
        }

        protected IEnumerable<Core.ManagingBpt.IRegionModelResident> DeserializeRegionResidents(IEnumerable<GlobeResident> models, Core.ManagingBpt.Computations computations)
        {
            var result = models.Select(x => this.DeserializeRegionResidentOnceResolved(x, computations)).ToArray();
            return result;
        }

        protected Core.ManagingBpt.IRegionModelResident DeserializeRegionResidentOnceResolved(GlobeResident resident, Core.ManagingBpt.Computations computations)
        {
            var resolver = new DeserializeRegionResidentOnceResolved_IGlobeResidentResolver(this, computations);
            resident.Accept(resolver);
            return resolver.Result;
        }

        private class DeserializeRegionResidentOnceResolved_IGlobeResidentResolver : IGlobeResidentResolver
        {
            private Deserializer deserializer;
            private Core.ManagingBpt.Computations computations;

            public DeserializeRegionResidentOnceResolved_IGlobeResidentResolver(Deserializer deserializer, Core.ManagingBpt.Computations computations)
            {
                this.deserializer = deserializer;
                this.computations = computations;
            }

            public Core.ManagingBpt.IRegionModelResident Result { get; private set; }

            public void Resolve(RegionModel model)
            {
                this.Result = this.deserializer.DeserializeRegion(model, this.computations);
            }

            public void Resolve(BasketCountryModel model)
            {
                this.Result = this.deserializer.DeserializeBasketCountry(model, this.computations);
            }

            public void Resolve(BasketRegionModel model)
            {
                this.Result = this.deserializer.DeserializeBasketRegion(model, this.computations);
            }

            public void Resolve(CountryModel model)
            {
                throw new InvalidOperationException();
            }

            public void Resolve(UnsavedBasketCountryModel model)
            {
                throw new InvalidOperationException();
            }

            public void Resolve(OtherModel model)
            {
                throw new InvalidOperationException();
            }
        }

        protected Core.ManagingBpt.BasketCountryModel DeserializeBasketCountry(BasketCountryModel model, Core.ManagingBpt.Computations computations)
        {
            var basket = this.deserializer.DeserializeCountryBasket(model.Basket);

            var baseExpression = this.modelBuilder.CreateBaseExpression();
            this.deserializer.PopulateEditableExpression(baseExpression, model.Base);

            var portfolioAdjustmentExpression = this.modelBuilder.CreatePortfolioAdjustmentExpression();
            this.deserializer.PopulateEditableExpression(portfolioAdjustmentExpression, model.PortfolioAdjustment);

            var result = this.modelBuilder.CreateBasketCountryModel(
                basket,
                computations,
                baseExpression,
                portfolioAdjustmentExpression
            );

            this.deserializer.PopulateUnchangableExpression(result.Benchmark, model.Benchmark);
            this.deserializer.PopulateUnchangableExpression(result.Overlay, model.Overlay);

            return result;
        }

        protected Core.ManagingBpt.BasketRegionModel DeserializeBasketRegion(BasketRegionModel model, Core.ManagingBpt.Computations computations)
        {
            var basket = this.deserializer.DeserializeRegionBasket(model.Basket);

            var baseExpression = this.modelBuilder.CreateBaseExpression();
            this.deserializer.PopulateEditableExpression(baseExpression, model.Base);

            var portfolioAdjustmentExpression = this.modelBuilder.CreatePortfolioAdjustmentExpression();
            this.deserializer.PopulateEditableExpression(portfolioAdjustmentExpression, model.PortfolioAdjustment);

            var countries = model.Countries.Select(x => this.DeserializeCountry(x)).ToArray();
            var result = this.modelBuilder.CreateBasketRegionModel(
                basket,
                countries,
                computations,
                baseExpression,
                portfolioAdjustmentExpression
            );

            return result;
        }

        protected Core.ManagingBpt.CountryModel DeserializeCountry(CountryModel model)
        {
            var country = this.deserializer.DeserializeCountry(model.Country);
            var result = this.modelBuilder.CreateCountryModel(country);
            this.deserializer.PopulateUnchangableExpression(result.Benchmark, model.Benchmark);
            this.deserializer.PopulateUnchangableExpression(result.Overlay, model.Overlay);
            return result;
        }

        protected Core.ManagingBpt.UnsavedBasketCountryModel DeserializeUnsavedBasketCountry(UnsavedBasketCountryModel model, Core.ManagingBpt.Computations computations)
        {
            var country = this.deserializer.DeserializeCountry(model.Country);
            
            var baseExpression = this.modelBuilder.CreateBaseExpression();
            this.deserializer.PopulateEditableExpression(baseExpression, model.Base);

            var portfolioAdjustmentExpression = this.modelBuilder.CreatePortfolioAdjustmentExpression();
            this.deserializer.PopulateEditableExpression(portfolioAdjustmentExpression, model.PortfolioAdjustment);
            
            var result = this.modelBuilder.CreateUnsavedBasketModel(
                country,
                computations,
                baseExpression,
                portfolioAdjustmentExpression
            );

            this.deserializer.PopulateUnchangableExpression(result.Benchmark, model.Benchmark);
            this.deserializer.PopulateUnchangableExpression(result.Overlay, model.Overlay);
            return result;
        }

        protected Core.ManagingBpt.OtherModel DeserializeOther(OtherModel model, Core.ManagingBpt.Computations computations)
        {
            var basketCountries = model.BasketCountries.Select(x => this.DeserializeBasketCountry(x, computations)).ToList();
            var unsavedBasketCountries = model.UnsavedBasketCountries.Select(x => this.DeserializeUnsavedBasketCountry(x, computations)).ToList();
            var result = this.modelBuilder.CreateOtherModel(
                basketCountries,
                unsavedBasketCountries
            );
            return result;
        }
    }
}
