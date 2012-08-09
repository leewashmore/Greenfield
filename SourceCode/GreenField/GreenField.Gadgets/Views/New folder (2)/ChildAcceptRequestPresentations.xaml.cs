using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel.Composition;
using Ashmore.Emm.GreenField.ICP.Meeting.Module.ViewModels;
using Ashmore.Emm.GreenField.BusinessLogic;
using Ashmore.Emm.GreenField.Common;
using Ashmore.Emm.GreenField.BusinessLogic.MeetingServiceReference;
using System.Collections.ObjectModel;

namespace Ashmore.Emm.GreenField.ICP.Meeting.Module.Views
{
    public partial class ChildAcceptRequestPresentations : ChildWindow
    {

        private ManageMeetings _manageMeetings;
        private ObservableCollection<MeetingInfo> MeetingList { get; set; }
        private DateTime _presentationDate { get; set; }

        public MeetingInfo SelectedMeeting { get; set; }
        


        public ChildAcceptRequestPresentations(ManageMeetings manageMeetings, DateTime presentationDate)
        {
            InitializeComponent();
            
            _manageMeetings = manageMeetings;
            _presentationDate = presentationDate;

            _manageMeetings.GetMeetings(GetMeetingsCallBackMethod);
        }

        #region Events

        private void btnAccept_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void cmbBoxMeetingDate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            btnAccept.IsEnabled = true;
            SelectedMeeting = cmbBoxMeetingDate.SelectedItem as MeetingInfo;
            txtBoxMeetingClosingDate.Text = String.Format(SelectedMeeting.MeetingClosedDateTime.ToString(), "g");
        }

        #endregion

        #region CallBack Method(s)

        private void GetMeetingsCallBackMethod(List<MeetingInfo> val)
        {
            MeetingList = new ObservableCollection<MeetingInfo>(val.Where(mi => mi.MeetingClosedDateTime >= DateTime.Now).OrderBy(mi => mi.MeetingDateTime).ToList());
            
            try { SelectedMeeting = MeetingList.Where(mi => mi.MeetingDateTime == _presentationDate).FirstOrDefault(); }
            catch (InvalidOperationException) { }

            cmbBoxMeetingDate.ItemsSource = MeetingList;
            cmbBoxMeetingDate.SelectedItem = SelectedMeeting;
        }

        #endregion

        


        
    }
}

