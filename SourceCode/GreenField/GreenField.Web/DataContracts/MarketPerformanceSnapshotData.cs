using System;
using System.Runtime.Serialization;
using GreenField.DAL;

namespace GreenField.Web.DataContracts
{
    /// <summary>
    /// Data constract for MarketSnapshotPerformanceData
    /// </summary>
    [DataContract]
    public class MarketSnapshotPerformanceData
    {
        /// <summary>
        /// Stores Market Snapshot Preference information
        /// </summary>
        [DataMember]
        public MarketSnapshotPreference MarketSnapshotPreferenceInfo { get; set; }

        /// <summary>
        /// Stores Date To Date Return
        /// </summary>
        [DataMember]
        public Decimal? DateToDateReturn { get; set; }

        /// <summary>
        /// Stores Week To Date Return
        /// </summary>
        [DataMember]
        public Decimal? WeekToDateReturn { get; set; }

        /// <summary>
        /// Stores Month To Date Return
        /// </summary>
        [DataMember]
        public Decimal? MonthToDateReturn { get; set; }

        /// <summary>
        /// Stores Quarter To Date Return
        /// </summary>
        [DataMember]
        public Decimal? QuarterToDateReturn { get; set; }

        /// <summary>
        /// Stores Year To Date Return
        /// </summary>
        [DataMember]
        public Decimal? YearToDateReturn { get; set; }

        /// <summary>
        /// Stores Last Year Return
        /// </summary>
        [DataMember]
        public Decimal? LastYearReturn { get; set; }

        /// <summary>
        /// Stores Second Last Year Return
        /// </summary>
        [DataMember]
        public Decimal? SecondLastYearReturn { get; set; }

        /// <summary>
        /// Stores Third Last Year Return
        /// </summary>
        [DataMember]
        public Decimal? ThirdLastYearReturn { get; set; }
    }

}