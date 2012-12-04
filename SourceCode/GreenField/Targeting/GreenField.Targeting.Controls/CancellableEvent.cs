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
using System.Diagnostics;

namespace GreenField.Targeting.Controls
{
    public class CancellableEventArgs : EventArgs
    {
        [DebuggerStepThrough]
        public CancellableEventArgs(Boolean isCancelled)
        {
            this.IsCancelled = isCancelled;
        }

        public Boolean IsCancelled { get; set; }
    }

    public delegate void CancellableEventHandler(Object sender, CancellableEventArgs args);
}
