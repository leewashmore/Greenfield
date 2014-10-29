using System;
using System.Collections.Generic;

namespace GreenField.Web.Helpers
{
    /// <summary>
    /// Key operating assumption information in powerpoint presentation file
    /// </summary>
    public class KeyOperatingAssumpations
    {
        /// <summary>
        /// Stores key operating assumption information in powerpoint presentation file
        /// </summary>
        public List<String> Assumptions { get; set; }
        public Dictionary<String,List<string>>  KeyAssumptions { get; set; }
    }
}
