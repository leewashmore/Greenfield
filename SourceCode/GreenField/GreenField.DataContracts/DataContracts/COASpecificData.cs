using System;
using System.Net;
using System.Runtime.Serialization;

namespace GreenField.DataContracts
{
    [DataContract]
    public class COASpecificData
    {
        [DataMember]
        public Int32 GridId { get; set; }

        [DataMember]
        public String ShowGrid { get; set; }

        [DataMember]
        public String GroupDescription { get; set; }

        [DataMember]
        public Decimal? Amount { get; set; }

        [DataMember]
        public String AmountType { get; set; }

        [DataMember]
        public Int32? PeriodYear { get; set; }

        [DataMember]
        public String PeriodType { get; set; }

        [DataMember]
        public String Description { get; set; }

        [DataMember]
        public String DataSource { get; set; }

        [DataMember]
        public Int32? Decimals { get; set; }

        [DataMember]
        public String IsPercentage { get; set; }

        [DataMember]
        public String RootSource { get; set; }

        [DataMember]
        public Int32? SortOrder { get; set; }

        [DataMember]
        public decimal? Multiplier { get; set; }

    }
}
