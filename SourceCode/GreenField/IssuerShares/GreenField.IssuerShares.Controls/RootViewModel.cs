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

namespace GreenField.IssuerShares.Controls
{
    [Export]
    public class RootViewModel : RootViewModelBase
    {
        private IClientFactory clientFactory;

        [ImportingConstructor]
        public RootViewModel(IClientFactory clientFactory)
        {
            this.clientFactory = clientFactory;
            this.CompositionViewModel = new CompositionViewModel();
        }

        protected override void Activate()
        {
        }

        protected override void Deactivate()
        {
            throw new NotImplementedException();
        }

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
