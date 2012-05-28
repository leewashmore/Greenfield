using System;
using System.Collections.Generic;
using System.Linq;
 
using System.Runtime.Serialization;

namespace GreenField.DataContracts
{
    [DataContract]
    public class CountrySelectionData
    {
        [DataMember]
        public String CountryCode { get; set; }

        [DataMember]
        public String CountryName { get; set; }

    }
}