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
using Telerik.Windows.Controls;
using GreenField.Common;
using GreenField.ServiceCaller;
using GreenField.DataContracts;
using Telerik.Windows.Controls.Charting;

namespace GreenField.Gadgets.Views
{
    /// <summary>
    /// Code-Behind class for Slice-1 ChartExtension
    /// </summary>
    public partial class ViewSlice1ChartExtension : ViewBaseUserControl
    {
        #region Variables

        /// <summary>
        /// Export Types
        /// </summary>
        private static class ExportTypes
        {
            public const string CHART_EXTENSION = "Chart Extension";
            public const string CHART_EXTENSION_DATA = "Chart Extension Data";
        }


        #endregion

        #region PropertyDeclaration

        /// <summary>
        /// Property of ViewModel type
        /// </summary>
        private ViewModelSlice1ChartExtension _dataContextSlice1ChartExtension;
        public ViewModelSlice1ChartExtension DataContextSlice1ChartExtension
        {
            get
            {
                return _dataContextSlice1ChartExtension;
            }
            set
            {
                _dataContextSlice1ChartExtension = value;
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor that initialises the Class
        /// </summary>
        /// <param name="dataContextSource"></param>
        public ViewSlice1ChartExtension(ViewModelSlice1ChartExtension dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            dataContextSource.ChartArea = this.chChartExtension.DefaultView.ChartArea;
            this.chChartExtension.DataBound += dataContextSource.ChartDataBound;
            this.ApplyChartStyles();
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
                
        #region EventsUnsubscribe

        /// <summary>
        /// UnSubscribing the Events
        /// </summary>
        public override void Dispose()
        {            
            this.DataContextSlice1ChartExtension.Dispose();
            this.DataContextSlice1ChartExtension = null;
            this.DataContext = null;
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

                if (grdRadChart.Visibility == Visibility.Visible)
                    RadExportOptionsInfo.Add(new RadExportOptions() { ElementName = ExportTypes.CHART_EXTENSION, Element = this.chChartExtension, ExportFilterOption = RadExportFilterOption.RADCHART_EXPORT_FILTER });
                else if (grdRadGridView.Visibility == Visibility.Visible)
                    RadExportOptionsInfo.Add(new RadExportOptions() { ElementName = ExportTypes.CHART_EXTENSION_DATA, Element = this.dgChartExtension, ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXPORT_FILTER });

                ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + GadgetNames.HOLDINGS_CHART_EXTENTION);
                childExportOptions.Show();
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog(ex.Message);
            }
        }

        #endregion

        #region Helpers

        private void ApplyChartStyles()
        {
            this.chChartExtension.DefaultView.ChartArea.AxisX.TicksDistance = 50;
            this.chChartExtension.DefaultView.ChartArea.AxisX.AxisStyles.ItemLabelStyle = this.Resources["ItemLabelStyle"] as Style;
            this.chChartExtension.DefaultView.ChartArea.AxisY.AxisStyles.ItemLabelStyle = this.Resources["ItemLabelStyle"] as Style;
            this.chChartExtension.DefaultView.ChartArea.ZoomScrollSettingsX.SliderSelectionStart = 0;
            this.chChartExtension.DefaultView.ChartArea.ZoomScrollSettingsX.SliderSelectionEnd = 0.5;
        }

        #endregion

        private void cmbTime_SelectionChanged(object sender, Telerik.Windows.Controls.SelectionChangedEventArgs e)
        {

        }

        private void chChartExtension_DataBound(object sender, Telerik.Windows.Controls.Charting.ChartDataBoundEventArgs e)
        {
            if (this.DataContext as ViewModelSlice1ChartExtension != null)
            {
                if ((this.DataContext as ViewModelSlice1ChartExtension).ChartExtensionData != null)
                {
                    (this.DataContext as ViewModelSlice1ChartExtension).AxisXMinValue = Convert.ToDateTime(((this.DataContext as ViewModelSlice1ChartExtension).ChartExtensionData.OrderBy(a => a.ToDate)).
                        Select(a => a.ToDate).FirstOrDefault()).ToOADate();
                    (this.DataContext as ViewModelSlice1ChartExtension).AxisXMaxValue = Convert.ToDateTime(((this.DataContext as ViewModelSlice1ChartExtension).ChartExtensionData.OrderByDescending(a => a.ToDate)).
                        Select(a => a.ToDate).FirstOrDefault()).ToOADate();
                    int dataCount = (this.DataContext as ViewModelSlice1ChartExtension).ChartExtensionData.Count;
                    if (dataCount != 0)
                    {
                        this.chChartExtension.DefaultView.ChartArea.AxisX.Step = dataCount / 10;
                    }
                }
            }

        }

        private void dgChartExtension_RowLoaded(object sender, Telerik.Windows.Controls.GridView.RowLoadedEventArgs e)
        {
            GroupedGridRowLoadedHandler.Implement(e);
        }

        private void chChartExtension_Loaded(object sender, RoutedEventArgs e)
        {
            if (chChartExtension.DefaultView.ChartLegend.Items.Count != 0)
            {
                ChartLegendItem var = this.chChartExtension.DefaultView.ChartLegend.Items[0];
                this.chChartExtension.DefaultView.ChartLegend.Items.Remove(var);
            }
        }
    }
}
