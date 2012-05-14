using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace GreenField.DAL
{
    [DataContract]
    public class PerformanceGridData
    {
        [DataMember]
        public String Name;

        [DataMember]
        public Decimal? MTD;

        [DataMember]
        public Decimal? QTD;

        [DataMember]
        public Decimal? YTD;

        [DataMember]
        public Decimal? FIRST_YEAR;

        [DataMember]
        public Decimal? THIRD_YEAR;

        [DataMember]
        public Decimal? FIFTH_YEAR;

        [DataMember]
        public Decimal? TENTH_YEAR;
    }
}