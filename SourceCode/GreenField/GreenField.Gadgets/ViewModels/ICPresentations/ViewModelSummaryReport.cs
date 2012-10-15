using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using GreenField.Common;
using GreenField.ServiceCaller;
using GreenField.ServiceCaller.MeetingDefinitions;

namespace GreenField.Gadgets.ViewModels
{
    /// <summary>
    /// View Model for ViewSummaryReport
    /// </summary>
    public class ViewModelSummaryReport : NotificationObject
    {
        #region Fields
        /// <summary>
        /// Region Manager
        /// </summary>
        private IRegionManager regionManager;
       
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
        #endregion

        #region Properties
        #region IsActive
        /// <summary>
        /// IsActive is true when parent control is displayed on UI
        /// </summary>
        private bool isActive;
        public bool IsActive
        {
            get { return isActive; }
            set
            {
                isActive = value;
                if (value)
                {
                    Initialize();
                }
            }
        }
        #endregion

        #region Busy Indicator Notification
        /// <summary>
        /// Displays/Hides busy indicator to notify user of the on going process
        /// </summary>
        private bool isBusyIndicatorBusy = false;
        public bool IsBusyIndicatorBusy
        {
            get { return isBusyIndicatorBusy; }
            set
            {
                isBusyIndicatorBusy = value;
                RaisePropertyChanged(() => this.IsBusyIndicatorBusy);
            }
        }

        /// <summary>
        /// Stores the message displayed over the busy indicator to notify user of the on going process
        /// </summary>
        private string busyIndicatorContent;
        public string BusyIndicatorContent
        {
            get { return busyIndicatorContent; }
            set
            {
                busyIndicatorContent = value;
                RaisePropertyChanged(() => this.BusyIndicatorContent);
            }
        }
        #endregion

        #region Binded
        /// <summary>
        /// Stores summary report information
        /// </summary>
        private List<SummaryReportData> summaryReportInfo;
        public List<SummaryReportData> SummaryReportInfo
        {
            get { return summaryReportInfo; }
            set
            {
                summaryReportInfo = value;
                RaisePropertyChanged(() => this.SummaryReportInfo);
            }
        }

        /// <summary>
        /// Stores selected report start date
        /// </summary>
        private DateTime? selectedStartDate;
        public DateTime? SelectedStartDate
        {
            get { return selectedStartDate; }
            set
            {
                selectedStartDate = value;
                RaisePropertyChanged(() => this.SelectedStartDate);
                RaisePropertyChanged(() => this.SearchCommand);
            }
        }

        /// <summary>
        /// Stores selected report end date
        /// </summary>
        private DateTime? selectedEndDate;
        public DateTime? SelectedEndDate
        {
            get { return selectedEndDate; }
            set
            {
                selectedEndDate = value;
                RaisePropertyChanged(() => this.SelectedEndDate);
                RaisePropertyChanged(() => this.SearchCommand);
            }
        }
        #endregion

        #region ICommand Properties
        /// <summary>
        /// Search Command
        /// </summary>
        public ICommand SearchCommand
        {
            get { return new DelegateCommand<object>(SearchCommandMethod, SearchCommandValidationMethod); }
        }
        #endregion
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="param">DashboardGadgetParam</param>
        public ViewModelSummaryReport(DashboardGadgetParam param)
        {
            this.dbInteractivity = param.DBInteractivity;
            this.logger = param.LoggerFacade;
            this.eventAggregator = param.EventAggregator;
            this.regionManager = param.RegionManager;
        }
        #endregion        

        #region ICommand Methods
        /// <summary>
        /// SearchCommand validation method
        /// </summary>
        /// <param name="param"></param>
        /// <returns>True/False</returns>
        private bool SearchCommandValidationMethod(object param)
        {
            return SelectedStartDate != null && SelectedEndDate != null;
        }

        /// <summary>
        /// SearchCommand execution method
        /// </summary>
        /// <param name="param"></param>
        private void SearchCommandMethod(object param)
        {
            Initialize();
        }        
        #endregion

        #region CallBack Methods
        /// <summary>
        /// RetrieveSummaryReportData Callback Method
        /// </summary>
        /// <param name="result">SummaryReportData</param>
        private void RetrieveSummaryReportDataCallbackMethod(List<SummaryReportData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result, 1);
                    SummaryReportInfo = result;
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
                Logging.LogEndMethod(logger, methodNamespace);
                BusyIndicatorNotification();
            }
        }
        #endregion
        
        #region Helper Methods
        /// <summary>
        /// Initializes view
        /// </summary>
        public void Initialize()
        {
            if (dbInteractivity != null && IsActive && SelectedStartDate != null && SelectedEndDate != null)
            {
                BusyIndicatorNotification(true, "Retrieving report details...");
                dbInteractivity.RetrieveSummaryReportData(Convert.ToDateTime(SelectedStartDate), Convert.ToDateTime(SelectedEndDate)
                    , RetrieveSummaryReportDataCallbackMethod);
            }
        }

        /// <summary>
        /// Display/Hide Busy Indicator
        /// </summary>
        /// <param name="isBusyIndicatorVisible">True to display indicator; default false</param>
        /// <param name="message">Content message for indicator; default null</param>
        private void BusyIndicatorNotification(bool isBusyIndicatorVisible = false, String message = null)
        {
            if (message != null)
            {
                BusyIndicatorContent = message;
            }
            IsBusyIndicatorBusy = isBusyIndicatorVisible;
        }
        #endregion        

        #region EventUnSubscribe
        /// <summary>
        /// Method that disposes the events
        /// </summary>
        public void Dispose()
        {           
        }
        #endregion           
    }
}
