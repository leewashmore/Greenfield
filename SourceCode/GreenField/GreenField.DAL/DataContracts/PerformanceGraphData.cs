using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace GreenField.DAL
{
    [DataContract]
    public class PerformanceGraphData
    {
        [DataMember]
        public String PORTFOLIO_ID;

        [DataMember]
        public String BENCHMARK_ID;

        [DataMember]
        public Double PORTFOLIO_PERFORMANCE;

        [DataMember]
        public Double BENCHMARK_PERFORMANCE;

        [DataMember]
        public DateTime EFFECTIVE_DATE;

        [DataMember]
        public Double MTD;

        [DataMember]
        public Double QTD;

        [DataMember]
        public Double YTD;

        [DataMember]
        public Double FIRST_YEAR;

        [DataMember]
        public Double THIRD_YEAR;

        [DataMember]
        public Double FIFTH_YEAR;

        [DataMember]
        public Double TENTH_YEAR;

      
        
    }
}