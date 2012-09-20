using System;
using System.Net;
using System.Linq;
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
using GreenField.ServiceCaller.MeetingDefinitions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Commands;
using GreenField.Gadgets.Models;
using Microsoft.Practices.Prism.Regions;
using GreenField.Common;
using GreenField.Gadgets.Views;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;
using GreenField.Gadgets.Helpers;
using GreenField.DataContracts;
using GreenField.UserSession;
using System.Xml.Linq;

namespace GreenField.Gadgets.ViewModels
{
    public class ViewModelCSTDataFieldSelector : NotificationObject
    {
        #region Fields
        /// <summary>
        /// MEF Singletons
        /// </summary>
        private IEventAggregator _eventAggregator;
        private IDBInteractivity _dbInteractivity;
        private ILoggerFacade _logger;
        private IManageSessions _manageSessions;
        private IRegionManager _regionManager;

        string userEnteredListName;
        string userEnteredAccessibility;
        #endregion

        #region Constructor
        public ViewModelCSTDataFieldSelector(DashboardGadgetParam param)
        {
            _logger = param.LoggerFacade;
            _dbInteractivity = param.DBInteractivity;
            _eventAggregator = param.EventAggregator;
            _manageSessions = param.ManageSessions;
            _regionManager = param.RegionManager;

            //fetch tabs data
            //FetchTabsData();

            //Flag = CSTNavigation.FetchString(CSTNavigationInfo.Flag) as string;
            //if (Flag == "Edit")
            //{
            //    SelectedDataList = CSTNavigation.Fetch(CSTNavigationInfo.SelectedDataList) as List<CSTUserPreferenceInfo>;
            //}
        }

        #endregion

        #region Properties

        public ObservableCollection<CSTUserPreferenceInfo> _selectedFieldsDataList;
        public ObservableCollection<CSTUserPreferenceInfo> SelectedFieldsDataList
        {
            get { return _selectedFieldsDataList; }
            set
            {
                _selectedFieldsDataList = value;
                RaisePropertyChanged(() => this.SelectedFieldsDataList);
            }
        }

        public CSTUserPreferenceInfo _selectedDataField;
        public CSTUserPreferenceInfo SelectedDataField
        {
            get { return _selectedDataField; }
            set
            {
                _selectedDataField = value;
                RaisePropertyChanged(() => this.SelectedDataField);
                RaisePropertyChanged(() => this.RemoveCommand);
            }
        }

        public string Flag { get; set; }

        public List<CustomSelectionData> _securityReferenceData;
        public List<CustomSelectionData> SecurityReferenceData
        {
            get { return _securityReferenceData; }
            set
            {
                _securityReferenceData = value;
                RaisePropertyChanged(() => this.SecurityReferenceData);
            }
        }

        public CustomSelectionData _selectedSecurityReferenceData;
        public CustomSelectionData SelectedSecurityReferenceData
        {
            get { return _selectedSecurityReferenceData; }
            set
            {
                _selectedSecurityReferenceData = value;
                RaisePropertyChanged(() => this.SelectedSecurityReferenceData);
                RaisePropertyChanged(() => this.AddSecurityRefCommand);
                RaisePropertyChanged(() => this.RemoveCommand);
            }
        }

        public List<CustomSelectionData> _periodFinancialsData;
        public List<CustomSelectionData> PeriodFinancialsData
        {
            get { return _periodFinancialsData; }
            set
            {
                _periodFinancialsData = value;
                RaisePropertyChanged(() => this.PeriodFinancialsData);
            }
        }

        public CustomSelectionData _selectedPeriodFinancialsData;
        public CustomSelectionData SelectedPeriodFinancialsData
        {
            get { return _selectedPeriodFinancialsData; }
            set
            {
                _selectedPeriodFinancialsData = value;
                RaisePropertyChanged(() => this.SelectedPeriodFinancialsData);
                RaisePropertyChanged(() => this.AddPeriodFinCommand);
                RaisePropertyChanged(() => this.RemoveCommand);
            }
        }

        public List<CustomSelectionData> _currentFinancialsData;
        public List<CustomSelectionData> CurrentFinancialsData
        {
            get { return _currentFinancialsData; }
            set
            {
                _currentFinancialsData = value;
                RaisePropertyChanged(() => this.CurrentFinancialsData);
            }
        }

        public CustomSelectionData _selectedCurrentFinancialsData;
        public CustomSelectionData SelectedCurrentFinancialsData
        {
            get { return _selectedCurrentFinancialsData; }
            set
            {
                _selectedCurrentFinancialsData = value;
                RaisePropertyChanged(() => this.SelectedCurrentFinancialsData);
                RaisePropertyChanged(() => this.AddCurrentFinCommand);
                RaisePropertyChanged(() => this.RemoveCommand);
            }
        }

        public List<CustomSelectionData> _fairValueData;
        public List<CustomSelectionData> FairValueData
        {
            get { return _fairValueData; }
            set
            {
                _fairValueData = value;
                RaisePropertyChanged(() => this.FairValueData);
            }
        }

        public CustomSelectionData _selectedFairValueData;
        public CustomSelectionData SelectedFairValueData
        {
            get { return _selectedFairValueData; }
            set
            {
                _selectedFairValueData = value;
                RaisePropertyChanged(() => this.SelectedFairValueData);
                RaisePropertyChanged(() => this.AddFairValueCommand);
                RaisePropertyChanged(() => this.RemoveCommand);
            }
        }

        public List<String> DataSourceInfo
        {
            get { return new List<String> { "PRIMARY", "INDUSTRY", "REUTERS" }; }

        }

        public String _selectedDataSourceInfo = "PRIMARY";
        public String SelectedDataSourceInfo
        {
            get { return _selectedDataSourceInfo; }
            set
            {
                _selectedDataSourceInfo = value;
                RaisePropertyChanged(() => this.SelectedDataSourceInfo);
            }
        }

        private List<string> _fairvalueDataSourceInfo;
        public List<string> FairvalueDataSourceInfo
        {
            get { return _fairvalueDataSourceInfo; }
            set
            {
                if (_fairvalueDataSourceInfo != value)
                {
                    _fairvalueDataSourceInfo = value;
                    RaisePropertyChanged(() => FairvalueDataSourceInfo);
                }
            }
        }

        public String _fairvalueSelectedDataSourceInfo = "PRIMARY";
        public String FairvalueSelectedDataSourceInfo
        {
            get { return _fairvalueSelectedDataSourceInfo; }
            set
            {
                _fairvalueSelectedDataSourceInfo = value;
                RaisePropertyChanged(() => this.FairvalueSelectedDataSourceInfo);
            }
        }

        public List<String> YearTypeInfo
        {
            get { return new List<String> { "CALENDAR", "FISCAL" }; }

        }

        public String _selectedYearTypeInfo = "CALENDAR";
        public String SelectedYearTypeInfo
        {
            get { return _selectedYearTypeInfo; }
            set
            {
                _selectedYearTypeInfo = value;
                RaisePropertyChanged(() => this.SelectedYearTypeInfo);
            }
        }

        public List<String> PeriodTypeInfo
        {
            get { return new List<String> { "ANNUAL", "Q1", "Q2", "Q3", "Q4" }; }

        }

        public String _selectedPeriodTypeInfo = "ANNUAL";
        public String SelectedPeriodTypeInfo
        {
            get { return _selectedPeriodTypeInfo; }
            set
            {
                _selectedPeriodTypeInfo = value;
                RaisePropertyChanged(() => this.SelectedPeriodTypeInfo);
            }
        }

        public List<int> FromYearInfo
        {
            get
            {
                int currentYear = DateTime.Now.Year;
                return new List<int> { currentYear, currentYear - 10, currentYear - 9, currentYear - 8, currentYear - 7, currentYear - 6, currentYear - 5,
                                       currentYear - 4, currentYear - 3, currentYear - 2, currentYear - 1, currentYear + 1, currentYear + 2,
                                       currentYear + 3, currentYear + 4, currentYear + 5 };
            }

        }

        public int _selectedFromYearInfo = DateTime.Now.Year;
        public int SelectedFromYearInfo
        {
            get { return _selectedFromYearInfo; }
            set
            {
                _selectedFromYearInfo = value;
                RaisePropertyChanged(() => this.SelectedFromYearInfo);
            }
        }

        public List<int> ToYearInfo
        {
            get
            {
                int currentYear = DateTime.Now.Year;
                return new List<int> { currentYear, currentYear - 10, currentYear - 9, currentYear - 8, currentYear - 7, currentYear - 6, currentYear - 5,
                                       currentYear - 4, currentYear - 3, currentYear - 2, currentYear - 1, currentYear + 1, currentYear + 2,
                                       currentYear + 3, currentYear + 4, currentYear + 5};
            }
        }

        public int _selectedToYearInfo = DateTime.Now.Year;
        public int SelectedToYearInfo
        {
            get { return _selectedToYearInfo; }
            set
            {
                _selectedToYearInfo = value;
                RaisePropertyChanged(() => this.SelectedToYearInfo);
            }
        }

        public bool _isAddButtonEnabled = false;
        public bool IsAddButtonEnabled
        {
            get { return _isAddButtonEnabled; }
            set
            {
                _isAddButtonEnabled = value;
            }
        }

        public bool _isRemoveButtonEnabled = false;
        public bool IsRemoveButtonEnabled
        {
            get { return _isRemoveButtonEnabled; }
            set
            {
                _isRemoveButtonEnabled = value;
            }
        }

        /// <summary>
        /// IsActive is true when parent control is displayed on UI
        /// </summary>
        private bool _isActive;
        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
                if (value)
                {
                    Initialize();
                }
            }
        }

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

        #region ICommand Properties

        public ICommand AddSecurityRefCommand
        {
            get { return new DelegateCommand<object>(AddSecurityRefCommandMethod, AddSecurityRefCommandValidationMethod); }
        }

        public ICommand AddPeriodFinCommand
        {
            get { return new DelegateCommand<object>(AddPeriodFinCommandMethod, AddPeriodFinCommandValidationMethod); }
        }

        public ICommand AddCurrentFinCommand
        {
            get { return new DelegateCommand<object>(AddCurrentFinCommandMethod, AddCurrentFinCommandValidationMethod); }
        }

        public ICommand AddFairValueCommand
        {
            get { return new DelegateCommand<object>(AddFairValueCommandMethod, AddFairValueCommandValidationMethod); }
        }

        public ICommand SubmitCommand
        {
            get { return new DelegateCommand<object>(SubmitCommandMethod, SubmitCommandValidationMethod); }
        }

        public ICommand RemoveCommand
        {
            get { return new DelegateCommand<object>(RemoveCommandMethod, RemoveCommandValidationMethod); }
        }

        #endregion

        #endregion

        #region ICommand Methods

        private bool AddSecurityRefCommandValidationMethod(object param)
        {
            string flag = CSTNavigation.FetchString(CSTNavigationInfo.Flag) as string;
            if (flag == "View")
            {
                IsAddButtonEnabled = false;
                return false;
            }
            else if (SelectedSecurityReferenceData != null )
            {
                if(SelectedFieldsDataList != null)
                {
                    if (SelectedFieldsDataList.Select(a => a.ScreeningId).Contains(SelectedSecurityReferenceData.ScreeningId))
                    {
                        IsAddButtonEnabled = false;
                        return false;
                    }
                    else
                    {
                        IsAddButtonEnabled = true;
                        return true;
                    }
                }
                else 
                {
                    IsAddButtonEnabled = true;
                    return true;
                }
            }
            else
            {
                IsAddButtonEnabled = false;
                return false;
            }
           
        }

        private void AddSecurityRefCommandMethod(object param)
        {
            ObservableCollection<CSTUserPreferenceInfo> temp = new ObservableCollection<CSTUserPreferenceInfo>();
            string listName;
            string accessibility;
            int tempOrder;
            if (Flag == "Edit")
            {
                temp = this.SelectedFieldsDataList;
                tempOrder = temp.Count;

                listName = SelectedFieldsDataList[0].ListName;
                accessibility = SelectedFieldsDataList[0].Accessibility;
            }
            else
            {
                if (SelectedFieldsDataList == null || SelectedFieldsDataList.Count == 0)
                {
                    listName = string.Empty;
                    accessibility = string.Empty;
                    tempOrder = -1;
                }
                else
                {
                    listName = SelectedFieldsDataList[0].ListName;
                    accessibility = SelectedFieldsDataList[0].Accessibility;
                    temp = this.SelectedFieldsDataList;
                    tempOrder = SelectedFieldsDataList.Count;
                }              
            }
            temp.Add(new CSTUserPreferenceInfo()
            {
                ScreeningId = SelectedSecurityReferenceData.ScreeningId,
                DataDescription = SelectedSecurityReferenceData.DataDescription,
                UserName = UserSession.SessionManager.SESSION.UserName,
                ListName = listName,
                Accessibility = accessibility,
                DataPointsOrder = tempOrder++,
                TableColumn = SelectedSecurityReferenceData.DataColumn
            });
            SelectedFieldsDataList = temp;
        }

        private bool AddPeriodFinCommandValidationMethod(object param)
        {
            string flag = CSTNavigation.FetchString(CSTNavigationInfo.Flag) as string;
            if (flag == "View")
            {
                IsAddButtonEnabled = false;
                return false;
            }
            else if (SelectedPeriodFinancialsData != null)
            {
                if (SelectedFieldsDataList != null)
                {
                    if (SelectedFieldsDataList.Select(a => a.ScreeningId).Contains(SelectedPeriodFinancialsData.ScreeningId))
                    {
                        IsAddButtonEnabled = false;
                        return false;
                    }
                    else
                    {
                        IsAddButtonEnabled = true;
                        return true;
                    }
                }
                else
                {
                    IsAddButtonEnabled = true;
                    return true;
                }
            }
            else
            {
                IsAddButtonEnabled = false;
                return false;
            }
        }

        private void AddPeriodFinCommandMethod(object param)
        {
            ObservableCollection<CSTUserPreferenceInfo> temp = new ObservableCollection<CSTUserPreferenceInfo>();
            int tempOrder;
            string listName;
            string accessibility;
            if (Flag == "Edit")
            {
                temp = this.SelectedFieldsDataList;
                tempOrder = temp.Count();
                listName = SelectedFieldsDataList[0].ListName;
                accessibility = SelectedFieldsDataList[0].Accessibility;
            }
            else
            {
                if (SelectedFieldsDataList == null || SelectedFieldsDataList.Count == 0)
                {
                    tempOrder = -1;
                    listName = string.Empty;
                    accessibility = string.Empty;
                }
                else
                {
                    temp = this.SelectedFieldsDataList;
                    tempOrder = SelectedFieldsDataList.Count;
                    listName = SelectedFieldsDataList[0].ListName;
                    accessibility = SelectedFieldsDataList[0].Accessibility;
                }               
            }
            for (int i = SelectedFromYearInfo; i <= SelectedToYearInfo; i++)
            {
                temp.Add(new CSTUserPreferenceInfo()
                {
                    ScreeningId = SelectedPeriodFinancialsData.ScreeningId,
                    DataDescription = SelectedPeriodFinancialsData.DataDescription,
                    UserName = UserSession.SessionManager.SESSION.UserName,
                    ListName = listName,
                    Accessibility = accessibility,
                    DataSource = SelectedDataSourceInfo,
                    PeriodType = SelectedPeriodTypeInfo,
                    YearType = SelectedYearTypeInfo,
                    FromDate = i,
                    ToDate = SelectedToYearInfo,
                    DataPointsOrder = tempOrder++,
                    TableColumn = SelectedPeriodFinancialsData.DataDescription
                });
            }
            SelectedFieldsDataList = temp;
        }

        private bool AddCurrentFinCommandValidationMethod(object param)
        {
            string flag = CSTNavigation.FetchString(CSTNavigationInfo.Flag) as string;
            if (flag == "View")
            {
                IsAddButtonEnabled = false;
                return false;
            }
            else if (SelectedCurrentFinancialsData != null)
            {
                if (SelectedFieldsDataList != null)
                {
                    if (SelectedFieldsDataList.Select(a => a.ScreeningId).Contains(SelectedCurrentFinancialsData.ScreeningId))
                    {
                        IsAddButtonEnabled = false;
                        return false;
                    }
                    else
                    {
                        IsAddButtonEnabled = true;
                        return true;
                    }
                }
                else
                {
                    IsAddButtonEnabled = true;
                    return true;
                }
            }
            else
            {
                IsAddButtonEnabled = false;
                return false;
            }
        }

        private void AddCurrentFinCommandMethod(object param)
        {
            ObservableCollection<CSTUserPreferenceInfo> temp = new ObservableCollection<CSTUserPreferenceInfo>();
            int tempOrder;
            string listName;
            string accessibility;
            if (Flag == "Edit")
            {
                listName = SelectedFieldsDataList[0].ListName;
                accessibility = SelectedFieldsDataList[0].Accessibility;
                temp = this.SelectedFieldsDataList;
                tempOrder = temp.Count();               
            }
            else
            {
                if (SelectedFieldsDataList == null || SelectedFieldsDataList.Count == 0)
                {
                    tempOrder = -1;
                    listName = string.Empty;
                    accessibility = string.Empty;
                }
                else
                {
                    listName = SelectedFieldsDataList[0].ListName;
                    accessibility = SelectedFieldsDataList[0].Accessibility;
                    temp = this.SelectedFieldsDataList;
                    tempOrder = SelectedFieldsDataList.Count;
                }
             }
            temp.Add(new CSTUserPreferenceInfo()
            {
                ScreeningId = SelectedCurrentFinancialsData.ScreeningId,
                DataDescription = SelectedCurrentFinancialsData.DataDescription,
                UserName = UserSession.SessionManager.SESSION.UserName,
                ListName = listName,
                Accessibility = accessibility,
                DataSource = SelectedDataSourceInfo,
                DataPointsOrder = tempOrder++,
                TableColumn = SelectedCurrentFinancialsData.DataDescription
            });
            SelectedFieldsDataList = temp;
        }

        private bool AddFairValueCommandValidationMethod(object param)
        {
            string flag = CSTNavigation.FetchString(CSTNavigationInfo.Flag) as string;
            if (flag == "View")
            {
                IsAddButtonEnabled = false;
                return false;
            }
            else if (SelectedFairValueData != null)
            {
                if (SelectedFieldsDataList != null)
                {
                    if (SelectedFieldsDataList.Select(a => a.ScreeningId).Contains(SelectedFairValueData.ScreeningId))
                    {
                        IsAddButtonEnabled = false;
                        return false;
                    }
                    else
                    {
                        IsAddButtonEnabled = true;
                        return true;
                    }
                }
                else
                {
                    IsAddButtonEnabled = true;
                    return true;
                }
            }
            else
            {
                IsAddButtonEnabled = false;
                return false;
            }
        }

        private void AddFairValueCommandMethod(object param)
        {
            //if (SelectedFieldsDataList != null)
            //{
            //    if (SelectedFieldsDataList.Count == 1 && SelectedFieldsDataList[0].ScreeningId == null)
            //    {
            //        return;
            //    }
            //    else if (!(SelectedFieldsDataList.Select(a => a.ScreeningId).Contains(SelectedFairValueData.ScreeningId)))
                 
            //    {
            //        return;
            //    }
            //}
            //else
            //{
                ObservableCollection<CSTUserPreferenceInfo> temp = new ObservableCollection<CSTUserPreferenceInfo>();
                int tempOrder;
                string listName;
                string accessibility;
                if (Flag == "Edit")
                {
                    temp = this.SelectedFieldsDataList;
                    tempOrder = temp.Count();
                    listName = SelectedFieldsDataList[0].ListName;
                    accessibility = SelectedFieldsDataList[0].Accessibility;
                }
                else
                {
                    if (SelectedFieldsDataList == null || SelectedFieldsDataList.Count == 0)
                    {
                        tempOrder = -1;
                        listName = string.Empty;
                        accessibility = string.Empty;
                    }
                    else
                    {
                        temp = this.SelectedFieldsDataList;
                        tempOrder = temp.Count();
                        listName = SelectedFieldsDataList[0].ListName;
                        accessibility = SelectedFieldsDataList[0].Accessibility;
                    }
                }

                temp.Add(new CSTUserPreferenceInfo()
                {
                    ScreeningId = SelectedFairValueData.ScreeningId,
                    DataDescription = SelectedFairValueData.DataDescription,
                    UserName = UserSession.SessionManager.SESSION.UserName,
                    ListName = listName,
                    Accessibility = accessibility,
                    DataSource = FairvalueSelectedDataSourceInfo,
                    DataPointsOrder = tempOrder++,
                    TableColumn = SelectedFairValueData.DataColumn
                });
                SelectedFieldsDataList = temp;
                //RaisePropertyChanged(() => this.SelectedFieldsDataList);
           // }
        }

        private bool RemoveCommandValidationMethod(object param)
        {
            string flag = CSTNavigation.FetchString(CSTNavigationInfo.Flag) as string;
            if (flag == "View")
            {
                return false;
            }
            else if (SelectedFieldsDataList == null)
            {
                return false;
            }
            else
            {
                if (SelectedDataField != null)
                {
                    IsRemoveButtonEnabled = true;
                    return true;
                }
                else
                {
                    return false; ;
                }
            }
        }

        private void RemoveCommandMethod(object param)
        {
            SelectedFieldsDataList.Remove(SelectedDataField);
        }

        private bool SubmitCommandValidationMethod(object param)
        {
            if (UserSession.SessionManager.SESSION == null)
                return false;
            else
                return true;
        }

        private void SubmitCommandMethod(object param)
        {            
            string flag = CSTNavigation.FetchString(CSTNavigationInfo.Flag) as string;
            if (flag == "View")
            {
                _regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardCustomScreeningTool", UriKind.Relative));
            }
            //prompt to ask if user wants to save list
            //if yes open child window
            //also need to send the user data selection list to the child view so that it can be updated when save clicked in child window

            else
            {
                if (SelectedFieldsDataList != null)
                {
                    CSTNavigation.UpdateString(CSTNavigationInfo.ListName, SelectedFieldsDataList[0].ListName);
                    CSTNavigation.UpdateString(CSTNavigationInfo.Accessibility, SelectedFieldsDataList[0].Accessibility);
                }
                ChildViewCSTDataListSave childViewCSTDataListSave = new ChildViewCSTDataListSave();
                childViewCSTDataListSave.Show();

                childViewCSTDataListSave.Unloaded += (se, e) =>
                {
                    if (childViewCSTDataListSave.DialogResult == true)
                    {
                        Prompt.ShowDialog("Confirm to save the list", "Save", MessageBoxButton.OKCancel, (result) =>
                            {
                                if (result == MessageBoxResult.OK)
                                {
                                    userEnteredListName = childViewCSTDataListSave.txtDataListName.Text;
                                    userEnteredAccessibility = childViewCSTDataListSave.SelectedAccessibility;
                                    if (Flag != "Edit")
                                    {
                                        if (_dbInteractivity != null)
                                        {
                                            string xmlData = SaveAsXmlBuilder(SessionManager.SESSION.UserName, SelectedFieldsDataList.ToList(), userEnteredListName, userEnteredAccessibility);
                                            if (xmlData != null)
                                            {
                                                _dbInteractivity.SaveUserDataPointsPreference(xmlData, SessionManager.SESSION.UserName, SaveUserDataPointsPreferenceCallBackMethod);
                                            }
                                        }
                                    }
                                    else if (Flag == "Edit")
                                    {
                                        if (_dbInteractivity != null && SelectedFieldsDataList[0].ListName != null && SelectedFieldsDataList[0].Accessibility != null)
                                        {
                                            string xmlData = SaveAsXmlBuilder(SessionManager.SESSION.UserName, SelectedFieldsDataList.ToList(), SelectedFieldsDataList[0].ListName, SelectedFieldsDataList[0].Accessibility);
                                            if (xmlData != null)
                                            {
                                                _dbInteractivity.UpdateUserDataPointsPreference(xmlData, SessionManager.SESSION.UserName,
                                                    SelectedFieldsDataList[0].ListName, userEnteredListName, userEnteredAccessibility, UpdateUserDataPointsPreferenceCallBackMethod);
                                            }
                                        }
                                    }
                                }
                            });
                    }
                };
            }
        }

        #endregion

        #region CallBack Methods

        private void SecurityReferenceTabDataPointsCallbackMethod(List<CustomSelectionData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);

            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result.ToString(), 1);
                    SecurityReferenceData = result;
                }
                else
                {
                    Prompt.ShowDialog("Message: Argument Null\nStackTrace: " + methodNamespace + ":result", "ArgumentNullDebug", MessageBoxButton.OK);
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            finally
            {
                Logging.LogEndMethod(_logger, methodNamespace);
                BusyIndicatorNotification();
            }
        }

        private void PeriodFinancialsTabDataPointsCallbackMethod(List<CustomSelectionData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);

            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result.ToString(), 1);
                    PeriodFinancialsData = result;
                }
                else
                {
                    Prompt.ShowDialog("Message: Argument Null\nStackTrace: " + methodNamespace + ":result", "ArgumentNullDebug", MessageBoxButton.OK);
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            finally
            {
                Logging.LogEndMethod(_logger, methodNamespace);
                BusyIndicatorNotification();
            }
        }

        private void CurrentFinancialsTabDataPointsCallbackMethod(List<CustomSelectionData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);

            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result.ToString(), 1);
                    CurrentFinancialsData = result;
                }
                else
                {
                    Prompt.ShowDialog("Message: Argument Null\nStackTrace: " + methodNamespace + ":result", "ArgumentNullDebug", MessageBoxButton.OK);
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            finally
            {
                Logging.LogEndMethod(_logger, methodNamespace);
                BusyIndicatorNotification();
            }
        }

        private void FairValueTabDataPointsCallbackMethod(List<CustomSelectionData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);

            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result.ToString(), 1);
                    FairValueData = result;
                }
                else
                {
                    Prompt.ShowDialog("Message: Argument Null\nStackTrace: " + methodNamespace + ":result", "ArgumentNullDebug", MessageBoxButton.OK);
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            finally
            {
                Logging.LogEndMethod(_logger, methodNamespace);
                BusyIndicatorNotification();
            }
        }

        private void SaveUserDataPointsPreferenceCallBackMethod(Boolean? result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);

            try
            {
                if (result == true)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result.ToString(), 1);
                    CSTNavigation.UpdateString(CSTNavigationInfo.Flag, "Created");
                    CSTNavigation.Update(CSTNavigationInfo.SelectedDataList, SelectedFieldsDataList.ToList());
                    _regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardCustomScreeningTool", UriKind.Relative));
                }
                else if (result == false)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result.ToString(), 1);
                }
                else
                {
                    Prompt.ShowDialog("Message: Argument Null\nStackTrace: " + methodNamespace + ":result", "ArgumentNullDebug", MessageBoxButton.OK);
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }

        }

        private void UpdateUserDataPointsPreferenceCallBackMethod(Boolean? result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);

            try
            {
                if (result == true)
                {
                    foreach (CSTUserPreferenceInfo item in SelectedFieldsDataList)
                    {
                        item.ListName = userEnteredListName;
                        item.Accessibility = userEnteredAccessibility;
                    }
                    Logging.LogMethodParameter(_logger, methodNamespace, result.ToString(), 1);
                    CSTNavigation.UpdateString(CSTNavigationInfo.Flag, "Edited");
                    CSTNavigation.Update(CSTNavigationInfo.SelectedDataList, SelectedFieldsDataList.ToList());
                    _regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardCustomScreeningTool", UriKind.Relative));
                }
                else if (result == false)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result.ToString(), 1);
                }
                else
                {
                    Prompt.ShowDialog("Message: Argument Null\nStackTrace: " + methodNamespace + ":result", "ArgumentNullDebug", MessageBoxButton.OK);
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }

        }

        private void RetrieveFairValueTabSourceCallbackMethod(List<string> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);

            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result.ToString(), 1);
                    FairvalueDataSourceInfo = result;
                }
                else
                {
                    Prompt.ShowDialog("Message: Argument Null\nStackTrace: " + methodNamespace + ":result", "ArgumentNullDebug", MessageBoxButton.OK);
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            finally
            {
                Logging.LogEndMethod(_logger, methodNamespace);
                BusyIndicatorNotification();
            }
        }

        #endregion

        #region EventUnSubscribe
        /// <summary>
        /// Method that disposes the events
        /// </summary>
        public void Dispose()
        {
            CSTNavigation.UpdateString(CSTNavigationInfo.Accessibility, null);
            CSTNavigation.UpdateString(CSTNavigationInfo.Flag, null);
            CSTNavigation.UpdateString(CSTNavigationInfo.ListName, null);
            CSTNavigation.Update(CSTNavigationInfo.SelectedDataList, null);
        }

        #endregion

        #region Helpers

        public void FetchTabsData()
        {
            if (_dbInteractivity != null)
            {
                BusyIndicatorNotification(true, "Retrieving Security Reference Data...");
                _dbInteractivity.RetrieveSecurityReferenceTabDataPoints(SecurityReferenceTabDataPointsCallbackMethod);
                BusyIndicatorNotification(true, "Retrieving Period Financials Data...");
                _dbInteractivity.RetrievePeriodFinancialsTabDataPoints(PeriodFinancialsTabDataPointsCallbackMethod);
                BusyIndicatorNotification(true, "Retrieving Current Financials Data...");
                _dbInteractivity.RetrieveCurrentFinancialsTabDataPoints(CurrentFinancialsTabDataPointsCallbackMethod);
                BusyIndicatorNotification(true, "Retrieving Fair Value Data...");
                _dbInteractivity.RetrieveFairValueTabDataPoints(FairValueTabDataPointsCallbackMethod);
                BusyIndicatorNotification(true, "Retrieving Fair Value DataSource...");
                _dbInteractivity.RetrieveFairValueTabSource(RetrieveFairValueTabSourceCallbackMethod);
            }
        }

        public void Initialize()
        {
            //SelectedFieldsDataList = null;
            //fetch tabs data
            FetchTabsData();
            List<CSTUserPreferenceInfo> temp = new List<CSTUserPreferenceInfo>();
            ObservableCollection<CSTUserPreferenceInfo> userPref = new ObservableCollection<CSTUserPreferenceInfo>();

            Flag = CSTNavigation.FetchString(CSTNavigationInfo.Flag) as string;
            if (Flag != null)
            {
                if (Flag == "Edit")
                {
                    temp = CSTNavigation.Fetch(CSTNavigationInfo.SelectedDataList) as List<CSTUserPreferenceInfo>;
                    foreach (CSTUserPreferenceInfo item in temp)
                    {
                        userPref.Add(item);
                    }
                    //SelectedFieldsDataList = CSTNavigation.Fetch(CSTNavigationInfo.SelectedDataList) as ObservableCollection<CSTUserPreferenceInfo>;
                    SelectedFieldsDataList = userPref;
                }
            }
            else
            {
                SelectedFieldsDataList = null;
            }
        }

        public void BusyIndicatorNotification(bool showBusyIndicator = false, String message = null)
        {
            if (message != null)
            {
                BusyIndicatorContent = message;
            }

            BusyIndicatorIsBusy = showBusyIndicator;
        }

        /// <summary>
        /// Construct XML for Save As Event
        /// </summary>
        /// <returns></returns>
        private string SaveAsXmlBuilder(String userName, List<CSTUserPreferenceInfo> userPreference, string listName, string accessibility)
        {
            string saveAsXml = String.Empty;

            try
            {
                if (userName != null && userPreference != null)
                {
                    XElement root = new XElement("Root");
                    foreach (CSTUserPreferenceInfo preference in userPreference)
                    {
                        XElement createRow = new XElement("CreateRow", new XAttribute("ListName", listName));
                        XElement createRowEntity = new XElement("CreateRowEntity");

                        createRowEntity.Add(new XAttribute("UserName", userName));
                        createRowEntity.Add(new XAttribute("ListName", listName));
                        createRowEntity.Add(new XAttribute("Accessibilty", accessibility));
                        createRowEntity.Add(new XAttribute("CreatedOn", DateTime.Now));
                        createRowEntity.Add(new XAttribute("ModifiedBy", userName));
                        createRowEntity.Add(new XAttribute("ModifiedOn", DateTime.Now));

                        createRow.Add(createRowEntity);
                        XElement createRowPreference = new XElement("CreateRowPreference");

                        createRowPreference.Add(new XAttribute("UserName", userName));
                        createRowPreference.Add(new XAttribute("ListName", listName));
                        createRowPreference.Add(new XAttribute("ScreeningId", preference.ScreeningId));
                        createRowPreference.Add(new XAttribute("DataDescription", preference.DataDescription));
                        if (preference.DataSource != null)
                            createRowPreference.Add(new XAttribute("DataSource", preference.DataSource));
                        else
                            createRowPreference.Add(new XAttribute("DataSource", string.Empty));

                        if (preference.PeriodType != null)
                            createRowPreference.Add(new XAttribute("PeriodType", preference.PeriodType));
                        else
                            createRowPreference.Add(new XAttribute("PeriodType", string.Empty));

                        if (preference.YearType != null)
                            createRowPreference.Add(new XAttribute("YearType", preference.YearType));
                        else
                            createRowPreference.Add(new XAttribute("YearType", string.Empty));

                        if (preference.FromDate != null)
                            createRowPreference.Add(new XAttribute("FromDate", preference.FromDate));
                        else
                            createRowPreference.Add(new XAttribute("FromDate", string.Empty));

                        if (preference.ToDate != null)
                            createRowPreference.Add(new XAttribute("ToDate", preference.ToDate));
                        else
                            createRowPreference.Add(new XAttribute("ToDate", string.Empty));

                        createRowPreference.Add(new XAttribute("DataPointsOrder", preference.DataPointsOrder));
                        createRowPreference.Add(new XAttribute("CreatedBy", userName));
                        createRowPreference.Add(new XAttribute("CreatedOn", DateTime.Now));
                        createRowPreference.Add(new XAttribute("ModifiedBy", userName));
                        createRowPreference.Add(new XAttribute("ModifiedOn", DateTime.Now));

                        createRow.Add(createRowPreference);
                        root.Add(createRow);
                    }
                    XDocument doc = new XDocument(
                       new XDeclaration("1.0", "utf-8", "yes"),
                       new XComment("Custom screening Tool save as preference details"), root);

                    saveAsXml = doc.ToString();
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog(ex.Message);
            }

            return saveAsXml;
        }

        #endregion

    }
}
