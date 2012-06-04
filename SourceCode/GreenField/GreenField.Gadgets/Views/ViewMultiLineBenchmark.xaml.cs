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
using Telerik.Windows.Controls;
using GreenField.ServiceCaller;

namespace GreenField.Gadgets.Views
{
    /// <summary>
    /// Code-Behind for Multi-LineBenchmark UI
    /// </summary>
    public partial class ViewMultiLineBenchmark : ViewBaseUserControl
    {
        #region Variables

        /// <summary>
        /// Export Types
        /// </summary>
        private static class ExportTypes
        {
            public const string MULTI_LINE_CHART = "Multi-Line Benchmark Chart";
            public const string MULTI_LINE_GRID = "Multi- Line Benchmark Grid";
        }

        #endregion

        #region PropertyDeclaration

        /// <summary>
        /// Property of type View-Model
        /// </summary>
        private ViewModelMultiLineBenchmark _dataContextMultilineBenchmark;
        public ViewModelMultiLineBenchmark DataContextMultilineBenchmark
        {
            get
            {
                return _dataContextMultilineBenchmark;
            }
            set
            {
                _dataContextMultilineBenchmark = value;
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor that initialises the class
        /// </summary>
        /// <param name="dataContextSource"></param>
        public ViewMultiLineBenchmark(ViewModelMultiLineBenchmark dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextMultilineBenchmark = dataContextSource;
            dataContextSource.ChartAreaMultiLineBenchmark = this.chMultiLineBenchmarkChart.DefaultView.ChartArea;
            this.chMultiLineBenchmarkChart.DataBound += dataContextSource.ChartDataBound;
            dataContextSource.MultiLineBenchmarkDataLoadedEvent += new DataRetrievalProgressIndicatorEventHandler(dataContextSource_MultiLineBenchmarkDataLoadedEvent);
            AddGridHeader();
        }
        
        #endregion

        #region ProgressIndicator

        /// <summary>
        /// Data Progress Indicator Event
        /// </summary>
        /// <param name="e"></param>
        void dataContextSource_MultiLineBenchmarkDataLoadedEvent(DataRetrievalProgressIndicatorEventArgs e)
        {
            if (e.ShowBusy)
            {
                this.busyIndicator.IsBusy = true;
            }
            else
            {
                this.busyIndicator.IsBusy = false;
            }
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
                List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>
                {
                    new RadExportOptions() { ElementName = ExportTypes.MULTI_LINE_CHART, Element = this.chMultiLineBenchmarkChart, ExportFilterOption = RadExportFilterOption.RADCHART_EXPORT_FILTER },
                    new RadExportOptions() { ElementName = ExportTypes.MULTI_LINE_GRID, Element = this.dgBenchmarkUI, ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXPORT_FILTER },
                };
                ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + GadgetNames.SECURITY_REFERENCE_PRICE_COMPARISON);
                childExportOptions.Show();
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog(ex.Message);
            }
        }

        #endregion

        #region EventsUnSubscribe

        /// <summary>
        /// Disposing off events and Event Handlers
        /// </summary>
        public override void Dispose()
        {

        } 

        #endregion

        #region HelperMethods

        /// <summary>
        /// Dynamically setting GridColumn Headers
        /// </summary>
        private void AddGridHeader()
        {
            dgBenchmarkUI.Columns[4].Header = DateTime.Today.Year.ToString();
            dgBenchmarkUI.Columns[5].Header = DateTime.Today.AddYears(-1).Year.ToString();
            dgBenchmarkUI.Columns[6].Header = DateTime.Today.AddYears(-2).Year.ToString();

        }

        #endregion
    }
}
