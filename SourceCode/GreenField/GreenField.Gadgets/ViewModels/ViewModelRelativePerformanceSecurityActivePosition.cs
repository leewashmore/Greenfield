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
using Microsoft.Practices.Prism.Events;
using GreenField.ServiceCaller;
using Microsoft.Practices.Prism.Logging;
using GreenField.ServiceCaller.SecurityReferenceDefinitions;
using GreenField.Common;
using System.Collections.ObjectModel;
using Microsoft.Practices.Prism.ViewModel;
using System.Collections.Generic;
using GreenField.ServiceCaller.BenchmarkHoldingsPerformanceDefinitions;

namespace GreenField.Gadgets.ViewModels
{
    public class ViewModelRelativePerformanceSecurityActivePosition : NotificationObject
    {
        #region Fields
        //MEF Singletons
        private IEventAggregator _eventAggregator;
        private IDBInteractivity _dbInteractivity;
        private ILoggerFacade _logger;

        private FundSelectionData _fundSelectionData;
        private BenchmarkSelectionData _benchmarkSelectionData;
        private DateTime _effectiveDate;
        #endregion

        #region Constructor
        public ViewModelRelativePerformanceSecurityActivePosition(DashboardGadgetParam param)
        {
            _eventAggregator = param.EventAggregator;
            _dbInteractivity = param.DBInteractivity;
            _logger = param.LoggerFacade;

            _fundSelectionData = param.DashboardGadgetPayload.FundSelectionData;
            _benchmarkSelectionData = param.DashboardGadgetPayload.BenchmarkSelectionData;
            _effectiveDate = param.DashboardGadgetPayload.EffectiveDate;

            //if (_effectiveDate != null && _fundSelectionData != null && _benchmarkSelectionData != null)
            //{
            //    _dbInteractivity.RetrieveRelativePerformanceSectorActivePositionData(_fundSelectionData, _benchmarkSelectionData, _effectiveDate, RetrieveRelativePerformanceSectorActivePositionDataCallbackMethod);
            //}
            _dbInteractivity.RetrieveRelativePerformanceSecurityActivePositionData(_fundSelectionData, _benchmarkSelectionData, _effectiveDate, RetrieveRelativePerformanceSecurityActivePositionDataCallbackMethod);
            
            if (_eventAggregator != null)
            {
                _eventAggregator.GetEvent<FundReferenceSetEvent>().Subscribe(HandleFundReferenceSet);
                _eventAggregator.GetEvent<BenchmarkReferenceSetEvent>().Subscribe(HandleBenchmarkReferenceSet);
                _eventAggregator.GetEvent<EffectiveDateSet>().Subscribe(HandleEffectiveDateSet);
                _eventAggregator.GetEvent<RelativePerformanceGridClickEvent>().Subscribe(HandleRelativePerformanceGridClickEvent);
            }
        }
        #endregion

        #region Properties
        #region UI Fields
        private ObservableCollection<RelativePerformanceActivePositionData> _relativePerformanceActivePositionInfo;
        public ObservableCollection<RelativePerformanceActivePositionData> RelativePerformanceActivePositionInfo
        {
            get { return _relativePerformanceActivePositionInfo; }
            set
            {
                if (_relativePerformanceActivePositionInfo != value)
                {
                    _relativePerformanceActivePositionInfo = value;
                    RaisePropertyChanged(() => this.RelativePerformanceActivePositionInfo);
                }
            }
        }        
        #endregion
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
                        _dbInteractivity.RetrieveRelativePerformanceSecurityActivePositionData(_fundSelectionData, _benchmarkSelectionData, _effectiveDate, RetrieveRelativePerformanceSecurityActivePositionDataCallbackMethod);
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
                        _dbInteractivity.RetrieveRelativePerformanceSecurityActivePositionData(_fundSelectionData, _benchmarkSelectionData, _effectiveDate, RetrieveRelativePerformanceSecurityActivePositionDataCallbackMethod);
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
                        _dbInteractivity.RetrieveRelativePerformanceSecurityActivePositionData(_fundSelectionData, _benchmarkSelectionData, _effectiveDate, RetrieveRelativePerformanceSecurityActivePositionDataCallbackMethod);
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

        public void HandleRelativePerformanceGridClickEvent(RelativePerformanceGridCellData filter)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (filter != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, filter, 1);
                    //if (_effectiveDate != null && _fundSelectionData != null && _benchmarkSelectionData != null)
                    //{
                    //    _dbInteractivity.RetrieveRelativePerformanceSectorActivePositionData(_fundSelectionData, _benchmarkSelectionData, _effectiveDate, RetrieveRelativePerformanceSectorActivePositionDataCallbackMethod, filter.SectorID, filter.SectorID);
                    //}
                    _dbInteractivity.RetrieveRelativePerformanceSecurityActivePositionData(_fundSelectionData, _benchmarkSelectionData, _effectiveDate, RetrieveRelativePerformanceSecurityActivePositionDataCallbackMethod, filter.CountryID, filter.SectorID);
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
        private void RetrieveRelativePerformanceSecurityActivePositionDataCallbackMethod(List<RelativePerformanceActivePositionData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    RelativePerformanceActivePositionInfo = new ObservableCollection<RelativePerformanceActivePositionData>(result);
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
