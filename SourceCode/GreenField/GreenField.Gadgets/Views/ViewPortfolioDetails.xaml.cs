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
            try
            {
                List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>
                {
                    new RadExportOptions() { ElementName = ExportTypes.PORTFOLIO_DETAILS_UI, Element = this.dgPortfolioDetails, ExportFilterOption = RadExportFilterOption.RADGRIDVIEW_EXPORT_FILTER },         
                };
                ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + GadgetNames.HOLDINGS_PORTFOLIO_DETAILS_UI);
                childExportOptions.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        /// <summary>
        /// Element Exporting Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgPortfolioDetails_ElementExporting(object sender, GridViewElementExportingEventArgs e)
        {
            RadGridView_ElementExport.ElementExporting(e, showGroupFooters: false);
        }

        /// <summary>
        /// Event handler when user wants to Export the Grid to PDF
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportPDF_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.DefaultExt = "*.pdf";
            dialog.Filter = "Adobe PDF Document (*.pdf)|*.pdf";

            if (dialog.ShowDialog() == true)
            {
                RadDocument document = CreateDocument(dgPortfolioDetails);

                document.LayoutMode = DocumentLayoutMode.Paged;
                document.Measure(RadDocument.MAX_DOCUMENT_SIZE);
                document.Arrange(new RectangleF(PointF.Empty, document.DesiredSize));

                PdfFormatProvider provider = new PdfFormatProvider();
                using (Stream output = dialog.OpenFile())
                {
                    provider.Export(document, output);
                }
            }

        }

        private RadDocument CreateDocument(RadGridView grid)
        {
            List<GridViewBoundColumnBase> columns = (from c in grid.Columns.OfType<GridViewBoundColumnBase>()
                                                     orderby c.DisplayIndex
                                                     select c).ToList();
            Table table = new Table();
            RadDocument document = new RadDocument();
            Telerik.Windows.Documents.Model.Section section = new Telerik.Windows.Documents.Model.Section();
            section.Blocks.Add(table);
            document.Sections.Add(section);

            if (grid.ShowColumnHeaders)
            {
                TableRow headerRow = new TableRow();
                if (grid.GroupDescriptors.Count() > 0)
                {
                    TableCell indentCell = new TableCell();
                    indentCell.PreferredWidth = new TableWidthUnit(grid.GroupDescriptors.Count() * 20);
                    indentCell.Background = Colors.Gray;
                    headerRow.Cells.Add(indentCell);
                }

                for (int i = 0; i < columns.Count(); i++)
                {
                    TableCell cell = new TableCell();
                    cell.Background = Colors.White;
                    AddCellValue(cell, columns[i].UniqueName);
                    cell.PreferredWidth = new TableWidthUnit((float)columns[i].ActualWidth);
                    headerRow.Cells.Add(cell);
                }

                table.Rows.Add(headerRow);
            }

            if (grid.Items.Groups != null)
            {
                for (int i = 0; i < grid.Items.Groups.Count(); i++)
                {
                    AddGroupRow(table, grid.Items.Groups[i] as QueryableCollectionViewGroup, columns, grid);
                }
            }
            else
            {
                AddDataRows(table, grid.Items, columns, grid);
            }

            return document;
        }

        private void AddDataRows(Table table, IList items, IList<GridViewBoundColumnBase> columns, RadGridView grid)
        {
            for (int i = 0; i < items.Count; i++)
            {
                TableRow row = new TableRow();

                if (grid.GroupDescriptors.Count() > 0)
                {
                    TableCell indentCell = new TableCell();
                    indentCell.PreferredWidth = new TableWidthUnit(grid.GroupDescriptors.Count() * 20);
                    indentCell.Background = Colors.White;
                    row.Cells.Add(indentCell);
                }

                for (int j = 0; j < columns.Count(); j++)
                {
                    TableCell cell = new TableCell();

                    object value = columns[j].GetValueForItem(items[i]);

                    AddCellValue(cell, value != null ? value.ToString() : string.Empty);

                    cell.PreferredWidth = new TableWidthUnit((float)columns[j].ActualWidth);
                    cell.Background = Colors.White;

                    row.Cells.Add(cell);
                }

                table.Rows.Add(row);
            }
        }

        private void AddGroupRow(Table table, QueryableCollectionViewGroup group, IList<GridViewBoundColumnBase> columns, RadGridView grid)
        {
            TableRow row = new TableRow();

            int level = GetGroupLevel(group);
            if (level > 0)
            {
                TableCell cell = new TableCell();
                cell.PreferredWidth = new TableWidthUnit(level * 20);
                cell.Background = Colors.White;
                row.Cells.Add(cell);
            }

            TableCell aggregatesCell = new TableCell();
            aggregatesCell.Background = Colors.White;
            aggregatesCell.ColumnSpan = columns.Count() + (grid.GroupDescriptors.Count() > 0 ? 1 : 0) - (level > 0 ? 1 : 0);

            AddCellValue(aggregatesCell, group.Key != null ? group.Key.ToString() : string.Empty);

            foreach (AggregateResult result in group.AggregateResults)
            {
                AddCellValue(aggregatesCell, result.FormattedValue != null ? result.FormattedValue.ToString() : string.Empty);
            }

            row.Cells.Add(aggregatesCell);

            table.Rows.Add(row);

            if (group.HasSubgroups)
            {
                for (int i = 0; i < group.Subgroups.Count(); i++)
                {
                    AddGroupRow(table, group.Subgroups[i] as QueryableCollectionViewGroup, columns, grid);
                }
            }
            else
            {
                for (int i = 0; i < group.ItemCount; i++)
                {
                    AddDataRows(table, group.Items, columns, grid);
                }
            }
        }

        private void AddCellValue(TableCell cell, string value)
        {
            Telerik.Windows.Documents.Model.Paragraph paragraph = new Telerik.Windows.Documents.Model.Paragraph();
            cell.Blocks.Add(paragraph);
            Telerik.Windows.Documents.Model.Span span = new Telerik.Windows.Documents.Model.Span();
            if (value == "")
                value = " ";
            span.Text = value;
            span.FontFamily = new System.Windows.Media.FontFamily("Arial");
            span.FontSize = 7;
            paragraph.Inlines.Add(span);
        }

        private int GetGroupLevel(IGroup group)
        {
            int level = 0;
            IGroup parent = group.ParentGroup;
            while (parent != null)
            {
                level++;
                parent = parent.ParentGroup;
            }
            return level;
        }

        #endregion

        #region Printing the DataGrid

        void doc_PrintPage(object sender, PrintPageEventArgs e)
        {
            e.PageVisual = canvas;
            if (totalHeight == 0)
            {
                totalHeight = grid.DesiredSize.Height;
            }

            Canvas.SetTop(grid, -offsetY);
            offsetY += e.PrintableArea.Height;
            e.HasMorePages = offsetY <= totalHeight;
        }

        /// <summary>
        /// Printing the DataGrid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPrint_Click(object sender, RoutedEventArgs e)
        {
            offsetY = 0d;
            totalHeight = 0d;

            grid = new RadGridView();
            grid.DataContext = dgPortfolioDetails.DataContext;
            grid.ItemsSource = dgPortfolioDetails.ItemsSource;
            grid.RowIndicatorVisibility = Visibility.Collapsed;
            grid.ShowGroupPanel = false;
            grid.CanUserFreezeColumns = false;
            grid.IsFilteringAllowed = false;
            grid.AutoExpandGroups = true;
            grid.AutoGenerateColumns = false;
            grid.FontFamily = new FontFamily("Arial");
            grid.FontSize = 7;


            foreach (GridViewDataColumn column in dgPortfolioDetails.Columns.OfType<GridViewDataColumn>())
            {
                GridViewDataColumn newColumn = new GridViewDataColumn();
                newColumn.DataMemberBinding = new System.Windows.Data.Binding(column.UniqueName);
                grid.Columns.Add(newColumn);
            }

            foreach (GridViewDataColumn column in grid.Columns.OfType<GridViewDataColumn>())
            {
                GridViewDataColumn currentColumn = column;
                GridViewDataColumn originalColumn = (from c in dgPortfolioDetails.Columns.OfType<GridViewDataColumn>()
                                                     where c.UniqueName == currentColumn.UniqueName
                                                     select c).FirstOrDefault();
                if (originalColumn != null)
                {
                    column.Width = originalColumn.ActualWidth;
                    column.DisplayIndex = originalColumn.DisplayIndex;

                }
            }

            StyleManager.SetTheme(grid, StyleManager.GetTheme(dgPortfolioDetails));

            grid.SortDescriptors.AddRange(dgPortfolioDetails.SortDescriptors);
            grid.GroupDescriptors.AddRange(dgPortfolioDetails.GroupDescriptors);
            grid.FilterDescriptors.AddRange(dgPortfolioDetails.FilterDescriptors);

            ScrollViewer.SetHorizontalScrollBarVisibility(grid, ScrollBarVisibility.Hidden);
            ScrollViewer.SetVerticalScrollBarVisibility(grid, ScrollBarVisibility.Hidden);
            PrintDocument doc = new PrintDocument();
            canvas = new Canvas();
            canvas.Children.Add(grid);
            doc.PrintPage += this.doc_PrintPage;
            doc.Print("RadGridView print");
        }

        #endregion

        #region RadDocument

        //private void btnPrint_Click(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    Dispatcher.BeginInvoke((Action)(() =>
        //    {
        //        RichTextBox.Document = CreateDocument(dgPortfolioDetails);
        //    }));

        //    PrintSettings printSettings = new PrintSettings();
        //    printSettings.DocumentName = "MyDocument";
        //    printSettings.PrintMode = PrintMode.Native;
        //    printSettings.PrintScaling = PrintScaling.ShrinkToPageSize;
        //    RichTextBox.Print(printSettings);
        //}

        #endregion

        #region EventUnsubscribe

        public override void Dispose()
        {
            this.DataContextPortfolioDetails.Dispose();
            this.DataContextPortfolioDetails = null;
            this.DataContext = null;
        }

        #endregion

        #region ApplyingGroupStyle

        /// <summary>
        /// Row loaded Event of DataGrid, Applying styles to Grid
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgPortfolioDetails_RowLoaded(object sender, RowLoadedEventArgs e)
        {
            GroupedGridRowLoadedHandler.Implement(e);
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

        /// <summary>
        /// Event when User starts filtering the data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgPortfolioDetails_Filtering(object sender, Telerik.Windows.Controls.GridView.GridViewFilteringEventArgs e)
        {
            IColumnFilterDescriptor filterDescriptor = e.ColumnFilterDescriptor as IColumnFilterDescriptor;
            DataContextPortfolioDetails.FilterDescriptor = e.ColumnFilterDescriptor.DistinctFilter.Member;
            //e.ColumnFilterDescriptor.Column.UniqueName;
        }

        private void dgPortfolioDetails_Filtered(object sender, Telerik.Windows.Controls.GridView.GridViewFilteredEventArgs e)
        {
            SetGroupedData();
        }

        /// <summary>
        /// Getting the currently filtered/grouped items from the DataGrid
        /// </summary>
        private void SetGroupedData()
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

        #endregion

    }
}