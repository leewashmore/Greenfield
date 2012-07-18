using System;
using System.Net;
using System.Runtime.Serialization;

namespace GreenField.DataContracts
{
    [DataContract]
    public class BenchmarkFilterSelectionData
    {
        [DataMember]
        public String FilterCode { get; set; }

        [DataMember]
        public String FilterName { get; set; }
    }
}
