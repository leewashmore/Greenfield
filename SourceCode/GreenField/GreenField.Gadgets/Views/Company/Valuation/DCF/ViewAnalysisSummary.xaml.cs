using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using GreenField.Common;
using GreenField.Gadgets.Helpers;
using GreenField.Gadgets.Models;
using GreenField.Gadgets.ViewModels;
using GreenField.ServiceCaller;
using GreenField.UserSession;
using Telerik.Windows.Controls;
using Telerik.Windows.Documents.Model;

namespace GreenField.Gadgets.Views
{
    /// <summary>
    /// View for DCF AnalysisSummary
    /// </summary>
    public partial class ViewAnalysisSummary : ViewBaseUserControl
    {
        #region PrivateVariables

        /// <summary>
        /// Property of Type ViewModelAnalysisSummary- ViewModel
        /// </summary>
        private ViewModelDCF dataContextSource;
        public ViewModelDCF DataContextSource
        {
            get
            {
                return dataContextSource;
            }
            set
            {
                dataContextSource = value;
            }
        }

        private string stockSpecificDiscount = "";

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
        /// <param name="dataContextSource">Instance of View-Model, ViewModelAnalysisSummary</param>
        public ViewAnalysisSummary(ViewModelDCF dataContextSource)
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
            public const string Analysis_Summary = "Analysis Summary";
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
                if (this.dgDCFAnalysisSummary.Visibility == Visibility.Visible)
                {
                    List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>
                        {
                                new RadExportOptions() { ElementName = ExportTypes.Analysis_Summary, Element = this.dgDCFAnalysisSummary, 
                                    ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXCEL_EXPORT_FILTER }
                        };
                    ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + ExportTypes.Analysis_Summary);
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
        private void btnExportPdf_Click(object sender, RoutedEventArgs e)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(this.DataContextSource.Logger, methodNamespace);
            try
            {
                List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>();
                RadExportOptionsInfo.Add(new RadExportOptions()
                {
                    ElementName = ExportTypes.Analysis_Summary,
                    Element = this.dgDCFAnalysisSummary,
                    ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_PDF_EXPORT_FILTER,
                    RichTextBox = this.RichTextBox
                });
                ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + ExportTypes.Analysis_Summary);
                childExportOptions.Show();
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
                List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>();
                RadExportOptionsInfo.Add(new RadExportOptions()
                {
                    ElementName = ExportTypes.Analysis_Summary,
                    Element = this.dgDCFAnalysisSummary,
                    ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_PRINT_FILTER,
                    RichTextBox = this.RichTextBox
                });
                ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + ExportTypes.Analysis_Summary);
                childExportOptions.Show();
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(this.DataContextSource.Logger, ex);
            }
        }

        #endregion

        /// <summary>
        /// Before Editing Begins
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgDCFAnalysisSummary_BeginningEdit(object sender, GridViewBeginningEditRoutedEventArgs e)
        {
            if ((e.Row.DataContext as DCFDisplayData).PropertyName == "Stock Specific Discount")
            {
                e.Cell.Value = "";
            }
            else
            {
                e.Cancel = true;
            }
            //int Index = this.dgDCFAnalysisSummary.Items.IndexOf(e.Cell.ParentRow.Item);
            //if (Index != 3)
            //{
            //    e.Cancel = true;
            //}
            //else
            //{
            //    e.Cell.Value = "";
            //}
        }

        #endregion

        #region GridEditingMethods

        /// <summary>
        /// Validating the Contents of the Edited Cell.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgDCFAnalysisSummary_CellValidating(object sender, GridViewCellValidatingEventArgs e)
        {
            if (e.Cell.Column.UniqueName == "High")
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
                    this.stockSpecificDiscount = textEntered;
                }
            }
        }

        /// <summary>
        /// Event that occurs after the Editing is completed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgDCFAnalysisSummary_RowEditEnded(object sender, GridViewRowEditEndedEventArgs e)
        {
            if (stockSpecificDiscount != null)
            {
                if (stockSpecificDiscount != "")
                {
                    this.DataContextSource.StockSpecificDiscount = Convert.ToDecimal(stockSpecificDiscount);
                }
            }
        }

        /// <summary>
        /// Row Loaded Event, changes the color for StockSpecificDiscount
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgDCFAnalysisSummary_RowLoaded(object sender, Telerik.Windows.Controls.GridView.RowLoadedEventArgs e)
        {
            if (e.Row != null)
            {
                if (e.Row.DataContext != null)
                {
                    var data = e.Row.DataContext as DCFDisplayData;
                    if (data == null)
                    {
                        return;
                    }
                    if ((e.Row.DataContext as DCFDisplayData).PropertyName == "Stock Specific Discount")
                    {
                        e.Row.Background = new SolidColorBrush(Colors.Yellow);
                    }
                    else
                    {                        
                        e.Row.Background = new SolidColorBrush(Colors.White);
                    }
                }
            }
        }

        #endregion

        #region PDF Export

        /// <summary>
        /// create RadDocument from the DataGrid
        /// </summary>
        /// <returns>Returns the RadDcoument for the Grid</returns>
        public override DCFPDFExport CreateDocument()
        {
            try
            {
                DCFPDFExport data = new DCFPDFExport();
                GridViewLength columnWidth = this.dgDCFAnalysisSummary.Columns[0].ActualWidth;
                data.DataTable = PDFExporter.CreateTable(dgDCFAnalysisSummary, 12, columnWidth, string.Empty);
                data.CountryName = this.DataContextSource.CountryName;
                data.SecurityName = this.DataContextSource.EntitySelectionData.ShortName;
                data.CreatedBy = SessionManager.SESSION.UserName;
                foreach (GridViewBoundColumnBase item in dgDCFAnalysisSummary.Columns)
                {
                    item.Width = GridViewLength.Auto;
                }
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
