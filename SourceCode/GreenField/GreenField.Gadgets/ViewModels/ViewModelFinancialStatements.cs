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
using GreenField.Common;
using GreenField.Gadgets.Models;
using Microsoft.Practices.Prism.ViewModel;
using GreenField.ServiceCaller.ExternalResearchDefinitions;
using GreenField.DataContracts;
using System.Collections.Generic;
using Microsoft.Practices.Prism.Events;
using GreenField.ServiceCaller;
using Microsoft.Practices.Prism.Logging;
using System.Linq;

namespace GreenField.Gadgets.ViewModels
{
    public class ViewModelFinancialStatements : NotificationObject
    {
        #region Fields
        /// <summary>
        /// MEF Singletons
        /// </summary>
        private IEventAggregator _eventAggregator;
        private IDBInteractivity _dbInteractivity;
        private ILoggerFacade _logger;
        private EntitySelectionData _entitySelectionData;
        #endregion

        public ViewModelFinancialStatements(DashboardGadgetParam param)
        {
            _logger = param.LoggerFacade;
            _dbInteractivity = param.DBInteractivity;
            _eventAggregator = param.EventAggregator;
            _entitySelectionData = param.DashboardGadgetPayload.EntitySelectionData;


            PeriodColumn.NavigationCompleted +=new PeriodColumnNavigationEventHandler(SetFinancialStatementDisplayInfo);

            if (_eventAggregator != null && _entitySelectionData != null)
            {
                _eventAggregator.GetEvent<SecurityReferenceSetEvent>().Subscribe(HandleSecurityReferenceSetEvent);
            }
            
            if (FinancialStatementInfo.Count.Equals(0) && _dbInteractivity!= null)
            {
                BusyIndicatorNotification(true, "Retrieving Financial Statement Data");
                _dbInteractivity.RetrieveFinancialStatementData("223340", FinancialStatementDataSource.REUTERS
                    , FinancialStatementPeriodType.ANNUAL, FinancialStatementFiscalType.FISCAL, FinancialStatementStatementType.BALANCE_SHEET, "CNY", RetrieveFinancialStatementDataCallbackMethod);
            }
        }

        public void HandleSecurityReferenceSetEvent(EntitySelectionData result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    _entitySelectionData = result;
                    if (_entitySelectionData != null)
                    {
                        BusyIndicatorNotification(true, "Retrieving Issuer Details based on selected security");
                        _dbInteractivity.RetrieveIssuerId(result, RetrieveIssuerIdCallbackMethod);
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

        private List<FinancialStatementDisplayData> _financialStatementDisplayInfo;
        public List<FinancialStatementDisplayData> FinancialStatementDisplayInfo
        {
            get { return _financialStatementDisplayInfo; }
            set 
            {
                _financialStatementDisplayInfo = value;
                RaisePropertyChanged(() => this.FinancialStatementDisplayInfo);
            }
        }

        private List<FinancialStatementData> _financialStatementInfo;
        public List<FinancialStatementData> FinancialStatementInfo
        {
            get 
            {
                if (_financialStatementInfo == null)
                    _financialStatementInfo = new List<FinancialStatementData>();
                return _financialStatementInfo; 
            }
            set 
            {
                if (_financialStatementInfo != value)
                {
                    _financialStatementInfo = value
                        .OrderBy(record => record.SORT_ORDER)
                        .ThenBy(record => record.PERIOD_TYPE)
                        .ThenBy(record => record.PERIOD)
                        .ToList();
                    SetFinancialStatementDisplayInfo();
                }
                
            }
        }

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
        

        #region Events
        /// <summary>
        /// Data Retrieval Progress Indicator
        /// </summary>
        public event DataRetrievalProgressIndicatorEventHandler FinancialStatementsDataLoadedEvent;
        #endregion

        public void Dispose()
        {

        }

        public void SetFinancialStatementDisplayInfo()
        {
            List<FinancialStatementDisplayData> result = new List<FinancialStatementDisplayData>();

            List<String> distinctDataDescriptors = FinancialStatementInfo.Select(record => record.DATA_DESC).Distinct().ToList();

            foreach (string dataDesc in distinctDataDescriptors)
            {
                FinancialStatementData yearOneData = FinancialStatementInfo.Where(record => record.DATA_DESC.ToUpper().Trim() == dataDesc.ToUpper().Trim() &&
                        record.PERIOD.ToUpper().Trim() == PeriodRecord.YearOne.ToString().ToUpper().Trim() && record.AMOUNT_TYPE.ToUpper().Trim() == (PeriodRecord.YearOneIsHistorical ? "A" : "E")
                        && record.PERIOD_TYPE.ToUpper().Trim() == "A").FirstOrDefault();

                FinancialStatementData yearTwoData = FinancialStatementInfo.Where(record => record.DATA_DESC.ToUpper().Trim() == dataDesc.ToUpper().Trim() &&
                        record.PERIOD.ToUpper().Trim() == PeriodRecord.YearTwo.ToString().ToUpper().Trim() && record.AMOUNT_TYPE.ToUpper().Trim() == (PeriodRecord.YearTwoIsHistorical ? "A" : "E")
                        && record.PERIOD_TYPE.ToUpper().Trim() == "A").FirstOrDefault();

                FinancialStatementData yearThreeData = FinancialStatementInfo.Where(record => record.DATA_DESC.ToUpper().Trim() == dataDesc.ToUpper().Trim() &&
                        record.PERIOD.ToUpper().Trim() == PeriodRecord.YearThree.ToString().ToUpper().Trim() && record.AMOUNT_TYPE.ToUpper().Trim() == (PeriodRecord.YearThreeIsHistorical ? "A" : "E")
                        && record.PERIOD_TYPE.ToUpper().Trim() == "A").FirstOrDefault();

                FinancialStatementData yearFourData = FinancialStatementInfo.Where(record => record.DATA_DESC.ToUpper().Trim() == dataDesc.ToUpper().Trim() &&
                        record.PERIOD.ToUpper().Trim() == PeriodRecord.YearFour.ToString().ToUpper().Trim() && record.AMOUNT_TYPE.ToUpper().Trim() == (PeriodRecord.YearFourIsHistorical ? "A" : "E")
                        && record.PERIOD_TYPE.ToUpper().Trim() == "A").FirstOrDefault();

                FinancialStatementData yearFiveData = FinancialStatementInfo.Where(record => record.DATA_DESC.ToUpper().Trim() == dataDesc.ToUpper().Trim() &&
                        record.PERIOD.ToUpper().Trim() == PeriodRecord.YearFive.ToString().ToUpper().Trim() && record.AMOUNT_TYPE.ToUpper().Trim() == (PeriodRecord.YearFiveIsHistorical ? "A" : "E")
                        && record.PERIOD_TYPE.ToUpper().Trim() == "A").FirstOrDefault();

                FinancialStatementData yearSixData = FinancialStatementInfo.Where(record => record.DATA_DESC.ToUpper().Trim() == dataDesc.ToUpper().Trim() &&
                        record.PERIOD.ToUpper().Trim() == PeriodRecord.YearSix.ToString().ToUpper().Trim() && record.AMOUNT_TYPE.ToUpper().Trim() == (PeriodRecord.YearSixIsHistorical ? "A" : "E")
                        && record.PERIOD_TYPE.ToUpper().Trim() == "A").FirstOrDefault();

                FinancialStatementData quarterOneData = FinancialStatementInfo.Where(record => record.DATA_DESC.ToUpper().Trim() == dataDesc.ToUpper().Trim() &&
                        record.PERIOD.ToUpper().Trim() == PeriodRecord.QuarterOneYear.ToString().ToUpper().Trim() && record.AMOUNT_TYPE.ToUpper().Trim() == (PeriodRecord.QuarterOneIsHistorical ? "A" : "E")
                        && record.PERIOD_TYPE.ToUpper().Trim() == "Q" + PeriodRecord.QuarterOneQuarter.ToString().ToUpper().Trim()).FirstOrDefault();

                FinancialStatementData quarterTwoData = FinancialStatementInfo.Where(record => record.DATA_DESC.ToUpper().Trim() == dataDesc.ToUpper().Trim() &&
                        record.PERIOD.ToUpper().Trim() == PeriodRecord.QuarterTwoYear.ToString().ToUpper().Trim() && record.AMOUNT_TYPE.ToUpper().Trim() == (PeriodRecord.QuarterTwoIsHistorical ? "A" : "E")
                        && record.PERIOD_TYPE.ToUpper().Trim() == "Q" + PeriodRecord.QuarterTwoQuarter.ToString().ToUpper().Trim()).FirstOrDefault();

                FinancialStatementData quarterThreeData = FinancialStatementInfo.Where(record => record.DATA_DESC.ToUpper().Trim() == dataDesc.ToUpper().Trim() &&
                        record.PERIOD.ToUpper().Trim() == PeriodRecord.QuarterThreeYear.ToString().ToUpper().Trim() && record.AMOUNT_TYPE.ToUpper().Trim() == (PeriodRecord.QuarterThreeIsHistorical ? "A" : "E")
                        && record.PERIOD_TYPE.ToUpper().Trim() == "Q" + PeriodRecord.QuarterThreeQuarter.ToString().ToUpper().Trim()).FirstOrDefault();

                FinancialStatementData quarterFourData = FinancialStatementInfo.Where(record => record.DATA_DESC.ToUpper().Trim() == dataDesc.ToUpper().Trim() &&
                        record.PERIOD.ToUpper().Trim() == PeriodRecord.QuarterFourYear.ToString().ToUpper().Trim() && record.AMOUNT_TYPE.ToUpper().Trim() == (PeriodRecord.QuarterFourIsHistorical ? "A" : "E")
                        && record.PERIOD_TYPE.ToUpper().Trim() == "Q" + PeriodRecord.QuarterFourQuarter.ToString().ToUpper().Trim()).FirstOrDefault();

                FinancialStatementData quarterFiveData = FinancialStatementInfo.Where(record => record.DATA_DESC.ToUpper().Trim() == dataDesc.ToUpper().Trim() &&
                        record.PERIOD.ToUpper().Trim() == PeriodRecord.QuarterFiveYear.ToString().ToUpper().Trim() && record.AMOUNT_TYPE.ToUpper().Trim() == (PeriodRecord.QuarterFiveIsHistorical ? "A" : "E")
                        && record.PERIOD_TYPE.ToUpper().Trim() == "Q" + PeriodRecord.QuarterFiveQuarter.ToString().ToUpper().Trim()).FirstOrDefault();

                FinancialStatementData quarterSixData = FinancialStatementInfo.Where(record => record.DATA_DESC.ToUpper().Trim() == dataDesc.ToUpper().Trim() &&
                        record.PERIOD.ToUpper().Trim() == PeriodRecord.QuarterSixYear.ToString().ToUpper().Trim() && record.AMOUNT_TYPE.ToUpper().Trim() == (PeriodRecord.QuarterSixIsHistorical ? "A" : "E")
                        && record.PERIOD_TYPE.ToUpper().Trim() == "Q" + PeriodRecord.QuarterSixQuarter.ToString().ToUpper().Trim()).FirstOrDefault();

                result.Add(new FinancialStatementDisplayData()
                {
                    DATA_DESC = dataDesc,
                    YEAR_ONE = yearOneData != null ? yearOneData.AMOUNT : null,
                    YEAR_TWO = yearTwoData != null ? yearTwoData.AMOUNT : null,
                    YEAR_THREE = yearThreeData != null ? yearThreeData.AMOUNT : null,
                    YEAR_FOUR = yearFourData != null ? yearFourData.AMOUNT : null,
                    YEAR_FIVE = yearFiveData != null ? yearFiveData.AMOUNT : null,
                    YEAR_SIX = yearSixData != null ? yearSixData.AMOUNT : null,
                    QUARTER_ONE = quarterOneData != null ? quarterOneData.AMOUNT : null,
                    QUARTER_TWO = quarterTwoData != null ? quarterTwoData.AMOUNT : null,
                    QUARTER_THREE = quarterThreeData != null ? quarterThreeData.AMOUNT : null,
                    QUARTER_FOUR = quarterFourData != null ? quarterFourData.AMOUNT : null,
                    QUARTER_FIVE = quarterFiveData != null ? quarterFiveData.AMOUNT : null,
                    QUARTER_SIX = quarterSixData != null ? quarterSixData.AMOUNT : null,
                });
            }

            FinancialStatementDisplayInfo = result;
            BusyIndicatorNotification();
            PeriodColumn.NavigationCompleted -= new PeriodColumnNavigationEventHandler(SetFinancialStatementDisplayInfo);
        }

        public void BusyIndicatorNotification(bool showBusyIndicator = false, String message = null)
        {
            if (message != null)
                BusyIndicatorContent = message;
            BusyIndicatorIsBusy = showBusyIndicator;            
        }

        #region Callback Methods
        public void RetrieveIssuerIdCallbackMethod(string result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    if (result.Equals(String.Empty))
                    {
                        Prompt.ShowDialog("No Issuer linked to the entity " + _entitySelectionData.LongName + " (" + _entitySelectionData.ShortName + " : " + _entitySelectionData.InstrumentID + ")");
                        return;
                    }

                    BusyIndicatorNotification(true, "Retrieving Financial Statement Data for ");
                    _dbInteractivity.RetrieveFinancialStatementData("223340", FinancialStatementDataSource.REUTERS
                        , FinancialStatementPeriodType.ANNUAL, FinancialStatementFiscalType.FISCAL, FinancialStatementStatementType.BALANCE_SHEET, "CNY", RetrieveFinancialStatementDataCallbackMethod);
                }
                else
                {
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                }
                BusyIndicatorNotification();
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        public void RetrieveFinancialStatementDataCallbackMethod(List<FinancialStatementData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    FinancialStatementInfo = result;
                }
                else
                {
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                }
                BusyIndicatorNotification();
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        } 
        #endregion
    }
}
