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
using GreenField.Gadgets.Views;
using Telerik.Windows.Controls.Charting;
using Telerik.Windows.Controls;

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
        private ObservableCollection<EntitySelectionData> _chartEntityList;
        public ObservableCollection<EntitySelectionData> ChartEntityList
        {
            get
            {
                if (_chartEntityList == null)
                    _chartEntityList = new ObservableCollection<EntitySelectionData>();
                return _chartEntityList;
            }
            set
            {
                _chartEntityList = value;
                if (ChartEntityList.Count != 0)
                {
                    SelectedBaseSecurity = ChartEntityList[0].ToString();
                }
                this.RaisePropertyChanged(() => this.ChartEntityList);
            }
        }

        private string _selectedBaseSecurity = "No Security Added";
        public string SelectedBaseSecurity
        {
            get
            {
                return _selectedBaseSecurity;
            }
            set
            {
                _selectedBaseSecurity = value;
                this.RaisePropertyChanged(() => this.SelectedBaseSecurity);
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
                return new ObservableCollection<string> { "1-Month", "2-Months", "3-Months", "6-Months", "YTD", "1-Year", "2-Years", 
                    "3-Years", "4-Years", "5-Years", "10-Years", "Custom" };
            }
        }

        /// <summary>
        /// Selection Time Range option
        /// </summary>
        private string _selectedTimeRange = "1-Year";
        public string SelectedTimeRange
        {
            get
            {
                return _selectedTimeRange;
            }
            set
            {
                if (_selectedTimeRange != value)
                {
                    if (value == "Custom")
                    {
                        ViewCustomDateChildWindow customDateWindow = new ViewCustomDateChildWindow();
                        customDateWindow.Show();
                        customDateWindow.Unloaded += (se, e) =>
                        {
                            if (Convert.ToBoolean(customDateWindow.enteredDateCorrect))
                            {
                                SelectedStartDate = Convert.ToDateTime(customDateWindow.startDate);
                                SelectedEndDate = Convert.ToDateTime(customDateWindow.endDate);
                                _selectedTimeRange = value;

                                //Retrieve Pricing Data for updated Time Range
                                if (ChartEntityList.Count != 0)
                                {
                                    RetrievePricingData(ChartEntityList,
                                            RetrievePricingReferenceDataCallBackMethod_TimeRange);
                                }
                                this.RaisePropertyChanged(() => this.SelectedTimeRange);
                            }
                            else
                            {
                                _selectedTimeRange = "1-Year";
                                this.RaisePropertyChanged(() => this.SelectedTimeRange);
                            }
                        };
                    }
                    else
                    {
                        _selectedTimeRange = value;
                        GetPeriod();
                        //Retrieve Pricing Data for updated Time Range
                        if (ChartEntityList.Count != 0)
                        {
                            RetrievePricingData(ChartEntityList,
                                    RetrievePricingReferenceDataCallBackMethod_TimeRange);
                        }
                        this.RaisePropertyChanged(() => this.SelectedTimeRange);
                    }
                }
            }
        }

        /// <summary>
        /// Selected StartDate Option in case of Custom Time Range
        /// </summary>
        private DateTime _selectedStartDate = DateTime.Now.AddYears(-1);
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
        private DateTime _selectedEndDate = DateTime.Today;
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

        private ObservableCollection<string> _frequencyInterval;
        public ObservableCollection<string> FrequencyInterval
        {
            get
            {
                if (_frequencyInterval == null)
                {
                    _frequencyInterval = new ObservableCollection<string>();
                    _frequencyInterval.Add("Daily");
                    _frequencyInterval.Add("Weekly");
                    _frequencyInterval.Add("Monthly");
                    _frequencyInterval.Add("Quarterly");
                    _frequencyInterval.Add("Half-Yearly");
                    _frequencyInterval.Add("Yearly");
                }
                return _frequencyInterval;
            }
            set
            {
                _frequencyInterval = value;
                this.RaisePropertyChanged(() => this.FrequencyInterval);
            }
        }

        private string _selectedFrequencyInterval = "Daily";
        public string SelectedFrequencyInterval
        {
            get
            {
                return _selectedFrequencyInterval;
            }
            set
            {
                _selectedFrequencyInterval = value;
                GetXAxisDataFormat();
                if (ChartEntityList.Count != 0)
                {
                    RetrievePricingData(ChartEntityList,
                            RetrievePricingReferenceDataCallBackMethod_TimeRange);
                }
                this.RaisePropertyChanged(() => this.SelectedFrequencyInterval);
            }
        }

        private string _chartXAxisDataFormat;
        public string ChartXAxisDataFormat
        {
            get
            {
                return _chartXAxisDataFormat;
            }
            set
            {
                _chartXAxisDataFormat = value;
                this.RaisePropertyChanged(() => this.ChartXAxisDataFormat);
            }
        }

        private double _closingPriceChartXMinimumValue = DateTime.Today.AddYears(-1).ToOADate();
        public double ClosingPriceChartXMinimumValue
        {
            get
            {
                return _closingPriceChartXMinimumValue;
            }
            set
            {
                _closingPriceChartXMinimumValue = value;
                this.RaisePropertyChanged(() => this.ClosingPriceChartXMinimumValue);
            }
        }

        private double _closingPriceChartXMaximumValue = DateTime.Today.ToOADate();
        public double ClosingPriceChartXMaximumValue
        {
            get
            {
                return _closingPriceChartXMaximumValue;
            }
            set
            {
                _closingPriceChartXMaximumValue = value;
                this.RaisePropertyChanged(() => this.ClosingPriceChartXMaximumValue);
            }
        }

        private double _closingPriceChartStep;
        public double ClosingPriceChartStep
        {
            get
            {
                return _closingPriceChartStep;
            }
            set
            {
                _closingPriceChartStep = value;
                this.RaisePropertyChanged(() => this.ClosingPriceChartStep);
            }
        }

        private double _levelCount = 1;
        public double LevelCount
        {
            get
            {
                return _levelCount;
            }
            set
            {
                _levelCount = value;
                this.RaisePropertyChanged(() => this.LevelCount);
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
                if (_returnTypeSelection != value)
                {
                    _returnTypeSelection = value;
                    if (ChartEntityList.Count != 0)
                    {
                        RetrievePricingData(ChartEntityList,
                                RetrievePricingReferenceDataCallBackMethod_TimeRange);
                    }
                    this.RaisePropertyChanged(() => this.ReturnTypeSelection);
                }
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

        private ChartArea _chartAreaPricing;
        public ChartArea ChartAreaPricing
        {
            get
            {
                return this._chartAreaPricing;
            }
            set
            {
                this._chartAreaPricing = value;
            }
        }

        private ChartArea _chartAreaVolume;
        public ChartArea ChartAreaVolume
        {
            get
            {
                return this._chartAreaVolume;
            }
            set
            {
                this._chartAreaVolume = value;
            }
        }

        #region ICommand
        public ICommand AddCommand
        {
            get { return new DelegateCommand<object>(AddCommandMethod); }
        }

        public ICommand DeleteCommand
        {
            get { return new DelegateCommand<object>(DeleteCommandMethod); }
        }

        private ICommand _zoomInCommand;
        public ICommand ZoomInCommand
        {
            get
            {
                if (_zoomInCommand == null)
                {
                    _zoomInCommand = new Telerik.Windows.Controls.DelegateCommand(ZoomInCommandMethod, ZoomInCommandValidation);
                }
                return _zoomInCommand;
            }
        }

        private ICommand _zoomOutCommand;
        public ICommand ZoomOutCommand
        {
            get
            {
                if (_zoomOutCommand == null)
                {
                    _zoomOutCommand = new Telerik.Windows.Controls.DelegateCommand(ZoomOutCommandMethod, ZoomOutCommandValidation);
                }
                return _zoomOutCommand;
            }
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
                    ChartEntityList.Add(SelectedSeriesReference);

                    //Making initially ChartEntityTypes False
                    ChartEntityTypes = true;

                    //List<EntitySelectionData> objEntitySelectionData= new List<EntitySelectionData>(SeriesReference);

                    //Checking the types of entity in the Chart
                    foreach (EntitySelectionData item in SeriesReferenceSource.Where(r => r.Type != "SECURITY").ToList())
                    {
                        List<EntitySelectionData> a = SeriesReferenceSource.Where(r => r.Type != "SECURITY").ToList();
                        //If it contains type Commodity/Index/Currency, ChartEntityTypes=true else false
                        if (ChartEntityList.Contains(item))
                        {
                            ChartEntityTypes = false;
                        }
                    }

                    DateTime periodStartDate;
                    DateTime periodEndDate;


                    _dbInteractivity.RetrievePricingReferenceData(ChartEntityList, SelectedStartDate, SelectedEndDate, ReturnTypeSelection, SelectedFrequencyInterval, ChartEntityTypes, (result) =>
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
            ChartEntityList.Remove(a);
        }

        public void ZoomInCommandMethod(object parameter)
        {
            ZoomIn(this.ChartAreaPricing);
            ZoomIn(this.ChartAreaVolume);
            ((Telerik.Windows.Controls.DelegateCommand)_zoomInCommand).InvalidateCanExecute();
            ((Telerik.Windows.Controls.DelegateCommand)_zoomOutCommand).InvalidateCanExecute();
        }

        public bool ZoomInCommandValidation(object parameter)
        {
            if (this.ChartAreaPricing == null || this.ChartAreaVolume == null)
                return false;

            return
                this.ChartAreaPricing.ZoomScrollSettingsX.Range > this.ChartAreaPricing.ZoomScrollSettingsX.MinZoomRange &&
                this.ChartAreaVolume.ZoomScrollSettingsX.Range > this.ChartAreaVolume.ZoomScrollSettingsX.MinZoomRange;
        }

        public void ZoomOutCommandMethod(object parameter)
        {
            ZoomOut(this.ChartAreaPricing);
            ZoomOut(this.ChartAreaVolume);
            ((Telerik.Windows.Controls.DelegateCommand)_zoomInCommand).InvalidateCanExecute();
            ((Telerik.Windows.Controls.DelegateCommand)_zoomOutCommand).InvalidateCanExecute();
        }

        public bool ZoomOutCommandValidation(object parameter)
        {
            if (this.ChartAreaPricing == null || this.ChartAreaVolume == null)
                return false;

            return this.ChartAreaPricing.ZoomScrollSettingsX.Range < 1d &&
                this.ChartAreaVolume.ZoomScrollSettingsX.Range < 1d;
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
            if (null != closingPriceDataLoadedEvent)
                closingPriceDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
        }

        /// <summary>
        /// Callback method for Pricing Reference Service call related to updated Security Reference - Updates Chart and Grid
        /// </summary>
        /// <param name="result">PricingReferenceData collection</param>
        private void RetrievePricingReferenceDataCallBackMethod_SecurityReference(List<PricingReferenceData> result)
        {
            if (result != null)
            {
                PlottedSeries.AddRange(result);
                PrimaryPlottedSeries.AddRange(result);
            }

            if (null != closingPriceDataLoadedEvent)
                closingPriceDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
        }

        #endregion

        #region Events
        public event DataRetrievalProgressIndicator closingPriceDataLoadedEvent;
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
                    //PlottedSeries.RemoveRange(RemoveItems);
                    //PrimaryPlottedSeries.Clear();
                    PlottedSeries.Clear();
                    PrimaryPlottedSeries.Clear();
                }

                ChartEntityList.Clear();
                ChartEntityList.Add(entitySelectionData);
                //Retrieve Pricing Data for Primary Security Reference
                if (null != closingPriceDataLoadedEvent)
                    closingPriceDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
                RetrievePricingData(ChartEntityList, RetrievePricingReferenceDataCallBackMethod_SecurityReference);

                SelectedBaseSecurity = entitySelectionData.ShortName.ToString();
            }

        }


        public void ChartDataBound(object sender, ChartDataBoundEventArgs e)
        {
            ((Telerik.Windows.Controls.DelegateCommand)_zoomInCommand).InvalidateCanExecute();
            ((Telerik.Windows.Controls.DelegateCommand)_zoomOutCommand).InvalidateCanExecute();
        }
        #endregion

        #region Helper Methods

        /// <summary>
        /// Retrieves Pricing Reference Data by making customized service call
        /// </summary>
        /// <param name="entityIdentifiers">List of Security Identifiers</param>
        /// <param name="callback">CallBack Method Predicate</param>
        private void RetrievePricingData(ObservableCollection<EntitySelectionData> entityIdentifiers, Action<List<PricingReferenceData>> callback)
        {
            if (null != closingPriceDataLoadedEvent)
                closingPriceDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
            _dbInteractivity.RetrievePricingReferenceData(entityIdentifiers, SelectedStartDate, SelectedEndDate, ReturnTypeSelection, SelectedFrequencyInterval, ChartEntityTypes, callback);
        }

        /// <summary>
        /// Get Period for Pricing Reference Data retrieval
        /// </summary>
        /// <param name="startDate">Data lower limit</param>
        /// <param name="endDate">Data upper limit</param>
        private void GetPeriod()
        {
            SelectedEndDate = DateTime.Today;
            switch (SelectedTimeRange)
            {
                case "1-Month":
                    SelectedStartDate = SelectedEndDate.AddMonths(-1);
                    break;
                case "2-Months":
                    SelectedStartDate = SelectedEndDate.AddMonths(-2);
                    break;
                case "3-Months":
                    SelectedStartDate = SelectedEndDate.AddMonths(-3);
                    break;
                case "6-Months":
                    SelectedStartDate = SelectedEndDate.AddMonths(-6);
                    break;
                case "9-Months":
                    SelectedStartDate = SelectedEndDate.AddMonths(-9);
                    break;
                case "1-Year":
                    SelectedStartDate = SelectedEndDate.AddMonths(-12);
                    break;
                case "2-Years":
                    SelectedStartDate = SelectedEndDate.AddMonths(-24);
                    break;
                case "3-Years":
                    SelectedStartDate = SelectedEndDate.AddMonths(-36);
                    break;
                case "4-Years":
                    SelectedStartDate = SelectedEndDate.AddMonths(-48);
                    break;
                case "5-Years":
                    SelectedStartDate = SelectedEndDate.AddMonths(-60);
                    break;
                case "10-Years":
                    SelectedStartDate = SelectedEndDate.AddMonths(-120);
                    break;
                case "YTD":
                    SelectedEndDate = DateTime.Today;
                    SelectedStartDate = new DateTime((int)(DateTime.Today.Year), 1, 1);
                    break;
                default:
                    SelectedStartDate = SelectedEndDate.AddMonths(-12);
                    break;
            }

        }

        private void GetXAxisDataFormat()
        {
            TimeSpan timeSpan = SelectedEndDate - SelectedStartDate;
            ClosingPriceChartXMinimumValue = SelectedStartDate.ToOADate();
            ClosingPriceChartXMaximumValue = SelectedEndDate.ToOADate();

            switch (SelectedFrequencyInterval)
            {
                case ("Daily"):
                    {
                        ChartXAxisDataFormat = "d";
                        LevelCount = timeSpan.Days;
                        break;
                    }
                case ("Weekly"):
                    {
                        ChartXAxisDataFormat = "d";
                        LevelCount = timeSpan.Days / 7;
                        break;
                    }
                case ("Monthly"):
                    {
                        ChartXAxisDataFormat = "Y";
                        LevelCount = timeSpan.Days / 30;
                        break;
                    }
                case ("Half-Yearly"):
                    {
                        ChartXAxisDataFormat = "Y";
                        LevelCount = timeSpan.Days / 180;
                        break;
                    }
                case ("Yearly"):
                    {
                        ChartXAxisDataFormat = "yyyy";
                        LevelCount = timeSpan.Days / 365;
                        break;
                    }

                default:
                    ChartXAxisDataFormat = "M";
                    break;
            }
        }

        private void ZoomIn(ChartArea chartArea)
        {
            chartArea.ZoomScrollSettingsX.SuspendNotifications();

            double zoomCenter = chartArea.ZoomScrollSettingsX.RangeStart + (chartArea.ZoomScrollSettingsX.Range / 2);
            double newRange = Math.Max(chartArea.ZoomScrollSettingsX.MinZoomRange, chartArea.ZoomScrollSettingsX.Range) / 2;
            chartArea.ZoomScrollSettingsX.RangeStart = Math.Max(0, zoomCenter - (newRange / 2));
            chartArea.ZoomScrollSettingsX.RangeEnd = Math.Min(1, zoomCenter + (newRange / 2));

            chartArea.ZoomScrollSettingsX.ResumeNotifications();
        }

        private void ZoomOut(ChartArea chartArea)
        {
            chartArea.ZoomScrollSettingsX.SuspendNotifications();

            double zoomCenter = chartArea.ZoomScrollSettingsX.RangeStart + (chartArea.ZoomScrollSettingsX.Range / 2);
            double newRange = Math.Min(1, chartArea.ZoomScrollSettingsX.Range) * 2;

            if (zoomCenter + (newRange / 2) > 1)
                zoomCenter = 1 - (newRange / 2);
            else if (zoomCenter - (newRange / 2) < 0)
                zoomCenter = newRange / 2;

            chartArea.ZoomScrollSettingsX.RangeStart = Math.Max(0, zoomCenter - newRange / 2);
            chartArea.ZoomScrollSettingsX.RangeEnd = Math.Min(1, zoomCenter + newRange / 2);

            chartArea.ZoomScrollSettingsX.ResumeNotifications();
        }

        #endregion
    }
}
