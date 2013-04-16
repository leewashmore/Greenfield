using System;
using System.Net;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace GreenField.DataContracts
{
    /// <summary>
    /// Data Contract for Investment Context Data
    /// </summary>
    [DataContract]
    public class InvestmentContextDetailsData
    {
        [DataMember]
        public string IssuerId { get; set; }

        [DataMember]
        public string IssuerName { get; set; }

        [DataMember]
        public string IsoCountryCode { get; set; }

        [DataMember]
        public string GicsSectorCode { get; set; }

        [DataMember]
        public string GicsSectorName { get; set; }

        [DataMember]
        public string GicsIndustryCode { get; set; }
        
        [DataMember]
        public string GicsIndustryName { get; set; }

        [DataMember]
        public decimal? MarketValue { get; set; }

        [DataMember]
        public decimal? MarketValueScrubbed { get; set; }

        [DataMember]
        public decimal? MarketCap { get; set; }

        [DataMember]
        public decimal? MarketCapScrubbed { get; set; }

        [DataMember]
        public decimal? ForwardPE { get; set; }

        [DataMember]
        public decimal? ForwardPEScrubbed { get; set; }

        [DataMember]
        public decimal? ForwardPBV { get; set; }

        [DataMember]
        public decimal? ForwardPBVScrubbed { get; set; }

        [DataMember]
        public decimal? PECurrentYear { get; set; }

        [DataMember]
        public decimal? PECurrentYearScrubbed { get; set; }

        [DataMember]
        public decimal? PENextYear { get; set; }

        [DataMember]
        public decimal? PENextYearScrubbed { get; set; }

        [DataMember]
        public decimal? PBVCurrentYear { get; set; }

        [DataMember]
        public decimal? PBVCurrentYearScrubbed { get; set; }

        [DataMember]
        public decimal? PBVNextYear { get; set; }

        [DataMember]
        public decimal? PBVNextYearScrubbed { get; set; }
        
        [DataMember]
        public decimal? EB_EBITDA_CurrentYear { get; set; }

        [DataMember]
        public decimal? EB_EBITDA_CurrentYearScrubbed { get; set; }

        [DataMember]
        public decimal? EB_EBITDA_NextYear { get; set; }

        [DataMember]
        public decimal? EB_EBITDA_NextYearScrubbed { get; set; }

        [DataMember]
        public decimal? DividendYield { get; set; }

        [DataMember]
        public decimal? DividendYieldScrubbed { get; set; }

        [DataMember]
        public decimal? ROE { get; set; }

        [DataMember]
        public decimal? ROEScrubbed { get; set; }

        [DataMember]
        public List<InvestmentContextDetailsData> children { get; set; }


      
    }
}