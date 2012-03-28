using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.ComponentModel.Composition;
using GreenField.Benchmark.ViewModels;
using Telerik.Windows.Controls;
using System.IO;
using Telerik.Windows.Documents.Model;
using Telerik.Windows.Documents.FormatProviders.Pdf;
using Telerik.Windows.Data;
using System.Collections;

#if !SILVERLIGHT
using Microsoft.Win32;
#else
using System.Windows.Controls;
using System.Windows.Printing;
#endif

namespace GreenField.Benchmark.Views
{
    [Export(typeof(ViewPortfolioDetails))]
    public partial class ViewPortfolioDetails : UserControl
    {
        #region Private Variables

        double offsetY;
        double totalHeight;
        Canvas canvas;
        RadGridView grid;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public ViewPortfolioDetails()
        {
            InitializeComponent();
        }

        #endregion

        #region SettingDataContext
        /// <summary>
        /// Setting the DataContext to the ViewModel
        /// </summary>
        [Import]
        public ViewModelPortfolioDetails DataContextSource
        {
            set
            {
                this.DataContext = value;
            }
        }

        #endregion

        #region ExportToExcel

        /// <summary>
        /// Event handler when user wants to Export the Grid to ExcelSheet
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            string extension = "xls";
            string selectedItem = "Excel";
            ExportFormat format = ExportFormat.Html;
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.DefaultExt = extension;
            dialog.Filter = String.Format("{1} files (*.{0})|*.{0}|All files (*.*)|*.*", extension, selectedItem);
            dialog.FilterIndex = 1;

            if (dialog.ShowDialog() == true)
            {
                using (Stream stream = dialog.OpenFile())
                {
                    GridViewExportOptions exportOptions = new GridViewExportOptions();
                    exportOptions.Format = format;
                    exportOptions.ShowColumnFooters = true;
                    exportOptions.ShowColumnHeaders = true;
                    exportOptions.ShowGroupFooters = true;
                    dgPortfolioDetails.Export(stream, exportOptions);
                }
            }
        }

        #endregion

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
            span.Text = value;
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

        //private void btnPrint_Click(object sender, System.Windows.RoutedEventArgs e)
        //{
        //    RadRichTextBox radTxtDoc = new RadRichTextBox();
        //    Dispatcher.BeginInvoke((Action)(() =>
        //    {
        //        radTxtDoc.Document = CreateDocument(dgPortfolioDetails);
        //    }));
        //    //radTxtDoc.Print("MyDocument", Telerik.Windows.Documents.UI.PrintMode.Native);
        //    radTxtDoc.Print(radTxtDoc.Document.ToString(), Telerik.Windows.Documents.UI.PrintMode.Native);
        //}

        #region Printing the DataGrid

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

        #endregion
    }
}