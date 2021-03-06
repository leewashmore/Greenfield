﻿using System;
using System.Net;
using System.Runtime.Serialization;

namespace GreenField.DataContracts
{
    [DataContract]
    public class ConsensusEstimateDetail
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

        [DataMember]
        public decimal ConsensusMedian{ get; set; }

        [DataMember]
        public string BrokerName { get; set; }

        [DataMember]
        public object BrokerPrice { get; set; }

        [DataMember]
        public string LastUpdateDate { get; set; }

        [DataMember]
        public string ReportedCurrency { get; set; }

        [DataMember]
        public string GroupDescription { get; set; }

        [DataMember]
        public Int32 SortOrder { get; set; }
    }
}
