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
using System.Collections.ObjectModel;
using Microsoft.Practices.Prism.Events;
using GreenField.ServiceCaller;
using Microsoft.Practices.Prism.Logging;
using GreenField.ServiceCaller.ProxyDataDefinitions;
using System.Linq;
using GreenField.Common;
using Microsoft.Practices.Prism.ViewModel;
using GreenField.Gadgets.Models;
using System.Collections.Generic;

namespace GreenField.Gadgets.ViewModels
{
    public class ViewModelRegionBreakDown : NotificationObject
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
        public ViewModelRegionBreakDown(DashboardGadgetParam param)
        {
            _eventAggregator = param.EventAggregator;
            _dbInteractivity = param.DBInteractivity;
            _logger = param.LoggerFacade;

            _fundSelectionData = param.DashboardGadgetPayload.FundSelectionData;
            _benchmarkSelectionData = param.DashboardGadgetPayload.BenchmarkSelectionData;
            _effectiveDate = param.DashboardGadgetPayload.EffectiveDate;

            if (_effectiveDate != null && _fundSelectionData != null && _benchmarkSelectionData != null)
            {
                _dbInteractivity.RetrieveRegionBreakdownData(_fundSelectionData, _benchmarkSelectionData, _effectiveDate, RetrieveRegionBreakdownDataCallbackMethod);
            }

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
        private ObservableCollection<RegionBreakdownData> _regionBreakdownInfo;
        public ObservableCollection<RegionBreakdownData> RegionBreakdownInfo
        {
            get { return _regionBreakdownInfo; }
            set
            {
                if (_regionBreakdownInfo != value)
                {
                    _regionBreakdownInfo = value;
                    RaisePropertyChanged(() => this.RegionBreakdownInfo);
                }
            }
        }

        private ObservableCollection<RegionSpecificData> _regionSpecificInfo;
        public ObservableCollection<RegionSpecificData> RegionSpecificInfo
        {
            get { return _regionSpecificInfo; }
            set
            {
                if (_regionSpecificInfo != value)
                {
                    _regionSpecificInfo = value;
                    RaisePropertyChanged(() => this.RegionSpecificInfo);
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
                        _dbInteractivity.RetrieveRegionBreakdownData(_fundSelectionData, _benchmarkSelectionData, _effectiveDate, RetrieveRegionBreakdownDataCallbackMethod);
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
                        _dbInteractivity.RetrieveRegionBreakdownData(_fundSelectionData, _benchmarkSelectionData, _effectiveDate, RetrieveRegionBreakdownDataCallbackMethod);
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
                        _dbInteractivity.RetrieveRegionBreakdownData(_fundSelectionData, _benchmarkSelectionData, _effectiveDate, RetrieveRegionBreakdownDataCallbackMethod);
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
        private void RetrieveRegionBreakdownDataCallbackMethod(List<RegionBreakdownData> regionBreakdownData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (regionBreakdownData != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, regionBreakdownData, 1);
                    RegionBreakdownInfo = new ObservableCollection<RegionBreakdownData>(regionBreakdownData);
                    foreach (RegionBreakdownData item in RegionBreakdownInfo)
                    {
                        if (RegionSpecificInfo == null)
                        {
                            RegionSpecificInfo = new ObservableCollection<RegionSpecificData>();
                        }
                        if (RegionSpecificInfo.Where(i => i.Region == item.Region).Count().Equals(0))
                        {
                            RegionSpecificInfo.Add(new RegionSpecificData()
                            {
                                Region = item.Region,
                                PortfolioShare = RegionBreakdownInfo.Where(t => t.Region == item.Region).Sum(r => r.PortfolioShare)
                            });
                        }
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
    }
}
