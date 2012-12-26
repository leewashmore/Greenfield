using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.Regions;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;

namespace GreenField.IssuerShares.App
{
    [Export]
    public class ShellViewModel
    {
        private IRegionManager regionManager;

        [ImportingConstructor]
        public ShellViewModel(IRegionManager regionManager)
        {
            this.regionManager = regionManager;
            this.RunCommand = new DelegateCommand(this.Run);
        }

        public ICommand RunCommand { get; private set; }
        public void Run()
        {
            this.regionManager.RequestNavigate("MainRegion", typeof(GreenField.IssuerShares.Controls.RootView).FullName);
        }

    }
}
