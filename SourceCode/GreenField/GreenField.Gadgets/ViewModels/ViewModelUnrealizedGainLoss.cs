using System;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Prism.Events;
using GreenField.ServiceCaller;
using Microsoft.Practices.Prism.Logging;
using GreenField.ServiceCaller.ProxyDataDefinitions;
using GreenField.Common;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Linq;
using System.Collections.Generic;
using GreenField.Gadgets.Helpers;
using GreenField.Common.Helper;

namespace GreenField.Gadgets.ViewModels
{
    public class ViewModelUnrealizedGainLoss : NotificationObject
    {
        private IEventAggregator _eventAggregator;
        private IDBInteractivity _dbInteractivity;
        private ILoggerFacade _logger;
        private SecurityReferenceData _securityReferenceData;
        private EntitySelectionData _entitySelectionData;

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="eventAggregator">MEF Eventaggrigator instance</param>
        public ViewModelUnrealizedGainLoss(DashBoardGadgetParam param)
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
                    RetrievePricingData(new ObservableCollection<String>(PlottedSeries.Select(p => p.Ticker).Distinct().ToList()), RetrievePricingReferenceDataCallBackMethod_TimeRange);
                }
            }
        }

        private ObservableCollection<String> _timeRange;
        public ObservableCollection<String> TimeRange
        {
            get
            {
                return new ObservableCollection<string> { "1-Month", "2-Month", "3-Month", "6-Month", "YTD", "1-Year", "2-Years", 
                    "3-Years", "4-Years", "5-Years", "10-Years", "Custom" };
            }
            set
            {
                _timeRange = value;
                this.RaisePropertyChanged(() => this.TimeRange);
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

        private ObservableCollection<String> _cbTimeValues;
        public ObservableCollection<String> CbTimeValues
        {
            get
            {
                return new ObservableCollection<string> { "1-Month", "2-Month", "3-Month", "6-Month", "YTD", "1-Year", "2-Years", 
                    "3-Years", "4-Years", "5-Years", "10-Years", "Custom" };
            }

            set
            {
                _cbTimeValues = value;
                this.RaisePropertyChanged(() => this.CbTimeValues);
            }
        }

        private ObservableCollection<String> _cbFrequencyValues;
        public ObservableCollection<String> CbFrequencyValues
        {
            get
            {
                return new ObservableCollection<string> { "Daily", "Weekly", "Monthly", "Quarterly", "Yearly" };
            }

            set
            {
                _cbFrequencyValues = value;
                this.RaisePropertyChanged(() => this.CbFrequencyValues);
            }
        }

        private DateTime _selectedStartDate;
        public DateTime SelectedStartDate
        {
            get
            {
                return _selectedStartDate;
            }
            set
            {
                _selectedStartDate = value;
                this.RaisePropertyChanged("SelectedStartDate");
            }
        }

        private DateTime _selectedEndDate;
        public DateTime SelectedEndDate
        {
            get
            {
                return _selectedEndDate;
            }
            set
            {
                _selectedEndDate = value;
                this.RaisePropertyChanged("SelectedEndDate");
            }
        }

        public EntitySelectionData SelectedSeriesReference { get; set; }

        private CollectionViewSource _seriesReference;
        public CollectionViewSource SeriesReference
        {
            get { return _seriesReference; }
            set
            {
                _seriesReference = value;
                RaisePropertyChanged(() => this.SeriesReference);
            }
        }

        /// <summary>
        /// Type of entites added to chart
        /// if true: only securities added
        /// if false: Commodity/Index/Currency Added
        /// </summary>
        private bool _chartEntityTypes;
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

        public ObservableCollection<EntitySelectionData> SeriesReferenceSource { get; set; }

        //private string _selectedTime;
        //public string SelectedTime
        //{
        //    get
        //    {
        //        return _selectedTime;
        //    }
        //    set
        //    {
        //        _selectedTime = value;
        //        if (value == "1-Year")
        //        {
        //            XAxisDataPoint = "entityID";
        //        }
        //        else
        //            XAxisDataPoint = "closingPrice";

        //        this.RaisePropertyChanged("SelectedTime");

        //    }
        //}


        #endregion

        #region FrequencySelection

        private string _frequencyInterval;
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

        #endregion

        #region CallBack Methods

        private void RetrieveEntitySelectionDataCallBackMethod(List<EntitySelectionData> result)
        {
            SeriesReference = new CollectionViewSource();
            SeriesReferenceSource = new ObservableCollection<EntitySelectionData>(result);
            SeriesReference.GroupDescriptions.Add(new PropertyGroupDescription("EntityCategory"));
            SeriesReference.SortDescriptions.Add(new System.ComponentModel.SortDescription
            {
                PropertyName = "EntityCategory",
                Direction = System.ComponentModel.ListSortDirection.Ascending
            });
            SeriesReference.Source = SeriesReferenceSource;

        }

        private void RetrievePricingData(ObservableCollection<String> entityIdentifiers, Action<List<PricingReferenceData>> callback)
        {
            DateTime periodStartDate;
            DateTime periodEndDate;
            GetPeriod(out periodStartDate, out periodEndDate);
            _dbInteractivity.RetrievePricingReferenceData(entityIdentifiers, periodStartDate, periodEndDate, ReturnTypeSelection, FrequencyInterval, ChartEntityTypes, callback);
        }

        private void RetrievePricingReferenceDataCallBackMethod_TimeRange(List<PricingReferenceData> result)
        {
            //ObservableCollection<PricingReferenceData> objCollection = new ObservableCollection<PricingReferenceData>();
            //string primarySecurityReferenceIdentifier = PrimaryPlottedSeries.First().EntityIdentifier;
            //PrimaryPlottedSeries.Clear();
            //PlottedSeries.Clear();

            //foreach (PricingReferenceData item in result)
            //{
            //    PlottedSeries.Add(item);
            //    if (item.EntityIdentifier == primarySecurityReferenceIdentifier)
            //        objCollection.Add(item);
            //}
            //PrimaryPlottedSeries = objCollection;


            //ObservableCollection<PricingReferenceData> objCollection = new ObservableCollection<PricingReferenceData>();
            string primarySecurityReferenceIdentifier = PrimaryPlottedSeries.First().Ticker;
            PrimaryPlottedSeries.Clear();
            PlottedSeries.Clear();
            PrimaryPlottedSeries.AddRange((result.Where(r => r.Ticker == primarySecurityReferenceIdentifier)).ToList());
            PlottedSeries.AddRange(result);
        }

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

        #region Event Handlers
        /// <summary>
        /// Assigns UI Field Properties based on Security reference
        /// </summary>
        /// <param name="securityReferenceData">SecurityReferenceData</param>
        public void HandleSecurityReferenceSet(EntitySelectionData entitySelectionData)
        {
            //if (securityReferenceData != null)
            //{
            //    RetrievePricingData(new ObservableCollection<String>() { securityReferenceData.Ticker }, RetrievePricingReferenceDataCallBackMethod_SecurityReference);
            //_dbInteractivity.RetrievePricingReferenceData(new ObservableCollection<String>() { SelectedSeriesReference.EntityIdentifier }, DateTime.Today.AddYears(-1), DateTime.Today, (result) =>
            //{
            //    foreach (PricingReferenceData item in result)
            //        PlottedSeries.Add(item);
            //    //ComparisonSeries.Add(SelectedSeriesReference);
            //    SelectedSeriesReference = null;
            //});

            //ArgumentNullException
            if (entitySelectionData == null)
                return;

            //Check if security reference data is already present
            if (PrimaryPlottedSeries.Where(p => p.Ticker == entitySelectionData.ShortName).Count().Equals(0))
            {
                //Check if no data exists
                if (!PrimaryPlottedSeries.Count.Equals(0))
                {
                    //Remove previous primary security reference data
                    List<PricingReferenceData> RemoveItems = PlottedSeries.Where(p => p.Ticker != PrimaryPlottedSeries.First().Ticker).ToList();
                    PlottedSeries.RemoveRange(RemoveItems);
                    PrimaryPlottedSeries.Clear();
                }

                //Retrieve Pricing Data for Primary Security Reference
                RetrievePricingData(new ObservableCollection<String>() { entitySelectionData.ShortName }, RetrievePricingReferenceDataCallBackMethod_SecurityReference);
            }
        }

        private void RetrievePricingReferenceDataCallBackMethod_SecurityReference(List<PricingReferenceData> result)
        {
            //foreach (PricingReferenceData item in result)
            //{
            //    //PlottedSeries.Add(item);
            //    //PrimaryPlottedSeries.Add(item);
            //    objCollection.Add(item);
            //    objCollectionPrimary.Add(item);
            //}
            //PlottedSeries = objCollection;
            //PrimaryPlottedSeries = objCollectionPrimary;


            PlottedSeries.AddRange(result);
            PrimaryPlottedSeries.AddRange(result);
        }

        #endregion
    }
}
