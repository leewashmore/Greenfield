using System;
using System.Windows;
using System.Linq;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Logging;
using GreenField.Gadgets.Helpers;
using GreenField.Common;
using GreenField.DataContracts;
using GreenField.ServiceCaller.ExternalResearchDefinitions;
using GreenField.ServiceCaller;
using System.Diagnostics;
using Telerik.Windows.Documents.Model;

namespace GreenField.Gadgets.ViewModels
{
   
    /// <summary>
    /// View-Model for ModelInvestmentContext
    /// </summary>
    public class ViewModelInvestmentContext: NotificationObject
    {
        #region PrivateVariables

        /// <summary>
        /// Instance of EventAggregator
        /// </summary>
        private IEventAggregator eventAggregator;

        /// <summary>
        /// Instance of IDbInteractivity
        /// </summary>
        private IDBInteractivity dbInteractivity;

        /// <summary>
        /// Instance of IloggerFacade
        /// </summary>
        private ILoggerFacade logger;
        public ILoggerFacade Logger
        {
            get
            {
                return logger;
            }
            set
            {
                logger = value;
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor that initialises the Class
        /// </summary>
        public ViewModelInvestmentContext(DashboardGadgetParam param)
        {
            this.dbInteractivity = param.DBInteractivity;
            this.eventAggregator = param.EventAggregator;
            this.logger = param.LoggerFacade;
            //this.UploadResults = new ObservableCollection<UploadResult>();
            //this.UploadResultsVisibility = Visibility.Collapsed;
            if (SelectionData.EntitySelectionData != null && SeriesReferenceSource == null)
            {
                RetrieveEntitySelectionDataCallBackMethod(SelectionData.EntitySelectionData.Where(a => a.Type == "SECURITY").ToList());
            }
            else
            {
                if (dbInteractivity != null)
                {
                    dbInteractivity.RetrieveEntitySelectionData(RetrieveEntitySelectionDataCallBackMethod);
                }
            }
        }

        

        #endregion

        #region PropertyDeclaration
        
        /// <summary>
        /// Selected Security
        /// </summary>
        private EntitySelectionData selectedSecurity;
        public EntitySelectionData SelectedSecurity
        {
            get
            {
                return selectedSecurity;
            }
            set
            {
                selectedSecurity = value;
                //EnableDownload = true;
                DownloadInvestmentContext();
                this.RaisePropertyChanged(() => this.SelectedSecurity);
            }
        }

        /// <summary>
        /// Stores the list of EntitySelectionData for all entity Types
        /// </summary>
        private List<EntitySelectionData> entitySelectionInfo;
        public List<EntitySelectionData> EntitySelectionInfo
        {
            get
            {
                if (entitySelectionInfo == null)
                    entitySelectionInfo = new List<EntitySelectionData>();
                return entitySelectionInfo;
            }
            set
            {
                entitySelectionInfo = value;
                RaisePropertyChanged(() => this.EntitySelectionInfo);

                SecuritySelectorInfo = value
                    .Where(record => record.Type == EntityType.SECURITY)
                    .ToList();
            }
        }

        /// <summary>
        /// Stores the list of EntitySelectionData for entity type - SECURITY
        /// </summary>
        private List<EntitySelectionData> securitySelectorInfo;
        public List<EntitySelectionData> SecuritySelectorInfo
        {
            get { return securitySelectorInfo; }
            set
            {
                securitySelectorInfo = value;
                RaisePropertyChanged(() => this.SecuritySelectorInfo);
            }
        }

        /// <summary>
        /// Stores search text entered by user - Refines SecuritySelectionInfo based on the text entered
        /// </summary>
        private string securitySearchText;
        public string SecuritySearchText
        {
            get { return securitySearchText; }
            set
            {
                securitySearchText = value;
                RaisePropertyChanged(() => this.SecuritySearchText);
                if (value != null)
                {
                    if (value != String.Empty && EntitySelectionInfo != null)
                        SecuritySelectorInfo = EntitySelectionInfo
                                    .Where(
                                    record => record.Type == EntityType.SECURITY &&
                                    (record.LongName.ToLower().Contains(value.ToLower())
                                        || record.ShortName.ToLower().Contains(value.ToLower())
                                        || record.InstrumentID.ToLower().Contains(value.ToLower())))
                                    .ToList();
                    else
                        SecuritySelectorInfo = EntitySelectionInfo.Where(record => record.Type == EntityType.SECURITY).ToList();
                }
            }
        }

        private static int fontSizePDF = 10;
        private string context = null;
        public string Context
        {
            get { return context; }
            set { context = value; }
        }

        #region Issuer Details

        /// <summary>
        /// Stores Issuer related data
        /// </summary>
        /// 
        private IssuerReferenceData issuerReferenceInfo;
        public IssuerReferenceData IssuerReferenceInfo
        {
            get { return issuerReferenceInfo; }
            set
            {
                if (issuerReferenceInfo != value)
                {
                    issuerReferenceInfo = value;
                    this.RaisePropertyChanged(() => this.IssuerReferenceInfo);
                }
            }
        }

        #endregion

        #region Busy Indicator
        /// <summary>
        /// Busy Indicator Status
        /// </summary>
        private bool busyIndicatorIsBusy;
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
        /// Busy Indicator Content
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

        #region SecurityComboBox
        /// <summary>
        /// Grouped Collection View for Auto-Complete Box
        /// </summary>
        private CollectionViewSource seriesReference;
        public CollectionViewSource SeriesReference
        {
            get
            {
                return seriesReference;
            }
            set
            {
                seriesReference = value;
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
        private EntitySelectionData selectedSeriesReference = new EntitySelectionData();
        public EntitySelectionData SelectedSeriesReference
        {
            get
            {
                return selectedSeriesReference;
            }
            set
            {
                selectedSeriesReference = value;
                this.RaisePropertyChanged(() => this.SelectedSeriesReference);
            }
        }

        /// <summary>
        /// Search Mode Filter - Checked (StartsWith); Unchecked (Contains)
        /// </summary>
        private bool searchFilterEnabled;
        public bool SearchFilterEnabled
        {
            get { return searchFilterEnabled; }
            set
            {
                if (searchFilterEnabled != value)
                {
                    searchFilterEnabled = value;
                    RaisePropertyChanged(() => SearchFilterEnabled);
                }
            }
        }

        /// <summary>
        /// Entered Text in the Auto-Complete Box - filters SeriesReferenceSource
        /// </summary>
        private string seriesEnteredText;
        public string SeriesEnteredText
        {
            get { return seriesEnteredText; }
            set
            {
                seriesEnteredText = value;
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
        private bool chartEntityTypes = true;
        public bool ChartEntityTypes
        {
            get
            {
                return chartEntityTypes;
            }
            set
            {
                chartEntityTypes = value;
                this.RaisePropertyChanged(() => this.ChartEntityTypes);
            }
        }

        #endregion

        /// <summary>
        /// Enable downloading the sheet
        /// </summary>
        private bool enableDownload;
        public bool EnableDownload
        {
            get
            {
                return enableDownload;
            }
            set
            {
                enableDownload = value;
                this.RaisePropertyChanged(() => this.EnableDownload);
            }
        }

        /// <summary>
        /// Byte Stream of Model-Download workbook
        /// </summary>


        private List<InvestmentContextDetailsData> investmentContextDataInfo;
        public List<InvestmentContextDetailsData> InvestmentContextDataInfo
        {
            get
            {
                if (investmentContextDataInfo == null)
                {
                    investmentContextDataInfo = new List<InvestmentContextDetailsData>();
                }
                return investmentContextDataInfo;
            }
            set
            {
                if (investmentContextDataInfo != value)
                {
                   investmentContextDataInfo = value;
                 //  this.RaisePropertyChanged(() => this.InvestmentContextDataInfo);
                }
            }
        }


        #endregion

        #region EventHandlers


        public void DownloadInvestmentContext()
        {
            if(selectedSecurity != null)
            {
            EnableDownload = false;
            context = "Both";
            dbInteractivity.RetrieveInvestmentContextData(SelectedSecurity.IssuerId, context, DownloadInvestmentContextCallbackMethod);
            BusyIndicatorNotification(true,"Retrieve Investment Context Data");
            }
            //RadDocument doc = GenerateRadDocument();
            
        }


        public void DownloadInvestmentContextCallbackMethod(List<InvestmentContextDetailsData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result.ToString(), 1);
                    InvestmentContextDataInfo = result;
                    EnableDownload = true;
                    BusyIndicatorNotification();

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
                BusyIndicatorNotification();
            }
        }


        /*private void ScrubData()
        {
            var DistinctDataId = investmentContextDataInfo.Select(a => a.DataId).Distinct().ToList();
            List<SecurityDataIdScrub> securityDataIdScrub = new List<SecurityDataIdScrub>();
            foreach (InvestmentContextData icd in investmentContextDataInfo)
            {
                securityDataIdScrub.Add(new SecurityDataIdScrub()
                {
                    DataId = icd.DataId,
                    SecurityId = icd.SecurityId,
                    GICS_Industry  = icd.gics_industry,
                    GICS_Industry_Name = icd.gics_industry_name,
                    GICS_Sector = icd.gics_sector,
                    GICS_Sector_Name = icd.gics_sector_name,
                    OriginalValue = icd.value
                });
            }
            DataScrubber ds = new DataScrubber();
                    ds.DoScrubbing(securityDataIdScrub, DistinctDataId[1], "Range");


        }*/

     

        #endregion

        #region CallbackMethods

        /// <summary>
        /// Callback method for Entity Reference Service call - Updates AutoCompleteBox
        /// </summary>
        /// <param name="result">EntityReferenceData Collection</param>
        public void RetrieveEntitySelectionDataCallBackMethod(List<EntitySelectionData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result.ToString(), 1);
                    EntitySelectionInfo = result.OrderBy(t => t.LongName).ToList();
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
                BusyIndicatorNotification();
            }
        }

      /*  /// <summary>
        /// Callback Method for FileStream
        /// </summary>
        /// <param name="result"></param>
        public void RetrieveDocumentsDataCallbackMethod(byte[] result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    ModelWorkbook = result;
                    EnableDownload = true;
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
                BusyIndicatorNotification();
            }
        }

        private int fileCount = 0;
        /// <summary>
        /// callback Method for ModelWorkbook
        /// </summary>
        public void RetrieveModelWorkbookUploadCallbackMethod(string fileName, string message)
        {
            fileCount--;
            var item = new UploadResult { FileName = fileName, Message = message};
            this.UploadResults.Add(item);
            
            this.UploadResultsVisibility = Visibility.Visible;
            try
            {
                Prompt.ShowDialog(message, fileName);
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            finally
            {
                if (fileCount == 0)
                {
                    BusyIndicatorNotification();
                    //this.RaisePropertyChanged(() => this.UploadResults);
                }
            }
        }
        */
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
            {
                BusyIndicatorContent = message;
            }
            BusyIndicatorIsBusy = showBusyIndicator;
        }
        
        #endregion


        
    }
}
