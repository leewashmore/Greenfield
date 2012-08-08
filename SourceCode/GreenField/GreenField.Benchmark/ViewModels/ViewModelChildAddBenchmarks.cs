using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Practices.Prism.ViewModel;
using System.Collections.Generic;
using GreenField.ServiceCaller.ProxyDataDefinitions;
using System.Collections.ObjectModel;
using GreenField.Common;

namespace GreenField.Benchmark.ViewModels
{
    public class ViewModelChildAddBenchmarks : NotificationObject
    {
        public ViewModelChildAddBenchmarks(List<BenchmarkSelectionData> result)
        {
            UngroupedBenchmarkSelectionInfo = new ObservableCollection<BenchmarkSelectionData>(result);            
        }

        private ObservableCollection<BenchmarkSelectionData> _ungroupedBenchmarkSelectionInfo;
        public ObservableCollection<BenchmarkSelectionData> UngroupedBenchmarkSelectionInfo
        {
            get { return _ungroupedBenchmarkSelectionInfo; }
            set
            {
                if (_ungroupedBenchmarkSelectionInfo != value)
                {
                    _ungroupedBenchmarkSelectionInfo = value;
                    RaisePropertyChanged(() => this.UngroupedBenchmarkSelectionInfo);
                }
            }
        }

        private BenchmarkSelectionData _selectedBenchmarkSelectionInfo;
        public BenchmarkSelectionData SelectedBenchmarkSelectionInfo
        {
            get { return _selectedBenchmarkSelectionInfo; }
            set
            {
                if (_selectedBenchmarkSelectionInfo != value)
                {
                    _selectedBenchmarkSelectionInfo = value;
                    RaisePropertyChanged(() => this.SelectedBenchmarkSelectionInfo);
                    BenchmarkTypeSelectionVisibility = Visibility.Visible;
                    SelectedUserBenchmarkPreference.BenchmarkName = value.Name;                    
                }
            }
        }

        private UserBenchmarkPreference _selectedUserBenchmarkPreference = new UserBenchmarkPreference();
        public UserBenchmarkPreference SelectedUserBenchmarkPreference
        {
            get { return _selectedUserBenchmarkPreference; }
            set { _selectedUserBenchmarkPreference = value; }
        }

        private Visibility _benchmarkTypeSelectionVisibility = Visibility.Collapsed;
        public Visibility BenchmarkTypeSelectionVisibility
        {
            get { return _benchmarkTypeSelectionVisibility; }
            set
            {
                if (_benchmarkTypeSelectionVisibility != value)
                {
                    _benchmarkTypeSelectionVisibility = value;
                    RaisePropertyChanged(() => this.BenchmarkTypeSelectionVisibility);
                }
            }
        }

        private string _totalReturnType = "Total (Gross)";
        public string TotalReturnType
        {
            get { return _totalReturnType; }
            set
            {
                if (_totalReturnType != value)
                {
                    _totalReturnType = value;
                    RaisePropertyChanged(() => this.TotalReturnType);
                }
            }
        }

        private bool? _totalReturnTypeChecked = true;
        public bool? TotalReturnTypeChecked
        {
            get { return _totalReturnTypeChecked; }
            set
            {
                if (_totalReturnTypeChecked != value)
                {
                    _totalReturnTypeChecked = value;
                    RaisePropertyChanged(() => this.TotalReturnTypeChecked);
                    if (value == true)
                    {
                        SelectedUserBenchmarkPreference.BenchmarkReturnType = String.Empty;
                    }
                }
            }
        }        

        private string _netReturnType = "Net";
        public string NetReturnType
        {
            get { return _netReturnType; }
            set
            {
                if (_netReturnType != value)
                {
                    _netReturnType = value;
                    RaisePropertyChanged(() => this.NetReturnType);
                }
            }
        }

        private bool? _netReturnTypeChecked = false;
        public bool? NetReturnTypeChecked
        {
            get { return _netReturnTypeChecked; }
            set
            {
                if (_netReturnTypeChecked != value)
                {
                    _netReturnTypeChecked = value;
                    RaisePropertyChanged(() => this.NetReturnTypeChecked);
                    if (value == true)
                    {
                        SelectedUserBenchmarkPreference.BenchmarkReturnType = EntityReturnType.NetReturnType;
                    }
                }
            }
        }        

        private string _priceReturnType = "Price";
        public string PriceReturnType
        {
            get { return _priceReturnType; }
            set
            {
                if (_priceReturnType != value)
                {
                    _priceReturnType = value;
                    RaisePropertyChanged(() => this.PriceReturnType);
                }
            }
        }

        private bool? _priceReturnTypeChecked = false;
        public bool? PriceReturnTypeChecked
        {
            get { return _priceReturnTypeChecked; }
            set
            {
                if (_priceReturnTypeChecked != value)
                {
                    _priceReturnTypeChecked = value;
                    if (value == true)
                    {
                        SelectedUserBenchmarkPreference.BenchmarkReturnType = EntityReturnType.PriceReturnType;
                    }
                }
            }
        }        
    }
}
