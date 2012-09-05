using System;
using System.Net;
using System.Runtime.Serialization;

namespace GreenField.DataContracts.DataContracts
{
     [DataContract]
    public class CustomScreeningToolData
    {
        [DataMember]
        public string SecurityName { get; set; }

        [DataMember]
        public string SecurityTicker { get; set; }

        [DataMember]
        public string Type { get; set; }

        [DataMember]
        public string Value { get; set; }

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
        public Object Amount { get; set; }

        [DataMember]
        public int Decimals { get; set; }

        [DataMember]
        public string IsPercentage { get; set; }
    }
}
