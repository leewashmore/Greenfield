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
        public String CountryID;

        [DataMember]
        public PerformanceGrade CountryPerformance;

        [DataMember]
        public Decimal? CountryYTD;
        
    }
}