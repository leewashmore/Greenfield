using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TopDown.Core.ManagingBenchmarks
{
    public class BenchmarkValueResolver
    {
        private Dictionary<String, Decimal> map;
        public BenchmarkValueResolver(IEnumerable<BenchmarkSumByIsoInfo> data)
        {
            this.map = new Dictionary<String, Decimal>();
            foreach (var item in data)
            {
                this.map.Add(item.IsoCode, item.BenchmarkWeightSum);
            }
        }
		public Decimal GetBenchmark(String isoCode)
        {
            Decimal found;
            if (this.map.TryGetValue(isoCode, out found))
            {
                return found;
            }
            else
            {
                return 0.0m;
            }
        }
    }
}
