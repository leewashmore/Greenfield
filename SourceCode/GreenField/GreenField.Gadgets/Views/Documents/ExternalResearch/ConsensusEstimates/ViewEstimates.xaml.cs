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
using GreenField.Gadgets.Models;

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
            this.DataContextEstimates = dataContextSource;

            PeriodRecord periodRecord = PeriodColumns.SetPeriodRecord(defaultHistoricalYearCount: 2, defaultHistoricalQuarterCount: 2, netColumnCount: 5);
            PeriodColumns.UpdateColumnInformation(this.dgConsensusEstimate, new PeriodColumnUpdateEventArg()
            {
                PeriodRecord = periodRecord,
                PeriodColumnHeader = PeriodColumns.SetColumnHeaders(periodRecord, displayPeriodType: false),
                PeriodIsYearly = true
            });

            PeriodColumns.PeriodColumnUpdate += (e) =>
            {
                if (e.PeriodColumnNamespace == typeof(ViewModelEstimates).FullName)
                {
                    PeriodColumns.UpdateColumnInformation(this.dgConsensusEstimate, e);
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
        /// To check whether the Dashboard is Active or not
        /// </summary>
        private bool _isActive;
        public override bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
                if (DataContextEstimates != null)
                    DataContextEstimates.IsActive = _isActive;
            }
        }

        /// <summary>
        /// Instance of ViewModelEstimates
        /// </summary>
        private ViewModelEstimates _dataContextEstimates;
        public ViewModelEstimates DataContextEstimates
        {
            get { return _dataContextEstimates; }
            set { _dataContextEstimates = value; }
        }


        private void LeftNavigation_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PeriodColumns.RaisePeriodColumnNavigationCompleted(new PeriodColumnNavigationEventArg()
            {
                PeriodColumnNamespace = typeof(ViewModelEstimates).FullName,
                PeriodColumnNavigationDirection = NavigationDirection.LEFT
            });
        }

        private void RightNavigation_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PeriodColumns.RaisePeriodColumnNavigationCompleted(new PeriodColumnNavigationEventArg()
            {
                PeriodColumnNamespace = typeof(ViewModelEstimates).FullName,
                PeriodColumnNavigationDirection = NavigationDirection.RIGHT
            });
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

        /// <summary>
        /// Export to Excel Button Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>();
            String elementName = "Consensus Estimate - " + (this.DataContextEstimates.EntitySelectionInfo).LongName + " (" + (this.DataContextEstimates.EntitySelectionInfo).ShortName + ") " +
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
