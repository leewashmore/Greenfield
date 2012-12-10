using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.Regions;

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
        }
    }
}
