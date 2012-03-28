using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace GreenField.Web.DataContracts
{
    [DataContract]
    public class RelativePerformanceData
    {
        [DataMember]
        public string EntityName { get; set; }

        [DataMember]
        public string EntityIdentifier { get; set; }

        [DataMember]
        public double QTDReturn { get; set; }

        [DataMember]
        public double YTDReturn { get; set; }

        [DataMember]
        public double LastYearReturn { get; set; }
    }
}