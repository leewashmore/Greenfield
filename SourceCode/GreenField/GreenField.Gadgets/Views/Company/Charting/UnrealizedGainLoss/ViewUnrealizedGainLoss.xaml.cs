﻿using System;
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
using System.IO;
using GreenField.Common;
using GreenField.ServiceCaller;
using GreenField.Gadgets.ViewModels;
using GreenField.Gadgets.Helpers;
using Telerik.Windows.Documents.Model;

namespace GreenField.Gadgets.Views
{
    /// <summary>
    /// View Class that interacts that has ViewModelUnrealizedGainLoss as its Data Context
    /// </summary>
    public partial class ViewUnrealizedGainLoss : ViewBaseUserControl
    {
        #region Fields
        /// <summary>
        /// Export Types to be passed to the ExportOptions class
        /// </summary>
        private static class ExportTypes
        {
            public const string UNREALIZED_GAINLOSS_CHART = "Unrealized Gain/Loss Chart";
            public const string UNREALIZED_GAINLOSS_DATA = "Unrealized Gain/Loss Data";
        }      
        #endregion   

        #region Properties
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
                if (DataContextUnrealizedGainLossChart != null)
                {
                    DataContextUnrealizedGainLossChart.IsActive = isActive;
                }
            }
        }

        /// <summary>
        /// Data Context property of the Veiw Model
        /// </summary>
        private ViewModelUnrealizedGainLoss dataContextUnrealizedGainLossChart;
        public ViewModelUnrealizedGainLoss DataContextUnrealizedGainLossChart
        {
            get { return dataContextUnrealizedGainLossChart; }
            set { dataContextUnrealizedGainLossChart = value; }
        } 
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataContextSource"></param>
        public ViewUnrealizedGainLoss(ViewModelUnrealizedGainLoss dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextUnrealizedGainLossChart = dataContextSource;
            dataContextSource.unrealizedGainLossDataLoadedEvent +=
                new DataRetrievalProgressIndicatorEventHandler(dataContextSource_unrealizedGainLossDataLoadedEvent);
            dataContextSource.ChartArea = this.chUnrealizedGainLoss.DefaultView.ChartArea;
            this.chUnrealizedGainLoss.DataBound += dataContextSource.ChartDataBound;
            this.grdRadChart.Visibility = Visibility.Visible;
            this.grdRadGridView.Visibility = Visibility.Collapsed;
            ApplyChartStyles();
        } 
        #endregion        

        #region Event Handlers
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
                if (this.grdRadChart.Visibility == Visibility.Visible)
                {
                    List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>
                    {                  
                        new RadExportOptions() { ElementName = ExportTypes.UNREALIZED_GAINLOSS_CHART
                            , Element = this.chUnrealizedGainLoss, ExportFilterOption = RadExportFilterOption.RADCHART_EXCEL_EXPORT_FILTER },                                       
                    };
                    ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + GadgetNames.SECURITY_REFERENCE_UNREALIZED_GAIN_LOSS);
                    childExportOptions.Show();
                }
                else
                {
                    if (this.grdRadGridView.Visibility == Visibility.Visible)
                    {
                        List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>
                        {
                            new RadExportOptions() { ElementName = ExportTypes.UNREALIZED_GAINLOSS_DATA, Element = this.dgUnrealizedGainLoss, ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXCEL_EXPORT_FILTER }
                        };
                        ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + GadgetNames.SECURITY_REFERENCE_UNREALIZED_GAIN_LOSS);
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
            try
            {
                List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>();

                if (grdRadChart.Visibility == Visibility.Visible)
                {
                    RadExportOptionsInfo.Add(new RadExportOptions()
                    {
                        ElementName = ExportTypes.UNREALIZED_GAINLOSS_CHART,
                        Element = this.chUnrealizedGainLoss,
                        ExportFilterOption = RadExportFilterOption.RADCHART_PRINT_FILTER,
                        RichTextBox = this.RichTextBox
                    });
                }
                else if (grdRadGridView.Visibility == Visibility.Visible)
                {
                    RadExportOptionsInfo.Add(new RadExportOptions()
                    {
                        ElementName = ExportTypes.UNREALIZED_GAINLOSS_DATA,
                        Element = this.dgUnrealizedGainLoss,
                        ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_PRINT_FILTER,
                        RichTextBox = this.RichTextBox
                    });
                }
                ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + GadgetNames.SECURITY_REFERENCE_UNREALIZED_GAIN_LOSS);
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

                if (grdRadChart.Visibility == Visibility.Visible)
                {
                    RadExportOptionsInfo.Add(new RadExportOptions() 
                    {
                        ElementName = ExportTypes.UNREALIZED_GAINLOSS_CHART,
                        Element = this.chUnrealizedGainLoss,
                        ExportFilterOption = RadExportFilterOption.RADCHART_PDF_EXPORT_FILTER
                    });                    
                }

                if (grdRadGridView.Visibility == Visibility.Visible)
                {
                    RadExportOptionsInfo.Add(new RadExportOptions()
                    { 
                        ElementName = ExportTypes.UNREALIZED_GAINLOSS_DATA,
                        Element = this.dgUnrealizedGainLoss,
                        ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_PDF_EXPORT_FILTER
                    });
                }
                ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + GadgetNames.SECURITY_REFERENCE_UNREALIZED_GAIN_LOSS);
                childExportOptions.Show();                
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
            }
        }

        /// <summary>
        /// Method to be called when the selection is changed in Frequency Comco box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Element Exporting for Export to excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">event object</param>
        private void dgUnrealizedGainLoss_ElementExporting(object sender, Telerik.Windows.Controls.GridViewElementExportingEventArgs e)
        {
            RadGridView_ElementExport.ElementExporting(e);
        } 
        #endregion

        #region Helper Methods
        /// <summary>
        /// Formatting the chart
        /// </summary>
        private void ApplyChartStyles()
        {
            this.chUnrealizedGainLoss.DefaultView.ChartArea.AxisX.AxisStyles.ItemLabelStyle = this.Resources["ItemLabelStyle"] as Style;
            this.chUnrealizedGainLoss.DefaultView.ChartArea.AxisY.AxisStyles.ItemLabelStyle = this.Resources["ItemLabelStyle"] as Style;
            this.chUnrealizedGainLoss.DefaultView.ChartArea.AxisX.AxisStyles.TitleStyle = this.Resources["AxisTitleStyle"] as Style;
            this.chUnrealizedGainLoss.DefaultView.ChartArea.AxisY.AxisStyles.TitleStyle = this.Resources["AxisTitleStyle"] as Style;
            this.chUnrealizedGainLoss.DefaultView.ChartLegend.Style = this.Resources["ChartLegendStyle"] as Style;
        }

        /// <summary>
        /// Disposing events
        /// </summary>
        public override void Dispose()
        {
            this.DataContextUnrealizedGainLossChart.unrealizedGainLossDataLoadedEvent -= new DataRetrievalProgressIndicatorEventHandler(dataContextSource_unrealizedGainLossDataLoadedEvent);
            this.DataContextUnrealizedGainLossChart.Dispose();
            this.DataContextUnrealizedGainLossChart = null;
            this.DataContext = null;
        }
        #endregion
    }
}
