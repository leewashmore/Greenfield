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
using GreenField.Gadgets.ViewModels;
using Telerik.Windows.Controls.Charting;
using GreenField.Gadgets.Helpers;
using GreenField.Common;

namespace GreenField.Gadgets.Views
{
    /// <summary>
    /// View Class of Holdings Pie Chart
    /// </summary>
    public partial class ViewHoldingsPieChart : ViewBaseUserControl
    {
        /// <summary>
        /// Export Types to be passed to the ExportOptions class
        /// </summary>
        private static class ExportTypes
        {
            public const string HOLDINGS_PIE_CHART = "Holdings Pie Chart for Sector";
            public const string HOLDINGS_PIE_GRID = "Holdings Pie Grid for Sector";
        }

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
            //this.dgHoldingsPercentageSector.ItemsSource = ((ViewModelHoldingsPieChart)this.DataContext).HoldingsPercentageInfo;
        }
        #endregion

        private ViewModelHoldingsPieChart _dataContextHoldingsPieChart;
        public ViewModelHoldingsPieChart DataContextHoldingsPieChart
        {
            get { return _dataContextHoldingsPieChart; }
            set { _dataContextHoldingsPieChart = value; }
        }

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
                Flipper.FlipItem(this.dgHoldingsPercentageSector, this.crtHoldingsPercentageSector);
            else
                Flipper.FlipItem(this.crtHoldingsPercentageSector, this.dgHoldingsPercentageSector);
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
                    new RadExportOptions() { ElementName = ExportTypes.HOLDINGS_PIE_GRID, Element = this.dgHoldingsPercentageSector, ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXPORT_FILTER },
                    new RadExportOptions() { ElementName = ExportTypes.HOLDINGS_PIE_CHART, Element = this.crtHoldingsPercentageSector, ExportFilterOption = RadExportFilterOption.RADCHART_EXPORT_FILTER },                    
                    
                };
                ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + GadgetNames.BENCHMARK_HOLDINGS_SECTOR_PIECHART);
                childExportOptions.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

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
