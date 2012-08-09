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
using Microsoft.Practices.Prism.ViewModel;
//using Ashmore.Emm.GreenField.BusinessLogic.MeetingServiceReference;
using GreenField.ServiceCaller.MeetingServiceReference;

namespace GreenField.Gadgets.Models
{
    public class ICPMeetingInfo : NotificationObject
    {
        private long _meetingID;
        public long MeetingID
        {
            get { return _meetingID; }
            set
            {
                _meetingID = value;
                RaisePropertyChanged(() => this.MeetingID);
            }
        }

        private DateTime _meetingDate = DateTime.Today;
        public DateTime MeetingDateProperty
        {
            get { return _meetingDate; }
            set
            {
                if (_meetingDate != value)
                {
                    DateTime MeetingDateTime = new DateTime(value.Year, value.Month, value.Day,
                       MeetingTimeProperty.Hour, MeetingTimeProperty.Minute, MeetingTimeProperty.Second);
                    DateTime MeetingClosedDateTime = new DateTime(MeetingClosedDateProperty.Year, MeetingClosedDateProperty.Month, MeetingClosedDateProperty.Day,
                        MeetingClosedTimeProperty.Hour, MeetingClosedTimeProperty.Minute, MeetingClosedTimeProperty.Second);

                    _meetingDate = MeetingDateTime;
                    MeetingTimeProperty = MeetingDateTime;
                    RaisePropertyChanged(() => this.MeetingDateProperty);


                    if (MeetingClosedDateTime > MeetingDateTime)
                    {
                        MeetingClosedTimeProperty = MeetingDateTime;
                        MeetingClosedDateProperty = MeetingDateTime;
                    }
                }
            }
        }

        private DateTime _meetingTime = DateTime.Today;
        public DateTime MeetingTimeProperty
        {
            get { return _meetingTime; }
            set
            {
                if (_meetingTime != value)
                {
                    DateTime MeetingDateTime = new DateTime(MeetingDateProperty.Year, MeetingDateProperty.Month, MeetingDateProperty.Day,
                        value.Hour, value.Minute, value.Second);
                    DateTime MeetingClosedDateTime = new DateTime(MeetingClosedDateProperty.Year, MeetingClosedDateProperty.Month, MeetingClosedDateProperty.Day,
                        MeetingClosedTimeProperty.Hour, MeetingClosedTimeProperty.Minute, MeetingClosedTimeProperty.Second);

                    _meetingTime = MeetingDateTime;
                    MeetingDateProperty = MeetingDateTime;
                    RaisePropertyChanged(() => this.MeetingTimeProperty);


                    if (MeetingClosedDateTime > MeetingDateTime)
                    {
                        MeetingClosedTimeProperty = MeetingDateTime;
                        MeetingClosedDateProperty = MeetingDateTime;
                    }

                }
            }
        }

        private DateTime _meetingClosedDate = DateTime.Today;
        public DateTime MeetingClosedDateProperty
        {
            get { return _meetingClosedDate; }
            set
            {
                if (_meetingClosedDate != value)
                {
                    DateTime MeetingDateTime = new DateTime(MeetingDateProperty.Year, MeetingDateProperty.Month, MeetingDateProperty.Day,
                        MeetingTimeProperty.Hour, MeetingTimeProperty.Minute, MeetingTimeProperty.Second);
                    DateTime MeetingClosedDateTime = new DateTime(value.Year, value.Month, value.Day,
                        MeetingClosedTimeProperty.Hour, MeetingClosedTimeProperty.Minute, MeetingClosedTimeProperty.Second);

                    _meetingClosedDate = MeetingClosedDateTime;
                    RaisePropertyChanged(() => this.MeetingClosedDateProperty);

                    if (MeetingClosedDateTime <= MeetingDateTime)
                        MeetingClosedTimeProperty = MeetingClosedDateTime;
                    else
                        MeetingClosedTimeProperty = MeetingDateTime;

                }
            }
        }

        private DateTime _meetingClosedTime = DateTime.Today;
        public DateTime MeetingClosedTimeProperty
        {
            get { return _meetingClosedTime; }
            set
            {
                if (_meetingClosedTime != value)
                {
                    DateTime MeetingDateTime = new DateTime(MeetingDateProperty.Year, MeetingDateProperty.Month, MeetingDateProperty.Day,
                        MeetingTimeProperty.Hour, MeetingTimeProperty.Minute, MeetingTimeProperty.Second);
                    DateTime MeetingClosedDateTime = new DateTime(MeetingClosedDateProperty.Year, MeetingClosedDateProperty.Month, MeetingClosedDateProperty.Day,
                        value.Hour, value.Minute, value.Second);
                    if (MeetingClosedDateTime <= MeetingDateTime)
                    {
                        _meetingClosedTime = MeetingClosedDateTime;
                        MeetingClosedDateProperty = MeetingClosedDateTime;
                    }
                    RaisePropertyChanged(() => this.MeetingClosedTimeProperty);
                }
            }
        }

        private string _description = string.Empty;
        public string DescriptionProperty
        {
            get { return _description; }
            set
            {
                if (_description != value)
                {
                    _description = value;
                    RaisePropertyChanged(() => this.DescriptionProperty);
                }
            }
        }

        public MeetingInfo GetMeetingInfo()
        {
            MeetingInfo retValue = new MeetingInfo();
            retValue.MeetingID = this.MeetingID;
            retValue.MeetingDateTime = this.MeetingDateProperty;
            retValue.MeetingClosedDateTime = this.MeetingClosedTimeProperty;
            retValue.Description = this.DescriptionProperty;

            retValue.ModifiedOn = DateTime.Now;
            retValue.CreatedOn = DateTime.Now;
            retValue.CreatedBy = "rvig";
            retValue.ModifiedBy = "rvig";

            return retValue;
        }
    }
}
