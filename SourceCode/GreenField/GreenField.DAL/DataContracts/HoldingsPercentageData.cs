using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace GreenField.DAL
{
    [DataContract]
    public class HoldingsPercentageData
    {
        [DataMember]
        public String SegmentName { get; set; }

        [DataMember]
        public String BenchmarkName { get; set; }

        [DataMember]
        public String FundName { get; set; }

        [DataMember]
        public DateTime EffectiveDate { get; set; }

        [DataMember]
        public decimal? PortfolioWeight { get; set; }

        [DataMember]
        public decimal? BenchmarkWeight { get; set; }
    }
}