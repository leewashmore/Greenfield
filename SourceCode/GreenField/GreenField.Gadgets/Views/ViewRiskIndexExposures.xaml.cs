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
using GreenField.Gadgets.Helpers;
using GreenField.Gadgets.ViewModels;
using GreenField.Common;
using GreenField.ServiceCaller;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;

namespace GreenField.Gadgets.Views
{
    public partial class ViewRiskIndexExposures : ViewBaseUserControl
    {
        #region Properties
        /// <summary>
        /// property to set data context
        /// </summary>
        private ViewModelRiskIndexExposures _dataContextViewModelTopHoldings;
        public ViewModelRiskIndexExposures DataContextViewModelTopHoldings
        {
            get { return _dataContextViewModelTopHoldings; }
            set { _dataContextViewModelTopHoldings = value; }
        }

        /// <summary>
        /// property to set IsActive variable of View Model
        /// </summary>
        private bool _isActive;
        public override bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
                if (DataContextViewModelTopHoldings != null) //DataContext instance
                    DataContextViewModelTopHoldings.IsActive = _isActive;
            }
        }

        /// <summary>
        /// Export Types to be passed to the ExportOptions class
        /// </summary>
        private static class ExportTypes
        {
            public const string HOLDINGS_RELATIVE_RISK_CHART = "Relative Risk";
            public const string HOLDINGS_RELATIVE_RISK_GRID = "Relative Risk";
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
           // dataContextSource.RiskIndexExposuresDataLoadedEvent += new DataRetrievalProgressIndicatorEventHandler(DataContextSourceRiskIndexExposuresLoadedevent);
            this.DataContextViewModelTopHoldings = dataContextSource;
        }
        #endregion        

        #region Method to Flip
        /// <summary>
        /// Flipping between Grid & PieChart
        /// Using the method FlipItem in class Flipper.cs
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
                    List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>
                {                   
                    new RadExportOptions()
                    {
                        ElementName = ExportTypes.HOLDINGS_RELATIVE_RISK_CHART,
                        Element = this.chartRelativerisk, 
                        ExportFilterOption = RadExportFilterOption.RADCHART_EXPORT_FILTER 
                    },                    
                    
                };
                    ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo,
                    "Export Options: " + GadgetNames.HOLDINGS_RELATIVE_RISK);
                    childExportOptions.Show();
                }

                else
                {
                    if (this.dgRelativeRisk.Visibility == Visibility.Visible)
                    {
                        ChildExportOptions childExportOptions = new ChildExportOptions
                    (new List<RadExportOptions>{new RadExportOptions() 
                    {
                        Element = this.dgRelativeRisk,
                        ElementName = "Relative Risk Data",
                        ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXPORT_FILTER
                    }}, "Export Options: " + GadgetNames.HOLDINGS_RELATIVE_RISK);
                        childExportOptions.Show();
                    }
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog(ex.Message);
            }
        }

        private void dgRelativeRisk_ElementExporting(object sender, GridViewElementExportingEventArgs e)
        {
            RadGridView_ElementExport.ElementExporting(e, showGroupFooters: true, aggregatedColumnIndex: new List<int> { 1, 2, 3 });
        }
        #endregion

        private void dgRelativeRisk_RowLoaded(object sender, RowLoadedEventArgs e)
        {
            GroupedGridRowLoadedHandler.Implement(e);
        }  

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

        private void chartRelativerisk_DataBound(object sender, Telerik.Windows.Controls.Charting.ChartDataBoundEventArgs e)
        {
            if (this.DataContext as ViewModelRiskIndexExposures != null)
            {
                if ((this.DataContext as ViewModelRiskIndexExposures).RiskIndexExposuresChartInfo != null)
                {
                    (this.DataContext as ViewModelRiskIndexExposures).AxisXMinValue = Convert.ToDecimal(((this.DataContext as ViewModelRiskIndexExposures).RiskIndexExposuresChartInfo.OrderBy(a => a.Value)).
                        Select(a => a.Value).FirstOrDefault());
                    (this.DataContext as ViewModelRiskIndexExposures).AxisXMaxValue = Convert.ToDecimal(((this.DataContext as ViewModelRiskIndexExposures).RiskIndexExposuresChartInfo.OrderByDescending(a => a.Value)).
                        Select(a => a.Value).FirstOrDefault());
                    
                    this.chartRelativerisk.DefaultView.ChartArea.AxisY.Step = 10;
                }
            }
        }
    }
}
