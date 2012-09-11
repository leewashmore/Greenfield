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
using GreenField.ServiceCaller.DCFDefinitions;

namespace GreenField.Gadgets.ViewModels
{
    /// <summary>
    /// View-Model for TerminalValueCalculations
    /// </summary>
    public class ViewModelTerminalValueCalculations : NotificationObject
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
        /// Constructor that initialises the class
        /// </summary>
        /// <param name="param">DashboardGadgetParam- Contains Singleton instances of Private Variables</param>
        public ViewModelTerminalValueCalculations(DashboardGadgetParam param)
        {
            _eventAggregator = param.EventAggregator;
            _dbInteractivity = param.DBInteractivity;
            _logger = param.LoggerFacade;

            EntitySelectionData = param.DashboardGadgetPayload.EntitySelectionData;
            if (_eventAggregator != null)
            {
                _eventAggregator.GetEvent<SecurityReferenceSetEvent>().Subscribe(HandleSecurityReferenceSetEvent);
                _eventAggregator.GetEvent<DCF_WACCSetEvent>().Subscribe(HandleWACCReferenceSetEvent);
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
                _isActive = value;
                this.RaisePropertyChanged(() => this.IsActive);
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

        /// <summary>
        /// List of Type TerminalValueCalculationsData
        /// </summary>
        private List<DCFTerminalValueCalculationsData> _terminalValueCalculationsData;
        public List<DCFTerminalValueCalculationsData> TerminalValueCalculationsData
        {
            get
            {
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
        private List<DCFDisplayData> _terminalValueCalculationsDisplayData;
        public List<DCFDisplayData> TerminalValueCalculationsDisplayData
        {
            get
            {
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
                this.RaisePropertyChanged(() => this.WACC);
            }
        }


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
                    if (IsActive && EntitySelectionData != null)
                    {
                        _dbInteractivity.RetrieveDCFTerminalValueCalculationsData(EntitySelectionData, RetrieveDCFTerminalValueCalculationsDataCallbackMethod);
                        _dbInteractivity.RetrieveCashFlows(EntitySelectionData, RetrieveDCFCashFlowYearlyDataCallbackMethod);
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

        /// <summary>
        /// WACC Set Event
        /// </summary>
        /// <param name="resultWACC">value of WACC</param>
        public void HandleWACCReferenceSetEvent(decimal resultWACC)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                //ArgumentNullException
                if (resultWACC != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, resultWACC, 1);
                    this.WACC = resultWACC;
                    if (YearlyCalculatedData != null)
                    {
                        YearlyCalculatedData = CalculateYearlyData(YearlyCalculatedData, WACC);
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
        public void SetAnalysisSummaryDisplayData()
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                RangeObservableCollection<DCFDisplayData> result = new RangeObservableCollection<DCFDisplayData>();

                decimal cashFlow2020 = Convert.ToDecimal(TerminalValueCalculationsData.Select(a => a.CashFlow2020).FirstOrDefault());
                decimal sustainableROIC = Convert.ToDecimal(TerminalValueCalculationsData.Select(a => a.SustainableROIC).FirstOrDefault());
                decimal sustainableDPR = Convert.ToDecimal(TerminalValueCalculationsData.Select(a => a.SustainableDividendPayoutRatio).FirstOrDefault());
                decimal longTermNominalGDPGrowth = Convert.ToDecimal(TerminalValueCalculationsData.Select(a => a.LongTermNominalGDPGrowth).FirstOrDefault());
                decimal TGR;
                decimal terminalValuePresent = Convert.ToDecimal(TerminalValueCalculationsData.Select(a => a.TerminalValuePresent).FirstOrDefault());
                decimal terminalValueNominal = Convert.ToDecimal(TerminalValueCalculationsData.Select(a => a.TerminalValueNominal).FirstOrDefault());

                TGR = Math.Min(sustainableROIC * (1 - sustainableDPR / 100), longTermNominalGDPGrowth / 100) * 100;

                result.Add(new DCFDisplayData() { PropertyName = "Cash Flow in 2020", Value = cashFlow2020.ToString() });
                result.Add(new DCFDisplayData() { PropertyName = "Sustainable ROIC", Value = sustainableROIC.ToString() });
                result.Add(new DCFDisplayData() { PropertyName = "Sustainable Dividend Payout Ratio", Value = sustainableDPR.ToString() });
                result.Add(new DCFDisplayData() { PropertyName = "Long-term Nominal GDP Growth", Value = longTermNominalGDPGrowth.ToString() });
                result.Add(new DCFDisplayData() { PropertyName = "Terminal Growth Rate", Value = cashFlow2020.ToString() });
                result.Add(new DCFDisplayData() { PropertyName = "Terminal Value (nominal)", Value = terminalValueNominal.ToString() });
                result.Add(new DCFDisplayData() { PropertyName = "Terminal Value (nominal)", Value = terminalValuePresent.ToString() });

                _eventAggregator.GetEvent<DCFTerminalValuepresent>().Publish(terminalValuePresent);

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
                    item.DISCOUNTING_FACTOR = Convert.ToDecimal(Math.Pow(Convert.ToDouble(1 + WACC), Convert.ToDouble(endDate - currentDate)));
                    if (item.DISCOUNTING_FACTOR != 0)
                    {
                        item.AMOUNT = item.AMOUNT / item.DISCOUNTING_FACTOR;
                    }
                    else
                    {
                        item.AMOUNT = 0;
                    }
                }
                if (periodData != null)
                {
                    _eventAggregator.GetEvent<DCFYearlyDataSetEvent>().Publish(periodData);
                }
                return periodData;
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
                return null;
            }
        }

        #endregion
    }
}