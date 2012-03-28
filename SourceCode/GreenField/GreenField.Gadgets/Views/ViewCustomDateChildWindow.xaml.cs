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
    public partial class ViewCustomDateChildWindow : ChildWindow
    {
        public bool enteredDateCorrect = false;
        public DateTime startDate = DateTime.Today;
        public DateTime endDate = DateTime.Today;
        public ViewCustomDateChildWindow()
        {
            InitializeComponent();
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (dpStartDate.SelectedDate == null)
            {
                MessageBox.Show("Please enter a valid start Date");
            }
            else if (dpEndDate.SelectedDate == null)
            {
                MessageBox.Show("Please enter a valid end Date");
            }
            else
            {
                if (dpStartDate.SelectedDate > dpEndDate.SelectedDate)
                {
                    MessageBox.Show("Start Date cannot be greater then End Date");
                }
                else
                {
                    enteredDateCorrect = true;
                    this.DialogResult = true;
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            enteredDateCorrect = false;
            this.DialogResult = false;
        }
    }
}