using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace GreenField.Web.Helpers
{
    [DataContract]
    public class ServiceFault
    {
        [DataMember]
        public string Description { get; set; }
    }
}