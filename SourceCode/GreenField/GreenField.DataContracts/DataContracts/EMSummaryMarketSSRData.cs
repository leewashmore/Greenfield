using System.Runtime.Serialization;

namespace GreenField.DataContracts
{
    /// <summary>
    /// Data Contract class for EM Summary Market SSR Data Gadget
    /// </summary>
    [DataContract]
    public class EMSummaryMarketSSRData
    {
        /// <summary>
        /// Region
        /// </summary>
        [DataMember]
        public string Region { get; set; }

        /// <summary>
        /// Country
        /// </summary>
        [DataMember]
        public string Country { get; set; }

        /// <summary>
        /// Benchmark Weight
        /// </summary>
        [DataMember]
        public decimal? BenchmarkWeight { get; set; }

        /// <summary>
        /// Base Target
        /// </summary>
        [DataMember]
        public decimal? BaseTarget { get; set; }

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
