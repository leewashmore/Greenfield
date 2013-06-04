using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Collections.ObjectModel;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using System.Windows.Input;
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
            var security = this.SelectedSecurity;
            if (security == null)
            {
                this.OnReset();
            }
            else
            {
                var args = new SecurityPickedEventArgs(security);
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

        private string pattern;
        public void RequestData(AutoCompleteRequest request)
        {
            if (this.pattern != request.Pattern)
            {
                this.pattern = request.Pattern;
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
        }

        public void TakeData(IEnumerable<ISecurity> securities)
        {
            this.Items.Clear();
            foreach (var security in securities)
            {
                this.Items.Add(security);
            }
            
           
        }

        public void Clear()
        {
            this.SelectedSecurity = null;
        }

        private Boolean isEnabled;
        private String securityPickerLabelName = "Security";

        public String SecurityPickerLabelName
        {
            get { return this.securityPickerLabelName; }
            set
            {
                this.securityPickerLabelName = value;
                this.RaisePropertyChanged(() => this.SecurityPickerLabelName);
            }
        }

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
