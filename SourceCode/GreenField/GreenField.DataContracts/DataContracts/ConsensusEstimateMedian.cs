using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace GreenField.DataContracts
{
    [DataContract]
    public class ConsensusEstimateMedian
    {
        [DataMember]
        public decimal Amount { get; set; }

        [DataMember]
        public string AmountType { get; set; }

        [DataMember]
        public string Currency { get; set; }

        [DataMember]
        public string DataSource { get; set; }

        [DataMember]
        public DateTime DataSourceDate { get; set; }

        [DataMember]
        public string EstimateDesc { get; set; }

        [DataMember]
        public string EstimateType { get; set; }

        [DataMember]
        public string FiscalType { get; set; }

        [DataMember]
        public decimal High { get; set; }

        [DataMember]
        public string IssuerId { get; set; }

        [DataMember]
        public decimal Low { get; set; }

        [DataMember]
        public decimal NumberOfEstimates { get; set; }

        [DataMember]
        public DateTime PeriodEndDate { get; set; }

        [DataMember]
        public string PeriodType { get; set; }

        [DataMember]
        public int PeriodYear { get; set; }

        [DataMember]
        public string SecrityId { get; set; }

        [DataMember]
        public string SourceCurrency { get; set; }

        [DataMember]
        public decimal StandardDeviation { get; set; }

    }
}
