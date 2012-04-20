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
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Prism.Events;
using GreenField.ServiceCaller;
using Microsoft.Practices.Prism.Logging;
using GreenField.ServiceCaller.SecurityReferenceDefinitions;
using GreenField.Common;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using GreenField.ServiceCaller.BenchmarkHoldingsPerformanceDefinitions;

namespace GreenField.Gadgets.ViewModels
{
    /// <summary>
    /// View model for ViewTopDetractor class
    /// </summary>
    public class ViewModelTopDetractor: NotificationObject
    {
        #region Fields
        /// <summary>
        /// MEF Singletons
        /// </summary>
        private IEventAggregator _eventAggregator;
        private IDBInteractivity _dbInteractivity;
        private ILoggerFacade _logger;
        /// <summary>
        /// DashboardGadgetPayLoad fields
        /// </summary>
        private FundSelectionData _fundSelectionData;
        private BenchmarkSelectionData _benchmarkSelectionData;
        private DateTime _effectiveDate;
        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="eventAggregator">Event Aggregation from Shell</param>
        /// <param name="dbInteractivity">Instance of Service Caller</param>
        /// <param name="logger">Instance of Logger</param>
        /// <param name="param">DashboardGadgetparam</param>
        public ViewModelTopDetractor(DashboardGadgetParam param)
        {
            _eventAggregator = param.EventAggregator;
            _dbInteractivity = param.DBInteractivity;
            _logger = param.LoggerFacade;

            _fundSelectionData = param.DashboardGadgetPayload.FundSelectionData;
            _benchmarkSelectionData = param.DashboardGadgetPayload.BenchmarkSelectionData;
            _effectiveDate = param.DashboardGadgetPayload.EffectiveDate;

            if (_effectiveDate != null && _fundSelectionData != null && _benchmarkSelectionData != null)
            {
                _dbInteractivity.RetrieveRelativePerformanceSecurityData(_fundSelectionData, _benchmarkSelectionData, _effectiveDate, RetrieveRelativePerformanceSecurityDataCallBackMethod, null, null, 1, 5);
            }

            if (_eventAggregator != null)
            {
                _eventAggregator.GetEvent<FundReferenceSetEvent>().Subscribe(HandleFundReferenceSet);
                _eventAggregator.GetEvent<BenchmarkReferenceSetEvent>().Subscribe(HandleBenchmarkReferenceSet);
                _eventAggregator.GetEvent<EffectiveDateSet>().Subscribe(HandleEffectiveDateSet);
                _eventAggregator.GetEvent<RelativePerformanceGridClickEvent>().Subscribe(HandleRelativePerformanceGridClickevent);
            }
        } 
        #endregion

        #region Properties
        
        #region UI Fields

        /// <summary>
        /// contains all data to be displayed in the gadget.
        /// </summary>
        private ObservableCollection<RelativePerformanceSecurityData> _topDetractorInfo;
        public ObservableCollection<RelativePerformanceSecurityData> TopDetractorInfo
        {
            get { return _topDetractorInfo; }
            set 
            {
                if (_topDetractorInfo != value)
                {
                    _topDetractorInfo = value;
                    RaisePropertyChanged(() => this.TopDetractorInfo);
                }
            }
        }
        
        #endregion    
        #endregion

        #region Event Handlers

        /// <summary>
        /// Event Handler to subscribed event 'FundReferenceSetEvent'
        /// </summary>
        /// <param name="fundSelectionData">FundSelectionData</param>
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
                        _dbInteractivity.RetrieveRelativePerformanceSecurityData(_fundSelectionData, _benchmarkSelectionData, _effectiveDate, RetrieveRelativePerformanceSecurityDataCallBackMethod, null, null, 1, 5);
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

        /// <summary>
        /// Event Handler to subscribed event 'EffectiveDateSet'
        /// </summary>
        /// <param name="effectiveDate">DateTime</param>
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
                        _dbInteractivity.RetrieveRelativePerformanceSecurityData(_fundSelectionData, _benchmarkSelectionData, _effectiveDate, RetrieveRelativePerformanceSecurityDataCallBackMethod, null, null, 1, 5);
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

        /// <summary>
        /// Event Handler to subscribed event 'BenchmarkReferenceSetEvent'
        /// </summary>
        /// <param name="benchmarkSelectionData">BenchmarkSelectionData</param>
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
                        _dbInteractivity.RetrieveRelativePerformanceSecurityData(_fundSelectionData, _benchmarkSelectionData, _effectiveDate, RetrieveRelativePerformanceSecurityDataCallBackMethod, null, null, 1, 5);
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

        /// <summary>
        /// Event Handler to subscribed event 'RelativePerformanceGridClickEvent'
        /// </summary>
        /// <param name="relativePerformanceGridCellData">RelativePerformanceGridCellData</param>
        public void HandleRelativePerformanceGridClickevent(RelativePerformanceGridCellData relativePerformanceGridCellData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (relativePerformanceGridCellData != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, relativePerformanceGridCellData, 1);
                    if (_effectiveDate != null && _fundSelectionData != null && _benchmarkSelectionData != null)
                    {
                        _dbInteractivity.RetrieveRelativePerformanceSecurityData(_fundSelectionData, _benchmarkSelectionData, _effectiveDate,
                            RetrieveRelativePerformanceSecurityDataCallBackMethod, relativePerformanceGridCellData.CountryID, relativePerformanceGridCellData.SectorID, 1, 5);
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

        /// <summary>
        /// Callback method for RetrieveRelativePerformanceSecurityData Service call
        /// </summary>
        /// <param name="result">RelativePerformanceSecurityData Collection</param>
        public void RetrieveRelativePerformanceSecurityDataCallBackMethod(List<RelativePerformanceSecurityData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    TopDetractorInfo = new ObservableCollection<RelativePerformanceSecurityData>(result);
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
