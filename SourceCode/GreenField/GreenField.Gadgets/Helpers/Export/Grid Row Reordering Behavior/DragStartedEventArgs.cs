using System;
using System.Collections.Generic;
using Telerik.Windows.Controls;

namespace GreenField.Gadgets.Helpers
{
    /// <summary>
    /// Custom Event Arguments for Row Reordering Behavior
    /// </summary>
    public class DragStartedEventArgs : EventArgs
    {
        /// <summary>
        /// Source RadGridView
        /// </summary>
        public RadGridView SourceGrid
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
        /// DragStartedEventArgs
        /// </summary>
        /// <param name="sourceGrid">Source RadGridView</param>
        /// <param name="draggedItems">Dragged items</param>
        public DragStartedEventArgs(RadGridView sourceGrid, IEnumerable<object> draggedItems)
            : base()
        {
            this.SourceGrid = sourceGrid;
            this.DraggedItems = draggedItems;
        }
    }
}
