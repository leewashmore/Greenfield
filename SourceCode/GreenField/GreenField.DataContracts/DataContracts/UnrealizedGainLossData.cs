using System;
using System.Collections.Generic;
using System.Linq;
 
using System.Runtime.Serialization;

namespace GreenField.DataContracts
{
    [DataContract]
    public class UnrealizedGainLossData
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
        public decimal? DailyClosingPrice { get; set; }

        [DataMember]
        public decimal DailyPriceReturn { get; set; }

        [DataMember]
        public decimal DailyGrossReturn { get; set; }

        [DataMember]
        public decimal? Volume { get; set; }

        [DataMember]
        public decimal DailySpotFX { get; set; }

        [DataMember]
        public string InstrumentID { get; set; }

        [DataMember]
        public decimal? AdjustedPrice { get; set; }

        [DataMember]
        public decimal? MovingAverage { get; set; }

        [DataMember]
        public decimal? NinetyDayWtAvg { get; set; }

        [DataMember]
        public decimal? Cost { get; set; }

        [DataMember]
        public decimal? WtAvgCost { get; set; }

        [DataMember]
        public decimal? UnrealizedGainLoss { get; set; }
    }
}