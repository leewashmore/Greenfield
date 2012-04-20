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
using Microsoft.Practices.Prism.ViewModel;
using System.Collections.Generic;
using GreenField.ServiceCaller.BenchmarkHoldingsPerformanceDefinitions;

namespace GreenField.Gadgets.ViewModels
{
    public class ViewModelAssetAllocation : NotificationObject
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
        public ViewModelAssetAllocation(DashboardGadgetParam param)
        {
            _eventAggregator = param.EventAggregator;
            _dbInteractivity = param.DBInteractivity;
            _logger = param.LoggerFacade;

            _fundSelectionData = param.DashboardGadgetPayload.FundSelectionData;
            _benchmarkSelectionData = param.DashboardGadgetPayload.BenchmarkSelectionData;
            _effectiveDate = param.DashboardGadgetPayload.EffectiveDate;

            //if (_effectiveDate != null && _fundSelectionData != null && _benchmarkSelectionData != null)
            //{
            //    _dbInteractivity.RetrieveAssetAllocationData(_fundSelectionData, _benchmarkSelectionData, _effectiveDate, RetrieveAssetAllocationDataCallbackMethod);
            //}
            _dbInteractivity.RetrieveAssetAllocationData(_fundSelectionData, _benchmarkSelectionData, _effectiveDate, RetrieveAssetAllocationDataCallbackMethod);
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
                        _dbInteractivity.RetrieveAssetAllocationData(_fundSelectionData, _benchmarkSelectionData, _effectiveDate, RetrieveAssetAllocationDataCallbackMethod);
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
                        _dbInteractivity.RetrieveAssetAllocationData(_fundSelectionData, _benchmarkSelectionData, _effectiveDate, RetrieveAssetAllocationDataCallbackMethod);
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
                        _dbInteractivity.RetrieveAssetAllocationData(_fundSelectionData, _benchmarkSelectionData, _effectiveDate, RetrieveAssetAllocationDataCallbackMethod);
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
        private void RetrieveAssetAllocationDataCallbackMethod(List<AssetAllocationData> assetAllocationData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (assetAllocationData != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, assetAllocationData, 1);
                    AssetAllocationInfo = assetAllocationData;
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

        #region EventUnsubscribe

        public void Dispose()
        {
            _eventAggregator.GetEvent<FundReferenceSetEvent>().Unsubscribe(HandleFundReferenceSet);
            _eventAggregator.GetEvent<BenchmarkReferenceSetEvent>().Unsubscribe(HandleBenchmarkReferenceSet);
            _eventAggregator.GetEvent<EffectiveDateSet>().Unsubscribe(HandleEffectiveDateSet);
        }

        #endregion

    }
}
