using System;
using System.Collections.Generic;
using System.Linq;
 
using System.Runtime.Serialization;

namespace GreenField.DataContracts
{
    [DataContract]
    public class PortfolioRiskReturnData
    {
        [DataMember]
        public String DataPointName { get; set; }
      
        [DataMember]
        public Decimal? BenchMarkValue1 { get; set; }

        [DataMember]
        public Decimal? BenchMarkValue2 { get; set; }

        [DataMember]
        public Decimal? BenchMarkValue3 { get; set; }

        [DataMember]
        public Decimal? BenchMarkValue4 { get; set; }

        [DataMember]
        public Decimal? BenchMarkValue5 { get; set; }

        [DataMember]
        public Decimal? PortfolioValue1 { get; set; }

        [DataMember]
        public Decimal? PortfolioValue2 { get; set; }

        [DataMember]
        public Decimal? PortfolioValue3 { get; set; }

        [DataMember]
        public Decimal? PortfolioValue4 { get; set; }

        [DataMember]
        public Decimal? PortfolioValue5 { get; set; }
    }
}