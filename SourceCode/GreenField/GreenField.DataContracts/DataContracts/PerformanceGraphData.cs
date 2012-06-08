using System;
using System.Collections.Generic;
using System.Linq;
 
using System.Runtime.Serialization;

namespace GreenField.DataContracts
{
    [DataContract]
    public class PerformanceGraphData
    {
        [DataMember]
        public String PORTFOLIO_ID;

        [DataMember]
        public String BENCHMARK_ID;

        [DataMember]
        public Decimal? PORTFOLIO_PERFORMANCE;

        [DataMember]
        public Decimal? BENCHMARK_PERFORMANCE;

    }
}