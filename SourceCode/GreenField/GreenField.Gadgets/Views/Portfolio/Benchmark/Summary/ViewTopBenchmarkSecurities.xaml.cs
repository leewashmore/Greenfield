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
using GreenField.Gadgets.ViewModels;
using GreenField.Common;
using GreenField.Gadgets.Helpers;
using GreenField.ServiceCaller;

namespace GreenField.Gadgets.Views
{
    /// <summary>
    /// View Class for Top Ten Benchmark Securities Gadget that has ViewModelTopBenchmarkSecurities as its data source
    /// </summary>
    public partial class ViewTopBenchmarkSecurities : ViewBaseUserControl
    {
        #region Private Static Class
        /// <summary>
        /// Export Types to be passed to the ExportOptions class
        /// </summary>
        private static class ExportTypes
        {         
            public const string BENCHMARK_GRID = "Top Ten Benchmark Securities Grid";
        }
        #endregion

        #region Properties
        /// <summary>
        /// Property of type ViewModelTopBenchmarkSecurities
        /// </summary>
        private ViewModelTopBenchmarkSecurities dataContextTopBenchmarkSecurities;
        public ViewModelTopBenchmarkSecurities DataContextTopBenchmarkSecurities
        {
            get { return dataContextTopBenchmarkSecurities; }
            set { dataContextTopBenchmarkSecurities = value; }
        }

        /// <summary>
        /// True is gadget is currently on display
        /// </summary>
        private bool isActive;
        public override bool IsActive
        {
            get { return isActive; }
            set
            {
                isActive = value;
                if (DataContextTopBenchmarkSecurities != null)
                    DataContextTopBenchmarkSecurities.IsActive = isActive;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor for the class having ViewModelTopBenchmarkSecurities as its data context
        /// </summary>
        /// <param name="dataContextSource"></param>
        public ViewTopBenchmarkSecurities(ViewModelTopBenchmarkSecurities dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextTopBenchmarkSecurities = dataContextSource;
            dataContextSource.topTenBenchmarkSecuritiesDataLoadedEvent +=
            new DataRetrievalProgressIndicatorEventHandler(dataContextSource_topTenBenchmarkSecuritiesDataLoadedEvent);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Data Retrieval Indicator
        /// </summary>
        /// <param name="e"></param>
        void dataContextSource_topTenBenchmarkSecuritiesDataLoadedEvent(DataRetrievalProgressIndicatorEventArgs e)
        {
            if (e.ShowBusy)
            {
                this.busyIndicatorGrid.IsBusy = true;
            }
            else
            {
                this.busyIndicatorGrid.IsBusy = false;
            }
        }

        #endregion  
     
        #region Export To Excel Methods
        /// <summary>
        /// Exports to excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            ChildExportOptions childExportOptions = new ChildExportOptions
                (
                new List<RadExportOptions>
                {
                    new RadExportOptions() 
                    {
                        Element = this.dgTopTenSecurities,
                        ElementName = "Top Ten Benchmark Securities",
                        ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXPORT_FILTER
                    } 
                }, "Export Options: " + GadgetNames.BENCHMARK_TOP_TEN_CONSTITUENTS);
            childExportOptions.Show();
        }

        /// <summary>
        ///Adding styles to export
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgTopTenBenchmarkSecurities_ElementExporting(object sender, GridViewElementExportingEventArgs e)
        {
            RadGridView_ElementExport.ElementExporting(e, isGroupFootersVisible: false);
        }
        #endregion
    
        #region RemoveEvents
        /// <summary>
        /// Dispose events
        /// </summary>
        public override void Dispose()
        {
            this.DataContextTopBenchmarkSecurities.topTenBenchmarkSecuritiesDataLoadedEvent -= new DataRetrievalProgressIndicatorEventHandler(dataContextSource_topTenBenchmarkSecuritiesDataLoadedEvent);
            this.DataContextTopBenchmarkSecurities.Dispose();
            this.DataContextTopBenchmarkSecurities = null;
            this.DataContext = null;
        }
        #endregion      
    }
}
