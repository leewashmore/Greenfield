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
    public class ViewModelCSTDataFieldSelector :  NotificationObject
    {
        #region Fields
        /// <summary>
        /// MEF Singletons
        /// </summary>
        private IEventAggregator _eventAggregator;
        private IDBInteractivity _dbInteractivity;
        private ILoggerFacade _logger;
        private IManageSessions _manageSessions;
       
        #endregion

        #region Constructor
        public ViewModelCSTDataFieldSelector(DashboardGadgetParam param)
        {
            _logger = param.LoggerFacade;
            _dbInteractivity = param.DBInteractivity;
            _eventAggregator = param.EventAggregator;
            _manageSessions = param.ManageSessions;

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

        public List<CSTUserPreferenceInfo> _selectedFieldsDataList;
        public List<CSTUserPreferenceInfo> SelectedFieldsDataList
        {
            get { return _selectedFieldsDataList; }
            set
            {
                if (value != null)
                {
                    _selectedFieldsDataList = value;
                    RaisePropertyChanged(() => this.SelectedFieldsDataList);
                }
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

        public  CustomSelectionData _selectedSecurityReferenceData;
        public CustomSelectionData SelectedSecurityReferenceData
        {
            get { return _selectedSecurityReferenceData; }
            set 
            { 
                _selectedSecurityReferenceData = value;
                RaisePropertyChanged(() => this.SelectedSecurityReferenceData);
                RaisePropertyChanged(() => this.AddCommand);
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
            }
        }

         public List<String> DataSourceInfo
         {
             get{ return new List<String>{"PRIMARY","INDUSTRY","REUTERS"};}

         }

        public String _selectedDataSourceInfo;
        public String SelectedDataSourceInfo
        {
            get { return _selectedDataSourceInfo; }
            set
            {
                _selectedDataSourceInfo = value;
                RaisePropertyChanged(() => this.SelectedDataSourceInfo);
            }
        }

        public List<String> YearTypeInfo
        {
            get { return new List<String> { "CALENDAR", "FISCAL" }; }

        }

        public String _selectedYearTypeInfo;
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

        public String _selectedPeriodTypeInfo;
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

        public ICommand AddCommand
        {
            get { return new DelegateCommand<object>(AddCommandMethod, AddCommandValidationMethod); }
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
     
        private bool AddCommandValidationMethod(object param)
        {
            return true;
        }

        private void AddCommandMethod(object param)
        {
           //add selected data point to selected data list
            //SelectedFieldsList = new List<CustomSelectionData>();
            //SelectedFieldsList.Add(new CustomSelectionData()
            //{
            //    ScreeningId = SelectedSecurityReferenceData.ScreeningId,
            //    DataDescription = SelectedSecurityReferenceData.DataDescription,
            //    LongDescription = SelectedSecurityReferenceData.LongDescription,
            //    DataColumn = SelectedSecurityReferenceData.DataColumn
            //});

            //SelectedFieldsOverviewInfo = SelectedFieldsList;

            SelectedFieldsDataList.Add(new CSTUserPreferenceInfo()
            {
                ScreeningId = SelectedSecurityReferenceData.ScreeningId,
                DataDescription = SelectedSecurityReferenceData.DataDescription
            });
        }

        private bool RemoveCommandValidationMethod(object param)
        {
            return true;
        }

        private void RemoveCommandMethod(object param)
        {
           
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
            CSTNavigation.UpdateString(CSTNavigationInfo.ListName, SelectedFieldsDataList[0].ListName);
            CSTNavigation.UpdateString(CSTNavigationInfo.Accessibility, SelectedFieldsDataList[0].Accessibility);

            //prompt to ask if user wants to save list
            //if yes open child window

            //also need to send the user data selection list to the child view so that it can be updated when save clicked in child window

            ChildViewCSTDataListSave childViewCSTDataListSave = new ChildViewCSTDataListSave();
            childViewCSTDataListSave.Show();

            childViewCSTDataListSave.Unloaded += (se, e) =>
            {
                if (childViewCSTDataListSave.DialogResult == true)
                {
                    Prompt.ShowDialog("Confirm to save the list","Save", MessageBoxButton.OKCancel, (result) =>
                        {
                            if (result == MessageBoxResult.OK)
                            {
                                string userEnteredListName = childViewCSTDataListSave.txtDataListName.Text;
                                string userEnteredAccessibility = childViewCSTDataListSave.SelectedAccessibility;
                                if (_dbInteractivity != null)
                                {
                                    string xmlData = SaveAsXmlBuilder(SessionManager.SESSION.UserName, MyProperty);
                                    if (xmlData != null)
                                    {
                                        _dbInteractivity.SaveUserDataPointsPreference(xmlData, SessionManager.SESSION.UserName, SaveUserDataPointsPreferenceCallBackMethod);
                                    }
                                }
                            }
                        });
                }
            };
        }

        #endregion

        /// <summary>
        /// Construct XML for Save As Event
        /// </summary>
        /// <returns></returns>
        private string SaveAsXmlBuilder(String userName, List<CSTUserPreferenceInfo> userPreference)
        {
            string saveAsXml = String.Empty;

            try
            {
                if (userName != null && userPreference != null)
                {
                    XDocument doc = new XDocument(
                       new XDeclaration("1.0", "utf-8", "yes"),
                       new XComment("Custom screening Tool save as preference details"));

                    foreach (CSTUserPreferenceInfo preference in userPreference)
                    {
                        doc.Add(new XElement("UserName", userName),
                            new XElement("ListName", preference.ListName),
                            new XElement("Accessibilty", preference.Accessibility),
                            new XElement("ScreeningId", preference.ScreeningId),
                            new XElement("DataSource", preference.DataSource),
                            new XElement("PeriodType", preference.PeriodType),
                            new XElement("YearType", preference.YearType),
                            new XElement("FromDate", preference.FromDate),
                            new XElement("ToDate", preference.ToDate),
                            new XElement("DataPointsOrder", preference.DataPointsOrder),
                            new XElement("CreatedBy", userName),
                            new XElement("CreatedOn", DateTime.Now),
                            new XElement("ModifiedBy", userName),
                            new XElement("ModifiedOn", DateTime.Now));
                    }

                    saveAsXml = doc.ToString();
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog(ex.Message);
            }

            return saveAsXml;
        }

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

        #endregion

        #region EventUnSubscribe
        /// <summary>
        /// Method that disposes the events
        /// </summary>
        public void Dispose()
        {

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
            }
        }

        public void Initialize()
        {
            //fetch tabs data
            FetchTabsData();

            Flag = CSTNavigation.FetchString(CSTNavigationInfo.Flag) as string;
            if (Flag != null)
            {
                if (Flag == "Edit")
                {
                    SelectedFieldsDataList = CSTNavigation.Fetch(CSTNavigationInfo.SelectedDataList) as List<CSTUserPreferenceInfo>;
                }
            }
        }

        public void BusyIndicatorNotification(bool showBusyIndicator = false, String message = null)
        {
            if (message != null)
                BusyIndicatorContent = message;

            BusyIndicatorIsBusy = showBusyIndicator;
        }

        //public void CreateCSTSelectorDataGrid(List<CSTUserPreferenceInfo> preference)
        //{
        //    List<CSTUserPreferenceInfo> temp = new List<CSTUserPreferenceInfo>();
        //    foreach (var item in preference)
        //    {
        //        switch (item.ScreeningId.Substring(0,2))
        //        {
        //            case "REF":
        //                temp.Add(new CSTUserPreferenceInfo() 
        //                {
        //                    UserName = item.UserName,
        //                    ScreeningId = item.ScreeningId,
        //                    DataDescription = SecurityReferenceData.Where(a=>a.ScreeningId == item.ScreeningId).Select(a=>a.DataDescription).FirstOrDefault(),
        //                    //LongDescription = item.LONG_DESC,
        //                    //DataColumn = item.TABLE_COLUMN
        //                    ListName = item.ListName,
        //                    Accessibility = item.Accessibility,
        //                    DataSource = item.DataSource,
        //                    PeriodType = item.PeriodType,
        //                    YearType = item.YearType,
        //                    FromDate = Convert.ToInt32(item.FromDate),
        //                    ToDate = Convert.ToInt32(item.ToDate),
        //                    DataPointsOrder = Convert.ToInt32(item.DataPointsOrder)
        //                });
        //                break;
        //            case "FIN":

        //                break;
        //            case "CUR":

        //                break;
        //            case "FVA":

        //                break;
        //            default:
        //                break;
        //        } 
        //    }

        //    SelectedFieldsDataList = temp;           
        //}

        #endregion
 private List<CSTUserPreferenceInfo> myVar;

        public List<CSTUserPreferenceInfo> MyProperty
        {
            get { return myVar; }
            set { myVar = value; RaisePropertyChanged(() => MyProperty); }
        }
    }
}
