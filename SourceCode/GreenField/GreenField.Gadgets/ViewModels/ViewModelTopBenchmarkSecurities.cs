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
using GreenField.ServiceCaller;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Events;
using GreenField.ServiceCaller.SecurityReferenceDefinitions;
using System.Collections.Generic;
using Microsoft.Practices.Prism.ViewModel;
using GreenField.Common;
using System.Collections.ObjectModel;
using GreenField.ServiceCaller.BenchmarkHoldingsDefinitions;

namespace GreenField.Gadgets.ViewModels
{
    /// <summary>
    /// Class that provides the interaction of the view model with the Service caller and the View.
    /// </summary>
    public class ViewModelTopBenchmarkSecurities : NotificationObject
    {
        #region PrivateMembers

        /// <summary>
        /// private member object of the IEventAggregator for event aggregation
        /// </summary>
        private IEventAggregator _eventAggregator;

        /// <summary>
        /// private member object of the IDBInteractivity for interaction with the Service Caller
        /// </summary>
        private IDBInteractivity _dbInteractivity;

        /// <summary>
        /// private member object of ILoggerFacade for logging
        /// </summary>
        private ILoggerFacade _logger;

        /// <summary>
        /// private member object of the PortfolioSelectionData class for storing Benchmark Selection Data
        /// </summary>
        private PortfolioSelectionData _portfolioSelectionData;

        /// <summary>
        /// Contains the effective date
        /// </summary>
        private DateTime? _effectiveDate;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor of the class that initializes various objects
        /// </summary>
        /// <param name="param">MEF Eventaggrigator instance</param>
        public ViewModelTopBenchmarkSecurities(DashboardGadgetParam param)
        {
            _eventAggregator = param.EventAggregator;
            _dbInteractivity = param.DBInteractivity;
            _logger = param.LoggerFacade;

            _portfolioSelectionData = param.DashboardGadgetPayload.PortfolioSelectionData;
            _effectiveDate = param.DashboardGadgetPayload.EffectiveDate;

            if (_eventAggregator != null)
            {
                _eventAggregator.GetEvent<PortfolioReferenceSetEvent>().Subscribe(HandlePortfolioReferenceSet);
                _eventAggregator.GetEvent<EffectiveDateReferenceSetEvent>().Subscribe(HandleEffectiveDateSet);
            }

            if (_effectiveDate != null && _portfolioSelectionData != null)
            {
                _dbInteractivity.RetrieveTopBenchmarkSecuritiesData(_portfolioSelectionData, Convert.ToDateTime(_effectiveDate), RetrieveTopSecuritiesDataCallbackMethod);
            }
           // _dbInteractivity.RetrieveTopBenchmarkSecuritiesData(_benchmarkSelectionData, _effectiveDate, RetrieveTopBenchmarkSecuritiesDataCallbackMethod);
            
        }
        #endregion

        #region Properties
        /// <summary>
        /// Collection containing Top Ten Benchmark Securities binded to grid 
        /// </summary>
        private ObservableCollection<TopBenchmarkSecuritiesData> _topBenchmarkSecuritiesInfo;
        public ObservableCollection<TopBenchmarkSecuritiesData> TopBenchmarkSecuritiesInfo
        {
            get { return _topBenchmarkSecuritiesInfo; }
            set
            {
                _topBenchmarkSecuritiesInfo = value;
                RaisePropertyChanged(() => this.TopBenchmarkSecuritiesInfo);
            }
        }

        /// <summary>
        /// Collection that contains the filter types to be displayed in the combo box
        /// </summary>
        public ObservableCollection<String> FilterTypes
        {
            get
            {
                return new ObservableCollection<string> { "Region", "Country", "Industry", "Sector" };
            }
        }

        /// <summary>
        /// String that contains the selected filter type
        /// </summary>
        private String _filterTypesSelection;
        public String FilterTypesSelection
        {
            get
            {
                return _filterTypesSelection;
            }
            set
            {

                _filterTypesSelection = value;
                //_dbInteractivity.RetriveValuesForFilters(_filterTypesSelection, RetrieveValuesForFiltersCallbackMethod);
                RaisePropertyChanged(() => this.FilterTypesSelection);
            }
        }
        /// <summary>
        ///  Collection that contains the value types to be displayed in the combo box
        /// </summary>
        private List<String> _valueTypes;
        public List<String> ValueTypes
        {
            get { return _valueTypes; }
            set
            {
                if (_valueTypes != value)
                {
                    _valueTypes = value;

                    RaisePropertyChanged(() => this.ValueTypes);
                }
            }
        }

        /// <summary>
        /// String that contains the selected value type
        /// </summary>
        private String _valueTypesSelection;
        public String ValueTypesSelection
        {
            get { return _valueTypesSelection; }
            set
            {
                if (_valueTypesSelection != value)
                {
                    _valueTypesSelection = value;

                    if (_portfolioSelectionData != null)
                    {
                        _dbInteractivity.RetrieveTopBenchmarkSecuritiesData(_portfolioSelectionData, Convert.ToDateTime(_effectiveDate), RetrieveTopSecuritiesDataCallbackMethod);

                    }
                    RaisePropertyChanged(() => this.ValueTypesSelection);
                }

            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Event for the notification of Data Load Completion
        /// </summary>
        public event DataRetrievalProgressIndicatorEventHandler topTenBenchmarkSecuritiesDataLoadedEvent;
        #endregion

        #region Event Handlers

        /// <summary>
        /// Assigns UI Field Properties based on Effective Date
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
                    _effectiveDate = effectiveDate;
                    if (_effectiveDate != null && _portfolioSelectionData != null)
                    {
                        if (null != topTenBenchmarkSecuritiesDataLoadedEvent)
                            topTenBenchmarkSecuritiesDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
                        _dbInteractivity.RetrieveTopBenchmarkSecuritiesData(_portfolioSelectionData, Convert.ToDateTime(_effectiveDate), RetrieveTopSecuritiesDataCallbackMethod);
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
        /// Assigns UI Field Properties based on Benchmark reference
        /// </summary>
        /// <param name="benchmarkSelectionData">Object of BenchmarkSelectionData Class containg Benchmark data</param>
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
                    if (_effectiveDate != null && _portfolioSelectionData != null)
                    {
                        if (null != topTenBenchmarkSecuritiesDataLoadedEvent)
                            topTenBenchmarkSecuritiesDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
                        _dbInteractivity.RetrieveTopBenchmarkSecuritiesData(_portfolioSelectionData, Convert.ToDateTime(_effectiveDate), RetrieveTopSecuritiesDataCallbackMethod);
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

        /// <summary>
        /// Plots the result in the grid after getting the resulting collection
        /// </summary>
        /// <param name="result">Contains the Top Ten Benchmark Securities Data</param>
        public void RetrieveTopSecuritiesDataCallbackMethod(List<TopBenchmarkSecuritiesData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    TopBenchmarkSecuritiesInfo = new ObservableCollection<TopBenchmarkSecuritiesData>(result);
                    if (null != topTenBenchmarkSecuritiesDataLoadedEvent)
                        topTenBenchmarkSecuritiesDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
                }
                else
                {
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                    topTenBenchmarkSecuritiesDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
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
        /// Callback method that assigns value to ValueTypes
        /// </summary>
        /// <param name="result">Contains the list of value types for a selected region</param>
        public void RetrieveValuesForFiltersCallbackMethod(List<String> result)
        {
            ValueTypes = result;
        }

        #region EventUnSubscribe

        public void Dispose()
        {
            _eventAggregator.GetEvent<PortfolioReferenceSetEvent>().Unsubscribe(HandlePortfolioReferenceSet);
            _eventAggregator.GetEvent<EffectiveDateReferenceSetEvent>().Unsubscribe(HandleEffectiveDateSet);
        }
        #endregion
        #endregion       
    }
}
