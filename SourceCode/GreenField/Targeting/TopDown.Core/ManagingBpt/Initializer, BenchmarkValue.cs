using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopDown.Core.ManagingBenchmarks;

namespace TopDown.Core.ManagingBpt
{
    public class BenchmarkValueInitializer
    {
        private GlobeTraverser traverser;

        public BenchmarkValueInitializer(GlobeTraverser traverser)
        {
            this.traverser = traverser;
        }

        public void Initialize(RootModel breakdown, BenchmarkValueResolver valueResolver)
        {
            var models = this.traverser.TraverseGlobe(breakdown.Globe);
            foreach (var model in models)
            {
                this.InitializeOnceResolved(model, valueResolver);
            }
        }

        private void InitializeOnceResolved(IModel model, BenchmarkValueResolver valueResolver)
        {
            model.Accept(new InitializeMultimethod(this, valueResolver));
        }

        private class InitializeMultimethod : IModelResolver
        {
            private BenchmarkValueInitializer initializer;
            private BenchmarkValueResolver valueResolver;

            public InitializeMultimethod(BenchmarkValueInitializer initializer, BenchmarkValueResolver valueResolver)
            {
                this.initializer = initializer;
                this.valueResolver = valueResolver;
            }

            public void Resolve(BasketCountryModel model)
            {
                model.Benchmark.InitialValue = this.valueResolver.GetBenchmark(model.Basket.Country.IsoCode);
            }

            public void Resolve(BasketRegionModel model)
            {
                // do nothing
            }

            public void Resolve(CountryModel model)
            {
                model.Benchmark.InitialValue = this.valueResolver.GetBenchmark(model.Country.IsoCode);
            }

            public void Resolve(OtherModel model)
            {
                // do nothing
            }

            public void Resolve(RegionModel model)
            {
                // do nothing
            }

            public void Resolve(UnsavedBasketCountryModel model)
            {
                model.Benchmark.InitialValue = this.valueResolver.GetBenchmark(model.Country.IsoCode);
            }
        }
    }
}
