using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.IO;
using System.Collections;
using System.Windows.Controls;
using System.Windows.Printing;
using Telerik.Windows.Documents.Model;
using Telerik.Windows.Documents.FormatProviders.Pdf;
using Telerik.Windows.Data;
using Telerik.Windows.Documents.UI;
using Telerik.Windows.Controls.GridView;
using Telerik.Windows.Controls;
using GreenField.DataContracts;
using GreenField.Gadgets.Helpers;
using GreenField.ServiceCaller.BenchmarkHoldingsDefinitions;
using GreenField.Gadgets.ViewModels;
using GreenField.Common;
using GreenField.ServiceCaller;

namespace GreenField.Gadgets.Views
{
    /// <summary>
    /// Class for the Attribution View
    /// </summary>
    public partial class ViewAttribution : ViewBaseUserControl
    {
        #region constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="DataContextSource">ViewModelAttribution as Data context for this View</param>
        public ViewAttribution(ViewModelAttribution dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextAttribution = dataContextSource;
            dataContextSource.AttributionDataLoadedEvent +=
            new DataRetrievalProgressIndicatorEventHandler(dataContextSource_attributionDataLoadedEvent);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Property of the type of View Model for this view
        /// </summary>
        private ViewModelAttribution dataContextAttribution;
        public ViewModelAttribution DataContextAttribution
        {
            get { return dataContextAttribution; }
            set { dataContextAttribution = value; }
        }

        /// <summary>
        /// True is gadget is currently on display
        /// </summary>
        private bool isActive;
        public override bool IsActive
        {
            get { return isActive; }
            set
            {
                isActive = value;
                if (DataContextAttribution != null)
                {
                    DataContextAttribution.IsActive = isActive;
                }
            }
        }
        #endregion

        #region EventHandler

        /// <summary>
        /// Data Retrieval Indicator
        /// </summary>
        /// <param name="e"></param>
        void dataContextSource_attributionDataLoadedEvent(DataRetrievalProgressIndicatorEventArgs e)
        {
            if (e.ShowBusy)
            {
                this.busyIndicatorGrid.IsBusy = true;
            }
            else
            {
                this.busyIndicatorGrid.IsBusy = false;
            }
        }
        #endregion

        #region RemoveEvents
        /// <summary>
        /// Disposing events
        /// </summary>
        public override void Dispose()
        {
            this.DataContextAttribution.AttributionDataLoadedEvent -= new DataRetrievalProgressIndicatorEventHandler(dataContextSource_attributionDataLoadedEvent);
            this.DataContextAttribution.Dispose();
            this.DataContextAttribution = null;
            this.DataContext = null;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Styles added to Export
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgAttribution_ElementExporting(object sender, Telerik.Windows.Controls.GridViewElementExportingEventArgs e)
        {
            RadGridView_ElementExport.ElementExporting(e);
        }
        #endregion

        #region ExportToExcel/PDF/Print

        #region ExcelExport
        /// <summary>
        /// Static class storing string types
        /// </summary>
        private static class ExportTypes
        {
            public const string PerformanceAttributionUI = "Performance Attribution";
        }

        /// <summary>
        /// Method to catch Click Event of Export to Excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.dgAttribution.Visibility == Visibility.Visible)
                {
                    List<RadExportOptions> radExportOptionsInfo = new List<RadExportOptions>
                {                  
                      new RadExportOptions() { ElementName = "Performance Attribution", Element = this.dgAttribution, ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXCEL_EXPORT_FILTER },                    
                };
                    ChildExportOptions childExportOptions = new ChildExportOptions(radExportOptionsInfo, "Export Options: " + GadgetNames.PERFORMANCE_ATTRIBUTION);
                    childExportOptions.Show();
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog(ex.Message);
            }
        }
        #endregion

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
                    ElementName = ExportTypes.PerformanceAttributionUI,
                    Element = this.dgAttribution,
                    ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_PRINT_FILTER,
                    RichTextBox = this.RichTextBox
                });

                ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + GadgetNames.PERFORMANCE_ATTRIBUTION);
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
                    ElementName = ExportTypes.PerformanceAttributionUI,
                    Element = this.dgAttribution,
                    ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_PDF_EXPORT_FILTER,
                    RichTextBox = this.RichTextBox
                });

                ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + GadgetNames.PERFORMANCE_ATTRIBUTION);
                childExportOptions.Show();

            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
            }
        }

        //#region PDFExport
        ///// <summary>
        ///// Event handler when user wants to Export the Grid to PDF
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void btnExportPDF_Click(object sender, RoutedEventArgs e)
        //{
        //    string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
        //    try
        //    {
        //        PDFExporter.btnExportPDF_Click(this.dgAttribution);
        //    }
        //    catch (Exception ex)
        //    {
        //        Prompt.ShowDialog(ex.Message);
        //    }
        //}
        //#endregion

        //#region Printing the DataGrid

        ///// <summary>
        ///// Printing the DataGrid
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void btnPrint_Click(object sender, RoutedEventArgs e)
        //{
        //    string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
        //    try
        //    {
        //        Dispatcher.BeginInvoke((Action)(() =>
        //        {
        //            RichTextBox.Document = PDFExporter.Print(dgAttribution, 6);
        //        }));
        //        this.RichTextBox.Document.SectionDefaultPageOrientation = PageOrientation.Landscape;
        //        RichTextBox.Print("MyDocument", Telerik.Windows.Documents.UI.PrintMode.Native);
        //    }
        //    catch (Exception ex)
        //    {
        //        Prompt.ShowDialog(ex.Message);
        //    }
        //}
        //#endregion
        #endregion
    }
}
