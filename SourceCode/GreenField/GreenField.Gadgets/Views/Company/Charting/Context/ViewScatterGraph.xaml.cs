using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Telerik.Windows.Controls.Charting;
using Telerik.Windows.Documents.Model;
using GreenField.Common;
using GreenField.DataContracts;
using GreenField.Gadgets.Helpers;
using GreenField.Gadgets.ViewModels;
using GreenField.ServiceCaller;
using GreenField.ServiceCaller.ExternalResearchDefinitions;

namespace GreenField.Gadgets.Views
{
    /// <summary>
    /// Code behind for ViewScatterGraph
    /// </summary>
    public partial class ViewScatterGraph : ViewBaseUserControl
    {
        #region Properties
        /// <summary>
        /// Data context instance of view
        /// </summary>
        private ViewModelScatterGraph dataContextSource;
        public ViewModelScatterGraph DataContextSource
        {
            get { return dataContextSource; }
            set { dataContextSource = value; }
        }

        /// <summary>
        /// Property to set IsActive variable of View Model
        /// </summary>
        private bool isActive;
        public override bool IsActive
        {
            get { return isActive; }
            set
            {
                isActive = value;
                if (DataContextSource != null)
                {
                    DataContextSource.IsActive = isActive;
                }
            }
        } 
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataContextSource">ViewModelScatterGraph</param>
        public ViewScatterGraph(ViewModelScatterGraph dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            DataContextSource = dataContextSource;
        } 
        #endregion

        #region Event Handlers
        /// <summary>
        /// btnExportExcel Click event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<RadExportOptions> radExportOptionsInfo = new List<RadExportOptions>();

                if (this.chScatter.Visibility == Visibility.Visible)
                {
                    radExportOptionsInfo.Add(new RadExportOptions()
                    {
                        ElementName = "Scatter Graph Chart",
                        Element = this.chScatter
                        ,
                        ExportFilterOption = RadExportFilterOption.RADCHART_EXCEL_EXPORT_FILTER
                    });
                }
                else if (this.dgScatterGraph.Visibility == Visibility.Visible)
                {
                    radExportOptionsInfo.Add(new RadExportOptions()
                    {
                        ElementName = "Scatter Graph Data",
                        Element = this.dgScatterGraph
                        ,
                        ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXCEL_EXPORT_FILTER
                    });
                }
                ChildExportOptions childExportOptions = new ChildExportOptions(radExportOptionsInfo, "Export Options: Scatter Graph");
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
                List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>();

                if (chScatter.Visibility == Visibility.Visible)
                {
                    RadExportOptionsInfo.Add(new RadExportOptions()
                    {
                        ElementName = "Scatter Graph Chart",
                        Element = this.chScatter,
                        ExportFilterOption = RadExportFilterOption.RADCHART_PRINT_FILTER,
                        RichTextBox = this.RichTextBox
                    });
                }
                else if (dgScatterGraph.Visibility == Visibility.Visible)
                {
                    RadExportOptionsInfo.Add(new RadExportOptions()
                    {
                        ElementName = "Scatter Graph Data",
                        Element = this.dgScatterGraph,
                        ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_PRINT_FILTER,
                        RichTextBox = this.RichTextBox
                    });
                }
                ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: Scatter Graph");
                childExportOptions.Show();
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
                List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>();

                if (chScatter.Visibility == Visibility.Visible)
                {
                    RadExportOptionsInfo.Add(new RadExportOptions()
                    {
                        ElementName = "Scatter Graph Chart",
                        Element = this.chScatter,
                        ExportFilterOption = RadExportFilterOption.RADCHART_PDF_EXPORT_FILTER,
                        RichTextBox = this.RichTextBox
                    });
                }
                else if (dgScatterGraph.Visibility == Visibility.Visible)
                {
                    RadExportOptionsInfo.Add(new RadExportOptions()
                    {
                        ElementName = "Scatter Graph Data",
                        Element = this.dgScatterGraph,
                        ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_PDF_EXPORT_FILTER,
                        RichTextBox = this.RichTextBox
                    });
                }
                ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: Scatter Graph");
                childExportOptions.Show();
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
            }
        }

        /// <summary>
        /// btnFlip Click event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFlip_Click(object sender, RoutedEventArgs e)
        {
            if (this.dgScatterGraph.Visibility == Visibility.Visible)
            {
                Flipper.FlipItem(this.dgScatterGraph, this.chScatter);
            }
            else
            {
                Flipper.FlipItem(this.chScatter, this.dgScatterGraph);
            }
        }

        /// <summary>
        /// dgScatterGraph ElementExporting event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgScatterGraph_ElementExporting(object sender, Telerik.Windows.Controls.GridViewElementExportingEventArgs e)
        {
            RadGridView_ElementExport.ElementExporting(e);
        }

        /// <summary>
        /// ChartArea ItemToolTipOpening event handler
        /// </summary>
        /// <param name="tooltip"></param>
        /// <param name="e"></param>
        private void ChartArea_ItemToolTipOpening(ItemToolTip2D tooltip, ItemToolTipEventArgs e)
        {
            RatioComparisonData dataPointContext = e.DataPoint.DataItem as RatioComparisonData;
            if (dataPointContext == null)
            {
                tooltip.Content = null;
                return;
            }

            tooltip.Content = dataPointContext.ISSUE_NAME + " ("
                + EnumUtils.GetDescriptionFromEnumValue<ScatterGraphValuationRatio>(DataContextSource.SelectedValuationRatio) 
                + ":" + dataPointContext.VALUATION + ", "
                + EnumUtils.GetDescriptionFromEnumValue<ScatterGraphFinancialRatio>(DataContextSource.SelectedFinancialRatio) 
                + ":" + dataPointContext.FINANCIAL + ")";
        }

        /// <summary>
        /// chScatter DataBound event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chScatter_DataBound(object sender, ChartDataBoundEventArgs e)
        {
            if (DataContextSource.RatioComparisonInfo != null)
            {
                this.chaScatter.Annotations.Clear();

                if (DataContextSource.RatioComparisonInfo.Count() != 0)
                {
                    Decimal? financialRatioTotal = DataContextSource.RatioComparisonInfo.Sum(record => record.FINANCIAL);
                    Decimal? financialRatioAverage = financialRatioTotal / DataContextSource.RatioComparisonInfo.Count();

                    Decimal? valuationRatioTotal = DataContextSource.RatioComparisonInfo.Sum(record => record.VALUATION);
                    Decimal? valuationRatioAverage = valuationRatioTotal / DataContextSource.RatioComparisonInfo.Count();

                    this.chaScatter.Annotations.Add(new CustomGridLine() 
                    {
                        XIntercept = Convert.ToDouble(valuationRatioAverage), 
                        StrokeThickness = 1 
                    });
                    this.chaScatter.Annotations.Add(new CustomGridLine() 
                    {
                        YIntercept = Convert.ToDouble(financialRatioAverage), 
                        StrokeThickness = 1 
                    });
                }
            }
        } 
        #endregion
    }
}
