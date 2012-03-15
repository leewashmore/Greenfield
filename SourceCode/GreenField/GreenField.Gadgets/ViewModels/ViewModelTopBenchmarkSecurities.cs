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
using System.Collections.ObjectModel;

namespace GreenField.Gadgets.ViewModels
{
    public class ViewModelTopBenchmarkSecurities : NotificationObject
    {
        #region Fields
        /// <summary>
        /// MEF Singletons
        /// </summary>
        private IDBInteractivity _dbInteractivity;
        private ILoggerFacade _logger;
        private IEventAggregator _eventAggregator;

        private BenchmarkSelectionData _benchmarkSelectionData;
        private DateTime _effectiveDate;
        #endregion

        #region Constructor
        public ViewModelTopBenchmarkSecurities(DashBoardGadgetParam param)
        {
            _eventAggregator = param.EventAggregator;
            _dbInteractivity = param.DBInteractivity;
            _logger = param.LoggerFacade;

            _benchmarkSelectionData = param.DashboardGadgetPayLoad.BenchmarkSelectionData;
            _effectiveDate = param.DashboardGadgetPayLoad.EffectiveDate;

            if (_eventAggregator != null)
            {
                _eventAggregator.GetEvent<BenchmarkReferenceSetEvent>().Subscribe(HandleBenchmarkReferenceSet);
                _eventAggregator.GetEvent<EffectiveDateSet>().Subscribe(HandleEffectiveDateSet);
            }

            //if (_effectiveDate != null && _benchmarkSelectionData != null)
            //{
            //    _dbInteractivity.RetrieveTopBenchmarkSecuritiesData(_benchmarkSelectionData, _effectiveDate, RetrieveTopSecuritiesDataCallbackMethod);
            //}
            _dbInteractivity.RetrieveTopBenchmarkSecuritiesData(_benchmarkSelectionData, _effectiveDate, RetrieveTopBenchmarkSecuritiesDataCallbackMethod);
            
        }
        #endregion

        #region Properties
        private ObservableCollection<TopBenchmarkSecuritiesData> _topBenchmarkSecuritiesInfo;
        public ObservableCollection<TopBenchmarkSecuritiesData> TopBenchmarkSecuritiesInfo
        {
            get { return _topBenchmarkSecuritiesInfo; }
            set
            {
                _topBenchmarkSecuritiesInfo = value;
                RaisePropertyChanged(() => this.TopBenchmarkSecuritiesInfo);
            }
        }
        #endregion

        #region Event Handlers
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
                    if (_effectiveDate != null && _benchmarkSelectionData != null)
                    {
                        _dbInteractivity.RetrieveTopBenchmarkSecuritiesData(_benchmarkSelectionData, _effectiveDate, RetrieveTopBenchmarkSecuritiesDataCallbackMethod);
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
                    if (_effectiveDate != null && _benchmarkSelectionData != null)
                    {
                        _dbInteractivity.RetrieveTopBenchmarkSecuritiesData(_benchmarkSelectionData, _effectiveDate, RetrieveTopBenchmarkSecuritiesDataCallbackMethod);
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
        public void RetrieveTopBenchmarkSecuritiesDataCallbackMethod(List<TopBenchmarkSecuritiesData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    TopBenchmarkSecuritiesInfo = new ObservableCollection<TopBenchmarkSecuritiesData>(result);
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
