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
using Telerik.Windows.Controls;
using System.Collections.Generic;

namespace GreenField.Benchmark.Helpers
{
    public class BeginningDragEventArgs : CancelEventArgs
    {
        public RadGridView SourceGrid
        {
            get;
            private set;
        }
        public IEnumerable<object> DraggedItems
        {
            get;
            private set;
        }
        public BeginningDragEventArgs(RadGridView sourceGrid, IEnumerable<object> draggedItems)
            : base()
        {
            this.SourceGrid = sourceGrid;
            this.DraggedItems = draggedItems;
        }
    }
}
