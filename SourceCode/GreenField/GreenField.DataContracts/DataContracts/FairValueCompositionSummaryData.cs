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
        public string SOURCE{get; set;}

        [DataMember]
        public string MEASURE{get;set;}

        [DataMember]
        public decimal? BUY{get; set;}

        [DataMember]
        public decimal? SELL { get; set; }

        [DataMember]
        public decimal? UPSIDE { get; set; }

        [DataMember]
        public DateTime? DATE { get; set; }

        [DataMember]
        public Int32? DATA_ID { get; set; }
    }    
}
