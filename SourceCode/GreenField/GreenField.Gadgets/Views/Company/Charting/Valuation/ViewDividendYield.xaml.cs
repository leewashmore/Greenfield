using System;
using System.Collections.Generic;
using System.Windows;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.Charting;
using Telerik.Windows.Documents.Model;
using GreenField.Common;
using GreenField.Gadgets.Helpers;
using GreenField.Gadgets.ViewModels;
using GreenField.ServiceCaller;

namespace GreenField.Gadgets.Views
{
    /// <summary>
    /// Code behind for ViewDividendYield
    /// </summary>
    public partial class ViewDividendYield : ViewBaseUserControl
    {
        #region Fields
        /// <summary>
        /// Export Types
        /// </summary>
        private static class ExportTypes
        {
            public const string Dividend_Yield = "Dividend Yield";
            public const string Dividend_Yield_Data = "Dividend Yield Data";
        }
        #endregion

        #region Properties
        /// <summary>
        /// Property of ViewModel type
        /// </summary>
        private ViewModelDividendYield dataContextDividendYield;
        public ViewModelDividendYield DataContextDividendYield
        {
            get
            {
                return dataContextDividendYield;
            }
            set
            {
                dataContextDividendYield = value;
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
                if (DataContextDividendYield != null) 
                    DataContextDividendYield.IsActive = isActive;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataContextSource">ViewModelDividendYield</param>
        public ViewDividendYield(ViewModelDividendYield dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextDividendYield = dataContextSource;
            dataContextSource.ChartArea = this.chDividendYield.DefaultView.ChartArea;
            this.chDividendYield.DataBound += dataContextSource.ChartDataBound;
            this.ApplyChartStyles();
        } 
        #endregion

        #region Event Handlers
        #region Data Load
        /// <summary>
        /// chDividendYield Loaded event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chDividendYield_Loaded(object sender, RoutedEventArgs e)
        {
            if (chDividendYield.DefaultView.ChartLegend.Items.Count != 0)
            {
                ChartLegendItem var = this.chDividendYield.DefaultView.ChartLegend.Items[0];
                this.chDividendYield.DefaultView.ChartLegend.Items.Remove(var);
            }
        }
        #endregion

        #region Export/Print
        /// <summary>
        /// Event for Grid Export
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ElementExportingEvent(object sender, GridViewElementExportingEventArgs e)
        {
            RadGridView_ElementExport.ElementExporting(e);
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
                List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>();

                if (chDividendYield.Visibility == Visibility.Visible)
                    RadExportOptionsInfo.Add(new RadExportOptions() { ElementName = ExportTypes.Dividend_Yield, Element = this.chDividendYield, ExportFilterOption = RadExportFilterOption.RADCHART_EXCEL_EXPORT_FILTER });

                else if (dgDividendYield.Visibility == Visibility.Visible)
                    RadExportOptionsInfo.Add(new RadExportOptions() { ElementName = ExportTypes.Dividend_Yield_Data, Element = this.dgDividendYield, ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXCEL_EXPORT_FILTER });

                ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + GadgetNames.EXTERNAL_RESEARCH_HISTORICAL_VALUATION_CHART_DividendYield);
                childExportOptions.Show();
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog(ex.Message);
            }
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
                if (this.dgDividendYield.Visibility == Visibility.Visible)
                {
                    Dispatcher.BeginInvoke((Action)(() =>
                    {
                        RichTextBox.Document = PDFExporter.Print(this.dgDividendYield, 6);
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
                if (this.dgDividendYield.Visibility == Visibility.Visible)
                {
                    PDFExporter.btnExportPDF_Click(this.dgDividendYield);
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
            }
        }
        #endregion

        #region Flipping
        /// <summary>
        /// Flipping between Grid & Chart
        /// Using the method FlipItem in class Flipper.cs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFlip_Click(object sender, RoutedEventArgs e)
        {
            if (this.chDividendYield.Visibility == System.Windows.Visibility.Visible)
            {
                Flipper.FlipItem(this.chDividendYield, this.dgDividendYield);
            }
            else
            {
                Flipper.FlipItem(this.dgDividendYield, this.chDividendYield);
            }
        }

        #endregion 
        #endregion

        #region Helper Methods
        /// <summary>
        /// Apply chart styles
        /// </summary>
        private void ApplyChartStyles()
        {
            this.chDividendYield.DefaultView.ChartArea.AxisX.TicksDistance = 50;
            this.chDividendYield.DefaultView.ChartArea.AxisX.AxisStyles.ItemLabelStyle = this.Resources["ItemLabelStyle"] as Style;
            this.chDividendYield.DefaultView.ChartArea.AxisY.AxisStyles.ItemLabelStyle = this.Resources["ItemLabelStyle"] as Style;
        }

        /// <summary>
        /// UnSubscribing the Events
        /// </summary>
        public override void Dispose()
        {
            this.DataContextDividendYield.Dispose();
            this.DataContextDividendYield = null;
            this.DataContext = null;
        }
        #endregion        
    }
}