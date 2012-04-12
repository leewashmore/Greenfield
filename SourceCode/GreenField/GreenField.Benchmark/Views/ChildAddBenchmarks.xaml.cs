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
using GreenField.ServiceCaller.ProxyDataDefinitions;
using GreenField.Common;
using GreenField.Benchmark.ViewModels;

namespace GreenField.Benchmark.Views
{
    public partial class ChildAddBenchmarks : ChildWindow
    {
        public ChildAddBenchmarks(List<BenchmarkSelectionData> result)
        {
            InitializeComponent();
            this.DataContext = new ViewModelChildAddBenchmarks(result);            
        }

        public UserBenchmarkPreference SelectedUserBenchmarkPreference { get; set; }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedUserBenchmarkPreference = (this.DataContext as ViewModelChildAddBenchmarks).SelectedUserBenchmarkPreference;
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

    }
}

