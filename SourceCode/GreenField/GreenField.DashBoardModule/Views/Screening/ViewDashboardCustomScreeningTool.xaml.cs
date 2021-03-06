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
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Logging;
using GreenField.ServiceCaller;
using GreenField.Common;
using GreenField.Common.Helper;
using GreenField.Gadgets.Views;
using GreenField.Gadgets.ViewModels;
using Microsoft.Practices.Prism.Regions;
using GreenField.Gadgets.Helpers;

namespace GreenField.DashBoardModule.Views.Screening
{
    [Export]
    public partial class ViewDashboardCustomScreeningTool : UserControl, INavigationAware
    {
        #region Fields
        private IEventAggregator _eventAggregator;
        private ILoggerFacade _logger;
        private IDBInteractivity _dBInteractivity;
        private IRegionManager _regionManager;
        public DashboardGadgetParam tempParam;
        #endregion

        [ImportingConstructor]
        public ViewDashboardCustomScreeningTool(ILoggerFacade logger, IEventAggregator eventAggregator,
            IDBInteractivity dbInteractivity,IRegionManager regionManager)
        {
            InitializeComponent();
            _eventAggregator = eventAggregator;
            _logger = logger;
            _dBInteractivity = dbInteractivity;
            _regionManager = regionManager;
            _eventAggregator.GetEvent<DashboardGadgetLoad>().Subscribe(HandleDashboardGadgetLoad);

            this.tbHeader.Text = GadgetNames.CUSTOM_SCREENING_TOOL;

        }

        public void HandleDashboardGadgetLoad(DashboardGadgetPayload payload)
        {
            if (this.cctrDashboardContent.Content != null)
                return;

            DashboardGadgetParam param = new DashboardGadgetParam()
            {
                DashboardGadgetPayload = payload,
                DBInteractivity = _dBInteractivity,
                EventAggregator = _eventAggregator,
                LoggerFacade = _logger,
                RegionManager = _regionManager
            };
            tempParam = param;
            RefreshScreen.refreshFlag = false;
            this.cctrDashboardContent.Content = new ViewCustomScreeningTool(new ViewModelCustomScreeningTool(param));
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            ViewBaseUserControl control = (ViewBaseUserControl)cctrDashboardContent.Content;
            if (control != null)
            {
                control.IsActive = false;
            }
            RefreshScreen.refreshFlag = false;
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            if (RefreshScreen.refreshFlag)
            {
                this.cctrDashboardContent.Content = new ViewCustomScreeningTool(new ViewModelCustomScreeningTool(tempParam));
            }
            ViewBaseUserControl control = (ViewBaseUserControl)cctrDashboardContent.Content;
            if (control != null)
            {
                control.IsActive = true;
            }
        }
    }
}
