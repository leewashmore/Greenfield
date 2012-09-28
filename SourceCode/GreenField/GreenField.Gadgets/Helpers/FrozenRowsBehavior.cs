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
using System.Windows.Interactivity;
using Telerik.Windows.Controls;
using Telerik.Windows.Controls.GridView;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Data;
using System.Collections.ObjectModel;

namespace GreenField.Gadgets.Helpers
{
    public class FrozenRowsBehavior : Behavior<RadGridView>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.LayoutUpdated += new EventHandler(AssociatedObject_LayoutUpdated);
            this.AssociatedObject.ColumnWidthChanged += new EventHandler<ColumnWidthChangedEventArgs>(AssociatedObject_ColumnWidthChanged);

        }

        void AssociatedObject_DataLoaded(object sender, EventArgs e)
        {
            SyncColumnWidths();
        }

        void AssociatedObject_ColumnWidthChanged(object sender, ColumnWidthChangedEventArgs e)
        {
            SyncColumnWidths();
        }

        private ObservableCollection<Object> PinnedItems = new ObservableCollection<object>();

        private RadGridView frozenRowsContainer;

        void AssociatedObject_LayoutUpdated(object sender, EventArgs e)
        {
            if (AssociatedObject == null || AssociatedObject.Items.Count == 0)
                return;

            GridViewHeaderRow headerRow = AssociatedObject.ChildrenOfType<GridViewHeaderRow>().FirstOrDefault();
            if (headerRow == null)
                return;

            this.AssociatedObject.LayoutUpdated -= new EventHandler(AssociatedObject_LayoutUpdated);
            this.frozenRowsContainer = new RadGridView();
            GenerateHeader();
            SelectiveScrollingGrid grid = headerRow.ChildrenOfType<SelectiveScrollingGrid>().FirstOrDefault();
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());
            frozenRowsContainer.SetValue(Grid.RowProperty, 1);
            frozenRowsContainer.SetValue(Grid.ColumnSpanProperty, 4);
            frozenRowsContainer.SetValue(Grid.RowProperty, 1);
            frozenRowsContainer.ShowGroupPanel = false;
            frozenRowsContainer.ShowColumnHeaders = false;
            frozenRowsContainer.RowIndicatorVisibility = AssociatedObject.RowIndicatorVisibility;
            frozenRowsContainer.SetValue(SelectiveScrollingGrid.SelectiveScrollingOrientationProperty, SelectiveScrollingOrientation.Vertical);


            frozenRowsContainer.ItemsSource = this.PinnedItems;

            frozenRowsContainer.SetValue(ScrollViewer.HorizontalScrollBarVisibilityProperty, ScrollBarVisibility.Hidden);

            frozenRowsContainer.HorizontalAlignment = HorizontalAlignment.Left;


            grid.Children.Add(frozenRowsContainer);

            var scrollViewer = this.AssociatedObject.ChildrenOfType<GridViewScrollViewer>().FirstOrDefault();
            scrollViewer.ScrollChanged += new ScrollChangedEventHandler(scrollViewer_ScrollChanged);
            this.frozenRowsContainer.SetBinding(FrameworkElement.WidthProperty, new Binding("ActualWidth") { Source = headerRow });
            this.frozenRowsContainer.Margin = new Thickness(-1, 0, 0, 0);
            this.frozenRowsContainer.SetBinding(RadGridView.FrozenColumnCountProperty,
                new Binding("FrozenColumnCount") { Source = this.AssociatedObject, Mode = BindingMode.TwoWay });

            this.PinItem(AssociatedObject.Items[0]);
            this.PinItem(AssociatedObject.Items[1]);
            this.PinItem(AssociatedObject.Items[2]);
            this.PinItem(AssociatedObject.Items[3]);

            SyncColumnWidths();
        }

        private void GenerateHeader()
        {
            this.frozenRowsContainer.Columns.Clear();
            foreach (GridViewDataColumn c in this.AssociatedObject.Columns)
            {
                GridViewDataColumn col = new GridViewDataColumn();
                col.Width = c.ActualWidth;
                col.Header = c.Header;
                col.UniqueName = c.UniqueName;
                col.DataMemberBinding = c.DataMemberBinding;
                this.frozenRowsContainer.Columns.Add(col);
            }
            this.frozenRowsContainer.Columns["Market Capitalization"].IsVisible = false;
        }

        private void SyncColumnWidths()
        {
            foreach (GridViewColumn c in this.AssociatedObject.Columns)
            {
                this.frozenRowsContainer.Columns[this.AssociatedObject.Columns.IndexOf(c)].Width = c.ActualWidth;
            }
        }

        private void UnpinItem(object item)
        {
            this.PinnedItems.Remove(item);
        }

        private void PinItem(object item)
        {
            if (this.PinnedItems.Contains(item))
                return;

            this.PinnedItems.Add(item);
            SyncColumnWidths();
        }

        void scrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (e.HorizontalChange == 0)
                return;

            this.frozenRowsContainer.ChildrenOfType<GridViewScrollViewer>().FirstOrDefault().ScrollToHorizontalOffset(e.HorizontalOffset);
        }

        void AssociatedObject_ColumnWidthChanging(object sender, ColumnWidthChangingEventArgs e)
        {
            if (frozenRowsContainer.Columns.Count != this.AssociatedObject.Columns.Count)
                return;

            for (int columnIndex = 0; columnIndex < this.AssociatedObject.Columns.Count; columnIndex++)
            {
                this.frozenRowsContainer.Columns[columnIndex].Width = this.AssociatedObject.Columns[columnIndex].ActualWidth;
            }
        }
    }	
}
