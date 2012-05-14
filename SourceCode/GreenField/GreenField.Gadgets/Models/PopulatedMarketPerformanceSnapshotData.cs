using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using GreenField.ServiceCaller.PerformanceDefinitions;
using System.Collections.Generic;

namespace GreenField.Gadgets.Models
{
    public class PopulatedMarketPerformanceSnapshotData
    {
        /// <summary>
        /// Stores the snapshot selection data with reference to the snapshot credentials
        /// </summary>
        public MarketSnapshotSelectionData MarketSnapshotSelectionInfo { get; set; }

        /// <summary>
        /// Stores the performance data for the snapshot selection data
        /// </summary>
        public List<MarketPerformanceSnapshotData> MarketPerformanceSnapshotInfo { get; set; }
    }
}
