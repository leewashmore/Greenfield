using System.Collections.Generic;
using System.ComponentModel;
using Telerik.Windows.Controls.GridView;

namespace GreenField.Gadgets.Helpers
{
    /// <summary>
    /// Custom Event Arguments for Row Reordering Behavior
    /// </summary>
    public class ReorderingEventArgs : CancelEventArgs
    {
        /// <summary>
        /// Source RadGridView
        /// </summary>
        public GridViewDataControl SourceGrid
        {
            get;
            private set;
        }

        /// <summary>
        /// Dragged items
        /// </summary>
        public IEnumerable<object> DraggedItems
        {
            get;
            private set;
        }

        /// <summary>
        /// Drop Index
        /// </summary>
        public int DropIndex
        {
            get;
            private set;
        }

        /// <summary>
        /// DragStartedEventArgs
        /// </summary>
        /// <param name="sourceGrid">Source RadGridView</param>
        /// <param name="draggedItems">Dragged items</param>
        /// <param name="dropIndex">Drop Index</param>
        public ReorderingEventArgs(GridViewDataControl sourceGrid, IEnumerable<object> draggedItems, int dropIndex)
            : base(false)
        {
            this.SourceGrid = sourceGrid;
            this.DraggedItems = draggedItems;
            this.DropIndex = dropIndex;
        }
    }
}
