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

namespace GreenField.Gadgets.ViewModels
{
    public class ViewModelCustomScreeningTool :  NotificationObject
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
        public ViewModelCustomScreeningTool(DashboardGadgetParam param)
        {
            _logger = param.LoggerFacade;
            _dbInteractivity = param.DBInteractivity;
            _eventAggregator = param.EventAggregator;          
        }
        #endregion

        #region Properties
        public List<String> SecuritySelectionInfo
        {
            get
            {
                { return new List<String> { "Portfolio", "Benchmark", "Custom"}; }
            }
        }

        public List<String> PortfolioSelectionInfo
        {
            get
            {
                { return new List<String> { "Portfolio1", "Portfolio2", "Portfolio3" }; }
            }
        }

        public List<String> BenchmarkSelectionInfo
        {
            get
            {
                { return new List<String> { "Benchmark1", "Benchmark2", "Benchmark3" }; }
            }
        }

        public List<String> CustomSelectionInfo // to be changed to a complex type with all regions,sectors,industry, country
        {
            get
            {
                { return new List<String> { "Region1", "Region2", "Region3" }; }
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
                        RaisePropertyChanged(() => this.PortfolioSelectionVisibility);
                    }
                    else if (SelectedCriteria == SecuritySelectionType.BENCHMARK)
                    {
                        BenchmarkSelectionVisibility = Visibility.Visible;
                        RaisePropertyChanged(() => this.BenchmarkSelectionVisibility);
                    }
                    else if (SelectedCriteria == SecuritySelectionType.CUSTOM)
                    {
                        CustomSelectionVisibility = Visibility.Visible;
                        RaisePropertyChanged(() => this.CustomSelectionVisibility);
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
                    RaisePropertyChanged(() =>this.SelectedPortfolio);
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
                _portfolioSelectionVisibility = Visibility.Visible;
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
                _benchmarkSelectionVisibility = Visibility.Visible;
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
                _customSelectionVisibility = Visibility.Visible;
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
            }
        }

        public ICommand SubmitCommand
        {
            get { return new DelegateCommand<object>(SubmitCommandMethod, SubmitCommandValidationMethod); }
        }


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
            if (UserSession.SessionManager.SESSION == null
                || SelectedSavedDataList == null)
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
    }
}
