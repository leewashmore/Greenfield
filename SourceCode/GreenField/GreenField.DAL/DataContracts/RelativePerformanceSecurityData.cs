using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace GreenField.DAL
{
    [DataContract]
    public class RelativePerformanceSecurityData
    {
        [DataMember]
        public string SecurityName { get; set; }

        [DataMember]
        public string SecurityCountryID { get; set; }

        [DataMember]
        public string SecuritySectorName { get; set; }

        [DataMember]
        public double SecurityAlpha { get; set; }

        [DataMember]
        public double SecurityActivePosition { get; set; }       
                
    }
}