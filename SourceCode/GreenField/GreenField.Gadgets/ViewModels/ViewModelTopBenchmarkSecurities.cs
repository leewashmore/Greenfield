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
using System.Collections.ObjectModel;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.ViewModel;
using GreenField.ServiceCaller;
using GreenField.ServiceCaller.SecurityReferenceDefinitions;
using GreenField.Common;
using GreenField.ServiceCaller.BenchmarkHoldingsDefinitions;
using GreenField.DataContracts;

namespace GreenField.Gadgets.ViewModels
{
    /// <summary>
    /// Class that provides the interaction of the view model with the Service caller and the View.
    /// </summary>
    public class ViewModelTopBenchmarkSecurities : NotificationObject
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
        /// private member object of the PortfolioSelectionData class for storing Benchmark Selection Data
        /// </summary>
        private PortfolioSelectionData portfolioSelectionData;
        /// <summary>
        /// Contains the effective date
        /// </summary>
        private DateTime? effectiveDate;
        #endregion

        #region Constructor

        /// <summary>
        /// Constructor of the class that initializes various objects
        /// </summary>
        /// <param name="param">MEF Eventaggrigator instance</param>
        public ViewModelTopBenchmarkSecurities(DashboardGadgetParam param)
        {
            eventAggregator = param.EventAggregator;
            dbInteractivity = param.DBInteractivity;
            logger = param.LoggerFacade;
            portfolioSelectionData = param.DashboardGadgetPayload.PortfolioSelectionData;
            effectiveDate = param.DashboardGadgetPayload.EffectiveDate;
            if (eventAggregator != null)
            {
                eventAggregator.GetEvent<PortfolioReferenceSetEvent>().Subscribe(HandlePortfolioReferenceSet);
                eventAggregator.GetEvent<EffectiveDateReferenceSetEvent>().Subscribe(HandleEffectiveDateSet);
            }
            if (effectiveDate != null && portfolioSelectionData != null && IsActive)
            {
                dbInteractivity.RetrieveTopBenchmarkSecuritiesData(portfolioSelectionData, Convert.ToDateTime(effectiveDate), RetrieveTopSecuritiesDataCallbackMethod);
            }     
        }
        #endregion

        #region Properties
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
                if (effectiveDate != null && portfolioSelectionData != null && isActive)
                {
                    BeginWebServiceCall(portfolioSelectionData, Convert.ToDateTime(effectiveDate));
                }
            }
        }

        /// <summary>
        /// Collection containing Top Ten Benchmark Securities binded to grid 
        /// </summary>
        private ObservableCollection<TopBenchmarkSecuritiesData> _topBenchmarkSecuritiesInfo;
        public ObservableCollection<TopBenchmarkSecuritiesData> TopBenchmarkSecuritiesInfo
        {
            get { return _topBenchmarkSecuritiesInfo; }
            set
            {
                _topBenchmarkSecuritiesInfo = value;
                RaisePropertyChanged(() => this.TopBenchmarkSecuritiesInfo);
            }
        }               
        #endregion

        #region Callback Methods
        /// <summary>
        /// Plots the result in the grid after getting the resulting collection
        /// </summary>
        /// <param name="result">Contains the Top Ten Benchmark Securities Data</param>
        public void RetrieveTopSecuritiesDataCallbackMethod(List<TopBenchmarkSecuritiesData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result, 1);
                    TopBenchmarkSecuritiesInfo = new ObservableCollection<TopBenchmarkSecuritiesData>(result);
                    if (null != topTenBenchmarkSecuritiesDataLoadedEvent)
                    {
                        topTenBenchmarkSecuritiesDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
                    }
                }
                else
                {
                    Logging.LogMethodParameterNull(logger, methodNamespace, 1);
                    topTenBenchmarkSecuritiesDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
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

        #region Events
        /// <summary>
        /// Event for the notification of Data Load Completion
        /// </summary>
        public event DataRetrievalProgressIndicatorEventHandler topTenBenchmarkSecuritiesDataLoadedEvent;
        #endregion

        #region Event Handlers
        /// <summary>
        /// Assigns UI Field Properties based on Effective Date
        /// </summary>
        /// <param name="effDate"></param>
        public void HandleEffectiveDateSet(DateTime effDate)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (effDate != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, effDate, 1);
                    effectiveDate = effDate;
                    if (effDate != null && portfolioSelectionData != null && IsActive)
                    {
                        BeginWebServiceCall(portfolioSelectionData, Convert.ToDateTime(effDate));
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
        /// Assigns UI Field Properties based on Benchmark reference
        /// </summary>
        /// <param name="benchmarkSelectionData">Object of BenchmarkSelectionData Class containg Benchmark data</param>
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
                    if (effectiveDate != null && portSelectionData != null && IsActive)
                    {
                        BeginWebServiceCall(portSelectionData, Convert.ToDateTime(effectiveDate));
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
        /// Makes calls to the web service through dbInteractivity
        /// </summary>
        /// <param name="portfolioSelectionData"></param>
        /// <param name="effectiveDate"></param>
        private void BeginWebServiceCall(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate)
        {
            if (effectiveDate != null && portfolioSelectionData != null)
            {
                if (null != topTenBenchmarkSecuritiesDataLoadedEvent)
                {
                    topTenBenchmarkSecuritiesDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
                }
                dbInteractivity.RetrieveTopBenchmarkSecuritiesData(portfolioSelectionData, effectiveDate, RetrieveTopSecuritiesDataCallbackMethod);
            }
        }
        #endregion               

        #region EventsUnsubscribe     
        /// <summary>
        /// Dispose Events
        /// </summary>
        public void Dispose()
        {
            eventAggregator.GetEvent<PortfolioReferenceSetEvent>().Unsubscribe(HandlePortfolioReferenceSet);
            eventAggregator.GetEvent<EffectiveDateReferenceSetEvent>().Unsubscribe(HandleEffectiveDateSet);
        }
        
        #endregion       
    }
}
