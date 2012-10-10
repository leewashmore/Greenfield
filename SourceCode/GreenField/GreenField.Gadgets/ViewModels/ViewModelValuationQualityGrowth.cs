using System;
using System.Collections.Generic;
using System.Windows;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.ViewModel;
using GreenField.Common;
using GreenField.DataContracts;
using GreenField.DataContracts.DataContracts;
using GreenField.ServiceCaller;

namespace GreenField.Gadgets.ViewModels
{
    public class ViewModelValuationQualityGrowth : NotificationObject    
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
        /// Private member containing the Key Value Pair
        /// </summary>
        private FilterSelectionData holdingDataFilter;

        /// <summary>
        /// Private member to store info about look thru enabled or not
        /// </summary>
        private bool lookThruEnabled;
        #endregion

        #region Constructor

        /// <summary>
        /// Constructor of the class that initializes various objects
        /// </summary>
        /// <param name="param">MEF Eventaggrigator instance</param>
        public ViewModelValuationQualityGrowth(DashboardGadgetParam param)
        {
            dbInteractivity = param.DBInteractivity;
            logger = param.LoggerFacade;
            eventAggregator = param.EventAggregator;
            portfolioSelectionData = param.DashboardGadgetPayload.PortfolioSelectionData;
            effectiveDate = param.DashboardGadgetPayload.EffectiveDate;
            holdingDataFilter = param.DashboardGadgetPayload.FilterSelectionData;
            lookThruEnabled = param.DashboardGadgetPayload.IsLookThruEnabled;          
            if (eventAggregator != null)
            {
                eventAggregator.GetEvent<PortfolioReferenceSetEvent>().Subscribe(HandleFundReferenceSet);
                eventAggregator.GetEvent<EffectiveDateReferenceSetEvent>().Subscribe(HandleEffectiveDateSet);
                eventAggregator.GetEvent<HoldingFilterReferenceSetEvent>().Subscribe(HandleFilterReferenceSetEvent);
                eventAggregator.GetEvent<LookThruFilterReferenceSetEvent>().Subscribe(HandleLookThruReferenceSetEvent);
            }
            if (effectiveDate != null && portfolioSelectionData != null  && holdingDataFilter != null && IsActive)
            {
                dbInteractivity.RetrieveValuationGrowthData(portfolioSelectionData, effectiveDate, holdingDataFilter.Filtertype, 
                    holdingDataFilter.FilterValues, lookThruEnabled, RetrieveValuationQualityGrowthCallbackMethod);
            }
            if (effectiveDate != null && portfolioSelectionData != null && holdingDataFilter == null && IsActive)
            {
                dbInteractivity.RetrieveValuationGrowthData(portfolioSelectionData, effectiveDate, "Show Everything", " ", 
                    lookThruEnabled, RetrieveValuationQualityGrowthCallbackMethod);
            } 
        }
        #endregion

        #region Properties
        private bool isActive;
        /// <summary>
        /// IsActive is true when parent control is displayed on UI
        /// </summary>
        public bool IsActive
        {
            get
            { return isActive; }
            set
            {
                isActive = value;
                if (effectiveDate != null && portfolioSelectionData != null && holdingDataFilter != null && IsActive)
                {
                    if (null != ValuationQualityGrowthDataLoadedEvent)
                    {
                        ValuationQualityGrowthDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
                    }
                    dbInteractivity.RetrieveValuationGrowthData(portfolioSelectionData, effectiveDate, holdingDataFilter.Filtertype, 
                        holdingDataFilter.FilterValues, lookThruEnabled, RetrieveValuationQualityGrowthCallbackMethod);
                }

                if (effectiveDate != null && portfolioSelectionData != null && holdingDataFilter == null && IsActive)
                {
                    if (null != ValuationQualityGrowthDataLoadedEvent)
                    {
                        ValuationQualityGrowthDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
                    }
                    dbInteractivity.RetrieveValuationGrowthData(portfolioSelectionData, effectiveDate, "Show Everything", " ", lookThruEnabled, 
                        RetrieveValuationQualityGrowthCallbackMethod);
                }
            }
        }

        /// <summary>
        /// Consists of whole Portfolio Data
        /// </summary>
        private List<ValuationQualityGrowthData> valuationQualityGrowthInfo;
        public List<ValuationQualityGrowthData> ValuationQualityGrowthInfo
        {
            get
            { return valuationQualityGrowthInfo; }
            set
            {
                valuationQualityGrowthInfo = value;
                RaisePropertyChanged(() => this.ValuationQualityGrowthInfo);
            }
        }
        #endregion

        #region Callback methods
        /// <summary>
        /// Callback method for this gadget
        /// </summary>
        /// <param name="result"></param>
        public void RetrieveValuationQualityGrowthCallbackMethod(List<ValuationQualityGrowthData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null && result.Count > 0)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result, 1);
                    ValuationQualityGrowthInfo = result;
                    if (null != ValuationQualityGrowthDataLoadedEvent)
                    {
                        ValuationQualityGrowthDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
                    }
                }
                else
                {
                    ValuationQualityGrowthInfo = result;
                    Logging.LogMethodParameterNull(logger, methodNamespace, 1);
                    ValuationQualityGrowthDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
                }
            }

            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", 
                    MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Assigns UI Field Properties based on Fund reference
        /// </summary>
        /// <param name="portSelectionData">Object of PortfolioSelectionData Class containing Fund data</param>
        public void HandleFundReferenceSet(PortfolioSelectionData portSelectionData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (portSelectionData != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, portSelectionData, 1);
                    portfolioSelectionData = portSelectionData;                   

                    if (effectiveDate != null && portfolioSelectionData != null && holdingDataFilter != null && IsActive)
                    {
                        if (null != ValuationQualityGrowthDataLoadedEvent)
                        {
                            ValuationQualityGrowthDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
                        }
                        dbInteractivity.RetrieveValuationGrowthData(portfolioSelectionData, effectiveDate, holdingDataFilter.Filtertype, 
                            holdingDataFilter.FilterValues, lookThruEnabled, RetrieveValuationQualityGrowthCallbackMethod);
                    }
                    if (effectiveDate != null && portfolioSelectionData != null && holdingDataFilter == null && IsActive)
                    {
                        if (null != ValuationQualityGrowthDataLoadedEvent)
                        {
                            ValuationQualityGrowthDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
                        }
                        dbInteractivity.RetrieveValuationGrowthData(portfolioSelectionData, effectiveDate, "Show Everything", " ", 
                            lookThruEnabled, RetrieveValuationQualityGrowthCallbackMethod);
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
        /// Assigns UI Field Properties based on Effective Date
        /// </summary>
        /// <param name="effectDate"></param>
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
                    if (effectiveDate != null && portfolioSelectionData != null && holdingDataFilter != null && IsActive)
                    {
                        if (null != ValuationQualityGrowthDataLoadedEvent)
                            ValuationQualityGrowthDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
                        dbInteractivity.RetrieveValuationGrowthData(portfolioSelectionData, effectiveDate, holdingDataFilter.Filtertype, 
                            holdingDataFilter.FilterValues, lookThruEnabled, RetrieveValuationQualityGrowthCallbackMethod);
                    }

                    if (effectiveDate != null && portfolioSelectionData != null && holdingDataFilter == null && IsActive)
                    {
                        if (null != ValuationQualityGrowthDataLoadedEvent)
                            ValuationQualityGrowthDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
                        dbInteractivity.RetrieveValuationGrowthData(portfolioSelectionData, effectiveDate, "Show Everything", " ", 
                            lookThruEnabled, RetrieveValuationQualityGrowthCallbackMethod);
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
        /// Assigns UI Field Properties based on Holding Filter reference
        /// </summary>
        /// <param name="filterSelectionData">Key value pais consisting of the Filter Type and Filter Value selected by the user </param>
        public void HandleFilterReferenceSetEvent(FilterSelectionData filterSelectionData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (filterSelectionData != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, filterSelectionData, 1);
                    holdingDataFilter = filterSelectionData;
                    if (effectiveDate != null && portfolioSelectionData != null && holdingDataFilter != null && IsActive)
                    {
                        if (null != ValuationQualityGrowthDataLoadedEvent)
                        {
                            ValuationQualityGrowthDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
                        }
                        dbInteractivity.RetrieveValuationGrowthData(portfolioSelectionData, effectiveDate, holdingDataFilter.Filtertype, 
                            holdingDataFilter.FilterValues, lookThruEnabled, RetrieveValuationQualityGrowthCallbackMethod);
                    }
                    if (effectiveDate != null && portfolioSelectionData != null && holdingDataFilter == null && IsActive)
                    {
                        if (null != ValuationQualityGrowthDataLoadedEvent)
                        {
                            ValuationQualityGrowthDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
                        }
                        dbInteractivity.RetrieveValuationGrowthData(portfolioSelectionData, effectiveDate, "Show Everything", " ", 
                            lookThruEnabled, RetrieveValuationQualityGrowthCallbackMethod);
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
        /// Event Handler for LookThru Status
        /// </summary>
        /// <param name="enableLookThru">True: LookThru Enabled/False: LookThru Disabled</param>
        public void HandleLookThruReferenceSetEvent(bool enableLookThru)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                Logging.LogMethodParameter(logger, methodNamespace, enableLookThru, 1);
                lookThruEnabled = enableLookThru;

                if (effectiveDate != null && portfolioSelectionData != null && holdingDataFilter != null && IsActive)
                {
                    if (null != ValuationQualityGrowthDataLoadedEvent)
                    {
                        ValuationQualityGrowthDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
                    }
                    dbInteractivity.RetrieveValuationGrowthData(portfolioSelectionData, effectiveDate, holdingDataFilter.Filtertype, 
                        holdingDataFilter.FilterValues, lookThruEnabled, RetrieveValuationQualityGrowthCallbackMethod);
                }
                if (effectiveDate != null && portfolioSelectionData != null && holdingDataFilter == null && IsActive)
                {
                    if (null != ValuationQualityGrowthDataLoadedEvent)
                    {
                        ValuationQualityGrowthDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
                    }
                    dbInteractivity.RetrieveValuationGrowthData(portfolioSelectionData, effectiveDate, "Show Everything", " ", lookThruEnabled, 
                        RetrieveValuationQualityGrowthCallbackMethod);
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
        public event DataRetrievalProgressIndicatorEventHandler ValuationQualityGrowthDataLoadedEvent;
        #endregion

        #region EventUnSubscribe
        /// <summary>
        /// Method that disposes the events
        /// </summary>
        public void Dispose()
        {
            eventAggregator.GetEvent<PortfolioReferenceSetEvent>().Unsubscribe(HandleFundReferenceSet);
            eventAggregator.GetEvent<EffectiveDateReferenceSetEvent>().Unsubscribe(HandleEffectiveDateSet);
            eventAggregator.GetEvent<HoldingFilterReferenceSetEvent>().Unsubscribe(HandleFilterReferenceSetEvent);
            eventAggregator.GetEvent<LookThruFilterReferenceSetEvent>().Unsubscribe(HandleLookThruReferenceSetEvent);
        }
        #endregion
    }
}
