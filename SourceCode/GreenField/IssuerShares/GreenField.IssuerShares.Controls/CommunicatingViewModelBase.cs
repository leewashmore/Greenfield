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
using Microsoft.Practices.Prism.ViewModel;
using System.Windows.Threading;
using System.ComponentModel.Composition;
using System.Collections.ObjectModel;

namespace GreenField.IssuerShares.Controls
{
    [Export]
    public class CommunicatingViewModelBase : NotificationObject
    {
        private Dispatcher dispatcher;
        [Import]
        public Dispatcher Dispatcher
        {
            get { return this.dispatcher; }
            set { this.dispatcher = value; }
        }

        public event CommunicationStateChangedEventHandler CommunicationStateChanged;


        protected virtual void OnCommunicationStateChanged(CommunicationStateChangedEventArgs args)
        {
            var handler = this.CommunicationStateChanged;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        protected void OnCommunicationStateChanged(ICommunicationStateModel model)
        {
            this.OnCommunicationStateChanged(new CommunicationStateChangedEventArgs(model));
        }

        protected virtual void StartLoading()
        {
            this.OnCommunicationStateChanged(new LoadingCommunicationStateModel());
        }

        protected virtual void FinishLoading()
        {
            this.OnCommunicationStateChanged(new LoadedCommunicationStateModel());
        }

        protected virtual void FinishLoading(Exception exception)
        {
            this.OnCommunicationStateChanged(new ErrorCommunicationStateModel(exception));
        }
    }
}

