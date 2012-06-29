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
using GreenField.DataContracts;
using GreenField.Common;

namespace GreenField.Gadgets.Views
{
    public partial class ViewConsensusEstimatesDetails : ViewBaseUserControl
    {
        private EntitySelectionData _entitySelectionData;
        private bool _periodIsYearly = true;
        
        public ViewConsensusEstimatesDetails(ViewModelConsensusEstimatesDetails dataContextSource)
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

        private void LeftNavigation_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PeriodColumns.RaisePeriodColumnNavigationCompleted(new PeriodColumns.PeriodColumnNavigationEventArg()
            {
                PeriodColumnNamespace = typeof(ViewModelConsensusEstimatesDetails).FullName,
                PeriodColumnNavigationDirection = PeriodColumns.NavigationDirection.LEFT
            });
            e.Handled = true;
        }

        private void RightNavigation_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PeriodColumns.RaisePeriodColumnNavigationCompleted(new PeriodColumns.PeriodColumnNavigationEventArg()
            {
                PeriodColumnNamespace = typeof(ViewModelConsensusEstimatesDetails).FullName,
                PeriodColumnNavigationDirection = PeriodColumns.NavigationDirection.RIGHT
            });
            e.Handled = true;
        }


        public override void Dispose()
        {
            (this.DataContext as ViewModelConsensusEstimatesDetails).Dispose();
            this.DataContext = null;      
  

        }

        private void dgConsensusEstimate_ElementExporting(object sender, Telerik.Windows.Controls.GridViewElementExportingEventArgs e)
        {
            RadGridView_ElementExport.ElementExporting(e, hideColumnIndex: new List<int> { 1, 12 });
        }

        private void dgConsensusEstimate_RowLoaded(object sender, Telerik.Windows.Controls.GridView.RowLoadedEventArgs e)
        {
            GroupedGridRowLoadedHandler.Implement(e);
        }

        private void btnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>();
            String elementName = "Consensus Estimate Details - " + _entitySelectionData.LongName + " (" + _entitySelectionData.ShortName + ") " +
                (_periodIsYearly ? this.dgConsensusEstimate.Columns[2].Header : this.dgConsensusEstimate.Columns[6].Header) + " - " +
                (_periodIsYearly ? this.dgConsensusEstimate.Columns[7].Header : this.dgConsensusEstimate.Columns[11].Header);
            RadExportOptionsInfo.Add(new RadExportOptions()
            {
                ElementName = elementName,
                Element = this.dgConsensusEstimate
                ,
                ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXPORT_FILTER
            });

            ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + GadgetNames.EXTERNAL_RESEARCH_CONSENSUS_DETAIL);
            childExportOptions.Show();
        }
    }
}
