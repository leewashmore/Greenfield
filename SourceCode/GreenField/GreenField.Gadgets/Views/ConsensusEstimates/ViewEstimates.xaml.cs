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
using GreenField.Gadgets.Helpers;
using GreenField.Gadgets.ViewModels;
using GreenField.Common;
using GreenField.DataContracts;

namespace GreenField.Gadgets.Views
{
    /// <summary>
    /// Code-Behind class for ConsensusGadgets-Estimates
    /// </summary>
    public partial class ViewEstimates : ViewBaseUserControl
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataContextSource">Instance of ViewModel</param>
        public ViewEstimates(ViewModelEstimates dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;

            PeriodColumns.UpdateColumnInformation(this.dgConsensusEstimate, new PeriodColumns.PeriodColumnUpdateEventArg()
            {
                PeriodRecord = PeriodColumns.SetPeriodRecord(),
                PeriodColumnHeader = PeriodColumns.SetColumnHeaders(showHistorical: false),
                PeriodIsYearly = true
            }, false);

            PeriodColumns.PeriodColumnUpdate += (e) =>
            {
                if (e.PeriodColumnNamespace == typeof(ViewModelConsensusEstimatesDetails).FullName)
                {
                    PeriodColumns.UpdateColumnInformation(this.dgConsensusEstimate, e, false);
                    _entitySelectionData = e.EntitySelectionData;
                    _periodIsYearly = e.PeriodIsYearly;
                    this.btnExportExcel.IsEnabled = true;
                }
            };
        }

        /// <summary>
        /// Instance of EntitySelectionData
        /// </summary>
        private EntitySelectionData _entitySelectionData;
        private bool _periodIsYearly = true;
                
        /// <summary>
        /// Left Navigation Button Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LeftNavigation_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PeriodColumns.RaisePeriodColumnNavigationCompleted(new PeriodColumns.PeriodColumnNavigationEventArg()
            {
                PeriodColumnNamespace = typeof(ViewModelEstimates).FullName,
                PeriodColumnNavigationDirection = PeriodColumns.NavigationDirection.LEFT
            });
            e.Handled = true;
        }

        /// <summary>
        /// Right Navigation Button Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RightNavigation_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PeriodColumns.RaisePeriodColumnNavigationCompleted(new PeriodColumns.PeriodColumnNavigationEventArg()
            {
                PeriodColumnNamespace = typeof(ViewModelEstimates).FullName,
                PeriodColumnNavigationDirection = PeriodColumns.NavigationDirection.RIGHT
            });
            e.Handled = true;
        }

        /// <summary>
        /// Dispose method to unsubscribe Events
        /// </summary>
        public override void Dispose()
        {
            (this.DataContext as ViewModelEstimates).Dispose();
            this.DataContext = null;      
        }

        /// <summary>
        /// Excel exporting EventHandler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgConsensusEstimate_ElementExporting(object sender, Telerik.Windows.Controls.GridViewElementExportingEventArgs e)
        {
            RadGridView_ElementExport.ElementExporting(e, hideColumnIndex: new List<int> { 1, 12 });
        }

        private void dgConsensusEstimate_RowLoaded(object sender, Telerik.Windows.Controls.GridView.RowLoadedEventArgs e)
        {
            GroupedGridRowLoadedHandler.Implement(e);
        }

        /// <summary>
        /// Export to Excel Button Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>();
            String elementName = "Consensus Estimate - " + _entitySelectionData.LongName + " (" + _entitySelectionData.ShortName + ") " +
                (_periodIsYearly ? this.dgConsensusEstimate.Columns[2].Header : this.dgConsensusEstimate.Columns[6].Header) + " - " +
                (_periodIsYearly ? this.dgConsensusEstimate.Columns[7].Header : this.dgConsensusEstimate.Columns[11].Header);
            RadExportOptionsInfo.Add(new RadExportOptions()
            {
                ElementName = elementName,
                Element = this.dgConsensusEstimate
                ,
                ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXPORT_FILTER
            });

            ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + GadgetNames.EXTERNAL_RESEARCH_CONSENSUS_MEDIAN_ESTIMATES);
            childExportOptions.Show();
        }



    }
}
