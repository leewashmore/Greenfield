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
using GreenField.Gadgets.Helpers;
using GreenField.Gadgets.ViewModels;
using GreenField.Common;
using GreenField.ServiceCaller;
using Telerik.Windows.Documents.Model;
using GreenField.DataContracts;

namespace GreenField.Gadgets.Views
{
    public partial class ViewFreeCashFlows : ViewBaseUserControl
    {

        #region PropertyDeclaration

        /// <summary>
        /// Property of ViewModel type
        /// </summary>
        private ViewModelFreeCashFlows _dataContextFreeCashFlows;
        public ViewModelFreeCashFlows DataContextFreeCashFlows
        {
            get
            {
                return _dataContextFreeCashFlows;
            }
            set
            {
                _dataContextFreeCashFlows = value;
            }
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
                if (DataContextFreeCashFlows != null) //DataContext instance
                    DataContextFreeCashFlows.IsActive = _isActive;
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

            }           

        }

        private void dgFreeCashFlows_RowLoaded(object sender, Telerik.Windows.Controls.GridView.RowLoadedEventArgs e)
        {
            if (e.Row != null)
            {
                if(e.Row.DataContext != null)
                {
                   var item = (e.Row.DataContext) as FreeCashFlowsData;
                   if(item != null)
                   {
                       if (item.ValueY0 != null)
                       {
                           if (item.ValueY0.Contains("("))
                               e.Row.Cells[1].Foreground = new SolidColorBrush(Colors.Red);
                       }
                       if (item.ValueY1 != null)
                       {
                           if (item.ValueY1.Contains("("))
                               e.Row.Cells[2].Foreground = new SolidColorBrush(Colors.Red);
                       }
                       if (item.ValueY2 != null)
                       {
                           if (item.ValueY2.Contains("("))
                               e.Row.Cells[3].Foreground = new SolidColorBrush(Colors.Red);
                       }
                       if (item.ValueY3 != null)
                       {
                           if (item.ValueY3.Contains("("))
                               e.Row.Cells[4].Foreground = new SolidColorBrush(Colors.Red);
                       }
                       if (item.ValueY4 != null)
                       {
                           if (item.ValueY4.Contains("("))
                               e.Row.Cells[5].Foreground = new SolidColorBrush(Colors.Red);
                       }
                       if (item.ValueY5 != null)
                       {
                           if (item.ValueY5.Contains("("))
                               e.Row.Cells[6].Foreground = new SolidColorBrush(Colors.Red);
                       }
                       if (item.ValueY6 != null)
                       {
                           if (item.ValueY6.Contains("("))
                               e.Row.Cells[7].Foreground = new SolidColorBrush(Colors.Red);
                       }
                       if (item.ValueY7 != null)
                       {
                           if (item.ValueY7.Contains("("))
                               e.Row.Cells[8].Foreground = new SolidColorBrush(Colors.Red);
                       }
                       if (item.ValueY8 != null)
                       {
                           if (item.ValueY8.Contains("("))
                               e.Row.Cells[9].Foreground = new SolidColorBrush(Colors.Red);
                       }
                       if (item.ValueY9 != null)
                       {
                           if (item.ValueY9.Contains("("))
                               e.Row.Cells[10].Foreground = new SolidColorBrush(Colors.Red);
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
            Logging.LogBeginMethod(this.DataContextFreeCashFlows._logger, methodNamespace);
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
                Logging.LogException(this.DataContextFreeCashFlows._logger, ex);
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
            Logging.LogBeginMethod(this.DataContextFreeCashFlows._logger, methodNamespace);
            try
            {
                PDFExporter.btnExportPDF_Click(this.dgFreeCashFlows);
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(this.DataContextFreeCashFlows._logger, ex);
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
            Logging.LogBeginMethod(this.DataContextFreeCashFlows._logger, methodNamespace);
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
                Logging.LogException(this.DataContextFreeCashFlows._logger, ex);
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
        public override Table CreateDocument()
        {
            try
            {
                if (dgFreeCashFlows.Items.Count > 0)
                {
                    return PDFExporter.CreateTable(dgFreeCashFlows, 12);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                return null;
            }
        }

    }
}
