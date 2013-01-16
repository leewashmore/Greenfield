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

namespace GreenField.IssuerShares.Controls
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
