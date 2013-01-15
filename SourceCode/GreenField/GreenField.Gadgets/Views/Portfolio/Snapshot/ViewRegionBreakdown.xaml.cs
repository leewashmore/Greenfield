using System;
using System.Collections.Generic;
using System.Windows;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;
using GreenField.Common;
using GreenField.Gadgets.Helpers;
using GreenField.Gadgets.ViewModels;
using GreenField.ServiceCaller;
using System.Collections.ObjectModel;
using GreenField.DataContracts;
using Telerik.Windows.Data;
using System.Linq;

namespace GreenField.Gadgets.Views
{
    public partial class ViewRegionBreakdown : ViewBaseUserControl
    {
        #region Property
        /// <summary>
        /// property to set data context
        /// </summary>
        private ViewModelRegionBreakdown dataContextRegionBreakdown;
        public ViewModelRegionBreakdown DataContextRegionBreakdown
        {
            get { return dataContextRegionBreakdown; }
            set { dataContextRegionBreakdown = value; }
        }

        /// <summary>
        /// property to set IsActive variable of View Model
        /// </summary>
        private bool isActive;
        public override bool IsActive
        {
            get { return isActive; }
            set
            {
                isActive = value;
                if (DataContextRegionBreakdown != null)
                { DataContextRegionBreakdown.IsActive = isActive; }
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="dataContextSource">ViewModelRegionBreakdown</param>
        public ViewRegionBreakdown(ViewModelRegionBreakdown dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextRegionBreakdown = dataContextSource;
        }
        #endregion

        #region Method to Flip
        /// <summary>
        /// Flipping between Grid & PieChart
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFlip_Click(object sender, RoutedEventArgs e)
        {
            if (this.crtRegionBreakdown.Visibility == System.Windows.Visibility.Visible)
            {
                Flipper.FlipItem(this.crtRegionBreakdown, this.dgRegionBreakdown);
            }
            else
            {
                Flipper.FlipItem(this.dgRegionBreakdown, this.crtRegionBreakdown);
            }
        }
        #endregion

        #region Export To Excel
        /// <summary>
        /// Method to catch Click Event of Export to Excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.crtRegionBreakdown.Visibility == Visibility.Visible)
                {
                    List<RadExportOptions> radExportOptionsInfo = new List<RadExportOptions>
                {                   
                    new RadExportOptions()
                    {
                        ElementName = "Region Breakdown Chart",
                        Element = this.crtRegionBreakdown, 
                        ExportFilterOption = RadExportFilterOption.RADCHART_EXCEL_EXPORT_FILTER 
                    },                 
                };
                    ChildExportOptions childExportOptions = new ChildExportOptions(radExportOptionsInfo, "Export Options: " + GadgetNames.HOLDINGS_REGION_BREAKDOWN);
                    childExportOptions.Show();
                }
                else
                {
                    if (this.dgRegionBreakdown.Visibility == Visibility.Visible)
                    {
                        ChildExportOptions childExportOptions = new ChildExportOptions(new List<RadExportOptions>
                            {
                                new RadExportOptions() 
                                {
                                    Element = this.dgRegionBreakdown,
                                    ElementName = "Region Breakdown Data",
                                    ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXCEL_EXPORT_FILTER
                                }
                            }, "Export Options: " + GadgetNames.HOLDINGS_REGION_BREAKDOWN);
                        childExportOptions.Show();
                    }
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
            try
            {
                if (this.crtRegionBreakdown.Visibility == Visibility.Visible)
                {
                    List<RadExportOptions> radExportOptionsInfo = new List<RadExportOptions>
                {                   
                    new RadExportOptions()
                    {
                        ElementName = "Region Breakdown Chart",
                        Element = this.crtRegionBreakdown, 
                        ExportFilterOption = RadExportFilterOption.RADCHART_PRINT_FILTER,
                        RichTextBox = this.RichTextBox
                    }
                };
                    ChildExportOptions childExportOptions = new ChildExportOptions(radExportOptionsInfo, "Export Options: " + GadgetNames.HOLDINGS_REGION_BREAKDOWN);
                    childExportOptions.Show();
                }
                else
                {
                    if (this.dgRegionBreakdown.Visibility == Visibility.Visible)
                    {
                        ChildExportOptions childExportOptions = new ChildExportOptions(new List<RadExportOptions>
                            {
                                new RadExportOptions() 
                                {
                                    Element = this.dgRegionBreakdown,
                                    ElementName = "Region Breakdown Data",
                                    ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_PRINT_FILTER,
                                    RichTextBox = this.RichTextBox
                                }
                            }, "Export Options: " + GadgetNames.HOLDINGS_REGION_BREAKDOWN);
                        childExportOptions.Show();
                    }
                }
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

                if (this.crtRegionBreakdown.Visibility == Visibility.Visible)
                {
                    List<RadExportOptions> radExportOptionsInfo = new List<RadExportOptions>
                {                   
                    new RadExportOptions()
                    {
                        ElementName = "Region Breakdown Chart",
                        Element = this.crtRegionBreakdown, 
                        ExportFilterOption = RadExportFilterOption.RADCHART_PDF_EXPORT_FILTER 
                    },                 
                };
                    ChildExportOptions childExportOptions = new ChildExportOptions(radExportOptionsInfo, "Export Options: "
                        + GadgetNames.HOLDINGS_REGION_BREAKDOWN);
                    childExportOptions.Show();
                }
                else
                {
                    if (this.dgRegionBreakdown.Visibility == Visibility.Visible)
                    {
                        ChildExportOptions childExportOptions = new ChildExportOptions(new List<RadExportOptions>
                            {
                                new RadExportOptions() 
                                {
                                    Element = this.dgRegionBreakdown,
                                    ElementName = "Region Breakdown Data",
                                    ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_PDF_EXPORT_FILTER
                                }
                            }, "Export Options: " + GadgetNames.HOLDINGS_REGION_BREAKDOWN);
                        childExportOptions.Show();
                    }
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
            }
        }

        /// <summary>
        /// handles element exporting for export to excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgRegionBreakdown_ElementExporting(object sender, GridViewElementExportingEventArgs e)
        {
            RadGridView_ElementExport.ElementExporting(e, isGroupFootersVisible: true, aggregatedColumnIndex: new List<int> { 1, 2, 3 });
        }
        #endregion

        #region Dispose Method
        /// <summary>
        /// method to dispose all running events
        /// </summary>
        public override void Dispose()
        {
            this.DataContextRegionBreakdown.Dispose();
            this.DataContextRegionBreakdown = null;
            this.DataContext = null;
        }
        #endregion

        private void dgRegionBreakdown_Sorting(object sender, GridViewSortingEventArgs e)
        {
            List<RegionBreakdownData> datasource = (e.DataControl.ItemsSource as ObservableCollection<RegionBreakdownData>).ToList();
            DataItemCollection collection = (sender as RadGridView).Items;
            List<RegionBreakdownData> data = new List<RegionBreakdownData>();
            foreach (RegionBreakdownData item in collection)
            {
                data.Add(item);
            }

            if ((e.Column as GridViewDataColumn).DataMemberBinding.Path.Path == "Security" ||
                e.NewSortingState == SortingState.None)
            {
                ArrangeSortOrder(data);
                return;
            }

            List<String> distinctRegions = data.Select(a => a.Region).Distinct().ToList();
            List<OrderedAggregates> orderedRegion = GetRegionAggregates(data, distinctRegions, (e.Column as GridViewDataColumn).DataMemberBinding.Path.Path);
            orderedRegion = e.NewSortingState == SortingState.Ascending
                ? orderedRegion.OrderBy(a => a.Aggregate).ToList()
                : orderedRegion.OrderByDescending(a => a.Aggregate).ToList();

            int regionCount = 1;
            foreach (String region in orderedRegion.Select(a => a.Description))
            {
                List<string> distinctCountries = data.Where(a => a.Region == region).OrderBy(a => a.Country)
                        .Select(a => a.Country).Distinct().ToList();
                List<OrderedAggregates> orderedCountnies = GetCountryAggregates(data, distinctCountries, region, (e.Column as GridViewDataColumn).DataMemberBinding.Path.Path);
                orderedCountnies = e.NewSortingState == SortingState.Ascending
                    ? orderedCountnies.OrderBy(a => a.Aggregate).ToList()
                    : orderedCountnies.OrderByDescending(a => a.Aggregate).ToList();
                int countryCount = 1;
                foreach (String country in orderedCountnies.Select(a => a.Description))
                {
                    foreach (RegionBreakdownData item in datasource)
                    {
                        if (item.Region == region && item.Country == country)
                        {
                            item.RegionSortOrder = String.Format("{0}. {1}", regionCount < 10 ? " " + regionCount.ToString() : regionCount.ToString("00"), region);
                            item.CountrySortOrder = String.Format("{0}. {1}", countryCount < 10 ? " " + countryCount.ToString() : countryCount.ToString("00"), country);
                        }
                    }
                    countryCount++;
                }
                regionCount++;
            }

            this.dgRegionBreakdown.ItemsSource = new ObservableCollection<RegionBreakdownData>(datasource);
            
        }

        public void ArrangeSortOrder(List<RegionBreakdownData> data)
        {
            List<String> distinctRegions = data.OrderBy(a => a.Region)
                .Select(a => a.Region).Distinct().ToList();
            int regionCount = 1;
            foreach (String region in distinctRegions)
            {
                List<string> distinctCountries = data.Where(a => a.Region == region).OrderBy(a => a.Country)
                        .Select(a => a.Country).Distinct().ToList();
                int countryCount = 1;
                foreach (String country in distinctCountries)
                {
                    List<RegionBreakdownData> records = data.Where(a => a.Region == region && a.Country == country).ToList();
                    foreach (RegionBreakdownData record in records)
                    {
                        record.RegionSortOrder = String.Format("{0}. {1}", regionCount < 10 ? " " + regionCount.ToString() : regionCount.ToString("00"), region);
                        record.CountrySortOrder = String.Format("{0}. {1}", countryCount < 10 ? " " + countryCount.ToString() : countryCount.ToString("00"), country);
                    }
                    countryCount++;
                }
                regionCount++;
            }
        }

        private List<OrderedAggregates> GetRegionAggregates(List<RegionBreakdownData> data, List<String> distinctRegions
            , String propertyName)
        {
            List<OrderedAggregates> orderedAggregates = new List<OrderedAggregates>();
            switch (propertyName)
            {
                case "PortfolioShare":
                    foreach (String description in distinctRegions)
                    {
                        decimal? aggregate = data.Where(a => a.Region == description)
                            .Sum(a => a.PortfolioShare);
                        orderedAggregates.Add(new OrderedAggregates() { Aggregate = aggregate, Description = description });
                    }
                    break;
                case "BenchmarkShare":
                    foreach (String description in distinctRegions)
                    {
                        decimal? aggregate = data.Where(a => a.Region == description)
                            .Sum(a => a.BenchmarkShare);
                        orderedAggregates.Add(new OrderedAggregates() { Aggregate = aggregate, Description = description });
                    }
                    break;
                case "ActivePosition":
                    foreach (String description in distinctRegions)
                    {
                        decimal? aggregate = data.Where(a => a.Region == description)
                            .Sum(a => a.ActivePosition);
                        orderedAggregates.Add(new OrderedAggregates() { Aggregate = aggregate, Description = description });
                    }
                    break;
                default:
                    return null;
            }

            return orderedAggregates;
        }

        private List<OrderedAggregates> GetCountryAggregates(List<RegionBreakdownData> data, List<String> distinctCountries
            , String region, String propertyName)
        {
            List<OrderedAggregates> orderedAggregates = new List<OrderedAggregates>();
            switch (propertyName)
            {
                case "PortfolioShare":
                    foreach (String description in distinctCountries)
                    {
                        decimal? aggregate = data.Where(a => a.Region == region && a.Country == description)
                            .Sum(a => a.PortfolioShare);
                        orderedAggregates.Add(new OrderedAggregates() { Aggregate = aggregate, Description = description });
                    }
                    break;
                case "BenchmarkShare":
                    foreach (String description in distinctCountries)
                    {
                        decimal? aggregate = data.Where(a => a.Region == region && a.Country == description)
                            .Sum(a => a.BenchmarkShare);
                        orderedAggregates.Add(new OrderedAggregates() { Aggregate = aggregate, Description = description });
                    }
                    break;
                case "ActivePosition":
                    foreach (String description in distinctCountries)
                    {
                        decimal? aggregate = data.Where(a => a.Region == region && a.Country == description)
                            .Sum(a => a.ActivePosition);
                        orderedAggregates.Add(new OrderedAggregates() { Aggregate = aggregate, Description = description });
                    }
                    break;
                default:
                    return null;
            }

            return orderedAggregates;
        }

        class OrderedAggregates
        {
            public String Description { get; set; }
            public object Aggregate { get; set; }
        }
    }
}
