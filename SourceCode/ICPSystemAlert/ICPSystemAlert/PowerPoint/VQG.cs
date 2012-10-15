using System;
using System.Collections.Generic;

namespace ICPSystemAlert
{
    /// <summary>
    /// Value, quality and growth information in powerpoint presentation file
    /// </summary>
    public class VQG
    {
        /// <summary>
        /// Stores Value information
        /// </summary>
        public List<String> Value { get; set; }

        /// <summary>
        /// Stores Quality information
        /// </summary>
        public List<String> Quality { get; set; }

        /// <summary>
        /// Stores Growth information
        /// </summary>
        public List<String> Growth { get; set; }
    }
}
