using System;
using System.Collections.Generic;
using System.Linq;
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
    /// View-Model for Target Price Gadget(Consensus Estimates)
    /// </summary>
    public class ViewModelTargetPrice : NotificationObject
    {
        #region PrivateVariables

        /// <summary>
        /// Instance of IDbInteractivity
        /// </summary>
        private IDBInteractivity dbInteractivity;

        /// <summary>
        /// Instance of ILoggerFacade
        /// </summary>
        private ILoggerFacade logger;

        /// <summary>
        /// Instance of Event Aggregator
        /// </summary>
        private IEventAggregator eventAggregator;

        /// <summary>
        /// IsActive is true when parent control is displayed on UI
        /// </summary>
        private bool isActive;
        public bool IsActive
        {
            get
            {
                return isActive;
            }
            set
            {
                isActive = value;
                if (isActive)
                {
                    if (SelectedSecurity != null)
                    {
                        dbInteractivity.RetrieveTargetPriceData(SelectedSecurity, RetrieveTargetPriceDataCallbackMethod);
                        BusyIndicatorNotification(true, "Updating information based on selected Security");
                    }
                }
                this.RaisePropertyChanged(() => this.IsActive);
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor that initialises the class
        /// </summary>
        /// <param name="param"></param>
        public ViewModelTargetPrice(DashboardGadgetParam param)
        {
            eventAggregator = param.EventAggregator;
            this.dbInteractivity = param.DBInteractivity;
            this.logger = param.LoggerFacade;
            this.SelectedSecurity = param.DashboardGadgetPayload.EntitySelectionData;
            this.eventAggregator = param.EventAggregator;
            if (SelectedSecurity != null)
            {
                dbInteractivity.RetrieveTargetPriceData(SelectedSecurity, RetrieveTargetPriceDataCallbackMethod);
                BusyIndicatorStatus = true;
            }
            if (eventAggregator != null)
            {
                eventAggregator.GetEvent<SecurityReferenceSetEvent>().Subscribe(HandleSecurityReferenceSet);
            }
        }

        #endregion

        #region PropertyDeclaration

        /// <summary>
        /// Selected Security from the ToolBar
        /// </summary>
        private EntitySelectionData selectedSecurity;
        public EntitySelectionData SelectedSecurity
        {
            get
            {
                return selectedSecurity;
            }
            set
            {
                selectedSecurity = value;
                this.RaisePropertyChanged(() => this.SelectedSecurity);
            }
        }

        /// <summary>
        /// Variable of type TargetPriceCEData
        /// </summary>
        private TargetPriceCEData targetPriceData;
        public TargetPriceCEData TargetPriceData
        {
            get
            {
                return targetPriceData;
            }
            set
            {
                targetPriceData = value;
                this.RaisePropertyChanged(() => this.TargetPriceData);
            }
        }


        /// <summary>
        /// Busy Indicator Status
        /// </summary>
        private bool busyIndicatorStatus;
        public bool BusyIndicatorStatus
        {
            get
            {
                return busyIndicatorStatus;
            }
            set
            {
                busyIndicatorStatus = value;
                this.RaisePropertyChanged(() => this.BusyIndicatorStatus);
            }
        }

        #region Busy Indicator
        /// <summary>
        /// Busy Indicator Status
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
        /// Busy Indicator Content
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
        /// Text Bound to CurrentPriceText
        /// </summary>
        private string _currentPriceText = "Current Price";
        public string CurrentPriceText
        {
            get
            {
                return _currentPriceText;
            }
            set
            {
                _currentPriceText = value;
                this.RaisePropertyChanged(() => this.CurrentPriceText);
            }
        }


        #endregion

        #region EventHandlers

        /// <summary>
        /// Event Handler to subscribed event 'SecurityReferenceSet'
        /// </summary>
        /// <param name="securityReferenceData">SecurityReferenceData</param>
        public void HandleSecurityReferenceSet(EntitySelectionData entitySelectionData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                //ArgumentNullException
                if (entitySelectionData != null)
                {
                    SelectedSecurity = entitySelectionData;
                    if (SelectedSecurity != null && IsActive)
                    {
                        dbInteractivity.RetrieveTargetPriceData(SelectedSecurity, RetrieveTargetPriceDataCallbackMethod);
                        BusyIndicatorNotification(true, "Updating information based on selected Security");
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

        }


        #endregion

        #region Callback Methods

        /// <summary>
        /// Callback method for TargetPriceData service
        /// </summary>
        /// <param name="result"></param>
        public void RetrieveTargetPriceDataCallbackMethod(List<TargetPriceCEData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    TargetPriceData = new TargetPriceCEData();
                    CurrentPriceText = "Current Price";
                    if (result.Count != 0)
                    {
                        Logging.LogMethodParameter(logger, methodNamespace, result, 1);
                        TargetPriceData = result.FirstOrDefault();
                        CurrentPriceText = "Current Price (" + Convert.ToDateTime(TargetPriceData.CurrentPriceDate).ToShortDateString() + " )";
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
            finally
            {
                BusyIndicatorNotification();
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }

        #endregion

        #region UnsubscribeEvents

        /// <summary>
        /// UnSubscribe the EventHandlers
        /// </summary>
        public void Dispose()
        {

        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Busy Indicator Notification
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
