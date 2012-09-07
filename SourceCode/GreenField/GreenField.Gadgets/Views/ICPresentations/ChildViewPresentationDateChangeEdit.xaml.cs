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
using System.Collections.ObjectModel;

namespace GreenField.Gadgets.Views
{
    public partial class ChildViewPresentationDateChangeEdit : ChildWindow
    {
        public ChildViewPresentationDateChangeEdit(List<DateTime> validPresentationDates = null, DateTime? selectedDate = null)
        {
            InitializeComponent();
            
            if (validPresentationDates == null)
                validPresentationDates = new List<DateTime>() { DateTime.Today.AddDays(1), DateTime.Today.AddMonths(3) };

            this.dpPresentationDate.SelectedDate = selectedDate == null ? DateTime.Today : selectedDate;
            
            DateTime startDate = validPresentationDates.OrderBy(g=>g).First();
            DateTime endDate = validPresentationDates.OrderByDescending(g => g).First();
            this.dpPresentationDate.SelectableDateStart = startDate;
            this.dpPresentationDate.SelectableDateEnd = endDate;
            IEnumerable<DateTime> inValidDates = Enumerable.Range(0, endDate.Subtract(startDate).Days + 1).Select(d => startDate.AddDays(d));
            this.dpPresentationDate.AreWeekNumbersVisible = false;
            this.dpPresentationDate.IsTodayHighlighted = false;
            this.dpPresentationDate.FirstDayOfWeek = DayOfWeek.Monday;
            //this.dpPresentationDate.BlackoutDates = inValidDates.Where(g => ! validPresentationDates.Contains(g));
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

        public DateTime SelectedPresentationDateTime { get; set; }
        public Boolean AlertNotification { get; set; }

        public ObservableCollection<DateTime> BlackoutDates { get; set; }        

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void dpPresentationDate_SelectionChanged(object sender, Telerik.Windows.Controls.SelectionChangedEventArgs e)
        {
            if(this.dpPresentationDate.SelectedDate == null)
                return;
            SelectedPresentationDateTime = Convert.ToDateTime(this.dpPresentationDate.SelectedDate);
            this.OKButton.IsEnabled = true;
        }

        private void chkbAlert_Checked(object sender, RoutedEventArgs e)
        {
            if(this.chkbAlert.IsChecked != null)
                AlertNotification = Convert.ToBoolean(this.chkbAlert.IsChecked);
        }
    }
}

