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
using Microsoft.Practices.Prism.ViewModel;
using System.Collections.Generic;
using GreenField.ServiceCaller.SecurityReferenceDefinitions;
using System.Collections.ObjectModel;
using GreenField.Common;
using System.Linq;
using System.Windows.Data;
using System.ComponentModel;
using GreenField.ServiceCaller.PerformanceDefinitions;
using GreenField.DataContracts;
using GreenField.ServiceCaller;
using Microsoft.Practices.Prism.Logging;

namespace GreenField.Gadgets.ViewModels
{
    public class ChildViewModelInsertEntity : NotificationObject
    {
        IDBInteractivity _dBInteractivity;
        ILoggerFacade _logger;

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="result">List of EntitySelectionData object</param>
        public ChildViewModelInsertEntity(List<EntitySelectionData> result, IDBInteractivity dBInteractivity, ILoggerFacade logger)
        {
            EntitySelectionInfoSource = result;
            _dBInteractivity = dBInteractivity;
            _logger = logger;
        }
        #endregion

        #region Properties
        #region Entity Selection
        /// <summary>
        /// DataSource for the Grouped Collection View
        /// </summary>
        private List<EntitySelectionData> _entitySelectionInfoSource;
        public List<EntitySelectionData> EntitySelectionInfoSource
        {
            get { return _entitySelectionInfoSource; }
            set
            {
                _entitySelectionInfoSource = value;
                EntitySelectionInfo = value.Where(record => record.Type == EntityType.SECURITY).ToList();
            }
        }


        private List<EntitySelectionData> _entitySelectionInfo;
        public List<EntitySelectionData> EntitySelectionInfo
        {
            get { return _entitySelectionInfo; }
            set
            {
                _entitySelectionInfo = value;
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

        private List<EntitySelectionData> _entityFilterSelectionInfo;
        public List<EntitySelectionData> EntityFilterSelectionInfo
        {
            get { return _entityFilterSelectionInfo; }
            set
            {
                _entityFilterSelectionInfo = value;
                RaisePropertyChanged(() => this.EntityFilterSelectionInfo);
            }
        }

        /// <summary>
        /// Grouped Collection View for Auto-Complete Box
        /// </summary>
        private CollectionViewSource _entitySelectionGroupInfo;
        public CollectionViewSource EntitySelectionGroupInfo
        {
            get { return _entitySelectionGroupInfo; }
            set
            {
                _entitySelectionGroupInfo = value;
                RaisePropertyChanged(() => this.EntitySelectionGroupInfo);
            }
        }

        /// <summary>
        /// Entered Text in the Auto-Complete Box - filters EntitySelectionGroupInfo
        /// </summary>
        private string _entitySelectionEnteredText = String.Empty;
        public string EntitySelectionEnteredText
        {
            get { return _entitySelectionEnteredText; }
            set
            {
                _entitySelectionEnteredText = value;
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
        private EntitySelectionData _selectedEntity = new EntitySelectionData();
        public EntitySelectionData SelectedEntity
        {
            get { return _selectedEntity; }
            set
            {
                _selectedEntity = value;
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
        private bool? _securityToggleChecked = true;
        public bool? SecurityToggleChecked
        {
            get { return _securityToggleChecked; }
            set
            {
                _securityToggleChecked = value;
                RaisePropertyChanged(() => this.SecurityToggleChecked);
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
        private bool? _benchmarkToggleChecked = false;
        public bool? BenchmarkToggleChecked
        {
            get { return _benchmarkToggleChecked; }
            set
            {
                _benchmarkToggleChecked = value;
                RaisePropertyChanged(() => this.BenchmarkToggleChecked);
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
        /// BenchmarkToggle IsChecked
        /// </summary>
        private bool? _indexToggleChecked = false;
        public bool? IndexToggleChecked
        {
            get { return _indexToggleChecked; }
            set
            {
                _indexToggleChecked = value;
                RaisePropertyChanged(() => this.IndexToggleChecked);
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
        private bool? _commodityToggleChecked = false;
        public bool? CommodityToggleChecked
        {
            get { return _commodityToggleChecked; }
            set
            {
                _commodityToggleChecked = value;
                RaisePropertyChanged(() => this.CommodityToggleChecked);
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

        /// <summary>
        /// Checked State of None Benchmark Node RadioButton
        /// </summary>
        private bool? _noneBenchmarkFilterChecked = true;
        public bool? NoneBenchmarkFilterChecked
        {
            get { return _noneBenchmarkFilterChecked; }
            set
            {
                _noneBenchmarkFilterChecked = value;
                RaisePropertyChanged(() => this.NoneBenchmarkFilterChecked);
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
        private bool? _countryBenchmarkFilterChecked = false;
        public bool? CountryBenchmarkFilterChecked
        {
            get { return _countryBenchmarkFilterChecked; }
            set
            {
                _countryBenchmarkFilterChecked = value;
                RaisePropertyChanged(() => this.CountryBenchmarkFilterChecked);
                if (value == true)
                {
                    SelectedMarketSnapshotPreference.EntityNodeType = EntityNodeType.COUNTRY;
                    SelectedMarketSnapshotPreference.EntityNodeValueCode = null;
                    SelectedMarketSnapshotPreference.EntityNodeValueName = null;
                    BenchmarkFilterSelectionInfo = null;
                    BenchmarkFilterValueVisibility = Visibility.Visible;
                    BenchmarkFilterEmptyText = "Populating Country Node Data based on selected benchmark ...";
                    if (_dBInteractivity != null)
                    {
                        BenchmarkFilterCallInactive = false;
                        _dBInteractivity.RetrieveBenchmarkFilterSelectionData(SelectedEntity.ShortName, SelectedEntity.LongName
                            , "Country", RetrieveBenchmarkFilterSelectionDataCallbackMethod);
                    }
                }
            }
        }

        /// <summary>
        /// Checked State of Sector Benchmark Node RadioButton
        /// </summary>
        private bool? _sectorBenchmarkFilterChecked = false;
        public bool? SectorBenchmarkFilterChecked
        {
            get { return _sectorBenchmarkFilterChecked; }
            set
            {
                _sectorBenchmarkFilterChecked = value;
                RaisePropertyChanged(() => this.SectorBenchmarkFilterChecked);
                if (value == true)
                {
                    SelectedMarketSnapshotPreference.EntityNodeType = EntityNodeType.SECTOR;
                    SelectedMarketSnapshotPreference.EntityNodeValueCode = null;
                    SelectedMarketSnapshotPreference.EntityNodeValueName = null;
                    BenchmarkFilterSelectionInfo = null;
                    BenchmarkFilterValueVisibility = Visibility.Visible;
                    BenchmarkFilterEmptyText = "Populating Sector Node Data based on selected benchmark ...";
                    if (_dBInteractivity != null)
                    {
                        BenchmarkFilterCallInactive = false;
                        _dBInteractivity.RetrieveBenchmarkFilterSelectionData(SelectedEntity.ShortName, SelectedEntity.LongName
                            , "Sector", RetrieveBenchmarkFilterSelectionDataCallbackMethod);
                    }
                }
            }
        }

        private List<BenchmarkFilterSelectionData> _benchmarkFilterSelectionInfo;
        public List<BenchmarkFilterSelectionData> BenchmarkFilterSelectionInfo
        {
            get { return _benchmarkFilterSelectionInfo; }
            set
            {
                _benchmarkFilterSelectionInfo = value;
                RaisePropertyChanged(() => this.BenchmarkFilterSelectionInfo);                
            }
        }

        private BenchmarkFilterSelectionData _selectedBenchmarkFilter;
        public BenchmarkFilterSelectionData SelectedBenchmarkFilter
        {
            get { return _selectedBenchmarkFilter; }
            set
            {
                _selectedBenchmarkFilter = value;
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

        private Visibility _benchmarkFilterVisibility = Visibility.Collapsed;
        public Visibility BenchmarkFilterVisibility
        {
            get { return _benchmarkFilterVisibility; }
            set
            {
                _benchmarkFilterVisibility = value;
                RaisePropertyChanged(() => this.BenchmarkFilterVisibility);
            }
        }

        private Visibility _benchmarkFilterValueVisibility = Visibility.Collapsed;
        public Visibility BenchmarkFilterValueVisibility
        {
            get { return _benchmarkFilterValueVisibility; }
            set
            {
                _benchmarkFilterValueVisibility = value;
                RaisePropertyChanged(() => this.BenchmarkFilterValueVisibility);
            }
        }

        private String _benchmarkFilterEmptyText;
        public String BenchmarkFilterEmptyText
        {
            get { return _benchmarkFilterEmptyText; }
            set
            {
                _benchmarkFilterEmptyText = value;
                RaisePropertyChanged(() => this.BenchmarkFilterEmptyText);
            }
        }

        private bool _benchmarkFilterCallInactive = true;
        public bool BenchmarkFilterCallInactive
        {
            get { return _benchmarkFilterCallInactive; }
            set
            {
                _benchmarkFilterCallInactive = value;
                RaisePropertyChanged(() => this.BenchmarkFilterCallInactive);
            }
        }        
        
        #region Return Type Selection
        /// <summary>
        /// Checked State of Total Return RadioButton
        /// </summary>
        private bool? _totalReturnTypeChecked = true;
        public bool? TotalReturnTypeChecked
        {
            get { return _totalReturnTypeChecked; }
            set
            {
                if (_totalReturnTypeChecked != value)
                {
                    _totalReturnTypeChecked = value;
                    RaisePropertyChanged(() => this.TotalReturnTypeChecked);
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
        private bool? _priceReturnTypeChecked = false;
        public bool? PriceReturnTypeChecked
        {
            get { return _priceReturnTypeChecked; }
            set
            {
                if (_priceReturnTypeChecked != value)
                {
                    _priceReturnTypeChecked = value;
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
        private Visibility _returnTypeSelectionVisibility = Visibility.Collapsed;
        public Visibility ReturnTypeSelectionVisibility
        {
            get { return _returnTypeSelectionVisibility; }
            set
            {
                _returnTypeSelectionVisibility = value;
                RaisePropertyChanged(() => this.ReturnTypeSelectionVisibility);
            }
        }
        #endregion

        #region MarketSnapshotPreference ResultSet
        /// <summary>
        /// Selected Entity constructed into MarketSnapshotPreference object
        /// </summary>
        private MarketSnapshotPreference _selectedMarketSnapshotPreference;
        public MarketSnapshotPreference SelectedMarketSnapshotPreference
        {
            get
            {
                if (_selectedMarketSnapshotPreference == null)
                {
                    _selectedMarketSnapshotPreference = new MarketSnapshotPreference() { EntityReturnType = EntityReturnType.TotalReturnType };
                }
                return _selectedMarketSnapshotPreference;
            }
            set { _selectedMarketSnapshotPreference = value; }
        }
        #endregion

        #endregion

        #region Callback Method
        /// <summary>
        /// Callback method for RetrieveBenchmarkFilterSelectionData Service call - Gets all filter Values for a specific benchmark
        /// </summary>
        /// <param name="result">List of BenchmarkFilterSelectionData objects</param>        
        public void RetrieveBenchmarkFilterSelectionDataCallbackMethod(List<BenchmarkFilterSelectionData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    BenchmarkFilterSelectionInfo = result;
                    BenchmarkFilterEmptyText = "Select value ...";
                    if (result.Count > 0)
                        SelectedBenchmarkFilter = BenchmarkFilterSelectionInfo[0];
                    BenchmarkFilterCallInactive = true;
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

        
    }
}
