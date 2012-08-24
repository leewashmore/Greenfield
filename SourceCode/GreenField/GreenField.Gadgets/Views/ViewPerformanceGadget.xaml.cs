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
using GreenField.Common;
using GreenField.ServiceCaller;

namespace GreenField.Gadgets.Views
{
    /// <summary>
    /// View Class for Performance Gadget that has ViewPerformanceGadget as its data source
    /// </summary>
    public partial class ViewPerformanceGadget : ViewBaseUserControl
    {
        #region StaticClass
        /// <summary>
        /// Export Types to be passed to the ExportOptions class
        /// </summary>
        private static class ExportTypes
        {
            public const string PERFORMANCE_GADGET_CHART = "Performance Gadget Chart";
            public const string PERFORMANCE_GADGET_DATA = "Performance Gadget Data";
        }
        #endregion 

        #region Properties
        /// <summary>
        /// True is gadget is currently on display
        /// </summary>
        private bool _isActive;
        public override bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
                if (this.DataContext != null)
                    ((ViewModelPerformanceGadget)this.DataContext).IsActive = _isActive;
            }
        }
        /// <summary>
        /// Data Context Property
        /// </summary>
        private ViewModelPerformanceGadget _dataContextPerformanceGadget;
        public ViewModelPerformanceGadget DataContextPerformanceGadget
        {
            get { return _dataContextPerformanceGadget; }
            set { _dataContextPerformanceGadget = value; }
        }

        
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor for the class having ViewModelPerformanceGadget as its data context
        /// </summary>
        /// <param name="dataContextSource"></param>
        public ViewPerformanceGadget(ViewModelPerformanceGadget dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextPerformanceGadget = dataContextSource;
            dataContextSource.performanceGraphDataLoadedEvent +=
            new DataRetrievalProgressIndicatorEventHandler(dataContextSource_performanceGraphDataLoadedEvent);
            dataContextSource.ChartArea = this.chPerformanceGadget.DefaultView.ChartArea;
            this.chPerformanceGadget.DataBound += dataContextSource.ChartDataBound;
            this.grdRadChart.Visibility = Visibility.Visible;
            this.grdRadGridView.Visibility = Visibility.Collapsed;
            this.chPerformanceGadget.DefaultView.ChartLegend.Style = this.Resources["ChartLegendStyle"] as Style;
        }
        #endregion

        #region PrivateMethods
        /// <summary>
        /// Flipping between Grid & Chart
        /// Using the method FlipItem in class Flipper.cs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFlip_Click(object sender, RoutedEventArgs e)
        {
            if (this.grdRadGridView.Visibility == Visibility.Visible)
                Flipper.FlipItem(this.grdRadGridView, this.grdRadChart);
            else
                Flipper.FlipItem(this.grdRadChart, this.grdRadGridView);
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
                if (this.grdRadChart.Visibility == Visibility.Visible)
                {
                    List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>
                {
                  
                    new RadExportOptions() { ElementName = ExportTypes.PERFORMANCE_GADGET_CHART, Element = this.chPerformanceGadget, ExportFilterOption = RadExportFilterOption.RADCHART_EXPORT_FILTER },                    
                    
                };
                    ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + GadgetNames.PERFORMANCE_GRAPH);
                    childExportOptions.Show();
                }
                else
                {
                    if (this.grdRadGridView.Visibility == Visibility.Visible)
                    {
                        List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>
                        {
                            new RadExportOptions() { ElementName = ExportTypes.PERFORMANCE_GADGET_DATA, Element = this.dgPerformanceGadget, ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXPORT_FILTER }
                        };
                        ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + GadgetNames.PERFORMANCE_GRAPH);
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
        /// Data Retrieval Indicator
        /// </summary>
        /// <param name="e"></param>
        void dataContextSource_performanceGraphDataLoadedEvent(DataRetrievalProgressIndicatorEventArgs e)
        {
            if (e.ShowBusy)
            {
                this.busyIndicatorChart.IsBusy = true;
                this.busyIndicatorGrid.IsBusy = true;
            }
            else
            {
                this.busyIndicatorChart.IsBusy = false;
                this.busyIndicatorGrid.IsBusy = false;
            }
        }

        private void dgPerformanceGadget_RowLoaded(object sender, Telerik.Windows.Controls.GridView.RowLoadedEventArgs e)
        {
            //GroupedGridRowLoadedHandler.Implement(e);
        }

        private void dgPerformanceGraph_ElementExporting(object sender, Telerik.Windows.Controls.GridViewElementExportingEventArgs e)
        {
            RadGridView_ElementExport.ElementExporting(e);
        }
        #endregion

        #region RemoveEvents

        public override void Dispose()
        {
            this.DataContextPerformanceGadget.performanceGraphDataLoadedEvent -= new DataRetrievalProgressIndicatorEventHandler(dataContextSource_performanceGraphDataLoadedEvent);
            this.DataContextPerformanceGadget.Dispose();
            this.DataContextPerformanceGadget = null;
            this.DataContext = null;
        }

        #endregion
    }
}
