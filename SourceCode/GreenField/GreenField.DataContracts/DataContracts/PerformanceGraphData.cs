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
        public String PortfolioID;

        [DataMember]
        public String BenchmarkID;

        [DataMember]
        public Decimal? PortfolioPerformance;

        [DataMember]
        public Decimal? BenchmarkPerformance;

        [DataMember]
        public DateTime EffectiveDate;
    }
}