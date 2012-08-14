using System;
using System.Net;
using System.Runtime.Serialization;

namespace GreenField.DataContracts
{
    [DataContract]
    public class FinstatDetailData
    {
        [DataMember]
        public string DataSource { get; set; }

        [DataMember]
        public string RootSource { get; set; }

        [DataMember]
        public DateTime RootSourceDate { get; set; }

        [DataMember]
        public int EstimateId{ get; set; }

        [DataMember]
        public int PeriodYear { get; set; }

        [DataMember]
        public int DataId{ get; set; }

        [DataMember]
        public decimal Amount { get; set; }

        [DataMember]
        public decimal Multiplier { get; set; }

        [DataMember]
        public int Decimals{ get; set; }

        [DataMember]
        public string Percentage { get; set; }

        [DataMember]
        public string BoldFont { get; set; }

        [DataMember]
        public string GroupDescription { get; set; }

        [DataMember]
        public string AmountType { get; set; }

        [DataMember]
        public string PeriodType { get; set; }

        [DataMember]
        public int SortOrder { get; set; }

        [DataMember]
        public string Harmonic { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public decimal? HarmonicFirst { get; set; }

        [DataMember]
        public decimal? HarmonicSecond { get; set; }
    }
}
