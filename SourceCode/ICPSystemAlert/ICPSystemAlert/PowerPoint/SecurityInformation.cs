using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ICPSystemAlert
{
     public class SecurityInformation
    {
        public String SecurityName { get; set; }
        public String Analyst { get; set; }
        public String Country { get; set; }
        public String Industry { get; set; }
        public String MktCap { get; set; }
        public String Price { get; set; }
        public String FVCalc { get; set; }
        public String BSR { get; set; }
        public String Recommendation { get; set; }
        public String CurrentHoldings { get; set; }
        public String NAV { get; set; }
        public String BenchmarkWeight { get; set; }
        public String ActiveWeight { get; set; }
        public String RetAbsolute { get; set; }
        public String RetLoc { get; set; }
        public String RetEMV { get; set; }
    }
}
