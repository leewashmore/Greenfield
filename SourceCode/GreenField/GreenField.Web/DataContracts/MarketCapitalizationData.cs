using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace GreenField.Web.DataContracts
{
    [DataContract]
    public class MarketCapitalizationData
    {
        [DataMember]
        public string Benchmark_ID { get; set; }

        [DataMember]
        public string Portfolio_ID { get; set; }

        [DataMember]
        public decimal? PortfolioDirtyValuePC { get; set; }

        [DataMember]
        public decimal? BenchmarkWeight { get; set; }

        [DataMember]
        public decimal? MarketCapitalInUSD { get; set; }

        [DataMember]
        public string SecurityThemeCode { get; set; }

        [DataMember]
        public decimal? LargeRange { get; set; }

        [DataMember]
        public decimal? MediumRange { get; set; }

        [DataMember]
        public decimal? SmallRange { get; set; }

        [DataMember]
        public decimal? MicroRange { get; set; }

        [DataMember]
        public decimal? UndefinedRange { get; set; }

        [DataMember]
        public decimal? PortfolioSumMegaRange { get; set; }

        [DataMember]
        public decimal? PortfolioSumLargeRange { get; set; }

        [DataMember]
        public decimal? PortfolioSumMediumRange { get; set; }

        [DataMember]
        public decimal? PortfolioSumSmallRange { get; set; }

        [DataMember]
        public decimal? PortfolioSumMicroRange { get; set; }

        [DataMember]
        public decimal? PortfolioSumUndefinedRange { get; set; }

        [DataMember]
        public decimal? BenchmarkSumMegaRange { get; set; }

        [DataMember]
        public decimal? BenchmarkSumLargeRange { get; set; }

        [DataMember]
        public decimal? BenchmarkSumMediumRange { get; set; }

        [DataMember]
        public decimal? BenchmarkSumSmallRange { get; set; }

        [DataMember]
        public decimal? BenchmarkSumMicroRange { get; set; }

        [DataMember]
        public decimal? BenchmarkSumUndefinedRange { get; set; }

        [DataMember]
        public decimal? PortfolioWtdAvg { get; set; }

        [DataMember]
        public decimal? BenchmarkWtdAvg { get; set; }

        [DataMember]
        public decimal? PortfolioWtdMedian { get; set; }

        [DataMember]
        public decimal? BenchmarkWtdMedian { get; set; }

        [DataMember]
        public string AsecSecShortName { get; set; }

    }

   
}