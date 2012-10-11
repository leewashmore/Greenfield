using System;

namespace GreenField.Web.Helpers
{
    /// <summary>
    /// Security information for a powerpoint presentation file
    /// </summary>
    public class SecurityInformation
    {
        /// <summary>
        /// Security Name
        /// </summary>
        public String SecurityName { get; set; }

        /// <summary>
        /// Analyst name
        /// </summary>
        public String Analyst { get; set; }

        /// <summary>
        /// country name
        /// </summary>
        public String Country { get; set; }

        /// <summary>
        /// industry name
        /// </summary>
        public String Industry { get; set; }

        /// <summary>
        /// market capitalization
        /// </summary>
        public String MktCap { get; set; }

        /// <summary>
        /// security closing price
        /// </summary>
        public String Price { get; set; }

        /// <summary>
        /// security future value calculations
        /// </summary>
        public String FVCalc { get; set; }

        /// <summary>
        /// benchmark weightage
        /// </summary>
        public String BSR { get; set; }

        /// <summary>
        /// security recommendation
        /// </summary>
        public String Recommendation { get; set; }

        /// <summary>
        /// current holdings - YES/NO
        /// </summary>
        public String CurrentHoldings { get; set; }

        /// <summary>
        /// security net asset value
        /// </summary>
        public String NAV { get; set; }

        /// <summary>
        /// benchmark weightage
        /// </summary>
        public String BenchmarkWeight { get; set; }
        
        /// <summary>
        /// active weight
        /// </summary>
        public String ActiveWeight { get; set; }

        /// <summary>
        /// YTD relative absolute
        /// </summary>
        public String RetAbsolute { get; set; }

        /// <summary>
        /// YTD relative Loc
        /// </summary>
        public String RetLoc { get; set; }

        /// <summary>
        /// YTD relative EMV
        /// </summary>
        public String RetEMV { get; set; }
    }
}
