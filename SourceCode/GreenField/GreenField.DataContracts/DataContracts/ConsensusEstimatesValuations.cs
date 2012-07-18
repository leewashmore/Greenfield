using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;


namespace GreenField.DataContracts
{
    [DataContract]
    public class ConsensusEstimatesValuations
    {
        [DataMember]
        public decimal Amount { get; set; }


        [DataMember]
        public string AmountType { get; set; }

        [DataMember]
        public decimal AshmoreEMMAmount { get; set; }

        [DataMember]
        public string DataSource { get; set; }

        [DataMember]
        public DateTime DataSourceDate { get; set; }

        [DataMember]
        public string EstimateType { get; set; }

        [DataMember]
        public string EstimateId { get; set; }

        [DataMember]
        public decimal High { get; set; }

        [DataMember]
        public string IssuerId { get; set; }

        [DataMember]
        public decimal Low { get; set; }

        [DataMember]
        public decimal NumberOfEstimates { get; set; }

        [DataMember]
        public string Period { get; set; }

        [DataMember]
        public string PeriodType { get; set; }

        [DataMember]
        public int PeriodYear { get; set; }

        [DataMember]
        public string SourceCurrency { get; set; }

        [DataMember]
        public decimal StandardDeviation { get; set; }
    }
}
