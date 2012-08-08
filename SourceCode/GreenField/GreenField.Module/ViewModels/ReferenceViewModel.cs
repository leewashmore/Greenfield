using System;
using Microsoft.Practices.Prism.ViewModel;
using System.ComponentModel.Composition;
using GreenField.ServiceCaller;
using System.Collections.Generic;
using GreenField.ServiceCaller.ProxyDataDefinitions;

namespace GreenField.Module.ViewModels
{
    [Export(typeof(ReferenceViewModel))]
    public class ReferenceViewModel : NotificationObject
    {
        [Import]
        public IDBInteractivity _dbInteractivity { get; set; }

        public ReferenceViewModel()
        {
        }

        private String _message;
        public String Message
        {
            get
            {
                if (String.IsNullOrEmpty(_message))
                    _message = "In Reference View";
                return _message;
            }
            set
            {
                _message = value;
                this.RaisePropertyChanged("Message");
            }
        }
        private List<string> referenceViewList = new List<String>();
        public List<String> ReferenceViewList
        {
            get
            {
                if (referenceViewList.Count == 0)
                    _dbInteractivity.RetrieveReferenceDataListView(RetrieveReferenceDataListViewCompleted);
                return referenceViewList;
            }
            set
            {
                referenceViewList = value;
                this.RaisePropertyChanged("ReferenceViewList");
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
                RetrieveReferenceDataForSelectedView();
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

        private List<ReferenceData> __referenceDataList = new List<ReferenceData>();
        public List<ReferenceData> ReferenceDataList
        {
            get
            {
                return __referenceDataList;
            }
            set
            {
                __referenceDataList = value;
                this.RaisePropertyChanged("ReferenceDataList");
            }
        }
        public void RetrieveReferenceDataListViewCompleted(List<String> result)
        {
            if (result.Count > 0)
                ReferenceViewList = result;
        }

        public void RetrieveReferenceDataForSelectedView()
        {
            if (!String.IsNullOrEmpty(QueryText))
                _dbInteractivity.RetrieveReferenceDataForSelectedView(QueryText, RetrieveReferenceDataForSelectedViewCompleted);
        }

        public void RetrieveReferenceDataForSelectedViewCompleted(List<ReferenceData> result)
        {
            ReferenceDataList = result;
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
            RetrieveReferenceDataForSelectedView();
        }

        private bool CaptureCanExecute(object parameter)
        {
            return true;
        }    
    }
}
