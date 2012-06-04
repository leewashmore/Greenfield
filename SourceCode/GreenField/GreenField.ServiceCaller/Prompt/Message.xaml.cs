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

namespace GreenField.ServiceCaller
{
    public partial class Message : ChildWindow
    {
        public Message(string messageText, string captionText, MessageBoxButton buttonType)
        {
            InitializeComponent();
            this.txtMessage.Text = messageText;
            this.Title = captionText;
            this.CancelButton.Visibility = buttonType == MessageBoxButton.OKCancel ? Visibility.Visible : Visibility.Collapsed;
        }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}

