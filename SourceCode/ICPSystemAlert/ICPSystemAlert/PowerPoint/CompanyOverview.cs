using System;
using System.Collections.Generic;

namespace ICPSystemAlert
{
    /// <summary>
    /// Company overview information in powerpoint presentation file
    /// </summary>
    public class CompanyOverview
    {
        /// <summary>
        /// Stores security information in powerpoint presentation file
        /// </summary>
        public SecurityInformation SecurityInfo { get; set; }

        /// <summary>
        /// Stores company overview bullet points in powerpoint presentation file
        /// </summary>
        public List<String> CompanyOverviewList { get; set; }
    }
}
