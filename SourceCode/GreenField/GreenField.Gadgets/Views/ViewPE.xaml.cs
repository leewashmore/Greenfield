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
    public partial class ViewPE : ViewBaseUserControl
    {

        #region Variables

        /// <summary>
        /// Export Types
        /// </summary>
        private static class ExportTypes
        {
            public const string P_E = "P/E";
            public const string P_E_DATA = "P/E Data";
        }


        #endregion

        #region PropertyDeclaration

        /// <summary>
        /// Property of ViewModel type
        /// </summary>
        private ViewModelPE _dataContextPE;
        public ViewModelPE DataContextPE
        {
            get
            {
                return _dataContextPE;
            }
            set
            {
                _dataContextPE = value;
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
                if (DataContextPE != null) //DataContext instance
                    DataContextPE.IsActive = _isActive;
            }
        }
        #endregion

        public ViewPE(ViewModelPE dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextPE = dataContextSource;
            dataContextSource.ChartArea = this.chPE.DefaultView.ChartArea;
            this.ApplyChartStyles();
        }
        private void dgPE_RowLoaded(object sender, Telerik.Windows.Controls.GridView.RowLoadedEventArgs e)
        {
            
        }
        private void chPE_Loaded(object sender, RoutedEventArgs e)
        {
            if (chPE.DefaultView.ChartLegend.Items.Count != 0)
            {
                ChartLegendItem var = this.chPE.DefaultView.ChartLegend.Items[0];
                this.chPE.DefaultView.ChartLegend.Items.Remove(var);
            }
        }
        
        private void ApplyChartStyles()
        {
            this.chPE.DefaultView.ChartArea.AxisX.TicksDistance = 50;
            this.chPE.DefaultView.ChartArea.AxisX.AxisStyles.ItemLabelStyle = this.Resources["ItemLabelStyle"] as Style;
            this.chPE.DefaultView.ChartArea.AxisY.AxisStyles.ItemLabelStyle = this.Resources["ItemLabelStyle"] as Style;
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

                if (chPE.Visibility == Visibility.Visible)
                    RadExportOptionsInfo.Add(new RadExportOptions() { ElementName = ExportTypes.P_E, Element = this.chPE, ExportFilterOption = RadExportFilterOption.RADCHART_EXPORT_FILTER });
                else if (dgPE.Visibility == Visibility.Visible)
                {
                    RadExportOptionsInfo.Add(new RadExportOptions() { ElementName = ExportTypes.P_E_DATA, Element = this.chPE, ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXPORT_FILTER });
                }

                ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + GadgetNames.EXTERNAL_RESEARCH_HISTORICAL_VALUATION_CHART_PE);
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
            this.DataContextPE.Dispose();
            this.DataContextPE = null;
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
    }
}
