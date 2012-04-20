using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using GreenField.DAL;

namespace GreenField.Web.DataContracts
{
    [DataContract]
    public class MarketPerformanceSnapshotData
    {
        [DataMember]
        public MarketSnapshotPreference MarketSnapshotPreferenceInfo { get; set; }

        [DataMember]
        public double? DateToDateReturn { get; set; }

        [DataMember]
        public double? WeekToDateReturn { get; set; }

        [DataMember]
        public double? MonthToDateReturn { get; set; }

        [DataMember]
        public double? QuarterToDateReturn { get; set; }

        [DataMember]
        public double? YearToDateReturn { get; set; }

        [DataMember]
        public double? LastYearReturn { get; set; }

        [DataMember]
        public double? SecondLastYearReturn { get; set; }

        [DataMember]
        public double? ThirdLastYearReturn { get; set; }
    }

}