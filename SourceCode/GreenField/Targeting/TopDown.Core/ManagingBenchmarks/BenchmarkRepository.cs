using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopDown.Core.Persisting;
using TopDown.Core.ManagingSecurities;

namespace TopDown.Core.ManagingBenchmarks
{
    public class BenchmarkRepository
    {
        private Dictionary<String, IGrouping<String, BenchmarkInfo>> benchmarksByBenchmarkId;
        private ILookup<String, BenchmarkInfo> benchmarkByTicker;

        protected BenchmarkRepository()
            : this(DateTime.Now, new BenchmarkInfo[] { })
        {
        }

        public BenchmarkRepository(DateTime timestamp, IDataManager manager)
            : this(timestamp, manager.GetBenchmarks(timestamp))
        {
        }

        protected BenchmarkRepository(DateTime timestamp, IEnumerable<BenchmarkInfo> benchmarks)
        {
            this.Timestamp = timestamp;
            
			this.benchmarksByBenchmarkId = benchmarks
                .GroupBy(x => x.BenchmarkId)
                .ToDictionary(x => x.Key);
            
			this.benchmarkByTicker = benchmarks.ToLookup(x => x.Ticker);
        }

        public DateTime Timestamp { get; private set; }

        public IEnumerable<BenchmarkSumByIsoInfo> GetByBenchmarkId(String benchmarkId)
        {
            IGrouping<String, BenchmarkInfo> found;
            if (this.benchmarksByBenchmarkId.TryGetValue(benchmarkId, out found))
            {
                var result = found
                    .Where(x => !String.IsNullOrWhiteSpace(x.IsoCountryCode))
                    .GroupBy(x => x.IsoCountryCode)
                    .Select(x => new BenchmarkSumByIsoInfo
                    {
                        IsoCode = x.Key,
                        BenchmarkWeightSum = x.Sum(y => y.BenchmarkWeight.HasValue ? y.BenchmarkWeight.Value : 0.0m) / 100.0m
                    }).ToList();
                return result;
            }
            else
            {
                return new BenchmarkSumByIsoInfo[] { };
            }
        }

        public BenchmarkInfo TryGetBySecurity(ISecurity security)
        {
            var ticker = security.Ticker;
            if (this.benchmarkByTicker.Contains(ticker))
            {
                var benchmarks = this.benchmarkByTicker[ticker];
                return benchmarks.OrderByDescending(x => x.PortfolioDate).FirstOrDefault();
            }
            else
            {
                return null;
            }

        }
    }
}
