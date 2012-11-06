using System;
using System.Windows;
using System.Windows.Input;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Prism.Events;
using Telerik.Windows.Controls.Charting;
using Telerik.Windows.Controls;
using GreenField.DataContracts;
using GreenField.ServiceCaller;
using GreenField.Common;
using GreenField.Gadgets.Helpers;

namespace GreenField.Gadgets.ViewModels
{
    /// <summary>
    /// ViewModel for Multi-Line Benchmark UI
    /// </summary>
    public class ViewModelMultiLineBenchmark : NotificationObject
    {
        #region Private Fields
        
        /// <summary>
        /// Instance of IDbInteractivity
        /// </summary>
        private IDBInteractivity dbInteractivity;
        
        /// <summary>
        /// Instance of Logger facade
        /// </summary>
        private ILoggerFacade logger;
        
        /// <summary>
        /// Instance of Event Aggregator
        /// </summary>
        private IEventAggregator eventAggregator;
        
        /// <summary>
        /// Instance of FilterSelectionData
        /// </summary>
        private FilterSelectionData filterSelectionData;
        
        /// <summary>
        /// Period SelectionData
        /// </summary>
        private string periodSelectionData;
        
        

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
                dbInteractivity = param.DBInteractivity;
                logger = param.LoggerFacade;
                eventAggregator = param.EventAggregator;

                selectedPortfolio = param.DashboardGadgetPayload.PortfolioSelectionData;
                periodSelectionData = param.DashboardGadgetPayload.PeriodSelectionData;
                filterSelectionData = param.DashboardGadgetPayload.FilterSelectionData;

                if (eventAggregator != null)
                    SubscribeEvents(eventAggregator);

                if ((selectedPortfolio != null) && (periodSelectionData != null))
                {
                    Dictionary<string, string> objSelectedEntity = new Dictionary<string, string>();
                    if (selectedPortfolio.PortfolioId != null)
                        objSelectedEntity.Add("PORTFOLIO", selectedPortfolio.PortfolioId);

                    if (filterSelectionData != null && filterSelectionData.FilterValues != null)
                    {
                        if (SelectedEntities.ContainsKey("COUNTRY"))
                            SelectedEntities.Remove("COUNTRY");
                        if (SelectedEntities.ContainsKey("SECTOR"))
                            SelectedEntities.Remove("SECTOR");

                        if (filterSelectionData.Filtertype == "Country")
                        {
                            if (filterSelectionData.FilterValues != null)
                                SelectedEntities.Add("COUNTRY", filterSelectionData.FilterValues);
                        }
                        else if (filterSelectionData.Filtertype == "Sector")
                        {
                            if (filterSelectionData.FilterValues != null)
                                SelectedEntities.Add("SECTOR", filterSelectionData.FilterValues);
                        }
                    }

                    if (objSelectedEntity != null || objSelectedEntity.Count != 0 && IsActive)
                    {
                        dbInteractivity.RetrieveBenchmarkChartReturnData(objSelectedEntity, RetrieveBenchmarkChartDataCallBackMethod);
                        BusyIndicatorStatus = true;
                    }
                }
            }
        }

        #endregion

        #region PropertyDeclaration

        private ObservableCollection<BenchmarkSelectionData> chartEntityList;
        public ObservableCollection<BenchmarkSelectionData> ChartEntityList
        {
            get
            {
                if (chartEntityList == null)
                    chartEntityList = new ObservableCollection<BenchmarkSelectionData>();
                return chartEntityList;
            }
            set
            {
                chartEntityList = value;
                this.RaisePropertyChanged(() => this.ChartEntityList);
            }
        }

        /// <summary>
        /// Selected Portfolio
        /// </summary>
        private PortfolioSelectionData selectedPortfolio;
        public PortfolioSelectionData PortfolioSelectionData
        {
            get
            {
                if (selectedPortfolio == null)
                    selectedPortfolio = new PortfolioSelectionData();
                return selectedPortfolio;
            }
            set
            {
                selectedPortfolio = value;
                this.RaisePropertyChanged(() => this.selectedPortfolio);
            }
        }

        /// <summary>
        /// Selected Entities
        /// </summary>
        private Dictionary<string, string> selectedEntities;
        public Dictionary<string, string> SelectedEntities
        {
            get
            {
                if (selectedEntities == null)
                    selectedEntities = new Dictionary<string, string>();
                return selectedEntities;
            }
            set
            {
                selectedEntities = value;
                this.RaisePropertyChanged(() => this.SelectedEntities);
            }
        }

        /// <summary>
        /// Selected Period from the tool-bar
        /// </summary>
        private string selectedPeriod;
        public string SelectedPeriod
        {
            get
            {
                return selectedPeriod;
            }
            set
            {
                selectedPeriod = value;
                this.RaisePropertyChanged(() => this.SelectedPeriod);
            }
        }

        /// <summary>
        /// Status of Busy Indicator
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
                if (SelectedEntities != null && SelectedEntities.ContainsKey("PORTFOLIO") && isActive)
                {
                    dbInteractivity.RetrieveBenchmarkChartReturnData(SelectedEntities, RetrieveBenchmarkChartDataCallBackMethod);
                    dbInteractivity.RetrieveBenchmarkGridReturnData(SelectedEntities, RetrieveBenchmarkGridDataCallBackMethod);
                    BusyIndicatorStatus = true;
                }
            }
        }

        #region ChartProperties

        /// <summary>
        /// Collection of Benchmark Data-Chart
        /// </summary>
        private RangeObservableCollection<BenchmarkChartReturnData> multiLineBenchmarkUIChartData;
        public RangeObservableCollection<BenchmarkChartReturnData> MultiLineBenchmarkUIChartData
        {
            get
            {
                if (multiLineBenchmarkUIChartData == null)
                {
                    multiLineBenchmarkUIChartData = new RangeObservableCollection<BenchmarkChartReturnData>();
                }
                return multiLineBenchmarkUIChartData;
            }
            set
            {
                multiLineBenchmarkUIChartData = value;
                this.RaisePropertyChanged(() => this.MultiLineBenchmarkUIChartData);
            }
        }

        /// <summary>
        /// Chart Area bound to the chart
        /// </summary>
        private ChartArea chartAreaMultiLineBenchmark;
        public ChartArea ChartAreaMultiLineBenchmark
        {
            get
            {
                return this.chartAreaMultiLineBenchmark;
            }
            set
            {
                this.chartAreaMultiLineBenchmark = value;
            }
        }
        
        /// <summary>
        /// Minimum Value of X-Axis
        /// </summary>
        private double axisXMinValue;
        public double AxisXMinValue
        {
            get { return axisXMinValue; }
            set
            {
                axisXMinValue = value;
                this.RaisePropertyChanged(() => this.AxisXMinValue);
            }
        }

        /// <summary>
        /// Maximum value of X-Axis
        /// </summary>
        private double axisXMaxValue;
        public double AxisXMaxValue
        {
            get { return axisXMaxValue; }
            set
            {
                axisXMaxValue = value;
                this.RaisePropertyChanged(() => this.AxisXMaxValue);
            }
        }

        /// <summary>
        /// Step of the Chart
        /// </summary>
        private int axisXStep;
        public int AxisXStep
        {
            get { return axisXStep; }
            set
            {
                axisXStep = value;
            }
        }

        #endregion

        #region GridProperties

        /// <summary>
        /// Collection of Benchmark Data-Grid
        /// </summary>
        private RangeObservableCollection<BenchmarkGridReturnData> multiLineBenchmarkUIGridData;
        public RangeObservableCollection<BenchmarkGridReturnData> MultiLineBenchmarUIGridData
        {
            get
            {
                if (multiLineBenchmarkUIGridData == null)
                {
                    multiLineBenchmarkUIGridData = new RangeObservableCollection<BenchmarkGridReturnData>();
                }
                return multiLineBenchmarkUIGridData;
            }
            set
            {
                multiLineBenchmarkUIGridData = value;
                this.RaisePropertyChanged(() => this.MultiLineBenchmarUIGridData);
            }
        }

        #region GridEntities

        /// <summary>
        /// Previous YearHeader
        /// </summary>
        private string previousYearData = DateTime.Now.AddYears(-1).Year.ToString();
        public string PreviousYearDataColumnHeader
        {
            get
            {
                return previousYearData;
            }
            set
            {
                previousYearData = value;
                this.RaisePropertyChanged(() => this.PreviousYearDataColumnHeader);
            }
        }

        /// <summary>
        /// 2-Previous YearsHeader
        /// </summary>
        private string twoPreviousYearData = DateTime.Now.AddYears(-2).Year.ToString();
        public string TwoPreviousYearDataColumnHeader
        {
            get
            {
                return twoPreviousYearData;
            }
            set
            {
                twoPreviousYearData = value;
                this.RaisePropertyChanged(() => this.TwoPreviousYearDataColumnHeader);
            }
        }

        /// <summary>
        /// 3-Previous YearsHeader
        /// </summary>
        private string threePreviousYearData = DateTime.Now.AddYears(-3).Year.ToString();
        public string ThreePreviousYearDataColumnHeader
        {
            get
            {
                return threePreviousYearData;
            }
            set
            {
                threePreviousYearData = value;
                this.RaisePropertyChanged(() => this.ThreePreviousYearDataColumnHeader);
            }
        }

        #endregion

        #endregion

        #endregion      

        #region EventSubscribe

        /// <summary>
        /// Subscribing to Events
        /// </summary>
        /// <param name="eventAggregator"></param>
        public void SubscribeEvents(IEventAggregator eventAggregator)
        {
            eventAggregator.GetEvent<PeriodReferenceSetEvent>().Subscribe(HandlePeriodReferenceSet);
            eventAggregator.GetEvent<PortfolioReferenceSetEvent>().Subscribe(HandlePortfolioReferenceSet);
            eventAggregator.GetEvent<HoldingFilterReferenceSetEvent>().Subscribe(HandleFilterReferenceSet);
        }

        #endregion

        #region ICommand

        /// <summary>
        /// Zoom-In Command Button
        /// </summary>
        private ICommand zoomInCommand;
        public ICommand ZoomInCommand
        {
            get
            {
                if (zoomInCommand == null)
                {
                    zoomInCommand = new Telerik.Windows.Controls.DelegateCommand(ZoomInCommandMethod, ZoomInCommandValidation);
                }
                return zoomInCommand;
            }
        }

        /// <summary>
        /// Zoom-Out Command Button
        /// </summary>
        private ICommand zoomOutCommand;
        public ICommand ZoomOutCommand
        {
            get
            {
                if (zoomOutCommand == null)
                {
                    zoomOutCommand = new Telerik.Windows.Controls.DelegateCommand(ZoomOutCommandMethod, ZoomOutCommandValidation);
                }
                return zoomOutCommand;
            }
        }

        #endregion

        #region EventHandlers

        /// <summary>
        /// Handle Security change Event
        /// </summary>
        /// <param name="filterSelectionData">Details of Selected Security</param>
        public void HandleFilterReferenceSet(FilterSelectionData filterSelectionData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                //ArgumentNullException
                if (filterSelectionData != null)
                {
                    this.filterSelectionData = filterSelectionData;

                    if (SelectedEntities.ContainsKey("COUNTRY"))
                    {
                        SelectedEntities.Remove("COUNTRY");
                    }
                    if (SelectedEntities.ContainsKey("SECTOR"))
                    {
                        SelectedEntities.Remove("SECTOR");
                    }
                    if (filterSelectionData.Filtertype == "Country")
                    {
                        if (filterSelectionData.FilterValues != null)
                        {
                            SelectedEntities.Add("COUNTRY", filterSelectionData.FilterValues);
                        }
                    }
                    else if (filterSelectionData.Filtertype == "Sector")
                    {
                        if (filterSelectionData.FilterValues != null)
                        {
                            SelectedEntities.Add("SECTOR", filterSelectionData.FilterValues);
                        }
                    }

                    if (SelectedEntities != null && SelectedEntities.ContainsKey("PORTFOLIO") && IsActive)
                    {
                        dbInteractivity.RetrieveBenchmarkChartReturnData(SelectedEntities, RetrieveBenchmarkChartDataCallBackMethod);
                        dbInteractivity.RetrieveBenchmarkGridReturnData(SelectedEntities, RetrieveBenchmarkGridDataCallBackMethod);
                        BusyIndicatorStatus = true;
                    }
                }
                else
                {
                    Logging.LogMethodParameterNull(logger, methodNamespace, 1);
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
        }

        /// <summary>
        /// Handle Portfolio Change Event
        /// </summary>
        /// <param name="portfolioSelectionData">Detail of Selected Portfolio</param>
        public void HandlePortfolioReferenceSet(PortfolioSelectionData portfolioSelectionData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                //ArgumentNullException
                if (portfolioSelectionData != null && portfolioSelectionData.PortfolioId != null)
                {
                    if (SelectedEntities.ContainsKey("PORTFOLIO"))
                    {
                        SelectedEntities.Remove("PORTFOLIO");
                    }
                    SelectedEntities.Add("PORTFOLIO", portfolioSelectionData.PortfolioId);
                    if (SelectedEntities != null && SelectedEntities.ContainsKey("PORTFOLIO") && periodSelectionData != null && IsActive)
                    {
                        dbInteractivity.RetrieveBenchmarkChartReturnData(SelectedEntities, RetrieveBenchmarkChartDataCallBackMethod);
                        dbInteractivity.RetrieveBenchmarkGridReturnData(SelectedEntities, RetrieveBenchmarkGridDataCallBackMethod);
                        BusyIndicatorStatus = true;
                    }
                }
                else
                {
                    Logging.LogMethodParameterNull(logger, methodNamespace, 1);
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
        }

        /// <summary>
        /// Event handler to handle Period change Event
        /// </summary>
        /// <param name="periodSelectionData">Selected Period</param>
        public void HandlePeriodReferenceSet(string periodSelectionData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                //ArgumentNullException
                if (periodSelectionData != null)
                {
                    this.periodSelectionData = periodSelectionData;

                    if (MultiLineBenchmarkUIChartData.Count != 0)
                    {
                        BusyIndicatorStatus = true;
                        MultiLineBenchmarkUIChartData = CalculateDataAccordingToPeriod(MultiLineBenchmarkUIChartData, periodSelectionData);
                        BusyIndicatorStatus = false;
                    }
                    else
                    {
                        if (SelectedEntities != null && SelectedEntities.ContainsKey("PORTFOLIO") && periodSelectionData != null && IsActive)
                        {
                            dbInteractivity.RetrieveBenchmarkChartReturnData(SelectedEntities, RetrieveBenchmarkChartDataCallBackMethod);
                            dbInteractivity.RetrieveBenchmarkGridReturnData(SelectedEntities, RetrieveBenchmarkGridDataCallBackMethod);
                            BusyIndicatorStatus = true;
                        }
                    }
                }
                else
                {
                    Logging.LogMethodParameterNull(logger, methodNamespace, 1);
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
                BusyIndicatorStatus = false;
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
            ((Telerik.Windows.Controls.DelegateCommand)zoomInCommand).InvalidateCanExecute();
            ((Telerik.Windows.Controls.DelegateCommand)zoomOutCommand).InvalidateCanExecute();
        }

        /// <summary>
        /// Zoom In Command Method Validation
        /// </summary>
        /// <param name="parameter"></param>
        public bool ZoomInCommandValidation(object parameter)
        {
            if (this.ChartAreaMultiLineBenchmark == null)
            {
                return false;
            }
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
            ((Telerik.Windows.Controls.DelegateCommand)zoomInCommand).InvalidateCanExecute();
            ((Telerik.Windows.Controls.DelegateCommand)zoomOutCommand).InvalidateCanExecute();
        }

        /// <summary>
        /// Zoom Out Command Method Validation
        ///  </summary>
        /// <param name="parameter"></param>
        public bool ZoomOutCommandValidation(object parameter)
        {
            if (this.ChartAreaMultiLineBenchmark == null)
            {
                return false;
            }
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
            Logging.LogBeginMethod(logger, methodNamespace);

            try
            {
                if (result != null)
                {
                    MultiLineBenchmarkUIChartData.Clear();
                    MultiLineBenchmarkUIChartData.AddRange(result);
                    MultiLineBenchmarkUIChartData = CalculateDataAccordingToPeriod(MultiLineBenchmarkUIChartData, periodSelectionData);
                }
                else
                {
                    Logging.LogMethodParameterNull(logger, methodNamespace, 1);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            finally
            {
                BusyIndicatorStatus = false;
            }
        }

        /// <summary>
        /// Callback method for Benchmark Grid-Updates Grid
        /// </summary>
        /// <param name="result"></param>
        private void RetrieveBenchmarkGridDataCallBackMethod(List<BenchmarkGridReturnData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    MultiLineBenchmarUIGridData.Clear();
                    MultiLineBenchmarUIGridData.AddRange(result);
                }
                else
                {
                    Logging.LogMethodParameterNull(logger, methodNamespace, 1);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            finally
            {
                BusyIndicatorStatus = false;
            }
        }

        #endregion

        #region HelperMethods

        /// <summary>
        /// Data Context Source for chart
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ChartDataBound(object sender, ChartDataBoundEventArgs e)
        {
            ((DelegateCommand)zoomInCommand).InvalidateCanExecute();
            ((DelegateCommand)zoomOutCommand).InvalidateCanExecute();
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
        
        /// <summary>
        /// Validation Method for Zoom Out button
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns>boolean value</returns>
        public bool CanZoomOut(object parameter)
        {
            if (this.ChartAreaMultiLineBenchmark == null)
            {
                return false;
            }
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

        #region UnsubscribeEvents

        /// <summary>
        /// Unsubscribe to Events
        /// </summary>
        public void Dispose()
        {
            eventAggregator.GetEvent<PeriodReferenceSetEvent>().Unsubscribe(HandlePeriodReferenceSet);
            eventAggregator.GetEvent<PortfolioReferenceSetEvent>().Unsubscribe(HandlePortfolioReferenceSet);
            eventAggregator.GetEvent<HoldingFilterReferenceSetEvent>().Unsubscribe(HandleFilterReferenceSet);
        }

        #endregion
    }
}

