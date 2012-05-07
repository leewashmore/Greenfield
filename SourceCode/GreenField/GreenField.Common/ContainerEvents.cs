﻿using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Practices.Prism.Events;
using System.IO;
using GreenField.ServiceCaller.SecurityReferenceDefinitions;
using GreenField.ServiceCaller;
using Microsoft.Practices.Prism.Logging;
using GreenField.Common.Helper;
using GreenField.ServiceCaller.BenchmarkHoldingsPerformanceDefinitions;
using GreenField.ServiceCaller.PerformanceDefinitions;
using System.Collections.Generic;

namespace GreenField.Common
{

    public class RelativePerformanceGridClickEvent : CompositePresentationEvent<RelativePerformanceGridCellData> { }

    public class DashboardGadgetParamFetchEvent : CompositePresentationEvent<DashboardGadgetParam> { }

    public class SecurityReferenceSetEvent : CompositePresentationEvent<EntitySelectionData> { }

    public class PortfolioReferenceSetEvent : CompositePresentationEvent<PortfolioSelectionData> { }

    public class EffectiveDateReferenceSetEvent : CompositePresentationEvent<DateTime> { }

    public class PeriodReferenceSetEvent : CompositePresentationEvent<String> { }

    public class BenchmarkReferenceSetEvent : CompositePresentationEvent<BenchmarkSelectionData> { }

    public class HoldingFilterReferenceSetEvent : CompositePresentationEvent<FilterSelectionData> { }

    public class MarketPerformanceSnapshotReferenceSetEvent : CompositePresentationEvent<MarketSnapshotSelectionData> { }

    public class MarketPerformanceSnapshotActionEvent : CompositePresentationEvent<MarketPerformanceSnapshotActionPayload> { }

    public class MarketPerformanceSnapshotActionCompletionEvent : CompositePresentationEvent<MarketPerformanceSnapshotActionPayload> { }    

    public class DashboardGadgetLoad : CompositePresentationEvent<DashboardGadgetPayload> { }

    public class DashboardGadgetSave : CompositePresentationEvent<object> { }

    public class DashboardTileViewItemAdded : CompositePresentationEvent<DashboardTileViewItemInfo> { }

    public class DashboardTileViewItemInfo
    {
        public string DashboardTileHeader { get; set; }
        public object DashboardTileObject { get; set; }        
    }

    /// <summary>
    /// Prism parameters passed to Gadget viewmodel class
    /// </summary>
    public class DashboardGadgetParam
    {
        public IEventAggregator EventAggregator { get; set; }
        public IDBInteractivity DBInteractivity { get; set; }
        public IManageSessions ManageSessions { get; set; }
        public ILoggerFacade LoggerFacade { get; set; }
        public DashboardGadgetPayload DashboardGadgetPayload { get; set; }        
    }

    /// <summary>
    /// Stores the payload for the MarketPerformanceSnapshotActionEvent and MarketPerformanceSnapshotActionCompletionEvent
    /// </summary>
    public class MarketPerformanceSnapshotActionPayload
    {
        /// <summary>
        /// Store the Action Type
        /// </summary>
        public MarketPerformanceSnapshotActionType ActionType { get; set; }

        /// <summary>
        /// Stores user's snapshot details
        /// </summary>
        public List<MarketSnapshotSelectionData> MarketSnapshotSelectionInfo { get; set; }

        /// <summary>
        /// Stores snapshot details for the selected snapshot
        /// </summary>
        public MarketSnapshotSelectionData SelectedMarketSnapshotSelectionIndo { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public MarketPerformanceSnapshotActionPayload()
        {
        }
    }
}


