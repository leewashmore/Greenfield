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
using GreenField.Module.ViewModels;
using GreenField.ServiceCaller;

namespace GreenField.Module.Views
{
    [Export] 
    public partial class HoldingsView : UserControl
    {
        public HoldingsView()
        {
            DBInteractivity dbInteractivity = new DBInteractivity();
            InitializeComponent();
            this.DataContext = new HoldingsViewModel(dbInteractivity);
        }

        //[Import]
        //public HoldingsViewModel ViewModel
        //{
        //    set
        //    {
        //        this.DataContext = value;
        //    }
        //}
    }
}
