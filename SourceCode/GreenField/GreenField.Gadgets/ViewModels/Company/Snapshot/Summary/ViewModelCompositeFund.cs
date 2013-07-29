using System;
using System.Collections.Generic;
using System.Windows;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.ViewModel;
using GreenField.Common;
using GreenField.DataContracts;
using GreenField.ServiceCaller;

namespace GreenField.Gadgets.ViewModels
{
    /// <summary>
    /// View model for ViewModelCompositeFund class
    /// </summary>
    public class ViewModelCompositeFund : NotificationObject
    {
        #region Fields
        /// <summary>
        /// MEF Singletons
        /// </summary>
        private IEventAggregator eventAggregator;
        private IDBInteractivity dbInteractivity;
        private ILoggerFacade logger;
        private EntitySelectionData entitySelectionData;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public ViewModelCompositeFund(DashboardGadgetParam param)
        {
            logger = param.LoggerFacade;
            dbInteractivity = param.DBInteractivity;
            eventAggregator = param.EventAggregator;
            PortfolioSelectionData = param.DashboardGadgetPayload.PortfolioSelectionData;
            entitySelectionData = param.DashboardGadgetPayload.EntitySelectionData;

            if (eventAggregator != null)
            {
                eventAggregator.GetEvent<PortfolioReferenceSetEvent>().Subscribe(HandlePortfolioReferenceSet);
                eventAggregator.GetEvent<SecurityReferenceSetEvent>().Subscribe(HandleSecurityReferenceSetEvent);
            }
        } 
        #endregion

        #region Properties
        /// <summary>
        /// IsActive is true when parent control is displayed on UI
        /// </summary>
        private bool isActive;
        public bool IsActive
        {
            get{ return isActive; }
            set
            {
                if (isActive != value)
                {
                    isActive = value;
                    if (entitySelectionData != null && PortfolioSelectionData != null && IsActive)
                    {
                        BusyIndicatorNotification(true, "Retrieving Data based on selected security and portfolio");
                        dbInteractivity.RetrieveCompositeFundData(entitySelectionData, PortfolioSelectionData, RetrieveCompositeFundDataCallBackMethod);
                    }
                }
            }
        }

        /// <summary>
        /// DashboardGadgetPayLoad field
        /// </summary>
        private PortfolioSelectionData portfolioSelectionInfo;
        public PortfolioSelectionData PortfolioSelectionData
        {
            get { return portfolioSelectionInfo; }
            set
            {
                if (portfolioSelectionInfo != value)
                {
                    portfolioSelectionInfo = value;
                    RaisePropertyChanged(() => PortfolioSelectionData);
                }
            }
        }

        #region Busy Indicator
        /// <summary>
        /// if busy indicator is busy or not
        /// </summary>
        private bool busyIndicatorIsBusy;
        public bool BusyIndicatorIsBusy
        {
            get { return busyIndicatorIsBusy; }
            set
            {
                busyIndicatorIsBusy = value;
                RaisePropertyChanged(() => this.BusyIndicatorIsBusy);
            }
        }

        /// <summary>
        /// content to show below busy indicator
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

        /// <summary>
        /// contain result set obtained from web service
        /// </summary>
        private List<CompositeFundData> compositeFundInfo;
        public List<CompositeFundData> CompositeFundInfo
        {
            get { return compositeFundInfo; }
            set
            {
                if (compositeFundInfo != value)
                {
                    compositeFundInfo = value;
                    RaisePropertyChanged(() => CompositeFundInfo);
                }
            }
        }

        /// <summary>
        /// create display data
        /// </summary>
        private CompositeFundData compositeDisplayData;
        public CompositeFundData CompositeDisplayData
        {
            get { return compositeDisplayData; }
            set 
            {
                if (compositeDisplayData != value)
                {
                    compositeDisplayData = value;
                    RaisePropertyChanged(() => CompositeDisplayData);
                    BusyIndicatorNotification();
                }
            }
        }

        /// <summary>
        /// contains value for checkbox
        /// </summary>
        private bool displayIssuerIsChecked = false;
        public bool DisplayIssuerIsChecked
        {
            get { return displayIssuerIsChecked; }
            set 
            {
                if (displayIssuerIsChecked != value)
                {
                    displayIssuerIsChecked = value;
                    RaisePropertyChanged(() => DisplayIssuerIsChecked);
                    if (CompositeFundInfo != null && CompositeFundInfo.Count > 0)
                    {
                        BusyIndicatorNotification(true, "Retrieving Data based on Issuer View");
                        CreateDisplayData(CompositeFundInfo, value);
                    }
                }
            }
        }      
        #endregion

        #region Event Handlers
        /// <summary>
        /// Event Handler to subscribed event 'PortfolioReferenceSetEvent'
        /// </summary>
        /// <param name="portfolioSelectionData">PortfolioSelectionData</param>
        public void HandlePortfolioReferenceSet(PortfolioSelectionData portfolioSelectionData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (portfolioSelectionData != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, portfolioSelectionData, 1);
                    PortfolioSelectionData = portfolioSelectionData;
                    if (entitySelectionData != null && PortfolioSelectionData != null && IsActive)
                    {
                        BusyIndicatorNotification(true, "Retrieving Data based on selected security and portfolio");
                        dbInteractivity.RetrieveCompositeFundData(entitySelectionData, PortfolioSelectionData, RetrieveCompositeFundDataCallBackMethod);
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

        /// <summary>
        /// Assigns UI Field Properties based on Entity Selection Data
        /// </summary>
        /// <param name="entitySelectionData">EntitySelectionData</param>
        public void HandleSecurityReferenceSetEvent(EntitySelectionData result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result, 1);
                    entitySelectionData = result;

                    if (entitySelectionData != null && PortfolioSelectionData != null && IsActive)
                    {
                        BusyIndicatorNotification(true, "Retrieving Data based on selected security and portfolio");
                        dbInteractivity.RetrieveCompositeFundData(entitySelectionData, PortfolioSelectionData, RetrieveCompositeFundDataCallBackMethod);
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

        #region CallBack Methods
        /// <summary>
        /// CallBack method for service method call
        /// </summary>
        /// <param name="result"></param>
        public void RetrieveCompositeFundDataCallBackMethod(List<CompositeFundData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    CompositeFundInfo = result;
                    CreateDisplayData(result, DisplayIssuerIsChecked);
                }
                else
                {
                    Logging.LogMethodParameterNull(logger, methodNamespace, 1);
                }
                BusyIndicatorNotification();
            }
            catch (Exception ex)
            {
                BusyIndicatorNotification();
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }

            Logging.LogEndMethod(logger, methodNamespace);
        }
        #endregion

        #region Helper Method
        /// <summary>
        /// show data according to issuer view checkbox (used by Holdings and Positioning gadget)
        /// </summary>
        /// <param name="record"></param>
        /// <param name="issuerViewChecked"></param>
        public void CreateDisplayData(List<CompositeFundData> record,bool issuerViewChecked)
        {
            if (record != null && record.Count > 0)
            {
                if (issuerViewChecked)
                {
                    CompositeDisplayData = record[1];
                }
                else
                {
                    CompositeDisplayData = record[0];
                }
            }
        }

        /// <summary>
        /// method to dispose all subscribed events
        /// </summary>
        public void Dispose()
        {
            eventAggregator.GetEvent<PortfolioReferenceSetEvent>().Unsubscribe(HandlePortfolioReferenceSet);
            eventAggregator.GetEvent<SecurityReferenceSetEvent>().Unsubscribe(HandleSecurityReferenceSetEvent);
        }

        /// <summary>
        /// set busy indicator status
        /// </summary>
        /// <param name="showBusyIndicator"></param>
        /// <param name="message"></param>
        public void BusyIndicatorNotification(bool showBusyIndicator = false, String message = null)
        {
            if (message != null)
                BusyIndicatorContent = message;
            BusyIndicatorIsBusy = showBusyIndicator;
        }
        #endregion
    }
}
