using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TopDown.Core.ManagingBenchmarks
{
	public class CountryIsoCodesExtractor
	{
		public IEnumerable<String> ExtractCodes(IEnumerable<BenchmarkSumByIsoInfo> benchmarks)
		{
			return benchmarks.Select(x => x.IsoCode)
				.Where(x => !String.IsNullOrWhiteSpace(x))
				.ToArray();
		}
	}
}
