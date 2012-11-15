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
    public partial class ViewSectorBreakdown : ViewBaseUserControl
    {
        #region Properties
        /// <summary>
        /// property to set data context
        /// </summary>
        private ViewModelSectorBreakdown dataContextSectorBreakdown;
        public ViewModelSectorBreakdown DataContextSectorBreakdown
        {
            get { return dataContextSectorBreakdown; }
            set { dataContextSectorBreakdown = value; }
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
                if (DataContextSectorBreakdown != null)
                { DataContextSectorBreakdown.IsActive = isActive; }
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataContextSource"></param>
        public ViewSectorBreakdown(ViewModelSectorBreakdown dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextSectorBreakdown = dataContextSource;
        } 
        #endregion

        #region Flip Method
        /// <summary>
        /// Flipping between Grid & PieChart
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
                    List<RadExportOptions> radExportOptionsInfo = new List<RadExportOptions>{ new RadExportOptions()
                    {
                        ElementName = "Sector Breakdown Chart",
                        Element = this.crtSectorBreakdown, 
                        ExportFilterOption = RadExportFilterOption.RADCHART_EXCEL_EXPORT_FILTER 
                    },              
                };
                    ChildExportOptions childExportOptions = new ChildExportOptions(radExportOptionsInfo, "Export Options: " + GadgetNames.HOLDINGS_SECTOR_BREAKDOWN);
                    childExportOptions.Show();
                }
                else
                {
                    if (this.dgSectorBreakdown.Visibility == Visibility.Visible)
                    {
                        ChildExportOptions childExportOptions = new ChildExportOptions(new List<RadExportOptions>
                        {
                            new RadExportOptions()
                            {
                                Element = this.dgSectorBreakdown,
                                ElementName = "Sector Breakdown Data",
                                ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXCEL_EXPORT_FILTER
                            }
                        }, "Export Options: " + GadgetNames.HOLDINGS_SECTOR_BREAKDOWN);
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
            try
            {
                if (this.crtSectorBreakdown.Visibility == Visibility.Visible)
                {
                    List<RadExportOptions> radExportOptionsInfo = new List<RadExportOptions>{ new RadExportOptions()
                    {
                        ElementName = "Sector Breakdown Chart",
                        Element = this.crtSectorBreakdown, 
                        ExportFilterOption = RadExportFilterOption.RADCHART_PRINT_FILTER,
                        RichTextBox = this.RichTextBox
                    },              
                };
                    ChildExportOptions childExportOptions = new ChildExportOptions(radExportOptionsInfo, "Export Options: " + GadgetNames.HOLDINGS_SECTOR_BREAKDOWN);
                    childExportOptions.Show();
                }
                else
                {
                    if (this.dgSectorBreakdown.Visibility == Visibility.Visible)
                    {
                        ChildExportOptions childExportOptions = new ChildExportOptions(new List<RadExportOptions>
                        {
                            new RadExportOptions()
                            {
                                Element = this.dgSectorBreakdown,
                                ElementName = "Sector Breakdown Data",
                                ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_PRINT_FILTER,
                                RichTextBox = this.RichTextBox
                            }
                        }, "Export Options: " + GadgetNames.HOLDINGS_SECTOR_BREAKDOWN);
                        childExportOptions.Show();
                    }
                }
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

                if (this.crtSectorBreakdown.Visibility == Visibility.Visible)
                {
                    List<RadExportOptions> radExportOptionsInfo = new List<RadExportOptions>{ new RadExportOptions()
                    {
                        ElementName = "Sector Breakdown Chart",
                        Element = this.crtSectorBreakdown, 
                        ExportFilterOption = RadExportFilterOption.RADCHART_PDF_EXPORT_FILTER 
                    },              
                };
                    ChildExportOptions childExportOptions = new ChildExportOptions(radExportOptionsInfo, "Export Options: " + GadgetNames.HOLDINGS_SECTOR_BREAKDOWN);
                    childExportOptions.Show();
                }
                else
                {
                    if (this.dgSectorBreakdown.Visibility == Visibility.Visible)
                    {
                        ChildExportOptions childExportOptions = new ChildExportOptions(new List<RadExportOptions>
                        {
                            new RadExportOptions()
                            {
                                Element = this.dgSectorBreakdown,
                                ElementName = "Sector Breakdown Data",
                                ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_PDF_EXPORT_FILTER
                            }
                        }, "Export Options: " + GadgetNames.HOLDINGS_SECTOR_BREAKDOWN);
                        childExportOptions.Show();
                    }
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
            }
        }

        private void dgSectorBreakdown_ElementExporting(object sender, GridViewElementExportingEventArgs e)
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
            this.DataContextSectorBreakdown.Dispose();
            this.DataContextSectorBreakdown = null;
            this.DataContext = null;
        } 
        #endregion
    }
}
