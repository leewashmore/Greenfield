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
using GreenField.Gadgets.Helpers;
using GreenField.Common;
using Telerik.Windows.Controls.GridView;
using Telerik.Windows.Controls;
using GreenField.ServiceCaller;

namespace GreenField.Gadgets.Views
{
    public partial class ViewSectorBreakdown : ViewBaseUserControl
    {
        #region Properties
        /// <summary>
        /// property to set data context
        /// </summary>
        private ViewModelSectorBreakdown _dataContextSectorBreakdown;
        public ViewModelSectorBreakdown DataContextSectorBreakdown
        {
            get { return _dataContextSectorBreakdown; }
            set { _dataContextSectorBreakdown = value; }
        }

        /// <summary>
        /// property to set IsActive variable of View Model
        /// </summary>
        private bool _isActive;
        public override bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
                if (DataContextSectorBreakdown != null) //DataContext instance
                    DataContextSectorBreakdown.IsActive = _isActive;
            }
        }

        /// <summary>
        /// Export Types to be passed to the ExportOptions class
        /// </summary>
        private static class ExportTypes
        {
            public const string HOLDINGS_SECTOR_BREAKDOWN_CHART = "Sector Breakdown";
            public const string HOLDINGS_SECTOR_BREAKDOWN_GRID = "Sector Breakdown";
        }
        #endregion

        #region Constructor
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="dataContextSource"></param>
        public ViewSectorBreakdown(ViewModelSectorBreakdown dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextSectorBreakdown = dataContextSource;
        } 
        #endregion

        #region Event       

        /// <summary>
        /// Disabling the indentation when grouping is applied in the grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgSectorBreakdown_Rowloaded(object sender, RowLoadedEventArgs e)
        {
            GroupedGridRowLoadedHandler.Implement(e); 
        }
        #endregion

        #region Flip Method
        /// <summary>
        /// Flipping between Grid & PieChart
        /// Using the method FlipItem in class Flipper.cs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFlip_Click(object sender, RoutedEventArgs e)
        {
            if (this.crtSectorBreakdown.Visibility == System.Windows.Visibility.Visible)
            {
                Flipper.FlipItem(this.crtSectorBreakdown, this.dgSectorBreakdown);
            }
            else
            {
                Flipper.FlipItem(this.dgSectorBreakdown, this.crtSectorBreakdown);
            }
        } 
        #endregion

        #region Export To Excel
        /// <summary>
        /// Method to catch Click Event of Export to Excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.crtSectorBreakdown.Visibility == Visibility.Visible)
                {
                    List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>
                {                   
                    new RadExportOptions()
                    {
                        ElementName = ExportTypes.HOLDINGS_SECTOR_BREAKDOWN_CHART,
                        Element = this.crtSectorBreakdown, 
                        ExportFilterOption = RadExportFilterOption.RADCHART_EXPORT_FILTER 
                    },                    
                    
                };
                    ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo,
                    "Export Options: " + GadgetNames.HOLDINGS_SECTOR_BREAKDOWN);
                    childExportOptions.Show();
                }

                else
                {
                    if (this.dgSectorBreakdown.Visibility == Visibility.Visible)
                    {
                        ChildExportOptions childExportOptions = new ChildExportOptions
                    (new List<RadExportOptions>{new RadExportOptions() 
                    {
                        Element = this.dgSectorBreakdown,
                        ElementName = "Sector Breakdown Data",
                        ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXPORT_FILTER
                    }}, "Export Options: " + GadgetNames.HOLDINGS_SECTOR_BREAKDOWN);
                        childExportOptions.Show();
                    }
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog(ex.Message);
            }
        }

        private void dgSectorBreakdown_ElementExporting(object sender, GridViewElementExportingEventArgs e)
        {
            //RadGridView_ElementExport.ElementExporting(e, showGroupFooters: true);
            RadGridView_ElementExport.ElementExporting(e, showGroupFooters: true, aggregatedColumnIndex: new List<int> { 1, 2, 3 });
        }
        #endregion

        #region Dispose Method

        /// <summary>
        /// method to dispose all running events
        /// </summary>
        public override void Dispose()
        {
            this.DataContextSectorBreakdown.Dispose();
            this.DataContextSectorBreakdown = null;
            this.DataContext = null;
        } 
        #endregion

    }
}
