using System;
using System.Collections.Generic;
using Telerik.Windows.Controls.GridView;

namespace GreenField.Gadgets.Helpers
{
    /// <summary>
    /// Custom Event Arguments for Row Reordering Behavior
    /// </summary>
    public class ReorderedEventArgs : EventArgs
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
        /// ReorderedEventArgs
        /// </summary>
        /// <param name="sourceGrid">Source RadGridView</param>
        /// <param name="draggedItems">Dragged items</param>
        public ReorderedEventArgs(GridViewDataControl sourceGrid, IEnumerable<object> draggedItems)
            : base()
        {
            this.SourceGrid = sourceGrid;
            this.DraggedItems = draggedItems;
        }
    }
}
