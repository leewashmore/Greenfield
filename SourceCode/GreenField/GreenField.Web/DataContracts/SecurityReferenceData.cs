using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace GreenField.Web.DataContracts
{
    [DataContract]
    public class SecurityReferenceData
    {
        [DataMember]
        public String IssueName { get; set; }

        [DataMember]
        public String Ticker { get; set; }

        [DataMember]
        public String Country { get; set; }

        [DataMember]
        public String Sector { get; set; }

        [DataMember]
        public String Industry { get; set; }

        [DataMember]
        public String SubIndustry { get; set; }

        [DataMember]
        public String Exchange { get; set; }

        [DataMember]
        public String Currency { get; set; }

        [DataMember]
        public String FiscalYearEnd { get; set; }

        [DataMember]
        public String Website { get; set; }

        [DataMember]
        public String Description { get; set; }

    }
}