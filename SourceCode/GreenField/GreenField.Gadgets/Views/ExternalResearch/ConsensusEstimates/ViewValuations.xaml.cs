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
using GreenField.ServiceCaller;

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
        /// Instance of EntitySelectionData
        /// </summary>
        private EntitySelectionData _entitySelectionData;
        private bool _periodIsYearly = true;

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
                if (DataContextValuations != null)
                    DataContextValuations.IsActive = _isActive;
            }
        }
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
                    PeriodColumns.UpdateColumnInformation(this.dgConsensusEstimateValuations, e);
                    this.btnExportExcel.IsEnabled = true;
                    this.dgConsensusEstimateValuations.Columns[0].Header = "Consensus Valuations in " + this.DataContextValuations.SelectedCurrency;
                }
            };
        }

        #endregion

        #region GridNavigationButtons
        /// <summary>
        /// Left Navigation Button Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LeftNavigation_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(this.DataContextValuations.Logger, methodNamespace);

            try
            {
                PeriodColumns.RaisePeriodColumnNavigationCompleted(new PeriodColumnNavigationEventArg()
                    {
                        PeriodColumnNamespace = typeof(ViewModelValuations).FullName,
                        PeriodColumnNavigationDirection = NavigationDirection.LEFT
                    });
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(this.DataContextValuations.Logger, ex);
            }
        }

        /// <summary>
        /// Right Navigation Button Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RightNavigation_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(this.DataContextValuations.Logger, methodNamespace);

            try
            {
                PeriodColumns.RaisePeriodColumnNavigationCompleted(new PeriodColumnNavigationEventArg()
                    {
                        PeriodColumnNamespace = typeof(ViewModelValuations).FullName,
                        PeriodColumnNavigationDirection = NavigationDirection.RIGHT
                    });
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(this.DataContextValuations.Logger, ex);
            }
        }
        #endregion

        #region EventsUnsusbcribe
        /// <summary>
        /// Dispose method to unsubscribe Events
        /// </summary>
        public override void Dispose()
        {
            (this.DataContext as ViewModelValuations).Dispose();
            this.DataContext = null;
        }
        #endregion

        #region Export
        /// <summary>
        /// Excel exporting EventHandler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgConsensusEstimateValuations_ElementExporting(object sender, Telerik.Windows.Controls.GridViewElementExportingEventArgs e)
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
            Logging.LogBeginMethod(this.DataContextValuations.Logger, methodNamespace);
            try
            {
                List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>();
                String elementName = "Consensus Estimate - " + (this.DataContextValuations.EntitySelectionInfo).LongName + " (" + (this.DataContextValuations.EntitySelectionInfo).ShortName + ") " +
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
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(this.DataContextValuations.Logger, ex);
            }
        }
        #endregion

    }
}
