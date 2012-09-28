using System;
using System.Net;
using System.Runtime.Serialization;

namespace GreenField.DataContracts
{
    [DataContract]
    public class CompositeFundData
    {
        [DataMember]
        public string CountryName { get; set; }

        [DataMember]
        public string SecurityId { get; set; }
        
        [DataMember]
        public decimal Target { get; set; }
        
        [DataMember]
        public string IssuerId { get; set; }

        [DataMember]
        public object PortfolioTarget { get; set; }

        [DataMember]
        public object PortfolioTargetInCountry  { get; set; }

        [DataMember]
        public object Holdings { get; set; }
        
        [DataMember]
        public object BenchmarkWeight  { get; set; }
        
        [DataMember]
        public object BenchmarkWeightInCountry  { get; set; }

        [DataMember]
        public object ActivePosition { get; set; }

        [DataMember]
        public object ActivePositionInCountry { get; set; }
    }
}
