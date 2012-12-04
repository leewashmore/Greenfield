using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TopDown.Core.ManagingBpst
{
	public class BenchmarkInitializer
	{
		public void InitializeCore(
			CoreModel core,
			ManagingBenchmarks.BenchmarkRepository benchmarkRepository
		)
		{
			core.Securities.ForEach(securityModel => this.InitializeSecurity(securityModel, benchmarkRepository));
		}

		public void InitializeSecurity(
			SecurityModel security,
			ManagingBenchmarks.BenchmarkRepository benchmarkRepository
		)
		{
			var benchmarkInfoOpt = benchmarkRepository.TryGetBySecurity(security.Security);
			if (benchmarkInfoOpt != null && benchmarkInfoOpt.BenchmarkWeight.HasValue)
			{
				security.Benchmark.InitialValue = benchmarkInfoOpt.BenchmarkWeight.Value;
			}
		}
	}
}
