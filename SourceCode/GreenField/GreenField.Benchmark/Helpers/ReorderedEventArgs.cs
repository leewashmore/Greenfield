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
using Telerik.Windows.Controls.GridView;
using System.Collections.Generic;

namespace GreenField.Benchmark.Helpers
{
    public class ReorderedEventArgs : EventArgs
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
        public ReorderedEventArgs(GridViewDataControl sourceGrid, IEnumerable<object> draggedItems)
            : base()
        {
            this.SourceGrid = sourceGrid;
            this.DraggedItems = draggedItems;
        }
    }
}
