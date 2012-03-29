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
using GreenField.ServiceCaller.ProxyDataDefinitions;
using GreenField.ServiceCaller;
using Microsoft.Practices.Prism.Logging;
using GreenField.Common.Helper;

namespace GreenField.Common
{
    public class RelativePerformanceGridClickEvent : CompositePresentationEvent<RelativePerformanceGridCellData> { }

    public class SecurityReferenceSetEvent : CompositePresentationEvent<EntitySelectionData> { }

    public class FundReferenceSetEvent : CompositePresentationEvent<FundSelectionData> { }

    public class BenchmarkReferenceSetEvent : CompositePresentationEvent<BenchmarkSelectionData> { }

    public class EffectiveDateSet : CompositePresentationEvent<DateTime> { }

    public class DashboardGadgetLoad : CompositePresentationEvent<DashboardGadgetPayLoad> { }

    public class DashboardGadgetSave : CompositePresentationEvent<object> { }

    public class DashBoardTileViewItemAdded : CompositePresentationEvent<DashBoardTileViewItemInfo> { }

    public class DashBoardTileViewItemInfo
    {
        public string DashBoardTileHeader { get; set; }
        public object DashBoardTileObject { get; set; }        
    }

    /// <summary>
    /// Prism parameters passed to Gadget viewmodel class
    /// </summary>
    public class DashBoardGadgetParam
    {
        public IEventAggregator EventAggregator { get; set; }
        public IDBInteractivity DBInteractivity { get; set; }
        public ILoggerFacade LoggerFacade { get; set; }
        public DashboardGadgetPayLoad DashboardGadgetPayLoad { get; set; }        
    }


}


