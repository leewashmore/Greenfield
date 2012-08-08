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
using GreenField.ServiceCaller;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Events;
using GreenField.ServiceCaller.SecurityReferenceDefinitions;
using System.Collections.Generic;
using Microsoft.Practices.Prism.ViewModel;
using GreenField.Common;
using GreenField.Common.Helper;
using System.Collections.ObjectModel;
using GreenField.ServiceCaller.BenchmarkHoldingsDefinitions;
using GreenField.DataContracts;
using GreenField.Gadgets.Models;

namespace GreenField.Gadgets.ViewModels
{
    /// <summary>
    /// Class that provides the interaction of the view model with the Service caller and the View.
    /// </summary>
    public class ViewModelPortfolioRiskReturns : NotificationObject
    {
        #region Fields
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
        /// Contains the effective date
        /// </summary>
        private DateTime? _effectiveDate;
        #endregion

        #region Constructor

        // <summary>
        /// Constructor of the class that initializes various objects
        /// </summary>
        /// <param name="param">MEF Eventaggrigator instance</param>
        public ViewModelPortfolioRiskReturns(DashboardGadgetParam param)
        {
            _dbInteractivity = param.DBInteractivity;
            _logger = param.LoggerFacade;
            _eventAggregator = param.EventAggregator;
            _PortfolioSelectionData = param.DashboardGadgetPayload.PortfolioSelectionData;
            _selectedPeriod = param.DashboardGadgetPayload.PeriodSelectionData;
            _effectiveDate = param.DashboardGadgetPayload.EffectiveDate;

            if (_eventAggregator != null)
            {
                _eventAggregator.GetEvent<PortfolioReferenceSetEvent>().Subscribe(HandleFundReferenceSet);             
                _eventAggregator.GetEvent<EffectiveDateReferenceSetEvent>().Subscribe(HandleEffectiveDateSet);
                _eventAggregator.GetEvent<PeriodReferenceSetEvent>().Subscribe(HandlePeriodReferenceSet, false);
            }

            if (_effectiveDate != null && _PortfolioSelectionData != null )
            {
                _dbInteractivity.RetrievePortfolioRiskReturnData(_PortfolioSelectionData, Convert.ToDateTime(_effectiveDate), RetrievePortfolioRiskReturnDataCallbackMethod);
            }
          
        }
        #endregion

        #region Properties
        private List<PortfolioRiskReturnData> _portfolioRiskReturnInfo;
        public List<PortfolioRiskReturnData> PortfolioRiskReturnInfo
        {
            get
            {                
                return _portfolioRiskReturnInfo;
            }
            set
            {
                _portfolioRiskReturnInfo = value;
                RaisePropertyChanged(() => this.PortfolioRiskReturnInfo);
            }
        }

         private List<PerformancePeriodData> _portfolioRiskReturnPeriodInfo;
        public List<PerformancePeriodData> PortfolioRiskReturnPeriodInfo
        {
            get
            {                
                return _portfolioRiskReturnPeriodInfo;
            }
            set
            {
                _portfolioRiskReturnPeriodInfo = value;
                RaisePropertyChanged(() => this.PortfolioRiskReturnPeriodInfo);
            }
        }


        /// <summary>
        /// The Period selected by the user.
        /// </summary>
        private String _selectedPeriod;
        public String SelectedPeriod
        {
            get
            {
                return _selectedPeriod;
            }
            set
            {
                _selectedPeriod = value;
                RaisePropertyChanged(() => this.SelectedPeriod);
                if (PortfolioRiskReturnInfo != null)
                {
                    switch (value)
                    {
                        case "1Y":

                                List<PerformancePeriodData> result = new List<PerformancePeriodData>();                            
                                PerformancePeriodData entry = new PerformancePeriodData();                                
                                entry.DataPointName = PortfolioRiskReturnInfo[0].DataPointName;
                                entry.BenchmarkValue = PortfolioRiskReturnInfo[0].BenchMarkValue1;
                                entry.PortfolioValue = PortfolioRiskReturnInfo[0].PortfolioValue1;
                                result.Add(entry);
                                entry = new PerformancePeriodData();
                                entry.DataPointName = PortfolioRiskReturnInfo[1].DataPointName;
                                entry.BenchmarkValue = PortfolioRiskReturnInfo[1].BenchMarkValue1;
                                entry.PortfolioValue = PortfolioRiskReturnInfo[1].PortfolioValue1;
                                result.Add(entry);
                                entry = new PerformancePeriodData();
                                entry.DataPointName = PortfolioRiskReturnInfo[2].DataPointName;
                                entry.BenchmarkValue = PortfolioRiskReturnInfo[2].BenchMarkValue1;
                                entry.PortfolioValue = PortfolioRiskReturnInfo[2].PortfolioValue1;
                                result.Add(entry);
                                entry = new PerformancePeriodData();
                                entry.DataPointName = PortfolioRiskReturnInfo[3].DataPointName;
                                entry.BenchmarkValue = PortfolioRiskReturnInfo[3].BenchMarkValue1;
                                entry.PortfolioValue = PortfolioRiskReturnInfo[3].PortfolioValue1;
                                result.Add(entry);
                                entry = new PerformancePeriodData();
                                entry.DataPointName = PortfolioRiskReturnInfo[4].DataPointName;
                                entry.BenchmarkValue = PortfolioRiskReturnInfo[4].BenchMarkValue1;
                                entry.PortfolioValue = PortfolioRiskReturnInfo[4].PortfolioValue1;
                                result.Add(entry);
                            
                            PortfolioRiskReturnPeriodInfo = result;
                            if (null != portfolioRiskReturnDataLoadedEvent)
                                portfolioRiskReturnDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
                            break;
                        case "3Y":
                                List<PerformancePeriodData> result1 = new List<PerformancePeriodData>();                            
                                PerformancePeriodData entry1 = new PerformancePeriodData();
                                entry1.DataPointName = PortfolioRiskReturnInfo[0].DataPointName;
                                entry1.BenchmarkValue = PortfolioRiskReturnInfo[0].BenchMarkValue2;
                                entry1.PortfolioValue = PortfolioRiskReturnInfo[0].PortfolioValue2;
                                result1.Add(entry1);
                                entry1 = new PerformancePeriodData();
                                entry1.DataPointName = PortfolioRiskReturnInfo[1].DataPointName;
                                entry1.BenchmarkValue = PortfolioRiskReturnInfo[1].BenchMarkValue2;
                                entry1.PortfolioValue = PortfolioRiskReturnInfo[1].PortfolioValue2;
                                result1.Add(entry1);
                                entry1 = new PerformancePeriodData();
                                entry1.DataPointName = PortfolioRiskReturnInfo[2].DataPointName;
                                entry1.BenchmarkValue = PortfolioRiskReturnInfo[2].BenchMarkValue2;
                                entry1.PortfolioValue = PortfolioRiskReturnInfo[2].PortfolioValue2;
                                result1.Add(entry1);
                                entry1 = new PerformancePeriodData();
                                entry1.DataPointName = PortfolioRiskReturnInfo[3].DataPointName;
                                entry1.BenchmarkValue = PortfolioRiskReturnInfo[3].BenchMarkValue2;
                                entry1.PortfolioValue = PortfolioRiskReturnInfo[3].PortfolioValue2;
                                result1.Add(entry1);
                                entry1 = new PerformancePeriodData();
                                entry1.DataPointName = PortfolioRiskReturnInfo[4].DataPointName;
                                entry1.BenchmarkValue = PortfolioRiskReturnInfo[4].BenchMarkValue2;
                                entry1.PortfolioValue = PortfolioRiskReturnInfo[4].PortfolioValue2;
                                result1.Add(entry1);
                                PortfolioRiskReturnPeriodInfo = result1;
                                if (null != portfolioRiskReturnDataLoadedEvent)
                                portfolioRiskReturnDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
                            break;

                        case "5Y" : 

                                List<PerformancePeriodData> result2 = new List<PerformancePeriodData>();                            
                                PerformancePeriodData entry2 = new PerformancePeriodData();
                                entry2.DataPointName = PortfolioRiskReturnInfo[0].DataPointName;
                                entry2.BenchmarkValue = PortfolioRiskReturnInfo[0].BenchMarkValue3;
                                entry2.PortfolioValue = PortfolioRiskReturnInfo[0].PortfolioValue3;
                                result2.Add(entry2);
                                entry2 = new PerformancePeriodData();
                                entry2.DataPointName = PortfolioRiskReturnInfo[1].DataPointName;
                                entry2.BenchmarkValue = PortfolioRiskReturnInfo[1].BenchMarkValue3;
                                entry2.PortfolioValue = PortfolioRiskReturnInfo[1].PortfolioValue3;
                                result2.Add(entry2);
                                entry2 = new PerformancePeriodData();
                                entry2.DataPointName = PortfolioRiskReturnInfo[2].DataPointName;
                                entry2.BenchmarkValue = PortfolioRiskReturnInfo[2].BenchMarkValue3;
                                entry2.PortfolioValue = PortfolioRiskReturnInfo[2].PortfolioValue3;
                                result2.Add(entry2);
                                entry2 = new PerformancePeriodData();
                                entry2.DataPointName = PortfolioRiskReturnInfo[3].DataPointName;
                                entry2.BenchmarkValue = PortfolioRiskReturnInfo[3].BenchMarkValue3;
                                entry2.PortfolioValue = PortfolioRiskReturnInfo[3].PortfolioValue3;
                                result2.Add(entry2);
                                entry2 = new PerformancePeriodData();
                                entry2.DataPointName = PortfolioRiskReturnInfo[4].DataPointName;
                                entry2.BenchmarkValue = PortfolioRiskReturnInfo[4].BenchMarkValue3;
                                entry2.PortfolioValue = PortfolioRiskReturnInfo[4].PortfolioValue3;
                                result2.Add(entry2);
                                PortfolioRiskReturnPeriodInfo = result2;
                                if (null != portfolioRiskReturnDataLoadedEvent)
                                portfolioRiskReturnDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
                                break;

                        case "10Y":

                                List<PerformancePeriodData> result3 = new List<PerformancePeriodData>();
                                PerformancePeriodData entry3 = new PerformancePeriodData();
                                entry3.DataPointName = PortfolioRiskReturnInfo[0].DataPointName;
                                entry3.BenchmarkValue = PortfolioRiskReturnInfo[0].BenchMarkValue4;
                                entry3.PortfolioValue = PortfolioRiskReturnInfo[0].PortfolioValue4;
                                result3.Add(entry3);
                                entry3 = new PerformancePeriodData();
                                entry3.DataPointName = PortfolioRiskReturnInfo[1].DataPointName;
                                entry3.BenchmarkValue = PortfolioRiskReturnInfo[1].BenchMarkValue4;
                                entry3.PortfolioValue = PortfolioRiskReturnInfo[1].PortfolioValue4;
                                result3.Add(entry3);
                                entry3 = new PerformancePeriodData();
                                entry3.DataPointName = PortfolioRiskReturnInfo[2].DataPointName;
                                entry3.BenchmarkValue = PortfolioRiskReturnInfo[2].BenchMarkValue4;
                                entry3.PortfolioValue = PortfolioRiskReturnInfo[2].PortfolioValue4;
                                result3.Add(entry3);
                                entry3 = new PerformancePeriodData();
                                entry3.DataPointName = PortfolioRiskReturnInfo[3].DataPointName;
                                entry3.BenchmarkValue = PortfolioRiskReturnInfo[3].BenchMarkValue4;
                                entry3.PortfolioValue = PortfolioRiskReturnInfo[3].PortfolioValue4;
                                result3.Add(entry3);
                                entry3 = new PerformancePeriodData();
                                entry3.DataPointName = PortfolioRiskReturnInfo[4].DataPointName;
                                entry3.BenchmarkValue = PortfolioRiskReturnInfo[4].BenchMarkValue4;
                                entry3.PortfolioValue = PortfolioRiskReturnInfo[4].PortfolioValue4;
                                result3.Add(entry3);
                                PortfolioRiskReturnPeriodInfo = result3;
                                if (null != portfolioRiskReturnDataLoadedEvent)
                                    portfolioRiskReturnDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
                                break;
                        case "SI":

                                List<PerformancePeriodData> result4 = new List<PerformancePeriodData>();
                                PerformancePeriodData entry4 = new PerformancePeriodData();
                                entry4.DataPointName = PortfolioRiskReturnInfo[0].DataPointName;
                                entry4.BenchmarkValue = PortfolioRiskReturnInfo[0].BenchMarkValue5;
                                entry4.PortfolioValue = PortfolioRiskReturnInfo[0].PortfolioValue5;
                                result4.Add(entry4);
                                entry4 = new PerformancePeriodData();
                                entry4.DataPointName = PortfolioRiskReturnInfo[1].DataPointName;
                                entry4.BenchmarkValue = PortfolioRiskReturnInfo[1].BenchMarkValue5;
                                entry4.PortfolioValue = PortfolioRiskReturnInfo[1].PortfolioValue5;
                                result4.Add(entry4);
                                entry4 = new PerformancePeriodData();
                                entry4.DataPointName = PortfolioRiskReturnInfo[2].DataPointName;
                                entry4.BenchmarkValue = PortfolioRiskReturnInfo[2].BenchMarkValue5;
                                entry4.PortfolioValue = PortfolioRiskReturnInfo[2].PortfolioValue5;
                                result4.Add(entry4);
                                entry4 = new PerformancePeriodData();
                                entry4.DataPointName = PortfolioRiskReturnInfo[3].DataPointName;
                                entry4.BenchmarkValue = PortfolioRiskReturnInfo[3].BenchMarkValue5;
                                entry4.PortfolioValue = PortfolioRiskReturnInfo[3].PortfolioValue5;
                                result4.Add(entry4);
                                entry4 = new PerformancePeriodData();
                                entry4.DataPointName = PortfolioRiskReturnInfo[4].DataPointName;
                                entry4.BenchmarkValue = PortfolioRiskReturnInfo[4].BenchMarkValue5;
                                entry4.PortfolioValue = PortfolioRiskReturnInfo[4].PortfolioValue5;
                                result4.Add(entry4);

                                PortfolioRiskReturnPeriodInfo = result4;
                                if (null != portfolioRiskReturnDataLoadedEvent)
                                    portfolioRiskReturnDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
                                break;

                        default:
                                List<PerformancePeriodData> result5 = new List<PerformancePeriodData>();
                                PortfolioRiskReturnPeriodInfo = result5;
                                if (null != portfolioRiskReturnDataLoadedEvent)
                                    portfolioRiskReturnDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
                                break;

                    }

                }
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Event for the notification of Data Load Completion
        /// </summary>
        public event DataRetrievalProgressIndicatorEventHandler portfolioRiskReturnDataLoadedEvent;
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
                    if (_effectiveDate != null && _PortfolioSelectionData != null)
                    {
                        if (null != portfolioRiskReturnDataLoadedEvent)
                            portfolioRiskReturnDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
                        _dbInteractivity.RetrievePortfolioRiskReturnData(_PortfolioSelectionData, Convert.ToDateTime(_effectiveDate), RetrievePortfolioRiskReturnDataCallbackMethod);
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
                    if (_effectiveDate != null && _PortfolioSelectionData != null)
                    {
                        if (null != portfolioRiskReturnDataLoadedEvent)
                            portfolioRiskReturnDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
                        _dbInteractivity.RetrievePortfolioRiskReturnData(_PortfolioSelectionData,Convert.ToDateTime(_effectiveDate), RetrievePortfolioRiskReturnDataCallbackMethod);
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

        #region Callback Methods

        /// <summary>
        /// Plots the result in the grid after getting the resulting collection
        /// </summary>
        /// <param name="result">Contains the Portfolio Risk Return Data</param>
        public void RetrievePortfolioRiskReturnDataCallbackMethod(List<PortfolioRiskReturnData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    PortfolioRiskReturnInfo = new List<PortfolioRiskReturnData>(result);
                    if (null != portfolioRiskReturnDataLoadedEvent)
                    portfolioRiskReturnDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
                }
                else
                {
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                    portfolioRiskReturnDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
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
        /// Assigns UI Field Properties based on Period
        /// </summary>
        /// <param name="selectedPeriodType">Period selected by the user</param>
        public void HandlePeriodReferenceSet(String selectedPeriodType)
        {

            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (selectedPeriodType != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, selectedPeriodType, 1);
                    if (_PortfolioSelectionData != null && _effectiveDate != null)
                        if (null != portfolioRiskReturnDataLoadedEvent)
                            portfolioRiskReturnDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
                    SelectedPeriod = selectedPeriodType;

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
      
        #region EventUnSubscribe
        /// <summary>
        /// Method that disposes the events
        /// </summary>
        public void Dispose()
        {
            _eventAggregator.GetEvent<PortfolioReferenceSetEvent>().Unsubscribe(HandleFundReferenceSet);
            _eventAggregator.GetEvent<EffectiveDateReferenceSetEvent>().Unsubscribe(HandleEffectiveDateSet);           
        }

        #endregion
    }
}
