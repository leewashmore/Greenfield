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
using GreenField.ServiceCaller.ProxyDataDefinitions;
using Microsoft.Practices.Prism.ViewModel;
using GreenField.Common;
using System.Collections.ObjectModel;

namespace GreenField.Gadgets.ViewModels
{
    public class ViewModelHoldingsPieChart : NotificationObject
    {
        #region Fields
        /// <summary>
        /// MEF Singletons
        /// </summary>
        private IDBInteractivity _dbInteractivity;
        private ILoggerFacade _logger;
        private IEventAggregator _eventAggregator;
        /// <summary>
        /// DashboardGadgetPayLoad fields
        /// </summary>
        private BenchmarkSelectionData _benchmarkSelectionData;
        #endregion

        #region Constructor
        public ViewModelHoldingsPieChart(DashBoardGadgetParam param)
        {
            _dbInteractivity = param.DBInteractivity;
            _logger = param.LoggerFacade;
            _eventAggregator = param.EventAggregator;
            _benchmarkSelectionData = param.DashboardGadgetPayLoad.BenchmarkSelectionData;
            EffectiveDate = param.DashboardGadgetPayLoad.EffectiveDate;

            if (_eventAggregator != null)
            {
                _eventAggregator.GetEvent<BenchmarkReferenceSetEvent>().Subscribe(HandleBenchmarkReferenceSet);
                _eventAggregator.GetEvent<EffectiveDateSet>().Subscribe(HandleEffectiveDateSet);
            }

            //if (_benchmarkSelectionData != null && EffectiveDate != null)
            //{
            //    _dbInteractivity.RetrieveHoldingsPercentageData(_benchmarkSelectionData, EffectiveDate, RetrieveHoldingsPercentageDataCallbackMethod);
            //}
         //   _dbInteractivity.RetrieveHoldingsPercentageData(_benchmarkSelectionData, EffectiveDate, RetrieveHoldingsPercentageDataCallbackMethod);
        }
        #endregion

        #region Properties
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

        private ObservableCollection<HoldingsPercentageData> _holdingsPercentageInfoForRegion;
        public ObservableCollection<HoldingsPercentageData> HoldingsPercentageInfoForRegion
        {
            get { return _holdingsPercentageInfoForRegion; }
            set
            {
                _holdingsPercentageInfoForRegion = value;
                RaisePropertyChanged(() => this.HoldingsPercentageInfoForRegion);
            }
        }

        private DateTime _effectiveDate;
        public DateTime EffectiveDate
        {
            get { return _effectiveDate; }
            set
            {
                _effectiveDate = value;
                RaisePropertyChanged(() => this.EffectiveDate);
            }
        }

        private ObservableCollection<String> _filterTypes;
        public ObservableCollection<String> FilterTypes
        {
            get
            {
                return new ObservableCollection<string> { "Region", "Country", "Industry", "Sector" };
            }
        }

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


        private String _valueTypesSelection;
        public String ValueTypesSelection
        {
            get { return _valueTypesSelection; }
            set
            {
                    _valueTypesSelection = value;
                    _dbInteractivity.RetrieveHoldingsPercentageData(_benchmarkSelectionData, EffectiveDate, FilterTypesSelection, ValueTypesSelection, RetrieveHoldingsPercentageDataCallbackMethod);
                    _dbInteractivity.RetrieveHoldingsPercentageDataForRegion(_benchmarkSelectionData, EffectiveDate, FilterTypesSelection, ValueTypesSelection, RetrieveHoldingsPercentageDataForRegionCallbackMethod);
                    RaisePropertyChanged(() => this.ValueTypesSelection);
                
            }
        }

        //private List<String> _customLabels;
        //public List<String> CustomLabels
        //{
        //    get 
        //    {           
        //        return _customLabels;
        //    }

        //    set 
        //    {
        //        _customLabels = value;

        //        RaisePropertyChanged(() => this.CustomLabels);
            
        //    }
        
        //}


        
        #endregion

        #region Event Handlers
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
                    if (EffectiveDate != null && _benchmarkSelectionData != null)
                    {
                      //  _dbInteractivity.RetrieveHoldingsPercentageData(_benchmarkSelectionData, EffectiveDate, RetrieveHoldingsPercentageDataCallbackMethod);
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

        public void HandleBenchmarkReferenceSet(BenchmarkSelectionData benchmarkSelectionData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (benchmarkSelectionData != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, benchmarkSelectionData, 1);
                    _benchmarkSelectionData = benchmarkSelectionData;
                    if (EffectiveDate != null && _benchmarkSelectionData != null)
                    {
                       // _dbInteractivity.RetrieveHoldingsPercentageData(_benchmarkSelectionData, EffectiveDate, RetrieveHoldingsPercentageDataCallbackMethod);
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
        public void RetrieveHoldingsPercentageDataCallbackMethod(List<HoldingsPercentageData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {

                    HoldingsPercentageInfo = new ObservableCollection<HoldingsPercentageData>(result);
                    //for (int i = 0; i < result.Count; i++)
                    //{
                    //    String label = result[i].SegmentName + result[i].BenchmarkWeight + result[i].PortfolioWeight;
                    //    CustomLabels.Add(label);
                    //}
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

        public void RetrieveHoldingsPercentageDataForRegionCallbackMethod(List<HoldingsPercentageData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    HoldingsPercentageInfoForRegion = new ObservableCollection<HoldingsPercentageData>(result);
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

        public void RetrieveValuesForFiltersCallbackMethod(List<String> result)
        {
            ValueTypes = result;
        
        }



        #endregion
    }
}
