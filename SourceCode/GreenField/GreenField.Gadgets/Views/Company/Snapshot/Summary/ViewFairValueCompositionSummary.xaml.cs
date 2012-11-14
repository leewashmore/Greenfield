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
    public partial class ViewFairValueCompositionSummary : ViewBaseUserControl
    {

        #region PropertyDeclaration

        /// <summary>
        /// Property of ViewModel type
        /// </summary>
        private ViewModelFairValueCompositionSummary dataContextFairValueCompositionSummary;
        public ViewModelFairValueCompositionSummary DataContextFairValueCompositionSummary
        {
            get
            {
                return dataContextFairValueCompositionSummary;
            }
            set
            {
                dataContextFairValueCompositionSummary = value;
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
                if (DataContextFairValueCompositionSummary != null) //DataContext instance
                    DataContextFairValueCompositionSummary.IsActive = isActive;
            }
        }
        #endregion

        #region CONSTRUCTOR
        /// <summary>
        /// CONSTRUCTOR
        /// </summary>
        /// <param name="DataContextFairValueCompositionSummary"></param>
        public ViewFairValueCompositionSummary(ViewModelFairValueCompositionSummary DataContextFairValueCompositionSummary)
        {
            InitializeComponent();
            this.DataContext = DataContextFairValueCompositionSummary;
            this.DataContextFairValueCompositionSummary = DataContextFairValueCompositionSummary;
            //this.DataContextFairValueCompositionSummary.RetrieveFairValueCompositionSummaryDataCompleteEvent += new RetrieveFairValueCompositionSummaryDataCompletedEventHandler(RetrieveFairValueCompositionSummaryDataCompletedEvent);
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
            Logging.LogBeginMethod(this.DataContextFairValueCompositionSummary.logger, methodNamespace);
            try
            {
                if (this.dgFairValueCompositionSummary.Visibility == Visibility.Visible)
                {
                    List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>
                        {
                                new RadExportOptions() { ElementName = ExportTypes.DCF_FREE_CASH_FLOWS_DATA, Element = this.dgFairValueCompositionSummary, ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXCEL_EXPORT_FILTER }
                        };
                    ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + ExportTypes.DCF_FREE_CASH_FLOWS_DATA);
                    childExportOptions.Show();
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(this.DataContextFairValueCompositionSummary.logger, ex);
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

        //#region PDFExport
        ///// <summary>
        ///// Event handler when user wants to Export the Grid to PDF
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void btnExportPDF_Click(object sender, RoutedEventArgs e)
        //{
        //    string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
        //    Logging.LogBeginMethod(this.DataContextFairValueCompositionSummary.logger, methodNamespace);
        //    try
        //    {
        //        PDFExporter.btnExportPDF_Click(this.dgFairValueCompositionSummary);
        //    }
        //    catch (Exception ex)
        //    {
        //        Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
        //        Logging.LogException(this.DataContextFairValueCompositionSummary.logger, ex);
        //    }
        //}
        //#endregion

        #region Printing the DataGrid

        ///// <summary>
        ///// Printing the DataGrid
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void btnPrint_Click(object sender, RoutedEventArgs e)
        //{
        //    string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
        //    Logging.LogBeginMethod(this.DataContextFairValueCompositionSummary.logger, methodNamespace);
        //    try
        //    {
        //        Dispatcher.BeginInvoke((Action)(() =>
        //        {
        //            RichTextBox.Document = PDFExporter.Print(dgFairValueCompositionSummary, 6);
        //        }));

        //        this.RichTextBox.Document.SectionDefaultPageOrientation = PageOrientation.Landscape;
        //        RichTextBox.Print("MyDocument", Telerik.Windows.Documents.UI.PrintMode.Native);
        //    }
        //    catch (Exception ex)
        //    {
        //        Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
        //        Logging.LogException(this.DataContextFairValueCompositionSummary.logger, ex);
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
            Logging.LogBeginMethod(this.DataContextFairValueCompositionSummary.logger, methodNamespace);
            try
            {
                List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>();


                RadExportOptionsInfo.Add(new RadExportOptions()
                {
                    ElementName = ExportTypes.DCF_FREE_CASH_FLOWS_DATA,
                    Element = this.dgFairValueCompositionSummary,
                    ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_PRINT_FILTER,
                    RichTextBox = this.RichTextBox
                });

                ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + GadgetNames.SECURITY_REFERENCE_PRICE_COMPARISON);
                childExportOptions.Show();
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(this.DataContextFairValueCompositionSummary.logger, ex);
            }
        }

        /// <summary>
        /// Event handler when user wants to Export the Grid to PDF
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportPDF_Click(object sender, RoutedEventArgs e)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(this.DataContextFairValueCompositionSummary.logger, methodNamespace);
            try
            {
                List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>();


                RadExportOptionsInfo.Add(new RadExportOptions() { ElementName = ExportTypes.DCF_FREE_CASH_FLOWS_DATA, Element = this.dgFairValueCompositionSummary, ExportFilterOption = RadExportFilterOption.RADCHART_PDF_EXPORT_FILTER });

                ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + GadgetNames.PORTFOLIO_CONSTRUCTION_FAIR_VALUE_COMPOSITION);
                childExportOptions.Show();
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(this.DataContextFairValueCompositionSummary.logger, ex);
            }
        }
        
        #endregion

        #region EventsUnsubscribe

        /// <summary>
        /// UnSubscribing the Events
        /// </summary>
        public override void Dispose()
        {
            this.DataContextFairValueCompositionSummary.Dispose();
            this.DataContextFairValueCompositionSummary = null;
            this.DataContext = null;
        }

        #endregion

    }
}
