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
        public String PortfolioID {get; set; }

        [DataMember]
        public String BenchmarkID { get; set; }

        [DataMember]
        public Decimal? PortfolioPerformance { get; set; }

        [DataMember]
        public Decimal? BenchmarkPerformance { get; set; }

        [DataMember]
        public DateTime EffectiveDate { get; set; }
    }
}