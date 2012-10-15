using System;
using System.Collections.Generic;
using System.Windows;
using Telerik.Windows.Controls;
using Telerik.Windows.Documents.Model;
using GreenField.Common;
using GreenField.Gadgets.Helpers;
using GreenField.Gadgets.ViewModels;
using GreenField.ServiceCaller;

namespace GreenField.Gadgets.Views
{
    /// <summary>
    /// Code behind for ViewSummaryReport
    /// </summary>
    public partial class ViewSummaryReport : ViewBaseUserControl
    {
        #region Properties
        /// <summary>
        /// property to set data context
        /// </summary>
        private ViewModelSummaryReport dataContextViewModelSummaryReport;
        public ViewModelSummaryReport DataContextViewModelSummaryReport
        {
            get { return dataContextViewModelSummaryReport; }
            set { dataContextViewModelSummaryReport = value; }
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
                if (DataContextViewModelSummaryReport != null)
                {
                    DataContextViewModelSummaryReport.IsActive = isActive;
                }
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataContextSource">ViewModelSummaryReport</param>
        public ViewSummaryReport(ViewModelSummaryReport dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextViewModelSummaryReport = dataContextSource;
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// dtpStartDate SelectionChanged EventHandler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">SelectionChangedEventArgs</param>
        private void dtpStartDate_SelectionChanged(object sender, Telerik.Windows.Controls.SelectionChangedEventArgs e)
        {
            dtpEndDate.SelectableDateStart = dtpStartDate.SelectedDate;
        }

        /// <summary>
        /// dtpEndDate SelectionChanged EventHandler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">SelectionChangedEventArgs</param>
        private void dtpEndDate_SelectionChanged(object sender, Telerik.Windows.Controls.SelectionChangedEventArgs e)
        {
            dtpStartDate.SelectableDateEnd = dtpEndDate.SelectedDate;
        }

        /// <summary>
        /// btnExcelExport Click EventHandler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">RoutedEventArgs</param>
        private void btnExcelExport_Click(object sender, RoutedEventArgs e)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(this.DataContextViewModelSummaryReport.logger, methodNamespace);
            try
            {
                List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>
                {
                    new RadExportOptions() { ElementName = "Summary Report", Element = this.dgICPSummaryReport
                        , ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXPORT_FILTER }
                };
                ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: Summary Report");
                childExportOptions.Show();
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(this.DataContextViewModelSummaryReport.logger, ex);
            }
        }

        /// <summary>
        /// btnPDFExport Click EventHandler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">RoutedEventArgs</param>
        private void btnPDFExport_Click(object sender, RoutedEventArgs e)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(this.DataContextViewModelSummaryReport.logger, methodNamespace);
            try
            {
                PDFExporter.btnExportPDF_Click(this.dgICPSummaryReport, orientation: PageOrientation.Landscape);
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(this.DataContextViewModelSummaryReport.logger, ex);
            }
        }

        /// <summary>
        /// btnPrinterExport Click EventHandler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">RoutedEventArgs</param>
        private void btnPrinterExport_Click(object sender, RoutedEventArgs e)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(this.DataContextViewModelSummaryReport.logger, methodNamespace);
            try
            {
                Dispatcher.BeginInvoke((Action)(() =>
                {
                    this.richTextBox.Document = PDFExporter.Print(dgICPSummaryReport, 6);
                }));

                richTextBox.Document.SectionDefaultPageOrientation = PageOrientation.Landscape;
                richTextBox.Print("Investment Committee Summary Report", Telerik.Windows.Documents.UI.PrintMode.Native);
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(this.DataContextViewModelSummaryReport.logger, ex);
            }
        }

        /// <summary>
        /// dgICPSummaryReport ElementExporting EventHandler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">GridViewElementExportingEventArgs</param>
        private void dgICPSummaryReport_ElementExporting(object sender, GridViewElementExportingEventArgs e)
        {
            RadGridView_ElementExport.ElementExporting(e);
        } 
        #endregion

        #region Dispose Method
        /// <summary>
        /// method to dispose all running events
        /// </summary>
        public override void Dispose()
        {
            this.DataContextViewModelSummaryReport.Dispose();
            this.DataContextViewModelSummaryReport = null;
            this.DataContext = null;
        }
        #endregion
    }
}
