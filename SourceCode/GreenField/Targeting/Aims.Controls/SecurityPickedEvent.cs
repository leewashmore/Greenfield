using System;
using System.Diagnostics;
using Aims.Data.Client;

namespace Aims.Controls
{
    public class SecurityPickedEventArgs : EventArgs
    {
        [DebuggerStepThrough]
        public SecurityPickedEventArgs(ISecurity security)
        {
            this.Security = security;
        }

        public ISecurity Security { get; private set; }
    }

    public delegate void SecurityPickedEventHandler(Object sender, SecurityPickedEventArgs args);
}
