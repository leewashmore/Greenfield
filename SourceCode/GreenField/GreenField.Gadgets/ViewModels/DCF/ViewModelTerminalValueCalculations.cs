using System;
using System.Windows;
using System.Windows.Input;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Prism.Logging;
using GreenField.ServiceCaller;
using Microsoft.Practices.Prism.Events;
using GreenField.Common;
using GreenField.Gadgets.Helpers;
using System.Collections.Generic;
using Telerik.Windows.Controls.Charting;
using System.Collections.ObjectModel;
using GreenField.DataContracts;
using System.Linq;
using GreenField.Gadgets.Models;

namespace GreenField.Gadgets.ViewModels
{
    /// <summary>
    /// View-Model for TerminalValueCalculations
    /// </summary>
    public class ViewModelTerminalValueCalculations : NotificationObject
    {
        #region PrivateVariables

        /// <summary>
        /// Event Aggregator
        /// </summary>
        private IEventAggregator _eventAggregator;

        /// <summary>
        /// Instance of Service Caller Class
        /// </summary>
        private IDBInteractivity _dbInteractivity;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor that initialises the class
        /// </summary>
        /// <param name="param">DashboardGadgetParam- Contains Singleton instances of Private Variables</param>
        public ViewModelTerminalValueCalculations(DashboardGadgetParam param)
        {
            _eventAggregator = param.EventAggregator;
            _dbInteractivity = param.DBInteractivity;
            _logger = param.LoggerFacade;

            EntitySelectionData = param.DashboardGadgetPayload.EntitySelectionData;
            if (_eventAggregator != null)
            {
                _eventAggregator.GetEvent<SecurityReferenceSetEvent>().Subscribe(HandleSecurityReferenceSetEvent);
            }

            if (EntitySelectionData != null)
            {
                HandleSecurityReferenceSetEvent(EntitySelectionData);
            }
        }

        #endregion

        #region PropertyDeclaration

        #region SelectedSecurity

        /// <summary>
        /// Selected Security
        /// </summary>
        private EntitySelectionData _entitySelectionData;
        public EntitySelectionData EntitySelectionData
        {
            get
            {
                return _entitySelectionData;
            }
            set
            {
                _entitySelectionData = value;
                this.RaisePropertyChanged(() => this.EntitySelectionData);
            }
        }

        #endregion

        #region Dashboard

        /// <summary>
        /// Bool to check whether the Current Dashboard is Selected or Not
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
                _isActive = value;
                this.RaisePropertyChanged(() => this.IsActive);
            }
        }

        #endregion

        #region Busy Indicator

        /// <summary>
        /// Busy Indicator Status
        /// </summary>
        private bool _busyIndicatorIsBusy;
        public bool BusyIndicatorIsBusy
        {
            get { return _busyIndicatorIsBusy; }
            set
            {
                _busyIndicatorIsBusy = value;
                RaisePropertyChanged(() => this.BusyIndicatorIsBusy);
            }
        }

        /// <summary>
        /// Busy Indicator Content
        /// </summary>
        private string _busyIndicatorContent;
        public string BusyIndicatorContent
        {
            get { return _busyIndicatorContent; }
            set
            {
                _busyIndicatorContent = value;
                RaisePropertyChanged(() => this.BusyIndicatorContent);
            }
        }
        #endregion

        #region DataGrid

        /// <summary>
        /// List of Type TerminalValueCalculationsData
        /// </summary>
        private List<DCFTerminalValueCalculationsData> _terminalValueCalculationsData;
        public List<DCFTerminalValueCalculationsData> TerminalValueCalculationsData
        {
            get
            {
                return _terminalValueCalculationsData;
            }
            set
            {
                _terminalValueCalculationsData = value;
                this.RaisePropertyChanged(() => this.TerminalValueCalculationsData);
            }
        }

        /// <summary>
        /// List of type TerminalValueCalculationsDisplayData to show in the Grid
        /// </summary>
        private List<DCFDisplayData> _terminalValueCalculationsDisplayData;
        public List<DCFDisplayData> TerminalValueCalculationsDisplayData
        {
            get
            {
                return _terminalValueCalculationsDisplayData;
            }
            set
            {
                _terminalValueCalculationsDisplayData = value;
                this.RaisePropertyChanged(() => this.TerminalValueCalculationsDisplayData);
            }
        }

        /// <summary>
        /// FreeCashFlow for Year9
        /// </summary>
        private decimal _freeCashFlowY9;
        public decimal FreeCashFlowY9
        {
            get 
            {
                return _freeCashFlowY9; 
            }
            set
            {
                _freeCashFlowY9 = value; 
            }
        }
        

        #endregion

        #region LoggerFacade

        /// <summary>
        /// Public property for LoggerFacade _logger
        /// </summary>
        private ILoggerFacade _logger;
        public ILoggerFacade Logger
        {
            get
            {
                return _logger;
            }
            set
            {
                _logger = value;
            }
        }

        #endregion

        #region Calculations

        /// <summary>
        /// TerminalGrowthRate from FreeCashFlows
        /// </summary>
        private decimal? _terminalGrowthRate;
        public decimal? TerminalGrowthRate
        {
            get 
            {
                return _terminalGrowthRate; 
            }
            set
            {
                _terminalGrowthRate = value;
                this.RaisePropertyChanged(() => this.TerminalGrowthRate);
            }
        }

        /// <summary>
        /// TerminalValuePresent
        /// </summary>
        private decimal? _terminalValuePresent;
        public decimal? TerminalValuePresent
        {
            get 
            {
                return _terminalValuePresent; 
            }
            set
            {
                _terminalValuePresent = value;
                this.RaisePropertyChanged(() => this.TerminalValuePresent);
            }
        }

        /// <summary>
        /// TerminalValueNominal
        /// </summary>
        private decimal? _terminalValueNominal;
        public decimal? TerminalValueNominal
        {
            get 
            {
                return _terminalValueNominal; 
            }
            set
            {
                _terminalValueNominal = value;
                this.RaisePropertyChanged(() => this.TerminalValueNominal);
            }
        }
        

        #endregion
        
        #endregion

        #region EventHandlers

        /// <summary>
        /// Event Handler to subscribed event 'SecurityReferenceSet'
        /// </summary>
        /// <param name="securityReferenceData">SecurityReferenceData</param>
        public void HandleSecurityReferenceSetEvent(EntitySelectionData entitySelectionData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                //ArgumentNullException
                if (entitySelectionData != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, entitySelectionData, 1);

                    EntitySelectionData = entitySelectionData;
                    if (IsActive && EntitySelectionData != null)
                    {
                        _dbInteractivity.RetrieveDCFTerminalValueCalculationsData(EntitySelectionData, RetrieveDCFTerminalValueCalculationsDataCallbackMethod);
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

        #region CallbackMethods

        /// <summary>
        /// Consensus Estimate Data callback Method
        /// </summary>
        /// <param name="result"></param>
        public void RetrieveDCFTerminalValueCalculationsDataCallbackMethod(List<DCFTerminalValueCalculationsData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
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
            finally
            {
                BusyIndicatorNotification();
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        #endregion

        #region HelperMethods

        /// <summary>
        /// Busy Indicator Notification
        /// </summary>
        /// <param name="showBusyIndicator">Busy Indicator Running/Stopped: True/False</param>
        /// <param name="message">Message in Busy Indicator</param>
        public void BusyIndicatorNotification(bool showBusyIndicator = false, String message = null)
        {
            if (message != null)
                BusyIndicatorContent = message;
            BusyIndicatorIsBusy = showBusyIndicator;
        }

        #endregion

    }
}
