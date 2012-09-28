using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Telerik.Windows.Controls;
using System.IO;
using Telerik.Windows.Documents.Model;
using Telerik.Windows.Documents.FormatProviders.Pdf;
using Telerik.Windows.Data;
using System.Collections;
using GreenField.DataContracts;

#if !SILVERLIGHT
using Microsoft.Win32;
#else
using System.Windows.Controls;
using System.Windows.Printing;
using GreenField.Gadgets.Helpers;
using Telerik.Windows.Controls.GridView;
using GreenField.ServiceCaller.BenchmarkHoldingsDefinitions;
using GreenField.Gadgets.ViewModels;
using GreenField.Common;
using Telerik.Windows.Documents.UI;
using GreenField.ServiceCaller;
#endif



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
            dataContextSource.attributionDataLoadedEvent +=
        new DataRetrievalProgressIndicatorEventHandler(dataContextSource_attributionDataLoadedEvent);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Property of the type of View Model for this view
        /// </summary>
        private ViewModelAttribution _dataContextAttribution;
        public ViewModelAttribution DataContextAttribution
        {
            get { return _dataContextAttribution; }
            set { _dataContextAttribution = value; }
        }

        /// <summary>
        /// True is gadget is currently on display
        /// </summary>
        private bool _isActive;
        public override bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
                if (DataContextAttribution != null)
                    DataContextAttribution.IsActive = _isActive;
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
            this.DataContextAttribution.attributionDataLoadedEvent -= new DataRetrievalProgressIndicatorEventHandler(dataContextSource_attributionDataLoadedEvent);
            this.DataContextAttribution.Dispose();
            this.DataContextAttribution = null;
            this.DataContext = null;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// When row gets loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgAttribution_RowLoaded(object sender, Telerik.Windows.Controls.GridView.RowLoadedEventArgs e)
        {
           
        }


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
            public const string PERFORMANCE_ATTRIBUTION_UI = "Performance Attribution";
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
                    List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>
                {
                  
                      new RadExportOptions() { ElementName = "Performance Attribution", Element = this.dgAttribution, ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXPORT_FILTER },
                    
                };
                    ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + GadgetNames.PERFORMANCE_ATTRIBUTION);
                    childExportOptions.Show();
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog(ex.Message);
            }
        }

#endregion

        #region HelperMethods
        /// <summary>
        /// Element Exporting Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgPortfolioDetails_ElementExporting(object sender, GridViewElementExportingEventArgs e)
        {
            RadGridView_ElementExport.ElementExporting(e, showGroupFooters: false);
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

            try
            {
                PDFExporter.btnExportPDF_Click(this.dgAttribution);
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog(ex.Message);
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
   
            try
            {
                Dispatcher.BeginInvoke((Action)(() =>
                {
                    RichTextBox.Document = PDFExporter.Print(dgAttribution, 6);
                }));

                this.RichTextBox.Document.SectionDefaultPageOrientation = PageOrientation.Landscape;
                RichTextBox.Print("MyDocument", Telerik.Windows.Documents.UI.PrintMode.Native);
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog(ex.Message);
            }
        }
             #endregion
     #endregion
    }
}
