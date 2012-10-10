using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.ViewModel;
using GreenField.Common;
using GreenField.DataContracts;
using GreenField.ServiceCaller;
using GreenField.ServiceCaller.PerformanceDefinitions;

namespace GreenField.Gadgets.ViewModels
{
    /// <summary>
    /// View Model for ChildViewInsertEntity
    /// </summary>
    public class ChildViewModelInsertEntity : NotificationObject
    {
        #region Fields
        /// <summary>
        /// Service Caller MEF singleton
        /// </summary>
        IDBInteractivity dBInteractivity;

        /// <summary>
        /// Logging MEF Singleton
        /// </summary>
        ILoggerFacade logger; 
        #endregion

        #region Properties
        #region Entity Selection
        /// <summary>
        /// DataSource for the Grouped Collection View
        /// </summary>
        private List<EntitySelectionData> entitySelectionInfoSource;
        public List<EntitySelectionData> EntitySelectionInfoSource
        {
            get { return entitySelectionInfoSource; }
            set
            {
                entitySelectionInfoSource = value;
                EntitySelectionInfo = value.Where(record => record.Type == EntityType.SECURITY).ToList();
            }
        }

        /// <summary>
        /// Stores entity selection data for entity type - security
        /// </summary>
        private List<EntitySelectionData> entitySelectionInfo;
        public List<EntitySelectionData> EntitySelectionInfo
        {
            get { return entitySelectionInfo; }
            set
            {
                entitySelectionInfo = value;
                RaisePropertyChanged(() => this.EntitySelectionInfo);
                if (value != null)
                {
                    if (EntitySelectionEnteredText != String.Empty)
                    {
                        EntityFilterSelectionInfo = value
                            .Where(record => record.ShortName.ToLower().Contains(EntitySelectionEnteredText.ToLower())
                                || record.LongName.ToLower().Contains(EntitySelectionEnteredText.ToLower()))
                            .ToList();
                    }
                    else
                        EntityFilterSelectionInfo = value;
                }
            }
        }

        /// <summary>
        /// Filtered entities based on text search
        /// </summary>
        private List<EntitySelectionData> entityFilterSelectionInfo;
        public List<EntitySelectionData> EntityFilterSelectionInfo
        {
            get { return entityFilterSelectionInfo; }
            set
            {
                entityFilterSelectionInfo = value;
                RaisePropertyChanged(() => this.EntityFilterSelectionInfo);
            }
        }

        /// <summary>
        /// Grouped Collection View for Auto-Complete Box
        /// </summary>
        private CollectionViewSource entitySelectionGroupInfo;
        public CollectionViewSource EntitySelectionGroupInfo
        {
            get { return entitySelectionGroupInfo; }
            set
            {
                entitySelectionGroupInfo = value;
                RaisePropertyChanged(() => this.EntitySelectionGroupInfo);
            }
        }

        /// <summary>
        /// Entered Text in the Auto-Complete Box - filters EntitySelectionGroupInfo
        /// </summary>
        private string entitySelectionEnteredText = String.Empty;
        public string EntitySelectionEnteredText
        {
            get { return entitySelectionEnteredText; }
            set
            {
                entitySelectionEnteredText = value;
                RaisePropertyChanged(() => this.EntitySelectionEnteredText);

                if (EntitySelectionInfo != null)
                {
                    if (value != String.Empty && value != null)
                    {
                        EntityFilterSelectionInfo = EntitySelectionInfo
                            .Where(record => record.ShortName.ToLower().Contains(value.ToLower())
                                || record.LongName.ToLower().Contains(value.ToLower()))
                            .ToList();
                    }
                    else
                        EntityFilterSelectionInfo = EntitySelectionInfo;
                }
            }
        }

        /// <summary>
        /// Selected Entity - Return Type Selector visibility depends on Type of entity
        /// </summary>
        private EntitySelectionData selectedEntity = new EntitySelectionData();
        public EntitySelectionData SelectedEntity
        {
            get { return selectedEntity; }
            set
            {
                selectedEntity = value;
                this.RaisePropertyChanged(() => this.SelectedEntity);
                if (value != null)
                {
                    if (value.Type == EntityType.SECURITY)
                    {
                        ReturnTypeSelectionVisibility = Visibility.Visible;
                        BenchmarkFilterVisibility = Visibility.Collapsed;
                        SelectedMarketSnapshotPreference.EntityNodeType = null;
                        SelectedMarketSnapshotPreference.EntityNodeValueCode = null;
                        SelectedMarketSnapshotPreference.EntityNodeValueName = null;
                    }
                    else if (value.Type == EntityType.BENCHMARK)
                    {
                        ReturnTypeSelectionVisibility = Visibility.Collapsed;
                        BenchmarkFilterVisibility = Visibility.Visible;
                        SelectedMarketSnapshotPreference.EntityReturnType = null;
                    }
                    else
                    {
                        ReturnTypeSelectionVisibility = Visibility.Collapsed;
                        BenchmarkFilterVisibility = Visibility.Collapsed;
                        SelectedMarketSnapshotPreference.EntityReturnType = null;
                        SelectedMarketSnapshotPreference.EntityNodeType = null;
                        SelectedMarketSnapshotPreference.EntityNodeValueCode = null;
                        SelectedMarketSnapshotPreference.EntityNodeValueName = null;
                    }

                    SelectedMarketSnapshotPreference.EntityName = value.LongName;
                    SelectedMarketSnapshotPreference.EntityType = value.Type;
                    SelectedMarketSnapshotPreference.EntityId = value.InstrumentID;
                }
            }
        }
        #endregion

        #region Toggle Checked Bindings
        /// <summary>
        /// SecurityToggle IsChecked
        /// </summary>
        private bool? isSecurityToggleChecked = true;
        public bool? IsSecurityToggleChecked
        {
            get { return isSecurityToggleChecked; }
            set
            {
                isSecurityToggleChecked = value;
                RaisePropertyChanged(() => this.IsSecurityToggleChecked);
                if (value == true)
                {
                    EntitySelectionInfo = EntitySelectionInfoSource
                        .Where(record => record.Type == EntityType.SECURITY)
                        .ToList();
                    BenchmarkFilterValueVisibility = Visibility.Collapsed;
                    BenchmarkFilterVisibility = Visibility.Collapsed;
                    ReturnTypeSelectionVisibility = Visibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// BenchmarkToggle IsChecked
        /// </summary>
        private bool? isBenchmarkToggleChecked = false;
        public bool? IsBenchmarkToggleChecked
        {
            get { return isBenchmarkToggleChecked; }
            set
            {
                isBenchmarkToggleChecked = value;
                RaisePropertyChanged(() => this.IsBenchmarkToggleChecked);
                if (value == true)
                {
                    EntitySelectionInfo = EntitySelectionInfoSource
                                .Where(record => record.Type == EntityType.BENCHMARK)
                                .ToList();
                    BenchmarkFilterValueVisibility = Visibility.Collapsed;
                    BenchmarkFilterVisibility = Visibility.Collapsed;
                    ReturnTypeSelectionVisibility = Visibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// IndexToggle IsChecked
        /// </summary>
        private bool? isIndexToggleChecked = false;
        public bool? IsIndexToggleChecked
        {
            get { return isIndexToggleChecked; }
            set
            {
                isIndexToggleChecked = value;
                RaisePropertyChanged(() => this.IsIndexToggleChecked);
                if (value == true)
                {
                    EntitySelectionInfo = EntitySelectionInfoSource
                                .Where(record => record.Type == EntityType.INDEX)
                                .ToList();
                    BenchmarkFilterValueVisibility = Visibility.Collapsed;
                    BenchmarkFilterVisibility = Visibility.Collapsed;
                    ReturnTypeSelectionVisibility = Visibility.Collapsed;
                }
            }
        }

        /// <summary>
        /// CommodityToggle IsChecked
        /// </summary>
        private bool? isCommodityToggleChecked = false;
        public bool? IsCommodityToggleChecked
        {
            get { return isCommodityToggleChecked; }
            set
            {
                isCommodityToggleChecked = value;
                RaisePropertyChanged(() => this.IsCommodityToggleChecked);
                if (value == true)
                {
                    EntitySelectionInfo = EntitySelectionInfoSource
                                .Where(record => record.Type == EntityType.COMMODITY)
                                .ToList();
                    BenchmarkFilterValueVisibility = Visibility.Collapsed;
                    BenchmarkFilterVisibility = Visibility.Collapsed;
                    ReturnTypeSelectionVisibility = Visibility.Collapsed;
                }
            }
        }
        #endregion

        #region Benchmark Filter
        /// <summary>
        /// Checked State of None Benchmark Node RadioButton
        /// </summary>
        private bool? isNoneBenchmarkFilterChecked = true;
        public bool? IsNoneBenchmarkFilterChecked
        {
            get { return isNoneBenchmarkFilterChecked; }
            set
            {
                isNoneBenchmarkFilterChecked = value;
                RaisePropertyChanged(() => this.IsNoneBenchmarkFilterChecked);
                if (value == true)
                {
                    SelectedMarketSnapshotPreference.EntityNodeType = EntityNodeType.NONE;
                    SelectedMarketSnapshotPreference.EntityNodeValueCode = null;
                    SelectedMarketSnapshotPreference.EntityNodeValueName = null;
                    BenchmarkFilterSelectionInfo = null;
                    BenchmarkFilterValueVisibility = Visibility.Collapsed;
                    BenchmarkFilterEmptyText = String.Empty;
                }
            }
        }

        /// <summary>
        /// Checked State of Country Benchmark Node RadioButton
        /// </summary>
        private bool? isCountryBenchmarkFilterChecked = false;
        public bool? IsCountryBenchmarkFilterChecked
        {
            get { return isCountryBenchmarkFilterChecked; }
            set
            {
                isCountryBenchmarkFilterChecked = value;
                RaisePropertyChanged(() => this.IsCountryBenchmarkFilterChecked);
                if (value == true)
                {
                    SelectedMarketSnapshotPreference.EntityNodeType = EntityNodeType.COUNTRY;
                    SelectedMarketSnapshotPreference.EntityNodeValueCode = null;
                    SelectedMarketSnapshotPreference.EntityNodeValueName = null;
                    BenchmarkFilterSelectionInfo = null;
                    BenchmarkFilterValueVisibility = Visibility.Visible;
                    BenchmarkFilterEmptyText = "Populating Country Node Data based on selected benchmark ...";
                    if (dBInteractivity != null)
                    {
                        IsBenchmarkFilterCallInactive = false;
                        dBInteractivity.RetrieveBenchmarkFilterSelectionData(SelectedEntity.ShortName, SelectedEntity.LongName
                            , "Country", RetrieveBenchmarkFilterSelectionDataCallbackMethod);
                    }
                }
            }
        }

        /// <summary>
        /// Checked State of Sector Benchmark Node RadioButton
        /// </summary>
        private bool? isSectorBenchmarkFilterChecked = false;
        public bool? IsSectorBenchmarkFilterChecked
        {
            get { return isSectorBenchmarkFilterChecked; }
            set
            {
                isSectorBenchmarkFilterChecked = value;
                RaisePropertyChanged(() => this.IsSectorBenchmarkFilterChecked);
                if (value == true)
                {
                    SelectedMarketSnapshotPreference.EntityNodeType = EntityNodeType.SECTOR;
                    SelectedMarketSnapshotPreference.EntityNodeValueCode = null;
                    SelectedMarketSnapshotPreference.EntityNodeValueName = null;
                    BenchmarkFilterSelectionInfo = null;
                    BenchmarkFilterValueVisibility = Visibility.Visible;
                    BenchmarkFilterEmptyText = "Populating Sector Node Data based on selected benchmark ...";
                    if (dBInteractivity != null)
                    {
                        IsBenchmarkFilterCallInactive = false;
                        dBInteractivity.RetrieveBenchmarkFilterSelectionData(SelectedEntity.ShortName, SelectedEntity.LongName
                            , "Sector", RetrieveBenchmarkFilterSelectionDataCallbackMethod);
                    }
                }
            }
        }

        /// <summary>
        /// Stores benchmark filter selection information
        /// </summary>
        private List<BenchmarkFilterSelectionData> benchmarkFilterSelectionInfo;
        public List<BenchmarkFilterSelectionData> BenchmarkFilterSelectionInfo
        {
            get { return benchmarkFilterSelectionInfo; }
            set
            {
                benchmarkFilterSelectionInfo = value;
                RaisePropertyChanged(() => this.BenchmarkFilterSelectionInfo);
            }
        }

        /// <summary>
        /// Stores selected benchmark filter selection information
        /// </summary>
        private BenchmarkFilterSelectionData selectedBenchmarkFilter;
        public BenchmarkFilterSelectionData SelectedBenchmarkFilter
        {
            get { return selectedBenchmarkFilter; }
            set
            {
                selectedBenchmarkFilter = value;
                this.RaisePropertyChanged(() => this.SelectedBenchmarkFilter);
                if (value == null)
                {
                    SelectedMarketSnapshotPreference.EntityNodeValueCode = null;
                    SelectedMarketSnapshotPreference.EntityNodeValueName = null;
                    return;
                }

                SelectedMarketSnapshotPreference.EntityNodeValueCode = value.FilterCode;
                SelectedMarketSnapshotPreference.EntityNodeValueName = value.FilterName;
            }
        }

        /// <summary>
        /// Stores visibility of benchmark filter
        /// </summary>
        private Visibility benchmarkFilterVisibility = Visibility.Collapsed;
        public Visibility BenchmarkFilterVisibility
        {
            get { return benchmarkFilterVisibility; }
            set
            {
                benchmarkFilterVisibility = value;
                RaisePropertyChanged(() => this.BenchmarkFilterVisibility);
            }
        }

        /// <summary>
        /// Stores visibility of benchmark filter value
        /// </summary>
        private Visibility benchmarkFilterValueVisibility = Visibility.Collapsed;
        public Visibility BenchmarkFilterValueVisibility
        {
            get { return benchmarkFilterValueVisibility; }
            set
            {
                benchmarkFilterValueVisibility = value;
                RaisePropertyChanged(() => this.BenchmarkFilterValueVisibility);
            }
        }

        /// <summary>
        /// Stores benchmark filter empty text
        /// </summary>
        private String benchmarkFilterEmptyText;
        public String BenchmarkFilterEmptyText
        {
            get { return benchmarkFilterEmptyText; }
            set
            {
                benchmarkFilterEmptyText = value;
                RaisePropertyChanged(() => this.BenchmarkFilterEmptyText);
            }
        }

        /// <summary>
        /// Stores benchmark filter activity
        /// </summary>
        private bool isBenchmarkFilterCallInactive = true;
        public bool IsBenchmarkFilterCallInactive
        {
            get { return isBenchmarkFilterCallInactive; }
            set
            {
                isBenchmarkFilterCallInactive = value;
                RaisePropertyChanged(() => this.IsBenchmarkFilterCallInactive);
            }
        } 
        #endregion

        #region Return Type Selection
        /// <summary>
        /// Checked State of Total Return RadioButton
        /// </summary>
        private bool? isTotalReturnTypeChecked = true;
        public bool? IsTotalReturnTypeChecked
        {
            get { return isTotalReturnTypeChecked; }
            set
            {
                if (isTotalReturnTypeChecked != value)
                {
                    isTotalReturnTypeChecked = value;
                    RaisePropertyChanged(() => this.IsTotalReturnTypeChecked);
                    if (value == true)
                    {
                        SelectedMarketSnapshotPreference.EntityReturnType = EntityReturnType.TotalReturnType;
                    }
                }
            }
        }

        /// <summary>
        /// Checked State of Price Return RadioButton
        /// </summary>
        private bool? isPriceReturnTypeChecked = false;
        public bool? IsPriceReturnTypeChecked
        {
            get { return isPriceReturnTypeChecked; }
            set
            {
                if (isPriceReturnTypeChecked != value)
                {
                    isPriceReturnTypeChecked = value;
                    if (value == true)
                    {
                        SelectedMarketSnapshotPreference.EntityReturnType = EntityReturnType.PriceReturnType;
                    }
                }
            }
        }

        /// <summary>
        /// Visibility State of the Return Type Selection StackPanel
        /// </summary>
        private Visibility returnTypeSelectionVisibility = Visibility.Collapsed;
        public Visibility ReturnTypeSelectionVisibility
        {
            get { return returnTypeSelectionVisibility; }
            set
            {
                returnTypeSelectionVisibility = value;
                RaisePropertyChanged(() => this.ReturnTypeSelectionVisibility);
            }
        }
        #endregion

        #region MarketSnapshotPreference ResultSet
        /// <summary>
        /// Selected Entity constructed into MarketSnapshotPreference object
        /// </summary>
        private MarketSnapshotPreference selectedMarketSnapshotPreference;
        public MarketSnapshotPreference SelectedMarketSnapshotPreference
        {
            get
            {
                if (selectedMarketSnapshotPreference == null)
                {
                    selectedMarketSnapshotPreference = new MarketSnapshotPreference() { EntityReturnType = EntityReturnType.TotalReturnType };
                }
                return selectedMarketSnapshotPreference;
            }
            set { selectedMarketSnapshotPreference = value; }
        }
        #endregion
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="result">List of EntitySelectionData object</param>
        public ChildViewModelInsertEntity(List<EntitySelectionData> result, IDBInteractivity dBInteractivity, ILoggerFacade logger)
        {
            EntitySelectionInfoSource = result;
            this.dBInteractivity = dBInteractivity;
            this.logger = logger;
        }
        #endregion        

        #region Callback Method
        /// <summary>
        /// Callback method for RetrieveBenchmarkFilterSelectionData Service call - Gets all filter Values for a specific benchmark
        /// </summary>
        /// <param name="result">List of BenchmarkFilterSelectionData objects</param>        
        public void RetrieveBenchmarkFilterSelectionDataCallbackMethod(List<BenchmarkFilterSelectionData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, result, 1);
                    BenchmarkFilterSelectionInfo = result;
                    BenchmarkFilterEmptyText = "Select value ...";
                    if (result.Count > 0)
                        SelectedBenchmarkFilter = BenchmarkFilterSelectionInfo[0];
                    IsBenchmarkFilterCallInactive = true;
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
    }
}
