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

namespace GreenField.Gadgets.Views
{
    public partial class ViewQuarterlyResultsComparison : ViewBaseUserControl
    {
        #region Constructor
        public ViewQuarterlyResultsComparison(ViewModelQuarterlyResultsComparison dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextQuarterlyResultsComparison = dataContextSource; 
            dataContextSource.quarterlyResultsComoarisonDataLoadedEvent +=
           new DataRetrievalProgressIndicatorEventHandler(dataContextSource_quarterlyResultsComoarisonDataLoadedEvent);
        }
        #endregion

        /// <summary>
        /// Data Retrieval Indicator
        /// </summary>
        /// <param name="e"></param>
        void dataContextSource_quarterlyResultsComoarisonDataLoadedEvent(DataRetrievalProgressIndicatorEventArgs e)
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

        #region RemoveEvents
        /// <summary>
        /// Disposing events
        /// </summary>
        public override void Dispose()
        {
            this.DataContextQuarterlyResultsComparison.quarterlyResultsComoarisonDataLoadedEvent -= new DataRetrievalProgressIndicatorEventHandler(dataContextSource_quarterlyResultsComoarisonDataLoadedEvent);            
            this.DataContextQuarterlyResultsComparison = null;
            this.DataContext = null;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Property of the type of View Model for this view
        /// </summary>
        private ViewModelQuarterlyResultsComparison _dataContextQuarterlyResultsComparison;
        public ViewModelQuarterlyResultsComparison DataContextQuarterlyResultsComparison
        {
            get { return _dataContextQuarterlyResultsComparison; }
            set { _dataContextQuarterlyResultsComparison = value; }
        }
        #endregion

        #region Export To Excel Methods
        private void btnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            ChildExportOptions childExportOptions = new ChildExportOptions
                (
                new List<RadExportOptions>
                {
                    new RadExportOptions() 
                    {
                        Element = this.dgQuarterlyComparison,
                        ElementName = "Quarterly Comparison Results",
                        ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXPORT_FILTER
                    } 
                }, "Export Options: " + GadgetNames.QUARTERLY_RESULTS_COMPARISON);
            childExportOptions.Show();
        }
        private void dgQuarterlyResults_ElementExporting(object sender, GridViewElementExportingEventArgs e)
        {
            RadGridView_ElementExport.ElementExporting(e, showGroupFooters: false);
        }

        #endregion

    }
}
