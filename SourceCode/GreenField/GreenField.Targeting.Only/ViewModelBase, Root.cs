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

namespace GreenField.Targeting.Only
{
    /// <summary>
    /// Is capable of reporting errors.
    /// </summary>
    public abstract class RootViewModelBase : ViewModelBase, IDirtyViewModel, IComeAndGoViewModel, IConfirmNavigationRequest
    {
        public abstract Boolean HasUnsavedChanges { get; }


        private Boolean isLoading;
        /// <summary>
        /// Controls whether a spiner that says "Loading..." is shown.
        /// </summary>
        public Boolean IsLoading
        {
            get { return this.isLoading; }
            set
            {
                this.isLoading = value;
                this.RaisePropertyChanged(() => this.IsLoading);
            }
        }


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

            continuationCallback(navigateAway);
        }
    }
}
