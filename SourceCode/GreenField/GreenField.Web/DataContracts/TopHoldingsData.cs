using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace GreenField.Web.DataContracts
{
    [DataContract]
    public class TopHoldingsData
    {
        [DataMember]
        public string Ticker { get; set; }

        [DataMember]
        public string Holding { get; set; }

        [DataMember]
        public Single? MarketValue { get; set; }

        [DataMember]
        public double? PortfolioShare { get; set; }

        [DataMember]
        public double? BenchmarkShare { get; set; }

        [DataMember]
        public double? BetShare { get; set; }
    }
}