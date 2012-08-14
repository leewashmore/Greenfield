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

        [DataMember]
        public String RegionCode { get; set; }

        [DataMember]
        public String SectorCode { get; set; }

        [DataMember]
        public String SectorName { get; set; }

        [DataMember]
        public String IndustryCode { get; set; }

        [DataMember]
        public String IndustryName { get; set; }

        [DataMember]
        public int? SecurityId { get; set; }

        [DataMember]
        public String IssueName { get; set; }

        [DataMember]
        public String SubIndustryName { get; set; }

        [DataMember]
        public String Ticker { get; set; }

        [DataMember]
        public String TradingCurrency { get; set; }

        [DataMember]
        public String PrimaryAnalyst { get; set; }

        [DataMember]
        public String IndustryAnalyst { get; set; }

    }
}
