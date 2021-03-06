﻿using System;
using System.Collections.Generic;
using System.Linq;
 
using System.Runtime.Serialization;

namespace GreenField.DataContracts
{
    /// <summary>
    /// Data Contract class for Closing Price Chart.
    /// </summary>
    [DataContract]
    public class PricingReferenceData
    {
        [DataMember]
        public string Type { get; set; }

        [DataMember]
        public string IssueName { get; set; }

        [DataMember]
        public string Ticker { get; set; }

        [DataMember]
        public DateTime FromDate { get; set; }

        [DataMember]
        public decimal DailyClosingPrice { get; set; }

        [DataMember]
        public decimal DailyPriceReturn { get; set; }

        [DataMember]
        public decimal DailyGrossReturn { get; set; }

        [DataMember]
        public decimal Volume { get; set; }

        [DataMember]
        public decimal DailySpotFX { get; set; }

        [DataMember]
        public string InstrumentID { get; set; }

        [DataMember]
        public decimal IndexedPrice { get; set; }

        [DataMember]
        public decimal AdjustedDollarPrice { get; set; }

        [DataMember]
        public int SortingID { get; set; }
    }
}