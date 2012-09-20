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
		}

		private ObservableCollection<Object> PinnedItems = new ObservableCollection<object>();

		private RadGridView frozenRowsContainer;

		void AssociatedObject_LayoutUpdated(object sender, EventArgs e)
		{
            if (AssociatedObject.Items.Count == 0)
            {
                return;
            }

            GridViewHeaderRow headerRow = AssociatedObject.ChildrenOfType<GridViewHeaderRow>().FirstOrDefault();
            if (headerRow == null)
            {
                return;
            }

            this.AssociatedObject.LayoutUpdated -= new EventHandler(AssociatedObject_LayoutUpdated);            
            this.frozenRowsContainer = new RadGridView();            
            SelectiveScrollingGrid grid = headerRow.ChildrenOfType<SelectiveScrollingGrid>().FirstOrDefault();
            grid.RowDefinitions.Add(new RowDefinition());
            grid.RowDefinitions.Add(new RowDefinition());
            frozenRowsContainer.SetValue(Grid.RowProperty, 1);
            frozenRowsContainer.SetValue(Grid.ColumnSpanProperty, 4);
            frozenRowsContainer.SetValue(Grid.RowProperty, 1);
            frozenRowsContainer.ShowGroupPanel = false;
            frozenRowsContainer.ShowColumnHeaders = false;
            frozenRowsContainer.SetValue(SelectiveScrollingGrid.SelectiveScrollingOrientationProperty, SelectiveScrollingOrientation.Vertical);


            frozenRowsContainer.ItemsSource = this.PinnedItems;

            frozenRowsContainer.SetValue(ScrollViewer.HorizontalScrollBarVisibilityProperty, ScrollBarVisibility.Hidden);

            frozenRowsContainer.HorizontalAlignment = HorizontalAlignment.Left;
            grid.Children.Add(frozenRowsContainer);
            
            var scrollViewer = this.AssociatedObject.ChildrenOfType<GridViewScrollViewer>().FirstOrDefault();
            scrollViewer.ScrollChanged += new ScrollChangedEventHandler(scrollViewer_ScrollChanged);
            this.frozenRowsContainer.SetBinding(FrameworkElement.WidthProperty, new Binding("ActualWidth") { Source = headerRow });
            this.frozenRowsContainer.Margin = new Thickness(-1, 0, 0, 0);
            this.frozenRowsContainer.SetBinding(RadGridView.FrozenColumnCountProperty, new Binding("FrozenColumnCount") { Source = this.AssociatedObject, Mode = BindingMode.TwoWay });

            if (AssociatedObject.Items.Count > 0)
            {                
                this.PinnedItems.Add(AssociatedObject.Items[0]);
                this.PinnedItems.Add(AssociatedObject.Items[1]);
                this.PinnedItems.Add(AssociatedObject.Items[2]);
                this.PinnedItems.Add(AssociatedObject.Items[3]);
                AssociatedObject.Items.RemoveAt(0);
                AssociatedObject.Items.RemoveAt(1);
                AssociatedObject.Items.RemoveAt(2);
                AssociatedObject.Items.RemoveAt(3);
                SyncColumnWidths();
            }
		}

		private void SyncColumnWidths()
		{
			foreach (GridViewColumn c in this.AssociatedObject.Columns)
			{
				this.frozenRowsContainer.Columns[this.AssociatedObject.Columns.IndexOf(c)].Width = c.ActualWidth;
			}
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
