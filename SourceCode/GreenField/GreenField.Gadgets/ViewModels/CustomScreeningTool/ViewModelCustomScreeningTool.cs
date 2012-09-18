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

namespace GreenField.Gadgets.ViewModels
{
    public class ViewModelCustomScreeningTool : NotificationObject
    {
        #region Fields
        /// <summary>
        /// MEF Singletons
        /// </summary>
        private IEventAggregator _eventAggregator;
        private IDBInteractivity _dbInteractivity;
        private ILoggerFacade _logger;
        private List<PortfolioSelectionData> _portfolioSelectionData;
        private IRegionManager _regionManager;
        private List<EntitySelectionData> _benchmarkSelectionData;

        #endregion

        #region Constructor
        public ViewModelCustomScreeningTool(DashboardGadgetParam param)
        {
            _logger = param.LoggerFacade;
            //param.DashboardGadgetPayload.EntitySelectionData
            _dbInteractivity = param.DBInteractivity;
            _eventAggregator = param.EventAggregator;
            _regionManager = param.RegionManager;

            //BusyIndicatorNotification(true, "Retrieving Portfolio Selection Data...");
            ////fetch PortfolioId list 
            //_dbInteractivity.RetrievePortfolioSelectionData(PortfolioSelectionDataCallbackMethod);

            //BusyIndicatorNotification(true, "Retrieving Benchmark Selection Data...");
            ////fetch Benchmark list
            //_dbInteractivity.RetrieveEntitySelectionData(EntitySelectionDataCallbackMethod);

            ////retrieve custom selection data
            //RetrieveCustomSelectionData();

            //BusyIndicatorNotification();
        }
        #endregion

        #region Properties

        public List<String> _securitySelectionCriteria;
        public List<String> SecuritySelectionCriteria
        {
            get
            {
                { return _securitySelectionCriteria = new List<String> { "Portfolio", "Benchmark", "Custom" }; }
            }
            set
            {
                _securitySelectionCriteria = value;
                RaisePropertyChanged(() => this.SecuritySelectionCriteria);
            }
        }


        public List<String> _portfolioSelectionInfo;
        public List<String> PortfolioSelectionInfo
        {
            get
            {
                return _portfolioSelectionInfo;
            }
            set
            {
                _portfolioSelectionInfo = value;
                RaisePropertyChanged(() => this.PortfolioSelectionInfo);
            }
        }

        public List<String> _benchmarkSelectionInfo;
        public List<String> BenchmarkSelectionInfo
        {
            get
            {
                return _benchmarkSelectionInfo;
            }
            set
            {
                _benchmarkSelectionInfo = value;
                RaisePropertyChanged(() => this.BenchmarkSelectionInfo);
            }
        }

        public List<String> _customSelectionRegionInfo;
        public List<String> CustomSelectionRegionInfo
        {
            get
            {
                return _customSelectionRegionInfo;
            }
            set
            {
                _customSelectionRegionInfo = value;
                RaisePropertyChanged(() => this.CustomSelectionRegionInfo);
            }
        }


        public List<String> _customSelectionSectorInfo;
        public List<String> CustomSelectionSectorInfo
        {
            get
            {
                return _customSelectionSectorInfo;
            }
            set
            {
                _customSelectionSectorInfo = value;
                RaisePropertyChanged(() => this.CustomSelectionSectorInfo);
            }
        }

        public List<String> _customSelectionCountryInfo;
        public List<String> CustomSelectionCountryInfo
        {
            get
            {
                return _customSelectionCountryInfo;
            }
            set
            {
                _customSelectionCountryInfo = value;
                RaisePropertyChanged(() => this.CustomSelectionCountryInfo);
            }
        }

        public List<String> _customSelectionIndustryInfo;
        public List<String> CustomSelectionIndustryInfo
        {
            get
            {
                return _customSelectionIndustryInfo;
            }
            set
            {
                _customSelectionIndustryInfo = value;
                RaisePropertyChanged(() => this.CustomSelectionIndustryInfo);
            }
        }

        public String _selectedCriteria;
        public String SelectedCriteria
        {

            get { return _selectedCriteria; }
            set
            {
                _selectedCriteria = value;
                RaisePropertyChanged(() => this.SelectedCriteria);
                if (SelectedCriteria == SecuritySelectionType.PORTFOLIO)
                {
                    PortfolioSelectionVisibility = Visibility.Visible;
                    BenchmarkSelectionVisibility = Visibility.Collapsed;
                    CustomSelectionVisibility = Visibility.Collapsed;
                    RaisePropertyChanged(() => this.PortfolioSelectionVisibility);
                    RaisePropertyChanged(() => this.BenchmarkSelectionVisibility);
                    RaisePropertyChanged(() => this.CustomSelectionVisibility);
                }
                else if (SelectedCriteria == SecuritySelectionType.BENCHMARK)
                {
                    BenchmarkSelectionVisibility = Visibility.Visible;
                    PortfolioSelectionVisibility = Visibility.Collapsed;
                    CustomSelectionVisibility = Visibility.Collapsed;
                    RaisePropertyChanged(() => this.BenchmarkSelectionVisibility);
                    RaisePropertyChanged(() => this.PortfolioSelectionVisibility);
                    RaisePropertyChanged(() => this.CustomSelectionVisibility);
                }
                else if (SelectedCriteria == SecuritySelectionType.CUSTOM)
                {
                    CustomSelectionVisibility = Visibility.Visible;
                    PortfolioSelectionVisibility = Visibility.Collapsed;
                    BenchmarkSelectionVisibility = Visibility.Collapsed;
                    RaisePropertyChanged(() => this.CustomSelectionVisibility);
                    RaisePropertyChanged(() => this.PortfolioSelectionVisibility);
                    RaisePropertyChanged(() => this.BenchmarkSelectionVisibility);
                }
            }
        }

        public String _selectedPortfolio;
        public String SelectedPortfolio
        {
            get { return _selectedPortfolio; }
            set
            {
                if (value != null)
                {
                    _selectedPortfolio = value;
                    RaisePropertyChanged(() => this.SelectedPortfolio);
                    // PortfolioSelectionData p = new PortfolioSelectionData();
                    //p = _portfolioSelectionData.Where(a => a.PortfolioId == _selectedPortfolio).FirstOrDefault();
                    //_dbInteractivity.RetrieveSecurityData(p, null, SelectedRegion, SelectedCountry, SelectedSector, SelectedIndustry
                    //                                   , null, RetrieveSecurityDataCallbackMethod);
                    ResultsListVisibility = Visibility.Collapsed;                    
                    RaisePropertyChanged(() => this.SubmitCommand);
                }
            }
        }

        public String _selectedBenchmark;
        public String SelectedBenchmark
        {
            get { return _selectedBenchmark; }
            set
            {
                _selectedBenchmark = value;
                RaisePropertyChanged(() => this.SelectedBenchmark);
                RaisePropertyChanged(() => this.DataListSelectionGridViewVisibility);
            }
        }

        public String _selectedRegion;
        public String SelectedRegion
        {
            get { return _selectedRegion; }
            set
            {
                _selectedRegion = value;
                RaisePropertyChanged(() => this.SelectedRegion);
                RaisePropertyChanged(() => this.DataListSelectionGridViewVisibility);
            }
        }

        public String _selectedCountry;
        public String SelectedCountry
        {
            get { return _selectedCountry; }
            set
            {
                _selectedCountry = value;
                RaisePropertyChanged(() => this.SelectedCountry);
                RaisePropertyChanged(() => this.DataListSelectionGridViewVisibility);
            }
        }

        public String _selectedSector;
        public String SelectedSector
        {
            get { return _selectedSector; }
            set
            {
                _selectedSector = value;
                RaisePropertyChanged(() => this.SelectedSector);
                RaisePropertyChanged(() => this.DataListSelectionGridViewVisibility);
            }
        }

        public String _selectedIndustry;
        public String SelectedIndustry
        {
            get { return _selectedIndustry; }
            set
            {
                _selectedIndustry = value;
                RaisePropertyChanged(() => this.SelectedIndustry);
                RaisePropertyChanged(() => this.DataListSelectionGridViewVisibility);
            }
        }



        private Visibility _portfolioSelectionVisibility = Visibility.Collapsed;
        public Visibility PortfolioSelectionVisibility
        {
            get
            {
                //_portfolioSelectionVisibility = Visibility.Visible;
                return _portfolioSelectionVisibility;
            }
            set
            {
                _portfolioSelectionVisibility = value;
                RaisePropertyChanged(() => this.PortfolioSelectionVisibility);
            }
        }

        private Visibility _benchmarkSelectionVisibility = Visibility.Collapsed;
        public Visibility BenchmarkSelectionVisibility
        {
            get
            {
                //_benchmarkSelectionVisibility = Visibility.Visible;
                return _benchmarkSelectionVisibility;
            }
            set
            {
                _benchmarkSelectionVisibility = value;
                RaisePropertyChanged(() => this.BenchmarkSelectionVisibility);
            }
        }

        private Visibility _customSelectionVisibility = Visibility.Collapsed;
        public Visibility CustomSelectionVisibility
        {
            get
            {
                //_customSelectionVisibility = Visibility.Visible;
                return _customSelectionVisibility;
            }
            set
            {
                _customSelectionVisibility = value;
                RaisePropertyChanged(() => this.CustomSelectionVisibility);
            }
        }


        private Visibility _securitySelectionGridViewVisibility = Visibility.Visible;
        public Visibility SecuritySelectionGridViewVisibility
        {
            get
            {
                return _securitySelectionGridViewVisibility;
            }
            set
            {
                _securitySelectionGridViewVisibility = value;
                RaisePropertyChanged(() => this.SecuritySelectionGridViewVisibility);
            }
        }

        #region ICommand Properties

        public ICommand SubmitCommand
        {
            get { return new DelegateCommand<object>(SubmitCommandMethod, SubmitCommandValidationMethod); }
        }

        #endregion

        #region Data List Selector

        private List<CSTUserPreferenceInfo> _cstUserPreferenceInfo;
        public List<CSTUserPreferenceInfo> CSTUserPreference
        {
            get { return _cstUserPreferenceInfo; }
            set
            {
                _cstUserPreferenceInfo = value;
                RaisePropertyChanged(() => this.CSTUserPreference);
            }
        }

        public List<String> _savedDataListInfo;
        public List<String> SavedDataListInfo
        {
            get
            {
                return _savedDataListInfo;
                //{ return new List<String> { "DataList1", "DataList2", "DataList3" }; }
            }
            set
            {
                _savedDataListInfo = value;
                RaisePropertyChanged(() => this.SavedDataListInfo);
            }
        }

        public String _selectedDataListInfo;
        public String SelectedDataListInfo
        {
            get
            {
                return _selectedDataListInfo;
            }
            set
            {
                _selectedDataListInfo = value;
                RaisePropertyChanged(() => this.SelectedDataListInfo);
                SelectedSavedDataList = CSTUserPreference.Where(a => a.ListName == _selectedDataListInfo).ToList();
                ResultsListVisibility = Visibility.Collapsed;
            }
        }

        public List<CSTUserPreferenceInfo> _selectedSavedDataList;
        public List<CSTUserPreferenceInfo> SelectedSavedDataList
        {
            get { return _selectedSavedDataList; }
            set
            {
                _selectedSavedDataList = value;
                RaisePropertyChanged(() => this.SelectedSavedDataList);
                CSTNavigation.Update(CSTNavigationInfo.SelectedDataList, SelectedSavedDataList);
            }
        }

        private Visibility _dataListSelectionGridViewVisibility = Visibility.Collapsed;
        public Visibility DataListSelectionGridViewVisibility
        {
            get
            {
                return _dataListSelectionGridViewVisibility;
            }
            set
            {
                _dataListSelectionGridViewVisibility = value;
                RaisePropertyChanged(() => this.DataListSelectionGridViewVisibility);

            }
        }

        private Visibility _resultsListVisibility = Visibility.Collapsed;
        public Visibility ResultsListVisibility
        {
            get
            {
                return _resultsListVisibility;
            }
            set
            {
                _resultsListVisibility = value;
                RaisePropertyChanged(() => this.ResultsListVisibility);

            }
        }



        public ICommand OkCommand
        {
            get { return new DelegateCommand<object>(OkCommandMethod, OkCommandValidationMethod); }
        }

        public ICommand CreateDataListCommand
        {
            get { return new DelegateCommand<object>(CreateDataListCommandCommandMethod, CreateDataListCommandCommandValidationMethod); }
        }

        #endregion

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

        private List<CustomScreeningSecurityData> _securityData;
        public List<CustomScreeningSecurityData> SecurityData
        {
            get { return _securityData; }
            set
            {
                if (_securityData != value)
                {
                    _securityData = value;
                    RaisePropertyChanged(() => SecurityData);
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

        #endregion

        #region ICommand Methods

        private bool SubmitCommandValidationMethod(object param)
        {
            return true;
        }

        private void SubmitCommandMethod(object param)
        {
            //create list of selected options and save in the user class and fecth the securities based on filter selctions
            //SecuritySelectionGridViewVisibility = Visibility.Collapsed;
            //DataListSelectionGridViewVisibility = Visibility.Visible;

            DataListSelectionGridViewVisibility = Visibility.Visible;
            RaisePropertyChanged(() => this.DataListSelectionGridViewVisibility);

            if (_dbInteractivity == null || (UserSession.SessionManager.SESSION == null))
                return;
            else
            {
                BusyIndicatorNotification(true, "Retrieving User Preference Data...");
                _dbInteractivity.GetCustomScreeningUserPreferences(UserSession.SessionManager.SESSION.UserName, CustomScreeningUserPreferencesCallbackMethod);
            }
        }


        #region Data List Selector

        private bool OkCommandValidationMethod(object param)
        {
            //if (UserSession.SessionManager.SESSION == null
            //    || SelectedSavedDataList == null)
            //    return false;

            if (UserSession.SessionManager.SESSION == null)
                return false;

            //Check if user is creater of data list
            // bool userRoleValidation = UserSession.SessionManager.SESSION.UserName == SelectedSavedDataList.Presenter;


            // return userRoleValidation;
            return true;
        }

        private void OkCommandMethod(object param)
        {
            //if user the creater of datalist, prompt for edit of data fileds
            //if user says 'yes', data fileds selector screen appears with all data fields pre populated in selected fields list.
            //if user says'no' open resluts screen            
            
            if (UserSession.SessionManager.SESSION.UserName.ToLower().Equals(SelectedSavedDataList[0].UserName.ToLower()))
            {
                Prompt.ShowDialog("Do you wish to edit the data fields?", "Confirmation", MessageBoxButton.OKCancel, (result) =>
                {
                    if (result == MessageBoxResult.OK)
                    {
                        CSTNavigation.UpdateString(CSTNavigationInfo.Flag, "Edit");
                        //open pre populated selected fields list
                        _regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardCustomScreeningToolNewDataList", UriKind.Relative));
                    }
                    else if (result == MessageBoxResult.Cancel)
                    {
                        CSTNavigation.UpdateString(CSTNavigationInfo.Flag, "View");
                        //redirect to results views
                        PortfolioSelectionData p = new PortfolioSelectionData();
                        p = _portfolioSelectionData.Where(a => a.PortfolioId == SelectedPortfolio).FirstOrDefault();

                        EntitySelectionData b = new EntitySelectionData();
                        b = _benchmarkSelectionData.Where(a => a.LongName == SelectedBenchmark).FirstOrDefault();
                        _dbInteractivity.RetrieveSecurityData(p, b, SelectedRegion, SelectedCountry, SelectedSector, SelectedIndustry,
                                                                SelectedSavedDataList, RetrieveSecurityDataCallbackMethod);                    
                    }
                    else
                    {
                        CSTNavigation.UpdateString(CSTNavigationInfo.Flag, "View");
                        //redirect to results views
                    }
                });
            }
        }

        private bool CreateDataListCommandCommandValidationMethod(object param)
        {
            if (UserSession.SessionManager.SESSION == null)
                return false;
            else
                return true;
        }

        private void CreateDataListCommandCommandMethod(object param)
        {
            CSTNavigation.Update(CSTNavigationInfo.SelectedDataList, null);
            _regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardCustomScreeningToolNewDataList", UriKind.Relative));
        }

        #endregion

        #endregion

        #region EventUnSubscribe
        /// <summary>
        /// Method that disposes the events
        /// </summary>
        public void Dispose()
        {

        }

        #endregion

        #region Callback Methods
        private void PortfolioSelectionDataCallbackMethod(List<PortfolioSelectionData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);

            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result.ToString(), 1);
                    _portfolioSelectionData = result;
                    PortfolioSelectionInfo = result.Select(o => o.PortfolioId).ToList();

                    if (_dbInteractivity != null)
                    {
                        BusyIndicatorNotification(true, "Retrieving Data...");
                        _dbInteractivity.RetrieveEntitySelectionData(EntitySelectionDataCallbackMethod);
                    }
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
                BusyIndicatorNotification();
            }
            finally
            {
                Logging.LogEndMethod(_logger, methodNamespace);
                //BusyIndicatorNotification();
            }
        }

        private void EntitySelectionDataCallbackMethod(List<EntitySelectionData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);

            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result.ToString(), 1);

                    List<string> res = new List<string>();
                    _benchmarkSelectionData = result;
                    BenchmarkSelectionInfo = result.Where(a => a.Type.Equals("BENCHMARK")).Select(a => a.LongName).ToList();
                    //res = result.Where(a => a.Type.Equals("BENCHMARK")).Select(a => a.LongName).ToList();
                    //retrieve custom selection data
                    //RetrieveCustomSelectionData();
                    if (_dbInteractivity != null)
                    {
                        BusyIndicatorNotification(true, "Retrieving Data...");
                        _dbInteractivity.RetrieveCustomControlsList("Region", CustomControlsListRegionCallbackMethod);
                    }
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

        private void CustomControlsListRegionCallbackMethod(List<string> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    CustomSelectionRegionInfo = result;
                    if (_dbInteractivity != null)
                    {
                        BusyIndicatorNotification(true, "Retrieving Data...");
                        _dbInteractivity.RetrieveCustomControlsList("Country", CustomControlsListCountryCallbackMethod);
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
            finally
            {
                Logging.LogEndMethod(_logger, methodNamespace);
                BusyIndicatorNotification();
            }
        }

        private void CustomControlsListCountryCallbackMethod(List<string> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    CustomSelectionCountryInfo = result;
                    if (_dbInteractivity != null)
                    {
                        BusyIndicatorNotification(true, "Retrieving Data...");
                        _dbInteractivity.RetrieveCustomControlsList("Sector", CustomControlsListSectorCallbackMethod);
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
            finally
            {
                Logging.LogEndMethod(_logger, methodNamespace);
                BusyIndicatorNotification();
            }
        }

        private void CustomControlsListSectorCallbackMethod(List<string> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    CustomSelectionSectorInfo = result;
                    if (_dbInteractivity != null)
                    {
                        BusyIndicatorNotification(true, "Retrieving Data...");
                        _dbInteractivity.RetrieveCustomControlsList("Industry", CustomControlsListIndustryCallbackMethod);
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
            finally
            {
                Logging.LogEndMethod(_logger, methodNamespace);
                BusyIndicatorNotification();
            }
        }

        private void CustomControlsListIndustryCallbackMethod(List<string> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    CustomSelectionIndustryInfo = result;
                    BusyIndicatorNotification();
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
                BusyIndicatorNotification();
            }
            finally
            {
                Logging.LogEndMethod(_logger, methodNamespace);
                //BusyIndicatorNotification();
            }
        }

        private void RetrieveSecurityDataCallbackMethod(List<CustomScreeningSecurityData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    SecurityData = result;
                    ResultsListVisibility = Visibility.Visible;
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
            finally
            {
                Logging.LogEndMethod(_logger, methodNamespace);
                //BusyIndicatorNotification();
            }
        }

        private void CustomScreeningUserPreferencesCallbackMethod(List<CSTUserPreferenceInfo> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    CSTUserPreference = result;

                    SavedDataListInfo = (from res in result select new { ListName = res.ListName }).AsEnumerable().Select(t => t.ListName).Distinct()
                                  .ToList();
                    BusyIndicatorNotification();
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
                BusyIndicatorNotification();
            }
            finally
            {
                Logging.LogEndMethod(_logger, methodNamespace);
                //BusyIndicatorNotification();
            }
        }

        #endregion

        #region Helpers

        //public void RetrieveCustomSelectionData()
        //{
        //    if (_dbInteractivity != null)
        //    {
        //        // BusyIndicatorNotification(true, "Retrieving Region Selection Data...");
        //        //_dbInteractivity.RetrieveCustomControlsList("Region", CustomControlsListRegionCallbackMethod);
        //        //BusyIndicatorNotification(true, "Retrieving Country Selection Data...");
        //        //_dbInteractivity.RetrieveCustomControlsList("Country", CustomControlsListCountryCallbackMethod);
        //        //BusyIndicatorNotification(true, "Retrieving Sector Selection Data...");
        //        //_dbInteractivity.RetrieveCustomControlsList("Sector", CustomControlsListSectorCallbackMethod);
        //        //BusyIndicatorNotification(true, "Retrieving Industry Selection Data...");
        //        //_dbInteractivity.RetrieveCustomControlsList("Industry", CustomControlsListIndustryCallbackMethod);

        //    }
        //}

        public void Initialize()
        {
            SelectionRaisePropertyChanged();

            CSTNavigation.UpdateString(CSTNavigationInfo.Accessibility, null);
            CSTNavigation.UpdateString(CSTNavigationInfo.Flag, null);
            CSTNavigation.UpdateString(CSTNavigationInfo.ListName, null);
            //CSTNavigation.Update(CSTNavigationInfo.SelectedDataList, null);

            if (_dbInteractivity != null && IsActive)
            {
                SecuritySelectionGridViewVisibility = Visibility.Visible;
                DataListSelectionGridViewVisibility = Visibility.Collapsed;
                BusyIndicatorNotification(true, "Retrieving Data...");
                //fetch PortfolioId list 
                _dbInteractivity.RetrievePortfolioSelectionData(PortfolioSelectionDataCallbackMethod);
                //fetch final list of securities
                RetrieveResultsList();
            }
            
        }

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

        public void BusyIndicatorNotification(bool showBusyIndicator = false, String message = null)
        {
            if (message != null)
                BusyIndicatorContent = message;

            BusyIndicatorIsBusy = showBusyIndicator;
        }

        public void RetrieveResultsList()
        {
            SelectedSavedDataList = CSTNavigation.Fetch(CSTNavigationInfo.SelectedDataList) as List<CSTUserPreferenceInfo>;
            if (SelectedSavedDataList != null)
            {
                PortfolioSelectionData p = new PortfolioSelectionData();
                p = _portfolioSelectionData.Where(a => a.PortfolioId == SelectedPortfolio).FirstOrDefault();

                EntitySelectionData b = new EntitySelectionData();
                b = _benchmarkSelectionData.Where(a => a.LongName == SelectedBenchmark).FirstOrDefault();
                _dbInteractivity.RetrieveSecurityData(p, b, SelectedRegion, SelectedCountry, SelectedSector, SelectedIndustry,
                                                        SelectedSavedDataList, RetrieveSecurityDataCallbackMethod);
            }
        }
        #endregion
    }
}

