using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Collections.ObjectModel;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using System.Windows.Input;
using System.Windows;
using Aims.Data.Client;

namespace Aims.Controls
{
    public class SecurityPickerViewModel : NotificationObject
    {
        private ISecurityPickerClientFactory clientFactory;
        private ISecurity selectedSecurity;
        private ICommunicationState communicationState;

        [DebuggerStepThrough]
        public SecurityPickerViewModel(ICommunicationState communicationState, ISecurityPickerClientFactory clientFactory)
        {
            this.communicationState = communicationState;
            this.clientFactory = clientFactory;
            this.RequestDataCommand = new DelegateCommand<AutoCompleteRequest>(request => this.ConsiderRequestingData(request));
            this.PickSecurityCommand = new DelegateCommand(this.ConsiderPickingSecurity);
            this.Items = new ObservableCollection<ISecurity>();
            this.isEnabled = false;
            this.Exclusions = new List<ISecurity>();
        }

        public IEnumerable<ISecurity> Exclusions { get; private set; }

        public void SetExclusions(IEnumerable<ISecurity> exclusions)
        {
            this.Exclusions = exclusions;
        }

        public ObservableCollection<ISecurity> Items { get; private set; }

        public ISecurity SelectedSecurity
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
                var args = new SecurityPickedEventArgs(selectedSecurity);
                this.OnPicked(args);
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
            if (!this.communicationState.IsLoading)
            {
                this.RequestData(request);
            }
        }

        public void RequestData(AutoCompleteRequest request)
        {
            this.communicationState.StartLoading();
            var client = this.clientFactory.CreateSecurityPickerClient();
            client.RequestSecurities(
                request.Pattern,
                data =>
                {
                    this.TakeData(data);
                    request.Callback();
                    this.communicationState.FinishLoading();
                },
                this.communicationState.FinishLoading
            );
        }

        public void TakeData(IEnumerable<ISecurity> securities)
        {
            this.Items.Clear();
            var map = this.Exclusions.Select(x => x.Ticker);
            foreach (var security in securities)
            {
                this.Items.Add(security);

                if (map.Contains(security.Ticker))
                    this.Items.Remove(security);
            }
        }

        public void Clear()
        {
            this.SelectedSecurity = null;
        }

        private Boolean isEnabled;

        public Boolean IsEnabled
        {
            get { return this.isEnabled; }
            set
            {
                this.isEnabled = value;
                this.RaisePropertyChanged(() => this.IsEnabled);
            }
        }

        public void Deactivate()
        {

        }
    }
}
