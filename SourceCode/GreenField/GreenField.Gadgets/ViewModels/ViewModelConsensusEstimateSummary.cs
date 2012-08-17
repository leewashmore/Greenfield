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
using GreenField.ServiceCaller;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Logging;
using GreenField.Common;
using GreenField.ServiceCaller.ExternalResearchDefinitions;
using System.Collections.Generic;
using GreenField.DataContracts;
using GreenField.DataContracts.DataContracts;

namespace GreenField.Gadgets.ViewModels
{
    public class ViewModelConsensusEstimateSummary : NotificationObject
    {
        #region Fields

        /// <summary>
        /// private member object of the IDBInteractivity for interaction with the Service Caller
        /// </summary>
        private IDBInteractivity _dbInteractivity;
        /// <summary>
        /// private member object of the IEventAggregator for event aggregation
        /// </summary>
        private IEventAggregator _eventAggregator;
        /// <summary>
        /// private member object of ILoggerFacade for logging
        /// </summary>
        private ILoggerFacade _logger;

        /// <summary>
        /// private member object of the EntitySelectionData class for storing Entity Selection Data
        /// </summary>
        private EntitySelectionData _entitySelectionData;    

        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="param">DashBoardGadgetParam</param>
        public ViewModelConsensusEstimateSummary(DashboardGadgetParam param)
        {
            _eventAggregator = param.EventAggregator;
            _dbInteractivity = param.DBInteractivity;
            _logger = param.LoggerFacade;
            _entitySelectionData = param.DashboardGadgetPayload.EntitySelectionData;
            if (_entitySelectionData !=null && IsActive)
            {
                _dbInteractivity.RetrieveConsensusEstimatesSummaryData(_entitySelectionData, RetrieveConsensusEstimatesSummaryDataDataCallbackMethod);
            }
            if (_eventAggregator != null)
            {
                _eventAggregator.GetEvent<SecurityReferenceSetEvent>().Subscribe(HandleSecurityReferenceSet,false);
               
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
        private bool _isActive;
        public bool IsActive
        {
            get
            {
                return _isActive;
            }
            set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    if (_entitySelectionData != null && _isActive)
                    {
                        if (null != consensusEstimatesSummaryDataLoadedEvent)
                            consensusEstimatesSummaryDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
                        _dbInteractivity.RetrieveConsensusEstimatesSummaryData(_entitySelectionData, RetrieveConsensusEstimatesSummaryDataDataCallbackMethod);
                        
                    }
                }
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Event for the notification of Data Load Completion
        /// </summary>
        public event DataRetrievalProgressIndicatorEventHandler consensusEstimatesSummaryDataLoadedEvent;

        public event RetrieveConsensusEstimatesSummaryCompleteEventHandler RetrieveConsensusEstimatesSummaryDataCompletedEvent;
        #endregion

        #region Event Handlers
        /// <summary>
        /// Assigns UI Field Properties based on Security reference
        /// </summary>
        /// <param name="securityReferenceData">entitySelectionData</param>
        public void HandleSecurityReferenceSet(EntitySelectionData entitySelectionData)
        {

            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (entitySelectionData != null && IsActive)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, entitySelectionData, 1);
                    _entitySelectionData = entitySelectionData;
                    if (null != consensusEstimatesSummaryDataLoadedEvent)
                        consensusEstimatesSummaryDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
                    _dbInteractivity.RetrieveConsensusEstimatesSummaryData(_entitySelectionData, RetrieveConsensusEstimatesSummaryDataDataCallbackMethod);
                }
                else
                {
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
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
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null && result.Count > 0)
                {

                    ConsensusSummaryInfo = result;
                    if (null != consensusEstimatesSummaryDataLoadedEvent)
                        consensusEstimatesSummaryDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
                    RetrieveConsensusEstimatesSummaryDataCompletedEvent(new RetrieveConsensusSummaryCompletedEventsArgs { ConsensusInfo  = result});
                   
                }
                else
                {
                    ConsensusSummaryInfo = result;                  
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                    consensusEstimatesSummaryDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }
        #endregion

        #region EventUnSubscribe
        /// <summary>
        /// Method that disposes the events
        /// </summary>
        public void Dispose()
        {
            _eventAggregator.GetEvent<SecurityReferenceSetEvent>().Unsubscribe(HandleSecurityReferenceSet);
        }
        #endregion


    }
}
