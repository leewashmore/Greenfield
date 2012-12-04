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
using System.ComponentModel;
using Microsoft.Practices.Prism.Regions;
using System.Threading;

namespace GreenField.Targeting.Controls
{
    /// <summary>
    /// Is capable of reporting errors.
    /// </summary>
    public abstract class RootViewModelBase : NotificationObject, IDirtyViewModel, IComeAndGoViewModel, IConfirmNavigationRequest
    {
        public RootViewModelBase()
        {
            this.CommunicationStateModel = new LoadedCommunicationStateModel();
        }

        public abstract Boolean HasUnsavedChanges { get; }

        // navigation

        public Boolean IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public abstract void Activate();
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            this.Activate();
        }

        public abstract void Deactivate();
        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            this.Deactivate();
        }

        public void ConfirmNavigationRequest(NavigationContext navigationContext, Action<Boolean> continuationCallback)
        {
            Boolean navigateAway;

            // a different view has been requested
            // so we need to check if there are any unsaved changes
            navigateAway = this.CanGo();

            continuationCallback(navigateAway);
        }

        /// <summary>
        /// Checks if there are any changes. In case there are some, it throws a popup asking if the changes need to be saved.
        /// </summary>
        protected Boolean CanGo()
        {
            Boolean navigateAway;
            if (this.HasUnsavedChanges)
            {
                // there are unsaved changes
                var result = MessageBox.Show("Discard changes? OK: Discard, Cancel: Continue editing", "There are unsaved changes", MessageBoxButton.OKCancel);
                if (result == MessageBoxResult.OK)
                {
                    // don't need any changes, can leave them
                    navigateAway = true;
                }
                else
                {
                    // need them, please stay
                    navigateAway = false;
                }
            }
            else
            {
                // no changes no questions
                navigateAway = true;
            }
            return navigateAway;
        }

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

            public void Resolve(IssuesCommunicationStateModel content)
            {
                this.model.RegisterAcknowledgeableCommunicationState(content);
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

            public void Resolve(IssuesCommunicationStateModel content)
            {
                this.model.UnregisterAcknowledgeableCommunicationState(content);
            }

            public void Resolve(LoadedCommunicationStateModel content)
            {
                // do nothing
            }
        }



    }
}
