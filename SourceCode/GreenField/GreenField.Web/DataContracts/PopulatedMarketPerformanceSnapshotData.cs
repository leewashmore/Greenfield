using System;
using System.Net;
using System.Runtime.Serialization;
using System.Collections.Generic;
using GreenField.DAL;

namespace GreenField.Web.DataContracts
{
    [DataContract]
    public class PopulatedMarketSnapshotPerformanceData
    {
        /// <summary>
        /// Stores the snapshot selection data with reference to the snapshot credentials
        /// </summary>
        [DataMember]
        public MarketSnapshotSelectionData MarketSnapshotSelectionInfo { get; set; }

        /// <summary>
        /// Stores the performance data for the snapshot selection data
        /// </summary>
        [DataMember]
        public List<MarketSnapshotPerformanceData> MarketPerformanceSnapshotInfo { get; set; }
    }
}
