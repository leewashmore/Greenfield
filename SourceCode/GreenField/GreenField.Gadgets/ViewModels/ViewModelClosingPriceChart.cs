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
using System.Linq;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.ViewModel;
using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.Events;
using GreenField.ServiceCaller.ProxyDataDefinitions;
using System.Collections.Generic;
using System.Windows.Data;
using System.Collections.ObjectModel;
using Microsoft.Practices.Prism.Commands;
using GreenField.Common;
using GreenField.Gadgets.Helpers;

namespace GreenField.Gadgets.ViewModels
{
    public class ViewModelClosingPriceChart : NotificationObject
    {
        #region Private Fields
        /// <summary>
        /// MEF Singletons
        /// </summary>
        private IDBInteractivity _dbInteractivity;
        private ILoggerFacade _logger;
        private IEventAggregator _eventAggregator;
        private EntitySelectionData _entitySelectionData;
        #endregion

        #region Constructor

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventAggregator"></param>
        /// <param name="dbInteractivity"></param>
        /// <param name="logger"></param>
        /// <param name="entitySelectionData"></param>
        public ViewModelClosingPriceChart(DashBoardGadgetParam param)
        {
            _dbInteractivity = param.DBInteractivity;
            _logger = param.LoggerFacade;
            _eventAggregator = param.EventAggregator;
            _entitySelectionData = param.DashboardGadgetPayLoad.EntitySelectionData;

            _dbInteractivity.RetrieveEntitySelectionData(RetrieveEntitySelectionDataCallBackMethod);
            _eventAggregator.GetEvent<SecurityReferenceSetEvent>().Subscribe(HandleSecurityReferenceSet, false);
            if (_entitySelectionData != null)
                HandleSecurityReferenceSet(_entitySelectionData);
        }

        #endregion

        #region Properties
        #region UI Fields

        /// <summary>
        /// Storing the names of all entities added to chart.
        /// </summary>
        private ObservableCollection<string> _chartEntityList;
        public ObservableCollection<string> ChartEntityList
        {
            get
            {
                if (_chartEntityList == null)
                    _chartEntityList = new ObservableCollection<string>();
                return _chartEntityList;
            }
            set
            {
                _chartEntityList = value;
                this.RaisePropertyChanged(() => this.ChartEntityList);
            }
        }

        #region Time Period Selection
        /// <summary>
        /// Collection of Time Range Options
        /// </summary>
        private ObservableCollection<String> _timeRange;
        public ObservableCollection<String> TimeRange
        {
            get
            {
                return new ObservableCollection<string> { "1-Month", "2-Month", "3-Month", "6-Month", "YTD", "1-Year", "2-Years", 
                    "3-Years", "4-Years", "5-Years", "10-Years", "Custom" };
            }
        }

        /// <summary>
        /// Selection Time Range option
        /// </summary>
        private string _selectedTimeRange = "1-Year";
        public string SelectedTimeRange
        {
            get { return _selectedTimeRange; }
            set
            {
                if (_selectedTimeRange != value)
                {
                    _selectedTimeRange = value;
                    //Retrieve Pricing Data for updated Time Range
                    RetrievePricingData(new ObservableCollection<String>
                        (PlottedSeries.Select(p => p.Ticker).Distinct().ToList()),
                            RetrievePricingReferenceDataCallBackMethod_TimeRange);
                }
            }
        }

        /// <summary>
        /// Selected StartDate Option in case of Custom Time Range
        /// </summary>
        private DateTime _selectedStartDate;
        public DateTime SelectedStartDate
        {
            get { return _selectedStartDate; }
            set
            {
                if (_selectedStartDate != value)
                {
                    _selectedStartDate = value;
                    RaisePropertyChanged(() => this.SelectedStartDate);
                }
            }
        }

        /// <summary>
        /// Selected EndDate Option in case of Custom Time Range
        /// </summary>
        private DateTime _selectedEndDate;
        public DateTime SelectedEndDate
        {
            get { return _selectedEndDate; }
            set
            {
                if (_selectedEndDate != value)
                {
                    _selectedEndDate = value;
                    RaisePropertyChanged(() => this.SelectedEndDate);
                }
            }
        }
        #endregion

        #region FrequencySelection

        private string _frequencyInterval = "";
        public string FrequencyInterval
        {
            get
            {
                return _frequencyInterval;
            }
            set
            {
                _frequencyInterval = value;
                this.RaisePropertyChanged(() => this.FrequencyInterval);
            }
        }

        #endregion

        #region Plotting Additional Series
        /// <summary>
        /// Grouped Collection View for Auto-Complete Box
        /// </summary>
        private CollectionViewSource _seriesReference;
        public CollectionViewSource SeriesReference
        {
            get
            {
                return _seriesReference;
            }
            set
            {
                _seriesReference = value;
                RaisePropertyChanged(() => this.SeriesReference);
            }
        }

        /// <summary>
        /// DataSource for the Grouped Collection View
        /// </summary>
        public ObservableCollection<EntitySelectionData> SeriesReferenceSource { get; set; }


        private EntitySelectionData _selectedSeriesReference = new EntitySelectionData();
        /// <summary>
        /// Selected Entity
        /// </summary>
        public EntitySelectionData SelectedSeriesReference
        {
            get
            {
                return _selectedSeriesReference;
            }
            set
            {
                _selectedSeriesReference = value;
                this.RaisePropertyChanged(() => this.SelectedSeriesReference);
            }
        }

        /// <summary>
        /// Search Mode Filter - Checked (StartsWith); Unchecked (Contains)
        /// </summary>
        private bool _searchFilterEnabled;
        public bool SearchFilterEnabled
        {
            get { return _searchFilterEnabled; }
            set
            {
                if (_searchFilterEnabled != value)
                {
                    _searchFilterEnabled = value;
                    RaisePropertyChanged(() => SearchFilterEnabled);
                }
            }
        }

        /// <summary>
        /// Entered Text in the Auto-Complete Box - filters SeriesReferenceSource
        /// </summary>
        private string _seriesEnteredText;
        public string SeriesEnteredText
        {
            get { return _seriesEnteredText; }
            set
            {
                _seriesEnteredText = value;
                RaisePropertyChanged(() => this.SeriesEnteredText);
                if (value != null)
                    SeriesReference.Source = SearchFilterEnabled == false
                        ? SeriesReferenceSource.Where(o => o.ShortName.ToLower().Contains(value.ToLower()))
                        : SeriesReferenceSource.Where(o => o.ShortName.ToLower().StartsWith(value.ToLower()));
                else
                    SeriesReference.Source = SeriesReferenceSource;
            }
        }

        /// <summary>
        /// Type of entites added to chart
        /// if true:Commodity/Index/Currency Added
        /// if false:only securities added 
        /// </summary>
        private bool _chartEntityTypes = true;
        public bool ChartEntityTypes
        {
            get
            {
                return _chartEntityTypes;
            }
            set
            {
                _chartEntityTypes = value;
                this.RaisePropertyChanged(() => this.ChartEntityTypes);
            }
        }

        #endregion

        #region Chart/Grid Entities
        private string newEntity;
        public string NewEntity
        {
            get { return newEntity; }
            set
            {
                if (newEntity != value)
                {
                    newEntity = value;
                    RaisePropertyChanged(() => NewEntity);
                }
            }
        }

        private bool _returnTypeSelection;
        public bool ReturnTypeSelection
        {
            get
            {
                return _returnTypeSelection;
            }
            set
            {
                _returnTypeSelection = value;
                this.RaisePropertyChanged(() => this.ReturnTypeSelection);
            }
        }

        #endregion

        private RangeObservableCollection<PricingReferenceData> _plottedSeries;
        public RangeObservableCollection<PricingReferenceData> PlottedSeries
        {
            get
            {
                if (_plottedSeries == null)
                    _plottedSeries = new RangeObservableCollection<PricingReferenceData>();
                return _plottedSeries;
            }
            set
            {
                if (_plottedSeries != value)
                {
                    _plottedSeries = value;
                    RaisePropertyChanged(() => this.PlottedSeries);
                }
            }
        }

        private RangeObservableCollection<PricingReferenceData> _primaryPlottedSeries;
        public RangeObservableCollection<PricingReferenceData> PrimaryPlottedSeries
        {
            get
            {
                if (_primaryPlottedSeries == null)
                    _primaryPlottedSeries = new RangeObservableCollection<PricingReferenceData>();
                return _primaryPlottedSeries;
            }
            set
            {
                if (_primaryPlottedSeries != value)
                {
                    _primaryPlottedSeries = value;
                    RaisePropertyChanged(() => this.PrimaryPlottedSeries);
                }
            }
        }

        private ObservableCollection<EntitySelectionData> _comparisonSeries = new ObservableCollection<EntitySelectionData>();
        public ObservableCollection<EntitySelectionData> ComparisonSeries
        {
            get
            {
                return _comparisonSeries;
            }
            set
            {
                _comparisonSeries = value;
                this.RaisePropertyChanged(() => this.ComparisonSeries);
            }
        }

        #endregion

        #region ICommand
        public ICommand AddCommand
        {
            get { return new DelegateCommand<object>(AddCommandMethod); }
        }

        public ICommand DeleteCommand
        {
            get { return new DelegateCommand<object>(DeleteCommandMethod); }
        }
        #endregion

        #endregion

        #region ICommand Methods

        private void AddCommandMethod(object param)
        {
            if (SelectedSeriesReference != null)
            {
                if (!PlottedSeries.Any(t => t.InstrumentID == SelectedSeriesReference.InstrumentID))
                {
                    //string type = SelectedSeriesReference.Type.ToString();
                    ChartEntityList.Add(SelectedSeriesReference.InstrumentID.ToString());

                    //Making initially ChartEntityTypes False
                    ChartEntityTypes = true;

                    //List<EntitySelectionData> objEntitySelectionData= new List<EntitySelectionData>(SeriesReference);

                    //Checking the types of entity in the Chart
                    foreach (EntitySelectionData item in SeriesReferenceSource.Where(r => r.Type != "SECURITY").ToList())
                    {
                        List<EntitySelectionData> a = SeriesReferenceSource.Where(r => r.Type != "SECURITY").ToList();
                        //If it contains type Commodity/Index/Currency, ChartEntityTypes=true else false
                        if (ChartEntityList.Contains(item.InstrumentID.ToString()))
                        {
                            ChartEntityTypes = false;
                        }
                    }

                    _dbInteractivity.RetrievePricingReferenceData(ChartEntityList, DateTime.Today.AddYears(-1), DateTime.Today, ReturnTypeSelection, FrequencyInterval, ChartEntityTypes, (result) =>
                    {
                        PlottedSeries.Clear();
                        PlottedSeries.AddRange(result);
                        List<PricingReferenceData> x = PlottedSeries.ToList();
                        ComparisonSeries.Add(SelectedSeriesReference);
                        SelectedSeriesReference = null;
                    });
                }
            }
        }

        private void DeleteCommandMethod(object param)
        {
            EntitySelectionData a = param as EntitySelectionData;
            List<PricingReferenceData> removeItem = new List<PricingReferenceData>();
            removeItem = PlottedSeries.Where(w => w.InstrumentID == a.InstrumentID).ToList();
            if (removeItem != null)
                PlottedSeries.RemoveRange(removeItem);
            ComparisonSeries.Remove(a);
            ChartEntityList.Remove(a.ShortName);
        }

        #endregion

        #region Callback Methods
        /// <summary>
        /// Callback method for Entity Reference Service call - Updates AutoCompleteBox
        /// </summary>
        /// <param name="result">EntityReferenceData Collection</param>
        private void RetrieveEntitySelectionDataCallBackMethod(List<EntitySelectionData> result)
        {
            SeriesReference = new CollectionViewSource();
            SeriesReferenceSource = new ObservableCollection<EntitySelectionData>(result);
            SeriesReference.GroupDescriptions.Add(new PropertyGroupDescription("Type"));
            SeriesReference.SortDescriptions.Add(new System.ComponentModel.SortDescription
            {
                PropertyName = "Type",
                Direction = System.ComponentModel.ListSortDirection.Ascending
            });
            SeriesReference.Source = SeriesReferenceSource;
        }

        /// <summary>
        /// Callback method for Pricing Reference Service call related to updated Time Range - Updates Chart and Grid
        /// </summary>
        /// <param name="result">PricingReferenceData collection</param>
        private void RetrievePricingReferenceDataCallBackMethod_TimeRange(List<PricingReferenceData> result)
        {
            string primarySecurityReferenceIdentifier = PrimaryPlottedSeries.First().InstrumentID;
            PlottedSeries.Clear();
            PrimaryPlottedSeries.Clear();
            PlottedSeries.AddRange(result);
            PrimaryPlottedSeries.AddRange(result.Where(item => item.InstrumentID == primarySecurityReferenceIdentifier).ToList());
        }

        /// <summary>
        /// Callback method for Pricing Reference Service call related to updated Security Reference - Updates Chart and Grid
        /// </summary>
        /// <param name="result">PricingReferenceData collection</param>
        private void RetrievePricingReferenceDataCallBackMethod_SecurityReference(List<PricingReferenceData> result)
        {
            PlottedSeries.AddRange(result);
            PrimaryPlottedSeries.AddRange(result);
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Event Handler to subscribed event 'SecurityReferenceSet'
        /// </summary>
        /// <param name="securityReferenceData">SecurityReferenceData</param>
        public void HandleSecurityReferenceSet(EntitySelectionData entitySelectionData)
        {
            //ArgumentNullException
            if (entitySelectionData == null)
                return;

            //Check if security reference data is already present
            if (PrimaryPlottedSeries.Where(p => p.InstrumentID == entitySelectionData.InstrumentID).Count().Equals(0))
            {
                //Check if no data exists
                if (!PrimaryPlottedSeries.Count.Equals(0))
                {
                    //Remove previous primary security reference data
                    List<PricingReferenceData> RemoveItems = PlottedSeries.Where(p => p.InstrumentID != PrimaryPlottedSeries.First().InstrumentID).ToList();
                    PlottedSeries.RemoveRange(RemoveItems);
                    PrimaryPlottedSeries.Clear();
                }

                ChartEntityList.Clear();
                //Retrieve Pricing Data for Primary Security Reference
                RetrievePricingData(new ObservableCollection<String>() { entitySelectionData.InstrumentID }, RetrievePricingReferenceDataCallBackMethod_SecurityReference);
                ChartEntityList.Add(entitySelectionData.InstrumentID);
            }

        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Retrieves Pricing Reference Data by making customized service call
        /// </summary>
        /// <param name="entityIdentifiers">List of Security Identifiers</param>
        /// <param name="callback">CallBack Method Predicate</param>
        private void RetrievePricingData(ObservableCollection<String> entityIdentifiers, Action<List<PricingReferenceData>> callback)
        {
            DateTime periodStartDate;
            DateTime periodEndDate;
            GetPeriod(out periodStartDate, out periodEndDate);
            _dbInteractivity.RetrievePricingReferenceData(entityIdentifiers, periodStartDate, periodEndDate, ReturnTypeSelection, FrequencyInterval, ChartEntityTypes, callback);
        }

        /// <summary>
        /// Get Period for Pricing Reference Data retrieval
        /// </summary>
        /// <param name="startDate">Data lower limit</param>
        /// <param name="endDate">Data upper limit</param>
        private void GetPeriod(out DateTime startDate, out DateTime endDate)
        {
            endDate = DateTime.Today;
            switch (SelectedTimeRange)
            {
                case "1-Month":
                    startDate = endDate.AddMonths(-1);
                    break;
                case "2-Months":
                    startDate = endDate.AddMonths(-2);
                    break;
                case "3-Months":
                    startDate = endDate.AddMonths(-3);
                    break;
                case "6-Months":
                    startDate = endDate.AddMonths(-6);
                    break;
                case "9-Months":
                    startDate = endDate.AddMonths(-9);
                    break;
                case "1-Year":
                    startDate = endDate.AddMonths(-12);
                    break;
                case "2-Years":
                    startDate = endDate.AddMonths(-24);
                    break;
                case "3-Years":
                    startDate = endDate.AddMonths(-36);
                    break;
                case "4-Years":
                    startDate = endDate.AddMonths(-48);
                    break;
                case "5-Years":
                    startDate = endDate.AddMonths(-60);
                    break;
                case "10-Years":
                    startDate = endDate.AddMonths(-120);
                    break;
                case "Custom":
                    startDate = SelectedStartDate;
                    endDate = SelectedEndDate;
                    break;
                default:
                    startDate = endDate.AddMonths(-12);
                    break;
            }

        }

        #endregion
    }
}
