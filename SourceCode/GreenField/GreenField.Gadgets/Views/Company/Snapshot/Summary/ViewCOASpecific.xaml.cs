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
using Telerik.Windows.Controls.GridView;
using Telerik.Windows.Controls;
using GreenField.Gadgets.Helpers;
using GreenField.Gadgets.ViewModels;
using GreenField.Common;
using GreenField.Gadgets.Models;
using GreenField.ServiceCaller;

namespace GreenField.Gadgets.Views
{
    public partial class ViewCOASpecific : ViewBaseUserControl
    {
        #region Constructor
        public ViewCOASpecific(ViewModelCOASpecific dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            DataContextSource = dataContextSource;
            //Update column headers and visibility
            PeriodRecord periodRecord = PeriodColumns.SetPeriodRecord(0, 3, 4, 6, false);
            PeriodColumns.UpdateColumnInformation(this.dgCOASpecific, new PeriodColumnUpdateEventArg()
            {
                PeriodColumnNamespace = typeof(ViewModelCOASpecific).FullName,
                PeriodRecord = periodRecord,
                PeriodColumnHeader = PeriodColumns.SetColumnHeaders(periodRecord),
                PeriodIsYearly = true                
            }, false);           
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
        #endregion

        #region Properties
        /// <summary>
        /// True if gadget is currently on display
        /// </summary>
        private bool isActive;
        public override bool IsActive
        {
            get { return isActive; }
            set
            {
                isActive = value;
                if (this.DataContext != null)
                {
                    ((ViewModelCOASpecific)this.DataContext).IsActive = isActive;
                }
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

        /// <summary>
        /// Export to excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportExcel_Click(object sender, RoutedEventArgs e)
        {            
            if (this.grdRadChart.Visibility == Visibility.Visible)
            {
                List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>();
                RadExportOptionsInfo.Add(new RadExportOptions() { ElementName = "Gadget With Period Columns COA Specific", 
                    Element = this.chCOASpecific, ExportFilterOption = RadExportFilterOption.RADCHART_EXCEL_EXPORT_FILTER });
                ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + 
                    GadgetNames.GADGET_WITH_PERIOD_COLUMNS_COA_SPECIFIC);
                childExportOptions.Show();
            }
            else
            {
                if (this.grdRadGridView.Visibility == Visibility.Visible)
                {
                    List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>();
                    RadExportOptionsInfo.Add(new RadExportOptions() { ElementName = "Gadget With Period Columns COA Specific", 
                        Element = this.dgCOASpecific, ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXCEL_EXPORT_FILTER });
                    ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + 
                        GadgetNames.GADGET_WITH_PERIOD_COLUMNS_COA_SPECIFIC);
                    childExportOptions.Show();
                }
            }
        }

        /// <summary>
        /// Printing the DataGrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(this.DataContextSource._logger, methodNamespace);
            try
            {
                List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>();
                if (this.grdRadGridView.Visibility == Visibility.Visible)
                {
                    RadExportOptionsInfo.Add(new RadExportOptions()
                    {
                        ElementName = ExportTypes.COA_SPECIFIC_GRID,
                        Element = this.dgCOASpecific,
                        ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_PRINT_FILTER,
                        RichTextBox = this.RichTextBox,
                        SkipColumnDisplayIndex = new List<int> { 1, 8 }
                    });
                }
                else if (this.grdRadChart.Visibility == Visibility.Visible)
                {
                    RadExportOptionsInfo.Add(new RadExportOptions()
                    {
                        ElementName = ExportTypes.COA_SPECIFIC_CHART,
                        Element = this.chCOASpecific,
                        ExportFilterOption = RadExportFilterOption.RADCHART_PRINT_FILTER,
                        RichTextBox = this.RichTextBox
                    });
                }

                ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + GadgetNames.GADGET_WITH_PERIOD_COLUMNS_COA_SPECIFIC);
                childExportOptions.Show();
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(this.DataContextSource._logger, ex);
            }
        }

        private static class ExportTypes
        {
            public const string COA_SPECIFIC_CHART = "COA specific chart";
            public const string COA_SPECIFIC_GRID = "COA specific data";
        }
        
        /// <summary>
        /// Event handler when user wants to Export the Grid to PDF
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportPdf_Click(object sender, RoutedEventArgs e)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(this.DataContextSource._logger, methodNamespace);
            try
            {
                List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>();
                if (this.grdRadGridView.Visibility == Visibility.Visible)
                {
                    RadExportOptionsInfo.Add(new RadExportOptions()
                    {
                        ElementName = ExportTypes.COA_SPECIFIC_GRID,
                        Element = this.dgCOASpecific,
                        ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_PDF_EXPORT_FILTER,
                        RichTextBox = this.RichTextBox,
                        SkipColumnDisplayIndex = new List<int> { 1, 8 }
                    });
                }
                else if (this.grdRadChart.Visibility == Visibility.Visible)
                {
                    RadExportOptionsInfo.Add(new RadExportOptions()
                    {
                        ElementName = ExportTypes.COA_SPECIFIC_CHART,
                        Element = this.chCOASpecific,
                        ExportFilterOption = RadExportFilterOption.RADCHART_PDF_EXPORT_FILTER,
                        RichTextBox = this.RichTextBox
                    });
                }

                ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + GadgetNames.GADGET_WITH_PERIOD_COLUMNS_COA_SPECIFIC);
                childExportOptions.Show();
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(this.DataContextSource._logger, ex);
            }
        }
        
        /// <summary>
        /// Syles added to Export to Excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgCOASpecific_ElementExporting(object sender, GridViewElementExportingEventArgs e)
        {
            RadGridView_ElementExport.ElementExporting(e, hideColumnIndex: new List<int> { 1, 8 });
        }
        #endregion

        #region Flip
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
            {
                Flipper.FlipItem(this.grdRadChart, this.grdRadGridView);
            }
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
        #endregion

        #region Period Colum Update
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
        #endregion
        #endregion

        #region Event Unsubscribe
        /// <summary>
        /// Dispose Events
        /// </summary>
        public override void Dispose()
        {
            PeriodColumns.PeriodColumnUpdate -= new PeriodColumnUpdateEvent(PeriodColumns_PeriodColumnUpdate);
            (this.DataContext as ViewModelCOASpecific).Dispose();
            this.DataContext = null;
        }
        #endregion        

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

        /// <summary>
       /// Row Loaded
       /// </summary>
       /// <param name="sender"></param>
       /// <param name="e"></param>
        private void dgCOASpecific_RowLoaded(object sender, RowLoadedEventArgs e)
        {
            PeriodColumns.RowDataCustomizationforCOASpecificGadget(e);
        }       
        #endregion
    }
}
