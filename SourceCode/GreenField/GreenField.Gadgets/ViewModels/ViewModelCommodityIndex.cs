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


namespace GreenField.Gadgets.ViewModels
{
    public class ViewModelCommodityIndex : NotificationObject
    {
        #region Fields

        /// <summary>
        /// private member object of the IDBInteractivity for interaction with the Service Caller
        /// </summary>
        private IDBInteractivity dbInteractivity;

        /// <summary>
        /// private member object of the IEventAggregator for event aggregation
        /// </summary>
        private IEventAggregator eventAggregator;
        /// <summary>
        /// private member object of ILoggerFacade for logging
        /// </summary>
        private ILoggerFacade logger;
        /// <summary>
        /// Private member object of FXCommodityData for Commodity
        /// </summary>
        private List<FXCommodityData> commodityData;
        /// <summary>
        /// Private member stores selected commodity ID
        /// </summary>
        private string commodityID;
        /// <summary>
        /// Private member stores commodity grid visibility
        /// </summary>
        private Visibility commodityGridVisibility = Visibility.Collapsed;      
         /// <summary>
         /// Private memebr to hold BusyIndicator's bool value (either true or false)
         /// </summary>
        private bool isBusyIndicatorStatus;

        #endregion
        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="param">DashBoardGadgetParam</param>
        public ViewModelCommodityIndex(DashboardGadgetParam param)
        {
            eventAggregator = param.EventAggregator;
            dbInteractivity = param.DBInteractivity;
            logger = param.LoggerFacade;
            commodityID = param.DashboardGadgetPayload.CommoditySelectedVal;
            if (eventAggregator != null)
            {
                eventAggregator.GetEvent<CommoditySelectionSetEvent>().Subscribe(HandleCommodityReferenceSet);
            }
            if (commodityID != null && IsActive)
            {
                dbInteractivity.RetrieveCommoditySelectionData(RetrieveCommodityDataCallbackMethod);
            }
                CallingWebMethod();
        }
        #endregion

        #region Properties
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
                RaisePropertyChanged(() => this.CommodityData);
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
                RaisePropertyChanged(() => this.CommodityGridVisibility);
            }
        }

        /// <summary>
        /// sets BusyIndicator visibilty
        /// </summary>
        public bool IsBusyIndicatorStatus
        {
            get { return isBusyIndicatorStatus; }
            set
            {
                isBusyIndicatorStatus = value;
                RaisePropertyChanged(() => this.IsBusyIndicatorStatus);
            }
        }

        /// <summary>
        /// IsActive is true when parent control is displayed on UI
        /// </summary>
        private bool isActive;
        public bool IsActive
        {
            get { return isActive; }
            set
            {
                isActive = value;
                CallingWebMethod();
            }
        }
        #endregion

        #region Event
        
        /// <summary>
        /// event to handle data
        /// </summary>
        public event RetrieveCommodityDataCompleteEventHandler RetrieveCommodityDataCompleteEvent;
        #endregion

        #region EventHandler
        public void HandleCommodityReferenceSet(string commodityId)
        {

            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (commodityID != null && IsActive)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, commodityID, 1);
                    commodityID = commodityId;
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
            Logging.LogEndMethod(logger, methodNamespace);
        }
        #endregion
       
        #region Callback Method
        /// <summary>
        /// Callback method that assigns value to the Commodity property
        /// </summary>
        /// <param name="result">contains the Commodity data </param>
        public void RetrieveCommodityDataCallbackMethod(List<FXCommodityData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                CommodityGridVisibility = Visibility.Collapsed;
                if (result != null && result.Count > 0)
                {

                    Logging.LogMethodParameter(logger, methodNamespace, result, 1);
                    CommodityGridVisibility = Visibility.Visible;
                    CommodityData = result;
                    RetrieveCommodityDataCompleteEvent(new RetrieveCommodityDataCompleteEventArgs() { CommodityInfo = result });

                }
                else
                {
                    Logging.LogMethodParameterNull(logger, methodNamespace, 1);
                }

            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            finally { IsBusyIndicatorStatus = false; }
            Logging.LogEndMethod(logger, methodNamespace);
        }
        #endregion

        #region WEB METHOD CALL
        private void CallingWebMethod()
        {
            if (IsActive)
            {
                dbInteractivity.RetrieveCommodityData(commodityID, RetrieveCommodityDataCallbackMethod);
                IsBusyIndicatorStatus = true;
            }
        }
        #endregion

        #region EventUnSubscribe
        /// <summary>
        /// Method that disposes the events
        /// </summary>
        public void Dispose()
        {
            eventAggregator.GetEvent<CommoditySelectionSetEvent>().Unsubscribe((HandleCommodityReferenceSet));           
        }
        #endregion
    }
}
