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
        public decimal? PortfolioShare { get; set; }

        [DataMember]
        public decimal? BenchmarkShare { get; set; }

        [DataMember]
        public decimal? ActivePosition { get; set; }
    }
}