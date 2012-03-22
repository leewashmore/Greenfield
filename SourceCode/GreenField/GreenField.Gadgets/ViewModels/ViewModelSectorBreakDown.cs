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
using System.Collections.ObjectModel;
using System.Linq;
using GreenField.Common;
using System.Collections.Generic;
using Microsoft.Practices.Prism.ViewModel;
using GreenField.Gadgets.Models;

namespace GreenField.Gadgets.ViewModels
{
    public class ViewModelSectorBreakDown : NotificationObject
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
        public ViewModelSectorBreakDown(DashBoardGadgetParam param)
        {
            _eventAggregator = param.EventAggregator;
            _dbInteractivity = param.DBInteractivity;
            _logger = param.LoggerFacade;

            _fundSelectionData = param.DashboardGadgetPayLoad.FundSelectionData;
            _benchmarkSelectionData = param.DashboardGadgetPayLoad.BenchmarkSelectionData;
            _effectiveDate = param.DashboardGadgetPayLoad.EffectiveDate;

            if (_effectiveDate != null && _fundSelectionData != null && _benchmarkSelectionData != null)
            {
                _dbInteractivity.RetrieveSectorBreakdownData(_fundSelectionData, _benchmarkSelectionData, _effectiveDate, RetrieveSectorBreakdownDataCallbackMethod);
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
        private ObservableCollection<SectorBreakdownData> _sectorBreakdownInfo;
        public ObservableCollection<SectorBreakdownData> SectorBreakdownInfo
        {
            get { return _sectorBreakdownInfo; }
            set
            {
                if (_sectorBreakdownInfo != value)
                {
                    _sectorBreakdownInfo = value;
                    RaisePropertyChanged(() => this.SectorBreakdownInfo);
                }
            }
        }

        private ObservableCollection<SectorSpecificData> _sectorSpecificInfo;
        public ObservableCollection<SectorSpecificData> SectorSpecificInfo
        {
            get { return _sectorSpecificInfo; }
            set
            {
                if (_sectorSpecificInfo != value)
                {
                    _sectorSpecificInfo = value;
                    RaisePropertyChanged(() => this.SectorSpecificInfo);
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
                        _dbInteractivity.RetrieveSectorBreakdownData(_fundSelectionData, _benchmarkSelectionData, _effectiveDate, RetrieveSectorBreakdownDataCallbackMethod);
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
                        _dbInteractivity.RetrieveSectorBreakdownData(_fundSelectionData, _benchmarkSelectionData, _effectiveDate, RetrieveSectorBreakdownDataCallbackMethod);
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
                        _dbInteractivity.RetrieveSectorBreakdownData(_fundSelectionData, _benchmarkSelectionData, _effectiveDate, RetrieveSectorBreakdownDataCallbackMethod);
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
        private void RetrieveSectorBreakdownDataCallbackMethod(List<SectorBreakdownData> sectorBreakdownData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (sectorBreakdownData != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, sectorBreakdownData, 1);
                    SectorBreakdownInfo = new ObservableCollection<SectorBreakdownData>(sectorBreakdownData);
                    foreach (SectorBreakdownData item in SectorBreakdownInfo)
                    {
                        if (SectorSpecificInfo == null)
                        {
                            SectorSpecificInfo = new ObservableCollection<SectorSpecificData>();
                        }
                        if (SectorSpecificInfo.Where(i => i.Sector == item.Sector).Count().Equals(0))
                        {
                            SectorSpecificInfo.Add(new SectorSpecificData()
                            {
                                Sector = item.Sector,
                                PortfolioShare = SectorBreakdownInfo.Where(t => t.Sector == item.Sector).Sum(r => r.PortfolioShare)
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
