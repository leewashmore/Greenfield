using System;
using System.Collections.Generic;
using System.Linq;
 
using System.Runtime.Serialization;

namespace GreenField.DataContracts
{
    [DataContract]
    public class PerformanceGridData
    {
        [DataMember]
        public String Name {get;set;}

        [DataMember]
        public Decimal? MTD { get; set; }

        [DataMember]
        public Decimal? QTD { get; set; }

        [DataMember]
        public Decimal? YTD { get; set; }

        [DataMember]
        public Decimal? FIRST_YEAR { get; set; }

        [DataMember]
        public Decimal? THIRD_YEAR { get; set; }

        [DataMember]
        public Decimal? FIFTH_YEAR { get; set; }

        [DataMember]
        public Decimal? TENTH_YEAR { get; set; }
    }
}