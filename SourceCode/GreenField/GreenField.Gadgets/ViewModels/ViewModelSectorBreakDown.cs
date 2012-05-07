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

            if (EffectiveDate != null && _PortfolioSelectionData != null)
            {
                _dbInteractivity.RetrieveSectorBreakdownData(_PortfolioSelectionData, Convert.ToDateTime(_effectiveDate), RetrieveSectorBreakdownDataCallbackMethod);
            }

            if (_eventAggregator != null)
            {
                _eventAggregator.GetEvent<PortfolioReferenceSetEvent>().Subscribe(HandlePortfolioReferenceSet);
                _eventAggregator.GetEvent<EffectiveDateReferenceSetEvent>().Subscribe(HandleEffectiveDateSet);
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

        #endregion
        #endregion

        #region Event Handlers
        /// <summary>
        /// Event Handler to subscribed event 'PortfolioReferenceSetEvent'
        /// </summary>
        /// <param name="portfolioSelectionData">PortfolioSelectionData</param>
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
                    if (EffectiveDate != null && _PortfolioSelectionData != null)
                    {
                        _dbInteractivity.RetrieveSectorBreakdownData(_PortfolioSelectionData, Convert.ToDateTime(_effectiveDate), RetrieveSectorBreakdownDataCallbackMethod);
                        if (SectorBreakdownDataLoadEvent != null)
                            SectorBreakdownDataLoadEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
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
                    if (EffectiveDate != null && _PortfolioSelectionData != null)
                    {
                        _dbInteractivity.RetrieveSectorBreakdownData(_PortfolioSelectionData, Convert.ToDateTime(_effectiveDate), RetrieveSectorBreakdownDataCallbackMethod);
                        if (SectorBreakdownDataLoadEvent != null)
                            SectorBreakdownDataLoadEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
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

        #region Event
        /// <summary>
        /// event to handle data retrieval progress indicator
        /// </summary>
        public event DataRetrievalProgressIndicatorEventHandler SectorBreakdownDataLoadEvent;

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
                if (SectorBreakdownDataLoadEvent != null)
                    SectorBreakdownDataLoadEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
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
        }

        #endregion
    }
}
