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
//using Ashmore.Emm.GreenField.ICP.Meeting.Module.ViewModels;
using GreenField.Gadgets.ViewModels;
using System.ComponentModel.Composition;
using GreenField.Gadgets.Helpers;
using GreenField.Common;
using GreenField.ServiceCaller;
using Telerik.Windows.Controls;
using Telerik.Windows.Documents.Model;
using GreenField.ServiceCaller.MeetingDefinitions;

namespace GreenField.Gadgets.Views
{
    public partial class ViewSummaryReport : ViewBaseUserControl
    {

        #region Properties
        /// <summary>
        /// property to set data context
        /// </summary>
        private ViewModelSummaryReport _dataContextViewModelSummaryReport;
        public ViewModelSummaryReport DataContextViewModelSummaryReport
        {
            get { return _dataContextViewModelSummaryReport; }
            set { _dataContextViewModelSummaryReport = value; }
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
                if (DataContextViewModelSummaryReport != null) //DataContext instance
                    DataContextViewModelSummaryReport.IsActive = _isActive;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataContextSource"></param>
        public ViewSummaryReport(ViewModelSummaryReport dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextViewModelSummaryReport = dataContextSource;
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

        

        private void dtpStartDate_SelectionChanged(object sender, Telerik.Windows.Controls.SelectionChangedEventArgs e)
        {
            dtpEndDate.SelectableDateStart = dtpStartDate.SelectedDate;
        }

        private void dtpEndDate_SelectionChanged(object sender, Telerik.Windows.Controls.SelectionChangedEventArgs e)
        {
            dtpStartDate.SelectableDateEnd = dtpEndDate.SelectedDate;
        }

        private void btnExcelExport_Click(object sender, RoutedEventArgs e)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(this.DataContextViewModelSummaryReport._logger, methodNamespace);
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
                Logging.LogException(this.DataContextViewModelSummaryReport._logger, ex);
            }
        }

        private void btnPDFExport_Click(object sender, RoutedEventArgs e)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(this.DataContextViewModelSummaryReport._logger, methodNamespace);
            try
            {
                PDFExporter.btnExportPDF_Click(this.dgICPSummaryReport, layoutMode: DocumentLayoutMode.Paged);
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(this.DataContextViewModelSummaryReport._logger, ex);
            }
        }

        private void btnPrinterExport_Click(object sender, RoutedEventArgs e)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(this.DataContextViewModelSummaryReport._logger, methodNamespace);
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
                Logging.LogException(this.DataContextViewModelSummaryReport._logger, ex);
            }
        }

        private void dgICPSummaryReport_ElementExporting(object sender, GridViewElementExportingEventArgs e)
        {
            RadGridView_ElementExport.ElementExporting(e);
        }


    }
}
