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
using GreenField.ServiceCaller.BenchmarkHoldingsDefinitions;
using GreenField.ServiceCaller.PerformanceDefinitions;
using GreenField.DataContracts;

namespace GreenField.Gadgets.ViewModels
{
    public class ViewModelRelativePerformanceSectorActivePosition : NotificationObject
    {
        #region Fields
        //MEF Singletons
        private IEventAggregator _eventAggregator;
        private IDBInteractivity _dbInteractivity;
        private ILoggerFacade _logger;

        // Selection Data
        private PortfolioSelectionData _PortfolioSelectionData;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="param">DashboardGadgetParam</param>
        public ViewModelRelativePerformanceSectorActivePosition(DashboardGadgetParam param)
        {
            _eventAggregator = param.EventAggregator;
            _dbInteractivity = param.DBInteractivity;
            _logger = param.LoggerFacade;

            _PortfolioSelectionData = param.DashboardGadgetPayload.PortfolioSelectionData;
            _eventAggregator.GetEvent<PeriodReferenceSetEvent>().Subscribe(HandlePeriodReferenceSet);
            EffectiveDate = param.DashboardGadgetPayload.EffectiveDate;

            if (_effectiveDate != null && _PortfolioSelectionData != null && Period != null)
            {
                _dbInteractivity.RetrieveRelativePerformanceSectorActivePositionData(_PortfolioSelectionData,Convert.ToDateTime(_effectiveDate),_period, RetrieveRelativePerformanceSectorActivePositionDataCallbackMethod);
            }
            
            if (_eventAggregator != null)
            {
                _eventAggregator.GetEvent<PortfolioReferenceSetEvent>().Subscribe(HandlePortfolioReferenceSet);
                _eventAggregator.GetEvent<EffectiveDateReferenceSetEvent>().Subscribe(HandleEffectiveDateSet);
                _eventAggregator.GetEvent<PeriodReferenceSetEvent>().Subscribe(HandlePeriodReferenceSet);
                _eventAggregator.GetEvent<RelativePerformanceGridClickEvent>().Subscribe(HandleRelativePerformanceGridClickEvent);
            }
        }
        #endregion

        #region Properties
        #region UI Fields
        /// <summary>
        /// Contains data to be displayed in the gadget
        /// </summary>
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

        /// <summary>
        /// Effective date selected
        /// </summary>
        private DateTime? _effectiveDate;
        public DateTime? EffectiveDate
        {
            get { return _effectiveDate; }
            set
            {
                if (_effectiveDate != value)
                {
                    _effectiveDate = value;
                    RaisePropertyChanged(() => EffectiveDate);
                }
            }
        }

        /// <summary>
        /// Period selected
        /// </summary>
        private string _period;
        public string Period
        {
            get { return _period; }
            set
            {
                if (_period != value)
                {
                    _period = value;
                    RaisePropertyChanged(() => Period);
                }
            }
        }        
        #endregion
        #endregion

        #region Events
        /// <summary>
        /// event handling for data retrieval progress indicator
        /// </summary>
        public event DataRetrievalProgressIndicatorEventHandler SectorActivePositionDataLoadEvent;

        #endregion

        #region Event Handlers
        /// <summary>
        /// Event Handler to subscribed event 'PortfolioReferenceSetEvent'
        /// </summary>
        /// <param name="portfolioSelectionData">PortfolioSelectionData</param>
        public void HandlePortfolioReferenceSet(PortfolioSelectionData portfolioSelectionData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);

            try
            {
                if (portfolioSelectionData != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, portfolioSelectionData, 1);
                    _PortfolioSelectionData = portfolioSelectionData;
                    if (EffectiveDate != null && _PortfolioSelectionData != null && Period != null)
                    {
                        _dbInteractivity.RetrieveRelativePerformanceSectorActivePositionData(_PortfolioSelectionData,Convert.ToDateTime(_effectiveDate),_period, RetrieveRelativePerformanceSectorActivePositionDataCallbackMethod);
                        if (SectorActivePositionDataLoadEvent != null)
                            SectorActivePositionDataLoadEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
                    }
                }
                else
                {
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        /// <summary>
        /// Event Handler to subscribed event 'EffectiveDateSet'
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
                    EffectiveDate = effectiveDate;
                    if (EffectiveDate != null && _PortfolioSelectionData != null && Period != null)
                    {
                        _dbInteractivity.RetrieveRelativePerformanceSectorActivePositionData(_PortfolioSelectionData, Convert.ToDateTime(_effectiveDate), _period, RetrieveRelativePerformanceSectorActivePositionDataCallbackMethod);
                        if (SectorActivePositionDataLoadEvent != null)
                            SectorActivePositionDataLoadEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
                    }
                }
                else
                {
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        /// <summary>
        /// Event Handler to subscribed event 'PeriodReferenceSetEvent'
        /// </summary>
        /// <param name="period"></param>
        public void HandlePeriodReferenceSet(string period)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (period != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, period, 1);
                    Period = period;
                    if (EffectiveDate != null && _PortfolioSelectionData != null && Period != null)
                    {
                       _dbInteractivity.RetrieveRelativePerformanceSectorActivePositionData(_PortfolioSelectionData, Convert.ToDateTime(_effectiveDate), _period, RetrieveRelativePerformanceSectorActivePositionDataCallbackMethod);

                        if (SectorActivePositionDataLoadEvent != null)
                            SectorActivePositionDataLoadEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
                    }
                }
                else
                {
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        /// <summary>
        /// Event Handler to subscribed event 'RelativePerformanceGridClickEvent'
        /// </summary>
        /// <param name="relativePerformanceGridCellData">RelativePerformanceGridCellData</param>
        public void HandleRelativePerformanceGridClickEvent(RelativePerformanceGridCellData filter)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (filter != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, filter, 1);
                    if (_effectiveDate != null && _PortfolioSelectionData != null && Period != null)
                    {
                        _dbInteractivity.RetrieveRelativePerformanceSectorActivePositionData(_PortfolioSelectionData, Convert.ToDateTime(_effectiveDate),_period, RetrieveRelativePerformanceSectorActivePositionDataCallbackMethod, filter.CountryID, filter.SectorID);
                        if (SectorActivePositionDataLoadEvent != null)
                            SectorActivePositionDataLoadEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
                    }
                }
                else
                {
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }
        #endregion

        #region Callback Methods
        /// <summary>
        /// Callback method for RetrieveRelativePerformanceSectorActivePositionData Service call
        /// </summary>
        /// <param name="result">RelativePerformanceActivePositionData Collection</param>
        private void RetrieveRelativePerformanceSectorActivePositionDataCallbackMethod(List<RelativePerformanceActivePositionData> result)
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
                if (SectorActivePositionDataLoadEvent != null)
                    SectorActivePositionDataLoadEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }
        #endregion

        #region Dispose Method
        /// <summary>
        /// method to dispose all subscribed events
        /// </summary>
        public void Dispose()
        {
            _eventAggregator.GetEvent<PortfolioReferenceSetEvent>().Unsubscribe(HandlePortfolioReferenceSet);
            _eventAggregator.GetEvent<EffectiveDateReferenceSetEvent>().Unsubscribe(HandleEffectiveDateSet);
            _eventAggregator.GetEvent<PeriodReferenceSetEvent>().Unsubscribe(HandlePeriodReferenceSet);
            _eventAggregator.GetEvent<RelativePerformanceGridClickEvent>().Unsubscribe(HandleRelativePerformanceGridClickEvent);
        }
        #endregion
    }
}
