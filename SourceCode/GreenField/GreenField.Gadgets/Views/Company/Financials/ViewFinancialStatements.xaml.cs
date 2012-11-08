using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;
using Telerik.Windows.Documents.Model;
using GreenField.Common;
using GreenField.Gadgets.Helpers;
using GreenField.Gadgets.Models;
using GreenField.Gadgets.ViewModels;
using GreenField.ServiceCaller;

namespace GreenField.Gadgets.Views
{
    /// <summary>
    /// Code behind for ViewFinancialStatements
    /// </summary>
    public partial class ViewFinancialStatements : ViewBaseUserControl
    {
        #region Properties
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
                if (DataContextSource != null)
                {
                    DataContextSource.IsActive = isActive;
                }
            }
        }

        /// <summary>
        /// View model class object
        /// </summary>
        public ViewModelFinancialStatements DataContextSource { get; set; }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataContextSource">ViewModelFinancialStatements</param>
        public ViewFinancialStatements(ViewModelFinancialStatements dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            DataContextSource = dataContextSource;

            //update column headers and visibility
            PeriodRecord periodRecord = PeriodColumns.SetPeriodRecord();
            PeriodColumns.UpdateColumnInformation(this.dgFinancialReport, new PeriodColumnUpdateEventArg()
            {
                PeriodColumnNamespace = typeof(ViewModelFinancialStatements).FullName,
                PeriodRecord = periodRecord,
                PeriodColumnHeader = PeriodColumns.SetColumnHeaders(periodRecord),
                PeriodIsYearly = true
            });
            
            //event Subcription - PeriodColumnUpdateEvent
            PeriodColumns.PeriodColumnUpdate += new PeriodColumnUpdateEvent(PeriodColumns_PeriodColumnUpdate);
        }
        #endregion               

        #region Event Handlers
        #region Navigation
        /// <summary>
        /// Left navigation button click event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">MouseButtonEventArgs</param>
        private void LeftNavigation_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PeriodColumns.RaisePeriodColumnNavigationCompleted(new PeriodColumnNavigationEventArg()
            {
                PeriodColumnNamespace = typeof(ViewModelFinancialStatements).FullName,
                PeriodColumnNavigationDirection = NavigationDirection.LEFT
            });
        }

        /// <summary>
        /// Right navigation button click event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">MouseButtonEventArgs</param>
        private void RightNavigation_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PeriodColumns.RaisePeriodColumnNavigationCompleted(new PeriodColumnNavigationEventArg()
            {
                PeriodColumnNamespace = typeof(ViewModelFinancialStatements).FullName,
                PeriodColumnNavigationDirection = NavigationDirection.RIGHT
            });
        }         

        /// <summary>
        /// PeriodColumnUpdateEvent Event Handler - Updates column information and enables export button first time data is received
        /// </summary>
        /// <param name="e">PeriodColumnUpdateEventArg</param>
        void PeriodColumns_PeriodColumnUpdate(PeriodColumnUpdateEventArg e)
        {
            if (e.PeriodColumnNamespace == typeof(ViewModelFinancialStatements).FullName && IsActive)
            {
                PeriodColumns.UpdateColumnInformation(this.dgFinancialReport, e, isQuarterImplemented: true);
                PeriodColumns.UpdateColumnInformation(this.dgFinancialReportExt, e, isQuarterImplemented: true);
                this.btnExportExcel.IsEnabled = true;
            }
        }
        #endregion

        #region Styling
        /// <summary>
        /// dgFinancialReport RowLoaded EventHandler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">RowLoadedEventArgs</param>
        private void dgFinancialReport_RowLoaded(object sender, RowLoadedEventArgs e)
        {
            PeriodColumns.RowDataCustomization(e);
        } 
        #endregion

        #region Export
        /// <summary>
        /// dgFinancialReport ElementExporting EventHandler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">GridViewElementExportingEventArgs</param>
        private void dgFinancialReport_ElementExporting(object sender, GridViewElementExportingEventArgs e)
        {
            RadGridView_ElementExport.ElementExporting(e, hideColumnIndex: new List<int> { 1, 14 });
        }

        /// <summary>
        /// dgFinancialReportExt ElementExporting EventHandler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">GridViewElementExportingEventArgs</param>
        private void dgFinancialReportExt_ElementExporting(object sender, GridViewElementExportingEventArgs e)
        {
            RadGridView_ElementExport.ElementExporting(e, hideColumnIndex: new List<int> { 1, 14 }, headerCellValueConverter: () =>
            {
                if (e.Value == null)
                    return null;
                return e.Value.ToString().Replace("A", "E");
            });
        }

        /// <summary>
        /// btnExportExcel Click EventHandler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">RoutedEventArgs</param>
        private void btnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>();

            RadExportOptionsInfo.Add(new RadExportOptions() { ElementName = "Financial Statement Data"
                , Element = this.dgFinancialReport, ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXPORT_FILTER });
            RadExportOptionsInfo.Add(new RadExportOptions() { ElementName = "External Research Data"
                , Element = this.dgFinancialReportExt, ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXPORT_FILTER });

            ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: Financial Statements");
            childExportOptions.Show();        
        }

        /// <summary>
        /// Printing the DataGrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.dgFinancialReportExt.Visibility == Visibility.Visible)
                {
                    Dispatcher.BeginInvoke((Action)(() =>
                    {
                        RichTextBox.Document = PDFExporter.Print(this.dgFinancialReportExt, 6);
                    }));

                    this.RichTextBox.Document.SectionDefaultPageOrientation = PageOrientation.Landscape;
                    RichTextBox.Print("MyDocument", Telerik.Windows.Documents.UI.PrintMode.Native);
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
            try
            {
                if (this.dgFinancialReportExt.Visibility == Visibility.Visible)
                {
                    PDFExporter.btnExportPDF_Click(this.dgFinancialReportExt, skipColumnDisplayIndex: new List<int> { 1, 14 });
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
            }
        }
        #endregion
        #endregion

        #region Event Unsubscribe
        /// <summary>
        /// Dispose objects from memory
        /// </summary>
        public override void Dispose()
        {
            PeriodColumns.PeriodColumnUpdate -= new PeriodColumnUpdateEvent(PeriodColumns_PeriodColumnUpdate);
            (this.DataContext as ViewModelFinancialStatements).Dispose();
            this.DataContext = null;
        }
        #endregion       
    }
}