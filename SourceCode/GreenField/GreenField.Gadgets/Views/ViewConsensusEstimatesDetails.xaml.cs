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
using GreenField.Gadgets.Models;

namespace GreenField.Gadgets.Views
{
    public partial class ViewConsensusEstimatesDetails : ViewBaseUserControl
    {
        #region Properties
        /// <summary>
        /// property to set data context
        /// </summary>
        private ViewModelConsensusEstimatesDetails _dataContextConsensusEstimatesDetails;
        public ViewModelConsensusEstimatesDetails DataContextConsensusEstimatesDetails
        {
            get { return _dataContextConsensusEstimatesDetails; }
            set { _dataContextConsensusEstimatesDetails = value; }
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
                if (DataContextConsensusEstimatesDetails != null) //DataContext instance
                    DataContextConsensusEstimatesDetails.IsActive = _isActive;
            }
        }
        #endregion

        public ViewConsensusEstimatesDetails(ViewModelConsensusEstimatesDetails dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            DataContextConsensusEstimatesDetails = dataContextSource;

            PeriodRecord periodRecord = PeriodColumns.SetPeriodRecord(defaultHistoricalYearCount: 2, defaultHistoricalQuarterCount: 2, netColumnCount: 5);
            PeriodColumns.UpdateColumnInformation(this.dgConsensusEstimate, new PeriodColumnUpdateEventArg()
            {
                PeriodRecord = periodRecord,
                PeriodColumnHeader = PeriodColumns.SetColumnHeaders(periodRecord, displayPeriodType: false),
                PeriodIsYearly = true
            });

            PeriodColumns.PeriodColumnUpdate += (e) =>
            {
                if (e.PeriodColumnNamespace == typeof(ViewModelConsensusEstimatesDetails).FullName)
                {
                    PeriodColumns.UpdateColumnInformation(this.dgConsensusEstimate, e, false);
                    this.btnExportExcel.IsEnabled = true;
                }
            };
        }

        private void LeftNavigation_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PeriodColumns.RaisePeriodColumnNavigationCompleted(new PeriodColumnNavigationEventArg()
            {
                PeriodColumnNamespace = typeof(ViewModelConsensusEstimatesDetails).FullName,
                PeriodColumnNavigationDirection = NavigationDirection.LEFT
            });
        }

        private void RightNavigation_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PeriodColumns.RaisePeriodColumnNavigationCompleted(new PeriodColumnNavigationEventArg()
            {
                PeriodColumnNamespace = typeof(ViewModelConsensusEstimatesDetails).FullName,
                PeriodColumnNavigationDirection = NavigationDirection.RIGHT
            });
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
            
            RadExportOptionsInfo.Add(new RadExportOptions()
            {
                ElementName = "Consensus Estimate Details",
                Element = this.dgConsensusEstimate
                ,
                ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXPORT_FILTER
            });

            ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + GadgetNames.EXTERNAL_RESEARCH_CONSENSUS_DETAIL);
            childExportOptions.Show();
        }
    }
}
