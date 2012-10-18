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
using GreenField.DataContracts;
using GreenField.Gadgets.Helpers;
using GreenField.Gadgets.ViewModels;
using GreenField.ServiceCaller;

namespace GreenField.Gadgets.Views
{
    public partial class ViewFreeCashFlows : ViewBaseUserControl
    {

        #region PropertyDeclaration

        /// <summary>
        /// Property of ViewModel type
        /// </summary>
        private ViewModelFreeCashFlows dataContextFreeCashFlows;
        public ViewModelFreeCashFlows DataContextFreeCashFlows
        {
            get
            {
                return dataContextFreeCashFlows;
            }
            set
            {
                dataContextFreeCashFlows = value;
            }
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
                if (DataContextFreeCashFlows != null) //DataContext instance
                    DataContextFreeCashFlows.IsActive = isActive;
            }
        }
        #endregion

        #region CONSTRUCTOR
        /// <summary>
        /// CONSTRUCTOR
        /// </summary>
        /// <param name="DataContextFreeCashFlows"></param>
        public ViewFreeCashFlows(ViewModelFreeCashFlows DataContextFreeCashFlows)
        {
            InitializeComponent();
            this.DataContext = DataContextFreeCashFlows;
            this.DataContextFreeCashFlows = DataContextFreeCashFlows;
            this.DataContextFreeCashFlows.RetrieveFreeCashFlowsDataCompleteEvent += new RetrieveFreeCashFlowsDataCompletedEventHandler(RetrieveFreeCashFlowsDataCompletedEvent);
        }
        #endregion

        # region EVENT
        private List<FreeCashFlowsData> FreeCashFlowsData;
        public void RetrieveFreeCashFlowsDataCompletedEvent(RetrieveFreeCashFlowsDataCompleteEventArs e)
        {
            FreeCashFlowsData = e.FreeCashFlowsInfo;
            if (FreeCashFlowsData != null)
            {
                dgFreeCashFlows.Columns[1].Header = System.DateTime.Now.Year + "\n" + "Y1";
                dgFreeCashFlows.Columns[2].Header = System.DateTime.Now.Year + 1 + "\n" + "Y2";
                dgFreeCashFlows.Columns[3].Header = System.DateTime.Now.Year + 2 + "\n" + "Y3";
                dgFreeCashFlows.Columns[4].Header = System.DateTime.Now.Year + 3 + "\n" + "Y4";
                dgFreeCashFlows.Columns[5].Header = System.DateTime.Now.Year + 4 + "\n" + "Y5";
                dgFreeCashFlows.Columns[6].Header = System.DateTime.Now.Year + 5 + "\n" + "Y6";
                dgFreeCashFlows.Columns[7].Header = System.DateTime.Now.Year + 6 + "\n" + "Y7";
                dgFreeCashFlows.Columns[8].Header = System.DateTime.Now.Year + 7 + "\n" + "Y8";
                dgFreeCashFlows.Columns[9].Header = System.DateTime.Now.Year + 8 + "\n" + "Y9";
                dgFreeCashFlows.Columns[10].Header = System.DateTime.Now.Year + 9 + "\n" + "Y10";

                dgFreeCashFlows.Columns[1].UniqueName = System.DateTime.Now.Year + "\n" + "Y1";
                dgFreeCashFlows.Columns[2].UniqueName = System.DateTime.Now.Year + 1 + "\n" + "Y2";
                dgFreeCashFlows.Columns[3].UniqueName = System.DateTime.Now.Year + 2 + "\n" + "Y3";
                dgFreeCashFlows.Columns[4].UniqueName = System.DateTime.Now.Year + 3 + "\n" + "Y4";
                dgFreeCashFlows.Columns[5].UniqueName = System.DateTime.Now.Year + 4 + "\n" + "Y5";
                dgFreeCashFlows.Columns[6].UniqueName = System.DateTime.Now.Year + 5 + "\n" + "Y6";
                dgFreeCashFlows.Columns[7].UniqueName = System.DateTime.Now.Year + 6 + "\n" + "Y7";
                dgFreeCashFlows.Columns[8].UniqueName = System.DateTime.Now.Year + 7 + "\n" + "Y8";
                dgFreeCashFlows.Columns[9].UniqueName = System.DateTime.Now.Year + 8 + "\n" + "Y9";
                dgFreeCashFlows.Columns[10].UniqueName = System.DateTime.Now.Year + 9 + "\n" + "Y10";

                dgFreeCashFlows.Columns[1].HeaderCellStyle = Resources["GridViewHeaderCellStyle"] as Style;
                dgFreeCashFlows.Columns[2].HeaderCellStyle = Resources["GridViewHeaderCellStyle"] as Style;
                dgFreeCashFlows.Columns[3].HeaderCellStyle = Resources["GridViewHeaderCellStyle"] as Style;
                dgFreeCashFlows.Columns[4].HeaderCellStyle = Resources["GridViewHeaderCellStyle"] as Style;
                dgFreeCashFlows.Columns[5].HeaderCellStyle = Resources["GridViewHeaderCellStyle"] as Style;
                dgFreeCashFlows.Columns[6].HeaderCellStyle = Resources["GridViewHeaderCellStyle"] as Style;
                dgFreeCashFlows.Columns[7].HeaderCellStyle = Resources["GridViewHeaderCellStyle"] as Style;
                dgFreeCashFlows.Columns[8].HeaderCellStyle = Resources["GridViewHeaderCellStyle"] as Style;
                dgFreeCashFlows.Columns[9].HeaderCellStyle = Resources["GridViewHeaderCellStyle"] as Style;
                dgFreeCashFlows.Columns[10].HeaderCellStyle = Resources["GridViewHeaderCellStyle"] as Style;

                dgFreeCashFlows.Columns[6].Background = new SolidColorBrush(Colors.LightGray);
                dgFreeCashFlows.Columns[7].Background = new SolidColorBrush(Colors.LightGray);
                dgFreeCashFlows.Columns[8].Background = new SolidColorBrush(Colors.LightGray);
                dgFreeCashFlows.Columns[9].Background = new SolidColorBrush(Colors.LightGray);
                dgFreeCashFlows.Columns[10].Background = new SolidColorBrush(Colors.LightGray);

            }

        }

        private void dgFreeCashFlows_RowLoaded(object sender, Telerik.Windows.Controls.GridView.RowLoadedEventArgs e)
        {
            if (e.Row != null)
            {
                if (e.Row.DataContext != null)
                {
                    var item = (e.Row.DataContext) as FreeCashFlowsData;
                    if (item != null)
                    {
                        if (item.ValueY0 != null)
                        {
                            if (item.ValueY0.Contains("("))
                                dgFreeCashFlows.Columns[1].CellStyle = Resources["GridViewCellStyleRedText"] as Style;
                        }
                        if (item.ValueY1 != null)
                        {
                            if (item.ValueY1.Contains("("))
                                dgFreeCashFlows.Columns[2].CellStyle = Resources["GridViewCellStyleRedText"] as Style;
                        }
                        if (item.ValueY2 != null)
                        {
                            if (item.ValueY2.Contains("("))
                                dgFreeCashFlows.Columns[3].CellStyle = Resources["GridViewCellStyleRedText"] as Style;
                        }
                        if (item.ValueY3 != null)
                        {
                            if (item.ValueY3.Contains("("))
                                dgFreeCashFlows.Columns[4].CellStyle = Resources["GridViewCellStyleRedText"] as Style;
                        }
                        if (item.ValueY4 != null)
                        {
                            if (item.ValueY4.Contains("("))
                                dgFreeCashFlows.Columns[5].CellStyle = Resources["GridViewCellStyleRedText"] as Style;
                        }
                        if (item.ValueY5 != null)
                        {
                            if (item.ValueY5.Contains("("))
                                dgFreeCashFlows.Columns[6].CellStyle = Resources["GridViewCellStyleRedText"] as Style;
                        }
                        if (item.ValueY6 != null)
                        {
                            if (item.ValueY6.Contains("("))
                                dgFreeCashFlows.Columns[7].CellStyle = Resources["GridViewCellStyleRedText"] as Style;
                        }
                        if (item.ValueY7 != null)
                        {
                            if (item.ValueY7.Contains("("))
                                dgFreeCashFlows.Columns[8].CellStyle = Resources["GridViewCellStyleRedText"] as Style;
                        }
                        if (item.ValueY8 != null)
                        {
                            if (item.ValueY8.Contains("("))
                                dgFreeCashFlows.Columns[9].CellStyle = Resources["GridViewCellStyleRedText"] as Style;
                        }
                        if (item.ValueY9 != null)
                        {
                            if (item.ValueY9.Contains("("))
                                dgFreeCashFlows.Columns[10].CellStyle = Resources["GridViewCellStyleRedText"] as Style;
                        }
                    }
                }

            }
        }
        #endregion

        #region ExcelExport

        /// <summary>
        /// Static class storing string types
        /// </summary>
        private static class ExportTypes
        {
            public const string DCF_FREE_CASH_FLOWS_DATA = "DCF - Free Cash Flows Data";
        }

        /// <summary>
        /// Method to catch Click Event of Export to Excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(this.DataContextFreeCashFlows.logger, methodNamespace);
            try
            {
                if (this.dgFreeCashFlows.Visibility == Visibility.Visible)
                {
                    List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>
                        {
                                new RadExportOptions() { ElementName = ExportTypes.DCF_FREE_CASH_FLOWS_DATA, Element = this.dgFreeCashFlows, ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXPORT_FILTER }
                        };
                    ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + ExportTypes.DCF_FREE_CASH_FLOWS_DATA);
                    childExportOptions.Show();
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(this.DataContextFreeCashFlows.logger, ex);
            }
        }

        #endregion

        #region HelperMethods

        /// <summary>
        /// Element Exporting Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ElementExportingEvent(object sender, Telerik.Windows.Controls.GridViewElementExportingEventArgs e)
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
            Logging.LogBeginMethod(this.DataContextFreeCashFlows.logger, methodNamespace);
            try
            {
                PDFExporter.btnExportPDF_Click(this.dgFreeCashFlows);
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(this.DataContextFreeCashFlows.logger, ex);
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
            Logging.LogBeginMethod(this.DataContextFreeCashFlows.logger, methodNamespace);
            try
            {
                Dispatcher.BeginInvoke((Action)(() =>
                {
                    RichTextBox.Document = PDFExporter.Print(dgFreeCashFlows, 6);
                }));

                this.RichTextBox.Document.SectionDefaultPageOrientation = PageOrientation.Landscape;
                RichTextBox.Print("MyDocument", Telerik.Windows.Documents.UI.PrintMode.Native);
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(this.DataContextFreeCashFlows.logger, ex);
            }
        }

        #endregion

        #region EventsUnsubscribe

        /// <summary>
        /// UnSubscribing the Events
        /// </summary>
        public override void Dispose()
        {
            this.DataContextFreeCashFlows.Dispose();
            this.DataContextFreeCashFlows = null;
            this.DataContext = null;
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
                GridViewLength columnWidth = this.dgFreeCashFlows.Columns[0].ActualWidth;
                data.DataTable = PDFExporter.CreateTable(dgFreeCashFlows, 11, columnWidth, "FreeCashFlows");
                foreach (GridViewBoundColumnBase item in dgFreeCashFlows.Columns)
                {
                    item.Width = GridViewLength.Auto;
                }
                dgFreeCashFlows.InvalidateMeasure();
                return data;
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                return null;
            }
        }

    }
}
