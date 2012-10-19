using System;
using System.Net;
using System.Runtime.Serialization;

namespace GreenField.DataContracts.DataContracts
{
    /// <summary>
    /// Data Contract class for EM Summary Market Data Gadget
    /// </summary>
    [DataContract]
    public class EMSummaryMarketData
    {
        [DataMember]
        public string Region { get; set; }

        [DataMember]
        public string Country { get; set; }

        [DataMember]
        public decimal? BenchmarkWeight { get; set; }

        [DataMember]
        public decimal? BaseTarget { get; set; }

        [DataMember]
        public decimal? Active { get; set; }

        [DataMember]
        public decimal? YTDReturns { get; set; }

        [DataMember]
        public decimal? PECurYear { get; set; }

        [DataMember]
        public decimal? PECurYearCon { get; set; }

        [DataMember]
        public decimal? PENextYear { get; set; }

        [DataMember]
        public decimal? PENextYearCon { get; set; }

        [DataMember]
        public decimal? USDEarCurYear { get; set; }

        [DataMember]
        public decimal? USDEarCurYearCon { get; set; }

        [DataMember]
        public decimal? USDEarNextYear { get; set; }

        [DataMember]
        public decimal? USDEarNextYearCon { get; set; }

        [DataMember]
        public decimal? PBVCurYear { get; set; }

        [DataMember]
        public decimal? DYCurYear { get; set; }

        [DataMember]
        public decimal? ROECurYear { get; set; }
    }
}
