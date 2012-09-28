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
    public partial class ChildViewReSubmitPresentation : ChildWindow
    {
        public ChildViewReSubmitPresentation()
        {
            InitializeComponent();
        }

        public Boolean AlertNotification { get; set; }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void chkbAlert_Checked(object sender, RoutedEventArgs e)
        {
            if(this.chkbAlert.IsChecked != null)
                AlertNotification = Convert.ToBoolean(this.chkbAlert.IsChecked);
        }
    }
}

