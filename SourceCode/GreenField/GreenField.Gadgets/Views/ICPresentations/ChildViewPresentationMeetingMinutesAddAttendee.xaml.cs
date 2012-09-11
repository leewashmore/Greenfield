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

namespace GreenField.Gadgets.Views
{
    public partial class ChildViewPresentationMeetingMinutesAddAttendee : ChildWindow
    {
        public ChildViewPresentationMeetingMinutesAddAttendee(List<String> userInfo)
        {
            InitializeComponent();
            UserInfo = userInfo;
            this.cbUser.ItemsSource = UserInfo;
            this.cbAttendanceType.ItemsSource = AttendanceTypeInfo;
        }

        public List<String> UserInfo { get; set; }

        public List<String> AttendanceTypeInfo
        {
            get { return new List<string> { "Attended", "Video Conference", "Tele Conference", "Not Present" }; }
        }

        public String SelectedUser { get; set; }
        public String SelectedAttendanceType { get; set; }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void cbUser_SelectionChanged(object sender, Telerik.Windows.Controls.SelectionChangedEventArgs e)
        {
            SelectedUser = (String)this.cbUser.SelectedValue;
            if (SelectedUser != null && SelectedAttendanceType != null)
                this.OKButton.IsEnabled = true;
        }

        private void cbAttendanceType_SelectionChanged(object sender, Telerik.Windows.Controls.SelectionChangedEventArgs e)
        {
            SelectedAttendanceType = (String)this.cbAttendanceType.SelectedValue;
            if (SelectedUser != null && SelectedAttendanceType != null)
                this.OKButton.IsEnabled = true;
        }

        
    }
}

