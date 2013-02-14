using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TopDown.Core.Persisting;
using TopDown.Core.ManagingSecurities;
using Aims.Core;

namespace TopDown.Core.ManagingBenchmarks
{
    public class BenchmarkRepository
    {
        private Dictionary<String, IGrouping<String, BenchmarkInfo>> benchmarksByBenchmarkId;
        private ILookup<String, BenchmarkInfo> benchmarkByShortName;
        private ILookup<String, BenchmarkInfo> benchmarkByCountry;

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
            
			this.benchmarkByShortName = benchmarks.ToLookup(x => x.AsecSecShortName);
            this.benchmarkByCountry = benchmarks.ToLookup(x => x.IsoCountryCode);
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

        public IEnumerable<BenchmarkInfo> GetBenchmarksByIsoCountryCode(String isoCode)
        {
            IEnumerable<BenchmarkInfo> found;
            if (this.benchmarkByCountry.Contains(isoCode))
            {
                found = this.benchmarkByCountry[isoCode];
            }
            else
            {
                found = new BenchmarkInfo[] { };
            }
            return found;
        }

        public IEnumerable<BenchmarkInfo> TryGetBySecurity(ISecurity security)
        {
            var shortName = security.ShortName;
            if (this.benchmarkByShortName.Contains(shortName))
            {
                var benchmarks = this.benchmarkByShortName[shortName];
                return benchmarks.OrderByDescending(x => x.PortfolioDate);
            }
            else
            {
                return new BenchmarkInfo[] {};
            }

        }
    }
}
