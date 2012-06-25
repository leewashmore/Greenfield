using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel.Composition;
using Telerik.Windows;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;
using GreenField.ServiceCaller.SecurityReferenceDefinitions;
using GreenField.Gadgets.ViewModels;
using GreenField.Gadgets.Helpers;
using System.Collections.ObjectModel;
using GreenField.ServiceCaller.PerformanceDefinitions;
using GreenField.Gadgets.Models;
using GreenField.Common;

namespace GreenField.Gadgets.Views
{
    public partial class ViewMarketPerformanceSnapshot : ViewBaseUserControl
    {
        #region Fields
        private ViewModelMarketPerformanceSnapshot _dataContextSource; 
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataContextSource">DataContext ViewModelMarketPerformanceSnapshot Instance</param>   
        public ViewMarketPerformanceSnapshot(ViewModelMarketPerformanceSnapshot dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;

            _dataContextSource = dataContextSource;
            this.SetGridHeaders();

            
        } 
        #endregion

        #region Event Handlers
        /// <summary>
        /// Method to catch click event to open context menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RadContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            if (sender == null || e == null)
                return;

            RadContextMenu menu = (RadContextMenu)sender;

            if (menu == null)
                return;


            GridViewRow row = menu.GetClickedElement<GridViewRow>();
            GridViewGroupRow groupRow = menu.GetClickedElement<GridViewGroupRow>();
            GridViewHeaderRow headerRow = menu.GetClickedElement<GridViewHeaderRow>();

            if (row != null)
            {
                row.IsSelected = row.IsCurrent = true;
                (menu.Items[0] as RadMenuItem).IsEnabled = true;
                (menu.Items[1] as RadMenuItem).IsEnabled = true;
                (menu.Items[2] as RadMenuItem).IsEnabled = true;
                (menu.Items[3] as RadMenuItem).IsEnabled = true;
                return;
            }

            if (groupRow != null)
            {
                (menu.Items[0] as RadMenuItem).IsEnabled = true;
                (menu.Items[1] as RadMenuItem).IsEnabled = true;
                (menu.Items[2] as RadMenuItem).IsEnabled = true;
                (menu.Items[3] as RadMenuItem).IsEnabled = false;
                if (!(groupRow.Group.Key is string))
                    return;

                string groupName = groupRow.Group.Key as string;
                int itemCount = 0;
                foreach (MarketPerformanceSnapshotData item in this.radGridSnapshot.Items)
                {
                    if (item.MarketSnapshotPreferenceInfo.GroupName == groupName
                        && item.MarketSnapshotPreferenceInfo.EntityOrder == 1)
                    {
                        this.radGridSnapshot.SelectedItem = item;
                        return;
                    }
                    itemCount++;
                }

                return;
            }

            if (headerRow != null)
            {
                (menu.Items[0] as RadMenuItem).IsEnabled = true;
                (menu.Items[1] as RadMenuItem).IsEnabled = false;
                (menu.Items[2] as RadMenuItem).IsEnabled = false;
                (menu.Items[3] as RadMenuItem).IsEnabled = false;
                return;
            }

            menu.IsOpen = false;

        }

        /// <summary>
        /// radGridSnapshot RowLoaded Event Handler - applied custom styles
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">RowLoadedEventArgs</param>
        private void radGridSnapshot_RowLoaded(object sender, RowLoadedEventArgs e)
        {
            GroupedGridRowLoadedHandler.Implement(e);
        }

        /// <summary>
        /// ReorderBehavior Reordered Event Handler - Rebinds Datacontext post reordering
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">ReorderedEventArgs</param>
        private void ReorderBehavior_Reordered(object sender, ReorderedEventArgs e)
        {
            //Update MarketSnapshotPreferenceInfo after reordering
            _dataContextSource.MarketSnapshotPreferenceInfo = _dataContextSource.MarketPerformanceSnapshotInfo
                .Select(record => record.MarketSnapshotPreferenceInfo)
                .ToList();

            #region Client cache implementation
            if (_dataContextSource.PopulatedMarketPerformanceSnapshotInfo != null)
            {
                PopulatedMarketPerformanceSnapshotData selectedPopulatedMarketPerformanceSnapshotInfo = _dataContextSource.PopulatedMarketPerformanceSnapshotInfo
                                    .Where(record => record.MarketSnapshotSelectionInfo == _dataContextSource.SelectedMarketSnapshotSelectionInfo).FirstOrDefault();

                if (selectedPopulatedMarketPerformanceSnapshotInfo != null)
                {
                    _dataContextSource.PopulatedMarketPerformanceSnapshotInfo
                        .Where(record => record.MarketSnapshotSelectionInfo == _dataContextSource.SelectedMarketSnapshotSelectionInfo)
                        .FirstOrDefault()
                        .MarketPerformanceSnapshotInfo = _dataContextSource.MarketPerformanceSnapshotInfo.ToList();
                }
            }
            #endregion

            this.radGridSnapshot.Rebind();
            _dataContextSource.TestEntityOrdering();

        }

        /// <summary>
        /// ReorderBehavior Reordering Event Handler - Adjusts Entity Order and GroupPreferenceId of entities after reordering
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">ReorderingEventArgs</param>
        private void ReorderBehavior_Reordering(object sender, ReorderingEventArgs e)
        {
            //If there are no dragged Items reordering is redundant
            if (e.DraggedItems.Count().Equals(0))
                return;

            #region Get Dragged Element
            //Dragged Element
            MarketPerformanceSnapshotData draggedElement = e.DraggedItems.FirstOrDefault() as MarketPerformanceSnapshotData;

            //Null Exception Handling
            if (draggedElement == null)
                return;
            #endregion

            #region Get Data Context
            //Collection of MorningSnapshotData binded to the grid
            List<MarketPerformanceSnapshotData> dataContext = (e.SourceGrid.ItemsSource as ObservableCollection<MarketPerformanceSnapshotData>).ToList();

            //Null Exception Handling
            if (dataContext == null)
                return;
            #endregion

            #region Get Drag Drop Details
            //Drag Drop Indexes
            int dragIndex = dataContext.IndexOf(draggedElement);
            int dropIndex = e.DropIndex;
            //True if insertion is done after an element and False if it's done  before the element
            bool dropPositionIsAfter = DragDropPosition.Value == DropPosition.After;
            //Check if the drop Index exceed the Count of List - item dropped after the last element
            bool dropIndexExceedsCount = dropIndex >= dataContext.Count;
            #endregion

            #region Drag Location Parameters
            MarketSnapshotPreference dragBenchmarkDetails = new MarketSnapshotPreference()
            {
                EntityName = dataContext.ElementAt(dragIndex).MarketSnapshotPreferenceInfo.EntityName,
                EntityReturnType = dataContext.ElementAt(dragIndex).MarketSnapshotPreferenceInfo.EntityReturnType,
                EntityOrder = dataContext.ElementAt(dragIndex).MarketSnapshotPreferenceInfo.EntityOrder,
                GroupName = dataContext.ElementAt(dragIndex).MarketSnapshotPreferenceInfo.GroupName,
                GroupPreferenceID = dataContext.ElementAt(dragIndex).MarketSnapshotPreferenceInfo.GroupPreferenceID
            };
            #endregion

            #region Drop Location Parameters
            MarketSnapshotPreference dropBenchmarkDetails = new MarketSnapshotPreference()
            {
                EntityName = dropIndexExceedsCount ? null : dataContext.ElementAt(dropIndex).MarketSnapshotPreferenceInfo.EntityName,
                EntityReturnType = dropIndexExceedsCount ? null : dataContext.ElementAt(e.DropIndex).MarketSnapshotPreferenceInfo.EntityReturnType,
                EntityOrder = dropIndexExceedsCount
                                    ? dataContext.ElementAt(dropIndex - 1).MarketSnapshotPreferenceInfo.EntityOrder + 1
                                    : dataContext.ElementAt(dropIndex).MarketSnapshotPreferenceInfo.EntityOrder,
                GroupName = dropIndexExceedsCount
                                ? dataContext.ElementAt(dropIndex - 1).MarketSnapshotPreferenceInfo.GroupName
                                : dataContext.ElementAt(dropIndex).MarketSnapshotPreferenceInfo.GroupName,
                GroupPreferenceID = dropIndexExceedsCount
                                ? dataContext.ElementAt(dropIndex - 1).MarketSnapshotPreferenceInfo.GroupPreferenceID
                                : dataContext.ElementAt(dropIndex).MarketSnapshotPreferenceInfo.GroupPreferenceID
            };
            #endregion

            #region Managing discrepancies in drop location
            //If drop location is after the last element of same or another group the behavior picks the next element nevertheless

            bool dragdropGroupIsSame = dragBenchmarkDetails.GroupPreferenceID == dropBenchmarkDetails.GroupPreferenceID;
            bool dropEntityIsMisread = dropBenchmarkDetails.EntityOrder == 1 && dropPositionIsAfter;

            //bool dropIsLastOfSameGroup = dragdropGroupIsSame
            //    ? dropBenchmarkDetails.EntityOrder == GetBenchmarkCountInGroup(dataContext, dropBenchmarkDetails.GroupPreferenceID) + 1
            //    : dropBenchmarkDetails.EntityOrder == 1 && dropPositionIsAfter;

            bool dropIsLastOfSameGroup = GetLastGroupPreferenceId(dataContext, dropBenchmarkDetails.GroupPreferenceID) == dragBenchmarkDetails.GroupPreferenceID
                                            && dropEntityIsMisread;
            bool dropIsLastOfOtherGroup = (!dropIsLastOfSameGroup) && dropEntityIsMisread;
            #endregion

            bool dragIsWithinGroup = dragdropGroupIsSame ? (!dropIsLastOfOtherGroup) : dropIsLastOfSameGroup;

            if (dragIsWithinGroup)
            {
                UpdateParametersForSameGroupReordering(dataContext, dragBenchmarkDetails, dropBenchmarkDetails, dropIsLastOfSameGroup);
            }
            else
            {
                UpdateParametersForDiffGroupReordering(dataContext, dragBenchmarkDetails, dropBenchmarkDetails, dropIsLastOfOtherGroup);
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Set Snapshot Grid Column Headers
        /// </summary>
        private void SetGridHeaders()
        {
            //Entity Description
            this.radGridSnapshot.Columns[0].Header = "Description";
            //Entity Return Type
            this.radGridSnapshot.Columns[1].Header = "Return";
            //Market Performance for Last Working Date
            this.radGridSnapshot.Columns[2].Header = DateTime.Today.AddDays(-1).ToString("d");
            //Market Performance for Week to Date
            this.radGridSnapshot.Columns[3].Header = "WTD";
            //Market Performance for Month to Date
            this.radGridSnapshot.Columns[4].Header = "MTD";
            //Market Performance for Quarter to Date
            this.radGridSnapshot.Columns[5].Header = "QTD";
            //Market Performance for Year to Date
            this.radGridSnapshot.Columns[6].Header = "YTD";
            //Market Performance for Last Year
            this.radGridSnapshot.Columns[7].Header = DateTime.Today.AddYears(-1).Year.ToString();
            //Market Performance for Second Last Year
            this.radGridSnapshot.Columns[8].Header = DateTime.Today.AddYears(-2).Year.ToString();
            //Market Performance for Third Last Year
            this.radGridSnapshot.Columns[9].Header = DateTime.Today.AddYears(-3).Year.ToString();
        }

        /// <summary>
        /// Method to get count of entities within propertyName group
        /// </summary>
        /// <param name="dataContext">List of MarketPerformanceSnapshotData</param>
        /// <param name="groupPreferenceId">GroupPreferenceId value</param>
        /// <returns></returns>
        private int GetEntityCountInGroup(List<MarketPerformanceSnapshotData> dataContext, int groupPreferenceId)
        {
            return dataContext.Where(t => t.MarketSnapshotPreferenceInfo.GroupPreferenceID == groupPreferenceId).Count();
        }

        /// <summary>
        /// Method to get GroupPreferenceId of the last group in snapshot
        /// </summary>
        /// <param name="dataContext">List of MarketPerformanceSnapshotData</param>
        /// <param name="groupPreferenceId">GroupPreferenceId value</param>
        /// <returns>GroupPreferenceId value</returns>
        private int GetLastGroupPreferenceId(List<MarketPerformanceSnapshotData> dataContext, int groupPreferenceId)
        {
            List<MarketPerformanceSnapshotData> orderedDataContext = dataContext
                .OrderBy(record => record.MarketSnapshotPreferenceInfo.GroupPreferenceID)
                .ThenBy(record => record.MarketSnapshotPreferenceInfo.EntityOrder)
                .ToList();
            int lastGroupPreferenceId = 0;
            foreach (MarketPerformanceSnapshotData data in orderedDataContext)
            {
                if (data.MarketSnapshotPreferenceInfo.GroupPreferenceID < groupPreferenceId)
                {
                    if (data.MarketSnapshotPreferenceInfo.GroupPreferenceID > lastGroupPreferenceId)
                    {
                        lastGroupPreferenceId = data.MarketSnapshotPreferenceInfo.GroupPreferenceID;
                    }
                }
                else
                {
                    return lastGroupPreferenceId;
                }
            }

            return lastGroupPreferenceId;
        }

        /// <summary>
        /// Method to get name of the last group
        /// </summary>
        /// <param name="dataContext">List of MorningSnapshotData binded to the grid</param>
        /// <param name="groupPreferenceId">GroupPreferenceId</param>
        /// <returns></returns>
        private string GetLastGroupName(List<MarketPerformanceSnapshotData> dataContext, int groupPreferenceId)
        {
            List<MarketPerformanceSnapshotData> orderedDataContext = dataContext
                .OrderBy(record => record.MarketSnapshotPreferenceInfo.GroupPreferenceID)
                .ThenBy(record => record.MarketSnapshotPreferenceInfo.EntityOrder)
                .ToList();
            int lastGroupPreferenceId = 0;
            string lastGroupName = String.Empty;

            foreach (MarketPerformanceSnapshotData data in orderedDataContext)
            {
                if (data.MarketSnapshotPreferenceInfo.GroupPreferenceID < groupPreferenceId)
                {
                    if (data.MarketSnapshotPreferenceInfo.GroupPreferenceID > lastGroupPreferenceId)
                    {
                        lastGroupPreferenceId = data.MarketSnapshotPreferenceInfo.GroupPreferenceID;
                        lastGroupName = data.MarketSnapshotPreferenceInfo.GroupName;
                    }
                }
                else
                {
                    return lastGroupName;
                }
            }

            return lastGroupName;
        }

        /// <summary>
        /// Updates the Element Order in the list based on the drag drop parameters
        /// </summary>
        /// <param name="dataContext">List of MorningSnapshotData binded to the grid</param>
        /// <param name="dragBenchmarkDetails">Preference details of the dragged Item</param>
        /// <param name="dropBenchmarkDetails">Preference details of the next to drop location</param>
        /// <param name="dropIsLastOfSameGroup">True if the drop location is last within the drag item group</param>
        private void UpdateParametersForSameGroupReordering(List<MarketPerformanceSnapshotData> dataContext,
            MarketSnapshotPreference dragBenchmarkDetails, MarketSnapshotPreference dropBenchmarkDetails, bool dropIsLastOfSameGroup)
        {
            //Dropped at Group end with the next group empty
            if (dropBenchmarkDetails.EntityOrder == null)
                dropBenchmarkDetails.EntityOrder = GetEntityCountInGroup(dataContext, dragBenchmarkDetails.GroupPreferenceID) + 1;

            //Dropped at Group end within the same group
            if (dropIsLastOfSameGroup)
            {
                if (dragBenchmarkDetails.GroupPreferenceID != dropBenchmarkDetails.GroupPreferenceID)
                {
                    dropBenchmarkDetails.EntityOrder = GetEntityCountInGroup(dataContext, dragBenchmarkDetails.GroupPreferenceID) + 1;
                }
            }

            //Check drop flow - top ot bottom or vice versa
            bool dropLocationExceedsDragLocation = dropBenchmarkDetails.EntityOrder > dragBenchmarkDetails.EntityOrder;

            foreach (MarketPerformanceSnapshotData record in dataContext)
            {
                //No consideration of records in other groups
                if (record.MarketSnapshotPreferenceInfo.GroupPreferenceID != dragBenchmarkDetails.GroupPreferenceID)
                {
                    continue;
                }

                //Check if the record is between drag and drop location
                bool recordIsBetweenDragDropLocation = dropLocationExceedsDragLocation
                    ? record.MarketSnapshotPreferenceInfo.EntityOrder > dragBenchmarkDetails.EntityOrder
                        && record.MarketSnapshotPreferenceInfo.EntityOrder < dropBenchmarkDetails.EntityOrder
                    : record.MarketSnapshotPreferenceInfo.EntityOrder < dragBenchmarkDetails.EntityOrder
                        && record.MarketSnapshotPreferenceInfo.EntityOrder >= dropBenchmarkDetails.EntityOrder;

                //Shift record order between drag and drop location based on drop flow
                if (recordIsBetweenDragDropLocation)
                {
                    record.MarketSnapshotPreferenceInfo.EntityOrder = dropLocationExceedsDragLocation
                        ? record.MarketSnapshotPreferenceInfo.EntityOrder - 1
                        : record.MarketSnapshotPreferenceInfo.EntityOrder + 1;
                    continue;
                }

                //Check if the record is the drag element
                bool recordIsDragLocation = record.MarketSnapshotPreferenceInfo.EntityOrder == dragBenchmarkDetails.EntityOrder;

                //Change record order if the record element is the drag element
                if (recordIsDragLocation)
                {
                    record.MarketSnapshotPreferenceInfo.EntityOrder = dropLocationExceedsDragLocation
                        ? dropBenchmarkDetails.EntityOrder - 1
                        : dropBenchmarkDetails.EntityOrder;
                }
            }
        }

        /// <summary>
        /// Method to update parameters for reordering inter group
        /// </summary>
        /// <param name="dataContext">List of MorningSnapshotData binded to the grid</param>
        /// <param name="dragBenchmarkDetails">Preference details of the dragged Item</param>
        /// <param name="dropBenchmarkDetails">Preference details of the next to drop location</param>
        /// <param name="dropIsLastOfOtherGroup">true if the drop is done at location last of drop location group</param>
        private void UpdateParametersForDiffGroupReordering(List<MarketPerformanceSnapshotData> dataContext,
            MarketSnapshotPreference dragBenchmarkDetails, MarketSnapshotPreference dropBenchmarkDetails, bool dropIsLastOfOtherGroup)
        {
            bool dropLocationExceedsDragLocation = dropIsLastOfOtherGroup
                ? dropBenchmarkDetails.GroupPreferenceID - 1 > dragBenchmarkDetails.GroupPreferenceID
                : dropBenchmarkDetails.GroupPreferenceID > dragBenchmarkDetails.GroupPreferenceID;

            //True if insertion is done after an element and False if it's done  before the element
            bool dropPositionIsAfter = DragDropPosition.Value == DropPosition.After;

            foreach (MarketPerformanceSnapshotData record in dataContext)
            {
                bool dragGroupRecordIsShifted = record.MarketSnapshotPreferenceInfo.EntityOrder > dragBenchmarkDetails.EntityOrder
                        && record.MarketSnapshotPreferenceInfo.GroupPreferenceID == dragBenchmarkDetails.GroupPreferenceID;

                if (dragGroupRecordIsShifted)
                {
                    record.MarketSnapshotPreferenceInfo.EntityOrder--;
                    continue;
                }

                bool dropGroupRecordIsShifted = record.MarketSnapshotPreferenceInfo.EntityOrder > dropBenchmarkDetails.EntityOrder
                        && record.MarketSnapshotPreferenceInfo.GroupPreferenceID == dropBenchmarkDetails.GroupPreferenceID;


                if (dropGroupRecordIsShifted && !dropIsLastOfOtherGroup)
                {
                    record.MarketSnapshotPreferenceInfo.EntityOrder++;
                    continue;
                }


                bool recordIsDragLocation = record.MarketSnapshotPreferenceInfo.EntityOrder == dragBenchmarkDetails.EntityOrder
                    && record.MarketSnapshotPreferenceInfo.GroupPreferenceID == dragBenchmarkDetails.GroupPreferenceID;

                if (recordIsDragLocation)
                {
                    record.MarketSnapshotPreferenceInfo.EntityOrder = dropIsLastOfOtherGroup
                        ? GetEntityCountInGroup(dataContext, GetLastGroupPreferenceId(dataContext, dropBenchmarkDetails.GroupPreferenceID)) + 1
                        : dropBenchmarkDetails.EntityOrder;
                    record.MarketSnapshotPreferenceInfo.GroupName = dropIsLastOfOtherGroup
                    ? GetLastGroupName(dataContext, dropBenchmarkDetails.GroupPreferenceID)
                    : dropBenchmarkDetails.GroupName;
                    record.MarketSnapshotPreferenceInfo.GroupPreferenceID = dropIsLastOfOtherGroup
                    ? GetLastGroupPreferenceId(dataContext, dropBenchmarkDetails.GroupPreferenceID)
                    : dropBenchmarkDetails.GroupPreferenceID;
                    continue;
                }

                bool recordIsDropLocation = record.MarketSnapshotPreferenceInfo.EntityOrder == dropBenchmarkDetails.EntityOrder
                    && record.MarketSnapshotPreferenceInfo.GroupPreferenceID == dropBenchmarkDetails.GroupPreferenceID;

                if (recordIsDropLocation && !dropIsLastOfOtherGroup)
                {
                    record.MarketSnapshotPreferenceInfo.EntityOrder++;
                    continue;
                }



                //else if (recordIsDragLocation)
                //{
                //    record.MarketSnapshotPreferenceInfo.BenchmarkOrder = dropLocationExceedsDragLocation
                //        ? dropBenchmarkDetails.BenchmarkOrder - 1
                //        : dropBenchmarkDetails.BenchmarkOrder;
                //}
            }
        } 
        #endregion

        #region Event Unsubscribe
        public override void Dispose()
        {
            this._dataContextSource.Dispose();
            this._dataContextSource = null;
            this.DataContext = null;
        }
        #endregion

        private void btnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>();
            RadExportOptionsInfo.Add(new RadExportOptions()
            {
                ElementName = this.txtHeader.Text,
                Element = this.radGridSnapshot,
                ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXPORT_FILTER
            });

            ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + GadgetNames.BENCHMARKS_MARKET_PERFORMANCE_SNAPSHOT);
            childExportOptions.Show();
        }

        private void radGridSnapshot_ElementExporting(object sender, GridViewElementExportingEventArgs e)
        {
            RadGridView_ElementExport.ElementExporting(e, showGroupFooters: false);
        }
    }
}