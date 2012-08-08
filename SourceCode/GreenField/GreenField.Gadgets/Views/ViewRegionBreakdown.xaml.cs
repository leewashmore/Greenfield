using System.Collections.Generic;
using System.Linq;
using System.Windows;
using GreenField.Gadgets.ViewModels;
using GreenField.Gadgets.Helpers;
using GreenField.Common;
using Telerik.Windows.Controls.GridView;
using Telerik.Windows.Controls;
using System.Windows.Media;
using System;
using GreenField.ServiceCaller;
using GreenField.DataContracts;


namespace GreenField.Gadgets.Views
{
    public partial class ViewRegionBreakdown : ViewBaseUserControl
    {
        #region Property
        /// <summary>
        /// property to set data context
        /// </summary>
        private ViewModelRegionBreakdown _dataContextRegionBreakdown;
        public ViewModelRegionBreakdown DataContextRegionBreakdown
        {
            get { return _dataContextRegionBreakdown; }
            set { _dataContextRegionBreakdown = value; }
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
                if (DataContextRegionBreakdown != null) //DataContext instance
                    DataContextRegionBreakdown.IsActive = _isActive;
            }
        }

        /// <summary>
        /// Export Types to be passed to the ExportOptions class
        /// </summary>
        private static class ExportTypes
        {
            public const string HOLDINGS_REGION_BREAKDOWN_CHART = "Region Breakdown";
            public const string HOLDINGS_REGION_BREAKDOWN_GRID = "Region Breakdown";
        }
        #endregion

        #region Constructor
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="dataContextSource"></param>
        public ViewRegionBreakdown(ViewModelRegionBreakdown dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextRegionBreakdown = dataContextSource;
        }
        #endregion

        #region Method to Flip
        /// <summary>
        /// Flipping between Grid & PieChart
        /// Using the method FlipItem in class Flipper.cs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFlip_Click(object sender, RoutedEventArgs e)
        {
            if (this.crtRegionBreakdown.Visibility == System.Windows.Visibility.Visible)
            {
                Flipper.FlipItem(this.crtRegionBreakdown, this.dgRegionBreakdown);
            }
            else
            {
                Flipper.FlipItem(this.dgRegionBreakdown, this.crtRegionBreakdown);
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
                if (this.crtRegionBreakdown.Visibility == Visibility.Visible)
                {
                    List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>
                {                   
                    new RadExportOptions()
                    {
                        ElementName = ExportTypes.HOLDINGS_REGION_BREAKDOWN_CHART,
                        Element = this.crtRegionBreakdown, 
                        ExportFilterOption = RadExportFilterOption.RADCHART_EXPORT_FILTER 
                    },                    
                    
                };
                    ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo,
                    "Export Options: " + GadgetNames.HOLDINGS_REGION_BREAKDOWN);
                    childExportOptions.Show();
                }

                else
                {
                    if (this.dgRegionBreakdown.Visibility == Visibility.Visible)
                    {
                    //    ChildExportOptions childExportOptions = new ChildExportOptions
                    //(new List<RadExportOptions>{new RadExportOptions() 
                    //{
                    //    Element = this.dgRegionBreakdown,
                    //    ElementName = "Region Breakdown Data",
                    //    ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXPORT_FILTER
                    //}}, "Export Options: " + GadgetNames.HOLDINGS_REGION_BREAKDOWN);
                    //    childExportOptions.Show();

                        ExportExcel.ExportGridExcel(dgRegionBreakdown);

                    }
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog(ex.Message);
            }
        }

        private void dgRegionBreakdown_ElementExporting(object sender, GridViewElementExportingEventArgs e)
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
            this.DataContextRegionBreakdown.Dispose();
            this.DataContextRegionBreakdown = null;
            this.DataContext = null;
        }
        #endregion

        private void dgRegionBreakdown_RowLoaded(object sender, RowLoadedEventArgs e)
        {
            GroupedGridRowLoadedHandler.Implement(e);
        }
    }
}
