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
using GreenField.Gadgets.Models;

namespace GreenField.Gadgets.Views
{
    public partial class ViewCOASpecific : ViewBaseUserControl
    {
        public ViewCOASpecific(ViewModelCOASpecific dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;

            //Update column headers and visibility
            PeriodRecord periodRecord = PeriodColumns.SetPeriodRecord();
            PeriodColumns.UpdateColumnInformation(this.dgCOASpecific, new PeriodColumnUpdateEventArg()
            {
                PeriodColumnNamespace = typeof(ViewModelCOASpecific).FullName,
                PeriodRecord = periodRecord,
                PeriodColumnHeader = PeriodColumns.SetColumnHeaders(periodRecord),
                PeriodIsYearly = true
            });

            //Event Subcription - PeriodColumnUpdateEvent
            PeriodColumns.PeriodColumnUpdate += new PeriodColumnUpdateEvent(PeriodColumns_PeriodColumnUpdate);
        }

       

        #region Properties
        /// <summary>
        /// True if gadget is currently on display
        /// </summary>
        private bool _isActive;
        public override bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
                if (this.DataContext != null)
                    ((ViewModelCOASpecific)this.DataContext).IsActive = _isActive;
            }
        }




        /// <summary>
        /// View model class object
        /// </summary>
        public ViewModelCOASpecific DataContextSource { get; set; }
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
                PeriodColumnNamespace = typeof(ViewModelCOASpecific).FullName,
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
                PeriodColumnNamespace = typeof(ViewModelCOASpecific).FullName,
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

        /// <summary>
        /// PeriodColumnUpdateEvent Event Handler - Updates column information and enables export button first time data is received
        /// </summary>
        /// <param name="e">PeriodColumnUpdateEventArg</param>
        void PeriodColumns_PeriodColumnUpdate(PeriodColumnUpdateEventArg e)
        {
            if (e.PeriodColumnNamespace == typeof(ViewModelCOASpecific).FullName && IsActive)
            {
                PeriodColumns.UpdateColumnInformation(this.dgCOASpecific, e, isQuarterImplemented: false);              
                //this.btnExportExcel.IsEnabled = true;
            }
        }

        #region Event Unsubscribe
        public override void Dispose()
        {
            PeriodColumns.PeriodColumnUpdate -= new PeriodColumnUpdateEvent(PeriodColumns_PeriodColumnUpdate);

            (this.DataContext as ViewModelCOASpecific).Dispose();
            this.DataContext = null;
        }
        #endregion



    }
}
