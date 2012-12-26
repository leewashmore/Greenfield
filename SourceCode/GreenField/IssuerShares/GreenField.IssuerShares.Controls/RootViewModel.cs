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

namespace GreenField.IssuerShares.Controls
{
    [Export]
    public class RootViewModel : RootViewModelBase
    {
        private IClientFactory clientFactory;

        private SecurityPickerClientFactory securityPickerClientFactory;

        [ImportingConstructor]
        public RootViewModel(IClientFactory clientFactory)
        {
            this.clientFactory = clientFactory;
            this.CompositionViewModel = new CompositionViewModel();

            this.securityPickerClientFactory = new SecurityPickerClientFactory(clientFactory);
            
            var securityPickerViewModel = new SecurityPickerViewModel(
                new OnlyErrorCommunicationState(this),
                this.securityPickerClientFactory
            );

            
            this.SecurityPickerViewModel = securityPickerViewModel;
            this.SecurityPickerViewModel.IsEnabled = true;

            this.EditorViewModel = new EditorViewModel();
            
        }

        protected override void Activate()
        {
            
        }

        protected override void Deactivate()
        {
        }

        public EditorViewModel EditorViewModel { get; set; }

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
    }
}
