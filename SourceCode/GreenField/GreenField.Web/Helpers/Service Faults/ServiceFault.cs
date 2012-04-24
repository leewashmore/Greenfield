using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace GreenField.Web.Helpers.Service_Faults
{
    [DataContract]
    public class ServiceFault
    {
        [DataMember]
        public string Description { get; set; }

        public ServiceFault(string description)
        {
            this.Description = description;
        }        
    }
}