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
using Microsoft.Practices.Prism.Events;
using GreenField.ServiceCaller;
using Microsoft.Practices.Prism.Logging;
using GreenField.Common;
using GreenField.DataContracts;
using Microsoft.Practices.Prism.ViewModel;
using System.Collections.Generic;
using System.Linq;

namespace GreenField.Gadgets.ViewModels
{
    public class ViewModelFreeCashFlows: NotificationObject
    {
        #region PRIVATE FIELDS
        //MEF Singletons

        /// <summary>
        /// private member object of the IEventAggregator for event aggregation
        /// </summary>
        private IEventAggregator _eventAggregator;

        /// <summary>
        /// private member object of the IDBInteractivity for interaction with the Service Caller
        /// </summary>
        private IDBInteractivity _dbInteractivity;

        /// <summary>
        /// private member object of ILoggerFacade for logging
        /// </summary>
        private ILoggerFacade _logger;
        
        /// <summary>
        /// Private member to store basic data
        /// </summary>
        private FreeCashFlowsData _freeCashFlowsDataInfo;       

        /// <summary>
        /// Private member to store basic data gadget visibilty
        /// </summary>
        private Visibility _freeCashFlowGadgetVisibility = Visibility.Collapsed;
        /// <summary>
        /// Private member to store Selected Security ID
        /// </summary>
        private EntitySelectionData _securitySelectionData = null;

        #endregion
        
        #region PROPERTIES

        /// <summary>
        /// Stores data for Basic data grid
        /// </summary>
        public FreeCashFlowsData FreeCashFlowsDataInfo
        {
            get { return _freeCashFlowsDataInfo; }
            set
            {
                if (_freeCashFlowsDataInfo != value)
                {
                    _freeCashFlowsDataInfo = value;
                    RaisePropertyChanged(() => this.FreeCashFlowsDataInfo);
                }
            }
        }
        public Visibility FreeCashFlowGadgetVisibility
        {
            get { return _freeCashFlowGadgetVisibility; }
            set
            {
                _freeCashFlowGadgetVisibility = value;
                RaisePropertyChanged(() => this.FreeCashFlowGadgetVisibility);
            }
        }

        /// <summary>
        /// IsActive is true when parent control is displayed on UI
        /// </summary>
        private bool _isActive;
        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
                if (_securitySelectionData != null && IsActive)
                {
                    if (_securitySelectionData.InstrumentID != null && _securitySelectionData.InstrumentID != string.Empty)
                    {
                        CallingWebMethod();
                    }
                }
            }
        }

        #endregion

        #region CONSTRUCTOR
        public ViewModelFreeCashFlows(DashboardGadgetParam param)
        {
            _eventAggregator = param.EventAggregator;
            _dbInteractivity = param.DBInteractivity;
            _logger = param.LoggerFacade;
            _securitySelectionData = param.DashboardGadgetPayload.EntitySelectionData;

            if (_eventAggregator != null)
            {
                _eventAggregator.GetEvent<SecurityReferenceSetEvent>().Subscribe(HandleSecurityReferenceSet);
            }
            if (_securitySelectionData != null && IsActive)
            {
                if (_securitySelectionData.InstrumentID != null && _securitySelectionData.InstrumentID != string.Empty)
                {
                    CallingWebMethod();
                }
            }
            
        }

        #endregion

        #region EVENTS
        /// <summary>
        /// event to handle data retrieval progress indicator
        /// </summary>
        public event DataRetrievalProgressIndicatorEventHandler FreeCashFlowsDataLoadEvent;

        #endregion

        #region EVENTHANDLERS
        /// <summary>
        /// Event Handler to subscribed event 'SecurityReferenceSet'
        /// </summary>
        /// <param name="securityReferenceData">SecurityReferenceData</param>
        public void HandleSecurityReferenceSet(EntitySelectionData entitySelectionData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (entitySelectionData != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, entitySelectionData, 1);
                    _securitySelectionData = entitySelectionData;

                    if (_securitySelectionData.InstrumentID != null && _securitySelectionData.InstrumentID != string.Empty)
                    {
                        CallingWebMethod();
                        
                    }
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
        }
        #endregion

        #region CALLBACK METHOD
        /// <summary>
        /// Callback method that assigns value to the BAsicDataInfo property
        /// </summary>
        /// <param name="result">basic data </param>
        private void RetrieveFreeCashFlowsDataCallbackMethod(List<FreeCashFlowsData> freeCashFlowsData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                FreeCashFlowsDataInfo = null;
                FreeCashFlowGadgetVisibility = Visibility.Collapsed;
                if (freeCashFlowsData != null && freeCashFlowsData.Count > 0)
                {
                    FreeCashFlowGadgetVisibility = Visibility.Visible;
                    Logging.LogMethodParameter(_logger, methodNamespace, freeCashFlowsData, 1);
                    FreeCashFlowsDataInfo = freeCashFlowsData.FirstOrDefault();
                    this.RaisePropertyChanged(() => this.FreeCashFlowsDataInfo);
                }
                else
                {
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                }
                if (FreeCashFlowsDataLoadEvent != null)
                    FreeCashFlowsDataLoadEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        #endregion

        #region SERVICE CALL METOHD
        /// <summary>
        /// Calls web service method
        /// </summary>
        private void CallingWebMethod()
        {
            if (FreeCashFlowsDataLoadEvent != null)
                            FreeCashFlowsDataLoadEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
                        //_dbInteractivity.RetrieveFreeCashFlowsData(_securitySelectionData, RetrieveFreeCashFlowsDataCallbackMethod);
            

        }
        #endregion  


        #region EventUnSubscribe

        public void Dispose()
        {
            _eventAggregator.GetEvent<SecurityReferenceSetEvent>().Unsubscribe(HandleSecurityReferenceSet);
        }
        #endregion
    }
}


   
