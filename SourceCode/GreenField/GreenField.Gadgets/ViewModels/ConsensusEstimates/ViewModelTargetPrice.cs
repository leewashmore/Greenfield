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
        private EntitySelectionData _entitySelectionData;
        private IEventAggregator _eventAggregator;
        
        /// <summary>
        /// IsActive is true when parent control is displayed on UI
        /// </summary>
        public bool IsActive { get; set; }

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
        /// Collection of TargetPriceCEData, showing data in DataGrid
        /// </summary>
        private RangeObservableCollection<TargetPriceCEData> _targetPriceData;
        public RangeObservableCollection<TargetPriceCEData> TargetPriceData
        {
            get
            {
                if (_targetPriceData == null)
                    _targetPriceData = new RangeObservableCollection<TargetPriceCEData>();
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
                    if (SelectedSecurity != null)
                    {
                        _dbInteractivity.RetrieveTargetPriceData(SelectedSecurity, RetrieveTargetPriceDataCallbackMethod);
                        BusyIndicatorStatus = true;
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
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    TargetPriceData.Clear();
                    TargetPriceData.AddRange(result);
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
                BusyIndicatorStatus = false;
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
    }
}
