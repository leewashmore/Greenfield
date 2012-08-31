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
    public partial class ViewEVEBITDA : ViewBaseUserControl
    {

        #region Variables

        /// <summary>
        /// Export Types
        /// </summary>
        private static class ExportTypes
        {
            public const string EV_EBITDA = "EV/EBITDA";
            public const string EV_EBITDA_DATA = "EV/EBITDA Data";
        }


        #endregion

        #region PropertyDeclaration

        /// <summary>
        /// Property of ViewModel type
        /// </summary>
        private ViewModelEVEBITDA _dataContextEVEBITDA;
        public ViewModelEVEBITDA DataContextEVEBITDA
        {
            get
            {
                return _dataContextEVEBITDA;
            }
            set
            {
                _dataContextEVEBITDA = value;
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
                if (DataContextEVEBITDA != null) //DataContext instance
                    DataContextEVEBITDA.IsActive = _isActive;
            }
        }
        #endregion

        public ViewEVEBITDA(ViewModelEVEBITDA dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextEVEBITDA = dataContextSource;
            dataContextSource.ChartArea = this.chEVEBITDA.DefaultView.ChartArea;
            this.chEVEBITDA.DataBound += dataContextSource.ChartDataBound;
            this.ApplyChartStyles();
        }
        private void dgEVEBITDA_RowLoaded(object sender, Telerik.Windows.Controls.GridView.RowLoadedEventArgs e)
        {
           
        }
        private void chEVEBITDA_Loaded(object sender, RoutedEventArgs e)
        {
            if (chEVEBITDA.DefaultView.ChartLegend.Items.Count != 0)
            {
                ChartLegendItem var = this.chEVEBITDA.DefaultView.ChartLegend.Items[0];
                this.chEVEBITDA.DefaultView.ChartLegend.Items.Remove(var);
            }
        }

       
        private void ApplyChartStyles()
        {
            this.chEVEBITDA.DefaultView.ChartArea.AxisX.TicksDistance = 50;
            this.chEVEBITDA.DefaultView.ChartArea.AxisX.AxisStyles.ItemLabelStyle = this.Resources["ItemLabelStyle"] as Style;
            this.chEVEBITDA.DefaultView.ChartArea.AxisY.AxisStyles.ItemLabelStyle = this.Resources["ItemLabelStyle"] as Style;
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

                if (chEVEBITDA.Visibility == Visibility.Visible)
                    RadExportOptionsInfo.Add(new RadExportOptions() { ElementName = ExportTypes.EV_EBITDA, Element = this.chEVEBITDA, ExportFilterOption = RadExportFilterOption.RADCHART_EXPORT_FILTER });
                else if (dgEVEBITDA.Visibility == Visibility.Visible)
                    RadExportOptionsInfo.Add(new RadExportOptions() { ElementName = ExportTypes.EV_EBITDA_DATA, Element = this.chEVEBITDA, ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXPORT_FILTER });

                ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + GadgetNames.EXTERNAL_RESEARCH_HISTORICAL_VALUATION_CHART_EVEBITDA);
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
            this.DataContextEVEBITDA.Dispose();
            this.DataContextEVEBITDA = null;
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
            if (this.chEVEBITDA.Visibility == System.Windows.Visibility.Visible)
            {
                Flipper.FlipItem(this.chEVEBITDA, this.dgEVEBITDA);
            }
            else
            {
                Flipper.FlipItem(this.dgEVEBITDA, this.chEVEBITDA);
            }
        }

        #endregion
    }
}
