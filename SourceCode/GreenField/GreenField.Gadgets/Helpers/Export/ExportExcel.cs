using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Telerik.Windows.Documents.Model;
using Telerik.Windows.Controls;
using Telerik.Windows.Documents.FormatProviders.Pdf;
using System.Collections.Generic;
using System.Linq;

namespace GreenField.Gadgets.Helpers.Export
{
    public static class ExportExcel
    {
        private static void Export_Click(RadGridView dgGeneric)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.DefaultExt = "*.pdf";
            dialog.Filter = "Adobe PDF Document (*.pdf)|*.pdf";

            if (dialog.ShowDialog() == true)
            {
                RadDocument document = CreateDocument(dgGeneric);

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
            Section section = new Section();
            section.Blocks.Add(table);
            document.Sections.Add(section);

            if (grid.ShowColumnHeaders)
            {
                TableRow headerRow = new TableRow();

                if (grid.GroupDescriptors.Count() > 0)
                {
                    TableCell indentCell = new TableCell();
                    indentCell.PreferredWidth = new TableWidthUnit(grid.GroupDescriptors.Count() * 20);
                    indentCell.Background = HeaderBackgroundPicker.SelectedColor;
                    headerRow.Cells.Add(indentCell);
                }

                for (int i = 0; i < columns.Count(); i++)
                {
                    TableCell cell = new TableCell();
                    cell.Background = HeaderBackgroundPicker.SelectedColor;
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
                    indentCell.Background = RowBackgroundPicker.SelectedColor;
                    row.Cells.Add(indentCell);
                }

                for (int j = 0; j < columns.Count(); j++)
                {
                    TableCell cell = new TableCell();

                    object value = columns[j].GetValueForItem(items[i]);

                    AddCellValue(cell, value != null ? value.ToString() : string.Empty);

                    cell.PreferredWidth = new TableWidthUnit((float)columns[j].ActualWidth);
                    cell.Background = RowBackgroundPicker.SelectedColor;

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
                cell.Background = GroupHeaderBackgroundPicker.SelectedColor;
                row.Cells.Add(cell);
            }

            TableCell aggregatesCell = new TableCell();
            aggregatesCell.Background = GroupHeaderBackgroundPicker.SelectedColor;
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
            Paragraph paragraph = new Paragraph();
            cell.Blocks.Add(paragraph);

            Span span = new Span();
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





    }
}
