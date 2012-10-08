using System;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Logging;
using Telerik.Windows.Controls.Charting;
using Telerik.Windows.Controls;
using GreenField.ServiceCaller;
using GreenField.ServiceCaller.SecurityReferenceDefinitions;
using GreenField.Common;
using GreenField.Gadgets.Helpers;
using GreenField.Common.Helper;
using GreenField.DataContracts;

namespace GreenField.Gadgets.ViewModels
{
    /// <summary>
    /// Class that provides the interaction of the view model with the Service caller and the View.
    /// </summary>
    public class ViewModelUnrealizedGainLoss : NotificationObject
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
        /// private member object of the EntitySelectionData class for storing Entity Selection Data
        /// </summary>
        private EntitySelectionData entitySelectionData; 
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor of the class that initializes various objects
        /// </summary>
        /// <param name="param">MEF Eventaggrigator instance</param>
        public ViewModelUnrealizedGainLoss(DashboardGadgetParam param)
        {
            dbInteractivity = param.DBInteractivity;
            logger = param.LoggerFacade;
            eventAggregator = param.EventAggregator;
            entitySelectionData = param.DashboardGadgetPayload.EntitySelectionData;
            eventAggregator.GetEvent<SecurityReferenceSetEvent>().Subscribe(HandleSecurityReferenceSet, false);
            if (entitySelectionData != null)
            {
                if (null != unrealizedGainLossDataLoadedEvent)
                {
                    unrealizedGainLossDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
                }
                RetrieveUnrealizedGainLossData(entitySelectionData);
            }
        }
        #endregion

        #region Properties
        #region UI Fields
        /// <summary>
        /// Collection binded to the Comparison chart - consists of unrealized gain loss,closing price and Closing date for the primary entity
        /// </summary>
        private RangeObservableCollection<UnrealizedGainLossData> plottedSeries;
        public RangeObservableCollection<UnrealizedGainLossData> PlottedSeries
        {
            get
            {
                if (plottedSeries == null)
                    plottedSeries = new RangeObservableCollection<UnrealizedGainLossData>();
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
        /// Property that is binded to the security selected by the user
        /// </summary>
        private string selectedSecurityName = "";
        public string PlottedSecurityName
        {
            get
            {
                return selectedSecurityName;
            }
            set
            {
                selectedSecurityName = value;
                this.RaisePropertyChanged(() => this.PlottedSecurityName);
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
                if (entitySelectionData != null && isActive)
                    RetrieveUnrealizedGainLossData(entitySelectionData);
            }
        }


        #region Time Period Selection and Frequency Selection
        /// <summary>
        /// Collection of Time Range options
        /// </summary>        
        public ObservableCollection<String> TimeRange
        {
            get
            {
                return new ObservableCollection<string> { "1-Month", "2-Months", "3-Months", "6-Months","YTD", "9-Months", "1-Year", "2-Years", 
                    "3-Years"};
            }
        }

        /// <summary>
        /// Collection of Frequency Range Options
        /// </summary>        
        public ObservableCollection<String> FrequencyRange
        {
            get
            {
                return new ObservableCollection<string> { "Daily", "Weekly", "Monthly", "Quarterly", "Yearly" };
            }
        }

        /// <summary>
        /// Property to be updated on Selection of Time Range option
        /// </summary>
        private string selectedTimeRange = "1-Year";
        public string SelectedTimeRange
        {
            get { return selectedTimeRange; }
            set
            {
                if (selectedTimeRange != value)
                {
                    selectedTimeRange = value;
                    if (entitySelectionData != null)
                        RetrieveUnrealizedGainLossData(entitySelectionData);
                    RaisePropertyChanged(() => this.SelectedTimeRange);
                }
            }
        }

        /// <summary>
        /// Property to be updated on Selection of Frequency Range Option 
        /// </summary>
        private string selectedFrequencyRange = "Daily";
        public string SelectedFrequencyRange
        {
            get { return selectedFrequencyRange; }
            set
            {
                if (selectedFrequencyRange != value)
                {
                    selectedFrequencyRange = value;
                    if (entitySelectionData != null)
                        RetrieveUnrealizedGainLossData(entitySelectionData);
                    RaisePropertyChanged(() => this.SelectedFrequencyRange);
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


        #endregion

        #endregion

        #region CallBack Methods        

        /// <summary>
        /// Method that calls the service Method through propertyName call to Service Caller
        /// </summary>
        /// <param name="Ticker">Unique Identifier for propertyName security</param>
        /// <param name="callback">Callback for this method</param>
        private void RetrieveUnrealizedGainLossData(EntitySelectionData entitySelectionData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (entitySelectionData != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, entitySelectionData, 1);
                    PlottedSecurityName = entitySelectionData.LongName;
                    DateTime periodStartDate;
                    DateTime periodEndDate;
                    GetPeriod(out periodStartDate, out periodEndDate);
                    if (IsActive)
                    {
                        if (null != unrealizedGainLossDataLoadedEvent)
                        {
                            unrealizedGainLossDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = true });
                        }
                        dbInteractivity.RetrieveUnrealizedGainLossData(entitySelectionData, periodStartDate, periodEndDate, SelectedFrequencyRange, RetrieveUnrealizedGainLossDataCallBackMethod);
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
        /// Method that calculates the Start Date and End Date time  
        /// </summary>
        /// <param name="startDate">returns the start date</param>
        /// <param name="endDate">returns the end date</param>
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
                case "YTD":
                    startDate = new DateTime(DateTime.Today.Year, 1, 1);
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
                    zoomInCommand = new DelegateCommand(ZoomIn, CanZoomIn);
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
                    zoomOutCommand = new DelegateCommand(ZoomOut, CanZoomOut);
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
        public event DataRetrievalProgressIndicatorEventHandler unrealizedGainLossDataLoadedEvent;
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
            ((DelegateCommand)zoomInCommand).InvalidateCanExecute();
            ((DelegateCommand)zoomOutCommand).InvalidateCanExecute();
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
            {
                zoomCenter = newRange / 2;
            }
            this.ChartArea.ZoomScrollSettingsX.RangeStart = Math.Max(0, zoomCenter - newRange / 2);
            this.ChartArea.ZoomScrollSettingsX.RangeEnd = Math.Min(1, zoomCenter + newRange / 2);
            this.ChartArea.ZoomScrollSettingsX.ResumeNotifications();
            ((DelegateCommand)zoomInCommand).InvalidateCanExecute();
            ((DelegateCommand)zoomOutCommand).InvalidateCanExecute();
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

        #region Event Handlers
        /// <summary>
        /// Assigns UI Field Properties based on Security reference
        /// </summary>
        /// <param name="securityReferenceData">entitySelectionData</param>
        public void HandleSecurityReferenceSet(EntitySelectionData entitySelectionData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (entitySelectionData != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, entitySelectionData, 1);
                  //  _entitySelectionData = entitySelectionData;
                    RetrieveUnrealizedGainLossData(entitySelectionData);
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
        #endregion

        #region CallbackMethods
        /// <summary>
        /// Plots the series on the chart after getting the resulting collection
        /// </summary>
        /// <param name="result">Contains the Unrealized Gain Loss Data</param>
        private void RetrieveUnrealizedGainLossDataCallBackMethod(List<UnrealizedGainLossData> result)
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
                    if (null != unrealizedGainLossDataLoadedEvent)
                        unrealizedGainLossDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
                }
                else
                {
                    Logging.LogMethodParameterNull(logger, methodNamespace, 1);
                    unrealizedGainLossDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
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
            ((DelegateCommand)zoomInCommand).InvalidateCanExecute();
            ((DelegateCommand)zoomOutCommand).InvalidateCanExecute();
        }
        #endregion

        #region EventUnSubscribe
        /// <summary>
        /// disposing events
        /// </summary>
        public void Dispose()
        {
            eventAggregator.GetEvent<SecurityReferenceSetEvent>().Unsubscribe(HandleSecurityReferenceSet);
        }
        #endregion
    }
}