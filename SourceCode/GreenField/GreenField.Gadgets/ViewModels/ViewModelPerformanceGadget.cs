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
using System.Collections.Generic;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Commands;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.Charting;
using GreenField.Common;
using GreenField.Gadgets.Helpers;
using GreenField.ServiceCaller.SecurityReferenceDefinitions;
using GreenField.ServiceCaller.BenchmarkHoldingsDefinitions;
using GreenField.DataContracts;
using GreenField.ServiceCaller;

namespace GreenField.Gadgets.ViewModels
{
    /// <summary>
    /// Class that provides the interaction of the view model with the Service caller and the View.
    /// </summary>
    public class ViewModelPerformanceGadget : NotificationObject
    {
        #region PrivateMembers
        /// <summary>
        /// private member object of the IEventAggregator for event aggregation
        /// </summary>
        private IEventAggregator eventAggregator;

        /// <summary>
        /// private member object of the IDBInteractivity for interaction with the Service Caller
        /// </summary>
        private IDBInteractivity dbInteractivity;

        /// <summary>
        /// private member object of ILoggerFacade for logging
        /// </summary>
        private ILoggerFacade logger;

        /// <summary>
        /// private member object of the PortfolioSelectionData class for storing Fund Selection Data
        /// </summary>
        private PortfolioSelectionData portfolioSelectionData;

        /// <summary>
        /// Stores Effective Date selected by the user
        /// </summary>
        private DateTime? effectiveDate;

        /// <summary>
        /// Selected Period
        /// </summary>
        public String selectedPeriod;

        /// <summary>
        /// Country selected from the heat map
        /// </summary>
        private String country;  
        #endregion

        #region Constructor

        /// <summary>
        /// Constructor of the class that initializes various objects
        /// </summary>
        /// <param name="param">MEF Eventaggrigator instance</param>
        public ViewModelPerformanceGadget(DashboardGadgetParam param)
        {
            dbInteractivity = param.DBInteractivity;
            logger = param.LoggerFacade;
            eventAggregator = param.EventAggregator;
            portfolioSelectionData = param.DashboardGadgetPayload.PortfolioSelectionData;
            effectiveDate = param.DashboardGadgetPayload.EffectiveDate;
            country = param.DashboardGadgetPayload.HeatMapCountryData;
            selectedPeriod = param.DashboardGadgetPayload.PeriodSelectionData;
            if (effectiveDate != null && portfolioSelectionData != null && selectedPeriod !=null && IsActive)
            {            
                dbInteractivity.RetrievePerformanceGraphData(portfolioSelectionData, Convert.ToDateTime(effectiveDate),selectedPeriod,"NoFiltering",
                    RetrievePerformanceGraphDataCallBackMethod);
            }
            if (eventAggregator != null)
            {
                eventAggregator.GetEvent<PortfolioReferenceSetEvent>().Subscribe(HandlePortfolioReferenceSet, false);
                eventAggregator.GetEvent<EffectiveDateReferenceSetEvent>().Subscribe(HandleEffectiveDateSet, false);
                eventAggregator.GetEvent<PeriodReferenceSetEvent>().Subscribe(HandlePeriodReferenceSet, false);
                eventAggregator.GetEvent<HeatMapClickEvent>().Subscribe(HandleCountrySelectionDataSet, false);
            } 
        }

        #endregion

        #region Properties

        #region UI Fields
        /// <summary>
        /// Collection binded to the Comparison chart - consists of unrealized gain loss,closing price and Closing date for the primary entity
        /// </summary>
        private RangeObservableCollection<PerformanceGraphData> plottedSeries;
        public RangeObservableCollection<PerformanceGraphData> PlottedSeries
        {
            get
            {
                if (plottedSeries == null)
                {
                    plottedSeries = new RangeObservableCollection<PerformanceGraphData>();
                }
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
        /// Collection that defines the chart area
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

        private bool isActive;
        /// <summary>
        /// IsActive is true when parent control is displayed on UI
        /// </summary>
        public bool IsActive
        {
            get
            {
                return isActive;
            }
            set
            {
                isActive = value;
                if (portfolioSelectionData != null && effectiveDate != null && selectedPeriod != null && isActive)
                {
                    if (country != null)
                    {
                        BeginWebServiceCall(portfolioSelectionData, Convert.ToDateTime(effectiveDate), selectedPeriod, country);
                    }
                    else
                    {
                        BeginWebServiceCall(portfolioSelectionData, Convert.ToDateTime(effectiveDate), selectedPeriod, "NoFiltering");
                    }
                }
            }
        }
        #endregion

        #region ICommand
        /// <summary>
        /// ICommand property for Zoom in button
        /// </summary>
        ICommand zoomInCommand;
        public ICommand ZoomInCommand
        {
            get
            {
                if (zoomInCommand == null)
                {
                    zoomInCommand = new Telerik.Windows.Controls.DelegateCommand(ZoomIn, CanZoomIn);
                }
                return zoomInCommand;
            }
        }

        /// <summary>
        /// ICommand property for Zoom out button
        /// </summary>
        ICommand zoomOutCommand;
        public ICommand ZoomOutCommand
        {
            get
            {
                if (zoomOutCommand == null)
                {
                    zoomOutCommand = new Telerik.Windows.Controls.DelegateCommand(ZoomOut, CanZoomOut);
                }
                return zoomOutCommand;
            }
        }
        #endregion        

        #endregion

        #region Events
        /// <summary>
        /// Event for the notification of Data Load Completion
        /// </summary>
        public event DataRetrievalProgressIndicatorEventHandler PerformanceGraphDataLoadedEvent;
        #endregion

        #region Event Handlers
        /// <summary>
        /// Assigns UI Field Properties based on Fund reference
        /// </summary>
        /// <param name="portSelectionData">Object of PortfolioSelectionData Class</param>
        public void HandlePortfolioReferenceSet(PortfolioSelectionData portSelectionData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (portSelectionData != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, portSelectionData, 1);
                    portfolioSelectionData = portSelectionData;
                    if (portSelectionData != null && effectiveDate != null && selectedPeriod!=null && IsActive)
                    {                        
                        BeginWebServiceCall(portfolioSelectionData, Convert.ToDateTime(effectiveDate), selectedPeriod, "NoFiltering");
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
        /// Assigns UI Field Properties based on Effective Date
        /// </summary>
        /// <param name="effectDate">Effective Date selected by the user</param>
        public void HandleEffectiveDateSet(DateTime effectDate)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (effectDate != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, effectDate, 1);
                    effectiveDate = effectDate;
                    if (portfolioSelectionData != null && effectiveDate != null && selectedPeriod != null && IsActive)
                    {
                        BeginWebServiceCall(portfolioSelectionData, Convert.ToDateTime(effectiveDate), selectedPeriod, "NoFiltering");
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
        /// Assigns UI Field Properties based on Period
        /// </summary>
        /// <param name="selectedPeriodType">Period selected by the user</param>
        public void HandlePeriodReferenceSet(String selectedPeriodType)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (selectedPeriodType != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, selectedPeriodType, 1);
                    selectedPeriod = selectedPeriodType;
                    if (portfolioSelectionData != null && effectiveDate != null && selectedPeriod != null && IsActive)
                    {                        
                        BeginWebServiceCall(portfolioSelectionData, Convert.ToDateTime(effectiveDate), selectedPeriod, "NoFiltering");
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
        /// Assigns UI Field Properties based on Country
        /// </summary>
        /// <param name="cou">Country Selected by the user from the heat Map</param>
        public void HandleCountrySelectionDataSet(String cou)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (cou != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, cou, 1);
                    country = cou;
                    if (portfolioSelectionData != null && effectiveDate != null && country != null && IsActive)
                    {
                        BeginWebServiceCall(portfolioSelectionData, Convert.ToDateTime(effectiveDate), selectedPeriod, country);
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
        /// Calling Web service through dbInteractivity
        /// </summary>
        /// <param name="portfolioSelectionData"></param>
        /// <param name="effectiveDate"></param>
        /// <param name="selectedPeriodType"></param>
        /// <param name="country"></param>
        private void BeginWebServiceCall(PortfolioSelectionData portfolioSelectionData, DateTime effectiveDate, String selectedPeriodType
            , String country)
        {
            if (null != PerformanceGraphDataLoadedEvent)
                PerformanceGraphDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
            dbInteractivity.RetrievePerformanceGraphData(portfolioSelectionData, effectiveDate, selectedPeriodType, country, RetrievePerformanceGraphDataCallBackMethod);
        }

        #endregion

        #region ICommand Methods
        /// <summary>
        /// ICommand Method for Zoom In button
        /// </summary>
        /// <param name="parameter"></param>
        public void ZoomIn(object parameter)
        {
            this.ChartArea.ZoomScrollSettingsX.SuspendNotifications();

            double zoomCenter = this.ChartArea.ZoomScrollSettingsX.RangeStart + (this.ChartArea.ZoomScrollSettingsX.Range / 2);
            double newRange = Math.Max(this.ChartArea.ZoomScrollSettingsX.MinZoomRange, this.ChartArea.ZoomScrollSettingsX.Range) / 2;
            this.ChartArea.ZoomScrollSettingsX.RangeStart = Math.Max(0, zoomCenter - (newRange / 2));
            this.ChartArea.ZoomScrollSettingsX.RangeEnd = Math.Min(1, zoomCenter + (newRange / 2));

            this.ChartArea.ZoomScrollSettingsX.ResumeNotifications();

            ((Telerik.Windows.Controls.DelegateCommand)zoomInCommand).InvalidateCanExecute();
            ((Telerik.Windows.Controls.DelegateCommand)zoomOutCommand).InvalidateCanExecute();
        }

        /// <summary>
        /// Validation Method for Zoom In button
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns>boolean value</returns>
        public bool CanZoomIn(object parameter)
        {
            if (this.ChartArea == null)
            {
                return false;
            }
            return this.ChartArea.ZoomScrollSettingsX.Range > this.ChartArea.ZoomScrollSettingsX.MinZoomRange;
        }

        /// <summary>
        /// ICommand Method for Zoom Out button
        /// </summary>
        /// <param name="parameter"></param>
        public void ZoomOut(object parameter)
        {
            this.ChartArea.ZoomScrollSettingsX.SuspendNotifications();

            double zoomCenter = this.ChartArea.ZoomScrollSettingsX.RangeStart + (this.ChartArea.ZoomScrollSettingsX.Range / 2);
            double newRange = Math.Min(1, this.ChartArea.ZoomScrollSettingsX.Range) * 2;

            if (zoomCenter + (newRange / 2) > 1)
            {
                zoomCenter = 1 - (newRange / 2);
            }
            else if (zoomCenter - (newRange / 2) < 0)
                zoomCenter = newRange / 2;

            this.ChartArea.ZoomScrollSettingsX.RangeStart = Math.Max(0, zoomCenter - newRange / 2);
            this.ChartArea.ZoomScrollSettingsX.RangeEnd = Math.Min(1, zoomCenter + newRange / 2);

            this.ChartArea.ZoomScrollSettingsX.ResumeNotifications();

            ((Telerik.Windows.Controls.DelegateCommand)zoomInCommand).InvalidateCanExecute();
            ((Telerik.Windows.Controls.DelegateCommand)zoomOutCommand).InvalidateCanExecute();
        }

        /// <summary>
        /// Validation Method for Zoom Out button
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns>boolean value</returns>
        public bool CanZoomOut(object parameter)
        {
            if (this.ChartArea == null)
            {
                return false;
            }
            return this.ChartArea.ZoomScrollSettingsX.Range < 1d;
        }
        #endregion

        #region CallbackMethods
        /// <summary>
        /// Plots the series on the chart after getting the resulting collection
        /// </summary>
        /// <param name="result">Contains the Performance Graph Data</param>
        private void RetrievePerformanceGraphDataCallBackMethod(List<PerformanceGraphData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result, 1);
                    PlottedSeries.Clear();
                    PlottedSeries.AddRange(result);
                    if (null != PerformanceGraphDataLoadedEvent)
                    {
                        PerformanceGraphDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
                    }
                }
                else
                {                    
                    Logging.LogMethodParameterNull(logger, methodNamespace, 1);
                    PerformanceGraphDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
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
        /// Data Context Source for chart
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ChartDataBound(object sender, ChartDataBoundEventArgs e)
        {
            ((Telerik.Windows.Controls.DelegateCommand)zoomInCommand).InvalidateCanExecute();
            ((Telerik.Windows.Controls.DelegateCommand)zoomOutCommand).InvalidateCanExecute();
        }
        #endregion

        #region EventUnSubscribe
        /// <summary>
        /// Method that disposes the events
        /// </summary>
        public void Dispose()
        {
            eventAggregator.GetEvent<PortfolioReferenceSetEvent>().Unsubscribe(HandlePortfolioReferenceSet);
            eventAggregator.GetEvent<EffectiveDateReferenceSetEvent>().Unsubscribe(HandleEffectiveDateSet);
            eventAggregator.GetEvent<PeriodReferenceSetEvent>().Unsubscribe(HandlePeriodReferenceSet);
            eventAggregator.GetEvent<HeatMapClickEvent>().Unsubscribe(HandleCountrySelectionDataSet);
        }
        #endregion
    }
}
