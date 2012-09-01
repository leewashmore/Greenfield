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
using GreenField.Common;
using GreenField.Gadgets.Helpers;
using Telerik.Windows.Controls.GridView;
using Telerik.Windows.Controls;
using GreenField.Gadgets.Models;
using GreenField.DataContracts;

namespace GreenField.Gadgets.Views
{
    public partial class ViewFinancialStatements : ViewBaseUserControl
    {
        #region Constructor
        public ViewFinancialStatements(ViewModelFinancialStatements dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            DataContextSource = dataContextSource;

            //Update column headers and visibility
            PeriodRecord periodRecord = PeriodColumns.SetPeriodRecord();
            PeriodColumns.UpdateColumnInformation(this.dgFinancialReport, new PeriodColumnUpdateEventArg()
            {
                PeriodColumnNamespace = typeof(ViewModelFinancialStatements).FullName,
                PeriodRecord = periodRecord,
                PeriodColumnHeader = PeriodColumns.SetColumnHeaders(periodRecord),
                PeriodIsYearly = true
            });
            
            //Event Subcription - PeriodColumnUpdateEvent
            PeriodColumns.PeriodColumnUpdate += new PeriodColumnUpdateEvent(PeriodColumns_PeriodColumnUpdate);
        }
        #endregion        

        #region Properties
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
                if (DataContextSource != null) //DataContext instance
                    DataContextSource.IsActive = _isActive;
            }
        }

        /// <summary>
        /// View model class object
        /// </summary>
        public ViewModelFinancialStatements DataContextSource { get; set; }
        #endregion

        #region Event Handlers
        #region Navigation
        /// <summary>
        /// Left navigation button click event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LeftNavigation_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PeriodColumns.RaisePeriodColumnNavigationCompleted(new PeriodColumnNavigationEventArg()
            {
                PeriodColumnNamespace = typeof(ViewModelFinancialStatements).FullName,
                PeriodColumnNavigationDirection = NavigationDirection.LEFT
            });
        }

        /// <summary>
        /// Right navigation button click event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RightNavigation_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PeriodColumns.RaisePeriodColumnNavigationCompleted(new PeriodColumnNavigationEventArg()
            {
                PeriodColumnNamespace = typeof(ViewModelFinancialStatements).FullName,
                PeriodColumnNavigationDirection = NavigationDirection.RIGHT
            });
        } 
        #endregion

        /// <summary>
        /// PeriodColumnUpdateEvent Event Handler - Updates column information and enables export button first time data is received
        /// </summary>
        /// <param name="e">PeriodColumnUpdateEventArg</param>
        void PeriodColumns_PeriodColumnUpdate(PeriodColumnUpdateEventArg e)
        {
            if (e.PeriodColumnNamespace == typeof(ViewModelFinancialStatements).FullName && IsActive)
            {
                PeriodColumns.UpdateColumnInformation(this.dgFinancialReport, e, isQuarterImplemented: true);
                PeriodColumns.UpdateColumnInformation(this.dgFinancialReportExt, e, isQuarterImplemented: true);
                this.btnExportExcel.IsEnabled = true;
            }
        }

        #region Styling
        private void dgFinancialReport_RowLoaded(object sender, RowLoadedEventArgs e)
        {
            //GroupedGridRowLoadedHandler.Implement(e);
            PeriodColumns.RowDataCustomization(e);
        } 
        #endregion

        #region Export
        private void dgFinancialReport_ElementExporting(object sender, GridViewElementExportingEventArgs e)
        {
            RadGridView_ElementExport.ElementExporting(e, hideColumnIndex: new List<int> { 1, 14 });
        }

        private void dgFinancialReportExt_ElementExporting(object sender, GridViewElementExportingEventArgs e)
        {
            RadGridView_ElementExport.ElementExporting(e, hideColumnIndex: new List<int> { 1, 14 }, headerCellValueConverter: () =>
            {
                if (e.Value == null)
                    return null;
                return e.Value.ToString().Replace("A", "E");
            });
        }

        private void btnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>();

            RadExportOptionsInfo.Add(new RadExportOptions() { ElementName = "Financial Statement Data", Element = this.dgFinancialReport, ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXPORT_FILTER });
            RadExportOptionsInfo.Add(new RadExportOptions() { ElementName = "External Research Data", Element = this.dgFinancialReportExt, ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXPORT_FILTER });

            ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: Financial Statements");
            childExportOptions.Show();        
        }
        #endregion
        #endregion

        #region Event Unsubscribe
        public override void Dispose()
        {
            PeriodColumns.PeriodColumnUpdate -= new PeriodColumnUpdateEvent(PeriodColumns_PeriodColumnUpdate);

            (this.DataContext as ViewModelFinancialStatements).Dispose();
            this.DataContext = null;
        }
        #endregion       
    }


}
