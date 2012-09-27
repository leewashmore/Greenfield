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
using Telerik.Windows.Controls.GridView;
using Telerik.Windows.Controls;

namespace GreenField.Gadgets.Views
{
    public partial class ViewCOASpecific : ViewBaseUserControl
    {
        public ViewCOASpecific(ViewModelCOASpecific dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;

            //Update column headers and visibility
            PeriodRecord periodRecord = PeriodColumns.SetPeriodRecord(0, 3, 4, 6, false);
            PeriodColumns.UpdateColumnInformation(this.dgCOASpecific, new PeriodColumnUpdateEventArg()
            {
                PeriodColumnNamespace = typeof(ViewModelCOASpecific).FullName,
                PeriodRecord = periodRecord,
                PeriodColumnHeader = PeriodColumns.SetColumnHeaders(periodRecord),
                PeriodIsYearly = true                
            }, false);

            dataContextSource.coaSpecificDataLoadedEvent +=
          new DataRetrievalProgressIndicatorEventHandler(dataContextSource_coaSpecificDataLoadedEvent);

            //Event Subcription - PeriodColumnUpdateEvent
            PeriodColumns.PeriodColumnUpdate += new PeriodColumnUpdateEvent(PeriodColumns_PeriodColumnUpdate);
            this.grdRadChart.Visibility = Visibility.Collapsed;
            this.grdRadGridView.Visibility = Visibility.Visible;
            this.txtADD.Visibility = Visibility.Collapsed;
            this.cmbAddSeries.Visibility = Visibility.Collapsed;
            this.btnAddToChart.Visibility = Visibility.Collapsed;
            this.itemDel.Visibility = Visibility.Collapsed;
            this.txtGadgetName.Visibility = Visibility.Collapsed;
            this.cbGadgetName.Visibility = Visibility.Collapsed;
            ApplyChartStyles();

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
            
            if (this.grdRadChart.Visibility == Visibility.Visible)
            {
                List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>();
                RadExportOptionsInfo.Add(new RadExportOptions() { ElementName = "Gadget With Period Columns COA Specific", Element = this.chCOASpecific, ExportFilterOption = RadExportFilterOption.RADCHART_EXPORT_FILTER });
                ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + GadgetNames.GADGET_WITH_PERIOD_COLUMNS_COA_SPECIFIC);
                childExportOptions.Show();
            }
            else
            {
                if (this.grdRadGridView.Visibility == Visibility.Visible)
                {
                    List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>();
                    RadExportOptionsInfo.Add(new RadExportOptions() { ElementName = "Gadget With Period Columns COA Specific", Element = this.dgCOASpecific, ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXPORT_FILTER });
                    ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + GadgetNames.GADGET_WITH_PERIOD_COLUMNS_COA_SPECIFIC);
                    childExportOptions.Show();
                }
            }
        }

        private void dgCOASpecific_ElementExporting(object sender, GridViewElementExportingEventArgs e)
        {
            RadGridView_ElementExport.ElementExporting(e, hideColumnIndex: new List<int> { 1, 8 });
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

        /// <summary>
        /// Flipping between Grid & Chart
        /// Using the method FlipItem in class Flipper.cs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFlip_Click(object sender, RoutedEventArgs e)
        {
            if (this.grdRadGridView.Visibility == Visibility.Visible)
            {
                Flipper.FlipItem(this.grdRadGridView, this.grdRadChart);
            }
            else
            Flipper.FlipItem(this.grdRadChart, this.grdRadGridView);

            if (this.grdRadGridView.Visibility == Visibility.Visible)
            {
                this.txtADD.Visibility = Visibility.Visible;
                this.cmbAddSeries.Visibility = Visibility.Visible;
                this.btnAddToChart.Visibility = Visibility.Visible;
                this.itemDel.Visibility = Visibility.Visible;
                this.txtGadgetName.Visibility = Visibility.Visible;
                this.cbGadgetName.Visibility = Visibility.Visible;
            }

            else
            {
                this.txtADD.Visibility = Visibility.Collapsed;
                this.cmbAddSeries.Visibility = Visibility.Collapsed;
                this.btnAddToChart.Visibility = Visibility.Collapsed;
                this.itemDel.Visibility = Visibility.Collapsed;
                this.txtGadgetName.Visibility = Visibility.Collapsed;
                this.cbGadgetName.Visibility = Visibility.Collapsed;
            
            }
            
            

        }

     

        /// <summary>
        /// Data Retrieval Indicator
        /// </summary>
        /// <param name="e"></param>
        void dataContextSource_coaSpecificDataLoadedEvent(DataRetrievalProgressIndicatorEventArgs e)
        {
            //if (e.ShowBusy)
            //{
            //    this.busyIndicatorChart.IsBusy = true;
            //    this.busyIndicatorGrid.IsBusy = true;
            //}
            //else
            //{
            //    this.busyIndicatorChart.IsBusy = false;
            //    this.busyIndicatorGrid.IsBusy = false;
            //}
        }

        #region Styling

        /// <summary>
        /// Formatting the chart
        /// </summary>
        private void ApplyChartStyles()
        {
            this.chCOASpecific.DefaultView.ChartArea.AxisX.AxisStyles.ItemLabelStyle = this.Resources["ItemLabelStyle"] as Style;
            this.chCOASpecific.DefaultView.ChartArea.AxisY.AxisStyles.ItemLabelStyle = this.Resources["ItemLabelStyle"] as Style;
            this.chCOASpecific.DefaultView.ChartArea.AxisX.AxisStyles.ItemLabelStyle = this.Resources["ItemLabelStyle"] as Style;
            this.chCOASpecific.DefaultView.ChartArea.AxisY.AxisStyles.ItemLabelStyle = this.Resources["ItemLabelStyle"] as Style;
            this.chCOASpecific.DefaultView.ChartLegend.Style = this.Resources["ChartLegendStyle"] as Style;            
        }

       
        private void dgCOASpecific_RowLoaded(object sender, RowLoadedEventArgs e)
        {
            PeriodColumns.RowDataCustomizationforCOASpecificGadget(e);
        }
       
        #endregion



    }
}
