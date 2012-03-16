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

        /// <summary>
        /// Collection binded to the Comparison chart - consists of unrealized gain loss and pricing data for the primary entity
        /// </summary>
        private RangeObservableCollection<UnrealizedGainLossData> _plottedSeries;
        public RangeObservableCollection<UnrealizedGainLossData> PlottedSeries
        {
            get
            {
                if (_plottedSeries == null)
                    _plottedSeries = new RangeObservableCollection<UnrealizedGainLossData>();
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

        #region Time Period Selection
        /// <summary>
        /// Collection of Time Range Options
        /// </summary>
        private ObservableCollection<String> _timeRange;
        public ObservableCollection<String> TimeRange
        {
            get
            {
                return new ObservableCollection<string> { "1-Month", "2-Month", "3-Month", "6-Month", "9-Month", "1-Year", "2-Years", 
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
                    
                    if(_entitySelectionData != null)
                        RetrieveUnrealizedGainLossData(_entitySelectionData.ShortName, RetrieveUnrealizedGainLossDataCallBackMethod_TimeRange);
                    RaisePropertyChanged(() => this.SelectedTimeRange);                                     
                }
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


        public ObservableCollection<EntitySelectionData> SeriesReferenceSource { get; set; }

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

        private void RetrieveUnrealizedGainLossData(String Ticker, Action<List<UnrealizedGainLossData>> callback)
        {
            DateTime periodStartDate;
            DateTime periodEndDate;
            GetPeriod(out periodStartDate, out periodEndDate);
            _dbInteractivity.RetrieveUnrealizedGainLossData(Ticker, periodStartDate, periodEndDate, callback);
        }

        private void RetrieveUnrealizedGainLossDataCallBackMethod_TimeRange(List<UnrealizedGainLossData> result)
        {                       
            PlottedSeries.Clear();
            
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
                default:
                    startDate = endDate.AddMonths(-12);
                    break;
            }
        }
        #endregion
        #endregion

        #region Event Handlers
        /// <summary>
        /// Assigns UI Field Properties based on Security reference
        /// </summary>
        /// <param name="securityReferenceData">SecurityReferenceData</param>
        public void HandleSecurityReferenceSet(EntitySelectionData entitySelectionData)
        {            
            if (entitySelectionData == null)
                return;

            _entitySelectionData = entitySelectionData;            
            
            RetrieveUnrealizedGainLossData(entitySelectionData.ShortName , RetrieveUnrealizedGainLossDataCallBackMethod_SecurityReference);            
        }

        private void RetrieveUnrealizedGainLossDataCallBackMethod_SecurityReference(List<UnrealizedGainLossData> result)
        {           
            PlottedSeries.Clear();
            PlottedSeries.AddRange(result);
            List<UnrealizedGainLossData> d = PlottedSeries.ToList();            
        }

        #endregion
    }
}
