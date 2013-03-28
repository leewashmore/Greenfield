using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Xml.Linq;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.ServiceLocation;
using GreenField.Common;
using GreenField.DataContracts;
using GreenField.Gadgets.Models;
using GreenField.Gadgets.Helpers;
using GreenField.Gadgets.Views;
using GreenField.ServiceCaller;
using GreenField.ServiceCaller.MeetingDefinitions;
using GreenField.UserSession;

namespace GreenField.Gadgets.ViewModels
{
    /// <summary>
    /// View model for ViewModelCustomScreeningTool class
    /// </summary>
    public class ViewModelCSTDataFieldSelector : NotificationObject
    {
        #region Fields
        /// <summary>
        /// MEF Singletons
        /// </summary>
        private IEventAggregator eventAggregator;
        private IDBInteractivity dbInteractivity;
        private ILoggerFacade logger;
        private IManageSessions manageSessions;
        private IRegionManager regionManager;
        /// <summary>
        /// Non-public instance fields
        /// </summary>
        private string userEnteredListName;
        private string userEnteredAccessibility;
        private string listNameInEditMode;
        private string accessibilityInEditMode;
        private int flagRefAdd = 0;
        private int flagFinAdd = 0;
        private int flagCurAdd = 0;
        private int flagFvaAdd = 0;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public ViewModelCSTDataFieldSelector(DashboardGadgetParam param)
        {
            logger = param.LoggerFacade;
            dbInteractivity = param.DBInteractivity;
            eventAggregator = param.EventAggregator;
            manageSessions = param.ManageSessions;
            regionManager = param.RegionManager;
        }

        #endregion

        #region Properties

        /// <summary>
        /// SelectedFieldsDataList
        /// </summary>
        private ObservableCollection<CSTUserPreferenceInfo> selectedFieldsDataList;
        public ObservableCollection<CSTUserPreferenceInfo> SelectedFieldsDataList
        {
            get
            {
                if (selectedFieldsDataList == null)
                    selectedFieldsDataList = new ObservableCollection<CSTUserPreferenceInfo>();
                return selectedFieldsDataList;
            }
            set
            {
                selectedFieldsDataList = value;
                RaisePropertyChanged(() => this.SelectedFieldsDataList);
            }
        }

        /// <summary>
        /// SelectedDataField
        /// </summary>
        private CSTUserPreferenceInfo selectedDataField;
        public CSTUserPreferenceInfo SelectedDataField
        {
            get { return selectedDataField; }
            set
            {
                selectedDataField = value;
                RaisePropertyChanged(() => this.SelectedDataField);
                RaisePropertyChanged(() => this.RemoveCommand);
            }
        }

        /// <summary>
        /// Flag - to check edit or view
        /// </summary>
        public string Flag { get; set; }

        /// <summary>
        /// Property to select protfolio
        /// </summary>
        private List<CustomSelectionData> securityReferenceData;
        public List<CustomSelectionData> SecurityReferenceData
        {
            get { return securityReferenceData; }
            set
            {
                securityReferenceData = value;
                RaisePropertyChanged(() => this.SecurityReferenceData);
            }
        }

        /// <summary>
        /// SelectedSecurityReferenceData
        /// </summary>
        private CustomSelectionData selectedSecurityReferenceData;
        public CustomSelectionData SelectedSecurityReferenceData
        {
            get { return selectedSecurityReferenceData; }
            set
            {
                selectedSecurityReferenceData = value;
                RaisePropertyChanged(() => this.SelectedSecurityReferenceData);
                RaisePropertyChanged(() => this.AddSecurityRefCommand);
                RaisePropertyChanged(() => this.RemoveCommand);
            }
        }

        /// <summary>
        /// PeriodFinancialsData
        /// </summary>
        private List<CustomSelectionData> periodFinancialsData;
        public List<CustomSelectionData> PeriodFinancialsData
        {
            get { return periodFinancialsData; }
            set
            {
                periodFinancialsData = value;
                RaisePropertyChanged(() => this.PeriodFinancialsData);
            }
        }

        /// <summary>
        /// SelectedPeriodFinancialsData
        /// </summary>
        private CustomSelectionData selectedPeriodFinancialsData;
        public CustomSelectionData SelectedPeriodFinancialsData
        {
            get { return selectedPeriodFinancialsData; }
            set
            {
                selectedPeriodFinancialsData = value;
                RaisePropertyChanged(() => this.SelectedPeriodFinancialsData);
                RaisePropertyChanged(() => this.AddPeriodFinCommand);
                RaisePropertyChanged(() => this.RemoveCommand);
            }
        }

        /// <summary>
        /// CurrentFinancialsData
        /// </summary>
        private List<CustomSelectionData> currentFinancialsData;
        public List<CustomSelectionData> CurrentFinancialsData
        {
            get { return currentFinancialsData; }
            set
            {
                currentFinancialsData = value;
                RaisePropertyChanged(() => this.CurrentFinancialsData);
            }
        }

        /// <summary>
        /// SelectedCurrentFinancialsData
        /// </summary>
        private CustomSelectionData selectedCurrentFinancialsData;
        public CustomSelectionData SelectedCurrentFinancialsData
        {
            get { return selectedCurrentFinancialsData; }
            set
            {
                selectedCurrentFinancialsData = value;
                RaisePropertyChanged(() => this.SelectedCurrentFinancialsData);
                RaisePropertyChanged(() => this.AddCurrentFinCommand);
                RaisePropertyChanged(() => this.RemoveCommand);
            }
        }

        /// <summary>
        /// FairValueData
        /// </summary>
        private List<CustomSelectionData> fairValueData;
        public List<CustomSelectionData> FairValueData
        {
            get { return fairValueData; }
            set
            {
                fairValueData = value;
                RaisePropertyChanged(() => this.FairValueData);
            }
        }

        /// <summary>
        /// SelectedFairValueData
        /// </summary>
        private CustomSelectionData selectedFairValueData;
        public CustomSelectionData SelectedFairValueData
        {
            get { return selectedFairValueData; }
            set
            {
                selectedFairValueData = value;
                RaisePropertyChanged(() => this.SelectedFairValueData);
                RaisePropertyChanged(() => this.AddFairValueCommand);
                RaisePropertyChanged(() => this.RemoveCommand);
            }
        }

        /// <summary>
        /// DataSourceInfo
        /// </summary>
        public List<String> DataSourceInfo
        {
            get { return new List<String> { "PRIMARY", "INDUSTRY", "REUTERS" }; }
        }

        /// <summary>
        /// SelectedDataSourceInfo
        /// </summary>
        private String selectedDataSourceInfo = "PRIMARY";
        public String SelectedDataSourceInfo
        {
            get { return selectedDataSourceInfo; }
            set
            {
                selectedDataSourceInfo = value;
                RaisePropertyChanged(() => this.SelectedDataSourceInfo);
                RaisePropertyChanged(() => this.AddPeriodFinCommand);
                RaisePropertyChanged(() => this.AddCurrentFinCommand);
            }
        }

        /// <summary>
        /// FairvalueDataSourceInfo
        /// </summary>
        private List<string> fairvalueDataSourceInfo;
        public List<string> FairvalueDataSourceInfo
        {
            get { return fairvalueDataSourceInfo; }
            set
            {
                if (fairvalueDataSourceInfo != value)
                {
                    fairvalueDataSourceInfo = value;
                    RaisePropertyChanged(() => FairvalueDataSourceInfo);
                }
            }
        }

        /// <summary>
        /// FairvalueSelectedDataSourceInfo
        /// </summary>
        private String fairvalueSelectedDataSourceInfo = "PRIMARY";
        public String FairvalueSelectedDataSourceInfo
        {
            get { return fairvalueSelectedDataSourceInfo; }
            set
            {
                fairvalueSelectedDataSourceInfo = value;
                RaisePropertyChanged(() => this.FairvalueSelectedDataSourceInfo);
                RaisePropertyChanged(() => this.AddFairValueCommand);
            }
        }

        /// <summary>
        /// YearTypeInfo
        /// </summary>
        public List<String> YearTypeInfo
        {
            get { return new List<String> { "CALENDAR", "FISCAL" }; }
        }

        /// <summary>
        /// SelectedYearTypeInfo
        /// </summary>
        private String selectedYearTypeInfo = "CALENDAR";
        public String SelectedYearTypeInfo
        {
            get { return selectedYearTypeInfo; }
            set
            {
                if (selectedYearTypeInfo != value)
                {
                    selectedYearTypeInfo = value;
                    RaisePropertyChanged(() => this.SelectedYearTypeInfo);
                    RaisePropertyChanged(() => this.AddPeriodFinCommand);
                }
            }
        }

        /// <summary>
        /// PeriodTypeInfo
        /// </summary>
        public List<String> PeriodTypeInfo
        {
            get { return new List<String> { "ANNUAL", "Q1", "Q2", "Q3", "Q4" }; }
        }

        /// <summary>
        /// SelectedPeriodTypeInfo
        /// </summary>
        private String selectedPeriodTypeInfo = "ANNUAL";
        public String SelectedPeriodTypeInfo
        {
            get { return selectedPeriodTypeInfo; }
            set
            {
                if (selectedPeriodTypeInfo != value)
                {
                    selectedPeriodTypeInfo = value;
                    RaisePropertyChanged(() => this.SelectedPeriodTypeInfo);
                    RaisePropertyChanged(() => this.AddPeriodFinCommand);
                }
            }
        }

        /// <summary>
        /// FromYearInfo
        /// </summary>
        public List<int> FromYearInfo
        {
            get
            {
                int currentYear = DateTime.Now.Year;
                return new List<int> { currentYear - 10, currentYear - 9, currentYear - 8, currentYear - 7, currentYear - 6, currentYear - 5,
                                       currentYear - 4, currentYear - 3, currentYear - 2, currentYear - 1, currentYear, currentYear + 1, currentYear + 2,
                                       currentYear + 3, currentYear + 4, currentYear + 5 };
            }
        }

        /// <summary>
        /// SelectedFromYearInfo
        /// </summary>
        private int selectedFromYearInfo = DateTime.Now.Year;
        public int SelectedFromYearInfo
        {
            get { return selectedFromYearInfo; }
            set
            {
                selectedFromYearInfo = value;
                RaisePropertyChanged(() => this.SelectedFromYearInfo);
            }
        }

        /// <summary>
        /// ToYearInfo
        /// </summary>
        public List<int> ToYearInfo
        {
            get
            {
                int currentYear = DateTime.Now.Year;
                return new List<int> { currentYear - 10, currentYear - 9, currentYear - 8, currentYear - 7, currentYear - 6, currentYear - 5,
                                       currentYear - 4, currentYear - 3, currentYear - 2, currentYear - 1, currentYear, currentYear + 1, currentYear + 2,
                                       currentYear + 3, currentYear + 4, currentYear + 5};
            }
        }

        /// <summary>
        /// SelectedToYearInfo
        /// </summary>
        private int selectedToYearInfo = DateTime.Now.Year;
        public int SelectedToYearInfo
        {
            get { return selectedToYearInfo; }
            set
            {
                selectedToYearInfo = value;
                RaisePropertyChanged(() => this.SelectedToYearInfo);
            }
        }

        /// <summary>
        /// IsAddButtonEnabled
        /// </summary>
        private bool isAddButtonEnabled = false;
        public bool IsAddButtonEnabled
        {
            get { return isAddButtonEnabled; }
            set
            {
                isAddButtonEnabled = value;
            }
        }

        /// <summary>
        /// IsRemoveButtonEnabled
        /// </summary>
        public bool isRemoveButtonEnabled = false;
        public bool IsRemoveButtonEnabled
        {
            get { return isRemoveButtonEnabled; }
            set
            {
                isRemoveButtonEnabled = value;
            }
        }

        /// <summary>
        /// IsActive is true when parent control is displayed on UI
        /// </summary>
        private bool isActive;
        public bool IsActive
        {
            get { return isActive; }
            set
            {
                isActive = value;
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
        private bool isBusyIndicatorBusy = false;
        public bool IsBusyIndicatorBusy
        {
            get { return isBusyIndicatorBusy; }
            set
            {
                isBusyIndicatorBusy = value;
                RaisePropertyChanged(() => this.IsBusyIndicatorBusy);
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

        #region ICommand Properties

        /// <summary>
        /// AddSecurityRefCommand
        /// </summary>
        public ICommand AddSecurityRefCommand
        {
            get { return new DelegateCommand<object>(AddSecurityRefCommandMethod, AddSecurityRefCommandValidationMethod); }
        }

        /// <summary>
        /// AddPeriodFinCommand
        /// </summary>
        public ICommand AddPeriodFinCommand
        {
            get { return new DelegateCommand<object>(AddPeriodFinCommandMethod, AddPeriodFinCommandValidationMethod); }
        }

        /// <summary>
        /// AddCurrentFinCommand
        /// </summary>
        public ICommand AddCurrentFinCommand
        {
            get { return new DelegateCommand<object>(AddCurrentFinCommandMethod, AddCurrentFinCommandValidationMethod); }
        }

        /// <summary>
        /// AddFairValueCommand
        /// </summary>
        public ICommand AddFairValueCommand
        {
            get { return new DelegateCommand<object>(AddFairValueCommandMethod, AddFairValueCommandValidationMethod); }
        }

        /// <summary>
        /// SubmitCommand
        /// </summary>
        public ICommand SubmitCommand
        {
            get { return new DelegateCommand<object>(SubmitCommandMethod, SubmitCommandValidationMethod); }
        }

        /// <summary>
        /// RemoveCommand
        /// </summary>
        public ICommand RemoveCommand
        {
            get { return new DelegateCommand<object>(RemoveCommandMethod, RemoveCommandValidationMethod); }
        }
        #endregion

        #endregion

        #region ICommand Methods

        /// <summary>
        /// AddSecurityRefCommandValidationMethod
        /// </summary>
        /// <param name="param">object</param>
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
                if (SelectedFieldsDataList != null && SelectedFieldsDataList.Count > 0)
                {
                    if (SelectedFieldsDataList.Select(a => a.ScreeningId).Contains(SelectedSecurityReferenceData.ScreeningId))
                    {
                        IsAddButtonEnabled = false;
                        return false;
                    }
                    else
                    {
                        IsAddButtonEnabled = true;
                        flagRefAdd = 1;
                        return true;
                    }
                }
                else 
                {
                    IsAddButtonEnabled = true;
                    flagRefAdd = 1;
                    return true;
                }
            }
            else
            {
                IsAddButtonEnabled = false;
                return false;
            }           
        }

        /// <summary>
        /// AddSecurityRefCommandMethod
        /// </summary>
        /// <param name="param">object</param>
        private void AddSecurityRefCommandMethod(object param)
        {
            if (flagRefAdd == 1)
            {
                ObservableCollection<CSTUserPreferenceInfo> temp = new ObservableCollection<CSTUserPreferenceInfo>();
                string listName;
                string accessibility;
                int tempOrder;
                if (Flag == "Edit")
                {
                    if (SelectedFieldsDataList == null || SelectedFieldsDataList.Count == 0)
                    {
                        accessibilityInEditMode = CSTNavigation.FetchString(CSTNavigationInfo.Accessibility) as string;
                        listNameInEditMode = CSTNavigation.FetchString(CSTNavigationInfo.ListName) as string;
                        tempOrder = -1;
                        listName = listNameInEditMode;
                        accessibility = accessibilityInEditMode;
                    }
                    else
                    {
                        temp = this.SelectedFieldsDataList;
                        tempOrder = temp.Count;
                        listName = SelectedFieldsDataList[0].ListName;
                        accessibility = SelectedFieldsDataList[0].Accessibility;
                    }
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
                    TableColumn = SelectedSecurityReferenceData.DataColumn,
                    ShortColumnDesc = SelectedSecurityReferenceData.ShortDescription
                });
                SelectedFieldsDataList = temp;
                flagRefAdd = 0;
            }
        }

        /// <summary>
        /// AddPeriodFinCommandValidationMethod
        /// </summary>
        /// <param name="param">object</param>
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
                if (SelectedFieldsDataList != null && SelectedFieldsDataList.Count > 0)
                {
                    if (SelectedFieldsDataList.Select(a => a.ScreeningId).Contains(SelectedPeriodFinancialsData.ScreeningId))
                    {
                        CSTUserPreferenceInfo temp = new CSTUserPreferenceInfo();
                        temp = SelectedFieldsDataList.Where(a => a.ScreeningId == SelectedPeriodFinancialsData.ScreeningId
                                                                 && a.DataSource == SelectedDataSourceInfo
                                                                 && a.PeriodType == SelectedPeriodTypeInfo
                                                                 && a.YearType == SelectedYearTypeInfo).FirstOrDefault();
                        if (temp != null)
                        {
                            IsAddButtonEnabled = false;
                            return false;
                        }
                        else
                        {
                            IsAddButtonEnabled = true;
                            flagFinAdd = 1;
                            return true;
                        }
                    }
                    else
                    {
                        IsAddButtonEnabled = true;
                        flagFinAdd = 1;
                        return true;
                    }
                }
                else
                {
                    IsAddButtonEnabled = true;
                    flagFinAdd = 1;
                    return true;
                }
            }
            else
            {
                IsAddButtonEnabled = false;
                return false;
            }
        }

        /// <summary>
        /// AddPeriodFinCommandMethod
        /// </summary>
        /// <param name="param">object</param>
        private void AddPeriodFinCommandMethod(object param)
        {
            if (flagFinAdd == 1)
            {
                ObservableCollection<CSTUserPreferenceInfo> temp = new ObservableCollection<CSTUserPreferenceInfo>();
                int tempOrder;
                string listName;
                string accessibility;
                if (Flag == "Edit")
                {
                    if (SelectedFieldsDataList == null || SelectedFieldsDataList.Count == 0)
                    {
                        accessibilityInEditMode = CSTNavigation.FetchString(CSTNavigationInfo.Accessibility) as string;
                        listNameInEditMode = CSTNavigation.FetchString(CSTNavigationInfo.ListName) as string;
                        tempOrder = -1;
                        listName = listNameInEditMode;
                        accessibility = accessibilityInEditMode;
                    }
                    else
                    {
                        temp = this.SelectedFieldsDataList;
                        tempOrder = temp.Count();
                        listName = SelectedFieldsDataList[0].ListName;
                        accessibility = SelectedFieldsDataList[0].Accessibility;
                    }
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
                        TableColumn = SelectedPeriodFinancialsData.DataDescription,
                        DataID = SelectedPeriodFinancialsData.DataID
                    });
                }
                SelectedFieldsDataList = temp;
                flagFinAdd = 0;
            }
        }

        /// <summary>
        /// AddCurrentFinCommandValidationMethod
        /// </summary>
        /// <param name="param">object</param>
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
                if (SelectedFieldsDataList != null && SelectedFieldsDataList.Count > 0)
                {
                    if (SelectedFieldsDataList.Select(a => a.ScreeningId).Contains(SelectedCurrentFinancialsData.ScreeningId))
                    {
                        List<CSTUserPreferenceInfo> existingRow = new List<CSTUserPreferenceInfo>();
                        existingRow = SelectedFieldsDataList.Where(a => a.ScreeningId == SelectedCurrentFinancialsData.ScreeningId).ToList();
                        if (existingRow.Select(a => a.DataSource).Contains(SelectedDataSourceInfo))
                        {
                            IsAddButtonEnabled = false;
                            return false;
                        }
                        else
                        {
                            IsAddButtonEnabled = true;
                            flagCurAdd = 1;
                            return true;
                        }
                    }
                    else
                    {
                        IsAddButtonEnabled = true;
                        flagCurAdd = 1;
                        return true;
                    }
                }
                else
                {
                    IsAddButtonEnabled = true;
                    flagCurAdd = 1;
                    return true;
                }
            }
            else
            {
                IsAddButtonEnabled = false;
                return false;
            }
        }

        /// <summary>
        /// AddCurrentFinCommandMethod
        /// </summary>
        /// <param name="param">object</param>
        private void AddCurrentFinCommandMethod(object param)
        {
            if (flagCurAdd == 1)
            {
                ObservableCollection<CSTUserPreferenceInfo> temp = new ObservableCollection<CSTUserPreferenceInfo>();
                int tempOrder;
                string listName;
                string accessibility;
                if (Flag == "Edit")
                {
                    if (SelectedFieldsDataList == null || SelectedFieldsDataList.Count == 0)
                    {
                        accessibilityInEditMode = CSTNavigation.FetchString(CSTNavigationInfo.Accessibility) as string;
                        listNameInEditMode = CSTNavigation.FetchString(CSTNavigationInfo.ListName) as string;
                        tempOrder = -1;
                        listName = listNameInEditMode;
                        accessibility = accessibilityInEditMode;
                    }
                    else
                    {
                        listName = SelectedFieldsDataList[0].ListName;
                        accessibility = SelectedFieldsDataList[0].Accessibility;
                        temp = this.SelectedFieldsDataList;
                        tempOrder = temp.Count();
                    }
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
                    TableColumn = SelectedCurrentFinancialsData.DataDescription,
                    DataID = SelectedCurrentFinancialsData.DataID
                });
                SelectedFieldsDataList = temp;
                flagCurAdd = 0;
            }
        }

        /// <summary>
        /// AddFairValueCommandValidationMethod
        /// </summary>
        /// <param name="param">object</param>
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
                if (SelectedFieldsDataList != null && SelectedFieldsDataList.Count > 0)
                {
                    if (SelectedFieldsDataList.Select(a => a.ScreeningId).Contains(SelectedFairValueData.ScreeningId))
                    {
                        List<CSTUserPreferenceInfo> existingRow = new List<CSTUserPreferenceInfo>();
                        existingRow = SelectedFieldsDataList.Where(a => a.ScreeningId == SelectedFairValueData.ScreeningId).ToList();

                        if (existingRow.Select(a => a.DataSource).Contains(FairvalueSelectedDataSourceInfo))
                        {
                            IsAddButtonEnabled = false;
                            return false;
                        }
                        else
                        {
                            IsAddButtonEnabled = true;
                            flagFvaAdd = 1;
                            return true;
                        }
                    }
                    else
                    {
                        IsAddButtonEnabled = true;
                        flagFvaAdd = 1;
                        return true;
                    }
                }
                else
                {
                    IsAddButtonEnabled = true;
                    flagFvaAdd = 1;
                    return true;
                }
            }
            else
            {
                IsAddButtonEnabled = false;
                return false;
            }
        }

        /// <summary>
        /// AddFairValueCommandMethod
        /// </summary>
        /// <param name="param">object</param>
        private void AddFairValueCommandMethod(object param)
        {
            if (flagFvaAdd == 1)
            {
                ObservableCollection<CSTUserPreferenceInfo> temp = new ObservableCollection<CSTUserPreferenceInfo>();
                int tempOrder;
                string listName;
                string accessibility;
                if (Flag == "Edit")
                {
                    if (SelectedFieldsDataList == null || SelectedFieldsDataList.Count == 0)
                    {
                        accessibilityInEditMode = CSTNavigation.FetchString(CSTNavigationInfo.Accessibility) as string;
                        listNameInEditMode = CSTNavigation.FetchString(CSTNavigationInfo.ListName) as string;
                        tempOrder = -1;
                        listName = listNameInEditMode;
                        accessibility = accessibilityInEditMode;
                    }
                    else
                    {
                        temp = this.SelectedFieldsDataList;
                        tempOrder = temp.Count();
                        listName = SelectedFieldsDataList[0].ListName;
                        accessibility = SelectedFieldsDataList[0].Accessibility;
                    }
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
                flagFvaAdd = 0;
            }
        }

        /// <summary>
        /// RemoveCommandValidationMethod
        /// </summary>
        /// <param name="param">object</param>
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
                    return false; 
                }
            }
        }

        /// <summary>
        /// RemoveCommandMethod
        /// </summary>
        /// <param name="param">object</param>
        private void RemoveCommandMethod(object param)
        {
            SelectedFieldsDataList.Remove(SelectedDataField);
            RaisePropertyChanged(() => SelectedFieldsDataList);
            RaisePropertyChanged(() => SelectedSecurityReferenceData);
            RaisePropertyChanged(() => SelectedPeriodFinancialsData);
            RaisePropertyChanged(() => SelectedCurrentFinancialsData);
            RaisePropertyChanged(() => SelectedFairValueData);
        }

        /// <summary>
        /// SubmitCommandValidationMethod
        /// </summary>
        /// <param name="param">object</param>
        private bool SubmitCommandValidationMethod(object param)
        {
            if (UserSession.SessionManager.SESSION == null)
                return false;
            else
                return true;
        }

        /// <summary>
        /// SubmitCommandMethod
        /// </summary>
        /// <param name="param">object</param>
        private void SubmitCommandMethod(object param)
        {            
            string flag = CSTNavigation.FetchString(CSTNavigationInfo.Flag) as string;

            if (flag == "View")
            {
                regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardCustomScreeningTool", UriKind.Relative));
            }
            else if (flag == "Create" && SelectedFieldsDataList.Count == 0)
            {
               MessageBox.Show("List contains no data points preference. Empty list cannot be created");
            }
            else
            {
               
                if (SelectedFieldsDataList.Count > 0)
                {
                    CSTNavigation.UpdateString(CSTNavigationInfo.ListName, SelectedFieldsDataList[0].ListName);
                    CSTNavigation.UpdateString(CSTNavigationInfo.Accessibility, SelectedFieldsDataList[0].Accessibility);
                }

                // show child window that will take user input for list name and its accessibility and prompt to save
                
                //also need to send the user data selection list to the child view so that it can be updated when save clicked in child window
                ChildViewCSTDataListSave childViewCSTDataListSave = new ChildViewCSTDataListSave();
                childViewCSTDataListSave.Show();

                childViewCSTDataListSave.Unloaded += (se, e) =>
                {
                    // if user chooses to save new or edited list, update changes to the database.
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
                                        if (dbInteractivity != null)
                                        {
                                            string xmlData = SaveAsXmlBuilder(SessionManager.SESSION.UserName, SelectedFieldsDataList.ToList(), 
                                                                    userEnteredListName, userEnteredAccessibility);

                                            if (xmlData != null)
                                            {
                                                dbInteractivity.SaveUserDataPointsPreference(xmlData, SessionManager.SESSION.UserName, 
                                                                    SaveUserDataPointsPreferenceCallBackMethod);
                                            }
                                        }
                                    }
                                    else if (Flag == "Edit")
                                    {
                                        if (dbInteractivity != null && SelectedFieldsDataList[0].ListName != null && SelectedFieldsDataList[0].Accessibility != null)
                                        {
                                            string xmlData = SaveAsXmlBuilder(SessionManager.SESSION.UserName, SelectedFieldsDataList.ToList(), 
                                                                        SelectedFieldsDataList[0].ListName, SelectedFieldsDataList[0].Accessibility);

                                            if (xmlData != null)
                                            {
                                                dbInteractivity.UpdateUserDataPointsPreference(xmlData, SessionManager.SESSION.UserName,
                                                    SelectedFieldsDataList[0].ListName, userEnteredListName, userEnteredAccessibility, 
                                                    UpdateUserDataPointsPreferenceCallBackMethod);
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

        /// <summary>
        /// Callback method for Security ReferenceTab DataPoints data service call - assigns value to UI Field Properties
        /// </summary>
        /// <param name="result">CustomSelectionData List</param>
        private void SecurityReferenceTabDataPointsCallbackMethod(List<CustomSelectionData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);

            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result.ToString(), 1);
                    SecurityReferenceData = result;
                    CheckIfColumnInListExist(result.Select(a => a.ScreeningId).ToList(),"REF");
                }
                else
                {
                    Prompt.ShowDialog("Message: Argument Null\nStackTrace: " + methodNamespace + ":result", "ArgumentNullDebug", MessageBoxButton.OK);
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
        /// Callback method for Period FinancialsTab DataPoints data service call - assigns value to UI Field Properties
        /// </summary>
        /// <param name="result">CustomSelectionData List</param>
        private void PeriodFinancialsTabDataPointsCallbackMethod(List<CustomSelectionData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);

            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result.ToString(), 1);
                    PeriodFinancialsData = result;
                    CheckIfColumnInListExist(result.Select(a => a.ScreeningId).ToList(),"FIN");
                }
                else
                {
                    Prompt.ShowDialog("Message: Argument Null\nStackTrace: " + methodNamespace + ":result", "ArgumentNullDebug", MessageBoxButton.OK);
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
        /// Callback method for Current FinancialsTab DataPoints data service call - assigns value to UI Field Properties
        /// </summary>
        /// <param name="result">CustomSelectionData List</param>
        private void CurrentFinancialsTabDataPointsCallbackMethod(List<CustomSelectionData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);

            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result.ToString(), 1);
                    CurrentFinancialsData = result;
                    CheckIfColumnInListExist(result.Select(a => a.ScreeningId).ToList(),"CUR");
                }
                else
                {
                    Prompt.ShowDialog("Message: Argument Null\nStackTrace: " + methodNamespace + ":result", "ArgumentNullDebug", MessageBoxButton.OK);
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
        /// Callback method for FairValueTab DataPoints data service call - assigns value to UI Field Properties
        /// </summary>
        /// <param name="result">CustomSelectionData List</param>
        private void FairValueTabDataPointsCallbackMethod(List<CustomSelectionData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);

            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result.ToString(), 1);
                    FairValueData = result;
                    CheckIfColumnInListExist(result.Select(a => a.ScreeningId).ToList(),"FVA");
                }
                else
                {
                    Prompt.ShowDialog("Message: Argument Null\nStackTrace: " + methodNamespace + ":result", "ArgumentNullDebug", MessageBoxButton.OK);
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
        /// Callback method for Retrieving FairValueTab Source data service call - assigns value to UI Field Properties
        /// </summary>
        /// <param name="result">PortfolioSelectionData List</param>
        private void RetrieveFairValueTabSourceCallbackMethod(List<string> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);

            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result.ToString(), 1);
                    FairvalueDataSourceInfo = result;
                }
                else
                {
                    Prompt.ShowDialog("Message: Argument Null\nStackTrace: " + methodNamespace + ":result", "ArgumentNullDebug", MessageBoxButton.OK);
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
        /// Callback method for saving User DataPoints preference service call
        /// </summary>
        /// <param name="result">Boolean? List</param>
        private void SaveUserDataPointsPreferenceCallBackMethod(Boolean? result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);

            try
            {
                if (result == true)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result.ToString(), 1);
                    // update fields for navigation data
                    CSTNavigation.UpdateString(CSTNavigationInfo.Flag, "Created");
                    CSTNavigation.Update(CSTNavigationInfo.SelectedDataList, SelectedFieldsDataList.ToList());
                    // navigate to the main view to display results grid for the selected data list.
                    regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardCustomScreeningTool", UriKind.Relative));
                }
                else if (result == false)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result.ToString(), 1);
                }
                else
                {
                    Prompt.ShowDialog("Message: Argument Null\nStackTrace: " + methodNamespace + ":result", "ArgumentNullDebug", MessageBoxButton.OK);
                    Logging.LogMethodParameterNull(logger, methodNamespace, 1);
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
        }

        /// <summary>
        /// Callback method for updating User DataPoints preference service call
        /// </summary>
        /// <param name="result">Boolean? List</param>
        private void UpdateUserDataPointsPreferenceCallBackMethod(Boolean? result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);

            try
            {
                if (result == true)
                {
                    foreach (CSTUserPreferenceInfo item in SelectedFieldsDataList)
                    {
                        item.ListName = userEnteredListName;
                        item.Accessibility = userEnteredAccessibility;
                    }
                    Logging.LogMethodParameter(logger, methodNamespace, result.ToString(), 1);
                    // update fields for navigation data
                    CSTNavigation.UpdateString(CSTNavigationInfo.Flag, "Edited");
                    CSTNavigation.Update(CSTNavigationInfo.SelectedDataList, SelectedFieldsDataList.ToList());
                    // navigate to the main view to display results grid for the selected data list.
                    regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardCustomScreeningTool", UriKind.Relative));
                }
                else if (result == false)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result.ToString(), 1);
                }
                else
                {
                    Prompt.ShowDialog("Message: Argument Null\nStackTrace: " + methodNamespace + ":result", "ArgumentNullDebug", MessageBoxButton.OK);
                    Logging.LogMethodParameterNull(logger, methodNamespace, 1);
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
        }
        #endregion        

        #region Helpers
        /// <summary>
        ///  Method to call web service methods to fetche the tabs data
        /// </summary>
        public void FetchTabsData()
        {
            if (dbInteractivity != null)
            {
                BusyIndicatorNotification(true, "Retrieving Security Reference Data...");
                dbInteractivity.RetrieveSecurityReferenceTabDataPoints(SecurityReferenceTabDataPointsCallbackMethod);

                BusyIndicatorNotification(true, "Retrieving Period Financials Data...");
                dbInteractivity.RetrievePeriodFinancialsTabDataPoints(PeriodFinancialsTabDataPointsCallbackMethod);

                BusyIndicatorNotification(true, "Retrieving Current Financials Data...");
                dbInteractivity.RetrieveCurrentFinancialsTabDataPoints(CurrentFinancialsTabDataPointsCallbackMethod);

                BusyIndicatorNotification(true, "Retrieving Fair Value Data...");
                dbInteractivity.RetrieveFairValueTabDataPoints(FairValueTabDataPointsCallbackMethod);

                BusyIndicatorNotification(true, "Retrieving Fair Value DataSource...");
                dbInteractivity.RetrieveFairValueTabSource(RetrieveFairValueTabSourceCallbackMethod);
            }
        }

        /// <summary>
        /// Method that will be called when the view is active
        /// </summary>
        public void Initialize()
        {
            // fetch tabs data
            FetchTabsData();
            List<CSTUserPreferenceInfo> temp = new List<CSTUserPreferenceInfo>();
            ObservableCollection<CSTUserPreferenceInfo> userPref = new ObservableCollection<CSTUserPreferenceInfo>();

            Flag = CSTNavigation.FetchString(CSTNavigationInfo.Flag) as string;
            if (Flag != null)
            {
                if (Flag == "Edit" || Flag == "View")
                {
                    temp = CSTNavigation.Fetch(CSTNavigationInfo.SelectedDataList) as List<CSTUserPreferenceInfo>;
                    foreach (CSTUserPreferenceInfo item in temp)
                    {
                        userPref.Add(item);
                    }
                    SelectedFieldsDataList = userPref;
                }
                if (Flag == "Create")
                    SelectedFieldsDataList = new ObservableCollection<CSTUserPreferenceInfo>();
            }          
        }

        /// <summary>
        /// BusyIndicatorNotification
        /// </summary>
        public void BusyIndicatorNotification(bool showBusyIndicator = false, String message = null)
        {
            if (message != null)
            {
                BusyIndicatorContent = message;
            }
            IsBusyIndicatorBusy = showBusyIndicator;
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
        /// <summary>
        /// Method to check if existing user preference contains non existing data points
        /// </summary>
        /// <param name="availableScreeningIds"></param>
        /// <param name="screeningIdentifier"></param>
        public void CheckIfColumnInListExist(List<string> availableScreeningIds,string screeningIdentifier)
        {
            List<string> screeningId = new List<string>();

            if(SelectedFieldsDataList.Count > 0)
            {
                screeningId = SelectedFieldsDataList.Select(a => a.ScreeningId).ToList();
            }
            if (availableScreeningIds.Count > 0)
            {
                foreach (string item in screeningId)
                {
                    if (item.StartsWith(screeningIdentifier))
                    {
                        if (!(availableScreeningIds.Contains(item)))
                        {
                            CSTUserPreferenceInfo temp = new CSTUserPreferenceInfo();
                            temp = SelectedFieldsDataList.Where(a => a.ScreeningId == item).FirstOrDefault();
                            SelectedFieldsDataList.Remove(temp);
                            RaisePropertyChanged(() => SelectedFieldsDataList);
                        }
                    }
                }
            }         
        }     
        #endregion
        
        #region EventUnSubscribe
        /// <summary>
        /// Method that disposes the events
        /// </summary>
        public void Dispose()
        {
            // update null for navigation fields
            CSTNavigation.UpdateString(CSTNavigationInfo.Accessibility, null);
            CSTNavigation.UpdateString(CSTNavigationInfo.Flag, null);
            CSTNavigation.UpdateString(CSTNavigationInfo.ListName, null);
            CSTNavigation.Update(CSTNavigationInfo.SelectedDataList, null);
        }
        #endregion
    }
}
