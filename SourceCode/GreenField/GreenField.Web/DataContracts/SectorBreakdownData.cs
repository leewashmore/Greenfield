using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace GreenField.Web.DataContracts
{
    [DataContract]
    public class SectorBreakdownData
    {
        [DataMember]
        public string Sector { get; set; }

        [DataMember]
        public string Industry { get; set; }

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