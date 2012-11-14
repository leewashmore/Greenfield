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
    /// Code-behind for SensitivityEPS
    /// </summary>
    public partial class ViewSensitivityEPS : ViewBaseUserControl
    {
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
        /// Constructor
        /// </summary>
        /// <param name="dataContextSource"></param>
        public ViewSensitivityEPS(ViewModelDCF dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextSource = dataContextSource;
        }

        /// <summary>
        /// Text-Input for FED_EPS
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtFWDEPS_TextInput(object sender, TextCompositionEventArgs e)
        {
            string textEntered = e.Text as string;
            decimal value;
            if (!Decimal.TryParse(textEntered, out value))
            {
                txtFWDEPS.Text = "";
                Prompt.ShowDialog("FWD EPS can only be Numeric");
            }
        }
        
        #region ExportToExcel/PDF/Print

        #region ExcelExport
        /// <summary>
        /// Static class storing string types
        /// </summary>
        private static class ExportTypes
        {
            public const string Sensitivity_EPS = "Sensitivity EPS";
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
                                new RadExportOptions() { ElementName = ExportTypes.Sensitivity_EPS, Element = this.dgDCFSensitivity, 
                                    ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXCEL_EXPORT_FILTER }
                        };
                    ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + ExportTypes.Sensitivity_EPS);
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
                PDFExporter.btnExportPDF_Click(this.dgDCFSensitivity, 12);
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
        /// Create RadDocument from the DataGrid
        /// </summary>
        /// <returns>Returns the RadDcoument for the Grid</returns>
        public override DCFPDFExport CreateDocument()
        {
            try
            {
                Table sensitivityTable = new Table();
                DCFPDFExport data = new DCFPDFExport();
                data.DataTable = PDFExporter.GenerateTable(gridSensitivityEPS, "Sensitivity EPS");
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
        /// Key-Down Event of TextBox for EPS
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtFWDEPS_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab)
            {
                return; //Added to handle TAB key after below post
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
            //var rStr = "^[0-9]*[0-9](|.[0-9]*[0-9]|,([0-9]*[0-9]))?$";
            var rStr = "^[0-9]+[.]?([0-9]{1,10})*$";
            var r = new Regex(rStr, RegexOptions.IgnoreCase);
            e.Handled = !r.IsMatch(s);
        }

        /// <summary>
        /// Generate PDF-Export
        /// </summary>
        /// <returns></returns>
        public override List<string> EPS_BVPS()
        {
            List<string> result = new List<string>();

            try
            {
                if (this.DataContextSource.FWDEPS != null)
                {
                    result.Add(this.DataContextSource.FWDEPS.ToString());
                }
                else
                {
                    result.Add(null);
                }

                if (this.DataContextSource.FWDBVPS != null)
                {
                    result.Add(this.DataContextSource.FWDBVPS.ToString());
                }
                else
                {
                    result.Add(null);
                }
                return result;
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(this.DataContextSource.Logger, ex);
                return null;
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
