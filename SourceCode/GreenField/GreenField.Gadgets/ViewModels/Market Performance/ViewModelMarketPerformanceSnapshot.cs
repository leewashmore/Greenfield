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
using GreenField.Gadgets.Views;
using GreenField.Common;
using Microsoft.Practices.Prism.Logging;
using GreenField.ServiceCaller.SessionDefinitions;
using Microsoft.Practices.Prism.Events;
using GreenField.ServiceCaller.BenchmarkHoldingsPerformanceDefinitions;

namespace GreenField.Gadgets.ViewModels
{
    /// <summary>
    /// view modelclass for Morning Snapshot
    /// </summary>[Export(typeof(ViewModelMorningSnapshot))]
    public class ViewModelMarketPerformanceSnapshot : NotificationObject
    {
        #region PrivateFields
        //MEF Singletons
        private IDBInteractivity _dbInteractivity;
        private IEventAggregator _eventAggregator;
        private IManageSessions _manageSessions;
        private ILoggerFacade _logger;

        private string _benchmarkGroup = String.Empty;
        private MarketSnapshotSelectionData _selectedMarketSnapshotPreference;
        
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dbInteractivity">IDBInteractivity Service</param>
        /// <param name="manageSessions">IManageSessions Service</param>
        /// <param name="logger">ILoggerFacade Service</param>
        public ViewModelMarketPerformanceSnapshot(DashboardGadgetParam param)
        {
            _dbInteractivity = param.DBInteractivity;
            _manageSessions = param.ManageSessions;
            _eventAggregator = param.EventAggregator;
            _logger = param.LoggerFacade;
            _selectedMarketSnapshotPreference = param.DashboardGadgetPayload.MarketSnapshotSelectionData;

            //Subscribe to MarketPerformanceSnapshotNameReferenceSetEvent to receive snapshot selection from shell
            _eventAggregator.GetEvent<MarketPerformanceSnapshotNameReferenceSetEvent>().Subscribe(HandleMarketPerformanceSnapshotNameReferenceSetEvent);

            //RetrieveEntitySelectionData Service Call
            if (_dbInteractivity != null)
            {
                _dbInteractivity.RetrieveEntitySelectionData(RetrieveEntitySelectionDataCallbackMethod);
            }

            //RetrieveMarketSnapshotPreference Service Call
            //if (SessionManager.SESSION == null)
            //{
            //    _manageSessions.GetSession(GetSessionCallbackMethod);                   
            //}
            //else
            //{
            //    if (_dbInteractivity != null && _selectedMarketSnapshotPreference != null)
            //    {
            //        _dbInteractivity.RetrieveMarketSnapshotPreference(SessionManager.SESSION.UserName
            //            , _selectedMarketSnapshotPreference.SnapshotName, RetrieveMarketSnapshotPreferenceCallbackMethod);
            //    }
            //} 
            _dbInteractivity.RetrieveMarketSnapshotPreference("rvig"
                        , "snap4", RetrieveMarketSnapshotPreferenceCallbackMethod);
        }       
        #endregion

        #region Properties
        /// <summary>
        /// Entity selection data for securities, commodities, benchmarks and indecies
        /// </summary>
        private List<EntitySelectionData> _entitySelectionInfo;
        public List<EntitySelectionData> EntitySelectionInfo
        {
            get { return _entitySelectionInfo; }
            set { _entitySelectionInfo = value; }
        }

        /// <summary>
        /// Market performance preference data for selected snapshot
        /// </summary>
        private List<MarketSnapshotPreference> _marketSnapshotPreferenceInfo;
        public List<MarketSnapshotPreference> MarketSnapshotPreferenceInfo
        {
            get
            {
                if (_marketSnapshotPreferenceInfo == null)
                {
                    _marketSnapshotPreferenceInfo = new List<MarketSnapshotPreference>();
                }
                return _marketSnapshotPreferenceInfo;
            }
            set
            {
                _marketSnapshotPreferenceInfo = value;
                RaisePropertyChanged(() => this.MarketSnapshotPreferenceInfo);
            }
        }

        /// <summary>
        /// Market performance data for selected snapshot
        /// </summary>
        private ObservableCollection<MarketPerformanceSnapshotData> _marketPerformanceSnapshotInfo;
        public ObservableCollection<MarketPerformanceSnapshotData> MarketPerformanceSnapshotInfo
        {
            get
            {
                if (_marketPerformanceSnapshotInfo == null)
                {
                    _marketPerformanceSnapshotInfo = new ObservableCollection<MarketPerformanceSnapshotData>();
                }
                return _marketPerformanceSnapshotInfo;
            }
            set
            {
                _marketPerformanceSnapshotInfo = value;
                RaisePropertyChanged(() => this.MarketPerformanceSnapshotInfo);
            }
        }

        /// <summary>
        /// Market performance data for selected snapshot entity
        /// </summary>
        private MarketPerformanceSnapshotData _selectedMarketPerformanceSnapshotInfo;
        public MarketPerformanceSnapshotData SelectedMarketPerformanceSnapshotInfo
        {
            get { return _selectedMarketPerformanceSnapshotInfo; }
            set
            {
                _selectedMarketPerformanceSnapshotInfo = value;
                RaisePropertyChanged(() => this.SelectedMarketPerformanceSnapshotInfo);
            }
        } 
        #endregion        

        #region ICommand
        public ICommand AddEntityGroupCommand
        {
            get
            {
                return new DelegateCommand<object>(AddEntityGroupCommandMethod);
            }
        }

        public ICommand RemoveEntityGroupCommand
        {
            get
            {
                return new DelegateCommand<object>(RemoveEntityGroupCommandMethod);
            }
        }

        public ICommand AddEntityToGroupCommand
        {
            get
            {
                return new DelegateCommand<object>(AddEntityToGroupCommandMethod);
            }
        }

        public ICommand RemoveEntityFromGroupCommand
        {
            get
            {
                return new DelegateCommand<object>(RemoveEntityFromGroupCommandMethod);
            }
        }
        #endregion

        #region ICommandMethods
        /// <summary>
        /// AddEntityGroupCommand Execution method - adds new group to snapshot
        /// </summary>
        /// <param name="param">Sender</param>
        private void AddEntityGroupCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                //Get all group names present in the snapshot
                List<string> snapshotGroupNames = MarketPerformanceSnapshotInfo
                    .Select(i => i.MarketSnapshotPreferenceInfo.GroupName).Distinct().ToList();

                //Get all entities present in the snapshot
                List<string> snapshotEntityNames = MarketPerformanceSnapshotInfo
                    .Select(p => p.MarketSnapshotPreferenceInfo.EntityName).Distinct().ToList();

                //Cases Where EntitySelectionInfo Data Asychronous call thread is still active
                if (EntitySelectionInfo == null)
                    return;

                //Open Child Window to receive inserted entity details
                ChildViewInsertEntity childViewModelInsertEntity
                    = new ChildViewInsertEntity(EntitySelectionInfo
                                                        .Where(record => !(snapshotEntityNames.Contains(record.LongName)))
                                                        .ToList()
                                                , groupNames: snapshotGroupNames);
                childViewModelInsertEntity.Show();
                childViewModelInsertEntity.Unloaded += (se, e) =>
                {
                    #region childViewModelInsertEntity Unloaded Event Handler
                    string childUnloadedMethodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
                    Logging.LogBeginMethod(_logger, childUnloadedMethodNamespace);
                    try
                    {
                        if (childViewModelInsertEntity.DialogResult == true)
                        {
                            if (childViewModelInsertEntity.InsertedMarketSnapshotPreference != null)
                            {
                                //Create Preference object
                                MarketSnapshotPreference insertedMarketSnapshotPreference = childViewModelInsertEntity.InsertedMarketSnapshotPreference;
                                insertedMarketSnapshotPreference.GroupPreferenceID = GetLastGroupPreferenceId() + 1;
                                insertedMarketSnapshotPreference.EntityOrder = 1;                               

                                MarketSnapshotPreferenceInfo.Add(insertedMarketSnapshotPreference);

                                //Service call to receive Market Performance Snapshot Data
                                if (_dbInteractivity != null)
                                {
                                    _dbInteractivity.RetrieveMarketPerformanceSnapshotData(MarketSnapshotPreferenceInfo, RetrieveMarketPerformanceSnapshotDataCallbackMethod);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                        Logging.LogException(_logger, ex);
                    }
                    Logging.LogEndMethod(_logger, childUnloadedMethodNamespace);
                    #endregion
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        /// <summary>
        /// RemoveEntityGroupCommand Execution method - removes selected group from snapshot
        /// </summary>
        /// <param name="param">Sender</param>
        private void RemoveEntityGroupCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (MessageBox.Show("Remove Group - '" + SelectedMarketPerformanceSnapshotInfo.MarketSnapshotPreferenceInfo.GroupName
                    + "'?", "Remove Group", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    //Remove Entity from the snapshot
                    foreach (MarketPerformanceSnapshotData entry in MarketPerformanceSnapshotInfo
                        .Where(record => record.MarketSnapshotPreferenceInfo.GroupName
                            == SelectedMarketPerformanceSnapshotInfo.MarketSnapshotPreferenceInfo.GroupName)
                        .ToList())
                    {
                        MarketPerformanceSnapshotInfo.Remove(entry);
                    }             
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
        /// AddEntityToGroupCommand Execution method - adds entity to group and places it over the selected entity
        /// </summary>
        /// <param name="param"></param>
        private void AddEntityToGroupCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                //Get all entities present in the snapshot
                List<string> snapshotEntityNames = MarketPerformanceSnapshotInfo
                    .Select(p => p.MarketSnapshotPreferenceInfo.EntityName).Distinct().ToList();

                //Cases Where EntitySelectionInfo Data Asychronous call thread is still active
                if (EntitySelectionInfo == null)
                    return;

                //Open Child Window to receive inserted entity details
                ChildViewInsertEntity childViewModelInsertEntity
                    = new ChildViewInsertEntity(EntitySelectionInfo
                                                        .Where(record => !(snapshotEntityNames.Contains(record.LongName)))
                                                        .ToList()
                                                , SelectedMarketPerformanceSnapshotInfo.MarketSnapshotPreferenceInfo.GroupName);
                childViewModelInsertEntity.Show();
                childViewModelInsertEntity.Unloaded += (se, e) =>
                {
                    #region childViewModelInsertEntity Unloaded Event Handler
                    string childUnloadedMethodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
                    Logging.LogBeginMethod(_logger, childUnloadedMethodNamespace);
                    try
                    {
                        if (childViewModelInsertEntity.DialogResult == true)
                        {
                            if (childViewModelInsertEntity.InsertedMarketSnapshotPreference != null)
                            {
                                //Create Preference object
                                MarketSnapshotPreference insertedMarketSnapshotPreference = childViewModelInsertEntity.InsertedMarketSnapshotPreference;
                                insertedMarketSnapshotPreference.GroupName = SelectedMarketPerformanceSnapshotInfo.MarketSnapshotPreferenceInfo.GroupName;
                                insertedMarketSnapshotPreference.GroupPreferenceID = SelectedMarketPerformanceSnapshotInfo.MarketSnapshotPreferenceInfo.GroupPreferenceID;

                                //Rearrange Entity Orders
                                insertedMarketSnapshotPreference.EntityOrder = SelectedMarketPerformanceSnapshotInfo.MarketSnapshotPreferenceInfo.EntityOrder;
                                foreach (MarketSnapshotPreference entity in MarketSnapshotPreferenceInfo)
                                {
                                    if (entity.GroupPreferenceID != insertedMarketSnapshotPreference.GroupPreferenceID)
                                        continue;
                                    if (entity.EntityOrder < insertedMarketSnapshotPreference.EntityOrder)
                                        continue;
                                    entity.EntityOrder++;
                                }

                                MarketSnapshotPreferenceInfo.Add(insertedMarketSnapshotPreference);

                                //Service call to receive Market Performance Snapshot Data
                                if (_dbInteractivity != null)
                                {
                                    _dbInteractivity.RetrieveMarketPerformanceSnapshotData(MarketSnapshotPreferenceInfo, RetrieveMarketPerformanceSnapshotDataCallbackMethod);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                        Logging.LogException(_logger, ex);
                    }
                    Logging.LogEndMethod(_logger, childUnloadedMethodNamespace); 
                    #endregion
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        /// <summary>
        /// RemoveEntityFromGroupCommand Execution method - removes selected entity from group
        /// </summary>
        /// <param name="param"></param>
        private void RemoveEntityFromGroupCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (MessageBox.Show("Remove Entity - '" + SelectedMarketPerformanceSnapshotInfo.MarketSnapshotPreferenceInfo.EntityName + "'?", "Remove Entity", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    //Rearrange Entity Order
                    foreach (MarketSnapshotPreference entity in MarketSnapshotPreferenceInfo)
                    {
                        if (entity.GroupPreferenceID != SelectedMarketPerformanceSnapshotInfo.MarketSnapshotPreferenceInfo.GroupPreferenceID)
                            continue;
                        if (entity.EntityOrder < SelectedMarketPerformanceSnapshotInfo.MarketSnapshotPreferenceInfo.EntityOrder)
                            continue;
                        entity.EntityOrder--;
                    }

                    //Remove Entity from the snapshot
                    MarketPerformanceSnapshotInfo.Remove(SelectedMarketPerformanceSnapshotInfo);
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

        #region Event Handlers
        /// <summary>
        /// MarketPerformanceSnapshotNameReferenceSetEvent Handler
        /// </summary>
        /// <param name="result">MarketSnapshotSelectionData object as payload</param>
        public void HandleMarketPerformanceSnapshotNameReferenceSetEvent(MarketSnapshotSelectionData result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    _selectedMarketSnapshotPreference = result;
                    //RetrieveMarketSnapshotPreference Service Call
                    if (SessionManager.SESSION == null)
                    {
                        _manageSessions.GetSession(GetSessionCallbackMethod);
                    }
                    else
                    {
                        if (_dbInteractivity != null && _selectedMarketSnapshotPreference != null)
                        {
                            _dbInteractivity.RetrieveMarketSnapshotPreference(SessionManager.SESSION.UserName
                                , _selectedMarketSnapshotPreference.SnapshotName, RetrieveMarketSnapshotPreferenceCallbackMethod);
                        }
                    }
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

        #region Callback Methods
        /// <summary>
        /// Callback method for RetrieveMarketSnapshotPreference Service call - Gets user's Snapshot preference for the selected Snapshot
        /// </summary>
        /// <param name="result">List of MarketSnapshotPreference objects</param>
        private void GetSessionCallbackMethod(Session result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    SessionManager.SESSION = result;
                    if (_dbInteractivity != null && _selectedMarketSnapshotPreference != null)
                    {
                        _dbInteractivity.RetrieveMarketSnapshotPreference(result.UserName
                            , _selectedMarketSnapshotPreference.SnapshotName, RetrieveMarketSnapshotPreferenceCallbackMethod);
                    }
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
        /// Callback method for RetrieveMarketSnapshotPreference Service call - Gets user's Snapshot preference for the selected Snapshot
        /// </summary>
        /// <param name="result">List of MarketSnapshotPreference objects</param>
        private void RetrieveMarketSnapshotPreferenceCallbackMethod(List<MarketSnapshotPreference> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    if (_dbInteractivity != null)
                    {
                        MarketSnapshotPreferenceInfo = result;
                        _dbInteractivity.RetrieveMarketPerformanceSnapshotData(result, RetrieveMarketPerformanceSnapshotDataCallbackMethod);
                    }
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
        /// Callback method for RetrieveMarketPerformanceSnapshotData Service call - Gets performance data for entities enlisted in the selected snapshot
        /// </summary>
        /// <param name="result">List of MarketPerformanceSnapshotData objects</param>
        private void RetrieveMarketPerformanceSnapshotDataCallbackMethod(List<MarketPerformanceSnapshotData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);

                    MarketPerformanceSnapshotInfo = new ObservableCollection<MarketPerformanceSnapshotData>(result);
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
        /// Callback method for RetrieveEntitySelectionData Service call - Gets all Entity available for selection
        /// </summary>
        /// <param name="result">List of EntitySelectionData objects</param>
        private void RetrieveEntitySelectionDataCallbackMethod(List<EntitySelectionData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    
                    //Entity Selection Data for Currency is not required
                    EntitySelectionInfo = result.Where(record => record.Type != EntityType.CURRENCY).ToList();
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

        #region Helper Methods
        /// <summary>
        /// Get the GroupPreferenceId of the last entity
        /// </summary>
        /// <returns></returns>
        private int GetLastGroupPreferenceId()
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            int lastGroupPreferenceId = 0;
            try
            {
                foreach (MarketPerformanceSnapshotData record in MarketPerformanceSnapshotInfo)
                {
                    if (record.MarketSnapshotPreferenceInfo.GroupPreferenceID > lastGroupPreferenceId)
                    {
                        lastGroupPreferenceId = record.MarketSnapshotPreferenceInfo.GroupPreferenceID;
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
            return lastGroupPreferenceId;
        } 
        #endregion
    }

}

