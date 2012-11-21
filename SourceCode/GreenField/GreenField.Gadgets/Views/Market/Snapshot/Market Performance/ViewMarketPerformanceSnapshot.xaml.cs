using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;
using GreenField.Common;
using GreenField.Gadgets.Helpers;
using GreenField.Gadgets.ViewModels;
using GreenField.ServiceCaller.PerformanceDefinitions;
using GreenField.ServiceCaller;

namespace GreenField.Gadgets.Views
{
    /// <summary>
    /// Code behind for ViewMarketPerformanceSnapshot
    /// </summary>
    public partial class ViewMarketPerformanceSnapshot : ViewBaseUserControl
    {
        #region Properties
        /// <summary>
        /// View Model Instance
        /// </summary>
        private ViewModelMarketPerformanceSnapshot dataContextSource;
        public ViewModelMarketPerformanceSnapshot DataContextSource 
        {
            get { return dataContextSource; }
            set { dataContextSource = value; }
        }

        /// <summary>
        /// property to set IsActive variable of View Model
        /// </summary>
        private bool isActive;
        public override bool IsActive
        {
            get { return isActive; }
            set
            {
                isActive = value;
                if (DataContextSource != null)
                {
                    DataContextSource.IsActive = isActive;
                }
            }
        }
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
            DataContextSource = dataContextSource;
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
            {
                return;
            }
            RadContextMenu menu = (RadContextMenu)sender;

            if (menu == null)
            {
                return;
            }
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
                foreach (MarketSnapshotPerformanceData item in this.radGridSnapshot.Items)
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
        /// ReorderBehavior Reordered Event Handler - Rebinds Datacontext post reordering
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">ReorderedEventArgs</param>
        private void ReorderBehavior_Reordered(object sender, ReorderedEventArgs e)
        {
            //Update MarketSnapshotPreferenceInfo after reordering
            DataContextSource.MarketSnapshotPreferenceInfo = DataContextSource.MarketSnapshotPerformanceInfo
                .Select(record => record.MarketSnapshotPreferenceInfo)
                .ToList();

            this.radGridSnapshot.Rebind();
        }

        /// <summary>
        /// ReorderBehavior Reordering Event Handler - Adjusts Entity Order and GroupPreferenceId of entities after reordering
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">ReorderingEventArgs</param>
        private void ReorderBehavior_Reordering(object sender, ReorderingEventArgs e)
        {
            //if there are no dragged Items reordering is redundant
            if (e.DraggedItems.Count().Equals(0))
                return;

            #region Get Dragged Element
            //dragged Element
            MarketSnapshotPerformanceData draggedElement = e.DraggedItems.FirstOrDefault() as MarketSnapshotPerformanceData;

            //null Exception Handling
            if (draggedElement == null)
            {
                return;
            }
            #endregion

            #region Get Data Context
            //collection of MorningSnapshotData binded to the grid
            List<MarketSnapshotPerformanceData> dataContext = (e.SourceGrid.ItemsSource as ObservableCollection<MarketSnapshotPerformanceData>).ToList();

            //null Exception Handling
            if (dataContext == null)
            {
                return;
            }
            #endregion

            #region Get Drag Drop Details
            //drag drop Indexes
            int dragIndex = dataContext.IndexOf(draggedElement);
            int dropIndex = e.DropIndex;
            //true if insertion is done after an element and False if it's done  before the element
            bool isDropPositionAfter = DragDropPosition.Value == DropPosition.After;
            //check if the drop Index exceed the Count of List - item dropped after the last element
            bool isDropIndexExceedingCount = dropIndex >= dataContext.Count;
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
                EntityName = isDropIndexExceedingCount ? null : dataContext.ElementAt(dropIndex).MarketSnapshotPreferenceInfo.EntityName,
                EntityReturnType = isDropIndexExceedingCount ? null : dataContext.ElementAt(e.DropIndex).MarketSnapshotPreferenceInfo.EntityReturnType,
                EntityOrder = isDropIndexExceedingCount
                    ? dataContext.ElementAt(dropIndex - 1).MarketSnapshotPreferenceInfo.EntityOrder + 1
                    : dataContext.ElementAt(dropIndex).MarketSnapshotPreferenceInfo.EntityOrder,
                GroupName = isDropIndexExceedingCount
                    ? dataContext.ElementAt(dropIndex - 1).MarketSnapshotPreferenceInfo.GroupName
                    : dataContext.ElementAt(dropIndex).MarketSnapshotPreferenceInfo.GroupName,
                GroupPreferenceID = isDropIndexExceedingCount
                    ? dataContext.ElementAt(dropIndex - 1).MarketSnapshotPreferenceInfo.GroupPreferenceID
                    : dataContext.ElementAt(dropIndex).MarketSnapshotPreferenceInfo.GroupPreferenceID
            };
            #endregion

            #region Managing discrepancies in drop location
            //if drop location is after the last element of same or another group the behavior picks the next element nevertheless
            bool isDragdropGroupSame = dragBenchmarkDetails.GroupPreferenceID == dropBenchmarkDetails.GroupPreferenceID;
            bool isDropEntityMisread = dropBenchmarkDetails.EntityOrder == 1 && isDropPositionAfter;

            bool isDropLastOfSameGroup = GetLastGroupPreferenceId(dataContext, dropBenchmarkDetails.GroupPreferenceID) 
                == dragBenchmarkDetails.GroupPreferenceID && isDropEntityMisread;
            bool isDropLastOfOtherGroup = (!isDropLastOfSameGroup) && isDropEntityMisread;
            #endregion

            bool isDragWithinGroup = isDragdropGroupSame ? (!isDropLastOfOtherGroup) : isDropLastOfSameGroup;

            if (isDragWithinGroup)
            {
                UpdateParametersForSameGroupReordering(dataContext, dragBenchmarkDetails, dropBenchmarkDetails, isDropLastOfSameGroup);
            }
            else
            {
                UpdateParametersForDiffGroupReordering(dataContext, dragBenchmarkDetails, dropBenchmarkDetails, isDropLastOfOtherGroup);
            }
        }

        /// <summary>
        /// Export Excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            List<RadExportOptions> radExportOptionsInfo = new List<RadExportOptions>();
            radExportOptionsInfo.Add(new RadExportOptions()
            {
                ElementName = this.txtHeader.Text,
                Element = this.radGridSnapshot,
                ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXCEL_EXPORT_FILTER
            });
            ChildExportOptions childExportOptions = new ChildExportOptions(radExportOptionsInfo, "Export Options: " 
                + GadgetNames.BENCHMARKS_MARKET_PERFORMANCE_SNAPSHOT);
            childExportOptions.Show();
        }

        /// <summary>
        /// Printing the DataGrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            try
            {
                List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>();
                RadExportOptionsInfo.Add(new RadExportOptions()
                {
                    ElementName = "Market Performance Snapshot",
                    Element = this.radGridSnapshot,
                    ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_PRINT_FILTER,
                    RichTextBox = this.RichTextBox
                });

                ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: Market Performance Snapshot");
                childExportOptions.Show();

            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
            }
        }

        /// <summary>
        /// Event handler when user wants to Export the Grid to PDF
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportPdf_Click(object sender, RoutedEventArgs e)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            try
            {

                List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>();
                RadExportOptionsInfo.Add(new RadExportOptions()
                {
                    ElementName = "MacroDB Key Annual Report",
                    Element = this.radGridSnapshot,
                    ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_PDF_EXPORT_FILTER
                });

                ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: Market Performance Snapshot");
                childExportOptions.Show();
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
            }
        }

        /// <summary>
        /// Grid Element Exporting
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radGridSnapshot_ElementExporting(object sender, GridViewElementExportingEventArgs e)
        {
            RadGridView_ElementExport.ElementExporting(e, isGroupFootersVisible: false);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Set Snapshot Grid Column Headers
        /// </summary>
        private void SetGridHeaders()
        {
            //entity description
            this.radGridSnapshot.Columns[0].Header = "Description";
            this.radGridSnapshot.Columns[0].UniqueName = "Description";
            //entity return Type
            this.radGridSnapshot.Columns[1].Header = "Return";
            this.radGridSnapshot.Columns[1].UniqueName = "Return";
            //market performance for last working date
            this.radGridSnapshot.Columns[2].Header = DateTime.Today.AddDays(-1).ToString("d");
            this.radGridSnapshot.Columns[2].UniqueName = DateTime.Today.AddDays(-1).ToString("d");
            //market performance for week to date
            this.radGridSnapshot.Columns[3].Header = "WTD";
            this.radGridSnapshot.Columns[3].UniqueName = "WTD";
            //market performance for month to date
            this.radGridSnapshot.Columns[4].Header = "MTD";
            this.radGridSnapshot.Columns[4].UniqueName = "MTD";
            //market performance for quarter to date
            this.radGridSnapshot.Columns[5].Header = "QTD";
            this.radGridSnapshot.Columns[5].UniqueName = "QTD";
            //market performance for year to date
            this.radGridSnapshot.Columns[6].Header = "YTD";
            this.radGridSnapshot.Columns[6].UniqueName = "YTD";
            //market performance for last year
            this.radGridSnapshot.Columns[7].Header = DateTime.Today.AddYears(-1).Year.ToString();
            this.radGridSnapshot.Columns[7].UniqueName = DateTime.Today.AddYears(-1).Year.ToString();
            //market performance for second last year
            this.radGridSnapshot.Columns[8].Header = DateTime.Today.AddYears(-2).Year.ToString();
            this.radGridSnapshot.Columns[8].UniqueName = DateTime.Today.AddYears(-2).Year.ToString();
            //market performance for third last year
            this.radGridSnapshot.Columns[9].Header = DateTime.Today.AddYears(-3).Year.ToString();
            this.radGridSnapshot.Columns[9].UniqueName = DateTime.Today.AddYears(-3).Year.ToString();
        }

        /// <summary>
        /// Method to get count of entities within propertyName group
        /// </summary>
        /// <param name="dataContext">List of MarketSnapshotPerformanceData</param>
        /// <param name="groupPreferenceId">GroupPreferenceId value</param>
        /// <returns></returns>
        private int GetEntityCountInGroup(List<MarketSnapshotPerformanceData> dataContext, int groupPreferenceId)
        {
            return dataContext.Where(t => t.MarketSnapshotPreferenceInfo.GroupPreferenceID == groupPreferenceId).Count();
        }

        /// <summary>
        /// Method to get GroupPreferenceId of the last group in snapshot
        /// </summary>
        /// <param name="dataContext">List of MarketSnapshotPerformanceData</param>
        /// <param name="groupPreferenceId">GroupPreferenceId value</param>
        /// <returns>GroupPreferenceId value</returns>
        private int GetLastGroupPreferenceId(List<MarketSnapshotPerformanceData> dataContext, int groupPreferenceId)
        {
            List<MarketSnapshotPerformanceData> orderedDataContext = dataContext
                .OrderBy(record => record.MarketSnapshotPreferenceInfo.GroupPreferenceID)
                .ThenBy(record => record.MarketSnapshotPreferenceInfo.EntityOrder)
                .ToList();
            int lastGroupPreferenceId = 0;
            foreach (MarketSnapshotPerformanceData data in orderedDataContext)
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
        private string GetLastGroupName(List<MarketSnapshotPerformanceData> dataContext, int groupPreferenceId)
        {
            List<MarketSnapshotPerformanceData> orderedDataContext = dataContext
                .OrderBy(record => record.MarketSnapshotPreferenceInfo.GroupPreferenceID)
                .ThenBy(record => record.MarketSnapshotPreferenceInfo.EntityOrder)
                .ToList();
            int lastGroupPreferenceId = 0;
            string lastGroupName = String.Empty;

            foreach (MarketSnapshotPerformanceData data in orderedDataContext)
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
        /// <param name="isDropLastOfSameGroup">True if the drop location is last within the drag item group</param>
        private void UpdateParametersForSameGroupReordering(List<MarketSnapshotPerformanceData> dataContext,
            MarketSnapshotPreference dragBenchmarkDetails, MarketSnapshotPreference dropBenchmarkDetails, bool isDropLastOfSameGroup)
        {
            //dropped at group end with the next group empty
            if (dropBenchmarkDetails.EntityOrder == null)
            {
                dropBenchmarkDetails.EntityOrder = GetEntityCountInGroup(dataContext, dragBenchmarkDetails.GroupPreferenceID) + 1;
            }
            //dropped at group end within the same group
            if (isDropLastOfSameGroup)
            {
                if (dragBenchmarkDetails.GroupPreferenceID != dropBenchmarkDetails.GroupPreferenceID)
                {
                    dropBenchmarkDetails.EntityOrder = GetEntityCountInGroup(dataContext, dragBenchmarkDetails.GroupPreferenceID) + 1;
                }
            }

            //check drop flow - top ot bottom or vice versa
            bool isDropLocationExceedingDragLocation = dropBenchmarkDetails.EntityOrder > dragBenchmarkDetails.EntityOrder;

            foreach (MarketSnapshotPerformanceData record in dataContext)
            {
                //no consideration of records in other groups
                if (record.MarketSnapshotPreferenceInfo.GroupPreferenceID != dragBenchmarkDetails.GroupPreferenceID)
                {
                    continue;
                }
                //check if the record is between drag and drop location
                bool isRecordBetweenDragDropLocation = isDropLocationExceedingDragLocation
                    ? record.MarketSnapshotPreferenceInfo.EntityOrder > dragBenchmarkDetails.EntityOrder
                        && record.MarketSnapshotPreferenceInfo.EntityOrder < dropBenchmarkDetails.EntityOrder
                    : record.MarketSnapshotPreferenceInfo.EntityOrder < dragBenchmarkDetails.EntityOrder
                        && record.MarketSnapshotPreferenceInfo.EntityOrder >= dropBenchmarkDetails.EntityOrder;

                //shift record order between drag and drop location based on drop flow
                if (isRecordBetweenDragDropLocation)
                {
                    record.MarketSnapshotPreferenceInfo.EntityOrder = isDropLocationExceedingDragLocation
                        ? record.MarketSnapshotPreferenceInfo.EntityOrder - 1
                        : record.MarketSnapshotPreferenceInfo.EntityOrder + 1;
                    continue;
                }
                //check if the record is the drag element
                bool isRecordDragLocation = record.MarketSnapshotPreferenceInfo.EntityOrder == dragBenchmarkDetails.EntityOrder;

                //change record order if the record element is the drag element
                if (isRecordDragLocation)
                {
                    record.MarketSnapshotPreferenceInfo.EntityOrder = isDropLocationExceedingDragLocation
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
        /// <param name="isDropLastOfOtherGroup">true if the drop is done at location last of drop location group</param>
        private void UpdateParametersForDiffGroupReordering(List<MarketSnapshotPerformanceData> dataContext,
            MarketSnapshotPreference dragBenchmarkDetails, MarketSnapshotPreference dropBenchmarkDetails, bool isDropLastOfOtherGroup)
        {
            bool isDropLocationExceedingDragLocation = isDropLastOfOtherGroup
                ? dropBenchmarkDetails.GroupPreferenceID - 1 > dragBenchmarkDetails.GroupPreferenceID
                : dropBenchmarkDetails.GroupPreferenceID > dragBenchmarkDetails.GroupPreferenceID;

            //true if insertion is done after an element and false if it's done  before the element
            bool isDropPositionAfter = DragDropPosition.Value == DropPosition.After;

            foreach (MarketSnapshotPerformanceData record in dataContext)
            {
                bool isDragGroupRecordShifted = record.MarketSnapshotPreferenceInfo.EntityOrder > dragBenchmarkDetails.EntityOrder
                        && record.MarketSnapshotPreferenceInfo.GroupPreferenceID == dragBenchmarkDetails.GroupPreferenceID;

                if (isDragGroupRecordShifted)
                {
                    record.MarketSnapshotPreferenceInfo.EntityOrder--;
                    continue;
                }
                bool isDropGroupRecordShifted = record.MarketSnapshotPreferenceInfo.EntityOrder > dropBenchmarkDetails.EntityOrder
                        && record.MarketSnapshotPreferenceInfo.GroupPreferenceID == dropBenchmarkDetails.GroupPreferenceID;

                if (isDropGroupRecordShifted && !isDropLastOfOtherGroup)
                {
                    record.MarketSnapshotPreferenceInfo.EntityOrder++;
                    continue;
                }
                bool isRecordDragLocation = record.MarketSnapshotPreferenceInfo.EntityOrder == dragBenchmarkDetails.EntityOrder
                    && record.MarketSnapshotPreferenceInfo.GroupPreferenceID == dragBenchmarkDetails.GroupPreferenceID;

                if (isRecordDragLocation)
                {
                    record.MarketSnapshotPreferenceInfo.EntityOrder = isDropLastOfOtherGroup
                        ? GetEntityCountInGroup(dataContext, GetLastGroupPreferenceId(dataContext, dropBenchmarkDetails.GroupPreferenceID)) + 1
                        : dropBenchmarkDetails.EntityOrder;
                    record.MarketSnapshotPreferenceInfo.GroupName = isDropLastOfOtherGroup
                    ? GetLastGroupName(dataContext, dropBenchmarkDetails.GroupPreferenceID)
                    : dropBenchmarkDetails.GroupName;
                    record.MarketSnapshotPreferenceInfo.GroupPreferenceID = isDropLastOfOtherGroup
                    ? GetLastGroupPreferenceId(dataContext, dropBenchmarkDetails.GroupPreferenceID)
                    : dropBenchmarkDetails.GroupPreferenceID;
                    continue;
                }
                bool isRecordDropLocation = record.MarketSnapshotPreferenceInfo.EntityOrder == dropBenchmarkDetails.EntityOrder
                    && record.MarketSnapshotPreferenceInfo.GroupPreferenceID == dropBenchmarkDetails.GroupPreferenceID;

                if (isRecordDropLocation && !isDropLastOfOtherGroup)
                {
                    record.MarketSnapshotPreferenceInfo.EntityOrder++;
                    continue;
                }
            }
        }
        #endregion

        #region Event Unsubscribe
        /// <summary>
        /// Dispose objects from memory
        /// </summary>
        public override void Dispose()
        {
            this.DataContextSource.Dispose();
            this.DataContextSource = null;
            this.DataContext = null;
        }
        #endregion
    }
}