using System;
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
         public string EstimateDesc { get; set; }

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
    }
}
