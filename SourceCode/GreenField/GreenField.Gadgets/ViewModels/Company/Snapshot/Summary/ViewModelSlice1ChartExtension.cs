using System;
using System.Collections.Generic;
using System.Windows;
using System.Linq;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Events;
using Telerik.Windows.Controls.Charting;
using GreenField.Common;
using GreenField.Gadgets.Helpers;
using GreenField.ServiceCaller;
using GreenField.DataContracts;

namespace GreenField.Gadgets.ViewModels
{
    /// <summary>
    /// View-Model for Slice-1 Chart Extension
    /// </summary>
    public class ViewModelSlice1ChartExtension : NotificationObject
    {
        #region Fields
               
        /// <summary>
        /// Event Aggregator
        /// </summary>
        private IEventAggregator eventAggregator;

        /// <summary>
        /// Instance of Service Caller Class
        /// </summary>
        private IDBInteractivity dbInteractivity;

        /// <summary>
        /// Instance of LoggerFacade
        /// </summary>
        public ILoggerFacade logger;

        /// <summary>
        /// Details of selected Security
        /// </summary>
        private EntitySelectionData entitySelectionData;

        /// <summary>
        /// Details of Selected Period
        /// </summary>
        private string period;
        
        #endregion

        #region Constructor

        /// <summary>
        /// Constructor that Initialises the class
        /// </summary>
        /// <param name="param"></param>
        public ViewModelSlice1ChartExtension(DashboardGadgetParam param)
        {
            eventAggregator = param.EventAggregator;
            dbInteractivity = param.DBInteractivity;
            logger = param.LoggerFacade;
            selectedPortfolio = param.DashboardGadgetPayload.PortfolioSelectionData;
            entitySelectionData = param.DashboardGadgetPayload.EntitySelectionData;
            period = param.DashboardGadgetPayload.PeriodSelectionData;
            if (entitySelectionData != null && period != null)
            {
                Dictionary<string, string> objDictionary = new Dictionary<string, string>();
                objDictionary.Add("SECURITY", entitySelectionData.LongName);
                DateTime startDate = DateTime.Today.AddYears(-1);
                if (IsActive)
                {
                    dbInteractivity.RetrieveChartExtensionData(objDictionary, startDate, RetrieveChartExtensionDataCallbackMethod);
                    BusyIndicatorStatus = true;
                }
            }
            if (eventAggregator != null)
            {
                SubscribeEvents(eventAggregator);
            }
        }

        #endregion

        #region PropertyDeclaration

        /// <summary>
        /// Collection of ChartExtensionData
        /// </summary>
        private RangeObservableCollection<ChartExtensionData> chartExtensionPlottedData;
        public RangeObservableCollection<ChartExtensionData> ChartExtensionPlottedData
        {
            get
            {
                if (chartExtensionPlottedData == null)
                {
                    chartExtensionPlottedData = new RangeObservableCollection<ChartExtensionData>();
                }
                return chartExtensionPlottedData;
            }
            set
            {
                chartExtensionPlottedData = value;
                this.RaisePropertyChanged(() => this.ChartExtensionPlottedData);
            }
        }

        /// <summary>
        /// Details of Selected Security
        /// </summary>
        private EntitySelectionData selectedSecurity;
        public EntitySelectionData SelectedSecurity
        {
            get
            {
                return selectedSecurity;
            }
            set
            {
                selectedSecurity = value;
                this.RaisePropertyChanged(() => this.SelectedSecurity);
            }
        }

        /// <summary>
        /// Details of Selected Portfolio
        /// </summary>
        private PortfolioSelectionData selectedPortfolio;
        public PortfolioSelectionData SelectedPortfolio
        {
            get
            {
                return selectedPortfolio;
            }
            set
            {
                selectedPortfolio = value;
                this.RaisePropertyChanged(() => this.SelectedPortfolio);
            }
        }

        /// <summary>
        /// Details of Selected Date
        /// </summary>
        private DateTime? selectedStartDate = DateTime.Today.AddYears(-1);
        public DateTime? SelectedStartDate
        {
            get
            {
                return selectedStartDate;
            }
            set
            {
                selectedStartDate = value;
                this.RaisePropertyChanged(() => this.SelectedStartDate);
            }
        }

        /// <summary>
        /// Id's of selected Portfolio & Security
        /// </summary>
        private Dictionary<string, string> selectedEntities;
        public Dictionary<string, string> SelectedEntities
        {
            get
            {
                if (selectedEntities == null)
                {
                    selectedEntities = new Dictionary<string, string>();
                }
                return selectedEntities;
            }
            set
            {
                selectedEntities = value;
                this.RaisePropertyChanged(() => this.SelectedEntities);
            }
        }

        /// <summary>
        /// ChartArea property bound to ChartArea of dgChartExtension 
        /// </summary>
        private ChartArea chartArea;
        public ChartArea ChartArea
        {
            get
            {
                return this.chartArea;
            }
            set
            {
                this.chartArea = value;
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
        /// Collection of Time Range Options
        /// </summary>
        public ObservableCollection<String> TimeRange
        {
            get
            {
                return new ObservableCollection<string> { "1-Month", "2-Months", "3-Months", "6-Months", "YTD", "1-Year", "2-Years", 
                    "3-Years", "4-Years", "5-Years", "10-Years" };
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
                selectedTimeRange = value;
                GetPeriod();
                if (SelectedEntities != null && SelectedStartDate != null)
                {
                    if ((SelectedEntities.ContainsKey("PORTFOLIO") && SelectedEntities.ContainsKey("SECURITY"))
                        || (SelectedEntities.ContainsKey("SECURITY")))
                    {
                        dbInteractivity.RetrieveChartExtensionData(SelectedEntities, Convert.ToDateTime(SelectedStartDate), RetrieveChartExtensionDataCallbackMethod);
                        BusyIndicatorStatus = true;
                    }
                }
                this.RaisePropertyChanged(() => this.SelectedTimeRange);
            }
        }

        /// <summary>
        /// Legend Label for Transaction Axis
        /// </summary>
        private string transactionLegendLabel;
        public string TransactionLegendLabel
        {
            get
            {
                return transactionLegendLabel;
            }
            set
            {
                transactionLegendLabel = value;
                this.RaisePropertyChanged(() => this.TransactionLegendLabel);
            }
        }


        /// <summary>
        /// Minimum Value for X-Axis of Chart
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
        /// Maximum Value for X-Axis of Chart
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
        /// Step size of XAxis of Chart
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
                if (SelectedEntities.ContainsKey("PORTFOLIO") && SelectedEntities.ContainsKey("SECURITY") && SelectedStartDate != null && period != null && isActive)
                {
                    dbInteractivity.RetrieveChartExtensionData(SelectedEntities, Convert.ToDateTime(SelectedStartDate), RetrieveChartExtensionDataCallbackMethod);
                    BusyIndicatorStatus = true;
                }
            }
        }

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
        
        #endregion

        #region ICommandMethods

        /// <summary>
        /// Zoom In Command Method
        /// </summary>
        /// <param name="parameter"></param>
        public void ZoomInCommandMethod(object parameter)
        {
            ZoomIn(this.ChartArea);
            ((Telerik.Windows.Controls.DelegateCommand)zoomInCommand).InvalidateCanExecute();
            ((Telerik.Windows.Controls.DelegateCommand)zoomOutCommand).InvalidateCanExecute();
        }

        /// <summary>
        /// Zoom In Command Method Validation
        /// </summary>
        /// <param name="parameter"></param>
        public bool ZoomInCommandValidation(object parameter)
        {
            if (this.ChartArea == null)
                return false;

            return
                this.ChartArea.ZoomScrollSettingsX.Range > this.ChartArea.ZoomScrollSettingsX.MinZoomRange;
        }

        /// <summary>
        /// Zoom Out Command Method
        /// </summary>
        /// <param name="parameter"></param>
        public void ZoomOutCommandMethod(object parameter)
        {
            ZoomOut(this.ChartArea);
            ((Telerik.Windows.Controls.DelegateCommand)zoomInCommand).InvalidateCanExecute();
            ((Telerik.Windows.Controls.DelegateCommand)zoomOutCommand).InvalidateCanExecute();
        }

        /// <summary>
        /// Zoom Out Command Method Validation
        ///  </summary>
        /// <param name="parameter"></param>
        public bool ZoomOutCommandValidation(object parameter)
        {
            if (this.ChartArea == null)
            {
                return false;
            }
            return this.ChartArea.ZoomScrollSettingsX.Range < 1d;
        }

        #endregion

        #region EventSubscribers

        /// <summary>
        /// Subscribe Events
        /// </summary>
        /// <param name="_eventAggregator">Event Aggregator</param>
        private void SubscribeEvents(IEventAggregator _eventAggregator)
        {
            _eventAggregator.GetEvent<PortfolioReferenceSetEvent>().Subscribe(HandlePortfolioReferenceSet);
            _eventAggregator.GetEvent<SecurityReferenceSetEvent>().Subscribe(HandleSecurityReferenceSet, false);
            _eventAggregator.GetEvent<PeriodReferenceSetEvent>().Subscribe(HandleEffectivePeriodSet);
        }

        #endregion

        #region EventHandlers

        /// <summary>
        /// Event handler for PortfolioSelection changed Event
        /// </summary>
        /// <param name="PortfolioSelectionData"></param>
        public void HandlePortfolioReferenceSet(PortfolioSelectionData PortfolioSelectionData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                //arguement null exception
                if (PortfolioSelectionData != null)
                {
                    SelectedPortfolio = PortfolioSelectionData;
                    if (SelectedEntities.ContainsKey("PORTFOLIO"))
                        SelectedEntities.Remove("PORTFOLIO");
                    SelectedEntities.Add("PORTFOLIO", PortfolioSelectionData.PortfolioId);

                    if (SelectedEntities.ContainsKey("PORTFOLIO") && SelectedEntities.ContainsKey("SECURITY") && SelectedStartDate != null && period != null && IsActive)
                    {
                        dbInteractivity.RetrieveChartExtensionData(SelectedEntities, Convert.ToDateTime(SelectedStartDate), RetrieveChartExtensionDataCallbackMethod);
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
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
        }

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
                //argument null exception
                if (entitySelectionData != null)
                {
                    SelectedSecurity = entitySelectionData;
                    if (SelectedEntities.ContainsKey("SECURITY"))
                        SelectedEntities.Remove("SECURITY");
                    SelectedEntities.Add("SECURITY", entitySelectionData.LongName);

                    if (SelectedStartDate != null && SelectedEntities != null && period != null && SelectedEntities.ContainsKey("PORTFOLIO") && IsActive)
                    {
                        dbInteractivity.RetrieveChartExtensionData(SelectedEntities, Convert.ToDateTime(SelectedStartDate), RetrieveChartExtensionDataCallbackMethod);
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
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
        }

        /// <summary>
        /// Handle Date Change Event
        /// </summary>
        /// <param name="effectiveDate">Effective Date</param>
        public void HandleEffectivePeriodSet(string period)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (period != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, period, 1);
                    this.period = period;
                    if (ChartExtensionPlottedData.Count != 0)
                    {
                        RetrieveChartAccordingDataPeriod(period);
                    }
                    else if (SelectedEntities != null && SelectedStartDate != null && period != null && SelectedEntities.ContainsKey("PORTFOLIO") && SelectedEntities.ContainsKey("SECURITY") && IsActive)
                    {
                        dbInteractivity.RetrieveChartExtensionData(SelectedEntities, Convert.ToDateTime(SelectedStartDate), RetrieveChartExtensionDataCallbackMethod);
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
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }

            Logging.LogEndMethod(logger, methodNamespace);
        }

        /// <summary>
        /// Binding zoom commands to Chart
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ChartDataBound(object sender, ChartDataBoundEventArgs e)
        {
            ((Telerik.Windows.Controls.DelegateCommand)zoomInCommand).InvalidateCanExecute();
            ((Telerik.Windows.Controls.DelegateCommand)zoomOutCommand).InvalidateCanExecute();
        }

        #endregion

        #region CallbackMethods

        /// <summary>
        /// Callback Method for ChartExtensionData
        /// </summary>
        /// <param name="chartExtensionData">Returns Collection of ChartExtensionData</param>
        private void RetrieveChartExtensionDataCallbackMethod(List<ChartExtensionData> chartExtensionData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (chartExtensionData != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, chartExtensionData, 1);
                    ChartExtensionPlottedData.Clear();
                    if (chartExtensionData.Any(a => a.AmountTraded != null))
                    {
                        TransactionLegendLabel = chartExtensionData.Where(a => a.Type == "SECURITY").Select(a => a.Ticker).FirstOrDefault();
                    }
                    else
                    {
                        TransactionLegendLabel = " ";
                    }
                    ChartExtensionPlottedData.AddRange(chartExtensionData.OrderBy(a => a.SortId).ToList());
                    RetrieveChartAccordingDataPeriod(period);
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
            Logging.LogEndMethod(logger, methodNamespace);
        }

        #endregion

        #region HelperMethods

        /// <summary>
        /// Show Sector/Country data in Chart according to SelectedPeriod
        /// </summary>
        /// <param name="selectedPeriod">Selected Period from the Toolbar</param>
        /// <param name="chartedData"></param>
        private void RetrieveChartAccordingDataPeriod(string selectedPeriod)
        {
            try
            {
                if (selectedPeriod == null)
                {
                    return;
                }
                if (ChartExtensionPlottedData == null)
                {
                    return;
                }
                if (ChartExtensionPlottedData.Count == 0)
                {
                    return;
                }
                List<ChartExtensionData> data = ChartExtensionPlottedData.ToList();

                #region SwitchAccordingToPeriod

                switch (selectedPeriod)
                {
                    case ("1D"):
                        {
                            foreach (ChartExtensionData item in data)
                            {
                                if (item.Type.ToUpper().Trim() == "SECTOR" || item.Type.ToUpper().Trim() == "COUNTRY")
                                {
                                    item.PriceReturn = item.OneD;
                                }
                            }
                            break;
                        }
                    case ("1W"):
                        {
                            foreach (ChartExtensionData item in data)
                            {
                                if (item.Type.ToUpper().Trim() == "SECTOR" || item.Type.ToUpper().Trim() == "COUNTRY")
                                {
                                    item.PriceReturn = item.WTD;
                                }
                            }
                            break;
                        }
                    case ("MTD"):
                        {
                            foreach (ChartExtensionData item in data)
                            {
                                if (item.Type.ToUpper().Trim() == "SECTOR" || item.Type.ToUpper().Trim() == "COUNTRY")
                                {
                                    item.PriceReturn = item.MTD;
                                }
                            }
                            break;
                        }
                    case ("QTD"):
                        {
                            foreach (ChartExtensionData item in data)
                            {
                                if (item.Type.ToUpper().Trim() == "SECTOR" || item.Type.ToUpper().Trim() == "COUNTRY")
                                {
                                    item.PriceReturn = item.QTD;
                                }
                            }
                            break;
                        }
                    case ("YTD"):
                        {
                            foreach (ChartExtensionData item in data)
                            {
                                if (item.Type.ToUpper().Trim() == "SECTOR" || item.Type.ToUpper().Trim() == "COUNTRY")
                                {
                                    item.PriceReturn = item.YTD;
                                }
                            }
                            break;
                        }
                    case ("1Y"):
                        {
                            foreach (ChartExtensionData item in data)
                            {
                                if (item.Type.ToUpper().Trim() == "SECTOR" || item.Type.ToUpper().Trim() == "COUNTRY")
                                {
                                    item.PriceReturn = item.OneY;
                                }
                            }
                            break;
                        }
                    default:
                        {
                            foreach (ChartExtensionData item in data)
                            {
                                if (item.Type.ToUpper().Trim() == "SECTOR" || item.Type.ToUpper().Trim() == "COUNTRY")
                                {
                                    item.PriceReturn = item.OneD;
                                }
                            }
                            break;
                        }
                }
                #endregion

                ChartExtensionPlottedData.Clear();
                ChartExtensionPlottedData.AddRange(data);
                this.RaisePropertyChanged(() => this.ChartExtensionPlottedData);
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
        }

        /// <summary>
        /// Zoom In Algo
        /// </summary>
        /// <param name="chartArea"></param>
        private void ZoomIn(ChartArea chartArea)
        {
            try
            {
                chartArea.ZoomScrollSettingsX.SuspendNotifications();
                double zoomCenter = chartArea.ZoomScrollSettingsX.RangeStart + (chartArea.ZoomScrollSettingsX.Range / 2);
                double newRange = Math.Max(chartArea.ZoomScrollSettingsX.MinZoomRange, chartArea.ZoomScrollSettingsX.Range) / 2;
                chartArea.ZoomScrollSettingsX.RangeStart = Math.Max(0, zoomCenter - (newRange / 2));
                chartArea.ZoomScrollSettingsX.RangeEnd = Math.Min(1, zoomCenter + (newRange / 2));

                chartArea.ZoomScrollSettingsX.ResumeNotifications();
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
        }

        /// <summary>
        /// Zoom out Algo
        /// </summary>
        /// <param name="chartArea"></param>
        private void ZoomOut(ChartArea chartArea)
        {
            try
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
            switch (SelectedTimeRange)
            {
                case "1-Month":
                    SelectedStartDate = DateTime.Today.AddMonths(-1);
                    break;
                case "2-Months":
                    SelectedStartDate = DateTime.Today.AddMonths(-2);
                    break;
                case "3-Months":
                    SelectedStartDate = DateTime.Today.AddMonths(-3);
                    break;
                case "6-Months":
                    SelectedStartDate = DateTime.Today.AddMonths(-6);
                    break;
                case "9-Months":
                    SelectedStartDate = DateTime.Today.AddMonths(-9);
                    break;
                case "1-Year":
                    SelectedStartDate = DateTime.Today.AddMonths(-12);
                    break;
                case "2-Years":
                    SelectedStartDate = DateTime.Today.AddMonths(-24);
                    break;
                case "3-Years":
                    SelectedStartDate = DateTime.Today.AddMonths(-36);
                    break;
                case "4-Years":
                    SelectedStartDate = DateTime.Today.AddMonths(-48);
                    break;
                case "5-Years":
                    SelectedStartDate = DateTime.Today.AddMonths(-60);
                    break;
                case "10-Years":
                    SelectedStartDate = DateTime.Today.AddMonths(-120);
                    break;
                case "YTD":
                    SelectedStartDate = new DateTime((int)(DateTime.Today.Year), 1, 1);
                    break;
                default:
                    SelectedStartDate = DateTime.Today.AddMonths(-12);
                    break;
            }
        }

        #endregion

        #region EventUnSubscribe

        /// <summary>
        /// Dsiposing off Events and Event Subscribers
        /// </summary>
        public void Dispose()
        {
            if (eventAggregator != null)
            {
                eventAggregator.GetEvent<PortfolioReferenceSetEvent>().Unsubscribe(HandlePortfolioReferenceSet);
                eventAggregator.GetEvent<SecurityReferenceSetEvent>().Unsubscribe(HandleSecurityReferenceSet);
                eventAggregator.GetEvent<PeriodReferenceSetEvent>().Unsubscribe(HandleEffectivePeriodSet);
            }
        }

        #endregion
    }
}