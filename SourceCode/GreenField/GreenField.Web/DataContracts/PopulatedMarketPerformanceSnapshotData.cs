using System.Collections.Generic;
using System.Runtime.Serialization;
using GreenField.DAL;

namespace GreenField.Web.DataContracts
{
    /// <summary>
    /// Data contract for PopulatedMarketSnapshotPerformanceData
    /// </summary>
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
