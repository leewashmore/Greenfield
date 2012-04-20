using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace GreenField.Web.DataContracts
{
    [DataContract]
    public class TopBenchmarkSecuritiesData
    {
        [DataMember]
        public String IssuerName;

        [DataMember]
        public Double Weight;

        [DataMember]
        public Double MTD;

        [DataMember]
        public Double QTD;

        [DataMember]
        public Double YTD;

        [DataMember]
        public Double PreviousYear;

        [DataMember]
        public Double SecondPreviousYear;

        [DataMember]
        public Double ThirdPreviousYear;
    }
}