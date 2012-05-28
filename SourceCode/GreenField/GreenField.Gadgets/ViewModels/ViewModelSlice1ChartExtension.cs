using System;
using System.Windows;
using System.Windows.Input;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Prism.Logging;
using GreenField.ServiceCaller;
using Microsoft.Practices.Prism.Events;
using GreenField.ServiceCaller.BenchmarkHoldingsDefinitions;
using GreenField.ServiceCaller.SecurityReferenceDefinitions;
using GreenField.Common;
using GreenField.Gadgets.Helpers;
using System.Collections.Generic;
using Telerik.Windows.Controls.Charting;
using System.Collections.ObjectModel;
using Telerik.Windows.Controls;
using GreenField.DataContracts;

namespace GreenField.Gadgets.ViewModels
{
    public class ViewModelSlice1ChartExtension : NotificationObject
    {

        #region Fields

        //MEF Singletons

        /// <summary>
        /// Event Aggregator
        /// </summary>
        private IEventAggregator _eventAggregator;

        /// <summary>
        /// Instance of Service Caller Class
        /// </summary>
        private IDBInteractivity _dbInteractivity;

        /// <summary>
        /// Instance of LoggerFacade
        /// </summary>
        private ILoggerFacade _logger;

        /// <summary>
        /// Details of Selected Portfolio
        /// </summary>
        private PortfolioSelectionData _portfolioSelectionData;

        /// <summary>
        /// Selected Date
        /// </summary>
        private DateTime? _effectiveDate;

        /// <summary>
        /// Details of selected Security
        /// </summary>
        private EntitySelectionData _entitySelectionData;

        /// <summary>
        /// Details of Selected Period
        /// </summary>
        private string _period;


        #endregion

        #region Constructor

        public ViewModelSlice1ChartExtension(DashboardGadgetParam param)
        {
            _eventAggregator = param.EventAggregator;
            _dbInteractivity = param.DBInteractivity;
            _logger = param.LoggerFacade;
            _selectedPortfolio = param.DashboardGadgetPayload.PortfolioSelectionData;
            _entitySelectionData = param.DashboardGadgetPayload.EntitySelectionData;
            _period = param.DashboardGadgetPayload.PeriodSelectionData;
            if (_entitySelectionData != null && _period != null)
            {
                Dictionary<string, string> objDictionary = new Dictionary<string, string>();
                objDictionary.Add("SECURITY", _entitySelectionData.LongName);
                DateTime startDate = DateTime.Today.AddYears(-1);
                _dbInteractivity.RetrieveChartExtensionData(objDictionary, startDate, RetrieveChartExtensionDataCallbackMethod);
            }

            if (_eventAggregator != null)
                SubscribeEvents(_eventAggregator);

        }

        #endregion

        #region PropertyDeclaration

        /// <summary>
        /// Collection of ChartExtensionData
        /// </summary>
        private RangeObservableCollection<ChartExtensionData> _chartExtensionData;
        public RangeObservableCollection<ChartExtensionData> ChartExtensionData
        {
            get
            {
                if (_chartExtensionData == null)
                    _chartExtensionData = new RangeObservableCollection<ChartExtensionData>();
                return _chartExtensionData;
            }
            set
            {
                _chartExtensionData = value;
                this.RaisePropertyChanged(() => this.ChartExtensionData);
            }
        }

        /// <summary>
        /// Details of Selected Security
        /// </summary>
        private EntitySelectionData _selectedSecurity;
        public EntitySelectionData SelectedSecurity
        {
            get
            {
                return _selectedSecurity;
            }
            set
            {
                _selectedSecurity = value;
                this.RaisePropertyChanged(() => this.SelectedSecurity);
            }
        }

        /// <summary>
        /// Details of Selected Portfolio
        /// </summary>
        private PortfolioSelectionData _selectedPortfolio;
        public PortfolioSelectionData SelectedPortfolio
        {
            get
            {
                return _selectedPortfolio;
            }
            set
            {
                _selectedPortfolio = value;
                this.RaisePropertyChanged(() => this.SelectedPortfolio);
            }
        }

        /// <summary>
        /// Details of Selected Date
        /// </summary>
        private DateTime? _selectedStartDate = DateTime.Today.AddYears(-1);
        public DateTime? SelectedStartDate
        {
            get
            {
                return _selectedStartDate;
            }
            set
            {
                _selectedStartDate = value;
                this.RaisePropertyChanged(() => this.SelectedStartDate);
            }
        }

        /// <summary>
        /// Id's of selected Portfolio & Security
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

        private ChartArea _chartArea;
        public ChartArea ChartArea
        {
            get
            {
                return this._chartArea;
            }
            set
            {
                this._chartArea = value;
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
        private string _selectedTimeRange = "1-Year";
        public string SelectedTimeRange
        {
            get
            {
                return _selectedTimeRange;
            }
            set
            {
                _selectedTimeRange = value;
                GetPeriod();
                if (SelectedEntities != null && SelectedStartDate != null)
                {
                    if (SelectedEntities.ContainsKey("PORTFOLIO") && SelectedEntities.ContainsKey("SECURITY"))
                    {
                        _dbInteractivity.RetrieveChartExtensionData(SelectedEntities, Convert.ToDateTime(SelectedStartDate), RetrieveChartExtensionDataCallbackMethod);
                        if (null != ChartExtensionDataLoadedEvent)
                            ChartExtensionDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
                    }
                }
                this.RaisePropertyChanged(() => this.SelectedTimeRange);
            }
        }

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


        #endregion

        #region Events

        public event DataRetrievalProgressIndicatorEventHandler ChartExtensionDataLoadedEvent;

        #endregion

        #region ICommandMethods

        /// <summary>
        /// Zoom In Command Method
        /// </summary>
        /// <param name="parameter"></param>
        public void ZoomInCommandMethod(object parameter)
        {
            ZoomIn(this.ChartArea);
            ((Telerik.Windows.Controls.DelegateCommand)_zoomInCommand).InvalidateCanExecute();
            ((Telerik.Windows.Controls.DelegateCommand)_zoomOutCommand).InvalidateCanExecute();
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
            ((Telerik.Windows.Controls.DelegateCommand)_zoomInCommand).InvalidateCanExecute();
            ((Telerik.Windows.Controls.DelegateCommand)_zoomOutCommand).InvalidateCanExecute();
        }

        /// <summary>
        /// Zoom Out Command Method Validation
        ///  </summary>
        /// <param name="parameter"></param>
        public bool ZoomOutCommandValidation(object parameter)
        {
            if (this.ChartArea == null)
                return false;

            return this.ChartArea.ZoomScrollSettingsX.Range < 1d;
        }

        #endregion

        #region EventSubscribers

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
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                //Arguement Null Exception
                if (PortfolioSelectionData != null)
                {
                    SelectedPortfolio = PortfolioSelectionData;
                    if (SelectedEntities.ContainsKey("PORTFOLIO"))
                        SelectedEntities.Remove("PORTFOLIO");
                    SelectedEntities.Add("PORTFOLIO", PortfolioSelectionData.PortfolioId);

                    if (SelectedEntities.ContainsKey("PORTFOLIO") && SelectedEntities.ContainsKey("SECURITY") && SelectedStartDate != null)
                    {
                        _dbInteractivity.RetrieveChartExtensionData(SelectedEntities, Convert.ToDateTime(SelectedStartDate), RetrieveChartExtensionDataCallbackMethod);
                        if (null != ChartExtensionDataLoadedEvent)
                            ChartExtensionDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
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
        /// Event Handler to subscribed event 'SecurityReferenceSet'
        /// </summary>
        /// <param name="securityReferenceData">SecurityReferenceData</param>
        public void HandleSecurityReferenceSet(EntitySelectionData entitySelectionData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                //ArgumentNullException
                if (entitySelectionData != null)
                {
                    SelectedSecurity = entitySelectionData;
                    if (SelectedEntities.ContainsKey("SECURITY"))
                        SelectedEntities.Remove("SECURITY");
                    SelectedEntities.Add("SECURITY", entitySelectionData.LongName);

                    if (SelectedStartDate != null && SelectedEntities != null)
                    {
                        _dbInteractivity.RetrieveChartExtensionData(SelectedEntities, Convert.ToDateTime(SelectedStartDate), RetrieveChartExtensionDataCallbackMethod);
                        if (null != ChartExtensionDataLoadedEvent)
                            ChartExtensionDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
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
        /// Handle Date Change Event
        /// </summary>
        /// <param name="effectiveDate">Effective Date</param>
        public void HandleEffectivePeriodSet(string period)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (period != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, period, 1);

                    if (SelectedEntities != null)
                    {
                        if (SelectedEntities.ContainsKey("SECURITY") && SelectedStartDate != null)
                        {
                            //Waiting for calculations for Country/Sector Returns
                        }
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
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        /// <summary>
        /// Binding zoom commands to Chart
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ChartDataBound(object sender, ChartDataBoundEventArgs e)
        {
            ((Telerik.Windows.Controls.DelegateCommand)_zoomInCommand).InvalidateCanExecute();
            ((Telerik.Windows.Controls.DelegateCommand)_zoomOutCommand).InvalidateCanExecute();
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
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (chartExtensionData != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, chartExtensionData, 1);
                    ChartExtensionData.Clear();
                    ChartExtensionData.AddRange(chartExtensionData);
                }
                else
                {
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                }
                if (null != ChartExtensionDataLoadedEvent)
                    ChartExtensionDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
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
            if (_eventAggregator != null)
            {
                _eventAggregator.GetEvent<PortfolioReferenceSetEvent>().Unsubscribe(HandlePortfolioReferenceSet);
                _eventAggregator.GetEvent<SecurityReferenceSetEvent>().Unsubscribe(HandleSecurityReferenceSet);
                _eventAggregator.GetEvent<PeriodReferenceSetEvent>().Unsubscribe(HandleEffectivePeriodSet);
            }
        }

        #endregion

    }
}