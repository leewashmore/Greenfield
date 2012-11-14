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
    /// Code behind for ViewPRevenue
    /// </summary>
    public partial class ViewPRevenue : ViewBaseUserControl
    {
        #region Fields
        /// <summary>
        /// Export Types
        /// </summary>
        private static class ExportTypes
        {
            public const string P_Revenue = "P/Revenue";
            public const string P_Revenue_DATA = "P/Revenue Data";
        }
        #endregion

        #region Properties
        /// <summary>
        /// Property of ViewModel type
        /// </summary>
        private ViewModelPRevenue dataContextPRevenue;
        public ViewModelPRevenue DataContextPRevenue
        {
            get
            {
                return dataContextPRevenue;
            }
            set
            {
                dataContextPRevenue = value;
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
                if (DataContextPRevenue != null) //DataContext instance
                    DataContextPRevenue.IsActive = isActive;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataContextSource">ViewModelPRevenue</param>
        public ViewPRevenue(ViewModelPRevenue dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextPRevenue = dataContextSource;
            dataContextSource.ChartArea = this.chPRevenue.DefaultView.ChartArea;
            this.chPRevenue.DataBound += dataContextSource.ChartDataBound;
            this.ApplyChartStyles();            
        }
        #endregion

        #region Event Handlers
        #region Data Load
        /// <summary>
        /// chPRevenue Loaded event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chPRevenue_Loaded(object sender, RoutedEventArgs e)
        {

            if (chPRevenue.DefaultView.ChartLegend.Items.Count != 0)
            {
                ChartLegendItem var = this.chPRevenue.DefaultView.ChartLegend.Items[0];
                this.chPRevenue.DefaultView.ChartLegend.Items.Remove(var);
            }
        }
        #endregion

        #region Export
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

                if (chPRevenue.Visibility == Visibility.Visible)
                    RadExportOptionsInfo.Add(new RadExportOptions() { ElementName = ExportTypes.P_Revenue, Element = this.chPRevenue, ExportFilterOption = RadExportFilterOption.RADCHART_EXCEL_EXPORT_FILTER });

                else if (dgPRevenue.Visibility == Visibility.Visible)
                    RadExportOptionsInfo.Add(new RadExportOptions() { ElementName = ExportTypes.P_Revenue_DATA, Element = this.dgPRevenue, ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXCEL_EXPORT_FILTER });

                ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + GadgetNames.EXTERNAL_RESEARCH_HISTORICAL_VALUATION_CHART_PREVENUE);
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
                if (this.dgPRevenue.Visibility == Visibility.Visible)
                {
                    Dispatcher.BeginInvoke((Action)(() =>
                    {
                        RichTextBox.Document = PDFExporter.Print(this.dgPRevenue, 6);
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
                if (this.dgPRevenue.Visibility == Visibility.Visible)
                {
                    PDFExporter.btnExportPDF_Click(this.dgPRevenue);
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
            if (this.chPRevenue.Visibility == System.Windows.Visibility.Visible)
            {
                Flipper.FlipItem(this.chPRevenue, this.dgPRevenue);
            }
            else
            {
                Flipper.FlipItem(this.dgPRevenue, this.chPRevenue);
            }
        }

        #endregion 
        #endregion

        #region Helper Methods
        private void ApplyChartStyles()
        {
            this.chPRevenue.DefaultView.ChartArea.AxisX.TicksDistance = 50;
            this.chPRevenue.DefaultView.ChartArea.AxisX.AxisStyles.ItemLabelStyle = this.Resources["ItemLabelStyle"] as Style;
            this.chPRevenue.DefaultView.ChartArea.AxisY.AxisStyles.ItemLabelStyle = this.Resources["ItemLabelStyle"] as Style;
        }
        
        /// <summary>
        /// UnSubscribing the Events
        /// </summary>
        public override void Dispose()
        {
            this.DataContextPRevenue.Dispose();
            this.DataContextPRevenue = null;
            this.DataContext = null;
        }
        #endregion        
    }
}