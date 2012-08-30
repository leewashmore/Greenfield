using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Telerik.Windows.Controls;
using System.IO;
using Telerik.Windows.Documents.Model;
using Telerik.Windows.Documents.FormatProviders.Pdf;
using Telerik.Windows.Data;
using System.Collections;
using GreenField.DataContracts;

#if !SILVERLIGHT
using Microsoft.Win32;
#else
using System.Windows.Controls;
using System.Windows.Printing;
using GreenField.Gadgets.Helpers;
using Telerik.Windows.Controls.GridView;
using GreenField.ServiceCaller.BenchmarkHoldingsDefinitions;
using GreenField.Gadgets.ViewModels;
using GreenField.Common;
using Telerik.Windows.Documents.UI;
using GreenField.ServiceCaller;
#endif

namespace GreenField.Gadgets.Views
{
    /// <summary>
    /// XAML.cs class for Portfolio Details UI
    /// </summary>
    public partial class ViewPortfolioDetails : ViewBaseUserControl
    {
        #region Private Variables

        double offsetY;
        double totalHeight;
        Canvas canvas;
        RadGridView grid;
        FilterDescriptorCollection gridFilterDescriptors;
        RangeObservableCollection<PortfolioDetailsData> gridDataSource;

        /// <summary>
        /// View Model
        /// </summary>
        private ViewModelPortfolioDetails _dataContextPortfolioDetails;
        public ViewModelPortfolioDetails DataContextPortfolioDetails
        {
            get
            {
                return _dataContextPortfolioDetails;
            }
            set
            {
                _dataContextPortfolioDetails = value;
            }
        }

        /// <summary>
        /// To check whether the Dashboard is Active or not
        /// </summary>
        private bool _isActive;
        public override bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
                if (DataContextPortfolioDetails != null)
                    DataContextPortfolioDetails.IsActive = _isActive;
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
                        new RadExportOptions() { ElementName = ExportTypes.PORTFOLIO_DETAILS_UI, Element = this.dgPortfolioDetails, ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXPORT_FILTER }
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
            RadGridView_ElementExport.ElementExporting(e, showGroupFooters: false);
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
                bool a = e.Cancel;
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
        /// DataLoadingEvent of DataGrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgPortfolioDetails_DataLoading(object sender, GridViewDataLoadingEventArgs e)
        {

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

        #endregion
    }
}