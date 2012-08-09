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
//using Ashmore.Emm.GreenField.Common;
using GreenField.Common;

namespace GreenField.Gadgets.Models
{
    public class ICPVoterInfo : NotificationObject
    {
        private long _voterID;
        public long VoterID
        {
            get { return _voterID; }
            set
            {
                _voterID = value;
                RaisePropertyChanged(() => this.VoterID);
            }
        }

        private bool? _rbtnAgree = false;
        public bool? RbtnAgree
        {
            get { return _rbtnAgree; }
            set
            {
                if (_rbtnAgree != value)
                {
                    _rbtnAgree = value;
                    RaisePropertyChanged(() => RbtnAgree);
                }
            }
        }

        private bool? _rbtnModify = false;
        public bool? RbtnModify
        {
            get { return _rbtnModify; }
            set
            {
                if (_rbtnModify != value)
                {
                    _rbtnModify = value;
                    RaisePropertyChanged(() => RbtnModify);
                }
                if (value == true)
                    VisibilityModeProperty = Visibility.Visible;
                else
                    VisibilityModeProperty = Visibility.Collapsed;
            }
        }

        private bool? _rbtnAbstain = false;
        public bool? RbtnAbstain
        {
            get { return _rbtnAbstain; }
            set
            {
                if (_rbtnAbstain != value)
                {
                    _rbtnAbstain = value;
                    RaisePropertyChanged(() => RbtnAbstain);
                }
            }
        }

        private Visibility _visibilityModeProperty = Visibility.Collapsed;
        public Visibility VisibilityModeProperty
        {
            get { return _visibilityModeProperty; }
            set
            {
                if (_visibilityModeProperty != value)
                {
                    _visibilityModeProperty = value;
                    RaisePropertyChanged(() => this.VisibilityModeProperty);
                }
            }
        }

        private int _voterBuyRange;
        public int VoterBuyRange
        {
            get { return _voterBuyRange; }
            set
            {
                if (_voterBuyRange != value)
                {
                    _voterBuyRange = value;
                    RaisePropertyChanged(() => VoterBuyRange);
                }
            }
        }

        private string _voterPfvMeasure = string.Empty;
        public string VoterPFVMeasure
        {
            get { return _voterPfvMeasure; }
            set
            {
                if (_voterPfvMeasure != value)
                {
                    _voterPfvMeasure = value;
                    RaisePropertyChanged(() => VoterPFVMeasure);
                }
            }
        }

        private int _voterSellRange;
        public int VoterSellRange
        {
            get { return _voterSellRange; }
            set
            {
                if (_voterSellRange != value)
                {
                    _voterSellRange = value;
                    RaisePropertyChanged(() => VoterSellRange);
                }
            }
        }

        private string _voterNotes;
        public string VoterNotes
        {
            get { return _voterNotes; }
            set
            {
                if (_voterNotes != value)
                {
                    _voterNotes = value;
                    RaisePropertyChanged(() => VoterNotes);
                }
            }
        }

        private bool? _requestDiscussion = false;
        public bool? RequestDiscussion
        {
            get { return _requestDiscussion; }
            set
            {
                if (_requestDiscussion != value)
                {
                    _requestDiscussion = value;
                    RaisePropertyChanged(() => RequestDiscussion);
                }
            }
        }

        long? voteType = null;

        public VoterInfo GetVoterInfo(long presentationID)
        {

            if (this.RbtnAgree == true)
                voteType = VoteTypes.Agree;
            else if (this.RbtnModify == true)
                voteType = VoteTypes.Modify;
            else if (this.RbtnAbstain == true)
                voteType = VoteTypes.Abstain;

            VoterInfo retValue = new VoterInfo();

            retValue.VoterID = this.VoterID;
            retValue.PresentationID = presentationID;
            retValue.Name = "mansi";
            retValue.Notes = this.VoterNotes;
            retValue.VoteTypeID = this.voteType;
            retValue.PostMeetingFlag = false;
            retValue.DiscussionFlag = this.RequestDiscussion;
            retValue.VoterPFVMeasure = this.VoterPFVMeasure;
            retValue.VoterBuyRange = this.VoterBuyRange;
            retValue.VoterSellRange = this.VoterSellRange; ;
            retValue.ModifiedOn = DateTime.Now;
            retValue.CreatedOn = DateTime.Now;
            retValue.CreatedBy = "mgupta";
            retValue.ModifiedBy = "mgupta";

            return retValue;
        }
    }
}
