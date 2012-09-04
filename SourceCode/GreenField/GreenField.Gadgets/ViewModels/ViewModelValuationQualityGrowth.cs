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
using GreenField.DataContracts;
using Microsoft.Practices.Prism.Logging;
using GreenField.ServiceCaller;
using Microsoft.Practices.Prism.Events;
using GreenField.ServiceCaller.ExternalResearchDefinitions;
using System.Collections.Generic;
using GreenField.DataContracts.DataContracts;

namespace GreenField.Gadgets.ViewModels
{
    public class ViewModelValuationQualityGrowth : NotificationObject
    
    {
        #region PrivateMembers

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
        /// private member object of the PortfolioSelectionData class for storing Fund Selection Data
        /// </summary>
        private PortfolioSelectionData _PortfolioSelectionData;

        /// <summary>
        /// Stores Effective Date selected by the user
        /// </summary>
        private DateTime? _effectiveDate;

        /// <summary>
        /// Private member containing the Key Value Pair
        /// </summary>
        private FilterSelectionData _holdingDataFilter;

        /// <summary>
        /// Private member to store info about look thru enabled or not
        /// </summary>
        private bool _lookThruEnabled;


        #endregion

        #region Constructor

        /// <summary>
        /// Constructor of the class that initializes various objects
        /// </summary>
        /// <param name="param">MEF Eventaggrigator instance</param>
        public ViewModelValuationQualityGrowth(DashboardGadgetParam param)
        {
            _dbInteractivity = param.DBInteractivity;
            _logger = param.LoggerFacade;
            _eventAggregator = param.EventAggregator;
            _PortfolioSelectionData = param.DashboardGadgetPayload.PortfolioSelectionData;
            _effectiveDate = param.DashboardGadgetPayload.EffectiveDate;
            _holdingDataFilter = param.DashboardGadgetPayload.FilterSelectionData;
            _lookThruEnabled = param.DashboardGadgetPayload.IsLookThruEnabled;
          
            if (_eventAggregator != null)
            {
                _eventAggregator.GetEvent<PortfolioReferenceSetEvent>().Subscribe(HandleFundReferenceSet);
                _eventAggregator.GetEvent<EffectiveDateReferenceSetEvent>().Subscribe(HandleEffectiveDateSet);
                _eventAggregator.GetEvent<HoldingFilterReferenceSetEvent>().Subscribe(HandleFilterReferenceSetEvent);
                _eventAggregator.GetEvent<LookThruFilterReferenceSetEvent>().Subscribe(HandleLookThruReferenceSetEvent);
            }

            if (_effectiveDate != null && _PortfolioSelectionData != null  && _holdingDataFilter != null && IsActive)
            {
                _dbInteractivity.RetrieveValuationGrowthData(_PortfolioSelectionData, _effectiveDate, _holdingDataFilter.Filtertype, _holdingDataFilter.FilterValues, _lookThruEnabled, RetrieveValuationQualityGrowthCallbackMethod);
            }

            if (_effectiveDate != null && _PortfolioSelectionData != null && _holdingDataFilter == null && IsActive)
            {
                _dbInteractivity.RetrieveValuationGrowthData(_PortfolioSelectionData, _effectiveDate, "Show Everything", " ", _lookThruEnabled, RetrieveValuationQualityGrowthCallbackMethod);
            }
 
        }

        private bool _isActive;
        /// <summary>
        /// IsActive is true when parent control is displayed on UI
        /// </summary>
        public bool IsActive
        {
            get
            {
                return _isActive;
            }
            set
            {
                _isActive = value;
                if (_effectiveDate != null && _PortfolioSelectionData != null && _holdingDataFilter != null && IsActive)
                {
                    if (null != valuationQualityGrowthDataLoadedEvent)
                        valuationQualityGrowthDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
                    _dbInteractivity.RetrieveValuationGrowthData(_PortfolioSelectionData, _effectiveDate, _holdingDataFilter.Filtertype, _holdingDataFilter.FilterValues, _lookThruEnabled, RetrieveValuationQualityGrowthCallbackMethod);
                }

                if (_effectiveDate != null && _PortfolioSelectionData != null && _holdingDataFilter == null && IsActive)
                {
                    if (null != valuationQualityGrowthDataLoadedEvent)
                        valuationQualityGrowthDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
                    _dbInteractivity.RetrieveValuationGrowthData(_PortfolioSelectionData, _effectiveDate, "Show Everything", " ", _lookThruEnabled, RetrieveValuationQualityGrowthCallbackMethod);
                }
            }
        }

        /// <summary>
        /// Consists of whole Portfolio Data
        /// </summary>
        private List<ValuationQualityGrowthData> _valuationQualityGrowthInfo;
        public List<ValuationQualityGrowthData> ValuationQualityGrowthInfo
        {
            get
            {
                return _valuationQualityGrowthInfo;
            }
            set
            {
                _valuationQualityGrowthInfo = value;
                RaisePropertyChanged(() => this.ValuationQualityGrowthInfo);
            }
        }

        public void RetrieveValuationQualityGrowthCallbackMethod(List<ValuationQualityGrowthData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null && result.Count > 0)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    ValuationQualityGrowthInfo = result;
                    if (null != valuationQualityGrowthDataLoadedEvent)
                        valuationQualityGrowthDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
                }
                else
                {
                    ValuationQualityGrowthInfo = result;
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                    valuationQualityGrowthDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
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

        #region Event Handlers
        /// <summary>
        /// Assigns UI Field Properties based on Fund reference
        /// </summary>
        /// <param name="PortfolioSelectionData">Object of PortfolioSelectionData Class containing Fund data</param>
        public void HandleFundReferenceSet(PortfolioSelectionData PortfolioSelectionData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);

            try
            {
                if (PortfolioSelectionData != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, PortfolioSelectionData, 1);
                    _PortfolioSelectionData = PortfolioSelectionData;                   

                    if (_effectiveDate != null && _PortfolioSelectionData != null && _holdingDataFilter != null && IsActive)
                    {
                        if (null != valuationQualityGrowthDataLoadedEvent)
                            valuationQualityGrowthDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
                        _dbInteractivity.RetrieveValuationGrowthData(_PortfolioSelectionData, _effectiveDate, _holdingDataFilter.Filtertype, _holdingDataFilter.FilterValues, _lookThruEnabled, RetrieveValuationQualityGrowthCallbackMethod);
                    }

                    if (_effectiveDate != null && _PortfolioSelectionData != null && _holdingDataFilter == null && IsActive)
                    {
                        if (null != valuationQualityGrowthDataLoadedEvent)
                            valuationQualityGrowthDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
                        _dbInteractivity.RetrieveValuationGrowthData(_PortfolioSelectionData, _effectiveDate, "Show Everything", " ", _lookThruEnabled, RetrieveValuationQualityGrowthCallbackMethod);
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
        /// Assigns UI Field Properties based on Effective Date
        /// </summary>
        /// <param name="effectiveDate"></param>
        public void HandleEffectiveDateSet(DateTime effectiveDate)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (effectiveDate != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, effectiveDate, 1);
                    _effectiveDate = effectiveDate;
                    if (_effectiveDate != null && _PortfolioSelectionData != null && _holdingDataFilter != null && IsActive)
                    {
                        if (null != valuationQualityGrowthDataLoadedEvent)
                            valuationQualityGrowthDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
                        _dbInteractivity.RetrieveValuationGrowthData(_PortfolioSelectionData, _effectiveDate, _holdingDataFilter.Filtertype, _holdingDataFilter.FilterValues, _lookThruEnabled, RetrieveValuationQualityGrowthCallbackMethod);
                    }

                    if (_effectiveDate != null && _PortfolioSelectionData != null && _holdingDataFilter == null && IsActive)
                    {
                        if (null != valuationQualityGrowthDataLoadedEvent)
                            valuationQualityGrowthDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
                        _dbInteractivity.RetrieveValuationGrowthData(_PortfolioSelectionData, _effectiveDate, "Show Everything", " ", _lookThruEnabled, RetrieveValuationQualityGrowthCallbackMethod);
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
        /// Assigns UI Field Properties based on Holding Filter reference
        /// </summary>
        /// <param name="filterSelectionData">Key value pais consisting of the Filter Type and Filter Value selected by the user </param>
        public void HandleFilterReferenceSetEvent(FilterSelectionData filterSelectionData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (filterSelectionData != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, filterSelectionData, 1);
                    _holdingDataFilter = filterSelectionData;
                    if (_effectiveDate != null && _PortfolioSelectionData != null && _holdingDataFilter != null && IsActive)
                    {
                        if (null != valuationQualityGrowthDataLoadedEvent)
                            valuationQualityGrowthDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
                        _dbInteractivity.RetrieveValuationGrowthData(_PortfolioSelectionData, _effectiveDate, _holdingDataFilter.Filtertype, _holdingDataFilter.FilterValues, _lookThruEnabled, RetrieveValuationQualityGrowthCallbackMethod);
                    }

                    if (_effectiveDate != null && _PortfolioSelectionData != null && _holdingDataFilter == null && IsActive)
                    {
                        if (null != valuationQualityGrowthDataLoadedEvent)
                            valuationQualityGrowthDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
                        _dbInteractivity.RetrieveValuationGrowthData(_PortfolioSelectionData, _effectiveDate, "Show Everything", " ", _lookThruEnabled, RetrieveValuationQualityGrowthCallbackMethod);
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
        /// Event Handler for LookThru Status
        /// </summary>
        /// <param name="enableLookThru">True: LookThru Enabled/False: LookThru Disabled</param>
        public void HandleLookThruReferenceSetEvent(bool enableLookThru)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {

                Logging.LogMethodParameter(_logger, methodNamespace, enableLookThru, 1);
                _lookThruEnabled = enableLookThru;

                if (_effectiveDate != null && _PortfolioSelectionData != null && _holdingDataFilter != null && IsActive)
                {
                    if (null != valuationQualityGrowthDataLoadedEvent)
                        valuationQualityGrowthDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
                    _dbInteractivity.RetrieveValuationGrowthData(_PortfolioSelectionData, _effectiveDate, _holdingDataFilter.Filtertype, _holdingDataFilter.FilterValues, _lookThruEnabled, RetrieveValuationQualityGrowthCallbackMethod);
                }

                if (_effectiveDate != null && _PortfolioSelectionData != null && _holdingDataFilter == null && IsActive)
                {
                    if (null != valuationQualityGrowthDataLoadedEvent)
                        valuationQualityGrowthDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
                    _dbInteractivity.RetrieveValuationGrowthData(_PortfolioSelectionData, _effectiveDate, "Show Everything", " ", _lookThruEnabled, RetrieveValuationQualityGrowthCallbackMethod);
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

        #region Events
        /// <summary>
        /// Event for the notification of Data Load Completion
        /// </summary>
        public event DataRetrievalProgressIndicatorEventHandler valuationQualityGrowthDataLoadedEvent;
        #endregion

        #region EventUnSubscribe
        /// <summary>
        /// Method that disposes the events
        /// </summary>
        public void Dispose()
        {
            _eventAggregator.GetEvent<PortfolioReferenceSetEvent>().Unsubscribe(HandleFundReferenceSet);
            _eventAggregator.GetEvent<EffectiveDateReferenceSetEvent>().Unsubscribe(HandleEffectiveDateSet);
            _eventAggregator.GetEvent<HoldingFilterReferenceSetEvent>().Unsubscribe(HandleFilterReferenceSetEvent);
            _eventAggregator.GetEvent<LookThruFilterReferenceSetEvent>().Unsubscribe(HandleLookThruReferenceSetEvent);

        }

        #endregion

    }
}
