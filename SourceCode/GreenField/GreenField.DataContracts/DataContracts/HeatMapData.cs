using System;
using System.Collections.Generic;
using System.Linq;
 
using System.Runtime.Serialization;

namespace GreenField.DataContracts
{
    [DataContract]
    public class HeatMapData
    {
        [DataMember]
        public String CountryID { get; set; }

        [DataMember]
        public PerformanceGrade CountryPerformance { get; set; }

        [DataMember]
        public Decimal? CountryYTD { get; set; }
       
        [DataMember]
        public Decimal? BenchmarkYTD { get; set; }
        
    }
}