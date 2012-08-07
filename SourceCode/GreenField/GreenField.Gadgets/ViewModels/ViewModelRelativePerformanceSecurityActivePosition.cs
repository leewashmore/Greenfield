using System;
using System.Net;
using System.Windows;
using System.Linq;
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
    public class ViewModelRelativePerformanceSecurityActivePosition : NotificationObject
    {
        #region Fields
        //MEF Singletons
        private IEventAggregator _eventAggregator;
        private IDBInteractivity _dbInteractivity;
        private ILoggerFacade _logger;

        //Selection Data
       PortfolioSelectionData _PortfolioSelectionData;

       //To check that grid is not re populated for same values when Excess Contribution is clicked
       RelativePerformanceGridCellData checkValue = new RelativePerformanceGridCellData();
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
       /// <param name="param">DashboardGadgetParam</param>
        public ViewModelRelativePerformanceSecurityActivePosition(DashboardGadgetParam param)
        {
            _eventAggregator = param.EventAggregator;
            _dbInteractivity = param.DBInteractivity;
            _logger = param.LoggerFacade;

           _PortfolioSelectionData = param.DashboardGadgetPayload.PortfolioSelectionData;
           EffectiveDate = param.DashboardGadgetPayload.EffectiveDate;
           Period = param.DashboardGadgetPayload.PeriodSelectionData;

           if (EffectiveDate != null && _PortfolioSelectionData != null && Period != null && IsActive)
           {
               _dbInteractivity.RetrieveRelativePerformanceSecurityActivePositionData(_PortfolioSelectionData, Convert.ToDateTime(_effectiveDate), _period,RetrieveRelativePerformanceSecurityActivePositionDataCallbackMethod);
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

        private List<RelativePerformanceActivePositionData> _relativePerformanceActivePositionOrigInfo;
        public List<RelativePerformanceActivePositionData> RelativePerformanceActivePositionOrigInfo
        {
            get
            {
                if (_relativePerformanceActivePositionOrigInfo == null)
                    _relativePerformanceActivePositionOrigInfo = new List<RelativePerformanceActivePositionData>();
                return _relativePerformanceActivePositionOrigInfo;
            }
            set
            {
                if (_relativePerformanceActivePositionOrigInfo != value)
                {
                    _relativePerformanceActivePositionOrigInfo = value;
                    UpdateRelativePerformanceActivePositionInfo(value, Convert.ToBoolean(DisplayIssuerIsChecked));
                }
            }
        }

        /// <summary>
        /// Contains data to be displayed in the gadget
        /// </summary>
        private ObservableCollection<RelativePerformanceActivePositionData> _relativePerformanceActivePositionInfo;
        public ObservableCollection<RelativePerformanceActivePositionData> RelativePerformanceActivePositionInfo
        {
            get
            {
                if (_relativePerformanceActivePositionInfo == null)
                    _relativePerformanceActivePositionInfo = new ObservableCollection<RelativePerformanceActivePositionData>();
                return _relativePerformanceActivePositionInfo; 
            }
            set
            {
                if (_relativePerformanceActivePositionInfo != value)
                {
                    _relativePerformanceActivePositionInfo = value;
                    RaisePropertyChanged(() => this.RelativePerformanceActivePositionInfo);
                }
            }
        }

        private bool? _displayIssuerIsChecked = false;
        public bool? DisplayIssuerIsChecked
        {
            get { return _displayIssuerIsChecked; }
            set
            {
                if (_displayIssuerIsChecked != value)
                {
                    _displayIssuerIsChecked = value;
                    RaisePropertyChanged(() => this.DisplayIssuerIsChecked);
                    UpdateRelativePerformanceActivePositionInfo(RelativePerformanceActivePositionOrigInfo, Convert.ToBoolean(value));
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
                        _dbInteractivity.RetrieveRelativePerformanceSecurityActivePositionData(_PortfolioSelectionData, Convert.ToDateTime(_effectiveDate), _period, RetrieveRelativePerformanceSecurityActivePositionDataCallbackMethod);
                        BusyIndicatorStatus = true;
                    }
                }
            }
        }
       
        #endregion
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
                    if (EffectiveDate != null && _PortfolioSelectionData != null && Period != null && IsActive)
                    {
                        _dbInteractivity.RetrieveRelativePerformanceSecurityActivePositionData(_PortfolioSelectionData, Convert.ToDateTime(_effectiveDate), _period, RetrieveRelativePerformanceSecurityActivePositionDataCallbackMethod);
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
                    if (EffectiveDate != null && _PortfolioSelectionData != null && Period != null && IsActive)
                    {
                        _dbInteractivity.RetrieveRelativePerformanceSecurityActivePositionData(_PortfolioSelectionData, Convert.ToDateTime(_effectiveDate), _period, RetrieveRelativePerformanceSecurityActivePositionDataCallbackMethod);
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
                       _dbInteractivity.RetrieveRelativePerformanceSecurityActivePositionData(_PortfolioSelectionData, Convert.ToDateTime(_effectiveDate), _period, RetrieveRelativePerformanceSecurityActivePositionDataCallbackMethod);
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
        public void HandleRelativePerformanceGridClickEvent(RelativePerformanceGridCellData filter)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (filter != null)
                {
                    if (checkValue.CountryID != filter.CountryID || checkValue.SectorID != filter.SectorID)
                    {
                        checkValue = filter;
                        Logging.LogMethodParameter(_logger, methodNamespace, filter, 1);
                        if (_effectiveDate != null && _PortfolioSelectionData != null && Period != null && IsActive)
                        {
                            _dbInteractivity.RetrieveRelativePerformanceSecurityActivePositionData(_PortfolioSelectionData, Convert.ToDateTime(_effectiveDate), _period, RetrieveRelativePerformanceSecurityActivePositionDataCallbackMethod, filter.CountryID, filter.SectorID);
                            BusyIndicatorStatus = true;
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
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }
        #endregion

        #region Callback Methods
        /// <summary>
        /// Callback method for RetrieveRelativePerformanceSecurityActivePositionData Service call
        /// </summary>
        /// <param name="result">RelativePerformanceActivePositionData Collection</param>
        private void RetrieveRelativePerformanceSecurityActivePositionDataCallbackMethod(List<RelativePerformanceActivePositionData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    RelativePerformanceActivePositionOrigInfo = result;
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

        private void UpdateRelativePerformanceActivePositionInfo(List<RelativePerformanceActivePositionData> value, bool issuerFilter = false)
        {
            if (issuerFilter)
            {
                List<RelativePerformanceActivePositionData> groupedData = new List<RelativePerformanceActivePositionData>();
                List<String> distinctIssuers = value.Select(record => record.EntityGroup).Distinct().ToList();
                foreach (String item in distinctIssuers)
                {
                    Decimal? aggMarketValue = 0;
                    Decimal? aggFundWeight = 0;
                    Decimal? aggBenchmarkWeight = 0;

                    foreach (RelativePerformanceActivePositionData data in value.Where(g => g.EntityGroup == item))
                    {
                        aggMarketValue += data.MarketValue == null ? 0 : data.MarketValue;
                        aggFundWeight += data.FundWeight == null ? 0 : data.FundWeight;
                        aggBenchmarkWeight += data.BenchmarkWeight == null ? 0 : data.BenchmarkWeight;
                    }

                    groupedData.Add(new RelativePerformanceActivePositionData()
                    {
                        Entity = item,
                        EntityGroup = null,
                        MarketValue = aggMarketValue,
                        FundWeight = aggFundWeight,
                        BenchmarkWeight = aggBenchmarkWeight,
                        ActivePosition = aggFundWeight - aggBenchmarkWeight
                    });

                    RelativePerformanceActivePositionInfo = new ObservableCollection<RelativePerformanceActivePositionData>(groupedData);
                }

            }
            else
            {
                //List<RelativePerformanceActivePositionData> unGroupedData = new List<RelativePerformanceActivePositionData>();

                //foreach (RelativePerformanceActivePositionData item in value)
                //{
                //    unGroupedData.Add(new RelativePerformanceActivePositionData()
                //    {
                //        Entity = item.Entity,
                //        EntityGroup = item.EntityGroup,
                //        MarketValue = item.MarketValue,
                //        FundWeight = item.FundWeight,
                //        BenchmarkWeight = item.BenchmarkWeight,
                //        ActivePosition = item.ActivePosition
                //    });
                //}

                RelativePerformanceActivePositionInfo = new ObservableCollection<RelativePerformanceActivePositionData>(value);
            }
        }

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
