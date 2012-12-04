using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Collections.ObjectModel;
using TopDown.FacingServer.Backend.Targeting;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using System.Windows.Input;
using System.Windows;

namespace GreenField.Targeting.Controls
{
    public class SecurityPickerViewModel : CommunicatingViewModelBase
    {
        private IClientFactory clientFactory;
        private Int32 maxNumberOfSecurities;
        private SecurityModel selectedSecurity;

        [DebuggerStepThrough]
        public SecurityPickerViewModel(IClientFactory clientFactory, Int32 maxNumberOfSecurities)
        {
            this.clientFactory = clientFactory;
            this.maxNumberOfSecurities = maxNumberOfSecurities;
            this.RequestDataCommand = new DelegateCommand<AutoCompleteRequest>(request => this.ConsiderRequestingData(request));
            this.PickSecurityCommand = new DelegateCommand(this.ConsiderPickingSecurity);
            this.Items = new ObservableCollection<SecurityModel>();
        }

        public ObservableCollection<SecurityModel> Items { get; private set; }

        public SecurityModel SelectedSecurity
        {
            get { return this.selectedSecurity; }
            set
            {
                this.selectedSecurity = value;
                this.RaisePropertyChanged(() => this.SelectedSecurity);
            }
        }

        public ICommand PickSecurityCommand { get; private set; }

        private void ConsiderPickingSecurity()
        {
            var selectedSecurity = this.SelectedSecurity;
            if (selectedSecurity == null)
            {
                this.OnReset();
            }
            else
            {
                this.OnPicked(new SecurityPickedEventArgs(selectedSecurity));
            }
            this.SelectedSecurity = null;
        }

        public event SecurityPickedEventHandler SecurityPicked;
        protected virtual void OnPicked(SecurityPickedEventArgs args)
        {
            var handler = this.SecurityPicked;
            if (handler != null)
            {
                handler(this, args);
            }
        }

        public event EventHandler Reset;
        protected virtual void OnReset()
        {
            var handler = this.Reset;
            if (handler != null)
            {
                handler(this, new EventArgs());
            }
        }

        public ICommand RequestDataCommand { get; private set; }

        private void ConsiderRequestingData(AutoCompleteRequest request)
        {
            if (!this.IsLoading)
            {
                this.RequestData(request);
            }
        }

        public void RequestData(AutoCompleteRequest request)
        {
            this.StartLoading();
            var client = this.clientFactory.CreateClient();
            client.PickSecuritiesCompleted += (sender, args) => RuntimeHelper.TakeCareOfResult(
                "Getting securities that match \"" + request.Pattern + "\".",
                args, x => new AutoCompleteResponse<ObservableCollection<SecurityModel>>(request, x.Result), this.TakeData, this.FinishLoading
            );
            client.PickSecuritiesAsync(request.Pattern, this.maxNumberOfSecurities);
        }

        public void TakeData(AutoCompleteResponse<ObservableCollection<SecurityModel>> response)
        {
            this.Items.Clear();
            foreach (var security in response.Result)
            {
                this.Items.Add(security);
            }
            response.Request.Callback();
            this.FinishLoading();
        }

        public void Clear()
        {
            this.SelectedSecurity = null;
        }

        public void Deactivate()
        {
            throw new NotImplementedException();
        }
    }
}
