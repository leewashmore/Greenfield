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

            BusyIndicatorNotification(true, "Retrieving Portfolio Selection Data...");
            //fetch PortfolioId list 
            _dbInteractivity.RetrievePortfolioSelectionData(PortfolioSelectionDataCallbackMethod);

            BusyIndicatorNotification(true, "Retrieving Benchmark Selection Data...");
            //fetch Benchmark list
            _dbInteractivity.RetrieveEntitySelectionData(EntitySelectionDataCallbackMethod);

            //retrieve custom selection data
            RetrieveCustomSelectionData();

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
                if (value != null)
                {
                    _securitySelectionCriteria = value;
                    RaisePropertyChanged(() => this.SecuritySelectionCriteria);
                }
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
                if (value != null)
                {
                    _portfolioSelectionInfo = value;
                    RaisePropertyChanged(() => this.PortfolioSelectionInfo);
                }
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
                if (value != null)
                {
                    _benchmarkSelectionInfo = value;
                    RaisePropertyChanged(() => this.BenchmarkSelectionInfo);
                }
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
                if (value != null)
                {
                    _customSelectionRegionInfo = value;
                    RaisePropertyChanged(() => this.CustomSelectionRegionInfo);
                }
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
                if (value != null)
                {
                    _customSelectionSectorInfo = value;
                    RaisePropertyChanged(() => this.CustomSelectionSectorInfo);
                }
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
                if (value != null)
                {
                    _customSelectionCountryInfo = value;
                    RaisePropertyChanged(() => this.CustomSelectionCountryInfo);
                }
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
                if (value != null)
                {
                    _customSelectionIndustryInfo = value;
                    RaisePropertyChanged(() => this.CustomSelectionIndustryInfo);
                }
            }
        }

        public String _selectedCriteria;
        public String SelectedCriteria
        {

            get { return _selectedCriteria; }
            set
            {
                if (value != null)
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
                    PortfolioSelectionData p = new PortfolioSelectionData();
                    p = _portfolioSelectionData.Where(a => a.PortfolioId == _selectedPortfolio).FirstOrDefault();
                    _dbInteractivity.RetrieveSecurityData(p, null, SelectedRegion, SelectedCountry, SelectedSector, SelectedIndustry
                                                       , null, RetrieveSecurityDataCallbackMethod);
                }
            }
        }

        public String _selectedBenchmark;
        public String SelectedBenchmark
        {
            get { return _selectedBenchmark; }
            set
            {
                if (value != null)
                {
                    _selectedBenchmark = value;
                    RaisePropertyChanged(() => this.SelectedBenchmark);
                }
            }
        }

        public String _selectedRegion;
        public String SelectedRegion
        {
            get { return _selectedRegion; }
            set
            {
                if (value != null)
                {
                    _selectedRegion = value;
                    RaisePropertyChanged(() => this.SelectedRegion);
                }
            }
        }

        public String _selectedCountry;
        public String SelectedCountry
        {
            get { return _selectedCountry; }
            set
            {
                if (value != null)
                {
                    _selectedCountry = value;
                    RaisePropertyChanged(() => this.SelectedCountry);
                }
            }
        }

        public String _selectedSector;
        public String SelectedSector
        {
            get { return _selectedSector; }
            set
            {
                if (value != null)
                {
                    _selectedSector = value;
                    RaisePropertyChanged(() => this.SelectedSector);
                }
            }
        }

        public String _selectedIndustry;
        public String SelectedIndustry
        {
            get { return _selectedIndustry; }
            set
            {
                if (value != null)
                {
                    _selectedIndustry = value;
                    RaisePropertyChanged(() => this.SelectedIndustry);
                }
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

        public List<String> SavedDataListInfo
        {
            get
            {
                { return new List<String> { "DataList1", "DataList2", "DataList3" }; }
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

        public String _selectedSavedDataList;
        public String SelectedSavedDataList
        {
            get { return _selectedSavedDataList; }
            set
            {
                if (value != null)
                {
                    _selectedSavedDataList = value;
                    RaisePropertyChanged(() => this.SelectedSavedDataList);
                }
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
                //if (value)
                //{
                //    Initialize();
                //}
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
            SecuritySelectionGridViewVisibility = Visibility.Collapsed;
            DataListSelectionGridViewVisibility = Visibility.Visible;
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
            //if user says 'yes', data fileds selector screen appears with all data fileds pre populated in selected fields list.
            //if user says'no' open resluts screen

            Prompt.ShowDialog("Do you wish to edit the data fields?", "Confirmation", MessageBoxButton.OK, (result) =>
            {
                if (result == MessageBoxResult.OK)
                {
                    //open pre populated selected fields list
                }
                else
                {
                    //display results screen
                }

            });
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
                    BenchmarkSelectionInfo = result.Where(a => a.Type.Equals("BENCHMARK")).Select(a => a.LongName).ToList();
                    //res = result.Where(a => a.Type.Equals("BENCHMARK")).Select(a => a.LongName).ToList();

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

        #endregion

        #region Helpers

        public void RetrieveCustomSelectionData()
        {
            if (_dbInteractivity != null)
            {
                 BusyIndicatorNotification(true, "Retrieving Region Selection Data...");
                _dbInteractivity.RetrieveCustomControlsList("Region", CustomControlsListRegionCallbackMethod);
                BusyIndicatorNotification(true, "Retrieving Country Selection Data...");
                _dbInteractivity.RetrieveCustomControlsList("Country", CustomControlsListCountryCallbackMethod);
                BusyIndicatorNotification(true, "Retrieving Sector Selection Data...");
                _dbInteractivity.RetrieveCustomControlsList("Sector", CustomControlsListSectorCallbackMethod);
                BusyIndicatorNotification(true, "Retrieving Industry Selection Data...");
                _dbInteractivity.RetrieveCustomControlsList("Industry", CustomControlsListIndustryCallbackMethod);
               
            }
        }

        public void BusyIndicatorNotification(bool showBusyIndicator = false, String message = null)
        {
            if (message != null)
                BusyIndicatorContent = message;

            BusyIndicatorIsBusy = showBusyIndicator;
        }
        #endregion
    }
}
 
