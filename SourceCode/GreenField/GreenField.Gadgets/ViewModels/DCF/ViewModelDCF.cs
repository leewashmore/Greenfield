﻿using System;
using System.Windows;
using System.Windows.Input;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Prism.Logging;
using GreenField.ServiceCaller;
using Microsoft.Practices.Prism.Events;
using GreenField.Common;
using GreenField.Gadgets.Helpers;
using System.Collections.Generic;
using Telerik.Windows.Controls.Charting;
using System.Collections.ObjectModel;
using GreenField.DataContracts;
using System.Linq;
using GreenField.Gadgets.Models;
using Greenfield.Gadgets.Helpers;
using GreenField.ServiceCaller.DCFDefinitions;
using Greenfield.Gadgets.Models;

namespace GreenField.Gadgets.ViewModels
{
    /// <summary>
    /// View-Model for DCF Gadgets
    /// </summary>
    public class ViewModelDCF : NotificationObject
    {
        #region PrivateVariables

        /// <summary>
        /// Event Aggregator
        /// </summary>
        private IEventAggregator _eventAggregator;

        /// <summary>
        /// Instance of Service Caller Class
        /// </summary>
        private IDBInteractivity _dbInteractivity;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="param"></param>
        public ViewModelDCF(DashboardGadgetParam param)
        {
            _eventAggregator = param.EventAggregator;
            _dbInteractivity = param.DBInteractivity;
            _logger = param.LoggerFacade;

            EntitySelectionData = param.DashboardGadgetPayload.EntitySelectionData;
            if (_eventAggregator != null)
            {
                _eventAggregator.GetEvent<SecurityReferenceSetEvent>().Subscribe(HandleSecurityReferenceSetEvent);
            }

            if (EntitySelectionData != null)
            {
                HandleSecurityReferenceSetEvent(EntitySelectionData);
            }
        }

        #endregion

        #region PropertyDeclaration

        #region SelectedSecurity

        /// <summary>
        /// Selected Security
        /// </summary>
        private EntitySelectionData _entitySelectionData;
        public EntitySelectionData EntitySelectionData
        {
            get
            {
                return _entitySelectionData;
            }
            set
            {
                _entitySelectionData = value;
                this.RaisePropertyChanged(() => this.EntitySelectionData);
            }
        }              

        #endregion

        #region Dashboard

        /// <summary>
        /// Bool to check whether the Current Dashboard is Selected or Not
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
                    if (value)
                    {
                        if (EntitySelectionData != null)
                        {
                            _dbInteractivity.RetrieveDCFTerminalValueCalculationsData(EntitySelectionData, RetrieveDCFTerminalValueCalculationsDataCallbackMethod);
                            _dbInteractivity.RetrieveCashFlows(EntitySelectionData, RetrieveDCFCashFlowYearlyDataCallbackMethod);
                            _dbInteractivity.RetrieveDCFAnalysisData(EntitySelectionData, RetrieveDCFAnalysisDataCallbackMethod);

                            _dbInteractivity.RetrieveDCFCurrentPrice(EntitySelectionData, RetrieveCurrentPriceDataCallbackMethod);
                            _dbInteractivity.RetrieveDCFSummaryData(EntitySelectionData, RetrieveDCFSummaryDataCallbackMethod);

                            BusyIndicatorNotification(true, "Fetching Data for Selected Security");
                        }
                    }
                    this.RaisePropertyChanged(() => this.IsActive);
                }
            }
        }

        #endregion

        #region Busy Indicator

        /// <summary>
        /// Busy Indicator Status
        /// </summary>
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

        /// <summary>
        /// Busy Indicator Content
        /// </summary>
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

        #region DataGrid

        #region TerminalValueCalculations

        /// <summary>
        /// List of Type TerminalValueCalculationsData
        /// </summary>
        private List<DCFTerminalValueCalculationsData> _terminalValueCalculationsData;
        public List<DCFTerminalValueCalculationsData> TerminalValueCalculationsData
        {
            get
            {
                if (_terminalValueCalculationsData == null)
                    _terminalValueCalculationsData = new List<DCFTerminalValueCalculationsData>();
                return _terminalValueCalculationsData;
            }
            set
            {
                _terminalValueCalculationsData = value;
                this.RaisePropertyChanged(() => this.TerminalValueCalculationsData);
            }
        }

        /// <summary>
        /// List of type TerminalValueCalculationsDisplayData to show in the Grid
        /// </summary>
        private RangeObservableCollection<DCFDisplayData> _terminalValueCalculationsDisplayData;
        public RangeObservableCollection<DCFDisplayData> TerminalValueCalculationsDisplayData
        {
            get
            {
                if (_terminalValueCalculationsDisplayData == null)
                {
                    _terminalValueCalculationsDisplayData = new RangeObservableCollection<DCFDisplayData>();
                }
                return _terminalValueCalculationsDisplayData;
            }
            set
            {
                _terminalValueCalculationsDisplayData = value;
                this.RaisePropertyChanged(() => this.TerminalValueCalculationsDisplayData);
            }
        }

        /// <summary>
        /// FreeCashFlow for Year9
        /// </summary>
        private decimal _freeCashFlowY9;
        public decimal FreeCashFlowY9
        {
            get
            {
                return _freeCashFlowY9;
            }
            set
            {
                _freeCashFlowY9 = value;
            }
        }

        #endregion

        #region Analysis Summary

        /// <summary>
        /// Collection of type DCFAnalysisSummaryData bound to the Data-Grid
        /// </summary>
        private RangeObservableCollection<DCFAnalysisSummaryData> _analysisSummaryData;
        public RangeObservableCollection<DCFAnalysisSummaryData> AnalysisSummaryData
        {
            get
            {
                if (_analysisSummaryData == null)
                    _analysisSummaryData = new RangeObservableCollection<DCFAnalysisSummaryData>();
                return _analysisSummaryData;
            }
            set
            {
                _analysisSummaryData = value;
                this.RaisePropertyChanged(() => this.AnalysisSummaryData);
            }
        }

        /// <summary>
        /// Default Display Data
        /// </summary>
        private RangeObservableCollection<DCFDisplayData> _analysisSummaryDisplayData;
        public RangeObservableCollection<DCFDisplayData> AnalysisSummaryDisplayData
        {
            get
            {
                if (_analysisSummaryDisplayData == null)
                    _analysisSummaryDisplayData = SetDefaultAnalysisDisplayData();
                return _analysisSummaryDisplayData;
            }
            set
            {
                this._analysisSummaryDisplayData = value;
                this.RaisePropertyChanged(() => this.AnalysisSummaryDisplayData);
            }
        }

        /// <summary>
        /// Stock Specific Discount
        /// </summary>
        private decimal? _stockSpecificDiscount = 0;
        public decimal? StockSpecificDiscount
        {
            get
            {
                return _stockSpecificDiscount;
            }
            set
            {
                _stockSpecificDiscount = value;
                if (AnalysisSummaryData.Count == 0)
                {
                    if (AnalysisSummaryDisplayData.Where(a => a.PropertyName == "Stock Specific Discount").FirstOrDefault() != null)
                        AnalysisSummaryDisplayData.Where(a => a.PropertyName == "Stock Specific Discount").FirstOrDefault().Value = Convert.ToString(value) + "%";
                    this.RaisePropertyChanged(() => this.AnalysisSummaryDisplayData);
                }
                else
                    SetAnalysisSummaryDisplayData();
                this.RaisePropertyChanged(() => this.StockSpecificDiscount);
            }
        }


        #endregion

        #region DCFSummary

        /// <summary>
        /// Data Returned from Service
        /// </summary>
        private List<DCFSummaryData> _summaryData;
        public List<DCFSummaryData> SummaryData
        {
            get
            {
                if (_summaryData == null)
                {
                    _summaryData = new List<DCFSummaryData>();
                }
                return _summaryData;
            }
            set
            {
                _summaryData = value;
                SetSummaryDisplayData();
                this.RaisePropertyChanged(() => this.SummaryData);
            }
        }

        /// <summary>
        /// Summary Display Data
        /// </summary>
        private RangeObservableCollection<DCFDisplayData> _summaryDisplayData;
        public RangeObservableCollection<DCFDisplayData> SummaryDisplayData
        {
            get
            {
                if (_summaryDisplayData == null)
                {
                    _summaryDisplayData = new RangeObservableCollection<DCFDisplayData>();
                }
                return _summaryDisplayData;
            }
            set
            {
                _summaryDisplayData = value;
                this.RaisePropertyChanged(() => this.SummaryDisplayData);
            }
        }


        #endregion

        #region Sensitivity

        /// <summary>
        /// 
        /// </summary>
        private RangeObservableCollection<SensitivityData> _sensitivityDisplayData;
        public RangeObservableCollection<SensitivityData> SensitivityDisplayData
        {
            get
            {
                return _sensitivityDisplayData;
            }
            set
            {
                _sensitivityDisplayData = value;
                this.RaisePropertyChanged(() => this.SensitivityDisplayData);
            }
        }


        #endregion

        #endregion

        #region LoggerFacade

        /// <summary>
        /// Public property for LoggerFacade _logger
        /// </summary>
        private ILoggerFacade _logger;
        public ILoggerFacade Logger
        {
            get
            {
                return _logger;
            }
            set
            {
                _logger = value;
            }
        }

        #endregion

        #region Calculations

        /// <summary>
        /// TerminalGrowthRate from FreeCashFlows
        /// </summary>
        private decimal? _terminalGrowthRate;
        public decimal? TerminalGrowthRate
        {
            get
            {
                return _terminalGrowthRate;
            }
            set
            {
                _terminalGrowthRate = value;
                this.RaisePropertyChanged(() => this.TerminalGrowthRate);
            }
        }

        /// <summary>
        /// Yearly Calculated Data
        /// </summary>
        private List<DCFCashFlowData> _yearlyCalculatedData;
        public List<DCFCashFlowData> YearlyCalculatedData
        {
            get
            {
                if (_yearlyCalculatedData == null)
                {
                    _yearlyCalculatedData = new List<DCFCashFlowData>();
                }
                return _yearlyCalculatedData;
            }
            set
            {
                _yearlyCalculatedData = value;
                this.RaisePropertyChanged(() => this.YearlyCalculatedData);
            }
        }

        /// <summary>
        /// Value of WACC
        /// </summary>
        private decimal _WACC;
        public decimal WACC
        {
            get
            {
                return _WACC;
            }
            set
            {
                _WACC = value;
                SetTerminalValueCalculationsDisplayData();
                this.RaisePropertyChanged(() => this.WACC);
            }
        }

        #region Sensitivity

        private DCFCalculationParameters _calculationParameters;
        public DCFCalculationParameters CalculationParameters
        {
            get
            {
                if (_calculationParameters == null)
                    _calculationParameters = new DCFCalculationParameters();
                return _calculationParameters;
            }
            set
            {
                _calculationParameters = value;
                this.RaisePropertyChanged(() => this.CalculationParameters);
            }
        }

        #region Statistics

        /// <summary>
        /// Max Share Value
        /// </summary>
        private string _maxShareVal;
        public string MaxShareVal
        {
            get { return _maxShareVal; }
            set
            {
                _maxShareVal = value;
                this.RaisePropertyChanged(() => this.MaxShareVal);
            }
        }

        /// <summary>
        /// Min Share Value
        /// </summary>
        private string _minShareVal;
        public string MinShareVal
        {
            get { return _minShareVal; }
            set
            {
                _minShareVal = value;
                this.RaisePropertyChanged(() => this.MinShareVal);
            }
        }

        /// <summary>
        /// Max Share Value
        /// </summary>
        private string _avgShareVal;
        public string AvgShareVal
        {
            get { return _avgShareVal; }
            set
            {
                _avgShareVal = value;
                this.RaisePropertyChanged(() => this.AvgShareVal);
            }
        }

        /// <summary>
        /// Avg Share Value
        /// </summary>
        private string _maxUpside;
        public string MaxUpside
        {
            get { return _maxUpside; }
            set
            {
                _maxUpside = value;
                this.RaisePropertyChanged(() => this.MaxUpside);
            }
        }

        /// <summary>
        /// Avg Share Value
        /// </summary>
        private string _minUpside;
        public string MinUpside
        {
            get { return _minUpside; }
            set
            {
                _minUpside = value;
                this.RaisePropertyChanged(() => this.MinUpside);
            }
        }

        /// <summary>
        /// Avg Share Value
        /// </summary>
        private string _avgUpside;
        public string AvgUpside
        {
            get { return _avgUpside; }
            set
            {
                _avgUpside = value;
                this.RaisePropertyChanged(() => this.AvgUpside);
            }
        }




        #endregion


        #endregion


        /// <summary>
        /// TerminalValuePresent
        /// </summary>
        private decimal? _terminalValuePresent;
        public decimal? TerminalValuePresent
        {
            get
            {
                return _terminalValuePresent;
            }
            set
            {
                _terminalValuePresent = value;
                SetSummaryDisplayData();
                this.RaisePropertyChanged(() => this.TerminalValuePresent);
            }
        }

        /// <summary>
        /// TerminalValueNominal
        /// </summary>
        private decimal? _terminalValueNominal;
        public decimal? TerminalValueNominal
        {
            get
            {
                return _terminalValueNominal;
            }
            set
            {
                _terminalValueNominal = value;
                this.RaisePropertyChanged(() => this.TerminalValueNominal);
            }
        }

        #region Summary

        /// <summary>
        /// Current Market Price
        /// </summary>
        private decimal? _currentMarketPrice;
        public decimal? CurrentMarketPrice
        {
            get
            {
                return _currentMarketPrice;
            }
            set
            {
                _currentMarketPrice = value;
                this.RaisePropertyChanged(() => this.CurrentMarketPrice);
            }
        }

        /// <summary>
        /// Yearly Calculated Data
        /// </summary>
        private List<DCFCashFlowData> _yearlySummaryCalculatedData;
        public List<DCFCashFlowData> YearlySummaryCalculatedData
        {
            get
            {
                return _yearlyCalculatedData;
            }
            set
            {
                _yearlyCalculatedData = value;
                this.RaisePropertyChanged(() => this.YearlySummaryCalculatedData);
            }
        }

        private decimal _presentValueExplicitForecast;
        public decimal PresentValueExplicitForecast
        {
            get { return _presentValueExplicitForecast; }
            set
            {
                _presentValueExplicitForecast = value;
                this.RaisePropertyChanged(() => this.PresentValueExplicitForecast);
            }
        }



        #endregion


        #endregion

        #endregion

        #region EventHandlers

        /// <summary>
        /// Event Handler to subscribed event 'SecurityReferenceSet'
        /// </summary>
        /// <param name="securityReferenceData">SecurityReferenceData</param>
        public void HandleSecurityReferenceSetEvent(EntitySelectionData entitySelectionData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                //ArgumentNullException
                if (entitySelectionData != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, entitySelectionData, 1);
                    EntitySelectionData = entitySelectionData;
                    if (IsActive)
                    {
                        if (IsActive && EntitySelectionData != null)
                        {
                            //_dbInteractivity.RetrieveDCFTerminalValueCalculationsData(EntitySelectionData, RetrieveDCFTerminalValueCalculationsDataCallbackMethod);
                            //_dbInteractivity.RetrieveCashFlows(EntitySelectionData, RetrieveDCFCashFlowYearlyDataCallbackMethod);
                            _dbInteractivity.RetrieveDCFAnalysisData(EntitySelectionData, RetrieveDCFAnalysisDataCallbackMethod);

                            //_dbInteractivity.RetrieveDCFCurrentPrice(entitySelectionData, RetrieveCurrentPriceDataCallbackMethod);
                            //_dbInteractivity.RetrieveDCFSummaryData(entitySelectionData, RetrieveDCFSummaryDataCallbackMethod);

                            BusyIndicatorNotification(true, "Fetching Data for Selected Security");
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
        }

        #endregion

        #region CallbackMethods

        /// <summary>
        /// Consensus Estimate Data callback Method
        /// </summary>
        /// <param name="result"></param>
        public void RetrieveDCFTerminalValueCalculationsDataCallbackMethod(List<DCFTerminalValueCalculationsData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    TerminalValueCalculationsData = result;
                    SetTerminalValueCalculationsDisplayData();
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
                BusyIndicatorNotification();
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        /// <summary>
        /// Consensus Estimate Data callback Method
        /// </summary>
        /// <param name="result"></param>
        public void RetrieveDCFCashFlowYearlyDataCallbackMethod(List<DCFCashFlowData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    YearlyCalculatedData = result;
                    if (WACC != null)
                    {
                        YearlyCalculatedData = CalculateYearlyData(YearlyCalculatedData, WACC);
                    }
                    SetTerminalValueCalculationsDisplayData();
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
                BusyIndicatorNotification();
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        /// <summary>
        /// Consensus Estimate Data callback Method
        /// </summary>
        /// <param name="result"></param>
        public void RetrieveDCFAnalysisDataCallbackMethod(List<DCFAnalysisSummaryData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    AnalysisSummaryData.Clear();
                    AnalysisSummaryData.AddRange(result);
                    SetAnalysisSummaryDisplayData();
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
                BusyIndicatorNotification();
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        /// <summary>
        /// Consensus Estimate Data callback Method
        /// </summary>
        /// <param name="result"></param>
        public void RetrieveDCFSummaryDataCallbackMethod(List<DCFSummaryData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    SummaryData = result;
                    SetSummaryDisplayData();
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
                BusyIndicatorNotification();
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        /// <summary>
        /// Consensus Estimate Data callback Method
        /// </summary>
        /// <param name="result"></param>
        public void RetrieveCurrentPriceDataCallbackMethod(decimal? result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    CurrentMarketPrice = result;
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
                BusyIndicatorNotification();
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        #endregion

        #region HelperMethods

        /// <summary>
        /// Busy Indicator Notification
        /// </summary>
        /// <param name="showBusyIndicator">Busy Indicator Running/Stopped: True/False</param>
        /// <param name="message">Message in Busy Indicator</param>
        public void BusyIndicatorNotification(bool showBusyIndicator = false, String message = null)
        {
            if (message != null)
                BusyIndicatorContent = message;
            BusyIndicatorIsBusy = showBusyIndicator;
        }

        /// <summary>
        /// Convert Data to Pivotted Form
        /// </summary>
        /// <param name="data">Set the Display Data</param>
        public void SetTerminalValueCalculationsDisplayData()
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                List<DCFDisplayData> result = new List<DCFDisplayData>();

                decimal cashFlow2020 = Math.Round(Convert.ToDecimal(YearlyCalculatedData.Where(a => a.PERIOD_YEAR == (DateTime.Today.AddYears(8).Year)).
                    Select(a => a.AMOUNT).FirstOrDefault()), 4);
                decimal sustainableROIC = Math.Round(Convert.ToDecimal(TerminalValueCalculationsData.Select(a => a.SustainableROIC).FirstOrDefault()), 4);
                decimal sustainableDPR = Math.Round(Convert.ToDecimal(TerminalValueCalculationsData.Select(a => a.SustainableDividendPayoutRatio).FirstOrDefault()), 4);
                decimal longTermNominalGDPGrowth = Math.Round(Convert.ToDecimal(TerminalValueCalculationsData.Select(a => a.LongTermNominalGDPGrowth).FirstOrDefault()), 4);
                decimal TGR;

                decimal discountingFactorY10 = Math.Round(Convert.ToDecimal(YearlyCalculatedData.Where(a => a.PERIOD_YEAR == (DateTime.Today.AddYears(9).Year)).
                    Select(a => a.DISCOUNTING_FACTOR).FirstOrDefault()), 4);
                TGR = Math.Round((Math.Min(sustainableROIC * (1 - sustainableDPR / 100), longTermNominalGDPGrowth / 100) * 100), 4);
                decimal terminalValueNominal = Math.Round(Convert.ToDecimal(DCFCalculations.CalculateNominalTerminalValue(WACC, TGR, cashFlow2020)), 4);
                decimal terminalValuePresent = Math.Round(Convert.ToDecimal(DCFCalculations.CalculatePresentTerminalValue(terminalValueNominal, discountingFactorY10)), 4);

                result.Add(new DCFDisplayData() { PropertyName = "Cash Flow in 2020", Value = cashFlow2020.ToString() });
                result.Add(new DCFDisplayData() { PropertyName = "Sustainable ROIC", Value = sustainableROIC.ToString() });
                result.Add(new DCFDisplayData() { PropertyName = "Sustainable Dividend Payout Ratio", Value = sustainableDPR.ToString() });
                result.Add(new DCFDisplayData() { PropertyName = "Long-term Nominal GDP Growth", Value = longTermNominalGDPGrowth.ToString() });
                result.Add(new DCFDisplayData() { PropertyName = "Terminal Growth Rate", Value = TGR.ToString() });
                result.Add(new DCFDisplayData() { PropertyName = "Terminal Value (nominal)", Value = terminalValueNominal.ToString() });
                result.Add(new DCFDisplayData() { PropertyName = "Terminal Value (nominal)", Value = terminalValuePresent.ToString() });

                TerminalValueCalculationsDisplayData.Clear();
                TerminalValueCalculationsDisplayData.AddRange(result);
                TerminalValuePresent = terminalValuePresent;

                CalculationParameters.Year10CashFlow = (Convert.ToDecimal(YearlyCalculatedData.Where(a => a.PERIOD_YEAR == (DateTime.Today.AddYears(9).Year)).
                    Select(a => a.AMOUNT).FirstOrDefault()));
                CalculationParameters.TerminalGrowthRate = TGR;
                CalculationParameters.Year10DiscountingFactor = (Convert.ToDecimal(YearlyCalculatedData.Where(a => a.PERIOD_YEAR == (DateTime.Today.AddYears(9).Year)).
                    Select(a => a.DISCOUNTING_FACTOR).FirstOrDefault()));
                CalculationParameters.CurrentMarketPrice = Convert.ToDecimal(CurrentMarketPrice);
                if (EntitySelectionData != null)
                {
                    _dbInteractivity.RetrieveDCFCurrentPrice(EntitySelectionData, RetrieveCurrentPriceDataCallbackMethod);
                    _dbInteractivity.RetrieveDCFSummaryData(EntitySelectionData, RetrieveDCFSummaryDataCallbackMethod);
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
        }

        /// <summary>
        /// Calculate Discounting Factor/PresentValues for 10 year period
        /// </summary>
        public List<DCFCashFlowData> CalculateYearlyData(List<DCFCashFlowData> periodData, decimal WACC)
        {
            try
            {
                DateTime currentDate = DateTime.Today;
                List<DateTime> endDates = new List<DateTime>();
                foreach (DCFCashFlowData item in periodData)
                {
                    DateTime endDate = new DateTime(item.PERIOD_YEAR, 12, 31);
                    TimeSpan timeSpan = endDate - currentDate;
                    item.DISCOUNTING_FACTOR = Convert.ToDecimal(Math.Pow(Convert.ToDouble(1 + WACC), Convert.ToDouble(timeSpan.Days / 365)));
                    if (item.DISCOUNTING_FACTOR != 0)
                    {
                        item.AMOUNT = item.FREE_CASH_FLOW / item.DISCOUNTING_FACTOR;
                    }
                    else
                    {
                        item.AMOUNT = 0;
                    }
                }
                PresentValueExplicitForecast = periodData.Select(a => Convert.ToDecimal(a.AMOUNT)).Sum();
                return periodData;
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
                return null;
            }
        }

        /// <summary>
        /// Convert Data to Pivotted Form
        /// </summary>
        /// <param name="data"></param>
        public void SetAnalysisSummaryDisplayData()
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                RangeObservableCollection<DCFDisplayData> result = new RangeObservableCollection<DCFDisplayData>();

                decimal costOfEquity;
                decimal weightOfEquity;
                decimal costOfDebt;
                decimal resultWACC;

                result.Add(new DCFDisplayData() { PropertyName = "Market Risk Premium", Value = Convert.ToString(Math.Round((Convert.ToDecimal(AnalysisSummaryData.Select(a => a.MarketRiskPremium).FirstOrDefault())), 4)) + "%" });
                result.Add(new DCFDisplayData() { PropertyName = "Beta (*)", Value = Convert.ToString(Math.Round(Convert.ToDecimal(AnalysisSummaryData.Select(a => a.Beta).FirstOrDefault()), 4)) + "%" });
                result.Add(new DCFDisplayData() { PropertyName = "Risk Free Rate", Value = Convert.ToString(Math.Round(Convert.ToDecimal(AnalysisSummaryData.Select(a => a.RiskFreeRate).FirstOrDefault()), 4)) });
                result.Add(new DCFDisplayData()
                {
                    PropertyName = "Stock Specific Discount",
                    Value = Convert.ToString(Math.Round(Convert.ToDecimal(StockSpecificDiscount), 4)) + "%"
                });

                result.Add(new DCFDisplayData() { PropertyName = "Marginal Tax Rate", Value = Convert.ToString(Math.Round(Convert.ToDecimal(AnalysisSummaryData.Select(a => a.MarketRiskPremium).FirstOrDefault()), 4)) + "%" });


                costOfEquity = Convert.ToDecimal(AnalysisSummaryData.Select(a => a.Beta).FirstOrDefault()) * Convert.ToDecimal(AnalysisSummaryData.Select(a => a.MarketRiskPremium).FirstOrDefault()) + Convert.ToDecimal(AnalysisSummaryData.Select(a => a.RiskFreeRate).FirstOrDefault()) + Convert.ToDecimal(StockSpecificDiscount);
                result.Add(new DCFDisplayData()
                {
                    PropertyName = "Cost of Equity",
                    Value = Convert.ToString(Math.Round(Convert.ToDecimal(costOfEquity), 4)) + "%"
                });

                costOfDebt = Convert.ToDecimal(Math.Round(Convert.ToDecimal(AnalysisSummaryData.Select(a => a.MarketRiskPremium).FirstOrDefault()), 4));
                result.Add(new DCFDisplayData() { PropertyName = "Cost of Debt", Value = Convert.ToString(Math.Round(Convert.ToDecimal(costOfDebt), 4)) + "%" });


                result.Add(new DCFDisplayData() { PropertyName = "Market Cap", Value = Convert.ToString(Math.Round(Convert.ToDecimal(AnalysisSummaryData.Select(a => a.MarketCap).FirstOrDefault()), 4)) });
                result.Add(new DCFDisplayData() { PropertyName = "Gross Debt", Value = Convert.ToString(Math.Round(Convert.ToDecimal(AnalysisSummaryData.Select(a => a.MarketRiskPremium).FirstOrDefault()), 4)) });
                if ((Convert.ToDecimal(Math.Round(Convert.ToDecimal(AnalysisSummaryData.Select(a => a.MarketCap).FirstOrDefault()) + Convert.ToDecimal(AnalysisSummaryData.Select(a => a.GrossDebt).FirstOrDefault()), 4)) == 0))
                {
                    weightOfEquity = 0;
                    resultWACC = 0;
                }
                else
                {
                    weightOfEquity = Convert.ToDecimal(AnalysisSummaryData.Select(a => a.MarketCap).FirstOrDefault()) / (Convert.ToDecimal(AnalysisSummaryData.Select(a => a.MarketCap).FirstOrDefault()) + Convert.ToDecimal(AnalysisSummaryData.Select(a => a.GrossDebt).FirstOrDefault()));
                    resultWACC = (weightOfEquity * costOfEquity) + ((1 - weightOfEquity) * (costOfDebt * (1 - Convert.ToDecimal(AnalysisSummaryData.Select(a => a.MarginalTaxRate).FirstOrDefault()))));
                }
                result.Add(new DCFDisplayData() { PropertyName = "Weight of Equity", Value = Convert.ToString(Math.Round(Convert.ToDecimal(weightOfEquity), 4)) + "%" });
                result.Add(new DCFDisplayData() { PropertyName = "WACC", Value = Convert.ToString(Math.Round(Convert.ToDecimal(resultWACC), 4)) + "%" });

                AnalysisSummaryDisplayData = result;
                this.RaisePropertyChanged(() => this.AnalysisSummaryDisplayData);
                this.WACC = resultWACC;

                CalculationParameters.CostOfEquity = costOfEquity;
                CalculationParameters.CostOfDebt = costOfDebt;
                CalculationParameters.MarketCap = Convert.ToDecimal(AnalysisSummaryData.Select(a => a.MarketCap).FirstOrDefault());
                CalculationParameters.MarginalTaxRate = Convert.ToDecimal(AnalysisSummaryData.Select(a => a.MarketRiskPremium).FirstOrDefault());
                CalculationParameters.GrossDebt = Convert.ToDecimal(AnalysisSummaryData.Select(a => a.MarketRiskPremium).FirstOrDefault());

                if (EntitySelectionData != null)
                {
                    _dbInteractivity.RetrieveDCFTerminalValueCalculationsData(EntitySelectionData, RetrieveDCFTerminalValueCalculationsDataCallbackMethod);
                    _dbInteractivity.RetrieveCashFlows(EntitySelectionData, RetrieveDCFCashFlowYearlyDataCallbackMethod);
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
        }

        /// <summary>
        /// Set Default Display Data
        /// </summary>
        /// <returns></returns>
        private RangeObservableCollection<DCFDisplayData> SetDefaultAnalysisDisplayData()
        {
            RangeObservableCollection<DCFDisplayData> result = new RangeObservableCollection<DCFDisplayData>();
            result.Add(new DCFDisplayData() { PropertyName = "Market Risk Premium" });
            result.Add(new DCFDisplayData() { PropertyName = "Beta (*)" });
            result.Add(new DCFDisplayData() { PropertyName = "Risk Free Rate" });
            result.Add(new DCFDisplayData() { PropertyName = "Stock Specific Discount" });
            result.Add(new DCFDisplayData() { PropertyName = "Marginal Tax Rate" });
            result.Add(new DCFDisplayData() { PropertyName = "Cost of Equity" });
            result.Add(new DCFDisplayData() { PropertyName = "Cost of Debt" });
            result.Add(new DCFDisplayData() { PropertyName = "Market Cap" });
            result.Add(new DCFDisplayData() { PropertyName = "Gross Debt" });
            result.Add(new DCFDisplayData() { PropertyName = "Weight of Equity" });
            result.Add(new DCFDisplayData() { PropertyName = "WACC" });
            return result;
        }

        /// <summary>
        /// Convert Data to Pivotted Form
        /// </summary>
        /// <param name="data">Set the Display Data</param>
        public void SetSummaryDisplayData()
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                List<DCFDisplayData> result = new List<DCFDisplayData>();
                decimal DCFValuePerShare;
                decimal totalEnterpriseValue = DCFCalculations.CalculateTotalEnterpriseValue(PresentValueExplicitForecast, Convert.ToDecimal(TerminalValuePresent));
                decimal cash = SummaryData.Select(a => a.Cash).FirstOrDefault();
                decimal FVInvestments = SummaryData.Select(a => a.FVInvestments).FirstOrDefault();
                decimal grossDebt = SummaryData.Select(a => a.GrossDebt).FirstOrDefault();
                decimal FVMinorities = SummaryData.Select(a => a.FVMinorities).FirstOrDefault();
                decimal equityValue = DCFCalculations.CalculateEquityValue(totalEnterpriseValue, cash, FVInvestments, grossDebt, FVMinorities);
                decimal numberOfShares = SummaryData.Select(a => a.NumberOfShares).FirstOrDefault();
                if (numberOfShares != 0)
                {
                    DCFValuePerShare = DCFCalculations.CalculateDCFValuePerShare(equityValue, numberOfShares);
                }
                else
                {
                    DCFValuePerShare = 0;
                }
                decimal upsideDownside = DCFCalculations.CalculateUpsideValue(DCFValuePerShare, Convert.ToDecimal(CurrentMarketPrice));

                result.Add(new DCFDisplayData() { PropertyName = "Present Value of Explicit Forecast", Value = Convert.ToString(Math.Round(Convert.ToDecimal(PresentValueExplicitForecast), 4)) });
                result.Add(new DCFDisplayData() { PropertyName = "Terminal Value", Value = Convert.ToString(Math.Round(Convert.ToDecimal(TerminalValuePresent), 4)) });
                result.Add(new DCFDisplayData() { PropertyName = "Total Enterprise Value", Value = Convert.ToString(Math.Round(Convert.ToDecimal(totalEnterpriseValue), 4)) });
                result.Add(new DCFDisplayData() { PropertyName = "(+) Cash", Value = Convert.ToString(Math.Round(Convert.ToDecimal(cash), 4)) });
                result.Add(new DCFDisplayData() { PropertyName = "(+) FV of Investments & Associates", Value = Convert.ToString(Math.Round(Convert.ToDecimal(FVInvestments), 4)) });
                result.Add(new DCFDisplayData() { PropertyName = "(-) Gross Debt", Value = Convert.ToString(Math.Round(Convert.ToDecimal(grossDebt), 4)) });
                result.Add(new DCFDisplayData() { PropertyName = "(-) FV of Minorities", Value = Convert.ToString(Math.Round(Convert.ToDecimal(FVMinorities), 4)) });
                result.Add(new DCFDisplayData() { PropertyName = "Equity Value", Value = Convert.ToString(Math.Round(Convert.ToDecimal(equityValue), 4)) });
                result.Add(new DCFDisplayData() { PropertyName = "Number of Shares", Value = Convert.ToString(Math.Round(Convert.ToDecimal(numberOfShares), 4)) });
                result.Add(new DCFDisplayData() { PropertyName = "DCF Value Per Share", Value = Convert.ToString(Math.Round(Convert.ToDecimal(DCFValuePerShare), 4)) });
                result.Add(new DCFDisplayData() { PropertyName = "Upside", Value = Convert.ToString(Math.Round(Convert.ToDecimal(upsideDownside), 4)) });
                SummaryDisplayData.Clear();
                SummaryDisplayData.AddRange(result);
                this.RaisePropertyChanged(() => this.SummaryData);

                CalculationParameters.Cash = cash;
                CalculationParameters.FutureValueOfInvestments = FVInvestments;
                CalculationParameters.FutureValueOfMinorties = FVMinorities;
                CalculationParameters.NumberOfShares = numberOfShares;
                CalculationParameters.PresentValueOfForecast = PresentValueExplicitForecast;

                GenerateSensitivityDisplayData();

            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
        }

        /// <summary>
        /// Generate Data bound for Sensitivity Gadget
        /// </summary>
        public void GenerateSensitivityDisplayData()
        {
            try
            {
                decimal costOfEquity = CalculationParameters.CostOfEquity;
                decimal terminalValueGrowth = CalculationParameters.TerminalGrowthRate;

                /////to be Removed
                //{
                //    costOfEquity = 197 / 1000;
                //    CalculationParameters.CostOfEquity = Convert.ToDecimal(197.0 / 1000.0);
                //    terminalValueGrowth = Convert.ToDecimal(103.0 / 1000.0);
                //    CalculationParameters.TerminalGrowthRate = 103 / 1000;
                //    CalculationParameters.Cash = 1053;
                //    CalculationParameters.CostOfDebt = 103 / 1000;
                //    CalculationParameters.CurrentMarketPrice = 1933 / 100;
                //    CalculationParameters.FutureValueOfInvestments = 1;
                //    CalculationParameters.FutureValueOfMinorties = 1;
                //    CalculationParameters.GrossDebt = 157878;
                //    CalculationParameters.MarginalTaxRate = 253 / 1000;
                //    CalculationParameters.MarketCap = 252150;
                //    CalculationParameters.NumberOfShares = 13044;
                //    CalculationParameters.PresentValueOfForecast = 5033;
                //    CalculationParameters.Year10CashFlow = 282;
                //    CalculationParameters.Year10DiscountingFactor = 370 / 100;
                //}

                List<decimal> upSideValues = new List<decimal>();
                List<decimal> TGR = new List<decimal>();
                SensitivityDisplayData = new RangeObservableCollection<SensitivityData>();

                SensitivityDisplayData.Add(new SensitivityData() { C1 = " Step 0.25%", C2 = "", C3 = "-0.50%", C4 = "-0.25%", C5 = "0%", C6 = "0.25%", C7 = "0.50%" });

                Dictionary<int, decimal> VPS = new Dictionary<int, decimal>();

                DCFValue result = new DCFValue();

                CalculationParameters.CostOfEquity = Convert.ToDecimal((CalculationParameters.CostOfEquity) / Convert.ToDecimal(100.0)) - Convert.ToDecimal(5.0 / 1000.0);
                CalculationParameters.TerminalGrowthRate = Convert.ToDecimal((CalculationParameters.TerminalGrowthRate) / Convert.ToDecimal(100.0)) - Convert.ToDecimal(5.0 / 1000.0);

                for (int i = 0; i < 5; i++)
                {
                    SensitivityData data = new SensitivityData();
                    for (int j = 0; j < 5; j++)
                    {
                        result = new DCFValue();
                        result = DCFCalculations.CalculateDCFValue(CalculationParameters);
                        TGR.Add(result.DCFValuePerShare);
                        VPS.Add(j, result.DCFValuePerShare);
                        upSideValues.Add(result.UpsideValue);
                        CalculationParameters.CostOfEquity = CalculationParameters.CostOfEquity + Convert.ToDecimal(25.0 / 10000.0);
                    }
                    data.C1 = i.ToString();
                    data.C2 = ((-5.0 + (i * 2.5)) / 10.0).ToString() + "%";
                    if (VPS.ContainsKey(0))
                        data.C3 = Convert.ToString(Math.Round(Convert.ToDecimal(VPS.Where(a => a.Key == 0).Select(a => a.Value).FirstOrDefault()), 4) + "%");
                    if (VPS.ContainsKey(1))
                        data.C4 = Convert.ToString(Math.Round(Convert.ToDecimal(VPS.Where(a => a.Key == 1).Select(a => a.Value).FirstOrDefault()), 4) + "%");
                    if (VPS.ContainsKey(1))
                        data.C5 = Convert.ToString(Math.Round(Convert.ToDecimal(VPS.Where(a => a.Key == 2).Select(a => a.Value).FirstOrDefault()), 4) + "%");
                    if (VPS.ContainsKey(1))
                        data.C6 = Convert.ToString(Math.Round(Convert.ToDecimal(VPS.Where(a => a.Key == 3).Select(a => a.Value).FirstOrDefault()), 4) + "%");
                    if (VPS.ContainsKey(1))
                        data.C7 = Convert.ToString(Math.Round(Convert.ToDecimal(VPS.Where(a => a.Key == 4).Select(a => a.Value).FirstOrDefault()), 4) + "%");
                    VPS = new Dictionary<int, decimal>();
                    SensitivityDisplayData.Add(data);
                    CalculationParameters.CostOfEquity = costOfEquity;
                    CalculationParameters.TerminalGrowthRate = CalculationParameters.TerminalGrowthRate + Convert.ToDecimal(25.0 / 10000.0);
                }
                MaxShareVal = Convert.ToString(Math.Round(TGR.Select(a => a).Max(), 4)) + " %";
                MinShareVal = Convert.ToString((Math.Round(TGR.Select(a => a).Min(), 4))) + " %";
                AvgShareVal = Convert.ToString((Math.Round(TGR.Select(a => a).Average(), 4))) + " %";

                MaxUpside = Convert.ToString((Math.Round(upSideValues.Select(a => a).Max(), 4))) + " %";
                MinUpside = Convert.ToString((Math.Round(upSideValues.Select(a => a).Min(), 4))) + " %";
                AvgUpside = Convert.ToString((Math.Round(upSideValues.Select(a => a).Average(), 4))) + " %";
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
        }

        #endregion

    }
}