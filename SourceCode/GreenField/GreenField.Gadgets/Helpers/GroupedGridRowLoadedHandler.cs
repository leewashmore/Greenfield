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
using Telerik.Windows.Controls.GridView;
using Telerik.Windows.Controls;

namespace GreenField.Gadgets.Helpers
{
    public static class GroupedGridRowLoadedHandler
    {
        public static void Implement(RowLoadedEventArgs e)
        {
            if (e.Row is GridViewRow)
            {
                var row = e.Row as GridViewRow;
                row.FontSize = 7;
                row.FontFamily = new FontFamily("Arial");
                row.FontWeight = FontWeights.Normal;
                row.Height = 15;

                if (row != null && row.IndentLevel > 1)
                {
                    foreach (var indent in row.ChildrenOfType<GridViewIndentCell>())
                    {
                        if (indent != null)
                        {
                            indent.Background = new SolidColorBrush(Colors.White);
                        }
                    }
                }

                var parentRow = row.ParentOfType<GridViewGroupRow>();
                if (parentRow != null)
                {
                    foreach (var indent in parentRow.ChildrenOfType<GridViewIndentCell>())
                    {
                        if (indent != null)
                        {
                            indent.Background = new SolidColorBrush(Colors.White);
                            indent.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 159, 29, 33));
                        }
                    }
                }

                var superParentRow = parentRow.ParentOfType<GridViewGroupRow>();
                if (superParentRow != null)
                {
                    foreach (var indent in superParentRow.ChildrenOfType<GridViewIndentCell>())
                    {
                        if (indent != null)
                        {
                            indent.Background = new SolidColorBrush(Colors.White);
                            indent.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 159, 29, 33));
                        }
                    }
                }
            }

            if (e.Row is GridViewHeaderRow)
            {
                var row = e.Row as GridViewHeaderRow;
                if (row != null && row.IndentLevel > 1)
                {
                    foreach (var indent in row.ChildrenOfType<GridViewHeaderIndentCell>())
                    {
                        if (indent != null)
                        {
                            //indent.Visibility = System.Windows.Visibility.Collapsed;
                            indent.Background = new SolidColorBrush(Color.FromArgb(255, 159, 29, 33));
                            indent.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 159, 29, 33));
                        }
                    }
                }
            }

            if (e.Row is GridViewGroupFooterRow)
            {
                var row = e.Row as GridViewGroupFooterRow;
                if (row != null)
                {
                    row.Foreground = new SolidColorBrush(Colors.White);
                    row.FontSize = 7;
                    row.FontFamily = new FontFamily("Arial");
                    row.FontWeight = FontWeights.Bold;
                }
            }
        }
    }
}
