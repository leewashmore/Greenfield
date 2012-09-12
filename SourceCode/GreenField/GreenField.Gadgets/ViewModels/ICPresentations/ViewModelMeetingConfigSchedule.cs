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
//using System.Configuration;



namespace GreenField.Gadgets.ViewModels
{
    public class ViewModelMeetingConfigSchedule : NotificationObject
    {

        #region Fields
        private Boolean calculationFlag = false;
        private DateTime SUNDAY_UTC = new DateTime(2012, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private DateTime MONDAY_UTC = new DateTime(2012, 1, 2, 0, 0, 0, DateTimeKind.Utc);
        private DateTime TUESDAY_UTC = new DateTime(2012, 1, 3, 0, 0, 0, DateTimeKind.Utc);
        private DateTime WEDNESDAY_UTC = new DateTime(2012, 1, 4, 0, 0, 0, DateTimeKind.Utc);
        private DateTime THURSDAY_UTC = new DateTime(2012, 1, 5, 0, 0, 0, DateTimeKind.Utc);
        private DateTime FRIDAY_UTC = new DateTime(2012, 1, 6, 0, 0, 0, DateTimeKind.Utc);
        private DateTime SATURDAY_UTC = new DateTime(2012, 1, 7, 0, 0, 0, DateTimeKind.Utc);

        private DateTime SUNDAY_LOCAL = new DateTime(2012, 1, 1, 0, 0, 0, DateTimeKind.Local);
        private DateTime MONDAY_LOCAL = new DateTime(2012, 1, 2, 0, 0, 0, DateTimeKind.Local);
        private DateTime TUESDAY_LOCAL = new DateTime(2012, 1, 3, 0, 0, 0, DateTimeKind.Local);
        private DateTime WEDNESDAY_LOCAL = new DateTime(2012, 1, 4, 0, 0, 0, DateTimeKind.Local);
        private DateTime THURSDAY_LOCAL = new DateTime(2012, 1, 5, 0, 0, 0, DateTimeKind.Local);
        private DateTime FRIDAY_LOCAL = new DateTime(2012, 1, 6, 0, 0, 0, DateTimeKind.Local);
        private DateTime SATURDAY_LOCAL = new DateTime(2012, 1, 7, 0, 0, 0, DateTimeKind.Local);

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
        private ILoggerFacade _logger;

        /// <summary>
        /// IsActive is true when parent control is displayed on UI
        /// </summary>
        private bool _isActive;
        public bool IsActive
        {
            get
            {
                return _isActive;
            }
            set
            {
                _isActive = value;
                if (value)
                {
                    if (_dbInteractivity != null)
                    {
                        BusyIndicatorNotification(true, "Retrieving Investment Committee Meeting configuration details...");
                        _dbInteractivity.GetMeetingConfigSchedule(GetMeetingConfigScheduleCallbackMethod);
                    }
                }
            }
        }
        #endregion

        #region Constructor
        public ViewModelMeetingConfigSchedule(DashboardGadgetParam param)
        {

            _dbInteractivity = param.DBInteractivity;
            _logger = param.LoggerFacade;
            _eventAggregator = param.EventAggregator;

            while (SUNDAY_UTC.DayOfWeek != DayOfWeek.Sunday)
            {
                SUNDAY_UTC = SUNDAY_UTC.AddDays(1);
                MONDAY_UTC = MONDAY_UTC.AddDays(1);
                TUESDAY_UTC = TUESDAY_UTC.AddDays(1);
                WEDNESDAY_UTC = WEDNESDAY_UTC.AddDays(1);
                THURSDAY_UTC = THURSDAY_UTC.AddDays(1);
                FRIDAY_UTC = FRIDAY_UTC.AddDays(1);
                SATURDAY_UTC = SATURDAY_UTC.AddDays(1);
            }

            while (SUNDAY_LOCAL.DayOfWeek != DayOfWeek.Sunday)
            {
                SUNDAY_LOCAL = SUNDAY_LOCAL.AddDays(1);
                MONDAY_LOCAL = MONDAY_LOCAL.AddDays(1);
                TUESDAY_LOCAL = TUESDAY_LOCAL.AddDays(1);
                WEDNESDAY_LOCAL = WEDNESDAY_LOCAL.AddDays(1);
                THURSDAY_LOCAL = THURSDAY_LOCAL.AddDays(1);
                FRIDAY_LOCAL = FRIDAY_LOCAL.AddDays(1);
                SATURDAY_LOCAL = SATURDAY_LOCAL.AddDays(1);
            }
        }

        #endregion

        #region Properties
        #region Binded Properties

        public List<String> PresentationDay
        {
            get
            {
                List<String> _presentationDay = new List<String>();
                _presentationDay.Add(DayOfWeek.Monday.ToString());
                _presentationDay.Add(DayOfWeek.Tuesday.ToString());
                _presentationDay.Add(DayOfWeek.Wednesday.ToString());
                _presentationDay.Add(DayOfWeek.Thursday.ToString());
                _presentationDay.Add(DayOfWeek.Friday.ToString());
                _presentationDay.Add(DayOfWeek.Saturday.ToString());
                _presentationDay.Add(DayOfWeek.Sunday.ToString());
                return _presentationDay;
            }
        }

        private String _selectedPresentationDay;
        public String SelectedPresentationDay
        {
            get { return _selectedPresentationDay; }
            set
            {
                _selectedPresentationDay = value;
                RaisePropertyChanged(() => this.SelectedPresentationDay);
                if (calculationFlag && SelectedPresentationDateTime != null && value != null)
                {
                    SelectedPresentationDateTime = GetDateTimeFromDayTime(value, Convert.ToDateTime(SelectedPresentationDateTime), false);
                }
            }
        }

        private DateTime? _selectedPresentationDateTime;
        public DateTime? SelectedPresentationDateTime
        {
            get { return _selectedPresentationDateTime; }
            set
            {
                if (value != null)
                {
                    _selectedPresentationDateTime = value;
                    RaisePropertyChanged(() => this.SelectedPresentationDateTime);

                    if (calculationFlag && SelectedPresentationDay != null)
                    {
                        CalculateDeadlines(SelectedPresentationDay, Convert.ToDateTime(value));
                    }
                }                
            }
        }

        private float _configPresentationDeadline;
        public float ConfigPresentationDeadline
        {
            get { return _configPresentationDeadline; }
            set
            {
                _configPresentationDeadline = value;
            }
        }

        private float _configPreMeetingVotingDeadLine;
        public float ConfigPreMeetingVotingDeadLine
        {
            get { return _configPreMeetingVotingDeadLine; }
            set
            {
                _configPreMeetingVotingDeadLine = value;
            }
        }

        private DateTime _presentationDeadline;
        public DateTime PresentationDeadline
        {
            get { return _presentationDeadline; }
            set
            {
                _presentationDeadline = value;
                RaisePropertyChanged(() => this.PresentationDeadline);
            }
        }

        private DateTime _preMeetingVotingDeadline;
        public DateTime PreMeetingVotingDeadline
        {
            get { return _preMeetingVotingDeadline; }
            set
            {
                _preMeetingVotingDeadline = value;
                RaisePropertyChanged(() => this.PreMeetingVotingDeadline);
            }
        }

        #region Busy Indicator
        /// <summary>
        /// Busy Indicator Status
        /// </summary>
        private bool _busyIndicatorIsBusy;
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
        /// Busy Indicator Content
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

        #endregion

        #region ICommand Properties

        public ICommand SubmitCommand
        {
            get { return new DelegateCommand<object>(MeetingConfigurationSaveItem); }
        }
        #endregion


        #endregion

        #region ICommand Methods

        private void MeetingConfigurationSaveItem(object param)
        {
            MeetingConfigurationSchedule config = new MeetingConfigurationSchedule();
            DateTime presentationDateTime = Convert.ToDateTime(SelectedPresentationDateTime);
            config.PresentationDay = GetDateTimeFromDayTime(presentationDateTime.DayOfWeek.ToString(), presentationDateTime, false).ToUniversalTime().DayOfWeek.ToString();
            config.PresentationTime = GetDateTimeFromDayTime(presentationDateTime.DayOfWeek.ToString(), presentationDateTime, false).ToUniversalTime();
            config.PresentationTimeZone = "UTC";//SelectedTimeZone;
            config.PresentationDeadlineDay = GetDateTimeFromDayTime(PresentationDeadline.DayOfWeek.ToString(), PresentationDeadline, false).ToUniversalTime().DayOfWeek.ToString();
            config.PresentationDeadlineTime = GetDateTimeFromDayTime(PresentationDeadline.DayOfWeek.ToString(), PresentationDeadline, false).ToUniversalTime();
            config.PreMeetingVotingDeadlineDay = GetDateTimeFromDayTime(PreMeetingVotingDeadline.DayOfWeek.ToString(), PreMeetingVotingDeadline, false).ToUniversalTime().DayOfWeek.ToString();
            config.PreMeetingVotingDeadlineTime = GetDateTimeFromDayTime(PreMeetingVotingDeadline.DayOfWeek.ToString(), PreMeetingVotingDeadline, false).ToUniversalTime();
            config.CreatedBy = "System";
            config.CreatedOn = DateTime.UtcNow;
            config.ModifiedBy = "System";
            config.ModifiedOn = DateTime.UtcNow;

            // _dbInteractivity.CreateMeetingConfigSchedule(config, (msg) => { MessageBox.Show(msg); });
            if (_dbInteractivity != null)
            {
                BusyIndicatorNotification(true, "Updating Investment Committee Meeting Schedule...");
                _dbInteractivity.UpdateMeetingConfigSchedule(UserSession.SessionManager.SESSION.UserName, config, UpdateMeetingConfigCallback);
            }
        }

        

        #endregion

        #region Helper Methods

        private void SelectionRaisePropertyChanged()
        {
            RaisePropertyChanged(() => this.SubmitCommand);
        }

        private void CalculateDeadlines(String presentationday, DateTime presentationTime)
        {
            DateTime presentationDateTime = GetDateTimeFromDayTime(presentationday, presentationTime, false);

            PresentationDeadline = CalculateDeadlines(Convert.ToDateTime(SelectedPresentationDateTime), ConfigPresentationDeadline);
            PreMeetingVotingDeadline = CalculateDeadlines(Convert.ToDateTime(SelectedPresentationDateTime), ConfigPreMeetingVotingDeadLine);
        }

        private DateTime CalculateDeadlines(DateTime dt, float duration)
        {
            TimeSpan tDays = new TimeSpan(Convert.ToInt16(duration), Convert.ToInt16((duration - Convert.ToInt16(duration)) * 60), 0);

            DateTime newTime = dt - tDays;
            DateTime counter = newTime;

            int offs = 0;
            while (counter.Date <= dt.Date)
            {
                if (counter.DayOfWeek.Equals(DayOfWeek.Saturday) || counter.DayOfWeek.Equals(DayOfWeek.Sunday))
                {
                    offs++;
                }

                counter = counter.AddDays(1);
            }

            newTime = newTime.AddDays(0 - offs);
            if (newTime.DayOfWeek.Equals(DayOfWeek.Saturday))
            {
                newTime = newTime.AddDays(-1);

            }
            else if (newTime.DayOfWeek.Equals(DayOfWeek.Sunday))
            {
                newTime = newTime.AddDays(-2);
            }

            return newTime;
        }

        /// <summary>
        /// Busy Indicator Notification
        /// </summary>
        /// <param name="showBusyIndicator"></param>
        /// <param name="message"></param>
        public void BusyIndicatorNotification(bool showBusyIndicator = false, String message = null)
        {
            if (message != null)
                BusyIndicatorContent = message;
            BusyIndicatorIsBusy = showBusyIndicator;
        }

        private DateTime GetDateTimeFromDayTime(String day, DateTime time, Boolean IsUTC = true)
        {
            DateTime result = DateTime.UtcNow;
            switch (day)
            {
                case "Friday":
                    result = IsUTC ? FRIDAY_UTC.Add(time.TimeOfDay) : FRIDAY_LOCAL.Add(time.TimeOfDay);
                    break;
                case "Monday":
                    result = IsUTC ? MONDAY_UTC.Add(time.TimeOfDay) : MONDAY_LOCAL.Add(time.TimeOfDay);
                    break;
                case "Saturday":
                    result = IsUTC ? SATURDAY_UTC.Add(time.TimeOfDay) : SATURDAY_LOCAL.Add(time.TimeOfDay);
                    break;
                case "Sunday":
                    result = IsUTC ? SUNDAY_UTC.Add(time.TimeOfDay) : SUNDAY_LOCAL.Add(time.TimeOfDay);
                    break;
                case "Thursday":
                    result = IsUTC ? THURSDAY_UTC.Add(time.TimeOfDay) : THURSDAY_LOCAL.Add(time.TimeOfDay);
                    break;
                case "Tuesday":
                    result = IsUTC ? TUESDAY_UTC.AddHours(time.Hour).AddMinutes(time.Minute).AddSeconds(time.Second)
                        : TUESDAY_LOCAL.AddHours(time.Hour).AddMinutes(time.Minute).AddSeconds(time.Second);
                    break;
                case "Wednesday":
                    result = IsUTC ? WEDNESDAY_UTC.Add(time.TimeOfDay) : WEDNESDAY_LOCAL.Add(time.TimeOfDay);
                    break;
                default:
                    break;
            }

            return result;
        }
        #endregion

        #region CallBack Methods
        private void GetMeetingConfigScheduleCallbackMethod(MeetingConfigurationSchedule val)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (val != null)
                {
                    #region Get Local DateTimes
                    String presentationDay = val.PresentationDay;
                    DateTime presentationTime = val.PresentationTime;
                    DateTime presentationDateTime = GetDateTimeFromDayTime(presentationDay, presentationTime).ToLocalTime();

                    String preMeetingVotingDeadlineDay = val.PreMeetingVotingDeadlineDay;
                    DateTime preMeetingVotingDeadlineTime = val.PreMeetingVotingDeadlineTime;
                    DateTime preMeetingVotingDeadlineDateTime = GetDateTimeFromDayTime(preMeetingVotingDeadlineDay, preMeetingVotingDeadlineTime).ToLocalTime();

                    String presentationDeadlineDay = val.PresentationDeadlineDay;
                    DateTime presentationDeadlineTime = val.PresentationDeadlineTime;
                    DateTime presentationDeadlineDateTime = GetDateTimeFromDayTime(presentationDeadlineDay, presentationDeadlineTime).ToLocalTime(); 
                    #endregion

                    SelectedPresentationDay = presentationDateTime.DayOfWeek.ToString();
                    SelectedPresentationDateTime = presentationDateTime;

                    //SelectedTimeZone = val.PresentationTimeZone;
                    
                    ConfigPresentationDeadline = (float)val.ConfigurablePresentationDeadline;
                    ConfigPreMeetingVotingDeadLine = (float)val.ConfigurablePreMeetingVotingDeadline;
                    calculationFlag = true;

                    CalculateDeadlines(SelectedPresentationDay, Convert.ToDateTime(SelectedPresentationDateTime));
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

        private void UpdateMeetingConfigCallback(Boolean? result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    Prompt.ShowDialog("Investment Committee Schedule has been successfully updated");
                }
                else
                {
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                    Prompt.ShowDialog("An error occurred while updating Investment Committee Schedule");                    
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
