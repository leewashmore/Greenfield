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
using GreenField.DataContracts;

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
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="param">DashboardGadgetParam</param>
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

        /// <summary>
        /// Contains security level data to be displayed in grids shown on toggling
        /// </summary>
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
        /// <summary>
        /// Event handling for relative performance gadget grid building
        /// </summary>
        public event RelativePerformanceGridBuildEventHandler RelativePerformanceGridBuildEvent;

        /// <summary>
        /// Event handling for building grid shown on click over sector name in relative performance gadget grid  
        /// </summary>
        public event RelativePerformanceToggledSectorGridBuildEventHandler RelativePerformanceToggledSectorGridBuildEvent;

        /// <summary>
        /// event handling for data retrieval progress indicator
        /// </summary>
        public event DataRetrievalProgressIndicatorEventHandler RelativePerformanceDataLoadEvent;

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
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        /// <summary>
        /// Event Handler to subscribed event 'RelativePerformanceGridCountrySectorClickEvent'
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
                        _dbInteractivity.RetrieveRelativePerformanceSecurityData(_PortfolioSelectionData, Convert.ToDateTime(_effectiveDate), _period, RetrieveRelativePerformanceSecurityDataCallbackMethod, relativePerformanceGridCellData.CountryID, relativePerformanceGridCellData.SectorID);
                        else if(relativePerformanceGridCellData.CountryID == null)
                            _dbInteractivity.RetrieveRelativePerformanceSecurityData(_PortfolioSelectionData, Convert.ToDateTime(_effectiveDate), _period, RetrieveRelativePerformanceSecurityDataCallbackMethod, relativePerformanceGridCellData.CountryID, relativePerformanceGridCellData.SectorID);
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
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        #endregion

        #region Callback Methods
        /// <summary>
        /// Callback method for RetrieveRelativePerformanceSectorData Service call
        /// </summary>
        /// <param name="result">RelativePerformanceSectorData Collection</param>
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
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        /// <summary>
        /// Callback method for RetrieveRelativePerformanceData Service call
        /// </summary>
        /// <param name="result">RelativePerformanceData Collection</param>
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
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        /// <summary>
        /// Callback method for RetrieveRelativePerformanceSecurityData Service call
        /// </summary>
        /// <param name="result">RelativePerformanceSecurityData Collection</param>
        public void RetrieveRelativePerformanceSecurityDataCallbackMethod(List<RelativePerformanceSecurityData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    SecurityDetails = new ObservableCollection<RelativePerformanceSecurityData>(result);

                    RelativePerformanceToggledSectorGridBuildEvent.Invoke(new RelativePerformanceToggledSectorGridBuildEventArgs()
                    {
                        RelativePerformanceCountryNameInfo = SecurityDetails.Select(r => r.SecurityCountryId).Distinct().ToList(),
                        RelativePerformanceSecurityInfo = SecurityDetails.ToList()
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
            _eventAggregator.GetEvent<RelativePerformanceGridCountrySectorClickEvent>().Unsubscribe(HandleRelativePerformanceGridCountrySectorClickEvent);
        }

        #endregion
    }
}
