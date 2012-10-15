using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace GreenField.Gadgets.Views
{
    /// <summary>
    /// Code behind for ChildViewPresentationDateChangeEdit
    /// </summary>
    public partial class ChildViewPresentationDateChangeEdit : ChildWindow
    {
        #region Properties
        /// <summary>
        /// Stores selected presentation datetime
        /// </summary>
        public DateTime SelectedPresentationDateTime { get; set; }

        /// <summary>
        /// Stores true if alert notification check box is checked
        /// </summary>
        public Boolean IsAlertNotificationChecked { get; set; }

        /// <summary>
        /// Stores collection of datetimes to be blacked out
        /// </summary>
        public ObservableCollection<DateTime> BlackoutDates { get; set; }
        #endregion        

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="validPresentationDates">Valid presentation datetimes</param>
        /// <param name="selectedDate">selected datetime</param>
        public ChildViewPresentationDateChangeEdit(List<DateTime> validPresentationDates = null, DateTime? selectedDate = null)
        {
            InitializeComponent();

            if (validPresentationDates == null)
            {
                validPresentationDates = new List<DateTime>() { DateTime.Today.AddDays(1), DateTime.Today.AddMonths(3) };
            }
            this.dpPresentationDate.SelectedDate = selectedDate == null ? DateTime.Today : selectedDate;
            DateTime startDate = validPresentationDates.OrderBy(g => g).First();
            DateTime endDate = validPresentationDates.OrderByDescending(g => g).First();
            this.dpPresentationDate.SelectableDateStart = startDate;
            this.dpPresentationDate.SelectableDateEnd = endDate;
            IEnumerable<DateTime> inValidDates = Enumerable.Range(0, endDate.Subtract(startDate).Days + 1).Select(d => startDate.AddDays(d));
            this.dpPresentationDate.AreWeekNumbersVisible = false;
            this.dpPresentationDate.IsTodayHighlighted = false;
            this.dpPresentationDate.FirstDayOfWeek = DayOfWeek.Monday;
            this.DataContext = BlackoutDates;
            this.dpPresentationDate.Loaded += (se, e) =>
            {
                this.dpPresentationDate.BlackoutDates = inValidDates.Where(g => !validPresentationDates.Contains(g));
            };

            this.dpPresentationDate.DisplayDateChanged += (se, e) =>
            {
                this.dpPresentationDate.BlackoutDates = inValidDates.Where(g => !validPresentationDates.Contains(g));
            };

            this.dpPresentationDate.DisplayModeChanged += (se, e) =>
            {
                this.dpPresentationDate.BlackoutDates = inValidDates.Where(g => !validPresentationDates.Contains(g));
            };
            BlackoutDates = new ObservableCollection<DateTime>(inValidDates.Where(g => !validPresentationDates.Contains(g)));
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
        /// dpPresentationDate SelectionChanged EventHandler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">SelectionChangedEventArgs</param>
        private void dpPresentationDate_SelectionChanged(object sender, Telerik.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (this.dpPresentationDate.SelectedDate == null)
            {
                return;
            }
            SelectedPresentationDateTime = Convert.ToDateTime(this.dpPresentationDate.SelectedDate);
            this.OKButton.IsEnabled = true;
        }

        /// <summary>
        /// chkbAlert Checked EventHandler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">RoutedEventArgs</param>
        private void chkbAlert_Checked(object sender, RoutedEventArgs e)
        {
            if (this.chkbAlert.IsChecked != null)
            {
                IsAlertNotificationChecked = Convert.ToBoolean(this.chkbAlert.IsChecked);
            }
        } 
        #endregion
    }
}

