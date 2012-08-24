using System;
using System.Net;
using System.Runtime.Serialization;

namespace GreenField.DataContracts.DataContracts
{
    [DataContract]
    public class ValuationQualityGrowthData
    {
        [DataMember]
        public String Description { get; set; }

        [DataMember]
        public Decimal? Portfolio { get; set; }

        [DataMember]
        public Decimal? Benchmark { get; set; }

        [DataMember]
        public Decimal? Relative { get; set; }
    }
}
