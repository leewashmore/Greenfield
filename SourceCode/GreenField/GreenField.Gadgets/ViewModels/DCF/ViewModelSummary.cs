using System;
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
using Greenfield.Gadgets.Helpers;

namespace GreenField.Gadgets.ViewModels
{
    /// <summary>
    /// View-Model for DCFSummary
    /// </summary>
    public class ViewModelDCFSummary : NotificationObject
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

        public ViewModelDCFSummary(DashboardGadgetParam param)
        {
            _eventAggregator = param.EventAggregator;
            _dbInteractivity = param.DBInteractivity;
            _logger = param.LoggerFacade;

            EntitySelectionData = param.DashboardGadgetPayload.EntitySelectionData;
            if (_eventAggregator != null)
            {
                _eventAggregator.GetEvent<SecurityReferenceSetEvent>().Subscribe(HandleSecurityReferenceSetEvent);
                _eventAggregator.GetEvent<DCF_WACCSetEvent>().Subscribe(HandleWACCReferenceSetEvent);
                _eventAggregator.GetEvent<DCFYearlyDataSetEvent>().Subscribe(HandleYearlyDataSetEvent);
                _eventAggregator.GetEvent<DCFTerminalValuepresent>().Subscribe(HandleTerminalValueSetEvent);
            }

            if (EntitySelectionData != null)
            {
                HandleSecurityReferenceSetEvent(EntitySelectionData);
            }
        }

        #endregion

        #region PropertyDeclaration

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

        #region Calculations

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

        private decimal _terminalValuePresent;
        public decimal TerminalValuePresent
        {
            get { return _terminalValuePresent; }
            set
            {
                _terminalValuePresent = value;
                this.RaisePropertyChanged(() => this.TerminalValuePresent);
            }
        }


        #endregion

        #region DataGrid

        /// <summary>
        /// Data Returned from Service
        /// </summary>
        private List<DCFSummaryData> _summaryData;
        public List<DCFSummaryData> SummaryData
        {
            get
            {
                return _summaryData;
            }
            set
            {
                _summaryData = value;
                SetSummaryDisplayData();
                this.RaisePropertyChanged(() => this.SummaryData);
            }
        }

        private List<DCFDisplayData> _summaryDisplayData;
        public List<DCFDisplayData> SummaryDisplayData
        {
            get { return _summaryDisplayData; }
            set { _summaryDisplayData = value; }
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
                        _dbInteractivity.RetrieveDCFCurrentPrice(entitySelectionData, RetrieveCurrentPriceDataCallbackMethod);
                        _dbInteractivity.RetrieveDCFSummaryData(entitySelectionData, RetrieveDCFSummaryDataCallbackMethod);
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
        /// Terminal Value Set Event
        /// </summary>
        /// <param name="terminalValuePresent"></param>
        public void HandleTerminalValueSetEvent(decimal terminalValuePresent)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                //ArgumentNullException
                if (terminalValuePresent != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, terminalValuePresent, 1);
                    TerminalValuePresent = terminalValuePresent;
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
        /// Yearly Data Set Event
        /// </summary>
        /// <param name="result">List of type DCFCashFlowData</param>
        public void HandleYearlyDataSetEvent(List<DCFCashFlowData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    YearlyCalculatedData = result;
                    PresentValueExplicitForecast = Convert.ToDecimal(result.Select(a => a.AMOUNT).Sum());
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

        #region CallbackMethods

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
        public void SetSummaryDisplayData()
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                List<DCFDisplayData> result = new List<DCFDisplayData>();
                decimal totalEnterpriseValue = DCFCalculations.CalculateTotalEnterpriseValue(PresentValueExplicitForecast, TerminalValuePresent);
                decimal cash = SummaryData.Select(a => a.Cash).FirstOrDefault();
                decimal FVInvestments= SummaryData.Select(a => a.FVInvestments).FirstOrDefault();
                decimal grossDebt= SummaryData.Select(a => a.GrossDebt).FirstOrDefault();
                decimal FVMinorities= SummaryData.Select(a => a.FVMinorities).FirstOrDefault();
                decimal equityValue= DCFCalculations.CalculateEquityValue(totalEnterpriseValue,cash,FVInvestments,grossDebt,FVMinorities);
                decimal numberOfShares= SummaryData.Select(a => a.NumberOfShares).FirstOrDefault();
                decimal DCFValuePerShare=DCFCalculations.CalculateDCFValuePerShare(equityValue,numberOfShares);
                decimal upsideDownside=DCFCalculations.CalculateUpsideValue(DCFValuePerShare,Convert.ToDecimal(CurrentMarketPrice));

                result.Add(new DCFDisplayData() { PropertyName = "Present Value of Explicit Forecast", Value = Convert.ToString(PresentValueExplicitForecast) });
                result.Add(new DCFDisplayData() { PropertyName = "Terminal Value", Value = Convert.ToString(TerminalValuePresent) });
                result.Add(new DCFDisplayData() { PropertyName = "Total Enterprise Value", Value = Convert.ToString(totalEnterpriseValue) });
                result.Add(new DCFDisplayData() { PropertyName = "(+) Cash", Value = Convert.ToString(cash) });
                result.Add(new DCFDisplayData() { PropertyName = "(+) FV of Investments & Associates", Value = Convert.ToString(FVInvestments) });
                result.Add(new DCFDisplayData() { PropertyName = "(-) Gross Debt", Value = Convert.ToString(grossDebt) });
                result.Add(new DCFDisplayData() { PropertyName = "(-) FV of Minorities", Value = Convert.ToString(FVMinorities) });
                result.Add(new DCFDisplayData() { PropertyName = "Equity Value", Value = Convert.ToString(equityValue) });
                result.Add(new DCFDisplayData() { PropertyName = "Number of Shares", Value = Convert.ToString(numberOfShares) });
                result.Add(new DCFDisplayData() { PropertyName = "DCF Value Per Share", Value = Convert.ToString(DCFValuePerShare) });
                result.Add(new DCFDisplayData() { PropertyName = "Upside", Value = Convert.ToString(upsideDownside) });
                SummaryDisplayData = result;
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
