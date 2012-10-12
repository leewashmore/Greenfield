using System;
using System.Runtime.Serialization;

namespace GreenField.DataContracts
{
    [DataContract]
    public class SecurityBaseviewData
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
        public String PrimaryAnalyst { get; set; }

        [DataMember]
        public String Currency { get; set; }

        [DataMember]
        public String FiscalYearend { get; set; }

        [DataMember]
        public String Website { get; set; }

        [DataMember]
        public String Description { get; set; }

        [DataMember]
        public string IssuerId { get; set; }

        [DataMember]
        public int? SecurityId { get; set; }

    }
}
