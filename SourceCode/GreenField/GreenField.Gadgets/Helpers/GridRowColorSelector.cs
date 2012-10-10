using System.Windows;
using Telerik.Windows.Controls;
using GreenField.DataContracts;

namespace GreenField.Gadgets.Helpers
{
    /// <summary>
    /// class to format cell content font depending upon value for market value
    /// </summary>
    public class GridRowColorSelector : StyleSelector
    {
        public override Style SelectStyle(object item, DependencyObject container)
        {
            if (item is RelativePerformanceSecurityData)
            {
                RelativePerformanceSecurityData relativePerformanceSecurityData = item as RelativePerformanceSecurityData ;
                if (relativePerformanceSecurityData.SecurityMarketValue > 0)
                    return PositionHeld;
                else if (relativePerformanceSecurityData.SecurityMarketValue < 0 || relativePerformanceSecurityData.SecurityMarketValue == 0)
                    return PositionNotHeld;
            }
            return null;
        }

        /// <summary>
        /// property returned depending upon market value
        /// </summary>
        public Style PositionHeld { get; set; }

        /// <summary>
        /// property returned depending upon market value
        /// </summary>
        public Style PositionNotHeld { get; set; }
    }
}
