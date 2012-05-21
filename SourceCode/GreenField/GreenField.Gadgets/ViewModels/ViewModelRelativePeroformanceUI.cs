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
using GreenField.ServiceCaller;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Events;
using GreenField.ServiceCaller.SecurityReferenceDefinitions;
using GreenField.ServiceCaller.BenchmarkHoldingsDefinitions;
using System.Collections.Generic;
using GreenField.Gadgets.Helpers;
using GreenField.ServiceCaller.PerformanceDefinitions;
using System.Collections.ObjectModel;
using GreenField.DataContracts;

namespace GreenField.Gadgets.ViewModels
{
    public class ViewModelRelativePeroformanceUI : NotificationObject
    {
        #region Private Fields

        /// <summary>
        /// MEF Singletons
        /// </summary>
        private IDBInteractivity _dbInteractivity;
        private ILoggerFacade _logger;
        private IEventAggregator _eventAggregator;
        private PortfolioSelectionData _portfolioSelectionData;
        private EntitySelectionData _entitySelectionData;
        private DateTime? _effectiveDate;

        #endregion

        #region Constructor

        public ViewModelRelativePeroformanceUI(DashboardGadgetParam param)
        {
            _dbInteractivity = param.DBInteractivity;
            _logger = param.LoggerFacade;
            _eventAggregator = param.EventAggregator;

            _portfolioSelectionData = param.DashboardGadgetPayload.PortfolioSelectionData;
            _entitySelectionData = param.DashboardGadgetPayload.EntitySelectionData;
            _effectiveDate = param.DashboardGadgetPayload.EffectiveDate;

            if (_portfolioSelectionData != null)
                HandleFundReferenceSet(_portfolioSelectionData);
            if (_entitySelectionData != null)
                HandleSecurityReferenceSet(_entitySelectionData);
            if (_effectiveDate != null)
                HandleEffectiveDateSet(Convert.ToDateTime(_effectiveDate));

            if (_eventAggregator != null)
            {
                _eventAggregator.GetEvent<SecurityReferenceSetEvent>().Subscribe(HandleSecurityReferenceSet, false);
                _eventAggregator.GetEvent<PortfolioReferenceSetEvent>().Subscribe(HandleFundReferenceSet);
                _eventAggregator.GetEvent<EffectiveDateReferenceSetEvent>().Subscribe(HandleEffectiveDateSet);
            }
        }

        #endregion

        #region PropertyDeclaration

        private EntitySelectionData _selectedSecurity;
        public EntitySelectionData SelectedSecurity
        {
            get
            {
                return _selectedSecurity;
            }
            set
            {
                _selectedSecurity = value;
                this.RaisePropertyChanged(() => this.SelectedSecurity);
            }
        }

        private PortfolioSelectionData _selectedPortfolio;
        public PortfolioSelectionData SelectedPortfolio
        {
            get
            {
                return _selectedPortfolio;
            }
            set
            {
                _selectedPortfolio = value;
                this.RaisePropertyChanged(() => this.SelectedPortfolio);
            }
        }

        private Dictionary<string, string> _selectedEntityValues;
        public Dictionary<string, string> SelectedEntityValues
        {
            get
            {
                if (_selectedEntityValues == null)
                    _selectedEntityValues = new Dictionary<string, string>();
                return _selectedEntityValues;
            }
            set
            {
                _selectedEntityValues = value;
                this.RaisePropertyChanged(() => this.SelectedEntityValues);
            }
        }

        private DateTime _selectedDate;
        public DateTime SelectedDate
        {
            get
            {
                return _selectedDate;
            }
            set
            {
                _selectedDate = value;
                this.RaisePropertyChanged(() => this.SelectedDate);
            }
        }

        private RangeObservableCollection<RelativePerformanceUIData> _relativePerformanceReturnData;
        public RangeObservableCollection<RelativePerformanceUIData> RelativePerformanceReturnData
        {
            get
            {
                if (_relativePerformanceReturnData == null)
                    _relativePerformanceReturnData = new RangeObservableCollection<RelativePerformanceUIData>();
                return _relativePerformanceReturnData;
            }
            set
            {
                _relativePerformanceReturnData = value;
                this.RaisePropertyChanged(() => this.RelativePerformanceReturnData);
            }
        }

        #endregion

        #region Events

        public event DataRetrievalProgressIndicatorEventHandler RelativePerformanceReturnDataLoadedEvent;

        #endregion

        #region EventHandlers

        /// <summary>
        /// Handle Fund Change Event
        /// </summary>
        /// <param name="PortfolioSelectionData">Details of Selected Portfolio</param>
        public void HandleFundReferenceSet(PortfolioSelectionData PortfolioSelectionData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);

            try
            {
                if (PortfolioSelectionData != null)
                {
                    if (SelectedEntityValues.ContainsKey("PORTFOLIO"))
                        SelectedEntityValues.Remove("PORTFOLIO");
                    Logging.LogMethodParameter(_logger, methodNamespace, PortfolioSelectionData, 1);
                    SelectedPortfolio = PortfolioSelectionData;
                    SelectedEntityValues.Add("PORTFOLIO", PortfolioSelectionData.PortfolioId);

                    if (SelectedSecurity != null && SelectedDate != null && SelectedPortfolio != null && SelectedEntityValues != null)
                    {
                        _dbInteractivity.RetrieveRelativePerformanceUIData(SelectedEntityValues, SelectedDate, RelativePerformanceUIDataCallbackMethod);
                        if (null != RelativePerformanceReturnDataLoadedEvent)
                            RelativePerformanceReturnDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
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
        /// Handle Security Change Event
        /// </summary>
        /// <param name="PortfolioSelectionData">Details of Selected Security</param>
        public void HandleSecurityReferenceSet(EntitySelectionData entitySelectionData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);

            try
            {
                if (entitySelectionData != null)
                {
                    if (entitySelectionData.InstrumentID == null)
                        throw new Exception("Security Data Cannot be Fetched for this Security");

                    if (SelectedEntityValues.ContainsKey("SECURITY"))
                        SelectedEntityValues.Remove("SECURITY");
                    Logging.LogMethodParameter(_logger, methodNamespace, entitySelectionData, 1);
                    SelectedSecurity = entitySelectionData;
                    SelectedEntityValues.Add("SECURITY", entitySelectionData.InstrumentID);

                    if (SelectedPortfolio != null && SelectedDate != null && SelectedSecurity != null && SelectedEntityValues != null)
                    {
                        _dbInteractivity.RetrieveRelativePerformanceUIData(SelectedEntityValues, SelectedDate, RelativePerformanceUIDataCallbackMethod);
                        if (null != RelativePerformanceReturnDataLoadedEvent)
                            RelativePerformanceReturnDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
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
        /// Handle Date Change Event
        /// </summary>
        /// <param name="effectiveDate">Effective Date</param>
        public void HandleEffectiveDateSet(DateTime effectiveDate)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (effectiveDate != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, effectiveDate, 1);
                    SelectedDate = effectiveDate;
                    if (SelectedDate != null && SelectedEntityValues != null && SelectedSecurity != null && SelectedPortfolio != null)
                    {
                        _dbInteractivity.RetrieveRelativePerformanceUIData(SelectedEntityValues, SelectedDate, RelativePerformanceUIDataCallbackMethod);
                        if (null != RelativePerformanceReturnDataLoadedEvent)
                            RelativePerformanceReturnDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
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

        #region CallbackMethods

        private void RelativePerformanceUIDataCallbackMethod(List<RelativePerformanceUIData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (null != RelativePerformanceReturnDataLoadedEvent)
                    RelativePerformanceReturnDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });

                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    RelativePerformanceReturnData.Clear();
                    RelativePerformanceReturnData.AddRange(result);
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

    }
}