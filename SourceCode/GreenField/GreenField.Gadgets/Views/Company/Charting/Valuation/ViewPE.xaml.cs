using System;
using System.Collections.Generic;
using System.Windows;
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
    /// Code behind for ViewPE
    /// </summary>
    public partial class ViewPE : ViewBaseUserControl
    {
        #region Fields
        /// <summary>
        /// Export Types
        /// </summary>
        private static class ExportTypes
        {
            public const string P_E = "P/E";
            public const string P_E_DATA = "P/E Data";
        }
        #endregion

        #region Properties
        /// <summary>
        /// Property of ViewModel type
        /// </summary>
        private ViewModelPE dataContextPE;
        public ViewModelPE DataContextPE
        {
            get
            {
                return dataContextPE;
            }
            set
            {
                dataContextPE = value;
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
                if (DataContextPE != null) //DataContext instance
                    DataContextPE.IsActive = isActive;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataContextSource">ViewModelPE</param>
        public ViewPE(ViewModelPE dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextPE = dataContextSource;
            dataContextSource.ChartArea = this.chPE.DefaultView.ChartArea;
            this.chPE.DataBound += dataContextSource.ChartDataBound;
            this.ApplyChartStyles();
        }
        #endregion

        #region Event Handler
        #region Data load
        /// <summary>
        /// chPE Loaded event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chPE_Loaded(object sender, RoutedEventArgs e)
        {
            if (chPE.DefaultView.ChartLegend.Items.Count != 0)
            {
                ChartLegendItem var = this.chPE.DefaultView.ChartLegend.Items[0];
                this.chPE.DefaultView.ChartLegend.Items.Remove(var);
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

                if (chPE.Visibility == Visibility.Visible)
                    RadExportOptionsInfo.Add(new RadExportOptions() { ElementName = ExportTypes.P_E, Element = this.chPE, ExportFilterOption = RadExportFilterOption.RADCHART_EXCEL_EXPORT_FILTER });

                else if (dgPE.Visibility == Visibility.Visible)
                    RadExportOptionsInfo.Add(new RadExportOptions() { ElementName = ExportTypes.P_E_DATA, Element = this.dgPE, ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXCEL_EXPORT_FILTER });

                ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + GadgetNames.EXTERNAL_RESEARCH_HISTORICAL_VALUATION_CHART_PE);
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

                if (chPE.Visibility == Visibility.Visible)
                {
                    RadExportOptionsInfo.Add(new RadExportOptions()
                    {
                        ElementName = ExportTypes.P_E,
                        Element = this.chPE,
                        ExportFilterOption = RadExportFilterOption.RADCHART_PRINT_FILTER,
                        RichTextBox = this.RichTextBox
                    });
                }
                else if (dgPE.Visibility == Visibility.Visible)
                {
                    RadExportOptionsInfo.Add(new RadExportOptions()
                    {
                        ElementName = ExportTypes.P_E_DATA,
                        Element = this.dgPE,
                        ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_PRINT_FILTER,
                        RichTextBox = this.RichTextBox
                    });
                }
                ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, GadgetNames.EXTERNAL_RESEARCH_HISTORICAL_VALUATION_CHART_PE);
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

                if (chPE.Visibility == Visibility.Visible)
                {
                    RadExportOptionsInfo.Add(new RadExportOptions()
                    {
                        ElementName = ExportTypes.P_E,
                        Element = this.chPE,
                        ExportFilterOption = RadExportFilterOption.RADCHART_PDF_EXPORT_FILTER,
                        RichTextBox = this.RichTextBox
                    });
                }
                else if (dgPE.Visibility == Visibility.Visible)
                {
                    RadExportOptionsInfo.Add(new RadExportOptions()
                    {
                        ElementName = ExportTypes.P_E_DATA,
                        Element = this.dgPE,
                        ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_PDF_EXPORT_FILTER,
                        RichTextBox = this.RichTextBox
                    });
                }
                ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, GadgetNames.EXTERNAL_RESEARCH_HISTORICAL_VALUATION_CHART_PE);
                childExportOptions.Show();
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
            if (this.chPE.Visibility == System.Windows.Visibility.Visible)
            {
                Flipper.FlipItem(this.chPE, this.dgPE);
            }
            else
            {
                Flipper.FlipItem(this.dgPE, this.chPE);
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
            this.chPE.DefaultView.ChartArea.AxisX.TicksDistance = 50;
            this.chPE.DefaultView.ChartArea.AxisX.AxisStyles.ItemLabelStyle = this.Resources["ItemLabelStyle"] as Style;
            this.chPE.DefaultView.ChartArea.AxisY.AxisStyles.ItemLabelStyle = this.Resources["ItemLabelStyle"] as Style;
        }
        
        /// <summary>
        /// UnSubscribing the Events
        /// </summary>
        public override void Dispose()
        {
            this.DataContextPE.Dispose();
            this.DataContextPE = null;
            this.DataContext = null;
        }
        #endregion
    }
}