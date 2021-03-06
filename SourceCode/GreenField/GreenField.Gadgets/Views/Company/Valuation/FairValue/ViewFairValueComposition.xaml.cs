﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;
using Telerik.Windows.Documents.Model;
using GreenField.Common;
using GreenField.DataContracts;
using GreenField.Gadgets.Helpers;
using GreenField.Gadgets.ViewModels;
using GreenField.ServiceCaller;

namespace GreenField.Gadgets.Views
{
    public partial class ViewFairValueComposition : ViewBaseUserControl
    {
        #region PropertyDeclaration

        /// <summary>
        /// Property of ViewModel type
        /// </summary>
        private ViewModelFairValueComposition dataContextFairValueComposition;
        public ViewModelFairValueComposition DataContextFairValueComposition
        {
            get
            {
                return dataContextFairValueComposition;
            }
            set
            {
                dataContextFairValueComposition = value;
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
                if (DataContextFairValueComposition != null)
                {
                    DataContextFairValueComposition.IsActive = isActive;
                }
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
            if (e.Cell.Column.UniqueName == "Sell" || e.Cell.Column.UniqueName == "Buy")
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
                    {
                        return;
                    }
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

            if (row != null)
            {
                FairValueData data = row.DataContext as FairValueData;

                if (comboBox.SelectedValue != null)
                {
                    if (data.DataId != Convert.ToInt16(comboBox.SelectedValue))
                    {
                        data.DataId = Convert.ToInt16(comboBox.SelectedValue);
                        data.Measure = comboBox.Text;
                        DataContextFairValueComposition.EditedMeasurePropertyFairValueRow = data;
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
            public const string DCF_FREE_CASH_FLOWS_DATA = "Fair Value Composition";
        }

        /// <summary>
        /// Method to catch Click Event of Export to Excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(this.DataContextFairValueComposition.logger, methodNamespace);
            try
            {
                if (this.dgFairValueComposition.Visibility == Visibility.Visible)
                {
                    List<RadExportOptions> radExportOptionsInfo = new List<RadExportOptions>
                        {
                                new RadExportOptions() 
                                { 
                                    ElementName = ExportTypes.DCF_FREE_CASH_FLOWS_DATA, 
                                    Element = this.dgFairValueComposition, 
                                    ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXCEL_EXPORT_FILTER 
                                }
                        };
                    ChildExportOptions childExportOptions = new ChildExportOptions(radExportOptionsInfo, "Export Options: " + ExportTypes.DCF_FREE_CASH_FLOWS_DATA);
                    childExportOptions.Show();
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(this.DataContextFairValueComposition.logger, ex);
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
        private void btnExportPdf_Click(object sender, RoutedEventArgs e)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(this.DataContextFairValueComposition.logger, methodNamespace);
            try
            {
                List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>();
                RadExportOptionsInfo.Add(new RadExportOptions()
                {
                    ElementName = ExportTypes.DCF_FREE_CASH_FLOWS_DATA,
                    Element = this.dgFairValueComposition,
                    ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_PDF_EXPORT_FILTER,
                    RichTextBox = this.RichTextBox
                });
                ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + ExportTypes.DCF_FREE_CASH_FLOWS_DATA);
                childExportOptions.Show();                
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(this.DataContextFairValueComposition.logger, ex);
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
            Logging.LogBeginMethod(this.DataContextFairValueComposition.logger, methodNamespace);
            try
            {
                List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>();
                RadExportOptionsInfo.Add(new RadExportOptions()
                {
                    ElementName = ExportTypes.DCF_FREE_CASH_FLOWS_DATA,
                    Element = this.dgFairValueComposition,
                    ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_PRINT_FILTER,
                    RichTextBox = this.RichTextBox
                });
                ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + ExportTypes.DCF_FREE_CASH_FLOWS_DATA);
                childExportOptions.Show(); 
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(this.DataContextFairValueComposition.logger, ex);
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
            if ((dgFairValueComposition.Columns[1] as GridViewComboBoxColumn) != null)
            {
                (dgFairValueComposition.Columns[1] as GridViewComboBoxColumn).ItemsSource = DataContextFairValueComposition.MeasuresData;
            }
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
            if (e.Cell.Column.UniqueName == "Buy")
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
