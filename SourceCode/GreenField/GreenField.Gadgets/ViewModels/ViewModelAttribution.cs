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
using GreenField.ServiceCaller.ProxyDataDefinitions;
using GreenField.Common;
using System.Collections.Generic;

namespace GreenField.Gadgets.ViewModels
{
    /// <summary>
    /// Class that provides the interaction of the view model with the Service caller and the View.
    /// </summary>
    public class ViewModelAttribution : NotificationObject
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
        /// private member object of the FundSelectionData class for storing Fund Selection Data
        /// </summary>
        private FundSelectionData _fundSelectionData;

       

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor of the class that initializes various objects
        /// </summary>
        /// <param name="param">MEF Eventaggrigator instance</param>
        public ViewModelAttribution(DashboardGadgetParam param)
        {
            _dbInteractivity = param.DBInteractivity;
            _logger = param.LoggerFacade;
            _eventAggregator = param.EventAggregator;
            _fundSelectionData = param.DashboardGadgetPayload.FundSelectionData;

            //_dbInteractivity.RetrieveFundSelectionData(RetrieveFundSelectionDataCallBackMethod);
            _eventAggregator.GetEvent<FundReferenceSetEvent>().Subscribe(HandleFundReferenceSet, false);
            if (_fundSelectionData != null)
                HandleFundReferenceSet(_fundSelectionData);
        }

        #endregion

        #region Properties
        #region UI Fields

        /// <summary>
        /// Collection binded to the Comparison chart - consists of Attribution data for the selected fund
        /// </summary>
        private List<AttributionData> _attributionDataInfo;
        public List<AttributionData> AttributionDataInfo
        {
            get
            {
                if (_attributionDataInfo == null)
                    _attributionDataInfo = new List<AttributionData>();
                return _attributionDataInfo;
            }
            set
            {
                if (_attributionDataInfo != value)
                {
                    _attributionDataInfo = value;
                    RaisePropertyChanged(() => this.AttributionDataInfo);
                }
            }
        }
        #endregion

        #region Callback Method

        /// <summary>
        /// Method that calls the service Method through a call to Service Caller
        /// </summary>
        /// <param name="nameOfFund">Unique Identifier for a fund</param>
        /// <param name="callback">Callback for this method</param>
        private void RetrieveAttributionData(String nameOfFund, Action<List<AttributionData>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (nameOfFund != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, nameOfFund, 1);
                    if (callback != null)
                    {
                        Logging.LogMethodParameter(_logger, methodNamespace, callback, 2);
                        if (null != attributionDataLoadedEvent)
                            attributionDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
                        _dbInteractivity.RetrieveAttributionData(nameOfFund, callback);
                    }
                    else
                    {
                        Logging.LogMethodParameterNull(_logger, methodNamespace, 2);
                    }
                }
                else
                {
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);

        }

        #endregion

        #endregion

        #region Events
        /// <summary>
        /// Event for the notification of Data Load Completion
        /// </summary>
        public event DataRetrievalProgressIndicatorEventHandler attributionDataLoadedEvent;
        #endregion

        #region Event Handlers
        /// <summary>
        /// Assigns UI Field Properties based on Fund reference
        /// </summary>
        /// <param name="fundSelectionData">Object of FundSelectionData class containg the Fund Selection Data </param>
        public void HandleFundReferenceSet(FundSelectionData fundSelectionData)
        {

            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (fundSelectionData != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, fundSelectionData, 1);
                    _fundSelectionData = fundSelectionData;
                    RetrieveAttributionData(fundSelectionData.Name.ToString(), RetrieveAttributionDataCallBackMethod);
                }
                else
                {
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        #endregion       

        #region CallbackMethods
        /// <summary>
        /// Plots the series on the chart after getting the resulting collection
        /// </summary>
        /// <param name="result">Contains the Attribution Data</param>
        private void RetrieveAttributionDataCallBackMethod(List<AttributionData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    AttributionDataInfo = result;                    
                    if (null != attributionDataLoadedEvent)
                            attributionDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
                }
                else
                {
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                    attributionDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        #endregion      

    }
}
