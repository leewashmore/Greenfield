using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GreenField.Common;
using GreenField.Gadgets.Helpers;
using GreenField.Gadgets.ViewModels;
using GreenField.ServiceCaller;
using Telerik.Windows.Documents.Model;

namespace GreenField.Gadgets.Views
{
    /// <summary>
    /// Code-behind for Sensitivity BVPS
    /// </summary>
    public partial class ViewSensitivityBVPS : ViewBaseUserControl
    {
        /// <summary>
        /// Instance of View-Model: ViewModelDCF
        /// </summary>
        private ViewModelDCF dataContextSource;
        public ViewModelDCF DataContextSource
        {
            get { return dataContextSource; }
            set { dataContextSource = value; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataContextSource">ViewModel</param>
        public ViewSensitivityBVPS(ViewModelDCF dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextSource = dataContextSource;
        }

        private void txtFWDBVPS_TextInput(object sender, TextCompositionEventArgs e)
        {
            string textEntered = e.Text as string;
            decimal value;
            if (!Decimal.TryParse(textEntered, out value))
            {
                txtFWDBVPS.Text = "";
            }
        }

        #region ExportToExcel/PDF/Print

        #region ExcelExport
        /// <summary>
        /// Static class storing string types
        /// </summary>
        private static class ExportTypes
        {
            public const string Sensitivity_BVPS = "Sensitivity BVPS";
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
                if (this.dgDCFSensitivity.Visibility == Visibility.Visible)
                {
                    List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>
                        {
                                new RadExportOptions() { ElementName = ExportTypes.Sensitivity_BVPS, Element = this.dgDCFSensitivity,
                                    ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXCEL_EXPORT_FILTER }
                        };
                    ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + ExportTypes.Sensitivity_BVPS);
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
        private void dgDCFAnalysisSummary_ElementExporting(object sender, Telerik.Windows.Controls.GridViewElementExportingEventArgs e)
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
                PDFExporter.btnExportPDF_Click(this.dgDCFSensitivity,12);
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
                    RichTextBox.Document = PDFExporter.Print(dgDCFSensitivity, 12);
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

        /// <summary>
        /// create RadDocument from the DataGrid
        /// </summary>
        /// <returns>Returns the RadDcoument for the Grid</returns>
        public override DCFPDFExport CreateDocument()
        {
            try
            {
                Table sensitivityTable = new Table();
                sensitivityTable = PDFExporter.GenerateTable(gridSensitivityBVPS, "Sensitivity BVPS");
                DCFPDFExport data = new DCFPDFExport();
                data.DataTable = sensitivityTable;
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

        /// <summary>
        /// Reg-Ex to check Entered Value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBlock_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                return; //added to handle tab key 
            }
            var thisKeyStr = "";
            if (e.PlatformKeyCode == 190 || e.PlatformKeyCode == 110)
            {
                thisKeyStr = ".";
            }
            else
            {
                thisKeyStr = e.Key.ToString().Replace("D", "").Replace("NumPad", "");
            }
            var s = (sender as TextBox).Text + thisKeyStr;
            var rStr = "^[0-9]+[.]?([0-9]{1,10})*$";
            var r = new Regex(rStr, RegexOptions.IgnoreCase);
            e.Handled = !r.IsMatch(s);
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
