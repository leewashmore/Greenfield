using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
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
    /// Code behind for ViewPCE
    /// </summary>
    public partial class ViewPCE : ViewBaseUserControl
    {
        #region Fields
        /// <summary>
        /// Export Types
        /// </summary>
        private static class ExportTypes
        {
            public const string P_CE = "P/CE";
            public const string P_CE_DATA = "P/CE Data";
        }
        #endregion

        #region Properties
        /// <summary>
        /// Property of ViewModel type
        /// </summary>
        private ViewModelPCE dataContextPCE;
        public ViewModelPCE DataContextPCE
        {
            get
            {
                return dataContextPCE;
            }
            set
            {
                dataContextPCE = value;
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
                if (DataContextPCE != null) //DataContext instance
                    DataContextPCE.IsActive = isActive;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataContextSource">ViewModelPCE</param>
        public ViewPCE(ViewModelPCE dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextPCE = dataContextSource;
            dataContextSource.ChartArea = this.chPCE.DefaultView.ChartArea;
            this.chPCE.DataBound += dataContextSource.ChartDataBound;
            this.ApplyChartStyles();
        }
        #endregion

        #region Event Handlers
        #region Data Load
        /// <summary>
        /// Chart laoded event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chPCE_Loaded(object sender, RoutedEventArgs e)
        {
            if (chPCE.DefaultView.ChartLegend.Items.Count != 0)
            {
                ChartLegendItem var = this.chPCE.DefaultView.ChartLegend.Items[0];
                this.chPCE.DefaultView.ChartLegend.Items.Remove(var);
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

                if (chPCE.Visibility == Visibility.Visible)
                    RadExportOptionsInfo.Add(new RadExportOptions() { ElementName = ExportTypes.P_CE, Element = this.chPCE, ExportFilterOption = RadExportFilterOption.RADCHART_EXCEL_EXPORT_FILTER });

                else if (dgPCE.Visibility == Visibility.Visible)
                    RadExportOptionsInfo.Add(new RadExportOptions() { ElementName = ExportTypes.P_CE_DATA, Element = this.dgPCE, ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXCEL_EXPORT_FILTER });

                ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + GadgetNames.EXTERNAL_RESEARCH_HISTORICAL_VALUATION_CHART_PCE);
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
                if (this.dgPCE.Visibility == Visibility.Visible)
                {
                    Dispatcher.BeginInvoke((Action)(() =>
                    {
                        RichTextBox.Document = PDFExporter.Print(this.dgPCE, 6);
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
                if (this.dgPCE.Visibility == Visibility.Visible)
                {
                    PDFExporter.btnExportPDF_Click(this.dgPCE);
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
            if (this.chPCE.Visibility == System.Windows.Visibility.Visible)
            {
                Flipper.FlipItem(this.chPCE, this.dgPCE);
            }
            else
            {
                Flipper.FlipItem(this.dgPCE, this.chPCE);
            }
        }

        #endregion
        #endregion

        #region Helper Methods
        /// <summary>
        /// Apply Chart Styles
        /// </summary>
        private void ApplyChartStyles()
        {
            this.chPCE.DefaultView.ChartArea.AxisX.TicksDistance = 50;
            this.chPCE.DefaultView.ChartArea.AxisX.AxisStyles.ItemLabelStyle = this.Resources["ItemLabelStyle"] as Style;
            this.chPCE.DefaultView.ChartArea.AxisY.AxisStyles.ItemLabelStyle = this.Resources["ItemLabelStyle"] as Style;
        }
        
        /// <summary>
        /// UnSubscribing the Events
        /// </summary>
        public override void Dispose()
        {
            this.DataContextPCE.Dispose();
            this.DataContextPCE = null;
            this.DataContext = null;
        }
        #endregion
    }
}