using System;
using System.Windows;
using System.Windows.Input;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Data;
using System.Collections.ObjectModel;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Prism.Events;
using Telerik.Windows.Controls.Charting;
using GreenField.Common;
using GreenField.Gadgets.Helpers;
using GreenField.DataContracts;
using GreenField.ServiceCaller.SecurityReferenceDefinitions;
using GreenField.ServiceCaller;

namespace GreenField.Gadgets.ViewModels
{
    public class ViewModelClosingPriceChart : NotificationObject
    {
        /// <summary>
        /// MEF Singletons
        /// </summary>
        #region Private Fields

        ///Instance of IDBInteractivity        
        private IDBInteractivity dbInteractivity;

        /// <summary>
        /// Instance of ILoggerFacade
        /// </summary>
        public ILoggerFacade logger;

        /// <summary>
        /// Instance of IEventAggregator
        /// </summary>
        private IEventAggregator eventAggregator;

        /// <summary>
        /// Instance of EntitySelectionData
        /// </summary>
        private EntitySelectionData entitySelectionData;

        /// <summary>
        /// IsActive is true when parent control is displayed on UI
        /// </summary>
        private bool isActive;
        public bool IsActive
        {
            get
            {
                return isActive;
            }
            set
            {
                isActive = value;
                if (entitySelectionData != null && isActive)
                {
                    HandleSecurityReferenceSet(entitySelectionData);
                }
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="eventAggregator">Event Aggregation from Shell</param>
        /// <param name="dbInteractivity">Instance of Service Caller</param>
        /// <param name="logger">Instance of Logger</param>
        /// <param name="entitySelectionData"></param>
        public ViewModelClosingPriceChart(DashboardGadgetParam param)
        {
            dbInteractivity = param.DBInteractivity;
            logger = param.LoggerFacade;
            eventAggregator = param.EventAggregator;
            entitySelectionData = param.DashboardGadgetPayload.EntitySelectionData;

            if (SelectionData.EntitySelectionData != null && SeriesReferenceSource == null)
            {
                RetrieveEntitySelectionDataCallBackMethod(SelectionData.EntitySelectionData);
            }
            eventAggregator.GetEvent<SecurityReferenceSetEvent>().Subscribe(HandleSecurityReferenceSet, false);

            if (entitySelectionData != null)
            {
                HandleSecurityReferenceSet(entitySelectionData);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Storing the names of all entities added to chart.
        /// </summary>
        private ObservableCollection<EntitySelectionData> chartEntityList;
        public ObservableCollection<EntitySelectionData> ChartEntityList
        {
            get
            {
                if (chartEntityList == null)
                {
                    chartEntityList = new ObservableCollection<EntitySelectionData>();
                }
                if (chartEntityList.Count >= 1)
                {
                    AddToChartVisibility = "Visible";
                }
                else
                {
                    AddToChartVisibility = "Collapsed";
                }
                return chartEntityList;
            }
            set
            {
                chartEntityList = value;
                if (ChartEntityList.Count != 0)
                {
                    SelectedBaseSecurity = ChartEntityList[0].ToString();
                }
                this.RaisePropertyChanged(() => this.ChartEntityList);
            }
        }

        /// <summary>
        /// Display the name of Base Security Selected
        /// </summary>
        private string selectedBaseSecurity = "No Security Added";
        public string SelectedBaseSecurity
        {
            get
            {
                return selectedBaseSecurity;
            }
            set
            {
                selectedBaseSecurity = value;
                this.RaisePropertyChanged(() => this.SelectedBaseSecurity);
            }
        }

        #region Time Period Selection
        
        /// <summary>
        /// Collection of Time Range Options
        /// </summary>
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
        private string selectedTimeRange = "1-Year";
        public string SelectedTimeRange
        {
            get
            {
                return selectedTimeRange;
            }
            set
            {
                if ((selectedTimeRange != value) || (value == "Custom"))
                {
                    if (value == "Custom")
                    {
                        selectedTimeRange = value;

                        //Retrieve Pricing Data for updated Time Range
                        if (ChartEntityList.Count != 0)
                        {
                            RetrievePricingData(ChartEntityList, RetrievePricingReferenceDataCallBackMethod_TimeRange);
                        }
                        this.RaisePropertyChanged(() => this.SelectedTimeRange);
                    }
                    else
                    {
                        selectedTimeRange = value;
                        GetPeriod();
                        //Retrieve Pricing Data for updated Time Range
                        if (ChartEntityList.Count != 0)
                        {
                            RetrievePricingData(ChartEntityList, RetrievePricingReferenceDataCallBackMethod_TimeRange);
                        }
                        this.RaisePropertyChanged(() => this.SelectedTimeRange);
                    }
                }
            }
        }


        /// <summary>
        /// Selected StartDate Option in case of Custom Time Range
        /// </summary>
        private DateTime selectedStartDate = DateTime.Now.AddYears(-1);
        public DateTime SelectedStartDate
        {
            get { return selectedStartDate; }
            set
            {
                if (selectedStartDate != value)
                {
                    selectedStartDate = value;
                    RaisePropertyChanged(() => this.SelectedStartDate);
                }
            }
        }

        /// <summary>
        /// Selected EndDate Option in case of Custom Time Range
        /// </summary>
        private DateTime selectedEndDate = DateTime.Today;
        public DateTime SelectedEndDate
        {
            get { return selectedEndDate; }
            set
            {
                if (selectedEndDate != value)
                {
                    selectedEndDate = value;
                    RaisePropertyChanged(() => this.SelectedEndDate);
                }
            }
        }
        #endregion

        #region FrequencySelection

        /// <summary>
        /// Frequency Interval for chart
        /// </summary>
        private ObservableCollection<string> frequencyInterval;
        public ObservableCollection<string> FrequencyInterval
        {
            get
            {
                if (frequencyInterval == null)
                {
                    frequencyInterval = new ObservableCollection<string>();
                    frequencyInterval.Add("Daily");
                    frequencyInterval.Add("Weekly");
                    frequencyInterval.Add("Monthly");
                    frequencyInterval.Add("Quarterly");
                    frequencyInterval.Add("Half-Yearly");
                    frequencyInterval.Add("Yearly");
                }
                return frequencyInterval;
            }
            set
            {
                frequencyInterval = value;
                this.RaisePropertyChanged(() => this.FrequencyInterval);
            }
        }

        /// <summary>
        /// Selected Frequency interval
        /// </summary>
        private string selectedFrequencyInterval = "Daily";
        public string SelectedFrequencyInterval
        {
            get
            {
                return selectedFrequencyInterval;
            }
            set
            {
                selectedFrequencyInterval = value;
                if (ChartEntityList.Count != 0)
                {
                    RetrievePricingData(ChartEntityList,
                            RetrievePricingReferenceDataCallBackMethod_TimeRange);
                }
                this.RaisePropertyChanged(() => this.SelectedFrequencyInterval);
            }
        }

        #endregion

        #region Plotting Additional Series
        /// <summary>
        /// Grouped Collection View for Auto-Complete Box
        /// </summary>
        private CollectionViewSource seriesReference;
        public CollectionViewSource SeriesReference
        {
            get
            {
                return seriesReference;
            }
            set
            {
                seriesReference = value;
                RaisePropertyChanged(() => this.SeriesReference);
            }
        }

        /// <summary>
        /// DataSource for the Grouped Collection View
        /// </summary>
        public ObservableCollection<EntitySelectionData> SeriesReferenceSource { get; set; }

        /// <summary>
        /// Selected Entity
        /// </summary>
        private EntitySelectionData selectedSeriesReference = new EntitySelectionData();
        public EntitySelectionData SelectedSeriesReference
        {
            get
            {
                return selectedSeriesReference;
            }
            set
            {
                selectedSeriesReference = value;
                this.RaisePropertyChanged(() => this.SelectedSeriesReference);
            }
        }

        /// <summary>
        /// Search Mode Filter - Checked (StartsWith); Unchecked (Contains)
        /// </summary>
        private bool searchFilterEnabled;
        public bool SearchFilterEnabled
        {
            get { return searchFilterEnabled; }
            set
            {
                if (searchFilterEnabled != value)
                {
                    searchFilterEnabled = value;
                    RaisePropertyChanged(() => SearchFilterEnabled);
                }
            }
        }

        /// <summary>
        /// Entered Text in the Auto-Complete Box - filters SeriesReferenceSource
        /// </summary>
        private string seriesEnteredText;
        public string SeriesEnteredText
        {
            get { return seriesEnteredText; }
            set
            {
                seriesEnteredText = value;
                RaisePropertyChanged(() => this.SeriesEnteredText);
                if (value != null)
                {
                    SeriesReference.Source = SearchFilterEnabled == false
                          ? SeriesReferenceSource.Where(o => o.ShortName.ToLower().Contains(value.ToLower()))
                          : SeriesReferenceSource.Where(o => o.ShortName.ToLower().StartsWith(value.ToLower()));
                }
                else
                {
                    SeriesReference.Source = SeriesReferenceSource;
                }
            }
        }

        /// <summary>
        /// Type of entites added to chart
        /// if true:Commodity/Index/Currency Added
        /// if false:only securities added 
        /// </summary>
        private bool chartEntityTypes = true;
        public bool ChartEntityTypes
        {
            get
            {
                return chartEntityTypes;
            }
            set
            {
                chartEntityTypes = value;
                this.RaisePropertyChanged(() => this.ChartEntityTypes);
            }
        }

        #endregion

        #region Chart/Grid Entities

        /// <summary>
        /// New Entity
        /// </summary>
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

        /// <summary>
        /// CheckBox Selection for Total Return/Gross Return
        /// </summary>
        private bool returnTypeSelection;
        public bool ReturnTypeSelection
        {
            get
            {
                return returnTypeSelection;
            }
            set
            {
                if (returnTypeSelection != value)
                {
                    returnTypeSelection = value;
                    if (ChartEntityList.Count != 0)
                    {
                        RetrievePricingData(ChartEntityList,
                                RetrievePricingReferenceDataCallBackMethod_TimeRange);
                    }
                    if (returnTypeSelection)
                    {
                        SelectedBaseSecurity = SelectedBaseSecurity + " (total)";
                        foreach (EntitySelectionData item in ComparisonSeries)
                        {
                            if (item.Type == "SECURITY")
                            {
                                item.ShortName = item.ShortName + " (total)";
                            }
                        }
                    }
                    else
                    {
                        SelectedBaseSecurity = SelectedBaseSecurity.Replace(" (total)", "");
                        foreach (EntitySelectionData item in ComparisonSeries)
                        {
                            if (item.Type == "SECURITY")
                            {
                                item.ShortName = item.ShortName.Replace(" (total)", "");
                            }
                        }
                    }
                    this.RaisePropertyChanged(() => this.ReturnTypeSelection);
                }
            }
        }

        #endregion

        /// <summary>
        /// Plotted Series on the Chart
        /// </summary>
        private RangeObservableCollection<PricingReferenceData> plottedSeries;
        public RangeObservableCollection<PricingReferenceData> PlottedSeries
        {
            get
            {
                if (plottedSeries == null)
                    plottedSeries = new RangeObservableCollection<PricingReferenceData>();
                return plottedSeries;
            }
            set
            {
                if (plottedSeries != value)
                {
                    plottedSeries = value;
                    RaisePropertyChanged(() => this.PlottedSeries);
                }
            }
        }

        /// <summary>
        /// Series bound to Volume Chart
        /// </summary>
        private RangeObservableCollection<PricingReferenceData> primaryPlottedSeries;
        public RangeObservableCollection<PricingReferenceData> PrimaryPlottedSeries
        {
            get
            {
                if (primaryPlottedSeries == null)
                    primaryPlottedSeries = new RangeObservableCollection<PricingReferenceData>();
                return primaryPlottedSeries;
            }
            set
            {
                if (primaryPlottedSeries != value)
                {
                    primaryPlottedSeries = value;
                    RaisePropertyChanged(() => this.PrimaryPlottedSeries);
                }
            }
        }

        /// <summary>
        /// Series to show List of Securities Added to chart
        /// </summary>
        private ObservableCollection<EntitySelectionData> comparisonSeries = new ObservableCollection<EntitySelectionData>();
        public ObservableCollection<EntitySelectionData> ComparisonSeries
        {
            get
            {
                return comparisonSeries;
            }
            set
            {
                comparisonSeries = value;
                this.RaisePropertyChanged(() => this.ComparisonSeries);
            }
        }

        /// <summary>
        /// Busy Indicator Status
        /// </summary>
        private bool busyIndicatorStatus;
        public bool BusyIndicatorStatus
        {
            get
            {
                return busyIndicatorStatus;
            }
            set
            {
                busyIndicatorStatus = value;
                this.RaisePropertyChanged(() => this.BusyIndicatorStatus);
            }
        }

        /// <summary>
        /// Bound to ChartArea in View(Pricing Chart)
        /// </summary>
        private ChartArea chartAreaPricing;
        public ChartArea ChartAreaPricing
        {
            get
            {
                return this.chartAreaPricing;
            }
            set
            {
                this.chartAreaPricing = value;
            }
        }

        /// <summary>
        /// Bound to ChartArea in View(Volume Chart)
        /// </summary>
        private ChartArea chartAreaVolume;
        public ChartArea ChartAreaVolume
        {
            get
            {
                return this.chartAreaVolume;
            }
            set
            {
                this.chartAreaVolume = value;
            }
        }

        /// <summary>
        /// Show/Hide Add to Chart Control
        /// </summary>
        private string addToChartVisibility = "Collapsed";
        public string AddToChartVisibility
        {
            get
            {
                return addToChartVisibility;
            }
            set
            {
                addToChartVisibility = value;
                this.RaisePropertyChanged(() => this.AddToChartVisibility);
            }
        }

        #region ICommand
        /// <summary>
        /// Add to chart method
        /// </summary>
        public ICommand AddCommand
        {
            get { return new DelegateCommand<object>(AddCommandMethod); }
        }

        /// <summary>
        /// Delete Series from Chart
        /// </summary>
        public ICommand DeleteCommand
        {
            get { return new DelegateCommand<object>(DeleteCommandMethod); }
        }

        /// <summary>
        /// Zoom-In Command Button
        /// </summary>
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

        /// <summary>
        /// Zoom-Out Command Button
        /// </summary>
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

        /// <summary>
        /// Add to Chart Command Method
        /// </summary>
        /// <param name="param"></param>
        private void AddCommandMethod(object param)
        {
            if (SelectedSeriesReference != null)
            {
                if (!PlottedSeries.Any(t => t.InstrumentID == SelectedSeriesReference.InstrumentID))
                {
                    if (ChartEntityList.Count >= 5)
                    {
                        Prompt.ShowDialog("Cannot Add more than 5 Entities at a Time");
                        return;
                    }

                    //string type = SelectedSeriesReference.Type.ToString();
                    ChartEntityList.Add(SelectedSeriesReference);

                    //Making initially ChartEntityTypes False
                    ChartEntityTypes = true;

                    BusyIndicatorStatus = true;
                    dbInteractivity.RetrievePricingReferenceData(ChartEntityList, SelectedStartDate, SelectedEndDate, ReturnTypeSelection, SelectedFrequencyInterval, (result) =>
                    {
                        PlottedSeries.Clear();
                        PlottedSeries.AddRange(result.OrderBy(a => a.SortingID).ToList());
                        BusyIndicatorStatus = false;
                        ComparisonSeries.Add(SelectedSeriesReference);
                        if (ReturnTypeSelection)
                        {
                            foreach (EntitySelectionData item in (ComparisonSeries))
                            {
                                if (item.InstrumentID == SelectedSeriesReference.InstrumentID)
                                {
                                    item.ShortName = item.ShortName + " (total)";
                                }
                            }
                        }
                        SelectedSeriesReference = null;
                    });
                }
            }
        }

        /// <summary>
        /// Delete Series from Chart
        /// </summary>
        /// <param name="param"></param>
        private void DeleteCommandMethod(object param)
        {
            EntitySelectionData a = param as EntitySelectionData;
            List<PricingReferenceData> removeItem = new List<PricingReferenceData>();
            removeItem = PlottedSeries.Where(w => w.InstrumentID == a.InstrumentID).ToList();
            if (removeItem != null)
            {
                PlottedSeries.RemoveRange(removeItem);
            }
            ComparisonSeries.Remove(a);
            ChartEntityList.Remove(a);
            if (ChartEntityList.Count == 1)
            {
                RetrievePricingData(ChartEntityList, RetrievePricingReferenceDataCallBackMethod_SecurityReference);
            }
        }

        /// <summary>
        /// Zoom In Command Method
        /// </summary>
        /// <param name="parameter"></param>
        public void ZoomInCommandMethod(object parameter)
        {
            ZoomIn(this.ChartAreaPricing);
            ZoomIn(this.ChartAreaVolume);
            ((Telerik.Windows.Controls.DelegateCommand)_zoomInCommand).InvalidateCanExecute();
            ((Telerik.Windows.Controls.DelegateCommand)_zoomOutCommand).InvalidateCanExecute();
        }

        /// <summary>
        /// Zoom In Command Method Validation
        /// </summary>
        /// <param name="parameter"></param>
        public bool ZoomInCommandValidation(object parameter)
        {
            if (this.ChartAreaPricing == null || this.ChartAreaVolume == null)
                return false;

            return
                this.ChartAreaPricing.ZoomScrollSettingsX.Range > this.ChartAreaPricing.ZoomScrollSettingsX.MinZoomRange &&
                this.ChartAreaVolume.ZoomScrollSettingsX.Range > this.ChartAreaVolume.ZoomScrollSettingsX.MinZoomRange;
        }

        /// <summary>
        /// Zoom Out Command Method
        /// </summary>
        /// <param name="parameter"></param>
        public void ZoomOutCommandMethod(object parameter)
        {
            ZoomOut(this.ChartAreaPricing);
            ZoomOut(this.ChartAreaVolume);
            ((Telerik.Windows.Controls.DelegateCommand)_zoomInCommand).InvalidateCanExecute();
            ((Telerik.Windows.Controls.DelegateCommand)_zoomOutCommand).InvalidateCanExecute();
        }

        /// <summary>
        /// Zoom Out Command Method Validation
        ///  </summary>
        /// <param name="parameter"></param>
        public bool ZoomOutCommandValidation(object parameter)
        {
            try
            {
                if (this.ChartAreaPricing == null || this.ChartAreaVolume == null)
                {
                    return false;
                }
                return this.ChartAreaPricing.ZoomScrollSettingsX.Range < 1d &&
                    this.ChartAreaVolume.ZoomScrollSettingsX.Range < 1d;
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
                return false;
            }
        }

        #endregion

        #region Callback Methods
        /// <summary>
        /// Callback method for Entity Reference Service call - Updates AutoCompleteBox
        /// </summary>
        /// <param name="result">EntityReferenceData Collection</param>
        public void RetrieveEntitySelectionDataCallBackMethod(List<EntitySelectionData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    SeriesReference = new CollectionViewSource();
                    SeriesReferenceSource = new ObservableCollection<EntitySelectionData>(result);
                    SeriesReference.GroupDescriptions.Add(new PropertyGroupDescription("Type"));
                    SeriesReference.SortDescriptions.Add(new System.ComponentModel.SortDescription
                    {
                        PropertyName = "SortOrder",
                        Direction = System.ComponentModel.ListSortDirection.Ascending
                    });
                    SeriesReference.SortDescriptions.Add(new System.ComponentModel.SortDescription
                    {
                        PropertyName = "LongName",
                        Direction = System.ComponentModel.ListSortDirection.Ascending
                    });
                    SeriesReference.Source = SeriesReferenceSource;
                }
                else
                {
                    Logging.LogMethodParameterNull(logger, methodNamespace, 1);
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
        }

        /// <summary>
        /// Callback method for Pricing Reference Service call related to updated Time Range - Updates Chart and Grid
        /// </summary>
        /// <param name="result">PricingReferenceData collection</param>
        private void RetrievePricingReferenceDataCallBackMethod_TimeRange(List<PricingReferenceData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);

            try
            {
                if (result != null)
                {
                    if (ChartEntityList.Count == 1)
                    {
                        PrimaryPlottedSeries.Clear();
                        PrimaryPlottedSeries.AddRange(result.OrderBy(a => a.SortingID).ToList());
                    }
                    else
                    {
                        string primarySecurityReferenceIdentifier = PrimaryPlottedSeries.First().InstrumentID;
                        PrimaryPlottedSeries.Clear();
                        PrimaryPlottedSeries.AddRange(result.Where(item => item.InstrumentID == primarySecurityReferenceIdentifier).ToList());
                    }

                    PlottedSeries.Clear();
                    PlottedSeries.AddRange(result);

                }
                else
                {
                    Logging.LogMethodParameterNull(logger, methodNamespace, 1);
                }

            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            finally
            {
                BusyIndicatorStatus = false;
            }
        }

        /// <summary>
        /// Callback method for Pricing Reference Service call related to updated Security Reference - Updates Chart and Grid
        /// </summary>
        /// <param name="result">PricingReferenceData collection</param>
        private void RetrievePricingReferenceDataCallBackMethod_SecurityReference(List<PricingReferenceData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);

            try
            {
                if (result != null)
                {
                    PlottedSeries.Clear();
                    PrimaryPlottedSeries.Clear();
                    PlottedSeries.AddRange(result);
                    PrimaryPlottedSeries.AddRange(result.OrderBy(a => a.SortingID).ToList());
                }
                else
                {
                    Logging.LogMethodParameterNull(logger, methodNamespace, 1);
                }

            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            finally
            {
                BusyIndicatorStatus = false;
            }
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Event Handler to subscribed event 'SecurityReferenceSet'
        /// </summary>
        /// <param name="securityReferenceData">SecurityReferenceData</param>
        public void HandleSecurityReferenceSet(EntitySelectionData entitySelectionData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                //ArgumentNullException
                if (entitySelectionData != null)
                {
                    //Check if security reference data is already present
                    if (PrimaryPlottedSeries.Where(p => p.InstrumentID == entitySelectionData.InstrumentID).Count().Equals(0))
                    {
                        //Check if no data exists
                        if (!PrimaryPlottedSeries.Count.Equals(0))
                        {
                            //Remove previous primary security reference data
                            PlottedSeries.Clear();
                            PrimaryPlottedSeries.Clear();
                        }
                        this.entitySelectionData = entitySelectionData;
                        ChartEntityList.Clear();
                        ChartEntityList.Add(entitySelectionData);

                        //Retrieve Pricing Data for Primary Security Reference
                        if (IsActive)
                        {
                            BusyIndicatorStatus = true;
                            RetrievePricingData(ChartEntityList, RetrievePricingReferenceDataCallBackMethod_SecurityReference);
                            SelectedBaseSecurity = entitySelectionData.ShortName.ToString();
                        }
                    }
                    else
                    {
                        Logging.LogMethodParameterNull(logger, methodNamespace, 1);
                    }
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }

        }

        /// <summary>
        /// Checking the status of Chart, whether zoom can be executed or not
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
            try
            {
                BusyIndicatorStatus = true;
                dbInteractivity.RetrievePricingReferenceData(entityIdentifiers, SelectedStartDate, SelectedEndDate, ReturnTypeSelection, SelectedFrequencyInterval, callback);
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
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

        /// <summary>
        /// Zoom In Algo
        /// </summary>
        /// <param name="chartArea"></param>
        private void ZoomIn(ChartArea chartArea)
        {
            chartArea.ZoomScrollSettingsX.SuspendNotifications();
            double zoomCenter = chartArea.ZoomScrollSettingsX.RangeStart + (chartArea.ZoomScrollSettingsX.Range / 2);
            double newRange = Math.Max(chartArea.ZoomScrollSettingsX.MinZoomRange, chartArea.ZoomScrollSettingsX.Range) / 2;
            chartArea.ZoomScrollSettingsX.RangeStart = Math.Max(0, zoomCenter - (newRange / 2));
            chartArea.ZoomScrollSettingsX.RangeEnd = Math.Min(1, zoomCenter + (newRange / 2));
            chartArea.ZoomScrollSettingsX.ResumeNotifications();
        }

        /// <summary>
        /// Zoom out Algo
        /// </summary>
        /// <param name="chartArea"></param>
        private void ZoomOut(ChartArea chartArea)
        {
            chartArea.ZoomScrollSettingsX.SuspendNotifications();
            double zoomCenter = chartArea.ZoomScrollSettingsX.RangeStart + (chartArea.ZoomScrollSettingsX.Range / 2);
            double newRange = Math.Min(1, chartArea.ZoomScrollSettingsX.Range) * 2;

            if (zoomCenter + (newRange / 2) > 1)
            {
                zoomCenter = 1 - (newRange / 2);
            }
            else if (zoomCenter - (newRange / 2) < 0)
            {
                zoomCenter = newRange / 2;
            }
            chartArea.ZoomScrollSettingsX.RangeStart = Math.Max(0, zoomCenter - newRange / 2);
            chartArea.ZoomScrollSettingsX.RangeEnd = Math.Min(1, zoomCenter + newRange / 2);
            chartArea.ZoomScrollSettingsX.ResumeNotifications();
        }

        #endregion

        #region EventUnSubscribe

        /// <summary>
        /// Events Unsubscribe
        /// </summary>
        public void Dispose()
        {
            eventAggregator.GetEvent<SecurityReferenceSetEvent>().Unsubscribe(HandleSecurityReferenceSet);
        }

        #endregion

    }
}