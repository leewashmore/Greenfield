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
using Microsoft.Practices.Prism.Events;
using System.IO;
using GreenField.ServiceCaller.SecurityReferenceDefinitions;
using GreenField.ServiceCaller;
using Microsoft.Practices.Prism.Logging;
using GreenField.Common.Helper;
using GreenField.ServiceCaller.BenchmarkHoldingsPerformanceDefinitions;

namespace GreenField.Common
{
    public class RelativePerformanceGridClickEvent : CompositePresentationEvent<RelativePerformanceGridCellData> { }

    public class DashboardGadgetParamFetchEvent : CompositePresentationEvent<DashboardGadgetParam> { }

    public class SecurityReferenceSetEvent : CompositePresentationEvent<EntitySelectionData> { }

    public class FundReferenceSetEvent : CompositePresentationEvent<FundSelectionData> { }

    public class BenchmarkReferenceSetEvent : CompositePresentationEvent<BenchmarkSelectionData> { }

    public class MarketPerformanceSnapshotNameReferenceSetEvent : CompositePresentationEvent<MarketSnapshotSelectionData> { }

    public class EffectiveDateSet : CompositePresentationEvent<DateTime> { }

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

}


