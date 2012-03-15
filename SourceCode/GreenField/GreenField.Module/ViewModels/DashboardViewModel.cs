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
using Microsoft.Practices.Prism.ViewModel;
using GreenField.ServiceCaller;
using System.Collections;
using System.Collections.Generic;
using GreenField.Common.Helper;

namespace GreenField.Module.ViewModels
{
    [Export(typeof(DashboardViewModel))]
    public class DashboardViewModel : NotificationObject
    {
        [Import]
        public IDBInteractivity _dbInteractivity { get; set; }

        public DashboardViewModel()
        {
        }

        private IList _itemSourceList;
        public IList ItemSourceList
        {
            get
            {
                if (_itemSourceList == null)
                    _dbInteractivity.RetrievePerformanceDataForSelectedView("1=1", RetrievePerformanceDataForSelectedViewCompleted);
                return _itemSourceList;
            }
            set
            {
                _itemSourceList = value;
                this.RaisePropertyChanged("ItemSourceList");
            }
        }

        private List<PersistPreference> _userPreferenceList;
        public List<PersistPreference> UserPreferenceList
        {
            get
            {
                if(_userPreferenceList == null)
                    _dbInteractivity.GetPersonalizedDashboardData(GetPersonalizedDashboardDataCompleted);
                return _userPreferenceList;
            }
            set
            {
                _userPreferenceList = value;
                this.RaisePropertyChanged("UserPreferenceList");
            }
        }

        private DelegateCommand _saveCommand;
        public DelegateCommand SaveCommand
        {
            get
            {
                if (_saveCommand == null)
                    _saveCommand = new DelegateCommand(OnCapture, CaptureCanExecute);

                return _saveCommand;
            }
        }

        private void OnCapture(object parameter)
        {
            
        }

        private bool CaptureCanExecute(object parameter)
        {
            return true;
        }

        public void GetPersonalizedDashboardDataCompleted(string userPref)
        {
            UserPreferenceList = Serializer.Deserialize<List<PersistPreference>>(userPref);
        }

        public void RetrievePerformanceDataForSelectedViewCompleted(IList result)
        {
            ItemSourceList = result;
        }

    }
}
