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
using Telerik.Windows.Controls.Charting;
using GreenField.Gadgets.Helpers;
using GreenField.Common;
using GreenField.ServiceCaller;
using GreenField.Gadgets.ViewModels;

namespace GreenField.Gadgets.Views
{
    /// <summary>
    /// View Class of Holdings Pie Chart
    /// </summary>
    public partial class ViewHoldingsPieChart : ViewBaseUserControl
    {
        #region Private Fields
        /// <summary>
        /// Export Types to be passed to the ExportOptions class
        /// </summary>
        private static class ExportTypes
        {
            public const string HOLDINGS_PIE_CHART = "Holdings Pie Chart for Sector";
            public const string HOLDINGS_PIE_GRID = "Holdings Pie Grid for Sector";
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataContextSource">ViewModelHoldingsPieChart as the data context</param>
        public ViewHoldingsPieChart(ViewModelHoldingsPieChart dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextHoldingsPieChart = dataContextSource;
            dataContextSource.holdingsPieChartDataLoadedEvent +=
            new DataRetrievalProgressIndicatorEventHandler(dataContextSource_holdingsPieChartDataLoadedEvent);
            this.crtHoldingsPercentageSector.Visibility = Visibility.Visible;
            this.dgHoldingsPercentageSector.Visibility = Visibility.Collapsed;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Data Context Property
        /// </summary>
        private ViewModelHoldingsPieChart dataContextHoldingsPieChart;
        public ViewModelHoldingsPieChart DataContextHoldingsPieChart
        {
            get { return dataContextHoldingsPieChart; }
            set { dataContextHoldingsPieChart = value; }
        }

        /// <summary>
        /// True is gadget is currently on display
        /// </summary>
        private bool isActive;
        public override bool IsActive
        {
            get { return isActive; }
            set
            {
                isActive = value;
                if (DataContextHoldingsPieChart != null)
                {
                    DataContextHoldingsPieChart.IsActive = isActive;
                }
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Data Retrieval Indicator
        /// </summary>
        /// <param name="e"></param>
        void dataContextSource_holdingsPieChartDataLoadedEvent(DataRetrievalProgressIndicatorEventArgs e)
        {
            if (e.ShowBusy)
            {
                this.busyIndicatorChart.IsBusy = true;
                this.busyIndicatorGrid.IsBusy = true;
            }
            else
            {
                this.busyIndicatorChart.IsBusy = false;
                this.busyIndicatorGrid.IsBusy = false;
            }
        }

        /// <summary>
        /// Flipping between Grid & Chart
        /// Using the method FlipItem in class Flipper.cs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFlip_Click(object sender, RoutedEventArgs e)
        {
            if (this.dgHoldingsPercentageSector.Visibility == Visibility.Visible)
            {
                Flipper.FlipItem(this.dgHoldingsPercentageSector, this.crtHoldingsPercentageSector);
            }
            else
            {
                Flipper.FlipItem(this.crtHoldingsPercentageSector, this.dgHoldingsPercentageSector);
            }
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
                if (this.crtHoldingsPercentageSector.Visibility == Visibility.Visible)
                {
                    List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>
                {                   
                    new RadExportOptions() { ElementName = ExportTypes.HOLDINGS_PIE_CHART, Element = this.crtHoldingsPercentageSector, ExportFilterOption = RadExportFilterOption.RADCHART_EXCEL_EXPORT_FILTER },                    
                    
                };
                    ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + GadgetNames.BENCHMARK_HOLDINGS_SECTOR_PIECHART);
                    childExportOptions.Show();
                }
                else
                {
                    if (this.dgHoldingsPercentageSector.Visibility == Visibility.Visible)
                    {
                        List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>
                        {
                            new RadExportOptions() { ElementName = ExportTypes.HOLDINGS_PIE_GRID, Element = this.dgHoldingsPercentageSector, ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXCEL_EXPORT_FILTER }
                        };
                        ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + GadgetNames.BENCHMARK_HOLDINGS_SECTOR_PIECHART);
                        childExportOptions.Show();
                    }
                }
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
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            try
            {
                List<RadExportOptions> radExportOptionsInfo = new List<RadExportOptions>();
                if (this.crtHoldingsPercentageSector.Visibility == Visibility.Visible)
                {
                    radExportOptionsInfo.Add(new RadExportOptions()
                    {
                        ElementName = ExportTypes.HOLDINGS_PIE_CHART,
                        Element = this.crtHoldingsPercentageSector,
                        ExportFilterOption = RadExportFilterOption.RADCHART_PRINT_FILTER,
                        RichTextBox = this.RichTextBox
                    });
                }
                else if (this.dgHoldingsPercentageSector.Visibility == Visibility.Visible)
                {
                    radExportOptionsInfo.Add(new RadExportOptions()
                    {
                        ElementName = ExportTypes.HOLDINGS_PIE_GRID,
                        Element = this.dgHoldingsPercentageSector,
                        ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_PRINT_FILTER,
                        RichTextBox = this.RichTextBox
                    });
                }

                ChildExportOptions childExportOptions = new ChildExportOptions(radExportOptionsInfo, "Export Options: "
                    + GadgetNames.BENCHMARK_HOLDINGS_REGION_PIECHART);
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
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            try
            {
                List<RadExportOptions> radExportOptionsInfo = new List<RadExportOptions>();
                if (this.crtHoldingsPercentageSector.Visibility == Visibility.Visible)
                {
                    radExportOptionsInfo.Add(new RadExportOptions()
                    {
                        ElementName = ExportTypes.HOLDINGS_PIE_CHART,
                        Element = this.crtHoldingsPercentageSector,
                        ExportFilterOption = RadExportFilterOption.RADCHART_PDF_EXPORT_FILTER,
                        RichTextBox = this.RichTextBox
                    });
                }
                else if (this.dgHoldingsPercentageSector.Visibility == Visibility.Visible)
                {
                    radExportOptionsInfo.Add(new RadExportOptions()
                    {
                        ElementName = ExportTypes.HOLDINGS_PIE_GRID,
                        Element = this.dgHoldingsPercentageSector,
                        ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_PDF_EXPORT_FILTER,
                        RichTextBox = this.RichTextBox
                    });
                }

                ChildExportOptions childExportOptions = new ChildExportOptions(radExportOptionsInfo, "Export Options: "
                    + GadgetNames.BENCHMARK_HOLDINGS_REGION_PIECHART);
                childExportOptions.Show();
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
            }
        }

        /// <summary>
        /// dgHoldingsPercentageSector ElementExporting event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgHoldingsPercentageSector_ElementExporting(object sender, Telerik.Windows.Controls.GridViewElementExportingEventArgs e)
        {
            RadGridView_ElementExport.ElementExporting(e);
        }
        #endregion

        /// <summary>
        /// Dispose
        /// </summary>
        #region RemoveEvents
        public override void Dispose()
        {
            this.DataContextHoldingsPieChart.holdingsPieChartDataLoadedEvent -= new DataRetrievalProgressIndicatorEventHandler(dataContextSource_holdingsPieChartDataLoadedEvent);
            this.DataContextHoldingsPieChart.Dispose();
            this.DataContextHoldingsPieChart = null;
            this.DataContext = null;
        }
        #endregion

        
    }
}
