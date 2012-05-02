using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace GreenField.Web.DataContracts
{
    [DataContract]
    public class RelativePerformanceActivePositionData
    {
        /// <summary>
        /// Country / Sector / Security name for which the active position record is being referenced
        /// </summary>
        [DataMember]
        public string Entity { get; set; }

        /// <summary>
        /// Market value with respect to the entity
        /// </summary>
        [DataMember]
        public double? MarketValue { get; set; }

        /// <summary>
        /// Percentage representation of weight entity holds in the Fund/Composite
        /// </summary>
        [DataMember]
        public double? FundWeight { get; set; }

        /// <summary>
        /// Percentage representation of weight entity holds in the Benchmark
        /// </summary>
        [DataMember]
        public double? BenchmarkWeight { get; set; }

        /// <summary>
        ///Difference between the percentage representation of weight entity holds in the Fund/Composite and Benchmark
        /// </summary>
        [DataMember]
        public double? ActivePosition { get; set; }
    }

    [DataContract]
    public class RelativePerformanceActivePosition
    {
        [DataMember]
        public string NodeName { get; set; }

        [DataMember]
        public DateTime ToDate { get; set; }

        [DataMember]
        public string PortGroup { get; set; }

        [DataMember]
        public string Portfolio { get; set; }

        [DataMember]
        public string AGGLVL1 { get; set; }

        [DataMember]
        public string AGGLVL1LongName { get; set; }

        [DataMember]
        public string Issuer { get; set; }

        [DataMember]
        public string Country { get; set; }

        [DataMember]
        public string CountryName { get; set; }

        [DataMember]
        public string CountryZone { get; set; }

        [DataMember]
        public string CountryZone1 { get; set; }

        [DataMember]
        public string SecGroup { get; set; }

        [DataMember]
        public string Sec_Group_Name { get; set; }

        [DataMember]
        public decimal? MarketValue { get; set; }

        [DataMember]
        public decimal? PortfolioWeight1M { get; set; }

        [DataMember]
        public decimal? PortfolioWeight3M { get; set; }

        [DataMember]
        public decimal? PortfolioWeight6M { get; set; }

        [DataMember]
        public decimal? PortfolioWeight1Y { get; set; }

        [DataMember]
        public decimal? PortfolioWeight3Y { get; set; }

        [DataMember]
        public decimal? PortfolioWeight5Y { get; set; }

        [DataMember]
        public decimal? PortfolioWeightSI { get; set; }

        [DataMember]
        public decimal? BenchmarkWeight1M { get; set; }

        [DataMember]
        public decimal? BenchmarkWeight3M { get; set; }

        [DataMember]
        public decimal? BenchmarkWeight6M { get; set; }

        [DataMember]
        public decimal? BenchmarkWeight1Y { get; set; }

        [DataMember]
        public decimal? BenchmarkWeight3Y { get; set; }

        [DataMember]
        public decimal? BenchmarkWeight5Y { get; set; }

        [DataMember]
        public decimal? BenchmarkWeightSI { get; set; }

        [DataMember]
        public decimal? ActivePosition { get; set; }

        [DataMember]
        public decimal? PortfolioWeightDisplay { get; set; }

        [DataMember]
        public decimal? BenchmarkWeightDisplay { get; set; }

        [DataMember]
        public string PortfolioId { get; set; }
    }
}