using System;
using System.Runtime.Serialization;

namespace GreenField.DataContracts
{
    [DataContract]
    public class TargetPriceCEData
    {
        [DataMember]
        public string Ticker { get; set; }

        [DataMember]
        public string CurrentPrice { get; set; }

        [DataMember]
        public string MedianTargetPrice { get; set; }

        [DataMember]
        public DateTime LastUpdate { get; set; }

        [DataMember]
        public decimal NoOfEstimates { get; set; }

        [DataMember]
        public decimal High { get; set; }

        [DataMember]
        public decimal Low { get; set; }

        [DataMember]
        public decimal StandardDeviation { get; set; }

        [DataMember]
        public string ConsensusRecommendation { get; set; }
    }
}
