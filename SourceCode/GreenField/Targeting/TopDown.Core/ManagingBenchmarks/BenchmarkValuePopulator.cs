using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using TopDown.Core.Geographic.BreakingDown;


namespace TopDown.Core.Geographic
{
    public class BenchmarkValuePopulator
    {
        public void Populate(BreakdownModel breakdown, BenchmarkValueResolver valueResolver)
        {
            // populating countries and regions
            var residents = breakdown.GetResidents();
            foreach (var resident in residents)
            {
                this.PopulateOnceResolved(resident, valueResolver);
            }
        }

        public void PopulateOnceResolved(IBreakdownModelResident resident, BenchmarkValueResolver valueResolver)
        {
            var resolver = new PopulateOnceResolved_IBreakdownModelResidentResolver(this, valueResolver);
            resident.Accept(resolver);
        }

        private class PopulateOnceResolved_IBreakdownModelResidentResolver : IBreakdownModelResidentResolver
        {
            private BenchmarkValuePopulator populator;
            private BenchmarkValueResolver valueResolver;

            public PopulateOnceResolved_IBreakdownModelResidentResolver(BenchmarkValuePopulator populator, BenchmarkValueResolver valueResolver)
            {
                this.populator = populator;
                this.valueResolver = valueResolver;
            }

            public void Resolve(RegionModel model)
            {
                this.populator.Populate(model, this.valueResolver);
            }

            public void Resolve(OtherModel model)
            {
                this.populator.Populate(model, this.valueResolver);
            }
        }

        public Decimal Populate(RegionModel region, BenchmarkValueResolver valueResolver)
        {
            var residents = region.GetResidents();
            var total = 0m;
            foreach (var resident in residents)
            {
                total += this.PopulateOnceResolved(resident, valueResolver);
            }
            region.Benchmark = total;
            return total;
        }

        public Decimal PopulateOnceResolved(IRegionModelResident resident, BenchmarkValueResolver valueResolver)
        {
            var resolver = new PopulateOnceResolved_IRegionModelResidentResolver(this, valueResolver);
            resident.Accept(resolver);
            return resolver.Result;
        }

        private class PopulateOnceResolved_IRegionModelResidentResolver : IRegionModelResidentResolver
        {
            private BenchmarkValuePopulator populator;
            private BenchmarkValueResolver valueResolver;

            public PopulateOnceResolved_IRegionModelResidentResolver(BenchmarkValuePopulator populator, BenchmarkValueResolver valueResolver)
            {
                this.populator = populator;
                this.valueResolver = valueResolver;
            }

            public Decimal Result { get; private set; }

            public void Resolve(BasketCountryModel model)
            {
                this.Result = this.populator.Populate(model, this.valueResolver);
            }

            public void Resolve(BasketRegionModel model)
            {
                this.Result = this.populator.Populate(model, this.valueResolver);
            }

            public void Resolve(RegionModel model)
            {
                this.Result = this.populator.Populate(model, this.valueResolver);
            }
        }

        public Decimal Populate(OtherModel other, BenchmarkValueResolver valueResolver)
        {
            var total = 0m;
            var residents = other.GetResidents();
            foreach (var resident in residents)
            {
                total += this.PopulateOnceResolved(resident, valueResolver);
            }
            other.Benchmark = total;
            return total;
        }

        public Decimal PopulateOnceResolved(IOtherModelResident resident, BenchmarkValueResolver valueResolver)
        {
            var resolver = new PopulateOnceResolved_IOtherModelResidentResolver(this, valueResolver);
            resident.Accept(resolver);
            return resolver.Result;
        }

        private class PopulateOnceResolved_IOtherModelResidentResolver : IOtherModelResidentResolver
        {
            private BenchmarkValuePopulator populator;
            private BenchmarkValueResolver valueResolver;

            public PopulateOnceResolved_IOtherModelResidentResolver(BenchmarkValuePopulator populator, BenchmarkValueResolver valueResolver)
            {
                this.populator = populator;
                this.valueResolver = valueResolver;
            }

            public Decimal Result { get; private set; }

            public void Resolve(BasketCountryModel basketCountry)
            {
                this.Result = this.populator.Populate(basketCountry, this.valueResolver);
            }

            public void Resolve(UnsavedBasketCountryModel unsavedBasketCountry)
            {
                this.Result = this.populator.Populate(unsavedBasketCountry, this.valueResolver);
            }
        }

        public Decimal Populate(BasketCountryModel basketCountry, BenchmarkValueResolver valueResolver)
        {
            var value = valueResolver.GetBenchmark(basketCountry.Country.IsoCode.Value);
            basketCountry.Benchmark = value;
            return value;
        }

        public Decimal Populate(BasketRegionModel basketRegion, BenchmarkValueResolver valueResolver)
        {
            var total = 0m;
            var countries = basketRegion.GetCountries();
            foreach (var country in countries)
            {
                total += this.Populate(country, valueResolver);
            }
            return total;
        }

        public Decimal Populate(CountryModel country, BenchmarkValueResolver valueResolver)
        {
            var value = valueResolver.GetBenchmark(country.Country.IsoCode.Value);
            country.Benchmark = value;
            return value;
        }

        public Decimal Populate(UnsavedBasketCountryModel unsavedBasketCountry, BenchmarkValueResolver valueResolver)
        {
            var value = valueResolver.GetBenchmark(unsavedBasketCountry.Country.IsoCode.Value);
            unsavedBasketCountry.Benchmark = value;
            return value;
        }
    }
}
