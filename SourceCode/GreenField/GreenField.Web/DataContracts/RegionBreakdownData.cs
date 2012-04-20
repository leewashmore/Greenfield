using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace GreenField.Web.DataContracts
{
    [DataContract]
    public class RegionBreakdownData
    {
        [DataMember]
        public string Region { get; set; }

        [DataMember]
        public string Country { get; set; }

        [DataMember]
        public string Security { get; set; }

        [DataMember]
        public double? PortfolioShare { get; set; }

        [DataMember]
        public double? BenchmarkShare { get; set; }

        [DataMember]
        public double? BetShare { get; set; }
    }
}