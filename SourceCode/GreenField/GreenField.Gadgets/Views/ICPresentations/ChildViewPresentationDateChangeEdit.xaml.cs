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
    public partial class ChildViewPresentationDateChangeEdit : ChildWindow
    {
        public ChildViewPresentationDateChangeEdit(List<DateTime> validPresentationDates = null)
        {
            InitializeComponent();
            
            if (validPresentationDates == null)
                validPresentationDates = new List<DateTime>() { DateTime.Today.AddDays(1), DateTime.Today.AddMonths(3) };
            
            DateTime startDate = validPresentationDates.OrderBy(g=>g).First();
            DateTime endDate = validPresentationDates.OrderByDescending(g => g).First();
            this.dpPresentationDate.DisplayDateStart = startDate;
            this.dpPresentationDate.DisplayDateEnd = endDate;
            IEnumerable<DateTime> inValidDates = Enumerable.Range(0, endDate.Subtract(startDate).Days + 1).Select(d => startDate.AddDays(d));
            this.dpPresentationDate.BlackoutDates = inValidDates.Where(g => ! validPresentationDates.Contains(g));
        }

        public DateTime SelectedPresentationDate { get; set; }
        public Boolean AlertNotification { get; set; }

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
            SelectedPresentationDate = Convert.ToDateTime(this.dpPresentationDate.SelectedDate);
            this.OKButton.IsEnabled = true;
        }

        private void chkbAlert_Checked(object sender, RoutedEventArgs e)
        {
            if(this.chkbAlert.IsChecked != null)
                AlertNotification = Convert.ToBoolean(this.chkbAlert.IsChecked);
        }
    }
}

