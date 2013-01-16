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
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;

namespace GreenField.IssuerShares.Controls
{
    public abstract class RootViewModelBase : NotificationObject, IConfirmNavigationRequest
    {
        public RootViewModelBase()
        {
            this.CommunicationStateModel = new LoadedCommunicationStateModel();
        }

        public void ConfirmNavigationRequest(NavigationContext navigationContext, Action<Boolean> continuationCallback)
        {
            continuationCallback(true);
        }


        public Boolean IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            this.Deactivate();
        }

        protected abstract void Activate();

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            this.Activate();
        }

        protected abstract void Deactivate();

        // taking care of the busy (loading) indicator
        protected void WhenCommunicationStateChanges(Object sender, CommunicationStateChangedEventArgs args)
        {
            this.CommunicationStateModel = args.CommunicationStateModel;
        }

        private ICommunicationStateModel communicationStateModel;
        public ICommunicationStateModel CommunicationStateModel
        {
            get { return this.communicationStateModel; }
            set
            {
                var before = this.communicationStateModel;
                if (before != null)
                {
                    before.Accept(new UnregisterCommunicationStateOnceResolved_ICommunicationStateModelResolver(this));
                }
                if (value != null)
                {
                    value.Accept(new RegisterCommunicationStateOnceResolved_ICommunicationStateModelResolver(this));
                }
                this.communicationStateModel = value;
                this.RaisePropertyChanged(() => this.CommunicationStateModel);
            }
        }

        protected void RegisterAcknowledgeableCommunicationState(AcknowledgeableCommunicationStateModelBase model)
        {
            model.Acknowledged += WhenCommunicationStateAcknowledged;
        }

        protected void UnregisterAcknowledgeableCommunicationState(AcknowledgeableCommunicationStateModelBase model)
        {
            model.Acknowledged -= WhenCommunicationStateAcknowledged;
        }

        private void WhenCommunicationStateAcknowledged(object sender, EventArgs e)
        {
            this.CommunicationStateModel = new LoadedCommunicationStateModel();
        }

        private class RegisterCommunicationStateOnceResolved_ICommunicationStateModelResolver : ICommunicationStateModelResolver
        {
            private RootViewModelBase model;

            public RegisterCommunicationStateOnceResolved_ICommunicationStateModelResolver(RootViewModelBase model)
            {
                this.model = model;
            }

            public void Resolve(ErrorCommunicationStateModel content)
            {
                this.model.RegisterAcknowledgeableCommunicationState(content);
            }

            public void Resolve(LoadingCommunicationStateModel content)
            {
                // do nothing
            }

            public void Resolve(LoadedCommunicationStateModel content)
            {
                // do nothing
            }
        }

        private class UnregisterCommunicationStateOnceResolved_ICommunicationStateModelResolver : ICommunicationStateModelResolver
        {
            private RootViewModelBase model;

            public UnregisterCommunicationStateOnceResolved_ICommunicationStateModelResolver(RootViewModelBase model)
            {
                this.model = model;
            }

            public void Resolve(ErrorCommunicationStateModel content)
            {
                this.model.UnregisterAcknowledgeableCommunicationState(content);
            }

            public void Resolve(LoadingCommunicationStateModel content)
            {
                // do nothing
            }

            public void Resolve(LoadedCommunicationStateModel content)
            {
                // do nothing
            }
        }


    }
}
