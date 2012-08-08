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
using System.Linq;
using Telerik.Windows.Controls.GridView;

namespace GreenField.Benchmark.Helpers
{
    public static class GridViewDataControlExtensions
    {
        internal static RadRowItem GetContainerFromItemRecursive(this BaseItemsControl itemsControl, object dataItem)
        {
            RadRowItem container = null;
            var panel = itemsControl.ChildrenOfType<GridViewVirtualizingPanel>().FirstOrDefault();
            if (itemsControl != null && panel != null)
            {
                foreach (var child in panel.Children)
                {
                    var groupRow = child as GridViewGroupRow;
                    var row = child as GridViewRow;
                    if (groupRow != null)
                        container = groupRow.GetContainerFromItemRecursive(dataItem);
                    if (row != null && ReferenceEquals(row.Item, dataItem))
                        container = child as RadRowItem;
                    if (container != null)
                        return container;
                }
            }
            return container;
        }
        internal static RadRowItem GetContainerFromDataItem(this GridViewDataControl dataControl, object item)
        {
            if (item == null || dataControl == null)
                return null;
            if (dataControl.IsGrouping)
                return dataControl.GetContainerFromItemRecursive(item);
            else
                return dataControl.ItemContainerGenerator.ContainerFromItem(item) as RadRowItem;
        }
    }
}
