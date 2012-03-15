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
using GreenField.ServiceCaller.ProxyDataDefinitions;
using System.Collections.Generic;
using Microsoft.Practices.Prism.ViewModel;
using GreenField.Common;
using GreenField.Common.Helper;
using System.Collections.ObjectModel;

namespace GreenField.Gadgets.ViewModels
{
    public class ViewModelPortfolioRiskReturns : NotificationObject
    {
        #region Fields
        /// <summary>
        /// MEF Singletons
        /// </summary>
        private IDBInteractivity _dbInteractivity;
        private ILoggerFacade _logger;
        private IEventAggregator _eventAggregator;

        private FundSelectionData _fundSelectionData;
        private BenchmarkSelectionData _benchmarkSelectionData;
        private DateTime _effectiveDate;
        #endregion

        #region Constructor
        public ViewModelPortfolioRiskReturns(DashBoardGadgetParam param)
        {
            _dbInteractivity = param.DBInteractivity;
            _logger = param.LoggerFacade;
            _eventAggregator = param.EventAggregator;

            _fundSelectionData = param.DashboardGadgetPayLoad.FundSelectionData;
            _benchmarkSelectionData = param.DashboardGadgetPayLoad.BenchmarkSelectionData;
            _effectiveDate = param.DashboardGadgetPayLoad.EffectiveDate;

            if (_eventAggregator != null)
            {
                _eventAggregator.GetEvent<FundReferenceSetEvent>().Subscribe(HandleFundReferenceSet);
                _eventAggregator.GetEvent<BenchmarkReferenceSetEvent>().Subscribe(HandleBenchmarkReferenceSet);
                _eventAggregator.GetEvent<EffectiveDateSet>().Subscribe(HandleEffectiveDateSet);
            }

            //if (_effectiveDate != null && _fundSelectionData != null && _benchmarkSelectionData != null)
            //{
            //    _dbInteractivity.RetrievePortfolioRiskReturnData(_fundSelectionData, _benchmarkSelectionData, _effectiveDate, RetrievePortfolioRiskReturnDataCallbackMethod);
            //}
            _dbInteractivity.RetrievePortfolioRiskReturnData(_fundSelectionData, _benchmarkSelectionData, _effectiveDate, RetrievePortfolioRiskReturnDataCallbackMethod);
        }
        #endregion

        #region Properties
        private ObservableCollection<PortfolioRiskReturnData> _portfolioRiskReturnInfo;
        public ObservableCollection<PortfolioRiskReturnData> PortfolioRiskReturnInfo
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
        #endregion

        #region Event Handlers
        public void HandleFundReferenceSet(FundSelectionData fundSelectionData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);

            try
            {
                if (fundSelectionData != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, fundSelectionData, 1);
                    _fundSelectionData = fundSelectionData;
                    if (_effectiveDate != null && _fundSelectionData != null && _benchmarkSelectionData != null)
                    {
                        _dbInteractivity.RetrievePortfolioRiskReturnData(_fundSelectionData, _benchmarkSelectionData, _effectiveDate, RetrievePortfolioRiskReturnDataCallbackMethod);
                    }
                }
                else
                {
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

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
                    if (_effectiveDate != null && _fundSelectionData != null && _benchmarkSelectionData != null)
                    {
                        _dbInteractivity.RetrievePortfolioRiskReturnData(_fundSelectionData, _benchmarkSelectionData, _effectiveDate, RetrievePortfolioRiskReturnDataCallbackMethod);
                    }
                }
                else
                {
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        public void HandleBenchmarkReferenceSet(BenchmarkSelectionData benchmarkSelectionData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (benchmarkSelectionData != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, benchmarkSelectionData, 1);
                    _benchmarkSelectionData = benchmarkSelectionData;
                    if (_effectiveDate != null && _fundSelectionData != null && _benchmarkSelectionData != null)
                    {
                        _dbInteractivity.RetrievePortfolioRiskReturnData(_fundSelectionData, _benchmarkSelectionData, _effectiveDate, RetrievePortfolioRiskReturnDataCallbackMethod);
                    }
                }
                else
                {
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }
        #endregion

        #region Callback Methods
        public void RetrievePortfolioRiskReturnDataCallbackMethod(List<PortfolioRiskReturnData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    PortfolioRiskReturnInfo = new ObservableCollection<PortfolioRiskReturnData>(result);
                }
                else
                {
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }
        #endregion       
    }
}
