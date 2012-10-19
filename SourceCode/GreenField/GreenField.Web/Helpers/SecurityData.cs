using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GreenField.Web.Helpers
{
    /// <summary>
    /// Class containing the data from GF_SECURITY_BASEVIEW
    /// </summary>
    public class SecurityData
    {
        /// <summary>
        /// Benchmark Name
        /// </summary>
        public string BenchmarkName { get; set; }
        /// <summary>
        /// Security id
        /// </summary>
        public string SecurityId { get; set; }
        /// <summary>
        /// Issuer id
        /// </summary>
        public string IssuerId { get; set; }
        /// <summary>
        /// Issue Name
        /// </summary>
        public string IssueName { get; set; }
        /// <summary>
        /// Security Short Name
        /// </summary>
        public string AsecShortName { get; set; }
    }
}