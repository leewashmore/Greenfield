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
using System.Collections.Generic;
using GreenField.ServiceCaller.SecurityReferenceDefinitions;
using Microsoft.Practices.Prism.ViewModel;
using GreenField.Common;
using System.Collections.ObjectModel;
using GreenField.ServiceCaller.BenchmarkHoldingsPerformanceDefinitions;

namespace GreenField.Gadgets.ViewModels
{
    /// <summary>
    /// Class that provides the interaction of the view model with the Service caller and the View.
    /// </summary>
    public class ViewModelHoldingsPieChart : NotificationObject
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
        /// private member object of the PortfolioSelectionData class for storing Fund Selection Data
        /// </summary>
        private PortfolioSelectionData _PortfolioSelectionData;  
      
        #endregion

       #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="param">MEF Eventaggrigator instance</param>
        public ViewModelHoldingsPieChart(DashboardGadgetParam param)
        {
            _dbInteractivity = param.DBInteractivity;
            _logger = param.LoggerFacade;
            _eventAggregator = param.EventAggregator;           
            _PortfolioSelectionData = param.DashboardGadgetPayload.PortfolioSelectionData;
            EffectiveDate = param.DashboardGadgetPayload.EffectiveDate;

            if (EffectiveDate != null && _PortfolioSelectionData != null)
            {
                _dbInteractivity.RetrieveHoldingsPercentageData(_PortfolioSelectionData, EffectiveDate, FilterTypesSelection, ValueTypesSelection, RetrieveHoldingsPercentageDataCallbackMethod);
            }

            if (_eventAggregator != null)
            {
                _eventAggregator.GetEvent<PortfolioReferenceSetEvent>().Subscribe(HandleFundReferenceSet);
                _eventAggregator.GetEvent<EffectiveDateReferenceSetEvent>().Subscribe(HandleEffectiveDateSet);
            }           

            //if (_benchmarkSelectionData != null && EffectiveDate != null)
            //{
            //    _dbInteractivity.RetrieveHoldingsPercentageData(_benchmarkSelectionData, EffectiveDate, RetrieveHoldingsPercentageDataCallbackMethod);
            //}
         //   _dbInteractivity.RetrieveHoldingsPercentageData(_benchmarkSelectionData, EffectiveDate, RetrieveHoldingsPercentageDataCallbackMethod);


            
            //_dbInteractivity.RetrieveIndexConstituentsData(_benchmarkSelectionData, _effectiveDate, RetrieveIndexConstituentsDataCallbackMethod);
           
        }
        #endregion

       #region Properties
        #region UI Fields

        /// <summary>
        /// Collection that contains the holdings data to be binded to the sector chart
        /// </summary>
        private ObservableCollection<HoldingsPercentageData> _holdingsPercentageInfo;
        public ObservableCollection<HoldingsPercentageData> HoldingsPercentageInfo
        {
            get { return _holdingsPercentageInfo; }
            set
            {
                _holdingsPercentageInfo = value;
                RaisePropertyChanged(()=> this.HoldingsPercentageInfo);
            }
        }

        /// <summary>
        /// Effective date appended by as of
        /// </summary>
        public String EffectiveDateString
        {
            get
            {
                return "as of " + EffectiveDate.ToLongDateString();
            }
        }

        /// <summary>
        ///Effective Date as selected by the user 
        /// </summary>
        private DateTime _effectiveDate;
        public DateTime EffectiveDate
        {
            get { return _effectiveDate; }
            set
            {
                if (_effectiveDate != value)
                {
                    _effectiveDate = value;

                    if (_PortfolioSelectionData != null && EffectiveDate != null)
                    {
                        _dbInteractivity.RetrieveHoldingsPercentageData(_PortfolioSelectionData, EffectiveDate, FilterTypesSelection, ValueTypesSelection, RetrieveHoldingsPercentageDataCallbackMethod);

                    }

                    RaisePropertyChanged(() => this.EffectiveDate);
                }
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
                    _dbInteractivity.RetriveValuesForFilters(_filterTypesSelection, RetrieveValuesForFiltersCallbackMethod); 
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

                    if (_PortfolioSelectionData != null && EffectiveDate != null)
                    {
                        _dbInteractivity.RetrieveHoldingsPercentageData(_PortfolioSelectionData, EffectiveDate, FilterTypesSelection, ValueTypesSelection, RetrieveHoldingsPercentageDataCallbackMethod);                      

                    }
                    RaisePropertyChanged(() => this.ValueTypesSelection);
                }  
                
            }
        }

        #endregion
        #endregion

       #region Events
        /// <summary>
        /// Event for the notification of Data Load Completion
        /// </summary>
        public event DataRetrievalProgressIndicatorEventHandler holdingsPieChartDataLoadedEvent;
        #endregion

       #region Event Handlers
        /// <summary>
        /// Assigns UI Field Properties based on Selected Effective Date
        /// </summary>
        /// <param name="effectiveDate">Effectice date as selected by the user</param>
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
                        _dbInteractivity.RetrieveHoldingsPercentageData(_PortfolioSelectionData, EffectiveDate, FilterTypesSelection, ValueTypesSelection, RetrieveHoldingsPercentageDataCallbackMethod);
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
        /// Assigns UI Field Properties based on Fund reference
        /// </summary>
        /// <param name="PortfolioSelectionData">Object of PortfolioSelectionData Class</param>
        public void HandleFundReferenceSet(PortfolioSelectionData PortfolioSelectionData)
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
                        if (null != holdingsPieChartDataLoadedEvent)
                            holdingsPieChartDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
                        _dbInteractivity.RetrieveHoldingsPercentageData(PortfolioSelectionData, EffectiveDate, FilterTypesSelection, ValueTypesSelection, RetrieveHoldingsPercentageDataCallbackMethod);
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
        /// Callback method that assigns value to the HoldingsPercentageInfo property
        /// </summary>
        /// <param name="result">contains the holdings data for the sector pie chart</param>
        public void RetrieveHoldingsPercentageDataCallbackMethod(List<HoldingsPercentageData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try 
            {
                if (result != null)
                {

                    HoldingsPercentageInfo = new ObservableCollection<HoldingsPercentageData>(result);
                    if (null != holdingsPieChartDataLoadedEvent)
                        holdingsPieChartDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
                }
                else
                {
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                    holdingsPieChartDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
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

        #endregion

       #region EventUnSubscribe

        public void Dispose()
        {
            _eventAggregator.GetEvent<PortfolioReferenceSetEvent>().Unsubscribe(HandleFundReferenceSet);
        }

        #endregion
    }
}
