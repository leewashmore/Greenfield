using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GreenField.Web.Helpers
{
    /// <summary>
    /// Class that holds Benchmark Data for EM Summary Market Data
    /// </summary>
    public class EMSummaryMarketBenchmarkData
    {      
        /// <summary>
        /// Country Code
        /// </summary>
        public string CountryCode { get; set; }
        /// <summary>
        /// Security Short Name
        /// </summary>
        public string AsecShortName { get; set; }
        /// <summary>
        /// Issuer id
        /// </summary>
        public string IssuerId { get; set; }
        /// <summary>
        /// Issue Name
        /// </summary>
        public string IssueName { get; set; }
        /// <summary>
        /// Benchmark Weight
        /// </summary>
        public Decimal? BenWeight { get; set; }                                                             
    }
}