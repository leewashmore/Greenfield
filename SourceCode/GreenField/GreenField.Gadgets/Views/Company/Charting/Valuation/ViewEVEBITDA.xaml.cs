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
    /// Code behind for ViewEVEBITDA
    /// </summary>
    public partial class ViewEVEBITDA : ViewBaseUserControl
    {
        #region Fields
        /// <summary>
        /// Export Types
        /// </summary>
        private static class ExportTypes
        {
            public const string EV_EBITDA = "EV/EBITDA";
            public const string EV_EBITDA_DATA = "EV/EBITDA Data";
        }
        #endregion

        #region Properties
        /// <summary>
        /// Property of ViewModel type
        /// </summary>
        private ViewModelEVEBITDA dataContextEVEBITDA;
        public ViewModelEVEBITDA DataContextEVEBITDA
        {
            get
            {
                return dataContextEVEBITDA;
            }
            set
            {
                dataContextEVEBITDA = value;
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
                if (DataContextEVEBITDA != null) //DataContext instance
                    DataContextEVEBITDA.IsActive = isActive;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataContextSource">ViewModelEVEBITDA</param>
        public ViewEVEBITDA(ViewModelEVEBITDA dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextEVEBITDA = dataContextSource;
            dataContextSource.ChartArea = this.chEVEBITDA.DefaultView.ChartArea;
            this.chEVEBITDA.DataBound += dataContextSource.ChartDataBound;
            this.ApplyChartStyles();
        } 
        #endregion

        #region Event Handlers
        #region Data Load
        /// <summary>
        /// chEVEBITDA Loaded event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chEVEBITDA_Loaded(object sender, RoutedEventArgs e)
        {
            if (chEVEBITDA.DefaultView.ChartLegend.Items.Count != 0)
            {
                ChartLegendItem var = this.chEVEBITDA.DefaultView.ChartLegend.Items[0];
                this.chEVEBITDA.DefaultView.ChartLegend.Items.Remove(var);
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

                if (chEVEBITDA.Visibility == Visibility.Visible)
                    RadExportOptionsInfo.Add(new RadExportOptions() { ElementName = ExportTypes.EV_EBITDA, Element = this.chEVEBITDA, ExportFilterOption = RadExportFilterOption.RADCHART_EXCEL_EXPORT_FILTER });

                else if (dgEVEBITDA.Visibility == Visibility.Visible)
                    RadExportOptionsInfo.Add(new RadExportOptions() { ElementName = ExportTypes.EV_EBITDA_DATA, Element = this.dgEVEBITDA, ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXCEL_EXPORT_FILTER });

                ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + GadgetNames.EXTERNAL_RESEARCH_HISTORICAL_VALUATION_CHART_EVEBITDA);
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
                if (this.dgEVEBITDA.Visibility == Visibility.Visible)
                {
                    Dispatcher.BeginInvoke((Action)(() =>
                    {
                        RichTextBox.Document = PDFExporter.Print(this.dgEVEBITDA, 6);
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
                if (this.dgEVEBITDA.Visibility == Visibility.Visible)
                {
                    PDFExporter.btnExportPDF_Click(this.dgEVEBITDA);
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
            if (this.chEVEBITDA.Visibility == System.Windows.Visibility.Visible)
            {
                Flipper.FlipItem(this.chEVEBITDA, this.dgEVEBITDA);
            }
            else
            {
                Flipper.FlipItem(this.dgEVEBITDA, this.chEVEBITDA);
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
            this.chEVEBITDA.DefaultView.ChartArea.AxisX.TicksDistance = 50;
            this.chEVEBITDA.DefaultView.ChartArea.AxisX.AxisStyles.ItemLabelStyle = this.Resources["ItemLabelStyle"] as Style;
            this.chEVEBITDA.DefaultView.ChartArea.AxisY.AxisStyles.ItemLabelStyle = this.Resources["ItemLabelStyle"] as Style;
        }

        /// <summary>
        /// UnSubscribing the Events
        /// </summary>
        public override void Dispose()
        {
            this.DataContextEVEBITDA.Dispose();
            this.DataContextEVEBITDA = null;
            this.DataContext = null;
        }
        #endregion        
    }
}
