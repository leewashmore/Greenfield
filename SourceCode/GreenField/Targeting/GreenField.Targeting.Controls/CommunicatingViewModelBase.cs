using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.ViewModel;
using TopDown.FacingServer.Backend.Targeting;
using System.Collections.ObjectModel;

namespace GreenField.Targeting.Controls
{
    public class CommunicatingViewModelBase : NotificationObject
    {
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
            this.IsLoading = true;
            this.OnCommunicationStateChanged(new LoadingCommunicationStateModel());
        }

        protected virtual void FinishLoading()
        {
            this.OnCommunicationStateChanged(new LoadedCommunicationStateModel());
            this.IsLoading = false;
        }

        protected virtual void FinishLoading(ObservableCollection<IssueModel> issues)
        {
            this.OnCommunicationStateChanged(new IssuesCommunicationStateModel(issues));
            this.IsLoading = false;
        }

        protected virtual void FinishLoading(Exception exception)
        {
            this.OnCommunicationStateChanged(new ErrorCommunicationStateModel(exception));
            this.IsLoading = false;
        }

        public Boolean IsLoading { get; private set; }
    }
}
