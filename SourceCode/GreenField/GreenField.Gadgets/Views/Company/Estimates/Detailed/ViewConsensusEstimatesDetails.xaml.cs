using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Logging;
using GreenField.Common;
using GreenField.Gadgets.Helpers;
using GreenField.Gadgets.Models;
using GreenField.Gadgets.ViewModels;
using GreenField.ServiceCaller;

namespace GreenField.Gadgets.Views
{
    /// <summary>
    /// class for view for ConsensusEstimatesDetails
    /// </summary>
    public partial class ViewConsensusEstimatesDetails : ViewBaseUserControl
    {
        /// <summary>
        /// private fields
        /// </summary>
        private IEventAggregator eventAggregator;
        private ILoggerFacade logger;

        #region Properties
        /// <summary>
        /// property to set data context
        /// </summary>
        private ViewModelConsensusEstimatesDetails dataContextConsensusEstimatesDetails;
        public ViewModelConsensusEstimatesDetails DataContextConsensusEstimatesDetails
        {
            get { return dataContextConsensusEstimatesDetails; }
            set { dataContextConsensusEstimatesDetails = value; }
        }

        /// <summary>
        /// property to set IsActive variable of View Model
        /// </summary>
        private bool isActive;
        public override bool IsActive
        {
            get { return isActive; }
            set
            {
                isActive = value;
                if (DataContextConsensusEstimatesDetails != null)
                { DataContextConsensusEstimatesDetails.IsActive = isActive; }
            }
        }

        #endregion

        #region Constructor
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="dataContextSource"></param>
        public ViewConsensusEstimatesDetails(ViewModelConsensusEstimatesDetails dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            DataContextConsensusEstimatesDetails = dataContextSource;
            eventAggregator = (this.DataContext as ViewModelConsensusEstimatesDetails).eventAggregator;
            logger = (this.DataContext as ViewModelConsensusEstimatesDetails).logger;

            eventAggregator.GetEvent<ConsensusEstimateDetailCurrencyChangeEvent>().Subscribe(HandleConsensusEstimateDetailCurrencyChangeEvent);

            PeriodRecord periodRecord = PeriodColumns.SetPeriodRecord(defaultHistoricalYearCount: 2, defaultHistoricalQuarterCount: 2, netColumnCount: 5);
            PeriodColumns.UpdateColumnInformation(this.dgConsensusEstimate, new PeriodColumnUpdateEventArg()
            {
                PeriodRecord = periodRecord,
                PeriodColumnHeader = PeriodColumns.SetColumnHeaders(periodRecord, displayPeriodType: false),
                PeriodIsYearly = true
            });

            dgConsensusEstimate.Columns[0].Header = "Median Estimates in " + dataContextSource.SelectedCurrency.ToString() + "(Millions)";

            PeriodColumns.PeriodColumnUpdate += (e) =>
            {
                if (e.PeriodColumnNamespace == typeof(ViewModelConsensusEstimatesDetails).FullName)
                {
                    PeriodColumns.UpdateColumnInformation(this.dgConsensusEstimate, e, false);
                    this.btnExportExcel.IsEnabled = true;
                }
            };
        }
        #endregion

        #region GridMovementHandlers
        /// <summary>
        /// Left Navigation button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LeftNavigation_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PeriodColumns.RaisePeriodColumnNavigationCompleted(new PeriodColumnNavigationEventArg()
            {
                PeriodColumnNamespace = typeof(ViewModelConsensusEstimatesDetails).FullName,
                PeriodColumnNavigationDirection = NavigationDirection.LEFT
            });
        }

        /// <summary>
        /// Right-Navigation button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RightNavigation_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PeriodColumns.RaisePeriodColumnNavigationCompleted(new PeriodColumnNavigationEventArg()
            {
                PeriodColumnNamespace = typeof(ViewModelConsensusEstimatesDetails).FullName,
                PeriodColumnNavigationDirection = NavigationDirection.RIGHT
            });
        }
        #endregion

        #region Events
        /// <summary>
        /// Event Handler to subscribed event 'RelativePerformanceGridClickEvent'
        /// </summary>
        /// <param name="relativePerformanceGridCellData">RelativePerformanceGridCellData</param>
        public void HandleConsensusEstimateDetailCurrencyChangeEvent(ChangedCurrencyInEstimateDetail currency)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(logger, methodNamespace);
            try
            {
                if (currency.CurrencyName != null)
                {
                    Logging.LogMethodParameter(logger, methodNamespace, currency, 1);
                    dgConsensusEstimate.Columns[0].Header = "Median Estimates in " + currency.CurrencyName.ToString() + "(Millions)";
                }
                else
                {
                    dgConsensusEstimate.Columns[0].Header = "Median Estimates in USD(Millions)";
                    Logging.LogMethodParameterNull(logger, methodNamespace, 1);
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(logger, ex);
            }
            Logging.LogEndMethod(logger, methodNamespace);
        }
        #endregion

        #region Export to excel
        /// <summary>
        /// handles element exporting when exported to excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgConsensusEstimate_ElementExporting(object sender, Telerik.Windows.Controls.GridViewElementExportingEventArgs e)
        {
            RadGridView_ElementExport.ElementExporting(e, isGroupFootersVisible: false);
            RadGridView_ElementExport.ElementExporting(e, hideColumnIndex: new List<int> { 1, 12 });
        }

        /// <summary>
        /// catch export to excel button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(this.DataContextConsensusEstimatesDetails.logger, methodNamespace);
            try
            {
                List<RadExportOptions> radExportOptionsInfo = new List<RadExportOptions>();
                radExportOptionsInfo.Add(new RadExportOptions()
                {
                    ElementName = "Consensus Estimate Detail",
                    Element = this.dgConsensusEstimate,
                    ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXPORT_FILTER
                });
                ChildExportOptions childExportOptions = new ChildExportOptions(radExportOptionsInfo, "Export Options: Consensus Estimate Detail");
                childExportOptions.Show();
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(this.DataContextConsensusEstimatesDetails.logger, ex);
            }
        }
        #endregion

        #region Dispose method
        /// <summary>
        /// method to dispose all running events
        /// </summary>
        public override void Dispose()
        {
            (this.DataContext as ViewModelConsensusEstimatesDetails).Dispose();
            this.DataContextConsensusEstimatesDetails = null;
            eventAggregator.GetEvent<ConsensusEstimateDetailCurrencyChangeEvent>().Unsubscribe(HandleConsensusEstimateDetailCurrencyChangeEvent);
            this.DataContext = null;
        }
        #endregion
    }
}