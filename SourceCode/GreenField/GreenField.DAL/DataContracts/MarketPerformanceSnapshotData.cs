using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using GreenField.DAL;


namespace GreenField.DAL
{
    [DataContract]
    public class MarketPerformanceSnapshotData
    {
        [DataMember]
        public MarketSnapshotPreference MarketSnapshotPreferenceInfo { get; set; }

        [DataMember]
        public Decimal? DateToDateReturn { get; set; }

        [DataMember]
        public Decimal? WeekToDateReturn { get; set; }

        [DataMember]
        public Decimal? MonthToDateReturn { get; set; }

        [DataMember]
        public Decimal? QuarterToDateReturn { get; set; }

        [DataMember]
        public Decimal? YearToDateReturn { get; set; }

        [DataMember]
        public Decimal? LastYearReturn { get; set; }

        [DataMember]
        public Decimal? SecondLastYearReturn { get; set; }

        [DataMember]
        public Decimal? ThirdLastYearReturn { get; set; }
    }

}