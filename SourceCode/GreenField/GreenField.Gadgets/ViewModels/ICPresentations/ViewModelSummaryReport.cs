using System;
using System.Net;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel.Composition;
using GreenField.ServiceCaller;
using Microsoft.Practices.Prism.ViewModel;
using GreenField.ServiceCaller.MeetingDefinitions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Commands;
using GreenField.Gadgets.Models;
using Microsoft.Practices.Prism.Regions;
using GreenField.Common;
using GreenField.Gadgets.Views;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;
using GreenField.Gadgets.Helpers;
using GreenField.DataContracts;

namespace GreenField.Gadgets.ViewModels
{
    public class ViewModelSummaryReport : NotificationObject
    {
        #region Fields
        private IRegionManager _regionManager;
       // private ManageMeetings _manageMeetings;
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
        public ILoggerFacade _logger;        
        #endregion

        #region Constructor
        public ViewModelSummaryReport(DashboardGadgetParam param)
        {
            _dbInteractivity = param.DBInteractivity;
            _logger = param.LoggerFacade;
            _eventAggregator = param.EventAggregator;
            _regionManager = param.RegionManager;
        }
        #endregion

        #region Properties
        #region IsActive
        /// <summary>
        /// IsActive is true when parent control is displayed on UI
        /// </summary>
        private bool _isActive;
        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
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
        private bool _busyIndicatorIsBusy = false;
        public bool BusyIndicatorIsBusy
        {
            get { return _busyIndicatorIsBusy; }
            set
            {
                _busyIndicatorIsBusy = value;
                RaisePropertyChanged(() => this.BusyIndicatorIsBusy);
            }
        }

        /// <summary>
        /// Stores the message displayed over the busy indicator to notify user of the on going process
        /// </summary>
        private string _busyIndicatorContent;
        public string BusyIndicatorContent
        {
            get { return _busyIndicatorContent; }
            set
            {
                _busyIndicatorContent = value;
                RaisePropertyChanged(() => this.BusyIndicatorContent);
            }
        }
        #endregion        

        #region Binded
        private List<SummaryReportData> _summaryReportInfo;
        public List<SummaryReportData> SummaryReportInfo
        {
            get { return _summaryReportInfo; }
            set
            {
                _summaryReportInfo = value;
                RaisePropertyChanged(() => this.SummaryReportInfo);
            }
        }

        private DateTime? _selectedStartDate;
        public DateTime? SelectedStartDate
        {
            get { return _selectedStartDate; }
            set
            {
                _selectedStartDate = value;
                RaisePropertyChanged(() => this.SelectedStartDate);
                RaisePropertyChanged(() => this.SearchCommand);
            }
        }

        private DateTime? _selectedEndDate;
        public DateTime? SelectedEndDate
        {
            get { return _selectedEndDate; }
            set
            {
                _selectedEndDate = value;
                RaisePropertyChanged(() => this.SelectedEndDate);
                RaisePropertyChanged(() => this.SearchCommand);
            }
        }        
        #endregion

        #region ICommand Properties
        public ICommand SearchCommand
        {
            get { return new DelegateCommand<object>(SearchCommandMethod, SearchCommandValidationMethod); }
        }        
        #endregion        

        #endregion

        #region ICommand Methods
        private bool SearchCommandValidationMethod(object param)
        {
            return SelectedStartDate != null && SelectedEndDate != null;
        }

        private void SearchCommandMethod(object param)
        {
            Initialize();
        }        
        #endregion
        
        #region Helper Methods

        public void Initialize()
        {
            if (_dbInteractivity != null && IsActive && SelectedStartDate != null && SelectedEndDate != null)
            {
                BusyIndicatorNotification(true, "Retrieving report details...");
                _dbInteractivity.RetrieveSummaryReportData(Convert.ToDateTime(SelectedStartDate), Convert.ToDateTime(SelectedEndDate)
                    , RetrieveSummaryReportDataCallbackMethod);
            }
        }       
        

        public void BusyIndicatorNotification(bool showBusyIndicator = false, String message = null)
        {
            if (message != null)
                BusyIndicatorContent = message;

            BusyIndicatorIsBusy = showBusyIndicator;
        }

        #endregion

        #region CallBack Methods
        private void RetrieveSummaryReportDataCallbackMethod(List<SummaryReportData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    SummaryReportInfo = result;                    
                }
                else
                {
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);                    
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            finally
            {
                Logging.LogEndMethod(_logger, methodNamespace);
                BusyIndicatorNotification();
            }            
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
