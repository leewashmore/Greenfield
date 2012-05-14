using System;
using System.Windows;
using System.Windows.Input;
using GreenField.ServiceCaller;
using System.Linq;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Prism.Events;
using GreenField.ServiceCaller.SecurityReferenceDefinitions;
using System.Collections.Generic;
using System.Windows.Data;
using System.Collections.ObjectModel;
using Microsoft.Practices.Prism.Commands;
using GreenField.Common;
using GreenField.Gadgets.Helpers;
using Telerik.Windows.Controls.Charting;
using GreenField.ServiceCaller.BenchmarkHoldingsDefinitions;


namespace GreenField.Gadgets.ViewModels
{
    public class ViewModelMultiLineBenchmark : NotificationObject
    {
        #region Private Fields

        /// <summary>
        /// MEF Singletons
        /// </summary>
        private IDBInteractivity _dbInteractivity;
        private ILoggerFacade _logger;
        private IEventAggregator _eventAggregator;
        private EntitySelectionData _entitySelectionData;
        private PortfolioSelectionData _portfolioSelectionData;
        private string _periodSelectionData;

        #endregion

        #region Constructor

        public ViewModelMultiLineBenchmark(DashboardGadgetParam param)
        {
            if (param != null)
            {
                _dbInteractivity = param.DBInteractivity;
                _logger = param.LoggerFacade;
                _eventAggregator = param.EventAggregator;

                _entitySelectionData = param.DashboardGadgetPayload.EntitySelectionData;
                _selectedPortfolio = param.DashboardGadgetPayload.PortfolioSelectionData;
                _periodSelectionData = param.DashboardGadgetPayload.PeriodSelectionData;

                if ((_entitySelectionData != null) && (_periodSelectionData != null) && (_entitySelectionData.LongName != null))
                {
                    Dictionary<string, string> objSelectedEntity = new Dictionary<string, string>();
                    objSelectedEntity.Add("SECURITY", _entitySelectionData.LongName);
                    if (_selectedPortfolio != null && _selectedPortfolio.PortfolioId != null)
                        objSelectedEntity.Add("PORTFOLIO", _selectedPortfolio.PortfolioId);
                    StartDate = GetStartDate(_periodSelectionData);
                    if (StartDate != null)
                        _dbInteractivity.RetrieveBenchmarkChartReturnData(objSelectedEntity, StartDate, RetrieveBenchmarkChartDataCallBackMethod);
                }
            }
        }

        #endregion

        #region EventSubscribe

        public void SubscribeEvents(IEventAggregator _eventAggregator)
        {
            _eventAggregator.GetEvent<PeriodReferenceSetEvent>().Subscribe(HandleEffectiveDateSet);
            _eventAggregator.GetEvent<PortfolioReferenceSetEvent>().Subscribe(HandlePortfolioReferenceSet);
            _eventAggregator.GetEvent<SecurityReferenceSetEvent>().Subscribe(HandleSecurityReferenceSet);
        }

        #endregion

        #region PropertyDeclaration

        private ObservableCollection<BenchmarkSelectionData> _chartEntityList;
        public ObservableCollection<BenchmarkSelectionData> ChartEntityList
        {
            get
            {
                if (_chartEntityList == null)
                    _chartEntityList = new ObservableCollection<BenchmarkSelectionData>();
                if (_chartEntityList.Count >= 1)
                    AddToChartVisibility = "Visible";
                else
                    AddToChartVisibility = "Collapsed";
                return _chartEntityList;
            }
            set
            {
                _chartEntityList = value;
                this.RaisePropertyChanged(() => this.ChartEntityList);
            }
        }

        /// <summary>
        /// Selected Security
        /// </summary>
        private EntitySelectionData _selectedSecurity;
        public EntitySelectionData SelectedSecurity
        {
            get
            {
                if (_selectedSecurity == null)
                    _selectedSecurity = new EntitySelectionData();
                return _entitySelectionData;
            }
            set
            {
                _entitySelectionData = value;
                this.RaisePropertyChanged(() => this.SelectedSecurity);
            }
        }

        /// <summary>
        /// Selected Portfolio
        /// </summary>
        private PortfolioSelectionData _selectedPortfolio;
        public PortfolioSelectionData PortfolioSelectionData
        {
            get
            {
                if (_selectedPortfolio == null)
                    _selectedPortfolio = new PortfolioSelectionData();
                return _selectedPortfolio;
            }
            set
            {
                _selectedPortfolio = value;
                this.RaisePropertyChanged(() => this._selectedPortfolio);
            }
        }

        private Dictionary<string, string> _selectedEntities;
        public Dictionary<string, string> SelectedEntities
        {
            get
            {
                if (_selectedEntities == null)
                    _selectedEntities = new Dictionary<string, string>();
                return _selectedEntities;
            }
            set
            {
                _selectedEntities = value;
                this.RaisePropertyChanged(() => this.SelectedEntities);
            }
        }


        #region ChartProperties

        /// <summary>
        /// Collection of Benchmark Data-Chart
        /// </summary>
        private RangeObservableCollection<BenchmarkChartReturnData> _plottedBenchmarkSeries = new RangeObservableCollection<BenchmarkChartReturnData>();
        public RangeObservableCollection<BenchmarkChartReturnData> PlottedBenchmarkSeries
        {
            get
            {
                return _plottedBenchmarkSeries;
            }
            set
            {
                _plottedBenchmarkSeries = value;
                this.RaisePropertyChanged(() => this.PlottedBenchmarkSeries);
            }
        }

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

        #endregion

        #region GridProperties

        /// <summary>
        /// Collection of Benchmark Data-Grid
        /// </summary>
        private RangeObservableCollection<BenchmarkGridReturnData> _gridBenchmarkData;
        public RangeObservableCollection<BenchmarkGridReturnData> GridBenchmarkData
        {
            get
            {
                if (_gridBenchmarkData == null)
                    _gridBenchmarkData = new RangeObservableCollection<BenchmarkGridReturnData>();
                return _gridBenchmarkData;
            }
            set
            {
                _gridBenchmarkData = value;
                this.RaisePropertyChanged(() => this.GridBenchmarkData);
            }
        }

        #region GridEntities

        private string _previousYearData = DateTime.Now.AddYears(-1).Year.ToString();
        public string PreviousYearDataColumnHeader
        {
            get
            {
                return _previousYearData;
            }
            set
            {
                _previousYearData = value;
                this.RaisePropertyChanged(() => this.PreviousYearDataColumnHeader);
            }
        }

        private string _twoPreviousYearData = DateTime.Now.AddYears(-2).Year.ToString();
        public string TwoPreviousYearDataColumnHeader
        {
            get
            {
                return _twoPreviousYearData;
            }
            set
            {
                _twoPreviousYearData = value;
                this.RaisePropertyChanged(() => this.TwoPreviousYearDataColumnHeader);
            }
        }

        private string _threePreviousYearData = DateTime.Now.AddYears(-3).Year.ToString();
        public string ThreePreviousYearDataColumnHeader
        {
            get
            {
                return _threePreviousYearData;
            }
            set
            {
                _threePreviousYearData = value;
                this.RaisePropertyChanged(() => this.ThreePreviousYearDataColumnHeader);
            }
        }

        #endregion

        #endregion

        #region Time Range Properties

        /// <summary>
        /// Chart Start Date
        /// </summary>
        private DateTime _startDate;
        public DateTime StartDate
        {
            get
            {
                return _startDate;
            }
            set
            {
                _startDate = value;
                this.RaisePropertyChanged(() => this.StartDate);
            }
        }

        #endregion

        #region Plotting Additional Series

        /// <summary>
        /// Grouped Collection View for Auto-Complete Box
        /// </summary>
        private CollectionViewSource _benchmarkReference;
        public CollectionViewSource BenchmarkReference
        {
            get
            {
                return _benchmarkReference;
            }
            set
            {
                _benchmarkReference = value;
                RaisePropertyChanged(() => this.BenchmarkReference);
            }
        }

        /// <summary>
        /// DataSource for the Grouped Collection View
        /// </summary>
        public ObservableCollection<BenchmarkSelectionData> BenchmarkReferenceSource { get; set; }

        /// <summary>
        /// Selected Entity
        /// </summary>
        private BenchmarkSelectionData _selectedBenchmarkReference = new BenchmarkSelectionData();
        public BenchmarkSelectionData SelectedBenchmarkReference
        {
            get
            {
                return _selectedBenchmarkReference;
            }
            set
            {
                _selectedBenchmarkReference = value;
                this.RaisePropertyChanged(() => this.SelectedBenchmarkReference);
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
        private string _benchmarkEnteredText;
        public string BenchmarkEnteredText
        {
            get { return _benchmarkEnteredText; }
            set
            {
                _benchmarkEnteredText = value;
                RaisePropertyChanged(() => this.BenchmarkEnteredText);
                if (value != null)
                    BenchmarkReference.Source = SearchFilterEnabled == false
                        ? BenchmarkReferenceSource.Where(o => o.Name.ToLower().Contains(value.ToLower()))
                        : BenchmarkReferenceSource.Where(o => o.Name.ToLower().StartsWith(value.ToLower()));
                else
                    BenchmarkReference.Source = BenchmarkReferenceSource;
                List<BenchmarkSelectionData> a = BenchmarkReferenceSource.ToList();
            }
        }

        /// <summary>
        /// Show/Hide Add to Chart Control
        /// </summary>
        private string _addToChartVisibility = "Collapsed";
        public string AddToChartVisibility
        {
            get
            {
                return _addToChartVisibility;
            }
            set
            {
                _addToChartVisibility = value;
                this.RaisePropertyChanged(() => this.AddToChartVisibility);
            }
        }

        /// <summary>
        /// Series to show List of Indices/BEnchmarks Added to chart
        /// </summary>
        private ObservableCollection<BenchmarkSelectionData> _comparisonSeries = new ObservableCollection<BenchmarkSelectionData>();
        public ObservableCollection<BenchmarkSelectionData> ComparisonSeries
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

        #endregion

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

        #region EventHandlers

        public void HandleSecurityReferenceSet(EntitySelectionData entitySelectionData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                //ArgumentNullException
                if (entitySelectionData != null && entitySelectionData.LongName != null)
                {
                    if (SelectedEntities.ContainsKey("SECURITY"))
                        SelectedEntities.Remove("SECURITY");

                    SelectedEntities.Add("SECURITY", entitySelectionData.LongName);
                    if (StartDate != null && SelectedEntities != null)
                    {
                        _dbInteractivity.RetrieveBenchmarkChartReturnData(SelectedEntities, StartDate, RetrieveBenchmarkChartDataCallBackMethod);
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
        }

        public void HandlePortfolioReferenceSet(PortfolioSelectionData portfolioSelectionData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                //ArgumentNullException
                if (portfolioSelectionData != null && portfolioSelectionData.PortfolioId != null)
                {
                    if (SelectedEntities.ContainsKey("PORTFOLIO"))
                        SelectedEntities.Remove("PORTFOLIO");

                    SelectedEntities.Add("PORTFOLIO", portfolioSelectionData.PortfolioId);
                    if (StartDate != null && SelectedEntities != null && SelectedEntities.ContainsKey("SECURITY"))
                    {
                        _dbInteractivity.RetrieveBenchmarkChartReturnData(SelectedEntities, StartDate, RetrieveBenchmarkChartDataCallBackMethod);
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
        }

        public void HandleEffectiveDateSet(string periodSelectionData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                //ArgumentNullException
                if (periodSelectionData != null)
                {
                    StartDate = GetStartDate(periodSelectionData);
                    if (SelectedEntities.ContainsKey("SECURITY") && StartDate != null)
                    {
                        _dbInteractivity.RetrieveBenchmarkChartReturnData(SelectedEntities, StartDate, RetrieveBenchmarkChartDataCallBackMethod);
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
        }

        #endregion

        #region ICommandMethods

        /// <summary>
        /// Add to Chart Command Method
        /// </summary>
        /// <param name="param"></param>
        private void AddCommandMethod(object param)
        {
            if (SelectedBenchmarkReference != null)
            {
                if (!PlottedBenchmarkSeries.Any(t => t.InstrumentID == SelectedBenchmarkReference.InstrumentID))
                {
                    ChartEntityList.Add(SelectedBenchmarkReference);
                    _dbInteractivity.RetrieveBenchmarkChartReturnData(ChartEntityList.ToList(), Convert.ToDateTime(_effectiveDateSet), RetrieveBenchmarkChartDataCallBackMethod_BenchmarkReference);
                }
            }
        }

        /// <summary>
        /// Delete Series from Chart
        /// </summary>
        /// <param name="param"></param>
        private void DeleteCommandMethod(object param)
        {
            BenchmarkSelectionData a = param as BenchmarkSelectionData;
            List<BenchmarkChartReturnData> removeItem = new List<BenchmarkChartReturnData>();
            removeItem = PlottedBenchmarkSeries.Where(w => w.InstrumentID == a.InstrumentID).ToList();
            if (removeItem != null)
                PlottedBenchmarkSeries.RemoveRange(removeItem);
            ComparisonSeries.Remove(a);
            ChartEntityList.Remove(a);
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
            if (this.ChartAreaPricing == null || this.ChartAreaVolume == null)
                return false;

            return this.ChartAreaPricing.ZoomScrollSettingsX.Range < 1d &&
                this.ChartAreaVolume.ZoomScrollSettingsX.Range < 1d;
        }

        #endregion

        #region Callback Methods

        /// <summary>
        /// Callback method for Benchmark Reference Service call - Updates AutoCompleteBox
        /// </summary>
        /// <param name="result">BenchmarkSelectionData Collection</param>
        private void RetrieveBenchmarkSelectionDataCallBackMethod(List<BenchmarkSelectionData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    BenchmarkReference = new CollectionViewSource();
                    BenchmarkReferenceSource = new ObservableCollection<BenchmarkSelectionData>(result);
                    BenchmarkReference.GroupDescriptions.Add(new PropertyGroupDescription("Type"));
                    BenchmarkReference.SortDescriptions.Add(new System.ComponentModel.SortDescription
                    {
                        PropertyName = "Type",
                        Direction = System.ComponentModel.ListSortDirection.Ascending
                    });
                    BenchmarkReference.Source = BenchmarkReferenceSource;
                    List<BenchmarkSelectionData> a = BenchmarkReferenceSource.ToList();
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
        }

        /// <summary>
        /// Callback method for Benchmark Reference Service call related to updated Time Range - Updates Chart
        /// </summary>
        /// <param name="result">PricingReferenceData collection</param>
        private void RetrieveBenchmarkChartDataCallBackMethod(List<BenchmarkChartReturnData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);

            try
            {
                if (result != null)
                {
                    PlottedBenchmarkSeries.Clear();
                    PlottedBenchmarkSeries.AddRange(result);
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
        }

        /// <summary>
        /// Callback method for Pricing Reference Service call related to updated Security Reference - Updates Chart
        /// </summary>
        /// <param name="result">PricingReferenceData collection</param>
        private void RetrieveBenchmarkChartDataCallBackMethod_BenchmarkReference(List<BenchmarkChartReturnData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);

            try
            {
                if (result != null)
                {
                    PlottedBenchmarkSeries.Clear();
                    PlottedBenchmarkSeries.AddRange(result);
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
        }

        /// <summary>
        /// Callback method for Benchmark Grid-Updates Grid
        /// </summary>
        /// <param name="result"></param>
        private void RetrieveBenchmarkGridDataCallBackMethod(List<BenchmarkGridReturnData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);

            try
            {
                if (result != null)
                {
                    GridBenchmarkData.Clear();
                    GridBenchmarkData.AddRange(result);
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
        }

        #endregion

        #region HelperMethods

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
                zoomCenter = 1 - (newRange / 2);
            else if (zoomCenter - (newRange / 2) < 0)
                zoomCenter = newRange / 2;

            chartArea.ZoomScrollSettingsX.RangeStart = Math.Max(0, zoomCenter - newRange / 2);
            chartArea.ZoomScrollSettingsX.RangeEnd = Math.Min(1, zoomCenter + newRange / 2);

            chartArea.ZoomScrollSettingsX.ResumeNotifications();
        }

        /// <summary>
        /// Helper method to calculate start date for the chart
        /// </summary>
        /// <param name="period">Period selected from toolbar</param>
        /// <returns>startDate of type DateTime</returns>
        public DateTime GetStartDate(string period)
        {
            DateTime startDate = DateTime.Today;

            switch (period)
            {
                case "1M":
                    {
                        startDate = startDate.AddMonths(-1);
                        break;
                    }
                case "3M":
                    {
                        startDate = startDate.AddMonths(-3);
                        break;
                    }
                case "6M":
                    {
                        startDate = startDate.AddMonths(-6);
                        break;
                    }
                case "YTD":
                    {
                        startDate = new DateTime(startDate.Year, 1, 1);
                        break;
                    }
                case "1Y":
                    {
                        startDate = startDate.AddYears(-1);
                        break;
                    }
                case "3Y":
                    {
                        startDate = startDate.AddYears(-3);
                        break;
                    }
                case "5Y":
                    {
                        startDate = startDate.AddYears(-5);
                        break;
                    }
                case "SI":
                    {
                        startDate = new DateTime(2000, 1, 1);
                        break;
                    }
                default:
                    {
                        startDate = startDate.AddMonths(-1);
                        break;
                    }
            }
            return startDate;
        }

        #endregion
    }
}