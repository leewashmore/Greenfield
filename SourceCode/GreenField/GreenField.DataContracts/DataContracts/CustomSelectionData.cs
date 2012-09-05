using System;
using System.Net;
using System.Runtime.Serialization;

namespace GreenField.DataContracts.DataContracts
{
    [DataContract]
    public class CustomSelectionData
    {
        [DataMember]
        public string Country { get; set; }

        [DataMember]
        public string Region { get; set; }

        [DataMember]
        public string Sector { get; set; }

        [DataMember]
        public string Industry { get; set; }
    }
}
