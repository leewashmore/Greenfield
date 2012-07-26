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
        public string IssuerId { get; set; }

        [DataMember]
        public int EstimateId { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public String Period { get; set; }

        [DataMember]
        public string AmountType { get; set; }

        [DataMember]
        public int PeriodYear { get; set; }

        [DataMember]
        public string PeriodType { get; set; }

        [DataMember]
        public decimal Amount { get; set; }

        [DataMember]
        public decimal? AshmoreEmmAmount { get; set; }

        [DataMember]
        public int NumberOfEstimates { get; set; }

        [DataMember]
        public decimal High { get; set; }

        [DataMember]
        public decimal Low { get; set; }

        [DataMember]
        public decimal StandardDeviation { get; set; }

        [DataMember]
        public string SourceCurrency { get; set; }

        [DataMember]
        public string DataSource { get; set; }

        [DataMember]
        public DateTime DataSourceDate { get; set; }

        [DataMember]
        public decimal? Actual { get; set; }

        [DataMember]
        public decimal YOYGrowth { get; set; }

        [DataMember]
        public decimal? Variance { get; set; }

    }
}
