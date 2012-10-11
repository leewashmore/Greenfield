using System;
using System.Collections.Generic;

namespace GreenField.Web.Helpers
{
    /// <summary>
    /// Strength, weakness, opportunity and threat information in powerpoint presentation file
    /// </summary>
    public class SWOTAnalysis
    {
        /// <summary>
        /// Stores Strength information
        /// </summary>
        public List<String> Strength { get; set; }

        /// <summary>
        /// Stores Weakness information
        /// </summary>
        public List<String> Weakness { get; set; }

        /// <summary>
        /// Stores Opportunity information
        /// </summary>
        public List<String> Opportunity { get; set; }

        /// <summary>
        /// Stores Threat information
        /// </summary>
        public List<String> Threat { get; set; }
    }
}
