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
using System.ComponentModel.Composition;
using GreenField.ServiceCaller;
using Microsoft.Practices.Prism.ViewModel;
using System.Collections.Generic;
using GreenField.ServiceCaller.ProxyDataDefinitions;

namespace GreenField.Module.ViewModels
{
    [Export(typeof(HoldingsViewModel))]
    public class HoldingsViewModel : NotificationObject
    {
        //[Import]
        //public IDBInteractivity _dbInteractivity { get; set; }

        IDBInteractivity _dbInteractivity;

        public HoldingsViewModel(IDBInteractivity dbInteractivity)
        {
            _dbInteractivity = dbInteractivity;
        }

        private String _message;
        public String Message
        {
            get
            {
                if (String.IsNullOrEmpty(_message))
                    _message = "In Holdings View";
                return _message;
            }
            set
            {
                _message = value;
                this.RaisePropertyChanged("Message");
            }
        }

        private List<string> dimensionViewList = new List<String>();
        public List<String> DimensionViewList
        {
            get
            {
                if (dimensionViewList.Count == 0)
                    _dbInteractivity.RetrieveDimensionDataListView(RetrieveDimenstionDataListViewCompleted);
                return dimensionViewList;
            }
            set
            {
                dimensionViewList = value;
                this.RaisePropertyChanged("DimensionViewList");
            }
        }

        private String _selectedView;
        public String SelectedView
        {
            get
            {
                return _selectedView;
            }
            set
            {
                _selectedView = value;
                RetrieveDimensionDataForSelectedView();
                this.RaisePropertyChanged("SelectedView");
            }
        }

        private String _queryText;
        public String QueryText
        {
            get
            {
                return _queryText;

            }

            set
            {

                _queryText = value;
                this.RaisePropertyChanged("QueryText");

            }
        }

        private List<HoldingsData> __holdingsDataList = new List<HoldingsData>();
        public List<HoldingsData> HoldingsDataList
        {
            get
            {
                return __holdingsDataList;
            }
            set
            {
                __holdingsDataList = value;
                this.RaisePropertyChanged("HoldingsDataList");
            }
        }

        public void RetrieveDimenstionDataListViewCompleted(List<String> result)
        {
            if (result.Count > 0)
                DimensionViewList = result;
        }

        public void RetrieveDimensionDataForSelectedView()
        {
            if (!String.IsNullOrEmpty(QueryText))
                _dbInteractivity.RetrieveDimensionDataForSelectedView(QueryText, RetrieveDimensionDataForSelectedViewCompleted);
        }

        public void RetrieveDimensionDataForSelectedViewCompleted(List<HoldingsData> result)
        {
            HoldingsDataList = result;
        }

        private DelegateCommand _runQueryCommand;
        public DelegateCommand RunQueryCommand
        {
            get
            {
                if (_runQueryCommand == null)
                    _runQueryCommand = new DelegateCommand(OnCapture, CaptureCanExecute);

                return _runQueryCommand;
            }
        }

        private void OnCapture(object parameter)
        {
            RetrieveDimensionDataForSelectedView();
        }

        private bool CaptureCanExecute(object parameter)
        {
            return true;
        }
    }
}
