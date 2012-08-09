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
using System.Xml.Linq;
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
using GreenField.Gadgets.Models;
using GreenField.DataContracts;
using GreenField.Gadgets.Helpers;
using GreenField.UserSession;


namespace GreenField.Gadgets.ViewModels
{
    /// <summary>
    /// view modelclass for Morning Snapshot
    /// </summary>[Export(typeof(ViewModelMorningSnapshot))]
    public class ViewModelMarketPerformanceSnapshot : NotificationObject
    {
        #region Fields
        //MEF Singletons
        private IEventAggregator _eventAggregator;
        private IManageSessions _manageSessions;
        private ILoggerFacade _logger;

        private List<MarketSnapshotPreference> _createPreferenceInfo = new List<MarketSnapshotPreference>();
        private List<MarketSnapshotPreference> _updatePreferenceInfo = new List<MarketSnapshotPreference>();
        private List<MarketSnapshotPreference> _deletePreferenceInfo = new List<MarketSnapshotPreference>();
        private List<int> _deleteGroupInfo = new List<int>();
        private List<string> _createGroupInfo = new List<string>();
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

            //RetrieveEntitySelectionData Service Call
            if (SelectionData.EntitySelectionData != null && EntitySelectionInfo == null)
            {
                BusyIndicatorNotification(true, "Retrieving Entity Selection Data ...");
                RetrieveEntitySelectionDataCallbackMethod(SelectionData.EntitySelectionData);
            }

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
        #region Service Caller IDBInteractivity
        /// <summary>
        /// Service Caller instance - to be used in class behind to invoke Entity Selection data retrieval after the constructor of view is invoked
        /// </summary>
        public IDBInteractivity _dbInteractivity { get; set; }
        #endregion

        #region Snapshot Header
        public string MorningSnapshotHeader
        {
            get
            {
                return "Market Performance Snapshot (" + DateTime.Today.ToString("MMMM d, yyyy") + ") - Emerging Markets";
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
        /// Data retrieved from this object on reiteration to propertyName particular snapshot by user
        /// </summary>
        private List<PopulatedMarketPerformanceSnapshotData> _populatedMarketPerformanceSnapshotOriginalInfo;
        public List<PopulatedMarketPerformanceSnapshotData> PopulatedMarketPerformanceSnapshotOriginalInfo
        {
            get
            {
                if (_populatedMarketPerformanceSnapshotOriginalInfo == null)
                    _populatedMarketPerformanceSnapshotOriginalInfo = new List<PopulatedMarketPerformanceSnapshotData>();
                return _populatedMarketPerformanceSnapshotOriginalInfo;
            }
            set { _populatedMarketPerformanceSnapshotOriginalInfo = value; }
        }

        /// <summary>
        /// Stores cached MarketPerformanceSnapshotData received on every iteration
        /// Data retrieved from this object on reiteration to propertyName particular snapshot by user
        /// </summary>
        private List<PopulatedMarketPerformanceSnapshotData> _populatedMarketPerformanceSnapshotInfo;
        public List<PopulatedMarketPerformanceSnapshotData> PopulatedMarketPerformanceSnapshotInfo
        {
            get
            {
                //if (_populatedMarketPerformanceSnapshotInfo == null)
                //    _populatedMarketPerformanceSnapshotInfo = new List<PopulatedMarketPerformanceSnapshotData>();
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
                //if (_marketSnapshotPreferenceInfo == null)
                //{
                //    _marketSnapshotPreferenceInfo = new List<MarketSnapshotPreference>();
                //}
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
                //if (_marketPerformanceSnapshotInfo == null)
                //{
                //    _marketPerformanceSnapshotInfo = new ObservableCollection<MarketPerformanceSnapshotData>();
                //}
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
        /// Stores the selected Market performance data for propertyName specific snapshot entity
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

        #region Busy Indicator Notification
        /// <summary>
        /// Displays/Hides busy indicator to notify user of the on going process
        /// </summary>
        private bool _busyIndicatorIsBusy = false;
        public bool BusyIndicatorIsBusy
        {
            get { return _busyIndicatorIsBusy; }
            set
            {
                _busyIndicatorIsBusy = value;
                RaisePropertyChanged(() => this.BusyIndicatorIsBusy);
            }
        }

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

        #region Gadget Active Check
        /// <summary>
        /// IsActive is true when parent control is displayed on UI
        /// </summary>
        private bool _isActive;
        public bool IsActive
        {
            get
            {
                return _isActive;
            }
            set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    if (_dbInteractivity != null && SelectedMarketSnapshotSelectionInfo != null && _isActive)
                    {
                        BusyIndicatorNotification(true, "Retrieving preference structure for selected snapshot ...");
                        _dbInteractivity.RetrieveMarketSnapshotPreference(SelectedMarketSnapshotSelectionInfo.SnapshotPreferenceId
                            , RetrieveMarketSnapshotPreferenceCallbackMethod);
                    }
                }
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
                //Cases Where EntitySelectionInfo Data Asychronous call thread is still active
                if (EntitySelectionInfo == null)
                {
                    if (SelectionData.EntitySelectionData != null)
                    {
                        BusyIndicatorNotification(true, "Retrieving Entity Selection Data ...");
                        RetrieveEntitySelectionDataCallbackMethod(SelectionData.EntitySelectionData);
                    }
                    return;
                }


                //Get all group names present in the snapshot
                List<string> snapshotGroupNames = new List<string>();
                if (MarketPerformanceSnapshotInfo != null)
                {
                    snapshotGroupNames = MarketPerformanceSnapshotInfo
                        .Select(i => i.MarketSnapshotPreferenceInfo.GroupName).Distinct().ToList();
                }

                ChildViewInsertEntity childViewModelInsertEntity
                    = new ChildViewInsertEntity(EntitySelectionInfo, _dbInteractivity, _logger, groupNames: snapshotGroupNames);
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
                                //Create Preference object - We assign propertyName new uncommitted GroupPreferenceId to the entity and place it with Entity Order 1
                                MarketSnapshotPreference insertedMarketSnapshotPreference = childViewModelInsertEntity.InsertedMarketSnapshotPreference;

                                if (MarketSnapshotPreferenceInfo != null)
                                {
                                    if (MarketSnapshotPreferenceInfo.Where(record => record.EntityId == insertedMarketSnapshotPreference.EntityId
                                                                && record.EntityName == insertedMarketSnapshotPreference.EntityName
                                                                && record.EntityNodeType == insertedMarketSnapshotPreference.EntityNodeType
                                                                && record.EntityNodeValueCode == insertedMarketSnapshotPreference.EntityNodeValueCode
                                                                && record.EntityNodeValueName == insertedMarketSnapshotPreference.EntityNodeValueName
                                                                && record.EntityReturnType == insertedMarketSnapshotPreference.EntityReturnType
                                                                && record.EntityType == insertedMarketSnapshotPreference.EntityType).FirstOrDefault() != null)
                                    {
                                        Prompt.ShowDialog("Entity already exists in the snapshot");
                                        BusyIndicatorNotification();
                                        return;
                                    }
                                }

                                insertedMarketSnapshotPreference.GroupPreferenceID = GetLastGroupPreferenceId() + 1;
                                insertedMarketSnapshotPreference.EntityOrder = 1;

                                //Add the inserted entity to MarketSnapshotPreferenceInfo 
                                _revertSnapshotPreference = insertedMarketSnapshotPreference;
                                _revertSnapshotChange = RevertAddEntityGroupCommandMethod;

                                if (MarketSnapshotPreferenceInfo == null)
                                {
                                    MarketSnapshotPreferenceInfo = new List<MarketSnapshotPreference>();
                                }
                                MarketSnapshotPreferenceInfo.Add(insertedMarketSnapshotPreference);

                                //Reorder MarketSnapshotPreferenceInfo by GroupPreferenceID and then by EntityOrder
                                MarketSnapshotPreferenceInfo = MarketSnapshotPreferenceInfo
                                    .OrderBy(record => record.GroupPreferenceID)
                                    .ThenBy(record => record.EntityOrder)
                                    .ToList();

                                TestEntityOrdering();

                                //Service call to receive Market Performance Snapshot Data
                                if (_dbInteractivity != null && IsActive)
                                {
                                    BusyIndicatorNotification(true, "Retrieving performance data for inserted entity ...");
                                    _dbInteractivity.RetrieveMarketPerformanceSnapshotData(new List<MarketSnapshotPreference> { insertedMarketSnapshotPreference }
                                        , RetrieveMarketPerformanceSnapshotDataByEntityCallbackMethod);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                        Logging.LogException(_logger, ex);
                        BusyIndicatorNotification();
                    }
                    Logging.LogEndMethod(_logger, childUnloadedMethodNamespace);
                    #endregion
                };
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        private void RevertAddEntityGroupCommandMethod(MarketSnapshotPreference preference)
        {
            MarketSnapshotPreferenceInfo.Remove(preference);

            MarketSnapshotPreferenceInfo = MarketSnapshotPreferenceInfo
                .OrderBy(record => record.GroupPreferenceID)
                .ThenBy(record => record.EntityOrder)
                .ToList();

            TestEntityOrdering();
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
                Prompt.ShowDialog("Remove Group - '" + SelectedMarketPerformanceSnapshotInfo.MarketSnapshotPreferenceInfo.GroupName
                    + "'?", "Remove Group", MessageBoxButton.OKCancel, (messageResult) =>
                {
                    if (messageResult == MessageBoxResult.OK)
                    {
                        BusyIndicatorNotification(true, "Removing selected group from the snapshot");
                        //Remove Entity from the snapshot                    
                        if (MarketPerformanceSnapshotInfo != null)
                        {
                            foreach (MarketPerformanceSnapshotData entry in MarketPerformanceSnapshotInfo
                                            .Where(record => record.MarketSnapshotPreferenceInfo.GroupName
                                                == SelectedMarketPerformanceSnapshotInfo.MarketSnapshotPreferenceInfo.GroupName)
                                            .ToList())
                            {
                                MarketPerformanceSnapshotInfo.Remove(entry);
                            }

                            MarketSnapshotPreferenceInfo = MarketPerformanceSnapshotInfo
                                    .Select(record => record.MarketSnapshotPreferenceInfo)
                                    .ToList();
                        }

                        #region Client cache implementation
                        if (PopulatedMarketPerformanceSnapshotInfo != null)
                        {
                            PopulatedMarketPerformanceSnapshotData selectedPopulatedMarketPerformanceSnapshotInfo = PopulatedMarketPerformanceSnapshotInfo
                                                .Where(record => record.MarketSnapshotSelectionInfo == SelectedMarketSnapshotSelectionInfo).FirstOrDefault();

                            if (selectedPopulatedMarketPerformanceSnapshotInfo != null)
                            {
                                PopulatedMarketPerformanceSnapshotInfo
                                    .Where(record => record.MarketSnapshotSelectionInfo == SelectedMarketSnapshotSelectionInfo)
                                    .FirstOrDefault()
                                    .MarketPerformanceSnapshotInfo = MarketPerformanceSnapshotInfo.ToList();
                            }
                        }
                        #endregion

                        BusyIndicatorNotification();
                        TestEntityOrdering();
                    }
                });


            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
            BusyIndicatorNotification();

        }

        private void RevertRemoveEntityGroupCommandMethod(MarketSnapshotPreference preference)
        {

        }

        MarketSnapshotPreference _revertSnapshotPreference;
        Action<MarketSnapshotPreference> _revertSnapshotChange;

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
                //Cases Where EntitySelectionInfo Data Asychronous call thread is still active
                if (EntitySelectionInfo == null)
                {
                    if (SelectionData.EntitySelectionData != null)
                    {
                        BusyIndicatorNotification(true, "Retrieving Entity Selection Data ...");
                        RetrieveEntitySelectionDataCallbackMethod(SelectionData.EntitySelectionData);
                    }
                    return;
                }

                ChildViewInsertEntity childViewModelInsertEntity
                    = new ChildViewInsertEntity(EntitySelectionInfo, _dbInteractivity, _logger, SelectedMarketPerformanceSnapshotInfo.MarketSnapshotPreferenceInfo.GroupName);
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
                                BusyIndicatorNotification(true, "Updating preference structure ...");
                                //Create Preference object
                                MarketSnapshotPreference insertedMarketSnapshotPreference = childViewModelInsertEntity.InsertedMarketSnapshotPreference;

                                if (MarketSnapshotPreferenceInfo != null)
                                {
                                    if (MarketSnapshotPreferenceInfo.Where(record => record.EntityId == insertedMarketSnapshotPreference.EntityId
                                                                && record.EntityName == insertedMarketSnapshotPreference.EntityName
                                                                && record.EntityNodeType == insertedMarketSnapshotPreference.EntityNodeType
                                                                && record.EntityNodeValueCode == insertedMarketSnapshotPreference.EntityNodeValueCode
                                                                && record.EntityNodeValueName == insertedMarketSnapshotPreference.EntityNodeValueName
                                                                && record.EntityReturnType == insertedMarketSnapshotPreference.EntityReturnType
                                                                && record.EntityType == insertedMarketSnapshotPreference.EntityType).FirstOrDefault() != null)
                                    {
                                        Prompt.ShowDialog("Entity already exists in the snapshot");
                                        BusyIndicatorNotification();
                                        return;
                                    }
                                }

                                insertedMarketSnapshotPreference.GroupName = SelectedMarketPerformanceSnapshotInfo.MarketSnapshotPreferenceInfo.GroupName;
                                insertedMarketSnapshotPreference.GroupPreferenceID = SelectedMarketPerformanceSnapshotInfo.MarketSnapshotPreferenceInfo.GroupPreferenceID;

                                //Rearrange Entity Orders
                                insertedMarketSnapshotPreference.EntityOrder = SelectedMarketPerformanceSnapshotInfo.MarketSnapshotPreferenceInfo.EntityOrder;
                                if (MarketSnapshotPreferenceInfo == null)
                                {
                                    MarketSnapshotPreferenceInfo = new List<MarketSnapshotPreference>();
                                }

                                foreach (MarketSnapshotPreference entity in MarketSnapshotPreferenceInfo)
                                {
                                    if (entity.GroupPreferenceID != insertedMarketSnapshotPreference.GroupPreferenceID)
                                        continue;
                                    if (entity.EntityOrder < insertedMarketSnapshotPreference.EntityOrder)
                                        continue;
                                    entity.EntityOrder++;
                                }

                                //Add inserted entity to MarketSnapshotPreferenceInfo
                                _revertSnapshotPreference = insertedMarketSnapshotPreference;
                                _revertSnapshotChange = RevertAddEntityToGroupCommandMethod;
                                MarketSnapshotPreferenceInfo.Add(insertedMarketSnapshotPreference);

                                //Reorder MarketSnapshotPreferenceInfo
                                MarketSnapshotPreferenceInfo = MarketSnapshotPreferenceInfo
                                    .OrderBy(record => record.GroupPreferenceID)
                                    .ThenBy(record => record.EntityOrder)
                                    .ToList();

                                TestEntityOrdering();

                                //Service call to receive Market Performance Snapshot Data
                                if (_dbInteractivity != null && IsActive)
                                {
                                    BusyIndicatorNotification(true, "Retrieving performance data for inserted entity ...");
                                    _dbInteractivity.RetrieveMarketPerformanceSnapshotData(new List<MarketSnapshotPreference> { insertedMarketSnapshotPreference }
                                           , RetrieveMarketPerformanceSnapshotDataByEntityCallbackMethod);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                        Logging.LogException(_logger, ex);
                        BusyIndicatorNotification();
                    }
                    Logging.LogEndMethod(_logger, childUnloadedMethodNamespace);
                    #endregion
                };
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        private void RevertAddEntityToGroupCommandMethod(MarketSnapshotPreference preference)
        {
            MarketSnapshotPreferenceInfo.Remove(preference);
            foreach (MarketSnapshotPreference entity in MarketSnapshotPreferenceInfo)
            {
                if (entity.GroupPreferenceID != preference.GroupPreferenceID)
                    continue;
                if (entity.EntityOrder < preference.EntityOrder)
                    continue;
                entity.EntityOrder--;
            }

            MarketSnapshotPreferenceInfo = MarketSnapshotPreferenceInfo
                .OrderBy(record => record.GroupPreferenceID)
                .ThenBy(record => record.EntityOrder)
                .ToList();

            TestEntityOrdering();
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
                Prompt.ShowDialog("Remove Entity - '" + SelectedMarketPerformanceSnapshotInfo.MarketSnapshotPreferenceInfo.EntityName + "'?", "Remove Entity", MessageBoxButton.OKCancel, (result) =>
                    {
                        if (result == MessageBoxResult.OK)
                        {
                            try
                            {
                                BusyIndicatorNotification(true, "Removing selected entity from the snapshot");
                                if (MarketSnapshotPreferenceInfo != null)
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

                                    //Reorder MarketSnapshotPreferenceInfo
                                    MarketSnapshotPreferenceInfo = MarketPerformanceSnapshotInfo
                                            .Select(record => record.MarketSnapshotPreferenceInfo)
                                            .ToList();
                                }

                                #region Client cache implementation
                                if (PopulatedMarketPerformanceSnapshotInfo != null)
                                {
                                    PopulatedMarketPerformanceSnapshotData selectedPopulatedMarketPerformanceSnapshotInfo = PopulatedMarketPerformanceSnapshotInfo
                                                        .Where(record => record.MarketSnapshotSelectionInfo == SelectedMarketSnapshotSelectionInfo).FirstOrDefault();

                                    if (selectedPopulatedMarketPerformanceSnapshotInfo != null)
                                    {
                                        PopulatedMarketPerformanceSnapshotInfo
                                            .Where(record => record.MarketSnapshotSelectionInfo == SelectedMarketSnapshotSelectionInfo)
                                            .FirstOrDefault()
                                            .MarketPerformanceSnapshotInfo = MarketPerformanceSnapshotInfo.ToList();
                                    }
                                }
                                #endregion
                                TestEntityOrdering();
                            }
                            catch (Exception ex)
                            {
                                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                                Logging.LogException(_logger, ex);
                            }
                            BusyIndicatorNotification();
                        }
                    });

            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
            BusyIndicatorNotification();
        }

        private void RevertRemoveEntityFromGroupCommandMethod(MarketSnapshotPreference preference)
        {

        }
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

                    //if (SelectedMarketSnapshotSelectionInfo != null)
                    //{
                    //    if (SelectedMarketSnapshotSelectionInfo.SnapshotName == result.SnapshotName)
                    //        return;
                    //}                   

                    SelectedMarketSnapshotSelectionInfo = result;

                    //If the selected snapshot has already been cached on client side no requirement of another service call
                    #region Client cache check
                    if (PopulatedMarketPerformanceSnapshotInfo != null)
                    {
                        PopulatedMarketPerformanceSnapshotData PopulatedMarketPerformanceSnapshotOrigInfo
                                                    = PopulatedMarketPerformanceSnapshotInfo.Where(record => record.MarketSnapshotSelectionInfo == result).FirstOrDefault();
                        if (PopulatedMarketPerformanceSnapshotOrigInfo != null)
                        {
                            BusyIndicatorNotification(true, "Retrieving performance data for selected snapshot ...");
                            MarketSnapshotPreferenceOriginalInfo = GetMarketSnapshotPreferenceDeepCopy(PopulatedMarketPerformanceSnapshotOrigInfo
                                .MarketPerformanceSnapshotInfo
                                .Select(record => record.MarketSnapshotPreferenceInfo)
                                .OrderBy(record => record.GroupPreferenceID)
                                .ThenBy(record => record.EntityOrder)
                                .ToList());

                            MarketSnapshotPreferenceInfo = GetMarketSnapshotPreferenceDeepCopy(MarketSnapshotPreferenceOriginalInfo);

                            MarketPerformanceSnapshotInfo = new ObservableCollection<MarketPerformanceSnapshotData>
                                (GetMarketPerformanceSnapshotDataDeepCopy
                                    (PopulatedMarketPerformanceSnapshotOrigInfo.MarketPerformanceSnapshotInfo
                                    .OrderBy(record => record.MarketSnapshotPreferenceInfo.GroupPreferenceID)
                                    .ThenBy(record => record.MarketSnapshotPreferenceInfo.EntityOrder)
                                    .ToList())
                                );

                            BusyIndicatorNotification();
                            return;
                        }
                    }
                    #endregion

                    #region RetrieveMarketSnapshotPreference Service Call
                    if (_dbInteractivity != null && SelectedMarketSnapshotSelectionInfo != null && IsActive)
                    {
                        BusyIndicatorNotification(true, "Retrieving preference structure for selected snapshot ...");
                        _dbInteractivity.RetrieveMarketSnapshotPreference(SelectedMarketSnapshotSelectionInfo.SnapshotPreferenceId
                            , RetrieveMarketSnapshotPreferenceCallbackMethod);
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
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);

        }
        #endregion

        #region Snapshot Action Events
        /// <summary>
        /// MarketPerformanceSnapshot Action Event Handler
        /// </summary>
        /// <param name="result">MarketPerformanceSnapshotActionPayload data</param>
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
                    SelectedMarketSnapshotSelectionInfo = result.SelectedMarketSnapshotSelectionInfo;
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
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
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

                BusyIndicatorNotification(true, "Updating preference changes ...");
                string updateXML = SaveXmlBuilder();
                if (updateXML != String.Empty)
                {
                    _dbInteractivity.SaveMarketSnapshotPreference(updateXML, SaveMarketSnapshotPreferenceCallbackMethod);
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
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
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
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
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        /// <summary>
        ///  MarketPerformanceSnapshotRemoveAction Event Handler
        /// </summary>
        private void HandleMarketPerformanceSnapshotRemoveActionEvent()
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                Prompt.ShowDialog("Remove Snapshot - '" + SelectedMarketSnapshotSelectionInfo.SnapshotName + "' ?", "", MessageBoxButton.OKCancel, (messageResult) =>
                {
                    if (messageResult == MessageBoxResult.OK)
                    {
                        if (_dbInteractivity != null)
                        {
                            if (SessionManager.SESSION != null)
                            {
                                BusyIndicatorNotification(true, "Removing selected snapshot ...");
                                _dbInteractivity.RemoveMarketSnapshotPreference(SessionManager.SESSION.UserName
                                    , SelectedMarketSnapshotSelectionInfo.SnapshotName, RemoveMarketSnapshotPreferenceCallbackMethod);
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
                                            _dbInteractivity.RemoveMarketSnapshotPreference(SessionManager.SESSION.UserName
                                                , SelectedMarketSnapshotSelectionInfo.SnapshotName, RemoveMarketSnapshotPreferenceCallbackMethod);
                                        }
                                        else
                                        {
                                            Logging.LogMethodParameterNull(_logger, sessionMethodNamespace, 1);
                                            BusyIndicatorNotification();
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                                        Logging.LogException(_logger, ex);
                                    }
                                    Logging.LogEndMethod(_logger, sessionMethodNamespace);

                                });
                            }
                        }
                    }
                });

            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }
        #endregion

        #region ChildViewInsertSnapshot Unloaded Event
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
                                BusyIndicatorNotification(true, "Creating snapshot and assigning preference structure...");
                                string saveAsXml = SaveAsXmlBuilder(SessionManager.SESSION.UserName, view.tbSnapshotName.Text);
                                _dbInteractivity.SaveAsMarketSnapshotPreference(saveAsXml, SaveAsMarketSnapshotPreferenceCallbackMethod);
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

                                            BusyIndicatorNotification(true, "Creating snapshot and assigning preference structure...");
                                            string saveAsXml = SaveAsXmlBuilder(session.UserName, view.tbSnapshotName.Text);
                                            _dbInteractivity.SaveAsMarketSnapshotPreference(saveAsXml, SaveAsMarketSnapshotPreferenceCallbackMethod);
                                        }
                                        else
                                        {
                                            Logging.LogMethodParameterNull(_logger, sessionMethodNamespace, 1);
                                            BusyIndicatorNotification();
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
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
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
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
                                BusyIndicatorNotification(true, "Creating snapshot ...");
                                XDocument doc = new XDocument(
                                    new XDeclaration("1.0", "utf-8", "yes"),
                                    new XComment("Market performance snapshot save as preference details"),
                                    new XElement("Root",
                                        new XAttribute("UserName", SessionManager.SESSION.UserName),
                                        new XAttribute("SnapshotName", view.tbSnapshotName.Text)));
                                _dbInteractivity.SaveAsMarketSnapshotPreference(doc.ToString(), AddMarketSnapshotPreferenceCallbackMethod);
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
                                            BusyIndicatorNotification(true, "Creating snapshot ...");
                                            XDocument doc = new XDocument(
                                                new XDeclaration("1.0", "utf-8", "yes"),
                                                new XComment("Market performance snapshot save as preference details"),
                                                new XElement("Root",
                                                    new XAttribute("UserName", session.UserName),
                                                    new XAttribute("SnapshotName", view.tbSnapshotName.Text)));
                                            _dbInteractivity.SaveAsMarketSnapshotPreference(doc.ToString(), AddMarketSnapshotPreferenceCallbackMethod);
                                        }
                                        else
                                        {
                                            Logging.LogMethodParameterNull(_logger, sessionMethodNamespace, 1);
                                            BusyIndicatorNotification();
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
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
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
                BusyIndicatorNotification();
            }
            Logging.LogEndMethod(_logger, childUnloadedMethodNamespace);
        }
        #endregion
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
                        MarketSnapshotPreferenceOriginalInfo = GetMarketSnapshotPreferenceDeepCopy(result);
                        MarketSnapshotPreferenceInfo = GetMarketSnapshotPreferenceDeepCopy(result);

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
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
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
                    if (PopulatedMarketPerformanceSnapshotInfo == null)
                    {
                        PopulatedMarketPerformanceSnapshotInfo = new List<PopulatedMarketPerformanceSnapshotData>();
                    }
                    PopulatedMarketPerformanceSnapshotInfo.Add(new PopulatedMarketPerformanceSnapshotData()
                    {
                        MarketPerformanceSnapshotInfo = result,
                        MarketSnapshotSelectionInfo = SelectedMarketSnapshotSelectionInfo
                    });

                    PopulatedMarketPerformanceSnapshotOriginalInfo.Add(new PopulatedMarketPerformanceSnapshotData()
                        {
                            MarketPerformanceSnapshotInfo = GetMarketPerformanceSnapshotDataDeepCopy(result),
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
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        /// <summary>
        /// Callback method for RetrieveMarketPerformanceSnapshotData Service call - Gets performance data for propertyName single entity
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
                    if (result.Count == 1)
                    {
                        Logging.LogMethodParameter(_logger, methodNamespace, result, 1);

                        #region Add Performance data for the entity to MarketPerformanceSnapshotInfo
                        if (MarketPerformanceSnapshotInfo == null)
                        {
                            MarketPerformanceSnapshotInfo = new ObservableCollection<MarketPerformanceSnapshotData>();
                        }
                        MarketPerformanceSnapshotInfo.Add(result.FirstOrDefault());
                        #endregion

                        #region Reorder MarketPerformanceSnapshotInfo
                        MarketPerformanceSnapshotInfo = new ObservableCollection<MarketPerformanceSnapshotData>(MarketPerformanceSnapshotInfo
                                                        .OrderBy(record => record.MarketSnapshotPreferenceInfo.GroupPreferenceID)
                                                        .ThenBy(record => record.MarketSnapshotPreferenceInfo.EntityOrder)
                                                        .ToList());
                        #endregion

                        #region Client cache implementation
                        if (PopulatedMarketPerformanceSnapshotInfo != null)
                        {
                            PopulatedMarketPerformanceSnapshotData selectedPopulatedMarketPerformanceSnapshotInfo = PopulatedMarketPerformanceSnapshotInfo
                                                .Where(record => record.MarketSnapshotSelectionInfo == SelectedMarketSnapshotSelectionInfo).FirstOrDefault();

                            if (selectedPopulatedMarketPerformanceSnapshotInfo != null)
                            {
                                PopulatedMarketPerformanceSnapshotInfo
                                    .Where(record => record.MarketSnapshotSelectionInfo == SelectedMarketSnapshotSelectionInfo)
                                    .FirstOrDefault()
                                    .MarketPerformanceSnapshotInfo = MarketPerformanceSnapshotInfo.ToList();
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        _revertSnapshotChange(_revertSnapshotPreference);
                        Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                    }
                }
                else
                {
                    _revertSnapshotChange(_revertSnapshotPreference);
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
            BusyIndicatorNotification();
        }

        /// <summary>
        /// Callback method for SaveMarketSnapshotPreference Service call - saves user's Snapshot preference for the selected Snapshot
        /// </summary>
        /// <param name="result">update list of MarketSnapshotPreference</param>
        private void SaveMarketSnapshotPreferenceCallbackMethod(List<MarketSnapshotPreference> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);

                    #region Reassign the preference details to local properties
                    MarketSnapshotPreferenceOriginalInfo = GetMarketSnapshotPreferenceDeepCopy(result);
                    MarketSnapshotPreferenceInfo = GetMarketSnapshotPreferenceDeepCopy(result);
                    #endregion

                    #region Update MarketPerformanceSnapshotInfo
                    if (MarketPerformanceSnapshotInfo != null)
                    {
                        foreach (MarketPerformanceSnapshotData data in MarketPerformanceSnapshotInfo)
                        {
                            MarketSnapshotPreference preference = MarketSnapshotPreferenceInfo
                                .Where(record => record.EntityName == data.MarketSnapshotPreferenceInfo.EntityName
                                && record.EntityReturnType == data.MarketSnapshotPreferenceInfo.EntityReturnType)
                                .FirstOrDefault();

                            if (preference != null)
                            {
                                data.MarketSnapshotPreferenceInfo = preference;
                            }
                        }
                    }
                    #endregion

                    #region Client cache implementation
                    if (PopulatedMarketPerformanceSnapshotInfo != null)
                    {
                        PopulatedMarketPerformanceSnapshotData selectedPopulatedMarketPerformanceSnapshotInfo = PopulatedMarketPerformanceSnapshotInfo
                                            .Where(record => record.MarketSnapshotSelectionInfo == SelectedMarketSnapshotSelectionInfo).FirstOrDefault();

                        if (selectedPopulatedMarketPerformanceSnapshotInfo != null)
                        {
                            PopulatedMarketPerformanceSnapshotInfo
                                .Where(record => record.MarketSnapshotSelectionInfo == SelectedMarketSnapshotSelectionInfo)
                                .FirstOrDefault()
                                .MarketPerformanceSnapshotInfo = MarketPerformanceSnapshotInfo.ToList();
                        }
                    }

                    if (PopulatedMarketPerformanceSnapshotOriginalInfo != null)
                    {
                        PopulatedMarketPerformanceSnapshotData selectedPopulatedMarketPerformanceSnapshotInfo = PopulatedMarketPerformanceSnapshotOriginalInfo
                                            .Where(record => record.MarketSnapshotSelectionInfo == SelectedMarketSnapshotSelectionInfo).FirstOrDefault();

                        if (selectedPopulatedMarketPerformanceSnapshotInfo != null)
                        {
                            PopulatedMarketPerformanceSnapshotOriginalInfo
                                .Where(record => record.MarketSnapshotSelectionInfo == SelectedMarketSnapshotSelectionInfo)
                                .FirstOrDefault()
                                .MarketPerformanceSnapshotInfo = GetMarketPerformanceSnapshotDataDeepCopy(MarketPerformanceSnapshotInfo.ToList());
                        }
                    }
                    #endregion

                    #region Publish Action Completion Event
                    //Raise event for completion of the save action event
                    _eventAggregator.GetEvent<MarketPerformanceSnapshotActionCompletionEvent>()
                        .Publish(new MarketPerformanceSnapshotActionPayload()
                        {
                            ActionType = MarketPerformanceSnapshotActionType.SNAPSHOT_SAVE,
                            SelectedMarketSnapshotSelectionInfo = SelectedMarketSnapshotSelectionInfo,
                            MarketSnapshotSelectionInfo = MarketSnapshotSelectionInfo,
                        });
                    #endregion

                    TestEntityOrdering();
                }
                else
                {
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
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
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            BusyIndicatorNotification();
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        /// <summary>
        /// Callback method for SaveAsMarketSnapshotPreference Service call - creates propertyName new snapshot with assigned name and existing structure
        /// </summary>
        /// <param name="result">List of MarketSnapshotSelectionData objects</param>
        private void SaveAsMarketSnapshotPreferenceCallbackMethod(PopulatedMarketPerformanceSnapshotData result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);

                    #region Client cache implementation
                    #region Revert changes in original snapshot in the storage
                    //Revert the preference changes for the snapshot which is being saved as propertyName different snapshot
                    if (PopulatedMarketPerformanceSnapshotInfo != null && PopulatedMarketPerformanceSnapshotOriginalInfo != null)
                    {
                        PopulatedMarketPerformanceSnapshotData dirtySnapshotPopulatedData = PopulatedMarketPerformanceSnapshotInfo
                            .Where(record => record.MarketSnapshotSelectionInfo.SnapshotName == SelectedMarketSnapshotSelectionInfo.SnapshotName)
                            .FirstOrDefault();

                        PopulatedMarketPerformanceSnapshotData cleanSnapshotPopulatedData = PopulatedMarketPerformanceSnapshotOriginalInfo
                            .Where(record => record.MarketSnapshotSelectionInfo.SnapshotName == SelectedMarketSnapshotSelectionInfo.SnapshotName)
                            .FirstOrDefault();

                        if (dirtySnapshotPopulatedData != null && cleanSnapshotPopulatedData != null)
                        {
                            dirtySnapshotPopulatedData.MarketPerformanceSnapshotInfo = GetMarketPerformanceSnapshotDataDeepCopy
                                (cleanSnapshotPopulatedData.MarketPerformanceSnapshotInfo);
                        }
                    }
                    #endregion

                    #region Add performance data for the new snapshot to the storage
                    if (PopulatedMarketPerformanceSnapshotInfo == null)
                    {
                        PopulatedMarketPerformanceSnapshotInfo = new List<PopulatedMarketPerformanceSnapshotData>();
                    }
                    PopulatedMarketPerformanceSnapshotInfo.Add(result);
                    PopulatedMarketPerformanceSnapshotOriginalInfo.Add(GetPopulatedMarketPerformanceSnapshotDataDeepCopy(result));
                    #endregion
                    #endregion

                    #region Update Local Snapshot Selection Information
                    SelectedMarketSnapshotSelectionInfo = result.MarketSnapshotSelectionInfo;
                    if (MarketSnapshotSelectionInfo == null)
                    {
                        MarketSnapshotSelectionInfo = new List<MarketSnapshotSelectionData>();
                    }
                    MarketSnapshotSelectionInfo.Add(result.MarketSnapshotSelectionInfo);
                    #endregion

                    #region Update Local Snapshot Preference Information
                    MarketSnapshotPreferenceInfo = result.MarketPerformanceSnapshotInfo
                                    .Select(record => record.MarketSnapshotPreferenceInfo)
                                    .ToList();

                    MarketSnapshotPreferenceOriginalInfo = GetMarketSnapshotPreferenceDeepCopy(MarketSnapshotPreferenceInfo);
                    #endregion

                    #region Publish Action Completion Event
                    _eventAggregator.GetEvent<MarketPerformanceSnapshotActionCompletionEvent>()
                        .Publish(new MarketPerformanceSnapshotActionPayload()
                        {
                            ActionType = MarketPerformanceSnapshotActionType.SNAPSHOT_SAVE_AS,
                            SelectedMarketSnapshotSelectionInfo = result.MarketSnapshotSelectionInfo,
                            MarketSnapshotSelectionInfo = MarketSnapshotSelectionInfo
                        });
                    #endregion
                }
                else
                {
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
            BusyIndicatorNotification();
        }

        /// <summary>
        /// Callback method for SaveAsMarketSnapshotPreference Service call - creates propertyName new snapshot with assigned name and blank structure
        /// </summary>
        /// <param name="result">Added snapshot details</param>
        private void AddMarketSnapshotPreferenceCallbackMethod(PopulatedMarketPerformanceSnapshotData result)
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
                    if (PopulatedMarketPerformanceSnapshotInfo == null)
                    {
                        PopulatedMarketPerformanceSnapshotInfo = new List<PopulatedMarketPerformanceSnapshotData>();
                    }

                    PopulatedMarketPerformanceSnapshotInfo.Add(result);
                    #endregion

                    SelectedMarketSnapshotSelectionInfo = result.MarketSnapshotSelectionInfo;
                    if (MarketSnapshotSelectionInfo == null)
                    {
                        MarketSnapshotSelectionInfo = new List<MarketSnapshotSelectionData>();
                    }
                    MarketSnapshotSelectionInfo.Add(result.MarketSnapshotSelectionInfo);

                    MarketSnapshotPreferenceOriginalInfo = new List<MarketSnapshotPreference>();
                    MarketSnapshotPreferenceInfo = new List<MarketSnapshotPreference>();

                    _eventAggregator.GetEvent<MarketPerformanceSnapshotActionCompletionEvent>()
                        .Publish(new MarketPerformanceSnapshotActionPayload()
                        {
                            ActionType = MarketPerformanceSnapshotActionType.SNAPSHOT_ADD,
                            SelectedMarketSnapshotSelectionInfo = result.MarketSnapshotSelectionInfo,
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
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
            BusyIndicatorNotification();
        }

        /// <summary>
        /// Callback method for RemoveMarketSnapshotPreference Service call - removes selected snapshot
        /// </summary>
        /// <param name="result">True/False</param>
        private void RemoveMarketSnapshotPreferenceCallbackMethod(bool? result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    if (result == true)
                    {
                        Logging.LogMethodParameter(_logger, methodNamespace, result, 1);

                        #region Client cache update
                        if (PopulatedMarketPerformanceSnapshotInfo != null)
                        {
                            //Remove the snapshot entry from the client cache
                            PopulatedMarketPerformanceSnapshotData PopulatedMarketPerformanceSnapshotOriginalInfo
                                = PopulatedMarketPerformanceSnapshotInfo
                                .Where(record => record.MarketSnapshotSelectionInfo == SelectedMarketSnapshotSelectionInfo)
                                .FirstOrDefault();

                            if (PopulatedMarketPerformanceSnapshotOriginalInfo != null)
                            {
                                PopulatedMarketPerformanceSnapshotInfo.Remove(PopulatedMarketPerformanceSnapshotOriginalInfo);
                            }
                        }
                        #endregion

                        if (MarketSnapshotSelectionInfo != null)
                        {
                            MarketSnapshotSelectionInfo.Remove(SelectedMarketSnapshotSelectionInfo);
                        }
                        SelectedMarketSnapshotSelectionInfo = null;

                        MarketPerformanceSnapshotInfo = new ObservableCollection<MarketPerformanceSnapshotData>();
                        MarketSnapshotPreferenceOriginalInfo = new List<MarketSnapshotPreference>();
                        MarketSnapshotPreferenceInfo = new List<MarketSnapshotPreference>();

                        _eventAggregator.GetEvent<MarketPerformanceSnapshotActionCompletionEvent>()
                            .Publish(new MarketPerformanceSnapshotActionPayload()
                            {
                                ActionType = MarketPerformanceSnapshotActionType.SNAPSHOT_REMOVE,
                                SelectedMarketSnapshotSelectionInfo = null,
                                MarketSnapshotSelectionInfo = MarketSnapshotSelectionInfo
                            });
                    }
                    else
                    {
                        Prompt.ShowDialog("An error occured while updating database. Please try again");
                        Logging.LogMethodParameterFalse(_logger, methodNamespace, 1);
                    }
                }
                else
                {
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
            BusyIndicatorNotification();
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
                if (MarketPerformanceSnapshotInfo != null)
                {
                    foreach (MarketPerformanceSnapshotData record in MarketPerformanceSnapshotInfo)
                    {
                        if (record.MarketSnapshotPreferenceInfo.GroupPreferenceID > lastGroupPreferenceId)
                        {
                            lastGroupPreferenceId = record.MarketSnapshotPreferenceInfo.GroupPreferenceID;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
            return lastGroupPreferenceId;
        }

        /// <summary>
        /// Compares MarketSnapshotPreferenceInfo and MarketSnapshotPreferenceOriginalInfo and creates CRUD lists
        /// </summary>
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

                if (MarketSnapshotPreferenceOriginalInfo
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

        /// <summary>
        /// Display/Hide Busy Indicator
        /// </summary>
        /// <param name="showBusyIndicator">True to display indicator; default false</param>
        /// <param name="message">Content message for indicator; default null</param>
        public void BusyIndicatorNotification(bool showBusyIndicator = false, String message = null)
        {
            if (message != null)
                BusyIndicatorContent = message;

            BusyIndicatorIsBusy = showBusyIndicator;
        }

        /// <summary>
        /// Get Deep Copy for List of MarketSnapshotPreference type data
        /// </summary>
        /// <param name="data">List of MarketSnapshotPreference object</param>
        /// <returns>List of MarketSnapshotPreference object</returns>
        public List<MarketSnapshotPreference> GetMarketSnapshotPreferenceDeepCopy(List<MarketSnapshotPreference> list)
        {
            List<MarketSnapshotPreference> result = new List<MarketSnapshotPreference>();
            foreach (MarketSnapshotPreference preference in list)
            {
                MarketSnapshotPreference pref = new MarketSnapshotPreference()
                {
                    EntityName = preference.EntityName,
                    EntityOrder = preference.EntityOrder,
                    EntityPreferenceId = preference.EntityPreferenceId,
                    EntityReturnType = preference.EntityType,
                    GroupName = preference.GroupName,
                    GroupPreferenceID = preference.GroupPreferenceID
                };

                result.Add(pref);
            }

            return result;
        }

        /// <summary>
        /// Get Deep Copy for List of MarketPerformanceSnapshotData type data
        /// </summary>
        /// <param name="data">List of MarketPerformanceSnapshotData object</param>
        /// <returns>List of MarketPerformanceSnapshotData object</returns>
        public List<MarketPerformanceSnapshotData> GetMarketPerformanceSnapshotDataDeepCopy(List<MarketPerformanceSnapshotData> list)
        {
            List<MarketPerformanceSnapshotData> result = new List<MarketPerformanceSnapshotData>();

            foreach (MarketPerformanceSnapshotData record in list)
            {
                MarketSnapshotPreference preference = new MarketSnapshotPreference()
                {
                    EntityName = record.MarketSnapshotPreferenceInfo.EntityName,
                    EntityOrder = record.MarketSnapshotPreferenceInfo.EntityOrder,
                    EntityPreferenceId = record.MarketSnapshotPreferenceInfo.EntityPreferenceId,
                    EntityReturnType = record.MarketSnapshotPreferenceInfo.EntityReturnType,
                    EntityType = record.MarketSnapshotPreferenceInfo.EntityType,
                    GroupName = record.MarketSnapshotPreferenceInfo.GroupName,
                    GroupPreferenceID = record.MarketSnapshotPreferenceInfo.GroupPreferenceID
                };

                MarketPerformanceSnapshotData performanceData = new MarketPerformanceSnapshotData()
                {
                    DateToDateReturn = record.DateToDateReturn,
                    LastYearReturn = record.LastYearReturn,
                    MarketSnapshotPreferenceInfo = preference,
                    MonthToDateReturn = record.MonthToDateReturn,
                    QuarterToDateReturn = record.QuarterToDateReturn,
                    SecondLastYearReturn = record.SecondLastYearReturn,
                    ThirdLastYearReturn = record.ThirdLastYearReturn,
                    WeekToDateReturn = record.WeekToDateReturn,
                    YearToDateReturn = record.YearToDateReturn
                };

                result.Add(performanceData);
            }

            return result;
        }

        /// <summary>
        /// Get Deep Copy for PopulatedMarketPerformanceSnapshotData type data
        /// </summary>
        /// <param name="data">PopulatedMarketPerformanceSnapshotData object</param>
        /// <returns>PopulatedMarketPerformanceSnapshotData object</returns>
        public PopulatedMarketPerformanceSnapshotData GetPopulatedMarketPerformanceSnapshotDataDeepCopy(PopulatedMarketPerformanceSnapshotData data)
        {
            PopulatedMarketPerformanceSnapshotData result = new PopulatedMarketPerformanceSnapshotData();

            MarketSnapshotSelectionData selectionData = new MarketSnapshotSelectionData()
            {
                SnapshotName = data.MarketSnapshotSelectionInfo.SnapshotName,
                SnapshotPreferenceId = data.MarketSnapshotSelectionInfo.SnapshotPreferenceId
            };

            List<MarketPerformanceSnapshotData> performanceData = GetMarketPerformanceSnapshotDataDeepCopy(data.MarketPerformanceSnapshotInfo);

            result = new PopulatedMarketPerformanceSnapshotData()
            {
                MarketPerformanceSnapshotInfo = performanceData,
                MarketSnapshotSelectionInfo = selectionData
            };

            return result;
        }

        /// <summary>
        /// Construct XML for Save As Event
        /// </summary>
        /// <returns></returns>
        private string SaveAsXmlBuilder(String userName, String snapshotName)
        {
            string saveAsXml = String.Empty;

            try
            {
                MarketSnapshotPreferenceInfo = MarketSnapshotPreferenceInfo
                    .OrderBy(record => record.GroupPreferenceID)
                    .ThenBy(record => record.EntityOrder)
                    .ToList();

                string insertedGroupName = String.Empty;

                XElement root = new XElement("Root",
                    new XAttribute("UserName", userName),
                    new XAttribute("SnapshotName", snapshotName));

                XElement createGroup = null;
                foreach (MarketSnapshotPreference preference in MarketSnapshotPreferenceInfo)
                {
                    if (preference.GroupName != insertedGroupName)
                    {
                        if (createGroup != null)
                            root.Add(createGroup);
                        createGroup = new XElement("CreateGroup", new XAttribute("GroupName", preference.GroupName));
                        insertedGroupName = preference.GroupName;
                    }

                    //XElement createGroupEntity = new XElement("CreateGroupEntity",
                    //            new XAttribute("EntityName", preference.EntityName.ToString()),
                    //            new XAttribute("EntityReturnType", preference.EntityReturnType.ToString()),
                    //            new XAttribute("EntityOrder", preference.EntityOrder.ToString()),
                    //            new XAttribute("EntityType", preference.EntityType.ToString()),
                    //            new XAttribute("EntityId", preference.EntityId.ToString()),
                    //            new XAttribute("EntityNodeType", preference.EntityNodeType.ToString()),
                    //            new XAttribute("EntityNodeValueCode", preference.EntityNodeValueCode.ToString()),
                    //            new XAttribute("EntityNodeValueName", preference.EntityNodeValueName.ToString()));

                    XElement createGroupEntity = new XElement("CreateGroupEntity");
                    if (preference.EntityName != null)
                        createGroupEntity.Add(new XAttribute("EntityName", preference.EntityName.ToString()));
                    if (preference.EntityReturnType != null)
                        createGroupEntity.Add(new XAttribute("EntityReturnType", preference.EntityReturnType.ToString()));
                    if (preference.EntityOrder != null)
                        createGroupEntity.Add(new XAttribute("EntityOrder", preference.EntityOrder.ToString()));
                    if (preference.EntityType != null)
                        createGroupEntity.Add(new XAttribute("EntityType", preference.EntityType.ToString()));
                    if (preference.EntityId != null)
                        createGroupEntity.Add(new XAttribute("EntityId", preference.EntityId.ToString()));
                    if (preference.EntityNodeType != null)
                        createGroupEntity.Add(new XAttribute("EntityNodeType", preference.EntityNodeType.ToString()));
                    if (preference.EntityNodeValueCode != null)
                        createGroupEntity.Add(new XAttribute("EntityNodeValueCode", preference.EntityNodeValueCode.ToString()));
                    if (preference.EntityNodeValueName != null)
                        createGroupEntity.Add(new XAttribute("EntityNodeValueName", preference.EntityNodeValueName.ToString()));

                    createGroup.Add(createGroupEntity);
                }

                root.Add(createGroup);

                XDocument doc = new XDocument(
                    new XDeclaration("1.0", "utf-8", "yes"),
                    new XComment("Market performance snapshot save as preference details"),
                    root);

                saveAsXml = doc.ToString();
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog(ex.Message);
            }

            return saveAsXml;
        }

        /// <summary>
        /// Construct XML for Save Event
        /// </summary>
        /// <returns></returns>
        private string SaveXmlBuilder()
        {
            string saveXml = String.Empty;
            try
            {
                XElement root = new XElement("Root",
                    new XAttribute("SnapshotPreferenceId", SelectedMarketSnapshotSelectionInfo.SnapshotPreferenceId.ToString()));

                foreach (string groupName in _createGroupInfo)
                {

                    //int groupPrefId = Convert.ToInt32(entity.SetMarketSnapshotGroupPreference(marketSnapshotSelectionData.SnapshotPreferenceId, groupName).FirstOrDefault());
                    XElement createGroup = new XElement("CreateGroup", new XAttribute("GroupName", groupName));

                    foreach (MarketSnapshotPreference preference in _createPreferenceInfo)
                    {
                        if (preference.GroupName == groupName)
                        {
                            XElement createGroupEntity = new XElement("CreateGroupEntity");
                            if (preference.EntityName != null)
                                createGroupEntity.Add(new XAttribute("EntityName", preference.EntityName.ToString()));
                            if (preference.EntityReturnType != null)
                                createGroupEntity.Add(new XAttribute("EntityReturnType", preference.EntityReturnType.ToString()));
                            if (preference.EntityOrder != null)
                                createGroupEntity.Add(new XAttribute("EntityOrder", preference.EntityOrder.ToString()));
                            if (preference.EntityType != null)
                                createGroupEntity.Add(new XAttribute("EntityType", preference.EntityType.ToString()));
                            if (preference.EntityId != null)
                                createGroupEntity.Add(new XAttribute("EntityId", preference.EntityId.ToString()));
                            if (preference.EntityNodeType != null)
                                createGroupEntity.Add(new XAttribute("EntityNodeType", preference.EntityNodeType.ToString()));
                            if (preference.EntityNodeValueCode != null)
                                createGroupEntity.Add(new XAttribute("EntityNodeValueCode", preference.EntityNodeValueCode.ToString()));
                            if (preference.EntityNodeValueName != null)
                                createGroupEntity.Add(new XAttribute("EntityNodeValueName", preference.EntityNodeValueName.ToString()));

                            createGroup.Add(createGroupEntity);
                            //entity.SetMarketSnapshotEntityPreference(groupPrefId, preference.EntityName
                            //    , preference.EntityReturnType, preference.EntityType, preference.EntityOrder);
                        }
                    }

                    root.Add(createGroup);
                }


                foreach (MarketSnapshotPreference preference in _createPreferenceInfo)
                {
                    if (!_createGroupInfo.Contains(preference.GroupName))
                    {
                        XElement createEntity = new XElement("CreateEntity", new XAttribute("GroupPreferenceId", preference.GroupPreferenceID.ToString()));
                        if (preference.EntityName != null)
                            createEntity.Add(new XAttribute("EntityName", preference.EntityName.ToString()));
                        if (preference.EntityReturnType != null)
                            createEntity.Add(new XAttribute("EntityReturnType", preference.EntityReturnType.ToString()));
                        if (preference.EntityOrder != null)
                            createEntity.Add(new XAttribute("EntityOrder", preference.EntityOrder.ToString()));
                        if (preference.EntityType != null)
                            createEntity.Add(new XAttribute("EntityType", preference.EntityType.ToString()));
                        if (preference.EntityId != null)
                            createEntity.Add(new XAttribute("EntityId", preference.EntityId.ToString()));
                        if (preference.EntityNodeType != null)
                            createEntity.Add(new XAttribute("EntityNodeType", preference.EntityNodeType.ToString()));
                        if (preference.EntityNodeValueCode != null)
                            createEntity.Add(new XAttribute("EntityNodeValueCode", preference.EntityNodeValueCode.ToString()));
                        if (preference.EntityNodeValueName != null)
                            createEntity.Add(new XAttribute("EntityNodeValueName", preference.EntityNodeValueName.ToString()));
                        root.Add(createEntity);
                        //entity.SetMarketSnapshotEntityPreference(preference.GroupPreferenceID, preference.EntityName
                        //    , preference.EntityReturnType, preference.EntityType, preference.EntityOrder);
                    }
                }

                foreach (int groupPreferenceId in _deleteGroupInfo)
                {
                    XElement deleteGroup = new XElement("DeleteGroup",
                        new XAttribute("GroupPreferenceId", groupPreferenceId.ToString()));

                    root.Add(deleteGroup);
                    //entity.DeleteMarketSnapshotGroupPreference(groupPreferenceId);
                }

                foreach (MarketSnapshotPreference preference in _deletePreferenceInfo)
                {
                    XElement deleteEntity = new XElement("DeleteEntity",
                        new XAttribute("EntityPreferenceId", preference.EntityPreferenceId.ToString()));

                    root.Add(deleteEntity);
                    //entity.DeleteMarketSnapshotEntityPreference(preference.EntityPreferenceId);
                }

                foreach (MarketSnapshotPreference preference in _updatePreferenceInfo)
                {
                    XElement updateEntity = new XElement("UpdateEntity",
                        new XAttribute("GroupPreferenceId", preference.GroupPreferenceID.ToString()),
                        new XAttribute("EntityPreferenceId", preference.EntityPreferenceId.ToString()),
                        new XAttribute("EntityOrder", preference.EntityOrder.ToString()));

                    root.Add(updateEntity);
                    //entity.UpdateMarketSnapshotEntityPreference(preference.GroupPreferenceID
                    //    , preference.EntityPreferenceId, preference.EntityOrder);
                }

                XDocument doc = new XDocument(
                    new XDeclaration("1.0", "utf-8", "yes"),
                    new XComment("Market performance snapshot save preference details"),
                    root);

                saveXml = doc.ToString();
            }

            catch (Exception ex)
            {
                Prompt.ShowDialog(ex.Message);
            }

            return saveXml;
        }
        #endregion

        public void Dispose()
        {
            _eventAggregator.GetEvent<MarketPerformanceSnapshotReferenceSetEvent>().Unsubscribe(HandleMarketPerformanceSnapshotNameReferenceSetEvent);
            _eventAggregator.GetEvent<MarketPerformanceSnapshotActionEvent>().Unsubscribe(HandleMarketPerformanceSnapshotActionEvent);
        }

        public void TestEntityOrdering()
        {
            //string A = "Preference ";
            //foreach (MarketSnapshotPreference record in MarketSnapshotPreferenceInfo)
            //{
            //    if (record.EntityName != null)
            //    {
            //        A = A + record.EntityName + "\t" +
            //            record.GroupPreferenceID.ToString() + "\t" +
            //            ((int)record.EntityOrder).ToString() + "\n";
            //    }
            //}

            //A = A + "\n\nOriginal Preference ";
            //foreach (MarketSnapshotPreference record in MarketSnapshotPreferenceOriginalInfo)
            //{
            //    if (record.EntityName != null)
            //    {
            //        A = A + record.EntityName + "\t" +
            //            record.GroupPreferenceID.ToString() + "\t" +
            //            ((int)record.EntityOrder).ToString() + "\n";
            //    }
            //}
            //Prompt.ShowDialog(A);
        }





    }

}

