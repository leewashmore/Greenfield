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
using System.Collections.Generic;
using GreenField.ServiceCaller.ProxyDataDefinitions;
using Microsoft.Practices.Prism.ViewModel;
using GreenField.Common;
using System.Collections.ObjectModel;

namespace GreenField.Gadgets.ViewModels
{
    public class ViewModelHoldingsPieChart : NotificationObject
    {
        #region Fields
        /// <summary>
        /// MEF Singletons
        /// </summary>
        private IDBInteractivity _dbInteractivity;
        private ILoggerFacade _logger;
        private IEventAggregator _eventAggregator;
        /// <summary>
        /// DashboardGadgetPayLoad fields
        /// </summary>
        private BenchmarkSelectionData _benchmarkSelectionData;
        #endregion

        #region Constructor
        public ViewModelHoldingsPieChart(DashBoardGadgetParam param)
        {
            _dbInteractivity = param.DBInteractivity;
            _logger = param.LoggerFacade;
            _eventAggregator = param.EventAggregator;
            _benchmarkSelectionData = param.DashboardGadgetPayLoad.BenchmarkSelectionData;
            EffectiveDate = param.DashboardGadgetPayLoad.EffectiveDate;

            if (_eventAggregator != null)
            {
                _eventAggregator.GetEvent<BenchmarkReferenceSetEvent>().Subscribe(HandleBenchmarkReferenceSet);
                _eventAggregator.GetEvent<EffectiveDateSet>().Subscribe(HandleEffectiveDateSet);
            }

            //if (_benchmarkSelectionData != null && EffectiveDate != null)
            //{
            //    _dbInteractivity.RetrieveHoldingsPercentageData(_benchmarkSelectionData, EffectiveDate, RetrieveHoldingsPercentageDataCallbackMethod);
            //}
            _dbInteractivity.RetrieveHoldingsPercentageData(_benchmarkSelectionData, EffectiveDate, RetrieveHoldingsPercentageDataCallbackMethod);
        }
        #endregion

        #region Properties
        private ObservableCollection<HoldingsPercentageData> _holdingsPercentageInfo;
        public ObservableCollection<HoldingsPercentageData> HoldingsPercentageInfo
        {
            get { return _holdingsPercentageInfo; }
            set
            {
                _holdingsPercentageInfo = value;
                RaisePropertyChanged(()=> this.HoldingsPercentageInfo);
            }
        }

        private DateTime _effectiveDate;
        public DateTime EffectiveDate
        {
            get { return _effectiveDate; }
            set
            {
                _effectiveDate = value;
                RaisePropertyChanged(() => this.EffectiveDate);
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
                    EffectiveDate = effectiveDate;
                    if (EffectiveDate != null && _benchmarkSelectionData != null)
                    {
                        _dbInteractivity.RetrieveHoldingsPercentageData(_benchmarkSelectionData, EffectiveDate, RetrieveHoldingsPercentageDataCallbackMethod);
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
                    if (EffectiveDate != null && _benchmarkSelectionData != null)
                    {
                        _dbInteractivity.RetrieveHoldingsPercentageData(_benchmarkSelectionData, EffectiveDate, RetrieveHoldingsPercentageDataCallbackMethod);
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
        public void RetrieveHoldingsPercentageDataCallbackMethod(List<HoldingsPercentageData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    HoldingsPercentageInfo = new ObservableCollection<HoldingsPercentageData>(result);
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
