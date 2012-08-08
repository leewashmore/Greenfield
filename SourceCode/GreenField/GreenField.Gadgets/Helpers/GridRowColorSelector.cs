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
using Telerik.Windows.Controls;
using GreenField.Gadgets.ViewModels;
using GreenField.ServiceCaller.BenchmarkHoldingsDefinitions;
using GreenField.DataContracts;

namespace GreenField.Gadgets.Helpers
{
    public class GridRowColorSelector : StyleSelector
    {
        public override Style SelectStyle(object item, DependencyObject container)
        {
            if (item is RelativePerformanceSecurityData)
            {
                RelativePerformanceSecurityData relativePerformanceSecurityData = item as RelativePerformanceSecurityData ;
                if (relativePerformanceSecurityData.SecurityMarketValue > 0)
                    return PositionHeld;
                else if (relativePerformanceSecurityData.SecurityMarketValue < 0)
                    return PositionNotHeld;
            }
            return null;
        }

        public Style PositionHeld { get; set; }
        public Style PositionNotHeld { get; set; }
    }
}
