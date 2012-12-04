using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace GreenField.Targeting.Controls
{
    public class CommunicationStateChangedEventArgs : EventArgs
    {
        [DebuggerStepThrough]
        public CommunicationStateChangedEventArgs(ICommunicationStateModel model)
        {
            this.CommunicationStateModel = model;
        }

        public ICommunicationStateModel CommunicationStateModel { get; private set; }
    }

    public delegate void CommunicationStateChangedEventHandler(Object sender, CommunicationStateChangedEventArgs args);
}
