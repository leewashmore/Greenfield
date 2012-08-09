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
using Ashmore.Emm.GreenField.BusinessLogic;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Prism.Regions;
using Ashmore.Emm.GreenField.ICP.Meeting.Module.Model;
using System.ComponentModel.Composition;
using System.Collections.Generic;
using Microsoft.Practices.Prism.Events;
using Ashmore.Emm.GreenField.Common;
using System.Collections.ObjectModel;
using Ashmore.Emm.GreenField.BusinessLogic.MeetingServiceReference;
using System.Linq;

namespace GreenField.Gadgets.ViewModels
{
    [Export]
    public class ViewModelAcceptPresentations : NotificationObject
    {
        ManageMeetings _manageMeetings;

        public ICommand AcceptCommand
        {
            get { return new DelegateCommand<object>(ICPPresentationsAcceptItem, ICPPresentationsAcceptValidation); }
        }

        public ICommand CancelCommand
        {
            get { return new DelegateCommand<object>(ICPPresentationsCancelItem); }
        }

        public DateTime PresentationDate { get; set; }

        public ViewPluginFlagEnumeration ViewPluginFlagEnumerationObject { get; set; }

        private bool _updateMeetingListFlag;
        public bool UpdateMeetingListFlag
        {
            get { return _updateMeetingListFlag; }
            set 
            { 
                _updateMeetingListFlag = value; 
                if (value == true)
                    _manageMeetings.GetMeetings(GetMeetingsCallBackMethod);
            }
        }

        private ObservableCollection<MeetingInfo> _meetinglist;
        public ObservableCollection<MeetingInfo> MeetingList
        {
            get { return _meetinglist; }
            set
            {
                if (_meetinglist != value)
                {
                    _meetinglist = value;
                    RaisePropertyChanged(() => this.MeetingList);
                }
            }
        }

        private MeetingInfo _selectedMeeting;
        public MeetingInfo SelectedMeeting
        {
            get
            {
                return _selectedMeeting;
            }
            set
            {
                if (_selectedMeeting != value)
                {
                    _selectedMeeting = value;
                    RaisePropertyChanged(() => this.SelectedMeeting);
                    RaisePropertyChanged(() => this.AcceptCommand);
                }
            }
        }

        IEventAggregator _eventAggregator;

        #region Constructor

        [ImportingConstructor]
        public ViewModelAcceptPresentations(ManageMeetings manageMeetings, IEventAggregator eventAggr)
        {
            _manageMeetings = manageMeetings;
            _eventAggregator = eventAggr;
        }


        #endregion

        #region ICommand Methods

        private void ICPPresentationsAcceptItem(object param)
        {
            switch (ViewPluginFlagEnumerationObject)
            {
                case ViewPluginFlagEnumeration.Create:
                    _eventAggregator.GetEvent<RegisterPresentationRequest>().Publish(this.SelectedMeeting);
                    break;
                case ViewPluginFlagEnumeration.Update:
                    _eventAggregator.GetEvent<AcceptPresentationRequest>().Publish(this.SelectedMeeting);
                    break;
            }
            
            SelectedMeeting = null;
        }

        private void ICPPresentationsCancelItem(object param)
        {

        }

        private bool ICPPresentationsAcceptValidation(object param)
        {
            return (SelectedMeeting == null) ? false : true;
        }

        #endregion

        #region CallBack Methods

        private void GetMeetingsCallBackMethod(List<MeetingInfo> val)
        {
            MeetingList = new ObservableCollection<MeetingInfo>(val.Where(mi => mi.MeetingClosedDateTime >= DateTime.Now).OrderBy(mi=>mi.MeetingDateTime).ToList());
            try { SelectedMeeting = MeetingList.Where(mi => mi.MeetingDateTime == PresentationDate).Single(); }
            catch (InvalidOperationException) { }
            
            UpdateMeetingListFlag = false;
        }

        #endregion
    }
}
