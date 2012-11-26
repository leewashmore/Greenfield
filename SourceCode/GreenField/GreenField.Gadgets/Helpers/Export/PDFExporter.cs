using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using GreenField.Common;
using GreenField.ServiceCaller;
using Telerik.Windows.Controls;
using Telerik.Windows.Data;
using Telerik.Windows.Documents.FormatProviders.Pdf;
using Telerik.Windows.Documents.Model;
using Telerik.Windows.Media.Imaging;
using Telerik.Windows.Documents.Layout;
using Telerik.Windows.Controls.GridView;


namespace GreenField.Gadgets.Helpers
{
    /// <summary>
    /// Helper class for Exporting Grid as PDF
    /// </summary>
    public static class PDFExporter
    {
        #region DataGrid

        #region PrivateVariables
        /// <summary>
        /// FontSize of Document
        /// </summary>
        private static int fontSizePDF = 6;
        #endregion

        #region PDF Export
        /// <summary>
        /// Event handler when user wants to Export the Grid to PDF
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void btnExportPDF_Click(RadGridView dataGrid, int fontSize = 6, DocumentLayoutMode layoutMode = DocumentLayoutMode.Paged,
            PageOrientation orientation = PageOrientation.Portrait, List<int> skipColumnDisplayIndex = null, Stream stream = null
            , Func<int, int, object, object, object> cellValueOverwrite = null, Func<int, int, string> columnAggregateOverWrite = null)
        {
            try
            {
                if (stream == null)
                {
                    SaveFileDialog dialog = new SaveFileDialog();
                    dialog.DefaultExt = "*.pdf";
                    dialog.Filter = "Adobe PDF Document (*.pdf)|*.pdf";

                    fontSizePDF = fontSize;

                    if (dialog.ShowDialog() == true)
                    {
                        RadDocument document = CreateDocument(dataGrid, skipColumnDisplayIndex, cellValueOverwrite, columnAggregateOverWrite);
                        document.LayoutMode = layoutMode;
                        document.SectionDefaultPageOrientation = orientation;
                        document.Measure(RadDocument.MAX_DOCUMENT_SIZE);
                        document.Arrange(new RectangleF(PointF.Empty, document.DesiredSize));
                        PdfFormatProvider provider = new PdfFormatProvider();
                        using (Stream output = dialog.OpenFile())
                        {
                            provider.Export(document, output);
                        }
                    }
                }
                else
                {
                    RadDocument document = CreateDocument(dataGrid, skipColumnDisplayIndex, cellValueOverwrite, columnAggregateOverWrite);
                    document.LayoutMode = layoutMode;
                    document.SectionDefaultPageOrientation = orientation;
                    document.Measure(RadDocument.MAX_DOCUMENT_SIZE);
                    document.Arrange(new RectangleF(PointF.Empty, document.DesiredSize));
                    PdfFormatProvider provider = new PdfFormatProvider();
                    provider.Export(document, stream);
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
            }
        }

        public static void ExportPDF_RadDocument(RadDocument document, int fontSize = 12)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.DefaultExt = "*.pdf";
            dialog.Filter = "Adobe PDF Document (*.pdf)|*.pdf";

            fontSizePDF = fontSize;

            if (dialog.ShowDialog() == true)
            {
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

        public static RadDocument ExportArray(RadGridView dataGrid, int fontSize = 12)
        {
            try
            {
                fontSizePDF = fontSize;
                RadDocument document = CreateDocument(dataGrid);
                return document;
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                return null;
            }
        }

        #endregion

        #region Print

        /// <summary>
        /// Printing the Grid
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="fontSize"></param>
        /// <returns></returns>
        public static RadDocument Print(RadGridView grid, int fontSize = 8)
        {
            try
            {
                fontSizePDF = fontSize;
                return CreateDocument(grid);
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                return null;
            }
        }

        /// <summary>
        /// Printing the Grid
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="fontSize"></param>
        /// <returns></returns>
        public static RadDocument PrintGrid(RadGridView grid, List<int> skipColumnDisplayIndex = null)
        {
            try
            {
                fontSizePDF = 8;
                return CreateDocument(grid, skipColumnDisplayIndex);
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                return null;
            }
        }
        #endregion

        #region Helper Methods

        /// <summary>
        /// Helper Function for PDF
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static RadDocument CreateDocument(RadGridView grid, List<int> skipColumnDisplayIndex = null
            , Func<int, int, object, object, object> cellValueOverwrite = null, Func<int, int, string> columnAggregateOverWrite = null)
        {
            if (skipColumnDisplayIndex == null)
            {
                skipColumnDisplayIndex = new List<int>();
            }

            List<GridViewBoundColumnBase> columns = (from c in grid.Columns.OfType<GridViewBoundColumnBase>()
                                                     orderby c.DisplayIndex
                                                     select c).ToList();
            columns = columns.Where(g => g.IsVisible == true && (!skipColumnDisplayIndex.Contains(g.DisplayIndex))).ToList();

            List<int> aggregateLog = new List<int>();
            foreach (var column in grid.Columns)
            {
                if (column is GridViewDataColumn)
                {
                    if ((column as GridViewDataColumn).AggregateFunctions.Count != 0)
                    {
                        aggregateLog.Add(column.DisplayIndex);
                    }
                }
                if (column is GridViewComboBoxColumn)
                {
                    if ((column as GridViewComboBoxColumn).AggregateFunctions.Count != 0)
                    {
                        aggregateLog.Add(column.DisplayIndex);
                    }
                }
            }
            List<int> visibleAggregateResultIndex = new List<int>();
            for (int i = 0; i < aggregateLog.Count; i++)
            {
                if (grid.Columns[aggregateLog[i]].IsVisible)
                {
                    visibleAggregateResultIndex.Add(i);
                }
            }

            Table table = new Table();
            RadDocument document = new RadDocument();
            Telerik.Windows.Documents.Model.Section section = new Telerik.Windows.Documents.Model.Section();
            section.Blocks.Add(table);
            document.Sections.Add(section);

            TableRow headerRow = new TableRow();
            for (int i = 0; i < columns.Count(); i++)
            {
                TableCell cell = new TableCell() { VerticalAlignment = RadVerticalAlignment.Center };
                cell.Background = Color.FromArgb(255, 228, 229, 229);
                AddCellValue(cell, columns[i].UniqueName);
                cell.PreferredWidth = new TableWidthUnit((float)columns[i].ActualWidth);
                headerRow.Cells.Add(cell);
            }

            table.Rows.Add(headerRow);

            if (grid.Items.Groups != null)
            {
                for (int i = 0; i < grid.Items.Groups.Count(); i++)
                {
                    AddGroupRow(table, grid.Items.Groups[i] as QueryableCollectionViewGroup, columns, grid, visibleAggregateResultIndex
                        , cellValueOverwrite);
                }
            }
            else
            {
                AddDataRows(table, grid.Items, columns, grid, cellValueOverwrite);
            }

            if (grid.ShowColumnFooters)
            {
                TableRow columnFooterRow = new TableRow();
                int aggregateIndex = -1;
                for (int i = 0; i < columns.Count(); i++)
                {
                    TableCell cell = new TableCell() { VerticalAlignment = RadVerticalAlignment.Center };
                    cell.Background = Color.FromArgb(255, 228, 229, 229);
                    String footerString = String.Empty;

                    if(columns[i].AggregateFunctions.Count == 1)
                    {
                        aggregateIndex++;
                    }

                    if (columns[i].Footer != null)
                    {
                        if(columns[i].Footer is TextBlock)
                        {
                            footerString = (columns[i].Footer as TextBlock).Text;
                        }
                        else if(columns[i].Footer is AggregateResultsList)
                        {
                            footerString = (columns[i].Footer as AggregateResultsList).ChildrenOfType<TextBlock>().First().Text;
                        }
                        else
                        {
                            footerString = columns[i].Footer.ToString();
                        }                        
                    }
                    else if (columns[i].AggregateFunctions.Count == 1)
                    {
                        if (columnAggregateOverWrite != null)
                        {
                            footerString = columnAggregateOverWrite(i, aggregateIndex);
                        }
                        else if (grid.AggregateResults[aggregateIndex].FormattedValue != null)
                        {
                            footerString = grid.AggregateResults[aggregateIndex].FormattedValue.ToString();
                        }
                    }
                    AddCellValue(cell, footerString);
                    cell.PreferredWidth = new TableWidthUnit((float)columns[i].ActualWidth);
                    columnFooterRow.Cells.Add(cell);
                }

                table.Rows.Add(columnFooterRow); 
            }
            return document;
        }

        /// <summary>
        /// Create Table
        /// </summary>
        /// <param name="grid">DataGrid</param>
        /// <param name="fontSize">FontSize</param>
        /// <param name="header">Header</param>
        /// <returns>Table</returns>
        public static Table CreateTable(RadGridView grid, int fontSize, GridViewLength width, string header = ""
            , Func<int, int, object, object, object> cellValueOverwrite = null)
        {
            List<GridViewBoundColumnBase> columns = (from c in grid.Columns.OfType<GridViewBoundColumnBase>()
                                                     orderby c.DisplayIndex
                                                     select c).ToList();

            List<int> aggregateLog = new List<int>();
            foreach (GridViewDataColumn column in grid.Columns)
            {
                if (column.AggregateFunctions.Count != 0)
                {
                    aggregateLog.Add(column.DisplayIndex);
                }
            }
            List<int> visibleAggregateResultIndex = new List<int>();
            for (int i = 0; i < aggregateLog.Count; i++)
            {
                if (grid.Columns[aggregateLog[i]].IsVisible)
                {
                    visibleAggregateResultIndex.Add(i);
                }
            }

            if (width != null)
            {
                foreach (GridViewBoundColumnBase item in columns)
                {
                    item.TextWrapping = TextWrapping.NoWrap;
                    item.Width = 200;
                }
            }
            grid.InvalidateMeasure();
            Table table = new Table();
            fontSizePDF = fontSize;

            if (!String.IsNullOrEmpty(header))
            {
                TableRow headerRow = new TableRow();
                TableCell cell = new TableCell();
                cell.Background = Color.FromArgb(255, 228, 229, 229);
                AddCellValue(cell, header);
                cell.ColumnSpan = columns.Count;
                headerRow.Cells.Add(cell);
                table.Rows.Add(headerRow);
            }

            //if (grid.ShowColumnHeaders)
            //{
            TableRow colHeaderRow = new TableRow();
            if (grid.GroupDescriptors.Count() > 0)
            {
                TableCell indentCell = new TableCell();
                indentCell.PreferredWidth = new TableWidthUnit(grid.GroupDescriptors.Count() * 30);
                indentCell.Background = Colors.Gray;
                colHeaderRow.Cells.Add(indentCell);
            }

            for (int i = 0; i < columns.Count(); i++)
            {
                TableCell cell = new TableCell();
                cell.Background = Color.FromArgb(255, 228, 229, 229);
                AddCellValue(cell, columns[i].UniqueName);
                cell.PreferredWidth = new TableWidthUnit((float)columns[i].ActualWidth);
                colHeaderRow.Cells.Add(cell);
            }

            table.Rows.Add(colHeaderRow);
            //}

            if (grid.Items.Groups != null)
            {
                for (int i = 0; i < grid.Items.Groups.Count(); i++)
                {
                    AddGroupRow(table, grid.Items.Groups[i] as QueryableCollectionViewGroup, columns, grid, visibleAggregateResultIndex
                        , cellValueOverwrite);
                }
            }
            else
            {
                AddDataRows(table, grid.Items, columns, grid, cellValueOverwrite);
            }

            return table;
        }

        /// <summary>
        /// Helper Method for PDF Export
        /// </summary>
        /// <param name="table"></param>
        /// <param name="items"></param>
        /// <param name="columns"></param>
        /// <param name="grid"></param>
        private static void AddDataRows(Table table, IList items, IList<GridViewBoundColumnBase> columns, RadGridView grid
            , Func<int, int, object, object, object> cellValueOverwrite)
        {
            for (int i = 0; i < items.Count; i++)
            {
                TableRow row = new TableRow();

                for (int j = 0; j < columns.Count(); j++)
                {
                    TableCell cell = new TableCell();
                    object value = cellValueOverwrite != null ? cellValueOverwrite(i, j, columns, items) : columns[j].GetValueForItem(items[i]);
                    AddCellValue(cell, value == null || value.ToString().Trim() == String.Empty ? "-" : value.ToString());
                    cell.PreferredWidth = new TableWidthUnit((float)columns[j].ActualWidth);
                    cell.Background = Colors.White;
                    row.Cells.Add(cell);
                }
                //table.Rows.Add(row);
                table.AddRow(row);
            }
        }

        /// <summary>
        /// Helper method for Export PDF
        /// </summary>
        /// <param name="table"></param>
        /// <param name="group"></param>
        /// <param name="columns"></param>
        /// <param name="grid"></param>
        private static void AddGroupRow(Table table, QueryableCollectionViewGroup group, IList<GridViewBoundColumnBase> columns
            , RadGridView grid, List<int> visibleAggregateIndex, Func<int, int, object, object, object> cellValueOverwrite)
        {
            TableRow row = new TableRow();

            int level = GetGroupLevel(group);

            TableCell groupHeaderCell = new TableCell();
            groupHeaderCell.TextAlignment = RadTextAlignment.Center;
            groupHeaderCell.VerticalAlignment = RadVerticalAlignment.Center;
            groupHeaderCell.Padding = new Padding(0, 5, 5, 0);
            groupHeaderCell.Background = level > 0 ? Color.FromArgb(255, 228, 229, 229) : Color.FromArgb(255, 198, 200, 200);
            groupHeaderCell.ColumnSpan = columns.Count();// +(grid.GroupDescriptors.Count() > 0 ? 1 : 0) - (level > 0 ? 1 : 0);
            AddCellValue(groupHeaderCell, group.Key != null ? group.Key.ToString() : string.Empty);
            row.Cells.Add(groupHeaderCell);

            table.Rows.Add(row);

            if (group.HasSubgroups)
            {
                for (int i = 0; i < group.Subgroups.Count(); i++)
                {
                    AddGroupRow(table, group.Subgroups[i] as QueryableCollectionViewGroup, columns, grid, visibleAggregateIndex, cellValueOverwrite);
                }
            }
            else
            {
                AddDataRows(table, group.Items, columns, grid, cellValueOverwrite);
            }

            TableRow aggregateRow = new TableRow();
            //TableCell groupIndicatorCell = new TableCell() { Background = Color.FromArgb(255, 228, 229, 229) };
            //aggregateRow.Cells.Add(groupIndicatorCell);

            int j = 0;
            for (int i = 0; i < columns.Count; i++)
            {
                if (columns[i].AggregateFunctions.Count.Equals(0))
                {
                    TableCell aggregatesCell = new TableCell() { Background = Color.FromArgb(255, 228, 229, 229) };
                    aggregateRow.Cells.Add(aggregatesCell);
                }
                else
                {
                    TableCell aggregatesCell = new TableCell() { VerticalAlignment = RadVerticalAlignment.Center };
                    aggregatesCell.Background = Color.FromArgb(255, 228, 229, 229);
                    AddCellValue(aggregatesCell, group.AggregateResults[j].FormattedValue != null
                        ? group.AggregateResults[j].FormattedValue.ToString() : string.Empty);
                    aggregateRow.Cells.Add(aggregatesCell);
                    j++;
                }
            }
            table.Rows.Add(aggregateRow);
        }

        /// <summary>
        /// Helper Method for PDF Export
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="value"></param>
        private static void AddCellValue(TableCell cell, string value)
        {
            Telerik.Windows.Documents.Model.Paragraph paragraph = new Telerik.Windows.Documents.Model.Paragraph();
            Telerik.Windows.Documents.Model.Span span = new Telerik.Windows.Documents.Model.Span();

            if (value != null && value != "")
            {
                span.Text = value;
                span.FontFamily = new System.Windows.Media.FontFamily("Arial");
                span.FontSize = fontSizePDF;
                paragraph.Inlines.Add(span);
            }
            cell.Blocks.Add(paragraph);
        }

        /// <summary>
        /// Helper Method for PDF Export
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        private static int GetGroupLevel(IGroup group)
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

        /// <summary>
        /// Converts the UI element into Table
        /// </summary>
        /// <param name="element">UI element</param>
        /// <returns>Table</returns>
        public static Table GenerateTable(UIElement element, String header = "")
        {
            Table contentTbl = new Table();
            TableRow contentRow = new TableRow();
            TableCell contentCell = new TableCell();
            WriteableBitmap wb = new WriteableBitmap(element, null);
            ImageInline image = new ImageInline(wb);
            Telerik.Windows.Documents.Model.Paragraph p = new Telerik.Windows.Documents.Model.Paragraph();
            p.Inlines.Add(image);
            contentCell.Blocks.Add(p);
            if (!String.IsNullOrEmpty(header))
            {
                TableRow headerRow = new TableRow();
                TableCell cell = new TableCell();
                cell.Background = Color.FromArgb(255, 228, 229, 229);
                AddCellValue(cell, header);
                cell.PreferredWidth = new TableWidthUnit((float)element.RenderSize.Width);
                headerRow.Cells.Add(cell);
                contentTbl.Rows.Add(headerRow);
            }

            contentRow.Cells.Add(contentCell);
            contentTbl.Rows.Add(contentRow);
            return contentTbl;
        }

        #endregion

        #endregion

        #region Chart

        #region PrintChart
        /// <summary>
        /// Event handler when user wants to Export the Grid to PDF
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void btnExportChartPDF_Click(RadChart chart, Stream stream, DocumentLayoutMode layoutMode = DocumentLayoutMode.Paged,
            PageOrientation orientation = PageOrientation.Landscape)
        {
            try
            {
                RadDocument document = PrintChart(chart);
                document.LayoutMode = layoutMode;
                document.SectionDefaultPageOrientation = orientation;
                document.Measure(RadDocument.MAX_DOCUMENT_SIZE);
                document.Arrange(new RectangleF(PointF.Empty, document.DesiredSize));
                PdfFormatProvider provider = new PdfFormatProvider();
                provider.Export(document, stream);
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
            }
        }

        /// <summary>
        /// Method to Print Chart
        /// </summary>
        /// <param name="chart"></param>
        public static RadDocument PrintChart(RadChart chart)
        {
            try
            {
                RadDocument document = new RadDocument();
                document = CreateChartDocumentPart(chart);
                return document;
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                return null;
            }
        }

        /// <summary>
        /// Helper Method for printing Chart
        /// </summary>
        /// <param name="chart">The Chart to be Printed</param>
        private static RadDocument CreateChartDocumentPart(RadChart chart)
        {
            try
            {
                RadDocument document = new RadDocument();
                Telerik.Windows.Documents.Model.Section section = new Telerik.Windows.Documents.Model.Section();
                Telerik.Windows.Documents.Model.Paragraph paragraph = new Telerik.Windows.Documents.Model.Paragraph();
                BitmapImage bi = new BitmapImage();

                using (MemoryStream ms = new MemoryStream())
                {
                    chart.ExportToImage(ms, new PngBitmapEncoder());
                    bi.SetSource(ms);
                }

                double imageWidth = chart.ActualWidth;
                double imageHeight = chart.ActualHeight;

                if (imageWidth > 625)
                {
                    imageWidth = 625;
                    imageHeight = chart.ActualHeight * imageWidth / chart.ActualWidth;
                }

                ImageInline image = new ImageInline(new WriteableBitmap(bi)) { Width = imageWidth, Height = imageHeight };
                paragraph.Inlines.Add(image);
                section.Blocks.Add(paragraph);
                document.Sections.Add(section);
                return document;
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                return null;
            }
        }

        #endregion

        #endregion

    }
}
