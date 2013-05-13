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
using System.Windows.Data;

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
                                    ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXCEL_EXPORT_FILTER }
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
            if (e.Element == ExportElement.Row)
            {
                var value = e.Value as PortfolioDetailsData;
                if (value.Children != null)
                {

                    foreach (var child in value.Children)
                    {
                        e.Value = child;
                        RadGridView_ElementExport.ElementExporting(e, isGroupFootersVisible: false);
                    }
                }
                else
                {
                    RadGridView_ElementExport.ElementExporting(e, isGroupFootersVisible: false);
                }
            }
            else
            {
                RadGridView_ElementExport.ElementExporting(e, isGroupFootersVisible: false);
                //The following commented code could be used if reformatting in exported file is necessary - Lane 05-09-2013
                /*RadGridView_ElementExport.ElementExporting(e, isGroupFootersVisible: false, cellValueConverter: () =>
                {
                    object result = e.Value;
                    if (e.Value != null && e.Element == ExportElement.Cell)
                    {
                        GridViewDataColumn column = (e.Context as GridViewDataColumn);
                        if (column != null && (column.DisplayIndex == 4 || column.DisplayIndex == 5 || column.DisplayIndex == 6 || column.DisplayIndex == 7))  //might be able to use names.
                        {
                            decimal? resultInt = 0;
                            resultInt = Convert.ToDecimal(result);  
                            resultInt = resultInt/100; 
                            result = resultInt.ToString();
                        }
                    }
                    return result;
                });
               */
            }
        }

        private void dgPortfolioDetails_ElementExported(object sender, GridViewElementExportedEventArgs e)
        {
            if (e.Element == ExportElement.Row)
            {
                var value = e.Context as PortfolioDetailsData;
            }
        }
        #endregion

        /// <summary>
        /// Printing the DataGrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            try
            {
                List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>();
                RadExportOptionsInfo.Add(new RadExportOptions()
                {
                    ElementName = "Portfolio Risk Return",
                    Element = this.dgPortfolioDetails,
                    ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_PRINT_FILTER,
                    RichTextBox = this.RichTextBox
                });

                ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + ExportTypes.PORTFOLIO_DETAILS_UI);
                childExportOptions.Show();

            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
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
            try
            {

                List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>();
                RadExportOptionsInfo.Add(new RadExportOptions()
                {
                    ElementName = "Portfolio Risk Return",
                    Element = this.dgPortfolioDetails,
                    ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_PDF_EXPORT_FILTER
                });
                ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + ExportTypes.PORTFOLIO_DETAILS_UI);
                childExportOptions.Show();
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
            }
        }

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
                        if (!this.dgPortfolioDetails.GroupDescriptors.Contains(e.GroupDescriptor))
                        {
                            e.Cancel = true;
                            // this.dgPortfolioDetails.GroupDescriptors.Clear();
                            dgPortfolioDetails.GroupDescriptors.Add(e.GroupDescriptor);

                        }
                        Telerik.Windows.Controls.GridView.ColumnGroupDescriptor groupDescriptor = e.GroupDescriptor as Telerik.Windows.Controls.GridView.ColumnGroupDescriptor;
                        DataContextPortfolioDetails.GroupingColumn = Convert.ToString(groupDescriptor.Column.UniqueName);
                    }



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
                // SetGroupedData();
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
            if (this.DataContextPortfolioDetails.EnableLookThru)
            {
                dgPortfolioDetails.Columns["Portfolio Name"].IsVisible = true;
                dgPortfolioDetails.Columns["Portfolio Path"].IsVisible = true;
            }
            else
            {
                dgPortfolioDetails.Columns["Portfolio Name"].IsVisible = false;
                dgPortfolioDetails.Columns["Portfolio Path"].IsVisible = false;
            }

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
            //this.dgPortfolioDetails.Columns["Revenue Growth Current Year"].Header = "Revenue Growth " + DateTime.Today.Year.ToString();
            //this.dgPortfolioDetails.Columns["Revenue Growth Next Year"].Header = "Revenue Growth " + (DateTime.Today.Year + 1).ToString();
            //this.dgPortfolioDetails.Columns["Net Income Growth Current Year"].Header = "Net Income Growth " + DateTime.Today.Year.ToString();
            //this.dgPortfolioDetails.Columns["Net Income Growth Next Year"].Header = "Net Income Growth " + (DateTime.Today.Year + 1).ToString();

            this.dgPortfolioDetails.Columns["Revenue Growth Current Year"].UniqueName = "Revenue Growth " + DateTime.Today.Year.ToString();
            this.dgPortfolioDetails.Columns["Revenue Growth Next Year"].UniqueName = "Revenue Growth " + (DateTime.Today.Year + 1).ToString();
            this.dgPortfolioDetails.Columns["Net Income Growth Current Year"].UniqueName = "Net Income Growth " + DateTime.Today.Year.ToString();
            this.dgPortfolioDetails.Columns["Net Income Growth Next Year"].UniqueName = "Net Income Growth " + (DateTime.Today.Year + 1).ToString();

        }

        #endregion

        private void dgPortfolioDetails_RowLoaded(object sender, RowLoadedEventArgs e)
        {
            GridViewRow row = e.Row as GridViewRow;
            var data = e.DataElement as PortfolioDetailsData;
            if (data != null && row != null)
                row.IsExpandable = data.IsExpanded;

        }

        private void dgPortfolioDetails_DataLoading(object sender, GridViewDataLoadingEventArgs e)
        {
            GridViewDataControl dataControl = (GridViewDataControl)sender;
            if (dataControl.ParentRow != null)
            {
                //dataControl is the child gridview
                dataControl.ShowGroupPanel = false;

            }
        }

        //private void BindDetailsColumnsWidth(RadGridView oDetailsGrid, RadGridView oMasterGrid)
        //{
        //    Telerik.Windows.Controls.GridViewColumn oCol;

        //    for (int i = 0; i < oDetailsGrid.Columns.Count - 1; i++)
        //    {
        //        oCol = oDetailsGrid.Columns[i];

        //        if (oCol.IsVisible == true)
        //        {
        //            oCol.SetValue(Telerik.Windows.Controls.GridViewColumn.WidthProperty,
        //                        new Binding("ActualWidth")
        //                        {
        //                            Source = oMasterGrid.Columns[i]
        //                        });
        //        }
        //    }
        //}

    }
}