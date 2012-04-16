﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel.Composition;
using GreenField.Gadgets.Views;
using GreenField.Gadgets.ViewModels;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Logging;
using GreenField.ServiceCaller;
using GreenField.Common;
using GreenField.Common.Helper;

namespace GreenField.DashboardModule.Views
{
    [Export]
    public partial class ViewDashboardCompanyChartingUnrealizedGainLoss : UserControl
    {
        #region Fields
        private IEventAggregator _eventAggregator;
        private ILoggerFacade _logger;
        private IDBInteractivity _dBInteractivity;
        #endregion

        [ImportingConstructor]
        public ViewDashboardCompanyChartingUnrealizedGainLoss(ILoggerFacade logger, IEventAggregator eventAggregator,
            IDBInteractivity dbInteractivity)
        {
            InitializeComponent();
            _eventAggregator = eventAggregator;
            _logger = logger;
            _dBInteractivity = dbInteractivity;

            _eventAggregator.GetEvent<DashboardGadgetLoad>().Subscribe(HandleDashboardGadgetLoad);

            this.tbHeader.Text = GadgetNames.SECURITY_REFERENCE_UNREALIZED_GAIN_LOSS;
        }

        public void HandleDashboardGadgetLoad(DashboardGadgetPayload payload)
        {
            DashboardGadgetParam param = new DashboardGadgetParam()
            {
                DashboardGadgetPayload = payload,
                DBInteractivity = _dBInteractivity,
                EventAggregator = _eventAggregator,
                LoggerFacade = _logger
            };

            this.cctrDashboardContent.Content = new ViewUnrealizedGainLoss(new ViewModelUnrealizedGainLoss(param));
        }
    }
}
