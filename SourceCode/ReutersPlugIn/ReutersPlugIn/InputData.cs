using System;

namespace ReutersPlugIn
{
    /// <summary>
    /// Input from 'Model Reference' worksheet
    /// </summary>
    class InputData
    {
        /// <summary>
        /// Issuer ID
        /// </summary>
        public String IssuerID { get; set; }

        /// <summary>
        /// Issue Name
        /// </summary>
        public String IssueName { get; set; }

        /// <summary>
        /// COA Type
        /// </summary>
        public String COAType { get; set; }

        /// <summary>
        /// Currency
        /// </summary>
        public String Currency { get; set; }
    }
}
