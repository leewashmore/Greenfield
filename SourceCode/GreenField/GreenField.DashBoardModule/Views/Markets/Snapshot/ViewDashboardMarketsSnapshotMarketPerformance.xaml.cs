using System;
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
using Microsoft.Practices.Prism.Regions;
using GreenField.Gadgets.Helpers;

namespace GreenField.DashboardModule.Views
{
    [Export]
    public partial class ViewDashboardMarketsSnapshotMarketPerformance : UserControl, INavigationAware//, IConfirmNavigationRequest
    {
        #region Fields
        private IEventAggregator _eventAggregator;
        private ILoggerFacade _logger;
        private IDBInteractivity _dBInteractivity;
        #endregion

        [ImportingConstructor]
        public ViewDashboardMarketsSnapshotMarketPerformance(ILoggerFacade logger, IEventAggregator eventAggregator,
            IDBInteractivity dbInteractivity)
        {
            InitializeComponent();

            _eventAggregator = eventAggregator;
            _logger = logger;
            _dBInteractivity = dbInteractivity;

            _eventAggregator.GetEvent<DashboardGadgetLoad>().Subscribe(HandleDashboardGadgetLoad);
            

            //this.tbHeader.Text = GadgetNames.BENCHMARKS_MARKET_PERFORMANCE_SNAPSHOT;
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
                LoggerFacade = _logger
            };

            this.cctrDashboardContent.Content = new ViewMarketPerformanceSnapshot(new ViewModelMarketPerformanceSnapshot(param));
        }

        #region INavigationAware Methods
        public bool IsNavigationTarget(NavigationContext navigationContext)
        {            
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            _eventAggregator.GetEvent<MarketPerformanceSnapshotActionCompletionEvent>().Publish(new MarketPerformanceSnapshotActionPayload() { ActionType = MarketPerformanceSnapshotActionType.SNAPSHOT_PAGE_NAVIGATION });
            ViewBaseUserControl control = (ViewBaseUserControl)cctrDashboardContent.Content;
            control.IsActive = false;
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            _eventAggregator.GetEvent<MarketPerformanceSnapshotActionCompletionEvent>().Publish(new MarketPerformanceSnapshotActionPayload() { ActionType = MarketPerformanceSnapshotActionType.SNAPSHOT_PAGE_NAVIGATION });
            ViewBaseUserControl control = (ViewBaseUserControl)cctrDashboardContent.Content;
            control.IsActive = true;
        }
        #endregion

        #region IConfirmNavigationRequest Method
        public void ConfirmNavigationRequest(NavigationContext navigationContext, Action<bool> continuationCallback)
        {
            Prompt.ShowDialog("On navigation any unsaved changes might be lost. Are you sure you want to navigate", "", MessageBoxButton.OKCancel, (messageResult) =>
            {
                if (messageResult == MessageBoxResult.OK)
                {
                    continuationCallback(true);
                    return;
                }
                continuationCallback(false);
            });
        } 
        #endregion
    }
}
