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
using GreenField.Gadgets.ViewModels;
using GreenField.Common;
using GreenField.Gadgets.Helpers;
using GreenField.ServiceCaller;

namespace GreenField.Gadgets.Views
{
    /// <summary>
    /// View Class for Performance Grid that has ViewModelPerformanceGrid as its data source
    /// </summary>
    public partial class ViewPerformanceGrid : ViewBaseUserControl
    {
        #region Constructor
        /// <summary>
        /// Constructor for the class having ViewModelPerformanceGrid as its data context
        /// </summary>
        /// <param name="dataContextSource"></param>
        public ViewPerformanceGrid(ViewModelPerformanceGrid dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextPerformanceGrid = dataContextSource;
            dataContextSource.PerformanceGridDataLoadedEvent +=
            new DataRetrievalProgressIndicatorEventHandler(dataContextSource_performanceGridDataLoadedEvent);
        }
        #endregion

        #region Private Members
        /// <summary>
        /// Data Retrieval Indicator
        /// </summary>
        /// <param name="e"></param>
        void dataContextSource_performanceGridDataLoadedEvent(DataRetrievalProgressIndicatorEventArgs e)
        {
            if (e.ShowBusy)
            {
                this.busyIndicatorGrid.IsBusy = true;
            }
            else
            {
                this.busyIndicatorGrid.IsBusy = false;
            }
        }

        /// <summary>
        /// Method to catch Click Event of Export to Excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.dgPerformance.Visibility == Visibility.Visible)
                {
                    List<RadExportOptions> radExportOptionsInfo = new List<RadExportOptions>
                    {
                          new RadExportOptions() { ElementName = "Performance Grid", Element = this.dgPerformance, ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXPORT_FILTER }
                    };
                    ChildExportOptions childExportOptions = new ChildExportOptions(radExportOptionsInfo, "Export Options: " + GadgetNames.PERFORMANCE_GRID);
                    childExportOptions.Show();
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog(ex.Message);
            }
        }

        /// <summary>
        /// Styles added to Export
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgPerformanceGrid_ElementExporting(object sender, Telerik.Windows.Controls.GridViewElementExportingEventArgs e)
        {
            RadGridView_ElementExport.ElementExporting(e);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Data Context Property
        /// </summary>
        private ViewModelPerformanceGrid dataContextPerformanceGrid;
        public ViewModelPerformanceGrid DataContextPerformanceGrid
        {
            get { return dataContextPerformanceGrid; }
            set { dataContextPerformanceGrid = value; }
        }

        /// <summary>
        /// True is gadget is currently on display
        /// </summary>
        private bool isActive;
        public override bool IsActive
        {
            get { return isActive; }
            set
            {
                isActive = value;
                if (this.DataContext != null)
                {
                    ((ViewModelPerformanceGrid)DataContext).IsActive = isActive;
                }
            }
        }
        #endregion

        #region RemoveEvents
        /// <summary>
        /// Disposes Events
        /// </summary>
        public override void Dispose()
        {
            this.DataContextPerformanceGrid.PerformanceGridDataLoadedEvent -= new DataRetrievalProgressIndicatorEventHandler(dataContextSource_performanceGridDataLoadedEvent);
            this.DataContextPerformanceGrid.Dispose();
            this.DataContextPerformanceGrid = null;
            this.DataContext = null;
        }
        #endregion
    }
}
