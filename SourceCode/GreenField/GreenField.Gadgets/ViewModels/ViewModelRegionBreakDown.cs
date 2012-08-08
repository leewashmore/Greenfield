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
using GreenField.ServiceCaller.SecurityReferenceDefinitions;
using System.Linq;
using GreenField.Common;
using Microsoft.Practices.Prism.ViewModel;
using GreenField.Gadgets.Models;
using System.Collections.Generic;
using GreenField.ServiceCaller.BenchmarkHoldingsDefinitions;
using GreenField.DataContracts;

namespace GreenField.Gadgets.ViewModels
{
    /// <summary>
    /// view model for ViewRegionBreakDown
    /// </summary>
    public class ViewModelRegionBreakdown : NotificationObject
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
        private PortfolioSelectionData _PortfolioSelectionData;

       
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="param">DashBoardGadgetParam</param>
        public ViewModelRegionBreakdown(DashboardGadgetParam param)
        {
            _eventAggregator = param.EventAggregator;
            _dbInteractivity = param.DBInteractivity;
            _logger = param.LoggerFacade;

            _PortfolioSelectionData = param.DashboardGadgetPayload.PortfolioSelectionData;
            EffectiveDate = param.DashboardGadgetPayload.EffectiveDate;
            IsExCashSecurity = param.DashboardGadgetPayload.IsExCashSecurityData;
            LookThruEnabled = param.DashboardGadgetPayload.IsLookThruEnabled;

            if ((_PortfolioSelectionData != null) && (EffectiveDate != null) && IsActive)
            {
                _dbInteractivity.RetrieveRegionBreakdownData(_PortfolioSelectionData, Convert.ToDateTime(_effectiveDate),_isExCashSecurity,_lookThruEnabled, RetrieveRegionBreakdownDataCallbackMethod);
                BusyIndicatorStatus = true;
            }

            if (_eventAggregator != null)
            {
                _eventAggregator.GetEvent<PortfolioReferenceSetEvent>().Subscribe(HandlePortfolioReferenceSet);
                _eventAggregator.GetEvent<EffectiveDateReferenceSetEvent>().Subscribe(HandleEffectiveDateSet);
                _eventAggregator.GetEvent<ExCashSecuritySetEvent>().Subscribe(HandleExCashSecuritySetEvent);
                _eventAggregator.GetEvent<LookThruFilterReferenceSetEvent>().Subscribe(HandleLookThruReferenceSetEvent);
            }
        }
        #endregion

        #region Properties
        #region UI Fields

        /// <summary>
        /// Private member to store info about including or excluding cash securities
        /// </summary>
        private bool _isExCashSecurity = false;
        public bool  IsExCashSecurity
        {
            get { return _isExCashSecurity; }
            set
            {
                if (_isExCashSecurity != value)
                {
                    _isExCashSecurity = value;
                    RaisePropertyChanged(() => IsExCashSecurity);
                }
            }
        }
        

        /// <summary>
        /// Private member to store info about look thru enabled or not
        /// </summary>
        private bool _lookThruEnabled = false;
        public bool LookThruEnabled
        {
            get { return _lookThruEnabled; }
            set
            {
                if (_lookThruEnabled != value)
                {
                    _lookThruEnabled = value;
                    RaisePropertyChanged(() => LookThruEnabled);
                }
            }
        }

        /// <summary>
        /// contains data for the grid in the gadget
        /// </summary>
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

        /// <summary>
        /// contains data for the chart in the gadget
        /// </summary>
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
                    if ((_PortfolioSelectionData != null) && (EffectiveDate != null) && _isActive)
                    {
                        _dbInteractivity.RetrieveRegionBreakdownData(_PortfolioSelectionData, Convert.ToDateTime(_effectiveDate), _isExCashSecurity, _lookThruEnabled, RetrieveRegionBreakdownDataCallbackMethod);
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
                    if ((_PortfolioSelectionData != null) && (EffectiveDate != null) && IsActive)
                    {
                        _dbInteractivity.RetrieveRegionBreakdownData(_PortfolioSelectionData, Convert.ToDateTime(_effectiveDate), _isExCashSecurity, _lookThruEnabled, RetrieveRegionBreakdownDataCallbackMethod);
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
                    if ((_PortfolioSelectionData != null) && (EffectiveDate != null) && IsActive)
                    {
                        _dbInteractivity.RetrieveRegionBreakdownData(_PortfolioSelectionData, Convert.ToDateTime(_effectiveDate), _isExCashSecurity, _lookThruEnabled, RetrieveRegionBreakdownDataCallbackMethod);
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
                if (IsExCashSecurity != isExCashSec)
                    {
                        IsExCashSecurity = isExCashSec;

                        if ((_PortfolioSelectionData != null) && (EffectiveDate != null) && IsActive)
                        {
                            _dbInteractivity.RetrieveRegionBreakdownData(_PortfolioSelectionData, Convert.ToDateTime(_effectiveDate), _isExCashSecurity, _lookThruEnabled, RetrieveRegionBreakdownDataCallbackMethod);
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
                if (LookThruEnabled != enableLookThru)
                {
                    LookThruEnabled = enableLookThru;
                    if ((_PortfolioSelectionData != null) && (EffectiveDate != null) && IsActive)
                    {
                        _dbInteractivity.RetrieveRegionBreakdownData(_PortfolioSelectionData, Convert.ToDateTime(_effectiveDate), _isExCashSecurity, _lookThruEnabled, RetrieveRegionBreakdownDataCallbackMethod);
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
        #endregion
               
        #region Callback Methods

        /// <summary>
        /// Callback method for RetrieveRegionBreakdownData service call
        /// </summary>
        /// <param name="regionBreakdownData">RegionBreakdownData collection</param>
        private void RetrieveRegionBreakdownDataCallbackMethod(List<RegionBreakdownData> regionBreakdownData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (regionBreakdownData != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, regionBreakdownData, 1);
                    List<RegionBreakdownData> templist = new List<RegionBreakdownData>(regionBreakdownData);
                    foreach (RegionBreakdownData item in templist)
                    {
                        if (item.Region == "Not classified" || item.Region == null || item.Region == string.Empty || item.Region == " ")
                            item.Region = CapitalizeFirstLetterAfterSpace("Not classified");

                        if(item.Country == null || item.Country == string.Empty || item.Country == " ")
                            item.Country = CapitalizeFirstLetterAfterSpace("Not classified");
                    }


                    RegionBreakdownInfo = new ObservableCollection<RegionBreakdownData>(templist);
                   
                    RegionSpecificInfo = new ObservableCollection<RegionSpecificData>();
                    foreach (RegionBreakdownData item in RegionBreakdownInfo)
                    {
                        
                        if (RegionSpecificInfo.Where(i => i.Region == item.Region).Count().Equals(0))
                        {
                            RegionSpecificInfo.Add(new RegionSpecificData()
                            {
                                Region = item.Region,
                                PortfolioShare = RegionBreakdownInfo.Where(t => t.Region == item.Region).Sum(r => r.PortfolioShare),
                                BenchmarkShare = RegionBreakdownInfo.Where(t => t.Region == item.Region).Sum(r => r.BenchmarkShare)
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

        #region Helper Methods
        public string CapitalizeFirstLetterAfterSpace(string value)
        {
            char[] array = value.ToCharArray();

            // Scan through the letters, checking for spaces.
            // ... Uppercase the lowercase letters following spaces.
            for (int i = 1; i < array.Length; i++)
            {
                if (array[i - 1] == ' ')
                {
                    if (char.IsLower(array[i]))
                    {
                        array[i] = char.ToUpper(array[i]);
                    }
                }
            }
            return new string(array);
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
        }

        #endregion
    }
}
