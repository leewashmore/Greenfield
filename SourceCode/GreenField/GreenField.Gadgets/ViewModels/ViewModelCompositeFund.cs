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
using Microsoft.Practices.Prism.Events;
using GreenField.ServiceCaller;
using Microsoft.Practices.Prism.Logging;
using GreenField.DataContracts;
using GreenField.Common;
using System.Collections.Generic;

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
        private IEventAggregator _eventAggregator;
        private IDBInteractivity _dbInteractivity;
        private ILoggerFacade _logger;
        private EntitySelectionData _entitySelectionData;

        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public ViewModelCompositeFund(DashboardGadgetParam param)
        {
            _logger = param.LoggerFacade;
            _dbInteractivity = param.DBInteractivity;
            _eventAggregator = param.EventAggregator;
            PortfolioSelectionData = param.DashboardGadgetPayload.PortfolioSelectionData;
            _entitySelectionData = param.DashboardGadgetPayload.EntitySelectionData;

            if (_eventAggregator != null)
            {
                _eventAggregator.GetEvent<PortfolioReferenceSetEvent>().Subscribe(HandlePortfolioReferenceSet);
                _eventAggregator.GetEvent<SecurityReferenceSetEvent>().Subscribe(HandleSecurityReferenceSetEvent);
            }
           
            //if (_entitySelectionData != null)
            //{
            //    HandleSecurityReferenceSetEvent(_entitySelectionData);
            //}
        } 
        #endregion

        #region Properties

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
                    if (_entitySelectionData != null && PortfolioSelectionData != null && IsActive)
                    {
                        BusyIndicatorNotification(true, "Retrieving Data based on selected security and portfolio");
                        _dbInteractivity.RetrieveCompositeFundData(_entitySelectionData, PortfolioSelectionData, RetrieveCompositeFundDataCallBackMethod);
                    }
                }
            }
        }

        /// <summary>
        /// DashboardGadgetPayLoad field
        /// </summary>
        private PortfolioSelectionData _portfolioSelectionData;
        public PortfolioSelectionData PortfolioSelectionData
        {
            get { return _portfolioSelectionData; }
            set
            {
                if (_portfolioSelectionData != value)
                {
                    _portfolioSelectionData = value;
                    RaisePropertyChanged(() => PortfolioSelectionData);
                }
            }
        }

        #region Busy Indicator
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

        private List<CompositeFundData> _compositeFundInfo;
        public List<CompositeFundData> CompositeFundInfo
        {
            get { return _compositeFundInfo; }
            set
            {
                if (_compositeFundInfo != value)
                {
                    _compositeFundInfo = value;
                    RaisePropertyChanged(() => CompositeFundInfo);
                }
            }
        }

        private CompositeFundData _compositeDisplayData;
        public CompositeFundData CompositeDisplayData
        {
            get { return _compositeDisplayData; }
            set 
            {
                if (_compositeDisplayData != value)
                {
                    _compositeDisplayData = value;
                    RaisePropertyChanged(() => CompositeDisplayData);
                    BusyIndicatorNotification();
                }
            }
        }

        private bool _displayIssuerIsChecked = false;
        public bool DisplayIssuerIsChecked
        {
            get { return _displayIssuerIsChecked; }
            set 
            {
                if (_displayIssuerIsChecked != value)
                {
                    _displayIssuerIsChecked = value;
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
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (portfolioSelectionData != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, portfolioSelectionData, 1);
                    PortfolioSelectionData = portfolioSelectionData;
                    if (_entitySelectionData != null && PortfolioSelectionData != null && IsActive)
                    {
                        BusyIndicatorNotification(true, "Retrieving Data based on selected security and portfolio");
                        _dbInteractivity.RetrieveCompositeFundData(_entitySelectionData, PortfolioSelectionData, RetrieveCompositeFundDataCallBackMethod);
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
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        /// <summary>
        /// Assigns UI Field Properties based on Entity Selection Data
        /// </summary>
        /// <param name="entitySelectionData">EntitySelectionData</param>
        public void HandleSecurityReferenceSetEvent(EntitySelectionData result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    _entitySelectionData = result;

                    if (_entitySelectionData != null && PortfolioSelectionData != null && IsActive)
                    {
                        BusyIndicatorNotification(true, "Retrieving Data based on selected security and portfolio");
                        _dbInteractivity.RetrieveCompositeFundData(_entitySelectionData, PortfolioSelectionData, RetrieveCompositeFundDataCallBackMethod);
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
            Logging.LogEndMethod(_logger, methodNamespace);
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
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    CompositeFundInfo = result;
                    CreateDisplayData(result, DisplayIssuerIsChecked);
                }
                else
                {
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                }
                BusyIndicatorNotification();
            }
            catch (Exception ex)
            {
                BusyIndicatorNotification();
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }

            Logging.LogEndMethod(_logger, methodNamespace);
        }
        #endregion

        #region Helper Method
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
            _eventAggregator.GetEvent<PortfolioReferenceSetEvent>().Unsubscribe(HandlePortfolioReferenceSet);
            _eventAggregator.GetEvent<SecurityReferenceSetEvent>().Unsubscribe(HandleSecurityReferenceSetEvent);
        }

        public void BusyIndicatorNotification(bool showBusyIndicator = false, String message = null)
        {
            if (message != null)
                BusyIndicatorContent = message;
            BusyIndicatorIsBusy = showBusyIndicator;
        }
        #endregion

    }
}
