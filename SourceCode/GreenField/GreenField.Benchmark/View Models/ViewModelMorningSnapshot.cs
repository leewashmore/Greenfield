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
using System.Linq;
using System.Collections.ObjectModel;
using GreenField.ServiceCaller.ProxyDataDefinitions;
using System.Collections.Generic;
using Microsoft.Practices.Prism.Commands;
using GreenField.Benchmark.Views;
using GreenField.Common;

namespace GreenField.Benchmark.ViewModel
{
    [Export(typeof(ViewModelMorningSnapshot))]
    public class ViewModelMorningSnapshot : NotificationObject
    {
        #region PrivateFields
        private IDBInteractivity _dbInteractivity;
        #endregion

        #region Constructor
        [ImportingConstructor]
        public ViewModelMorningSnapshot(IDBInteractivity dbInteractivity, IManageSessions manageSessions)
        {
            _dbInteractivity = dbInteractivity;
            manageSessions.GetSession((session) =>
                {
                    //_dbInteractivity.RetrieveBenchmarkSelectionData(RetrieveBenchmarkSelectionDataCallbackMethod)
                    _dbInteractivity.RetrieveUserPreferenceBenchmarkData(session.UserName, RetrieveUserPreferenceBenchmarkDataCallBackMethod);
                    _dbInteractivity.RetrieveBenchmarkReferenceData(RetrieveBenchmarkReferenceDataCallBackMethod);
                });
        }       

        #endregion

        #region Properties

        #region UI Fields

        public string MorningSnapshotUIHeader
        {
            get { return "Morning Snapshot (" + DateTime.Today.ToLongDateString() + ") - Emerging Markets"; }
        }

        private ObservableCollection<BenchmarkReferenceData> _allBenchmarks;
        public ObservableCollection<BenchmarkReferenceData> AllBenchmarks
        {
            get
            {
                if (_allBenchmarks == null)
                    _allBenchmarks = new ObservableCollection<BenchmarkReferenceData>();
                return _allBenchmarks;
            }
            set
            {
                if (_allBenchmarks != value)
                {
                    _allBenchmarks = value;
                    RaisePropertyChanged(() => AllBenchmarks);
                }
            }
        }

        private ObservableCollection<BenchmarkReferenceData> _benchmarkGroupedData;
        public ObservableCollection<BenchmarkReferenceData> BenchmarkGroupedData
        {
            get
            {
                if (_benchmarkGroupedData == null)
                    _benchmarkGroupedData = new ObservableCollection<BenchmarkReferenceData>();
                return _benchmarkGroupedData;
            }
            set
            {
                if (_benchmarkGroupedData != value)
                {
                    _benchmarkGroupedData = value;
                    RaisePropertyChanged(() => BenchmarkGroupedData);
                }
            }
        }

        private BenchmarkReferenceData _selectedRow;
        public BenchmarkReferenceData SelectedRow
        {
            get { return _selectedRow; }
            set
            {
                if (_selectedRow != value)
                {
                    _selectedRow = value;
                    RaisePropertyChanged(() => SelectedRow);
                }
            }
        }


        private string _newGroupName;
        public string NewGroupName
        {
            get { return _newGroupName; }
            set
            {
                if (_newGroupName != value)
                {
                    _newGroupName = value;
                    RaisePropertyChanged(() => NewGroupName);
                }
            }
        }

        private ObservableCollection<BenchmarkReferenceData> _selectedBenchmark;
        public ObservableCollection<BenchmarkReferenceData> SelectedBenchmark
        {
            get { return _selectedBenchmark; }
            set
            {
                if (_selectedBenchmark != value)
                {
                    _selectedBenchmark = value;
                    RaisePropertyChanged(() => SelectedBenchmark);
                }
            }
        }


        #endregion

        #region ICommand

        public ICommand AddGroupNameCommand
        {
            get
            {
                return new DelegateCommand<object>(AddGroupNameCommandMethod);
            }
        }

        public ICommand DeleteGroupCommand
        {
            get
            {
                return new DelegateCommand<object>(DeleteGroupCommandMethod);
            }
        }

        public ICommand AddBenchmarkToGroupCommand
        {
            get
            {
                return new DelegateCommand<object>(AddBenchmarkToGroupCommandMethod);
            }
        }

        public ICommand DeleteBenchmarkfromGroupCommand
        {
            get
            {
                return new DelegateCommand<object>(DeleteBenchmarkfromGroupCommandMethod);
            }
        }

        #endregion

        #endregion

        #region ICommandMethods

        private void DeleteGroupCommandMethod(object param)
        {
            if (SelectedRow != null)
            {
                _dbInteractivity.DeleteUserGroupPreference(SessionManager.SESSION.UserName, SelectedRow.GroupName, DeleteUserGroupPreferenceCallbackMethod);                 
            }  
        }

        private void AddGroupNameCommandMethod(object param)
        {
            ChildAddNewGroup childAddNewGroup = new ChildAddNewGroup();
            childAddNewGroup.Show();
            childAddNewGroup.Unloaded += (se, e) =>
             {
                 if (childAddNewGroup.DialogResult == true)
                 {
                     NewGroupName = childAddNewGroup.txtEnterValue.Text;
                     _dbInteractivity.AddUserGroupPreference(SessionManager.SESSION.UserName, NewGroupName, (result) =>
                         {
                             if (result)
                             {
                                 if (SelectedRow.GroupName == null)
                                 {
                                     BenchmarkGroupedData.Clear();
                                     BenchmarkGroupedData.Add(new BenchmarkReferenceData()
                                     {
                                         GroupName = NewGroupName
                                     });
                                 }
                                 else
                                 {
                                     BenchmarkGroupedData.Add(new BenchmarkReferenceData()
                                     {
                                         GroupName = NewGroupName
                                     });
                                 }
                             }
                         });
                 }
             };
            RaisePropertyChanged(() => BenchmarkGroupedData);
        }

        private BenchmarkReferenceData addedBenchmarkReferenceData = new BenchmarkReferenceData();
        
        private void AddBenchmarkToGroupCommandMethod(object param)
        {
            if (SelectedRow.GroupName != null)
            {
                //ChildAddBenchmarks childAddBenchmarks = new ChildAddBenchmarks(AllBenchmarks.Where(i => !(BenchmarkGroupedData.Contains(i))).ToList());
                List<string> BenchmarkGroupedData_BenchmarkName = BenchmarkGroupedData.Select(r => r.BenchmarkName).Distinct().ToList();
                ChildAddBenchmarks childAddBenchmarks = new ChildAddBenchmarks(AllBenchmarks.Where(i => !(BenchmarkGroupedData_BenchmarkName.Contains(i.BenchmarkName))).ToList());
                childAddBenchmarks.Show();
                childAddBenchmarks.Unloaded += (se, e) =>
                 {
                     if (childAddBenchmarks.DialogResult == true)
                     {
                         if (childAddBenchmarks.SelectedBenchmarkReferenceData != null)
                         {
                             addedBenchmarkReferenceData = childAddBenchmarks.SelectedBenchmarkReferenceData;

                             _dbInteractivity.AddUserBenchmarkPreference
                                 (SessionManager.SESSION.UserName, SelectedRow.GroupName.ToString(), addedBenchmarkReferenceData.BenchmarkName, addedBenchmarkReferenceData.BenchmarkReturnType, AddUserBenchmarkPreferenceCallbackMethod);
                         }
                     }
                 };                
            }
            else
            { MessageBox.Show("add groups first"); }
        }

        private void DeleteBenchmarkfromGroupCommandMethod(object param)
        {
            if (SelectedRow != null)
            {
                _dbInteractivity.DeleteUserBenchmarkPreference(SessionManager.SESSION.UserName, SelectedRow.GroupName, SelectedRow.BenchmarkName, (result) =>
                    {
                        if (result)
                        {
                            List<BenchmarkReferenceData> tempList = BenchmarkGroupedData.Where(t => t.GroupName == SelectedRow.GroupName).ToList();
                            if (tempList != null)
                            {
                                if (tempList.Count == 1)
                                {
                                    BenchmarkGroupedData.Remove(SelectedRow);
                                    BenchmarkGroupedData.Add(new BenchmarkReferenceData()
                                    {
                                        GroupName = SelectedRow.GroupName
                                    });
                                }
                                else
                                    BenchmarkGroupedData.Remove(SelectedRow);
                            }
                        }
                    });
            }
        }

        #endregion

        #region Callback Methods

        private void RetrieveUserPreferenceBenchmarkDataCallBackMethod(List<BenchmarkReferenceData> result)
        {
            
            BenchmarkGroupedData = new ObservableCollection<BenchmarkReferenceData>(result);
            if (BenchmarkGroupedData.Count == 0)
            {
                BenchmarkGroupedData.Add(new BenchmarkReferenceData()
                {
                });
            }
        }

        private void RetrieveBenchmarkReferenceDataCallBackMethod(List<BenchmarkReferenceData> result)
        {
            AllBenchmarks = new ObservableCollection<BenchmarkReferenceData>(result);
        }

        private void DeleteUserGroupPreferenceCallbackMethod(bool result)
        {
            if (result)
            {
                BenchmarkGroupedData =
                    new ObservableCollection<BenchmarkReferenceData>(BenchmarkGroupedData.Where(i => i.GroupName != SelectedRow.GroupName).ToList());
                if (BenchmarkGroupedData.Count == 0)
                    BenchmarkGroupedData.Add(new BenchmarkReferenceData()
                    {

                    });
            }
        }


        private void AddUserBenchmarkPreferenceCallbackMethod(bool result)
        {
            if (result)
            {
                if (SelectedRow.BenchmarkName == null)
                {
                    BenchmarkGroupedData.Remove(SelectedRow);
                    BenchmarkGroupedData.Add(addedBenchmarkReferenceData);
                }
                else
                {
                    BenchmarkGroupedData.Add(addedBenchmarkReferenceData);
                }
            }
        }

        #endregion

       
    }

}

