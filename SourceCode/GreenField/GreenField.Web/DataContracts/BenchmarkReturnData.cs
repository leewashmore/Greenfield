using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace GreenField.Web.DataContracts
{
    [DataContract]
    public class BenchmarkChartReturnData
    {
        [DataMember]
        public string InstrumentID { get; set; }

        [DataMember]
        public string Ticker { get; set; }

        [DataMember]
        public string IssueName { get; set; }

        [DataMember]
        public string Type { get; set; }

        [DataMember]
        public DateTime FromDate { get; set; }

        [DataMember]
        public decimal DailyReturn { get; set; }

    }

    [DataContract]
    public class BenchmarkGridReturnData
    {
        [DataMember]
        public string InstrumentID { get; set; }

        [DataMember]
        public string Ticker { get; set; }

        [DataMember]
        public string IssueName { get; set; }

        [DataMember]
        public string Type { get; set; }

        [DataMember]
        public decimal MTD { get; set; }

        [DataMember]
        public decimal QTD { get; set; }

        [DataMember]
        public decimal YTD { get; set; }

        [DataMember]
        public decimal PreviousYearData { get; set; }

        [DataMember]
        public decimal TwoPreviousYearData { get; set; }

        [DataMember]
        public decimal ThreePreviousYearData { get; set; }
    }
}