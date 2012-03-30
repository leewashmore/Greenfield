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
using Microsoft.Practices.Prism.Logging;

namespace GreenField.Benchmark.ViewModel
{
    /// <summary>
    /// view modelclass for Morning Snapshot
    /// </summary>
    [Export(typeof(ViewModelMorningSnapshot))]
    public class ViewModelMorningSnapshot : NotificationObject
    {
        #region PrivateFields
        //MEF Singletons
        private IDBInteractivity _dbInteractivity;
        private IManageSessions _manageSessions;
        private ILoggerFacade _logger;

        private string _benchmarkGroup = String.Empty;
        private UserBenchmarkPreference _selectedUserBenchmarkPreference;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dbInteractivity">IDBInteractivity Service</param>
        /// <param name="manageSessions">IManageSessions Service</param>
        /// <param name="logger">ILoggerFacade Service</param>
        [ImportingConstructor]
        public ViewModelMorningSnapshot(IDBInteractivity dbInteractivity, IManageSessions manageSessions, ILoggerFacade logger)
        {
            _dbInteractivity = dbInteractivity;
            _manageSessions = manageSessions;
            _logger = logger;

            _manageSessions.GetSession((session) =>
                {
                    _dbInteractivity.RetrieveBenchmarkSelectionData(RetrieveBenchmarkSelectionDataCallBackMethod);
                    _dbInteractivity.RetrieveUserPreferenceBenchmarkData(session.UserName, RetrieveUserPreferenceBenchmarkDataCallBackMethod);                    
                });
        }       

        #endregion

        #region Properties

        #region UI Fields

        /// <summary>
        /// Benchmark Selection Data
        /// </summary>
        private ObservableCollection<BenchmarkSelectionData> _benchmarkSelectionInfo;
	    public ObservableCollection<BenchmarkSelectionData> BenchmarkSelectionInfo
	    {
		    get { return _benchmarkSelectionInfo;}
            set
            {
                if (_benchmarkSelectionInfo != value)
                {
                    _benchmarkSelectionInfo = value;
                    RaisePropertyChanged(() => this.BenchmarkSelectionInfo);
                }
            }
	    }

        /// <summary>
        /// Morning Snapshot Data - based on user preference
        /// </summary>
        private ObservableCollection<MorningSnapshotData> _morningSnapshotInfo;
        public ObservableCollection<MorningSnapshotData> MorningSnapshotInfo
        {
            get { return _morningSnapshotInfo; }
            set
            {
                if (_morningSnapshotInfo != value)
                {
                    _morningSnapshotInfo = value;
                    RaisePropertyChanged(() => MorningSnapshotInfo);
                }
            }
        }

        /// <summary>
        /// Selected Morning Snapshot Row - based on selection
        /// </summary>
        private MorningSnapshotData _selectedMorningSnapshotRow;
        public MorningSnapshotData SelectedMorningSnapshotRow
        {
            get { return _selectedMorningSnapshotRow; }
            set
            {
                if (_selectedMorningSnapshotRow != value)
                {
                    _selectedMorningSnapshotRow = value;
                    RaisePropertyChanged(() => this.SelectedMorningSnapshotRow);
                    RefreshContextMenu();
                }
            }
        }

        /// <summary>
        /// Morning Snapshot Header - includes dynamic snapshot date
        /// </summary>
        public string MorningSnapshotHeader
        {
            get { return "Morning Snapshot (" + DateTime.Today.ToLongDateString() + ") - Emerging Markets"; }
        }   

        #endregion

        #region ICommand

        public ICommand AddBenchmarkGroupCommand
        {
            get
            {
                return new DelegateCommand<object>(AddBenchmarkGroupCommandMethod);
            }
        }

        public ICommand RemoveBenchmarkGroupCommand
        {
            get
            {
                return new DelegateCommand<object>(RemoveBenchmarkGroupCommandMethod, RemoveBenchmarkGroupCommandValidation);
            }
        }

        public ICommand AddBenchmarkToGroupCommand
        {
            get
            {
                return new DelegateCommand<object>(AddBenchmarkToGroupCommandMethod, AddBenchmarkToGroupCommandValidation);
            }
        }

        public ICommand RemoveBenchmarkfromGroupCommand
        {
            get
            {
                return new DelegateCommand<object>(RemoveBenchmarkfromGroupCommandMethod, RemoveBenchmarkFromGroupCommandValidation);
            }
        }

        #endregion

        #endregion

        #region ICommandMethods

        /// <summary>
        /// AddBenchmarkGroupCommand Execution method - adds new group to the morning snapshot grid
        /// </summary>
        /// <param name="param">Sender</param>
        private void AddBenchmarkGroupCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                //Get existing group names
                List<string> groupNames = MorningSnapshotInfo.Count == 1 ? 
                    ( MorningSnapshotInfo[0].MorningSnapshotPreferenceInfo == null ? null : MorningSnapshotInfo.Select(i=>i.MorningSnapshotPreferenceInfo.GroupName).Distinct().ToList() )
                     : MorningSnapshotInfo.Select(i => i.MorningSnapshotPreferenceInfo.GroupName).Distinct().ToList();
                
                ChildAddNewGroup childAddNewGroup = new ChildAddNewGroup("Add New Group", groupNames);
                childAddNewGroup.Show();
                _benchmarkGroup = String.Empty;
                childAddNewGroup.Unloaded += (se, e) =>
                 {
                     if (childAddNewGroup.DialogResult == true)
                     {
                         _benchmarkGroup = childAddNewGroup.GroupName;
                         _dbInteractivity.AddUserPreferenceBenchmarkGroup(SessionManager.SESSION.UserName, _benchmarkGroup, AddUserPreferenceBenchmarkGroupCallBackMethod);
                     }
                 };
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);           
        }

        private bool RemoveBenchmarkGroupCommandValidation(object param)
        {
            return SelectedMorningSnapshotRow != null ? SelectedMorningSnapshotRow.MorningSnapshotPreferenceInfo != null : false;
        }

        private void RemoveBenchmarkGroupCommandMethod(object param)
        {
            if (MessageBox.Show("Remove Group - '" + _selectedMorningSnapshotRow.MorningSnapshotPreferenceInfo.GroupName + "'?","Remove Group", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                _dbInteractivity.RemoveUserPreferenceBenchmarkGroup(SessionManager.SESSION.UserName, _selectedMorningSnapshotRow.MorningSnapshotPreferenceInfo.GroupName, RemoveUserPreferenceBenchmarkGroupCallBackMethod); 
            }
        }

        private bool AddBenchmarkToGroupCommandValidation(object param)
        {
            return SelectedMorningSnapshotRow == null ? false : SelectedMorningSnapshotRow.MorningSnapshotPreferenceInfo == null ? false : SelectedMorningSnapshotRow.MorningSnapshotPreferenceInfo.GroupName != null;
        }

        /// <summary>
        /// AddBenchmarkToGroupCommand Execution method - adds benchmark to group as Total(Gross), Net Return or Price Return
        /// </summary>
        /// <param name="param"></param>
        private void AddBenchmarkToGroupCommandMethod(object param)
        {
            List<string> morningSnapshotBenchmarkNames = MorningSnapshotInfo.Select(p=>p.MorningSnapshotPreferenceInfo.BenchmarkName).Distinct().ToList();
            ChildAddBenchmarks childAddBenchmarks = new ChildAddBenchmarks(BenchmarkSelectionInfo.Where(b => !(morningSnapshotBenchmarkNames.Contains(b.Name))).ToList());
            childAddBenchmarks.Show();
            childAddBenchmarks.Unloaded += (se, e) =>
            {
                if (childAddBenchmarks.DialogResult == true)
                {
                    if (childAddBenchmarks.SelectedUserBenchmarkPreference != null)
                    {
                        _selectedUserBenchmarkPreference = childAddBenchmarks.SelectedUserBenchmarkPreference;
                        _selectedUserBenchmarkPreference.GroupName = SelectedMorningSnapshotRow.MorningSnapshotPreferenceInfo.GroupName;
                        _dbInteractivity.AddUserPreferenceBenchmark
                            (SessionManager.SESSION.UserName, _selectedUserBenchmarkPreference, AddUserPreferenceBenchmarkCallBackMethod);
                    }
                }
            };
        }

        private bool RemoveBenchmarkFromGroupCommandValidation(object param)
        {
            if (SelectedMorningSnapshotRow == null) return false;
            if (SelectedMorningSnapshotRow.MorningSnapshotPreferenceInfo == null) return false;
            if (SelectedMorningSnapshotRow.MorningSnapshotPreferenceInfo.GroupName == null) return false;
            return SelectedMorningSnapshotRow.MorningSnapshotPreferenceInfo.BenchmarkName != null;            
        }
        private void RemoveBenchmarkfromGroupCommandMethod(object param)
        {
            if (MessageBox.Show("Remove Benchmark - '" + _selectedMorningSnapshotRow.MorningSnapshotPreferenceInfo.BenchmarkName + "'?", "Remove Benchmark", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                _dbInteractivity.RemoveUserPreferenceBenchmark(SessionManager.SESSION.UserName, _selectedMorningSnapshotRow.MorningSnapshotPreferenceInfo, RemoveUserPreferenceBenchmarkCallBackMethod);
            }           
        }

        #endregion

        #region Callback Methods

        /// <summary>
        /// Callback method for RetrieveBenchmarkSelectionData Service call - Gets all Benchmark available for selection
        /// </summary>
        /// <param name="result">BenchmarkSelectionData</param>
        private void RetrieveBenchmarkSelectionDataCallBackMethod(List<BenchmarkSelectionData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    BenchmarkSelectionInfo = new ObservableCollection<BenchmarkSelectionData>(result);
                    RefreshContextMenu();                 
                }
                else
                {
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        /// <summary>
        /// Callback method for RetrieveUserPreferenceBenchmarkData Service call - Gets user preference and calls service RetrieveMorningSnapshotData based on result
        /// </summary>
        /// <param name="result">UserBenchmarkPreference</param>
        private void RetrieveUserPreferenceBenchmarkDataCallBackMethod(List<UserBenchmarkPreference> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    _dbInteractivity.RetrieveMorningSnapshotData(result, RetrieveMorningSnapshotDataCallBackMethod);
                    RefreshContextMenu();
                }
                else
                {
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);            
        }

        /// <summary>
        /// Callback method for RetrieveMorningSnapshotData Service call - Gets morning snapshot data based on user preference
        /// </summary>
        /// <param name="result">MorningSnapshotData</param>
        private void RetrieveMorningSnapshotDataCallBackMethod(List<MorningSnapshotData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    if (MorningSnapshotInfo == null)
                    {
                        MorningSnapshotInfo = new ObservableCollection<MorningSnapshotData>(result);
                        if (result.Count == 0)
                        {
                            MorningSnapshotInfo.Add(new MorningSnapshotData());
                        }                        
                    }
                    else
                    {
                        foreach(MorningSnapshotData record in result)
                        {
                            MorningSnapshotInfo.Add(record);
                        }
                    }
                    RefreshContextMenu();
                }
                else
                {
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace); 
            
            //if (BenchmarkGroupedData.Count == 0)
            //{
            //    BenchmarkGroupedData.Add(new MorningSnapshotData()
            //    {
            //    });
            //}
        }

        /// <summary>
        /// Callback method for AddUserPreferenceBenchmarkGroup Service call - creates a blank group
        /// </summary>
        /// <param name="result">True/False</param>
        private void AddUserPreferenceBenchmarkGroupCallBackMethod(bool result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != false)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    if (_selectedMorningSnapshotRow.MorningSnapshotPreferenceInfo == null)
                    {
                        MorningSnapshotInfo.Clear();                        
                    }

                    MorningSnapshotInfo.Add(new MorningSnapshotData()
                    {
                        MorningSnapshotPreferenceInfo = new UserBenchmarkPreference()
                        {
                            GroupName = _benchmarkGroup
                        }
                    });
                    RefreshContextMenu();
                    
                }
                else
                {
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace); 
        }

        /// <summary>
        /// Callback method for RemoveUserPreferenceBenchmarkGroup Service call - removes selected group
        /// </summary>
        /// <param name="result">True/False</param>
        private void RemoveUserPreferenceBenchmarkGroupCallBackMethod(bool result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != false)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    MorningSnapshotInfo = new ObservableCollection<MorningSnapshotData>
                        (MorningSnapshotInfo.Where(i => i.MorningSnapshotPreferenceInfo.GroupName != _selectedMorningSnapshotRow.MorningSnapshotPreferenceInfo.GroupName).ToList());
                    if (MorningSnapshotInfo.Count == 0)
                    {
                        MorningSnapshotInfo.Add(new MorningSnapshotData());
                    }
                    RefreshContextMenu();
                }
                else
                {
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);            
        }

        /// <summary>
        /// Callback method for AddUserPreferenceBenchmark Service call - adds benchmark to selected group
        /// </summary>
        /// <param name="result">True/False</param>
        private void AddUserPreferenceBenchmarkCallBackMethod(bool result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != false)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    if (_selectedMorningSnapshotRow.MorningSnapshotPreferenceInfo.BenchmarkName == null)
                    {
                        MorningSnapshotInfo.Remove(_selectedMorningSnapshotRow);                       
                    }
                    _dbInteractivity.RetrieveMorningSnapshotData(new List<UserBenchmarkPreference>{ _selectedUserBenchmarkPreference }, RetrieveMorningSnapshotDataCallBackMethod);
                    RefreshContextMenu();
                }
                else
                {
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);            
        }

        /// <summary>
        /// Callback method for AddUserPreferenceBenchmark Service call - adds benchmark to selected group
        /// </summary>
        /// <param name="result">True/False</param>
        private void RemoveUserPreferenceBenchmarkCallBackMethod(bool result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != false)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    int groupBenchmarkCount = MorningSnapshotInfo.Where(i => i.MorningSnapshotPreferenceInfo.GroupName == _selectedMorningSnapshotRow.MorningSnapshotPreferenceInfo.GroupName).Count();
                    if (groupBenchmarkCount == 1)
                    {
                        MorningSnapshotInfo.Add(new MorningSnapshotData()
                        {
                            MorningSnapshotPreferenceInfo = new UserBenchmarkPreference()
                            {
                                GroupName = _selectedMorningSnapshotRow.MorningSnapshotPreferenceInfo.GroupName
                            }
                        });
                    }
                    MorningSnapshotInfo.Remove(_selectedMorningSnapshotRow);
                    RefreshContextMenu();
                }
                else
                {
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        #endregion

        private void RefreshContextMenu()
        {
            RaisePropertyChanged(() => this.AddBenchmarkGroupCommand);
            RaisePropertyChanged(() => this.RemoveBenchmarkGroupCommand);
            RaisePropertyChanged(() => this.AddBenchmarkToGroupCommand);
            RaisePropertyChanged(() => this.RemoveBenchmarkfromGroupCommand);
        }
       
    }

}

