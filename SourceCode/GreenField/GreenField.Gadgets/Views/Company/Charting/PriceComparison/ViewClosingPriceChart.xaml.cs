﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Telerik.Windows.Controls;
using Telerik.Windows.Data;
using Telerik.Windows.Documents.Model;
using GreenField.Common;
using GreenField.Gadgets.Helpers;
using GreenField.Gadgets.ViewModels;
using GreenField.ServiceCaller;

namespace GreenField.Gadgets.Views
{
    /// <summary>
    /// XAML.CS class for ClosingPriceGadget
    /// </summary>
    public partial class ViewClosingPriceChart : ViewBaseUserControl
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

        private ViewModelClosingPriceChart dataContextClosingPriceChart;
        public ViewModelClosingPriceChart DataContextClosingPriceChart
        {
            get { return dataContextClosingPriceChart; }
            set { dataContextClosingPriceChart = value; }
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
                if (DataContextClosingPriceChart != null)
                    DataContextClosingPriceChart.IsActive = isActive;
            }
        }


        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public ViewClosingPriceChart(ViewModelClosingPriceChart dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextClosingPriceChart = dataContextSource;
            dataContextSource.ChartAreaPricing = this.chPricing.DefaultView.ChartArea;
            this.chPricing.DataBound += dataContextSource.ChartDataBound;
            dataContextSource.ChartAreaVolume = this.chVolume.DefaultView.ChartArea;
            this.chVolume.DataBound += dataContextSource.ChartDataBound;
            ApplyChartStyles();
        }

        #endregion

        #region Styling

        /// <summary>
        /// Formatting the chart
        /// </summary>
        private void ApplyChartStyles()
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(this.DataContextClosingPriceChart.logger, methodNamespace);
            try
            {
                this.chPricing.DefaultView.ChartArea.AxisX.AxisStyles.ItemLabelStyle = this.Resources["ItemLabelStyle"] as Style;
                this.chPricing.DefaultView.ChartArea.AxisY.AxisStyles.ItemLabelStyle = this.Resources["ItemLabelStyle"] as Style;
                this.chVolume.DefaultView.ChartArea.AxisX.AxisStyles.ItemLabelStyle = this.Resources["ItemLabelStyle"] as Style;
                this.chVolume.DefaultView.ChartArea.AxisY.AxisStyles.ItemLabelStyle = this.Resources["ItemLabelStyle"] as Style;
                this.chPricing.DefaultView.ChartLegend.Style = this.Resources["ChartLegendStyle"] as Style;
                this.chVolume.DefaultView.ChartLegend.Header = string.Empty;
                this.chPricing.DefaultView.ChartArea.AxisX.TicksDistance = 50;
                this.chVolume.DefaultView.ChartLegend.Visibility = Visibility.Collapsed;
                //this.cmbAddSeries.CanAutocompleteSelectItems = false;
                this.cmbTime.SelectedValue = "1-Year";
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(this.DataContextClosingPriceChart.logger, ex);
            }
        }


        /// <summary>
        /// Frequency Interval Changed Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                        this.chPricing.DefaultView.ChartArea.AxisX.LabelStep = 3;

                        this.chVolume.DefaultView.ChartArea.AxisX.DefaultLabelFormat = "d";
                        this.chVolume.DefaultView.ChartArea.SmartLabelsEnabled = true;
                        this.chVolume.DefaultView.ChartArea.AxisX.AutoRange = true;
                        this.chVolume.DefaultView.ChartArea.AxisX.Step = 5;
                        this.chVolume.DefaultView.ChartArea.AxisX.LabelStep = 3;

                        break;
                    }
                case ("Weekly"):
                    {
                        this.chPricing.DefaultView.ChartArea.AxisX.DefaultLabelFormat = "d";
                        this.chPricing.DefaultView.ChartArea.SmartLabelsEnabled = true;
                        this.chPricing.DefaultView.ChartArea.AxisX.AutoRange = true;
                        this.chPricing.DefaultView.ChartArea.AxisX.Step = 7;
                        this.chPricing.DefaultView.ChartArea.AxisX.LabelStep = 3;

                        this.chVolume.DefaultView.ChartArea.AxisX.DefaultLabelFormat = "d";
                        this.chVolume.DefaultView.ChartArea.SmartLabelsEnabled = true;
                        this.chVolume.DefaultView.ChartArea.AxisX.AutoRange = true;
                        this.chVolume.DefaultView.ChartArea.AxisX.Step = 7;
                        this.chVolume.DefaultView.ChartArea.AxisX.LabelStep = 3;

                        break;
                    }
                case ("Monthly"):
                    {
                        this.chPricing.DefaultView.ChartArea.AxisX.DefaultLabelFormat = "Y";
                        this.chPricing.DefaultView.ChartArea.SmartLabelsEnabled = true;
                        this.chPricing.DefaultView.ChartArea.AxisX.AutoRange = true;
                        this.chPricing.DefaultView.ChartArea.AxisX.Step = 1;
                        this.chPricing.DefaultView.ChartArea.AxisX.LabelStep = 3;

                        this.chVolume.DefaultView.ChartArea.AxisX.DefaultLabelFormat = "Y";
                        this.chVolume.DefaultView.ChartArea.SmartLabelsEnabled = true;
                        this.chVolume.DefaultView.ChartArea.AxisX.AutoRange = true;
                        this.chVolume.DefaultView.ChartArea.AxisX.Step = 1;
                        this.chVolume.DefaultView.ChartArea.AxisX.LabelStep = 3;

                        break;
                    }
                case ("Half-Yearly"):
                    {
                        this.chPricing.DefaultView.ChartArea.AxisX.DefaultLabelFormat = "Y";
                        this.chPricing.DefaultView.ChartArea.SmartLabelsEnabled = true;
                        this.chPricing.DefaultView.ChartArea.AxisX.AutoRange = true;
                        this.chPricing.DefaultView.ChartArea.AxisX.Step = 6;
                        this.chPricing.DefaultView.ChartArea.AxisX.LabelStep = 3;

                        this.chVolume.DefaultView.ChartArea.AxisX.DefaultLabelFormat = "Y";
                        this.chVolume.DefaultView.ChartArea.SmartLabelsEnabled = true;
                        this.chVolume.DefaultView.ChartArea.AxisX.AutoRange = true;
                        this.chVolume.DefaultView.ChartArea.AxisX.Step = 6;
                        this.chVolume.DefaultView.ChartArea.AxisX.LabelStep = 3;

                        break;
                    }
                case ("Yearly"):
                    {
                        this.chPricing.DefaultView.ChartArea.AxisX.DefaultLabelFormat = "Y";
                        this.chPricing.DefaultView.ChartArea.SmartLabelsEnabled = true;
                        this.chPricing.DefaultView.ChartArea.AxisX.AutoRange = true;
                        this.chPricing.DefaultView.ChartArea.AxisX.Step = 1;
                        this.chPricing.DefaultView.ChartArea.AxisX.LabelStep = 3;

                        this.chVolume.DefaultView.ChartArea.AxisX.DefaultLabelFormat = "Y";
                        this.chVolume.DefaultView.ChartArea.SmartLabelsEnabled = true;
                        this.chVolume.DefaultView.ChartArea.AxisX.AutoRange = true;
                        this.chVolume.DefaultView.ChartArea.AxisX.Step = 1;
                        this.chVolume.DefaultView.ChartArea.AxisX.LabelStep = 3;

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
            if (this.grdRadGridView.Visibility == System.Windows.Visibility.Visible)
            {
                Flipper.FlipItem(this.grdRadGridView, this.grdRadChart);
            }
            else
            {
                Flipper.FlipItem(this.grdRadChart, this.grdRadGridView);
            }
        }

        #endregion

        #region Export/Print

        /// <summary>
        /// Method to catch Click Event of Export to Excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(this.DataContextClosingPriceChart.logger, methodNamespace);
            try
            {
                List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>();

                if (grdRadChart.Visibility == Visibility.Visible)
                {
                    RadExportOptionsInfo.Add(new RadExportOptions() { ElementName = ExportTypes.CLOSING_PRICE_CHART, Element = this.chPricing, ExportFilterOption = RadExportFilterOption.RADCHART_EXCEL_EXPORT_FILTER });
                    RadExportOptionsInfo.Add(new RadExportOptions() { ElementName = ExportTypes.VOLUME_CHART, Element = this.chVolume, ExportFilterOption = RadExportFilterOption.RADCHART_EXCEL_EXPORT_FILTER });
                }

                if (grdRadGridView.Visibility == Visibility.Visible)
                {
                    RadExportOptionsInfo.Add(new RadExportOptions() { ElementName = ExportTypes.PRICING_DATA, Element = this.dgPricing, ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXCEL_EXPORT_FILTER });
                }

                ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + GadgetNames.SECURITY_REFERENCE_PRICE_COMPARISON);
                childExportOptions.Show();
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(this.DataContextClosingPriceChart.logger, ex);
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
            Logging.LogBeginMethod(this.DataContextClosingPriceChart.logger, methodNamespace);
            try
            {
                List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>();

                if (grdRadChart.Visibility == Visibility.Visible)
                {
                    RadExportOptionsInfo.Add(new RadExportOptions() 
                    {
                        ElementName = ExportTypes.CLOSING_PRICE_CHART, 
                        Element = this.chPricing, ExportFilterOption = RadExportFilterOption.RADCHART_PRINT_FILTER,
                        RichTextBox = this.RichTextBox
                    });
                    RadExportOptionsInfo.Add(new RadExportOptions() 
                    {
                        ElementName = ExportTypes.VOLUME_CHART, Element = this.chVolume, 
                        ExportFilterOption = RadExportFilterOption.RADCHART_PRINT_FILTER,
                        RichTextBox = this.RichTextBox
                    });
                }
                else if (grdRadGridView.Visibility == Visibility.Visible)
                {
                    RadExportOptionsInfo.Add(new RadExportOptions()
                    { 
                        ElementName = ExportTypes.PRICING_DATA,
                        Element = this.dgPricing, ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_PRINT_FILTER,
                        RichTextBox = this.RichTextBox
                    });
                }
                ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + GadgetNames.SECURITY_REFERENCE_PRICE_COMPARISON);
                childExportOptions.Show();
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(this.DataContextClosingPriceChart.logger, ex);
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
            Logging.LogBeginMethod(this.DataContextClosingPriceChart.logger, methodNamespace);
            try
            {
                List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>();

                if (grdRadChart.Visibility == Visibility.Visible)
                {
                    RadExportOptionsInfo.Add(new RadExportOptions() { ElementName = ExportTypes.CLOSING_PRICE_CHART, Element = this.chPricing, ExportFilterOption = RadExportFilterOption.RADCHART_PDF_EXPORT_FILTER });
                    RadExportOptionsInfo.Add(new RadExportOptions() { ElementName = ExportTypes.VOLUME_CHART, Element = this.chVolume, ExportFilterOption = RadExportFilterOption.RADCHART_PDF_EXPORT_FILTER });
                }
                else if (grdRadGridView.Visibility == Visibility.Visible)
                {
                    RadExportOptionsInfo.Add(new RadExportOptions() { ElementName = ExportTypes.PRICING_DATA, Element = this.dgPricing, ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_PDF_EXPORT_FILTER });
                }
                ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + GadgetNames.SECURITY_REFERENCE_PRICE_COMPARISON);
                childExportOptions.Show();
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(this.DataContextClosingPriceChart.logger, ex);
            }
        }

        /// <summary>
        /// Event for Grid Export
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ElementExportingEvent(object sender, GridViewElementExportingEventArgs e)
        {
            RadGridView_ElementExport.ElementExporting(e);
        }

        #endregion

        #region HelperMethods

        /// <summary>
        /// Get Aggregates Function
        /// </summary>
        /// <param name="group"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        private string GetAggregates(QueryableCollectionViewGroup group, GridViewDataColumn column)
        {
            List<string> aggregates = new List<string>();

            foreach (AggregateFunction function in column.AggregateFunctions)
            {
                foreach (AggregateResult result in group.AggregateResults)
                {
                    if (function.FunctionName == result.FunctionName && result.FormattedValue != null)
                    {
                        aggregates.Add(result.FormattedValue.ToString());
                    }
                }
            }
            return String.Join(",", aggregates.ToArray());
        }

        /// <summary>
        /// Time Interval Drop Down Closed Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbTime_DropDownClosed(object sender, EventArgs e)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(this.DataContextClosingPriceChart.logger, methodNamespace);
            try
            {
                if (Convert.ToString(cmbTime.SelectedValue) == "Custom")
                {
                    ViewCustomDateChildWindow customDateWindow = new ViewCustomDateChildWindow();
                    customDateWindow.Show();
                    customDateWindow.Unloaded += (se, a) =>
                    {
                        if (Convert.ToBoolean(customDateWindow.enteredDateCorrect))
                        {
                            DataContextClosingPriceChart.SelectedStartDate = Convert.ToDateTime(customDateWindow.dpStartDate.SelectedDate);
                            DataContextClosingPriceChart.SelectedEndDate = Convert.ToDateTime(customDateWindow.dpEndDate.SelectedDate);
                        }
                        else
                        {
                            this.cmbTime.SelectedValue = "1-Year";
                        }
                        this.DataContextClosingPriceChart.SelectedTimeRange = Convert.ToString(cmbTime.SelectedValue);
                    };
                }
                else
                {
                    this.DataContextClosingPriceChart.SelectedTimeRange = Convert.ToString(cmbTime.SelectedValue);
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(this.DataContextClosingPriceChart.logger, ex);
            }
        }       

        /// <summary>
        /// Add Series Drop Down Opening Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbAddSeries_DropDownOpened(object sender, EventArgs e)
        {
            if (SelectionData.EntitySelectionData != null && dataContextClosingPriceChart.SeriesReferenceSource == null)
            {
                dataContextClosingPriceChart.RetrieveEntitySelectionDataCallBackMethod(SelectionData.EntitySelectionData);
            }
        }

        #endregion

        #region RemoveEvents

        /// <summary>
        /// Event Handlers Unsusbcribe
        /// </summary>
        public override void Dispose()
        {
            this.DataContextClosingPriceChart.Dispose();
            this.DataContextClosingPriceChart = null;
            this.DataContext = null;
        }

        #endregion
    }    
}