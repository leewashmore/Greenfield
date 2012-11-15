using System.Collections.Generic;
using System.Windows;
using Telerik.Windows.Controls;
using GreenField.Common;
using GreenField.Gadgets.Helpers;
using GreenField.Gadgets.ViewModels;
using System;
using GreenField.ServiceCaller;

namespace GreenField.Gadgets.Views
{
    public partial class ViewTopHoldings : ViewBaseUserControl
    {
        #region Properties
        /// <summary>
        /// property to set data context
        /// </summary>
        private ViewModelTopHoldings dataContextViewModelTopHoldings;
        public ViewModelTopHoldings DataContextViewModelTopHoldings
        {
            get { return dataContextViewModelTopHoldings; }
            set { dataContextViewModelTopHoldings = value; }
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
                if (DataContextViewModelTopHoldings != null)
                { DataContextViewModelTopHoldings.IsActive = isActive; }
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="dataContextSource"></param>
        public ViewTopHoldings(ViewModelTopHoldings dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextViewModelTopHoldings = dataContextSource;
        }
        #endregion

        #region Export To Excel Methods
        /// <summary>
        /// method to export the grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            ChildExportOptions childExportOptions = new ChildExportOptions
                (
                new List<RadExportOptions>
                {
                    new RadExportOptions() 
                    {
                        Element = this.dgTopHoldings,
                        ElementName = "Top 10 Holdings Data",
                        ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXCEL_EXPORT_FILTER
                    } 
                }, "Export Options: " + GadgetNames.HOLDINGS_TOP_TEN_HOLDINGS);
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
                    ElementName = GadgetNames.HOLDINGS_TOP_TEN_HOLDINGS,
                    Element = this.dgTopHoldings,
                    ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_PRINT_FILTER,
                    RichTextBox = this.RichTextBox
                });

                ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + GadgetNames.HOLDINGS_TOP_TEN_HOLDINGS);
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
                    ElementName = GadgetNames.HOLDINGS_TOP_TEN_HOLDINGS,
                    Element = this.dgTopHoldings,
                    ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_PDF_EXPORT_FILTER,
                    RichTextBox = this.RichTextBox
                });

                ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + GadgetNames.HOLDINGS_TOP_TEN_HOLDINGS);
                childExportOptions.Show();
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
            }
        }

        /// <summary>
        /// handling element exporting
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgTopHoldings_ElementExporting(object sender, GridViewElementExportingEventArgs e)
        {
            RadGridView_ElementExport.ElementExporting(e, isGroupFootersVisible: false);
        }
        #endregion

        #region Dispose Method
        /// <summary>
        /// method to dispose all running events
        /// </summary>
        public override void Dispose()
        {
            this.DataContextViewModelTopHoldings.Dispose();
            this.DataContextViewModelTopHoldings = null;
            this.DataContext = null;
        }
        #endregion
    }
}
