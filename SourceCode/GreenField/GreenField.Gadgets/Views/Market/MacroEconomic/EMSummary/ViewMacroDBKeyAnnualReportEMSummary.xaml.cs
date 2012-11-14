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
using Telerik.Windows.Data;
using System.ComponentModel;
using Telerik.Windows.Controls;
using System.Windows.Data;
using GreenField.Common;
using Telerik.Windows.Controls.GridView;
using GreenField.ServiceCaller;

namespace GreenField.Gadgets.Views
{
    public partial class ViewMacroDBKeyAnnualReportEMSummary : ViewBaseUserControl
    {
        #region Private fields
        /// <summary>
        /// List of type MacroDatabaseKeyAnnualReportData
        /// </summary>
        private List<MacroDatabaseKeyAnnualReportData> macroInfo;

        /// <summary>
        /// Current year
        /// </summary>
        private int currYear = DateTime.Now.Year;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor of the class
        /// </summary>
        /// <param name="dataContextSource">dataContextSource</param>
        public ViewMacroDBKeyAnnualReportEMSummary(ViewModelMacroDBKeyAnnualReportEMSummary dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextMacroDBKeyAnnualReportEMSummary = dataContextSource;
            dataContextSource.RetrieveMacroEMSummaryDataCompletedEvent += new
            Common.RetrieveMacroCountrySummaryDataCompleteEventHandler(dataContextSource_RetrieveMacroDataCompletedEvent);
            dataContextSource.MacroDBKeyAnnualReportEMSummaryDataLoadedEvent +=
            new DataRetrievalProgressIndicatorEventHandler(dataContextSource_macroDBKeyAnnualReportEMSummaryDataLoadedEvent);
            SetGridColumnHeaders();
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// Event Handler for LeftNavigation Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void LeftButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.DataContext != null)
            {
                (this.DataContext as ViewModelMacroDBKeyAnnualReportEMSummary).MoveLeftCommandMethod(null);
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
                (this.DataContext as ViewModelMacroDBKeyAnnualReportEMSummary).MoveRightCommandMethod(null);
            }
        }

        /// <summary>
        /// Data Retrieval Indicator
        /// </summary>
        /// <param name="e"></param>
        void dataContextSource_macroDBKeyAnnualReportEMSummaryDataLoadedEvent(DataRetrievalProgressIndicatorEventArgs e)
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
            this.dgMacroDBKeyReport.ItemsSource = ((ViewModelMacroDBKeyAnnualReportEMSummary)this.DataContext).FiveYearMacroCountryData;
            this.dgMacroDBKeyReport.GroupDescriptors.Clear();
            this.dgMacroDBKeyReport.SortDescriptors.Clear();
            currYear = ((ViewModelMacroDBKeyAnnualReportEMSummary)this.DataContext).CurrentYear;
            if (currYear >= 2024 || currYear <= 1989)
            {
                return;
            }
            if (macroInfo != null)
            {
                GroupDescriptor descriptor = new GroupDescriptor();
                descriptor.Member = "CategoryName";
                descriptor.SortDirection = ListSortDirection.Ascending;
                this.dgMacroDBKeyReport.GroupDescriptors.Add(descriptor);
                SortDescriptor sdescriptor = new SortDescriptor();
                sdescriptor.Member = "SortOrder";
                sdescriptor.SortDirection = ListSortDirection.Ascending;
                this.dgMacroDBKeyReport.SortDescriptors.Add(sdescriptor);
                this.dgMacroDBKeyReport.AutoGenerateColumns = false;
                dgMacroDBKeyReport.Columns[2].Header = (currYear - 3).ToString();
                dgMacroDBKeyReport.Columns[3].Header = (currYear - 2).ToString();
                dgMacroDBKeyReport.Columns[4].Header = (currYear - 1).ToString();
                dgMacroDBKeyReport.Columns[5].Header = (currYear).ToString();
                dgMacroDBKeyReport.Columns[6].Header = (currYear + 1).ToString();
                dgMacroDBKeyReport.Columns[7].Header = (currYear + 2).ToString();
                int currentYear = DateTime.Today.Year;
                dgMacroDBKeyReport.Columns[9].Header = "Five Year Average" + "(" + (currentYear - 4).ToString() + "-" + (currentYear).ToString() + ")";
            }
        }
        #endregion

        #region Properties

        /// <summary>
        /// DataContext Property
        /// </summary>
        private ViewModelMacroDBKeyAnnualReportEMSummary dataContextMacroDBKeyAnnualReportEMSummary;
        public ViewModelMacroDBKeyAnnualReportEMSummary DataContextMacroDBKeyAnnualReportEMSummary
        {
            get { return dataContextMacroDBKeyAnnualReportEMSummary; }
            set { dataContextMacroDBKeyAnnualReportEMSummary = value; }
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
                    ((ViewModelMacroDBKeyAnnualReportEMSummary)this.DataContext).IsActive = isActive;
                }
            }
        }

        #endregion

        #region Class Methods
        /// <summary>
        /// Setting initial value of column headers
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
            dgMacroDBKeyReport.Columns[9].Header = "Five Year Average" + "(" + (currentYear - 4).ToString() + "-" + 
                (currentYear).ToString() + ")";
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
                    List<RadExportOptions> radExportOptionsInfo = new List<RadExportOptions>
                    {
                        new RadExportOptions() { ElementName = "MacroDB Key Annual Report EM Summary", 
                            Element = this.dgMacroDBKeyReport, ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXCEL_EXPORT_FILTER }
                    };
                    ChildExportOptions childExportOptions = new ChildExportOptions(radExportOptionsInfo, "Export Options: " + 
                        GadgetNames.MODELS_FX_MACRO_ECONOMICS_MACRO_DATABASE_KEY_DATA_REPORT);
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
                    ElementName = "MacroDB Key Annual Report EM Summary",
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
                RadExportOptionsInfo.Add(new RadExportOptions() { ElementName = "MacroDB Key Annual Report EM Summary", Element = this.dgMacroDBKeyReport, ExportFilterOption = RadExportFilterOption.RADCHART_PDF_EXPORT_FILTER });

                ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + GadgetNames.PORTFOLIO_CONSTRUCTION_FAIR_VALUE_COMPOSITION);
                childExportOptions.Show();
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                //Logging.LogException(this.DataContextSlice1ChartExtension.logger, ex);
            }
        }

        /// <summary>
        /// Styles added to Export to excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
            this.DataContextMacroDBKeyAnnualReportEMSummary.MacroDBKeyAnnualReportEMSummaryDataLoadedEvent -= 
                new DataRetrievalProgressIndicatorEventHandler(dataContextSource_macroDBKeyAnnualReportEMSummaryDataLoadedEvent);
            this.DataContextMacroDBKeyAnnualReportEMSummary.Dispose();
            this.DataContextMacroDBKeyAnnualReportEMSummary = null;
            this.DataContext = null;
        }

        #endregion
    }
}
