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

namespace GreenField.Gadgets.ViewModels.Targeting.BroadGlobalActive
{
    public class EditorViewModel : NotificationObject
    {
        private IDBInteractivity repository;
        private IEventAggregator eventAggregator;
        private ILoggerFacade logger;
        private List<FXCommodityData> commodityData;
        private Visibility commodityGridVisibility = Visibility.Collapsed;
        private Boolean isBusyIndicatorStatus;

        public EditorViewModel(DashboardGadgetParam param)
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

        public void Initialize(Int32 targetingTypeId, String broadGlobalActivePortfolioId, DateTime benchmarkDate)
        {
            
            this.repository.GetBgaModel(targetingTypeId, broadGlobalActivePortfolioId, benchmarkDate, this.TakeData);
            this.IsBusyIndicatorStatus = true;
        }

        protected void TakeData(BgaRootModel data)
        {
            if (data == null) throw new ApplicationException("No data has arrived.");

            this.TakeDataUnsafe(data);
            this.IsBusyIndicatorStatus = false;
        }

        protected void TakeDataUnsafe(BgaRootModel data)
        {
            this.Data = data;
        }

        public void Dispose()
        {
        }

        public BgaRootModel Data { get; set; }


    }
}
