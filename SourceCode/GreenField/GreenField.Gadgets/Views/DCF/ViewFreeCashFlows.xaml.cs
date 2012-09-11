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
        
    }
}
