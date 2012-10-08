using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Telerik.Windows;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;
using GreenField.DataContracts;
using GreenField.Gadgets.Helpers;
using GreenField.Gadgets.ViewModels;

namespace GreenField.Gadgets.Views
{
    /// <summary>
    /// XAML.cs class for CSTDataFieldSelector
    /// </summary>
    public partial class ViewCSTDataFieldSelector : ViewBaseUserControl
    {   
        #region Properties
        /// <summary>
        /// Property to set data context
        /// </summary>
        private ViewModelCSTDataFieldSelector dataContextViewModelCSTDataFieldSelector;
        public ViewModelCSTDataFieldSelector DataContextViewModelCSTDataFieldSelector
        {
            get { return dataContextViewModelCSTDataFieldSelector; }
            set { dataContextViewModelCSTDataFieldSelector = value; }
        }

        /// <summary>
        /// Property to set IsActive variable of View Model
        /// </summary>
        private bool isActive;
        public override bool IsActive
        {
            get { return isActive; }
            set
            {
                isActive = value;
                // dataContext instance
                if (DataContextViewModelCSTDataFieldSelector != null) 
                    DataContextViewModelCSTDataFieldSelector.IsActive = isActive;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataContextSource"></param>
        public ViewCSTDataFieldSelector(ViewModelCSTDataFieldSelector dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextViewModelCSTDataFieldSelector = dataContextSource;
        }     
        #endregion

        /// <summary>
        /// ReorderBehavior Reordered Event Handler - Rebinds Datacontext post reordering
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">ReorderedEventArgs</param>
        private void ReorderBehavior_Reordered(object sender, ReorderedEventArgs e)
        {
            //Update MarketSnapshotPreferenceInfo after reordering
            //DataContextViewModelCSTDataFieldSelector.SelectedFieldsDataList = DataContextViewModelCSTDataFieldSelector.SelectedFieldsDataList
            //    .Select(record => record.SelectedFieldsDataList)
            //    .ToList();

            #region Client cache implementation
            //if (_dataContextSource.PopulatedMarketPerformanceSnapshotInfo != null)
            //{
            //    PopulatedMarketSnapshotPerformanceData selectedPopulatedMarketPerformanceSnapshotInfo = _dataContextSource.PopulatedMarketPerformanceSnapshotInfo
            //                        .Where(record => record.MarketSnapshotSelectionInfo == _dataContextSource.SelectedMarketSnapshotSelectionInfo).FirstOrDefault();

            //    if (selectedPopulatedMarketPerformanceSnapshotInfo != null)
            //    {
            //        _dataContextSource.PopulatedMarketPerformanceSnapshotInfo
            //            .Where(record => record.MarketSnapshotSelectionInfo == _dataContextSource.SelectedMarketSnapshotSelectionInfo)
            //            .FirstOrDefault()
            //            .MarketPerformanceSnapshotInfo = _dataContextSource.MarketPerformanceSnapshotInfo.ToList();
            //    }
            //}
            #endregion

            //this.radGridSnapshot.Rebind();
            //_dataContextSource.TestEntityOrdering();

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
            CSTUserPreferenceInfo draggedElement = e.DraggedItems.FirstOrDefault() as CSTUserPreferenceInfo;

            //Null Exception Handling
            if (draggedElement == null)
                return;
            #endregion

            #region Get Data Context
            //Collection of CSTUserPreferenceInfo binded to the grid
            List<CSTUserPreferenceInfo> dataContext = (e.SourceGrid.ItemsSource as ObservableCollection<CSTUserPreferenceInfo>).ToList();

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

            //dataContext.ElementAt(dropIndex).DataPointsOrder = dropPositionIsAfter ? dataContext.ElementAt(dragIndex).DataPointsOrder + (dropIndex - dragIndex)
            //                                                                       : dataContext.ElementAt(dragIndex).DataPointsOrder - (dragIndex - dropIndex);

            //Check if the drop Index exceed the Count of List - item dropped after the last element
            bool dropIndexExceedsCount = dropIndex >= dataContext.Count;
            #endregion

            #region Drag Location Parameters
            CSTUserPreferenceInfo dragUserPreferenceInfo = new CSTUserPreferenceInfo()
            {
                ScreeningId = dataContext.ElementAt(dragIndex).ScreeningId,
                DataDescription = dataContext.ElementAt(dragIndex).DataDescription,
                UserName = UserSession.SessionManager.SESSION.UserName,
                ListName = dataContext.ElementAt(dragIndex).ListName,
                Accessibility = dataContext.ElementAt(dragIndex).Accessibility,
                DataPointsOrder = dataContext.ElementAt(dragIndex).DataPointsOrder,
                TableColumn = dataContext.ElementAt(dragIndex).TableColumn
            };
            #endregion

            #region Drop Location Parameters
            CSTUserPreferenceInfo dropUserPreferenceInfo = new CSTUserPreferenceInfo()
            {
                ScreeningId = dropIndexExceedsCount ? null : dataContext.ElementAt(dropIndex).ScreeningId,
                DataDescription = dropIndexExceedsCount ? null : dataContext.ElementAt(e.DropIndex).DataDescription,
                UserName = UserSession.SessionManager.SESSION.UserName,
                ListName = dataContext.ElementAt(dragIndex).ListName,
                Accessibility = dataContext.ElementAt(dragIndex).Accessibility,
                DataPointsOrder = dropIndexExceedsCount ? dataContext.ElementAt(dropIndex - 1).DataPointsOrder
                                                      : dataContext.ElementAt(dropIndex).DataPointsOrder,
                TableColumn = dataContext.ElementAt(dragIndex).TableColumn
            };
            #endregion

            UpdateParametersForSameGroupReordering(dataContext, dragUserPreferenceInfo, dropUserPreferenceInfo, dropIndexExceedsCount);            
        }

        /// <summary>
        /// Updates the Element Order in the list based on the drag drop parameters
        /// </summary>
        /// <param name="dataContext">List of CSTUserPreferenceInfo binded to the grid</param>
        /// <param name="dragUserPreferenceInfo">CSTUserPreferenceInfo</param>
        /// <param name="dropBenchmarkDetails">CSTUserPreferenceInfo</param>        
        private void UpdateParametersForSameGroupReordering(List<CSTUserPreferenceInfo> dataContext,
            CSTUserPreferenceInfo dragUserPreferenceInfo, CSTUserPreferenceInfo dropUserPreferenceInfo, bool dropIndexExceedsCount)
        {
            ////Dropped at end
            //if (dropIndexExceedsCount)
            //{
            //    if (dragUserPreferenceInfo.DataPointsOrder != dropUserPreferenceInfo.DataPointsOrder)
            //    {
            //        dropUserPreferenceInfo.DataPointsOrder = dataContext.Count + 1;
            //    }
            //}

            //Check drop flow - top ot bottom or vice versa
            bool dropLocationExceedsDragLocation = dropUserPreferenceInfo.DataPointsOrder > dragUserPreferenceInfo.DataPointsOrder;

            foreach (CSTUserPreferenceInfo record in dataContext)
            {
                //Check if the record is between drag and drop location
                bool recordIsBetweenDragDropLocation = dropLocationExceedsDragLocation
                    ? record.DataPointsOrder > dragUserPreferenceInfo.DataPointsOrder
                        && record.DataPointsOrder < dropUserPreferenceInfo.DataPointsOrder
                    : record.DataPointsOrder < dragUserPreferenceInfo.DataPointsOrder
                        && record.DataPointsOrder >= dropUserPreferenceInfo.DataPointsOrder;

                //Shift record order between drag and drop location based on drop flow
                if (recordIsBetweenDragDropLocation)
                {
                    record.DataPointsOrder = dropLocationExceedsDragLocation
                        ? record.DataPointsOrder - 1
                        : record.DataPointsOrder + 1;
                    continue;
                }

                //Check if the record is the drag element
                bool recordIsDragLocation = record.DataPointsOrder == dragUserPreferenceInfo.DataPointsOrder;

                //Change record order if the record element is the drag element
                if (recordIsDragLocation)
                {
                    record.DataPointsOrder = dropLocationExceedsDragLocation
                        ? dropUserPreferenceInfo.DataPointsOrder - 1
                        : dropUserPreferenceInfo.DataPointsOrder;
                }
            }
        }

        #region Dispose Method
        /// <summary>
        /// Method to dispose all running events
        /// </summary>
        public override void Dispose()
        {
            this.DataContextViewModelCSTDataFieldSelector.Dispose();
            this.DataContextViewModelCSTDataFieldSelector = null;
            this.DataContext = null;
        }
        #endregion
    }
}
