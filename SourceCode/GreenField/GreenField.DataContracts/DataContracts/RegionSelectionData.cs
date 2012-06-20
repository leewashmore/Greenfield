using System;
using System.Net;
using System.Collections.Generic;
using System.Linq; 
using System.Runtime.Serialization;

namespace GreenField.DataContracts
{ 
    [DataContract]
    public class RegionSelectionData
    {        
        [DataMember]
        public String Region { get; set; }

        [DataMember]
        public String Country { get; set; }

    }
}
