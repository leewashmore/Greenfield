﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.Regions;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using GreenField.IssuerShares.Controls;

namespace GreenField.IssuerShares.App
{
    [Export]
    public class ShellViewModel
    {
        private IRegionManager regionManager;
        private IEventAggregator aggregator;

        [ImportingConstructor]
        public ShellViewModel(IRegionManager regionManager, IEventAggregator aggregator)
        {
            this.regionManager = regionManager;
            this.RunCommand = new DelegateCommand(this.Run);
            this.aggregator = aggregator;
        }

        public ICommand RunCommand { get; private set; }
        public void Run()
        {
            this.regionManager.RequestNavigate("MainRegion", typeof(GreenField.IssuerShares.Controls.RootView).FullName);
            this.aggregator.GetEvent<SecurityPickedGlobalEvent>().Publish(new SecurityPickedGlobalEventInfo { SecurityShortName = "CNANCONCHE" }); //TWTPKHCE, RUSBERBPN, TWHTCCORPE
        }

    }
}
