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
using System.ComponentModel;
using Telerik.Windows.Controls.GridView;
using System.Collections.Generic;

namespace GreenField.Gadgets.Helpers
{
    public class ReorderingEventArgs : CancelEventArgs
    {
        public GridViewDataControl SourceGrid
        {
            get;
            private set;
        }
        public IEnumerable<object> DraggedItems
        {
            get;
            private set;
        }
        public int DropIndex
        {
            get;
            private set;
        }
        public ReorderingEventArgs(GridViewDataControl sourceGrid, IEnumerable<object> draggedItems, int dropIndex)
            : base(false)
        {
            this.SourceGrid = sourceGrid;
            this.DraggedItems = draggedItems;
            this.DropIndex = dropIndex;
        }
    }
}
