using System;
using System.Net;
using System.Runtime.Serialization;

namespace GreenField.DataContracts
{
    [DataContract]
    public class FairValueMeasureDataId
    {
        [DataMember]
        public string Measure { get; set; }

        [DataMember]
        public int? DATA_ID { get; set; }
    }
}
