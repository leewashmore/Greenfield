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
using GreenField.DataContracts;
using GreenField.Common;
using GreenField.Gadgets.Models;

namespace GreenField.Gadgets.Views
{
    /// <summary>
    /// Code-Behind for ConsensusEstimates-Valuations Gadget
    /// </summary>
    public partial class ViewValuations : ViewBaseUserControl
    {
        #region PropertyDeclaration

        /// <summary>
        /// Property of View-Model
        /// </summary>
        private ViewModelValuations _dataContextValuations;
        public ViewModelValuations DataContextValuations
        {
            get { return _dataContextValuations; }
            set { _dataContextValuations = value; }
        }

        /// <summary>
        /// To check whether the Dashboard is Active or not
        /// </summary>
        private bool _isActive;
        public override bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
                if (DataContextValuations != null)
                    DataContextValuations.IsActive = _isActive;
            }
        }

        /// <summary>
        /// Instance of EntitySelectionData
        /// </summary>
        private EntitySelectionData _entitySelectionData;
        private bool _periodIsYearly = true;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataContextSource"></param>
        public ViewValuations(ViewModelValuations dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextValuations = dataContextSource;
            InitializeComponent();
            this.DataContext = dataContextSource;

            PeriodRecord periodRecord = PeriodColumns.SetPeriodRecord(defaultHistoricalYearCount: 2, defaultHistoricalQuarterCount: 2, netColumnCount: 5);
            PeriodColumns.UpdateColumnInformation(this.dgConsensusEstimateValuations, new PeriodColumnUpdateEventArg()
            {
                PeriodRecord = periodRecord,
                PeriodColumnHeader = PeriodColumns.SetColumnHeaders(periodRecord, displayPeriodType: false),
                PeriodIsYearly = true
            });

            PeriodColumns.PeriodColumnUpdate += (e) =>
            {
                if (e.PeriodColumnNamespace == typeof(ViewModelValuations).FullName)
                {
                    PeriodColumns.UpdateColumnInformation(this.dgConsensusEstimateValuations, e, false);
                    this.btnExportExcel.IsEnabled = true;
                }
            };
        }

        /// <summary>
        /// Left Navigation Button Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LeftNavigation_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PeriodColumns.RaisePeriodColumnNavigationCompleted(new PeriodColumnNavigationEventArg()
            {
                PeriodColumnNamespace = typeof(ViewModelValuations).FullName,
                PeriodColumnNavigationDirection = NavigationDirection.LEFT
            });
        }

        /// <summary>
        /// Right Navigation Button Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RightNavigation_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PeriodColumns.RaisePeriodColumnNavigationCompleted(new PeriodColumnNavigationEventArg()
            {
                PeriodColumnNamespace = typeof(ViewModelValuations).FullName,
                PeriodColumnNavigationDirection = NavigationDirection.RIGHT
            });
        }

        /// <summary>
        /// Dispose method to unsubscribe Events
        /// </summary>
        public override void Dispose()
        {
            (this.DataContext as ViewModelValuations).Dispose();
            this.DataContext = null;
        }

        /// <summary>
        /// Excel exporting EventHandler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgConsensusEstimateValuations_ElementExporting(object sender, Telerik.Windows.Controls.GridViewElementExportingEventArgs e)
        {
            RadGridView_ElementExport.ElementExporting(e, hideColumnIndex: new List<int> { 1, 12 });
        }

        private void dgConsensusEstimateValuations_RowLoaded(object sender, Telerik.Windows.Controls.GridView.RowLoadedEventArgs e)
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
                (_periodIsYearly ? this.dgConsensusEstimateValuations.Columns[2].Header : this.dgConsensusEstimateValuations.Columns[6].Header) + " - " +
                (_periodIsYearly ? this.dgConsensusEstimateValuations.Columns[7].Header : this.dgConsensusEstimateValuations.Columns[11].Header);
            RadExportOptionsInfo.Add(new RadExportOptions()
            {
                ElementName = elementName,
                Element = this.dgConsensusEstimateValuations
                ,
                ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXPORT_FILTER
            });

            ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + GadgetNames.EXTERNAL_RESEARCH_CONSENSUS_MEDIAN_ESTIMATES);
            childExportOptions.Show();
        }

        #endregion
    }
}
