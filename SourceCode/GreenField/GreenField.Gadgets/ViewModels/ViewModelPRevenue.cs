using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.ViewModel;
using Telerik.Windows.Controls.Charting;
using GreenField.Common;
using GreenField.DataContracts;
using GreenField.DataContracts.DataContracts;
using GreenField.Gadgets.Helpers;
using GreenField.ServiceCaller;



namespace GreenField.Gadgets.ViewModels
{
    public class ViewModelPRevenue : NotificationObject
    {
        #region Fields

        //MEF Singletons

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
        private ILoggerFacade logger;

        /// <summary>
        /// Details of selected Security
        /// </summary>
        private EntitySelectionData securitySelectionData;

        /// <summary>
        /// Stores Chart data
        /// </summary>
        private RangeObservableCollection<PRevenueData> pRevenuePlottedData;

        /// <summary>
        /// Stores chart title
        /// </summary>
        private string chartTitle = "P/Revenue";

        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="eventAggregator">MEF Eventaggregator instance</param>
        public ViewModelPRevenue(DashboardGadgetParam param)
        {
            eventAggregator = param.EventAggregator;
            dbInteractivity = param.DBInteractivity;
            logger = param.LoggerFacade;
            securitySelectionData = param.DashboardGadgetPayload.EntitySelectionData;
            if (eventAggregator != null)
                eventAggregator.GetEvent<SecurityReferenceSetEvent>().Subscribe(HandleSecurityReferenceSet);
            if (securitySelectionData != null)
                HandleSecurityReferenceSet(securitySelectionData);
        }
        #endregion

        #region PROPERTIES

        /// <summary>
        /// Details of Selected Security
        /// </summary>
        public EntitySelectionData SelectedSecurity
        {
            get
            {
                return securitySelectionData;
            }
            set
            {
                securitySelectionData = value;
                this.RaisePropertyChanged(() => this.SelectedSecurity);
            }
        }

        public RangeObservableCollection<PRevenueData> PRevenuePlottedData
        {
            get
            {
                if (pRevenuePlottedData == null)
                    pRevenuePlottedData = new RangeObservableCollection<PRevenueData>();
                return pRevenuePlottedData;
            }
            set
            {
                pRevenuePlottedData = value;
                RaisePropertyChanged(() => this.PRevenuePlottedData);
            }

        }
        /// <summary>
        /// ChartArea property bound to ChartArea of dgPRevenue 
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
        }/// <summary>
        /// <summary>
        /// Minimum Value for X-Axis of Chart
        /// </summary>
        private decimal axisXMinValue;
        public decimal AxisXMinValue
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
        private decimal axisXMaxValue;
        public decimal AxisXMaxValue
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
                CallingWebMethod();
            }
        }


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

        #region ICommand
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
            if (this.ChartArea == null )
                return false;

            return this.ChartArea.ZoomScrollSettingsX.Range < 1d ;
        }

        #endregion

        #region Helper Methods

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


        #endregion
        #region EVENTHANDLERS
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
                securitySelectionData = entitySelectionData;
                if (entitySelectionData != null && IsActive)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, entitySelectionData, 1);                    
                    CallingWebMethod();
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
        /// Checking the status of Chart, whether zoom can be executed or not
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ChartDataBound(object sender, ChartDataBoundEventArgs e)
        {
            ((Telerik.Windows.Controls.DelegateCommand)zoomInCommand).InvalidateCanExecute();
            ((Telerik.Windows.Controls.DelegateCommand)zoomOutCommand).InvalidateCanExecute();
        }

        #endregion

        #region CALLBACK METHOD
        /// <summary>
        /// Callback method that assigns value to the BAsicDataInfo property
        /// </summary>
        /// <param name="result">basic data </param>
        private void RetrievePRevenueDataCallbackMethod(List<PRevenueData> pRevenueData)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (pRevenueData != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, pRevenueData, 1);
                    //PRevenuePlottedData = new RangeObservableCollection<PRevenueData>(pRevenueData);
                    PRevenuePlottedData.Clear();
                    PRevenuePlottedData.AddRange(pRevenueData.ToList());
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
            finally { BusyIndicatorStatus = false; }
            Logging.LogEndMethod(logger, methodNamespace);
        }

        #endregion

        #region WEB SERVICE CALL
        private void CallingWebMethod()
        {
            if (securitySelectionData != null && IsActive)
            {
                dbInteractivity.RetrievePRevenueData(securitySelectionData, chartTitle, RetrievePRevenueDataCallbackMethod);
                BusyIndicatorStatus = true;
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
                eventAggregator.GetEvent<SecurityReferenceSetEvent>().Unsubscribe(HandleSecurityReferenceSet);

            }
        }

        #endregion
    }
}

