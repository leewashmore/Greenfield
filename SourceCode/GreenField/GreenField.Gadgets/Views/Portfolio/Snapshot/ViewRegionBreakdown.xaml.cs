using System;
using System.Collections.Generic;
using System.Windows;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;
using GreenField.Common;
using GreenField.Gadgets.Helpers;
using GreenField.Gadgets.ViewModels;
using GreenField.ServiceCaller;

namespace GreenField.Gadgets.Views
{
    public partial class ViewRegionBreakdown : ViewBaseUserControl
    {
        #region Property
        /// <summary>
        /// property to set data context
        /// </summary>
        private ViewModelRegionBreakdown dataContextRegionBreakdown;
        public ViewModelRegionBreakdown DataContextRegionBreakdown
        {
            get { return dataContextRegionBreakdown; }
            set { dataContextRegionBreakdown = value; }
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
                if (DataContextRegionBreakdown != null)
                { DataContextRegionBreakdown.IsActive = isActive; }
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="dataContextSource">ViewModelRegionBreakdown</param>
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
                    List<RadExportOptions> radExportOptionsInfo = new List<RadExportOptions>
                {                   
                    new RadExportOptions()
                    {
                        ElementName = "Region Breakdown Data",
                        Element = this.crtRegionBreakdown, 
                        ExportFilterOption = RadExportFilterOption.RADCHART_EXCEL_EXPORT_FILTER 
                    },                 
                };
                    ChildExportOptions childExportOptions = new ChildExportOptions(radExportOptionsInfo, "Export Options: " + GadgetNames.HOLDINGS_REGION_BREAKDOWN);
                    childExportOptions.Show();
                }
                else
                {
                    if (this.dgRegionBreakdown.Visibility == Visibility.Visible)
                    {
                        ChildExportOptions childExportOptions = new ChildExportOptions(new List<RadExportOptions>
                            {
                                new RadExportOptions() 
                                {
                                    Element = this.dgRegionBreakdown,
                                    ElementName = "Region Breakdown Data",
                                    ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXCEL_EXPORT_FILTER
                                }
                            }, "Export Options: " + GadgetNames.HOLDINGS_REGION_BREAKDOWN);
                        childExportOptions.Show();
                    }
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog(ex.Message);
            }
        }

        /// <summary>
        /// Printing the DataGrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            //Logging.LogBeginMethod(this.DataContextHoldingsPieChart.logger, methodNamespace);
            try
            {
                if (this.crtRegionBreakdown.Visibility == Visibility.Visible)
                {
                    List<RadExportOptions> radExportOptionsInfo = new List<RadExportOptions>
                {                   
                    new RadExportOptions()
                    {
                        ElementName = "Region Breakdown Data",
                        Element = this.crtRegionBreakdown, 
                        ExportFilterOption = RadExportFilterOption.RADCHART_PRINT_FILTER 
                    },                 
                };
                    ChildExportOptions childExportOptions = new ChildExportOptions(radExportOptionsInfo, "Export Options: " + GadgetNames.HOLDINGS_REGION_BREAKDOWN);
                    childExportOptions.Show();
                }
                else
                {
                    if (this.dgRegionBreakdown.Visibility == Visibility.Visible)
                    {
                        ChildExportOptions childExportOptions = new ChildExportOptions(new List<RadExportOptions>
                            {
                                new RadExportOptions() 
                                {
                                    Element = this.dgRegionBreakdown,
                                    ElementName = "Region Breakdown Data",
                                    ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_PRINT_FILTER
                                }
                            }, "Export Options: " + GadgetNames.HOLDINGS_REGION_BREAKDOWN);
                        childExportOptions.Show();
                    }
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                //Logging.LogException(this.DataContextSlice1ChartExtension.logger, ex);
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
            //Logging.LogBeginMethod(this.DataContextSlice1ChartExtension.logger, methodNamespace);
            try
            {

                if (this.crtRegionBreakdown.Visibility == Visibility.Visible)
                {
                    List<RadExportOptions> radExportOptionsInfo = new List<RadExportOptions>
                {                   
                    new RadExportOptions()
                    {
                        ElementName = "Region Breakdown Data",
                        Element = this.crtRegionBreakdown, 
                        ExportFilterOption = RadExportFilterOption.RADCHART_PDF_EXPORT_FILTER 
                    },                 
                };
                    ChildExportOptions childExportOptions = new ChildExportOptions(radExportOptionsInfo, "Export Options: " + GadgetNames.HOLDINGS_REGION_BREAKDOWN);
                    childExportOptions.Show();
                }
                else
                {
                    if (this.dgRegionBreakdown.Visibility == Visibility.Visible)
                    {
                        ChildExportOptions childExportOptions = new ChildExportOptions(new List<RadExportOptions>
                            {
                                new RadExportOptions() 
                                {
                                    Element = this.dgRegionBreakdown,
                                    ElementName = "Region Breakdown Data",
                                    ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_PDF_EXPORT_FILTER
                                }
                            }, "Export Options: " + GadgetNames.HOLDINGS_REGION_BREAKDOWN);
                        childExportOptions.Show();
                    }
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                //Logging.LogException(this.DataContextSlice1ChartExtension.logger, ex);
            }
        }

        /// <summary>
        /// handles element exporting for export to excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgRegionBreakdown_ElementExporting(object sender, GridViewElementExportingEventArgs e)
        {
            RadGridView_ElementExport.ElementExporting(e, isGroupFootersVisible: true, aggregatedColumnIndex: new List<int> { 1, 2, 3 });
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
    }
}
