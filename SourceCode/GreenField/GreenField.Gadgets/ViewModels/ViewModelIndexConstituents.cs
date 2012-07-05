﻿using System;
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
using GreenField.Common;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Practices.Prism.ViewModel;
using GreenField.ServiceCaller.BenchmarkHoldingsDefinitions;
using GreenField.DataContracts;

namespace GreenField.Gadgets.ViewModels
{
    /// <summary>
    /// View model for ViewIndexConstituents class
    /// </summary>
    public class ViewModelIndexConstituents : NotificationObject
    {
        #region Fields
        /// <summary>
        /// MEF Singletons
        /// </summary>
        private IEventAggregator _eventAggregator;
        private IDBInteractivity _dbInteractivity;
        private ILoggerFacade _logger;

        /// <summary>
        /// Private member to store info about look thru enabled or not
        /// </summary>
        private bool _lookThruEnabled = false;

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
                    if ((_portfolioSelectionData != null) && (EffectiveDate != null) && _isActive)
                    {
                        _dbInteractivity.RetrieveIndexConstituentsData(_portfolioSelectionData, Convert.ToDateTime(_effectiveDate), _lookThruEnabled, RetrieveIndexConstituentsDataCallbackMethod);
                        BusyIndicatorStatus = true;
                    }
                }
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="param">DashboardGadgetparam</param>
        public ViewModelIndexConstituents(DashboardGadgetParam param)
        {
            _eventAggregator = param.EventAggregator;
            _dbInteractivity = param.DBInteractivity;
            _logger = param.LoggerFacade;

            PortfolioSelectionData = param.DashboardGadgetPayload.PortfolioSelectionData;
            EffectiveDate = param.DashboardGadgetPayload.EffectiveDate;
            _lookThruEnabled = param.DashboardGadgetPayload.IsLookThruEnabled;
            if ((_portfolioSelectionData != null) && (EffectiveDate != null) && IsActive)
            {
                _dbInteractivity.RetrieveIndexConstituentsData(_portfolioSelectionData, Convert.ToDateTime(_effectiveDate),_lookThruEnabled, RetrieveIndexConstituentsDataCallbackMethod);
                BusyIndicatorStatus = true;
            }
            if (_eventAggregator != null)
            {
                _eventAggregator.GetEvent<PortfolioReferenceSetEvent>().Subscribe(HandlePortfolioReferenceSet);
                _eventAggregator.GetEvent<EffectiveDateReferenceSetEvent>().Subscribe(HandleEffectiveDateSet);
                _eventAggregator.GetEvent<LookThruFilterReferenceSetEvent>().Subscribe(HandleLookThruReferenceSetEvent);
            }
        }
        #endregion

        #region Properties
        #region UI Fields

        /// <summary>
        /// contains all data to be displayed in the gadget
        /// </summary>
        private ObservableCollection<IndexConstituentsData> _indexConstituentsInfo;
        public ObservableCollection<IndexConstituentsData> IndexConstituentsInfo
        {
            get { return _indexConstituentsInfo; }
            set
            {
                if (_indexConstituentsInfo != value)
                {
                    _indexConstituentsInfo = value;
                    RaisePropertyChanged(() => this.IndexConstituentsInfo);
                }
            }
        }

        /// <summary>
        /// DashboardGadgetPayLoad field
        /// </summary>
        private PortfolioSelectionData _portfolioSelectionData;
        public PortfolioSelectionData PortfolioSelectionData
        {
            get { return _portfolioSelectionData; }
            set
            {
                if (_portfolioSelectionData != value)
                {
                    _portfolioSelectionData = value;
                    RaisePropertyChanged(() => PortfolioSelectionData);
                }
            }
        }


        /// <summary>
        /// effective date selected
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
                    RaisePropertyChanged(() => this.EffectiveDate);
                }
            }
        }

        /// <summary>
        /// benchmarkId for portfolio selected
        /// </summary>
        private string _benchmarkId;
        public string BenchmarkId
        {
            get { return _benchmarkId; }
            set 
            {
                if (_benchmarkId != value)
                {
                    _benchmarkId = value;
                    RaisePropertyChanged(() => BenchmarkId);
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
        #endregion

        #region Event Handlers

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
                    if (EffectiveDate != null && PortfolioSelectionData != null && IsActive)
                    {
                        _dbInteractivity.RetrieveIndexConstituentsData(_portfolioSelectionData, Convert.ToDateTime(_effectiveDate),_lookThruEnabled, RetrieveIndexConstituentsDataCallbackMethod);
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
                    PortfolioSelectionData = portfolioSelectionData;
                    if (EffectiveDate != null && PortfolioSelectionData != null && IsActive)
                    {
                        _dbInteractivity.RetrieveIndexConstituentsData(_portfolioSelectionData, Convert.ToDateTime(_effectiveDate), _lookThruEnabled, RetrieveIndexConstituentsDataCallbackMethod);
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

                if (EffectiveDate != null && PortfolioSelectionData != null && IsActive)
                {
                    _dbInteractivity.RetrieveIndexConstituentsData(_portfolioSelectionData, Convert.ToDateTime(_effectiveDate), _lookThruEnabled, RetrieveIndexConstituentsDataCallbackMethod);
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
        /// Callback method for RetrieveIndexConstituentsData service call
        /// </summary>
        /// <param name="indexConstituentsData">IndexConstituentsData collection</param>
        private void RetrieveIndexConstituentsDataCallbackMethod(List<IndexConstituentsData> indexConstituentsData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (indexConstituentsData != null && indexConstituentsData.Count != 0)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, indexConstituentsData, 1);
                    IndexConstituentsInfo = new ObservableCollection<IndexConstituentsData>(indexConstituentsData);
                    BenchmarkId = IndexConstituentsInfo.ElementAt(0).BenchmarkId.ToString();
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
            _eventAggregator.GetEvent<LookThruFilterReferenceSetEvent>().Unsubscribe(HandleLookThruReferenceSetEvent);
        }

        #endregion
    }

   
}
