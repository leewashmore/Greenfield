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
    /// <summary>
    /// Class that provides the interaction of the view model with the Service caller and the View.
    /// </summary>
    public class ViewModelTopBenchmarkSecurities : NotificationObject
    {
        #region PrivateMembers

        /// <summary>
        /// private member object of the IEventAggregator for event aggregation
        /// </summary>
        private IEventAggregator _eventAggregator;

        /// <summary>
        /// private member object of the IDBInteractivity for interaction with the Service Caller
        /// </summary>
        private IDBInteractivity _dbInteractivity;

        /// <summary>
        /// private member object of ILoggerFacade for logging
        /// </summary>
        private ILoggerFacade _logger;

        /// <summary>
        /// private member object of the BenchmarkSelectionData class for storing Benchmark Selection Data
        /// </summary>
        private BenchmarkSelectionData _benchmarkSelectionData;

        /// <summary>
        /// Contains the effective date
        /// </summary>
        private DateTime _effectiveDate;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor of the class that initializes various objects
        /// </summary>
        /// <param name="param">MEF Eventaggrigator instance</param>
        public ViewModelTopBenchmarkSecurities(DashboardGadgetParam param)
        {
            _eventAggregator = param.EventAggregator;
            _dbInteractivity = param.DBInteractivity;
            _logger = param.LoggerFacade;

            _benchmarkSelectionData = param.DashboardGadgetPayload.BenchmarkSelectionData;
            _effectiveDate = param.DashboardGadgetPayload.EffectiveDate;

            if (_eventAggregator != null)
            {
                _eventAggregator.GetEvent<BenchmarkReferenceSetEvent>().Subscribe(HandleBenchmarkReferenceSet);
                _eventAggregator.GetEvent<EffectiveDateSet>().Subscribe(HandleEffectiveDateSet);
            }

            if (_effectiveDate != null && _benchmarkSelectionData != null)
            {
                _dbInteractivity.RetrieveTopBenchmarkSecuritiesData(_benchmarkSelectionData, _effectiveDate, RetrieveTopSecuritiesDataCallbackMethod);
            }
           // _dbInteractivity.RetrieveTopBenchmarkSecuritiesData(_benchmarkSelectionData, _effectiveDate, RetrieveTopBenchmarkSecuritiesDataCallbackMethod);
            
        }
        #endregion

        #region Properties
        /// <summary>
        /// Collection containing Top Ten Benchmark Securities binded to grid 
        /// </summary>
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

        #region Events
        /// <summary>
        /// Event for the notification of Data Load Completion
        /// </summary>
        public event DataRetrievalProgressIndicatorEventHandler topTenBenchmarkSecuritiesDataLoadedEvent;
        #endregion

        #region Event Handlers

        /// <summary>
        /// Assigns UI Field Properties based on Effective Date
        /// </summary>
        /// <param name="effectiveDate"></param>
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
                        _dbInteractivity.RetrieveTopBenchmarkSecuritiesData(_benchmarkSelectionData, _effectiveDate, RetrieveTopSecuritiesDataCallbackMethod);
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
        /// Assigns UI Field Properties based on Benchmark reference
        /// </summary>
        /// <param name="benchmarkSelectionData">Object of BenchmarkSelectionData Class containg Benchmark data</param>
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
                        _dbInteractivity.RetrieveTopBenchmarkSecuritiesData(_benchmarkSelectionData, _effectiveDate, RetrieveTopSecuritiesDataCallbackMethod);
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
        /// Plots the result in the grid after getting the resulting collection
        /// </summary>
        /// <param name="result">Contains the Top Ten Benchmark Securities Data</param>
        public void RetrieveTopSecuritiesDataCallbackMethod(List<TopBenchmarkSecuritiesData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    TopBenchmarkSecuritiesInfo = new ObservableCollection<TopBenchmarkSecuritiesData>(result);
                    if (null != topTenBenchmarkSecuritiesDataLoadedEvent)
                        topTenBenchmarkSecuritiesDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
                }
                else
                {
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                    topTenBenchmarkSecuritiesDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
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
