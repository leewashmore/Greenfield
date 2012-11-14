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
using System.ComponentModel;
using System.Windows.Shapes;
using System.Windows.Data;
using Telerik.Windows.Data;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;
using GreenField.Gadgets.Helpers;
using GreenField.Gadgets.ViewModels;
using GreenField.ServiceCaller.ModelFXDefinitions;
using GreenField.Common;
using GreenField.DataContracts;
using GreenField.ServiceCaller;

namespace GreenField.Gadgets.Views
{
    public partial class ViewMacroDBKeyAnnualReport : ViewBaseUserControl
    {
        #region Private Fields
        /// <summary>
        /// private list
        /// </summary>
        private List<MacroDatabaseKeyAnnualReportData> macroInfo;
        /// <summary>
        /// stores the current year value
        /// </summary>
        private int currentYear = DateTime.Now.Year;
        #endregion

        #region constructor
        /// <summary>
        /// Constructor of the class
        /// </summary>
        /// <param name="dataContextSource"></param>
        public ViewMacroDBKeyAnnualReport(ViewModelMacroDBKeyAnnualReport dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextMacroDBKeyAnnualReport = dataContextSource;
            dataContextSource.RetrieveMacroDataCompletedEvent += new Common.RetrieveMacroCountrySummaryDataCompleteEventHandler(dataContextSource_RetrieveMacroDataCompletedEvent);
            dataContextSource.macroDBKeyAnnualReportCountryDataLoadedEvent +=
            new DataRetrievalProgressIndicatorEventHandler(dataContextSource_macroDBKeyAnnualReportCountryDataLoadedEvent);
            SetGridColumnHeaders();
        }
        #endregion

        #region EventHandlers
        /// <summary>
        /// Data Retrieval Indicator
        /// </summary>
        /// <param name="e"></param>
        void dataContextSource_macroDBKeyAnnualReportCountryDataLoadedEvent(DataRetrievalProgressIndicatorEventArgs e)
        {
            if (e.ShowBusy)
            {
                this.busyIndicatorGrid.IsBusy = true;
            }
            else
            {
                this.busyIndicatorGrid.IsBusy = false;
            }
        }
        /// <summary>
        /// Data Retrieval Indicator
        /// </summary>
        /// <param name="e"></param>
        public void dataContextSource_RetrieveMacroDataCompletedEvent(Common.RetrieveMacroCountrySummaryDataCompleteEventArgs e)
        {
            macroInfo = e.MacroInfo;
            this.dgMacroDBKeyReport.ItemsSource = ((ViewModelMacroDBKeyAnnualReport)this.DataContext).FiveYearMacroCountryData;
            this.dgMacroDBKeyReport.GroupDescriptors.Clear();
            this.dgMacroDBKeyReport.SortDescriptors.Clear();
            currentYear = ((ViewModelMacroDBKeyAnnualReport)this.DataContext).CurrentYear;
            if (currentYear >= 2024 || currentYear <= 1989)
            {
                return;
            }
            if (macroInfo != null)
            {
                GroupDescriptor descriptor = new GroupDescriptor();
                descriptor.Member = "CategoryName";               
                this.dgMacroDBKeyReport.GroupDescriptors.Add(descriptor);
                SortDescriptor sdescriptor = new SortDescriptor();
                sdescriptor.Member = "SortOrder";
                sdescriptor.SortDirection = ListSortDirection.Ascending;
                this.dgMacroDBKeyReport.SortDescriptors.Add(sdescriptor);
                this.dgMacroDBKeyReport.AutoGenerateColumns = false;
                dgMacroDBKeyReport.Columns[2].Header = (currentYear - 3).ToString();
                dgMacroDBKeyReport.Columns[3].Header = (currentYear - 2).ToString();
                dgMacroDBKeyReport.Columns[4].Header = (currentYear - 1).ToString();
                dgMacroDBKeyReport.Columns[5].Header = (currentYear).ToString();
                dgMacroDBKeyReport.Columns[6].Header = (currentYear + 1).ToString();
                dgMacroDBKeyReport.Columns[7].Header = (currentYear + 2).ToString();
                int currentYearLocal = DateTime.Today.Year;
                dgMacroDBKeyReport.Columns[9].Header = "Five Year Average" + "(" + (currentYearLocal - 4).ToString() + "-" + (currentYearLocal).ToString() + ")";
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Data context Property
        /// </summary>
        private ViewModelMacroDBKeyAnnualReport dataContextMacroDBKeyAnnualReport;
        public ViewModelMacroDBKeyAnnualReport DataContextMacroDBKeyAnnualReport
        {
            get { return dataContextMacroDBKeyAnnualReport; }
            set { dataContextMacroDBKeyAnnualReport = value; }
        }

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
                    ((ViewModelMacroDBKeyAnnualReport)this.DataContext).IsActive = isActive;
                }
            }
        }
        #endregion

        #region Class Methods
        /// <summary>
        /// Event Handler for LeftNavigation Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void LeftButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext != null)
            {
                (this.DataContext as ViewModelMacroDBKeyAnnualReport).MoveLeftCommandMethod(null);
            }
        }

        /// <summary>
        /// Event Handler for Right navigation click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void RightButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext != null)
            {
                (this.DataContext as ViewModelMacroDBKeyAnnualReport).MoveRightCommandMethod(null);
            }
        }

        /// <summary>
        /// Sets initial value for the column headers
        /// </summary>
        public void SetGridColumnHeaders()
        {
            int currentYear = DateTime.Today.Year;
            dgMacroDBKeyReport.Columns[2].Header = (currentYear - 3).ToString();
            dgMacroDBKeyReport.Columns[3].Header = (currentYear - 2).ToString();
            dgMacroDBKeyReport.Columns[4].Header = (currentYear - 1).ToString();
            dgMacroDBKeyReport.Columns[5].Header = (currentYear).ToString();
            dgMacroDBKeyReport.Columns[6].Header = (currentYear + 1).ToString();
            dgMacroDBKeyReport.Columns[7].Header = (currentYear + 2).ToString();
            dgMacroDBKeyReport.Columns[9].Header = "Five Year Average" + "(" + (currentYear - 4).ToString() + "-" + (currentYear).ToString() + ")";
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
                if (this.dgMacroDBKeyReport.Visibility == Visibility.Visible)
                {
                    List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>
                    {                  
                        new RadExportOptions() { ElementName = "MacroDB Key Annual Report", Element = this.dgMacroDBKeyReport, ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXCEL_EXPORT_FILTER }
                    };
                    ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + GadgetNames.MODELS_FX_MACRO_ECONOMICS_MACRO_DATABASE_KEY_ANNUAL_DATA_REPORT);
                    childExportOptions.Show();
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog(ex.Message);
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
            //Logging.LogBeginMethod(this.DataContextHoldingsPieChart.logger, methodNamespace);
            try
            {
                List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>();
                RadExportOptionsInfo.Add(new RadExportOptions()
                {
                    ElementName = "MacroDB Key Annual Report",
                    Element = this.dgMacroDBKeyReport,
                    ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_PRINT_FILTER,
                    //RichTextBox = this.RichTextBox
                });

                ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + GadgetNames.SECURITY_REFERENCE_PRICE_COMPARISON);
                childExportOptions.Show();

            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                //Logging.LogException(this.DataContextSlice1ChartExtension.logger, ex);
            }
        }

        /// <summary>
        /// Event handler when user wants to Export the Grid to PDF
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportPdf_Click(object sender, RoutedEventArgs e)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            //Logging.LogBeginMethod(this.DataContextSlice1ChartExtension.logger, methodNamespace);
            try
            {

                List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>();
                RadExportOptionsInfo.Add(new RadExportOptions() { ElementName = "MacroDB Key Annual Report", Element = this.dgMacroDBKeyReport, ExportFilterOption = RadExportFilterOption.RADCHART_PDF_EXPORT_FILTER });

                ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + GadgetNames.PORTFOLIO_CONSTRUCTION_FAIR_VALUE_COMPOSITION);
                childExportOptions.Show();
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                //Logging.LogException(this.DataContextSlice1ChartExtension.logger, ex);
            }
        }

        private void dgMacroDBKeyReport_ElementExporting(object sender, Telerik.Windows.Controls.GridViewElementExportingEventArgs e)
        {
            RadGridView_ElementExport.ElementExporting(e, hideColumnIndex: new List<int> { 1, 8 });
        }
        #endregion

        #region RemoveEvents
        /// <summary>
        /// Overrides Dispose
        /// </summary>
        public override void Dispose()
        {
            this.DataContextMacroDBKeyAnnualReport.macroDBKeyAnnualReportCountryDataLoadedEvent -= new DataRetrievalProgressIndicatorEventHandler(dataContextSource_macroDBKeyAnnualReportCountryDataLoadedEvent);
            this.DataContextMacroDBKeyAnnualReport.Dispose();
            this.DataContextMacroDBKeyAnnualReport = null;
            this.DataContext = null;
        }
        #endregion
    }
}
