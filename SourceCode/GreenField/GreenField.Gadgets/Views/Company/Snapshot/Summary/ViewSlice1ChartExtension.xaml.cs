using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Telerik.Windows.Controls.Charting;
using Telerik.Windows.Controls;
using Telerik.Windows.Documents.Model;
using GreenField.Gadgets.Helpers;
using GreenField.Gadgets.ViewModels;
using GreenField.Common;
using GreenField.ServiceCaller;

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
        private ViewModelSlice1ChartExtension dataContextSlice1ChartExtension;
        public ViewModelSlice1ChartExtension DataContextSlice1ChartExtension
        {
            get
            {
                return dataContextSlice1ChartExtension;
            }
            set
            {
                dataContextSlice1ChartExtension = value;
            }
        }

        /// <summary>
        /// To check whether the Dashboard is Active or not
        /// </summary>
        private bool isActive;
        public override bool IsActive
        {
            get { return isActive; }
            set
            {
                isActive = value;
                if (DataContextSlice1ChartExtension != null)
                {
                    DataContextSlice1ChartExtension.IsActive = isActive;
                }
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
            this.DataContextSlice1ChartExtension = dataContextSource;
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

        #region EventsUnsubscribe

        /// <summary>
        /// UnSubscribing the Events
        /// </summary>
        public override void Dispose()
        {
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
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(this.DataContextSlice1ChartExtension.logger, methodNamespace);
            try
            {
                List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>();

                if (grdRadChart.Visibility == Visibility.Visible)
                {
                    RadExportOptionsInfo.Add(new RadExportOptions()
                    {
                        ElementName = ExportTypes.CHART_EXTENSION,
                        Element = this.chChartExtension,
                        ExportFilterOption = RadExportFilterOption.RADCHART_EXCEL_EXPORT_FILTER
                    });
                }
                else if (grdRadGridView.Visibility == Visibility.Visible)
                {
                    RadExportOptionsInfo.Add(new RadExportOptions()
                    {
                        ElementName = ExportTypes.CHART_EXTENSION_DATA,
                        Element = this.dgChartExtension,
                        ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXCEL_EXPORT_FILTER
                    });
                }
                ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + GadgetNames.HOLDINGS_CHART_EXTENTION);
                childExportOptions.Show();
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(this.DataContextSlice1ChartExtension.logger, ex);
            }
        }

        /// <summary>
        /// Print the Grid/Chart
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(this.DataContextSlice1ChartExtension.logger, methodNamespace);
            try
            {
                if (grdRadGridView.Visibility == Visibility.Visible)
                {
                    Dispatcher.BeginInvoke((Action)(() =>
                    {
                        RichTextBox.Document = PDFExporter.Print(dgChartExtension, 10);
                    }));

                    RichTextBox.Print("MyDocument", Telerik.Windows.Documents.UI.PrintMode.Native);
                }
                else if (grdRadChart.Visibility == Visibility.Visible)
                {
                    Dispatcher.BeginInvoke((Action)(() =>
                    {
                        RichTextBox.Document = PDFExporter.PrintChart(chChartExtension);
                        RichTextBox.ChangeSectionPageOrientation(PageOrientation.Landscape);
                    }));

                    RichTextBox.Print("MyDocument", Telerik.Windows.Documents.UI.PrintMode.Native);
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(this.DataContextSlice1ChartExtension.logger, ex);
            }
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Chart Styles Apply
        /// </summary>
        private void ApplyChartStyles()
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(this.DataContextSlice1ChartExtension.logger, methodNamespace);
            try
            {
                this.chChartExtension.DefaultView.ChartArea.AxisX.TicksDistance = 50;
                this.chChartExtension.DefaultView.ChartArea.AxisX.AxisStyles.ItemLabelStyle = this.Resources["ItemLabelStyle"] as Style;
                this.chChartExtension.DefaultView.ChartArea.AxisY.AxisStyles.ItemLabelStyle = this.Resources["ItemLabelStyle"] as Style;
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(this.DataContextSlice1ChartExtension.logger, ex);
            }
        }

        /// <summary>
        /// Return the Color for the Series Added
        /// </summary>
        /// <param name="sortId">Sorting Id of Series</param>
        /// <returns>SolidColorBrush for the Series</returns>
        private SolidColorBrush ReturnLegendItemColor(int sortId)
        {
            switch (sortId)
            {
                case 1:
                    return new SolidColorBrush(Color.FromArgb(255, 159, 29, 33));
                case 2:
                    return new SolidColorBrush(Color.FromArgb(255, 167, 54, 44));
                case 3:
                    return new SolidColorBrush(Color.FromArgb(255, 190, 113, 92));
                default:
                    return new SolidColorBrush(Colors.Black);
            }
        }

        /// <summary>
        /// Data-Bound Event 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chChartExtension_DataBound(object sender, Telerik.Windows.Controls.Charting.ChartDataBoundEventArgs e)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(this.DataContextSlice1ChartExtension.logger, methodNamespace);
            try
            {
                if (this.DataContext as ViewModelSlice1ChartExtension != null)
                {
                    if (this.chChartExtension.DefaultView.ChartLegend.Items.Count != 0)
                    {
                        this.chChartExtension.DefaultView.ChartLegend.Items.Clear();
                    }

                    int i = 0;

                    if ((this.DataContext as ViewModelSlice1ChartExtension).ChartExtensionPlottedData != null)
                    {
                        (this.DataContext as ViewModelSlice1ChartExtension).AxisXMinValue = Convert.ToDateTime(((this.DataContext as ViewModelSlice1ChartExtension).ChartExtensionPlottedData.OrderBy(a => a.ToDate)).
                            Select(a => a.ToDate).FirstOrDefault()).ToOADate();
                        (this.DataContext as ViewModelSlice1ChartExtension).AxisXMaxValue = Convert.ToDateTime(((this.DataContext as ViewModelSlice1ChartExtension).ChartExtensionPlottedData.OrderByDescending(a => a.ToDate)).
                            Select(a => a.ToDate).FirstOrDefault()).ToOADate();
                        int dataCount = (this.DataContext as ViewModelSlice1ChartExtension).ChartExtensionPlottedData.Count;

                        if (dataCount != 0)
                        {
                            this.chChartExtension.DefaultView.ChartArea.AxisX.Step = dataCount / 10;
                        }

                        if (this.DataContextSlice1ChartExtension.ChartExtensionPlottedData.Any(a => a.AmountTraded != null))
                        {
                            ChartLegendItem transactionLegendItem = new ChartLegendItem();
                            transactionLegendItem.MarkerFill = new SolidColorBrush(Color.FromArgb(255, 33, 54, 113));
                            transactionLegendItem.Label = this.DataContextSlice1ChartExtension.ChartExtensionPlottedData.
                                Where(a => a.Type.ToUpper() == "SECURITY").Select(a => a.Ticker).FirstOrDefault();
                            this.chChartExtension.DefaultView.ChartLegend.Items.Add(transactionLegendItem);
                        }

                        if (this.DataContextSlice1ChartExtension.ChartExtensionPlottedData.Any(a => a.Type.ToUpper() == "SECURITY"))
                        {
                            i++;
                            ChartLegendItem securityLegendItem = new ChartLegendItem();
                            securityLegendItem.MarkerFill = ReturnLegendItemColor(i);
                            securityLegendItem.Label = this.DataContextSlice1ChartExtension.ChartExtensionPlottedData.
                                Where(a => a.Type.ToUpper() == "SECURITY").Select(a => a.Ticker).FirstOrDefault();
                            this.chChartExtension.DefaultView.ChartLegend.Items.Add(securityLegendItem);
                        }

                        if (this.DataContextSlice1ChartExtension.ChartExtensionPlottedData.Any(a => a.Type == "COUNTRY"))
                        {
                            i++;
                            ChartLegendItem countryLegendItem = new ChartLegendItem();
                            countryLegendItem.MarkerFill = ReturnLegendItemColor(i);
                            countryLegendItem.Label = this.DataContextSlice1ChartExtension.ChartExtensionPlottedData.
                                Where(a => a.Type.ToUpper() == "COUNTRY").Select(a => a.Ticker).FirstOrDefault();
                            this.chChartExtension.DefaultView.ChartLegend.Items.Add(countryLegendItem);
                        }

                        if (this.DataContextSlice1ChartExtension.ChartExtensionPlottedData.Any(a => a.Type.ToUpper() == "SECTOR"))
                        {
                            i++;
                            ChartLegendItem sectorLegendItem = new ChartLegendItem();
                            sectorLegendItem.MarkerFill = ReturnLegendItemColor(i);
                            sectorLegendItem.Label = this.DataContextSlice1ChartExtension.ChartExtensionPlottedData.
                                Where(a => a.Type.ToUpper() == "SECTOR").Select(a => a.Ticker).FirstOrDefault();
                            this.chChartExtension.DefaultView.ChartLegend.Items.Add(sectorLegendItem);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(this.DataContextSlice1ChartExtension.logger, ex);
            }
        }

        /// <summary>
        /// Chart Loaded Event Handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chChartExtension_Loaded(object sender, RoutedEventArgs e)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(this.DataContextSlice1ChartExtension.logger, methodNamespace);
            try
            {
                if (chChartExtension.DefaultView.ChartLegend.Items.Count != 0)
                {
                    ChartLegendItem var = this.chChartExtension.DefaultView.ChartLegend.Items[0];
                    this.chChartExtension.DefaultView.ChartLegend.Items.Remove(var);
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(this.DataContextSlice1ChartExtension.logger, ex);
            }
        }

        #endregion
    }
}
