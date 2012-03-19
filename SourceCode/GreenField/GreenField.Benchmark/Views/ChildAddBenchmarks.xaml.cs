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

namespace GreenField.Benchmark.Views
{
    public partial class ChildAddBenchmarks : ChildWindow
    {
        public ChildAddBenchmarks(List<BenchmarkReferenceData> benchmarkReferenceData)
        {
            InitializeComponent();
            
            UngroupedBenchmarkReferenceData = new ObservableCollection<BenchmarkReferenceData>(benchmarkReferenceData);

            radSelectBenchmark.ItemsSource = benchmarkReferenceData;
            radSelectBenchmark.DisplayMemberPath = "BenchmarkName";
           //  UngroupedBenchmarkReferenceData.Add(
        }

        
        private ObservableCollection<BenchmarkReferenceData> _ungroupedBenchmarkReferenceData;
        public ObservableCollection<BenchmarkReferenceData> UngroupedBenchmarkReferenceData
        {
            get { return _ungroupedBenchmarkReferenceData; }
            set { _ungroupedBenchmarkReferenceData = value; }
        }        

        private BenchmarkReferenceData _selectedBenchmarkReferenceData;
        public BenchmarkReferenceData SelectedBenchmarkReferenceData
        {
            get { return _selectedBenchmarkReferenceData; }
            set { _selectedBenchmarkReferenceData = value; }
        }

        private string _totalReturnType = BenchmarkReturnTypes.TotalReturnType;
        public string TotalReturnType
        {
            get { return _totalReturnType; }
            set { _totalReturnType = value; }
        }

        private string _netReturnType = BenchmarkReturnTypes.NetReturnType;
        public string NetReturnType
        {
            get { return _netReturnType; }
            set { _netReturnType = value; }
        }

        private string _priceReturnType = BenchmarkReturnTypes.PriceReturnType;
        public string PriceReturnType
        {
            get { return _priceReturnType; }
            set { _priceReturnType = value; }
        }

        public string SelectedReturnType { get; set; }
        
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            SelectedReturnType = rbtnNet.IsChecked == true ? NetReturnType : rbtnPrice.IsChecked == true ? PriceReturnType : TotalReturnType;
            SelectedBenchmarkReferenceData.BenchmarkReturnType = SelectedReturnType == BenchmarkReturnTypes.TotalReturnType ? null : SelectedReturnType;
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void radSelectBenchmark_SelectionChanged(object sender, Telerik.Windows.Controls.SelectionChangedEventArgs e)
        {
            this.borderRadiobutton.Visibility = Visibility.Visible;
        }
    }
}

