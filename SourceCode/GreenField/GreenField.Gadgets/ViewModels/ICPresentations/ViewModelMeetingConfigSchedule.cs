using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.ViewModel;
using GreenField.Common;
using GreenField.ServiceCaller;
using GreenField.ServiceCaller.MeetingDefinitions;

namespace GreenField.Gadgets.ViewModels
{
    /// <summary>
    /// View Model for ViewMeetingConfigSchedule
    /// </summary>
    public class ViewModelMeetingConfigSchedule : NotificationObject
    {
        #region Fields
        /// <summary>
        /// True if calculation is complete
        /// </summary>
        private Boolean isCalculated = false;

        /// <summary>
        /// Custom utc datetimes for weekdays
        /// </summary>
        private DateTime SUNDAY_UTC = new DateTime(2012, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private DateTime MONDAY_UTC = new DateTime(2012, 1, 2, 0, 0, 0, DateTimeKind.Utc);
        private DateTime TUESDAY_UTC = new DateTime(2012, 1, 3, 0, 0, 0, DateTimeKind.Utc);
        private DateTime WEDNESDAY_UTC = new DateTime(2012, 1, 4, 0, 0, 0, DateTimeKind.Utc);
        private DateTime THURSDAY_UTC = new DateTime(2012, 1, 5, 0, 0, 0, DateTimeKind.Utc);
        private DateTime FRIDAY_UTC = new DateTime(2012, 1, 6, 0, 0, 0, DateTimeKind.Utc);
        private DateTime SATURDAY_UTC = new DateTime(2012, 1, 7, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Custom local datetime for weekdays
        /// </summary>
        private DateTime SUNDAY_LOCAL = new DateTime(2012, 1, 1, 0, 0, 0, DateTimeKind.Local);
        private DateTime MONDAY_LOCAL = new DateTime(2012, 1, 2, 0, 0, 0, DateTimeKind.Local);
        private DateTime TUESDAY_LOCAL = new DateTime(2012, 1, 3, 0, 0, 0, DateTimeKind.Local);
        private DateTime WEDNESDAY_LOCAL = new DateTime(2012, 1, 4, 0, 0, 0, DateTimeKind.Local);
        private DateTime THURSDAY_LOCAL = new DateTime(2012, 1, 5, 0, 0, 0, DateTimeKind.Local);
        private DateTime FRIDAY_LOCAL = new DateTime(2012, 1, 6, 0, 0, 0, DateTimeKind.Local);
        private DateTime SATURDAY_LOCAL = new DateTime(2012, 1, 7, 0, 0, 0, DateTimeKind.Local);
        
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
                    if (dbInteractivity != null)
                    {
                        BusyIndicatorNotification(true, "Retrieving Investment Committee Meeting configuration details...");
                        dbInteractivity.GetMeetingConfigSchedule(GetMeetingConfigScheduleCallbackMethod);
                    }
                }
            }
        } 
        #endregion

        #region Binded Properties
        /// <summary>
        /// Reference Presentaion Days
        /// </summary>
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

        /// <summary>
        /// Selected presentation day
        /// </summary>
        private String selectedPresentationDay;
        public String SelectedPresentationDay
        {
            get { return selectedPresentationDay; }
            set
            {
                selectedPresentationDay = value;
                RaisePropertyChanged(() => this.SelectedPresentationDay);
                if (isCalculated && SelectedPresentationDateTime != null && value != null)
                {
                    SelectedPresentationDateTime = GetDateTimeFromDayTime(value, Convert.ToDateTime(SelectedPresentationDateTime), false);
                }
            }
        }

        /// <summary>
        /// Selected presentation datetime
        /// </summary>
        private DateTime? selectedPresentationDateTime;
        public DateTime? SelectedPresentationDateTime
        {
            get { return selectedPresentationDateTime; }
            set
            {
                if (value != null)
                {
                    selectedPresentationDateTime = value;
                    RaisePropertyChanged(() => this.SelectedPresentationDateTime);
                    if (isCalculated && SelectedPresentationDay != null)
                    {
                        CalculateDeadlines(SelectedPresentationDay, Convert.ToDateTime(value));
                    }
                }
            }
        }

        /// <summary>
        /// Configured presentation deadline
        /// </summary>
        private float configPresentationDeadline;
        public float ConfigPresentationDeadline
        {
            get { return configPresentationDeadline; }
            set { configPresentationDeadline = value; }
        }

        /// <summary>
        /// Configured pre meeting voting deadline
        /// </summary>
        private float configPreMeetingVotingDeadLine;
        public float ConfigPreMeetingVotingDeadLine
        {
            get { return configPreMeetingVotingDeadLine; }
            set { configPreMeetingVotingDeadLine = value; }
        }

        /// <summary>
        /// Presentation deadline
        /// </summary>
        private DateTime presentationDeadline;
        public DateTime PresentationDeadline
        {
            get { return presentationDeadline; }
            set
            {
                presentationDeadline = value;
                RaisePropertyChanged(() => this.PresentationDeadline);
            }
        }

        /// <summary>
        /// Pre-meeting presentation voting deadline
        /// </summary>
        private DateTime preMeetingVotingDeadline;
        public DateTime PreMeetingVotingDeadline
        {
            get { return preMeetingVotingDeadline; }
            set
            {
                preMeetingVotingDeadline = value;
                RaisePropertyChanged(() => this.PreMeetingVotingDeadline);
            }
        }        
        #endregion

        #region ICommand Properties
        /// <summary>
        /// Submit Command
        /// </summary>
        public ICommand SubmitCommand
        {
            get { return new DelegateCommand<object>(SubmitCommandMethod); }
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
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="param">DashboardGadgetParam</param>
        public ViewModelMeetingConfigSchedule(DashboardGadgetParam param)
        {

            dbInteractivity = param.DBInteractivity;
            logger = param.LoggerFacade;
            eventAggregator = param.EventAggregator;

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

        #region ICommand Methods
        /// <summary>
        /// SubmitCommand execution method
        /// </summary>
        /// <param name="param"></param>
        private void SubmitCommandMethod(object param)
        {
            MeetingConfigurationSchedule config = new MeetingConfigurationSchedule();
            DateTime presentationDateTime = Convert.ToDateTime(SelectedPresentationDateTime);
            config.PresentationDay = GetDateTimeFromDayTime(presentationDateTime.DayOfWeek.ToString(), presentationDateTime, false)
                .ToUniversalTime().DayOfWeek.ToString();
            config.PresentationTime = GetDateTimeFromDayTime(presentationDateTime.DayOfWeek.ToString(), presentationDateTime, false)
                .ToUniversalTime();
            if ((config.PresentationTime - presentationDateTime) != (DateTime.UtcNow - DateTime.Now))
            {
                config.PresentationTime = (config.PresentationTime > presentationDateTime)
                    ? presentationDateTime.Add(DateTime.UtcNow - DateTime.Now)
                    : presentationDateTime.Add(DateTime.Now - DateTime.UtcNow);
            }
            config.PresentationTimeZone = "UTC";//SelectedTimeZone;
            config.PresentationDeadlineDay = GetDateTimeFromDayTime(PresentationDeadline.DayOfWeek.ToString(), PresentationDeadline, false)
                .ToUniversalTime().DayOfWeek.ToString();
            config.PresentationDeadlineTime = GetDateTimeFromDayTime(PresentationDeadline.DayOfWeek.ToString(), PresentationDeadline, false)
                .ToUniversalTime();
            if ((config.PresentationDeadlineTime - PresentationDeadline) != (DateTime.UtcNow - DateTime.Now))
            {
                config.PresentationDeadlineTime = (config.PresentationDeadlineTime > PresentationDeadline)
                    ? PresentationDeadline.Add(DateTime.UtcNow - DateTime.Now)
                    : PresentationDeadline.Add(DateTime.Now - DateTime.UtcNow);
            }
            config.PreMeetingVotingDeadlineDay = GetDateTimeFromDayTime(PreMeetingVotingDeadline.DayOfWeek.ToString(), PreMeetingVotingDeadline, false)
                .ToUniversalTime().DayOfWeek.ToString();
            config.PreMeetingVotingDeadlineTime = GetDateTimeFromDayTime(PreMeetingVotingDeadline.DayOfWeek.ToString(), PreMeetingVotingDeadline, false)
                .ToUniversalTime();
            if ((config.PreMeetingVotingDeadlineTime - PreMeetingVotingDeadline) != (DateTime.UtcNow - DateTime.Now))
            {
                config.PreMeetingVotingDeadlineTime = (config.PreMeetingVotingDeadlineTime > PreMeetingVotingDeadline)
                    ? PreMeetingVotingDeadline.Add(DateTime.UtcNow - DateTime.Now)
                    : PreMeetingVotingDeadline.Add(DateTime.Now - DateTime.UtcNow);
            }
            config.CreatedBy = "System";
            config.CreatedOn = DateTime.UtcNow;
            config.ModifiedBy = "System";
            config.ModifiedOn = DateTime.UtcNow;

            if (dbInteractivity != null)
            {
                BusyIndicatorNotification(true, "Updating Investment Committee Meeting Schedule...");
                dbInteractivity.UpdateMeetingConfigSchedule(UserSession.SessionManager.SESSION.UserName, config, UpdateMeetingConfigCallbackMethod);
            }
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Calculate deadlines
        /// </summary>
        /// <param name="presentationDay">presentation day</param>
        /// <param name="presentationTime">presentation time</param>
        private void CalculateDeadlines(String presentationDay, DateTime presentationTime)
        {
            DateTime presentationDateTime = GetDateTimeFromDayTime(presentationDay, presentationTime, false);
            PresentationDeadline = CalculateDeadlines(Convert.ToDateTime(SelectedPresentationDateTime), ConfigPresentationDeadline);
            PreMeetingVotingDeadline = CalculateDeadlines(Convert.ToDateTime(SelectedPresentationDateTime), ConfigPreMeetingVotingDeadLine);
        }

        /// <summary>
        /// Calculate deadlines
        /// </summary>
        /// <param name="dt">Date</param>
        /// <param name="duration">duration</param>
        /// <returns>Calculated dateTime</returns>
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

        /// <summary>
        /// Get DateTime from day and time
        /// </summary>
        /// <param name="day">Day of Week</param>
        /// <param name="time">Time</param>
        /// <param name="IsUTC">True if time is UTC</param>
        /// <returns></returns>
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
        /// <summary>
        /// GetMeetingConfigSchedule Callback Method
        /// </summary>
        /// <param name="val">MeetingConfigurationSchedule</param>
        private void GetMeetingConfigScheduleCallbackMethod(MeetingConfigurationSchedule val)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (val != null)
                {
                    #region Get Local DateTimes
                    String presentationDay = val.PresentationDay;
                    DateTime presentationTime = val.PresentationTime;
                    DateTime presentationDateTime = GetDateTimeFromDayTime(presentationDay
                        , presentationTime).ToLocalTime();

                    String preMeetingVotingDeadlineDay = val.PreMeetingVotingDeadlineDay;
                    DateTime preMeetingVotingDeadlineTime = val.PreMeetingVotingDeadlineTime;
                    DateTime preMeetingVotingDeadlineDateTime = GetDateTimeFromDayTime(preMeetingVotingDeadlineDay
                        , preMeetingVotingDeadlineTime).ToLocalTime();

                    String presentationDeadlineDay = val.PresentationDeadlineDay;
                    DateTime presentationDeadlineTime = val.PresentationDeadlineTime;
                    DateTime presentationDeadlineDateTime = GetDateTimeFromDayTime(presentationDeadlineDay
                        , presentationDeadlineTime).ToLocalTime(); 
                    #endregion

                    SelectedPresentationDay = presentationDateTime.DayOfWeek.ToString();
                    SelectedPresentationDateTime = presentationDateTime;

                    ConfigPresentationDeadline = (float)val.ConfigurablePresentationDeadline;
                    ConfigPreMeetingVotingDeadLine = (float)val.ConfigurablePreMeetingVotingDeadline;
                    isCalculated = true;

                    CalculateDeadlines(SelectedPresentationDay, Convert.ToDateTime(SelectedPresentationDateTime));
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

        /// <summary>
        /// UpdateMeetingConfig Callback Method
        /// </summary>
        /// <param name="result">True if successful</param>
        private void UpdateMeetingConfigCallbackMethod(Boolean? result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result == true)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result, 1);
                    Prompt.ShowDialog("Investment Committee Schedule has been successfully updated");
                }
                else
                {
                    Logging.LogMethodParameterNull(logger, methodNamespace, 1);
                    Prompt.ShowDialog("An error occurred while updating Investment Committee Schedule");                    
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
