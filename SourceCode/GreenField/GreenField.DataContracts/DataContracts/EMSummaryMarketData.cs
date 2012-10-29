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
        public string BenchmarkId { get; set; }

        [DataMember]
        public DateTime PortfolioDate { get; set; }

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

        /// <summary>
        /// FX Rate Year 1 Quarter 1
        /// </summary>
        [DataMember]
        public decimal? FxY1Q1 { get; set; }

        /// <summary>
        /// FX Rate Year 1 Quarter 2
        /// </summary>
        [DataMember]
        public decimal? FxY1Q2 { get; set; }

        /// <summary>
        /// FX Rate Year 1 Quarter 3
        /// </summary>
        [DataMember]
        public decimal? FxY1Q3 { get; set; }

        /// <summary>
        /// FX Rate Year 1 Quarter 4
        /// </summary>
        [DataMember]
        public decimal? FxY1Q4 { get; set; }

        /// <summary>
        /// FX Rate Year 2 Quarter 1
        /// </summary>
        [DataMember]
        public decimal? FxY2Q1 { get; set; }

        /// <summary>
        /// FX Rate Year 2 Quarter 2
        /// </summary>
        [DataMember]
        public decimal? FxY2Q2 { get; set; }

        /// <summary>
        /// FX Rate Year 2 Quarter 3
        /// </summary>
        [DataMember]
        public decimal? FxY2Q3 { get; set; }

        /// <summary>
        /// FX Rate Year 2 Quarter 4
        /// </summary>
        [DataMember]
        public decimal? FxY2Q4 { get; set; }

        /// <summary>
        /// Real GDP growth rate last year
        /// </summary>
        [DataMember]
        public decimal? GdpY0 { get; set; }

        /// <summary>
        /// Real GDP growth rate present year
        /// </summary>
        [DataMember]
        public decimal? GdpY1 { get; set; }

        /// <summary>
        /// Real GDP growth rate next year
        /// </summary>
        [DataMember]
        public decimal? GdpY2 { get; set; }

        /// <summary>
        /// Inflation PCT last year
        /// </summary>
        [DataMember]
        public decimal? InflationY0 { get; set; }

        /// <summary>
        /// Inflation PCT present year
        /// </summary>
        [DataMember]
        public decimal? InflationY1 { get; set; }

        /// <summary>
        /// Inflation PCT last year
        /// </summary>
        [DataMember]
        public decimal? InflationY2 { get; set; }

        /// <summary>
        /// ST Interest rate last year
        /// </summary>
        [DataMember]
        public decimal? StInterestY0 { get; set; }

        /// <summary>
        /// ST Interest rate present year
        /// </summary>
        [DataMember]
        public decimal? StInterestY1 { get; set; }

        /// <summary>
        /// ST Interest rate last year
        /// </summary>
        [DataMember]
        public decimal? StInterestY2 { get; set; }

        /// <summary>
        /// Current Account PCT last year
        /// </summary>
        [DataMember]
        public decimal? CurrAccountY0 { get; set; }

        /// <summary>
        /// Current Account PCT present year
        /// </summary>
        [DataMember]
        public decimal? CurrAccountY1 { get; set; }

        /// <summary>
        /// Current Account PCT last year
        /// </summary>
        [DataMember]
        public decimal? CurrAccountY2 { get; set; }
    }
}
