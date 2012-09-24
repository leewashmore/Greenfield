using System;
using System.Net;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace GreenField.DataContracts
{
    [DataContract]
    public class FairValueCompositionSummaryData
    {
        [DataMember]
        public string Source{get; set;}

        [DataMember]
        public string Measure{get;set;}

        [DataMember]
        public decimal? Buy{get; set;}

        [DataMember]
        public decimal? Sell { get; set; }

        [DataMember]
        public decimal? Upside { get; set; }

        [DataMember]
        public DateTime? Date { get; set; }

        [DataMember]
        public Int32? DataId { get; set; }

        [DataMember]
        public String PrimaryAnalyst { get; set; }

        [DataMember]
        public String IndustryAnalyst { get; set; }
    }    
}
