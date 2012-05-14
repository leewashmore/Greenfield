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
using System.Linq;
using System.Collections.Generic;
using Microsoft.Practices.Prism.ViewModel;
using GreenField.ServiceCaller.BenchmarkHoldingsDefinitions;
using GreenField.ServiceCaller.PerformanceDefinitions;
using System.Collections.ObjectModel;

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
        public PortfolioSelectionData _PortfolioSelectionData;

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
            _PortfolioSelectionData = param.DashboardGadgetPayload.PortfolioSelectionData;
            EffectiveDate = param.DashboardGadgetPayload.EffectiveDate;
            _period = param.DashboardGadgetPayload.PeriodSelectionData;

            //Service Call to Retrieve Sector Data relating Fund Selection Data/ Benchmark Selection Data and Effective Date
            if (_effectiveDate != null && _PortfolioSelectionData != null && Period != null)
            {
                _dbInteractivity.RetrieveRelativePerformanceSectorData(_PortfolioSelectionData, Convert.ToDateTime(_effectiveDate), RetrieveRelativePerformanceSectorDataCallbackMethod);
            }

            if (_eventAggregator != null)
            {
                _eventAggregator.GetEvent<PortfolioReferenceSetEvent>().Subscribe(HandlePortfolioReferenceSet);
                _eventAggregator.GetEvent<EffectiveDateReferenceSetEvent>().Subscribe(HandleEffectiveDateSet);
                _eventAggregator.GetEvent<PeriodReferenceSetEvent>().Subscribe(HandlePeriodReferenceSet);
                _eventAggregator.GetEvent<RelativePerformanceGridCountrySectorClickEvent>().Subscribe(HandleRelativePerformanceGridCountrySectorClickEvent);
            }
        } 
        #endregion

        #region Properties

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

        private ObservableCollection<RelativePerformanceSecurityData> _securityDetails;
        public ObservableCollection<RelativePerformanceSecurityData> SecurityDetails
        {
            get { return _securityDetails; }
            set
            {
                if (_securityDetails != value)
                {
                    _securityDetails = value;
                    RaisePropertyChanged(() => SecurityDetails);
                }
            }
        }        

        #endregion

        #region Events
        public event RelativePerformanceGridBuildEventHandler RelativePerformanceGridBuildEvent;

        /// <summary>
        /// event to handle data retrieval progress indicator
        /// </summary>
        public event DataRetrievalProgressIndicatorEventHandler RelativePerformanceDataLoadEvent;

        #endregion

        #region Event Handlers
        public void HandlePortfolioReferenceSet(PortfolioSelectionData PortfolioSelectionData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);

            try
            {
                if (PortfolioSelectionData != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, PortfolioSelectionData, 1);
                    _PortfolioSelectionData = PortfolioSelectionData;
                    if (EffectiveDate != null && _PortfolioSelectionData != null && _period != null)
                    {
                        _dbInteractivity.RetrieveRelativePerformanceSectorData(_PortfolioSelectionData, Convert.ToDateTime(_effectiveDate), RetrieveRelativePerformanceSectorDataCallbackMethod);
                        if (RelativePerformanceDataLoadEvent != null)
                            RelativePerformanceDataLoadEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
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
                    EffectiveDate = effectiveDate;
                    if (EffectiveDate != null && _PortfolioSelectionData != null && _period != null)
                    {
                        _dbInteractivity.RetrieveRelativePerformanceSectorData(_PortfolioSelectionData,Convert.ToDateTime(_effectiveDate), RetrieveRelativePerformanceSectorDataCallbackMethod);
                        if (RelativePerformanceDataLoadEvent != null)
                            RelativePerformanceDataLoadEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
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
                    if (EffectiveDate != null && _PortfolioSelectionData != null && _period != null)
                    {
                        _dbInteractivity.RetrieveRelativePerformanceSectorData(_PortfolioSelectionData, Convert.ToDateTime(_effectiveDate), RetrieveRelativePerformanceSectorDataCallbackMethod);
                        if (RelativePerformanceDataLoadEvent != null)
                            RelativePerformanceDataLoadEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
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
        public void HandleRelativePerformanceGridCountrySectorClickEvent(RelativePerformanceGridCellData relativePerformanceGridCellData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (relativePerformanceGridCellData != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, relativePerformanceGridCellData, 1);
                    if (EffectiveDate != null && _PortfolioSelectionData != null)
                    {
                        if(relativePerformanceGridCellData.SectorID == null)
                        _dbInteractivity.RetrieveRelativePerformanceSecurityData(_PortfolioSelectionData, Convert.ToDateTime(_effectiveDate), _period, RetrieveRelativePerformanceSecurityDataCallBackMethod, relativePerformanceGridCellData.CountryID, relativePerformanceGridCellData.SectorID);
                        else if(relativePerformanceGridCellData.CountryID == null)
                            _dbInteractivity.RetrieveRelativePerformanceSecurityData(_PortfolioSelectionData, Convert.ToDateTime(_effectiveDate), _period, RetrieveRelativePerformanceSecurityDataCallBackMethod, relativePerformanceGridCellData.CountryID, relativePerformanceGridCellData.SectorID);
                        if (RelativePerformanceDataLoadEvent != null)
                            RelativePerformanceDataLoadEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
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
                    //Service Call to Retrieve Performance Data relating Fund Selection Data and Effective Date
                    _dbInteractivity.RetrieveRelativePerformanceData(_PortfolioSelectionData, Convert.ToDateTime(_effectiveDate),_period, RetrieveRelativePerformanceDataCallbackMethod);                    
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
                if (RelativePerformanceDataLoadEvent != null)
                    RelativePerformanceDataLoadEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

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
                    SecurityDetails = new ObservableCollection<RelativePerformanceSecurityData>(result);
                }
                else
                {
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                }
                if (RelativePerformanceDataLoadEvent != null)
                    RelativePerformanceDataLoadEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
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
        }

        #endregion
    }
}
