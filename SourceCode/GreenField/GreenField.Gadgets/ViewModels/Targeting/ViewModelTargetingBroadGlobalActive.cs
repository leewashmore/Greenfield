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
using GreenField.Common;
using GreenField.ServiceCaller;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Logging;
using GreenField.DataContracts;
using System.Collections.Generic;
using Microsoft.Practices.Prism.ViewModel;
using GreenField.ServiceCaller.TargetingDefinitions;

namespace GreenField.Gadgets.ViewModels
{
    public class ViewModelTargetingBroadGlobalActive : NotificationObject
    {
        private IDBInteractivity repository;
        private IEventAggregator eventAggregator;
        private ILoggerFacade logger;
        private List<FXCommodityData> commodityData;
        private Visibility commodityGridVisibility = Visibility.Collapsed;
        private Boolean isBusyIndicatorStatus;

        public ViewModelTargetingBroadGlobalActive(DashboardGadgetParam param)
        {
            this.eventAggregator = param.EventAggregator;
            this.repository = param.DBInteractivity;
            this.logger = param.LoggerFacade;
        }


        public List<FXCommodityData> CommodityData
        {
            get
            {
                return commodityData;
            }
            set
            {
                commodityData = value;
                this.RaisePropertyChanged(() => this.CommodityData);
            }
        }

        public Visibility CommodityGridVisibility
        {
            get { return commodityGridVisibility; }
            set
            {
                commodityGridVisibility = value;
                this.RaisePropertyChanged(() => this.CommodityGridVisibility);
            }
        }

        public Boolean IsBusyIndicatorStatus
        {
            get { return this.isBusyIndicatorStatus; }
            set
            {
                this.isBusyIndicatorStatus = value;
                this.RaisePropertyChanged(() => this.IsBusyIndicatorStatus);
            }
        }


        private Boolean isActive;
        public Boolean IsActive
        {
            get { return isActive; }
            set
            {
                isActive = value;
            }
        }

        public void RequestData(Int32 targetingTypeId, String broadGlobalActivePortfolioId, DateTime benchmarkDate)
        {
            if (!this.IsActive) return;
            this.repository.GetBgaModel(targetingTypeId, broadGlobalActivePortfolioId, benchmarkDate, this.TakeData);
            this.IsBusyIndicatorStatus = true;
        }


        protected void TakeData(RootModel data)
        {
            if (data == null) throw new ApplicationException("No data has arrived.");

            this.TakeDataUnsafe(data);
            this.IsBusyIndicatorStatus = false;
        }

        protected void TakeDataUnsafe(RootModel data)
        {
        }

        public void Dispose()
        {
        }
    }
}
