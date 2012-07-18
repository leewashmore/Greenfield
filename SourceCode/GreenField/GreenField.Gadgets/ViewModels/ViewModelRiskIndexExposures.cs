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
using Microsoft.Practices.Prism.Regions;
using GreenField.DataContracts;
using GreenField.Common;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using GreenField.Gadgets.Models;
using System.Linq;
using System.Reflection;

namespace GreenField.Gadgets.ViewModels
{
    /// <summary>
    /// View Model for Risk Index Exposures
    /// </summary>
    public class ViewModelRiskIndexExposures : NotificationObject
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
        private PortfolioSelectionData _portfolioSelectionData;

        /// <summary>
        /// Private member containing the Key Value Pair
        /// </summary>
        private FilterSelectionData _holdingDataFilter;

        /// <summary>
        /// Private member to store info about including or excluding cash securities
        /// </summary>
        private bool _isExCashSecurity;

        /// <summary>
        /// Private member to store info about look thru enabled or not
        /// </summary>
        private bool _lookThruEnabled;

        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="param">DashBoardGadgetParam</param>
        public ViewModelRiskIndexExposures(DashboardGadgetParam param)
        {
            _eventAggregator = param.EventAggregator;
            _dbInteractivity = param.DBInteractivity;
            _logger = param.LoggerFacade;

            _portfolioSelectionData = param.DashboardGadgetPayload.PortfolioSelectionData;
            _isExCashSecurity = param.DashboardGadgetPayload.IsExCashSecurityData;
            EffectiveDate = param.DashboardGadgetPayload.EffectiveDate;
            _holdingDataFilter = param.DashboardGadgetPayload.FilterSelectionData;
            _lookThruEnabled = param.DashboardGadgetPayload.IsLookThruEnabled;

            if ((_portfolioSelectionData != null) && (EffectiveDate != null) && _holdingDataFilter != null && IsActive)
            {
                _dbInteractivity.RetrieveRiskIndexExposuresData(_portfolioSelectionData, Convert.ToDateTime(_effectiveDate), _isExCashSecurity, _lookThruEnabled, 
                                                        _holdingDataFilter.Filtertype, _holdingDataFilter.FilterValues, RetrieveRiskIndexExposuresDataCallbackMethod);
                BusyIndicatorStatus = true;
            }

            else if ((_portfolioSelectionData != null) && (EffectiveDate != null) && _holdingDataFilter == null && IsActive)
            {
                _dbInteractivity.RetrieveRiskIndexExposuresData(_portfolioSelectionData, Convert.ToDateTime(_effectiveDate), _isExCashSecurity, _lookThruEnabled,
                                                        "Show Everything", " ", RetrieveRiskIndexExposuresDataCallbackMethod);
                BusyIndicatorStatus = true;
            }

            if (_eventAggregator != null)
            {
                _eventAggregator.GetEvent<PortfolioReferenceSetEvent>().Subscribe(HandlePortfolioReferenceSet);
                _eventAggregator.GetEvent<EffectiveDateReferenceSetEvent>().Subscribe(HandleEffectiveDateSet);
                _eventAggregator.GetEvent<ExCashSecuritySetEvent>().Subscribe(HandleExCashSecuritySetEvent);
                _eventAggregator.GetEvent<LookThruFilterReferenceSetEvent>().Subscribe(HandleLookThruReferenceSetEvent);
                _eventAggregator.GetEvent<HoldingFilterReferenceSetEvent>().Subscribe(HandleFilterReferenceSetEvent);
            }
        } 
        #endregion

        #region Properties
        #region UI Fields      

        /// <summary>
        /// property to contain effective date value from EffectiveDate Datepicker
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
        /// contains data to be displayed in this gadget
        /// </summary>
        private ObservableCollection<RiskIndexExposuresData> _riskIndexExposuresInfo;
        public ObservableCollection<RiskIndexExposuresData> RiskIndexExposuresInfo
        {
            get { return _riskIndexExposuresInfo; }
            set
            {
                if (_riskIndexExposuresInfo != value)
                {
                    _riskIndexExposuresInfo = value;
                    RaisePropertyChanged(() => this.RiskIndexExposuresInfo);
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

        private ObservableCollection<RiskIndexExposuresChartData> myVar1;
        public ObservableCollection<RiskIndexExposuresChartData> RiskIndexExposuresChartInfo
        {
            get { return myVar1; }
            set { myVar1 = value; RaisePropertyChanged(() => RiskIndexExposuresChartInfo); }
        }

        /// <summary>
        /// Minimum Value for X-Axis of Chart
        /// </summary>
        private decimal _axisXMinValue;
        public decimal AxisXMinValue
        {
            get { return _axisXMinValue; }
            set
            {
                _axisXMinValue = value;
                this.RaisePropertyChanged(() => this.AxisXMinValue);
            }
        }

        /// <summary>
        /// Maximum Value for X-Axis of Chart
        /// </summary>
        private decimal _axisXMaxValue;
        public decimal AxisXMaxValue
        {
            get { return _axisXMaxValue; }
            set
            {
                _axisXMaxValue = value;
                this.RaisePropertyChanged(() => this.AxisXMaxValue);
            }
        }

        /// <summary>
        /// Step size of XAxis of Chart
        /// </summary>
        private int _axisXStep;
        public int AxisXStep
        {
            get { return _axisXStep; }
            set
            {
                _axisXStep = value;

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
                    if ((_portfolioSelectionData != null) && (EffectiveDate != null) && _holdingDataFilter != null && _isActive)
                    {
                        _dbInteractivity.RetrieveRiskIndexExposuresData(_portfolioSelectionData, Convert.ToDateTime(_effectiveDate), _isExCashSecurity, _lookThruEnabled,
                                                                _holdingDataFilter.Filtertype, _holdingDataFilter.FilterValues, RetrieveRiskIndexExposuresDataCallbackMethod);
                        BusyIndicatorStatus = true;
                    }

                    else if ((_portfolioSelectionData != null) && (EffectiveDate != null) && _holdingDataFilter == null && _isActive)
                    {
                        _dbInteractivity.RetrieveRiskIndexExposuresData(_portfolioSelectionData, Convert.ToDateTime(_effectiveDate), _isExCashSecurity, _lookThruEnabled,
                                                                "Show Everything", " ", RetrieveRiskIndexExposuresDataCallbackMethod);
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
                    _portfolioSelectionData = portfolioSelectionData;
                    if ((_portfolioSelectionData != null) && (EffectiveDate != null) && _holdingDataFilter != null && IsActive)
                    {
                        _dbInteractivity.RetrieveRiskIndexExposuresData(_portfolioSelectionData, Convert.ToDateTime(_effectiveDate), _isExCashSecurity, _lookThruEnabled,
                                                        _holdingDataFilter.Filtertype, _holdingDataFilter.FilterValues, RetrieveRiskIndexExposuresDataCallbackMethod);
                        BusyIndicatorStatus = true;
                    }
                    else if ((_portfolioSelectionData != null) && (EffectiveDate != null) && _holdingDataFilter == null && IsActive)
                    {
                        _dbInteractivity.RetrieveRiskIndexExposuresData(_portfolioSelectionData, Convert.ToDateTime(_effectiveDate), _isExCashSecurity, _lookThruEnabled,
                                                                "Show Everything", " ", RetrieveRiskIndexExposuresDataCallbackMethod);
                        BusyIndicatorStatus = true;
                    }

                    else
                    {
                        Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                    }
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
                    if ((_portfolioSelectionData != null) && (EffectiveDate != null) && _holdingDataFilter != null && IsActive)
                    {
                        _dbInteractivity.RetrieveRiskIndexExposuresData(_portfolioSelectionData, Convert.ToDateTime(_effectiveDate), _isExCashSecurity, _lookThruEnabled,
                                                        _holdingDataFilter.Filtertype, _holdingDataFilter.FilterValues, RetrieveRiskIndexExposuresDataCallbackMethod);
                        BusyIndicatorStatus = true;
                    }
                    else if ((_portfolioSelectionData != null) && (EffectiveDate != null) && _holdingDataFilter == null && IsActive)
                    {
                        _dbInteractivity.RetrieveRiskIndexExposuresData(_portfolioSelectionData, Convert.ToDateTime(_effectiveDate), _isExCashSecurity, _lookThruEnabled,
                                                                "Show Everything", " ", RetrieveRiskIndexExposuresDataCallbackMethod);
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
        /// Event Handler to Check for Cash Securities
        /// </summary>
        /// <param name="isExCashSec"></param>
        public void HandleExCashSecuritySetEvent(bool isExCashSec)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                Logging.LogMethodParameter(_logger, methodNamespace, isExCashSec, 1);
                if (_isExCashSecurity != isExCashSec)
                {
                    _isExCashSecurity = isExCashSec;
                    if ((_portfolioSelectionData != null) && (EffectiveDate != null) && _holdingDataFilter != null && IsActive)
                    {
                        _dbInteractivity.RetrieveRiskIndexExposuresData(_portfolioSelectionData, Convert.ToDateTime(_effectiveDate), _isExCashSecurity, _lookThruEnabled,
                                                       _holdingDataFilter.Filtertype, _holdingDataFilter.FilterValues, RetrieveRiskIndexExposuresDataCallbackMethod);
                        BusyIndicatorStatus = true;
                    }
                    else if ((_portfolioSelectionData != null) && (EffectiveDate != null) && _holdingDataFilter == null && IsActive)
                    {
                        _dbInteractivity.RetrieveRiskIndexExposuresData(_portfolioSelectionData, Convert.ToDateTime(_effectiveDate), _isExCashSecurity, _lookThruEnabled,
                                                                "Show Everything", " ", RetrieveRiskIndexExposuresDataCallbackMethod);
                        BusyIndicatorStatus = true;
                    }
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
        /// Event Handler for LookThru Status
        /// </summary>
        /// <param name="enableLookThru">True: LookThru Enabled/False: LookThru Disabled</param>
        public void HandleLookThruReferenceSetEvent(bool enableLookThru)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                Logging.LogMethodParameter(_logger, methodNamespace, enableLookThru, 1);
                _lookThruEnabled = enableLookThru;

                if ((_portfolioSelectionData != null) && (EffectiveDate != null) && _holdingDataFilter != null && IsActive)
                {
                    _dbInteractivity.RetrieveRiskIndexExposuresData(_portfolioSelectionData, Convert.ToDateTime(_effectiveDate), _isExCashSecurity, _lookThruEnabled,
                                                         _holdingDataFilter.Filtertype, _holdingDataFilter.FilterValues, RetrieveRiskIndexExposuresDataCallbackMethod);
                    BusyIndicatorStatus = true;
                }
                else if ((_portfolioSelectionData != null) && (EffectiveDate != null) && _holdingDataFilter == null && IsActive)
                {
                    _dbInteractivity.RetrieveRiskIndexExposuresData(_portfolioSelectionData, Convert.ToDateTime(_effectiveDate), _isExCashSecurity, _lookThruEnabled,
                                                            "Show Everything", " ", RetrieveRiskIndexExposuresDataCallbackMethod);
                    BusyIndicatorStatus = true;
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
        /// Assigns UI Field Properties based on Holding Filter reference
        /// </summary>
        /// <param name="filterSelectionData">Key value pair consisting of the Filter Type and Filter Value selected by the user </param>
        public void HandleFilterReferenceSetEvent(FilterSelectionData filterSelectionData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (filterSelectionData != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, filterSelectionData, 1);
                    _holdingDataFilter = filterSelectionData;
                    if (EffectiveDate != null && _portfolioSelectionData != null && _holdingDataFilter != null && IsActive)
                    {
                        _dbInteractivity.RetrieveRiskIndexExposuresData(_portfolioSelectionData, Convert.ToDateTime(_effectiveDate), _isExCashSecurity, _lookThruEnabled,
                                                        _holdingDataFilter.Filtertype, _holdingDataFilter.FilterValues, RetrieveRiskIndexExposuresDataCallbackMethod);
                        BusyIndicatorStatus = true;
                    }
                    else if ((_portfolioSelectionData != null) && (EffectiveDate != null) && _holdingDataFilter == null && IsActive)
                    {
                        _dbInteractivity.RetrieveRiskIndexExposuresData(_portfolioSelectionData, Convert.ToDateTime(_effectiveDate), _isExCashSecurity, _lookThruEnabled,
                                                                "Show Everything", " ", RetrieveRiskIndexExposuresDataCallbackMethod);
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
        /// callback method for RetrieveRiskIndexExposuresData service call
        /// </summary>
        /// <param name="riskIndexExposuresData">RiskIndexExposuresData collection</param>
        private void RetrieveRiskIndexExposuresDataCallbackMethod(List<RiskIndexExposuresData> riskIndexExposuresData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (riskIndexExposuresData != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, riskIndexExposuresData, 1);
                    RiskIndexExposuresInfo = new ObservableCollection<RiskIndexExposuresData>(riskIndexExposuresData);                   
                    RiskIndexExposuresChartInfo = new ObservableCollection<RiskIndexExposuresChartData>();
                     
                     foreach (RiskIndexExposuresData item in RiskIndexExposuresInfo)
                     {                        
                         RiskIndexExposuresChartInfo.Add(new RiskIndexExposuresChartData()
                         {
                             EntityName = item.EntityType,
                             Descriptor = "Momentum",
                             Value = item.Momentum
                         });
                         RiskIndexExposuresChartInfo.Add(new RiskIndexExposuresChartData()
                         {
                             EntityName = item.EntityType,
                             Descriptor = "Volatility",
                             Value = item.Volatility
                         });
                         RiskIndexExposuresChartInfo.Add(new RiskIndexExposuresChartData()
                         {
                             EntityName = item.EntityType,
                             Descriptor = "Value",
                             Value = item.Value
                         });
                         RiskIndexExposuresChartInfo.Add(new RiskIndexExposuresChartData()
                         {
                             EntityName = item.EntityType,
                             Descriptor = "Size",
                             Value = item.Size
                         });
                         RiskIndexExposuresChartInfo.Add(new RiskIndexExposuresChartData()
                         {
                             EntityName = item.EntityType,
                             Descriptor = "SizeNonLinear",
                             Value = item.SizeNonLinear
                         });
                         RiskIndexExposuresChartInfo.Add(new RiskIndexExposuresChartData()
                         {
                             EntityName = item.EntityType,
                             Descriptor = "Growth",
                             Value = item.Growth
                         });
                         RiskIndexExposuresChartInfo.Add(new RiskIndexExposuresChartData()
                         {
                             EntityName = item.EntityType,
                             Descriptor = "Liquidity",
                             Value = item.Liquidity
                         });
                         RiskIndexExposuresChartInfo.Add(new RiskIndexExposuresChartData()
                         {
                             EntityName = item.EntityType,
                             Descriptor = "Leverage",
                             Value = item.Leverage
                         }); 
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
            _eventAggregator.GetEvent<ExCashSecuritySetEvent>().Unsubscribe(HandleExCashSecuritySetEvent);
            _eventAggregator.GetEvent<LookThruFilterReferenceSetEvent>().Unsubscribe(HandleLookThruReferenceSetEvent);
            _eventAggregator.GetEvent<HoldingFilterReferenceSetEvent>().Unsubscribe(HandleFilterReferenceSetEvent);
        }
        #endregion
    }
}
