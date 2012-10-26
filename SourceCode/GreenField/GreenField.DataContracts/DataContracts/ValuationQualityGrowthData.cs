using System;
using System.Net;
using System.Runtime.Serialization;

namespace GreenField.DataContracts.DataContracts
{
    [DataContract]
    public class ValuationQualityGrowthData
    {
        [DataMember]
        public String Description { get; set; }

        [DataMember]
        public Object Portfolio { get; set; }

        [DataMember]
        public Object Benchmark { get; set; }

        [DataMember]
        public Object Relative { get; set; }
    }
}
