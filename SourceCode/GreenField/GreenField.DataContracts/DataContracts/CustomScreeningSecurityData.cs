using System;
using System.Net;
using System.Runtime.Serialization;

namespace GreenField.DataContracts
{
     [DataContract]
    public class CustomScreeningSecurityData
    {
        [DataMember]
        public string SecurityId { get; set; }

        [DataMember]
        public string AsecShortName { get; set; }
         
        [DataMember]
        public string IssuerId { get; set; }

        [DataMember]
        public string IssueName { get; set; }

        [DataMember]
        public string Type { get; set; }

        [DataMember]
        public object Value { get; set; }

        [DataMember]
        public int DataId { get; set; }

        [DataMember]
        public int EstimateId { get; set; }

        [DataMember]
        public string DataSource { get; set; }

        [DataMember]
        public int PeriodYear { get; set; }

        [DataMember]
        public string PeriodType { get; set; }

        [DataMember]
        public string YearType { get; set; }

        [DataMember]
        public Object MarketCapAmount { get; set; }

        [DataMember]
        public int? Decimals { get; set; }

        [DataMember]
        public string IsPercentage { get; set; }

        [DataMember]
        public decimal? Multiplier { get; set; }
    }
}
