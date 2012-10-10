using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Xml.Linq;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.ViewModel;
using GreenField.Common;
using GreenField.DataContracts;
using GreenField.ServiceCaller;
using GreenField.ServiceCaller.ExternalResearchDefinitions;

namespace GreenField.Gadgets.ViewModels
{
    /// <summary>
    /// View Model for ViewScatterGraph
    /// </summary>
    public class ViewModelScatterGraph : NotificationObject
    {
        #region Fields
        /// <summary>
        /// Event Aggregator MEF instance
        /// </summary>
        private IEventAggregator eventAggregator;

        /// <summary>
        /// Service Caller MEF instance
        /// </summary>
        private IDBInteractivity dbInteractivity;

        /// <summary>
        /// Logging MEF instance
        /// </summary>
        private ILoggerFacade logger;

        /// <summary>
        /// Scatter chart defaults
        /// </summary>
        private ScatterChartDefaults scatterChartDefault;
        #endregion

        #region Properties
        #region IsActive
        /// <summary>
        /// Stores true if view is displayed on registered region
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
                    if (EntitySelectionInfo != null)
                    {
                        HandleSecurityReferenceSetEvent(EntitySelectionInfo);
                    }
                }
            }
        }
        #endregion

        #region Ratio Comparison Data
        /// <summary>
        /// Stores ratio comparison data for securities in requisite context
        /// </summary>
        private List<RatioComparisonData> ratioComparisonInfo;
        public List<RatioComparisonData> RatioComparisonInfo
        {
            get { return ratioComparisonInfo; }
            set
            {
                ratioComparisonInfo = value;
                RaisePropertyChanged(() => this.RatioComparisonInfo);
                if (value != null)
                {
                    IssueRatioComparisonInfo = value.Where(record => record.ISSUE_NAME == EntitySelectionInfo.LongName).ToList();
                }
            }
        }

        /// <summary>
        /// Stores ratio comparison data for the selected security
        /// </summary>
        private List<RatioComparisonData> issueRatioComparisonInfo;
        public List<RatioComparisonData> IssueRatioComparisonInfo
        {
            get { return issueRatioComparisonInfo; }
            set
            {
                issueRatioComparisonInfo = value;
                RaisePropertyChanged(() => this.IssueRatioComparisonInfo);
                MissingSecurityDataNotificationVisibility = value != null
                    ? (value.Count == 1 ? Visibility.Collapsed : Visibility.Visible)
                    : Visibility.Visible;
            }
        }

        /// <summary>
        /// Stores visibility of the missing security data notifications
        /// </summary>
        private Visibility missingSecurityDataNotificationVisibility = Visibility.Collapsed;
        public Visibility MissingSecurityDataNotificationVisibility
        {
            get { return missingSecurityDataNotificationVisibility; }
            set
            {
                missingSecurityDataNotificationVisibility = value;
                RaisePropertyChanged(() => this.MissingSecurityDataNotificationVisibility);
            }
        }
        #endregion

        #region Issuer Details
        /// <summary>
        /// Stores issuer related data
        /// </summary>
        /// 
        private IssuerReferenceData issuerReferenceInfo;
        public IssuerReferenceData IssuerReferenceInfo
        {
            get { return issuerReferenceInfo; }
            set { issuerReferenceInfo = value; }
        }
        #endregion

        #region Expander Input
        #region Financial Ratio
        /// <summary>
        /// Stores ScatterGraphFinancialRatio Enum Items
        /// </summary>
        public List<ScatterGraphFinancialRatio> FinancialRatioInfo
        {
            get { return EnumUtils.GetEnumDescriptions<ScatterGraphFinancialRatio>(); }
        }

        /// <summary>
        /// Stores selected ScatterGraphFinancialRatio
        /// </summary>
        private ScatterGraphFinancialRatio selectedFinancialRatio = ScatterGraphFinancialRatio.REVENUE_GROWTH;
        public ScatterGraphFinancialRatio SelectedFinancialRatio
        {
            get { return selectedFinancialRatio; }
            set
            {
                if (selectedFinancialRatio != value)
                {
                    selectedFinancialRatio = value;
                    RaisePropertyChanged(() => this.SelectedFinancialRatio);
                    if (ContextSecurityInfo != null)
                    {
                        String contextSecurityXML = GetContextSecurityXML(ContextSecurityInfo);
                        if (dbInteractivity != null && IsActive)
                        {
                            BusyIndicatorNotification(true, "Retrieving ratio comparison data for the selected security...");
                            dbInteractivity.RetrieveRatioComparisonData(contextSecurityXML, RetrieveRatioComparisonDataCallbackMethod);
                        }
                    }
                }
            }
        }
        #endregion

        #region Valuation Ratio
        /// <summary>
        /// Stores ScatterGraphValuationRatio Enum Items
        /// </summary>
        public List<ScatterGraphValuationRatio> ValuationRatioInfo
        {
            get { return EnumUtils.GetEnumDescriptions<ScatterGraphValuationRatio>(); }
        }

        /// <summary>
        /// Stores selected ScatterGraphValuationRatio
        /// </summary>
        private ScatterGraphValuationRatio _selectedValuationRatio = ScatterGraphValuationRatio.PRICE_TO_REVENUE;
        public ScatterGraphValuationRatio SelectedValuationRatio
        {
            get { return _selectedValuationRatio; }
            set
            {
                if (_selectedValuationRatio != value)
                {
                    _selectedValuationRatio = value;
                    RaisePropertyChanged(() => this.SelectedValuationRatio);
                    if (ContextSecurityInfo != null)
                    {
                        String contextSecurityXML = GetContextSecurityXML(ContextSecurityInfo);
                        if (dbInteractivity != null && IsActive)
                        {
                            BusyIndicatorNotification(true, "Retrieving ratio comparison data for the selected security...");
                            dbInteractivity.RetrieveRatioComparisonData(contextSecurityXML, RetrieveRatioComparisonDataCallbackMethod);
                        }
                    }
                }
            }
        }
        #endregion

        #region Context
        /// <summary>
        /// Stores ScatterGraphContext Enum Items
        /// </summary>
        public List<ScatterGraphContext> ContextInfo
        {
            get { return EnumUtils.GetEnumDescriptions<ScatterGraphContext>(); }
        }

        /// <summary>
        /// Stores selected ScatterGraphContext
        /// </summary>
        private ScatterGraphContext _selectedContext = ScatterGraphContext.COUNTRY;
        public ScatterGraphContext SelectedContext
        {
            get { return _selectedContext; }
            set
            {
                if (_selectedContext != value)
                {
                    _selectedContext = value;
                    RaisePropertyChanged(() => this.SelectedContext);
                    if (dbInteractivity != null && IssuerReferenceInfo != null && IsActive)
                    {
                        BusyIndicatorNotification(true, "Retrieving ratio security reference data...");
                        dbInteractivity.RetrieveRatioSecurityReferenceData(value, IssuerReferenceInfo
                            , RetrieveRatioSecurityReferenceDataCallbackMethod);
                    }
                }
            }
        }
        #endregion

        #region Period
        /// <summary>
        /// Stores ScatterGraphPeriod Enum Items
        /// </summary>
        public List<ScatterGraphPeriod> PeriodInfo
        {
            get { return EnumUtils.GetEnumDescriptions<ScatterGraphPeriod>(); }
        }

        /// <summary>
        /// Stores selected ScatterGraphPeriod
        /// </summary>
        private ScatterGraphPeriod _selectedPeriod = ScatterGraphPeriod.FORWARD;
        public ScatterGraphPeriod SelectedPeriod
        {
            get { return _selectedPeriod; }
            set
            {
                if (_selectedPeriod != value)
                {
                    _selectedPeriod = value;
                    RaisePropertyChanged(() => this.SelectedPeriod);
                    if (ContextSecurityInfo != null)
                    {
                        String contextSecurityXML = GetContextSecurityXML(ContextSecurityInfo);
                        if (dbInteractivity != null && IsActive)
                        {
                            BusyIndicatorNotification(true, "Retrieving ratio comparison data for the selected security...");
                            dbInteractivity.RetrieveRatioComparisonData(contextSecurityXML, RetrieveRatioComparisonDataCallbackMethod);
                        }
                    }
                }
            }
        }
        #endregion
        #endregion

        #region Security Information
        /// <summary>
        /// Stores security information received from toolbox selection
        /// </summary>
        private EntitySelectionData entitySelectionInfo;
        public EntitySelectionData EntitySelectionInfo
        {
            get { return entitySelectionInfo; }
            set
            {
                entitySelectionInfo = value;
                RaisePropertyChanged(() => this.EntitySelectionInfo);
            }
        }

        /// <summary>
        /// Stores comprehensive security information received from toolbox selection
        /// </summary>
        public List<GF_SECURITY_BASEVIEW> ContextSecurityInfo { get; set; }
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
        #endregion
        #endregion

        #region Constructor
        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="param">DashboardGadgetParam</param>
        public ViewModelScatterGraph(DashboardGadgetParam param)
        {
            logger = param.LoggerFacade;
            dbInteractivity = param.DBInteractivity;
            eventAggregator = param.EventAggregator;
            EntitySelectionInfo = param.DashboardGadgetPayload.EntitySelectionData;
            scatterChartDefault = (ScatterChartDefaults)param.AdditionalInfo;
            SetScatterChartDefaults(scatterChartDefault);

            if (eventAggregator != null)
            {
                eventAggregator.GetEvent<SecurityReferenceSetEvent>().Subscribe(HandleSecurityReferenceSetEvent);
            }            
        } 
        #endregion        

        #region Event Handlers
        /// <summary>
        /// Event Handler for SecurityReferenceSet event
        /// </summary>
        /// <param name="result"></param>
        public void HandleSecurityReferenceSetEvent(EntitySelectionData result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null && IsActive)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result, 1);
                    EntitySelectionInfo = result;
                    if (EntitySelectionInfo != null)
                    {
                        BusyIndicatorNotification(true, "Retrieving Issuer Details based on selected security");
                        dbInteractivity.RetrieveIssuerReferenceData(result, RetrieveIssuerReferenceDataCallbackMethod);
                    }
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
            Logging.LogEndMethod(logger, methodNamespace);
        }
        #endregion

        #region Callback Methods
        /// <summary>
        /// RetrieveIssuerReferenceData callback method - assigns IssuerReferenceInfo and calls RetrieveFinancialStatementData
        /// </summary>
        /// <param name="result">IssuerReferenceData</param>
        public void RetrieveIssuerReferenceDataCallbackMethod(IssuerReferenceData result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    IssuerReferenceInfo = result;
                    if (dbInteractivity != null)
                    {
                        BusyIndicatorNotification(true, "Retrieving ratio security reference data...");
                        dbInteractivity.RetrieveRatioSecurityReferenceData(SelectedContext, result, RetrieveRatioSecurityReferenceDataCallbackMethod); 
                    }
                }
                else
                {
                    Prompt.ShowDialog("No Issuer linked to the entity " + EntitySelectionInfo.LongName 
                        + " (" + EntitySelectionInfo.ShortName + " : " + EntitySelectionInfo.InstrumentID + ")");
                    Logging.LogMethodParameterNull(logger, methodNamespace, 1);
                    BusyIndicatorNotification();
                }                
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }

        /// <summary>
        /// RetrieveRatioSecurityReferenceData callback method - assigns ContextSecurityInfo and calls RetrieveRatioComparisonData
        /// </summary>
        /// <param name="result">List of GF_SECURITY_BASEVIEW</param>
        public void RetrieveRatioSecurityReferenceDataCallbackMethod(List<GF_SECURITY_BASEVIEW> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    ContextSecurityInfo = result;
                    
                    String contextSecurityXML = GetContextSecurityXML(result);
                    if (dbInteractivity != null)
                    {
                        BusyIndicatorNotification(true, "Retrieving ratio comparison data for the selected security...");
                        dbInteractivity.RetrieveRatioComparisonData(contextSecurityXML, RetrieveRatioComparisonDataCallbackMethod); 
                    }                    
                }
                else
                {
                    Prompt.ShowDialog("No Issuer linked to the entity " + EntitySelectionInfo.LongName 
                        + " (" + EntitySelectionInfo.ShortName + " : " + EntitySelectionInfo.InstrumentID + ")");
                    Logging.LogMethodParameterNull(logger, methodNamespace, 1);
                    BusyIndicatorNotification();
                }                
            }
            catch (Exception ex)
            {                
                Logging.LogException(logger, ex);
                BusyIndicatorNotification();
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }

        /// <summary>
        /// RetrieveRatioComparisonData callback method - assigns RatioComparisonInfo
        /// </summary>
        /// <param name="result">List of RatioComparisonData</param>
        public void RetrieveRatioComparisonDataCallbackMethod(List<RatioComparisonData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    RatioComparisonInfo = result;
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
                BusyIndicatorNotification();
            }
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Event unsubscriptions
        /// </summary>
        public void Dispose()
        {
            eventAggregator.GetEvent<SecurityReferenceSetEvent>().Unsubscribe(HandleSecurityReferenceSetEvent);
        }

        /// <summary>
        /// Converts security information into xml string to retrieve ratio comparison data.
        /// </summary>
        /// <param name="data">List of GF_SECURITY_BASEVIEW</param>
        /// <returns>xml string</returns>
        private String GetContextSecurityXML(List<GF_SECURITY_BASEVIEW> data)
        {
            string result = String.Empty;
            try
            {
                String periodType = "C";
                String periodYear = String.Empty;

                switch (SelectedPeriod)
                {
                    case ScatterGraphPeriod.TRAILING:
                        periodType = "C";
                        periodYear = String.Empty;
                        break;
                    case ScatterGraphPeriod.FORWARD:
                        periodType = "C";
                        periodYear = String.Empty;
                        break;
                    case ScatterGraphPeriod.YEAR:
                        periodType = "A";
                        periodYear = DateTime.Today.Year.ToString();
                        break;
                    case ScatterGraphPeriod.YEAR_PLUS_ONE:
                        periodType = "A";
                        periodYear = (DateTime.Today.Year + 1).ToString();
                        break;
                    case ScatterGraphPeriod.YEAR_PLUS_TWO:
                        periodType = "A";
                        periodYear = (DateTime.Today.Year + 2).ToString();
                        break;
                    case ScatterGraphPeriod.YEAR_PLUS_THREE:
                        periodType = "A";
                        periodYear = (DateTime.Today.Year + 3).ToString();
                        break;
                    default:
                        periodType = "C";
                        periodYear = String.Empty;
                        break;
                }
                Int32? financialDataId = RatioPeriodMapping.GetDataId(SelectedFinancialRatio, SelectedPeriod);
                Int32? financialEstimationId = RatioPeriodMapping.GetEstimationId(SelectedFinancialRatio, SelectedPeriod);
                Int32? valuationDataId = RatioPeriodMapping.GetDataId(SelectedValuationRatio, SelectedPeriod);
                Int32? valuationEstimationId = RatioPeriodMapping.GetEstimationId(SelectedValuationRatio, SelectedPeriod);

                XElement root = new XElement("RatioData",
                    new XAttribute("PeriodType", periodType),
                    new XAttribute("PeriodYear", periodYear),
                    new XAttribute("FinancialDataId", financialDataId.ToString()),
                    new XAttribute("FinancialEstimationId", financialEstimationId.ToString()),
                    new XAttribute("ValuationDataId", valuationDataId.ToString()),
                    new XAttribute("ValuationEstimationId", valuationEstimationId.ToString()));
                
                foreach (GF_SECURITY_BASEVIEW record in data)
                {
                    XElement securityData = new XElement("Issue",
                        new XAttribute("SecurityId", record.SECURITY_ID.ToString()),
                        new XAttribute("IssueName", record.ISSUE_NAME.ToString()),
                        new XAttribute("IssuerId", record.ISSUER_ID.ToString()));

                    root.Add(securityData);
                }
                XDocument doc = new XDocument(
                        new XDeclaration("1.0", "utf-8", "yes"),
                        root);

                result = doc.ToString();
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// Display/Hide Busy Indicator
        /// </summary>
        /// <param name="isBusyIndicatorVisible">True to display indicator; default false</param>
        /// <param name="message">Content message for indicator; default null</param>
        private void BusyIndicatorNotification(bool isBusyIndicatorVisible = false, String message = null)
        {
            if (message != null)
            {
                BusyIndicatorContent = message;
            }
            IsBusyIndicatorBusy = isBusyIndicatorVisible;
        }
        
        /// <summary>
        /// Sets scatter chart defaults for different instances for the same view
        /// </summary>
        /// <param name="chartDefault">ScatterChartDefaults</param>
        private void SetScatterChartDefaults(ScatterChartDefaults chartDefault)
        {
            switch (chartDefault)
            {
                case ScatterChartDefaults.BANK:
                    SelectedFinancialRatio = ScatterGraphFinancialRatio.REVENUE_GROWTH;
                    SelectedValuationRatio = ScatterGraphValuationRatio.PRICE_TO_REVENUE;
                    break;
                case ScatterChartDefaults.INDUSTRIAL:
                    SelectedFinancialRatio = ScatterGraphFinancialRatio.NET_INCOME_GROWTH;
                    SelectedValuationRatio = ScatterGraphValuationRatio.PRICE_TO_EQUITY;
                    break;
                case ScatterChartDefaults.INSURANCE:
                    SelectedFinancialRatio = ScatterGraphFinancialRatio.RETURN_ON_EQUITY;
                    SelectedValuationRatio = ScatterGraphValuationRatio.PRICE_TO_BOOK_VALUE;
                    break;
                case ScatterChartDefaults.UTILITY:
                    SelectedFinancialRatio = ScatterGraphFinancialRatio.FREE_CASH_FLOW_MARGIN;
                    SelectedValuationRatio = ScatterGraphValuationRatio.FREE_CASH_FLOW_YIELD;
                    break;
                default:
                    break;
            }
        }
        #endregion
    }
}
