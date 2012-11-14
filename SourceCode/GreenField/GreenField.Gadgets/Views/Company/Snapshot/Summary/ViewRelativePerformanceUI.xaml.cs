using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls;
using GreenField.Gadgets.Helpers;
using GreenField.Gadgets.ViewModels;
using GreenField.Common;
using GreenField.ServiceCaller;

namespace GreenField.Gadgets.Views
{
    /// <summary>
    /// XAML.CS class for RelativePerformanceUI gadget
    /// </summary>
    public partial class ViewRelativePerformanceUI : ViewBaseUserControl
    {
        #region PropertyDeclaration

        /// <summary>
        /// Property of type View-Model
        /// </summary>
        private ViewModelRelativePerformanceUI dataContextRelativePerformanceUI;
        public ViewModelRelativePerformanceUI DataContextRelativePerformanceUI
        {
            get { return dataContextRelativePerformanceUI; }
            set { dataContextRelativePerformanceUI = value; }
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
                if (DataContextRelativePerformanceUI != null)
                {
                    DataContextRelativePerformanceUI.IsActive = isActive;
                }
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor that initialises the class
        /// </summary>
        /// <param name="dataContextSource"></param>
        public ViewRelativePerformanceUI(ViewModelRelativePerformanceUI dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextRelativePerformanceUI = dataContextSource;
        }

        #endregion

        #region Printing

        #region PrivateVariablesPrinting

        /// <summary>
        /// OffSet value
        /// </summary>
        double offsetY;

        /// <summary>
        /// Total Height
        /// </summary>
        double totalHeight;

        /// <summary>
        /// Canvas
        /// </summary>
        Canvas canvas;
        
        /// <summary>
        /// Instance of RadGrid
        /// </summary>
        RadGridView grid;

        #endregion

        ///// <summary>
        ///// Print Button Click Event
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void btnPrint_Click(object sender, RoutedEventArgs e)
        //{
        //    string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
        //    Logging.LogBeginMethod(this.DataContextRelativePerformanceUI.logger, methodNamespace);

        //    try
        //    {
        //        Dispatcher.BeginInvoke((Action)(() =>
        //            {
        //                RichTextBox.Document = PDFExporter.Print(dgRelativePerformanceUI, 10);
        //            }));

        //        RichTextBox.Print("MyDocument", Telerik.Windows.Documents.UI.PrintMode.Native);
        //    }
        //    catch (Exception ex)
        //    {
        //        Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
        //        Logging.LogException(this.DataContextRelativePerformanceUI.logger, ex);
        //    }
        //}

        /// <summary>
        /// Printing the DataGrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(this.DataContextRelativePerformanceUI.logger, methodNamespace);
            try
            {
                List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>();


                RadExportOptionsInfo.Add(new RadExportOptions()
                {
                    ElementName = ExportTypes.RELATIVE_PERFORMANCE_UI,
                    Element = this.dgRelativePerformanceUI,
                    ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_PRINT_FILTER,
                    RichTextBox = this.RichTextBox
                });

                ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + GadgetNames.SECURITY_REFERENCE_PRICE_COMPARISON);
                childExportOptions.Show();
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(this.DataContextRelativePerformanceUI.logger, ex);
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
            Logging.LogBeginMethod(this.DataContextRelativePerformanceUI.logger, methodNamespace);
            try
            {
                List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>();


                RadExportOptionsInfo.Add(new RadExportOptions() { ElementName = ExportTypes.RELATIVE_PERFORMANCE_UI, Element = this.dgRelativePerformanceUI, ExportFilterOption = RadExportFilterOption.RADCHART_PDF_EXPORT_FILTER });

                ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + GadgetNames.PORTFOLIO_CONSTRUCTION_FAIR_VALUE_COMPOSITION);
                childExportOptions.Show();
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(this.DataContextRelativePerformanceUI.logger, ex);
            }
        }
        
        #endregion

        #region ExportToExcel

        /// <summary>
        /// Class Containing the Name of Exported File
        /// </summary>
        private static class ExportTypes
        {
            public const string RELATIVE_PERFORMANCE_UI = "Relative Performance UI";
        }

        /// <summary>
        /// Export Button Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(this.DataContextRelativePerformanceUI.logger, methodNamespace);
            try
            {
                List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>
                {
                    new RadExportOptions() { ElementName = ExportTypes.RELATIVE_PERFORMANCE_UI, Element = this.dgRelativePerformanceUI, ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXCEL_EXPORT_FILTER },                  
                };
                ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + GadgetNames.BENCHMARK_RELATIVE_PERFORMANCE);
                childExportOptions.Show();
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(this.DataContextRelativePerformanceUI.logger, ex);
            }
        }

        /// <summary>
        /// Helper method Export
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgRelativePerformanceUI_ElementExporting(object sender, GridViewElementExportingEventArgs e)
        {
            RadGridView_ElementExport.ElementExporting(e);
        }

        #endregion
                
        #region EventsUnsubscribe

        /// <summary>
        /// Disposing UserControl
        /// </summary>
        public override void Dispose()
        {
            this.DataContextRelativePerformanceUI.Dispose();
            this.DataContextRelativePerformanceUI = null;
            this.DataContext = null;
        }

        #endregion
    }
}
