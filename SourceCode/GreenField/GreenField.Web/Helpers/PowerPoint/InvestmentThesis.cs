using System;
using System.Collections.Generic;

namespace GreenField.Web.Helpers
{
    /// <summary>
    /// Investment thesis information in powerpoint presentation file
    /// </summary>
    public class InvestmentThesis
    {
        /// <summary>
        /// Stores investment thesis points in powerpoint presentation file
        /// </summary>
        public List<String> ThesisPoints { get; set; }

        /// <summary>
        /// Stores investment risk information in powerpoint presentation file
        /// </summary>
        public List<String> HighlightedRisks { get; set; }
    }
}
