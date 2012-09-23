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
using Telerik.Windows.Controls;
using System.Windows.Data;
using Telerik.Windows;
using Telerik.Windows.Controls.GridView;

namespace GreenField.Gadgets.Views
{
    public partial class ViewFairValueComposition : ViewBaseUserControl
    {

        #region PropertyDeclaration


        /// <summary>
        /// Property of ViewModel type
        /// </summary>
        private ViewModelFairValueComposition _dataContextFairValueComposition;
        public ViewModelFairValueComposition DataContextFairValueComposition
        {
            get
            {
                return _dataContextFairValueComposition;
            }
            set
            {
                _dataContextFairValueComposition = value;
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
                if (DataContextFairValueComposition != null) //DataContext instance
                    DataContextFairValueComposition.IsActive = _isActive;
            }
        }

        #endregion

        #region CONSTRUCTOR
        /// <summary>
        /// CONSTRUCTOR
        /// </summary>
        /// <param name="DataContextFairValueComposition"></param>
        public ViewFairValueComposition(ViewModelFairValueComposition DataContextFairValueComposition)
        {
            InitializeComponent();
            this.DataContext = DataContextFairValueComposition;
            this.DataContextFairValueComposition = DataContextFairValueComposition;
            this.AddHandler(RadComboBox.SelectionChangedEvent,
                new Telerik.Windows.Controls.SelectionChangedEventHandler(ComboSelectionChanged));
        }
        #endregion

        #region Edit Events
        /// <summary>
        /// Beginnig Edit Event
        /// </summary>
        public void dgFairValueComposition_BeginningEdit(object sender, GridViewBeginningEditRoutedEventArgs e)
        {
            if (e.Cell.DataColumn.UniqueName == "Source")
            {
                RadComboBox cbSourcecontrol = e.Cell.DataColumn.CellTemplate.FindChildByType< RadComboBox>();
            }
            if (e.Cell.DataColumn.UniqueName == "Sell")
            {
                e.Cell.Value = "";
            }         

        }
        /// <summary>
        /// Validating the Contents of the Edited Cell.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgFairValueComposition_CellValidating(object sender, GridViewCellValidatingEventArgs e)
        {
            if (e.Cell.Column.UniqueName == "Sell")
            {
                decimal value;
                var textEntered = e.NewValue.ToString();

                if (!Decimal.TryParse(textEntered, out value))
                {
                    e.IsValid = false;
                    e.ErrorMessage = "The Entered value should be a valid number";
                }
            }
        }
        /// <summary>
        /// Event that occurs after the Editing is completed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgFairValueComposition_RowEditEnded(object sender, GridViewRowEditEndedEventArgs e)
        {
           
        }

        /// <summary>
        /// Row Loaded Event, changes the color for StockSpecificDiscount
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgFairValueComposition_RowLoaded(object sender, Telerik.Windows.Controls.GridView.RowLoadedEventArgs e)
        {
            if (e.Row != null)
            {
                if (e.Row.DataContext != null)
                {
                    var data = e.Row.DataContext as FairValueCompositionSummaryData;
                    if (data == null)
                        return;
                    //if ((e.Row.DataContext as FairValueCompositionSummaryData).SELL)
                    //{
                    //    e.Row.Background = new SolidColorBrush(Colors.Yellow);
                    //}
                }
            }
        }

        /// <summary>
        /// Gets called when combob selection changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void ComboSelectionChanged(object sender, RadRoutedEventArgs args)
        {
            RadComboBox comboBox = (RadComboBox)args.OriginalSource;

            var row = comboBox.ParentOfType<GridViewRow>();

            FairValueData data = row.DataContext as FairValueData;

            if (data.DataId != Convert.ToInt16(comboBox.SelectedValue))
            {
                data.DataId = Convert.ToInt16(comboBox.SelectedValue);
                data.Measure = comboBox.Text;
                DataContextFairValueComposition.EditedMeasurePropertyFairValueRow = data;
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
            Logging.LogBeginMethod(this.DataContextFairValueComposition._logger, methodNamespace);
            try
            {
                if (this.dgFairValueComposition.Visibility == Visibility.Visible)
                {
                    List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>
                        {
                                new RadExportOptions() { ElementName = ExportTypes.DCF_FREE_CASH_FLOWS_DATA, Element = this.dgFairValueComposition, ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXPORT_FILTER }
                        };
                    ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + ExportTypes.DCF_FREE_CASH_FLOWS_DATA);
                    childExportOptions.Show();
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(this.DataContextFairValueComposition._logger, ex);
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
            Logging.LogBeginMethod(this.DataContextFairValueComposition._logger, methodNamespace);
            try
            {
                PDFExporter.btnExportPDF_Click(this.dgFairValueComposition);
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(this.DataContextFairValueComposition._logger, ex);
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
            Logging.LogBeginMethod(this.DataContextFairValueComposition._logger, methodNamespace);
            try
            {
                Dispatcher.BeginInvoke((Action)(() =>
                {
                    RichTextBox.Document = PDFExporter.Print(dgFairValueComposition, 6);
                }));

                this.RichTextBox.Document.SectionDefaultPageOrientation = PageOrientation.Landscape;
                RichTextBox.Print("MyDocument", Telerik.Windows.Documents.UI.PrintMode.Native);
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(this.DataContextFairValueComposition._logger, ex);
            }
        }

        #endregion

        #region EventsUnsubscribe

        /// <summary>
        /// UnSubscribing the Events
        /// </summary>
        public override void Dispose()
        {
            this.DataContextFairValueComposition.Dispose();
            this.DataContextFairValueComposition = null;
            this.DataContext = null;
        }

        #endregion

        private void dgFairValueComposition_DataLoaded(object sender, EventArgs e)
        {
            (dgFairValueComposition.Columns[1] as GridViewComboBoxColumn).ItemsSource = DataContextFairValueComposition.MeasuresData;            
        }

        private void dgFairValueComposition_CellEditEnded(object sender, GridViewCellEditEndedEventArgs e)
        {
            if (e.Cell.Column.UniqueName == "Sell")
            {
                FairValueData data = e.Cell.DataContext as FairValueData;

                if (data != null && DataContextFairValueComposition != null)
                {
                    DataContextFairValueComposition.EditedSellPropertyFairValueRow = data;
                }
            }
        }

    }
}
