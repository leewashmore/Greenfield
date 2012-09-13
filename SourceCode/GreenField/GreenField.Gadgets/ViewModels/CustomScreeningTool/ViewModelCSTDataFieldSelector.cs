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
using GreenField.DataContracts.DataContracts;

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
       
        #endregion

        #region Constructor
        public ViewModelCSTDataFieldSelector(DashboardGadgetParam param)
        {
            _logger = param.LoggerFacade;
            _dbInteractivity = param.DBInteractivity;
            _eventAggregator = param.EventAggregator;      

            //fetch tabs data
            FetchTabsData();
        }
      
        #endregion

        #region Properties

        public static List<CustomSelectionData> SelectedFieldsList { get; set; }

        private List<CustomSelectionData> _selectedFieldsOverviewInfo;

        public List<CustomSelectionData> SelectedFieldsOverviewInfo
        {
            get { return _selectedFieldsOverviewInfo; }
            set
            {
                _selectedFieldsOverviewInfo = value;
                RaisePropertyChanged(() => this.SelectedFieldsOverviewInfo);
            }
        }
        
       
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
                //if (value)
                //{
                //    Initialize();
                //}
            }
        }

        #region ICommand Properties

        public ICommand AddCommand
        {
            get { return new DelegateCommand<object>(AddCommandMethod, AddCommandValidationMethod); }
        }

        public ICommand SubmitCommand
        {
            get { return new DelegateCommand<object>(SubmitCommandMethod, SubmitCommandValidationMethod); }
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
            SelectedFieldsList = new List<CustomSelectionData>();
            SelectedFieldsList.Add(SelectedSecurityReferenceData);
            SelectedFieldsOverviewInfo = SelectedFieldsList;            
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
                               //save the list

                                if (_dbInteractivity != null)
                                {
                                    //BusyIndicatorNotification(true, "Updating data list...");
                                   //update the data list.. make a call to service method to update
                                }
                            }
                        });
                }
            };
        }

        #endregion

        #region Helpers

        public void FetchTabsData()
        {
            if (_dbInteractivity != null)
            {
                _dbInteractivity.RetrieveSecurityReferenceTabDataPoints(SecurityReferenceTabDataPointsCallbackMethod);
                _dbInteractivity.RetrievePeriodFinancialsTabDataPoints(PeriodFinancialsTabDataPointsCallbackMethod);
                _dbInteractivity.RetrieveCurrentFinancialsTabDataPoints(CurrentFinancialsTabDataPointsCallbackMethod);
                _dbInteractivity.RetrieveFairValueTabDataPoints(FairValueTabDataPointsCallbackMethod);
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
            Logging.LogEndMethod(_logger, methodNamespace);
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
            Logging.LogEndMethod(_logger, methodNamespace);
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
            Logging.LogEndMethod(_logger, methodNamespace);
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
            Logging.LogEndMethod(_logger, methodNamespace);
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
    }
}
