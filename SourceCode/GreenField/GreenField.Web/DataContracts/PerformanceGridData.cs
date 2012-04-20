using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace GreenField.Web.DataContracts
{
    [DataContract]
    public class PerformanceGridData
    {
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