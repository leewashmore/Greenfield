using System;
using System.Collections.Generic;
using System.Linq;
 
using System.Runtime.Serialization;

namespace GreenField.DataContracts
{
    [DataContract]
    public class TopBenchmarkSecuritiesData
    {
        [DataMember]
        public String IssuerName { get; set; }

        [DataMember]
        public Decimal? Weight { get; set; }

        [DataMember]
        public Decimal? OneDayReturn { get; set; }

        [DataMember]
        public Decimal? WTD { get; set; }

        [DataMember]
        public Decimal? MTD { get; set; }

        [DataMember]
        public Decimal? QTD { get; set; }

        [DataMember]
        public Decimal? YTD { get; set; }

      
    }
}