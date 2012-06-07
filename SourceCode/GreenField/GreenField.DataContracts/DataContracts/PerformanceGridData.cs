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
        public Decimal? TopRcTwr1D { get; set; }

        [DataMember]
        public Decimal? TopRcTwr1W { get; set; }

        [DataMember]
        public Decimal? TopRcTwrMtd { get; set; }

        [DataMember]
        public Decimal? TopRcTwrQtd { get; set; }

        [DataMember]
        public Decimal? TopRcTwrYtd { get; set; }

        [DataMember]
        public Decimal? TopRcTwr1Y { get; set; }

    }
}