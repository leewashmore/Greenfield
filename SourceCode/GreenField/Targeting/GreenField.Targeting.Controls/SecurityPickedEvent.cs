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
using TopDown.FacingServer.Backend.Targeting;
using System.Diagnostics;

namespace GreenField.Targeting.Controls
{
    public class SecurityPickedEventArgs : EventArgs
    {
        [DebuggerStepThrough]
        public SecurityPickedEventArgs(SecurityModel security)
        {
            this.Security = security;
        }

        public SecurityModel Security { get; private set; }
    }

    public delegate void SecurityPickedEventHandler(Object sender, SecurityPickedEventArgs args);
}
