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
using GreenField.ServiceCaller.ProxyDataDefinitions;
using GreenField.Gadgets.ViewModels;
using GreenField.Gadgets.Helpers;
using System.Collections.ObjectModel;

namespace GreenField.Gadgets.Views
{
    public partial class ViewMarketPerformanceSnapshot : UserControl
    {
        private ViewModelMarketPerformanceSnapshot _dataContextSource;

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
        /// Set Snapshot Grid Column Headers
        /// </summary>
        private void SetGridHeaders()
        {
            //Entity Descrition
            this.radGridSnapshot.Columns[0].Header = String.Empty;
            //Entity Return Type
            this.radGridSnapshot.Columns[1].Header = "Return";
            //Market Performance for Last Working Date
            this.radGridSnapshot.Columns[2].Header = DateTime.Today.AddDays(-1).ToShortDateString();
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

        private void ReorderBehavior_Reordered(object sender, ReorderedEventArgs e)
        {
            this.radGridSnapshot.Rebind();
            string A = String.Empty;
            List<MarketPerformanceSnapshotData> dataContext = (e.SourceGrid.ItemsSource as ObservableCollection<MarketPerformanceSnapshotData>).ToList();
            foreach (MarketPerformanceSnapshotData record in dataContext)
            {
                if (record.MarketSnapshotPreferenceInfo.EntityName != null)
                {
                    A = A + record.MarketSnapshotPreferenceInfo.EntityName + " " +
                        record.MarketSnapshotPreferenceInfo.GroupPreferenceID.ToString() + " " +
                        ((int)record.MarketSnapshotPreferenceInfo.EntityOrder).ToString() + "\n";
                }
            }
            MessageBox.Show(A);

        }

        private void ReorderBehavior_Reordering(object sender, ReorderingEventArgs e)
        {
            //If there are no dragged Items reordering is redundant
            if (e.DraggedItems.Count().Equals(0))
                return;

            //Collection of MorningSnapshotData binded to the grid
            List<MarketPerformanceSnapshotData> dataContext = (e.SourceGrid.ItemsSource as ObservableCollection<MarketPerformanceSnapshotData>).ToList();

            //Dragged Element
            MarketPerformanceSnapshotData draggedElement = e.DraggedItems.FirstOrDefault() as MarketPerformanceSnapshotData;

            //Null Exception Handling
            if (draggedElement == null)
                return;

            int dragIndex = dataContext.IndexOf(draggedElement);
            int dropIndex = e.DropIndex;

            //Check if the drop Index exceed the Count of List - item dropped after the last element
            bool dropIndexExceedsCount = dropIndex >= dataContext.Count;

            //True if insertion is done after an element and False if it's done  before the element
            bool dropPositionIsAfter = DragDropPosition.Value == DropPosition.After;

            #region Drag Location Parameters
            MarketSnapshotPreference dragBenchmarkDetails = new MarketSnapshotPreference()
            {
                EntityName = dataContext.ElementAt(dragIndex).MarketSnapshotPreferenceInfo.EntityName,
                EntityReturnType = dataContext.ElementAt(dragIndex).MarketSnapshotPreferenceInfo.EntityReturnType,
                EntityOrder = dataContext.ElementAt(dragIndex).MarketSnapshotPreferenceInfo.EntityOrder,
                GroupName = dataContext.ElementAt(dragIndex).MarketSnapshotPreferenceInfo.GroupName,
                GroupPreferenceID = dataContext.ElementAt(dragIndex).MarketSnapshotPreferenceInfo.GroupPreferenceID
            };

            //Count of Benchmarks in the Group of the dragged item
            //int dragGroupBenchmarkCount = GetBenchmarkCountInGroup(dataContext, dragGroupOrder);             
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

            //Count of Benchmarks in the Group of the drop location
            //int dropGroupBenchmarkCount = GetBenchmarkCountInGroup(dataContext, dropGroupOrder);             
            #endregion

            bool dragdropGroupIsSame = dragBenchmarkDetails.GroupPreferenceID == dropBenchmarkDetails.GroupPreferenceID;

            bool dropIsLastOfSameGroup = dragdropGroupIsSame
                ? dropBenchmarkDetails.EntityOrder == GetBenchmarkCountInGroup(dataContext, dropBenchmarkDetails.GroupPreferenceID) + 1
                : dropBenchmarkDetails.GroupPreferenceID == dragBenchmarkDetails.GroupPreferenceID + 1
                    && (dropBenchmarkDetails.EntityOrder == 1 || dropBenchmarkDetails.EntityOrder == null)
                    && dropPositionIsAfter;

            bool dragIsWithinGroup = dragdropGroupIsSame || dropIsLastOfSameGroup;

            if (dragIsWithinGroup)
            {
                UpdateParametersForSameGroupReordering(dataContext, dragBenchmarkDetails, dropBenchmarkDetails, dropIsLastOfSameGroup);
            }
            else
            {
                UpdateParametersForDiffGroupReordering(dataContext, dragBenchmarkDetails, dropBenchmarkDetails);
            }

        }

        private int GetBenchmarkCountInGroup(List<MarketPerformanceSnapshotData> dataContext, int groupPreferenceId)
        {
            return dataContext.Where(t => t.MarketSnapshotPreferenceInfo.GroupPreferenceID == groupPreferenceId).Count();
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
                dropBenchmarkDetails.EntityOrder = GetBenchmarkCountInGroup(dataContext, dragBenchmarkDetails.GroupPreferenceID) + 1;

            //Dropped at Group end within the same group
            if (dropIsLastOfSameGroup)
            {
                if (dragBenchmarkDetails.GroupPreferenceID != dropBenchmarkDetails.GroupPreferenceID)
                {
                    dropBenchmarkDetails.EntityOrder = GetBenchmarkCountInGroup(dataContext, dragBenchmarkDetails.GroupPreferenceID) + 1;
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
        /// 
        /// </summary>
        /// <param name="dataContext"></param>
        /// <param name="dragBenchmarkDetails"></param>
        /// <param name="dropBenchmarkDetails"></param>
        private void UpdateParametersForDiffGroupReordering(List<MarketPerformanceSnapshotData> dataContext,
            MarketSnapshotPreference dragBenchmarkDetails, MarketSnapshotPreference dropBenchmarkDetails)
        {
            bool dropLocationExceedsDragLocation = dropBenchmarkDetails.GroupPreferenceID > dragBenchmarkDetails.GroupPreferenceID;

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


                if (dropGroupRecordIsShifted)
                {
                    record.MarketSnapshotPreferenceInfo.EntityOrder++;
                    continue;
                }


                bool recordIsDragLocation = record.MarketSnapshotPreferenceInfo.EntityOrder == dragBenchmarkDetails.EntityOrder
                    && record.MarketSnapshotPreferenceInfo.GroupPreferenceID == dragBenchmarkDetails.GroupPreferenceID;

                if (recordIsDragLocation)
                {
                    record.MarketSnapshotPreferenceInfo.EntityOrder = dropBenchmarkDetails.EntityOrder;
                    record.MarketSnapshotPreferenceInfo.GroupName = dropBenchmarkDetails.GroupName;
                    record.MarketSnapshotPreferenceInfo.GroupPreferenceID = dropBenchmarkDetails.GroupPreferenceID;
                    continue;
                }

                bool recordIsDropLocation = record.MarketSnapshotPreferenceInfo.EntityOrder == dropBenchmarkDetails.EntityOrder
                    && record.MarketSnapshotPreferenceInfo.GroupPreferenceID == dropBenchmarkDetails.GroupPreferenceID;

                if (recordIsDropLocation)
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

            

    }
}