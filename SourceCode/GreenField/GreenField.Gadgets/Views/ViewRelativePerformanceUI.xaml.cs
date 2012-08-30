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
using Telerik.Windows.Controls;
using System.Windows.Printing;
using GreenField.ServiceCaller;

namespace GreenField.Gadgets.Views
{
    /// <summary>
    /// XAML.CS class for RelativePerformanceUI gadget
    /// </summary>
    public partial class ViewRelativePerformanceUI : ViewBaseUserControl
    {
        #region PrivateVariables

        #endregion

        #region PropertyDeclaration

        /// <summary>
        /// Property of type View-Model
        /// </summary>
        private ViewModelRelativePerformanceUI _dataContextRelativePerformanceUI;
        public ViewModelRelativePerformanceUI DataContextRelativePerformanceUI
        {
            get { return _dataContextRelativePerformanceUI; }
            set { _dataContextRelativePerformanceUI = value; }
        }

        /// <summary>
        /// To check whether the Dashboard is Active or not
        /// </summary>
        private bool _isActive;
        public override bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
                if (DataContextRelativePerformanceUI != null)
                    DataContextRelativePerformanceUI.IsActive = _isActive;
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

        double offsetY;
        double totalHeight;
        Canvas canvas;
        RadGridView grid;

        #endregion

        /// <summary>
        /// Print Button Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(this.DataContextRelativePerformanceUI._logger, methodNamespace);

            try
            {
                Dispatcher.BeginInvoke((Action)(() =>
                    {
                        RichTextBox.Document = PDFExporter.Print(dgRelativePerformanceUI, 10);
                    }));

                RichTextBox.Print("MyDocument", Telerik.Windows.Documents.UI.PrintMode.Native);
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(this.DataContextRelativePerformanceUI._logger, ex);
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
            Logging.LogBeginMethod(this.DataContextRelativePerformanceUI._logger, methodNamespace);
            try
            {
                List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>
                {
                    new RadExportOptions() { ElementName = ExportTypes.RELATIVE_PERFORMANCE_UI, Element = this.dgRelativePerformanceUI, ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXPORT_FILTER },                  
                };
                ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + GadgetNames.BENCHMARK_RELATIVE_PERFORMANCE);
                childExportOptions.Show();
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(this.DataContextRelativePerformanceUI._logger, ex);
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

        #region ApplyStyles


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
