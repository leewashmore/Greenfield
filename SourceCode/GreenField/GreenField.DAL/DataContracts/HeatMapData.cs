using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace GreenField.DAL
{
    [DataContract]
    public class HeatMapData
    {
        [DataMember]
        public String CountryID;

        [DataMember]
        public PerformanceGrade CountryPerformance;

        [DataMember]
        public Double? CountryYTD;
        
    }
}