using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TopDown.Core.ManagingBenchmarks
{
    /// <summary>
    /// Represents benchmark information by country that we get from the Dimension web-service.
    /// </summary>
    public class BenchmarkSumByIsoInfo
    {
        public String IsoCode { get; set; }
        public Decimal BenchmarkWeightSum { get; set; }
    }
}
