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

namespace Ashmore.Emm.GreenField.ICP.Meeting.Module.Views
{
    [Export]
    public partial class ViewAcceptPresentations : ChildWindow
    {

        public ViewAcceptPresentations()
        {
            InitializeComponent();
        }

        [Import]
        public ViewModelAcceptPresentations DataContextSource
        {
            set
            {
                this.DataContext = value;
            }
        }

        //public string Title
        //{
        //    set
        //    {
        //        this.Title = value;
        //    }
        //}


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

