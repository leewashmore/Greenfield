using System;
using System.Windows;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Prism.Events;
using GreenField.ServiceCaller;
using Microsoft.Practices.Prism.Logging;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using GreenField.Gadgets.Helpers;
using GreenField.Common;
using System.Linq;
using GreenField.DataContracts;
using GreenField.ServiceCaller.ExternalResearchDefinitions;
using System.Windows.Data;

namespace GreenField.Gadgets.ViewModels
{
    /// <summary>
    /// View-Model for ExcelModelUpload
    /// </summary>
    public class ViewModelExcelModelUpload : NotificationObject
    {
        #region PrivateVariables

        /// <summary>
        /// Instance of EventAggregator
        /// </summary>
        private IEventAggregator _eventAggregator;

        /// <summary>
        /// Instance of IDbInteractivity
        /// </summary>
        private IDBInteractivity _dbInteractivity;

        /// <summary>
        /// Instance of IloggerFacade
        /// </summary>
        private ILoggerFacade _logger;
        public ILoggerFacade Logger
        {
            get
            {
                return _logger;
            }
            set
            {
                _logger = value;
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor that initialises the Class
        /// </summary>
        public ViewModelExcelModelUpload(DashboardGadgetParam param)
        {
            this._dbInteractivity = param.DBInteractivity;
            this._eventAggregator = param.EventAggregator;
            this._logger = param.LoggerFacade;

            if (SelectionData.EntitySelectionData != null && SeriesReferenceSource == null)
            {
                RetrieveEntitySelectionDataCallBackMethod(SelectionData.EntitySelectionData.Where(a => a.Type == "SECURITY").ToList());
            }
            else
            {
                if (_dbInteractivity != null)
                {
                    _dbInteractivity.RetrieveEntitySelectionData(RetrieveEntitySelectionDataCallBackMethod);
                }
            }
        }

        #endregion

        #region PropertyDeclaration

        /// <summary>
        /// Selected Security
        /// </summary>
        private EntitySelectionData _selectedSecurity;
        public EntitySelectionData SelectedSecurity
        {
            get
            {
                return _selectedSecurity;
            }
            set
            {
                _selectedSecurity = value;
                HandleSecurityReferenceSet();
                this.RaisePropertyChanged(() => this.SelectedSecurity);
            }
        }

        #region Issuer Details

        /// <summary>
        /// Stores Issuer related data
        /// </summary>
        /// 
        private IssuerReferenceData _issuerReferenceInfo;
        public IssuerReferenceData IssuerReferenceInfo
        {
            get { return _issuerReferenceInfo; }
            set
            {
                if (_issuerReferenceInfo != value)
                {
                    _issuerReferenceInfo = value;
                    this.RaisePropertyChanged(() => this.IssuerReferenceInfo);
                }
            }
        }

        #endregion

        #region Busy Indicator
        /// <summary>
        /// Busy Indicator Status
        /// </summary>
        private bool _busyIndicatorIsBusy;
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
        /// Busy Indicator Content
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

        #region DashboardActiveStatus


        #endregion

        #region Plotting Additional Series
        /// <summary>
        /// Grouped Collection View for Auto-Complete Box
        /// </summary>
        private CollectionViewSource _seriesReference;
        public CollectionViewSource SeriesReference
        {
            get
            {
                return _seriesReference;
            }
            set
            {
                _seriesReference = value;
                RaisePropertyChanged(() => this.SeriesReference);
            }
        }

        /// <summary>
        /// DataSource for the Grouped Collection View
        /// </summary>
        public ObservableCollection<EntitySelectionData> SeriesReferenceSource { get; set; }

        /// <summary>
        /// Selected Entity
        /// </summary>
        private EntitySelectionData _selectedSeriesReference = new EntitySelectionData();
        public EntitySelectionData SelectedSeriesReference
        {
            get
            {
                return _selectedSeriesReference;
            }
            set
            {
                _selectedSeriesReference = value;
                this.RaisePropertyChanged(() => this.SelectedSeriesReference);
            }
        }

        /// <summary>
        /// Search Mode Filter - Checked (StartsWith); Unchecked (Contains)
        /// </summary>
        private bool _searchFilterEnabled;
        public bool SearchFilterEnabled
        {
            get { return _searchFilterEnabled; }
            set
            {
                if (_searchFilterEnabled != value)
                {
                    _searchFilterEnabled = value;
                    RaisePropertyChanged(() => SearchFilterEnabled);
                }
            }
        }

        /// <summary>
        /// Entered Text in the Auto-Complete Box - filters SeriesReferenceSource
        /// </summary>
        private string _seriesEnteredText;
        public string SeriesEnteredText
        {
            get { return _seriesEnteredText; }
            set
            {
                _seriesEnteredText = value;
                RaisePropertyChanged(() => this.SeriesEnteredText);
                if (value != null)
                    SeriesReference.Source = SearchFilterEnabled == false
                        ? SeriesReferenceSource.Where(o => o.ShortName.ToLower().Contains(value.ToLower()))
                        : SeriesReferenceSource.Where(o => o.ShortName.ToLower().StartsWith(value.ToLower()));
                else
                    SeriesReference.Source = SeriesReferenceSource;
            }
        }

        /// <summary>
        /// Type of entites added to chart
        /// if true:Commodity/Index/Currency Added
        /// if false:only securities added 
        /// </summary>
        private bool _chartEntityTypes = true;
        public bool ChartEntityTypes
        {
            get
            {
                return _chartEntityTypes;
            }
            set
            {
                _chartEntityTypes = value;
                this.RaisePropertyChanged(() => this.ChartEntityTypes);
            }
        }

        #endregion

        private bool _enableDownload;
        public bool EnableDownload
        {
            get
            {
                return _enableDownload;
            }
            set
            {
                _enableDownload = value;
                this.RaisePropertyChanged(() => this.EnableDownload);
            }
        }

        private byte[] _modelWorkbook;
        public byte[] ModelWorkbook
        {
            get
            {
                return _modelWorkbook;
            }
            set
            {
                _modelWorkbook = value;
                this.RaisePropertyChanged(() => this.ModelWorkbook);
            }
        }



        #endregion

        #region EventHandlers

        /// <summary>
        /// Event Handler to subscribed event 'SecurityReferenceSet'
        /// </summary>
        /// <param name="securityReferenceData">SecurityReferenceData</param>
        public void HandleSecurityReferenceSet()
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                //ArgumentNullException

                if (SelectedSecurity != null)
                {
                    EnableDownload = false;
                    BusyIndicatorNotification(true, "Retrieving Issuer Details based on selected security");
                    _dbInteractivity.RetrieveDocumentsData(SelectedSecurity, RetrieveDocumentsDataCallbackMethod);
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
        }

        #endregion

        #region CallbackMethods

        /// <summary>
        /// Callback method for Entity Reference Service call - Updates AutoCompleteBox
        /// </summary>
        /// <param name="result">EntityReferenceData Collection</param>
        public void RetrieveEntitySelectionDataCallBackMethod(List<EntitySelectionData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    SeriesReference = new CollectionViewSource();
                    SeriesReferenceSource = new ObservableCollection<EntitySelectionData>(result.Where(a => a.Type == "SECURITY").ToList());
                    SeriesReference.GroupDescriptions.Add(new PropertyGroupDescription("Type"));
                    SeriesReference.SortDescriptions.Add(new System.ComponentModel.SortDescription
                    {
                        PropertyName = "SortOrder",
                        Direction = System.ComponentModel.ListSortDirection.Ascending
                    });
                    SeriesReference.SortDescriptions.Add(new System.ComponentModel.SortDescription
                    {
                        PropertyName = "LongName",
                        Direction = System.ComponentModel.ListSortDirection.Ascending
                    });
                    SeriesReference.Source = SeriesReferenceSource;
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
        }

        /// <summary>
        /// Callback Method for FileStream
        /// </summary>
        /// <param name="result"></param>
        public void RetrieveDocumentsDataCallbackMethod(byte[] result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    ModelWorkbook = result;
                    EnableDownload = true;
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
                BusyIndicatorNotification();
            }
        }

        #endregion

        #region HelperMethods

        /// <summary>
        /// Busy Indicator Notification
        /// </summary>
        /// <param name="showBusyIndicator"></param>
        /// <param name="message"></param>
        public void BusyIndicatorNotification(bool showBusyIndicator = false, String message = null)
        {
            if (message != null)
                BusyIndicatorContent = message;
            BusyIndicatorIsBusy = showBusyIndicator;
        }

        #endregion

    }
}
