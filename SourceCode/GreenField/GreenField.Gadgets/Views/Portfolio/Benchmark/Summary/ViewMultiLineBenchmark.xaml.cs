using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Telerik.Windows.Controls;
using GreenField.ServiceCaller;
using GreenField.Gadgets.ViewModels;
using GreenField.Gadgets.Helpers;
using GreenField.Common;

namespace GreenField.Gadgets.Views
{
    /// <summary>
    /// Code-Behind for Multi-LineBenchmark UI
    /// </summary>
    public partial class ViewMultiLineBenchmark : ViewBaseUserControl
    {
        #region Variables

        /// <summary>
        /// Export Types
        /// </summary>
        private static class ExportTypes
        {
            public const string MULTI_LINE_CHART = "Multi-Line Benchmark Chart";
            public const string MULTI_LINE_GRID = "Multi- Line Benchmark Grid";
        }

        #endregion

        #region PropertyDeclaration

        /// <summary>
        /// Property of type View-Model
        /// </summary>
        private ViewModelMultiLineBenchmark dataContextMultilineBenchmark;
        public ViewModelMultiLineBenchmark DataContextMultilineBenchmark
        {
            get
            {
                return dataContextMultilineBenchmark;
            }
            set
            {
                dataContextMultilineBenchmark = value;
            }
        }

        /// <summary>
        /// To check whether the Dashboard is Active or not
        /// </summary>
        private bool isActive;
        public override bool IsActive
        {
            get { return isActive; }
            set
            {
                isActive = value;
                if (DataContextMultilineBenchmark != null)
                {
                    DataContextMultilineBenchmark.IsActive = isActive;
                }
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor that initialises the class
        /// </summary>
        /// <param name="dataContextSource"></param>
        public ViewMultiLineBenchmark(ViewModelMultiLineBenchmark dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextMultilineBenchmark = dataContextSource;
            dataContextSource.ChartAreaMultiLineBenchmark = this.chMultiLineBenchmarkChart.DefaultView.ChartArea;
            this.chMultiLineBenchmarkChart.DataBound += dataContextSource.ChartDataBound;
            this.AddGridHeader();
            this.ApplyChartStyles();
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
                List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>
                {
                    new RadExportOptions() { ElementName = ExportTypes.MULTI_LINE_CHART, Element = this.chMultiLineBenchmarkChart, 
                        ExportFilterOption = RadExportFilterOption.RADCHART_EXCEL_EXPORT_FILTER },
                    new RadExportOptions() { ElementName = ExportTypes.MULTI_LINE_GRID, Element = this.dgBenchmarkUI,
                        ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXCEL_EXPORT_FILTER },
                };
                ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + GadgetNames.SECURITY_REFERENCE_PRICE_COMPARISON);
                childExportOptions.Show();                
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog(ex.Message);
            }
        }

        #endregion

        #region EventsUnSubscribe

        /// <summary>
        /// Disposing off events and Event Handlers
        /// </summary>
        public override void Dispose()
        {
            this.DataContextMultilineBenchmark.Dispose();
        }

        #endregion

        #region HelperMethods

        /// <summary>
        /// Dynamically setting GridColumn Headers
        /// </summary>
        private void AddGridHeader()
        {
            dgBenchmarkUI.Columns[4].Header = DateTime.Today.Year.ToString();
            dgBenchmarkUI.Columns[5].Header = DateTime.Today.AddYears(-1).Year.ToString();
            dgBenchmarkUI.Columns[6].Header = DateTime.Today.AddYears(-2).Year.ToString();

        }

        /// <summary>
        /// Applying Chart Styles
        /// </summary>
        private void ApplyChartStyles()
        {
            this.chMultiLineBenchmarkChart.DefaultView.ChartArea.AxisX.AxisStyles.ItemLabelStyle = this.Resources["ItemLabelStyle"] as Style;
            this.chMultiLineBenchmarkChart.DefaultView.ChartArea.AxisY.AxisStyles.ItemLabelStyle = this.Resources["ItemLabelStyle"] as Style;
            this.chMultiLineBenchmarkChart.DefaultView.ChartLegend.Style = this.Resources["ChartLegendStyle"] as Style;
            this.chMultiLineBenchmarkChart.DefaultView.ChartLegend.Header = string.Empty;
            this.chMultiLineBenchmarkChart.DefaultView.ChartArea.AxisX.TicksDistance = 50;
        }

        /// <summary>
        /// Data-Bound Event of the Chart
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chMultiLineBenchmarkChart_DataBound(object sender, Telerik.Windows.Controls.Charting.ChartDataBoundEventArgs e)
        {
            if (this.DataContext as ViewModelMultiLineBenchmark != null)
            {
                if ((this.DataContext as ViewModelMultiLineBenchmark).MultiLineBenchmarkUIChartData != null)
                {
                    (this.DataContext as ViewModelMultiLineBenchmark).AxisXMinValue =
                        Convert.ToDateTime(((this.DataContext as ViewModelMultiLineBenchmark).MultiLineBenchmarkUIChartData.OrderBy(a => a.FromDate)).
                        Select(a => a.FromDate).FirstOrDefault()).ToOADate();
                    (this.DataContext as ViewModelMultiLineBenchmark).AxisXMaxValue =
                        Convert.ToDateTime(((this.DataContext as ViewModelMultiLineBenchmark).MultiLineBenchmarkUIChartData.OrderByDescending(a => a.FromDate)).
                        Select(a => a.FromDate).FirstOrDefault()).ToOADate();
                    int dataCount = (this.DataContext as ViewModelMultiLineBenchmark).MultiLineBenchmarkUIChartData.Count;
                    if (dataCount != 0)
                    {
                        this.chMultiLineBenchmarkChart.DefaultView.ChartArea.AxisX.Step = dataCount / 10;
                    }
                }
            }
        }

        #endregion
    
    }
}
