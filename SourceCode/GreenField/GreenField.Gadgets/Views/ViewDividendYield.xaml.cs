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
    public partial class ViewDividendYield : ViewBaseUserControl
    {

        #region Variables

        /// <summary>
        /// Export Types
        /// </summary>
        private static class ExportTypes
        {
            public const string Dividend_Yield = "Dividend Yield";
            public const string Dividend_Yield_DATA = "Dividend Yield Data";
        }


        #endregion

        #region PropertyDeclaration

        /// <summary>
        /// Property of ViewModel type
        /// </summary>
        private ViewModelDividendYield _dataContextDividendYield;
        public ViewModelDividendYield DataContextDividendYield
        {
            get
            {
                return _dataContextDividendYield;
            }
            set
            {
                _dataContextDividendYield = value;
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
                if (DataContextDividendYield != null) //DataContext instance
                    DataContextDividendYield.IsActive = _isActive;
            }
        }
        #endregion

        public ViewDividendYield(ViewModelDividendYield dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextDividendYield = dataContextSource;
            dataContextSource.ChartArea = this.chDividendYield.DefaultView.ChartArea;
            this.ApplyChartStyles();
        }
        private void dgDividendYield_RowLoaded(object sender, Telerik.Windows.Controls.GridView.RowLoadedEventArgs e)
        {
           
        }
        private void chDividendYield_Loaded(object sender, RoutedEventArgs e)
        {
            if (chDividendYield.DefaultView.ChartLegend.Items.Count != 0)
            {
                ChartLegendItem var = this.chDividendYield.DefaultView.ChartLegend.Items[0];
                this.chDividendYield.DefaultView.ChartLegend.Items.Remove(var);
            }
        }      

        private void ApplyChartStyles()
        {
            this.chDividendYield.DefaultView.ChartArea.AxisX.TicksDistance = 50;
            this.chDividendYield.DefaultView.ChartArea.AxisX.AxisStyles.ItemLabelStyle = this.Resources["ItemLabelStyle"] as Style;
            this.chDividendYield.DefaultView.ChartArea.AxisY.AxisStyles.ItemLabelStyle = this.Resources["ItemLabelStyle"] as Style;
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

                if (chDividendYield.Visibility == Visibility.Visible)
                    RadExportOptionsInfo.Add(new RadExportOptions() { ElementName = ExportTypes.Dividend_Yield, Element = this.chDividendYield, ExportFilterOption = RadExportFilterOption.RADCHART_EXPORT_FILTER });
                else if (dgDividendYield.Visibility == Visibility.Visible)
                    RadExportOptionsInfo.Add(new RadExportOptions() { ElementName = ExportTypes.Dividend_Yield_DATA, Element = this.chDividendYield, ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXPORT_FILTER });                    

                ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + GadgetNames.EXTERNAL_RESEARCH_HISTORICAL_VALUATION_CHART_DividendYield);
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
            this.DataContextDividendYield.Dispose();
            this.DataContextDividendYield = null;
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
            if (this.chDividendYield.Visibility == System.Windows.Visibility.Visible)
            {
                Flipper.FlipItem(this.chDividendYield, this.dgDividendYield);
            }
            else
            {
                Flipper.FlipItem(this.dgDividendYield, this.chDividendYield);
            }
        }

        #endregion
    }
}
