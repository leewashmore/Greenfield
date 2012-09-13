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
    public partial class ViewFairValueCompositionSummary: ViewBaseUserControl
    {
        
        #region PropertyDeclaration

        /// <summary>
        /// Property of ViewModel type
        /// </summary>
        private ViewModelFairValueCompositionSummary _dataContextFairValueCompositionSummary;
        public ViewModelFairValueCompositionSummary DataContextFairValueCompositionSummary
        {
            get
            {
                return _dataContextFairValueCompositionSummary;
            }
            set
            {
                _dataContextFairValueCompositionSummary = value;
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
                if (DataContextFairValueCompositionSummary != null) //DataContext instance
                    DataContextFairValueCompositionSummary.IsActive = _isActive;
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

        //# region EVENT
        //private List<FairValueCompositionSummaryData> FairValueCompositionSummaryData;
        //public void RetrieveFairValueCompositionSummaryDataCompletedEvent(RetrieveFairValueCompositionSummaryDataCompleteEventArs e)
        //{
        //    FairValueCompositionSummaryData = e.FairValueCompositionSummaryInfo;
        //    if (FairValueCompositionSummaryData != null)
        //    {
        //        dgFairValueCompositionSummary.Columns[1].Header = System.DateTime.Now.Year + "\n" + "Y1";
        //        dgFairValueCompositionSummary.Columns[2].Header = System.DateTime.Now.Year + 1 + "\n" + "Y2";
        //        dgFairValueCompositionSummary.Columns[3].Header = System.DateTime.Now.Year + 2 + "\n" + "Y3";
        //        dgFairValueCompositionSummary.Columns[4].Header = System.DateTime.Now.Year + 3 + "\n" + "Y4";
        //        dgFairValueCompositionSummary.Columns[5].Header = System.DateTime.Now.Year + 4 + "\n" + "Y5";
        //        //dgFairValueCompositionSummary.Columns[6].Header = System.DateTime.Now.Year + 5 + "\n" + "Y6";
        //        //dgFairValueCompositionSummary.Columns[7].Header = System.DateTime.Now.Year + 6 + "\n" + "Y7";
        //        //dgFairValueCompositionSummary.Columns[8].Header = System.DateTime.Now.Year + 7 + "\n" + "Y8";
        //        //dgFairValueCompositionSummary.Columns[9].Header = System.DateTime.Now.Year + 8 + "\n" + "Y9";
        //        //dgFairValueCompositionSummary.Columns[10].Header = System.DateTime.Now.Year + 9 + "\n" + "Y10";

        //        dgFairValueCompositionSummary.Columns[1].HeaderCellStyle = Resources["GridViewHeaderCellStyle"] as Style;
        //        dgFairValueCompositionSummary.Columns[2].HeaderCellStyle = Resources["GridViewHeaderCellStyle"] as Style;
        //        dgFairValueCompositionSummary.Columns[3].HeaderCellStyle = Resources["GridViewHeaderCellStyle"] as Style;
        //        dgFairValueCompositionSummary.Columns[4].HeaderCellStyle = Resources["GridViewHeaderCellStyle"] as Style;
        //        dgFairValueCompositionSummary.Columns[5].HeaderCellStyle = Resources["GridViewHeaderCellStyle"] as Style;
        //        //dgFairValueCompositionSummary.Columns[6].HeaderCellStyle = Resources["GridViewHeaderCellStyle"] as Style;
        //        //dgFairValueCompositionSummary.Columns[7].HeaderCellStyle = Resources["GridViewHeaderCellStyle"] as Style;
        //        //dgFairValueCompositionSummary.Columns[8].HeaderCellStyle = Resources["GridViewHeaderCellStyle"] as Style;
        //        //dgFairValueCompositionSummary.Columns[9].HeaderCellStyle = Resources["GridViewHeaderCellStyle"] as Style;
        //        //dgFairValueCompositionSummary.Columns[10].HeaderCellStyle = Resources["GridViewHeaderCellStyle"] as Style;
                
        //    }

        //}
        //#endregion
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
            Logging.LogBeginMethod(this.DataContextFairValueCompositionSummary._logger, methodNamespace);
            try
            {
                if (this.dgFairValueCompositionSummary.Visibility == Visibility.Visible)
                {
                    List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>
                        {
                                new RadExportOptions() { ElementName = ExportTypes.DCF_FREE_CASH_FLOWS_DATA, Element = this.dgFairValueCompositionSummary, ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXPORT_FILTER }
                        };
                    ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + ExportTypes.DCF_FREE_CASH_FLOWS_DATA);
                    childExportOptions.Show();
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(this.DataContextFairValueCompositionSummary._logger, ex);
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
            Logging.LogBeginMethod(this.DataContextFairValueCompositionSummary._logger, methodNamespace);
            try
            {
                PDFExporter.btnExportPDF_Click(this.dgFairValueCompositionSummary);
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(this.DataContextFairValueCompositionSummary._logger, ex);
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
            Logging.LogBeginMethod(this.DataContextFairValueCompositionSummary._logger, methodNamespace);
            try
            {
                Dispatcher.BeginInvoke((Action)(() =>
                {
                    RichTextBox.Document = PDFExporter.Print(dgFairValueCompositionSummary, 6);
                }));

                this.RichTextBox.Document.SectionDefaultPageOrientation = PageOrientation.Landscape;
                RichTextBox.Print("MyDocument", Telerik.Windows.Documents.UI.PrintMode.Native);
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(this.DataContextFairValueCompositionSummary._logger, ex);
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
