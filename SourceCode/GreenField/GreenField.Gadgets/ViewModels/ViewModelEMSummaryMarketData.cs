using System;
using System.Collections.Generic;
using System.Windows;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.ViewModel;
using GreenField.Common;
using GreenField.DataContracts;
using GreenField.DataContracts.DataContracts;
using GreenField.ServiceCaller;

namespace GreenField.Gadgets.ViewModels
{
    /// <summary>
    /// View model class for ViewEMSummaryMarketData
    /// </summary>
    public class ViewModelEMSummaryMarketData : NotificationObject
    {
        #region Fields
        /// <summary>
        /// private member object of the IEventAggregator for event aggregation
        /// </summary>
        private IEventAggregator eventAggregator;

        /// <summary>
        /// private member object of the IDBInteractivity for interaction with the Service Caller
        /// </summary>
        private IDBInteractivity dbInteractivity;

        /// <summary>
        /// private member object of ILoggerFacade for logging
        /// </summary>
        private ILoggerFacade logger;          
        #endregion

        #region Properties
        #region IsActive
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
                if (SelectedPortfolio != null && dbInteractivity != null && value)
                {
                    BusyIndicatorNotification(true, "Retrieving market summary report data...");
                    dbInteractivity.RetrieveEMSummaryMarketData(SelectedPortfolio.PortfolioId, RetrieveEMSummaryDataCallbackMethod);
                }
            }
        } 
        #endregion

        #region Binded
        /// <summary>
        /// private member object of the PortfolioSelectionData class for storing Fund Selection Data
        /// </summary>
        public PortfolioSelectionData SelectedPortfolio { get; set; }

        /// <summary>
        /// Property that stores the data from the web service
        /// </summary>
        private List<EMSummaryMarketData> emSummaryMarketDataInfo;
        public List<EMSummaryMarketData> EmSummaryMarketDataInfo
        {
            get
            {
                if (emSummaryMarketDataInfo == null)
                    emSummaryMarketDataInfo = new List<EMSummaryMarketData>();
                return emSummaryMarketDataInfo;
            }
            set
            {
                if (emSummaryMarketDataInfo != value)
                {
                    emSummaryMarketDataInfo = value;
                    RaisePropertyChanged(() => this.EmSummaryMarketDataInfo);
                }
            }
        } 
        #endregion

        #region Busy Indicator Notification
        /// <summary>
        /// Displays/Hides busy indicator to notify user of the on going process
        /// </summary>
        private bool isBusyIndicatorBusy = false;
        public bool IsBusyIndicatorBusy
        {
            get { return isBusyIndicatorBusy; }
            set
            {
                isBusyIndicatorBusy = value;
                RaisePropertyChanged(() => this.IsBusyIndicatorBusy);
            }
        }

        /// <summary>
        /// Stores the message displayed over the busy indicator to notify user of the on going process
        /// </summary>
        private string busyIndicatorContent;
        public string BusyIndicatorContent
        {
            get { return busyIndicatorContent; }
            set
            {
                busyIndicatorContent = value;
                RaisePropertyChanged(() => this.BusyIndicatorContent);
            }
        }
        #endregion
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor of the class that initializes various objects
        /// </summary>
        /// <param name="param">MEF Eventaggrigator instance</param>
        public ViewModelEMSummaryMarketData(DashboardGadgetParam param)
        {
            this.dbInteractivity = param.DBInteractivity;
            this.eventAggregator = param.EventAggregator;
            this.logger = param.LoggerFacade;
            if (eventAggregator != null)
            {
                eventAggregator.GetEvent<PortfolioReferenceSetEvent>().Subscribe(HandlePortfolioReferenceSet);                
            }
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Event Handler to subscribed event 'PortfolioReferenceSetEvent'
        /// </summary>
        /// <param name="result">PortfolioSelectionData</param>
        public void HandlePortfolioReferenceSet(PortfolioSelectionData result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);

            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result, 1);
                    SelectedPortfolio = result;
                    if (SelectedPortfolio != null && dbInteractivity != null && IsActive)
                    {
                        BusyIndicatorNotification(true, "Retrieving market summary report data...");
                        dbInteractivity.RetrieveEMSummaryMarketData(SelectedPortfolio.PortfolioId, RetrieveEMSummaryDataCallbackMethod);
                    }
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
            Logging.LogEndMethod(logger, methodNamespace);
        }
        #endregion

        #region CallbackMethod
        /// <summary>
        /// RetrieveEMSummaryData callback method
        /// </summary>
        /// <param name="result">List of EMSummaryMarketData class objects</param>
        public void RetrieveEMSummaryDataCallbackMethod(List<EMSummaryMarketData> result)
        {           
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null && result.Count > 0)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result, 1);
                    EmSummaryMarketDataInfo = result;                    
                }
                else
                {
                    EmSummaryMarketDataInfo = new List<EMSummaryMarketData>();                    
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            finally
            {
                Logging.LogEndMethod(logger, methodNamespace);
                BusyIndicatorNotification();
            }
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Display/Hide Busy Indicator
        /// </summary>
        /// <param name="isBusyIndicatorVisible">True to display indicator; default false</param>
        /// <param name="message">Content message for indicator; default null</param>
        private void BusyIndicatorNotification(bool isBusyIndicatorVisible = false, String message = null)
        {
            if (message != null)
            {
                BusyIndicatorContent = message;
            }
            IsBusyIndicatorBusy = isBusyIndicatorVisible;
        }
        
        /// <summary>
        /// method to dispose all subscribed events
        /// </summary>
        public void Dispose()
        {
            eventAggregator.GetEvent<PortfolioReferenceSetEvent>().Unsubscribe(HandlePortfolioReferenceSet);            
        }
        #endregion
    }
}