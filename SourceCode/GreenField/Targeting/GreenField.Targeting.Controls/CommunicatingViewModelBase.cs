using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;
using TopDown.FacingServer.Backend.Targeting;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.ComponentModel.Composition;

namespace GreenField.Targeting.Controls
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

        protected virtual void FinishLoading(ObservableCollection<IssueModel> issues)
        {
            this.OnCommunicationStateChanged(new IssuesCommunicationStateModel(issues));
        }

        protected virtual void FinishLoading(ObservableCollection<IssueModel> issues, Action acknowledgeCallback)
        {
            /*
                var timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromMilliseconds(100);
                timer.Tick += (s1, e1) =>
                {
                    acknowledgeCallback();
                    timer.Stop();
                    timer = null;
                };
                timer.Start();
            */

            var args = new IssuesCommunicationStateModel(issues);
            args.Acknowledged += (s, e) =>
            {
                this.dispatcher.BeginInvoke(acknowledgeCallback);
            };

            this.OnCommunicationStateChanged(args);
        }

        protected virtual void FinishLoading(Exception exception)
        {
            this.OnCommunicationStateChanged(new ErrorCommunicationStateModel(exception));
        }
    }
}
