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
using System.Collections.Generic;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Logging;
using GreenField.Common;
using GreenField.ServiceCaller.ExternalResearchDefinitions;
using GreenField.DataContracts;
using GreenField.DataContracts.DataContracts;
using GreenField.ServiceCaller;

namespace GreenField.Gadgets.ViewModels
{
    #region View Model for this gadget
    public class ViewModelConsensusEstimateSummary : NotificationObject
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
        /// private member object of the EntitySelectionData class for storing Entity Selection Data
        /// </summary>
        private EntitySelectionData entitySelectionData;    
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="param">DashBoardGadgetParam</param>
        public ViewModelConsensusEstimateSummary(DashboardGadgetParam param)
        {
            eventAggregator = param.EventAggregator;
            dbInteractivity = param.DBInteractivity;
            logger = param.LoggerFacade;
            entitySelectionData = param.DashboardGadgetPayload.EntitySelectionData;
            if (entitySelectionData !=null && IsActive)
            {
                dbInteractivity.RetrieveConsensusEstimatesSummaryData(entitySelectionData, 
                    RetrieveConsensusEstimatesSummaryDataDataCallbackMethod);
            }
            if (eventAggregator != null)
            {
                eventAggregator.GetEvent<SecurityReferenceSetEvent>().Subscribe(HandleSecurityReferenceSet,false);               
            } 
        }
        #endregion

        #region Properties

        /// <summary>
        /// Property binded to the grid 
        /// </summary>
        private List<ConsensusEstimatesSummaryData> consensusSummaryInfo;        
        public List<ConsensusEstimatesSummaryData> ConsensusSummaryInfo
        {
            get 
            {
                return consensusSummaryInfo;
            }

            set 
            {
                consensusSummaryInfo = value;
                RaisePropertyChanged(() => this.ConsensusSummaryInfo);
            }
        }

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
                if (isActive != value)
                {
                    isActive = value;
                    if (entitySelectionData != null && isActive)
                    {
                        if (null != ConsensusEstimatesSummaryDataLoadedEvent)
                        {
                            ConsensusEstimatesSummaryDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
                        }
                        dbInteractivity.RetrieveConsensusEstimatesSummaryData(entitySelectionData, 
                            RetrieveConsensusEstimatesSummaryDataDataCallbackMethod);                       
                    }
                }
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Event for the notification of Data Load Completion for Busy Indicator
        /// </summary>
        public event DataRetrievalProgressIndicatorEventHandler ConsensusEstimatesSummaryDataLoadedEvent;

        /// <summary>
        /// Event for the notification of Data Load Completion
        /// </summary>
        public event RetrieveConsensusEstimatesSummaryCompleteEventHandler RetrieveConsensusEstimatesSummaryDataCompletedEvent;
        #endregion

        #region Event Handlers
        /// <summary>
        /// Assigns UI Field Properties based on Security reference
        /// </summary>
        /// <param name="securityReferenceData">entitySelectionData</param>
        public void HandleSecurityReferenceSet(EntitySelectionData entSelectionData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (entSelectionData != null && IsActive)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, entSelectionData, 1);
                    entitySelectionData = entSelectionData;
                    if (null != ConsensusEstimatesSummaryDataLoadedEvent)
                    {
                        ConsensusEstimatesSummaryDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
                    }
                    dbInteractivity.RetrieveConsensusEstimatesSummaryData(entitySelectionData, 
                        RetrieveConsensusEstimatesSummaryDataDataCallbackMethod);
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

        #region Callback Methods
        /// <summary>
        /// Callback method that assigns value to the ConsensusSummaryInfo property
        /// </summary>
        /// <param name="result">contains the ConsensusSummary data for the grid</param>
        public void RetrieveConsensusEstimatesSummaryDataDataCallbackMethod(List<ConsensusEstimatesSummaryData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null && result.Count > 0)
                {
                    ConsensusSummaryInfo = result;
                    if (null != ConsensusEstimatesSummaryDataLoadedEvent)
                    {
                        ConsensusEstimatesSummaryDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
                    }
                    RetrieveConsensusEstimatesSummaryDataCompletedEvent(new RetrieveConsensusSummaryCompletedEventsArgs 
                    { ConsensusInfo  = result});                   
                }
                else
                {
                    ConsensusSummaryInfo = result;                  
                    Logging.LogMethodParameterNull(logger, methodNamespace, 1);
                    ConsensusEstimatesSummaryDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
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

        #region EventUnSubscribe
        /// <summary>
        /// Method that disposes the events
        /// </summary>
        public void Dispose()
        {
            eventAggregator.GetEvent<SecurityReferenceSetEvent>().Unsubscribe(HandleSecurityReferenceSet);
        }
        #endregion
    }
    #endregion
}
