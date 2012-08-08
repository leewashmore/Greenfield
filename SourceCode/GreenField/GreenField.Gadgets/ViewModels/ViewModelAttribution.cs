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
        /// <summary>
        /// stores Node Name Filter selected by the user 
        /// </summary>
        private String _nodeName;

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
            _nodeName = param.DashboardGadgetPayload.NodeNameSelectionData;
            _PortfolioSelectionData = param.DashboardGadgetPayload.PortfolioSelectionData;
            if (_effectiveDate != null && _PortfolioSelectionData != null && _selectedPeriod!=null && IsActive && _nodeName!=null)
            {
                _dbInteractivity.RetrieveAttributionData(_PortfolioSelectionData, Convert.ToDateTime(_effectiveDate),_nodeName, RetrieveAttributionDataCallBackMethod);
            }
            if (_eventAggregator != null)
            {
                _eventAggregator.GetEvent<PortfolioReferenceSetEvent>().Subscribe(HandleFundReferenceSet, false);
                _eventAggregator.GetEvent<EffectiveDateReferenceSetEvent>().Subscribe(HandleEffectiveDateSet, false);
                _eventAggregator.GetEvent<PeriodReferenceSetEvent>().Subscribe(HandlePeriodReferenceSet, false);
                _eventAggregator.GetEvent<NodeNameReferenceSetEvent>().Subscribe(HandleNodeNameReferenceSet, false);

            }          
        }

        #endregion

        #region Properties
        #region UI Fields

        /// <summary>
        /// Collection that contains all the data for propertyName particular portfolio and date
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
                            case "1D":
                                List<PeriodAttributeData> resultd = new List<PeriodAttributeData>();
                                for (int i = 0; i < AttributionDataInfo.Count; i++)
                                {
                                    if (AttributionDataInfo[i].PorRcAvgWgt1d == 0 && AttributionDataInfo[i].Bm1RcAvgWgt1d == 0)
                                        continue;
                                    if (AttributionDataInfo[i].PorRcAvgWgt1d == null)
                                        AttributionDataInfo[i].PorRcAvgWgt1d = 0;
                                    if (AttributionDataInfo[i].Bm1RcAvgWgt1d == null)
                                        AttributionDataInfo[i].Bm1RcAvgWgt1d = 0;
                                    PeriodAttributeData entry = new PeriodAttributeData();
                                    entry.Country = AttributionDataInfo[i].Country;
                                    entry.CountryName = AttributionDataInfo[i].CountryName;
                                    entry.BenchmarkWeight = AttributionDataInfo[i].Bm1RcAvgWgt1d;
                                    entry.PortfolioWeight = AttributionDataInfo[i].PorRcAvgWgt1d;
                                    entry.PortfolioReturn = AttributionDataInfo[i].FPorAshRcCtn1d;
                                    entry.BenchmarkReturn = AttributionDataInfo[i].FBm1AshRcCtn1d;
                                    entry.AssetAllocation = AttributionDataInfo[i].FBm1AshAssetAlloc1d;
                                    entry.StockSelectionTotal = AttributionDataInfo[i].FBm1AshSecSelec1d;
                                    resultd.Add(entry);
                                }
                                PeriodAttributionInfo = resultd;
                                if (null != attributionDataLoadedEvent)
                                    attributionDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
                                break;
                            case "1W":                                
                                List<PeriodAttributeData> result = new List<PeriodAttributeData>();                                    
                                for (int i = 0; i < AttributionDataInfo.Count; i++)
                                {
                                    if (AttributionDataInfo[i].PorRcAvgWgt1w == 0 && AttributionDataInfo[i].Bm1RcAvgWgt1w == 0)
                                        continue;
                                    if (AttributionDataInfo[i].PorRcAvgWgt1w == null)
                                        AttributionDataInfo[i].PorRcAvgWgt1w = 0;
                                    if (AttributionDataInfo[i].Bm1RcAvgWgt1w == null)
                                        AttributionDataInfo[i].Bm1RcAvgWgt1w = 0;
                                    PeriodAttributeData entry = new PeriodAttributeData();
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
                                 List<PeriodAttributeData> resultMtd = new List<PeriodAttributeData>();                                    
                                 for (int i = 0; i < AttributionDataInfo.Count; i++)
                                 {
                                     if (AttributionDataInfo[i].PorRcAvgWgtMtd == 0 && AttributionDataInfo[i].Bm1RcAvgWgtMtd == 0)
                                         continue;
                                     if (AttributionDataInfo[i].PorRcAvgWgtMtd == null)
                                         AttributionDataInfo[i].PorRcAvgWgtMtd = 0;
                                     if (AttributionDataInfo[i].Bm1RcAvgWgtMtd == null)
                                         AttributionDataInfo[i].Bm1RcAvgWgtMtd = 0;
                                     PeriodAttributeData entry = new PeriodAttributeData();
                                    entry.Country = AttributionDataInfo[i].Country;
                                    entry.CountryName = AttributionDataInfo[i].CountryName;
                                    entry.BenchmarkWeight = AttributionDataInfo[i].Bm1RcAvgWgtMtd;
                                    entry.PortfolioWeight = AttributionDataInfo[i].PorRcAvgWgtMtd;
                                    entry.PortfolioReturn = AttributionDataInfo[i].FPorAshRcCtnMtd;
                                    entry.BenchmarkReturn = AttributionDataInfo[i].FBm1AshRcCtnMtd;
                                    entry.AssetAllocation = AttributionDataInfo[i].FBm1AshAssetAllocMtd;
                                    entry.StockSelectionTotal = AttributionDataInfo[i].FBm1AshSecSelecMtd;
                                    resultMtd.Add(entry);
                                    }
                                 PeriodAttributionInfo = resultMtd;
                                if (null != attributionDataLoadedEvent)
                                    attributionDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
                                break;
                            case "QTD":
                                List<PeriodAttributeData> resultQtd = new List<PeriodAttributeData>();                                    
                                 for (int i = 0; i < AttributionDataInfo.Count; i++)
                                 {
                                     if (AttributionDataInfo[i].PorRcAvgWgtQtd == 0 && AttributionDataInfo[i].Bm1RcAvgWgtQtd == 0)
                                         continue;
                                     if (AttributionDataInfo[i].PorRcAvgWgtQtd == null)
                                         AttributionDataInfo[i].PorRcAvgWgtQtd = 0;
                                     if (AttributionDataInfo[i].Bm1RcAvgWgtQtd == null)
                                         AttributionDataInfo[i].Bm1RcAvgWgtQtd = 0;
                                     PeriodAttributeData entry = new PeriodAttributeData();
                                    entry.Country = AttributionDataInfo[i].Country;
                                    entry.CountryName = AttributionDataInfo[i].CountryName;
                                    entry.BenchmarkWeight = AttributionDataInfo[i].Bm1RcAvgWgtQtd;
                                    entry.PortfolioWeight = AttributionDataInfo[i].PorRcAvgWgtQtd;
                                    entry.PortfolioReturn = AttributionDataInfo[i].FPorAshRcCtnQtd;
                                    entry.BenchmarkReturn = AttributionDataInfo[i].FBm1AshRcCtnQtd;
                                    entry.AssetAllocation = AttributionDataInfo[i].FBm1AshAssetAllocQtd;
                                    entry.StockSelectionTotal = AttributionDataInfo[i].FBm1AshSecSelecQtd;
                                    resultQtd.Add(entry);
                                    }
                                 PeriodAttributionInfo = resultQtd;
                                  if (null != attributionDataLoadedEvent)
                                      attributionDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
                                break;
                            case "YTD":
                                 List<PeriodAttributeData> resultYTD = new List<PeriodAttributeData>();                                    
                                 for (int i = 0; i < AttributionDataInfo.Count; i++)
                                    {
                                        if (AttributionDataInfo[i].PorRcAvgWgtYtd == 0 && AttributionDataInfo[i].Bm1RcAvgWgtYtd == 0)
                                            continue;
                                        if (AttributionDataInfo[i].PorRcAvgWgtYtd == null)
                                            AttributionDataInfo[i].PorRcAvgWgtYtd = 0;
                                        if (AttributionDataInfo[i].Bm1RcAvgWgtYtd == null)
                                            AttributionDataInfo[i].Bm1RcAvgWgtYtd = 0;
                                     PeriodAttributeData entry = new PeriodAttributeData();
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
                                    {
                                        if (AttributionDataInfo[i].PorRcAvgWgt1y == 0 && AttributionDataInfo[i].Bm1RcAvgWgt1y == 0)
                                            continue;
                                        if (AttributionDataInfo[i].PorRcAvgWgt1y == null)
                                            AttributionDataInfo[i].PorRcAvgWgt1y = 0;
                                        if (AttributionDataInfo[i].Bm1RcAvgWgt1y == null)
                                            AttributionDataInfo[i].Bm1RcAvgWgt1y = 0;
                                     PeriodAttributeData entry = new PeriodAttributeData();
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
                if (_PortfolioSelectionData != null && _effectiveDate != null && _selectedPeriod != null && _isActive && _nodeName!=null)
                {
                    BeginWebServiceCall(_PortfolioSelectionData, Convert.ToDateTime(_effectiveDate),_nodeName);                    
                }
            }
        }
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
                    if (PortfolioSelectionData != null && _effectiveDate != null && _selectedPeriod!=null && IsActive && _nodeName!=null)
                    {                        
                        BeginWebServiceCall(_PortfolioSelectionData, Convert.ToDateTime(_effectiveDate),_nodeName);
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
                    if (_PortfolioSelectionData != null && _effectiveDate != null && _selectedPeriod!=null && IsActive && _nodeName!=null)
                    {                        
                       BeginWebServiceCall(_PortfolioSelectionData, Convert.ToDateTime(_effectiveDate),_nodeName);
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
                    _selectedPeriod = selectedPeriodType;
                    if (_PortfolioSelectionData != null && _effectiveDate != null && _selectedPeriod != null && _nodeName!=null)
                    {
                        if (AttributionDataInfo.Count==0 && IsActive)
                        {                            
                            BeginWebServiceCall(_PortfolioSelectionData, Convert.ToDateTime(_effectiveDate),_nodeName); //SelectedPeriod = selectedPeriodType;
                        }

                        else 
                        {
                          if (null != attributionDataLoadedEvent)
                               attributionDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
                            SelectedPeriod = selectedPeriodType;
                        }
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
        /// Assigns UI Field Properties based on Node Name Selected
        /// </summary>
        /// <param name="selectedNodeType">Node Name selected by the user</param>
       public void HandleNodeNameReferenceSet(String selectedNodeType)
        {

            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (selectedNodeType != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, selectedNodeType, 1);
                    _nodeName = selectedNodeType;
                    if (_PortfolioSelectionData != null && _effectiveDate != null && _selectedPeriod != null && IsActive && _nodeName!=null)
                    {
                        BeginWebServiceCall(_PortfolioSelectionData, Convert.ToDateTime(_effectiveDate),_nodeName);
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


        private void BeginWebServiceCall(PortfolioSelectionData PortfolioSelectionData, DateTime effectiveDate,String nodeName)
        {
            if (null != attributionDataLoadedEvent)
                attributionDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
            _dbInteractivity.RetrieveAttributionData(PortfolioSelectionData, effectiveDate, nodeName, RetrieveAttributionDataCallBackMethod);
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
                    SelectedPeriod = _selectedPeriod;
                    if (null != attributionDataLoadedEvent)
                        attributionDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
                }
                else
                {
                    AttributionDataInfo = result;
                    SelectedPeriod = _selectedPeriod;
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
            _eventAggregator.GetEvent<NodeNameReferenceSetEvent>().Unsubscribe(HandleNodeNameReferenceSet);
        }

        #endregion

    }
}
