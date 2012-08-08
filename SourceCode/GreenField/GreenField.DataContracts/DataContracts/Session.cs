using System;
using System.Collections.Generic;
using System.Linq;
 
using System.Runtime.Serialization;

namespace GreenField.DataContracts
{
    [DataContract(IsReference = false)]
    public class Session
    {
        [DataMemberAttribute(Name = "UserName", IsRequired = true)]
        public string UserName { get; set; }

        [DataMemberAttribute(Name = "Roles")]
        public List<string> Roles { get; set; }
    }
}