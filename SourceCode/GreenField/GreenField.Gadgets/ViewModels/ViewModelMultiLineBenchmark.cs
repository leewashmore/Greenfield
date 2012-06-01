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
using GreenField.Common;
using GreenField.Gadgets.Helpers;
using Telerik.Windows.Controls.Charting;
using GreenField.ServiceCaller.BenchmarkHoldingsDefinitions;
using Telerik.Windows.Controls.Charting;
using Telerik.Windows.Controls;
using GreenField.DataContracts;


namespace GreenField.Gadgets.ViewModels
{
    /// <summary>
    /// ViewModel for Multi-Line Benchmark UI
    /// </summary>
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

        /// <summary>
        /// Constructor that initialises the Class
        /// </summary>
        /// <param name="param"></param>
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

                if (_eventAggregator != null)
                    SubscribeEvents(_eventAggregator);

                if ((_entitySelectionData != null) && (_periodSelectionData != null) && (_entitySelectionData.LongName != null))
                {
                    Dictionary<string, string> objSelectedEntity = new Dictionary<string, string>();
                    objSelectedEntity.Add("SECURITY", _entitySelectionData.LongName);
                    if (_selectedPortfolio != null && _selectedPortfolio.PortfolioId != null)
                        objSelectedEntity.Add("PORTFOLIO", _selectedPortfolio.PortfolioId);

                    if (objSelectedEntity != null)
                        _dbInteractivity.RetrieveBenchmarkChartReturnData(objSelectedEntity, RetrieveBenchmarkChartDataCallBackMethod);
                }
            }
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

        /// <summary>
        /// Selected Entities
        /// </summary>
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

        /// <summary>
        /// Selected Period from the tool-bar
        /// </summary>
        private string _selectedPeriod;
        public string SelectedPeriod
        {
            get
            {
                return _selectedPeriod;
            }
            set
            {
                _selectedPeriod = value;
                this.RaisePropertyChanged(() => this.SelectedPeriod);
            }
        }

        #region ChartProperties

        /// <summary>
        /// Collection of Benchmark Data-Chart
        /// </summary>
        private RangeObservableCollection<BenchmarkChartReturnData> _multiLineBenchmarkUIChartData = new RangeObservableCollection<BenchmarkChartReturnData>();
        public RangeObservableCollection<BenchmarkChartReturnData> MultiLineBenchmarkUIChartData
        {
            get
            {
                return _multiLineBenchmarkUIChartData;
            }
            set
            {
                _multiLineBenchmarkUIChartData = value;
                this.RaisePropertyChanged(() => this.MultiLineBenchmarkUIChartData);
            }
        }

        private ChartArea _chartAreaMultiLineBenchmark;
        public ChartArea ChartAreaMultiLineBenchmark
        {
            get
            {
                return this._chartAreaMultiLineBenchmark;
            }
            set
            {
                this._chartAreaMultiLineBenchmark = value;
            }
        }

        /// <summary>
        /// Data Context Source for chart
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ChartDataBound(object sender, ChartDataBoundEventArgs e)
        {
            ((DelegateCommand)_zoomInCommand).InvalidateCanExecute();
            ((DelegateCommand)_zoomOutCommand).InvalidateCanExecute();
        }

        #endregion

        #region GridProperties

        /// <summary>
        /// Collection of Benchmark Data-Grid
        /// </summary>
        private RangeObservableCollection<BenchmarkGridReturnData> _multiLineBenchmarkUIGridData;
        public RangeObservableCollection<BenchmarkGridReturnData> MultiLineBenchmarUIGridData
        {
            get
            {
                if (_multiLineBenchmarkUIGridData == null)
                    _multiLineBenchmarkUIGridData = new RangeObservableCollection<BenchmarkGridReturnData>();
                return _multiLineBenchmarkUIGridData;
            }
            set
            {
                _multiLineBenchmarkUIGridData = value;
                this.RaisePropertyChanged(() => this.MultiLineBenchmarUIGridData);
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

        #endregion

        #region Events

        public event DataRetrievalProgressIndicatorEventHandler MultiLineBenchmarkDataLoadedEvent;

        #endregion

        #region EventSubscribe

        /// <summary>
        /// Subscribing to Events
        /// </summary>
        /// <param name="_eventAggregator"></param>
        public void SubscribeEvents(IEventAggregator _eventAggregator)
        {
            _eventAggregator.GetEvent<PeriodReferenceSetEvent>().Subscribe(HandlePeriodReferenceSet);
            _eventAggregator.GetEvent<PortfolioReferenceSetEvent>().Subscribe(HandlePortfolioReferenceSet);
            _eventAggregator.GetEvent<SecurityReferenceSetEvent>().Subscribe(HandleSecurityReferenceSet);
        }

        #endregion

        #region ICommand

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

        /// <summary>
        /// Handle Security change Event
        /// </summary>
        /// <param name="entitySelectionData">Details of Selected Security</param>
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
                    if (SelectedEntities != null && SelectedEntities.ContainsKey("SECURITY") && SelectedEntities.ContainsKey("PORTFOLIO"))
                    {
                        _dbInteractivity.RetrieveBenchmarkChartReturnData(SelectedEntities, RetrieveBenchmarkChartDataCallBackMethod);
                        _dbInteractivity.RetrieveBenchmarkGridReturnData(SelectedEntities, RetrieveBenchmarkGridDataCallBackMethod);
                        if (null != MultiLineBenchmarkDataLoadedEvent)
                            MultiLineBenchmarkDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
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

        /// <summary>
        /// Handle Portfolio Change Event
        /// </summary>
        /// <param name="portfolioSelectionData">Detail of Selected Portfolio</param>
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
                    if (SelectedEntities != null && SelectedEntities.ContainsKey("SECURITY") && SelectedEntities.ContainsKey("PORTFOLIO"))
                    {
                        _dbInteractivity.RetrieveBenchmarkChartReturnData(SelectedEntities, RetrieveBenchmarkChartDataCallBackMethod);
                        _dbInteractivity.RetrieveBenchmarkGridReturnData(SelectedEntities, RetrieveBenchmarkGridDataCallBackMethod);
                        if (null != MultiLineBenchmarkDataLoadedEvent)
                            MultiLineBenchmarkDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
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

        /// <summary>
        /// Event handler to handle Period change Event
        /// </summary>
        /// <param name="periodSelectionData">Selected Period</param>
        public void HandlePeriodReferenceSet(string periodSelectionData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                //ArgumentNullException
                if (periodSelectionData != null)
                {
                    if (null != MultiLineBenchmarkDataLoadedEvent)
                        MultiLineBenchmarkDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
                    _periodSelectionData = periodSelectionData;
                    MultiLineBenchmarkUIChartData = CalculateDataAccordingToPeriod(MultiLineBenchmarkUIChartData, periodSelectionData);

                    if (null != MultiLineBenchmarkDataLoadedEvent)
                        MultiLineBenchmarkDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
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
        /// Zoom In Command Method
        /// </summary>
        /// <param name="parameter"></param>
        public void ZoomInCommandMethod(object parameter)
        {
            ZoomIn(this.ChartAreaMultiLineBenchmark);
            ((Telerik.Windows.Controls.DelegateCommand)_zoomInCommand).InvalidateCanExecute();
            ((Telerik.Windows.Controls.DelegateCommand)_zoomOutCommand).InvalidateCanExecute();
        }

        /// <summary>
        /// Zoom In Command Method Validation
        /// </summary>
        /// <param name="parameter"></param>
        public bool ZoomInCommandValidation(object parameter)
        {
            if (this.ChartAreaMultiLineBenchmark == null)
                return false;

            return
                this.ChartAreaMultiLineBenchmark.ZoomScrollSettingsX.Range > this.ChartAreaMultiLineBenchmark.ZoomScrollSettingsX.MinZoomRange;
        }

        /// <summary>
        /// Zoom Out Command Method
        /// </summary>
        /// <param name="parameter"></param>
        public void ZoomOutCommandMethod(object parameter)
        {
            ZoomOut(this.ChartAreaMultiLineBenchmark);
            ((Telerik.Windows.Controls.DelegateCommand)_zoomInCommand).InvalidateCanExecute();
            ((Telerik.Windows.Controls.DelegateCommand)_zoomOutCommand).InvalidateCanExecute();
        }

        /// <summary>
        /// Zoom Out Command Method Validation
        ///  </summary>
        /// <param name="parameter"></param>
        public bool ZoomOutCommandValidation(object parameter)
        {
            if (this.ChartAreaMultiLineBenchmark == null)
                return false;

            return this.ChartAreaMultiLineBenchmark.ZoomScrollSettingsX.Range < 1d;
        }

        #endregion

        #region Callback Methods

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
                    MultiLineBenchmarkUIChartData.Clear();
                    MultiLineBenchmarkUIChartData.AddRange(result);
                    MultiLineBenchmarkUIChartData = CalculateDataAccordingToPeriod(MultiLineBenchmarkUIChartData, _periodSelectionData);
                }
                else
                {
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                }
                if (null != MultiLineBenchmarkDataLoadedEvent)
                    MultiLineBenchmarkDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
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
                    MultiLineBenchmarUIGridData.Clear();
                    MultiLineBenchmarUIGridData.AddRange(result);
                }
                else
                {
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                }
                if (null != MultiLineBenchmarkDataLoadedEvent)
                    MultiLineBenchmarkDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
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
        /// Validation Method for Zoom Out button
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns>boolean value</returns>
        public bool CanZoomOut(object parameter)
        {
            if (this.ChartAreaMultiLineBenchmark == null)
                return false;

            return this.ChartAreaMultiLineBenchmark.ZoomScrollSettingsX.Range < 1d;
        }

        /// <summary>
        /// Calculate Data according to the selected period
        /// </summary>
        /// <param name="plottedSeries">Currently plotted RangeObservableCollection of type BenchmarkChartReturnData</param>
        /// <param name="periodType">Selected Period</param>
        /// <returns>RangeObservableCollection of type BenchmarkChartReturnData</returns>
        public RangeObservableCollection<BenchmarkChartReturnData> CalculateDataAccordingToPeriod(RangeObservableCollection<BenchmarkChartReturnData> plottedSeries, string periodType)
        {
            switch (periodType)
            {
                case "1D":
                    {
                        foreach (BenchmarkChartReturnData item in plottedSeries)
                        {
                            item.IndexedValue = item.OneD;
                        }
                        break;
                    }
                case "WTD":
                    {
                        foreach (BenchmarkChartReturnData item in plottedSeries)
                        {
                            item.IndexedValue = item.WTD;
                        }
                        break;
                    }
                case "MTD":
                    {
                        foreach (BenchmarkChartReturnData item in plottedSeries)
                        {
                            item.IndexedValue = item.MTD;
                        }
                        break;
                    }
                case "YTD":
                    {
                        foreach (BenchmarkChartReturnData item in plottedSeries)
                        {
                            item.IndexedValue = item.YTD;
                        }
                        break;
                    }
                case "QTD":
                    {
                        foreach (BenchmarkChartReturnData item in plottedSeries)
                        {
                            item.IndexedValue = item.QTD;
                        }
                        break;
                    }
                case "OneY":
                    {
                        foreach (BenchmarkChartReturnData item in plottedSeries)
                        {
                            item.IndexedValue = item.OneY;
                        }
                        break;
                    }
                default:
                    {
                        foreach (BenchmarkChartReturnData item in plottedSeries)
                        {
                            item.IndexedValue = item.OneD;
                        }
                        break;
                    }
            }
            return plottedSeries;
        }

        #endregion
    }
}

