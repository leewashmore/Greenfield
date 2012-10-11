using System.Linq;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;

namespace GreenField.Gadgets.Helpers
{
    /// <summary>
    /// Extention to row reordering behavior - retrival of RadRowItem
    /// </summary>
    public static class GridViewDataControlExtensions
    {
        /// <summary>
        /// Returns RadRowItem by recursively searching in BaseItemsControl
        /// </summary>
        /// <param name="itemsControl">BaseItemsControl</param>
        /// <param name="dataItem">Data Item</param>
        /// <returns>RadRowItem</returns>
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

        /// <summary>
        /// Returns RadRowItem by recursively searching in GridViewDataControl
        /// </summary>
        /// <param name="dataControl">GridViewDataControl</param>
        /// <param name="item">Data Item</param>
        /// <returns>RadRowItem</returns>
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
