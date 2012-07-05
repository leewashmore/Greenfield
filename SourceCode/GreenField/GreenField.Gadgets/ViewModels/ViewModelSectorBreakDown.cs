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
using System.Collections.ObjectModel;
using System.Linq;
using GreenField.Common;
using System.Collections.Generic;
using Microsoft.Practices.Prism.ViewModel;
using GreenField.Gadgets.Models;
using GreenField.ServiceCaller.BenchmarkHoldingsDefinitions;
using GreenField.DataContracts;

namespace GreenField.Gadgets.ViewModels
{
    /// <summary>
    /// view model for ViewSectorBreakDown
    /// </summary>
    public class ViewModelSectorBreakdown : NotificationObject
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

        /// <summary>
        /// Private member to store info about including or excluding cash securities
        /// </summary>
        private bool _isExCashSecurity = false;

        /// <summary>
        /// Private member to store info about look thru enabled or not
        /// </summary>
        private bool _lookThruEnabled = false;
                
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="param">DashBoardGadgetParam</param>
        public ViewModelSectorBreakdown(DashboardGadgetParam param)
        {
            _eventAggregator = param.EventAggregator;
            _dbInteractivity = param.DBInteractivity;
            _logger = param.LoggerFacade;

            _PortfolioSelectionData = param.DashboardGadgetPayload.PortfolioSelectionData;
            EffectiveDate = param.DashboardGadgetPayload.EffectiveDate;
            _isExCashSecurity = param.DashboardGadgetPayload.IsExCashSecurityData;
            _lookThruEnabled = param.DashboardGadgetPayload.IsLookThruEnabled;

            if ((_PortfolioSelectionData != null) && (EffectiveDate != null) && IsActive)
            {
                _dbInteractivity.RetrieveSectorBreakdownData(_PortfolioSelectionData, Convert.ToDateTime(_effectiveDate),_isExCashSecurity,_lookThruEnabled, RetrieveSectorBreakdownDataCallbackMethod);
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
        /// contains data for the grid in the gadget
        /// </summary>
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

        /// <summary>
        /// contains data for the chart in the gadget
        /// </summary>
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
                        _dbInteractivity.RetrieveSectorBreakdownData(_PortfolioSelectionData, Convert.ToDateTime(_effectiveDate), _isExCashSecurity, _lookThruEnabled, RetrieveSectorBreakdownDataCallbackMethod);
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
                         _dbInteractivity.RetrieveSectorBreakdownData(_PortfolioSelectionData, Convert.ToDateTime(_effectiveDate),_isExCashSecurity,_lookThruEnabled, RetrieveSectorBreakdownDataCallbackMethod);
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
                        _dbInteractivity.RetrieveSectorBreakdownData(_PortfolioSelectionData, Convert.ToDateTime(_effectiveDate), _isExCashSecurity, _lookThruEnabled, RetrieveSectorBreakdownDataCallbackMethod);
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
                if (isExCashSec != null)
                {
                    _isExCashSecurity = isExCashSec;

                    if ((_PortfolioSelectionData != null) && (EffectiveDate != null) && IsActive)
                    {
                        _dbInteractivity.RetrieveSectorBreakdownData(_PortfolioSelectionData, Convert.ToDateTime(_effectiveDate), _isExCashSecurity, _lookThruEnabled, RetrieveSectorBreakdownDataCallbackMethod);
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

                if ((_PortfolioSelectionData != null) && (EffectiveDate != null) && IsActive)
                {
                    _dbInteractivity.RetrieveSectorBreakdownData(_PortfolioSelectionData, Convert.ToDateTime(_effectiveDate), _isExCashSecurity, _lookThruEnabled, RetrieveSectorBreakdownDataCallbackMethod);
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
        #endregion              

        #region Callback Methods
        /// <summary>
        /// Callback method for RetrieveSectorBreakdownData service call
        /// </summary>
        /// <param name="sectorBreakdownData">SectorBreakdownData collection</param>
        private void RetrieveSectorBreakdownDataCallbackMethod(List<SectorBreakdownData> sectorBreakdownData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (sectorBreakdownData != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, sectorBreakdownData, 1);
                    List<SectorBreakdownData> templist = new List<SectorBreakdownData>(sectorBreakdownData);
                    foreach (SectorBreakdownData item in templist)
                    {
                        if (item.Sector == "Not classified" || item.Sector == null || item.Sector == string.Empty || item.Sector == " ")
                            item.Sector = CapitalizeFirstLetterAfterSpace("Not classified");

                        if(item.Industry == null || item.Industry == string.Empty || item.Industry == " ")
                            item.Industry = CapitalizeFirstLetterAfterSpace("Not classified");
                    }

                    SectorBreakdownInfo = new ObservableCollection<SectorBreakdownData>(templist);

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
                                PortfolioShare = SectorBreakdownInfo.Where(t => t.Sector == item.Sector).Sum(r => r.PortfolioShare),
                                BenchmarkShare = SectorBreakdownInfo.Where(t => t.Sector == item.Sector).Sum(r => r.BenchmarkShare)
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
