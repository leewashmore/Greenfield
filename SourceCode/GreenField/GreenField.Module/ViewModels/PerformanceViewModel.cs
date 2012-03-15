using System;
using Microsoft.Practices.Prism.ViewModel;
using System.ComponentModel.Composition;
using GreenField.ServiceCaller;
using System.Collections.Generic;
using GreenField.ServiceCaller.ProxyDataDefinitions;

namespace GreenField.Module.ViewModels
{
    [Export(typeof(PerformanceViewModel))]
    public class PerformanceViewModel : NotificationObject
    {
        //[Import]
        //public IDBInteractivity _dbInteractivity { get; set; }

        IDBInteractivity _dbInteractivity;

        public PerformanceViewModel(IDBInteractivity dbInteractivity)
        {
            this._dbInteractivity = dbInteractivity;
        }

        private String _message;
        public String Message
        {
            get
            {
                if (String.IsNullOrEmpty(_message))
                    _message = "In Performance View";
                return _message;
            }
            set
            {
                _message = value;
                this.RaisePropertyChanged("Message");
            }
        }

        private List<String> performanceViewList = new List<string>();
        public List<String> PerformanceViewList
        {
            get
            {
                if (performanceViewList.Count == 0)
                    _dbInteractivity.RetrieveAggregateDataListView(RetrievePerformanceDataListViewCompleted);
                return performanceViewList;

            }
            set
            {
                performanceViewList = value;
                this.RaisePropertyChanged("PerformanceViewList");

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
                RetrievePerformanceDataForSelectedView();
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

        private List<PerformanceData> _performanceDataList = new List<PerformanceData>();
        public List<PerformanceData> PerformanceDataList
        {
            get
            {
                return _performanceDataList;

            }

            set
            {
                _performanceDataList = value;
                this.RaisePropertyChanged("PerformanceDataList");
            }

        }

        public void RetrievePerformanceDataListViewCompleted(List<String> result)
        {
            if (result.Count > 0)
                PerformanceViewList = result;
        }

        public void RetrievePerformanceDataForSelectedView()
        {
            if (!String.IsNullOrEmpty(QueryText))
                _dbInteractivity.RetrievePerformanceDataForSelectedView(QueryText, RetrievePerformanceDataForSelectedViewCompleted);
        }
        public void RetrievePerformanceDataForSelectedViewCompleted(List<PerformanceData> result)
        {
            PerformanceDataList = result;
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
            RetrievePerformanceDataForSelectedView();
        }

        private bool CaptureCanExecute(object parameter)
        {
            return true;
        }

    }
}
