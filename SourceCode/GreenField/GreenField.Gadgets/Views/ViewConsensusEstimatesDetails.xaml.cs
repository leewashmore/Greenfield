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
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Logging;
using GreenField.ServiceCaller;

namespace GreenField.Gadgets.Views
{
    public partial class ViewConsensusEstimatesDetails : ViewBaseUserControl
    {
        private IEventAggregator _eventAggregator;
        private ILoggerFacade _logger;

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

        #region Constructor
        public ViewConsensusEstimatesDetails(ViewModelConsensusEstimatesDetails dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            DataContextConsensusEstimatesDetails = dataContextSource;
            _eventAggregator = (this.DataContext as ViewModelConsensusEstimatesDetails)._eventAggregator;
            _logger = (this.DataContext as ViewModelConsensusEstimatesDetails)._logger;

            _eventAggregator.GetEvent<ConsensusEstimateDetailCurrencyChangeEvent>().Subscribe(HandleConsensusEstimateDetailCurrencyChangeEvent);

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
            this.DataContextConsensusEstimatesDetails = null;
            _eventAggregator.GetEvent<ConsensusEstimateDetailCurrencyChangeEvent>().Unsubscribe(HandleConsensusEstimateDetailCurrencyChangeEvent);
            this.DataContext = null;
        }

        /// <summary>
        /// Event Handler to subscribed event 'RelativePerformanceGridClickEvent'
        /// </summary>
        /// <param name="relativePerformanceGridCellData">RelativePerformanceGridCellData</param>
        public void HandleConsensusEstimateDetailCurrencyChangeEvent(ChangedCurrencyInEstimateDetail currency)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (currency.CurrencyName != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, currency, 1);
                        dgConsensusEstimate.Columns[0].Header = "Median Estimates in " + currency.CurrencyName.ToString() + "(Millions)";
                }
                else
                {
                    dgConsensusEstimate.Columns[0].Header = "Median Estimates in USD(Millions)";
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        private void dgConsensusEstimate_RowLoaded(object sender, Telerik.Windows.Controls.GridView.RowLoadedEventArgs e)
        {
            
        }

        private void dgConsensusEstimate_ElementExporting(object sender, Telerik.Windows.Controls.GridViewElementExportingEventArgs e)
        {
            RadGridView_ElementExport.ElementExporting(e, showGroupFooters: false);
            RadGridView_ElementExport.ElementExporting(e, hideColumnIndex: new List<int> { 1, 12 });
        }

        private void btnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            //ExportExcel.ExportGridExcel(dgConsensusEstimate);
        }

    }
}
