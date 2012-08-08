using System;
using System.Collections.Generic;
using System.Linq; 
using System.Runtime.Serialization;

namespace GreenField.DataContracts.DataContracts
{
    [DataContract]
    public class QuarterlyResultsData
    {
      [DataMember]
      public String ISSUER_ID  { get; set; }

      [DataMember]
      public String IssuerName { get; set; }

      [DataMember]
      public String Region { get; set; }

      [DataMember]
      public String Country { get; set; }

      [DataMember]
      public String Sector { get; set; }

      [DataMember]
      public String Industry { get; set; }

      [DataMember]
      public String Currency { get; set; }

      [DataMember]
      public DateTime? LastUpdate { get; set; }

       [DataMember]
       public Decimal? Q1 { get; set; }

       [DataMember]
       public Decimal? Q2 { get; set; }

       [DataMember]
       public Decimal? Q3 { get; set; }

       [DataMember]
       public Decimal? Q4 { get; set; }

       [DataMember]
       public Decimal? Annual { get; set; }

       [DataMember]
       public Decimal? QuarterlySum { get; set; }

       [DataMember]
       public Decimal? QuarterlySumPercentage { get; set; }

       [DataMember]
       public Decimal? Consensus { get; set; }

       [DataMember]
       public Decimal? ConsensusPercentage { get; set; }

       [DataMember]
       public Decimal? High { get; set; }

       [DataMember]
       public Decimal? Low { get; set; }

       [DataMember]
       public Int32? Brokers { get; set; }

       [DataMember]
       public DateTime? ConsensusUpdate { get; set; }
        
       [DataMember]
       public Double? XREF { get; set; }

       [DataMember]
       public Int32? EstimateId { get; set; }








    }
}
