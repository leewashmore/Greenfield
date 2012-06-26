using System;
using System.Net;
using System.Collections.Generic;
using System.Runtime.Serialization;
using GreenField.DAL;

namespace GreenField.Web.DataContracts
{
    [DataContract]
    public class IssuerReferenceData
    {
        [DataMember]
        public String IssuerId { get; set; }

        [DataMember]
        public String CountryCode { get; set; }

        [DataMember]
        public String CountryName { get; set; }

        [DataMember]
        public String CurrencyCode { get; set; }

        [DataMember]
        public String CurrencyName { get; set; }
    }


}
