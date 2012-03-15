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
using GreenField.Module.ViewModels;
using System.ComponentModel.Composition;
using GreenField.ServiceCaller;

namespace GreenField.Module.Views
{
    [Export]
    public partial class PerformanceView : UserControl
    {
        public PerformanceView()
        {
            DBInteractivity dbInteractivity = new DBInteractivity();
            InitializeComponent();
            this.DataContext = new PerformanceViewModel(dbInteractivity);
        }

        //[Import]
        //public PerformanceViewModel ViewModel
        //{
        //    set
        //    {
        //        this.DataContext = value;
        //    }
        //}
    }
}
