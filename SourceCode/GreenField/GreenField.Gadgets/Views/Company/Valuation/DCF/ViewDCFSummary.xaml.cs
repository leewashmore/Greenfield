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
using Telerik.Windows.Controls;
using Telerik.Windows.Documents.Model;
using GreenField.Common;
using GreenField.Gadgets.Helpers;
using GreenField.Gadgets.Models;
using GreenField.Gadgets.ViewModels;
using GreenField.ServiceCaller;

namespace GreenField.Gadgets.Views
{
    /// <summary>
    /// Code-Behind for DCFSummary
    /// </summary>
    public partial class ViewDCFSummary : ViewBaseUserControl
    {
        #region PropertyDeclaration

        /// <summary>
        /// Instance of View-Model
        /// </summary>
        private ViewModelDCF dataContextSource;
        public ViewModelDCF DataContextSource
        {
            get { return dataContextSource; }
            set { dataContextSource = value; }
        }

        /// <summary>
        /// FV Minority Investment
        /// </summary>
        private string minorityInvestment = "";
        public string MinorityInvestment
        {
            get { return minorityInvestment; }
            set { minorityInvestment = value; }
        }

        /// <summary>
        /// To check whether the Dashboard is Active or not
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
                    DataContextSource.IsActive = value;
                }
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataContextSource"></param>
        public ViewDCFSummary(ViewModelDCF dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextSource = dataContextSource;
        }

        #endregion

        #region ExportToExcel/PDF/Print

        #region ExcelExport
        /// <summary>
        /// Static class storing string types
        /// </summary>
        private static class ExportTypes
        {
            public const string DCF_Summary = "DCF Summary";
        }

        /// <summary>
        /// Method to catch Click Event of Export to Excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(this.DataContextSource.Logger, methodNamespace);
            try
            {
                if (this.dgDCFSummary.Visibility == Visibility.Visible)
                {
                    List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>
                        {
                                new RadExportOptions() { ElementName = ExportTypes.DCF_Summary, Element = this.dgDCFSummary, 
                                    ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXCEL_EXPORT_FILTER }
                        };
                    ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + ExportTypes.DCF_Summary);
                    childExportOptions.Show();
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(this.DataContextSource.Logger, ex);
            }
        }

        #endregion

        #region HelperMethods
        /// <summary>
        /// Element Exporting Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgDCFSummary_ElementExporting(object sender, Telerik.Windows.Controls.GridViewElementExportingEventArgs e)
        {
            RadGridView_ElementExport.ElementExporting(e);
        }

        #endregion

        #region PDFExport
        /// <summary>
        /// Event handler when user wants to Export the Grid to PDF
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportPDF_Click(object sender, RoutedEventArgs e)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(this.DataContextSource.Logger, methodNamespace);
            try
            {
                PDFExporter.btnExportPDF_Click(this.dgDCFSummary, 12);
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(this.DataContextSource.Logger, ex);
            }
        }
        #endregion

        #region Printing the DataGrid

        /// <summary>
        /// Printing the DataGrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(this.DataContextSource.Logger, methodNamespace);
            try
            {
                Dispatcher.BeginInvoke((Action)(() =>
                {
                    RichTextBox.Document = PDFExporter.Print(dgDCFSummary, 12);
                }));

                this.RichTextBox.Document.SectionDefaultPageOrientation = PageOrientation.Landscape;
                RichTextBox.Print("MyDocument", Telerik.Windows.Documents.UI.PrintMode.Native);
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(this.DataContextSource.Logger, ex);
            }
        }

        #endregion

        /// <summary>
        /// create RadDocument from the DataGrid
        /// </summary>
        /// <returns>Returns the RadDcoument for the Grid</returns>
        public override DCFPDFExport CreateDocument()
        {
            try
            {
                DCFPDFExport data = new DCFPDFExport();
                GridViewLength columnWidth = this.dgDCFSummary.Columns[0].ActualWidth;
                data.DataTable = PDFExporter.CreateTable(dgDCFSummary, 12, columnWidth, string.Empty);
                return data;
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(this.DataContextSource.Logger, ex);
                return null;
            }
        }


        #endregion

        #region Helper Methods

        /// <summary>
        /// Row Loaded Event of Data-Grid
        /// </summary>
        /// <param name="sender">Sender of the Event</param>
        /// <param name="e">Event Arguement</param>
        private void dgDCFSummary_RowLoaded(object sender, Telerik.Windows.Controls.GridView.RowLoadedEventArgs e)
        {
            if (e.Row != null)
            {
                if (e.Row.DataContext != null)
                {
                    var data = e.Row.DataContext as DCFDisplayData;
                    if (data == null)
                        return;
                    if ((e.Row.DataContext as DCFDisplayData).PropertyName == "(-) FV of Minorities")
                    {
                        e.Row.Background = new SolidColorBrush(Colors.Yellow);
                    }
                }
            }
        }

        /// <summary>
        /// DataGrid -Row Edit beginning Event
        /// </summary>
        /// <param name="sender">Sender of the event</param>
        /// <param name="e">Event Arguement</param>
        private void dgDCFSummary_BeginningEdit(object sender, GridViewBeginningEditRoutedEventArgs e)
        {
            int Index = this.dgDCFSummary.Items.IndexOf(e.Cell.ParentRow.Item);
            if (Index != 6)
            {
                e.Cancel = true;
            }
            else
            {
                e.Cell.Value = "";
            }
        }

        /// <summary>
        /// Row-Edit ended event of DataGrid
        /// </summary>
        /// <param name="sender">Sender of the Event</param>
        /// <param name="e">Event Arguement</param>
        private void dgDCFSummary_RowEditEnded(object sender, GridViewRowEditEndedEventArgs e)
        {
            if (MinorityInvestment != null)
            {
                if (MinorityInvestment != "")
                {
                    this.DataContextSource.MinorityInvestments = Convert.ToDecimal(MinorityInvestment);
                }
            }
        }

        /// <summary>
        /// Cell Validated Event
        /// </summary>
        /// <param name="sender">Sender of the Event</param>
        /// <param name="e">Event Arguement</param>
        private void dgDCFSummary_CellValidating(object sender, GridViewCellValidatingEventArgs e)
        {
            if (e.Cell.Column.UniqueName == "Values")
            {
                decimal value;
                var textEntered = e.NewValue as string;

                if (!Decimal.TryParse(textEntered, out value))
                {
                    e.IsValid = false;
                    e.ErrorMessage = "The Entered value should be a valid number";
                }
                else
                {
                    this.MinorityInvestment = textEntered;
                }
            }
        }

        #endregion

        #region Unsubscribe Events

        /// <summary>
        /// Unsubscribe Events
        /// </summary>
        public override void Dispose()
        {
            this.DataContextSource.Dispose();
        }

        #endregion
    }
}
