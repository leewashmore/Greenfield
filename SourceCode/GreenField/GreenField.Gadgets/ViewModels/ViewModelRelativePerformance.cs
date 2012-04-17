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
using GreenField.ServiceCaller.ProxyDataDefinitions;
using GreenField.Common;
using System.Collections.Generic;
using Microsoft.Practices.Prism.ViewModel;

namespace GreenField.Gadgets.ViewModels
{
    public class ViewModelRelativePerformance : NotificationObject
    {
        #region Fields
        //MEF Singletons
        public IEventAggregator _eventAggregator;
        public IDBInteractivity _dbInteractivity;
        public ILoggerFacade _logger;

        //Selection Data
        public FundSelectionData _fundSelectionData;
        public BenchmarkSelectionData _benchmarkSelectionData;
        public DateTime _effectiveDate;

        //Gadget Data
        private List<RelativePerformanceSectorData> _relativePerformanceSectorInfo;
        private List<RelativePerformanceData> _relativePerformanceInfo;

        
        #endregion

        #region Constructor
        public ViewModelRelativePerformance(DashboardGadgetParam param)
        {
            //MEF Singleton Initialization
            _eventAggregator = param.EventAggregator;
            _dbInteractivity = param.DBInteractivity;
            _logger = param.LoggerFacade;

            //Selection Data Initialization
            _fundSelectionData = param.DashboardGadgetPayload.FundSelectionData;
            _benchmarkSelectionData = param.DashboardGadgetPayload.BenchmarkSelectionData;
            _effectiveDate = param.DashboardGadgetPayload.EffectiveDate;

            //Service Call to Retrieve Sector Data relating Fund Selection Data/ Benchmark Selection Data and Effective Date
            //if (_effectiveDate != null && _fundSelectionData != null && _benchmarkSelectionData != null)
            //{
            //    _dbInteractivity.RetrieveRelativePerformanceSectorData(_fundSelectionData, _benchmarkSelectionData, _effectiveDate, RetrieveRelativePerformanceSectorDataCallbackMethod);
            //}
            _dbInteractivity.RetrieveRelativePerformanceSectorData(_fundSelectionData, _benchmarkSelectionData, _effectiveDate, RetrieveRelativePerformanceSectorDataCallbackMethod);

            if (_eventAggregator != null)
            {
                _eventAggregator.GetEvent<FundReferenceSetEvent>().Subscribe(HandleFundReferenceSet);
                _eventAggregator.GetEvent<BenchmarkReferenceSetEvent>().Subscribe(HandleBenchmarkReferenceSet);
                _eventAggregator.GetEvent<EffectiveDateSet>().Subscribe(HandleEffectiveDateSet);
            }
        } 
        #endregion

        #region Properties
        #region UI Fields

        private List<AssetAllocationData> _assetAllocationInfo;
        public List<AssetAllocationData> AssetAllocationInfo
        {
            get { return _assetAllocationInfo; }
            set
            {
                if (_assetAllocationInfo != value)
                {
                    _assetAllocationInfo = value;
                    RaisePropertyChanged(() => this.AssetAllocationInfo);
                }
            }
        } 
        #endregion
        #endregion

        #region Events
        public event RelativePerformanceGridBuildEventHandler RelativePerformanceGridBuildEvent;
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
                        _dbInteractivity.RetrieveRelativePerformanceSectorData(_fundSelectionData, _benchmarkSelectionData, _effectiveDate, RetrieveRelativePerformanceSectorDataCallbackMethod);
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
                        _dbInteractivity.RetrieveRelativePerformanceSectorData(_fundSelectionData, _benchmarkSelectionData, _effectiveDate, RetrieveRelativePerformanceSectorDataCallbackMethod);
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
                        _dbInteractivity.RetrieveRelativePerformanceSectorData(_fundSelectionData, _benchmarkSelectionData, _effectiveDate, RetrieveRelativePerformanceSectorDataCallbackMethod);
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
        private void RetrieveRelativePerformanceSectorDataCallbackMethod(List<RelativePerformanceSectorData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    _relativePerformanceSectorInfo = result;
                    //Service Call to Retrieve Performance Data relating Fund Selection Data/ Benchmark Selection Data and Effective Date
                    _dbInteractivity.RetrieveRelativePerformanceData(_fundSelectionData, _benchmarkSelectionData, _effectiveDate, RetrieveRelativePerformanceDataCallbackMethod);                    
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

        private void RetrieveRelativePerformanceDataCallbackMethod(List<RelativePerformanceData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    _relativePerformanceInfo = result;
                    RelativePerformanceGridBuildEvent.Invoke(new RelativePerformanceGridBuildEventArgs()
                    {
                        RelativePerformanceSectorInfo = _relativePerformanceSectorInfo,
                        RelativePerformanceInfo = _relativePerformanceInfo
                    });

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
