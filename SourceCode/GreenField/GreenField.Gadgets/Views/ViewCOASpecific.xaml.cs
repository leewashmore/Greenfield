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

namespace GreenField.Gadgets.Views
{
    public partial class ViewCOASpecific : ViewBaseUserControl
    {
        public ViewCOASpecific(ViewModelCOASpecific dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
        }

        public override void Dispose()
        {
            (this.DataContext as ViewModelConsensusEstimatesDetails).Dispose();
            this.DataContext = null;
        }

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
        #region Export
        private void btnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>();
            RadExportOptionsInfo.Add(new RadExportOptions() { ElementName = "Gadget With Period Columns COA Specific", Element = this.dgCOASpecific, ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXPORT_FILTER });           
            ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + GadgetNames.GADGET_WITH_PERIOD_COLUMNS_COA_SPECIFIC);
            childExportOptions.Show();
        }
        #endregion
        #endregion
    }
}
