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
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.Charting;
using GreenField.Common;
using GreenField.DataContracts;
using GreenField.Gadgets.Helpers;
using GreenField.Gadgets.ViewModels;
using GreenField.ServiceCaller;

namespace GreenField.Gadgets.Views
{
    public partial class ViewPBV : ViewBaseUserControl
    {

        #region Variables

        /// <summary>
        /// Export Types
        /// </summary>
        private static class ExportTypes
        {
            public const string P_BV = "P/BV";
            public const string P_BV_DATA = "P/BV Data";
        }


        #endregion

        #region PropertyDeclaration

        /// <summary>
        /// Property of ViewModel type
        /// </summary>
        private ViewModelPBV dataContextPBV;
        public ViewModelPBV DataContextPBV
        {
            get
            {
                return dataContextPBV;
            }
            set
            {
                dataContextPBV = value;
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
                if (DataContextPBV != null) //DataContext instance
                    DataContextPBV.IsActive = isActive;
            }
        }
        #endregion

        #region CONSTRUCTOR
        public ViewPBV(ViewModelPBV dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextPBV = dataContextSource;
            dataContextSource.ChartArea = this.chPBV.DefaultView.ChartArea;
            this.chPBV.DataBound += dataContextSource.ChartDataBound;
            this.ApplyChartStyles();
        }
        #endregion

        #region Event

        private void chPBV_Loaded(object sender, RoutedEventArgs e)
        {
            if (chPBV.DefaultView.ChartLegend.Items.Count != 0)
            {
                ChartLegendItem var = this.chPBV.DefaultView.ChartLegend.Items[0];
                this.chPBV.DefaultView.ChartLegend.Items.Remove(var);
            }
        }
        #endregion

        #region Helper Methods

        private void ApplyChartStyles()
        {
            this.chPBV.DefaultView.ChartArea.AxisX.TicksDistance = 50;
            this.chPBV.DefaultView.ChartArea.AxisX.AxisStyles.ItemLabelStyle = this.Resources["ItemLabelStyle"] as Style;
            this.chPBV.DefaultView.ChartArea.AxisY.AxisStyles.ItemLabelStyle = this.Resources["ItemLabelStyle"] as Style;
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

                if (chPBV.Visibility == Visibility.Visible)
                    RadExportOptionsInfo.Add(new RadExportOptions() { ElementName = ExportTypes.P_BV, Element = this.chPBV, ExportFilterOption = RadExportFilterOption.RADCHART_EXPORT_FILTER });

                else if (dgPBV.Visibility == Visibility.Visible)
                    RadExportOptionsInfo.Add(new RadExportOptions() { ElementName = ExportTypes.P_BV_DATA, Element = this.dgPBV, ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXPORT_FILTER });

                ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + GadgetNames.EXTERNAL_RESEARCH_HISTORICAL_VALUATION_CHART_PBV);
                childExportOptions.Show();
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog(ex.Message);
            }
        }

        #endregion

        #region EventsUnsubscribe

        /// <summary>
        /// UnSubscribing the Events
        /// </summary>
        public override void Dispose()
        {
            this.DataContextPBV.Dispose();
            this.DataContextPBV = null;
            this.DataContext = null;
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
            if (this.chPBV.Visibility == System.Windows.Visibility.Visible)
            {
                Flipper.FlipItem(this.chPBV, this.dgPBV);
            }
            else
            {
                Flipper.FlipItem(this.dgPBV, this.chPBV);
            }
        }

        #endregion
    }
}
