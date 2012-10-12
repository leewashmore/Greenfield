using System;
using System.Collections.Generic;
using System.Windows;
#if !SILVERLIGHT
using Microsoft.Win32;
#else
using System.Windows.Controls;
#endif
using Telerik.Windows.Controls.GridView;
using Telerik.Windows.Documents.Model;
using Telerik.Windows.Controls;
using Telerik.Windows.Data;
using GreenField.DataContracts;
using GreenField.ServiceCaller;
using GreenField.Common;
using GreenField.Gadgets.ViewModels;
using GreenField.Gadgets.Helpers;

namespace GreenField.Gadgets.Views
{
    /// <summary>
    /// XAML.cs class for Portfolio Details UI
    /// </summary>
    public partial class ViewPortfolioDetails : ViewBaseUserControl
    {
        #region Private Variables

        /// <summary>
        /// OffSet
        /// </summary>
        double offsetY;

        /// <summary>
        /// Total Height
        /// </summary>
        double totalHeight;

        /// <summary>
        /// Instance of Canvas
        /// </summary>
        Canvas canvas;

        /// <summary>
        /// Instance of RadGrid
        /// </summary>
        RadGridView grid;

        /// <summary>
        /// Instance of GridView-Filter Descriptor
        /// </summary>
        FilterDescriptorCollection gridFilterDescriptors;

        /// <summary>
        /// Data Source of the Grid
        /// </summary>
        RangeObservableCollection<PortfolioDetailsData> gridDataSource;

        /// <summary>
        /// View Model
        /// </summary>
        private ViewModelPortfolioDetails dataContextPortfolioDetails;
        public ViewModelPortfolioDetails DataContextPortfolioDetails
        {
            get
            {
                return dataContextPortfolioDetails;
            }
            set
            {
                dataContextPortfolioDetails = value;
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
                if (DataContextPortfolioDetails != null)
                    DataContextPortfolioDetails.IsActive = isActive;
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public ViewPortfolioDetails(ViewModelPortfolioDetails dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextPortfolioDetails = dataContextSource;
            this.dgPortfolioDetails.GroupPanelStyle = this.Resources["GridViewGroupPanelStyle"] as Style;
            this.dgPortfolioDetails.Columns[20].Header = "Revenue Growth " + DateTime.Today.Year.ToString();
            this.dgPortfolioDetails.Columns[21].Header = "Revenue Growth " + (DateTime.Today.Year + 1).ToString();
            this.dgPortfolioDetails.Columns[20].Header = "Net Income Growth " + DateTime.Today.Year.ToString();
            this.dgPortfolioDetails.Columns[20].Header = "Net Income Growth " + (DateTime.Today.Year + 1).ToString();
            ChangeHeaders();
        }

        #endregion

        #region ExportToExcel/PDF/Print

        #region ExcelExport

        /// <summary>
        /// Static class storing string types
        /// </summary>
        private static class ExportTypes
        {
            public const string PORTFOLIO_DETAILS_UI = "Portfolio Details";
        }

        /// <summary>
        /// Method to catch Click Event of Export to Excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(this.DataContextPortfolioDetails.Logger, methodNamespace);
            try
            {
                if (this.dgPortfolioDetails.Visibility == Visibility.Visible)
                {
                    List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>
                        {
                                new RadExportOptions() { ElementName = ExportTypes.PORTFOLIO_DETAILS_UI, Element = this.dgPortfolioDetails, 
                                    ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXPORT_FILTER }
                        };
                    ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + ExportTypes.PORTFOLIO_DETAILS_UI);
                    childExportOptions.Show();
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(this.DataContextPortfolioDetails.Logger, ex);
            }
        }
        #endregion

        #region HelperMethods
        /// <summary>
        /// Element Exporting Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgPortfolioDetails_ElementExporting(object sender, GridViewElementExportingEventArgs e)
        {
            RadGridView_ElementExport.ElementExporting(e, isGroupFootersVisible: false);
        }
        #endregion

        #region PDFExport
        /// <summary>
        /// Event handler when user wants to Export the Grid to PDF
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportPDF_Click(object sender, RoutedEventArgs e)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(this.DataContextPortfolioDetails.Logger, methodNamespace);
            try
            {
                PDFExporter.btnExportPDF_Click(this.dgPortfolioDetails);
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(this.DataContextPortfolioDetails.Logger, ex);
            }
        }
        #endregion

        #region Printing the DataGrid

        /// <summary>
        /// Printing the DataGrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(this.DataContextPortfolioDetails.Logger, methodNamespace);
            try
            {
                Dispatcher.BeginInvoke((Action)(() =>
                    {
                        RichTextBox.Document = PDFExporter.Print(dgPortfolioDetails, 6);
                    }));

                this.RichTextBox.Document.SectionDefaultPageOrientation = PageOrientation.Landscape;
                RichTextBox.Print("MyDocument", Telerik.Windows.Documents.UI.PrintMode.Native);
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(this.DataContextPortfolioDetails.Logger, ex);
            }
        }

        #endregion

        #endregion

        #region GroupingHelperMethods

        /// <summary>
        /// Event when User groups the data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgPortfolioDetails_Grouping(object sender, GridViewGroupingEventArgs e)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(this.DataContextPortfolioDetails.Logger, methodNamespace);
            try
            {
                if (e.Action.ToString() != "Remove")
                {
                    if (this.dgPortfolioDetails.GroupDescriptors.Count > 0)
                    {
                        e.Cancel = true;
                        this.dgPortfolioDetails.GroupDescriptors.Clear();
                        dgPortfolioDetails.GroupDescriptors.Add(e.GroupDescriptor);
                    }
                    Telerik.Windows.Controls.GridView.ColumnGroupDescriptor groupDescriptor = e.GroupDescriptor as Telerik.Windows.Controls.GridView.ColumnGroupDescriptor;
                    DataContextPortfolioDetails.GroupingColumn = Convert.ToString(groupDescriptor.Column.UniqueName);
                }
                else
                {
                    DataContextPortfolioDetails.GroupingColumn = "No Grouping";
                }
                this.dgPortfolioDetails.GroupPanelItemStyle = this.Resources["GridViewGroupPanelItemStyle"] as Style;
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(this.DataContextPortfolioDetails.Logger, ex);
            }
        }

        /// <summary>
        /// Event when User starts filtering the data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgPortfolioDetails_Filtering(object sender, Telerik.Windows.Controls.GridView.GridViewFilteringEventArgs e)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(this.DataContextPortfolioDetails.Logger, methodNamespace);
            try
            {
                MemberColumnFilterDescriptor filteredColumn = e.ColumnFilterDescriptor as MemberColumnFilterDescriptor;
                DataContextPortfolioDetails.FilterDescriptor = filteredColumn.Member;
                bool action = e.Cancel;
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(this.DataContextPortfolioDetails.Logger, ex);
            }
        }

        /// <summary>
        /// Event after grid has applied filtering. calls method to re-weight the Portfolio, Benchmark & Active-Position %
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgPortfolioDetails_Filtered(object sender, Telerik.Windows.Controls.GridView.GridViewFilteredEventArgs e)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(this.DataContextPortfolioDetails.Logger, methodNamespace);
            try
            {
                SetGroupedData();
                gridFilterDescriptors = dgPortfolioDetails.FilterDescriptors;
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(this.DataContextPortfolioDetails.Logger, ex);
            }
        }

        /// <summary>
        /// Getting the currently filtered/grouped items from the DataGrid
        /// </summary>
        private void SetGroupedData()
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(this.DataContextPortfolioDetails.Logger, methodNamespace);
            try
            {
                RangeObservableCollection<PortfolioDetailsData> collection = new RangeObservableCollection<PortfolioDetailsData>();
                foreach (PortfolioDetailsData item in dgPortfolioDetails.Items)
                {
                    PortfolioDetailsData data = new PortfolioDetailsData();
                    data.ActivePosition = item.ActivePosition;
                    data.AsecSecShortName = item.AsecSecShortName;
                    data.AshEmmModelWeight = item.AshEmmModelWeight;
                    data.BalanceNominal = item.BalanceNominal;
                    data.BenchmarkWeight = item.BenchmarkWeight;
                    data.DirtyValuePC = item.DirtyValuePC;
                    data.IndustryName = item.IndustryName;
                    data.IsoCountryCode = item.IsoCountryCode;
                    data.IssueName = item.IssueName;
                    data.MarketCapUSD = item.MarketCapUSD;
                    data.PortfolioDirtyValuePC = item.PortfolioDirtyValuePC;
                    data.PortfolioWeight = item.PortfolioWeight;
                    data.ProprietaryRegionCode = item.ProprietaryRegionCode;
                    data.ReAshEmmModelWeight = item.ReAshEmmModelWeight;
                    data.ReBenchmarkWeight = item.ReBenchmarkWeight;
                    data.RePortfolioWeight = item.RePortfolioWeight;
                    data.SectorName = item.SectorName;
                    data.SecurityType = item.SecurityType;
                    data.SubIndustryName = item.SubIndustryName;
                    data.Ticker = item.Ticker;
                    data.MarketCap = item.MarketCap;
                    data.Upside = item.Upside;
                    data.ForwardPE = item.ForwardPE;
                    data.ForwardPBV = item.ForwardPBV;
                    data.ForwardEB_EBITDA = item.ForwardEB_EBITDA;
                    data.RevenueGrowthCurrentYear = item.RevenueGrowthCurrentYear;
                    data.RevenueGrowthNextYear = item.RevenueGrowthNextYear;
                    data.NetIncomeGrowthCurrentYear = item.NetIncomeGrowthCurrentYear;
                    data.NetIncomeGrowthNextYear = item.NetIncomeGrowthNextYear;
                    data.ROE = item.ROE;
                    data.NetDebtEquity = item.NetDebtEquity;
                    data.FreecashFlowMargin = item.FreecashFlowMargin;
                    collection.Add(data);
                }
                DataContextPortfolioDetails.GroupedFilteredPortfolioDetailsData = collection;
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(this.DataContextPortfolioDetails.Logger, ex);
            }
        }

        #endregion

        #region EventUnsubscribe

        /// <summary>
        /// Unsusbcribing the Events
        /// </summary>
        public override void Dispose()
        {
            this.DataContextPortfolioDetails.Dispose();
            this.DataContextPortfolioDetails = null;
            this.DataContext = null;
        }

        #endregion

        #region HelperMethods

        /// <summary>
        /// DataLoaded Event of DataGrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgPortfolioDetails_DataLoaded(object sender, EventArgs e)
        {
            if (this.DataContextPortfolioDetails.CheckFilterApplied == 1)
            {
                this.DataContextPortfolioDetails.CheckFilterApplied--;
                if (this.DataContextPortfolioDetails.CheckFilterApplied == 0)
                {
                    SetGroupedData();
                    this.dgPortfolioDetails.Items.CommitEdit();
                }
            }
        }

        /// <summary>
        /// Include Benchmark Checkbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            gridDataSource = this.dgPortfolioDetails.ItemsSource as RangeObservableCollection<PortfolioDetailsData>;
            this.dgPortfolioDetails.Items.EditItem(gridDataSource);
        }

        /// <summary>
        /// Change the Headers of the Grid According to Year
        /// </summary>
        private void ChangeHeaders()
        {
            this.dgPortfolioDetails.Columns[20].Header = "Revenue Growth " + DateTime.Today.Year.ToString();
            this.dgPortfolioDetails.Columns[21].Header = "Revenue Growth " + (DateTime.Today.Year + 1).ToString();
            this.dgPortfolioDetails.Columns[22].Header = "Net Income Growth " + DateTime.Today.Year.ToString();
            this.dgPortfolioDetails.Columns[23].Header = "Net Income Growth " + (DateTime.Today.Year + 1).ToString();

            this.dgPortfolioDetails.Columns[20].UniqueName = "Revenue Growth " + DateTime.Today.Year.ToString();
            this.dgPortfolioDetails.Columns[21].UniqueName = "Revenue Growth " + (DateTime.Today.Year + 1).ToString();
            this.dgPortfolioDetails.Columns[22].UniqueName = "Net Income Growth " + DateTime.Today.Year.ToString();
            this.dgPortfolioDetails.Columns[23].UniqueName = "Net Income Growth " + (DateTime.Today.Year + 1).ToString();
        }

        #endregion
    }
}