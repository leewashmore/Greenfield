using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace GreenField.Web.DataContracts
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
        public double AdjustedPrice { get; set; }

        [DataMember]
        public double MovingAverage { get; set; }

        [DataMember]
        public double NinetyDayWtAvg { get; set; }

        [DataMember]
        public decimal Cost { get; set; }

        [DataMember]
        public double WtAvgCost { get; set; }

        [DataMember]
        public double UnrealizedGainLoss { get; set; }  
    }
}