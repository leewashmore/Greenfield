using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;
using GreenField.Common;
using GreenField.Gadgets.Helpers;
using GreenField.Gadgets.ViewModels;
using GreenField.ServiceCaller;

namespace GreenField.Gadgets.Views
{
    public partial class ViewRiskIndexExposures : ViewBaseUserControl
    {
        #region Properties
        /// <summary>
        /// property to set data context
        /// </summary>
        private ViewModelRiskIndexExposures dataContextViewModelTopHoldings;
        public ViewModelRiskIndexExposures DataContextViewModelTopHoldings
        {
            get { return dataContextViewModelTopHoldings; }
            set { dataContextViewModelTopHoldings = value; }
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
                if (DataContextViewModelTopHoldings != null) 
                { DataContextViewModelTopHoldings.IsActive = isActive; }
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="dataContextSource"></param>
        public ViewRiskIndexExposures(ViewModelRiskIndexExposures dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextViewModelTopHoldings = dataContextSource;
        }
        #endregion

        #region Helper Methods
        #region Method to Flip
        /// <summary>
        /// Flipping between Grid & PieChart
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFlip_Click(object sender, RoutedEventArgs e)
        {
            if (this.chartRelativerisk.Visibility == System.Windows.Visibility.Visible)
            {
                Flipper.FlipItem(this.chartRelativerisk, this.dgRelativeRisk);
            }
            else
            {
                Flipper.FlipItem(this.dgRelativeRisk, this.chartRelativerisk);
            }
        }
        #endregion

        #region Export To Excel
        /// <summary>
        /// Method to catch Click Event of Export to Excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.chartRelativerisk.Visibility == Visibility.Visible)
                {
                    List<RadExportOptions> radExportOptionsInfo = new List<RadExportOptions>{ new RadExportOptions()
                    {
                        ElementName =  "Relative Risk Data",
                        Element = this.chartRelativerisk, 
                        ExportFilterOption = RadExportFilterOption.RADCHART_EXCEL_EXPORT_FILTER 
                    },};
                    ChildExportOptions childExportOptions = new ChildExportOptions(radExportOptionsInfo, "Export Options: " + GadgetNames.HOLDINGS_RELATIVE_RISK);
                    childExportOptions.Show();
                }
                else
                {
                    if (this.dgRelativeRisk.Visibility == Visibility.Visible)
                    {
                        ChildExportOptions childExportOptions = new ChildExportOptions(new List<RadExportOptions>
                        { new RadExportOptions() 
                             {
                                Element = this.dgRelativeRisk,
                                ElementName = "Relative Risk Data",
                                ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXCEL_EXPORT_FILTER
                             }
                        }, "Export Options: " + GadgetNames.HOLDINGS_RELATIVE_RISK);
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
            //Logging.LogBeginMethod(this.DataContextHoldingsPieChart.logger, methodNamespace);
            try
            {
                if (this.chartRelativerisk.Visibility == Visibility.Visible)
                {
                    List<RadExportOptions> radExportOptionsInfo = new List<RadExportOptions>{ new RadExportOptions()
                    {
                        ElementName =  "Relative Risk Data",
                        Element = this.chartRelativerisk, 
                        ExportFilterOption = RadExportFilterOption.RADCHART_PRINT_FILTER 
                    },};
                    ChildExportOptions childExportOptions = new ChildExportOptions(radExportOptionsInfo, "Export Options: " + GadgetNames.HOLDINGS_RELATIVE_RISK);
                    childExportOptions.Show();
                }
                else
                {
                    if (this.dgRelativeRisk.Visibility == Visibility.Visible)
                    {
                        ChildExportOptions childExportOptions = new ChildExportOptions(new List<RadExportOptions>
                        { new RadExportOptions() 
                             {
                                Element = this.dgRelativeRisk,
                                ElementName = "Relative Risk Data",
                                ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_PRINT_FILTER
                             }
                        }, "Export Options: " + GadgetNames.HOLDINGS_RELATIVE_RISK);
                        childExportOptions.Show();
                    }
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                //Logging.LogException(this.DataContextSlice1ChartExtension.logger, ex);
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
            //Logging.LogBeginMethod(this.DataContextSlice1ChartExtension.logger, methodNamespace);
            try
            {

                if (this.chartRelativerisk.Visibility == Visibility.Visible)
                {
                    List<RadExportOptions> radExportOptionsInfo = new List<RadExportOptions>{ new RadExportOptions()
                    {
                        ElementName =  "Relative Risk Data",
                        Element = this.chartRelativerisk, 
                        ExportFilterOption = RadExportFilterOption.RADCHART_PDF_EXPORT_FILTER 
                    },};
                    ChildExportOptions childExportOptions = new ChildExportOptions(radExportOptionsInfo, "Export Options: " + GadgetNames.HOLDINGS_RELATIVE_RISK);
                    childExportOptions.Show();
                }
                else
                {
                    if (this.dgRelativeRisk.Visibility == Visibility.Visible)
                    {
                        ChildExportOptions childExportOptions = new ChildExportOptions(new List<RadExportOptions>
                        { new RadExportOptions() 
                             {
                                Element = this.dgRelativeRisk,
                                ElementName = "Relative Risk Data",
                                ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_PDF_EXPORT_FILTER
                             }
                        }, "Export Options: " + GadgetNames.HOLDINGS_RELATIVE_RISK);
                        childExportOptions.Show();
                    }
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                //Logging.LogException(this.DataContextSlice1ChartExtension.logger, ex);
            }
        }

        /// <summary>
        /// Handles element exporting while export to excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgRelativeRisk_ElementExporting(object sender, GridViewElementExportingEventArgs e)
        {
            RadGridView_ElementExport.ElementExporting(e, isGroupFootersVisible: true, aggregatedColumnIndex: new List<int> { 1, 2, 3 });
        }
        #endregion 
          
        /// <summary>
        /// calculating axis values for chart
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chartRelativerisk_DataBound(object sender, Telerik.Windows.Controls.Charting.ChartDataBoundEventArgs e)
        {
            if (this.DataContext as ViewModelRiskIndexExposures != null)
            {
                if ((this.DataContext as ViewModelRiskIndexExposures).RiskIndexExposuresChartInfo != null)
                {
                    (this.DataContext as ViewModelRiskIndexExposures).AxisXMinValue = Convert.ToDecimal(((this.DataContext as ViewModelRiskIndexExposures)
                        .RiskIndexExposuresChartInfo.OrderBy(a => a.Value)).Select(a => a.Value).FirstOrDefault());
                    (this.DataContext as ViewModelRiskIndexExposures).AxisXMaxValue = Convert.ToDecimal(((this.DataContext as ViewModelRiskIndexExposures)
                        .RiskIndexExposuresChartInfo.OrderByDescending(a => a.Value)).Select(a => a.Value).FirstOrDefault());

                    this.chartRelativerisk.DefaultView.ChartArea.AxisY.Step = 10;
                }
            }
        }
        #endregion

        #region Dispose Method
        /// <summary>
        /// method to dispose all running events
        /// </summary>
        public override void Dispose()
        {
            this.DataContextViewModelTopHoldings.Dispose();
            this.DataContextViewModelTopHoldings = null;
            this.DataContext = null;
        }
        #endregion
    }
}
