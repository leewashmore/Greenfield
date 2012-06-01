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
using GreenField.ServiceCaller.SecurityReferenceDefinitions;
using GreenField.Common;
using System.Collections.Generic;
using GreenField.ServiceCaller.BenchmarkHoldingsDefinitions;
using GreenField.Gadgets.Models;
using GreenField.DataContracts;

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
        /// private member object of the PortfolioSelectionData class for storing Fund Selection Data
        /// </summary>
        private PortfolioSelectionData _PortfolioSelectionData;

        /// <summary>
        /// Stores Effective Date selected by the user
        /// </summary>
        private DateTime? _effectiveDate;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor of the class that initializes various objects
        /// </summary>
        /// <param name="param">MEF Eventaggrigator instance</param>
        public ViewModelAttribution(DashboardGadgetParam param)
        {
            _effectiveDate = param.DashboardGadgetPayload.EffectiveDate;
            _dbInteractivity = param.DBInteractivity;
            _logger = param.LoggerFacade;
            _selectedPeriod = param.DashboardGadgetPayload.PeriodSelectionData;
            _eventAggregator = param.EventAggregator;
            _PortfolioSelectionData = param.DashboardGadgetPayload.PortfolioSelectionData;
            if (_effectiveDate != null && _PortfolioSelectionData != null)
            {
                _dbInteractivity.RetrieveAttributionData(_PortfolioSelectionData, Convert.ToDateTime(_effectiveDate), RetrieveAttributionDataCallBackMethod);
            }
            if (_eventAggregator != null)
            {
                _eventAggregator.GetEvent<PortfolioReferenceSetEvent>().Subscribe(HandleFundReferenceSet, false);
                _eventAggregator.GetEvent<EffectiveDateReferenceSetEvent>().Subscribe(HandleEffectiveDateSet, false);
                _eventAggregator.GetEvent<PeriodReferenceSetEvent>().Subscribe(HandlePeriodReferenceSet, false);
            }          
        }

        #endregion

        #region Properties
        #region UI Fields

        /// <summary>
        /// Collection that contains all the data for a particular portfolio and date
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
                    if (AttributionDataInfo != null)
                    {
                        switch (value)
                        {
                            case "1W":                                
                                List<PeriodAttributeData> result = new List<PeriodAttributeData>();                                    
                                for (int i = 0; i < AttributionDataInfo.Count; i++)
                                    {   PeriodAttributeData entry = new PeriodAttributeData();
                                    entry.Country = AttributionDataInfo[i].Country;
                                    entry.CountryName = AttributionDataInfo[i].CountryName;
                                    entry.BenchmarkWeight = AttributionDataInfo[i].Bm1RcAvgWgt1w;
                                    entry.PortfolioWeight = AttributionDataInfo[i].PorRcAvgWgt1w;
                                    entry.PortfolioReturn = AttributionDataInfo[i].FPorAshRcCtn1w;
                                    entry.BenchmarkReturn = AttributionDataInfo[i].FBm1AshRcCtn1w;
                                    entry.AssetAllocation = AttributionDataInfo[i].FBm1AshAssetAlloc1w;
                                    entry.StockSelectionTotal = AttributionDataInfo[i].FBm1AshSecSelec1w;
                                      result.Add(entry);
                                    }
                                PeriodAttributionInfo = result;
                                if (null != attributionDataLoadedEvent)
                                    attributionDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
                                break;
                            case "MTD":
                                 List<PeriodAttributeData> result3M = new List<PeriodAttributeData>();                                    
                                 for (int i = 0; i < AttributionDataInfo.Count; i++)
                                    {   PeriodAttributeData entry = new PeriodAttributeData();
                                    entry.Country = AttributionDataInfo[i].Country;
                                    entry.CountryName = AttributionDataInfo[i].CountryName;
                                    entry.BenchmarkWeight = AttributionDataInfo[i].Bm1RcAvgWgtMtd;
                                    entry.PortfolioWeight = AttributionDataInfo[i].PorRcAvgWgtMtd;
                                    entry.PortfolioReturn = AttributionDataInfo[i].FPorAshRcCtnMtd;
                                    entry.BenchmarkReturn = AttributionDataInfo[i].FBm1AshRcCtnMtd;
                                    entry.AssetAllocation = AttributionDataInfo[i].FBm1AshAssetAllocMtd;
                                    entry.StockSelectionTotal = AttributionDataInfo[i].FBm1AshSecSelecMtd;
                                      result3M.Add(entry);
                                    }
                                PeriodAttributionInfo = result3M;
                                if (null != attributionDataLoadedEvent)
                                    attributionDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
                                break;
                            case "QTD":
                                List<PeriodAttributeData> result6M = new List<PeriodAttributeData>();                                    
                                 for (int i = 0; i < AttributionDataInfo.Count; i++)
                                    {   PeriodAttributeData entry = new PeriodAttributeData();
                                    entry.Country = AttributionDataInfo[i].Country;
                                    entry.CountryName = AttributionDataInfo[i].CountryName;
                                    entry.BenchmarkWeight = AttributionDataInfo[i].Bm1RcAvgWgtQtd;
                                    entry.PortfolioWeight = AttributionDataInfo[i].PorRcAvgWgtQtd;
                                    entry.PortfolioReturn = AttributionDataInfo[i].FPorAshRcCtnQtd;
                                    entry.BenchmarkReturn = AttributionDataInfo[i].FBm1AshRcCtnQtd;
                                    entry.AssetAllocation = AttributionDataInfo[i].FBm1AshAssetAllocQtd;
                                    entry.StockSelectionTotal = AttributionDataInfo[i].FBm1AshSecSelecQtd;
                                      result6M.Add(entry);
                                    }
                                  PeriodAttributionInfo = result6M;
                                  if (null != attributionDataLoadedEvent)
                                      attributionDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
                                break;
                            case "YTD":
                                 List<PeriodAttributeData> resultYTD = new List<PeriodAttributeData>();                                    
                                 for (int i = 0; i < AttributionDataInfo.Count; i++)
                                    {   PeriodAttributeData entry = new PeriodAttributeData();
                                    entry.Country = AttributionDataInfo[i].Country;
                                    entry.CountryName = AttributionDataInfo[i].CountryName;
                                    entry.BenchmarkWeight = AttributionDataInfo[i].Bm1RcAvgWgtYtd;
                                      entry.PortfolioWeight = AttributionDataInfo[i].PorRcAvgWgtYtd;
                                      entry.PortfolioReturn = AttributionDataInfo[i].FPorAshRcCtnYtd;
                                      entry.BenchmarkReturn = AttributionDataInfo[i].FBm1AshRcCtnYtd;
                                      entry.AssetAllocation = AttributionDataInfo[i].FBm1AshAssetAllocYtd;
                                      entry.StockSelectionTotal = AttributionDataInfo[i].FBm1AshSecSelecYtd;
                                      resultYTD.Add(entry);
                                    }
                                 PeriodAttributionInfo = resultYTD;
                                 if (null != attributionDataLoadedEvent)
                                     attributionDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
                                break;
                            case "1Y":
                                List<PeriodAttributeData> result1Y = new List<PeriodAttributeData>();                                    
                                 for (int i = 0; i < AttributionDataInfo.Count; i++)
                                    {   PeriodAttributeData entry = new PeriodAttributeData();
                                    entry.Country = AttributionDataInfo[i].Country;
                                    entry.CountryName = AttributionDataInfo[i].CountryName;
                                    entry.BenchmarkWeight = AttributionDataInfo[i].Bm1RcAvgWgt1y;
                                      entry.PortfolioWeight = AttributionDataInfo[i].PorRcAvgWgt1y;
                                      entry.PortfolioReturn = AttributionDataInfo[i].FPorAshRcCtn1y;
                                      entry.BenchmarkReturn = AttributionDataInfo[i].FBm1AshRcCtn1y;
                                      entry.AssetAllocation = AttributionDataInfo[i].FBm1AshAssetAlloc1y;
                                      entry.StockSelectionTotal = AttributionDataInfo[i].FBm1AshSecSelec1y;
                                      result1Y.Add(entry);
                                    }
                                 PeriodAttributionInfo = result1Y;
                                 if (null != attributionDataLoadedEvent)
                                     attributionDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
                                break;
                            //case "3Y":
                            //    List<PeriodAttributeData> result3Y = new List<PeriodAttributeData>();                                    
                            //     for (int i = 0; i < AttributionDataInfo.Count; i++)
                            //        {   PeriodAttributeData entry = new PeriodAttributeData();
                            //        entry.Country = AttributionDataInfo[i].Country;
                            //        entry.CountryName = AttributionDataInfo[i].CountryName;
                            //        entry.BenchmarkWeight = AttributionDataInfo[i].Bm1RcAvgWgt3y;
                            //          entry.PortfolioWeight = AttributionDataInfo[i].PorRcAvgWgt3y;
                            //          entry.PortfolioReturn = AttributionDataInfo[i].FPorAshRcCtn3y;
                            //          entry.BenchmarkReturn = AttributionDataInfo[i].FBm1AshRcCtn3y;
                            //          entry.AssetAllocation = AttributionDataInfo[i].FBm1AshAssetAlloc3y;
                            //          entry.StockSelectionTotal = AttributionDataInfo[i].FBm1AshSecSelec3y;
                            //          result3Y.Add(entry);
                            //        }
                            //     PeriodAttributionInfo = result3Y;
                            //     if (null != attributionDataLoadedEvent)
                            //         attributionDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
                            //    break;
                            //case "5Y":
                            //      List<PeriodAttributeData> result5Y = new List<PeriodAttributeData>();                                    
                            //     for (int i = 0; i < AttributionDataInfo.Count; i++)
                            //        {   PeriodAttributeData entry = new PeriodAttributeData();
                            //        entry.Country = AttributionDataInfo[i].Country;
                            //        entry.CountryName = AttributionDataInfo[i].CountryName;
                            //        entry.BenchmarkWeight = AttributionDataInfo[i].PorRcAvgWgt5y;
                            //        entry.PortfolioWeight = AttributionDataInfo[i].Bm1RcAvgWgt5y;
                            //        entry.PortfolioReturn = AttributionDataInfo[i].FPorAshRcCtn5y;
                            //        entry.BenchmarkReturn = AttributionDataInfo[i].FBm1AshRcCtn5y;
                            //        entry.AssetAllocation = AttributionDataInfo[i].FBm1AshAssetAlloc5y;
                            //        entry.StockSelectionTotal = AttributionDataInfo[i].FBm1AshSecSelec5y;
                            //          result5Y.Add(entry);
                            //        }
                            //     PeriodAttributionInfo = result5Y;
                            //     if (null != attributionDataLoadedEvent)
                            //         attributionDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
                            //    break;
                            //case "SI":
                            //    List<PeriodAttributeData> resultSI = new List<PeriodAttributeData>();                                    
                            //     for (int i = 0; i < AttributionDataInfo.Count; i++)
                            //        {   PeriodAttributeData entry = new PeriodAttributeData();
                            //        entry.Country = AttributionDataInfo[i].Country;
                            //        entry.CountryName = AttributionDataInfo[i].CountryName;
                            //        entry.BenchmarkWeight = AttributionDataInfo[i].Bm1RcAvgWgtSi;
                            //          entry.PortfolioWeight = AttributionDataInfo[i].PorRcAvgWgtSi;
                            //          entry.PortfolioReturn = AttributionDataInfo[i].FPorAshRcCtnSi;
                            //          entry.BenchmarkReturn = AttributionDataInfo[i].FBm1AshRcCtnSi;
                            //          entry.AssetAllocation = AttributionDataInfo[i].FBm1AshAssetAllocSi;
                            //          entry.StockSelectionTotal = AttributionDataInfo[i].FBm1AshSecSelecSi;
                            //          resultSI.Add(entry);
                            //        }
                            //     PeriodAttributionInfo = resultSI;
                            //     if (null != attributionDataLoadedEvent)
                            //         attributionDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
                            //    break;
                            default:
                                List<PeriodAttributeData> result10Y = new List<PeriodAttributeData>();
                                PeriodAttributionInfo = result10Y;
                                 if (null != attributionDataLoadedEvent)
                                     attributionDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
                                break;
                        }

                    }
                
            } 
       

        }
        /// <summary>
        /// Collection binded to the Attribution Grid
        /// </summary>
        private List<PeriodAttributeData> _periodAttributionInfo;
        public List<PeriodAttributeData> PeriodAttributionInfo
        {
            get
            {
                if (_periodAttributionInfo == null)
                    _periodAttributionInfo = new List<PeriodAttributeData>();
                return _periodAttributionInfo;
            }
            set
            {
                if (_periodAttributionInfo != value)
                {
                    _periodAttributionInfo = value;  
                    RaisePropertyChanged(() => this.PeriodAttributionInfo);
                }
            }
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
        /// <param name="PortfolioSelectionData">Object of PortfolioSelectionData class containg the Fund Selection Data </param>
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
                    if (PortfolioSelectionData != null && _effectiveDate != null)
                    {
                        if (null != attributionDataLoadedEvent)
                            attributionDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
                        _dbInteractivity.RetrieveAttributionData(_PortfolioSelectionData, Convert.ToDateTime(_effectiveDate), RetrieveAttributionDataCallBackMethod);
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
        /// <param name="effectiveDate">Effective Date selected by the user</param>

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
                    if (_PortfolioSelectionData != null && _effectiveDate != null)
                    {
                        if (null != attributionDataLoadedEvent)
                            attributionDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
                        _dbInteractivity.RetrieveAttributionData(_PortfolioSelectionData, Convert.ToDateTime(_effectiveDate), RetrieveAttributionDataCallBackMethod);
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
                    if (null != attributionDataLoadedEvent)
                        attributionDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
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

        #region CallbackMethods
        /// <summary>
        /// Plots the grid after getting the resulting collection
        /// </summary>
        /// <param name="result">Contains the Attribution Data</param>
        private void RetrieveAttributionDataCallBackMethod(List<AttributionData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null && result.Count > 0)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    AttributionDataInfo = result;
                    if (null != attributionDataLoadedEvent)
                        attributionDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
                }
                else
                {
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                    if (null != attributionDataLoadedEvent)
                        attributionDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
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
            _eventAggregator.GetEvent<PeriodReferenceSetEvent>().Unsubscribe(HandlePeriodReferenceSet);
        }

        #endregion

    }
}
