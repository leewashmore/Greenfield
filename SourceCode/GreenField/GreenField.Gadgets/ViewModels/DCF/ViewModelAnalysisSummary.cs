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

namespace GreenField.Gadgets.ViewModels
{
    /// <summary>
    /// View-Model for DCF-AnalysisSummary
    /// </summary>
    public class ViewModelAnalysisSummary : NotificationObject
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

        /// <summary>
        /// Instance of LoggerFacade
        /// </summary>
        public ILoggerFacade _logger;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="param">Parameter of type DashboardGadgetParam</param>
        public ViewModelAnalysisSummary(DashboardGadgetParam param)
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
        /// Selected Security from the Tool-Bar
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

        #region ActiveDashboard

        /// <summary>
        /// To check for Active Dashboard
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

        #region DataGrid

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
        private List<DCFAnalysisSummaryDisplayData> _analysisSummaryDisplayData;
        public List<DCFAnalysisSummaryDisplayData> AnalysisSummaryDisplayData
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

        #region Calculations




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
                        _dbInteractivity.RetrieveDCFAnalysisData(EntitySelectionData, RetrieveDCFAnalysisDataCallbackMethod);
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
        /// <param name="data"></param>
        public void SetAnalysisSummaryDisplayData()
        {
            decimal costOfEquity;
            decimal weightOfEquity;
            decimal costOfDebt;
            decimal WACC;

            List<DCFAnalysisSummaryDisplayData> result = new List<DCFAnalysisSummaryDisplayData>();

            result.Add(new DCFAnalysisSummaryDisplayData() { PropertyName = "Market Risk Premium", Value = Convert.ToString(AnalysisSummaryData.Select(a => a.MarketRiskPremium).FirstOrDefault()) });
            result.Add(new DCFAnalysisSummaryDisplayData() { PropertyName = "Beta (*)", Value = Convert.ToString(AnalysisSummaryData.Select(a => a.Beta).FirstOrDefault()) });
            result.Add(new DCFAnalysisSummaryDisplayData() { PropertyName = "Risk Free Rate", Value = Convert.ToString(AnalysisSummaryData.Select(a => a.MarketRiskPremium).FirstOrDefault()) });
            result.Add(new DCFAnalysisSummaryDisplayData()
            {
                PropertyName = "Stock Specific Discount",
                Value = Convert.ToString(AnalysisSummaryDisplayData.Where(a => a.PropertyName == "Stock Specific Discount").Select(a => a.Value).FirstOrDefault())
            });

            result.Add(new DCFAnalysisSummaryDisplayData() { PropertyName = "Marginal Tax Rate", Value = Convert.ToString(AnalysisSummaryData.Select(a => a.MarketRiskPremium).FirstOrDefault()) });

            string aa = Convert.ToString(result.Where(a => a.PropertyName == "Stock Specific Discount").Select(a => a.Value).FirstOrDefault());

            result.Add(new DCFAnalysisSummaryDisplayData()
            {
                PropertyName = "Cost of Equity",
                Value = Convert.ToString(Convert.ToDecimal(AnalysisSummaryData.Select(a => a.Beta).FirstOrDefault()) * Convert.ToDecimal(AnalysisSummaryData.Select(a => a.MarketRiskPremium).FirstOrDefault()) + Convert.ToDecimal(AnalysisSummaryData.Select(a => a.RiskFreeRate).FirstOrDefault()) + Convert.ToDecimal(result.Where(a => a.PropertyName == "Stock Specific Discount").Select(a => a.Value).FirstOrDefault()))
            });

            costOfEquity = Convert.ToDecimal(AnalysisSummaryData.Select(a => a.Beta).FirstOrDefault()) * Convert.ToDecimal(AnalysisSummaryData.Select(a => a.MarketRiskPremium).FirstOrDefault()) + Convert.ToDecimal(AnalysisSummaryData.Select(a => a.RiskFreeRate).FirstOrDefault()) + Convert.ToDecimal(Convert.ToString(result.Where(a => a.PropertyName == "Stock Specific Discount").Select(a => a.Value).FirstOrDefault()));



            result.Add(new DCFAnalysisSummaryDisplayData() { PropertyName = "Cost of Debt", Value = Convert.ToString(AnalysisSummaryData.Select(a => a.MarketRiskPremium).FirstOrDefault()) });

            costOfDebt = Convert.ToDecimal(AnalysisSummaryData.Select(a => a.MarketRiskPremium).FirstOrDefault());

            result.Add(new DCFAnalysisSummaryDisplayData() { PropertyName = "Market Cap", Value = Convert.ToString(AnalysisSummaryData.Select(a => a.MarketRiskPremium).FirstOrDefault()) });
            result.Add(new DCFAnalysisSummaryDisplayData() { PropertyName = "Gross Debt", Value = Convert.ToString(AnalysisSummaryData.Select(a => a.MarketRiskPremium).FirstOrDefault()) });
            result.Add(new DCFAnalysisSummaryDisplayData() { PropertyName = "Weight of Equity", Value = Convert.ToString(Convert.ToDecimal(AnalysisSummaryData.Select(a => a.MarketCap).FirstOrDefault()) / (Convert.ToDecimal(AnalysisSummaryData.Select(a => a.MarketCap).FirstOrDefault()) + Convert.ToDecimal(AnalysisSummaryData.Select(a => a.GrossDebt).FirstOrDefault()))) });
            weightOfEquity = Convert.ToDecimal(AnalysisSummaryData.Select(a => a.MarketCap).FirstOrDefault()) / (Convert.ToDecimal(AnalysisSummaryData.Select(a => a.MarketCap).FirstOrDefault()) + Convert.ToDecimal(AnalysisSummaryData.Select(a => a.GrossDebt).FirstOrDefault()));


            WACC = (weightOfEquity * costOfEquity) + ((1 - weightOfEquity) * (costOfDebt * (1 - Convert.ToDecimal(AnalysisSummaryData.Select(a => a.MarginalTaxRate).FirstOrDefault()))));

            result.Add(new DCFAnalysisSummaryDisplayData() { PropertyName = "WACC", Value = Convert.ToString(WACC) });
            AnalysisSummaryDisplayData = result;
            this.RaisePropertyChanged(() => this.AnalysisSummaryDisplayData);

        }

        /// <summary>
        /// Set Default Display Data
        /// </summary>
        /// <returns></returns>
        private List<DCFAnalysisSummaryDisplayData> SetDefaultAnalysisDisplayData()
        {
            List<DCFAnalysisSummaryDisplayData> result = new List<DCFAnalysisSummaryDisplayData>();
            result.Add(new DCFAnalysisSummaryDisplayData() { PropertyName = "Market Risk Premium" });
            result.Add(new DCFAnalysisSummaryDisplayData() { PropertyName = "Beta (*)" });
            result.Add(new DCFAnalysisSummaryDisplayData() { PropertyName = "Risk Free Rate" });
            result.Add(new DCFAnalysisSummaryDisplayData() { PropertyName = "Stock Specific Discount" });
            result.Add(new DCFAnalysisSummaryDisplayData() { PropertyName = "Marginal Tax Rate" });
            result.Add(new DCFAnalysisSummaryDisplayData() { PropertyName = "Cost of Equity" });
            result.Add(new DCFAnalysisSummaryDisplayData() { PropertyName = "Cost of Debt" });
            result.Add(new DCFAnalysisSummaryDisplayData() { PropertyName = "Market Cap" });
            result.Add(new DCFAnalysisSummaryDisplayData() { PropertyName = "Gross Debt" });
            result.Add(new DCFAnalysisSummaryDisplayData() { PropertyName = "Weight of Equity" });
            result.Add(new DCFAnalysisSummaryDisplayData() { PropertyName = "WACC" });
            return result;
        }

        #endregion

        #region EventsUnsubscribe

        /// <summary>
        /// Unsubscribing Events & Event Handlers
        /// </summary>
        public void Dispose()
        {
            _eventAggregator.GetEvent<SecurityReferenceSetEvent>().Unsubscribe(HandleSecurityReferenceSetEvent);
        }

        #endregion
    }
}
