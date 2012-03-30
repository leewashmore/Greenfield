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
using GreenField.Gadgets.Helpers;
using System.IO;
using GreenField.Common;
using Telerik.Windows.Controls;
using Telerik.Windows.Data;
using Telerik.Windows.Controls.Primitives;

namespace GreenField.Gadgets.Views
{
    public partial class ViewClosingPriceChart : UserControl
    {
        #region Variables

        public DateTime startDate = DateTime.Today.AddYears(-1);
        public DateTime endDate = DateTime.Today;
        public bool checkCustomAlreadySelected = false;
        public string timePeriodSelected = "";

        private static class ExportTypes
        {
            public const string CLOSING_PRICE_CHART = "Closing Price Chart";
            public const string PRICING_DATA = "Closing Price Data";
            public const string VOLUME_CHART = "Volume Chart";
        }

        private ViewModelClosingPriceChart _dataContextClosingPriceChart;

        public ViewModelClosingPriceChart DataContextClosingPriceChart
        {
            get { return _dataContextClosingPriceChart; }
            set { _dataContextClosingPriceChart = value; }
        }
        

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public ViewClosingPriceChart(ViewModelClosingPriceChart DataContextSource)
        {
            InitializeComponent();
            this.DataContext = DataContextSource;
            this.DataContextClosingPriceChart = DataContextSource;
            DataContextSource.closingPriceDataLoadedEvent += new DataRetrievalProgressIndicator(DataContextSource_closingPriceDataLoadedEvent);
            DataContextSource.ChartAreaPricing = this.chPricing.DefaultView.ChartArea;
            this.chPricing.DataBound += DataContextSource.ChartDataBound;
            DataContextSource.ChartAreaVolume = this.chVolume.DefaultView.ChartArea;
            this.chVolume.DataBound += DataContextSource.ChartDataBound;
            ApplyChartStyles();
        }

        #endregion

        /// <summary>
        /// Formatting the chart
        /// </summary>
        private void ApplyChartStyles()
        {
            this.chPricing.DefaultView.ChartArea.AxisX.AxisStyles.ItemLabelStyle = this.Resources["ItemLabelStyle"] as Style;
            this.chPricing.DefaultView.ChartArea.AxisY.AxisStyles.ItemLabelStyle = this.Resources["ItemLabelStyle"] as Style;
            this.chVolume.DefaultView.ChartArea.AxisX.AxisStyles.ItemLabelStyle = this.Resources["ItemLabelStyle"] as Style;
            this.chVolume.DefaultView.ChartArea.AxisY.AxisStyles.ItemLabelStyle = this.Resources["ItemLabelStyle"] as Style;
            this.chVolume.DefaultView.ChartLegend.Header = string.Empty;
            this.chPricing.DefaultView.ChartArea.AxisX.TicksDistance = 50;
            this.chVolume.DefaultView.ChartLegend.Visibility = Visibility.Collapsed;
            this.cmbAddSeries.CanAutocompleteSelectItems = false;
            this.cmbTime.SelectedValue = "1-Year";

        }

        /// <summary>
        /// Data Retrieval Progress Indicator
        /// </summary>
        /// <param name="e"></param>
        void DataContextSource_closingPriceDataLoadedEvent(DataRetrievalProgressIndicatorEventArgs e)
        {
            if (e.ShowBusy)
            {
                this.busyIndicator.IsBusy = true;
                this.busyIndicatorchartVolume.IsBusy = true;
                this.busyIndicatorGrid.IsBusy = true;
            }
            else
            {
                this.busyIndicator.IsBusy = false;
                this.busyIndicatorchartVolume.IsBusy = false;
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
            if (this.grdRadGridView.Visibility == System.Windows.Visibility.Visible)
            {
                Flipper.FlipItem(this.grdRadGridView, this.grdRadChart);
            }
            else
            {
                Flipper.FlipItem(this.grdRadChart, this.grdRadGridView);
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
                List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>
                {
                    new RadExportOptions() { ElementName = ExportTypes.PRICING_DATA, Element = this.dgPricing, ExportFilterOption = RadExportFilterOptions.RADGRIDVIEW_EXPORT_FILTER },
                    new RadExportOptions() { ElementName = ExportTypes.CLOSING_PRICE_CHART, Element = this.chPricing, ExportFilterOption = RadExportFilterOptions.RADCHART_EXPORT_FILTER },
                    new RadExportOptions() { ElementName = ExportTypes.VOLUME_CHART, Element = this.chVolume, ExportFilterOption = RadExportFilterOptions.RADCHART_EXPORT_FILTER },
                    
                };
                ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + GadgetNames.PRICING);
                childExportOptions.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void cmbFrequencyInterval_SelectionChanged(object sender, Telerik.Windows.Controls.SelectionChangedEventArgs e)
        {
            string timeInterval = Convert.ToString(cmbTime.SelectedValue);
            switch (cmbFrequencyInterval.SelectedValue.ToString())
            {
                case ("Daily"):
                    {
                        this.chPricing.DefaultView.ChartArea.AxisX.DefaultLabelFormat = "d";
                        this.chPricing.DefaultView.ChartArea.SmartLabelsEnabled = true;
                        this.chPricing.DefaultView.ChartArea.AxisX.AutoRange = true;
                        this.chPricing.DefaultView.ChartArea.AxisX.Step = 5;
                        this.chPricing.DefaultView.ChartArea.AxisX.LabelStep = 2;

                        this.chVolume.DefaultView.ChartArea.AxisX.DefaultLabelFormat = "d";
                        this.chVolume.DefaultView.ChartArea.SmartLabelsEnabled = true;
                        this.chVolume.DefaultView.ChartArea.AxisX.AutoRange = true;
                        this.chVolume.DefaultView.ChartArea.AxisX.Step = 5;
                        this.chVolume.DefaultView.ChartArea.AxisX.LabelStep = 2;

                        break;
                    }
                case ("Weekly"):
                    {
                        this.chPricing.DefaultView.ChartArea.AxisX.DefaultLabelFormat = "d";
                        this.chPricing.DefaultView.ChartArea.SmartLabelsEnabled = true;
                        this.chPricing.DefaultView.ChartArea.AxisX.AutoRange = true;
                        this.chPricing.DefaultView.ChartArea.AxisX.Step = 7;
                        this.chPricing.DefaultView.ChartArea.AxisX.LabelStep = 2;

                        this.chVolume.DefaultView.ChartArea.AxisX.DefaultLabelFormat = "d";
                        this.chVolume.DefaultView.ChartArea.SmartLabelsEnabled = true;
                        this.chVolume.DefaultView.ChartArea.AxisX.AutoRange = true;
                        this.chVolume.DefaultView.ChartArea.AxisX.Step = 7;
                        this.chVolume.DefaultView.ChartArea.AxisX.LabelStep = 2;

                        break;
                    }
                case ("Monthly"):
                    {
                        this.chPricing.DefaultView.ChartArea.AxisX.DefaultLabelFormat = "m";
                        this.chPricing.DefaultView.ChartArea.SmartLabelsEnabled = true;
                        this.chPricing.DefaultView.ChartArea.AxisX.AutoRange = true;
                        this.chPricing.DefaultView.ChartArea.AxisX.Step = 1;
                        this.chPricing.DefaultView.ChartArea.AxisX.LabelStep = 2;

                        this.chVolume.DefaultView.ChartArea.AxisX.DefaultLabelFormat = "m";
                        this.chVolume.DefaultView.ChartArea.SmartLabelsEnabled = true;
                        this.chVolume.DefaultView.ChartArea.AxisX.AutoRange = true;
                        this.chVolume.DefaultView.ChartArea.AxisX.Step = 1;
                        this.chVolume.DefaultView.ChartArea.AxisX.LabelStep = 2;

                        break;
                    }
                case ("Half-Yearly"):
                    {
                        this.chPricing.DefaultView.ChartArea.AxisX.DefaultLabelFormat = "m";
                        this.chPricing.DefaultView.ChartArea.SmartLabelsEnabled = true;
                        this.chPricing.DefaultView.ChartArea.AxisX.AutoRange = true;
                        this.chPricing.DefaultView.ChartArea.AxisX.Step = 6;
                        this.chPricing.DefaultView.ChartArea.AxisX.LabelStep = 1;

                        this.chVolume.DefaultView.ChartArea.AxisX.DefaultLabelFormat = "m";
                        this.chVolume.DefaultView.ChartArea.SmartLabelsEnabled = true;
                        this.chVolume.DefaultView.ChartArea.AxisX.AutoRange = true;
                        this.chVolume.DefaultView.ChartArea.AxisX.Step = 6;
                        this.chVolume.DefaultView.ChartArea.AxisX.LabelStep = 1;

                        break;
                    }
                case ("Yearly"):
                    {
                        this.chPricing.DefaultView.ChartArea.AxisX.DefaultLabelFormat = "Y";
                        this.chPricing.DefaultView.ChartArea.SmartLabelsEnabled = true;
                        this.chPricing.DefaultView.ChartArea.AxisX.AutoRange = true;
                        this.chPricing.DefaultView.ChartArea.AxisX.Step = 1;
                        this.chPricing.DefaultView.ChartArea.AxisX.LabelStep = 2;

                        this.chVolume.DefaultView.ChartArea.AxisX.DefaultLabelFormat = "Y";
                        this.chVolume.DefaultView.ChartArea.SmartLabelsEnabled = true;
                        this.chVolume.DefaultView.ChartArea.AxisX.AutoRange = true;
                        this.chVolume.DefaultView.ChartArea.AxisX.Step = 1;
                        this.chVolume.DefaultView.ChartArea.AxisX.LabelStep = 2;

                        break;
                    }
                default:
                    {
                        this.chPricing.DefaultView.ChartArea.AxisX.DefaultLabelFormat = "d";
                        this.chPricing.DefaultView.ChartArea.SmartLabelsEnabled = true;
                        this.chVolume.DefaultView.ChartArea.AxisX.DefaultLabelFormat = "d";
                        this.chVolume.DefaultView.ChartArea.SmartLabelsEnabled = true;
                        break;
                    }
            }
        }

        private void ElementExportingEvent(object sender, GridViewElementExportingEventArgs e)
        {
            if (e.Element == ExportElement.HeaderRow || e.Element == ExportElement.FooterRow
                || e.Element == ExportElement.GroupFooterRow)
            {
                e.Background = Colors.Gray;
                e.Foreground = Colors.Black;
                e.FontSize = 20;
                e.FontWeight = FontWeights.Bold;
            }
            else if (e.Element == ExportElement.Row)
            {
                //e.Background = RowBackgroundPicker.SelectedColor;
                //e.Foreground = RowForegroundPicker.SelectedColor;
            }
            else if (e.Element == ExportElement.Cell &&
                e.Value != null && e.Value.Equals("Chocolade"))
            {
                e.FontFamily = new FontFamily("Verdana");
                e.Background = Colors.LightGray;
                e.Foreground = Colors.Blue;
            }
            else if (e.Element == ExportElement.GroupHeaderRow)
            {
                e.FontFamily = new FontFamily("Verdana");
                e.Background = Colors.LightGray;
                e.Height = 30;
            }
            else if (e.Element == ExportElement.GroupHeaderCell &&
                e.Value != null && e.Value.Equals("Chocolade"))
            {
                e.Value = "MyNewValue";
            }
            else if (e.Element == ExportElement.GroupFooterCell)
            {
                GridViewDataColumn column = e.Context as GridViewDataColumn;
                QueryableCollectionViewGroup qcvGroup = e.Value as QueryableCollectionViewGroup;

                if (column != null && qcvGroup != null && column.AggregateFunctions.Count() > 0)
                {
                    e.Value = GetAggregates(qcvGroup, column);
                }
            }
        }

        private string GetAggregates(QueryableCollectionViewGroup group, GridViewDataColumn column)
        {
            List<string> aggregates = new List<string>();

            foreach (AggregateFunction f in column.AggregateFunctions)
            {
                foreach (AggregateResult r in group.AggregateResults)
                {
                    if (f.FunctionName == r.FunctionName && r.FormattedValue != null)
                    {
                        aggregates.Add(r.FormattedValue.ToString());
                    }
                }
            }

            return String.Join(",", aggregates.ToArray());
        }

        /// <summary>
        /// To check if Total Return checkbox is checked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkSearchFilter_Checked(object sender, RoutedEventArgs e)
        {
            this.cmbAddSeries.TextSearchMode = TextSearchMode.StartsWith;
        }

        /// <summary>
        /// To check if Total Return checkbox is Unchecked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkSearchFilter_Unchecked(object sender, RoutedEventArgs e)
        {
            this.cmbAddSeries.TextSearchMode = TextSearchMode.Contains;
        }

        private void cmbTime_DropDownClosed(object sender, EventArgs e)
        {
            this.DataContextClosingPriceChart.SelectedTimeRange = Convert.ToString(cmbTime.SelectedValue);
        }

        private void cmbTime_SelectionChanged(object sender, Telerik.Windows.Controls.SelectionChangedEventArgs e)
        {
            //if (cmbTime.SelectedValue.ToString() == "Custom")
            //{
            //    checkCustomAlreadySelected = true;
            //}
            //else
            //{
            //    checkCustomAlreadySelected = false;
            //}
        }

    }
}