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
    public partial class ViewFCFYield : ViewBaseUserControl
    {

        #region Variables

        /// <summary>
        /// Export Types
        /// </summary>
        private static class ExportTypes
        {
            public const string FCF_Yield = "FCF Yield";
            public const string FCF_Yield_DATA = "FCF Yield Data";
        }


        #endregion

        #region PropertyDeclaration

        /// <summary>
        /// Property of ViewModel type
        /// </summary>
        private ViewModelFCFYield _dataContextFCFYield;
        public ViewModelFCFYield DataContextFCFYield
        {
            get
            {
                return _dataContextFCFYield;
            }
            set
            {
                _dataContextFCFYield = value;
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
                if (DataContextFCFYield != null) 
                    DataContextFCFYield.IsActive = _isActive;
            }
        }
        #endregion

        public ViewFCFYield(ViewModelFCFYield dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextFCFYield = dataContextSource;
            dataContextSource.ChartArea = this.chFCFYield.DefaultView.ChartArea;
            this.ApplyChartStyles();
        }
        private void dgFCFYield_RowLoaded(object sender, Telerik.Windows.Controls.GridView.RowLoadedEventArgs e)
        {
           
        }
        private void chFCFYield_Loaded(object sender, RoutedEventArgs e)
        {
            if (chFCFYield.DefaultView.ChartLegend.Items.Count != 0)
            {
                ChartLegendItem var = this.chFCFYield.DefaultView.ChartLegend.Items[0];
                this.chFCFYield.DefaultView.ChartLegend.Items.Remove(var);
            }
        }       
        private void ApplyChartStyles()
        {
            this.chFCFYield.DefaultView.ChartArea.AxisX.TicksDistance = 50;
            this.chFCFYield.DefaultView.ChartArea.AxisX.AxisStyles.ItemLabelStyle = this.Resources["ItemLabelStyle"] as Style;
            this.chFCFYield.DefaultView.ChartArea.AxisY.AxisStyles.ItemLabelStyle = this.Resources["ItemLabelStyle"] as Style;
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

                if (chFCFYield.Visibility == Visibility.Visible)
                    RadExportOptionsInfo.Add(new RadExportOptions() { ElementName = ExportTypes.FCF_Yield, Element = this.chFCFYield, ExportFilterOption = RadExportFilterOption.RADCHART_EXPORT_FILTER });
                else if (dgFCFYield.Visibility == Visibility.Visible)
                    RadExportOptionsInfo.Add(new RadExportOptions() { ElementName = ExportTypes.FCF_Yield, Element = this.dgFCFYield, ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXPORT_FILTER });

                ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + GadgetNames.EXTERNAL_RESEARCH_HISTORICAL_VALUATION_CHART_FCFYield);
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
            this.DataContextFCFYield.Dispose();
            this.DataContextFCFYield = null;
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
            if (this.chFCFYield.Visibility == System.Windows.Visibility.Visible)
            {
                Flipper.FlipItem(this.chFCFYield, this.dgFCFYield);
            }
            else
            {
                Flipper.FlipItem(this.dgFCFYield, this.chFCFYield);
            }
        }

        #endregion
    }
}
    

