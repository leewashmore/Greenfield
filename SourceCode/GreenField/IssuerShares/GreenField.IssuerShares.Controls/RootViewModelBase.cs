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
        public void ConfirmNavigationRequest(NavigationContext navigationContext, Action<Boolean> continuationCallback)
        {
            throw new NotImplementedException();
        }

        public Boolean IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            this.Activate();
        }

        protected abstract void Activate();

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            this.Deactivate();
        }

        protected abstract void Deactivate();
    }
}
