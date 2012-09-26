using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.ViewModel;
using GreenField.Common;
using GreenField.DataContracts;
using GreenField.Gadgets.Helpers;
using GreenField.ServiceCaller;

namespace GreenField.Gadgets.ViewModels
{
    public class ViewModelFairValueCompositionSummary: NotificationObject
    {
        #region PRIVATE FIELDS
        //MEF Singletons

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
        public ILoggerFacade logger;
            

        /// <summary>
        /// Private member to store basic data gadget visibilty
        /// </summary>
        private Visibility freeCashFlowGadgetVisibility = Visibility.Collapsed;
        /// <summary>
        /// Private member to store Selected Security ID
        /// </summary>
        private EntitySelectionData securitySelectionData = null;

        #endregion
        
        #region PROPERTIES       
        
        /// <summary>
        /// Stores fcf arranged data
        /// </summary>
        private List<FairValueCompositionSummaryData> fairValueCompositionSummaryData = null;
        public List<FairValueCompositionSummaryData> FairValueCompositionSummaryData
        {
            get
            {
                return fairValueCompositionSummaryData;
            }
            set
            {
                fairValueCompositionSummaryData = value;
                RaisePropertyChanged(() => this.FairValueCompositionSummaryData);
            }
        }
       
        public Visibility FreeCashFlowGadgetVisibility
        {
            get { return freeCashFlowGadgetVisibility; }
            set
            {
                freeCashFlowGadgetVisibility = value;
                RaisePropertyChanged(() => this.FreeCashFlowGadgetVisibility);
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
                if (securitySelectionData != null && IsActive)
                {
                    if (securitySelectionData.InstrumentID != null && securitySelectionData.InstrumentID != string.Empty)
                    {
                        CallingWebMethod();
                    }
                }
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
        #endregion

        #region CONSTRUCTOR
        public ViewModelFairValueCompositionSummary(DashboardGadgetParam param)
        {
            eventAggregator = param.EventAggregator;
            dbInteractivity = param.DBInteractivity;
            logger = param.LoggerFacade;
            securitySelectionData = param.DashboardGadgetPayload.EntitySelectionData;

            if (eventAggregator != null)
            {
                eventAggregator.GetEvent<SecurityReferenceSetEvent>().Subscribe(HandleSecurityReferenceSet);
            }
            if (securitySelectionData != null && IsActive)
            {
                if (securitySelectionData.InstrumentID != null && securitySelectionData.InstrumentID != string.Empty)
                {
                    CallingWebMethod();
                }
            }
            
        }

        #endregion       

        #region EVENTHANDLERS
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
                if (entitySelectionData != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, entitySelectionData, 1);
                    securitySelectionData = entitySelectionData;

                    if (securitySelectionData.InstrumentID != null && securitySelectionData.InstrumentID != string.Empty)
                    {
                        CallingWebMethod();
                        
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

        #region CALLBACK METHOD
        /// <summary>
        /// Callback method that assigns value to the BAsicDataInfo property
        /// </summary>
        /// <param name="result">basic data </param>
        private void RetrieveFairValueCompositionSummaryDataCallbackMethod(List<FairValueCompositionSummaryData> fairValueCompositionSummaryData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                FairValueCompositionSummaryData = null;
                FreeCashFlowGadgetVisibility = Visibility.Collapsed;
                if (fairValueCompositionSummaryData != null && fairValueCompositionSummaryData.Count > 0)
                {
                    FreeCashFlowGadgetVisibility = Visibility.Visible;
                    Logging.LogMethodParameter(logger, methodNamespace, fairValueCompositionSummaryData, 1);
                    FairValueCompositionSummaryData = fairValueCompositionSummaryData;  
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
            finally { BusyIndicatorStatus = false; }
            Logging.LogEndMethod(logger, methodNamespace);
        }

        #endregion  

        #region SERVICE CALL METOHD
        /// <summary>
        /// Calls web service method
        /// </summary>
        private void CallingWebMethod()
        {
            if (securitySelectionData != null && IsActive)
            {
                dbInteractivity.RetrieveFairValueCompostionSummaryData(securitySelectionData, RetrieveFairValueCompositionSummaryDataCallbackMethod);
                BusyIndicatorStatus = true;
            }            

        }
        #endregion  


        #region EventUnSubscribe

        public void Dispose()
        {
            eventAggregator.GetEvent<SecurityReferenceSetEvent>().Unsubscribe(HandleSecurityReferenceSet);
        }
        #endregion
    }
}
