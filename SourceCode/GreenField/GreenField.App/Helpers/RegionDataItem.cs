using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using Microsoft.Practices.Prism.ViewModel;


namespace GreenField.App.Helpers
{
    /// <summary>
    /// Object for Region dropdown
    /// </summary>
    public class RegionDataItem : NotificationObject
    {              
        private bool reentrancyCheck = false;
        private RegionDataItem parentItem;

        private ObservableCollection<RegionDataItem> subCategories = null;

        /// <summary>
        /// Name of node
        /// </summary>
        private string name;
        public string Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        /// <summary>
        /// Display Name of node
        /// </summary>
        private string displayName;
        public string DisplayName
        {
            get
            {
                return this.displayName;
            }
            set
            {
                this.displayName = value;
            }
        }

        /// <summary>
        /// If Node is checked or not
        /// </summary>
        private bool? isChecked = false;
        public bool? IsChecked
        {
            get
            {
                return this.isChecked;
            }
            set
            {
                if (this.isChecked != value)
                {
                    if (reentrancyCheck)
                        return;
                    this.reentrancyCheck = true;
                    this.isChecked = value;
                    this.UpdateCheckState();
                    RaisePropertyChanged(() => this.IsChecked);
                    this.reentrancyCheck = false;
                }
            }
        }

        /// <summary>
        /// Children of parent i.e. countries within a region
        /// </summary>
        public ObservableCollection<RegionDataItem> SubCategories
        {
            get
            {
                if (this.subCategories == null)
                {
                    this.subCategories = new ObservableCollection<RegionDataItem>();
                }
                return this.subCategories;
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parent"></param>
        public RegionDataItem(RegionDataItem parent)
        {
            this.parentItem = parent;
        }

        private void UpdateCheckState()
        {
            // update all children:
            if (this.SubCategories.Count != 0)
            {
                this.UpdateChildrenCheckState();
            }
            //update parent item
            if (this.parentItem != null)
            {
                bool? parentIsChecked = this.parentItem.DetermineCheckState();
                this.parentItem.IsChecked = parentIsChecked;

            }
        }

        private void UpdateChildrenCheckState()
        {
            foreach (var item in this.SubCategories)
            {
                if (this.IsChecked != null)
                {
                    item.IsChecked = this.IsChecked;
                }
            }
        }

        private bool? DetermineCheckState()
        {
            bool allChildrenChecked = this.SubCategories.Count(x => x.IsChecked == true) == this.SubCategories.Count;
            if (allChildrenChecked)
            {
                return true;
            }

            bool allChildrenUnchecked = this.SubCategories.Count(x => x.IsChecked == false) == this.SubCategories.Count;
            if (allChildrenUnchecked)
            {
                return false;
            }

            return null;
        }
    }
}
