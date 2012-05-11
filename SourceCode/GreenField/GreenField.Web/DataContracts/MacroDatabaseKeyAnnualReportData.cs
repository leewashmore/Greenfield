using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;



namespace GreenField.Web.DataContracts
{
    [DataContract]
    public class MacroDatabaseKeyAnnualReportData
    {
        [DataMember]        
        public string CountryName { get; set; }

        [DataMember]
        public string CategoryName { get; set; }

        [DataMember]
        public string DisplayType { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string Region { get; set; }

    }
}