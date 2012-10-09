using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.ViewModel;
using GreenField.Common;
using GreenField.DataContracts;
using GreenField.Gadgets.Views;
using GreenField.ServiceCaller;
using GreenField.ServiceCaller.PerformanceDefinitions;
using GreenField.UserSession;

namespace GreenField.Gadgets.ViewModels
{
    /// <summary>
    /// view model class for market performance snapshot
    /// </summary>
    public class ViewModelMarketPerformanceSnapshot : NotificationObject
    {
        #region Fields
        #region MEF Singletons
        /// <summary>
        /// Event Aggregation MEF Singleton
        /// </summary>
        private IEventAggregator eventAggregator;

        /// <summary>
        /// Manage Session Service MEF Singleton
        /// </summary>
        private IManageSessions manageSessions;

        /// <summary>
        /// Logging MEF Singleton
        /// </summary>
        private ILoggerFacade logger; 
        #endregion

        #region Preference structure changes
        /// <summary>
        /// Stores preference structure to be created on Db
        /// </summary>
        private List<MarketSnapshotPreference> createPreferenceInfo = new List<MarketSnapshotPreference>();

        /// <summary>
        /// Stores preference structure to be updated on Db
        /// </summary>
        private List<MarketSnapshotPreference> updatePreferenceInfo = new List<MarketSnapshotPreference>();

        /// <summary>
        /// Stores preference structure to be deleted on Db
        /// </summary>
        private List<MarketSnapshotPreference> deletePreferenceInfo = new List<MarketSnapshotPreference>();

        /// <summary>
        /// Stores preference group structure to be deleted on Db
        /// </summary>
        private List<int> deleteGroupInfo = new List<int>();

        /// <summary>
        /// Stores preference group structure to be created on Db
        /// </summary>
        private List<string> createGroupInfo = new List<string>(); 
        #endregion        
        #endregion

        #region Properties
        #region Service Caller IDBInteractivity
        /// <summary>
        /// Service Caller instance - to be used in class behind to invoke Entity Selection data retrieval after the constructor of view is invoked
        /// </summary>
        public IDBInteractivity DbInteractivity { get; set; }
        #endregion

        #region Snapshot Header
        /// <summary>
        /// Stores top header
        /// </summary>
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
        private List<EntitySelectionData> entitySelectionInfo;
        public List<EntitySelectionData> EntitySelectionInfo
        {
            get { return entitySelectionInfo; }
            set { entitySelectionInfo = value; }
        }
        #endregion

        #region MarketSnapshot Selection Data
        /// <summary>
        /// Stores the selected MarketSnapshotSelectionData received from shell
        /// </summary>
        private MarketSnapshotSelectionData selectedMarketSnapshotSelectionInfo;
        public MarketSnapshotSelectionData SelectedMarketSnapshotSelectionInfo
        {
            get { return selectedMarketSnapshotSelectionInfo; }
            set { selectedMarketSnapshotSelectionInfo = value; }
        }

        /// <summary>
        /// Stores the complete list of MarketSnapshotSelectionData created by user
        /// </summary>
        private List<MarketSnapshotSelectionData> marketSnapshotSelectionInfo;
        public List<MarketSnapshotSelectionData> MarketSnapshotSelectionInfo
        {
            get { return marketSnapshotSelectionInfo; }
            set { marketSnapshotSelectionInfo = value; }
        }
        #endregion

        #region Market Snapshot Preference
        /// <summary>
        /// Stores the original snapshot preference configuration for selected snapshot required for comparison before the changes are propagated to database
        /// </summary>
        private List<MarketSnapshotPreference> marketSnapshotPreferenceOriginalInfo;
        public List<MarketSnapshotPreference> MarketSnapshotPreferenceOriginalInfo
        {
            get { return marketSnapshotPreferenceOriginalInfo; }
            set { marketSnapshotPreferenceOriginalInfo = value; }
        }

        /// <summary>
        /// Storessnapshot preference configuration for selected snapshot that is modified by user
        /// </summary>
        private List<MarketSnapshotPreference> marketSnapshotPreferenceInfo;
        public List<MarketSnapshotPreference> MarketSnapshotPreferenceInfo
        {
            get { return marketSnapshotPreferenceInfo; }
            set
            {
                marketSnapshotPreferenceInfo = value;
                RaisePropertyChanged(() => this.MarketSnapshotPreferenceInfo);
            }
        }
        #endregion

        #region Market Performance Snapshot Data
        /// <summary>
        /// Stores Market performance data for selected snapshot received from service
        /// </summary>
        private ObservableCollection<MarketSnapshotPerformanceData> marketSnapshotPerformanceData;
        public ObservableCollection<MarketSnapshotPerformanceData> MarketSnapshotPerformanceInfo
        {
            get { return marketSnapshotPerformanceData; }
            set
            {
                marketSnapshotPerformanceData = value;
                RaisePropertyChanged(() => this.MarketSnapshotPerformanceInfo);
                MarketSnapshotPreferenceInfo = value
                    .Select(record => record.MarketSnapshotPreferenceInfo)
                    .ToList();
            }
        }

        /// <summary>
        /// Stores the selected Market performance data for propertyName specific snapshot entity
        /// </summary>
        private MarketSnapshotPerformanceData selectedMarketSnapshotPerformanceInfo;
        public MarketSnapshotPerformanceData SelectedMarketSnapshotPerformanceInfo
        {
            get { return selectedMarketSnapshotPerformanceInfo; }
            set
            {
                selectedMarketSnapshotPerformanceInfo = value;
                RaisePropertyChanged(() => this.SelectedMarketSnapshotPerformanceInfo);
            }
        }
        #endregion

        #region Busy Indicator Notification
        /// <summary>
        /// Displays/Hides busy indicator to notify user of the on going process
        /// </summary>
        private bool busyIndicatorIsBusy = false;
        public bool BusyIndicatorIsBusy
        {
            get { return busyIndicatorIsBusy; }
            set
            {
                busyIndicatorIsBusy = value;
                RaisePropertyChanged(() => this.BusyIndicatorIsBusy);
            }
        }

        /// <summary>
        /// Stores the message displayed over the busy indicator to notify user of the on going process
        /// </summary>
        private string busyIndicatorContent;
        public string BusyIndicatorContent
        {
            get { return busyIndicatorContent; }
            set
            {
                busyIndicatorContent = value;
                RaisePropertyChanged(() => this.BusyIndicatorContent);
            }
        }
        #endregion

        #region Gadget Active Check
        /// <summary>
        /// IsActive is true when parent control is displayed on UI
        /// </summary>
        private bool isActive;
        public bool IsActive
        {
            get
            {
                return isActive;
            }
            set
            {
                if (isActive != value)
                {
                    isActive = value;
                    if (DbInteractivity != null && SelectedMarketSnapshotSelectionInfo != null && isActive)
                    {
                        BusyIndicatorNotification(true, "Retrieving preference structure for selected snapshot ...");
                        DbInteractivity.RetrieveMarketSnapshotPreference(SelectedMarketSnapshotSelectionInfo.SnapshotPreferenceId
                            , RetrieveMarketSnapshotPreferenceCallbackMethod);
                    }
                }
            }
        }
        #endregion
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
            DbInteractivity = param.DBInteractivity;
            manageSessions = param.ManageSessions;
            eventAggregator = param.EventAggregator;
            logger = param.LoggerFacade;

            SelectedMarketSnapshotSelectionInfo = param.DashboardGadgetPayload.MarketSnapshotSelectionData;

            //RetrieveEntitySelectionData Service Call
            if (SelectionData.EntitySelectionData != null && EntitySelectionInfo == null)
            {
                BusyIndicatorNotification(true, "Retrieving Entity Selection Data ...");
                RetrieveEntitySelectionDataCallbackMethod(SelectionData.EntitySelectionData);
            }

            //Subscribe to MarketPerformanceSnapshotNameReferenceSetEvent to receive snapshot selection from shell
            if (eventAggregator != null)
            {
                eventAggregator.GetEvent<MarketPerformanceSnapshotReferenceSetEvent>().Subscribe(HandleMarketPerformanceSnapshotNameReferenceSetEvent);
                eventAggregator.GetEvent<MarketPerformanceSnapshotActionEvent>().Subscribe(HandleMarketPerformanceSnapshotActionEvent);
            }

            //RetrieveMarketSnapshotPreference Service Call
            if (SelectedMarketSnapshotSelectionInfo != null)
            {
                HandleMarketPerformanceSnapshotNameReferenceSetEvent(SelectedMarketSnapshotSelectionInfo);
            }
        }
        #endregion        

        #region ICommand
        /// <summary>
        /// Binded Command to Add Entity Context Menu
        /// </summary>
        public ICommand AddEntityGroupCommand
        {
            get
            {
                return new DelegateCommand<object>(AddEntityGroupCommandMethod);
            }
        }

        /// <summary>
        /// Binded Command to Remove Entity Context Menu
        /// </summary>
        public ICommand RemoveEntityGroupCommand
        {
            get
            {
                return new DelegateCommand<object>(RemoveEntityGroupCommandMethod);
            }
        }

        /// <summary>
        /// Binded Command to Add Group Context Menu
        /// </summary>
        public ICommand AddEntityToGroupCommand
        {
            get
            {
                return new DelegateCommand<object>(AddEntityToGroupCommandMethod);
            }
        }

        /// <summary>
        /// Binded Command to Remove Group Context Menu
        /// </summary>
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
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                //cases Where EntitySelectionInfo data asychronous call thread is still active
                if (EntitySelectionInfo == null)
                {
                    if (SelectionData.EntitySelectionData != null)
                        RetrieveEntitySelectionDataCallbackMethod(SelectionData.EntitySelectionData);
                    Prompt.ShowDialog("Entity Selection data is being retrieved");
                    return;
                }

                //get all group names present in the snapshot
                List<string> snapshotGroupNames = new List<string>();
                if (MarketSnapshotPerformanceInfo != null)
                {
                    snapshotGroupNames = MarketSnapshotPerformanceInfo
                        .Select(i => i.MarketSnapshotPreferenceInfo.GroupName).Distinct().ToList();
                }

                ChildViewInsertEntity childViewModelInsertEntity
                    = new ChildViewInsertEntity(EntitySelectionInfo, DbInteractivity, logger, groupNames: snapshotGroupNames);
                childViewModelInsertEntity.Show();
                childViewModelInsertEntity.Unloaded += (se, e) =>
                {
                    #region childViewModelInsertEntity Unloaded Event Handler
                    string childUnloadedMethodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
                    Logging.LogBeginMethod(logger, childUnloadedMethodNamespace);
                    try
                    {
                        if (childViewModelInsertEntity.DialogResult == true)
                        {
                            if (childViewModelInsertEntity.InsertedMarketSnapshotPreference != null)
                            {
                                //create preference object - assign property name new uncommitted GroupPreferenceId to the entity and place it with Entity Order 1
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

                                //service call to receive MarketPerformanceSnapshotData
                                if (DbInteractivity != null && IsActive)
                                {
                                    BusyIndicatorNotification(true, "Retrieving performance data for inserted entity ...");
                                    DbInteractivity.RetrieveMarketSnapshotPerformanceData(new List<MarketSnapshotPreference> { insertedMarketSnapshotPreference }
                                        , RetrieveMarketSnapshotPerformanceDataByEntityCallbackMethod);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                        Logging.LogException(logger, ex);
                        BusyIndicatorNotification();
                    }
                    Logging.LogEndMethod(logger, childUnloadedMethodNamespace);
                    #endregion
                };
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }        

        /// <summary>
        /// RemoveEntityGroupCommand Execution method - removes selected group from snapshot
        /// </summary>
        /// <param name="param">Sender</param>
        private void RemoveEntityGroupCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                Prompt.ShowDialog("Remove Group - '" + SelectedMarketSnapshotPerformanceInfo.MarketSnapshotPreferenceInfo.GroupName
                    + "'?", "Remove Group", MessageBoxButton.OKCancel, (messageResult) =>
                {
                    if (messageResult == MessageBoxResult.OK)
                    {
                        BusyIndicatorNotification(true, "Removing selected group from the snapshot");
                        //Remove Entity from the snapshot                    
                        if (MarketSnapshotPerformanceInfo != null)
                        {
                            foreach (MarketSnapshotPerformanceData entry in MarketSnapshotPerformanceInfo
                                .Where(record => record.MarketSnapshotPreferenceInfo.GroupName
                                    == SelectedMarketSnapshotPerformanceInfo.MarketSnapshotPreferenceInfo.GroupName)
                                    .ToList())
                            {
                                MarketSnapshotPerformanceInfo.Remove(entry);
                            }

                            MarketSnapshotPreferenceInfo = MarketSnapshotPerformanceInfo
                                .Select(record => record.MarketSnapshotPreferenceInfo)
                                .ToList();
                        }

                        BusyIndicatorNotification();
                    }
                });
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            finally
            {
                Logging.LogEndMethod(logger, methodNamespace);
                BusyIndicatorNotification();
            }
        }

        /// <summary>
        /// AddEntityToGroupCommand Execution method - adds entity to group and places it over the selected entity
        /// </summary>
        /// <param name="param">sender</param>
        private void AddEntityToGroupCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                //Cases Where EntitySelectionInfo Data Asychronous call thread is still active
                if (EntitySelectionInfo == null)
                {
                    if (SelectionData.EntitySelectionData != null)
                        RetrieveEntitySelectionDataCallbackMethod(SelectionData.EntitySelectionData);
                    Prompt.ShowDialog("Entity Selection data is being retrieved");
                    return;
                }

                ChildViewInsertEntity childViewModelInsertEntity
                    = new ChildViewInsertEntity(EntitySelectionInfo, DbInteractivity, logger, SelectedMarketSnapshotPerformanceInfo.MarketSnapshotPreferenceInfo.GroupName);
                childViewModelInsertEntity.Show();
                childViewModelInsertEntity.Unloaded += (se, e) =>
                {
                    #region childViewModelInsertEntity Unloaded Event Handler
                    string childUnloadedMethodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
                    Logging.LogBeginMethod(logger, childUnloadedMethodNamespace);
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

                                insertedMarketSnapshotPreference.GroupName = SelectedMarketSnapshotPerformanceInfo.MarketSnapshotPreferenceInfo.GroupName;
                                insertedMarketSnapshotPreference.GroupPreferenceID = SelectedMarketSnapshotPerformanceInfo.MarketSnapshotPreferenceInfo.GroupPreferenceID;                                
                                insertedMarketSnapshotPreference.EntityOrder = SelectedMarketSnapshotPerformanceInfo.MarketSnapshotPreferenceInfo.EntityOrder;

                                //Service call to receive Market Performance Snapshot Data
                                if (DbInteractivity != null && IsActive)
                                {
                                    BusyIndicatorNotification(true, "Retrieving performance data for inserted entity ...");
                                    DbInteractivity.RetrieveMarketSnapshotPerformanceData(new List<MarketSnapshotPreference> { insertedMarketSnapshotPreference }
                                           , RetrieveMarketSnapshotPerformanceDataByEntityCallbackMethod);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                        Logging.LogException(logger, ex);
                        BusyIndicatorNotification();
                    }
                    Logging.LogEndMethod(logger, childUnloadedMethodNamespace);
                    #endregion
                };
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }

        /// <summary>
        /// RemoveEntityFromGroupCommand Execution method - removes selected entity from group
        /// </summary>
        /// <param name="param">sender</param>
        private void RemoveEntityFromGroupCommandMethod(object param)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                Prompt.ShowDialog("Remove Entity - '" + SelectedMarketSnapshotPerformanceInfo.MarketSnapshotPreferenceInfo.EntityName + "'?", "Remove Entity", MessageBoxButton.OKCancel, (result) =>
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
                                        if (entity.GroupPreferenceID != SelectedMarketSnapshotPerformanceInfo.MarketSnapshotPreferenceInfo.GroupPreferenceID)
                                            continue;
                                        if (entity.EntityOrder < SelectedMarketSnapshotPerformanceInfo.MarketSnapshotPreferenceInfo.EntityOrder)
                                            continue;
                                        entity.EntityOrder--;
                                    }

                                    //Remove Entity from the snapshot
                                    MarketSnapshotPerformanceInfo.Remove(SelectedMarketSnapshotPerformanceInfo);

                                    //Reorder MarketSnapshotPreferenceInfo
                                    MarketSnapshotPreferenceInfo = MarketSnapshotPerformanceInfo
                                            .Select(record => record.MarketSnapshotPreferenceInfo)
                                            .ToList();
                                }
                            }
                            catch (Exception ex)
                            {
                                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                                Logging.LogException(logger, ex);
                            }
                            finally
                            {
                                BusyIndicatorNotification();
                            }
                        }
                    });
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            finally
            {
                Logging.LogEndMethod(logger, methodNamespace);
                BusyIndicatorNotification();
            }
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
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result, 1);
                    SelectedMarketSnapshotSelectionInfo = result;

                    #region RetrieveMarketSnapshotPreference Service Call
                    if (DbInteractivity != null && SelectedMarketSnapshotSelectionInfo != null && IsActive)
                    {
                        BusyIndicatorNotification(true, "Retrieving preference structure for selected snapshot ...");
                        DbInteractivity.RetrieveMarketSnapshotPreference(SelectedMarketSnapshotSelectionInfo.SnapshotPreferenceId
                            , RetrieveMarketSnapshotPreferenceCallbackMethod);
                    }
                    #endregion
                }
                else
                {
                    Logging.LogMethodParameterNull(logger, methodNamespace, 1);
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);
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
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result, 1);
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
                    Logging.LogMethodParameterNull(logger, methodNamespace, 1);
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }

        /// <summary>
        /// MarketPerformanceSnapshotSaveAction Event Handler
        /// </summary>
        private void HandleMarketPerformanceSnapshotSaveActionEvent()
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                BusyIndicatorNotification(true, "Retrieving preference changes ...");
                GetMarketSnapshotPreferenceCRUDInfo();

                BusyIndicatorNotification(true, "Updating preference changes ...");
                string updateXML = SaveXmlBuilder();
                if (updateXML != String.Empty && DbInteractivity != null)
                {
                    DbInteractivity.SaveMarketSnapshotPreference(updateXML, SaveMarketSnapshotPreferenceCallbackMethod);
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
                BusyIndicatorNotification();
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }

        /// <summary>
        /// MarketPerformanceSnapshotSaveAsAction Event Handler
        /// </summary>
        private void HandleMarketPerformanceSnapshotSaveAsActionEvent()
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                ChildViewInsertSnapshot childViewInsertSnapshot = new ChildViewInsertSnapshot(MarketSnapshotSelectionInfo);
                childViewInsertSnapshot.Show();
                childViewInsertSnapshot.Unloaded += new RoutedEventHandler(childViewInsertSnapshot_Unloaded_SaveAs);
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }

        /// <summary>
        ///  MarketPerformanceSnapshotAddAction Event Handler
        /// </summary>
        private void HandleMarketPerformanceSnapshotAddActionEvent()
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                ChildViewInsertSnapshot childViewInsertSnapshot = new ChildViewInsertSnapshot(MarketSnapshotSelectionInfo);
                childViewInsertSnapshot.Show();
                childViewInsertSnapshot.Unloaded += new RoutedEventHandler(childViewInsertSnapshot_Unloaded_Add);
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }

        /// <summary>
        ///  MarketPerformanceSnapshotRemoveAction Event Handler
        /// </summary>
        private void HandleMarketPerformanceSnapshotRemoveActionEvent()
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                Prompt.ShowDialog("Remove Snapshot - '" + SelectedMarketSnapshotSelectionInfo.SnapshotName + "' ?", "", MessageBoxButton.OKCancel, (messageResult) =>
                {
                    if (messageResult == MessageBoxResult.OK)
                    {
                        if (DbInteractivity != null)
                        {
                            if (SessionManager.SESSION != null)
                            {
                                BusyIndicatorNotification(true, "Removing selected snapshot ...");
                                DbInteractivity.RemoveMarketSnapshotPreference(SessionManager.SESSION.UserName
                                    , SelectedMarketSnapshotSelectionInfo.SnapshotName, RemoveMarketSnapshotPreferenceCallbackMethod);
                            }
                            else
                            {
                                BusyIndicatorNotification(true, "Retreiving session details ...");
                                manageSessions.GetSession((session) =>
                                {
                                    string sessionMethodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
                                    Logging.LogBeginMethod(logger, sessionMethodNamespace);
                                    try
                                    {
                                        if (session != null)
                                        {
                                            Logging.LogMethodParameter(logger, sessionMethodNamespace, session, 1);
                                            SessionManager.SESSION = session;

                                            BusyIndicatorNotification(true, "Removing selected snapshot ...");
                                            DbInteractivity.RemoveMarketSnapshotPreference(SessionManager.SESSION.UserName
                                                , SelectedMarketSnapshotSelectionInfo.SnapshotName, RemoveMarketSnapshotPreferenceCallbackMethod);
                                        }
                                        else
                                        {
                                            Logging.LogMethodParameterNull(logger, sessionMethodNamespace, 1);
                                            BusyIndicatorNotification();
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                                        Logging.LogException(logger, ex);
                                    }
                                    Logging.LogEndMethod(logger, sessionMethodNamespace);
                                });
                            }
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);
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
            Logging.LogBeginMethod(logger, childUnloadedMethodNamespace);
            try
            {
                ChildViewInsertSnapshot view = sender as ChildViewInsertSnapshot;
                if (view.DialogResult == true)
                {
                    if (view.tbSnapshotName.Text != String.Empty)
                    {
                        if (DbInteractivity != null)
                        {
                            if (SessionManager.SESSION != null)
                            {
                                BusyIndicatorNotification(true, "Creating snapshot and assigning preference structure...");

                                string saveAsXml = SaveAsXmlBuilder(SessionManager.SESSION.UserName, view.tbSnapshotName.Text);
                                DbInteractivity.SaveAsMarketSnapshotPreference(saveAsXml, SaveAsMarketSnapshotPreferenceCallbackMethod);
                            }
                            else
                            {
                                BusyIndicatorNotification(true, "Retreiving session details ...");
                                manageSessions.GetSession((session) =>
                                {
                                    string sessionMethodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
                                    Logging.LogBeginMethod(logger, sessionMethodNamespace);
                                    try
                                    {
                                        if (session != null)
                                        {
                                            Logging.LogMethodParameter(logger, sessionMethodNamespace, session, 1);
                                            SessionManager.SESSION = session;

                                            BusyIndicatorNotification(true, "Creating snapshot and assigning preference structure...");
                                            string saveAsXml = SaveAsXmlBuilder(session.UserName, view.tbSnapshotName.Text);
                                            DbInteractivity.SaveAsMarketSnapshotPreference(saveAsXml, SaveAsMarketSnapshotPreferenceCallbackMethod);
                                        }
                                        else
                                        {
                                            Logging.LogMethodParameterNull(logger, sessionMethodNamespace, 1);
                                            BusyIndicatorNotification();
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                                        Logging.LogException(logger, ex);
                                    }
                                    Logging.LogEndMethod(logger, sessionMethodNamespace);
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
                Logging.LogException(logger, ex);
                BusyIndicatorNotification();
            }
            Logging.LogEndMethod(logger, childUnloadedMethodNamespace);
        }

        /// <summary>
        /// ChildViewInsertSnapshot Unloaded Event Handler for Add Action
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">RoutedEventArgs</param>
        void childViewInsertSnapshot_Unloaded_Add(object sender, RoutedEventArgs e)
        {
            string childUnloadedMethodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, childUnloadedMethodNamespace);
            try
            {
                ChildViewInsertSnapshot view = sender as ChildViewInsertSnapshot;
                if (view.DialogResult == true)
                {
                    if (view.tbSnapshotName.Text != String.Empty)
                    {
                        if (DbInteractivity != null)
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
                                DbInteractivity.SaveAsMarketSnapshotPreference(doc.ToString(), AddMarketSnapshotPreferenceCallbackMethod);
                            }
                            else
                            {
                                BusyIndicatorNotification(true, "Retreiving session details ...");
                                manageSessions.GetSession((session) =>
                                {
                                    string sessionMethodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
                                    Logging.LogBeginMethod(logger, sessionMethodNamespace);
                                    try
                                    {
                                        if (session != null)
                                        {
                                            Logging.LogMethodParameter(logger, sessionMethodNamespace, session, 1);
                                            SessionManager.SESSION = session;
                                            BusyIndicatorNotification(true, "Creating snapshot ...");                                            
                                            XDocument doc = new XDocument(
                                                new XDeclaration("1.0", "utf-8", "yes"),
                                                new XComment("Market performance snapshot save as preference details"),
                                                new XElement("Root",
                                                    new XAttribute("UserName", session.UserName),
                                                    new XAttribute("SnapshotName", view.tbSnapshotName.Text)));
                                            DbInteractivity.SaveAsMarketSnapshotPreference(doc.ToString(), AddMarketSnapshotPreferenceCallbackMethod);
                                        }
                                        else
                                        {
                                            Logging.LogMethodParameterNull(logger, sessionMethodNamespace, 1);
                                            BusyIndicatorNotification();
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                                        Logging.LogException(logger, ex);
                                        BusyIndicatorNotification();
                                    }
                                    Logging.LogEndMethod(logger, sessionMethodNamespace);
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
                Logging.LogException(logger, ex);
                BusyIndicatorNotification();
            }
            Logging.LogEndMethod(logger, childUnloadedMethodNamespace);
        }
        #endregion
        #endregion

        #region Callback Methods
        /// <summary>
        /// Callback method for RetrieveMarketSnapshotPreference Service call - Gets user's Snapshot preference for the selected Snapshot
        /// Updates MarketSnapshotPreferenceOriginalInfo and MarketSnapshotPreferenceInfo
        /// </summary>
        /// <param name="result">List of MarketSnapshotPreference objects</param>
        private void RetrieveMarketSnapshotPreferenceCallbackMethod(List<MarketSnapshotPreference> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result, 1);
                    if (DbInteractivity != null)
                    {
                        MarketSnapshotPreferenceOriginalInfo = GetMarketSnapshotPreferenceDeepCopy(result);
                        MarketSnapshotPreferenceInfo = GetMarketSnapshotPreferenceDeepCopy(result);

                        BusyIndicatorNotification(true, "Retrieving performance data based on snapshot preference ...");
                        DbInteractivity.RetrieveMarketSnapshotPerformanceData(result, RetrieveMarketSnapshotPerformanceDataCallbackMethod);
                    }
                    else
                    {
                        BusyIndicatorNotification();
                    }
                }
                else
                {
                    Logging.LogMethodParameterNull(logger, methodNamespace, 1);
                    BusyIndicatorNotification();
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
                BusyIndicatorNotification();
            }
            finally
            {
                Logging.LogEndMethod(logger, methodNamespace);
            }
        }

        /// <summary>
        /// Callback method for RetrieveMarketSnapshotPerformanceData Service call - Gets performance data for entities enlisted in the selected snapshot
        /// </summary>
        /// <param name="result">List of MarketSnapshotPerformanceData objects</param>
        private void RetrieveMarketSnapshotPerformanceDataCallbackMethod(List<MarketSnapshotPerformanceData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result, 1);
                    MarketSnapshotPerformanceInfo = new ObservableCollection<MarketSnapshotPerformanceData>(result);
                }
                else
                {
                    Logging.LogMethodParameterNull(logger, methodNamespace, 1);
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            finally
            {
                Logging.LogEndMethod(logger, methodNamespace);
                BusyIndicatorNotification();
            }
        }

        /// <summary>
        /// Callback method for RetrieveMarketSnapshotPerformanceData Service call - Gets performance data for propertyName single entity
        /// </summary>
        /// <param name="result">List of MarketSnapshotPerformanceData objects</param>
        private void RetrieveMarketSnapshotPerformanceDataByEntityCallbackMethod(List<MarketSnapshotPerformanceData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    if (result.Count == 1)
                    {
                        Logging.LogMethodParameter(logger, methodNamespace, result, 1);
                        MarketSnapshotPerformanceData insertedMarketSnapshotPerformanceData = result.FirstOrDefault();

                        //Add the inserted entity to MarketSnapshotPreferenceInfo
                        if (MarketSnapshotPreferenceInfo == null)
                        {
                            MarketSnapshotPreferenceInfo = new List<MarketSnapshotPreference>();
                        }

                        if (MarketSnapshotPreferenceInfo.Where(record =>
                            record.GroupPreferenceID == insertedMarketSnapshotPerformanceData.MarketSnapshotPreferenceInfo.GroupPreferenceID).Count() > 0)
                        {
                            foreach (MarketSnapshotPreference entity in MarketSnapshotPreferenceInfo)
                            {
                                if (entity.GroupPreferenceID != insertedMarketSnapshotPerformanceData.MarketSnapshotPreferenceInfo.GroupPreferenceID)
                                    continue;
                                if (entity.EntityOrder < insertedMarketSnapshotPerformanceData.MarketSnapshotPreferenceInfo.EntityOrder)
                                    continue;
                                entity.EntityOrder++;
                            }
                        }

                        MarketSnapshotPreferenceInfo.Add(insertedMarketSnapshotPerformanceData.MarketSnapshotPreferenceInfo);

                        //Reorder MarketSnapshotPreferenceInfo by GroupPreferenceID and then by EntityOrder
                        MarketSnapshotPreferenceInfo = MarketSnapshotPreferenceInfo
                            .OrderBy(record => record.GroupPreferenceID)
                            .ThenBy(record => record.EntityOrder)
                            .ToList();

                        //Add Performance data for the entity to MarketPerformanceSnapshotInfo
                        if (MarketSnapshotPerformanceInfo == null)
                        {
                            MarketSnapshotPerformanceInfo = new ObservableCollection<MarketSnapshotPerformanceData>();
                        }
                        MarketSnapshotPerformanceInfo.Add(insertedMarketSnapshotPerformanceData);

                        //Reorder MarketPerformanceSnapshotInfo
                        MarketSnapshotPerformanceInfo = new ObservableCollection<MarketSnapshotPerformanceData>(MarketSnapshotPerformanceInfo
                            .OrderBy(record => record.MarketSnapshotPreferenceInfo.GroupPreferenceID)
                            .ThenBy(record => record.MarketSnapshotPreferenceInfo.EntityOrder)
                            .ToList());
                    }
                    else
                    {
                        Logging.LogMethodParameterNull(logger, methodNamespace, 1);
                    }
                }
                else
                {
                    Logging.LogMethodParameterNull(logger, methodNamespace, 1);
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            finally
            {
                Logging.LogEndMethod(logger, methodNamespace);
                BusyIndicatorNotification();
            }
        }

        /// <summary>
        /// Callback method for SaveMarketSnapshotPreference Service call - saves user's Snapshot preference for the selected Snapshot
        /// </summary>
        /// <param name="result">update list of MarketSnapshotPreference</param>
        private void SaveMarketSnapshotPreferenceCallbackMethod(List<MarketSnapshotPreference> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result, 1);

                    #region Reassign the preference details to local properties
                    MarketSnapshotPreferenceOriginalInfo = GetMarketSnapshotPreferenceDeepCopy(result);
                    MarketSnapshotPreferenceInfo = GetMarketSnapshotPreferenceDeepCopy(result);
                    #endregion

                    #region Update MarketPerformanceSnapshotInfo
                    if (MarketSnapshotPerformanceInfo != null)
                    {
                        foreach (MarketSnapshotPerformanceData data in MarketSnapshotPerformanceInfo)
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

                    #region Publish Action Completion Event
                    //Raise event for completion of the save action event
                    eventAggregator.GetEvent<MarketPerformanceSnapshotActionCompletionEvent>()
                        .Publish(new MarketPerformanceSnapshotActionPayload()
                        {
                            ActionType = MarketPerformanceSnapshotActionType.SNAPSHOT_SAVE,
                            SelectedMarketSnapshotSelectionInfo = SelectedMarketSnapshotSelectionInfo,
                            MarketSnapshotSelectionInfo = MarketSnapshotSelectionInfo,
                        });
                    #endregion
                }
                else
                {
                    Logging.LogMethodParameterNull(logger, methodNamespace, 1);
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            finally
            {
                Logging.LogEndMethod(logger, methodNamespace);
                BusyIndicatorNotification();
            }
        }

        /// <summary>
        /// Callback method for RetrieveEntitySelectionData Service call - Gets all Entity available for selection
        /// </summary>
        /// <param name="result">List of EntitySelectionData objects</param>
        public void RetrieveEntitySelectionDataCallbackMethod(List<EntitySelectionData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result, 1);

                    //Entity Selection Data for Currency is not required
                    EntitySelectionInfo = result.Where(record => record.Type != EntityType.CURRENCY).ToList();
                }
                else
                {
                    Logging.LogMethodParameterNull(logger, methodNamespace, 1);
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            finally
            {
                Logging.LogEndMethod(logger, methodNamespace);
                BusyIndicatorNotification();
            }
        }

        /// <summary>
        /// Callback method for SaveAsMarketSnapshotPreference Service call - creates propertyName new snapshot with assigned name and existing structure
        /// </summary>
        /// <param name="result">List of MarketSnapshotSelectionData objects</param>
        private void SaveAsMarketSnapshotPreferenceCallbackMethod(PopulatedMarketSnapshotPerformanceData result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result, 1);

                    #region Update Local Snapshot Selection Information
                    SelectedMarketSnapshotSelectionInfo = result.MarketSnapshotSelectionInfo;
                    if (MarketSnapshotSelectionInfo == null)
                    {
                        MarketSnapshotSelectionInfo = new List<MarketSnapshotSelectionData>();
                    }
                    MarketSnapshotSelectionInfo.Add(SelectedMarketSnapshotSelectionInfo);
                    #endregion

                    #region Update Local Snapshot Preference Information
                    MarketSnapshotPreferenceInfo = result.MarketPerformanceSnapshotInfo
                                    .Select(record => record.MarketSnapshotPreferenceInfo)
                                    .ToList();

                    MarketSnapshotPreferenceOriginalInfo = GetMarketSnapshotPreferenceDeepCopy(MarketSnapshotPreferenceInfo);
                    #endregion

                    #region Publish Action Completion Event
                    eventAggregator.GetEvent<MarketPerformanceSnapshotActionCompletionEvent>()
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
                    Logging.LogMethodParameterNull(logger, methodNamespace, 1);
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            finally
            {
                Logging.LogEndMethod(logger, methodNamespace);
                BusyIndicatorNotification();
            }
        }

        /// <summary>
        /// Callback method for SaveAsMarketSnapshotPreference Service call - creates propertyName new snapshot with assigned name and blank structure
        /// </summary>
        /// <param name="result">Added snapshot details</param>
        private void AddMarketSnapshotPreferenceCallbackMethod(PopulatedMarketSnapshotPerformanceData result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result, 1);
                    MarketSnapshotPerformanceInfo = new ObservableCollection<MarketSnapshotPerformanceData>();

                    SelectedMarketSnapshotSelectionInfo = result.MarketSnapshotSelectionInfo;
                    if (MarketSnapshotSelectionInfo == null)
                    {
                        MarketSnapshotSelectionInfo = new List<MarketSnapshotSelectionData>();
                    }
                    MarketSnapshotSelectionInfo.Add(result.MarketSnapshotSelectionInfo);

                    MarketSnapshotPreferenceOriginalInfo = new List<MarketSnapshotPreference>();
                    MarketSnapshotPreferenceInfo = new List<MarketSnapshotPreference>();

                    eventAggregator.GetEvent<MarketPerformanceSnapshotActionCompletionEvent>()
                        .Publish(new MarketPerformanceSnapshotActionPayload()
                        {
                            ActionType = MarketPerformanceSnapshotActionType.SNAPSHOT_ADD,
                            SelectedMarketSnapshotSelectionInfo = result.MarketSnapshotSelectionInfo,
                            MarketSnapshotSelectionInfo = MarketSnapshotSelectionInfo
                        });
                }
                else
                {
                    Logging.LogMethodParameterNull(logger, methodNamespace, 1);
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            finally
            {
                Logging.LogEndMethod(logger, methodNamespace);
                BusyIndicatorNotification();
            }
        }

        /// <summary>
        /// Callback method for RemoveMarketSnapshotPreference Service call - removes selected snapshot
        /// </summary>
        /// <param name="result">True/False</param>
        private void RemoveMarketSnapshotPreferenceCallbackMethod(bool? result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    if (result == true)
                    {
                        Logging.LogMethodParameter(logger, methodNamespace, result, 1);

                        if (MarketSnapshotSelectionInfo != null)
                        {
                            MarketSnapshotSelectionInfo.Remove(SelectedMarketSnapshotSelectionInfo);
                        }
                        SelectedMarketSnapshotSelectionInfo = null;

                        MarketSnapshotPerformanceInfo = new ObservableCollection<MarketSnapshotPerformanceData>();
                        MarketSnapshotPreferenceOriginalInfo = new List<MarketSnapshotPreference>();
                        MarketSnapshotPreferenceInfo = new List<MarketSnapshotPreference>();

                        eventAggregator.GetEvent<MarketPerformanceSnapshotActionCompletionEvent>()
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
                        Logging.LogMethodParameterFalse(logger, methodNamespace, 1);
                    }
                }
                else
                {
                    Logging.LogMethodParameterNull(logger, methodNamespace, 1);
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            finally
            {
                Logging.LogEndMethod(logger, methodNamespace);
                BusyIndicatorNotification();
            }
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
            Logging.LogBeginMethod(logger, methodNamespace);
            int lastGroupPreferenceId = 0;
            try
            {
                if (MarketSnapshotPerformanceInfo != null)
                {
                    foreach (MarketSnapshotPerformanceData record in MarketSnapshotPerformanceInfo)
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
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);
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

            createPreferenceInfo = crtPreferenceInfo;
            updatePreferenceInfo = updPreferenceInfo;
            deletePreferenceInfo = delPreferenceInfo;
            deleteGroupInfo = delGroupInfo;
            createGroupInfo = crtGroupInfo;
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

                foreach (string groupName in createGroupInfo)
                {
                    XElement createGroup = new XElement("CreateGroup", new XAttribute("GroupName", groupName));

                    foreach (MarketSnapshotPreference preference in createPreferenceInfo)
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
                        }
                    }

                    root.Add(createGroup);
                }

                foreach (MarketSnapshotPreference preference in createPreferenceInfo)
                {
                    if (!createGroupInfo.Contains(preference.GroupName))
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
                    }
                }

                foreach (int groupPreferenceId in deleteGroupInfo)
                {
                    XElement deleteGroup = new XElement("DeleteGroup",
                        new XAttribute("GroupPreferenceId", groupPreferenceId.ToString()));

                    root.Add(deleteGroup);
                }

                foreach (MarketSnapshotPreference preference in deletePreferenceInfo)
                {
                    XElement deleteEntity = new XElement("DeleteEntity",
                        new XAttribute("EntityPreferenceId", preference.EntityPreferenceId.ToString()));

                    root.Add(deleteEntity);
                }

                foreach (MarketSnapshotPreference preference in updatePreferenceInfo)
                {
                    XElement updateEntity = new XElement("UpdateEntity",
                        new XAttribute("GroupPreferenceId", preference.GroupPreferenceID.ToString()),
                        new XAttribute("EntityPreferenceId", preference.EntityPreferenceId.ToString()),
                        new XAttribute("EntityOrder", preference.EntityOrder.ToString()));

                    root.Add(updateEntity);
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

        /// <summary>
        /// Dispose objects from memory
        /// </summary>
        public void Dispose()
        {
            eventAggregator.GetEvent<MarketPerformanceSnapshotReferenceSetEvent>().Unsubscribe(HandleMarketPerformanceSnapshotNameReferenceSetEvent);
            eventAggregator.GetEvent<MarketPerformanceSnapshotActionEvent>().Unsubscribe(HandleMarketPerformanceSnapshotActionEvent);
        }
        #endregion        
    }
}

