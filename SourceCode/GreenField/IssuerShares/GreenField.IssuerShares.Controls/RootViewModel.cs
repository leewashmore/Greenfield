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
using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.Regions;
using Aims.Controls;
using Microsoft.Practices.Prism.Events;
using GreenField.IssuerShares.Client.Backend.IssuerShares;
using System.Linq;
using Aims.Data.Client;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;

namespace GreenField.IssuerShares.Controls
{
    [Export]
    public class RootViewModel : RootViewModelBase
    {
        private IClientFactory clientFactory;

        private SecurityPickerClientFactory securityPickerClientFactory;

        

        [ImportingConstructor]
        public RootViewModel(IClientFactory clientFactory, IEventAggregator aggregator)
        {
            this.clientFactory = clientFactory;
            

            this.securityPickerClientFactory = new SecurityPickerClientFactory(clientFactory);
            
            var securityPickerViewModel = new SecurityPickerViewModel(
                
                new OnlyErrorCommunicationState(this),
                this.securityPickerClientFactory
            );
            
            
            this.SecurityPickerViewModel = securityPickerViewModel;
            securityPickerViewModel.SecurityPicked += (s, e) =>
            {
                this.CompositionViewModel.AddSecurity(e.Security);
                this.SecurityPickerViewModel.Clear();
            };


            this.CompositionViewModel = new CompositionViewModel(clientFactory, aggregator);
            this.compositionViewModel.CommunicationStateChanged += this.WhenCommunicationStateChanges;
            this.HistoryViewModel = new HistoryViewModel(clientFactory, aggregator);
            this.historyViewModel.CommunicationStateChanged += this.WhenCommunicationStateChanges;
            aggregator.GetEvent<SecurityPickedGlobalEvent>().Subscribe(this.TakeSecurity);

            
            
        }

        public void TakeSecurity(SecurityPickedGlobalEventInfo info)
        {
            
            this.securityPickerClientFactory.Initialize(info.SecurityShortName);
            this.SecurityPickerViewModel.IsEnabled = true;
            this.CompositionViewModel.RequestData(info.SecurityShortName);
            this.HistoryViewModel.RequestData(info.SecurityShortName);
        }

        protected override void Activate()
        {
            
        }

        protected override void Deactivate()
        {
        }


        public SecurityPickerViewModel SecurityPickerViewModel { get; set; }

        private Controls.CompositionViewModel compositionViewModel;
        public CompositionViewModel CompositionViewModel
        {
            get
            {
                return this.compositionViewModel;
            }
            set
            {
                this.compositionViewModel = value;
                this.RaisePropertyChanged(() => this.CompositionViewModel);
            }
        }

        private Controls.HistoryViewModel historyViewModel;
        public HistoryViewModel HistoryViewModel
        {
            get
            {
                return this.historyViewModel;
            }
            set
            {
                this.historyViewModel = value;
                this.RaisePropertyChanged(() => this.HistoryViewModel);
            }
        }
    }
}
