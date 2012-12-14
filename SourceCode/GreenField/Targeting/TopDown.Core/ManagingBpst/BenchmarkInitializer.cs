using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopDown.Core.ManagingBaskets;
using TopDown.Core.ManagingBenchmarks;

namespace TopDown.Core.ManagingBpst
{
    public class BenchmarkInitializer
    {
        public void InitializeCore(
            IBasket basket,
            String benchmarkId,
            CoreModel core,
            ManagingBenchmarks.BenchmarkRepository benchmarkRepository
        )
        {
            foreach (var securityModel in core.Securities)
            {
                this.InitializeSecurity(basket, benchmarkId, securityModel, benchmarkRepository);
            }
        }

        public void InitializeSecurity(
            IBasket basket,
            String benchmarkId,
            SecurityModel security,
            ManagingBenchmarks.BenchmarkRepository benchmarkRepository
        )
        {
            var totalOpt = this.TryGetTotalByBasketOnceResolved(basket, benchmarkId, benchmarkRepository);
            
            var benchmarkInfoOpt = benchmarkRepository.TryGetBySecurity(security.Security);
            if (benchmarkInfoOpt != null && benchmarkInfoOpt.BenchmarkWeight.HasValue && benchmarkInfoOpt.BenchmarkId == benchmarkId)
            {
                if (totalOpt.HasValue)
                {
                    security.Benchmark.InitialValue = benchmarkInfoOpt.BenchmarkWeight.Value / totalOpt.Value;
                }
                else
                {
#warning When you have time think about this:
                    security.Benchmark.InitialValue = 0.0m;
                }
            }
        }

        protected Decimal? TryGetTotalByBasketOnceResolved(IBasket basket, String benchmarkId, BenchmarkRepository benchmarkRepository)
        {
            var resolver = new TryGetTotalByBasket_IBasketResolver(this, benchmarkRepository, benchmarkId);
            basket.Accept(resolver);
            return resolver.Result;

        }

        private class TryGetTotalByBasket_IBasketResolver : IBasketResolver
        {
            private BenchmarkRepository benchmarkRepository;
            private BenchmarkInitializer initializer;
            private string benchmarkId;

            public TryGetTotalByBasket_IBasketResolver(BenchmarkInitializer initializer, BenchmarkRepository benchmarkRepository, String benchmarkId)
            {
                this.benchmarkRepository = benchmarkRepository;
                this.initializer = initializer;
                this.benchmarkId = benchmarkId;
            }

            public Decimal? Result { get; private set; }

            public void Resolve(CountryBasket basket)
            {
                this.Result = this.initializer.TryGetTotalByBasket(basket, benchmarkRepository, this.benchmarkId);
            }

            public void Resolve(RegionBasket basket)
            {
                this.Result = this.initializer.TryGetTotalByBasket(basket, benchmarkRepository, this.benchmarkId);
            }

        }

        protected Decimal? TryGetTotalByBasket(CountryBasket basket, BenchmarkRepository benchmarkRepository, String benchmarkId)
        {
            var country = basket.Country;
            var result = this.TryGetTotalByCountry(country, benchmarkRepository, benchmarkId);
            return result;
        }

        protected Decimal? TryGetTotalByCountry(Aims.Core.Country country, BenchmarkRepository benchmarkRepository, String benchmarkId)
        {
            var benchmarkRecords = benchmarkRepository
                .GetBenchmarksByIsoCountryCode(country.IsoCode)
                .Where(x => x.BenchmarkId == benchmarkId);

            if (benchmarkRecords.Any())
            {
                Decimal? result = null;
                foreach (var record in benchmarkRecords)
                {
                    var value = record.BenchmarkWeight;
                    if (value.HasValue)
                    {
                        if (result.HasValue)
                        {
                            result = result.Value + value.Value;
                        }
                        else
                        {
                            result = value.Value;
                        }
                    }
                    else
                    {
                        // do nothing
                    }
                }
                return result;
            }
            else
            {
                return null;
            }
        }

        protected Decimal? TryGetTotalByBasket(RegionBasket basket, BenchmarkRepository benchmarkRepository, String benchmarkId)
        {
            Decimal? result = null;
            foreach (var country in basket.Countries)
            {
                var value = TryGetTotalByCountry(country, benchmarkRepository, benchmarkId);
                if (value.HasValue)
                {
                    if (result.HasValue)
                    {
                        result = result.Value + value.Value;
                    }
                    else
                    {
                        result = value.Value;
                    }
                }
                else
                {
                    // do nothing
                }
            }
            return result;
        }
    }
}
