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

namespace GreenField.Gadgets.Views
{
    public partial class ViewUnrealizedGainLoss : UserControl
    {
        private static class ExportTypes
        {
            public const string UNREALIZED_GAINLOSS_CHART = "Unrealized Gain/Loss Chart";
            public const string UNREALIZED_GAINLOSS_DATA = "Unrealized Gain/Loss Data";
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataContextSource"></param>
        public ViewUnrealizedGainLoss(ViewModelUnrealizedGainLoss dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            dataContextSource.unrealizedGainLossDataLoadedEvent +=
                new DataRetrievalProgressIndicatorEventHandler(dataContextSource_unrealizedGainLossDataLoadedEvent);
            dataContextSource.ChartArea = this.chUnrealizedGainLoss.DefaultView.ChartArea;
            this.chUnrealizedGainLoss.DataBound += dataContextSource.ChartDataBound;
            this.grdRadChart.Visibility = Visibility.Visible;
            this.grdRadGridView.Visibility = Visibility.Collapsed;
            ApplyChartStyles();
        }

        /// <summary>
        /// Formatting the chart
        /// </summary>
        private void ApplyChartStyles()
        {
            this.chUnrealizedGainLoss.DefaultView.ChartArea.AxisX.AxisStyles.ItemLabelStyle = this.Resources["ItemLabelStyle"] as Style;
            this.chUnrealizedGainLoss.DefaultView.ChartArea.AxisY.AxisStyles.ItemLabelStyle = this.Resources["ItemLabelStyle"] as Style; 
           
        }

        /// <summary>
        /// Data Retrieval Indicator
        /// </summary>
        /// <param name="e"></param>
        void dataContextSource_unrealizedGainLossDataLoadedEvent(DataRetrievalProgressIndicatorEventArgs e)
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
            if (this.grdRadGridView.Visibility == Visibility.Visible)
                Flipper.FlipItem(this.grdRadGridView, this.grdRadChart);
            else
                Flipper.FlipItem(this.grdRadChart, this.grdRadGridView);
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
                    new RadExportOptions() { ElementName = ExportTypes.UNREALIZED_GAINLOSS_DATA, Element = this.dgUnrealizedGainLoss, ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXPORT_FILTER },
                    new RadExportOptions() { ElementName = ExportTypes.UNREALIZED_GAINLOSS_CHART, Element = this.chUnrealizedGainLoss, ExportFilterOption = RadExportFilterOption.RADCHART_EXPORT_FILTER },                    
                    
                };
                ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + GadgetNames.UNREALIZED_GAINLOSS);
                childExportOptions.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void cmbFrequencyInterval_SelectionChanged(object sender, Telerik.Windows.Controls.SelectionChangedEventArgs e)
        {
            switch (cmbFrequencyInterval.SelectedValue.ToString())
            {
                case ("Daily"):
                    {
                        this.chUnrealizedGainLoss.DefaultView.ChartArea.AxisX.DefaultLabelFormat = "d";
                        this.chUnrealizedGainLoss.DefaultView.ChartArea.SmartLabelsEnabled = true;
                        this.chUnrealizedGainLoss.DefaultView.ChartArea.AxisX.AutoRange = true;
                        this.chUnrealizedGainLoss.DefaultView.ChartArea.AxisX.Step = 5;
                        this.chUnrealizedGainLoss.DefaultView.ChartArea.AxisX.LabelStep = 2;
                        break;
                    }
                case ("Weekly"):
                    {
                        this.chUnrealizedGainLoss.DefaultView.ChartArea.AxisX.DefaultLabelFormat = "d";
                        this.chUnrealizedGainLoss.DefaultView.ChartArea.SmartLabelsEnabled = true;
                        this.chUnrealizedGainLoss.DefaultView.ChartArea.AxisX.AutoRange = true;
                        this.chUnrealizedGainLoss.DefaultView.ChartArea.AxisX.Step = 7;
                        this.chUnrealizedGainLoss.DefaultView.ChartArea.AxisX.LabelStep = 2;

                        break;
                    }
                case ("Monthly"):
                    {
                        this.chUnrealizedGainLoss.DefaultView.ChartArea.AxisX.DefaultLabelFormat = "m";
                        this.chUnrealizedGainLoss.DefaultView.ChartArea.SmartLabelsEnabled = true;
                        this.chUnrealizedGainLoss.DefaultView.ChartArea.AxisX.AutoRange = true;
                        this.chUnrealizedGainLoss.DefaultView.ChartArea.AxisX.Step = 1;
                        this.chUnrealizedGainLoss.DefaultView.ChartArea.AxisX.LabelStep = 2;
                        break;
                    }
                case ("Yearly"):
                    {
                        this.chUnrealizedGainLoss.DefaultView.ChartArea.AxisX.DefaultLabelFormat = "Y";
                        this.chUnrealizedGainLoss.DefaultView.ChartArea.SmartLabelsEnabled = true;
                        this.chUnrealizedGainLoss.DefaultView.ChartArea.AxisX.AutoRange = true;
                        this.chUnrealizedGainLoss.DefaultView.ChartArea.AxisX.Step = 1;
                        this.chUnrealizedGainLoss.DefaultView.ChartArea.AxisX.LabelStep = 2;
                        break;
                    }
                default:
                    {
                        this.chUnrealizedGainLoss.DefaultView.ChartArea.AxisX.DefaultLabelFormat = "d";
                        this.chUnrealizedGainLoss.DefaultView.ChartArea.SmartLabelsEnabled = true;
                        this.chUnrealizedGainLoss.DefaultView.ChartArea.AxisX.Step = 1;
                        this.chUnrealizedGainLoss.DefaultView.ChartArea.AxisX.LabelStep = 2;
                        break;
                    }
            }
        }

        private void dgUnrealizedGainLoss_ElementExporting(object sender, Telerik.Windows.Controls.GridViewElementExportingEventArgs e)
        {
            RadGridView_ElementExport.ElementExporting(e);
        }
    }
}
