using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aims.Expressions;
using Core = TopDown.Core;

namespace GreenField.Targeting.Server.BroadGlobalActive
{
    public class Serializer
    {
        private Server.Serializer serializer;
        private Core.ManagingBpt.ModelChangeDetector changeDetector;

        public Serializer(
            GreenField.Targeting.Server.Serializer serializer,
            Core.ManagingBpt.ModelChangeDetector changeDetector
        )
        {
            this.serializer = serializer;
            this.changeDetector = changeDetector;
        }

        public RootModel SerializeRoot(Core.ManagingBpt.RootModel model, CalculationTicket ticket)
        {
            var hasBeenChanged = this.changeDetector.HasChanged(model);

            var result = new RootModel(
                this.serializer.SerializeTargetingType(model.TargetingType),
                this.serializer.SerializeBroadGlobalActivePorfolio(model.Portfolio),
                this.serializer.SerializeChangeset(model.LatestTtbbvChangeset),
                this.serializer.SerializeChangeset(model.LatestTtbptChangeset),
                this.serializer.SerializeChangeset(model.LatestPstoChangeset),
                this.serializer.SerializeChangeset(model.LatestPstChangeset),
                this.SerializeGlobe(model.Globe, ticket),
                this.SerializeCash(model.Cash, ticket),
                model.BenchmarkDate,
                hasBeenChanged,
                this.serializer.SerializeNullableExpression(model.PortfolioScaledGrandTotal, ticket),
                this.serializer.SerializeNullableExpression(model.TrueExposureGrandTotal, ticket),
                this.serializer.SerializeNullableExpression(model.TrueActiveGrandTotal, ticket),
                model.IsUserPermittedToSave
            );
            result.Factors = this.SerializeFactors(model.Factors);
            return result;
        }

        protected CashModel SerializeCash(Core.ManagingBpt.CashModel model, CalculationTicket ticket)
        {
            var result = new CashModel(
                this.serializer.SerializeNullableExpression(model.Base, ticket),
                this.serializer.SerializeNullableExpression(model.PortfolioScaled, ticket),
                this.serializer.SerializeNullableExpression(model.TrueExposure, ticket),
                this.serializer.SerializeNullableExpression(model.TrueActive, ticket)
            );
            return result;
        }

        protected GlobeModel SerializeGlobe(Core.ManagingBpt.GlobeModel model, CalculationTicket ticket)
        {
            var residents = this.SerializeGlobeResidents(model.Residents, ticket);
            var result = new GlobeModel(
                residents,
                this.serializer.SerializeNullableExpression(model.Base, ticket),
                this.serializer.SerializeExpression(model.Benchmark, ticket),
                this.serializer.SerializeExpression(model.Overlay, ticket),
                this.serializer.SerializeNullableExpression(model.PortfolioAdjustment, ticket),
                this.serializer.SerializeNullableExpression(model.PortfolioScaled, ticket),
                this.serializer.SerializeNullableExpression(model.TrueActive, ticket),
                this.serializer.SerializeNullableExpression(model.TrueExposure, ticket)
            );
            return result;
        }

        protected IEnumerable<GlobeResident> SerializeGlobeResidents(ICollection<Core.ManagingBpt.IGlobeResident> residents, CalculationTicket ticket)
        {
            return residents.Select(x => this.SerializeGlobeResidetOnceResolved(x, ticket)).ToArray();
        }

        protected GlobeResident SerializeGlobeResidetOnceResolved(Core.ManagingBpt.IGlobeResident resident, CalculationTicket ticket)
        {
            var resolver = new SerializeGlobeResidetOnceResolved_IBreakdownModelResidentResolver(this, ticket);
            resident.Accept(resolver);
            return resolver.Result;

        }

        private class SerializeGlobeResidetOnceResolved_IBreakdownModelResidentResolver : Core.ManagingBpt.IGlobeResidentResolver
        {
            private CalculationTicket ticket;
            private Serializer serializer;

            public SerializeGlobeResidetOnceResolved_IBreakdownModelResidentResolver(Serializer serializer, CalculationTicket ticket)
            {
                this.ticket = ticket;
                this.serializer = serializer;
            }

            public GlobeResident Result { get; private set; }

            public void Resolve(Core.ManagingBpt.RegionModel model)
            {
                this.Result = this.serializer.SerializeRegion(model, ticket);
            }

            public void Resolve(Core.ManagingBpt.BasketRegionModel model)
            {
                this.Result = this.serializer.SerializeBasketRegion(model, ticket);
            }

            public void Resolve(Core.ManagingBpt.OtherModel model)
            {
                this.Result = this.serializer.SerializeOtherRegion(model, ticket);
            }
        }

        protected RegionModel SerializeRegion(Core.ManagingBpt.RegionModel model, CalculationTicket ticket)
        {
            var result = new RegionModel(
                this.serializer.SerializeNullableExpression(model.Base, ticket),
                this.serializer.SerializeNullableExpression(model.BaseActive, ticket),
                this.serializer.SerializeExpression(model.Benchmark, ticket),
                model.Name,
                this.serializer.SerializeExpression(model.Overlay, ticket),
                this.serializer.SerializeNullableExpression(model.PortfolioAdjustment, ticket),
                this.serializer.SerializeNullableExpression(model.PortfolioScaled, ticket),
                this.SerializeRegionResidents(model.Residents, ticket),
                this.serializer.SerializeNullableExpression(model.TrueActive, ticket),
                this.serializer.SerializeNullableExpression(model.TrueExposure, ticket)
            );
            return result;
        }

        protected IEnumerable<GlobeResident> SerializeRegionResidents(IEnumerable<Core.ManagingBpt.IRegionModelResident> models, CalculationTicket ticket)
        {
            var result = models.Select(x => this.SerializeRegionResidentOnceResolved(x, ticket)).ToArray();
            return result;
        }

        private GlobeResident SerializeRegionResidentOnceResolved(Core.ManagingBpt.IRegionModelResident model, CalculationTicket ticket)
        {
            var resolver = new SerializeRegionResidentOnceResolved_IRegionModelResidentResolver(this, ticket);
            model.Accept(resolver);
            return resolver.Result;
        }

        private class SerializeRegionResidentOnceResolved_IRegionModelResidentResolver : TopDown.Core.ManagingBpt.IRegionModelResidentResolver
        {
            private Serializer serializer;
            private CalculationTicket ticket;

            public SerializeRegionResidentOnceResolved_IRegionModelResidentResolver(Serializer serializer, CalculationTicket ticket)
            {
                this.ticket = ticket;
                this.serializer = serializer;
            }
            public GlobeResident Result { get; private set; }

            public void Resolve(Core.ManagingBpt.BasketCountryModel model)
            {
                this.Result = this.serializer.SerializeBasketCountry(model, ticket);
            }

            public void Resolve(Core.ManagingBpt.BasketRegionModel model)
            {
                this.Result = this.serializer.SerializeBasketRegion(model, ticket);
            }

            public void Resolve(Core.ManagingBpt.RegionModel model)
            {
                this.Result = this.serializer.SerializeRegion(model, ticket);
            }
        }

        protected BasketRegionModel SerializeBasketRegion(Core.ManagingBpt.BasketRegionModel model, CalculationTicket ticket)
        {
            var result = new BasketRegionModel(
                this.serializer.SerializeEditableExpression(model.Base),
               this.serializer.SerializeNullableExpression(model.BaseActive, ticket),
                this.serializer.SerializeRegionBasket(model.Basket),
                this.serializer.SerializeExpression(model.Benchmark, ticket),
                this.SerializeCountries(model.Countries, ticket),
                this.serializer.SerializeExpression(model.Overlay, ticket),
                this.serializer.SerializeEditableExpression(model.PortfolioAdjustment),
                this.serializer.SerializeNullableExpression(model.PortfolioScaled, ticket),
                this.serializer.SerializeNullableExpression(model.TrueActive, ticket),
                this.serializer.SerializeNullableExpression(model.TrueExposure, ticket)
            );
            return result;
        }

        protected IEnumerable<CountryModel> SerializeCountries(IEnumerable<Core.ManagingBpt.CountryModel> models, CalculationTicket ticket)
        {
            var result = models.Select(x => this.SerializeCountry(x, ticket)).ToArray();
            return result;
        }

        protected CountryModel SerializeCountry(Core.ManagingBpt.CountryModel model, CalculationTicket ticket)
        {
            var result = new CountryModel(
                this.serializer.SerializeExpression(model.Benchmark, ticket),
                this.serializer.SerializeCountry(model.Country),
                this.serializer.SerializeExpression(model.Overlay, ticket)
            );
            return result;
        }

        protected OtherModel SerializeOtherRegion(Core.ManagingBpt.OtherModel model, CalculationTicket ticket)
        {
            var result = new OtherModel(
                this.serializer.SerializeNullableExpression(model.Base, ticket),
                this.serializer.SerializeNullableExpression(model.BaseActive, ticket),
                model.BasketCountries.Select(x => this.SerializeBasketCountry(x, ticket)).ToArray(),
                this.serializer.SerializeExpression(model.Benchmark, ticket),
                this.serializer.SerializeExpression(model.Overlay, ticket),
                this.serializer.SerializeNullableExpression(model.PortfolioAdjustment, ticket),
                this.serializer.SerializeNullableExpression(model.PortfolioScaled, ticket),
                this.serializer.SerializeNullableExpression(model.TrueActive, ticket),
                this.serializer.SerializeNullableExpression(model.TrueExposure, ticket),
                model.UnsavedBasketCountries.Select(x => this.SerializeUnsavedBasketCountry(x, ticket)).ToArray()
            );
            return result;
        }

        protected UnsavedBasketCountryModel SerializeUnsavedBasketCountry(Core.ManagingBpt.UnsavedBasketCountryModel model, CalculationTicket ticket)
        {
            var result = new UnsavedBasketCountryModel(
                this.serializer.SerializeEditableExpression(model.Base),
                this.serializer.SerializeNullableExpression(model.BaseActive, ticket),
                this.serializer.SerializeExpression(model.Benchmark, ticket),
                this.serializer.SerializeCountry(model.Country),
                this.serializer.SerializeExpression(model.Overlay, ticket),
                this.serializer.SerializeEditableExpression(model.PortfolioAdjustment),
                this.serializer.SerializeNullableExpression(model.PortfolioScaled, ticket),
                this.serializer.SerializeNullableExpression(model.TrueActive, ticket),
                this.serializer.SerializeNullableExpression(model.TrueExposure, ticket)
            );
            return result;
        }

        protected BasketCountryModel SerializeBasketCountry(Core.ManagingBpt.BasketCountryModel model, CalculationTicket ticket)
        {
            var result = new BasketCountryModel(
                this.serializer.SerializeEditableExpression(model.Base),
                this.serializer.SerializeNullableExpression(model.BaseActive, ticket),
                this.serializer.SerializeCountryBasket(model.Basket),
                this.serializer.SerializeExpression(model.Benchmark, ticket),
                this.serializer.SerializeExpression(model.Overlay, ticket),
                this.serializer.SerializeEditableExpression(model.PortfolioAdjustment),
                this.serializer.SerializeNullableExpression(model.PortfolioScaled, ticket),
                this.serializer.SerializeNullableExpression(model.TrueActive, ticket),
                this.serializer.SerializeNullableExpression(model.TrueExposure, ticket)
            );
            return result;
        }

        protected FactorModel SerializeFactors(Core.Overlaying.RootModel model)
        {
            var result = new FactorModel();
            foreach (var item in model.Items)
            {
                result.Items.Add(this.SerializeFactorItem(item));
            }
            return result;
        }

        protected FactorItemModel SerializeFactorItem(Core.Overlaying.ItemModel item)
        {
            var result = new FactorItemModel(
                this.serializer.SerializeBottomUpPortfolio(item.BottomUpPortfolio),
                this.serializer.SerializeEditableExpression(item.OverlayFactor)
            );
            return result;
        }

        public IEnumerable<Picker.TargetingTypeModel> SerializePicker(Core.Gadgets.PortfolioPicker.RootModel model)
        {
            return model.TargetingTypes.Select(x => new Picker.TargetingTypeModel(x.Id, x.Name, x.Portfolios.Select(y => new Picker.PortfolioModel(y.Id, y.Name)))).ToArray();
        }
    }
}
