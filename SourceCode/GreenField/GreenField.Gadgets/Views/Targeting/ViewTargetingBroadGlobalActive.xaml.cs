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
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Logging;
using GreenField.Common;
using GreenField.ServiceCaller;
using GreenField.ServiceCaller.ModelFXDefinitions;
using System.Collections.Generic;
using GreenField.Common.Helper;
using GreenField.DataContracts;
using GreenField.ServiceCaller.TargetingDefinitions;


namespace GreenField.Gadgets.ViewModels
{
    public class ViewTargetingBroadGlobalActive : NotificationObject
    {
        private IDBInteractivity repository;
        private IEventAggregator eventAggregator;
        private ILoggerFacade logger;
        private List<FXCommodityData> commodityData;
        private Visibility commodityGridVisibility = Visibility.Collapsed;
        private Boolean isBusyIndicatorStatus;

        public ViewTargetingBroadGlobalActive(DashboardGadgetParam param)
        {
            this.eventAggregator = param.EventAggregator;
            this.repository = param.DBInteractivity;
            this.logger = param.LoggerFacade;
            this.eventAggregator.GetEvent<CommoditySelectionSetEvent>().Subscribe(ListenToTargetingTypeSet);
        }

        /// <summary>
        /// Stores Commodity Data
        /// </summary>
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
        /// <summary>
        /// Stores visibility of grid
        /// </summary>
        public Visibility CommodityGridVisibility
        {
            get { return commodityGridVisibility; }
            set
            {
                commodityGridVisibility = value;
                this.RaisePropertyChanged(() => this.CommodityGridVisibility);
            }
        }

        /// <summary>
        /// sets BusyIndicator visibilty
        /// </summary>
        public Boolean IsBusyIndicatorStatus
        {
            get { return this.isBusyIndicatorStatus; }
            set
            {
                this.isBusyIndicatorStatus = value;
                this.RaisePropertyChanged(() => this.IsBusyIndicatorStatus);
            }
        }

        /// <summary>
        /// IsActive is true when parent control is displayed on UI
        /// </summary>
        private Boolean isActive;
        public Boolean IsActive
        {
            get { return isActive; }
            set
            {
                isActive = value;
                CallingWebMethod();
            }
        }

        public void ListenToTargetingTypeSet(String commodityId)
        {
            String methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                commodityID = commodityId;
                if (commodityID != null && IsActive)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, commodityID, 1);
                    CallingWebMethod();
                }
                else
                {
                    Logging.LogMethodParameterNull(logger, methodNamespace, 1);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            finally
            {
                Logging.LogEndMethod(logger, methodNamespace);
            }
        }

        public void RequestData(Int32 targetingTypeId, String broadGlobalActivePortfolioId)
        {
            if (!this.IsActive) return;
            this.repository.GetBgaModel(targetingTypeId, broadGlobalActivePortfolioId, this.TakeData);
            this.IsBusyIndicatorStatus = true;
        }


        protected void TakeData(RootModel data)
        {
            if (data == null) throw new ApplicationException("No data has arrived.");

            this.TakeDataUnsafe(data);
            this.RetrieveCommodityDataCompleteEvent(new RetrieveCommodityDataCompleteEventArgs { CommodityInfo = data });

            this.IsBusyIndicatorStatus = false;
        }

        protected void TakeDataUnsafe(RootModel data)
        {
            this.CommodityGridVisibility = Visibility.Visible;
            this.CommodityData = data;
            this.CommodityGridVisibility = Visibility.Collapsed;
        }

        public void Dispose()
        {
            this.eventAggregator.GetEvent<CommoditySelectionSetEvent>().Unsubscribe(this.ListenToTargetingTypeSet);
        }
    }
}
