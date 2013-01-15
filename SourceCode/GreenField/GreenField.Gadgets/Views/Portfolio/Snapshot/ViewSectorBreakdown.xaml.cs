using System;
using System.Collections.Generic;
using System.Windows;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;
using GreenField.Common;
using GreenField.Gadgets.Helpers;
using GreenField.Gadgets.ViewModels;
using GreenField.ServiceCaller;
using GreenField.DataContracts;
using System.Collections.ObjectModel;
using Telerik.Windows.Data;
using System.Linq;

namespace GreenField.Gadgets.Views
{
    public partial class ViewSectorBreakdown : ViewBaseUserControl
    {
        #region Properties
        /// <summary>
        /// property to set data context
        /// </summary>
        private ViewModelSectorBreakdown dataContextSectorBreakdown;
        public ViewModelSectorBreakdown DataContextSectorBreakdown
        {
            get { return dataContextSectorBreakdown; }
            set { dataContextSectorBreakdown = value; }
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
                if (DataContextSectorBreakdown != null)
                { DataContextSectorBreakdown.IsActive = isActive; }
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataContextSource"></param>
        public ViewSectorBreakdown(ViewModelSectorBreakdown dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextSectorBreakdown = dataContextSource;
        } 
        #endregion

        #region Flip Method
        /// <summary>
        /// Flipping between Grid & PieChart
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFlip_Click(object sender, RoutedEventArgs e)
        {
            if (this.crtSectorBreakdown.Visibility == System.Windows.Visibility.Visible)
            {
                Flipper.FlipItem(this.crtSectorBreakdown, this.dgSectorBreakdown);
            }
            else
            {
                Flipper.FlipItem(this.dgSectorBreakdown, this.crtSectorBreakdown);
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
                if (this.crtSectorBreakdown.Visibility == Visibility.Visible)
                {
                    List<RadExportOptions> radExportOptionsInfo = new List<RadExportOptions>{ new RadExportOptions()
                    {
                        ElementName = "Sector Breakdown Chart",
                        Element = this.crtSectorBreakdown, 
                        ExportFilterOption = RadExportFilterOption.RADCHART_EXCEL_EXPORT_FILTER 
                    },              
                };
                    ChildExportOptions childExportOptions = new ChildExportOptions(radExportOptionsInfo, "Export Options: " + GadgetNames.HOLDINGS_SECTOR_BREAKDOWN);
                    childExportOptions.Show();
                }
                else
                {
                    if (this.dgSectorBreakdown.Visibility == Visibility.Visible)
                    {
                        ChildExportOptions childExportOptions = new ChildExportOptions(new List<RadExportOptions>
                        {
                            new RadExportOptions()
                            {
                                Element = this.dgSectorBreakdown,
                                ElementName = "Sector Breakdown Data",
                                ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXCEL_EXPORT_FILTER
                            }
                        }, "Export Options: " + GadgetNames.HOLDINGS_SECTOR_BREAKDOWN);
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
                if (this.crtSectorBreakdown.Visibility == Visibility.Visible)
                {
                    List<RadExportOptions> radExportOptionsInfo = new List<RadExportOptions>{ new RadExportOptions()
                    {
                        ElementName = "Sector Breakdown Chart",
                        Element = this.crtSectorBreakdown, 
                        ExportFilterOption = RadExportFilterOption.RADCHART_PRINT_FILTER,
                        RichTextBox = this.RichTextBox
                    },              
                };
                    ChildExportOptions childExportOptions = new ChildExportOptions(radExportOptionsInfo, "Export Options: " + GadgetNames.HOLDINGS_SECTOR_BREAKDOWN);
                    childExportOptions.Show();
                }
                else
                {
                    if (this.dgSectorBreakdown.Visibility == Visibility.Visible)
                    {
                        ChildExportOptions childExportOptions = new ChildExportOptions(new List<RadExportOptions>
                        {
                            new RadExportOptions()
                            {
                                Element = this.dgSectorBreakdown,
                                ElementName = "Sector Breakdown Data",
                                ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_PRINT_FILTER,
                                RichTextBox = this.RichTextBox
                            }
                        }, "Export Options: " + GadgetNames.HOLDINGS_SECTOR_BREAKDOWN);
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

                if (this.crtSectorBreakdown.Visibility == Visibility.Visible)
                {
                    List<RadExportOptions> radExportOptionsInfo = new List<RadExportOptions>{ new RadExportOptions()
                    {
                        ElementName = "Sector Breakdown Chart",
                        Element = this.crtSectorBreakdown, 
                        ExportFilterOption = RadExportFilterOption.RADCHART_PDF_EXPORT_FILTER 
                    },              
                };
                    ChildExportOptions childExportOptions = new ChildExportOptions(radExportOptionsInfo, "Export Options: " + GadgetNames.HOLDINGS_SECTOR_BREAKDOWN);
                    childExportOptions.Show();
                }
                else
                {
                    if (this.dgSectorBreakdown.Visibility == Visibility.Visible)
                    {
                        ChildExportOptions childExportOptions = new ChildExportOptions(new List<RadExportOptions>
                        {
                            new RadExportOptions()
                            {
                                Element = this.dgSectorBreakdown,
                                ElementName = "Sector Breakdown Data",
                                ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_PDF_EXPORT_FILTER
                            }
                        }, "Export Options: " + GadgetNames.HOLDINGS_SECTOR_BREAKDOWN);
                        childExportOptions.Show();
                    }
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
            }
        }

        private void dgSectorBreakdown_ElementExporting(object sender, GridViewElementExportingEventArgs e)
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
            this.DataContextSectorBreakdown.Dispose();
            this.DataContextSectorBreakdown = null;
            this.DataContext = null;
        } 
        #endregion

        private void dgSectorBreakdown_Sorting(object sender, GridViewSortingEventArgs e)
        {
            List<SectorBreakdownData> datasource = (e.DataControl.ItemsSource as ObservableCollection<SectorBreakdownData>).ToList();
            DataItemCollection collection = (sender as RadGridView).Items;
            List<SectorBreakdownData> data = new List<SectorBreakdownData>();
            foreach (SectorBreakdownData item in collection)
            {
                data.Add(item);
            }

            if ((e.Column as GridViewDataColumn).DataMemberBinding.Path.Path == "Security" ||
                e.NewSortingState == SortingState.None)
            {
                ArrangeSortOrder(data);
                return;
            }

            List<String> distinctSectors = data.Select(a => a.Sector).Distinct().ToList();
            List<OrderedAggregates> orderedSectors = GetRegionAggregates(data, distinctSectors, (e.Column as GridViewDataColumn).DataMemberBinding.Path.Path);
            orderedSectors = e.NewSortingState == SortingState.Ascending
                ? orderedSectors.OrderBy(a => a.Aggregate).ToList()
                : orderedSectors.OrderByDescending(a => a.Aggregate).ToList();

            int sectorCount = 1;
            foreach (String sector in orderedSectors.Select(a => a.Description))
            {
                List<string> distinctIndustries = data.Where(a => a.Sector == sector).OrderBy(a => a.Industry)
                        .Select(a => a.Industry).Distinct().ToList();
                List<OrderedAggregates> orderedIndustries = GetCountryAggregates(data, distinctIndustries, sector, (e.Column as GridViewDataColumn).DataMemberBinding.Path.Path);
                orderedIndustries = e.NewSortingState == SortingState.Ascending
                    ? orderedIndustries.OrderBy(a => a.Aggregate).ToList()
                    : orderedIndustries.OrderByDescending(a => a.Aggregate).ToList();
                int industryCount = 1;
                foreach (String country in orderedIndustries.Select(a => a.Description))
                {
                    foreach (SectorBreakdownData item in datasource)
                    {
                        if (item.Sector == sector && item.Industry == country)
                        {
                            item.SectorSortOrder = String.Format("{0}. {1}", sectorCount < 10 ? " " + sectorCount.ToString() : sectorCount.ToString("00"), sector);
                            item.IndustrySortOrder = String.Format("{0}. {1}", industryCount < 10 ? " " + industryCount.ToString() : industryCount.ToString("00"), country);
                        }
                    }
                    industryCount++;
                }
                sectorCount++;
            }           
            this.dgSectorBreakdown.ItemsSource = new ObservableCollection<SectorBreakdownData>(datasource);
        }

        public void ArrangeSortOrder(List<SectorBreakdownData> data)
        {
            List<String> distinctRegions = data.OrderBy(a => a.Sector)
                .Select(a => a.Sector).Distinct().ToList();
            int regionCount = 1;
            foreach (String region in distinctRegions)
            {
                List<string> distinctCountries = data.Where(a => a.Sector == region).OrderBy(a => a.Industry)
                        .Select(a => a.Industry).Distinct().ToList();
                int countryCount = 1;
                foreach (String country in distinctCountries)
                {
                    List<SectorBreakdownData> records = data.Where(a => a.Sector == region && a.Industry == country).ToList();
                    foreach (SectorBreakdownData record in records)
                    {
                        record.SectorSortOrder = String.Format("{0}. {1}", regionCount < 10 ? " " + regionCount.ToString() : regionCount.ToString("00"), region);
                        record.IndustrySortOrder = String.Format("{0}. {1}", countryCount < 10 ? " " + countryCount.ToString() : countryCount.ToString("00"), country);
                    }
                    countryCount++;
                }
                regionCount++;
            }
        }

        private List<OrderedAggregates> GetRegionAggregates(List<SectorBreakdownData> data, List<String> distinctRegions
            , String propertyName)
        {
            List<OrderedAggregates> orderedAggregates = new List<OrderedAggregates>();
            switch (propertyName)
            {
                case "PortfolioShare":
                    foreach (String description in distinctRegions)
                    {
                        decimal? aggregate = data.Where(a => a.Sector == description)
                            .Sum(a => a.PortfolioShare);
                        orderedAggregates.Add(new OrderedAggregates() { Aggregate = aggregate, Description = description });
                    }
                    break;
                case "BenchmarkShare":
                    foreach (String description in distinctRegions)
                    {
                        decimal? aggregate = data.Where(a => a.Sector == description)
                            .Sum(a => a.BenchmarkShare);
                        orderedAggregates.Add(new OrderedAggregates() { Aggregate = aggregate, Description = description });
                    }
                    break;
                case "ActivePosition":
                    foreach (String description in distinctRegions)
                    {
                        decimal? aggregate = data.Where(a => a.Sector == description)
                            .Sum(a => a.ActivePosition);
                        orderedAggregates.Add(new OrderedAggregates() { Aggregate = aggregate, Description = description });
                    }
                    break;
                default:
                    return null;
            }

            return orderedAggregates;
        }

        private List<OrderedAggregates> GetCountryAggregates(List<SectorBreakdownData> data, List<String> distinctCountries
            , String region, String propertyName)
        {
            List<OrderedAggregates> orderedAggregates = new List<OrderedAggregates>();
            switch (propertyName)
            {
                case "PortfolioShare":
                    foreach (String description in distinctCountries)
                    {
                        decimal? aggregate = data.Where(a => a.Sector == region && a.Industry == description)
                            .Sum(a => a.PortfolioShare);
                        orderedAggregates.Add(new OrderedAggregates() { Aggregate = aggregate, Description = description });
                    }
                    break;
                case "BenchmarkShare":
                    foreach (String description in distinctCountries)
                    {
                        decimal? aggregate = data.Where(a => a.Sector == region && a.Industry == description)
                            .Sum(a => a.BenchmarkShare);
                        orderedAggregates.Add(new OrderedAggregates() { Aggregate = aggregate, Description = description });
                    }
                    break;
                case "ActivePosition":
                    foreach (String description in distinctCountries)
                    {
                        decimal? aggregate = data.Where(a => a.Sector == region && a.Industry == description)
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
