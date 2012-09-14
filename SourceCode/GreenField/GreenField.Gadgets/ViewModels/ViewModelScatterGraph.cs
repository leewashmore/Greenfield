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
using System.Linq;

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
        private ScatterChartDefaults _ScatterChartDefault;
        #endregion

        #region Constructor
        public ViewModelScatterGraph(DashboardGadgetParam param)
        {
            _logger = param.LoggerFacade;
            _dbInteractivity = param.DBInteractivity;
            _eventAggregator = param.EventAggregator;
            EntitySelectionInfo = param.DashboardGadgetPayload.EntitySelectionData;
            _ScatterChartDefault = (ScatterChartDefaults)param.AdditionalInfo;
            SetScatterChartDefaults(_ScatterChartDefault);

            if (_eventAggregator != null)
            {
                _eventAggregator.GetEvent<SecurityReferenceSetEvent>().Subscribe(HandleSecurityReferenceSetEvent);
            }            
        } 
        #endregion

        #region Properties
        #region IsActive
        private bool _isActive;
        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
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
        private List<RatioComparisonData> _ratioComparisonInfo;
        public List<RatioComparisonData> RatioComparisonInfo
        {
            get { return _ratioComparisonInfo; }
            set
            {
                _ratioComparisonInfo = value;
                RaisePropertyChanged(() => this.RatioComparisonInfo);
                if (value != null)
                {
                    IssueRatioComparisonInfo = value.Where(record => record.ISSUE_NAME == EntitySelectionInfo.LongName).ToList(); 
                }
            }
        }

        private List<RatioComparisonData> _issueRatioComparisonInfo;
        public List<RatioComparisonData> IssueRatioComparisonInfo
        {
            get { return _issueRatioComparisonInfo; }
            set
            {
                _issueRatioComparisonInfo = value;
                RaisePropertyChanged(() => this.IssueRatioComparisonInfo);
                MissingSecurityDataNotificationVisibility = value != null 
                    ? (value.Count == 1 ? Visibility.Visible : Visibility.Collapsed) 
                    : Visibility.Collapsed;
            }
        }

        private Visibility _missingSecurityDataNotificationVisibility = Visibility.Collapsed;
        public Visibility MissingSecurityDataNotificationVisibility
        {
            get { return _missingSecurityDataNotificationVisibility; }
            set 
            {
                _missingSecurityDataNotificationVisibility = value;
                RaisePropertyChanged(() => this.MissingSecurityDataNotificationVisibility);
            }
        }
        
        #endregion        

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
                    if (ContextSecurityInfo != null)
                    {
                        String contextSecurityXML = GetContextSecurityXML(ContextSecurityInfo);
                        if (_dbInteractivity != null && IsActive)
                        {
                            BusyIndicatorNotification(true, "Retrieving ratio comparison data for the selected security...");
                            _dbInteractivity.RetrieveRatioComparisonData(contextSecurityXML, RetrieveRatioComparisonDataCallbackMethod);
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
                        if (_dbInteractivity != null && IsActive)
                        {
                            BusyIndicatorNotification(true, "Retrieving ratio comparison data for the selected security...");
                            _dbInteractivity.RetrieveRatioComparisonData(contextSecurityXML, RetrieveRatioComparisonDataCallbackMethod);
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
                    if (_dbInteractivity != null && IsActive)
                    {
                        BusyIndicatorNotification(true, "Retrieving ratio security reference data...");
                        _dbInteractivity.RetrieveRatioSecurityReferenceData(value, IssuerReferenceInfo
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
                        if (_dbInteractivity != null && IsActive)
                        {
                            BusyIndicatorNotification(true, "Retrieving ratio comparison data for the selected security...");
                            _dbInteractivity.RetrieveRatioComparisonData(contextSecurityXML, RetrieveRatioComparisonDataCallbackMethod);
                        } 
                    }
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

        public List<GF_SECURITY_BASEVIEW> ContextSecurityInfo { get; set; }

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
                if (result != null && IsActive)
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
                    if (_dbInteractivity != null)
                    {
                        BusyIndicatorNotification(true, "Retrieving ratio security reference data...");
                        _dbInteractivity.RetrieveRatioSecurityReferenceData(SelectedContext, result, RetrieveRatioSecurityReferenceDataCallbackMethod); 
                    }
                }
                else
                {
                    Prompt.ShowDialog("No Issuer linked to the entity " + EntitySelectionInfo.LongName + " (" + EntitySelectionInfo.ShortName + " : " + EntitySelectionInfo.InstrumentID + ")");
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                    BusyIndicatorNotification();
                }
                
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
                    ContextSecurityInfo = result;
                    #region SampleData
                    //List<RatioComparisonData> RatioComparisonInfoData = new List<RatioComparisonData>();

                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName1", ISSUER_ID = "SampleIssuerId1", SECURITY_ID = "SampleSecurityId1", FINANCIAL = Convert.ToDecimal(1647.23013840552), VALUATION = Convert.ToDecimal(5526.99915689194) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName2", ISSUER_ID = "SampleIssuerId2", SECURITY_ID = "SampleSecurityId2", FINANCIAL = Convert.ToDecimal(53.3042274956635), VALUATION = Convert.ToDecimal(4551.89961519603) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName3", ISSUER_ID = "SampleIssuerId3", SECURITY_ID = "SampleSecurityId3", FINANCIAL = Convert.ToDecimal(396.317587136048), VALUATION = Convert.ToDecimal(3646.93356832197) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName4", ISSUER_ID = "SampleIssuerId4", SECURITY_ID = "SampleSecurityId4", FINANCIAL = Convert.ToDecimal(3653.71194647204), VALUATION = Convert.ToDecimal(1287.06345348033) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName5", ISSUER_ID = "SampleIssuerId5", SECURITY_ID = "SampleSecurityId5", FINANCIAL = Convert.ToDecimal(7811.00403052539), VALUATION = Convert.ToDecimal(27.0169082886363) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName6", ISSUER_ID = "SampleIssuerId6", SECURITY_ID = "SampleSecurityId6", FINANCIAL = Convert.ToDecimal(8249.54990949706), VALUATION = Convert.ToDecimal(267.018592597762) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName7", ISSUER_ID = "SampleIssuerId7", SECURITY_ID = "SampleSecurityId7", FINANCIAL = Convert.ToDecimal(2956.08081525349), VALUATION = Convert.ToDecimal(3154.22617471048) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName8", ISSUER_ID = "SampleIssuerId8", SECURITY_ID = "SampleSecurityId8", FINANCIAL = Convert.ToDecimal(2971.5318111141), VALUATION = Convert.ToDecimal(4548.97684432494) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName9", ISSUER_ID = "SampleIssuerId9", SECURITY_ID = "SampleSecurityId9", FINANCIAL = Convert.ToDecimal(7001.32475292048), VALUATION = Convert.ToDecimal(3652.9304764459) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName10", ISSUER_ID = "SampleIssuerId10", SECURITY_ID = "SampleSecurityId10", FINANCIAL = Convert.ToDecimal(2478.67088296735), VALUATION = Convert.ToDecimal(75.7968120422606) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName11", ISSUER_ID = "SampleIssuerId11", SECURITY_ID = "SampleSecurityId11", FINANCIAL = Convert.ToDecimal(1714.37864471438), VALUATION = Convert.ToDecimal(2263.32321549473) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName12", ISSUER_ID = "SampleIssuerId12", SECURITY_ID = "SampleSecurityId12", FINANCIAL = Convert.ToDecimal(1843.80486868278), VALUATION = Convert.ToDecimal(2029.03775148226) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName13", ISSUER_ID = "SampleIssuerId13", SECURITY_ID = "SampleSecurityId13", FINANCIAL = Convert.ToDecimal(3257.91874091796), VALUATION = Convert.ToDecimal(2235.17926081443) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName14", ISSUER_ID = "SampleIssuerId14", SECURITY_ID = "SampleSecurityId14", FINANCIAL = Convert.ToDecimal(3317.36770247165), VALUATION = Convert.ToDecimal(8070.30220746902) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName15", ISSUER_ID = "SampleIssuerId15", SECURITY_ID = "SampleSecurityId15", FINANCIAL = Convert.ToDecimal(1127.62952167559), VALUATION = Convert.ToDecimal(874.777655192926) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName16", ISSUER_ID = "SampleIssuerId16", SECURITY_ID = "SampleSecurityId16", FINANCIAL = Convert.ToDecimal(2021.15975570464), VALUATION = Convert.ToDecimal(1057.99473384004) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName17", ISSUER_ID = "SampleIssuerId17", SECURITY_ID = "SampleSecurityId17", FINANCIAL = Convert.ToDecimal(7188.8092773066), VALUATION = Convert.ToDecimal(5067.9174921331) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName18", ISSUER_ID = "SampleIssuerId18", SECURITY_ID = "SampleSecurityId18", FINANCIAL = Convert.ToDecimal(5773.40677866828), VALUATION = Convert.ToDecimal(2838.93769862765) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName19", ISSUER_ID = "SampleIssuerId19", SECURITY_ID = "SampleSecurityId19", FINANCIAL = Convert.ToDecimal(848.934468667462), VALUATION = Convert.ToDecimal(123.800139583342) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName20", ISSUER_ID = "SampleIssuerId20", SECURITY_ID = "SampleSecurityId20", FINANCIAL = Convert.ToDecimal(6094.59826464666), VALUATION = Convert.ToDecimal(7614.10383083151) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName21", ISSUER_ID = "SampleIssuerId21", SECURITY_ID = "SampleSecurityId21", FINANCIAL = Convert.ToDecimal(811.864580246483), VALUATION = Convert.ToDecimal(1694.86088572879) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName22", ISSUER_ID = "SampleIssuerId22", SECURITY_ID = "SampleSecurityId22", FINANCIAL = Convert.ToDecimal(1967.21053376921), VALUATION = Convert.ToDecimal(849.481375949323) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName23", ISSUER_ID = "SampleIssuerId23", SECURITY_ID = "SampleSecurityId23", FINANCIAL = Convert.ToDecimal(1170.85070807174), VALUATION = Convert.ToDecimal(938.370822260256) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName24", ISSUER_ID = "SampleIssuerId24", SECURITY_ID = "SampleSecurityId24", FINANCIAL = Convert.ToDecimal(1831.41905908375), VALUATION = Convert.ToDecimal(1195.13066294918) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName25", ISSUER_ID = "SampleIssuerId25", SECURITY_ID = "SampleSecurityId25", FINANCIAL = Convert.ToDecimal(1760.63323707436), VALUATION = Convert.ToDecimal(5985.24569224876) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName26", ISSUER_ID = "SampleIssuerId26", SECURITY_ID = "SampleSecurityId26", FINANCIAL = Convert.ToDecimal(4737.05960784804), VALUATION = Convert.ToDecimal(97.0538705893988) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName27", ISSUER_ID = "SampleIssuerId27", SECURITY_ID = "SampleSecurityId27", FINANCIAL = Convert.ToDecimal(2555.89836125721), VALUATION = Convert.ToDecimal(5798.30876924736) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName28", ISSUER_ID = "SampleIssuerId28", SECURITY_ID = "SampleSecurityId28", FINANCIAL = Convert.ToDecimal(2785.26074354648), VALUATION = Convert.ToDecimal(2952.35950510239) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName29", ISSUER_ID = "SampleIssuerId29", SECURITY_ID = "SampleSecurityId29", FINANCIAL = Convert.ToDecimal(1144.72031158477), VALUATION = Convert.ToDecimal(55.2819375737666) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName30", ISSUER_ID = "SampleIssuerId30", SECURITY_ID = "SampleSecurityId30", FINANCIAL = Convert.ToDecimal(712.604167324097), VALUATION = Convert.ToDecimal(2938.0205274788) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName31", ISSUER_ID = "SampleIssuerId31", SECURITY_ID = "SampleSecurityId31", FINANCIAL = Convert.ToDecimal(3077.10975576097), VALUATION = Convert.ToDecimal(462.617568920934) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName32", ISSUER_ID = "SampleIssuerId32", SECURITY_ID = "SampleSecurityId32", FINANCIAL = Convert.ToDecimal(977.011972213387), VALUATION = Convert.ToDecimal(3520.2078118903) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName33", ISSUER_ID = "SampleIssuerId33", SECURITY_ID = "SampleSecurityId33", FINANCIAL = Convert.ToDecimal(1137.09129253965), VALUATION = Convert.ToDecimal(20.4485007736294) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName34", ISSUER_ID = "SampleIssuerId34", SECURITY_ID = "SampleSecurityId34", FINANCIAL = Convert.ToDecimal(3035.85741733694), VALUATION = Convert.ToDecimal(269.014259484883) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName35", ISSUER_ID = "SampleIssuerId35", SECURITY_ID = "SampleSecurityId35", FINANCIAL = Convert.ToDecimal(3547.00683434153), VALUATION = Convert.ToDecimal(1858.30809070648) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName36", ISSUER_ID = "SampleIssuerId36", SECURITY_ID = "SampleSecurityId36", FINANCIAL = Convert.ToDecimal(4778.95911344004), VALUATION = Convert.ToDecimal(4621.11229443842) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName37", ISSUER_ID = "SampleIssuerId37", SECURITY_ID = "SampleSecurityId37", FINANCIAL = Convert.ToDecimal(2042.68308095658), VALUATION = Convert.ToDecimal(5304.93655946645) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName38", ISSUER_ID = "SampleIssuerId38", SECURITY_ID = "SampleSecurityId38", FINANCIAL = Convert.ToDecimal(2167.99860069405), VALUATION = Convert.ToDecimal(2091.3900472193) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName39", ISSUER_ID = "SampleIssuerId39", SECURITY_ID = "SampleSecurityId39", FINANCIAL = Convert.ToDecimal(5813.83482405768), VALUATION = Convert.ToDecimal(847.746875157521) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName40", ISSUER_ID = "SampleIssuerId40", SECURITY_ID = "SampleSecurityId40", FINANCIAL = Convert.ToDecimal(4018.79567966366), VALUATION = Convert.ToDecimal(3163.30966286612) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName41", ISSUER_ID = "SampleIssuerId41", SECURITY_ID = "SampleSecurityId41", FINANCIAL = Convert.ToDecimal(139.046461603412), VALUATION = Convert.ToDecimal(5623.29934624921) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName42", ISSUER_ID = "SampleIssuerId42", SECURITY_ID = "SampleSecurityId42", FINANCIAL = Convert.ToDecimal(106.680852827898), VALUATION = Convert.ToDecimal(5474.98000541007) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName43", ISSUER_ID = "SampleIssuerId43", SECURITY_ID = "SampleSecurityId43", FINANCIAL = Convert.ToDecimal(805.620807572427), VALUATION = Convert.ToDecimal(1314.89371205618) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName44", ISSUER_ID = "SampleIssuerId44", SECURITY_ID = "SampleSecurityId44", FINANCIAL = Convert.ToDecimal(2905.3842059215), VALUATION = Convert.ToDecimal(4056.80401762647) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName45", ISSUER_ID = "SampleIssuerId45", SECURITY_ID = "SampleSecurityId45", FINANCIAL = Convert.ToDecimal(3664.02592178282), VALUATION = Convert.ToDecimal(4440.34978461111) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName46", ISSUER_ID = "SampleIssuerId46", SECURITY_ID = "SampleSecurityId46", FINANCIAL = Convert.ToDecimal(179.653761080613), VALUATION = Convert.ToDecimal(8487.49800019174) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName47", ISSUER_ID = "SampleIssuerId47", SECURITY_ID = "SampleSecurityId47", FINANCIAL = Convert.ToDecimal(3537.29311640355), VALUATION = Convert.ToDecimal(267.486577365307) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName48", ISSUER_ID = "SampleIssuerId48", SECURITY_ID = "SampleSecurityId48", FINANCIAL = Convert.ToDecimal(384.117848489124), VALUATION = Convert.ToDecimal(364.766134260151) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName49", ISSUER_ID = "SampleIssuerId49", SECURITY_ID = "SampleSecurityId49", FINANCIAL = Convert.ToDecimal(4803.7887852883), VALUATION = Convert.ToDecimal(3260.46960241289) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName50", ISSUER_ID = "SampleIssuerId50", SECURITY_ID = "SampleSecurityId50", FINANCIAL = Convert.ToDecimal(1638.65068128585), VALUATION = Convert.ToDecimal(4592.98616860776) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName51", ISSUER_ID = "SampleIssuerId51", SECURITY_ID = "SampleSecurityId51", FINANCIAL = Convert.ToDecimal(1580.03403954648), VALUATION = Convert.ToDecimal(4868.97940869488) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName52", ISSUER_ID = "SampleIssuerId52", SECURITY_ID = "SampleSecurityId52", FINANCIAL = Convert.ToDecimal(3831.86335033336), VALUATION = Convert.ToDecimal(4903.40413090642) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName53", ISSUER_ID = "SampleIssuerId53", SECURITY_ID = "SampleSecurityId53", FINANCIAL = Convert.ToDecimal(1035.10801828931), VALUATION = Convert.ToDecimal(578.87301906908) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName54", ISSUER_ID = "SampleIssuerId54", SECURITY_ID = "SampleSecurityId54", FINANCIAL = Convert.ToDecimal(4382.44064476342), VALUATION = Convert.ToDecimal(4004.0147717992) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName55", ISSUER_ID = "SampleIssuerId55", SECURITY_ID = "SampleSecurityId55", FINANCIAL = Convert.ToDecimal(2497.62968833037), VALUATION = Convert.ToDecimal(4018.26275277497) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName56", ISSUER_ID = "SampleIssuerId56", SECURITY_ID = "SampleSecurityId56", FINANCIAL = Convert.ToDecimal(6163.30628686398), VALUATION = Convert.ToDecimal(5304.96559154403) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName57", ISSUER_ID = "SampleIssuerId57", SECURITY_ID = "SampleSecurityId57", FINANCIAL = Convert.ToDecimal(1521.67627107633), VALUATION = Convert.ToDecimal(2700.98917570764) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName58", ISSUER_ID = "SampleIssuerId58", SECURITY_ID = "SampleSecurityId58", FINANCIAL = Convert.ToDecimal(3497.05688589511), VALUATION = Convert.ToDecimal(7893.43578223884) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName59", ISSUER_ID = "SampleIssuerId59", SECURITY_ID = "SampleSecurityId59", FINANCIAL = Convert.ToDecimal(6316.55323360317), VALUATION = Convert.ToDecimal(5227.03292118198) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName60", ISSUER_ID = "SampleIssuerId60", SECURITY_ID = "SampleSecurityId60", FINANCIAL = Convert.ToDecimal(591.572246129173), VALUATION = Convert.ToDecimal(39.7515927125914) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName61", ISSUER_ID = "SampleIssuerId61", SECURITY_ID = "SampleSecurityId61", FINANCIAL = Convert.ToDecimal(1638.23589872222), VALUATION = Convert.ToDecimal(4548.55744603925) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName62", ISSUER_ID = "SampleIssuerId62", SECURITY_ID = "SampleSecurityId62", FINANCIAL = Convert.ToDecimal(86.6407221189424), VALUATION = Convert.ToDecimal(240.014920251111) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName63", ISSUER_ID = "SampleIssuerId63", SECURITY_ID = "SampleSecurityId63", FINANCIAL = Convert.ToDecimal(7725.15046513352), VALUATION = Convert.ToDecimal(308.97945793516) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName64", ISSUER_ID = "SampleIssuerId64", SECURITY_ID = "SampleSecurityId64", FINANCIAL = Convert.ToDecimal(9104.97314267186), VALUATION = Convert.ToDecimal(1553.57447799719) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName65", ISSUER_ID = "SampleIssuerId65", SECURITY_ID = "SampleSecurityId65", FINANCIAL = Convert.ToDecimal(1312.45967742362), VALUATION = Convert.ToDecimal(1604.11621564048) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName66", ISSUER_ID = "SampleIssuerId66", SECURITY_ID = "SampleSecurityId66", FINANCIAL = Convert.ToDecimal(136.119415487795), VALUATION = Convert.ToDecimal(409.231833845864) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName67", ISSUER_ID = "SampleIssuerId67", SECURITY_ID = "SampleSecurityId67", FINANCIAL = Convert.ToDecimal(2019.89413264564), VALUATION = Convert.ToDecimal(711.064432802639) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName68", ISSUER_ID = "SampleIssuerId68", SECURITY_ID = "SampleSecurityId68", FINANCIAL = Convert.ToDecimal(2165.13636441857), VALUATION = Convert.ToDecimal(5468.39646575411) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName69", ISSUER_ID = "SampleIssuerId69", SECURITY_ID = "SampleSecurityId69", FINANCIAL = Convert.ToDecimal(1642.50985291029), VALUATION = Convert.ToDecimal(8670.73947445593) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName70", ISSUER_ID = "SampleIssuerId70", SECURITY_ID = "SampleSecurityId70", FINANCIAL = Convert.ToDecimal(706.541176881744), VALUATION = Convert.ToDecimal(3687.50008611778) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName71", ISSUER_ID = "SampleIssuerId71", SECURITY_ID = "SampleSecurityId71", FINANCIAL = Convert.ToDecimal(546.376076004383), VALUATION = Convert.ToDecimal(5383.8668788644) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName72", ISSUER_ID = "SampleIssuerId72", SECURITY_ID = "SampleSecurityId72", FINANCIAL = Convert.ToDecimal(75.8016831537465), VALUATION = Convert.ToDecimal(326.26315934627) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName73", ISSUER_ID = "SampleIssuerId73", SECURITY_ID = "SampleSecurityId73", FINANCIAL = Convert.ToDecimal(1839.78194416439), VALUATION = Convert.ToDecimal(1210.04225470809) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName74", ISSUER_ID = "SampleIssuerId74", SECURITY_ID = "SampleSecurityId74", FINANCIAL = Convert.ToDecimal(4843.42854456877), VALUATION = Convert.ToDecimal(4030.97762695083) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName75", ISSUER_ID = "SampleIssuerId75", SECURITY_ID = "SampleSecurityId75", FINANCIAL = Convert.ToDecimal(2717.03502429917), VALUATION = Convert.ToDecimal(569.278526746938) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName76", ISSUER_ID = "SampleIssuerId76", SECURITY_ID = "SampleSecurityId76", FINANCIAL = Convert.ToDecimal(806.630277012408), VALUATION = Convert.ToDecimal(9.76111995331983) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName77", ISSUER_ID = "SampleIssuerId77", SECURITY_ID = "SampleSecurityId77", FINANCIAL = Convert.ToDecimal(1129.9214850268), VALUATION = Convert.ToDecimal(1803.9904352684) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName78", ISSUER_ID = "SampleIssuerId78", SECURITY_ID = "SampleSecurityId78", FINANCIAL = Convert.ToDecimal(8294.28661384554), VALUATION = Convert.ToDecimal(2974.05863581473) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName79", ISSUER_ID = "SampleIssuerId79", SECURITY_ID = "SampleSecurityId79", FINANCIAL = Convert.ToDecimal(6747.51389638186), VALUATION = Convert.ToDecimal(1633.86430381981) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName80", ISSUER_ID = "SampleIssuerId80", SECURITY_ID = "SampleSecurityId80", FINANCIAL = Convert.ToDecimal(5327.9311386288), VALUATION = Convert.ToDecimal(376.912232277749) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName81", ISSUER_ID = "SampleIssuerId81", SECURITY_ID = "SampleSecurityId81", FINANCIAL = Convert.ToDecimal(273.637836597214), VALUATION = Convert.ToDecimal(7709.65003782109) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName82", ISSUER_ID = "SampleIssuerId82", SECURITY_ID = "SampleSecurityId82", FINANCIAL = Convert.ToDecimal(1037.33826588391), VALUATION = Convert.ToDecimal(4653.2226720945) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName83", ISSUER_ID = "SampleIssuerId83", SECURITY_ID = "SampleSecurityId83", FINANCIAL = Convert.ToDecimal(220.874663983358), VALUATION = Convert.ToDecimal(1924.07495031141) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName84", ISSUER_ID = "SampleIssuerId84", SECURITY_ID = "SampleSecurityId84", FINANCIAL = Convert.ToDecimal(1195.04059298738), VALUATION = Convert.ToDecimal(8218.97329570685) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName85", ISSUER_ID = "SampleIssuerId85", SECURITY_ID = "SampleSecurityId85", FINANCIAL = Convert.ToDecimal(2008.30888744262), VALUATION = Convert.ToDecimal(8106.64402103387) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName86", ISSUER_ID = "SampleIssuerId86", SECURITY_ID = "SampleSecurityId86", FINANCIAL = Convert.ToDecimal(1274.36427863728), VALUATION = Convert.ToDecimal(1960.73362981823) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName87", ISSUER_ID = "SampleIssuerId87", SECURITY_ID = "SampleSecurityId87", FINANCIAL = Convert.ToDecimal(3598.81267787633), VALUATION = Convert.ToDecimal(1603.02430088307) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName88", ISSUER_ID = "SampleIssuerId88", SECURITY_ID = "SampleSecurityId88", FINANCIAL = Convert.ToDecimal(3250.67821880872), VALUATION = Convert.ToDecimal(5597.33641382014) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName89", ISSUER_ID = "SampleIssuerId89", SECURITY_ID = "SampleSecurityId89", FINANCIAL = Convert.ToDecimal(1657.78165100285), VALUATION = Convert.ToDecimal(4503.08158518368) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName90", ISSUER_ID = "SampleIssuerId90", SECURITY_ID = "SampleSecurityId90", FINANCIAL = Convert.ToDecimal(565.359375686992), VALUATION = Convert.ToDecimal(449.473305861851) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName91", ISSUER_ID = "SampleIssuerId91", SECURITY_ID = "SampleSecurityId91", FINANCIAL = Convert.ToDecimal(719.077278303591), VALUATION = Convert.ToDecimal(7434.08273777696) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName92", ISSUER_ID = "SampleIssuerId92", SECURITY_ID = "SampleSecurityId92", FINANCIAL = Convert.ToDecimal(5434.70005662174), VALUATION = Convert.ToDecimal(1658.46684559809) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName93", ISSUER_ID = "SampleIssuerId93", SECURITY_ID = "SampleSecurityId93", FINANCIAL = Convert.ToDecimal(8291.34892909673), VALUATION = Convert.ToDecimal(1129.48042578952) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName94", ISSUER_ID = "SampleIssuerId94", SECURITY_ID = "SampleSecurityId94", FINANCIAL = Convert.ToDecimal(1661.24266682278), VALUATION = Convert.ToDecimal(691.283646938162) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName95", ISSUER_ID = "SampleIssuerId95", SECURITY_ID = "SampleSecurityId95", FINANCIAL = Convert.ToDecimal(2843.27824262122), VALUATION = Convert.ToDecimal(772.745123735007) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName96", ISSUER_ID = "SampleIssuerId96", SECURITY_ID = "SampleSecurityId96", FINANCIAL = Convert.ToDecimal(6621.82140690555), VALUATION = Convert.ToDecimal(2407.41662918056) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName97", ISSUER_ID = "SampleIssuerId97", SECURITY_ID = "SampleSecurityId97", FINANCIAL = Convert.ToDecimal(2114.49913758176), VALUATION = Convert.ToDecimal(103.294413065376) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName98", ISSUER_ID = "SampleIssuerId98", SECURITY_ID = "SampleSecurityId98", FINANCIAL = Convert.ToDecimal(2573.80067599676), VALUATION = Convert.ToDecimal(511.599517803979) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName99", ISSUER_ID = "SampleIssuerId99", SECURITY_ID = "SampleSecurityId99", FINANCIAL = Convert.ToDecimal(2404.35602533453), VALUATION = Convert.ToDecimal(166.763028477574) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName100", ISSUER_ID = "SampleIssuerId100", SECURITY_ID = "SampleSecurityId100", FINANCIAL = Convert.ToDecimal(1446.53839142167), VALUATION = Convert.ToDecimal(3582.68299699011) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName101", ISSUER_ID = "SampleIssuerId101", SECURITY_ID = "SampleSecurityId101", FINANCIAL = Convert.ToDecimal(3206.12357485155), VALUATION = Convert.ToDecimal(7376.61242595055) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName102", ISSUER_ID = "SampleIssuerId102", SECURITY_ID = "SampleSecurityId102", FINANCIAL = Convert.ToDecimal(5591.68670837744), VALUATION = Convert.ToDecimal(5659.78699242509) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName103", ISSUER_ID = "SampleIssuerId103", SECURITY_ID = "SampleSecurityId103", FINANCIAL = Convert.ToDecimal(492.605972522908), VALUATION = Convert.ToDecimal(4341.60416416941) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName104", ISSUER_ID = "SampleIssuerId104", SECURITY_ID = "SampleSecurityId104", FINANCIAL = Convert.ToDecimal(6522.72278809712), VALUATION = Convert.ToDecimal(5307.16920011001) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName105", ISSUER_ID = "SampleIssuerId105", SECURITY_ID = "SampleSecurityId105", FINANCIAL = Convert.ToDecimal(1956.2813457635), VALUATION = Convert.ToDecimal(4533.15582438949) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName106", ISSUER_ID = "SampleIssuerId106", SECURITY_ID = "SampleSecurityId106", FINANCIAL = Convert.ToDecimal(2209.46259569827), VALUATION = Convert.ToDecimal(754.514636957009) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName107", ISSUER_ID = "SampleIssuerId107", SECURITY_ID = "SampleSecurityId107", FINANCIAL = Convert.ToDecimal(4311.86551147618), VALUATION = Convert.ToDecimal(1180.99221805955) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName108", ISSUER_ID = "SampleIssuerId108", SECURITY_ID = "SampleSecurityId108", FINANCIAL = Convert.ToDecimal(1503.03596786011), VALUATION = Convert.ToDecimal(3587.6405256525) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName109", ISSUER_ID = "SampleIssuerId109", SECURITY_ID = "SampleSecurityId109", FINANCIAL = Convert.ToDecimal(594.281542288392), VALUATION = Convert.ToDecimal(172.893857509389) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName110", ISSUER_ID = "SampleIssuerId110", SECURITY_ID = "SampleSecurityId110", FINANCIAL = Convert.ToDecimal(1564.17390220847), VALUATION = Convert.ToDecimal(470.136699004804) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName111", ISSUER_ID = "SampleIssuerId111", SECURITY_ID = "SampleSecurityId111", FINANCIAL = Convert.ToDecimal(822.265176053277), VALUATION = Convert.ToDecimal(2532.68340285041) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName112", ISSUER_ID = "SampleIssuerId112", SECURITY_ID = "SampleSecurityId112", FINANCIAL = Convert.ToDecimal(4229.27316981584), VALUATION = Convert.ToDecimal(1334.06045888587) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName113", ISSUER_ID = "SampleIssuerId113", SECURITY_ID = "SampleSecurityId113", FINANCIAL = Convert.ToDecimal(4499.88099516834), VALUATION = Convert.ToDecimal(6423.54912101234) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName114", ISSUER_ID = "SampleIssuerId114", SECURITY_ID = "SampleSecurityId114", FINANCIAL = Convert.ToDecimal(4548.12696490555), VALUATION = Convert.ToDecimal(973.320058644881) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName115", ISSUER_ID = "SampleIssuerId115", SECURITY_ID = "SampleSecurityId115", FINANCIAL = Convert.ToDecimal(3686.42079082848), VALUATION = Convert.ToDecimal(1542.98115267704) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName116", ISSUER_ID = "SampleIssuerId116", SECURITY_ID = "SampleSecurityId116", FINANCIAL = Convert.ToDecimal(1767.65576790478), VALUATION = Convert.ToDecimal(2896.03761347547) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName117", ISSUER_ID = "SampleIssuerId117", SECURITY_ID = "SampleSecurityId117", FINANCIAL = Convert.ToDecimal(289.784089477655), VALUATION = Convert.ToDecimal(2931.84604298405) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName118", ISSUER_ID = "SampleIssuerId118", SECURITY_ID = "SampleSecurityId118", FINANCIAL = Convert.ToDecimal(1286.95056892395), VALUATION = Convert.ToDecimal(3038.04385726948) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName119", ISSUER_ID = "SampleIssuerId119", SECURITY_ID = "SampleSecurityId119", FINANCIAL = Convert.ToDecimal(436.801091997138), VALUATION = Convert.ToDecimal(5611.97676823772) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName120", ISSUER_ID = "SampleIssuerId120", SECURITY_ID = "SampleSecurityId120", FINANCIAL = Convert.ToDecimal(647.562011738415), VALUATION = Convert.ToDecimal(2074.64869573602) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName121", ISSUER_ID = "SampleIssuerId121", SECURITY_ID = "SampleSecurityId121", FINANCIAL = Convert.ToDecimal(3035.64982966186), VALUATION = Convert.ToDecimal(5470.57740990718) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName122", ISSUER_ID = "SampleIssuerId122", SECURITY_ID = "SampleSecurityId122", FINANCIAL = Convert.ToDecimal(1430.94699458674), VALUATION = Convert.ToDecimal(1440.03812670869) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName123", ISSUER_ID = "SampleIssuerId123", SECURITY_ID = "SampleSecurityId123", FINANCIAL = Convert.ToDecimal(2572.51101720273), VALUATION = Convert.ToDecimal(1369.55835286099) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName124", ISSUER_ID = "SampleIssuerId124", SECURITY_ID = "SampleSecurityId124", FINANCIAL = Convert.ToDecimal(1272.34230004903), VALUATION = Convert.ToDecimal(615.088984018944) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName125", ISSUER_ID = "SampleIssuerId125", SECURITY_ID = "SampleSecurityId125", FINANCIAL = Convert.ToDecimal(106.424864753331), VALUATION = Convert.ToDecimal(297.91306952741) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName126", ISSUER_ID = "SampleIssuerId126", SECURITY_ID = "SampleSecurityId126", FINANCIAL = Convert.ToDecimal(2693.64979608048), VALUATION = Convert.ToDecimal(240.792317567599) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName127", ISSUER_ID = "SampleIssuerId127", SECURITY_ID = "SampleSecurityId127", FINANCIAL = Convert.ToDecimal(2318.76885424737), VALUATION = Convert.ToDecimal(123.60637190136) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName128", ISSUER_ID = "SampleIssuerId128", SECURITY_ID = "SampleSecurityId128", FINANCIAL = Convert.ToDecimal(1481.47642117175), VALUATION = Convert.ToDecimal(448.404705190667) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName129", ISSUER_ID = "SampleIssuerId129", SECURITY_ID = "SampleSecurityId129", FINANCIAL = Convert.ToDecimal(3198.17777498272), VALUATION = Convert.ToDecimal(326.05862713172) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName130", ISSUER_ID = "SampleIssuerId130", SECURITY_ID = "SampleSecurityId130", FINANCIAL = Convert.ToDecimal(1455.93941571893), VALUATION = Convert.ToDecimal(293.030269173205) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName131", ISSUER_ID = "SampleIssuerId131", SECURITY_ID = "SampleSecurityId131", FINANCIAL = Convert.ToDecimal(1485.97873738088), VALUATION = Convert.ToDecimal(1610.44356747533) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName132", ISSUER_ID = "SampleIssuerId132", SECURITY_ID = "SampleSecurityId132", FINANCIAL = Convert.ToDecimal(422.253511743734), VALUATION = Convert.ToDecimal(2295.61365305586) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName133", ISSUER_ID = "SampleIssuerId133", SECURITY_ID = "SampleSecurityId133", FINANCIAL = Convert.ToDecimal(1479.60839547571), VALUATION = Convert.ToDecimal(1571.11419974163) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName134", ISSUER_ID = "SampleIssuerId134", SECURITY_ID = "SampleSecurityId134", FINANCIAL = Convert.ToDecimal(5377.19822329277), VALUATION = Convert.ToDecimal(8697.10648851128) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName135", ISSUER_ID = "SampleIssuerId135", SECURITY_ID = "SampleSecurityId135", FINANCIAL = Convert.ToDecimal(15.8320481860274), VALUATION = Convert.ToDecimal(8623.99903039889) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName136", ISSUER_ID = "SampleIssuerId136", SECURITY_ID = "SampleSecurityId136", FINANCIAL = Convert.ToDecimal(711.508665485563), VALUATION = Convert.ToDecimal(1718.93298146089) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName137", ISSUER_ID = "SampleIssuerId137", SECURITY_ID = "SampleSecurityId137", FINANCIAL = Convert.ToDecimal(5360.04284845037), VALUATION = Convert.ToDecimal(3554.48576298478) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName138", ISSUER_ID = "SampleIssuerId138", SECURITY_ID = "SampleSecurityId138", FINANCIAL = Convert.ToDecimal(1171.82465669065), VALUATION = Convert.ToDecimal(5940.33451380505) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName139", ISSUER_ID = "SampleIssuerId139", SECURITY_ID = "SampleSecurityId139", FINANCIAL = Convert.ToDecimal(6038.90024947438), VALUATION = Convert.ToDecimal(4626.66933384846) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName140", ISSUER_ID = "SampleIssuerId140", SECURITY_ID = "SampleSecurityId140", FINANCIAL = Convert.ToDecimal(5062.34923390985), VALUATION = Convert.ToDecimal(2116.6852452129) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName141", ISSUER_ID = "SampleIssuerId141", SECURITY_ID = "SampleSecurityId141", FINANCIAL = Convert.ToDecimal(7784.63386447663), VALUATION = Convert.ToDecimal(2024.70090224626) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName142", ISSUER_ID = "SampleIssuerId142", SECURITY_ID = "SampleSecurityId142", FINANCIAL = Convert.ToDecimal(139.093136573434), VALUATION = Convert.ToDecimal(84.1358608210517) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName143", ISSUER_ID = "SampleIssuerId143", SECURITY_ID = "SampleSecurityId143", FINANCIAL = Convert.ToDecimal(275.070290371128), VALUATION = Convert.ToDecimal(3290.17097478323) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName144", ISSUER_ID = "SampleIssuerId144", SECURITY_ID = "SampleSecurityId144", FINANCIAL = Convert.ToDecimal(1202.21001553099), VALUATION = Convert.ToDecimal(3702.36857968816) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName145", ISSUER_ID = "SampleIssuerId145", SECURITY_ID = "SampleSecurityId145", FINANCIAL = Convert.ToDecimal(3743.86631371965), VALUATION = Convert.ToDecimal(5616.27681057179) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName146", ISSUER_ID = "SampleIssuerId146", SECURITY_ID = "SampleSecurityId146", FINANCIAL = Convert.ToDecimal(178.533435056025), VALUATION = Convert.ToDecimal(447.131746576067) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName147", ISSUER_ID = "SampleIssuerId147", SECURITY_ID = "SampleSecurityId147", FINANCIAL = Convert.ToDecimal(4673.46437228437), VALUATION = Convert.ToDecimal(422.20381517207) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName148", ISSUER_ID = "SampleIssuerId148", SECURITY_ID = "SampleSecurityId148", FINANCIAL = Convert.ToDecimal(2661.84508521556), VALUATION = Convert.ToDecimal(6034.92544034156) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName149", ISSUER_ID = "SampleIssuerId149", SECURITY_ID = "SampleSecurityId149", FINANCIAL = Convert.ToDecimal(412.426466568241), VALUATION = Convert.ToDecimal(882.889443507615) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName150", ISSUER_ID = "SampleIssuerId150", SECURITY_ID = "SampleSecurityId150", FINANCIAL = Convert.ToDecimal(454.164910272571), VALUATION = Convert.ToDecimal(3.01732386609717) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName151", ISSUER_ID = "SampleIssuerId151", SECURITY_ID = "SampleSecurityId151", FINANCIAL = Convert.ToDecimal(687.397392926465), VALUATION = Convert.ToDecimal(13.7704664274611) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName152", ISSUER_ID = "SampleIssuerId152", SECURITY_ID = "SampleSecurityId152", FINANCIAL = Convert.ToDecimal(5057.96588419753), VALUATION = Convert.ToDecimal(1909.35034592415) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName153", ISSUER_ID = "SampleIssuerId153", SECURITY_ID = "SampleSecurityId153", FINANCIAL = Convert.ToDecimal(1367.2832545858), VALUATION = Convert.ToDecimal(4447.58582785651) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName154", ISSUER_ID = "SampleIssuerId154", SECURITY_ID = "SampleSecurityId154", FINANCIAL = Convert.ToDecimal(1608.33916057019), VALUATION = Convert.ToDecimal(4627.47866752627) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName155", ISSUER_ID = "SampleIssuerId155", SECURITY_ID = "SampleSecurityId155", FINANCIAL = Convert.ToDecimal(2946.94015394887), VALUATION = Convert.ToDecimal(7968.20194301818) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName156", ISSUER_ID = "SampleIssuerId156", SECURITY_ID = "SampleSecurityId156", FINANCIAL = Convert.ToDecimal(8009.95630025665), VALUATION = Convert.ToDecimal(1476.64123691401) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName157", ISSUER_ID = "SampleIssuerId157", SECURITY_ID = "SampleSecurityId157", FINANCIAL = Convert.ToDecimal(550.248879960068), VALUATION = Convert.ToDecimal(1714.12589328015) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName158", ISSUER_ID = "SampleIssuerId158", SECURITY_ID = "SampleSecurityId158", FINANCIAL = Convert.ToDecimal(7528.66506272205), VALUATION = Convert.ToDecimal(3955.20940375361) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName159", ISSUER_ID = "SampleIssuerId159", SECURITY_ID = "SampleSecurityId159", FINANCIAL = Convert.ToDecimal(4041.68392542959), VALUATION = Convert.ToDecimal(4889.79728416697) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName160", ISSUER_ID = "SampleIssuerId160", SECURITY_ID = "SampleSecurityId160", FINANCIAL = Convert.ToDecimal(5317.96615307712), VALUATION = Convert.ToDecimal(2166.16360734047) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName161", ISSUER_ID = "SampleIssuerId161", SECURITY_ID = "SampleSecurityId161", FINANCIAL = Convert.ToDecimal(2376.79717629536), VALUATION = Convert.ToDecimal(924.387219476338) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName162", ISSUER_ID = "SampleIssuerId162", SECURITY_ID = "SampleSecurityId162", FINANCIAL = Convert.ToDecimal(1748.82052872209), VALUATION = Convert.ToDecimal(8384.95706328562) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName163", ISSUER_ID = "SampleIssuerId163", SECURITY_ID = "SampleSecurityId163", FINANCIAL = Convert.ToDecimal(1.2375744175539), VALUATION = Convert.ToDecimal(2469.80731906897) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName164", ISSUER_ID = "SampleIssuerId164", SECURITY_ID = "SampleSecurityId164", FINANCIAL = Convert.ToDecimal(412.109417518088), VALUATION = Convert.ToDecimal(1436.88352354291) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName165", ISSUER_ID = "SampleIssuerId165", SECURITY_ID = "SampleSecurityId165", FINANCIAL = Convert.ToDecimal(2702.95713728744), VALUATION = Convert.ToDecimal(1125.36073996194) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName166", ISSUER_ID = "SampleIssuerId166", SECURITY_ID = "SampleSecurityId166", FINANCIAL = Convert.ToDecimal(81.4320391943411), VALUATION = Convert.ToDecimal(3269.46801542595) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName167", ISSUER_ID = "SampleIssuerId167", SECURITY_ID = "SampleSecurityId167", FINANCIAL = Convert.ToDecimal(4363.65545994883), VALUATION = Convert.ToDecimal(3379.67126368368) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName168", ISSUER_ID = "SampleIssuerId168", SECURITY_ID = "SampleSecurityId168", FINANCIAL = Convert.ToDecimal(7874.27958675017), VALUATION = Convert.ToDecimal(0.782345091807244) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName169", ISSUER_ID = "SampleIssuerId169", SECURITY_ID = "SampleSecurityId169", FINANCIAL = Convert.ToDecimal(1890.19440785546), VALUATION = Convert.ToDecimal(4375.97526172597) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName170", ISSUER_ID = "SampleIssuerId170", SECURITY_ID = "SampleSecurityId170", FINANCIAL = Convert.ToDecimal(186.710896936736), VALUATION = Convert.ToDecimal(439.852143847009) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName171", ISSUER_ID = "SampleIssuerId171", SECURITY_ID = "SampleSecurityId171", FINANCIAL = Convert.ToDecimal(2438.66183806953), VALUATION = Convert.ToDecimal(9170.31606060514) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName172", ISSUER_ID = "SampleIssuerId172", SECURITY_ID = "SampleSecurityId172", FINANCIAL = Convert.ToDecimal(889.589685230247), VALUATION = Convert.ToDecimal(3151.9305636573) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName173", ISSUER_ID = "SampleIssuerId173", SECURITY_ID = "SampleSecurityId173", FINANCIAL = Convert.ToDecimal(3884.35730809195), VALUATION = Convert.ToDecimal(3716.7456760163) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName174", ISSUER_ID = "SampleIssuerId174", SECURITY_ID = "SampleSecurityId174", FINANCIAL = Convert.ToDecimal(657.074141587835), VALUATION = Convert.ToDecimal(8564.71462107219) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName175", ISSUER_ID = "SampleIssuerId175", SECURITY_ID = "SampleSecurityId175", FINANCIAL = Convert.ToDecimal(356.311597447565), VALUATION = Convert.ToDecimal(279.866075732923) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName176", ISSUER_ID = "SampleIssuerId176", SECURITY_ID = "SampleSecurityId176", FINANCIAL = Convert.ToDecimal(1640.66437337673), VALUATION = Convert.ToDecimal(5547.67730444408) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName177", ISSUER_ID = "SampleIssuerId177", SECURITY_ID = "SampleSecurityId177", FINANCIAL = Convert.ToDecimal(6923.96792073811), VALUATION = Convert.ToDecimal(5378.96851353177) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName178", ISSUER_ID = "SampleIssuerId178", SECURITY_ID = "SampleSecurityId178", FINANCIAL = Convert.ToDecimal(6774.17608706879), VALUATION = Convert.ToDecimal(6470.17360963958) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName179", ISSUER_ID = "SampleIssuerId179", SECURITY_ID = "SampleSecurityId179", FINANCIAL = Convert.ToDecimal(2.24374608908239), VALUATION = Convert.ToDecimal(2033.50087458471) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName180", ISSUER_ID = "SampleIssuerId180", SECURITY_ID = "SampleSecurityId180", FINANCIAL = Convert.ToDecimal(212.589092741589), VALUATION = Convert.ToDecimal(3601.69055268993) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName181", ISSUER_ID = "SampleIssuerId181", SECURITY_ID = "SampleSecurityId181", FINANCIAL = Convert.ToDecimal(4031.95462369506), VALUATION = Convert.ToDecimal(1463.03852908178) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName182", ISSUER_ID = "SampleIssuerId182", SECURITY_ID = "SampleSecurityId182", FINANCIAL = Convert.ToDecimal(2489.57226263937), VALUATION = Convert.ToDecimal(1424.06416825942) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName183", ISSUER_ID = "SampleIssuerId183", SECURITY_ID = "SampleSecurityId183", FINANCIAL = Convert.ToDecimal(135.73274509301), VALUATION = Convert.ToDecimal(2437.13290115266) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName184", ISSUER_ID = "SampleIssuerId184", SECURITY_ID = "SampleSecurityId184", FINANCIAL = Convert.ToDecimal(298.94836790036), VALUATION = Convert.ToDecimal(5063.58729054591) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName185", ISSUER_ID = "SampleIssuerId185", SECURITY_ID = "SampleSecurityId185", FINANCIAL = Convert.ToDecimal(5898.91814487543), VALUATION = Convert.ToDecimal(3196.38160278778) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName186", ISSUER_ID = "SampleIssuerId186", SECURITY_ID = "SampleSecurityId186", FINANCIAL = Convert.ToDecimal(1297.10658118511), VALUATION = Convert.ToDecimal(3323.86844656734) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName187", ISSUER_ID = "SampleIssuerId187", SECURITY_ID = "SampleSecurityId187", FINANCIAL = Convert.ToDecimal(7236.62535990183), VALUATION = Convert.ToDecimal(427.774926968116) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName188", ISSUER_ID = "SampleIssuerId188", SECURITY_ID = "SampleSecurityId188", FINANCIAL = Convert.ToDecimal(5468.33373095504), VALUATION = Convert.ToDecimal(3697.1123534864) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName189", ISSUER_ID = "SampleIssuerId189", SECURITY_ID = "SampleSecurityId189", FINANCIAL = Convert.ToDecimal(312.68150033153), VALUATION = Convert.ToDecimal(7878.55468909852) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName190", ISSUER_ID = "SampleIssuerId190", SECURITY_ID = "SampleSecurityId190", FINANCIAL = Convert.ToDecimal(65.226832037331), VALUATION = Convert.ToDecimal(4008.73295915207) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName191", ISSUER_ID = "SampleIssuerId191", SECURITY_ID = "SampleSecurityId191", FINANCIAL = Convert.ToDecimal(2803.97121092688), VALUATION = Convert.ToDecimal(362.566320130948) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName192", ISSUER_ID = "SampleIssuerId192", SECURITY_ID = "SampleSecurityId192", FINANCIAL = Convert.ToDecimal(1850.49844057295), VALUATION = Convert.ToDecimal(2427.08299752292) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName193", ISSUER_ID = "SampleIssuerId193", SECURITY_ID = "SampleSecurityId193", FINANCIAL = Convert.ToDecimal(1413.47911174069), VALUATION = Convert.ToDecimal(2151.48689175838) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName194", ISSUER_ID = "SampleIssuerId194", SECURITY_ID = "SampleSecurityId194", FINANCIAL = Convert.ToDecimal(87.4233372824809), VALUATION = Convert.ToDecimal(6852.23241281457) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName195", ISSUER_ID = "SampleIssuerId195", SECURITY_ID = "SampleSecurityId195", FINANCIAL = Convert.ToDecimal(234.474249339084), VALUATION = Convert.ToDecimal(164.119010138145) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName196", ISSUER_ID = "SampleIssuerId196", SECURITY_ID = "SampleSecurityId196", FINANCIAL = Convert.ToDecimal(615.18638107326), VALUATION = Convert.ToDecimal(1909.32911281165) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName197", ISSUER_ID = "SampleIssuerId197", SECURITY_ID = "SampleSecurityId197", FINANCIAL = Convert.ToDecimal(1404.49008983459), VALUATION = Convert.ToDecimal(879.769035380245) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName198", ISSUER_ID = "SampleIssuerId198", SECURITY_ID = "SampleSecurityId198", FINANCIAL = Convert.ToDecimal(324.654956881965), VALUATION = Convert.ToDecimal(6715.98272045866) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName199", ISSUER_ID = "SampleIssuerId199", SECURITY_ID = "SampleSecurityId199", FINANCIAL = Convert.ToDecimal(2128.16008848234), VALUATION = Convert.ToDecimal(831.322654294865) });
                    //RatioComparisonInfoData.Add(new RatioComparisonData() { ISSUE_NAME = "SampleIssueName200", ISSUER_ID = "SampleIssuerId200", SECURITY_ID = "SampleSecurityId200", FINANCIAL = Convert.ToDecimal(1148.36211122312), VALUATION = Convert.ToDecimal(1665.62630036377) });

                    //RatioComparisonInfo = RatioComparisonInfoData;

                    //BusyIndicatorNotification();
                    #endregion

                    String contextSecurityXML = GetContextSecurityXML(result);
                    if (_dbInteractivity != null)
                    {
                        BusyIndicatorNotification(true, "Retrieving ratio comparison data for the selected security...");
                        _dbInteractivity.RetrieveRatioComparisonData(contextSecurityXML, RetrieveRatioComparisonDataCallbackMethod); 
                    }
                    
                }
                else
                {
                    Prompt.ShowDialog("No Issuer linked to the entity " + EntitySelectionInfo.LongName + " (" + EntitySelectionInfo.ShortName + " : " + EntitySelectionInfo.InstrumentID + ")");
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                    BusyIndicatorNotification();
                }
                
            }
            catch (Exception ex)
            {                
                Logging.LogException(_logger, ex);
                BusyIndicatorNotification();
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
