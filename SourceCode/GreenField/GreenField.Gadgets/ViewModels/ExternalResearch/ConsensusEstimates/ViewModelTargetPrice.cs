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
using GreenField.Common;
using GreenField.ServiceCaller;
using GreenField.DataContracts;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Logging;
using GreenField.Gadgets.Helpers;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

namespace GreenField.Gadgets.ViewModels
{
    /// <summary>
    /// View-Model for Target Price Gadget(Consensus Estimates)
    /// </summary>
    public class ViewModelTargetPrice : NotificationObject
    {
        #region PrivateVariables

        /// <summary>
        /// MEF Singletons
        /// </summary>
        private IDBInteractivity _dbInteractivity;
        private ILoggerFacade _logger;
        private IEventAggregator _eventAggregator;

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
                _isActive = value;
                if (_isActive)
                {
                    if (SelectedSecurity != null)
                    {
                        _dbInteractivity.RetrieveTargetPriceData(SelectedSecurity, RetrieveTargetPriceDataCallbackMethod);
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
            _eventAggregator = param.EventAggregator;
            this._dbInteractivity = param.DBInteractivity;
            this._logger = param.LoggerFacade;
            this.SelectedSecurity = param.DashboardGadgetPayload.EntitySelectionData;
            this._eventAggregator = param.EventAggregator;
            if (SelectedSecurity != null)
            {
                _dbInteractivity.RetrieveTargetPriceData(SelectedSecurity, RetrieveTargetPriceDataCallbackMethod);
                BusyIndicatorStatus = true;
            }
            if (_eventAggregator != null)
            {
                _eventAggregator.GetEvent<SecurityReferenceSetEvent>().Subscribe(HandleSecurityReferenceSet);
            }
        }

        #endregion

        #region PropertyDeclaration

        /// <summary>
        /// Selected Security from the ToolBar
        /// </summary>
        private EntitySelectionData _selectedSecurity;
        public EntitySelectionData SelectedSecurity
        {
            get
            {
                return _selectedSecurity;
            }
            set
            {
                _selectedSecurity = value;
                this.RaisePropertyChanged(() => this.SelectedSecurity);
            }
        }

        /// <summary>
        /// Variable of type TargetPriceCEData
        /// </summary>
        private TargetPriceCEData _targetPriceData;
        public TargetPriceCEData TargetPriceData
        {
            get
            {
                return _targetPriceData;
            }
            set
            {
                _targetPriceData = value;
                this.RaisePropertyChanged(() => this.TargetPriceData);
            }
        }


        /// <summary>
        /// Busy Indicator Status
        /// </summary>
        private bool _busyIndicatorStatus;
        public bool BusyIndicatorStatus
        {
            get
            {
                return _busyIndicatorStatus;
            }
            set
            {
                _busyIndicatorStatus = value;
                this.RaisePropertyChanged(() => this.BusyIndicatorStatus);
            }
        }

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
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                //ArgumentNullException
                if (entitySelectionData != null)
                {
                    SelectedSecurity = entitySelectionData;
                    if (SelectedSecurity != null && IsActive)
                    {
                        _dbInteractivity.RetrieveTargetPriceData(SelectedSecurity, RetrieveTargetPriceDataCallbackMethod);
                        BusyIndicatorNotification(true, "Updating information based on selected Security");
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

        #region Callback Methods

        /// <summary>
        /// Callback method for TargetPriceData service
        /// </summary>
        /// <param name="result"></param>
        public void RetrieveTargetPriceDataCallbackMethod(List<TargetPriceCEData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    TargetPriceData = new TargetPriceCEData();
                    CurrentPriceText = "Current Price";
                    if (result.Count != 0)
                    {
                        Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                        TargetPriceData = result.FirstOrDefault();
                        CurrentPriceText = "Current Price (" + Convert.ToDateTime(TargetPriceData.CurrentPriceDate).ToShortDateString() + " )";
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
            finally
            {
                BusyIndicatorNotification();
            }
            Logging.LogEndMethod(_logger, methodNamespace);
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
