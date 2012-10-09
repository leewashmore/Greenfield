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
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.ViewModel;
using GreenField.ServiceCaller.SecurityReferenceDefinitions;
using GreenField.Common;
using GreenField.ServiceCaller.BenchmarkHoldingsDefinitions;
using GreenField.DataContracts;
using GreenField.ServiceCaller;

namespace GreenField.Gadgets.ViewModels
{
    /// <summary>
    /// Class that provides the interaction of the view model with the Service caller and the View.
    /// </summary>
    public class ViewModelPerformanceGrid : NotificationObject
    {
        #region PrivateMembers
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
        private ILoggerFacade logger;
        /// <summary>
        /// private member object of the PortfolioSelectionData class for storing Fund Selection Data
        /// </summary>
        private PortfolioSelectionData portfolioSelectionData;
        /// <summary>
        /// Stores Effective Date selected by the user
        /// </summary>
        private DateTime? effectiveDate;        
        /// <summary>
        /// Country selected from the heat map
        /// </summary>
        private String country;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor of the class that initializes various objects
        /// </summary>
        /// <param name="param">MEF Eventaggrigator instance</param>
        public ViewModelPerformanceGrid(DashboardGadgetParam param)
        {
            effectiveDate = param.DashboardGadgetPayload.EffectiveDate;
            dbInteractivity = param.DBInteractivity;
            logger = param.LoggerFacade;
            eventAggregator = param.EventAggregator;
            country = param.DashboardGadgetPayload.HeatMapCountryData;
            portfolioSelectionData = param.DashboardGadgetPayload.PortfolioSelectionData;
            if (effectiveDate != null && portfolioSelectionData != null && IsActive)
            {
                dbInteractivity.RetrievePerformanceGridData(portfolioSelectionData, Convert.ToDateTime(effectiveDate), "NoFiltering",  RetrievePerformanceGridDataCallbackMethod);
            }          
            if (eventAggregator != null)
            {
                eventAggregator.GetEvent<PortfolioReferenceSetEvent>().Subscribe(HandlePortfolioReferenceSet, false);
                eventAggregator.GetEvent<EffectiveDateReferenceSetEvent>().Subscribe(HandleEffectiveDateSet, false);
                eventAggregator.GetEvent<HeatMapClickEvent>().Subscribe(HandleCountrySelectionDataSet, false);                
            }  
        }
        #endregion

        #region Properties
        #region UI Fields
        /// <summary>
        /// Collection binded to the Grid
        /// </summary>
        private List<PerformanceGridData> performanceGridInfo;
        public List<PerformanceGridData> PerformanceInfo
        {
            get
            {
                return performanceGridInfo;
            }
            set
            {
                if (performanceGridInfo != value)
                {
                    performanceGridInfo = value;
                    RaisePropertyChanged(() => this.PerformanceInfo);
                }
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
                isActive = value;
                if (portfolioSelectionData != null && effectiveDate != null && isActive)
                {
                    if (country != null)
                    {
                        BeginWebServiceCall(portfolioSelectionData, Convert.ToDateTime(effectiveDate), country);
                    }
                    else
                        BeginWebServiceCall(portfolioSelectionData, Convert.ToDateTime(effectiveDate), "NoFiltering");
                }
            }
        }
        #endregion
        #endregion

        #region Events
        /// <summary>
        /// Event for the notification of Data Load Completion
        /// </summary>
        public event DataRetrievalProgressIndicatorEventHandler PerformanceGridDataLoadedEvent;
        #endregion

        #region Event Handlers
        /// <summary>
        /// Assigns UI Field Properties based on Fund reference
        /// </summary>
        /// <param name="portSelectionData">Object of PortfolioSelectionData Class</param>
        public void HandlePortfolioReferenceSet(PortfolioSelectionData portSelectionData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (portSelectionData != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, portSelectionData, 1);
                    portfolioSelectionData = portSelectionData;
                    if (effectiveDate != null && portfolioSelectionData != null && IsActive)
                    {                        
                        BeginWebServiceCall(portSelectionData, Convert.ToDateTime(effectiveDate), "NoFiltering");
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
            Logging.LogEndMethod(logger, methodNamespace);
        }

        /// <summary>
        /// Assigns UI Field Properties based on effectiveDate
        /// </summary>
        /// <param name="effectDate">effective Date selected by the user</param>
        public void HandleEffectiveDateSet(DateTime effectDate)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (effectDate != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, effectDate, 1);
                    effectiveDate = effectDate;
                    if (portfolioSelectionData != null && effectiveDate != null && IsActive)
                    {
                        BeginWebServiceCall(portfolioSelectionData, Convert.ToDateTime(effectiveDate), "NoFiltering");
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
            Logging.LogEndMethod(logger, methodNamespace);
        }

        /// <summary>
        /// Assigns UI Field Properties based on Country
        /// </summary>
        /// <param name="cou">Country selected by the user from the heat map</param>
        public void HandleCountrySelectionDataSet(String cou)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (cou != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, cou, 1);
                    country = cou;
                    if (portfolioSelectionData != null && effectiveDate != null && country != null && IsActive)
                    {
                        BeginWebServiceCall(portfolioSelectionData, Convert.ToDateTime(effectiveDate), country);
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
            Logging.LogEndMethod(logger, methodNamespace);
        }

        /// <summary>
        /// Calling web service through dbInteractivity 
        /// </summary>
        /// <param name="portfolioSelectionData"></param>
        /// <param name="effectiveDate"></param>
        /// <param name="country"></param>
        private void BeginWebServiceCall(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate, String country)
        {
            if (null != PerformanceGridDataLoadedEvent)
            {
                PerformanceGridDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
            }
            dbInteractivity.RetrievePerformanceGridData(portfolioSelectionData, effectiveDate, country, RetrievePerformanceGridDataCallbackMethod);
        }
        #endregion

        #region CallbackMethods
        /// <summary>
        /// Plots the result on grid after getting the resulting collection
        /// </summary>
        /// <param name="result">Contains the Performance Grid Data</param>
        private void RetrievePerformanceGridDataCallbackMethod(List<PerformanceGridData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null && result.Count > 0)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result, 1);
                    PerformanceInfo = result;
                    if (null != PerformanceGridDataLoadedEvent)
                    {
                        PerformanceGridDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
                    }
                }
                else
                {
                    PerformanceInfo = result;   
                    Logging.LogMethodParameterNull(logger, methodNamespace, 1);
                    PerformanceGridDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
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
            eventAggregator.GetEvent<PortfolioReferenceSetEvent>().Unsubscribe(HandlePortfolioReferenceSet);
            eventAggregator.GetEvent<EffectiveDateReferenceSetEvent>().Unsubscribe(HandleEffectiveDateSet);            
            eventAggregator.GetEvent<HeatMapClickEvent>().Unsubscribe(HandleCountrySelectionDataSet);
        }
        #endregion
    }
}
