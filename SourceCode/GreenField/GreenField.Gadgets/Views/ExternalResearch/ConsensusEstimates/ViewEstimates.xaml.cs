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
using GreenField.ServiceCaller;

namespace GreenField.Gadgets.Views
{
    /// <summary>
    /// Code-Behind class for ConsensusGadgets-Estimates
    /// </summary>
    public partial class ViewEstimates : ViewBaseUserControl
    {
        #region Constructor
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
                    this.dgConsensusEstimate.Columns[0].Header = "Median Estimates in " + this.DataContextEstimates.SelectedCurrency + "(Millions)";
                }
            };
        }
        #endregion

        #region ActiveDashboard
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
        #endregion

        #region PropertyDeclaration
        /// <summary>
        /// Instance of ViewModelEstimates
        /// </summary>
        private ViewModelEstimates _dataContextEstimates;
        public ViewModelEstimates DataContextEstimates
        {
            get { return _dataContextEstimates; }
            set { _dataContextEstimates = value; }
        }

        /// <summary>
        /// Instance of EntitySelectionData
        /// </summary>
        private EntitySelectionData _entitySelectionData;
        private bool _periodIsYearly = true;

        #endregion

        #region GridMovementHandlers
        /// <summary>
        /// Left Navigation button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LeftNavigation_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(this.DataContextEstimates._logger, methodNamespace);
            try
            {

                PeriodColumns.RaisePeriodColumnNavigationCompleted(new PeriodColumnNavigationEventArg()
                {
                    PeriodColumnNamespace = typeof(ViewModelEstimates).FullName,
                    PeriodColumnNavigationDirection = NavigationDirection.LEFT
                });
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(this.DataContextEstimates._logger, ex);
            }
        }

        /// <summary>
        /// Right-Navigation button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RightNavigation_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(this.DataContextEstimates._logger, methodNamespace);
            try
            {

                PeriodColumns.RaisePeriodColumnNavigationCompleted(new PeriodColumnNavigationEventArg()
                {
                    PeriodColumnNamespace = typeof(ViewModelEstimates).FullName,
                    PeriodColumnNavigationDirection = NavigationDirection.RIGHT
                });
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(this.DataContextEstimates._logger, ex);
            }
        }
        #endregion

        #region EventsUnsusbcribe
        /// <summary>
        /// Dispose method to unsubscribe Events
        /// </summary>
        public override void Dispose()
        {
            (this.DataContext as ViewModelEstimates).Dispose();
            this.DataContext = null;
        }
        #endregion

        #region Export
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
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(this.DataContextEstimates._logger, methodNamespace);
            try
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
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(this.DataContextEstimates._logger, ex);
            }
        }
        #endregion

    }
}
