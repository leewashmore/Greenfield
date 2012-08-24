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
using Telerik.Windows.Controls.Charting;
using GreenField.Common;
using GreenField.ServiceCaller;
using Telerik.Windows.Controls;

namespace GreenField.Gadgets.Views
{
    public partial class ViewPCE : ViewBaseUserControl
    {

        #region Variables

        /// <summary>
        /// Export Types
        /// </summary>
        private static class ExportTypes
        {
            public const string P_CE = "P/CE";
            public const string P_CE_DATA = "P/CE Data";
        }


        #endregion

        #region PropertyDeclaration

        /// <summary>
        /// Property of ViewModel type
        /// </summary>
        private ViewModelPCE _dataContextPCE;
        public ViewModelPCE DataContextPCE
        {
            get
            {
                return _dataContextPCE;
            }
            set
            {
                _dataContextPCE = value;
            }
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
                if (DataContextPCE != null) //DataContext instance
                    DataContextPCE.IsActive = _isActive;
            }
        }
        #endregion

        public ViewPCE(ViewModelPCE dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextPCE = dataContextSource;
            dataContextSource.ChartArea = this.chPCE.DefaultView.ChartArea;
            this.ApplyChartStyles();
        }
        private void dgPCE_RowLoaded(object sender, Telerik.Windows.Controls.GridView.RowLoadedEventArgs e)
        {
            GroupedGridRowLoadedHandler.Implement(e);
        }
        private void chPCE_Loaded(object sender, RoutedEventArgs e)
        {
            if (chPCE.DefaultView.ChartLegend.Items.Count != 0)
            {
                ChartLegendItem var = this.chPCE.DefaultView.ChartLegend.Items[0];
                this.chPCE.DefaultView.ChartLegend.Items.Remove(var);
            }
        }

        private void ApplyChartStyles()
        {
            this.chPCE.DefaultView.ChartArea.AxisX.TicksDistance = 50;
            this.chPCE.DefaultView.ChartArea.AxisX.AxisStyles.ItemLabelStyle = this.Resources["ItemLabelStyle"] as Style;
            this.chPCE.DefaultView.ChartArea.AxisY.AxisStyles.ItemLabelStyle = this.Resources["ItemLabelStyle"] as Style;
        }

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

                if (chPCE.Visibility == Visibility.Visible)
                    RadExportOptionsInfo.Add(new RadExportOptions() { ElementName = ExportTypes.P_CE, Element = this.chPCE, ExportFilterOption = RadExportFilterOption.RADCHART_EXPORT_FILTER });
                else if (dgPCE.Visibility == Visibility.Visible)
                {
                    RadExportOptionsInfo.Add(new RadExportOptions() { ElementName = ExportTypes.P_CE_DATA, Element = this.chPCE, ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXPORT_FILTER });
                    return;
                }

                ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + GadgetNames.EXTERNAL_RESEARCH_HISTORICAL_VALUATION_CHART_PCE);
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
            this.DataContextPCE.Dispose();
            this.DataContextPCE = null;
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
            if (this.chPCE.Visibility == System.Windows.Visibility.Visible)
            {
                Flipper.FlipItem(this.chPCE, this.dgPCE);
            }
            else
            {
                Flipper.FlipItem(this.dgPCE, this.chPCE);
            }
        }

        #endregion
    }
}
