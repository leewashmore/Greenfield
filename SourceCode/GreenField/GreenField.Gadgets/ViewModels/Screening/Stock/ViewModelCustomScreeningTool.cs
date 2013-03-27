using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using GreenField.Common;
using GreenField.DataContracts;
using GreenField.Gadgets.Helpers;
using GreenField.Gadgets.Models;
using GreenField.Gadgets.Views;
using GreenField.ServiceCaller;
using GreenField.ServiceCaller.MeetingDefinitions;
using System.Diagnostics;

namespace GreenField.Gadgets.ViewModels
{
    /// <summary>
    /// View model for ViewModelCustomScreeningTool class
    /// </summary>
    public class ViewModelCustomScreeningTool : NotificationObject
    {
        #region Fields
        /// <summary>
        /// MEF Singletons
        /// </summary>
        private IEventAggregator eventAggregator;
        private IDBInteractivity dbInteractivity;
        private ILoggerFacade logger;
        private List<PortfolioSelectionData> portfolioSelectionData;
        private IRegionManager regionManager;
        private List<EntitySelectionData> benchmarkSelectionData;
        /// <summary>
        /// Counter to check for busy indicator 
        /// </summary>
        private int flagBsyInd;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="param"></param>
        public ViewModelCustomScreeningTool(DashboardGadgetParam param)
        {
            logger = param.LoggerFacade;
            dbInteractivity = param.DBInteractivity;
            eventAggregator = param.EventAggregator;
            regionManager = param.RegionManager;
        }
        #endregion

        #region Properties

        #region Security Selction Grid

        #region Security Selection Criteria
        /// <summary>
        /// Property to select security criteria
        /// </summary>
        private List<String> securitySelectionCriteria;
        public List<String> SecuritySelectionCriteria
        {
            get
            {
                { return securitySelectionCriteria = new List<String> { "Portfolio", "Benchmark", "Custom" }; }
            }
            set
            {
                securitySelectionCriteria = value;
                RaisePropertyChanged(() => this.SecuritySelectionCriteria);
            }
        }

        /// <summary>
        /// SelectedCriteria
        /// </summary>
        private String selectedCriteria;
        public String SelectedCriteria
        {

            get { return selectedCriteria; }
            set
            {
                selectedCriteria = value;
                RaisePropertyChanged(() => this.SelectedCriteria);
                if (SelectedCriteria == SecuritySelectionType.PORTFOLIO)
                {
                    PortfolioSelectionVisibility = Visibility.Visible;
                    BenchmarkSelectionVisibility = Visibility.Collapsed;
                    CustomSelectionVisibility = Visibility.Collapsed;
                    RaisePropertyChanged(() => this.SubmitCommand);
                    RaisePropertyChanged(() => this.PortfolioSelectionVisibility);
                    RaisePropertyChanged(() => this.BenchmarkSelectionVisibility);
                    RaisePropertyChanged(() => this.CustomSelectionVisibility);
                                        
                }
                else if (SelectedCriteria == SecuritySelectionType.BENCHMARK)
                {
                    BenchmarkSelectionVisibility = Visibility.Visible;
                    PortfolioSelectionVisibility = Visibility.Collapsed;
                    CustomSelectionVisibility = Visibility.Collapsed;
                    RaisePropertyChanged(() => this.SubmitCommand);
                    RaisePropertyChanged(() => this.BenchmarkSelectionVisibility);
                    RaisePropertyChanged(() => this.PortfolioSelectionVisibility);
                    RaisePropertyChanged(() => this.CustomSelectionVisibility);
                }
                else if (SelectedCriteria == SecuritySelectionType.CUSTOM)
                {
                    CustomSelectionVisibility = Visibility.Visible;
                    PortfolioSelectionVisibility = Visibility.Collapsed;
                    BenchmarkSelectionVisibility = Visibility.Collapsed;
                    RaisePropertyChanged(() => this.SubmitCommand);
                    RaisePropertyChanged(() => this.CustomSelectionVisibility);
                    RaisePropertyChanged(() => this.PortfolioSelectionVisibility);
                    RaisePropertyChanged(() => this.BenchmarkSelectionVisibility);
                }
            }
        }
        #endregion

        #region Portfolio Selection
        /// <summary>
        /// Property to select protfolio
        /// </summary>
        private List<String> portfolioSelectionInfo;
        public List<String> PortfolioSelectionInfo
        {
            get
            {
                return portfolioSelectionInfo;
            }
            set
            {
                portfolioSelectionInfo = value;
                RaisePropertyChanged(() => this.PortfolioSelectionInfo);
            }
        }

        /// <summary>
        /// SelectedPortfolio
        /// </summary>
        private String selectedPortfolio;
        public String SelectedPortfolio
        {
            get { return selectedPortfolio; }
            set
            {
                if (selectedPortfolio != value)
                {
                    selectedPortfolio = value;
                    RaisePropertyChanged(() => this.SelectedPortfolio);
                    ResultsListVisibility = Visibility.Collapsed;
                    RaisePropertyChanged(() => this.SubmitCommand);
                    if (selectedPortfolio != null)
                    {
                        SelectedBenchmark = null;
                        SelectedCountry = null;
                        SelectedIndustry = null;
                        SelectedRegion = null;
                        SelectedSector = null;
                    }
                }
            }
        }
        #endregion

        #region Benchmark Selection
        /// <summary>
        /// Property to select benchmark
        /// </summary>
        private List<String> benchmarkSelectionInfo;
        public List<String> BenchmarkSelectionInfo
        {
            get
            {
                return benchmarkSelectionInfo;
            }
            set
            {
                benchmarkSelectionInfo = value;
                RaisePropertyChanged(() => this.BenchmarkSelectionInfo);
            }
        }

        /// <summary>
        /// SelectedBenchmark
        /// </summary>
        private String selectedBenchmark;
        public String SelectedBenchmark
        {
            get { return selectedBenchmark; }
            set
            {
                if (selectedBenchmark != value)
                {
                    selectedBenchmark = value;
                    RaisePropertyChanged(() => this.SelectedBenchmark);
                    RaisePropertyChanged(() => this.SubmitCommand);
                    RaisePropertyChanged(() => this.DataListSelectionGridViewVisibility);
                    if (selectedBenchmark != null)
                    {
                        SelectedPortfolio = null;
                        SelectedCountry = null;
                        SelectedIndustry = null;
                        SelectedRegion = null;
                        SelectedSector = null;
                    }
                }
            }
        }
        #endregion

        #region Custom Selection
        /// <summary>
        /// Property to select custom region
        /// </summary>
        private List<String> customSelectionRegionInfo;
        public List<String> CustomSelectionRegionInfo
        {
            get
            {
                return customSelectionRegionInfo;
            }
            set
            {
                customSelectionRegionInfo = value;
                RaisePropertyChanged(() => this.CustomSelectionRegionInfo);
            }
        }

        /// <summary>
        /// SelectedRegion
        /// </summary>
        private String selectedRegion;
        public String SelectedRegion
        {
            get { return selectedRegion; }
            set
            {
                if (selectedRegion != value)
                {
                    selectedRegion = value;
                    RaisePropertyChanged(() => this.SelectedRegion);
                    RaisePropertyChanged(() => this.SubmitCommand);
                    RaisePropertyChanged(() => this.DataListSelectionGridViewVisibility);
                    if (selectedRegion != null)
                    {
                        SelectedPortfolio = null;
                        SelectedBenchmark = null;
                    }
                }
            }
        }

        /// <summary>
        /// Property to select custom sector
        /// </summary>
        private List<String> customSelectionSectorInfo;
        public List<String> CustomSelectionSectorInfo
        {
            get
            {
                return customSelectionSectorInfo;
            }
            set
            {
                customSelectionSectorInfo = value;
                RaisePropertyChanged(() => this.CustomSelectionSectorInfo);
            }
        }

        /// <summary>
        /// SelectedSector
        /// </summary>
        private String selectedSector;
        public String SelectedSector
        {
            get { return selectedSector; }
            set
            {
                if (selectedSector != value)
                {
                    selectedSector = value;
                    RaisePropertyChanged(() => this.SelectedSector);
                    RaisePropertyChanged(() => this.SubmitCommand);
                    RaisePropertyChanged(() => this.DataListSelectionGridViewVisibility);
                    if (selectedSector != null)
                    {
                        SelectedPortfolio = null;
                        SelectedBenchmark = null;
                    }
                }
            }
        }

        /// <summary>
        /// Property to select custom country
        /// </summary>
        private List<String> customSelectionCountryInfo;
        public List<String> CustomSelectionCountryInfo
        {
            get
            {
                return customSelectionCountryInfo;
            }
            set
            {
                customSelectionCountryInfo = value;
                RaisePropertyChanged(() => this.CustomSelectionCountryInfo);
            }
        }

        /// <summary>
        /// SelectedCountry
        /// </summary>
        private String selectedCountry;
        public String SelectedCountry
        {
            get { return selectedCountry; }
            set
            {
                if (selectedCountry != value)
                {
                    selectedCountry = value;
                    RaisePropertyChanged(() => this.SelectedCountry);
                    RaisePropertyChanged(() => this.SubmitCommand);
                    RaisePropertyChanged(() => this.DataListSelectionGridViewVisibility);
                    if (selectedCountry != null)
                    {
                        SelectedPortfolio = null;
                        SelectedBenchmark = null;
                    }
                }
            }
        }

        /// <summary>
        /// Property to select custom industry
        /// </summary>
        private List<String> customSelectionIndustryInfo;
        public List<String> CustomSelectionIndustryInfo
        {
            get
            {
                return customSelectionIndustryInfo;
            }
            set
            {
                customSelectionIndustryInfo = value;
                RaisePropertyChanged(() => this.CustomSelectionIndustryInfo);
            }
        }

        /// <summary>
        /// SelectedIndustry
        /// </summary>
        private String selectedIndustry;
        public String SelectedIndustry
        {
            get { return selectedIndustry; }
            set
            {
                if (selectedIndustry != value)
                {
                    selectedIndustry = value;
                    RaisePropertyChanged(() => this.SelectedIndustry);
                    RaisePropertyChanged(() => this.SubmitCommand);
                    RaisePropertyChanged(() => this.DataListSelectionGridViewVisibility);
                    if (selectedIndustry != null)
                    {
                        SelectedPortfolio = null;
                        SelectedBenchmark = null;
                    }
                }
            }
        }
        #endregion

        #region Controls Visibility

        /// <summary>
        /// PortfolioSelectionVisibility
        /// </summary>
        private Visibility portfolioSelectionVisibility = Visibility.Collapsed;
        public Visibility PortfolioSelectionVisibility
        {
            get
            {
                return portfolioSelectionVisibility;
            }
            set
            {
                portfolioSelectionVisibility = value;
                RaisePropertyChanged(() => this.PortfolioSelectionVisibility);
            }
        }

        /// <summary>
        /// BenchmarkSelectionVisibility
        /// </summary>
        private Visibility benchmarkSelectionVisibility = Visibility.Collapsed;
        public Visibility BenchmarkSelectionVisibility
        {
            get
            {
                return benchmarkSelectionVisibility;
            }
            set
            {
                benchmarkSelectionVisibility = value;
                RaisePropertyChanged(() => this.BenchmarkSelectionVisibility);
            }
        }

        /// <summary>
        /// CustomSelectionVisibility
        /// </summary>
        private Visibility customSelectionVisibility = Visibility.Collapsed;
        public Visibility CustomSelectionVisibility
        {
            get
            {
                return customSelectionVisibility;
            }
            set
            {
                customSelectionVisibility = value;
                RaisePropertyChanged(() => this.CustomSelectionVisibility);
            }
        }

        /// <summary>
        /// Porperty to enable or disable visibility of the security selection grid
        /// </summary>
        private Visibility securitySelectionGridViewVisibility = Visibility.Visible;
        public Visibility SecuritySelectionGridViewVisibility
        {
            get
            {
                return securitySelectionGridViewVisibility;
            }
            set
            {
                securitySelectionGridViewVisibility = value;
                RaisePropertyChanged(() => this.SecuritySelectionGridViewVisibility);
            }
        }
        #endregion

        #region ICommand Properties

        /// <summary>
        /// SubmitCommand
        /// </summary>
        public ICommand SubmitCommand
        {
            get { return new DelegateCommand<object>(SubmitCommandMethod, SubmitCommandValidationMethod); }
        }

        #endregion
        #endregion

        #region Data List Selector

        /// <summary>
        /// Proerty that contains user preference for custom screening tool
        /// </summary>
        private List<CSTUserPreferenceInfo> cstUserPreferenceInfo;
        public List<CSTUserPreferenceInfo> CSTUserPreference
        {
            get { return cstUserPreferenceInfo; }
            set
            {
                cstUserPreferenceInfo = value;
                RaisePropertyChanged(() => this.CSTUserPreference);
                CSTNavigation.Update(CSTNavigationInfo.CSTUserPreference, CSTUserPreference);
            }
        }

        /// <summary>
        /// Proerty that contains disctinct saved data list information of users
        /// </summary>
        private List<String> savedDataListInfo;
        public List<String> SavedDataListInfo
        {
            get
            {
                return savedDataListInfo;
            }
            set
            {
                savedDataListInfo = value;
                RaisePropertyChanged(() => this.SavedDataListInfo);
            }
        }

        /// <summary>
        /// SelectedDataListInfo
        /// </summary>
        private String selectedDataListInfo;
        public String SelectedDataListInfo
        {
            get
            {
                return selectedDataListInfo;
            }
            set
            {
                selectedDataListInfo = value;
                RaisePropertyChanged(() => this.SelectedDataListInfo);
                SelectedSavedDataList = CSTUserPreference.Where(a => a.ListName == selectedDataListInfo).ToList();
                ResultsListVisibility = Visibility.Collapsed;
                RaisePropertyChanged(() => this.OkCommand);
            }
        }

        /// <summary>
        /// Property that contains user preference information for all records of the selected data list
        /// </summary>
        private List<CSTUserPreferenceInfo> selectedSavedDataList;
        public List<CSTUserPreferenceInfo> SelectedSavedDataList
        {
            get { return selectedSavedDataList; }
            set
            {
                selectedSavedDataList = value;
                RaisePropertyChanged(() => this.SelectedSavedDataList);
                CSTNavigation.Update(CSTNavigationInfo.SelectedDataList, SelectedSavedDataList);
            }
        }

        /// <summary>
        /// Porperty to enable or disable visibility of the DataList Selection GridView
        /// </summary>
        private Visibility dataListSelectionGridViewVisibility = Visibility.Collapsed;
        public Visibility DataListSelectionGridViewVisibility
        {
            get
            {
                return dataListSelectionGridViewVisibility;
            }
            set
            {
                dataListSelectionGridViewVisibility = value;
                RaisePropertyChanged(() => this.DataListSelectionGridViewVisibility);

            }
        }

        /// <summary>
        /// Porperty to enable or disable Ok button
        /// </summary>
        private bool isOkButtonEnabled = false;
        public bool IsOkButtonEnabled
        {
            get { return isOkButtonEnabled; }
            set
            {
                isOkButtonEnabled = value;
                RaisePropertyChanged(() => this.IsOkButtonEnabled);
            }
        }

        #region ICommand Properties
        /// <summary>
        /// OkCommand
        /// </summary>
        public ICommand OkCommand
        {
            get { return new DelegateCommand<object>(OkCommandMethod, OkCommandValidationMethod); }
        }

        /// <summary>
        /// CreateDataListCommand
        /// </summary>
        public ICommand CreateDataListCommand
        {
            get { return new DelegateCommand<object>(CreateDataListCommandMethod, CreateDataListCommandValidationMethod); }
        }
        #endregion

        #endregion

        #region Result Grid
        /// <summary>
        /// IsActive is true when parent control is displayed on UI
        /// </summary>
        private List<CustomScreeningSecurityData> securityData;
        public List<CustomScreeningSecurityData> SecurityData
        {
            get { return securityData; }
            set
            {
                if (securityData != value)
                {
                    securityData = value;
                    RaisePropertyChanged(() => SecurityData);
                }
            }
        }

        /// <summary>
        /// Porperty to enable or disable visibility of the Results Grid
        /// </summary>
        private Visibility resultsListVisibility = Visibility.Collapsed;
        public Visibility ResultsListVisibility
        {
            get
            {
                return resultsListVisibility;
            }
            set
            {
                resultsListVisibility = value;
                RaisePropertyChanged(() => this.ResultsListVisibility);

            }
        }

        #region Events
        /// <summary>
        /// Event for the Retrieval of Data 
        /// </summary>
        public event RetrieveCustomXmlDataCompleteEventHandler RetrieveCustomXmlDataCompletedEvent;
        #endregion

        #endregion

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

        /// <summary>
        /// FlagBusyIndicator
        /// </summary>
        private int flagBusyIndicator;
        public int FlagBusyIndicator
        {
            get { return flagBusyIndicator; }
            set
            {
                flagBusyIndicator = value;
                RaisePropertyChanged(() => this.FlagBusyIndicator);
            }
        }
        #endregion

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

        #endregion

        #region ICommand Methods
        /// <summary>
        /// SubmitCommandValidationMethod
        /// </summary>
        /// <param name="param">object</param>
        private bool SubmitCommandValidationMethod(object param)
        {
            if (SelectedCriteria == SecuritySelectionType.PORTFOLIO)
            {
                if (SelectedPortfolio != null)
                {
                    return true;
                }
                else
                    return false;
            }
            else if (SelectedCriteria == SecuritySelectionType.BENCHMARK)
            {
                if (SelectedBenchmark != null)
                {
                    return true;
                }
                else
                    return false;
            }
            else if (SelectedCriteria == SecuritySelectionType.CUSTOM)
            {
                if (SelectedCountry != null || selectedSector != null || SelectedRegion != null || SelectedIndustry != null)
                {
                    return true;
                }
                else
                    return false;
            }
            else return false;
        }

        /// <summary>
        /// SubmitCommandMethod
        /// </summary>
        /// <param name="param">object</param>
        private void SubmitCommandMethod(object param)
        {
            //create list of selected options and save in the user class and fetch the securities based on filter selctions
            DataListSelectionGridViewVisibility = Visibility.Visible;
            RaisePropertyChanged(() => this.DataListSelectionGridViewVisibility);

            if (dbInteractivity == null || (UserSession.SessionManager.SESSION == null))
                return;
            else
            {
                BusyIndicatorNotification(true, "Retrieving User Preference Data...");
                dbInteractivity.GetCustomScreeningUserPreferences(UserSession.SessionManager.SESSION.UserName, CustomScreeningUserPreferencesCallbackMethod);
            }
        }

        #region Data List Selector
        /// <summary>
        /// OkCommandValidationMethod
        /// </summary>
        /// <param name="param">object</param>
        private bool OkCommandValidationMethod(object param)
        {
            if (UserSession.SessionManager.SESSION == null || SelectedDataListInfo == null)
            {
                IsOkButtonEnabled = false;
                return false;
            }
            else
            {
                IsOkButtonEnabled = true;
                return true;
            }
        }

        /// <summary>
        /// OkCommandMethod
        /// </summary>
        /// <param name="param">object</param>
        private void OkCommandMethod(object param)
        {
            // if signed in user is creater of selected data list, prompt to Edit 
            if (UserSession.SessionManager.SESSION.UserName.ToLower().Equals(SelectedSavedDataList[0].UserName.ToLower()))
            {
                Prompt.ShowDialog("Do you wish to edit the data fields?", "Confirmation", MessageBoxButton.OKCancel, (result) =>
                {
                    // if user clicks 'Ok', data fileds selector screen appears with all data fields pre populated in selected fields list.
                    if (result == MessageBoxResult.OK)
                    {
                        CSTNavigation.UpdateString(CSTNavigationInfo.Flag, "Edit");
                        CSTNavigation.UpdateString(CSTNavigationInfo.ListName, SelectedSavedDataList[0].ListName);
                        CSTNavigation.UpdateString(CSTNavigationInfo.Accessibility, SelectedSavedDataList[0].Accessibility);
                        // open pre populated selected fields list
                        regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardCustomScreeningToolNewDataList", UriKind.Relative));
                    }
                    // if user clicks'Cancel' redirect to results screen           
                    else if (result == MessageBoxResult.Cancel)
                    {
                        PortfolioSelectionData p = new PortfolioSelectionData();
                        p = portfolioSelectionData.Where(a => a.PortfolioId == SelectedPortfolio).FirstOrDefault();

                        EntitySelectionData b = new EntitySelectionData();
                        b = benchmarkSelectionData.Where(a => a.LongName == SelectedBenchmark).FirstOrDefault();
                        BusyIndicatorNotification(true, "Retrieving Data for display");

                        dbInteractivity.RetrieveSecurityData(p, b, SelectedRegion, SelectedCountry, SelectedSector, SelectedIndustry,
                                                                SelectedSavedDataList, RetrieveSecurityDataCallbackMethod);
                    }
                });
            }
            // if signed in user is not creater of selected data list, prompt to View selected data list
            else
            {
                CSTNavigation.UpdateString(CSTNavigationInfo.Flag, "View");
                Prompt.ShowDialog("Do you wish to view the data fields?", "Confirmation", MessageBoxButton.OKCancel, (result) =>
                {
                    if (result == MessageBoxResult.OK)
                    {
                        // open pre populated selected fields list
                        regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardCustomScreeningToolNewDataList", UriKind.Relative));
                    }
                    else if (result == MessageBoxResult.Cancel)
                    {
                        CSTNavigation.UpdateString(CSTNavigationInfo.Flag, "View");
                        // redirect to results views
                        RetrieveResultsList();
                    }
                });
            }
        }

        /// <summary>
        /// CreateDataListCommandValidationMethod
        /// </summary>
        /// <param name="param">object</param>
        private bool CreateDataListCommandValidationMethod(object param)
        {
            if (UserSession.SessionManager.SESSION == null)
                return false;
            else
                return true;
        }

        /// <summary>
        /// CreateDataListCommandMethod
        /// </summary>
        /// <param name="param">object</param>
        private void CreateDataListCommandMethod(object param)
        {
            CSTNavigation.Update(CSTNavigationInfo.SelectedDataList, null);
            CSTNavigation.UpdateString(CSTNavigationInfo.Flag, "Create");
            // navigate to data list selector view
            regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardCustomScreeningToolNewDataList", UriKind.Relative));
        }
        #endregion

        #endregion

        #region Callback Methods

        /// <summary>
        /// Callback method for portfolio selection data service call - assigns value to UI Field Properties
        /// </summary>
        /// <param name="result">PortfolioSelectionData List</param>
        private void PortfolioSelectionDataCallbackMethod(List<PortfolioSelectionData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);

            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result.ToString(), 1);
                    portfolioSelectionData = result;
                    // fetch portfolio id of all records
                    PortfolioSelectionInfo = result.Select(o => o.PortfolioId).ToList();
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
            }
        }

        /// <summary>
        /// Callback method for benchmark data service call - assigns value to UI Field Properties
        /// </summary>
        /// <param name="result">EntitySelectionData List</param>
        private void EntitySelectionDataCallbackMethod(List<EntitySelectionData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);

            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result.ToString(), 1);

                    benchmarkSelectionData = result;
                    //fetch benchmarks
                    BenchmarkSelectionInfo = result.Where(a => a.Type.Equals("BENCHMARK")).OrderBy(x=>x.LongName).Select(a => a.LongName).ToList();
                    
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
            }
        }

        /// <summary>
        /// Callback method for custom selection region data service call - assigns value to UI Field Properties
        /// </summary>
        /// <param name="result"> string List</param>
        private void CustomControlsListRegionCallbackMethod(List<string> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result, 1);
                    CustomSelectionRegionInfo = result;
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
            }
        }

        /// <summary>
        /// Callback method for custom selection country data service call - assigns value to UI Field Properties
        /// </summary>
        /// <param name="result"> string List</param>
        private void CustomControlsListCountryCallbackMethod(List<string> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result, 1);
                    CustomSelectionCountryInfo = result;
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
            }
        }

        /// <summary>
        /// Callback method for custom selection sector data service call - assigns value to UI Field Properties
        /// </summary>
        /// <param name="result"> string List</param>
        private void CustomControlsListSectorCallbackMethod(List<string> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result, 1);
                    CustomSelectionSectorInfo = result;
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
                if(flagBsyInd != 1)
                {
                    BusyIndicatorNotification();
                }
            }
        }

        /// <summary>
        /// Callback method for custom selection industry data service call - assigns value to UI Field Properties
        /// </summary>
        /// <param name="result"> string List</param>
        private void CustomControlsListIndustryCallbackMethod(List<string> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result, 1);
                    CustomSelectionIndustryInfo = result;
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
            }
        }

        /// <summary>
        /// Callback method for custom screening user preference data service call - assigns value to UI Field Properties
        /// </summary>
        /// <param name="result"> string List</param>
        private void CustomScreeningUserPreferencesCallbackMethod(List<CSTUserPreferenceInfo> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result, 1);
                    CSTUserPreference = result;
                    // fetch disctinct saved data list names
                    SavedDataListInfo = result.OrderBy(a => a.ListName).Select(a => a.ListName).Distinct().ToList();
                    BusyIndicatorNotification();
                }
                else
                {
                    Logging.LogMethodParameterNull(logger, methodNamespace, 1);
                    BusyIndicatorNotification();
                }
            }
            catch (Exception ex)
            {
                BusyIndicatorNotification();
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            finally
            {
                Logging.LogEndMethod(logger, methodNamespace);
            }
        }

        /// <summary>
        /// Callback method for security data service call - assigns value to UI Field Properties
        /// </summary>
        /// <param name="result"> string List</param>
        private void RetrieveSecurityDataCallbackMethod(List<CustomScreeningSecurityData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result, 1);
                    SecurityData = result;
                    if (SelectedSavedDataList != null && SelectedSavedDataList.Count > 0 && SelectedSavedDataList[0].ScreeningId != null)
                    {
                        // create an xml format security data for selected securities and data list fields to be displayed in results grid
                        CreateXML(SecurityData);
                    }
                    else
                    {
                        BusyIndicatorNotification();
                    }
                    ResultsListVisibility = Visibility.Visible;
                }
                else
                {
                    Logging.LogMethodParameterNull(logger, methodNamespace, 1);
                    BusyIndicatorNotification();
                }
            }
            catch (Exception ex)
            {
                BusyIndicatorNotification();
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            finally
            {
                Logging.LogEndMethod(logger, methodNamespace);
            }
        }        

        #endregion

        #region Helpers

        /// <summary>
        /// Method that will be called when the view is active
        /// </summary>
        public void Initialize()
        {
            FlagBusyIndicator = 0;
            SelectionRaisePropertyChanged();

            // update fields to be accessed from navigated views
            CSTNavigation.UpdateString(CSTNavigationInfo.Accessibility, null);
            CSTNavigation.UpdateString(CSTNavigationInfo.ListName, null);

            if (dbInteractivity != null && IsActive)
            {
                SecuritySelectionGridViewVisibility = Visibility.Visible;
                DataListSelectionGridViewVisibility = Visibility.Collapsed;

                // fetch portfolioid list 
                FetchSelectionCriteriaData();

                // fetch final list of securities
                RetrieveResultsList();
            }
            CSTNavigation.UpdateString(CSTNavigationInfo.Flag, null);
        }

        /// <summary>
        /// Method that notifies controls
        /// </summary>
        private void SelectionRaisePropertyChanged()
        {
            RaisePropertyChanged(() => this.SelectedCriteria);
            RaisePropertyChanged(() => this.SelectedPortfolio);
            RaisePropertyChanged(() => this.SelectedBenchmark);
            RaisePropertyChanged(() => this.SelectedCountry);
            RaisePropertyChanged(() => this.SelectedIndustry);
            RaisePropertyChanged(() => this.SelectedRegion);
            RaisePropertyChanged(() => this.SelectedSector);
            RaisePropertyChanged(() => this.PortfolioSelectionVisibility);
            RaisePropertyChanged(() => this.BenchmarkSelectionVisibility);
            RaisePropertyChanged(() => this.CustomSelectionVisibility);
            RaisePropertyChanged(() => this.SecuritySelectionGridViewVisibility);
            RaisePropertyChanged(() => this.SavedDataListInfo);
            RaisePropertyChanged(() => this.SelectedDataListInfo);
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
        /// Method to fetch data for populating recults grid
        /// </summary>
        public void RetrieveResultsList()
        {
            string flag = CSTNavigation.FetchString(CSTNavigationInfo.Flag) as string;

            if (flag == "Created" || flag == "Edited")
            {
                SelectedSavedDataList = CSTNavigation.Fetch(CSTNavigationInfo.SelectedDataList) as List<CSTUserPreferenceInfo>;

                if (SelectedSavedDataList != null)
                {
                    PortfolioSelectionData p = new PortfolioSelectionData();
                    p = portfolioSelectionData.Where(a => a.PortfolioId == SelectedPortfolio).FirstOrDefault();

                    EntitySelectionData b = new EntitySelectionData();
                    b = benchmarkSelectionData.Where(a => a.LongName == SelectedBenchmark).FirstOrDefault();
                    BusyIndicatorNotification(true, "Retrieving Data for display");
                    flagBsyInd = 1;
                    dbInteractivity.RetrieveSecurityData(p, b, SelectedRegion, SelectedCountry, SelectedSector, SelectedIndustry,
                                                            SelectedSavedDataList, RetrieveSecurityDataCallbackMethod);
                }
            }
            else if (flag == "View")
            {
                if (SelectedSavedDataList != null)
                {
                    PortfolioSelectionData p = new PortfolioSelectionData();
                    p = portfolioSelectionData.Where(a => a.PortfolioId == SelectedPortfolio).FirstOrDefault();

                    EntitySelectionData b = new EntitySelectionData();
                    b = benchmarkSelectionData.Where(a => a.LongName == SelectedBenchmark).FirstOrDefault();
                    BusyIndicatorNotification(true, "Retrieving Data for display");
                    flagBsyInd = 1;
                    dbInteractivity.RetrieveSecurityData(p, b, SelectedRegion, SelectedCountry, SelectedSector, SelectedIndustry,
                                                            SelectedSavedDataList, RetrieveSecurityDataCallbackMethod);
                }
            }
            else if (flag == null)
            {
                flagBsyInd = 0;
            }
        }

        /// <summary>
        /// ReplaceSpecialCharacters
        /// </summary>
        public static string ReplaceSpecialCharacters(string raw)
        {
            string replacedString = Regex.Replace(raw, "[^0-9a-zA-Z]+", "_");
            replacedString = replacedString.Replace("___", "__");
            replacedString = replacedString.Replace("__", "_");

            if (replacedString.StartsWith("_"))
            {
                replacedString = replacedString.Substring(1);
            }

            if (replacedString.EndsWith("_"))
            {
                replacedString = replacedString.Substring(0, replacedString.Length - 1);
            }
            return replacedString;
        }

        /// <summary>
        /// Method to call web service methods for different selection criteria
        /// </summary>
        public void FetchSelectionCriteriaData()
        {
            BusyIndicatorNotification(true, "Retrieving Data...");
            dbInteractivity.RetrievePortfolioSelectionData(PortfolioSelectionDataCallbackMethod);

            BusyIndicatorNotification(true, "Retrieving Data...");
            dbInteractivity.RetrieveEntitySelectionData(EntitySelectionDataCallbackMethod);

            BusyIndicatorNotification(true, "Retrieving Data...");
            dbInteractivity.RetrieveCustomControlsList("Region", CustomControlsListRegionCallbackMethod);

            BusyIndicatorNotification(true, "Retrieving Data...");
            dbInteractivity.RetrieveCustomControlsList("Country", CustomControlsListCountryCallbackMethod);

            BusyIndicatorNotification(true, "Retrieving Data...");
            dbInteractivity.RetrieveCustomControlsList("Sector", CustomControlsListSectorCallbackMethod);

            BusyIndicatorNotification(true, "Retrieving Data...");
            dbInteractivity.RetrieveCustomControlsList("Industry", CustomControlsListIndustryCallbackMethod);
        }

        /// <summary>
        /// Method for generating xml for results grid
        /// </summary>
        /// <param name="result"> string List</param>
        private void CreateXML(List<CustomScreeningSecurityData> securityData)
        {
            Dictionary<String, String> changedColumnNames = new Dictionary<String, String>();

            foreach (String a in SelectedSavedDataList.Select(a => a.TableColumn).Distinct())
            {
                if (a != null)
                {
                    String b = ReplaceSpecialCharacters(a);
                    changedColumnNames.Add(a, b);
                }
            }

            StringBuilder output = new StringBuilder();
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings xws = new XmlWriterSettings();
            xws.OmitXmlDeclaration = true;
            xws.Indent = true;

            using (XmlWriter xw = XmlWriter.Create(sb, xws))
            {
                xw.WriteStartDocument();

                xw.WriteStartElement("data");
                xw.WriteStartElement("columns");
                xw.WriteStartElement("column");

                xw.WriteAttributeString("name", "Security Ticker");
                xw.WriteAttributeString("displayname", String.Empty);
                xw.WriteAttributeString("isaggregate", "false");
                xw.WriteAttributeString("isdisplay", "true");
                xw.WriteEndElement();

                xw.WriteStartElement("column");
                xw.WriteAttributeString("name", "Security Name");
                xw.WriteAttributeString("displayname", String.Empty);
                xw.WriteAttributeString("isaggregate", "false");
                xw.WriteAttributeString("isdisplay", "true");
                xw.WriteEndElement();

                xw.WriteStartElement("column");
                xw.WriteAttributeString("name", "Market Capitalization");
                xw.WriteAttributeString("displayname", "Market Capitalization");
                xw.WriteAttributeString("isaggregate", "false");
                xw.WriteAttributeString("isdisplay", "false");
                xw.WriteEndElement();

                foreach (CSTUserPreferenceInfo info in SelectedSavedDataList)
                {
                    if (info.ScreeningId.StartsWith("REF"))
                    {
                        xw.WriteStartElement("column");
                        xw.WriteAttributeString("name", changedColumnNames[info.TableColumn]);
                        xw.WriteAttributeString("displayname", info.DataDescription);
                        xw.WriteAttributeString("isaggregate", "false");
                        xw.WriteAttributeString("isdisplay", "true");
                        xw.WriteEndElement();
                    }
                    else if (info.ScreeningId.StartsWith("CUR") || info.ScreeningId.StartsWith("FVA"))
                    {
                        if (info.ScreeningId.StartsWith("CUR"))
                        {
                            xw.WriteStartElement("column");
                            xw.WriteAttributeString("name", changedColumnNames[info.TableColumn] + info.DataSource);
                            xw.WriteAttributeString("displayname", info.DataDescription);
                            xw.WriteAttributeString("isaggregate", "true");
                            xw.WriteAttributeString("isdisplay", "true");
                            xw.WriteEndElement();
                        }
                        else
                        {
                            xw.WriteStartElement("column");
                            xw.WriteAttributeString("name", changedColumnNames[info.TableColumn] + info.DataSource);
                            xw.WriteAttributeString("displayname", info.DataDescription);
                            xw.WriteAttributeString("isaggregate", "false");
                            xw.WriteAttributeString("isdisplay", "true");
                            xw.WriteEndElement();
                        }
                    }
                    else if (info.ScreeningId.StartsWith("FIN"))
                    {
                        xw.WriteStartElement("column");
                        xw.WriteAttributeString("name", changedColumnNames[info.TableColumn] + info.FromDate + info.DataSource.Substring(0, 3)
                            + info.PeriodType + info.YearType.Substring(0, 1));
                        xw.WriteAttributeString("displayname", info.TableColumn);
                        xw.WriteAttributeString("isaggregate", "true");
                        xw.WriteAttributeString("isdisplay", "true");
                        xw.WriteEndElement();
                    }
                }

                xw.WriteEndElement();
                xw.WriteStartElement("subcolumns");

                foreach (CSTUserPreferenceInfo info in SelectedSavedDataList)
                {
                    if (info.ScreeningId.StartsWith("REF"))
                    {
                        xw.WriteStartElement("subcolumn");
                        xw.WriteAttributeString("name", changedColumnNames[info.TableColumn]);
                        xw.WriteAttributeString("displayname", info.DataDescription);
                        xw.WriteString(String.Empty);
                        xw.WriteEndElement();
                    }
                    else if (info.ScreeningId.StartsWith("CUR") || info.ScreeningId.StartsWith("FVA"))
                    {
                        xw.WriteStartElement("subcolumn");
                        xw.WriteAttributeString("name", changedColumnNames[info.TableColumn] + info.DataSource);
                        xw.WriteAttributeString("displayname", info.DataDescription);
                        xw.WriteString(Convert.ToString(String.Empty));
                        xw.WriteEndElement();
                    }
                    else if (info.ScreeningId.StartsWith("FIN"))
                    {
                        xw.WriteStartElement("subcolumn");
                        xw.WriteAttributeString("name", changedColumnNames[info.TableColumn] + info.FromDate + info.DataSource.Substring(0, 3)
                            + info.PeriodType + info.YearType.Substring(0, 1));
                        xw.WriteAttributeString("displayname", info.TableColumn);
                        xw.WriteString(Convert.ToString(info.FromDate));
                        xw.WriteEndElement();
                    }
                }
                xw.WriteStartElement("subcolumn");
                xw.WriteAttributeString("name", "Security Ticker");
                xw.WriteAttributeString("displayname", String.Empty);
                xw.WriteString(String.Empty);
                xw.WriteEndElement();

                xw.WriteStartElement("subcolumn");
                xw.WriteAttributeString("name", "Security Name");
                xw.WriteAttributeString("displayname", String.Empty);
                xw.WriteString("Year");
                xw.WriteEndElement();

                xw.WriteStartElement("subcolumn");
                xw.WriteAttributeString("name", "Market Capitalization");
                xw.WriteAttributeString("displayname", "Market Capitalization");
                xw.WriteString(String.Empty);
                xw.WriteEndElement();

                xw.WriteEndElement();
                xw.WriteStartElement("subcolumns");
                foreach (CSTUserPreferenceInfo info in SelectedSavedDataList)
                {
                    if (info.ScreeningId.StartsWith("REF"))
                    {
                        xw.WriteStartElement("subcolumn");
                        xw.WriteAttributeString("name", changedColumnNames[info.TableColumn]);
                        xw.WriteAttributeString("displayname", info.DataDescription);
                        xw.WriteString(String.Empty);
                        xw.WriteEndElement();
                    }
                    else if (info.ScreeningId.StartsWith("CUR") || info.ScreeningId.StartsWith("FVA"))
                    {
                        xw.WriteStartElement("subcolumn");
                        xw.WriteAttributeString("name", changedColumnNames[info.TableColumn] + info.DataSource);
                        xw.WriteAttributeString("displayname", info.DataDescription);
                        xw.WriteString(String.Empty);
                        xw.WriteEndElement();
                    }
                    else if (info.ScreeningId.StartsWith("FIN"))
                    {
                        xw.WriteStartElement("subcolumn");
                        xw.WriteAttributeString("name", changedColumnNames[info.TableColumn] + info.FromDate + info.DataSource.Substring(0, 3)
                            + info.PeriodType + info.YearType.Substring(0, 1));
                        xw.WriteAttributeString("displayname", info.TableColumn);
                        xw.WriteString(Convert.ToString(info.PeriodType));
                        xw.WriteEndElement();
                    }
                }
                xw.WriteStartElement("subcolumn");
                xw.WriteAttributeString("name", "Security Ticker");
                xw.WriteAttributeString("displayname", String.Empty);
                xw.WriteString(String.Empty);
                xw.WriteEndElement();

                xw.WriteStartElement("subcolumn");
                xw.WriteAttributeString("name", "Security Name");
                xw.WriteAttributeString("displayname", String.Empty);
                xw.WriteString("Period Type");
                xw.WriteEndElement();

                xw.WriteStartElement("subcolumn");
                xw.WriteAttributeString("name", "Market Capitalization");
                xw.WriteAttributeString("displayname", "Market Capitalization");
                xw.WriteString(String.Empty);
                xw.WriteEndElement();

                xw.WriteEndElement();
                xw.WriteStartElement("subcolumns");
                foreach (CSTUserPreferenceInfo info in SelectedSavedDataList)
                {
                    if (info.ScreeningId.StartsWith("REF"))
                    {
                        xw.WriteStartElement("subcolumn");
                        xw.WriteAttributeString("name", changedColumnNames[info.TableColumn]);
                        xw.WriteAttributeString("displayname", info.DataDescription);
                        xw.WriteString(String.Empty);
                        xw.WriteEndElement();
                    }
                    else if (info.ScreeningId.StartsWith("CUR") || info.ScreeningId.StartsWith("FVA"))
                    {
                        xw.WriteStartElement("subcolumn");
                        xw.WriteAttributeString("name", changedColumnNames[info.TableColumn] + info.DataSource);
                        xw.WriteAttributeString("displayname", info.DataDescription);
                        xw.WriteString(String.Empty);
                        xw.WriteEndElement();
                    }
                    else if (info.ScreeningId.StartsWith("FIN"))
                    {
                        xw.WriteStartElement("subcolumn");
                        xw.WriteAttributeString("name", changedColumnNames[info.TableColumn] + info.FromDate + info.DataSource.Substring(0, 3)
                            + info.PeriodType + info.YearType.Substring(0, 1));
                        xw.WriteAttributeString("displayname", info.TableColumn);
                        xw.WriteString(Convert.ToString(info.YearType));
                        xw.WriteEndElement();
                    }
                }
                xw.WriteStartElement("subcolumn");
                xw.WriteAttributeString("name", "Security Ticker");
                xw.WriteAttributeString("displayname", String.Empty);
                xw.WriteString(String.Empty);
                xw.WriteEndElement();

                xw.WriteStartElement("subcolumn");
                xw.WriteAttributeString("name", "Security Name");
                xw.WriteAttributeString("displayname", String.Empty);
                xw.WriteString("Year Type");
                xw.WriteEndElement();

                xw.WriteStartElement("subcolumn");
                xw.WriteAttributeString("name", "Market Capitalization");
                xw.WriteAttributeString("displayname", "Market Capitalization");
                xw.WriteString(String.Empty);
                xw.WriteEndElement();

                xw.WriteEndElement();
                xw.WriteStartElement("subcolumns");
                foreach (CSTUserPreferenceInfo info in SelectedSavedDataList)
                {
                    if (info.ScreeningId.StartsWith("REF"))
                    {
                        xw.WriteStartElement("subcolumn");
                        xw.WriteAttributeString("name", changedColumnNames[info.TableColumn]);
                        xw.WriteAttributeString("displayname", info.DataDescription);
                        xw.WriteString(String.Empty);
                        xw.WriteEndElement();
                    }
                    else if (info.ScreeningId.StartsWith("CUR") || info.ScreeningId.StartsWith("FVA"))
                    {
                        xw.WriteStartElement("subcolumn");
                        xw.WriteAttributeString("name", changedColumnNames[info.TableColumn] + info.DataSource);
                        xw.WriteAttributeString("displayname", info.DataDescription);
                        xw.WriteString(info.DataSource);
                        xw.WriteEndElement();
                    }
                    else if (info.ScreeningId.StartsWith("FIN"))
                    {
                        xw.WriteStartElement("subcolumn");
                        xw.WriteAttributeString("name", changedColumnNames[info.TableColumn] + info.FromDate + info.DataSource.Substring(0, 3)
                            + info.PeriodType + info.YearType.Substring(0, 1));
                        xw.WriteAttributeString("displayname", info.TableColumn);
                        xw.WriteString(info.DataSource);
                        xw.WriteEndElement();
                    }
                }
                xw.WriteStartElement("subcolumn");
                xw.WriteAttributeString("name", "Security Ticker");
                xw.WriteAttributeString("displayname", String.Empty);
                xw.WriteString(String.Empty);
                xw.WriteEndElement();

                xw.WriteStartElement("subcolumn");
                xw.WriteAttributeString("name", "Security Name");
                xw.WriteAttributeString("displayname", String.Empty);
                xw.WriteString("Source");
                xw.WriteEndElement();

                xw.WriteStartElement("subcolumn");
                xw.WriteAttributeString("name", "Market Capitalization");
                xw.WriteAttributeString("displayname", "Market Capitalization");
                xw.WriteString(String.Empty);
                xw.WriteEndElement();

                xw.WriteEndElement();

                if (securityData != null)
                {
                    List<String> distinctIssueNames = securityData.Select(a => a.IssueName).Distinct().ToList();
                    foreach (String issueName in distinctIssueNames)
                    {
                        xw.WriteStartElement("row");
                        foreach (CSTUserPreferenceInfo info in SelectedSavedDataList)
                        {
                            if (info.ScreeningId.StartsWith("REF"))
                            {
                                xw.WriteStartElement("Element");
                                xw.WriteAttributeString("name", changedColumnNames[info.TableColumn]);
                                xw.WriteAttributeString("displayname", info.DataDescription);
                                
                                CustomScreeningSecurityData selectedSec = (from p in securityData
                                                                           where p.Type == info.TableColumn
                                                                           && p.IssueName == issueName
                                                                           select p).FirstOrDefault();

                                string attributeValue = (selectedSec == null || selectedSec.Value == null) ?
                                    String.Empty : selectedSec.Value.ToString();
                                xw.WriteString(attributeValue);
                               
                                xw.WriteEndElement();
                            }
                            else if (info.ScreeningId.StartsWith("CUR") || info.ScreeningId.StartsWith("FVA"))
                            {
                                xw.WriteStartElement("Element");
                                xw.WriteAttributeString("name", changedColumnNames[info.TableColumn] + info.DataSource);
                                xw.WriteAttributeString("displayname", info.DataDescription);

                                CustomScreeningSecurityData selectedSec = (from p in securityData
                                                                           where p.Type == info.TableColumn
                                                                              && p.IssueName == issueName
                                                                           && p.DataSource == info.DataSource
                                                                           select p).FirstOrDefault();

                                string attributeValue = (selectedSec == null || selectedSec.Value == null) ?
                                    String.Empty : selectedSec.Value.ToString();
                                xw.WriteString(attributeValue);
                              
                                xw.WriteEndElement();
                            }
                            else if (info.ScreeningId.StartsWith("FIN"))
                            {
                                xw.WriteStartElement("Element");
                                xw.WriteAttributeString("name", changedColumnNames[info.TableColumn] + info.FromDate + info.DataSource.Substring(0, 3)
                               + info.PeriodType + info.YearType.Substring(0, 1));
                                xw.WriteAttributeString("displayname", info.TableColumn);

                                CustomScreeningSecurityData selectedSec = (from p in securityData
                                                                           where p.Type == info.TableColumn
                                                                           && p.PeriodYear == info.FromDate
                                                                           && p.IssueName == issueName
                                                                           && p.PeriodType == info.PeriodType
                                                                           && p.DataSource == info.DataSource
                                                                           && p.YearType == info.YearType
                                                                           select p).FirstOrDefault();

                                string attributeValue = (selectedSec == null || selectedSec.Value == null) ?
                                    String.Empty : selectedSec.Value.ToString();
                                xw.WriteString(attributeValue);
                                
                                xw.WriteEndElement();
                            }
                        }

                        xw.WriteStartElement("Element");
                        xw.WriteAttributeString("name", "Security Ticker");
                        xw.WriteAttributeString("displayname", String.Empty);
                        String securityId = securityData.Where(a => a.IssueName == issueName).Select(a => a.AsecShortName).FirstOrDefault();
                        xw.WriteString(securityId);
                        xw.WriteEndElement();

                        xw.WriteStartElement("Element");
                        xw.WriteAttributeString("name", "Security Name");
                        xw.WriteAttributeString("displayname", String.Empty);
                        xw.WriteString(issueName);
                        xw.WriteEndElement();

                        xw.WriteStartElement("Element");
                        xw.WriteAttributeString("name", "Market Capitalization");
                        xw.WriteAttributeString("displayname", "Market Capitalization");
                        Object marketCapitalization = securityData.Where(a => a.IssueName == issueName).Select(a => a.MarketCapAmount).FirstOrDefault();
                        xw.WriteString(Convert.ToString(marketCapitalization));
                        xw.WriteEndElement();
                        xw.WriteEndElement();
                    
                    }
                }
                xw.WriteEndElement();
                xw.WriteEndDocument();
            }
           
            output.Append(sb.ToString() + Environment.NewLine);
            RetrieveCustomXmlDataCompletedEvent(new RetrieveCustomXmlDataCompleteEventArgs() { XmlInfo = output.ToString() });
        }

        #endregion
        
        #region EventUnSubscribe
        /// <summary>
        /// Method that disposes the events
        /// </summary>
        public void Dispose()
        {
            // unsubscribe event here
        }

        #endregion
    }
}

