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
using GreenField.ServiceCaller.BenchmarkHoldingsPerformanceDefinitions;

namespace GreenField.Benchmark.ViewModels
{
    public class ViewModelPortfolioDetails : NotificationObject
    {
        #region PrivateFields

        private IEventAggregator _eventAggregator;
        private IDBInteractivity _dbInteractivity;
        private ILoggerFacade _logger;
        private FundSelectionData _fundSelectionData;

        private FundSelectionData _dummyfundSelectionData = new FundSelectionData();

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dbInteractivity">Instance of service caller class</param>
        /// <param name="eventAggregator"></param>
        /// <param name="logger">Instance of LoggerFacade</param>
        public ViewModelPortfolioDetails(IDBInteractivity dbInteractivity, IEventAggregator eventAggregator, ILoggerFacade logger)
        {
            this._dbInteractivity = dbInteractivity;
            this._eventAggregator = eventAggregator;
            this._logger = logger;
            _eventAggregator.GetEvent<FundReferenceSetEvent>().Subscribe(HandleFundReferenceSet, true);

            HandleFundReferenceSet(_dummyfundSelectionData);

            if (_fundSelectionData != null)
                HandleFundReferenceSet(_fundSelectionData);
        }

        #endregion

        #region PropertiesDeclaration

        /// <summary>
        /// The Portfolio selected from the top menu Portfolio Selector Control
        /// </summary>
        private string _selectedPortfolioName;
        public string SelectedPortfolioName
        {
            get
            {
                return _selectedPortfolioName;
            }
            set
            {
                _selectedPortfolioName = value;
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



        #endregion

        #region CallbackMethods

        /// <summary>
        /// CallBack Method for Retrieving Portfolio Names
        /// </summary>
        /// <param name="result"></param>
        private void RetrievePortfolioDetailsDataCallbackMethod(List<PortfolioDetailsData> result)
        {
            if (result != null)
            {
                SelectedPortfolioDetailsData.Clear();
                SelectedPortfolioDetailsData.AddRange(result);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Service call to Retrieve the Details for a Portfolio
        /// </summary>
        /// <param name="objPortfolioName">PortfolioName</param>
        private void RetrievePortfolioDetailsData(string objPortfolioName, DateTime objSelectedDate, Action<List<PortfolioDetailsData>> callback)
        {
            _dbInteractivity.RetrievePortfolioDetailsData(objPortfolioName, objSelectedDate, callback);
        }

        private void GroupedData()
        {
            //List<string> groupedColumnData = ReturnGroupedColumnData(GroupingColumn.ToLower());
        }

        private void ReturnGroupedColumnData(string objGroupingColumnName)
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
                                data.RePortfolioWeight = data.DirtyValuePC / sumDirtyValuePC * 100;
                                data.ReBenchmarkWeight = data.BenchmarkWeight / sumBenchmarkWeight * 100;
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
                                data.RePortfolioWeight = data.DirtyValuePC / sumDirtyValuePC * 100;
                                data.ReBenchmarkWeight = data.BenchmarkWeight / sumBenchmarkWeight * 100;
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
                                data.RePortfolioWeight = data.DirtyValuePC / sumDirtyValuePC * 100;
                                data.ReBenchmarkWeight = data.BenchmarkWeight / sumBenchmarkWeight * 100;
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
                                data.RePortfolioWeight = data.DirtyValuePC / sumDirtyValuePC * 100;
                                data.ReBenchmarkWeight = data.BenchmarkWeight / sumBenchmarkWeight * 100;
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
                                data.RePortfolioWeight = data.DirtyValuePC / sumDirtyValuePC * 100;
                                data.ReBenchmarkWeight = data.BenchmarkWeight / sumBenchmarkWeight * 100;
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

        #endregion

        #region EventHandlers

        /// <summary>
        /// Event handler for FundSelection changed Event
        /// </summary>
        /// <param name="fundSelectionData"></param>
        private void HandleFundReferenceSet(FundSelectionData fundSelectionData)
        {
            try
            {
                //Arguement Null Exception
                if (fundSelectionData != null)
                {
                    RetrievePortfolioDetailsData(SelectedPortfolioName, SelectedDate, RetrievePortfolioDetailsDataCallbackMethod);
                }
            }
            catch (Exception ex)
            {

            }
        }

        #endregion
    }
}