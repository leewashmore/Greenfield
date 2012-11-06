using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace GreenField.Gadgets.Views
{
    /// <summary>
    /// Code behind for ChildViewPresentationMeetingMinutesAddAttendee
    /// </summary>
    public partial class ChildViewPresentationMeetingMinutesAddAttendee : ChildWindow
    {
        #region Properties
        /// <summary>
        /// Reference user information
        /// </summary>
        public List<String> UserInfo { get; set; }

        /// <summary>
        /// Reference attendance types
        /// </summary>
        public List<String> AttendanceTypeInfo
        {
            get { return new List<string> { "Attended", "Video Conference", "Tele Conference", "Not Present" }; }
        }

        /// <summary>
        /// Selected user
        /// </summary>
        public String SelectedUser { get; set; }

        /// <summary>
        /// Selected attendance type
        /// </summary>
        public String SelectedAttendanceType { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userInfo">List of users</param>
        public ChildViewPresentationMeetingMinutesAddAttendee(List<String> userInfo)
        {
            InitializeComponent();
            UserInfo = userInfo;
            this.cbUser.ItemsSource = UserInfo;
            this.cbAttendanceType.ItemsSource = AttendanceTypeInfo;
        } 
        #endregion        

        #region Event Handlers
        /// <summary>
        /// OKButton Click EventHandler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">RoutedEventArgs</param>
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        /// <summary>
        /// CancelButton Click EventHandler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">RoutedEventArgs</param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        /// <summary>
        /// cbUser SelectionChanged EventHandler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">SelectionChangedEventArgs</param>
        private void cbUser_SelectionChanged(object sender, Telerik.Windows.Controls.SelectionChangedEventArgs e)
        {
            SelectedUser = (String)this.cbUser.SelectedValue;
            if (SelectedUser != null && SelectedAttendanceType != null)
            {
                this.OKButton.IsEnabled = true;
            }
        }

        /// <summary>
        /// cbAttendanceType SelectionChanged EventHandler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">SelectionChangedEventArgs</param>
        private void cbAttendanceType_SelectionChanged(object sender, Telerik.Windows.Controls.SelectionChangedEventArgs e)
        {
            SelectedAttendanceType = (String)this.cbAttendanceType.SelectedValue;
            if (SelectedUser != null && SelectedAttendanceType != null)
            {
                this.OKButton.IsEnabled = true;
            }
        } 
        #endregion
    }
}