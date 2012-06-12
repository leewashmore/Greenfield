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
using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Prism.Events;
using GreenField.ServiceCaller;
using Microsoft.Practices.Prism.Logging;
using System.Collections.ObjectModel;
using GreenField.ServiceCaller.SecurityReferenceDefinitions;
using System.Collections.Generic;
using GreenField.Common.Helper;
using GreenField.Gadgets.Helpers;
using GreenField.Common;
using System.Linq;
using GreenField.DataContracts;
using GreenField.ServiceCaller.BenchmarkHoldingsDefinitions;

namespace GreenField.Gadgets.ViewModels
{
    public class ViewModelPortfolioDetails : NotificationObject
    {
        #region PrivateFields

        private IEventAggregator _eventAggregator;
        private IDBInteractivity _dbInteractivity;
        private ILoggerFacade _logger;
        private PortfolioSelectionData _portfolioSelectionData;
        private DateTime? _effectiveDate;


        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dbInteractivity">Instance of service caller class</param>
        /// <param name="eventAggregator"></param>
        /// <param name="logger">Instance of LoggerFacade</param>
        public ViewModelPortfolioDetails(DashboardGadgetParam param)
        {
            this._dbInteractivity = param.DBInteractivity;
            this._eventAggregator = param.EventAggregator;
            this._logger = param.LoggerFacade;
            _portfolioSelectionData = param.DashboardGadgetPayload.PortfolioSelectionData;
            _effectiveDate = param.DashboardGadgetPayload.EffectiveDate;
            ExcludeCashSecurities = param.DashboardGadgetPayload.IsExCashSecurityData;

            if (_eventAggregator != null && _effectiveDate != null && _portfolioSelectionData != null)
            {
                BusyIndicatorStatus = true;
                _dbInteractivity.RetrievePortfolioDetailsData(_portfolioSelectionData, Convert.ToDateTime(_effectiveDate), ExcludeCashSecurities, false, RetrievePortfolioDetailsDataCallbackMethod);
            }

            if (_eventAggregator != null)
            {
                _eventAggregator.GetEvent<PortfolioReferenceSetEvent>().Subscribe(HandlePortfolioReferenceSet);
                _eventAggregator.GetEvent<EffectiveDateReferenceSetEvent>().Subscribe(HandleEffectiveDateSet);
                _eventAggregator.GetEvent<ExCashSecuritySetEvent>().Subscribe(HandleExCashSecuritySetEvent);
            }
        }

        #endregion

        #region PropertiesDeclaration

        /// <summary>
        /// The Portfolio selected from the top menu Portfolio Selector Control
        /// </summary>
        private PortfolioSelectionData _selectedPortfolioId;
        public PortfolioSelectionData SelectedPortfolioId
        {
            get
            {
                return _selectedPortfolioId;
            }
            set
            {
                _selectedPortfolioId = value;
                this.RaisePropertyChanged(() => this.SelectedPortfolioDetailsData);
            }
        }

        /// <summary>
        /// Collection Containing the Data to be shown in the Grid
        /// </summary>
        private RangeObservableCollection<PortfolioDetailsData> _selectedPortfolioDetailsData = new RangeObservableCollection<PortfolioDetailsData>();
        public RangeObservableCollection<PortfolioDetailsData> SelectedPortfolioDetailsData
        {
            get
            {
                return _selectedPortfolioDetailsData;
            }
            set
            {
                _selectedPortfolioDetailsData = value;
                this.RaisePropertyChanged(() => this.SelectedPortfolioDetailsData);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private RangeObservableCollection<PortfolioDetailsData> _groupedFilteredPortfolioDetailsData;
        public RangeObservableCollection<PortfolioDetailsData> GroupedFilteredPortfolioDetailsData
        {
            get
            {
                return _groupedFilteredPortfolioDetailsData;
            }
            set
            {
                _groupedFilteredPortfolioDetailsData = value;
                if (value.Count > 0)
                {
                    ReturnGroupedColumnData(GroupingColumn);
                }
                this.RaisePropertyChanged(() => this.GroupedFilteredPortfolioDetailsData);
            }
        }

        /// <summary>
        /// Base column for grouping
        /// </summary>
        private string _groupingColumn;
        public string GroupingColumn
        {
            get
            {
                return _groupingColumn;
            }
            set
            {
                _groupingColumn = value;
                this.RaisePropertyChanged(() => this.GroupingColumn);
            }
        }


        /// <summary>
        /// Base Portfolio Data Collection
        /// </summary>
        private RangeObservableCollection<PortfolioDetailsData> _basePortfolioData;
        public RangeObservableCollection<PortfolioDetailsData> BasePortfolioData
        {
            get
            {
                return _basePortfolioData;
            }
            set
            {
                _basePortfolioData = value;
                this.RaisePropertyChanged(() => this.BasePortfolioData);
            }
        }

        private bool _getBenchmarkData;
        public bool GetBenchmarkData
        {
            get
            {
                return _getBenchmarkData;
            }
            set
            {
                _getBenchmarkData = value;
                this.RaisePropertyChanged(() => this.GetBenchmarkData);
            }
        }

        /// <summary>
        /// Collection of all Benchmark Names
        /// </summary>
        private ObservableCollection<string> _benchmarkNamesData;
        public ObservableCollection<string> BenchmarkNamesData
        {
            get
            {
                return _benchmarkNamesData;
            }
            set
            {
                _benchmarkNamesData = value;
                this.RaisePropertyChanged(() => this.BenchmarkNamesData);
            }
        }

        /// <summary>
        /// Property Bind to the BenchmarkSelectionComboBox
        /// </summary>
        private string _selectedBenchmark;
        public string SelectedBenchmark
        {
            get
            {
                return _selectedBenchmark;
            }
            set
            {
                _selectedBenchmark = value;
                this.RaisePropertyChanged(() => this.SelectedBenchmark);
            }
        }

        /// <summary>
        /// Selected date from the application toolbar.
        /// </summary>
        private DateTime _selectedDate = DateTime.Today;
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

        /// <summary>
        /// Check whether applying/removing grouping
        /// </summary>
        private bool _groupingStatus = false;
        public bool GroupingStatus
        {
            get
            {
                return _groupingStatus;
            }
            set
            {
                _groupingStatus = value;
                this.RaisePropertyChanged(() => this.GroupingStatus);
            }
        }

        /// <summary>
        /// Busy Indicator for Portfolio Details
        /// </summary>
        private bool _busyIndicatorStatus;
        public bool BusyIndicatorStatus
        {
            get
            {
                return _busyIndicatorStatus;
            }
            set
            {
                _busyIndicatorStatus = value;
                this.RaisePropertyChanged(() => this.BusyIndicatorStatus);
            }
        }

        /// <summary>
        /// Check to include Cash Securities
        /// </summary>
        private bool _excludeCashSecurities;
        public bool ExcludeCashSecurities
        {
            get
            {
                return _excludeCashSecurities;
            }
            set
            {
                _excludeCashSecurities = value;
                this.RaisePropertyChanged(() => this.ExcludeCashSecurities);
            }
        }



        #endregion

        #region CallbackMethods

        /// <summary>
        /// CallBack Method for Retrieving Portfolio Names
        /// </summary>
        /// <param name="result"></param>
        private void RetrievePortfolioDetailsDataCallbackMethod(List<PortfolioDetailsData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    SelectedPortfolioDetailsData.Clear();
                    SelectedPortfolioDetailsData.AddRange(CalculatePortfolioValues(result));
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

        #region HelperMethods

        /// <summary>
        /// Service call to Retrieve the Details for a Portfolio
        /// </summary>
        /// <param name="objPortfolioId">PortfolioName</param>
        private void RetrievePortfolioDetailsData(PortfolioSelectionData objPortfolioId, DateTime objSelectedDate, bool objgetBenchmark, Action<List<PortfolioDetailsData>> callback)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (objPortfolioId != null && objSelectedDate != null && _dbInteractivity != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, objSelectedDate, 1);
                    Logging.LogMethodParameter(_logger, methodNamespace, objPortfolioId, 1);
                    _dbInteractivity.RetrievePortfolioDetailsData(objPortfolioId, objSelectedDate, ExcludeCashSecurities, GetBenchmarkData, callback);
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
        /// Helper methods to Re-Calculate Weights while Grouping
        /// </summary>
        /// <param name="objGroupingColumnName">Column on the basis of which Grouping is Done</param>
        private void ReturnGroupedColumnData(string objGroupingColumnName)
        {
            try
            {
                BasePortfolioData = GroupedFilteredPortfolioDetailsData;
                switch (Convert.ToString(objGroupingColumnName).ToLower())
                {
                    case ("isocountrycode"):
                        {
                            List<string> groupedColumnData = BasePortfolioData.Select(s => s.IsoCountryCode).Distinct().ToList();
                            foreach (string item in groupedColumnData)
                            {
                                decimal? sumPortfolioWeight = (from p in BasePortfolioData
                                                               where p.IsoCountryCode == item
                                                               select p.PortfolioWeight).ToList().Sum();
                                decimal? sumBenchmarkWeight = (from p in BasePortfolioData
                                                               where p.IsoCountryCode == item
                                                               select p.BenchmarkWeight).ToList().Sum();
                                decimal? sumDirtyValuePC = (from p in BasePortfolioData
                                                            where p.IsoCountryCode == item
                                                            select p.DirtyValuePC).ToList().Sum();
                                decimal? sumAshEmmModelWeight = (from p in BasePortfolioData
                                                                 where p.IsoCountryCode == item
                                                                 select p.AshEmmModelWeight).ToList().Sum();

                                

                                foreach (PortfolioDetailsData data in SelectedPortfolioDetailsData.Where(w => w.IsoCountryCode == item).ToList())
                                {
                                    if (sumDirtyValuePC != 0)
                                        data.RePortfolioWeight = data.DirtyValuePC / sumDirtyValuePC * 100;

                                    if (sumBenchmarkWeight != 0)
                                        data.ReBenchmarkWeight = data.BenchmarkWeight / sumBenchmarkWeight * 100;

                                    if (sumAshEmmModelWeight != 0)
                                        data.ReAshEmmModelWeight = data.AshEmmModelWeight / sumAshEmmModelWeight * 100;

                                }
                            }
                            break;
                        }
                    case ("proprietaryregioncode"):
                        {
                            List<string> groupedColumnData = BasePortfolioData.Select(s => s.ProprietaryRegionCode).Distinct().ToList();
                            foreach (string item in groupedColumnData)
                            {
                                decimal? sumPortfolioWeight = (from p in BasePortfolioData
                                                               where p.ProprietaryRegionCode == item
                                                               select p.PortfolioWeight).ToList().Sum();
                                decimal? sumBenchmarkWeight = (from p in BasePortfolioData
                                                               where p.ProprietaryRegionCode == item
                                                               select p.BenchmarkWeight).ToList().Sum();
                                decimal? sumDirtyValuePC = (from p in BasePortfolioData
                                                            where p.ProprietaryRegionCode == item
                                                            select p.DirtyValuePC).ToList().Sum();
                                decimal? sumAshEmmModelWeight = (from p in BasePortfolioData
                                                                 where p.ProprietaryRegionCode == item
                                                                 select p.AshEmmModelWeight).ToList().Sum();

                               

                                foreach (PortfolioDetailsData data in SelectedPortfolioDetailsData.Where(w => w.ProprietaryRegionCode == item).ToList())
                                {
                                    if (sumDirtyValuePC != 0)
                                        data.RePortfolioWeight = data.DirtyValuePC / sumDirtyValuePC * 100;

                                    if (sumBenchmarkWeight != 0)
                                        data.ReBenchmarkWeight = data.BenchmarkWeight / sumBenchmarkWeight * 100;


                                    if (sumAshEmmModelWeight != 0)
                                        data.ReAshEmmModelWeight = data.AshEmmModelWeight / sumAshEmmModelWeight * 100;


                                }
                            }
                            break;
                        }
                    case ("sectorname"):
                        {
                            List<string> groupedColumnData = BasePortfolioData.Select(s => s.SectorName).Distinct().ToList();
                            foreach (string item in groupedColumnData)
                            {
                                decimal? sumPortfolioWeight = (from p in BasePortfolioData
                                                               where p.SectorName == item
                                                               select p.PortfolioWeight).ToList().Sum();
                                decimal? sumBenchmarkWeight = (from p in BasePortfolioData
                                                               where p.SectorName == item
                                                               select p.BenchmarkWeight).ToList().Sum();
                                decimal? sumDirtyValuePC = (from p in BasePortfolioData
                                                            where p.SectorName == item
                                                            select p.DirtyValuePC).ToList().Sum();
                                decimal? sumAshEmmModelWeight = (from p in BasePortfolioData
                                                                 where p.SectorName == item
                                                                 select p.AshEmmModelWeight).ToList().Sum();

                               

                                foreach (PortfolioDetailsData data in SelectedPortfolioDetailsData.Where(w => w.SectorName == item).ToList())
                                {
                                    if (sumDirtyValuePC != 0)
                                        data.RePortfolioWeight = data.DirtyValuePC / sumDirtyValuePC * 100;

                                    if (sumBenchmarkWeight != 0)
                                        data.ReBenchmarkWeight = data.BenchmarkWeight / sumBenchmarkWeight * 100;

                                    if (sumAshEmmModelWeight != 0)
                                        data.ReAshEmmModelWeight = data.AshEmmModelWeight / sumAshEmmModelWeight * 100;

                                }
                            }
                            break;
                        }
                    case ("industryname"):
                        {
                            List<string> groupedColumnData = BasePortfolioData.Select(s => s.IndustryName).Distinct().ToList();
                            foreach (string item in groupedColumnData)
                            {
                                decimal? sumPortfolioWeight = (from p in BasePortfolioData
                                                               where p.IndustryName == item
                                                               select p.PortfolioWeight).ToList().Sum();
                                decimal? sumBenchmarkWeight = (from p in BasePortfolioData
                                                               where p.IndustryName == item
                                                               select p.BenchmarkWeight).ToList().Sum();
                                decimal? sumDirtyValuePC = (from p in BasePortfolioData
                                                            where p.IndustryName == item
                                                            select p.DirtyValuePC).ToList().Sum();
                                decimal? sumAshEmmModelWeight = (from p in BasePortfolioData
                                                                 where p.IndustryName == item
                                                                 select p.AshEmmModelWeight).ToList().Sum();

                                

                                foreach (PortfolioDetailsData data in SelectedPortfolioDetailsData.Where(w => w.IndustryName == item).ToList())
                                {
                                    if (sumDirtyValuePC != 0)
                                        data.RePortfolioWeight = data.DirtyValuePC / sumDirtyValuePC * 100;

                                    if (sumBenchmarkWeight != 0)
                                        data.ReBenchmarkWeight = data.BenchmarkWeight / sumBenchmarkWeight * 100;

                                    if (sumAshEmmModelWeight != 0)
                                        data.ReAshEmmModelWeight = data.AshEmmModelWeight / sumAshEmmModelWeight * 100;

                                }
                            }
                            break;
                        }
                    case ("subindustryname"):
                        {
                            List<string> groupedColumnData = BasePortfolioData.Select(s => s.SubIndustryName).Distinct().ToList();
                            foreach (string item in groupedColumnData)
                            {
                                decimal? sumPortfolioWeight = (from p in BasePortfolioData
                                                               where p.SubIndustryName == item
                                                               select p.PortfolioWeight).ToList().Sum();
                                decimal? sumBenchmarkWeight = (from p in BasePortfolioData
                                                               where p.SubIndustryName == item
                                                               select p.BenchmarkWeight).ToList().Sum();
                                decimal? sumDirtyValuePC = (from p in BasePortfolioData
                                                            where p.SubIndustryName == item
                                                            select p.DirtyValuePC).ToList().Sum();
                                decimal? sumAshEmmModelWeight = (from p in BasePortfolioData
                                                                 where p.SubIndustryName == item
                                                                 select p.AshEmmModelWeight).ToList().Sum();

                               

                                foreach (PortfolioDetailsData data in SelectedPortfolioDetailsData.Where(w => w.SubIndustryName == item).ToList())
                                {
                                    if (sumDirtyValuePC != 0)
                                        data.RePortfolioWeight = data.DirtyValuePC / sumDirtyValuePC * 100;

                                    if (sumBenchmarkWeight != 0)
                                        data.ReBenchmarkWeight = data.BenchmarkWeight / sumBenchmarkWeight * 100;

                                    if (sumAshEmmModelWeight != 0)
                                        data.ReAshEmmModelWeight = data.AshEmmModelWeight / sumAshEmmModelWeight * 100;

                                }
                            }
                            break;
                        }
                    default:
                        {
                            foreach (PortfolioDetailsData data in SelectedPortfolioDetailsData)
                            {
                                data.ReAshEmmModelWeight = data.AshEmmModelWeight;
                                data.ReBenchmarkWeight = data.BenchmarkWeight;
                                data.RePortfolioWeight = data.PortfolioWeight;
                            }
                            break;
                        }
                }
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// Calculations for Portfolio Details UI
        /// </summary>
        /// <param name="portfolioDetailsData">Collection of Portfolio Details Data</param>
        /// <returns>Collection of PortfolioDetailsData</returns>
        private List<PortfolioDetailsData> CalculatePortfolioValues(List<PortfolioDetailsData> portfolioDetailsData)
        {
            try
            {
                if (portfolioDetailsData == null)
                    return new List<PortfolioDetailsData>();
                if (portfolioDetailsData.Count == 0)
                    return new List<PortfolioDetailsData>();

                decimal sumDirtyValuePC = 0;
                decimal sumModelWeight = 0;

                sumDirtyValuePC = portfolioDetailsData.Sum(a => Convert.ToDecimal(a.DirtyValuePC));
                sumModelWeight = portfolioDetailsData.Sum(a => Convert.ToDecimal(a.AshEmmModelWeight));

                if (sumDirtyValuePC == 0 && sumModelWeight == 0)
                    return portfolioDetailsData;

                if (sumDirtyValuePC == 0)
                    Prompt.ShowDialog("Portfolio Details UI :The Sum of Portfolio Weights is 0");

                if (sumModelWeight == 0)
                    Prompt.ShowDialog("Portfolio Details UI :The Sum of Model Weights is 0");


                foreach (PortfolioDetailsData item in portfolioDetailsData)
                {
                    if (sumDirtyValuePC != 0)
                    {
                        item.PortfolioWeight = item.DirtyValuePC / sumDirtyValuePC * 100;
                        item.RePortfolioWeight = item.PortfolioWeight;
                        item.ActivePosition = item.PortfolioWeight - item.BenchmarkWeight;
                    }
                    else
                    {
                        item.RePortfolioWeight = 0;
                        item.ActivePosition = 0;
                    }


                    if (sumModelWeight != 0)
                    {
                        item.AshEmmModelWeight = item.AshEmmModelWeight / sumModelWeight * 100;
                        item.ReAshEmmModelWeight = item.AshEmmModelWeight;
                    }
                    else
                    {
                        item.ReAshEmmModelWeight = 0;
                    }

                    item.ReBenchmarkWeight = item.BenchmarkWeight;

                }
                return portfolioDetailsData;
            }
            catch (Exception ex)
            {
                return new List<PortfolioDetailsData>();
            }
        }

        #endregion

        #region Events

        public event DataRetrievalProgressIndicatorEventHandler PortfolioDetailsDataLoadedEvent;

        #endregion

        #region EventHandlers

        /// <summary>
        /// Event handler for FundSelection changed Event
        /// </summary>
        /// <param name="PortfolioSelectionData"></param>
        public void HandlePortfolioReferenceSet(PortfolioSelectionData PortfolioSelectionData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                //Arguement Null Exception
                if (PortfolioSelectionData != null)
                {
                    SelectedPortfolioId = PortfolioSelectionData;
                    if (SelectedPortfolioId != null && _effectiveDate != null)
                    {
                        BusyIndicatorStatus = true;
                        RetrievePortfolioDetailsData(SelectedPortfolioId, Convert.ToDateTime(_effectiveDate), GetBenchmarkData, RetrievePortfolioDetailsDataCallbackMethod);
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
                    _effectiveDate = effectiveDate;
                    if (_effectiveDate != null && SelectedPortfolioId != null)
                    {
                        BusyIndicatorStatus = true;
                        _dbInteractivity.RetrievePortfolioDetailsData(_portfolioSelectionData, Convert.ToDateTime(_effectiveDate), ExcludeCashSecurities, false, RetrievePortfolioDetailsDataCallbackMethod);
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
                    ExcludeCashSecurities = isExCashSec;

                    if (_dbInteractivity != null && SelectedPortfolioId != null && _effectiveDate != null)
                    {
                        BusyIndicatorStatus = true;
                        RetrievePortfolioDetailsData(SelectedPortfolioId, Convert.ToDateTime(_effectiveDate), GetBenchmarkData, RetrievePortfolioDetailsDataCallbackMethod);
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

        #region EventUnsubscribe

        public void Dispose()
        {
            if (_eventAggregator != null)
            {
                _eventAggregator.GetEvent<PortfolioReferenceSetEvent>().Unsubscribe(HandlePortfolioReferenceSet);
                _eventAggregator.GetEvent<EffectiveDateReferenceSetEvent>().Unsubscribe(HandleEffectiveDateSet);
            }
        }

        #endregion
    }
}