using System;
using System.Net;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace GreenField.DataContracts
{
    /// <summary>
    /// Data Contract for FairValue Composition Summary Data
    /// </summary>
    [DataContract]
    public class FairValueCompositionSummaryData
    {
        /// <summary>
        /// Source i.e Primary, Industry, etc
        /// </summary>
        [DataMember]
        public string Source{get; set;}

        /// <summary>
        /// Measure e.g. Forward P/E Ratio, etc.
        /// </summary>
        [DataMember]
        public string Measure{get;set;}

        /// <summary>
        /// Buy Value
        /// </summary>
        [DataMember]
        public decimal? Buy{get; set;}

        /// <summary>
        /// Sell value
        /// </summary>
        [DataMember]
        public decimal? Sell { get; set; }

        /// <summary>
        /// Upside value
        /// </summary>
        [DataMember]
        public decimal? Upside { get; set; }

        /// <summary>
        /// Updated Date Time
        /// </summary>
        [DataMember]
        public DateTime? Date { get; set; }

        /// <summary>
        /// Date Id of Measure
        /// </summary>
        [DataMember]
        public Int32? DataId { get; set; }

        /// <summary>
        /// Name of Primary Analyst
        /// </summary>
        [DataMember]
        public String PrimaryAnalyst { get; set; }

        /// <summary>
        /// Name of secondary analyst
        /// </summary>
        [DataMember]
        public String IndustryAnalyst { get; set; }
    }    
}
