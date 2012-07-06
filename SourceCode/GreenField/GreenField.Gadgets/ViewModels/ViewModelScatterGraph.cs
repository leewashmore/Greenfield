using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using GreenField.Common;
using GreenField.DataContracts;
using Microsoft.Practices.Prism.ViewModel;
using System.Collections.Generic;
using GreenField.ServiceCaller.ExternalResearchDefinitions;
using Microsoft.Practices.Prism.Logging;
using GreenField.ServiceCaller;
using Microsoft.Practices.Prism.Events;
using System.Xml.Linq;

namespace GreenField.Gadgets.ViewModels
{
    public class ViewModelScatterGraph : NotificationObject
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
        public ViewModelScatterGraph(DashboardGadgetParam param)
        {
            //Int32? dataId = RatioPeriodMapping.GetEstimationId(ScatterGraphFinancialRatio.NET_DEBT_TO_EQUITY, ScatterGraphPeriod.TRAILING);
            _logger = param.LoggerFacade;
            _dbInteractivity = param.DBInteractivity;
            _eventAggregator = param.EventAggregator;
            EntitySelectionInfo = param.DashboardGadgetPayload.EntitySelectionData;

            if (_eventAggregator != null)
            {
                _eventAggregator.GetEvent<SecurityReferenceSetEvent>().Subscribe(HandleSecurityReferenceSetEvent);
            }

            if (EntitySelectionInfo != null)
            {
                HandleSecurityReferenceSetEvent(EntitySelectionInfo);
            }
        } 
        #endregion

        #region Properties
        private List<RatioComparisonData> _ratioComparisonInfo;
        public List<RatioComparisonData> RatioComparisonInfo
        {
            get { return _ratioComparisonInfo; }
            set 
            {
                _ratioComparisonInfo = value;
                RaisePropertyChanged(() => this.RatioComparisonInfo);
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
            set { _issuerReferenceInfo = value; }
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
        private ScatterGraphFinancialRatio _selectedFinancialRatio = ScatterGraphFinancialRatio.REVENUE_GROWTH;
        public ScatterGraphFinancialRatio SelectedFinancialRatio
        {
            get { return _selectedFinancialRatio; }
            set
            {
                if (_selectedFinancialRatio != value)
                {
                    _selectedFinancialRatio = value;
                    RaisePropertyChanged(() => this.SelectedFinancialRatio);
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
                }
            }
        }
        #endregion
        #endregion

        #region Security Information
        private EntitySelectionData _entitySelectionInfo;
        public EntitySelectionData EntitySelectionInfo
        {
            get { return _entitySelectionInfo; }
            set
            {
                _entitySelectionInfo = value;
                RaisePropertyChanged(() => this.EntitySelectionInfo);
            }
        }
        #endregion

        #region Busy Indicator
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

        #region Event Handlers
        public void HandleSecurityReferenceSetEvent(EntitySelectionData result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    EntitySelectionInfo = result;
                    if (EntitySelectionInfo != null)
                    {
                        BusyIndicatorNotification(true, "Retrieving Issuer Details based on selected security");
                        _dbInteractivity.RetrieveIssuerReferenceData(result, RetrieveIssuerReferenceDataCallbackMethod);
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
            Logging.LogEndMethod(_logger, methodNamespace);
        }
        #endregion

        #region Callback Methods
        /// <summary>
        /// RetrieveIssuerReferenceData Callback Method - assigns IssuerReferenceInfo and calls RetrieveFinancialStatementData
        /// </summary>
        /// <param name="result">IssuerReferenceData</param>
        public void RetrieveIssuerReferenceDataCallbackMethod(IssuerReferenceData result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    IssuerReferenceInfo = result;
                    _dbInteractivity.RetrieveRatioSecurityReferenceData(SelectedContext, result, RetrieveRatioSecurityReferenceDataCallbackMethod);
                }
                else
                {
                    Prompt.ShowDialog("No Issuer linked to the entity " + EntitySelectionInfo.LongName + " (" + EntitySelectionInfo.ShortName + " : " + EntitySelectionInfo.InstrumentID + ")");
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                }
                BusyIndicatorNotification();
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }


        public void RetrieveRatioSecurityReferenceDataCallbackMethod(List<GF_SECURITY_BASEVIEW> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    String contextSecurityXML = GetContextSecurityXML(result);
                    _dbInteractivity.RetrieveRatioComparisonData(contextSecurityXML, RetrieveRatioComparisonDataCallbackMethod);
                }
                else
                {
                    Prompt.ShowDialog("No Issuer linked to the entity " + EntitySelectionInfo.LongName + " (" + EntitySelectionInfo.ShortName + " : " + EntitySelectionInfo.InstrumentID + ")");
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                }
                BusyIndicatorNotification();
            }
            catch (Exception ex)
            {                
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        public void RetrieveRatioComparisonDataCallbackMethod(List<RatioComparisonData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    RatioComparisonInfo = result;
                }
                else
                {                    
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                }
                BusyIndicatorNotification();
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }
        #endregion

        #region Helper Methods
        public void Dispose()
        {
            _eventAggregator.GetEvent<SecurityReferenceSetEvent>().Unsubscribe(HandleSecurityReferenceSetEvent);
        }

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
                Int32? ValuationDataId = RatioPeriodMapping.GetDataId(SelectedValuationRatio, SelectedPeriod);
                Int32? ValuationEstimationId = RatioPeriodMapping.GetEstimationId(SelectedValuationRatio, SelectedPeriod);

                XElement root = new XElement("RatioData",
                    new XAttribute("PeriodType", periodType),
                    new XAttribute("PeriodYear", periodYear),
                    new XAttribute("FinancialDataId", financialDataId.ToString()),
                    new XAttribute("FinancialEstimationId", financialEstimationId.ToString()),
                    new XAttribute("ValuationDataId", ValuationDataId.ToString()),
                    new XAttribute("ValuationEstimationId", ValuationEstimationId.ToString()));
                
                foreach (GF_SECURITY_BASEVIEW record in data)
                {
                    XElement securityData = new XElement("Issue",
                        new XAttribute("SecurityId", record.ASEC_SEC_SHORT_NAME.ToString()),
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

        public void BusyIndicatorNotification(bool showBusyIndicator = false, String message = null)
        {
            if (message != null)
                BusyIndicatorContent = message;
            BusyIndicatorIsBusy = showBusyIndicator;
        }
        #endregion        
    }
}
