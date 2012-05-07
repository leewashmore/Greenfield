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
using GreenField.ServiceCaller.SecurityReferenceDefinitions;
using System.Collections.Generic;
using Microsoft.Practices.Prism.Commands;
using GreenField.Gadgets.Views;
using GreenField.Common;
using Microsoft.Practices.Prism.Logging;
using GreenField.ServiceCaller.SessionDefinitions;
using Microsoft.Practices.Prism.Events;
using GreenField.ServiceCaller.PerformanceDefinitions;

namespace GreenField.Gadgets.ViewModels
{
    /// <summary>
    /// view modelclass for Morning Snapshot
    /// </summary>[Export(typeof(ViewModelMorningSnapshot))]
    public class ViewModelMarketPerformanceSnapshot : NotificationObject
    {
        #region PrivateFields
        //MEF Singletons
        public IDBInteractivity _dbInteractivity;
        private IEventAggregator _eventAggregator;
        private IManageSessions _manageSessions;
        private ILoggerFacade _logger;

        private List<MarketSnapshotPreference> _createPreferenceInfo = new List<MarketSnapshotPreference>();
        private List<MarketSnapshotPreference> _updatePreferenceInfo = new List<MarketSnapshotPreference>();
        private List<MarketSnapshotPreference> _deletePreferenceInfo = new List<MarketSnapshotPreference>();
        private List<int> _deleteGroupInfo = new List<int>();
        private List<string> _createGroupInfo = new List<string>();
        private MarketSnapshotSelectionData _selectedLocalMarketSnapshotSelectionData = null;
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
            SelectedMarketSnapshotSelectionInfo = param.DashboardGadgetPayload.MarketSnapshotSelectionData;

            //Subscribe to MarketPerformanceSnapshotNameReferenceSetEvent to receive snapshot selection from shell
            if (_eventAggregator != null)
            {
                _eventAggregator.GetEvent<MarketPerformanceSnapshotReferenceSetEvent>().Subscribe(HandleMarketPerformanceSnapshotNameReferenceSetEvent);
                _eventAggregator.GetEvent<MarketPerformanceSnapshotActionEvent>().Subscribe(HandleMarketPerformanceSnapshotActionEvent); 
            }

            //RetrieveMarketSnapshotPreference Service Call
            if (SelectedMarketSnapshotSelectionInfo != null)                    
            {
                HandleMarketPerformanceSnapshotNameReferenceSetEvent(SelectedMarketSnapshotSelectionInfo);
            }
        }       
        #endregion

        #region Properties
        #region Snapshot Header
        public string MorningSnapshotHeader
        {
            get
            { 
                return "Market Performance Snapshot (" + DateTime.Today.ToShortDateString() + ")"; 
            }            
        } 
        #endregion        

        #region Entity Selection Data
        /// <summary>
        /// Entity selection data for securities, commodities, benchmarks/indecies
        /// </summary>
        private List<EntitySelectionData> _entitySelectionInfo;
        public List<EntitySelectionData> EntitySelectionInfo
        {
            get { return _entitySelectionInfo; }
            set { _entitySelectionInfo = value; }
        } 
        #endregion

        #region Populated MarketPerformanceSnapshot Data
        /// <summary>
        /// Stores cached MarketPerformanceSnapshotData received on every iteration
        /// Data retrieved from this object on reiteration to a particular snapshot by user
        /// </summary>
        private List<PopulatedMarketPerformanceSnapshotData> _populatedMarketPerformanceSnapshotInfo;
        public List<PopulatedMarketPerformanceSnapshotData> PopulatedMarketPerformanceSnapshotInfo
        {
            get 
            {
                if (_populatedMarketPerformanceSnapshotInfo == null)
                    _populatedMarketPerformanceSnapshotInfo = new List<PopulatedMarketPerformanceSnapshotData>();
                return _populatedMarketPerformanceSnapshotInfo; 
            }
            set { _populatedMarketPerformanceSnapshotInfo = value; }
        } 
        #endregion

        #region MarketSnapshot Selection Data
        /// <summary>
        /// Stores the selected MarketSnapshotSelectionData received from shell
        /// </summary>
        private MarketSnapshotSelectionData _selectedMarketSnapshotSelectionInfo;
        public MarketSnapshotSelectionData SelectedMarketSnapshotSelectionInfo
        {
            get { return _selectedMarketSnapshotSelectionInfo; }
            set { _selectedMarketSnapshotSelectionInfo = value; }
        }

        /// <summary>
        /// Stores the complete list of MarketSnapshotSelectionData created by user
        /// </summary>
        private List<MarketSnapshotSelectionData> _marketSnapshotSelectionInfo;
        public List<MarketSnapshotSelectionData> MarketSnapshotSelectionInfo
        {
            get { return _marketSnapshotSelectionInfo; }
            set { _marketSnapshotSelectionInfo = value; }
        } 
        #endregion

        #region Market Snapshot Preference
        /// <summary>
        /// Stores the original snapshot preference configuration for selected snapshot required for comparison before the changes are propagated to database
        /// </summary>
        private List<MarketSnapshotPreference> _marketSnapshotPreferenceOriginalInfo;
        public List<MarketSnapshotPreference> MarketSnapshotPreferenceOriginalInfo
        {
            get { return _marketSnapshotPreferenceOriginalInfo; }
            set { _marketSnapshotPreferenceOriginalInfo = value; }
        }

        /// <summary>
        /// Storessnapshot preference configuration for selected snapshot that is modified by user
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
        #endregion

        #region Market Performance Snapshot Data
        /// <summary>
        /// Stores Market performance data for selected snapshot received from service
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
                MarketSnapshotPreferenceInfo = value
                    .Select(record => record.MarketSnapshotPreferenceInfo)
                    .ToList();
            }
        }

        /// <summary>
        /// Stores the selected Market performance data for a specific snapshot entity
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

        #region Busy Indicator Notification Content
        /// <summary>
        /// Stores the message displayed over the busy indicator to notify user of the on going process
        /// </summary>
        private string _busyIndicatorContent;
        public string BusyIndicatorContent
        {
            get { return _busyIndicatorContent; }
            set
            {
                _busyIndicatorContent = value;
                RaisePropertyChanged(() => this.BusyIndicatorContent);
            }
        }         
        #endregion
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
                                    BusyIndicatorNotification(true, "Retrieving performance data for inserted entity ...");
                                    _dbInteractivity.RetrieveMarketPerformanceSnapshotData( new List<MarketSnapshotPreference> { insertedMarketSnapshotPreference }
                                        , RetrieveMarketPerformanceSnapshotDataByEntityCallbackMethod);
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
                        MarketSnapshotPreferenceInfo = MarketPerformanceSnapshotInfo
                            .Select(record => record.MarketSnapshotPreferenceInfo)
                            .ToList();
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
                                    BusyIndicatorNotification(true, "Retrieving performance data for inserted entity ...");
                                    _dbInteractivity.RetrieveMarketPerformanceSnapshotData(new List<MarketSnapshotPreference> { insertedMarketSnapshotPreference }
                                           , RetrieveMarketPerformanceSnapshotDataByEntityCallbackMethod);
                                    //_dbInteractivity.RetrieveMarketPerformanceSnapshotData(MarketSnapshotPreferenceInfo, RetrieveMarketPerformanceSnapshotDataCallbackMethod);
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
                    MarketSnapshotPreferenceInfo = MarketPerformanceSnapshotInfo
                            .Select(record => record.MarketSnapshotPreferenceInfo)
                            .ToList();
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

        #region Events
        public event DataRetrievalProgressIndicatorEventHandler SnapshotPerfromanceDataLoadedEvent;
        #endregion

        #region Event Handlers
        #region Snapshot Selection Change Event
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
                    SelectedMarketSnapshotSelectionInfo = result;

                    //If the selected snapshot has already been cached on client side no requirement of another service call
                    #region Client cache check
                    PopulatedMarketPerformanceSnapshotData PopulatedMarketPerformanceSnapshotOriginalInfo
                                    = PopulatedMarketPerformanceSnapshotInfo.Where(record => record.MarketSnapshotSelectionInfo == result).FirstOrDefault();
                    if (PopulatedMarketPerformanceSnapshotOriginalInfo != null)
                    {
                        BusyIndicatorNotification(true, "Retrieving performance data for selected snapshot ...");
                        MarketSnapshotPreferenceOriginalInfo = PopulatedMarketPerformanceSnapshotOriginalInfo
                            .MarketPerformanceSnapshotInfo
                            .Select(record => record.MarketSnapshotPreferenceInfo)
                            .ToList();
                        MarketSnapshotPreferenceInfo = MarketSnapshotPreferenceOriginalInfo;
                        MarketPerformanceSnapshotInfo = new ObservableCollection<MarketPerformanceSnapshotData>
                            (PopulatedMarketPerformanceSnapshotOriginalInfo.MarketPerformanceSnapshotInfo);
                        BusyIndicatorNotification(false);
                        return;
                    }
                    #endregion

                    #region RetrieveMarketSnapshotPreference Service Call
                    if (SessionManager.SESSION == null)
                    {
                        BusyIndicatorNotification(true, "Retrieving session details ...");
                        _manageSessions.GetSession((session) =>
                            {
                                string sessionMethodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
                                Logging.LogBeginMethod(_logger, sessionMethodNamespace);
                                if (session != null)
                                {
                                    Logging.LogMethodParameter(_logger, sessionMethodNamespace, session, 1);
                                    SessionManager.SESSION = session;

                                    if (_dbInteractivity != null && SelectedMarketSnapshotSelectionInfo != null)
                                    {
                                        BusyIndicatorNotification(true, "Retrieving preference structure for selected snapshot ...");
                                        _dbInteractivity.RetrieveMarketSnapshotPreference(session.UserName
                                            , SelectedMarketSnapshotSelectionInfo.SnapshotName, RetrieveMarketSnapshotPreferenceCallbackMethod);
                                    }
                                }
                                else
                                {
                                    Logging.LogMethodParameterNull(_logger, sessionMethodNamespace, 1);
                                    BusyIndicatorNotification();
                                }
                                Logging.LogEndMethod(_logger, sessionMethodNamespace);
                            });
                    }
                    else
                    {
                        if (_dbInteractivity != null && SelectedMarketSnapshotSelectionInfo != null)
                        {
                            BusyIndicatorNotification(true, "Retrieving preference structure for selected snapshot ...");
                            _dbInteractivity.RetrieveMarketSnapshotPreference(SessionManager.SESSION.UserName
                                , SelectedMarketSnapshotSelectionInfo.SnapshotName, RetrieveMarketSnapshotPreferenceCallbackMethod);
                        }
                    }
                    #endregion
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

        #region Snapshot Action Events
        public void HandleMarketPerformanceSnapshotActionEvent(MarketPerformanceSnapshotActionPayload result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    MarketSnapshotSelectionInfo = result.MarketSnapshotSelectionInfo;
                    SelectedMarketSnapshotSelectionInfo = result.SelectedMarketSnapshotSelectionIndo;
                    switch (result.ActionType)
                    {
                        case MarketPerformanceSnapshotActionType.SNAPSHOT_SAVE:
                            HandleMarketPerformanceSnapshotSaveActionEvent();
                            break;
                        case MarketPerformanceSnapshotActionType.SNAPSHOT_SAVE_AS:
                            HandleMarketPerformanceSnapshotSaveAsActionEvent();
                            break;
                        case MarketPerformanceSnapshotActionType.SNAPSHOT_ADD:
                            HandleMarketPerformanceSnapshotAddActionEvent();
                            break;
                        case MarketPerformanceSnapshotActionType.SNAPSHOT_REMOVE:
                            HandleMarketPerformanceSnapshotRemoveActionEvent();
                            break;
                        default:
                            break;
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
        /// MarketPerformanceSnapshotSaveAction Event Handler
        /// </summary>
        private void HandleMarketPerformanceSnapshotSaveActionEvent()
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                BusyIndicatorNotification(true, "Retrieving preference changes ...");
                GetMarketSnapshotPreferenceCRUDInfo();

                int snapshotId = SelectedMarketSnapshotSelectionInfo.SnapshotPreferenceId;

                if (SessionManager.SESSION == null)
                {
                    BusyIndicatorNotification(true, "Retrieving session details ...");
                    _manageSessions.GetSession((session) =>
                        {
                            string sessionMethodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
                            Logging.LogBeginMethod(_logger, sessionMethodNamespace);
                            try
                            {
                                if (session != null)
                                {
                                    Logging.LogMethodParameter(_logger, sessionMethodNamespace, session, 1);
                                    SessionManager.SESSION = session;
                                    if (_dbInteractivity != null)
                                    {
                                        BusyIndicatorNotification(true, "Updating preference changes ...");
                                        _selectedLocalMarketSnapshotSelectionData = SelectedMarketSnapshotSelectionInfo;
                                        _dbInteractivity.SaveMarketSnapshotPreference(SessionManager.SESSION.UserName, SelectedMarketSnapshotSelectionInfo
                                            , _createPreferenceInfo, _updatePreferenceInfo, _deletePreferenceInfo, _deleteGroupInfo, _createGroupInfo
                                            , SaveMarketSnapshotPreferenceCallbackMethod);
                                    }
                                }
                                else
                                {
                                    Logging.LogMethodParameterNull(_logger, sessionMethodNamespace, 1);
                                    BusyIndicatorNotification();
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                                Logging.LogException(_logger, ex);
                                BusyIndicatorNotification();
                            }
                            Logging.LogEndMethod(_logger, sessionMethodNamespace);
                        });
                }
                else
                {
                    BusyIndicatorNotification(true, "Updating preference changes ...");
                    _dbInteractivity.SaveMarketSnapshotPreference(SessionManager.SESSION.UserName, SelectedMarketSnapshotSelectionInfo
                        , _createPreferenceInfo, _updatePreferenceInfo, _deletePreferenceInfo, _deleteGroupInfo, _createGroupInfo
                        , SaveMarketSnapshotPreferenceCallbackMethod);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
                BusyIndicatorNotification();
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        /// <summary>
        /// MarketPerformanceSnapshotSaveAsAction Event Handler
        /// </summary>
        private void HandleMarketPerformanceSnapshotSaveAsActionEvent()
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                ChildViewInsertSnapshot childViewInsertSnapshot = new ChildViewInsertSnapshot(MarketSnapshotSelectionInfo);
                childViewInsertSnapshot.Show();
                childViewInsertSnapshot.Unloaded += new RoutedEventHandler(childViewInsertSnapshot_Unloaded_SaveAs);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        /// <summary>
        ///  MarketPerformanceSnapshotAddAction Event Handler
        /// </summary>
        private void HandleMarketPerformanceSnapshotAddActionEvent()
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                ChildViewInsertSnapshot childViewInsertSnapshot = new ChildViewInsertSnapshot(MarketSnapshotSelectionInfo);
                childViewInsertSnapshot.Show();
                childViewInsertSnapshot.Unloaded += new RoutedEventHandler(childViewInsertSnapshot_Unloaded_Add);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        private void HandleMarketPerformanceSnapshotRemoveActionEvent()
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (MessageBox.Show("Remove Snapshot - '" + SelectedMarketSnapshotSelectionInfo.SnapshotName + "' ?", "", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    if (_dbInteractivity != null)
                    {
                        if (SessionManager.SESSION != null)
                        {
                            BusyIndicatorNotification(true, "Removing selected snapshot ...");
                            _dbInteractivity.RemoveMarketSnapshotPreference(SessionManager.SESSION.UserName, SelectedMarketSnapshotSelectionInfo.SnapshotName, RemoveMarketSnapshotPreferenceCallbackMethod);
                        }
                        else
                        {
                            BusyIndicatorNotification(true, "Retreiving session details ...");
                            _manageSessions.GetSession((session) =>
                            {
                                string sessionMethodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
                                Logging.LogBeginMethod(_logger, sessionMethodNamespace);
                                try
                                {
                                    if (session != null)
                                    {
                                        Logging.LogMethodParameter(_logger, sessionMethodNamespace, session, 1);
                                        SessionManager.SESSION = session;

                                        BusyIndicatorNotification(true, "Removing selected snapshot ...");
                                        _dbInteractivity.RemoveMarketSnapshotPreference(SessionManager.SESSION.UserName, SelectedMarketSnapshotSelectionInfo.SnapshotName, RemoveMarketSnapshotPreferenceCallbackMethod);
                                    }
                                    else
                                    {
                                        Logging.LogMethodParameterNull(_logger, sessionMethodNamespace, 1);
                                        BusyIndicatorNotification();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                                    Logging.LogException(_logger, ex);
                                }
                                Logging.LogEndMethod(_logger, sessionMethodNamespace);

                            });
                        }
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
        #endregion

        /// <summary>
        /// ChildViewInsertSnapshot Unloaded Event Handler for SaveAs Action
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">RoutedEventArgs</param>
        void childViewInsertSnapshot_Unloaded_SaveAs(object sender, RoutedEventArgs e)
        {
            string childUnloadedMethodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, childUnloadedMethodNamespace);
            try
            {
                ChildViewInsertSnapshot view = sender as ChildViewInsertSnapshot;
                if (view.DialogResult == true)
                {
                    if (view.tbSnapshotName.Text != String.Empty)
                    {
                        if (_dbInteractivity != null)
                        {
                            if (SessionManager.SESSION != null)
                            {
                                BusyIndicatorNotification(true, "Retreiving performance data based on snapshot preference ...");
                                _dbInteractivity.SaveAsMarketSnapshotPreference(SessionManager.SESSION.UserName
                                    , view.tbSnapshotName.Text, MarketSnapshotPreferenceInfo, SaveAsMarketSnapshotPreferenceCallbackMethod);
                            }
                            else
                            {
                                BusyIndicatorNotification(true, "Retreiving session details ...");
                                _manageSessions.GetSession((session) =>
                                {
                                    string sessionMethodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
                                    Logging.LogBeginMethod(_logger, sessionMethodNamespace);
                                    try
                                    {
                                        if (session != null)
                                        {
                                            Logging.LogMethodParameter(_logger, sessionMethodNamespace, session, 1);
                                            SessionManager.SESSION = session;

                                            BusyIndicatorNotification(true, "Retreiving performance data based on snapshot preference ...");
                                            _dbInteractivity.SaveAsMarketSnapshotPreference(session.UserName
                                                , view.tbSnapshotName.Text, MarketSnapshotPreferenceInfo, SaveAsMarketSnapshotPreferenceCallbackMethod);
                                        }
                                        else
                                        {
                                            Logging.LogMethodParameterNull(_logger, sessionMethodNamespace, 1);
                                            BusyIndicatorNotification();
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                                        Logging.LogException(_logger, ex);
                                    }
                                    Logging.LogEndMethod(_logger, sessionMethodNamespace);

                                });
                            }
                        }
                    }
                }

                (sender as ChildViewInsertSnapshot).Unloaded -= new RoutedEventHandler(childViewInsertSnapshot_Unloaded_SaveAs);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
                BusyIndicatorNotification();
            }
            Logging.LogEndMethod(_logger, childUnloadedMethodNamespace);
        }

        /// <summary>
        /// ChildViewInsertSnapshot Unloaded Event Handler for Add Action
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">RoutedEventArgs</param>
        void childViewInsertSnapshot_Unloaded_Add(object sender, RoutedEventArgs e)
        {
            string childUnloadedMethodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, childUnloadedMethodNamespace);
            try
            {
                ChildViewInsertSnapshot view = sender as ChildViewInsertSnapshot;
                if (view.DialogResult == true)
                {
                    if (view.tbSnapshotName.Text != String.Empty)
                    {
                        if (_dbInteractivity != null)
                        {
                            MarketSnapshotPreferenceInfo = new List<MarketSnapshotPreference>();
                            if (SessionManager.SESSION != null)
                            {
                                BusyIndicatorNotification(true, "Adding new snapshot ...");
                                _dbInteractivity.SaveAsMarketSnapshotPreference(SessionManager.SESSION.UserName
                                    , view.tbSnapshotName.Text, MarketSnapshotPreferenceInfo, AddMarketSnapshotPreferenceCallbackMethod);
                            }
                            else
                            {
                                BusyIndicatorNotification(true, "Retreiving session details ...");
                                _manageSessions.GetSession((session) =>
                                {
                                    string sessionMethodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
                                    Logging.LogBeginMethod(_logger, sessionMethodNamespace);
                                    try
                                    {
                                        if (session != null)
                                        {
                                            Logging.LogMethodParameter(_logger, sessionMethodNamespace, session, 1);
                                            SessionManager.SESSION = session;

                                            BusyIndicatorNotification(true, "Adding new snapshot ...");
                                            _dbInteractivity.SaveAsMarketSnapshotPreference(session.UserName
                                                , view.tbSnapshotName.Text, MarketSnapshotPreferenceInfo, AddMarketSnapshotPreferenceCallbackMethod);
                                        }
                                        else
                                        {
                                            Logging.LogMethodParameterNull(_logger, sessionMethodNamespace, 1);
                                            BusyIndicatorNotification();
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                                        Logging.LogException(_logger, ex);
                                        BusyIndicatorNotification();
                                    }
                                    Logging.LogEndMethod(_logger, sessionMethodNamespace);

                                });
                            }
                        }
                    }
                }

                (sender as ChildViewInsertSnapshot).Unloaded -= new RoutedEventHandler(childViewInsertSnapshot_Unloaded_Add);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
                BusyIndicatorNotification();
            }
            Logging.LogEndMethod(_logger, childUnloadedMethodNamespace);
        }
        #endregion

        #region Callback Methods
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
                        MarketSnapshotPreferenceOriginalInfo = result;                        
                        MarketSnapshotPreferenceInfo = result;

                        BusyIndicatorNotification(true, "Retreiving performance data based on snapshot preference ...");
                        _dbInteractivity.RetrieveMarketPerformanceSnapshotData(result, RetrieveMarketPerformanceSnapshotDataCallbackMethod);
                    }
                }
                else
                {
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                    BusyIndicatorNotification();
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

                    #region Client cache implementation
                    PopulatedMarketPerformanceSnapshotInfo.Add(new PopulatedMarketPerformanceSnapshotData()
                                {                                    
                                    MarketPerformanceSnapshotInfo = result,
                                    MarketSnapshotSelectionInfo = SelectedMarketSnapshotSelectionInfo
                                }); 
                    #endregion
                }
                else
                {
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                }

                BusyIndicatorNotification();
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
        private void RetrieveMarketPerformanceSnapshotDataByEntityCallbackMethod(List<MarketPerformanceSnapshotData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    MarketPerformanceSnapshotInfo.Add(result.FirstOrDefault());

                    #region Client cache implementation
                    PopulatedMarketPerformanceSnapshotData populatedMarketPerformanceSnapshotInfo = PopulatedMarketPerformanceSnapshotInfo
                        .Where(record => record.MarketSnapshotSelectionInfo == SelectedMarketSnapshotSelectionInfo).FirstOrDefault();

                    if (populatedMarketPerformanceSnapshotInfo != null)
                    {
                        PopulatedMarketPerformanceSnapshotInfo.Remove(populatedMarketPerformanceSnapshotInfo);
                        populatedMarketPerformanceSnapshotInfo.MarketPerformanceSnapshotInfo.Add(result.FirstOrDefault());
                        PopulatedMarketPerformanceSnapshotInfo.Add(populatedMarketPerformanceSnapshotInfo);                        
                    }                    
                    #endregion
                }
                else
                {
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                }

                BusyIndicatorNotification();
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
        private void SaveMarketSnapshotPreferenceCallbackMethod(List<MarketSnapshotPreference> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    
                    //Reassign the preference details to local properties
                    MarketSnapshotPreferenceOriginalInfo = result;
                    MarketSnapshotPreferenceInfo = result;

                    #region Client cache update
                    //Remove the snapshot entry and re add the preference details to the client cache
                    PopulatedMarketPerformanceSnapshotData PopulatedMarketPerformanceSnapshotOriginalInfo
                        = PopulatedMarketPerformanceSnapshotInfo
                        .Where(record => record.MarketSnapshotSelectionInfo == _selectedLocalMarketSnapshotSelectionData)
                        .FirstOrDefault();

                    if (PopulatedMarketPerformanceSnapshotOriginalInfo != null)
                    {
                        PopulatedMarketPerformanceSnapshotInfo.Remove(PopulatedMarketPerformanceSnapshotOriginalInfo);
                        PopulatedMarketPerformanceSnapshotInfo.Add(new PopulatedMarketPerformanceSnapshotData()
                        {
                            MarketPerformanceSnapshotInfo = MarketPerformanceSnapshotInfo.ToList(),
                            MarketSnapshotSelectionInfo = _selectedLocalMarketSnapshotSelectionData
                        });                        
                    }
                    #endregion                    
                    
                    //Raise event for completion of the save action event
                    _eventAggregator.GetEvent<MarketPerformanceSnapshotActionCompletionEvent>()
                        .Publish(new MarketPerformanceSnapshotActionPayload()
                        {
                            ActionType = MarketPerformanceSnapshotActionType.SNAPSHOT_SAVE,
                            SelectedMarketSnapshotSelectionIndo = _selectedLocalMarketSnapshotSelectionData,
                            MarketSnapshotSelectionInfo = MarketSnapshotSelectionInfo,
                        });

                    _selectedLocalMarketSnapshotSelectionData = null;
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
            BusyIndicatorNotification();
        }         

        /// <summary>
        /// Callback method for RetrieveEntitySelectionData Service call - Gets all Entity available for selection
        /// </summary>
        /// <param name="result">List of EntitySelectionData objects</param>
        public void RetrieveEntitySelectionDataCallbackMethod(List<EntitySelectionData> result)
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
            BusyIndicatorNotification();
            Logging.LogEndMethod(_logger, methodNamespace);            
        }

        /// <summary>
        /// Callback method for RetrieveEntitySelectionData Service call - Gets all Entity available for selection
        /// </summary>
        /// <param name="result">List of EntitySelectionData objects</param>
        private void SaveAsMarketSnapshotPreferenceCallbackMethod(MarketSnapshotSelectionData result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);

                    #region Client cache implementation
                    PopulatedMarketPerformanceSnapshotInfo.Add(new PopulatedMarketPerformanceSnapshotData()
                    {
                        MarketPerformanceSnapshotInfo = MarketPerformanceSnapshotInfo.ToList(),
                        MarketSnapshotSelectionInfo = result
                    });
                    #endregion

                    SelectedMarketSnapshotSelectionInfo = result;
                    MarketSnapshotSelectionInfo.Add(result);
                    MarketSnapshotPreferenceOriginalInfo = MarketSnapshotPreferenceInfo;

                    _eventAggregator.GetEvent<MarketPerformanceSnapshotActionCompletionEvent>()
                        .Publish(new MarketPerformanceSnapshotActionPayload()
                        {
                            ActionType = MarketPerformanceSnapshotActionType.SNAPSHOT_SAVE_AS,
                            SelectedMarketSnapshotSelectionIndo = result,
                            MarketSnapshotSelectionInfo = MarketSnapshotSelectionInfo
                        });
                    
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
            if (SnapshotPerfromanceDataLoadedEvent != null)
            {
                SnapshotPerfromanceDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = false });
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        /// <summary>
        /// Callback method for SaveAsMarketSnapshotPreference Service call - 
        /// </summary>
        /// <param name="result">Added snapshot details</param>
        private void AddMarketSnapshotPreferenceCallbackMethod(MarketSnapshotSelectionData result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    MarketPerformanceSnapshotInfo = new ObservableCollection<MarketPerformanceSnapshotData>();

                    #region Client cache implementation
                    PopulatedMarketPerformanceSnapshotInfo.Add(new PopulatedMarketPerformanceSnapshotData()
                    {
                        MarketPerformanceSnapshotInfo = MarketPerformanceSnapshotInfo.ToList(),
                        MarketSnapshotSelectionInfo = result
                    });
                    #endregion

                    SelectedMarketSnapshotSelectionInfo = result;
                    MarketSnapshotSelectionInfo.Add(result);
                    MarketSnapshotPreferenceOriginalInfo = MarketSnapshotPreferenceInfo;

                    _eventAggregator.GetEvent<MarketPerformanceSnapshotActionCompletionEvent>()
                        .Publish(new MarketPerformanceSnapshotActionPayload()
                        {
                            ActionType = MarketPerformanceSnapshotActionType.SNAPSHOT_ADD,
                            SelectedMarketSnapshotSelectionIndo = result,
                            MarketSnapshotSelectionInfo = MarketSnapshotSelectionInfo
                        });

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
            BusyIndicatorNotification();
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        private void RemoveMarketSnapshotPreferenceCallbackMethod(bool? result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);


                    #region Client cache update
                    //Remove the snapshot entry from the client cache
                    PopulatedMarketPerformanceSnapshotData PopulatedMarketPerformanceSnapshotOriginalInfo
                        = PopulatedMarketPerformanceSnapshotInfo
                        .Where(record => record.MarketSnapshotSelectionInfo == _selectedLocalMarketSnapshotSelectionData)
                        .FirstOrDefault();

                    if (PopulatedMarketPerformanceSnapshotOriginalInfo != null)
                    {
                        PopulatedMarketPerformanceSnapshotInfo.Remove(PopulatedMarketPerformanceSnapshotOriginalInfo);                        
                    }
                    #endregion                    

                    MarketSnapshotSelectionInfo.Remove(_selectedLocalMarketSnapshotSelectionData);
                    _selectedLocalMarketSnapshotSelectionData = null;

                    _eventAggregator.GetEvent<MarketPerformanceSnapshotActionCompletionEvent>()
                        .Publish(new MarketPerformanceSnapshotActionPayload()
                        {
                            ActionType = MarketPerformanceSnapshotActionType.SNAPSHOT_REMOVE,
                            SelectedMarketSnapshotSelectionIndo = null,
                            MarketSnapshotSelectionInfo = MarketSnapshotSelectionInfo
                        });

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
            BusyIndicatorNotification();
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

        private void GetMarketSnapshotPreferenceCRUDInfo()
        {
            //Get createPreferenceInfo
            List<MarketSnapshotPreference> crtPreferenceInfo = new List<MarketSnapshotPreference>();
            List<MarketSnapshotPreference> updPreferenceInfo = new List<MarketSnapshotPreference>();
            List<MarketSnapshotPreference> delPreferenceInfo = new List<MarketSnapshotPreference>();
            List<int> delGroupInfo = new List<int>();
            List<string> crtGroupInfo = new List<string>();

            //Browse through updated preference list to find changes
            foreach (MarketSnapshotPreference preference in MarketSnapshotPreferenceInfo)
            {
                if (MarketSnapshotPreferenceOriginalInfo
                        .Where(record => record.GroupPreferenceID == preference.GroupPreferenceID)
                        .Count().Equals(0))
                {
                    
                    if (!(crtGroupInfo.Contains(preference.GroupName)))
                    {
                        
                        crtGroupInfo.Add(preference.GroupName);
                    }                    
                }

                if (MarketSnapshotPreferenceOriginalInfo
                    .Where(record => record.EntityPreferenceId == preference.EntityPreferenceId)
                    .Count().Equals(0))
                {
                    crtPreferenceInfo.Add(preference);
                    continue;
                }

                if ( MarketSnapshotPreferenceOriginalInfo
                    .Where(record => record.EntityPreferenceId == preference.EntityPreferenceId
                        && record.EntityOrder == preference.EntityOrder
                        && record.GroupPreferenceID == preference.GroupPreferenceID)
                    .Count().Equals(0))
                {
                    updPreferenceInfo.Add(preference);                    
                }
                
            }

            if (MarketSnapshotPreferenceOriginalInfo.Count > MarketSnapshotPreferenceInfo.Count)
            {
                foreach (MarketSnapshotPreference preference in MarketSnapshotPreferenceOriginalInfo)
                {
                    if (MarketSnapshotPreferenceInfo
                        .Where(record => record.GroupPreferenceID == preference.GroupPreferenceID)
                        .Count().Equals(0))
                    {
                        if (!(delGroupInfo.Contains(preference.GroupPreferenceID)))
                        {
                            delGroupInfo.Add(preference.GroupPreferenceID);
                        }
                        continue;
                    }

                    if (MarketSnapshotPreferenceInfo
                        .Where(record => record.EntityPreferenceId == preference.EntityPreferenceId)
                        .Count().Equals(0))
                    {
                        delPreferenceInfo.Add(preference);
                    }
                }
            }

            _createPreferenceInfo = crtPreferenceInfo;
            _updatePreferenceInfo = updPreferenceInfo;
            _deletePreferenceInfo = delPreferenceInfo;
            _deleteGroupInfo = delGroupInfo;
            _createGroupInfo = crtGroupInfo;
        }

        public void BusyIndicatorNotification(bool showBusyIndicator = false, String message = null)
        {
            if (message != null)
                BusyIndicatorContent = message;
            if (SnapshotPerfromanceDataLoadedEvent != null)
            {
                SnapshotPerfromanceDataLoadedEvent(new DataRetrievalProgressIndicatorEventArgs() { ShowBusy = showBusyIndicator });
            }
        }

        #endregion
    }

    public class PopulatedMarketPerformanceSnapshotData
    {
        /// <summary>
        /// Stores the snapshot selection data with reference to the snapshot credentials
        /// </summary>
        public MarketSnapshotSelectionData MarketSnapshotSelectionInfo { get; set; }

        /// <summary>
        /// Stores the performance data for the snapshot selection data
        /// </summary>
        public List<MarketPerformanceSnapshotData> MarketPerformanceSnapshotInfo { get; set; }
    }

}

