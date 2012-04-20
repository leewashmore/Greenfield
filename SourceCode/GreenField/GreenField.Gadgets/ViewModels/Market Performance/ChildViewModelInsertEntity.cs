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
using GreenField.ServiceCaller.ProxyDataDefinitions;
using System.Collections.ObjectModel;
using GreenField.Common;
using System.Linq;
using System.Windows.Data;
using System.ComponentModel;
using GreenField.ServiceCaller.BenchmarkHoldingsPerformanceDefinitions;

namespace GreenField.Gadgets.ViewModels
{
    public class ChildViewModelInsertEntity : NotificationObject
    {
        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="result">List of EntitySelectionData object</param>
        public ChildViewModelInsertEntity(List<EntitySelectionData> result)
        {
            EntitySelectionGroupInfoSource = result;
            EntitySelectionGroupInfo = new CollectionViewSource();
            EntitySelectionGroupInfo.GroupDescriptions.Add(new PropertyGroupDescription("Type"));
            EntitySelectionGroupInfo.SortDescriptions.Add(new SortDescription
                    {
                        PropertyName = "SortOrder",
                        Direction = ListSortDirection.Ascending
                    });
            EntitySelectionGroupInfo.Source = EntitySelectionGroupInfoSource;
        } 
        #endregion

        #region Properties
        #region Entity Selection
        /// <summary>
        /// DataSource for the Grouped Collection View
        /// </summary>
        public List<EntitySelectionData> EntitySelectionGroupInfoSource { get; set; }

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
        private string _entitySelectionEnteredText;
        public string EntitySelectionEnteredText
        {
            get { return _entitySelectionEnteredText; }
            set
            {
                _entitySelectionEnteredText = value;
                RaisePropertyChanged(() => this.EntitySelectionEnteredText);
                if (EntitySelectionGroupInfoSource != null)
                {
                    if (value != String.Empty)
                    {
                        EntitySelectionGroupInfo.Source = EntitySelectionGroupInfoSource
                            .Where(record => record.ShortName.ToLower().Contains(value.ToLower())
                                || record.LongName.ToLower().Contains(value.ToLower()));
                    }
                    else
                        EntitySelectionGroupInfo.Source = EntitySelectionGroupInfoSource;
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
                if (value.Type == EntityType.SECURITY || value.Type == EntityType.BENCHMARK)
                {
                    ReturnTypeSelectionVisibility = Visibility.Visible;
                }
                else
                {
                    ReturnTypeSelectionVisibility = Visibility.Collapsed;
                    SelectedMarketSnapshotPreference.EntityReturnType = null;
                }

                SelectedMarketSnapshotPreference.EntityName = value.LongName;
            }
        }
        #endregion

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
                    _selectedMarketSnapshotPreference = new MarketSnapshotPreference();
                }
                return _selectedMarketSnapshotPreference;
            }
            set { _selectedMarketSnapshotPreference = value; }
        }
        #endregion 
        #endregion        
    }
}
