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
using Telerik.Windows.Controls;
using GreenField.Common;
using GreenField.ServiceCaller;
using GreenField.DataContracts;

namespace GreenField.Gadgets.Views
{
    /// <summary>
    /// Code-Behind class for Slice-1 ChartExtension
    /// </summary>
    public partial class ViewSlice1ChartExtension : ViewBaseUserControl
    {
        #region Variables

        /// <summary>
        /// Export Types
        /// </summary>
        private static class ExportTypes
        {
            public const string CHART_EXTENSION = "Chart Extension";
            public const string CHART_EXTENSION_DATA = "Chart Extension Data";
        }


        #endregion

        #region PropertyDeclaration

        /// <summary>
        /// Property of ViewModel type
        /// </summary>
        private ViewModelSlice1ChartExtension _dataContextSlice1ChartExtension;
        public ViewModelSlice1ChartExtension DataContextSlice1ChartExtension
        {
            get
            {
                return _dataContextSlice1ChartExtension;
            }
            set
            {
                _dataContextSlice1ChartExtension = value;
            }
        }




        #endregion

        #region Constructor

        /// <summary>
        /// Constructor that initialises the Class
        /// </summary>
        /// <param name="dataContextSource"></param>
        public ViewSlice1ChartExtension(ViewModelSlice1ChartExtension dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            dataContextSource.ChartExtensionDataLoadedEvent += new DataRetrievalProgressIndicatorEventHandler(dataContextSource_ChartExtensionDataLoadedEvent);
            dataContextSource.ChartArea = this.chChartExtension.DefaultView.ChartArea;
            this.chChartExtension.DataBound += dataContextSource.ChartDataBound;
            this.ApplyChartStyles();
        }

        #endregion

        #region Flipping

        /// <summary>
        /// Flipping between Grid & Chart
        /// Using the method FlipItem in class Flipper.cs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFlip_Click(object sender, RoutedEventArgs e)
        {
            if (this.grdRadGridView.Visibility == System.Windows.Visibility.Visible)
            {
                Flipper.FlipItem(this.grdRadGridView, this.grdRadChart);
            }
            else
            {
                Flipper.FlipItem(this.grdRadChart, this.grdRadGridView);
            }
        }

        #endregion

        #region Events



        #endregion

        #region ProgressIndicator

        /// <summary>
        /// Data Progress Indicator Event
        /// </summary>
        /// <param name="e"></param>
        void dataContextSource_ChartExtensionDataLoadedEvent(DataRetrievalProgressIndicatorEventArgs e)
        {
            if (e.ShowBusy)
            {
                this.busyIndicator.IsBusy = true;
                this.busyIndicatorGrid.IsBusy = true;
            }
            else
            {
                this.busyIndicator.IsBusy = false;
                this.busyIndicatorGrid.IsBusy = false;
            }
        }

        #endregion

        #region EventsUnsubscribe

        /// <summary>
        /// UnSubscribing the Events
        /// </summary>
        public override void Dispose()
        {
            this.DataContextSlice1ChartExtension.ChartExtensionDataLoadedEvent -= new DataRetrievalProgressIndicatorEventHandler(dataContextSource_ChartExtensionDataLoadedEvent);
            this.DataContextSlice1ChartExtension.Dispose();
            this.DataContextSlice1ChartExtension = null;
            this.DataContext = null;
        }

        #endregion

        #region Export

        /// <summary>
        /// Event for Grid Export
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ElementExportingEvent(object sender, GridViewElementExportingEventArgs e)
        {
            RadGridView_ElementExport.ElementExporting(e);
        }

        /// <summary>
        /// Method to catch Click Event of Export to Excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>
                {
                    new RadExportOptions() { ElementName = ExportTypes.CHART_EXTENSION, Element = this.chChartExtension, ExportFilterOption = RadExportFilterOption.RADCHART_EXPORT_FILTER },
                    new RadExportOptions() { ElementName = ExportTypes.CHART_EXTENSION_DATA, Element = this.dgChartExtension, ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXPORT_FILTER },
                };
                ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + GadgetNames.SECURITY_REFERENCE_PRICE_COMPARISON);
                childExportOptions.Show();
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog(ex.Message);
            }
        }

        #endregion

        #region Helpers

        private void ApplyChartStyles()
        {
            this.chChartExtension.DefaultView.ChartArea.AxisX.TicksDistance = 50;
            //this.chChartExtension.DefaultView.ChartArea.AxisX.AxisStyles.TitleStyle = this.Resources["AxisTitleStyle"] as Style;
            //this.chChartExtension.DefaultView.ChartArea.AxisY.AxisStyles.TitleStyle = this.Resources["AxisTitleStyle"] as Style;
            this.chChartExtension.DefaultView.ChartArea.AxisX.AxisStyles.ItemLabelStyle = this.Resources["ItemLabelStyle"] as Style;
            this.chChartExtension.DefaultView.ChartArea.AxisY.AxisStyles.ItemLabelStyle = this.Resources["ItemLabelStyle"] as Style;
            this.chChartExtension.DefaultView.ChartArea.ZoomScrollSettingsX.SliderSelectionStart = 0;
            this.chChartExtension.DefaultView.ChartArea.ZoomScrollSettingsX.SliderSelectionEnd = 0.5;
        }

        #endregion

        private void cmbTime_SelectionChanged(object sender, Telerik.Windows.Controls.SelectionChangedEventArgs e)
        {
            //string timeInterval = Convert.ToString(cmbTime.SelectedValue);
            //switch (timeInterval.ToString())
            //{
            //    case ("1-Month"):
            //        {
            //            this.chChartExtension.DefaultView.ChartArea.AxisX.DefaultLabelFormat = "d";
            //            this.chChartExtension.DefaultView.ChartArea.SmartLabelsEnabled = true;
            //            this.chChartExtension.DefaultView.ChartArea.AxisX.AutoRange = true;
            //            //this.chChartExtension.DefaultView.ChartArea.AxisX.Step = 5;
            //            this.chChartExtension.DefaultView.ChartArea.AxisX.LabelStep = 2;
            //            this.chChartExtension.DefaultView.ChartArea.ZoomScrollSettingsX.SliderSelectionStart = 0;
            //            this.chChartExtension.DefaultView.ChartArea.ZoomScrollSettingsX.SliderSelectionEnd = 1;

            //            break;
            //        }
            //    case ("2-Months"):
            //        {
            //            this.chChartExtension.DefaultView.ChartArea.AxisX.DefaultLabelFormat = "d";
            //            this.chChartExtension.DefaultView.ChartArea.SmartLabelsEnabled = true;
            //            this.chChartExtension.DefaultView.ChartArea.AxisX.AutoRange = true;
            //            //this.chChartExtension.DefaultView.ChartArea.AxisX.Step = 7;
            //            this.chChartExtension.DefaultView.ChartArea.AxisX.LabelStep = 2;
            //            this.chChartExtension.DefaultView.ChartArea.ZoomScrollSettingsX.SliderSelectionStart = 0;
            //            this.chChartExtension.DefaultView.ChartArea.ZoomScrollSettingsX.SliderSelectionEnd = 1;

            //            break;
            //        }
            //    case ("3-Months"):
            //        {
            //            this.chChartExtension.DefaultView.ChartArea.AxisX.DefaultLabelFormat = "d";
            //            this.chChartExtension.DefaultView.ChartArea.SmartLabelsEnabled = true;
            //            this.chChartExtension.DefaultView.ChartArea.AxisX.AutoRange = true;
            //            //this.chChartExtension.DefaultView.ChartArea.AxisX.Step = 1;
            //            this.chChartExtension.DefaultView.ChartArea.AxisX.LabelStep = 2;
            //            this.chChartExtension.DefaultView.ChartArea.ZoomScrollSettingsX.SliderSelectionStart = 0;
            //            this.chChartExtension.DefaultView.ChartArea.ZoomScrollSettingsX.SliderSelectionEnd = 1;

            //            break;
            //        }
            //    case ("6-Months"):
            //        {
            //            this.chChartExtension.DefaultView.ChartArea.AxisX.DefaultLabelFormat = "d";
            //            this.chChartExtension.DefaultView.ChartArea.SmartLabelsEnabled = true;
            //            this.chChartExtension.DefaultView.ChartArea.AxisX.AutoRange = true;
            //            //this.chChartExtension.DefaultView.ChartArea.AxisX.Step = 6;
            //            this.chChartExtension.DefaultView.ChartArea.AxisX.LabelStep = 1;
            //            this.chChartExtension.DefaultView.ChartArea.ZoomScrollSettingsX.SliderSelectionStart = 0;
            //            this.chChartExtension.DefaultView.ChartArea.ZoomScrollSettingsX.SliderSelectionEnd = 0.5;

            //            break;
            //        }
            //    case ("YTD"):
            //        {
            //            this.chChartExtension.DefaultView.ChartArea.AxisX.DefaultLabelFormat = "d";
            //            this.chChartExtension.DefaultView.ChartArea.SmartLabelsEnabled = true;
            //            this.chChartExtension.DefaultView.ChartArea.AxisX.AutoRange = true;
            //            //this.chChartExtension.DefaultView.ChartArea.AxisX.Step = 1;
            //            this.chChartExtension.DefaultView.ChartArea.AxisX.LabelStep = 2;
            //            this.chChartExtension.DefaultView.ChartArea.ZoomScrollSettingsX.SliderSelectionStart = 0;
            //            this.chChartExtension.DefaultView.ChartArea.ZoomScrollSettingsX.SliderSelectionEnd = 0.5;

            //            break;
            //        }
            //    case ("1-Year"):
            //        {
            //            this.chChartExtension.DefaultView.ChartArea.AxisX.DefaultLabelFormat = "d";
            //            this.chChartExtension.DefaultView.ChartArea.SmartLabelsEnabled = true;
            //            this.chChartExtension.DefaultView.ChartArea.AxisX.AutoRange = true;
            //            //this.chChartExtension.DefaultView.ChartArea.AxisX.Step = 10;
            //            this.chChartExtension.DefaultView.ChartArea.AxisX.LabelStep = 2;
            //            this.chChartExtension.DefaultView.ChartArea.ZoomScrollSettingsX.SliderSelectionStart = 0;
            //            this.chChartExtension.DefaultView.ChartArea.ZoomScrollSettingsX.SliderSelectionEnd = 0.5;
            //            break;
            //        }
            //    case ("2-Years"):
            //        {
            //            this.chChartExtension.DefaultView.ChartArea.AxisX.DefaultLabelFormat = "d";
            //            this.chChartExtension.DefaultView.ChartArea.SmartLabelsEnabled = true;
            //            this.chChartExtension.DefaultView.ChartArea.AxisX.AutoRange = true;
            //            //this.chChartExtension.DefaultView.ChartArea.AxisX.Step = 20;
            //            this.chChartExtension.DefaultView.ChartArea.AxisX.LabelStep = 2;
            //            this.chChartExtension.DefaultView.ChartArea.ZoomScrollSettingsX.SliderSelectionStart = 0;
            //            this.chChartExtension.DefaultView.ChartArea.ZoomScrollSettingsX.SliderSelectionEnd = 0.25;
            //            break;
            //        }
            //    case ("3-Years"):
            //        {
            //            this.chChartExtension.DefaultView.ChartArea.AxisX.DefaultLabelFormat = "d";
            //            this.chChartExtension.DefaultView.ChartArea.SmartLabelsEnabled = true;
            //            this.chChartExtension.DefaultView.ChartArea.AxisX.AutoRange = true;
            //            //this.chChartExtension.DefaultView.ChartArea.AxisX.Step = 30;
            //            this.chChartExtension.DefaultView.ChartArea.AxisX.LabelStep = 2;
            //            this.chChartExtension.DefaultView.ChartArea.ZoomScrollSettingsX.SliderSelectionStart = 0;
            //            this.chChartExtension.DefaultView.ChartArea.ZoomScrollSettingsX.SliderSelectionEnd = 0.2;
            //            break;
            //        }
            //    case ("4-Years"):
            //        {
            //            this.chChartExtension.DefaultView.ChartArea.AxisX.DefaultLabelFormat = "d";
            //            this.chChartExtension.DefaultView.ChartArea.SmartLabelsEnabled = true;
            //            this.chChartExtension.DefaultView.ChartArea.AxisX.AutoRange = true;
            //            // this.chChartExtension.DefaultView.ChartArea.AxisX.Step = 40;
            //            this.chChartExtension.DefaultView.ChartArea.AxisX.LabelStep = 2;
            //            this.chChartExtension.DefaultView.ChartArea.ZoomScrollSettingsX.SliderSelectionStart = 0;
            //            this.chChartExtension.DefaultView.ChartArea.ZoomScrollSettingsX.SliderSelectionEnd = 0.2;
            //            break;
            //        }
            //    case ("5-Years"):
            //        {
            //            this.chChartExtension.DefaultView.ChartArea.AxisX.DefaultLabelFormat = "d";
            //            this.chChartExtension.DefaultView.ChartArea.SmartLabelsEnabled = true;
            //            this.chChartExtension.DefaultView.ChartArea.AxisX.AutoRange = true;
            //            //this.chChartExtension.DefaultView.ChartArea.AxisX.Step = 50;
            //            this.chChartExtension.DefaultView.ChartArea.AxisX.LabelStep = 2;
            //            this.chChartExtension.DefaultView.ChartArea.ZoomScrollSettingsX.SliderSelectionStart = 0;
            //            this.chChartExtension.DefaultView.ChartArea.ZoomScrollSettingsX.SliderSelectionEnd = 0.2;

            //            break;
            //        }
            //    case ("10-Years"):
            //        {
            //            this.chChartExtension.DefaultView.ChartArea.AxisX.DefaultLabelFormat = "d";
            //            this.chChartExtension.DefaultView.ChartArea.SmartLabelsEnabled = true;
            //            this.chChartExtension.DefaultView.ChartArea.AxisX.AutoRange = true;
            //            //this.chChartExtension.DefaultView.ChartArea.AxisX.Step = 100;
            //            this.chChartExtension.DefaultView.ChartArea.AxisX.LabelStep = 2;
            //            this.chChartExtension.DefaultView.ChartArea.ZoomScrollSettingsX.SliderSelectionStart = 0;
            //            this.chChartExtension.DefaultView.ChartArea.ZoomScrollSettingsX.SliderSelectionEnd = 0.1;
            //            break;
            //        }
            //    default:
            //        {
            //            this.chChartExtension.DefaultView.ChartArea.AxisX.DefaultLabelFormat = "d";
            //            this.chChartExtension.DefaultView.ChartArea.SmartLabelsEnabled = true;
            //            break;
            //        }
            //}
        }

        private void chChartExtension_DataBound(object sender, Telerik.Windows.Controls.Charting.ChartDataBoundEventArgs e)
        {
            if (this.DataContext as ViewModelSlice1ChartExtension != null)
            {
                if ((this.DataContext as ViewModelSlice1ChartExtension).ChartExtensionData != null)
                {
                    (this.DataContext as ViewModelSlice1ChartExtension).AxisXMinValue = Convert.ToDateTime(((this.DataContext as ViewModelSlice1ChartExtension).ChartExtensionData.OrderBy(a => a.ToDate)).
                        Select(a => a.ToDate).FirstOrDefault()).ToOADate();
                    (this.DataContext as ViewModelSlice1ChartExtension).AxisXMaxValue = Convert.ToDateTime(((this.DataContext as ViewModelSlice1ChartExtension).ChartExtensionData.OrderByDescending(a => a.ToDate)).
                        Select(a => a.ToDate).FirstOrDefault()).ToOADate();
                    int dataCount = (this.DataContext as ViewModelSlice1ChartExtension).ChartExtensionData.Count;
                    if (dataCount != 0)
                    {
                        this.chChartExtension.DefaultView.ChartArea.AxisX.Step = dataCount / 10;
                    }
                }
            }
        }
    }
}
