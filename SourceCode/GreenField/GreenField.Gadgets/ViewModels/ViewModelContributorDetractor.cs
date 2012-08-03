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
using GreenField.Common;
using GreenField.ServiceCaller.SecurityReferenceDefinitions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Practices.Prism.Events;
using GreenField.ServiceCaller;
using Microsoft.Practices.Prism.Logging;
using GreenField.ServiceCaller.BenchmarkHoldingsDefinitions;
using GreenField.ServiceCaller.PerformanceDefinitions;
using GreenField.DataContracts;

namespace GreenField.Gadgets.ViewModels
{
    /// <summary>
    /// View model for ViewContributorDetractor class
    /// </summary>
    public class ViewModelContributorDetractor : NotificationObject
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
        PortfolioSelectionData _PortfolioSelectionData;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="param">DashboardGadgetparam</param>
        public ViewModelContributorDetractor(DashboardGadgetParam param)
        {
            _eventAggregator = param.EventAggregator;
            _dbInteractivity = param.DBInteractivity;
            _logger = param.LoggerFacade;

            _PortfolioSelectionData = param.DashboardGadgetPayload.PortfolioSelectionData;
            EffectiveDate = param.DashboardGadgetPayload.EffectiveDate;
            _period = param.DashboardGadgetPayload.PeriodSelectionData;

            if (EffectiveDate != null && _PortfolioSelectionData != null && Period != null && IsActive)
            {
                _dbInteractivity.RetrieveRelativePerformanceSecurityData(_PortfolioSelectionData, Convert.ToDateTime(_effectiveDate),_period, RetrieveRelativePerformanceSecurityDataCallbackMethod);
                BusyIndicatorStatus = true;
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
        /// IsActive is true when parent control is displayed on UI
        /// </summary>
        private bool _isActive;
        public bool IsActive
        {
            get
            {
                return _isActive;
            }
            set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    if (EffectiveDate != null && _PortfolioSelectionData != null && Period != null && _isActive)
                    {
                        _dbInteractivity.RetrieveRelativePerformanceSecurityData(_PortfolioSelectionData, Convert.ToDateTime(_effectiveDate), _period, RetrieveRelativePerformanceSecurityDataCallbackMethod);
                        BusyIndicatorStatus = true;
                    }
                }
            }
        }
        #endregion

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
        /// contains all data to be displayed in the gadget.
        /// </summary>
        private ObservableCollection<RelativePerformanceSecurityData> _contributorDetractorInfo;
        public ObservableCollection<RelativePerformanceSecurityData> ContributorDetractorInfo
        {
            get { return _contributorDetractorInfo; }
            set 
            {
                if (_contributorDetractorInfo != value)
                {
                    _contributorDetractorInfo = value;
                    RaisePropertyChanged(() => ContributorDetractorInfo);

                }
            }
        }

        /// <summary>
        /// property to contain status value for busy indicator of the gadget
        /// </summary>
        private bool _busyIndicatorStatus;
        public bool BusyIndicatorStatus
        {
            get { return _busyIndicatorStatus; }
            set
            {
                if (_busyIndicatorStatus != value)
                {
                    _busyIndicatorStatus = value;
                    RaisePropertyChanged(() => BusyIndicatorStatus);
                }
            }
        }
        
        #endregion    
              

        #region Event Handlers

        /// <summary>
        /// Event Handler to subscribed event 'FundReferenceSetEvent'
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
                    if (EffectiveDate != null && _PortfolioSelectionData != null && Period != null && IsActive)
                    {
                        _dbInteractivity.RetrieveRelativePerformanceSecurityData(_PortfolioSelectionData, Convert.ToDateTime(_effectiveDate),_period, RetrieveRelativePerformanceSecurityDataCallbackMethod);
                        BusyIndicatorStatus = true;
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
                    EffectiveDate = effectiveDate;
                    if (EffectiveDate != null && _PortfolioSelectionData != null && Period != null && IsActive)
                    {
                        _dbInteractivity.RetrieveRelativePerformanceSecurityData(_PortfolioSelectionData, Convert.ToDateTime(_effectiveDate),_period, RetrieveRelativePerformanceSecurityDataCallbackMethod);
                        BusyIndicatorStatus = true;
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
                    if (EffectiveDate != null && _PortfolioSelectionData != null && Period != null && IsActive)
                    {
                       _dbInteractivity.RetrieveRelativePerformanceSecurityData(_PortfolioSelectionData, Convert.ToDateTime(_effectiveDate), _period, RetrieveRelativePerformanceSecurityDataCallbackMethod);
                       BusyIndicatorStatus = true;
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
        public void HandleRelativePerformanceGridClickEvent(RelativePerformanceGridCellData relativePerformanceGridCellData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (relativePerformanceGridCellData != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, relativePerformanceGridCellData, 1);
                    if (EffectiveDate != null && _PortfolioSelectionData != null && IsActive)
                    {
                        _dbInteractivity.RetrieveRelativePerformanceSecurityData(_PortfolioSelectionData, Convert.ToDateTime(_effectiveDate),_period, RetrieveRelativePerformanceSecurityDataCallbackMethod, relativePerformanceGridCellData.CountryID, relativePerformanceGridCellData.SectorID);
                        BusyIndicatorStatus = true;
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
                    ContributorDetractorInfo = new ObservableCollection<RelativePerformanceSecurityData>(result);
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
            finally
            {
                BusyIndicatorStatus = false;
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
