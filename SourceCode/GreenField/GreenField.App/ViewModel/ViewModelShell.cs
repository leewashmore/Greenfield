using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.ViewModel;
using GreenField.App.Helpers;
using GreenField.Common;
using GreenField.Common.Helper;
using GreenField.DashboardModule.Helpers;
using GreenField.DataContracts;
using GreenField.Gadgets.ViewModels;
using GreenField.Gadgets.Views;
using GreenField.ServiceCaller;
using GreenField.ServiceCaller.PerformanceDefinitions;
using GreenField.UserSession;
using System.Reflection;

namespace GreenField.App.ViewModel
{
    /// <summary>
    /// View model class for Shell
    /// </summary>
    [Export]
    public partial class ViewModelShell : NotificationObject
    {
        #region Fields
        /// <summary>
        /// Region Manager
        /// </summary>
        private IRegionManager regionManager;

        /// <summary>
        /// Manage Session service caller
        /// </summary>
        private IManageSessions manageSessions;

        /// <summary>
        /// Logging instance
        /// </summary>
        private ILoggerFacade logger;

        /// <summary>
        /// MEF Event Aggregator
        /// </summary>
        private IEventAggregator eventAggregator;

        /// <summary>
        /// Service caller instance
        /// </summary>
        private IDBInteractivity dbInteractivity;

        private Int32 activeCallCount = 0;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="regionManager">Prism IRegionManager</param>
        /// <param name="manageSessions">Service IManageSessions</param>
        /// <param name="logger">Service ILoggerFacade</param>
        /// <param name="eventAggregator">Prism IEventAggregator</param>
        /// <param name="dbInteractivity">Service IDBInteractivity</param>
        [ImportingConstructor]
        public ViewModelShell(IRegionManager regionManager, IManageSessions manageSessions,
            ILoggerFacade logger, IEventAggregator eventAggregator, IDBInteractivity dbInteractivity)
        {
            this.logger = logger;
            this.regionManager = regionManager;
            this.manageSessions = manageSessions;
            this.eventAggregator = eventAggregator;
            this.dbInteractivity = dbInteractivity;

            if (eventAggregator != null)
            {
                eventAggregator.GetEvent<MarketPerformanceSnapshotActionCompletionEvent>().Subscribe(HandleMarketPerformanceSnapshotActionCompletionEvent);
                eventAggregator.GetEvent<ToolboxUpdateEvent>().Subscribe(HandleToolboxUpdateEvent);
            }

            //SessionManager.SESSION = new Session() { UserName = "rvig", Roles = new List<string>() };
            //UserName = SessionManager.SESSION.UserName;

            if (manageSessions != null)
            {
                BusyIndicatorNotification(true, "Retrieving session information...");
                manageSessions.GetSession(GetSessionCallbackMethod);
            }
        }
        #endregion

        #region Properties
        #region UI Fields
        #region User/Role Management
        /// <summary>
        /// Property binding UserName TextBlock
        /// </summary>
        private string userName;
        public string UserName
        {
            get
            {
                if (userName == null)
                {
                    userName = SessionManager.SESSION != null ? SessionManager.SESSION.UserName : null;
                }
                return userName;
            }
            set
            {
                if (userName != value)
                    userName = value;
                RaisePropertyChanged(() => this.UserName);
            }
        }

        private String version;
        public String Version
        {
            get 
            {
                if (version == null)
                {
                    version = GetAssemblyVersion();
                }
                return version;                
            }
            set
            {
                version = value;
                RaisePropertyChanged(() => this.Version);
            }
        }        

        private Boolean roleIsICAdmin = false;
        public Boolean RoleIsICAdmin
        {
            get { return roleIsICAdmin; }
            set
            {
                roleIsICAdmin = value;
                RaisePropertyChanged(() => this.RoleIsICAdmin);
            }
        }

        private Boolean roleIsIC = false;
        public Boolean RoleIsIC
        {
            get { return roleIsIC; }
            set
            {
                roleIsIC = value;
                RaisePropertyChanged(() => this.RoleIsIC);
            }
        }
        #endregion

        #region Application Menu
        /// <summary>
        /// Stores true if Application Menu is expanded
        /// </summary>
        private Boolean isApplicationMenuExpanded;
        public Boolean IsApplicationMenuExpanded
        {
            get { return isApplicationMenuExpanded; }
            set
            {
                isApplicationMenuExpanded = value;
                RaisePropertyChanged(() => this.IsApplicationMenuExpanded);
            }
        }
        #endregion

        #region Payload
        /// <summary>
        /// Stores payload to be published through aggregate events
        /// </summary>
        private DashboardGadgetPayload selectorPayload;
        public DashboardGadgetPayload SelectorPayload
        {
            get
            {
                if (selectorPayload == null)
                    selectorPayload = new DashboardGadgetPayload();
                return selectorPayload;
            }
            set
            {
                selectorPayload = value;
            }
        }
        #endregion

        #region ToolBox
        #region Security Selector
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
        /// Stores selected security - Publishes SecurityReferenceSetEvent on set event
        /// </summary>
        private EntitySelectionData selectedSecurityInfo;
        public EntitySelectionData SelectedSecurityInfo
        {
            get { return selectedSecurityInfo; }
            set
            {
                if (selectedSecurityInfo != value)
                {
                    selectedSecurityInfo = value;
                    RaisePropertyChanged(() => this.SelectedSecurityInfo);
                    if (value != null)
                    {
                        SelectorPayload.EntitySelectionData = value;
                        eventAggregator.GetEvent<SecurityReferenceSetEvent>().Publish(value);
                    }
                }
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

        /// <summary>
        /// Stores visibility property of the security selector
        /// </summary>
        private Visibility securitySelectorVisibility = Visibility.Collapsed;
        public Visibility SecuritySelectorVisibility
        {
            get { return securitySelectorVisibility; }
            set
            {
                securitySelectorVisibility = value;
                RaisePropertyChanged(() => this.SecuritySelectorVisibility);
                if (value == Visibility.Visible && dbInteractivity != null && (EntitySelectionInfo == null || EntitySelectionInfo.Count == 0))
                {
                    BusyIndicatorNotification(true, "Retrieving reference data...");
                    dbInteractivity.RetrieveEntitySelectionData(RetrieveEntitySelectionDataCallbackMethod);
                }
            }
        }
        #endregion

        #region Portfolio Selector
        /// <summary>
        /// Stores the list of PortfolioSelectionData for all portfolios
        /// </summary>
        private List<PortfolioSelectionData> portfolioSelectionInfo;
        public List<PortfolioSelectionData> PortfolioSelectionInfo
        {
            get { return portfolioSelectionInfo; }
            set
            {
                portfolioSelectionInfo = value;
                RaisePropertyChanged(() => this.PortfolioSelectionInfo);
                PortfolioSelectorInfo = value;
            }
        }

        /// <summary>
        /// Stores the list of PortfolioSelectionData for selector
        /// </summary>
        private List<PortfolioSelectionData> portfolioSelectorInfo;
        public List<PortfolioSelectionData> PortfolioSelectorInfo
        {
            get { return portfolioSelectorInfo; }
            set
            {
                portfolioSelectorInfo = value;
                RaisePropertyChanged(() => this.PortfolioSelectorInfo);
            }
        }

        /// <summary>
        /// Stores selected portfolio - Publishes PortfolioReferenceSetEvent on set event
        /// </summary>
        private PortfolioSelectionData selectedPortfolioInfo;
        public PortfolioSelectionData SelectedPortfolioInfo
        {
            get { return selectedPortfolioInfo; }
            set
            {
                if (selectedPortfolioInfo != value)
                {
                    selectedPortfolioInfo = value;
                    RaisePropertyChanged(() => this.SelectedPortfolioInfo);
                    if (value != null)
                    {
                        SelectorPayload.PortfolioSelectionData = value;
                        if (dbInteractivity != null && filterValueVisibility == Visibility.Visible && SelectedPortfolioInfo != null)
                        {
                            BusyIndicatorNotification(true, "Retrieving reference data...", false);
                            dbInteractivity.RetrieveFilterSelectionData(value, SelectedEffectiveDateInfo, RetrieveFilterSelectionDataCallbackMethod);
                        }
                        eventAggregator.GetEvent<PortfolioReferenceSetEvent>().Publish(value);                        
                    }
                }
            }
        }

        /// <summary>
        /// Stores search text entered by user - Refines PortfolioSelectionInfo based on the text entered
        /// </summary>
        private string portfolioSearchText;
        public string PortfolioSearchText
        {
            get { return portfolioSearchText; }
            set
            {
                if (value != null)
                {
                    portfolioSearchText = value;
                    RaisePropertyChanged(() => this.PortfolioSearchText);
                    if (value != String.Empty && PortfolioSelectionInfo != null)
                        PortfolioSelectorInfo = PortfolioSelectionInfo
                                    .Where(record => record.PortfolioId.ToLower().Contains(value.ToLower()))
                                    .ToList();
                    else
                        PortfolioSelectorInfo = PortfolioSelectionInfo;
                }
            }
        }

        /// <summary>
        /// Stores visibility property of the portfolio selector
        /// </summary>
        private Visibility portfolioSelectorVisibility = Visibility.Collapsed;
        public Visibility PortfolioSelectorVisibility
        {
            get { return portfolioSelectorVisibility; }
            set
            {
                portfolioSelectorVisibility = value;
                RaisePropertyChanged(() => this.PortfolioSelectorVisibility);
                if (value == Visibility.Visible && PortfolioSelectionInfo == null)
                {
                    BusyIndicatorNotification(true, "Retrieving reference data...");
                    dbInteractivity.RetrievePortfolioSelectionData(RetrievePortfolioSelectionDataCallbackMethod);
                    BusyIndicatorNotification(true, "Retrieving reference data...");
                    dbInteractivity.RetrieveAvailableDatesInPortfolios(RetrieveAvailableDatesInPortfoliosCallbackMethod);
                }
            }
        }
        #endregion

        #region Effective Date Selector
        /// <summary>
        /// Stores selected effective date - Publishes EffectiveDateReferenceSetEvent on set event
        /// </summary>
        private DateTime? selectedEffectiveDateInfo = DateTime.Today.AddDays(-1).Date;
        public DateTime? SelectedEffectiveDateInfo
        {
            get { return selectedEffectiveDateInfo; }
            set
            {
                selectedEffectiveDateInfo = value;
                RaisePropertyChanged(() => this.SelectedEffectiveDateInfo);
                if (value != null)
                {
                    SelectorPayload.EffectiveDate = Convert.ToDateTime(value);
                    if (dbInteractivity != null && filterValueVisibility == Visibility.Visible && SelectedPortfolioInfo != null)
                    {
                        BusyIndicatorNotification(true, "Retrieving reference data...", false);
                        dbInteractivity.RetrieveFilterSelectionData(SelectedPortfolioInfo, value, RetrieveFilterSelectionDataCallbackMethod);
                    }
                    eventAggregator.GetEvent<EffectiveDateReferenceSetEvent>().Publish(Convert.ToDateTime(value));                    
                }
            }
        }

        /// <summary>
        /// Stores visibility property of the effective date selector
        /// </summary>
        private Visibility effectiveDateSelectorVisibility = Visibility.Collapsed;
        public Visibility EffectiveDateSelectorVisibility
        {
            get { return effectiveDateSelectorVisibility; }
            set
            {
                effectiveDateSelectorVisibility = value;
                RaisePropertyChanged(() => this.EffectiveDateSelectorVisibility);
                if (value == Visibility.Collapsed)
                {
                    SelectedEffectiveDateInfo = null;
                }
            }
        }

        /// <summary>
        /// Stores the list of available Dates For Portfolio
        /// </summary>
        private List<DateTime> availableDateList;
        public List<DateTime> AvailableDateList
        {
            get { return availableDateList; }
            set
            {
                availableDateList = value;
                RaisePropertyChanged(() => this.AvailableDateList);
                AvailableDateStringList = (from p in availableDateList
                                           select p.Date.ToShortDateString()).ToList();

            }
        }

        /// <summary>
        /// Stores the formatted value of Available dates in portfolio
        /// </summary>
        private List<String> availableDateStringList;
        public List<String> AvailableDateStringList
        {
            get { return availableDateStringList; }
            set
            {
                availableDateStringList = value;
                RaisePropertyChanged(() => this.AvailableDateStringList);
            }
        }

        private String selectedEffectiveDateString;
        public String SelectedEffectiveDateString
        {
            get { return selectedEffectiveDateString; }
            set
            {
                selectedEffectiveDateString = value;
                RaisePropertyChanged(() => this.SelectedEffectiveDateString);
                SelectedEffectiveDateInfo = Convert.ToDateTime(selectedEffectiveDateString);
            } 
        }

        #endregion

        #region Period Selector
        /// <summary>
        /// Period Type reference information
        /// </summary>
        public List<String> PeriodTypeInfo
        {
            get
            {
                return new List<String> { "1D", "1W", "MTD", "QTD", "YTD", "1Y" };
            }
        }

        /// <summary>
        /// Stores visibility property of the period selector
        /// </summary>
        private Visibility periodSelectorVisibility = Visibility.Collapsed;
        public Visibility PeriodSelectorVisibility
        {
            get { return periodSelectorVisibility; }
            set
            {
                periodSelectorVisibility = value;
                RaisePropertyChanged(() => this.PeriodSelectorVisibility);
            }
        }

        /// <summary>
        /// String that contains the selected filter type
        /// </summary>
        private String selectedPeriodType = "YTD";
        public String SelectedPeriodType
        {
            get
            {
                return selectedPeriodType;
            }
            set
            {
                selectedPeriodType = value;
                RaisePropertyChanged(() => this.SelectedPeriodType);

                if (value != null)
                {
                    SelectorPayload.PeriodSelectionData = value;
                    eventAggregator.GetEvent<PeriodReferenceSetEvent>().Publish(value);
                }
            }
        }
        #endregion

        #region Region Selector
        /// <summary>
        /// Region Items information
        /// </summary>
        private ObservableCollection<RegionDataItem> regionItems;
        public ObservableCollection<RegionDataItem> RegionItems
        {
            get
            {
                return regionItems;
            }
            set
            {
                regionItems = value;
                RaisePropertyChanged(() => this.RegionItems);
            }
        }

        /// <summary>
        /// Region Type information
        /// </summary>
        private List<RegionSelectionData> regionTypeInfo;
        public List<RegionSelectionData> RegionTypeInfo
        {
            get
            {

                return regionTypeInfo;

            }
            set
            {
                regionTypeInfo = value;
                if (value != null)
                    AddItemsToRegionSelectorComboBox(value);

                RaisePropertyChanged(() => this.RegionTypeInfo);
            }
        }

        /// <summary>
        /// Region country information
        /// </summary>
        private List<String> regionCountryNames;
        public List<String> RegionCountryNames
        {
            get
            {
                if (regionCountryNames == null)
                {
                    regionCountryNames = new List<string>();
                }
                return regionCountryNames;
            }
            set
            {
                regionCountryNames = value;
                RaisePropertyChanged(() => this.RegionCountryNames);
                eventAggregator.GetEvent<RegionFXEvent>().Publish(value);

            }
        }

        /// <summary>
        /// Stores visibility property of the country selector
        /// </summary>
        private Visibility regionFXSelectorVisibility = Visibility.Collapsed;
        public Visibility RegionFXSelectorVisibility
        {
            get { return regionFXSelectorVisibility; }
            set
            {
                regionFXSelectorVisibility = value;
                RaisePropertyChanged(() => this.RegionFXSelectorVisibility);
                if (value == Visibility.Visible && RegionTypeInfo == null && dbInteractivity != null)
                {
                    BusyIndicatorNotification(true, "Retrieving reference data...");
                    dbInteractivity.RetrieveRegionSelectionData(RetrieveRegionSelectionCallbackMethod);
                }
            }
        }
        #endregion

        #region Country Selector
        /// <summary>
        /// Country type information
        /// </summary>
        private List<CountrySelectionData> countryTypeInfo;
        public List<CountrySelectionData> CountryTypeInfo
        {
            get
            {

                return countryTypeInfo;

            }
            set
            {
                countryTypeInfo = value;
                CountryName = value.OrderBy(t => t.CountryName).Select(t => t.CountryName).Distinct().ToList();
                RaisePropertyChanged(() => this.CountryTypeInfo);
            }
        }

        /// <summary>
        /// country name information
        /// </summary>
        private List<String> countryName;
        public List<String> CountryName
        {
            get
            {
                return countryName;

            }
            set
            {
                countryName = value;
                RaisePropertyChanged(() => this.CountryName);

            }
        }

        /// <summary>
        /// country code information
        /// </summary>
        private String countryCode;
        public String CountryCode
        {
            get
            {
                return countryCode;

            }
            set
            {
                countryCode = value;
                RaisePropertyChanged(() => this.CountryCode);
                if (value != null)
                {
                    for (int i = 0; i < CountryTypeInfo.Count; i++)
                    {
                        if (CountryTypeInfo[i].CountryName == value)
                        {
                            SelectorPayload.CountrySelectionData = CountryTypeInfo[i].CountryCode;
                            eventAggregator.GetEvent<CountrySelectionSetEvent>().Publish(SelectorPayload.CountrySelectionData);

                        }
                    }
                }

            }
        }

        /// <summary>
        /// Stores search text entered by user - Refines PortfolioSelectionInfo based on the text entered
        /// </summary>
        private string countrySearchText;
        public string CountrySearchText
        {
            get { return countrySearchText; }
            set
            {
                if (value != null)
                {
                    countrySearchText = value;
                    RaisePropertyChanged(() => this.CountrySearchText);
                    if (value != String.Empty && CountryTypeInfo != null)
                        CountryName = CountryTypeInfo
                                    .OrderBy(t => t.CountryName)
                                    .Where(record => record.CountryName.ToLower().Contains(value.ToLower()))
                                    .Select(record => record.CountryName).Distinct().ToList();
                    else
                        CountryName = CountryTypeInfo.OrderBy(t => t.CountryName).Select(t => t.CountryName).Distinct().ToList();
                }
            }
        }

        /// <summary>
        /// Stores visibility property of the country selector
        /// </summary>
        private Visibility countrySelectorVisibility = Visibility.Collapsed;
        public Visibility CountrySelectorVisibility
        {
            get { return countrySelectorVisibility; }
            set
            {
                countrySelectorVisibility = value;
                RaisePropertyChanged(() => this.CountrySelectorVisibility);
                if (value == Visibility.Visible && CountryTypeInfo == null && dbInteractivity != null)
                {
                    BusyIndicatorNotification(true, "Retrieving reference data...");
                    dbInteractivity.RetrieveCountrySelectionData(RetrieveCountrySelectionCallbackMethod);
                }

            }
        }
        #endregion

        #region Commodity
        /// <summary>
        /// Stores Commodity data
        /// </summary>
        private List<FXCommodityData> commodityTypeInfo;
        public List<FXCommodityData> CommodityTypeInfo
        {
            get { return commodityTypeInfo; }
            set
            {
                commodityTypeInfo = value;
                CommodityIDs = value.Select(l => l.CommodityId).Distinct().ToList();
                RaisePropertyChanged(() => this.CommodityTypeInfo);
            }
        }

        /// <summary>
        /// Stores Commodity ID values
        /// </summary>
        private List<String> commodityIDs;
        public List<String> CommodityIDs
        {
            get { return commodityIDs; }
            set
            {
                commodityIDs = value;
                if (String.IsNullOrEmpty(SelCommodityId))
                    SelCommodityId = value.LastOrDefault();
                RaisePropertyChanged(() => this.CommodityIDs);
            }

        }

        /// <summary>
        /// Stores selected Commodity ID values
        /// </summary>
        private string commodityID;
        public string CommodityID
        {
            get { return commodityID; }
            set
            {
                commodityID = value;
                RaisePropertyChanged(() => this.CommodityID);
            }
        }

        /// <summary>
        /// Stores commodity ID value selected by user
        /// </summary>
        private string selCommodityId;
        public string SelCommodityId
        {
            get { return selCommodityId; }
            set
            {
                selCommodityId = value;

                if (value != null)
                    //_selectorPayload.CommoditySelectedVal = CommodityTypeInfo.Where(rec => rec.CommodityID.ToUpper().Contains(value.ToUpper())).Select(rec => rec.CommodityID).ToString();
                    if (CommodityTypeInfo != null && CommodityTypeInfo.Count > 0)
                    {
                        foreach (FXCommodityData item in CommodityTypeInfo)
                        {
                            if (item.CommodityId.ToUpper() == value.ToUpper())
                                selectorPayload.CommoditySelectedVal = value;
                        }
                    }
                RaisePropertyChanged(() => this.SelCommodityId);
                eventAggregator.GetEvent<CommoditySelectionSetEvent>().Publish(selectorPayload.CommoditySelectedVal);
            }
        }

        /// <summary>
        /// Stores Commodity Selector Visibility
        /// </summary>
        private Visibility commoditySelectorVisibilty = Visibility.Collapsed;
        public Visibility CommoditySelectorVisibility
        {
            get { return commoditySelectorVisibilty; }
            set
            {
                commoditySelectorVisibilty = value;
                RaisePropertyChanged(() => this.CommoditySelectorVisibility);
                if (value == Visibility.Visible && CommodityTypeInfo == null && dbInteractivity != null)
                {
                    BusyIndicatorNotification(true, "Retrieving reference data...");
                    dbInteractivity.RetrieveCommoditySelectionData(RetrieveFXCommoditySelectionCallbackMethod);
                }
            }
        }
        #endregion

        #region Filter Selector
        /// <summary>
        /// Filter type reference information
        /// </summary>
        public List<string> FilterTypeInfo
        {
            get
            {
                return new List<string> { "Region", "Country", "Sector", "Industry", "Show Everything" };
            }
        }

        /// <summary>
        /// String that contains the selected filter type
        /// </summary>
        private String selectedfilterType;
        public String SelectedFilterType
        {
            get
            {
                return selectedfilterType;
            }
            set
            {
                selectedfilterType = value;
                RaisePropertyChanged(() => this.SelectedFilterType);
                if (FilterSelectionInfo != null)
                {
                    if (value == "Show Everything")
                    {
                        this.FilterValueVisibility = Visibility.Collapsed;
                        FilterSelectionData filterSelData = new FilterSelectionData();
                        filterSelData.Filtertype = value;
                        filterSelData.FilterValues = string.Empty;
                        SelectedFilterValueInfo = null;
                        SelectorPayload.FilterSelectionData = filterSelData;
                        IsExCashSecurity = false;
                        eventAggregator.GetEvent<HoldingFilterReferenceSetEvent>().Publish(SelectorPayload.FilterSelectionData);
                    }
                    else
                    {
                        this.FilterValueVisibility = Visibility.Visible;
                        FilterSelectorInfo = FilterSelectionInfo
                                            .Where(record => record.Filtertype == value)
                                            .ToList();
                    }
                }
            }
        }

        /// <summary>
        ///  Collection that contains the value types to be displayed in the combo box
        /// </summary>
        private List<FilterSelectionData> filterSelectionInfo;
        public List<FilterSelectionData> FilterSelectionInfo
        {
            get { return filterSelectionInfo; }
            set
            {
                if (filterSelectionInfo != value)
                {
                    filterSelectionInfo = value;
                    RaisePropertyChanged(() => this.FilterSelectionInfo);
                }
            }
        }

        /// <summary>
        ///  Collection that contains the value types to be displayed in the combo box
        /// </summary>
        private List<FilterSelectionData> filterSelectorInfo;
        public List<FilterSelectionData> FilterSelectorInfo
        {
            get { return filterSelectorInfo; }
            set
            {
                if (filterSelectorInfo != value)
                {
                    filterSelectorInfo = value;
                    RaisePropertyChanged(() => this.FilterSelectorInfo);
                }
            }
        }

        /// <summary>
        /// Stores selected Value - Publishes FilterReferenceSetEvent on set event
        /// </summary>
        private FilterSelectionData selectedFilterValueInfo;
        public FilterSelectionData SelectedFilterValueInfo
        {
            get { return selectedFilterValueInfo; }
            set
            {
                if (selectedFilterValueInfo != value)
                {
                    selectedFilterValueInfo = value;
                    RaisePropertyChanged(() => this.SelectedFilterValueInfo);
                    if (value != null)
                    {
                        SelectorPayload.FilterSelectionData = value;
                        eventAggregator.GetEvent<HoldingFilterReferenceSetEvent>().Publish(value);
                    }
                }
            }
        }

        /// <summary>
        /// Stores visibility property of the filter selector for holdings pie chart
        /// </summary>
        private Visibility filterTypeVisibility = Visibility.Collapsed;
        public Visibility FilterTypeVisibility
        {
            get { return filterTypeVisibility; }
            set
            {
                filterTypeVisibility = value;
                RaisePropertyChanged(() => this.FilterTypeVisibility);
                if (value == Visibility.Visible && FilterSelectionInfo == null)
                {
                    if (dbInteractivity != null && SelectedEffectiveDateInfo != null)
                    {
                        BusyIndicatorNotification(true, "Retrieving reference data...", false);
                        dbInteractivity.RetrieveFilterSelectionData(SelectedPortfolioInfo, SelectedEffectiveDateInfo, RetrieveFilterSelectionDataCallbackMethod);
                    }
                }
            }
        }

        /// <summary>
        /// Stores visibility property of the filter selector for holdings pie chart
        /// </summary>
        private Visibility filterValueVisibility = Visibility.Collapsed;
        public Visibility FilterValueVisibility
        {
            get { return filterValueVisibility; }
            set
            {
                filterValueVisibility = value;
                RaisePropertyChanged(() => this.FilterValueVisibility);
            }
        }

        /// <summary>
        /// Stores visibility property of the filter selector for holdings pie chart
        /// </summary>
        private Visibility marketCapCashSelectorVisibility = Visibility.Collapsed;
        public Visibility MarketCapCashSelectorVisibility
        {
            get { return marketCapCashSelectorVisibility; }
            set
            {
                marketCapCashSelectorVisibility = value;
                RaisePropertyChanged(() => this.MarketCapCashSelectorVisibility);
            }
        }
        #endregion

        #region Snapshot Selector
        /// <summary>
        /// Stores the list of MarketSnapshotSelectionData for user
        /// </summary>
        private List<MarketSnapshotSelectionData> marketSnapshotSelectionInfo;
        public List<MarketSnapshotSelectionData> MarketSnapshotSelectionInfo
        {
            get { return marketSnapshotSelectionInfo; }
            set
            {
                if (marketSnapshotSelectionInfo != value)
                {
                    marketSnapshotSelectionInfo = value;
                    RaisePropertyChanged(() => MarketSnapshotSelectionInfo);
                    MarketSnapshotSelectorInfo = value;
                }
            }
        }

        /// <summary>
        /// Stores the list of MarketSnapshotSelectionData for selector
        /// </summary>
        private List<MarketSnapshotSelectionData> marketSnapshotSelectorInfo;
        public List<MarketSnapshotSelectionData> MarketSnapshotSelectorInfo
        {
            get { return marketSnapshotSelectorInfo; }
            set
            {
                if (marketSnapshotSelectorInfo != value)
                {
                    marketSnapshotSelectorInfo = value;
                    RaisePropertyChanged(() => MarketSnapshotSelectorInfo);
                }
            }
        }


        /// <summary>
        /// Stores selected snapshot - Publishes MarketPerformanceSnapshotReferenceSetEvent on set event
        /// </summary>
        private MarketSnapshotSelectionData selectedMarketSnapshotSelectionInfo;
        public MarketSnapshotSelectionData SelectedMarketSnapshotSelectionInfo
        {
            get { return selectedMarketSnapshotSelectionInfo; }
            set
            {
                if (selectedMarketSnapshotSelectionInfo != value)
                {
                    selectedMarketSnapshotSelectionInfo = value;
                    RaisePropertyChanged(() => SelectedMarketSnapshotSelectionInfo);
                    RaisePropertyChanged(() => this.MarketSnapshotSaveCommand);
                    RaisePropertyChanged(() => this.MarketSnapshotRemoveCommand);
                    if (value != null)
                    {
                        if (MarketPerformanceSnapshotSearchText != value.SnapshotName)
                        {
                            MarketPerformanceSnapshotSearchText = value.SnapshotName;
                        }
                        SelectorPayload.MarketSnapshotSelectionData = value;
                        eventAggregator.GetEvent<MarketPerformanceSnapshotReferenceSetEvent>().Publish(value);
                    }

                }
            }
        }

        /// <summary>
        /// Stores search text entered by user - Refines MarketSnapshotSelectionInfo based on the text entered
        /// </summary>
        private string marketPerformanceSnapshotSearchText;
        public string MarketPerformanceSnapshotSearchText
        {
            get { return marketPerformanceSnapshotSearchText; }
            set
            {
                marketPerformanceSnapshotSearchText = value;
                RaisePropertyChanged(() => this.MarketPerformanceSnapshotSearchText);
                if (value != String.Empty && value != null && MarketSnapshotSelectionInfo != null)
                    MarketSnapshotSelectorInfo = MarketSnapshotSelectionInfo
                                .Where(record => record.SnapshotName.ToLower().Contains(value.ToLower())).ToList();
                else
                    MarketSnapshotSelectorInfo = MarketSnapshotSelectionInfo;
            }
        }

        /// <summary>
        /// Stores visibility property of the snapshot selector
        /// </summary>
        private Visibility snapshotSelectorVisibility = Visibility.Collapsed;
        public Visibility SnapshotSelectorVisibility
        {
            get { return snapshotSelectorVisibility; }
            set
            {
                snapshotSelectorVisibility = value;
                RaisePropertyChanged(() => this.SnapshotSelectorVisibility);
                if (value == Visibility.Visible && MarketSnapshotSelectionInfo == null)
                {
                    RetrieveMarketSnapshotSelectionData();
                }
            }
        }

        /// <summary>
        /// Stores checked-unchecked value for ExCash checkbox
        /// </summary>
        private bool isExCashSecurity = false;
        public bool IsExCashSecurity
        {
            get { return isExCashSecurity; }
            set
            {
                isExCashSecurity = value;
                RaisePropertyChanged(() => this.IsExCashSecurity);
                if (SelectedFilterType == "Show Everything" && value == true)
                    SelectedFilterType = "";
                selectorPayload.IsExCashSecurityData = value;
                eventAggregator.GetEvent<ExCashSecuritySetEvent>().Publish(value);

            }
        }
        #endregion

        #region Cash/NoCash Selector
        /// <summary>
        /// Strores market cap excluding cash securities checkbox visibility
        /// </summary>
        private Visibility mktCapExCashSelectorVisibility = Visibility.Collapsed;
        public Visibility MktCapExCashSelectorVisibility
        {
            get { return mktCapExCashSelectorVisibility; }
            set
            {
                mktCapExCashSelectorVisibility = value;
                RaisePropertyChanged(() => this.MktCapExCashSelectorVisibility);
            }
        }
        #endregion

        #region NodeNameFilter Selector
        /// <summary>
        /// List of Node Names
        /// </summary>
        public List<String> NodeNameInfo
        {
            get
            {
                return new List<String> { "Country", "Sector", "Security" };
            }
        }

        /// <summary>
        /// Stores visibility property of the Node selector
        /// </summary>
        private Visibility nodeSelectorVisibility = Visibility.Collapsed;
        public Visibility NodeSelectorVisibility
        {
            get { return nodeSelectorVisibility; }
            set
            {
                nodeSelectorVisibility = value;
                RaisePropertyChanged(() => this.NodeSelectorVisibility);
            }
        }

        /// <summary>
        /// String that contains the selected Node Name
        /// </summary>
        private String selectedNodeName = "Country";
        public String SelectedNodeName
        {
            get
            {
                return selectedNodeName;
            }
            set
            {
                selectedNodeName = value;
                RaisePropertyChanged(() => this.SelectedNodeName);

                if (value != null)
                {
                    SelectorPayload.NodeNameSelectionData = value;
                    eventAggregator.GetEvent<NodeNameReferenceSetEvent>().Publish(value);
                }
            }
        }
        #endregion

        #region LookThruSelector
        /// <summary>
        /// Visibility of LookThru Selector
        /// </summary>
        private Visibility lookThruSelectorVisibility = Visibility.Collapsed;
        public Visibility LookThruSelectorVisibility
        {
            get { return lookThruSelectorVisibility; }
            set
            {
                lookThruSelectorVisibility = value;
                this.RaisePropertyChanged(() => this.LookThruSelectorVisibility);
            }
        }

        /// <summary>
        /// Stores checked/unchecked value of LookThru Selector
        /// </summary>
        private bool isLookThruEnabled = false;
        public bool IsLookThruEnabled
        {
            get
            {
                return isLookThruEnabled;
            }
            set
            {
                isLookThruEnabled = value;
                this.RaisePropertyChanged(() => this.IsLookThruEnabled);
                selectorPayload.IsLookThruEnabled = value;
                eventAggregator.GetEvent<LookThruFilterReferenceSetEvent>().Publish(value);
            }
        }
        #endregion

        #region Gadget Selector
        /// <summary>
        /// Stores gadget information for dashboard
        /// </summary>
        private List<GadgetInfo> gadgetSelectorStoreOffInfo;
        public List<GadgetInfo> GadgetSelectorStoreOffInfo
        {
            get
            {
                if (gadgetSelectorStoreOffInfo == null)
                {
                    gadgetSelectorStoreOffInfo = GadgetListing.Info;
                }
                return gadgetSelectorStoreOffInfo;
            }
            set
            {
                gadgetSelectorStoreOffInfo = value;
                if (value == null)
                {
                    GadgetSelectorInfo = null;
                }
                else
                {
                    if (GadgetSearchText != String.Empty && value != null)
                    {
                        GadgetSelectorInfo = value
                                    .Where(record => record.DisplayName.ToLower().Contains(GadgetSearchText.ToLower()))
                                    .ToList();
                    }
                    else
                    {
                        GadgetSelectorInfo = value;
                    }
                }
            }

        }

        /// <summary>
        /// Stores gadget information for dashboard
        /// </summary>
        private List<GadgetInfo> gadgetSelectorInfo;
        public List<GadgetInfo> GadgetSelectorInfo
        {
            get
            {
                if (gadgetSelectorInfo == null)
                {
                    gadgetSelectorInfo = GadgetListing.Info;
                }
                return gadgetSelectorInfo;
            }
            set
            {
                gadgetSelectorInfo = value;
                RaisePropertyChanged(() => this.GadgetSelectorInfo);
            }
        }

        /// <summary>
        /// Stores selected gadget information - Publishes DashboardTileViewItemAdded on set event
        /// </summary>
        private GadgetInfo selectedGadgetInfo;
        public GadgetInfo SelectedGadgetInfo
        {
            get { return selectedGadgetInfo; }
            set
            {
                if (selectedGadgetInfo != value)
                {
                    selectedGadgetInfo = value;
                    RaisePropertyChanged(() => this.SelectedGadgetInfo);
                    if (value != null)
                    {
                        IsApplicationMenuExpanded = false;
                        if (value.DisplayName == GadgetNames.HOLDINGS_DISCOUNTED_CASH_FLOW)
                        {
                            Type[] argumentTypes = new Type[] { typeof(DashboardGadgetParam) };
                            object[] argumentValues = new object[] { GetDashboardGadgetParam() };
                            Object viewModelObject = TypeResolution.GetNewTypeObject(typeof(ViewModelDCF), argumentTypes, argumentValues);
                            GadgetInfo gadgetInfo = new GadgetInfo()
                            {
                                DisplayName = GadgetNames.HOLDINGS_DISCOUNTED_CASH_FLOW_ASSUMPTIONS,
                                ViewType = typeof(ViewAnalysisSummary),
                                ViewModelType = typeof(ViewModelDCF)
                            };
                            AddItemToUserDashboard(gadgetInfo, viewModelObject);
                            gadgetInfo = new GadgetInfo()
                            {
                                DisplayName = GadgetNames.HOLDINGS_FREE_CASH_FLOW,
                                ViewType = typeof(ViewFreeCashFlows),
                                ViewModelType = typeof(ViewModelFreeCashFlows)
                            };
                            AddItemToUserDashboard(gadgetInfo);
                            gadgetInfo = new GadgetInfo()
                            {
                                DisplayName = GadgetNames.HOLDINGS_DISCOUNTED_CASH_FLOW_TERMINAL_VALUE_CALCULATIONS,
                                ViewType = typeof(ViewTerminalValueCalculations),
                                ViewModelType = typeof(ViewModelDCF)
                            };
                            AddItemToUserDashboard(gadgetInfo, viewModelObject);
                            gadgetInfo = new GadgetInfo()
                            {
                                DisplayName = GadgetNames.HOLDINGS_DISCOUNTED_CASH_FLOW_SUMMARY,
                                ViewType = typeof(ViewDCFSummary),
                                ViewModelType = typeof(ViewModelDCF)
                            };
                            AddItemToUserDashboard(gadgetInfo, viewModelObject);
                            gadgetInfo = new GadgetInfo()
                            {
                                DisplayName = GadgetNames.HOLDINGS_DISCOUNTED_CASH_FLOW_SENSIVITY,
                                ViewType = typeof(ViewSensitivity),
                                ViewModelType = typeof(ViewModelDCF)
                            };
                            AddItemToUserDashboard(gadgetInfo, viewModelObject);
                            gadgetInfo = new GadgetInfo()
                            {
                                DisplayName = GadgetNames.HOLDINGS_DISCOUNTED_CASH_FLOW_FORWARD_EPS,
                                ViewType = typeof(ViewSensitivityEPS),
                                ViewModelType = typeof(ViewModelDCF)
                            };
                            AddItemToUserDashboard(gadgetInfo, viewModelObject);
                            gadgetInfo = new GadgetInfo()
                            {
                                DisplayName = GadgetNames.HOLDINGS_DISCOUNTED_CASH_FLOW_FORWARD_BVPS,
                                ViewType = typeof(ViewSensitivityBVPS),
                                ViewModelType = typeof(ViewModelDCF)
                            };
                            AddItemToUserDashboard(gadgetInfo, viewModelObject);
                        }
                        else
                        {
                            AddItemToUserDashboard(value);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Stores search text entered by user - Refines SelectedGadgetInfo based on the text entered
        /// </summary>
        private string gadgetSearchText;
        public string GadgetSearchText
        {
            get { return gadgetSearchText; }
            set
            {
                gadgetSearchText = value;
                RaisePropertyChanged(() => this.GadgetSearchText);
                if (value != null)
                {
                    if (value != String.Empty && GadgetSelectorStoreOffInfo != null)
                    {
                        GadgetSelectorInfo = GadgetSelectorStoreOffInfo
                                    .Where(record => record.DisplayName.ToLower().Contains(value.ToLower()))
                                    .ToList();
                    }
                    else
                    {
                        GadgetSelectorInfo = GadgetSelectorStoreOffInfo;
                    }
                }
            }
        }

        /// <summary>
        /// Stores visibility property of the security selector
        /// </summary>
        private Visibility gadgetSelectorVisibility = Visibility.Collapsed;
        public Visibility GadgetSelectorVisibility
        {
            get { return gadgetSelectorVisibility; }
            set
            {
                gadgetSelectorVisibility = value;
                RaisePropertyChanged(() => this.GadgetSelectorVisibility);
            }
        }
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
        /// Displays/Hides busy indicator to notify user of the on going process
        /// </summary>
        private bool isFilterBusyIndicatorBusy = false;
        public bool IsFilterBusyIndicatorBusy
        {
            get { return isFilterBusyIndicatorBusy; }
            set
            {
                isFilterBusyIndicatorBusy = value;
                RaisePropertyChanged(() => this.IsFilterBusyIndicatorBusy);
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

        #region ICommand
        /// <summary>
        /// LogOutCommand
        /// </summary>
        public ICommand LogOutCommand
        {
            get { return new DelegateCommand<object>(LogOutCommandMethod); }
        }

        #region Dashboard
        
        #region Company
        #region Snapshot
        /// <summary>
        /// DashboardCompanySnapshotSummaryCommand
        /// </summary>
        public ICommand DashboardCompanySnapshotSummaryCommand
        {
            get { return new DelegateCommand<object>(DashboardCompanySnapshotSummaryCommandMethod); }
        }

        /// <summary>
        /// DashboardCompanySnapshotCompanyProfileCommand
        /// </summary>
        public ICommand DashboardCompanySnapshotCompanyProfileCommand
        {
            get { return new DelegateCommand<object>(DashboardCompanySnapshotCompanyProfileCommandMethod); }
        }

        /// <summary>
        /// DashboardCompanySnapshotTearSheetCommand
        /// </summary>
        public ICommand DashboardCompanySnapshotTearSheetCommand
        {
            get { return new DelegateCommand<object>(DashboardCompanySnapshotTearSheetCommandMethod); }
        }

        /// <summary>
        /// DashboardCompanyBasicDataCommand
        /// </summary>
        public ICommand DashboardCompanyBasicDataCommand
        {
            get
            {
                return new DelegateCommand<object>(DashboardCompanySnapshotBasicDataCommandMethod);
            }
        }

        /// <summary>
        /// DashboardConsensusEstimateSummaryCommand
        /// </summary>
        public ICommand DashboardConsensusEstimateSummaryCommand
        {
            get
            {
                return new DelegateCommand<object>(DashboardConsensusEstimateSummaryCommandMethod);
            }
        }
        #endregion

        #region Financials
        /// <summary>
        /// DashboardCompanyFinancialsSummaryCommand
        /// </summary>
        public ICommand DashboardCompanyFinancialsSummaryCommand
        {
            get { return new DelegateCommand<object>(DashboardCompanyFinancialsSummaryCommandMethod); }
        }

        /// <summary>
        /// DashboardCompanyFinancialsIncomeStatementCommand
        /// </summary>
        public ICommand DashboardCompanyFinancialsIncomeStatementCommand
        {
            get { return new DelegateCommand<object>(DashboardCompanyFinancialsIncomeStatementCommandMethod); }
        }

        /// <summary>
        /// DashboardCompanyFinancialsBalanceSheetCommand
        /// </summary>
        public ICommand DashboardCompanyFinancialsBalanceSheetCommand
        {
            get { return new DelegateCommand<object>(DashboardCompanyFinancialsBalanceSheetCommandMethod); }
        }

        /// <summary>
        /// DashboardCompanyFinancialsCashFlowCommand
        /// </summary>
        public ICommand DashboardCompanyFinancialsCashFlowCommand
        {
            get { return new DelegateCommand<object>(DashboardCompanyFinancialsCashFlowCommandMethod); }
        }

        /// <summary>
        /// DashboardCompanyFinstatCommand
        /// </summary>
        public ICommand DashboardCompanyFinstatCommand
        {
            get { return new DelegateCommand<object>(DashboardCompanyFinstatCommandMethod); }
        }

        /// <summary>
        /// DashboardCompanyFinancialsFinStatCommand
        /// </summary>
        public ICommand DashboardCompanyFinancialsFinStatCommand
        {
            get { return new DelegateCommand<object>(DashboardCompanyFinancialsFinStatCommandMethod); }
        }

        /// <summary>
        /// DashboardCompanyFinancialsPeerComparisonCommand
        /// </summary>
        public ICommand DashboardCompanyFinancialsPeerComparisonCommand
        {
            get { return new DelegateCommand<object>(DashboardCompanyFinancialsPeerComparisonCommandMethod); }
        }
        #endregion

        #region Estimates
        /// <summary>
        /// DashboardCompanyEstimatesConsensusCommand
        /// </summary>
        public ICommand DashboardCompanyEstimatesConsensusCommand
        {
            get { return new DelegateCommand<object>(DashboardCompanyEstimatesConsensusCommandMethod); }
        }

        /// <summary>
        /// DashboardCompanyEstimatesDetailedCommand
        /// </summary>
        public ICommand DashboardCompanyEstimatesDetailedCommand
        {
            get { return new DelegateCommand<object>(DashboardCompanyEstimatesDetailedCommandMethod); }
        }

        /// <summary>
        /// DashboardCompanyEstimatesComparisonCommand
        /// </summary>
        public ICommand DashboardCompanyEstimatesComparisonCommand
        {
            get { return new DelegateCommand<object>(DashboardCompanyEstimatesComparisonCommandMethod); }
        }
        #endregion

        #region Valuation
        /// <summary>
        /// DashboardCompanyValuationFairValueCommand
        /// </summary>
        public ICommand DashboardCompanyValuationFairValueCommand
        {
            get { return new DelegateCommand<object>(DashboardCompanyValuationFairValueCommandMethod); }
        }

        /// <summary>
        /// DashboardCompanyValuationDiscountedCashFlowCommand
        /// </summary>
        public ICommand DashboardCompanyValuationDiscountedCashFlowCommand
        {
            get { return new DelegateCommand<object>(DashboardCompanyValuationDiscountedCashFlowCommandMethod); }
        }
        #endregion

        #region Documents
        /// <summary>
        /// DashboardCompanyDocumentsCommand
        /// </summary>
        public ICommand DashboardCompanyDocumentsCommand
        {
            get { return new DelegateCommand<object>(DashboardCompanyDocumentsCommandMethod); }
        }

        /// <summary>
        /// DashboardCompanyDocumentsLoadModelCommand
        /// </summary>
        public ICommand DashboardCompanyDocumentsLoadModelCommand
        {
            get { return new DelegateCommand<object>(DashboardCompanyDocumentsLoadModelCommandMethod); }
        }
        #endregion

        #region Charting
        /// <summary>
        /// DashboardCompanyChartingClosingPriceCommand
        /// </summary>
        public ICommand DashboardCompanyChartingClosingPriceCommand
        {
            get { return new DelegateCommand<object>(DashboardCompanyChartingClosingPriceCommandMethod); }
        }

        /// <summary>
        /// DashboardCompanyChartingUnrealizedGainCommand
        /// </summary>
        public ICommand DashboardCompanyChartingUnrealizedGainCommand
        {
            get { return new DelegateCommand<object>(DashboardCompanyChartingUnrealizedGainCommandMethod); }
        }

        /// <summary>
        /// DashboardCompanyChartingContextCommand
        /// </summary>
        public ICommand DashboardCompanyChartingContextCommand
        {
            get { return new DelegateCommand<object>(DashboardCompanyChartingContextCommandMethod); }
        }

        /// <summary>
        /// DashboardCompanyChartingValuationCommand
        /// </summary>
        public ICommand DashboardCompanyChartingValuationCommand
        {
            get { return new DelegateCommand<object>(DashboardCompanyChartingValuationCommandMethod); }
        }
        #endregion

        #region Corporate Governance
        /// <summary>
        /// DashboardCompanyCorporateGovernanceQuestionnaireCommand
        /// </summary>
        public ICommand DashboardCompanyCorporateGovernanceQuestionnaireCommand
        {
            get { return new DelegateCommand<object>(DashboardCompanyCorporateGovernanceQuestionnaireCommandMethod); }
        }

        /// <summary>
        /// DashboardCompanyCorporateGovernanceReportCommand
        /// </summary>
        public ICommand DashboardCompanyCorporateGovernanceReportCommand
        {
            get { return new DelegateCommand<object>(DashboardCompanyCorporateGovernanceReportCommandMethod); }
        }
        #endregion
        #endregion

        #region Markets
        #region Snapshot
        /// <summary>
        /// DashboardMarketsSnapshotSummaryCommand
        /// </summary>
        public ICommand DashboardMarketsSnapshotSummaryCommand
        {
            get { return new DelegateCommand<object>(DashboardMarketsSnapshotSummaryCommandMethod); }
        }

        /// <summary>
        /// DashboardMarketsSnapshotMarketPerformanceCommand
        /// </summary>
        public ICommand DashboardMarketsSnapshotMarketPerformanceCommand
        {
            get { return new DelegateCommand<object>(DashboardMarketsSnapshotMarketPerformanceCommandMethod); }
        }

        /// <summary>
        /// DashboardMarketsSnapshotInternalModelValuationCommand
        /// </summary>
        public ICommand DashboardMarketsSnapshotInternalModelValuationCommand
        {
            get { return new DelegateCommand<object>(DashboardMarketsSnapshotInternalModelValuationCommandMethod); }
        }
        #endregion

        #region MacroEconomic
        /// <summary>
        /// DashboardMarketsMacroEconomicsEMSummaryCommand
        /// </summary>
        public ICommand DashboardMarketsMacroEconomicsEMSummaryCommand
        {
            get { return new DelegateCommand<object>(DashboardMarketsMacroEconomicsEMSummaryCommandMethod); }
        }

        /// <summary>
        /// DashboardMarketsMacroEconomicsCountrySummaryCommand
        /// </summary>
        public ICommand DashboardMarketsMacroEconomicsCountrySummaryCommand
        {
            get { return new DelegateCommand<object>(DashboardMarketsMacroEconomicsCountrySummaryCommandMethod); }
        }
        #endregion

        #region Commodities
        /// <summary>
        /// DashboardMarketsCommoditiesSummaryCommand
        /// </summary>
        public ICommand DashboardMarketsCommoditiesSummaryCommand
        {
            get { return new DelegateCommand<object>(DashboardMarketsCommoditiesSummaryCommandMethod); }
        }
        #endregion
        #endregion

        #region Portfolio
        #region Snapshot
        /// <summary>
        /// DashboardPortfolioSnapshotCommand
        /// </summary>
        public ICommand DashboardPortfolioSnapshotCommand
        {
            get { return new DelegateCommand<object>(DashboardPortfolioSnapshotCommandMethod); }
        }
        #endregion

        #region Holdings
        /// <summary>
        /// DashboardPortfolioHoldingsCommand
        /// </summary>
        public ICommand DashboardPortfolioHoldingsCommand
        {
            get { return new DelegateCommand<object>(DashboardPortfolioHoldingsCommandMethod); }
        }
        #endregion

        #region Performance
        /// <summary>
        /// DashboardPortfolioPerformanceSummaryCommand
        /// </summary>
        public ICommand DashboardPortfolioPerformanceSummaryCommand
        {
            get { return new DelegateCommand<object>(DashboardPortfolioPerformanceSummaryCommandMethod); }
        }

        /// <summary>
        /// DashboardPortfolioPerformanceAttributionCommand
        /// </summary>
        public ICommand DashboardPortfolioPerformanceAttributionCommand
        {
            get { return new DelegateCommand<object>(DashboardPortfolioPerformanceAttributionCommandMethod); }
        }

        /// <summary>
        /// DashboardPortfolioPerformanceRelativePerformanceCommand
        /// </summary>
        public ICommand DashboardPortfolioPerformanceRelativePerformanceCommand
        {
            get { return new DelegateCommand<object>(DashboardPortfolioPerformanceRelativePerformanceCommandMethod); }
        }
        #endregion

        #region Benchmark
        /// <summary>
        /// DashboardPortfolioBenchmarkSummaryCommand
        /// </summary>
        public ICommand DashboardPortfolioBenchmarkSummaryCommand
        {
            get { return new DelegateCommand<object>(DashboardPortfolioBenchmarkSummaryCommandMethod); }
        }

        /// <summary>
        /// DashboardPortfolioBenchmarkComponentsCommand
        /// </summary>
        public ICommand DashboardPortfolioBenchmarkComponentsCommand
        {
            get { return new DelegateCommand<object>(DashboardPortfolioBenchmarkComponentsCommandMethod); }
        }
        #endregion
        #endregion

        #region Screening
        #region QuarterlyResultsComparison
        /// <summary>
        /// DashboardQuarterlyResultsComparisonCommand
        /// </summary>
        public ICommand DashboardQuarterlyResultsComparisonCommand
        {
            get { return new DelegateCommand<object>(DashboardQuarterlyResultsComparisonCommandMethod); }
        }
        #endregion

        #region Stock
        /// <summary>
        /// DashboardCustomScreeningToolCommand
        /// </summary>
        public ICommand DashboardCustomScreeningToolCommand
        {
            get { return new DelegateCommand<object>(DashboardCustomScreeningToolCommandMethod); }
        }
        #endregion
        #endregion

        #region Investment Committee
        /// <summary>
        /// DashboardInvestmentCommitteeCreateEditCommand
        /// </summary>
        public ICommand DashboardInvestmentCommitteeCreateEditCommand
        {
            get { return new DelegateCommand<object>(DashboardInvestmentCommitteeCreateEditCommandMethod); }
        }

        /// <summary>
        /// DashboardInvestmentCommitteeVoteCommand
        /// </summary>
        public ICommand DashboardInvestmentCommitteeVoteCommand
        {
            get { return new DelegateCommand<object>(DashboardInvestmentCommitteeVoteCommandMethod); }
        }

        /// <summary>
        /// DashboardInvestmentCommitteePreMeetingReportCommand
        /// </summary>
        public ICommand DashboardInvestmentCommitteePreMeetingReportCommand
        {
            get { return new DelegateCommand<object>(DashboardInvestmentCommitteePreMeetingReportCommandMethod); }
        }

        /// <summary>
        /// DashboardInvestmentCommitteeMeetingMinutesCommand
        /// </summary>
        public ICommand DashboardInvestmentCommitteeMeetingMinutesCommand
        {
            get { return new DelegateCommand<object>(DashboardInvestmentCommitteeMeetingMinutesCommandMethod); }
        }

        /// <summary>
        /// DashboardInvestmentCommitteeSummaryReportCommand
        /// </summary>
        public ICommand DashboardInvestmentCommitteeSummaryReportCommand
        {
            get { return new DelegateCommand<object>(DashboardInvestmentCommitteeSummaryReportCommandMethod); }
        }

        /// <summary>
        /// DashboardInvestmentCommitteeMetricsReportCommand
        /// </summary>
        public ICommand DashboardInvestmentCommitteeMetricsReportCommand
        {
            get { return new DelegateCommand<object>(DashboardInvestmentCommitteeMetricsReportCommandMethod); }
        }
        #endregion

        #region Dashboard
        /// <summary>
        /// MyDashboardCommand
        /// </summary>
        public ICommand MyDashboardCommand
        {
            get
            {
                return new DelegateCommand<object>(MyDashboardCommandMethod);
            }
        }
        #endregion

        // targeting commands

        public ICommand TargetingBroadGlobalActiveCommand
        {
            get
            {
                var command = new DelegateCommand<Object>(TargetingBroadGlobalActiveCommandMethod);
                return command;
            }
        }

        public ICommand TargetingBottomUpCommand
        {
            get
            {
                var command = new DelegateCommand<Object>(TargetingBottomUpCommandMethod);
                return command;
            }
        }

        public ICommand TargetingBasketTargetsCommand
        {
            get
            {
                var command = new DelegateCommand<Object>(TargetingBasketTargetsCommandMethod);
                return command;
            }
        }

        #region Admin
        #region Investment Committee
        /// <summary>
        /// DashboardAdminInvestmentCommitteeChangeDateCommand
        /// </summary>
        public ICommand DashboardAdminInvestmentCommitteeChangeDateCommand
        {
            get { return new DelegateCommand<object>(DashboardAdminInvestmentCommitteeChangeDateCommandMethod); }
        }
        #endregion
        #endregion

        
        
        #region Others
        /// <summary>
        /// UserManagementCommand
        /// </summary>
        public ICommand UserManagementCommand
        {
            get
            {
                return new DelegateCommand<object>(UserManagementCommandMethod);
            }
        }

        /// <summary>
        /// RoleManagementCommand
        /// </summary>
        public ICommand RoleManagementCommand
        {
            get
            {
                return new DelegateCommand<object>(RoleManagementCommandMethod);
            }
        }
        #endregion


        #endregion

        #region ToolBox
        /// <summary>
        /// UserDashboardSaveCommand
        /// </summary>
        public ICommand UserDashboardSaveCommand
        {
            get
            {
                return new DelegateCommand<object>(UserDashboardSaveCommandMethod);
            }
        }

        /// <summary>
        /// MarketSnapshotSaveCommand
        /// </summary>
        public ICommand MarketSnapshotSaveCommand
        {
            get
            {
                return new DelegateCommand<object>(MarketSnapshotSaveCommandMethod, MarketSnapshotSaveCommandValidationMethod);
            }
        }

        /// <summary>
        /// MarketSnapshotSaveAsCommand
        /// </summary>
        public ICommand MarketSnapshotSaveAsCommand
        {
            get
            {
                return new DelegateCommand<object>(MarketSnapshotSaveAsCommandMethod);
            }
        }

        /// <summary>
        /// MarketSnapshotAddCommand
        /// </summary>
        public ICommand MarketSnapshotAddCommand
        {
            get
            {
                return new DelegateCommand<object>(MarketSnapshotAddCommandMethod);
            }
        }

        /// <summary>
        /// MarketSnapshotRemoveCommand
        /// </summary>
        public ICommand MarketSnapshotRemoveCommand
        {
            get
            {
                return new DelegateCommand<object>(MarketSnapshotRemoveCommandMethod, MarketSnapshotRemoveCommandValidationMethod);
            }
        }

        /// <summary>
        /// RetrieveRegionDataCommand
        /// </summary>
        public ICommand RetrieveRegionDataCommand
        {
            get
            {
                return new DelegateCommand<object>(RetrieveRegionDataCommandMethod);
            }
        }
        #endregion
        #endregion

        #endregion

        #region Event
        ///// <summary>
        ///// event to handle data retrieval progress indicator
        ///// </summary>
        //public event DataRetrievalProgressIndicatorEventHandler ShellDataLoadEvent;

        ///// <summary>
        ///// event to handle filter data retrieval progress indicator
        ///// </summary>
        //public event DataRetrievalProgressIndicatorEventHandler ShellFilterDataLoadEvent;

        ///// <summary>
        ///// event to handle snapshot selector data retrieval progress indicator
        ///// </summary>
        //public event DataRetrievalProgressIndicatorEventHandler ShellSnapshotDataLoadEvent;
        #endregion

        #region Event Handlers
        /// <summary>
        /// MarketPerformanceSnapshotActionCompletion Event Handler
        /// </summary>
        /// <param name="result">MarketPerformanceSnapshotActionPayload</param>
        public void HandleMarketPerformanceSnapshotActionCompletionEvent(MarketPerformanceSnapshotActionPayload result)
        {
            if (!(result.ActionType == MarketPerformanceSnapshotActionType.SNAPSHOT_PAGE_NAVIGATION))
            {
                MarketSnapshotSelectionInfo = result.MarketSnapshotSelectionInfo.OrderBy(record => record.SnapshotName).ToList();
                SelectedMarketSnapshotSelectionInfo = result.SelectedMarketSnapshotSelectionInfo;
            }
            else
            {
                SelectedMarketSnapshotSelectionInfo = null;
                //UpdateToolBoxSelectorVisibility();
            }

            RaisePropertyChanged(() => this.MarketSnapshotSaveCommand);
            RaisePropertyChanged(() => this.MarketSnapshotRemoveCommand);
        }

        /// <summary>
        /// ToolboxUpdate Event Handler
        /// </summary>
        /// <param name="result">DashboardCategoryType</param>
        public void HandleToolboxUpdateEvent(DashboardCategoryType result)
        {
            ToolBoxSelecter.SetToolBoxItemVisibility(result);
            UpdateToolBoxSelectorVisibility();
        }
        #endregion

        #region Callback Methods
        /// <summary>
        /// GetSession Callback Method
        /// </summary>
        /// <param name="result">Session object</param>
        private void GetSessionCallbackMethod(Session result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);

            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result.ToString(), 1);
                    Logging.LogSessionStart(logger);
                    SessionManager.SESSION = result;
                    UserName = SessionManager.SESSION.UserName;

                    if (result.Roles != null)
                    {
                        RoleIsICAdmin = result.Roles.Contains(MemberGroups.IC_ADMIN);
                        RoleIsIC = result.Roles.Any(record => record == MemberGroups.IC_ADMIN
                            || record == MemberGroups.IC_CHIEF_EXECUTIVE || record == MemberGroups.IC_VOTING_MEMBER
                            || record == MemberGroups.IC_NON_VOTING_MEMBER);
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
            finally
            {
                Logging.LogEndMethod(logger, methodNamespace);
                BusyIndicatorNotification();
            }
        }

        /// <summary>
        /// RetrieveEntitySelectionData Callback Method
        /// </summary>
        /// <param name="result">List of EntitySelectionData objects</param>
        private void RetrieveEntitySelectionDataCallbackMethod(List<EntitySelectionData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);

            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result.ToString(), 1);
                    EntitySelectionInfo = result.OrderBy(t => t.LongName).ToList();
                    SelectionData.EntitySelectionData = result.OrderBy(t => t.LongName).ToList();
                    if(dbInteractivity != null)
                    {
                        BusyIndicatorNotification(true, "Retrieving reference data...");
                        dbInteractivity.RetrieveAvailableDatesInPortfolios(RetrieveAvailableDatesInPortfoliosCallbackMethod);
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
            finally
            {
                Logging.LogEndMethod(logger, methodNamespace);
                BusyIndicatorNotification();
            }
        }

        /// <summary>
        /// RetrievePortfolioSelectionData Callback Method
        /// </summary>
        /// <param name="result">List of PortfolioSelectionData objects</param>
        private void RetrievePortfolioSelectionDataCallbackMethod(List<PortfolioSelectionData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);

            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result.ToString(), 1);
                    PortfolioSelectionInfo = result.OrderBy(o => o.PortfolioId).ToList();
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

        /// <summary>
        /// RetrieveMarketSnapshotSelectionData Callback Method
        /// </summary>
        /// <param name="result">List of MarketSnapshotSelectionData objects</param>
        private void RetrieveMarketSnapshotSelectionDataCallbackMethod(List<MarketSnapshotSelectionData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result.ToString(), 1);
                    MarketSnapshotSelectionInfo = result;                    
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

        /// <summary>
        /// Callback method that assigns value to ValueTypes
        /// </summary>
        /// <param name="result">Contains the list of value types for propertyName selected region</param>
        public void RetrieveFilterSelectionDataCallbackMethod(List<FilterSelectionData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result.ToString(), 1);
                    FilterSelectionInfo = result;
                    if (SelectedFilterType != null)
                    {
                        FilterSelectorInfo = FilterSelectionInfo
                            .Where(record => record.Filtertype == SelectedFilterType)
                            .ToList();
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
            finally
            {
                Logging.LogEndMethod(logger, methodNamespace);
                BusyIndicatorNotification(isAppLevel: false);
            }
        }

        private void RetrieveCountrySelectionCallbackMethod(List<CountrySelectionData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result.ToString(), 1);
                    CountryTypeInfo = result;
                }
                else
                {
                    Logging.LogMethodParameterNull(logger, methodNamespace, 1);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            finally
            {
                Logging.LogEndMethod(logger, methodNamespace);
                BusyIndicatorNotification();
            }
        }

        private void RetrieveRegionSelectionCallbackMethod(List<GreenField.DataContracts.RegionSelectionData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result.ToString(), 1);
                    RegionTypeInfo = result;
                }
                else
                {
                    Logging.LogMethodParameterNull(logger, methodNamespace, 1);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            finally
            {
                Logging.LogEndMethod(logger, methodNamespace);
                BusyIndicatorNotification();
            }
        }

        /// <summary>
        /// RetrievePortfolioSelectionData Callback Method
        /// </summary>
        /// <param name="result">List of PortfolioSelectionData objects</param>
        private void RetrieveAvailableDatesInPortfoliosCallbackMethod(List<DateTime> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result.ToString(), 1);
                    AvailableDateList = result.OrderByDescending(o => o.Date).ToList();
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
                Logging.LogEndMethod(logger, methodNamespace);
            }
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Get DashboardGadgetParam object
        /// </summary>
        /// <returns>DashboardGadgetParam</returns>
        private DashboardGadgetParam GetDashboardGadgetParam(String gadgetName = null)
        {
            DashboardGadgetParam param;
            Logging.LogBeginMethod(logger, String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name));
            try
            {
                Object additionalInfo = null;

                switch (gadgetName)
                {
                    case GadgetNames.EXTERNAL_RESEARCH_SCATTER_CHART_BANK:
                        additionalInfo = ScatterChartDefaults.BANK;
                        break;
                    case GadgetNames.EXTERNAL_RESEARCH_SCATTER_CHART_INDUSTRIAL:
                        additionalInfo = ScatterChartDefaults.INDUSTRIAL;
                        break;
                    case GadgetNames.EXTERNAL_RESEARCH_SCATTER_CHART_INSURANCE:
                        additionalInfo = ScatterChartDefaults.INSURANCE;
                        break;
                    case GadgetNames.EXTERNAL_RESEARCH_SCATTER_CHART_UTILITY:
                        additionalInfo = ScatterChartDefaults.UTILITY;
                        break;
                    case GadgetNames.EXTERNAL_RESEARCH_BALANCE_SHEET:
                        additionalInfo = FinancialStatementType.BALANCE_SHEET;
                        break;
                    case GadgetNames.EXTERNAL_RESEARCH_INCOME_STATEMENT:
                        additionalInfo = FinancialStatementType.INCOME_STATEMENT;
                        break;
                    case GadgetNames.EXTERNAL_RESEARCH_CASH_FLOW:
                        additionalInfo = FinancialStatementType.CASH_FLOW_STATEMENT;
                        break;
                    case GadgetNames.EXTERNAL_RESEARCH_FUNDAMENTALS_SUMMARY:
                        additionalInfo = FinancialStatementType.FUNDAMENTAL_SUMMARY;
                        break;
                    default:
                        break;
                }
                param = new DashboardGadgetParam()
                    {
                        DBInteractivity = dbInteractivity,
                        EventAggregator = eventAggregator,
                        AdditionalInfo = additionalInfo,
                        LoggerFacade = logger,
                        DashboardGadgetPayload = SelectorPayload,
                        RegionManager = regionManager
                    };
            }
            catch (Exception ex)
            {
                param = null;
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "NullReferenceException", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }

            Logging.LogEndMethod(logger, String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name));
            return param;
        }

        /// <summary>
        /// Updates the visibility properties of selectors based on the ToolBoxItemVisibility static class values
        /// </summary>
        private void UpdateToolBoxSelectorVisibility()
        {
            SecuritySelectorVisibility = ToolBoxItemVisibility.SECURITY_SELECTOR_VISIBILITY;
            PortfolioSelectorVisibility = ToolBoxItemVisibility.PORTFOLIO_SELECTOR_VISIBILITY;
            EffectiveDateSelectorVisibility = ToolBoxItemVisibility.EFFECTIVE_DATE_SELECTOR_VISIBILITY;
            PeriodSelectorVisibility = ToolBoxItemVisibility.PERIOD_SELECTOR_VISIBILITY;
            CountrySelectorVisibility = ToolBoxItemVisibility.COUNTRY_SELECTOR_VISIBILITY;
            SnapshotSelectorVisibility = ToolBoxItemVisibility.SNAPSHOT_SELECTOR_VISIBILITY;
            FilterTypeVisibility = ToolBoxItemVisibility.FILTER_TYPE_SELECTOR_VISIBILITY;
            FilterValueVisibility = ToolBoxItemVisibility.FILTER_VALUE_SELECTOR_VISIBILITY;
            MktCapExCashSelectorVisibility = ToolBoxItemVisibility.MKT_CAP_VISIBILITY;
            CommoditySelectorVisibility = ToolBoxItemVisibility.COMMODITY_SELECTOR_VISIBILTY;
            RegionFXSelectorVisibility = ToolBoxItemVisibility.REGIONFX_SELECTOR_VISIBILITY;
            LookThruSelectorVisibility = ToolBoxItemVisibility.LOOK_THRU_VISIBILITY;
            NodeSelectorVisibility = ToolBoxItemVisibility.NODENAME_SELECTOR_VISIBILITY;
            GadgetSelectorVisibility = ToolBoxItemVisibility.GADGET_SELECTOR_VISIBILITY;
        }

        private void RetrieveMarketSnapshotSelectionData()
        {
            Logging.LogBeginMethod(logger, String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name));
            try
            {
                if (SessionManager.SESSION != null)
                {
                    BusyIndicatorNotification(true, "Retrieving reference data...");
                    dbInteractivity.RetrieveMarketSnapshotSelectionData(SessionManager.SESSION.UserName, RetrieveMarketSnapshotSelectionDataCallbackMethod);
                }
                else
                {
                    manageSessions.GetSession((session) =>
                    {
                        if (session != null)
                        {
                            SessionManager.SESSION = session;
                            BusyIndicatorNotification(true, "Retrieving reference data...");
                            dbInteractivity.RetrieveMarketSnapshotSelectionData(SessionManager.SESSION.UserName, RetrieveMarketSnapshotSelectionDataCallbackMethod);
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
                BusyIndicatorNotification();
            }
            finally
            {                
                Logging.LogEndMethod(logger, String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name));
            }
        }

        private void RetrieveFXCommoditySelectionCallbackMethod(List<FXCommodityData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result.ToString(), 1);
                    CommodityTypeInfo = result;
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

        private void AddItemsToRegionSelectorComboBox(List<GreenField.DataContracts.RegionSelectionData> items)
        {
            if (items != null)
            {
                RegionItems = new ObservableCollection<RegionDataItem>();
                List<String> regions = (from p in items
                                        select p.Region).Distinct().ToList();
                foreach (string region in regions)
                {
                    RegionDataItem regionItem = new RegionDataItem(null);
                    regionItem.Name = region;
                    regionItem.DisplayName = region;
                    List<RegionSelectionData> selectedCountries = (from p in items
                                                                   where p.Region == region
                                                                   select p).ToList();
                    foreach (RegionSelectionData item in selectedCountries)
                    {
                        RegionDataItem country = new RegionDataItem(regionItem)
                        {
                            Name = item.Country,
                            DisplayName = item.CountryNames
                        };
                        regionItem.SubCategories.Add(country);
                    }
                    RegionItems.Add(regionItem);
                }
            }
        }

        private void AddItemToUserDashboard(GadgetInfo gadgetInfo, object viewModelObject = null)
        {
            Object content = null;
            if (gadgetInfo.ViewType.IsClass && gadgetInfo.ViewModelType.IsClass)
            {
                if (viewModelObject == null)
                {
                    Type[] argumentTypes = new Type[] { typeof(DashboardGadgetParam) };
                    object[] argumentValues = new object[] { GetDashboardGadgetParam(gadgetInfo.DisplayName) };
                    viewModelObject = TypeResolution.GetNewTypeObject(gadgetInfo.ViewModelType, argumentTypes, argumentValues);
                }
                content = TypeResolution.GetNewTypeObject(gadgetInfo.ViewType, new Type[] { gadgetInfo.ViewModelType }, new object[] { viewModelObject });
            }

            IsApplicationMenuExpanded = false;
            eventAggregator.GetEvent<DashboardTileViewItemAdded>().Publish
            (
                new DashboardTileViewItemInfo
                {
                    DashboardTileHeader = gadgetInfo.DisplayName,
                    DashboardTileObject = content
                }
            );
        }

        /// <summary>
        /// Display/Hide Busy Indicator
        /// </summary>
        /// <param name="isBusyIndicatorVisible">True to display indicator; default false</param>
        /// <param name="message">Content message for indicator; default null</param>
        private void BusyIndicatorNotification(bool isBusyIndicatorVisible = false, String message = null, Boolean isAppLevel = true)
        {
            if (message != null)
            {
                BusyIndicatorContent = message;
            }
            if (isAppLevel)
            {
                activeCallCount = isBusyIndicatorVisible ? activeCallCount + 1 : activeCallCount - 1;
                IsBusyIndicatorBusy = activeCallCount != 0;
            }
            else
            {
                IsFilterBusyIndicatorBusy = isBusyIndicatorVisible;
            }
            
        }

        private string GetAssemblyVersion()
        {
            var assemblyName = new AssemblyName(System.Windows.Application.Current.GetType().Assembly.FullName);
            if (assemblyName == null)
            {
                return string.Empty;
            }
            Version v = assemblyName.Version;
            if (v == null)
            {
                return string.Empty;
            }
            return v.ToString();
        }
        #endregion

    }
}
