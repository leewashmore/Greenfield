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
using Telerik.Windows.Data;
using Telerik.Windows.Controls.TreeView;
using Telerik.Windows.Controls.GridView;
using Telerik.Windows.Controls.DragDrop;
using System.Windows.Threading;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Globalization;

namespace GreenField.Benchmark.Helpers
{
    public class RowReorderBehavior : Behavior<RadGridView>
    {
        private const string DropPositionFeedbackElementName = "DragBetweenItemsFeedback";
        private TreeViewDragCue dragCue;
        private object currentDropItem;
        private DropPosition currentDropPosition;
        private ContentPresenter dropPositionFeedbackPresenter;
        private Grid dropPositionFeedbackPresenterHost;
        private GridViewScrollViewer scrollViewer;
        public event EventHandler<BeginningDragEventArgs> BeginningDrag;
        public event EventHandler<DragStartedEventArgs> DragStarted;
        public event EventHandler<ReorderingEventArgs> Reordering;
        public event EventHandler<ReorderedEventArgs> Reordered;
        public static readonly DependencyProperty DragCueStyleProperty =
           DependencyProperty.Register("DragCueStyle", typeof(Style), typeof(RowReorderBehavior), new PropertyMetadata(null, new PropertyChangedCallback(OnDragCueStylePropertyChanged)));
        public Style DragCueStyle
        {
            get
            {
                return (Style)GetValue(DragCueStyleProperty);
            }
            set
            {
                SetValue(DragCueStyleProperty, value);
            }
        }
        private static void OnDragCueStylePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RowReorderBehavior rowReorder = (RowReorderBehavior)sender;
            if (rowReorder.dragCue != null)
                rowReorder.dragCue.Style = rowReorder.DragCueStyle;
        }
        public DataTemplate DragCueActionContentTemplate
        {
            get
            {
                return (DataTemplate)GetValue(DragCueActionContentTemplateProperty);
            }
            set
            {
                SetValue(DragCueActionContentTemplateProperty, value);
            }
        }
        public static readonly DependencyProperty DragCueActionContentTemplateProperty =
            DependencyProperty.Register("DragCueActionContentTemplate", typeof(DataTemplate), typeof(RowReorderBehavior), new PropertyMetadata(null, new PropertyChangedCallback(OnDragCueActionContentTemplatePropertyChanged)));
        private static void OnDragCueActionContentTemplatePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RowReorderBehavior rowReorder = (RowReorderBehavior)sender;
            if (rowReorder.dragCue != null)
                rowReorder.dragCue.DragActionContentTemplate = rowReorder.DragCueActionContentTemplate;
        }
        public DataTemplate DragCueTooltipContentTemplate
        {
            get
            {
                return (DataTemplate)GetValue(DragCueTooltipContentTemplateProperty);
            }
            set
            {
                SetValue(DragCueTooltipContentTemplateProperty, value);
            }
        }
        public static readonly DependencyProperty DragCueTooltipContentTemplateProperty =
            DependencyProperty.Register("DragCueTooltipContentTemplate", typeof(DataTemplate), typeof(RowReorderBehavior), new PropertyMetadata(null, new PropertyChangedCallback(OnDragCueTooltipContentTemplatePropertyChanged)));
        private static void OnDragCueTooltipContentTemplatePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RowReorderBehavior rowReorder = (RowReorderBehavior)sender;
            if (rowReorder.dragCue != null)
                rowReorder.dragCue.DragTooltipContentTemplate = rowReorder.DragCueTooltipContentTemplate;
        }
        public DataTemplate DragCueItemTemplate
        {
            get
            {
                return (DataTemplate)GetValue(DragCueItemTemplateProperty);
            }
            set
            {
                SetValue(DragCueItemTemplateProperty, value);
            }
        }
        public static readonly DependencyProperty DragCueItemTemplateProperty =
            DependencyProperty.Register("DragCueItemTemplate", typeof(DataTemplate), typeof(RowReorderBehavior), new PropertyMetadata(null, new PropertyChangedCallback(DragCueItemTemplatePropertyChanged)));
        private static void DragCueItemTemplatePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RowReorderBehavior rowReorder = (RowReorderBehavior)sender;
            if (rowReorder.dragCue != null)
                rowReorder.dragCue.ItemTemplate = rowReorder.DragCueItemTemplate;
        }
        public bool PreserveSelectionOrderWhenReordering
        {
            get
            {
                return (bool)GetValue(PreserveSelectionOrderWhenReorderingProperty);
            }
            set
            {
                SetValue(PreserveSelectionOrderWhenReorderingProperty, value);
            }
        }
        public static readonly DependencyProperty PreserveSelectionOrderWhenReorderingProperty =
            DependencyProperty.Register("PreserveSelectionOrderWhenReordering", typeof(bool), typeof(RowReorderBehavior), new PropertyMetadata(false));
        public RowReorderBehavior()
        {
            this.dropPositionFeedbackPresenter = new ContentPresenter();
            this.dropPositionFeedbackPresenter.Name = DropPositionFeedbackElementName;
            this.dropPositionFeedbackPresenter.HorizontalAlignment = HorizontalAlignment.Left;
            this.dropPositionFeedbackPresenter.VerticalAlignment = VerticalAlignment.Top;
            this.dropPositionFeedbackPresenter.RenderTransformOrigin = new Point(0.5, 0.5);
        }
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.RowLoaded += new EventHandler<Telerik.Windows.Controls.GridView.RowLoadedEventArgs>(AssociatedObject_RowLoaded);
            this.AssociatedObject.DataLoaded += new EventHandler<EventArgs>(AssociatedObject_DataLoaded);
            this.AssociatedObject.SetValue(RadDragAndDropManager.AllowDropProperty, true);
            this.SubscribeToDragDropEvents();
        }
        void AssociatedObject_DataLoaded(object sender, EventArgs e)
        {
            this.AssociatedObject.DataLoaded -= new EventHandler<EventArgs>(AssociatedObject_DataLoaded);
            this.AttachDropPositionFeedback();
            this.scrollViewer = this.AssociatedObject.ChildrenOfType<GridViewScrollViewer>().FirstOrDefault();
            this.scrollViewer.ScrollChanged += new ScrollChangedEventHandler(scrollViewer_ScrollChanged);
            this.scrollViewerScrollTimer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromSeconds(0.02)
            };
            this.scrollViewerScrollTimer.Tick += new EventHandler(this.OnScrollViewerScrollTimerCompleted);
        }
        void scrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            this.HideDropPositionFeedbackPresenter();
        }
        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.RowLoaded -= new EventHandler<Telerik.Windows.Controls.GridView.RowLoadedEventArgs>(AssociatedObject_RowLoaded);
            this.AssociatedObject.SetValue(RadDragAndDropManager.AllowDropProperty, false);
            this.UnsubscribeFromDragDropEvents();
            this.DetachDropPositionFeedback();
        }
        private void DetachDropPositionFeedback()
        {
            if (this.IsDropPositionFeedbackAvailable())
                this.dropPositionFeedbackPresenterHost.Children.Remove(this.dropPositionFeedbackPresenter);
        }
        private void AttachDropPositionFeedback()
        {
            this.dropPositionFeedbackPresenterHost = this.AssociatedObject.ChildrenOfType<Grid>().FirstOrDefault();
            if (this.dropPositionFeedbackPresenterHost != null)
            {
                this.dropPositionFeedbackPresenter.Content = CreateDefaultDropPositionFeedback();
                this.dropPositionFeedbackPresenterHost.Children.Add(this.dropPositionFeedbackPresenter);
            }
            this.HideDropPositionFeedbackPresenter();
        }
        private static UIElement CreateDefaultDropPositionFeedback()
        {
            Grid grid = new Grid()
            {
                Height = 8,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                IsHitTestVisible = false,
                VerticalAlignment = VerticalAlignment.Stretch
            };
            grid.ColumnDefinitions.Add(new ColumnDefinition()
            {
                Width = new GridLength(8)
            });
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            Ellipse ellipse = new Ellipse()
            {
                Stroke = new SolidColorBrush(Colors.Orange),
                StrokeThickness = 2,
                Fill = new SolidColorBrush(Colors.Orange),
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                Width = 8,
                Height = 8
            };
            Rectangle rectangle = new Rectangle()
            {
                Fill = new SolidColorBrush(Colors.Orange),
                RadiusX = 2,
                RadiusY = 2,
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Height = 2
            };
            Grid.SetColumn(ellipse, 0);
            Grid.SetColumn(rectangle, 1);
            grid.Children.Add(ellipse);
            grid.Children.Add(rectangle);
            return grid;
        }
        //protected override void OnChanged()
        //{
        //    base.OnChanged();
        //}
        void AssociatedObject_RowLoaded(object sender, Telerik.Windows.Controls.GridView.RowLoadedEventArgs e)
        {
            if (e.Row is GridViewHeaderRow || e.Row is GridViewNewRow || e.Row is GridViewFooterRow)
                return;
            var row = e.Row as GridViewRow;
            this.InitializeRowDragAndDrop(row);
        }
        private void InitializeRowDragAndDrop(GridViewRow row)
        {
            if (row == null)
                return;
            row.SetValue(RadDragAndDropManager.AllowDragProperty, true);
            row.SetValue(RadDragAndDropManager.AllowDropProperty, true);
            RadDragAndDropManager.RemoveDropQueryHandler(row, OnGridViewRowDropQuery);
            RadDragAndDropManager.AddDropQueryHandler(row, OnGridViewRowDropQuery);
            RadDragAndDropManager.RemoveDropInfoHandler(row, OnGridViewRowDropInfo);
            RadDragAndDropManager.AddDropInfoHandler(row, OnGridViewRowDropInfo);
        }
        private void SubscribeToDragDropEvents()
        {
            RadDragAndDropManager.AddDropQueryHandler(this.AssociatedObject, OnDropQuery);
            RadDragAndDropManager.AddDropInfoHandler(this.AssociatedObject, OnDropInfo);
            RadDragAndDropManager.AddDragQueryHandler(this.AssociatedObject, OnDragQuery);
            RadDragAndDropManager.AddDragInfoHandler(this.AssociatedObject, OnDragInfo);
        }
        private void UnsubscribeFromDragDropEvents()
        {
            RadDragAndDropManager.RemoveDropQueryHandler(this.AssociatedObject, OnDropQuery);
            RadDragAndDropManager.RemoveDropInfoHandler(this.AssociatedObject, OnDropInfo);
            RadDragAndDropManager.RemoveDragQueryHandler(this.AssociatedObject, OnDragQuery);
            RadDragAndDropManager.RemoveDragInfoHandler(this.AssociatedObject, OnDragInfo);
        }
        public virtual DropPosition GetDropPositionFromPoint(Point absoluteMousePosition, GridViewRow row)
        {
            if (row != null)
            {
                var headerTopPoint = row.TransformToVisual(Application.Current.RootVisual).Transform(new Point());
                double mouseTop = absoluteMousePosition.Y - headerTopPoint.Y;
                if (mouseTop <= (row.ActualHeight * 2 / 4))
                {
                    return DropPosition.Before;
                }
                else if (mouseTop > (row.ActualHeight * 2 / 4))
                {
                    return DropPosition.After;
                }
            }
            return DropPosition.Inside;
        }
        private void TryReorderRows(ReorderingEventArgs args)
        {
            IList source = args.SourceGrid.ItemsSource as IList;
            if (args.Cancel || source == null || source.IsFixedSize)
                return;
            var newDropIndex = this.RemoveDraggedItems(source, args.DraggedItems, args.DropIndex);
            this.AddDraggedItems(source, args.DraggedItems, newDropIndex);
        }
        private int RemoveDraggedItems(IList sourceList, IEnumerable<object> draggedItems, int currentDropIndex)
        {
            var newDropIndex = UpdateDropIndex(draggedItems, sourceList, currentDropIndex);
            foreach (var itemToRemove in draggedItems)
                sourceList.Remove(itemToRemove);
            return newDropIndex;
        }
        private static int UpdateDropIndex(IEnumerable<object> draggedItems, IList sourceList, int currentDropIndex)
        {
            int newDropIndex = currentDropIndex;
            var itemsBeforeDropIndex =
                from object item in draggedItems
                where sourceList.IndexOf(item) < currentDropIndex
                select item;
            foreach (var item in itemsBeforeDropIndex)
            {
                if (newDropIndex > 0)
                    newDropIndex--;
            }
            return newDropIndex;
        }
        private void AddDraggedItems(IList sourceList, IEnumerable<object> draggedItems, int currentDropIndex)
        {
            for (int i = 0; i < draggedItems.Count(); i++)
            {
                sourceList.Insert(currentDropIndex + i, draggedItems.Skip(i).FirstOrDefault());
            }
        }
        protected void OnReordering(ReorderingEventArgs args)
        {
            if (this.Reordering != null)
                this.Reordering(this, args);
        }
        protected void OnReordered(ReorderedEventArgs args)
        {
            if (this.Reordered != null)
                this.Reordered(this, args);
        }
        private void OnDropQuery(object sender, DragDropQueryEventArgs e)
        {
            if (!(e.Options.Source is GridViewRow))
            {
                return;
            }

            e.QueryResult = true;
            e.Handled = true;
            // CHECK IF THE SOURCE COLLECTION CAN BE MODIFIED
        }
        private void OnDropInfo(object sender, DragDropEventArgs e)
        {
            if (!(e.Options.Source is GridViewRow))
            {
                return;
            }
            var gridView = e.Options.Destination as RadGridView;
            IEnumerable<object> draggedItems = e.Options.Payload as IEnumerable<object>;
            TreeViewDragCue cue = e.Options.DragCue as TreeViewDragCue;
            this.HideDropPositionFeedbackPresenter();
            if (e.Options.Status == DragStatus.DropPossible)
            {
                this.UpdateDragCueContent(cue, gridView.Items[gridView.Items.Count - 1], "Move after: ");
                cue.IsDropPossible = true;
            }
            else if (e.Options.Status == DragStatus.DropImpossible)
            {
                cue.DragActionContent = null;
                cue.IsDropPossible = false;
            }
            else if (e.Options.Status == DragStatus.DropComplete)
            {
                ReorderingEventArgs reorderingArgs = new ReorderingEventArgs(gridView, draggedItems, gridView.Items.Count);
                this.OnReordering(reorderingArgs);
                this.TryReorderRows(reorderingArgs);
                this.OnReordered(new ReorderedEventArgs(gridView, draggedItems));
            }
            e.Handled = true;
        }
        DispatcherTimer scrollViewerScrollTimer = new DispatcherTimer()
        {
            Interval = TimeSpan.FromSeconds(0.02)
        };
        Point previousScrollAdjustPosition;
        //   scrollViewerScrollTimer.Tick += new EventHandler(OnScrollViewerScrollTimerCompleted);

        private static bool ArePointsNear(Point currentPoint, Point mouseClickPoint, double threshold)
        {
            return Math.Abs(currentPoint.X - mouseClickPoint.X) < threshold
                && Math.Abs(currentPoint.Y - mouseClickPoint.Y) < threshold;
        }
        private void OnScrollViewerScrollTimerCompleted(object sender, EventArgs e)
        {
            if (RadDragAndDropManager.IsDragging
                && ArePointsNear(previousScrollAdjustPosition, RadDragAndDropManager.Options.CurrentDragPoint, 5)
                && this.scrollViewer != null)
            {
                //this.HideDropPositionFeedbackPresenter();
                AdjustScrollViewer(previousScrollAdjustPosition);
            }
            else
            {
                scrollViewerScrollTimer.Stop();
            }
        }
        private void OnDragQuery(object sender, DragDropQueryEventArgs e)
        {
            if (!(e.Options.Source is GridViewRow))
            {
                return;
            }

            var gridView = sender as RadGridView;
            if (gridView == null)
                return;
            this.AssociatedObject.ReleaseMouseCapture();
            this.previousScrollAdjustPosition = e.Options.CurrentDragPoint;
            if (this.IsInScrollableArea(this.previousScrollAdjustPosition))
            {
                this.scrollViewerScrollTimer.Start();
            }
            e.QueryResult = this.InitiateDrag(gridView, e);
            e.Handled = true;
        }
        private void OnDragInfo(object sender, DragDropEventArgs e)
        {
            if (!(e.Options.Source is GridViewRow))
            {
                return;
            }
            var gridView = sender as RadGridView;
            var draggedItems = e.Options.Payload as IEnumerable<object>;
            this.AssociatedObject.ReleaseMouseCapture();
            if (e.Options.Status == DragStatus.DragInProgress)
            {
                this.dragCue = this.CreateDragCue();
                this.dragCue.ItemsSource = draggedItems;
                e.Options.DragCue = this.dragCue;
            }
            else if (e.Options.Status == DragStatus.DragCancel)
            {
                //treeView.CancelDrag();
            }
            else if (e.Options.Status == DragStatus.DragComplete)
            {
            }
            else if (e.Options.Status == DragStatus.DropImpossible)
            {
                // HIDE DROP INDICATOR
            }
            e.Handled = true;
        }
        private bool IsInScrollableArea(Point currentPoint)
        {
            if (this.scrollViewer == null)
                return false;
            var size = this.scrollViewer.RenderSize;
            var stuff = Application.Current.RootVisual.TransformToVisual(this.scrollViewer).Transform(currentPoint);
            return
                stuff.Y <= 20 ||
                stuff.Y >= size.Height - 20;
        }
        private void AdjustScrollViewer(Point currentPoint)
        {
            if (this.scrollViewer == null)
                return;
            var p = currentPoint;
            var visual = this.scrollViewer.TransformToVisual(Application.Current.RootVisual);
            var topLeft = visual.Transform(new Point(0, 0));
            var relative = new Point(p.X - topLeft.X, p.Y - topLeft.Y);
            if (relative.Y > 0 && relative.Y < 40)
            {
                this.scrollViewer.ScrollToVerticalOffset(this.scrollViewer.VerticalOffset - (20 * ((40 - relative.Y) / 40)));
            }
            if (relative.Y > this.scrollViewer.ActualHeight - 40 && relative.Y < this.scrollViewer.ActualHeight)
            {
                this.scrollViewer.ScrollToVerticalOffset(this.scrollViewer.VerticalOffset + (20 * ((40 - (this.scrollViewer.ActualHeight - relative.Y)) / 40)));
            }
            if (relative.X > 0 && relative.X < 40)
            {
                this.scrollViewer.ScrollToHorizontalOffset(this.scrollViewer.HorizontalOffset - (20 * ((40 - relative.X) / 40)));
            }
            if (relative.X > this.scrollViewer.ActualWidth - 40 && relative.X < this.scrollViewer.ActualWidth)
            {
                this.scrollViewer.ScrollToHorizontalOffset(this.scrollViewer.HorizontalOffset + (20 * ((40 - (this.scrollViewer.ActualWidth - relative.X)) / 40)));
            }
        }
        private void OnGridViewRowDropQuery(object sender, DragDropQueryEventArgs e)
        {
            this.AssociatedObject.ReleaseMouseCapture();
            var row = sender as GridViewRow;
            var draggedItems = e.Options.Payload as IEnumerable<object>;
            TreeViewDragCue cue = e.Options.DragCue as TreeViewDragCue;
            if (e.Options.Status == DragStatus.DropDestinationQuery)
            {
                this.currentDropItem = row.Item;
                this.currentDropPosition = this.GetDropPositionFromPoint(e.Options.CurrentDragPoint, row);
                e.QueryResult = !draggedItems.Contains(this.currentDropItem);
                if (!e.QueryResult.Value)
                {
                    this.ClearDragCueContent(cue);
                }
            }
            e.Handled = true;
        }
        private void OnGridViewRowDropInfo(object sender, DragDropEventArgs e)
        {
            this.AssociatedObject.ReleaseMouseCapture();
            var senderRow = (GridViewRow)sender;
            var gridView = senderRow.GridViewDataControl;
            IEnumerable<object> draggedItems = e.Options.Payload as IEnumerable<object>;
            TreeViewDragCue cue = e.Options.DragCue as TreeViewDragCue;
            if (e.Options.Status == DragStatus.DropPossible)
            {
                var currentRow = this.AssociatedObject.GetContainerFromDataItem((this.currentDropItem)) as GridViewRow;
                //if(currentRow == null)
                // cancel drag
                this.UpdateDragCueContent(cue,
                    currentDropItem,
                    String.Format("Move {0}: ", Enum.GetName(typeof(DropPosition), this.currentDropPosition)).ToLower(CultureInfo.InvariantCulture));
                this.ShowDropPositionFeedbackPresenter(gridView, currentRow, this.currentDropPosition);
                cue.IsDropPossible = true;
            }
            else if (e.Options.Status == DragStatus.DropImpossible)
            {
                this.HideDropPositionFeedbackPresenter();
                this.ClearDragCueContent(cue);
                cue.IsDropPossible = false;
            }
            else if (e.Options.Status == DragStatus.DropComplete)
            {
                this.HideDropPositionFeedbackPresenter();
                var dropIndex = gridView.Items.IndexOf(this.currentDropItem);
                if (currentDropPosition == DropPosition.After)
                    dropIndex++;
                ReorderingEventArgs reorderingArgs = new ReorderingEventArgs(gridView, draggedItems, dropIndex);
                this.OnReordering(reorderingArgs);
                this.TryReorderRows(reorderingArgs);
                this.OnReordered(new ReorderedEventArgs(gridView, draggedItems));
            }
            e.Handled = true;
        }
        private bool IsDropPositionFeedbackAvailable()
        {
            return
                this.dropPositionFeedbackPresenterHost != null &&
                this.dropPositionFeedbackPresenter != null;
        }
        private void ShowDropPositionFeedbackPresenter(GridViewDataControl gridView, GridViewRow row, DropPosition lastRowDropPosition)
        {
            if (!this.IsDropPositionFeedbackAvailable())
                return;
            var yOffset = this.GetDropPositionFeedbackOffset(row, lastRowDropPosition);
            this.dropPositionFeedbackPresenter.Visibility = Visibility.Visible;
            this.dropPositionFeedbackPresenter.Width = row.ActualWidth;
            this.dropPositionFeedbackPresenter.RenderTransform = new TranslateTransform()
            {
                Y = yOffset
            };
        }
        private void HideDropPositionFeedbackPresenter()
        {
            this.dropPositionFeedbackPresenter.RenderTransform = new TranslateTransform()
            {
                X = 0,
                Y = -234324
            };
        }
        private double GetDropPositionFeedbackOffset(GridViewRow row, DropPosition dropPosition)
        {
            var yOffset = row.TransformToVisual(this.dropPositionFeedbackPresenterHost).Transform(new Point(0, 0)).Y;
            if (dropPosition == DropPosition.After)
                yOffset += row.ActualHeight;
            yOffset -= (this.dropPositionFeedbackPresenter.ActualHeight / 2.0);
            return yOffset;
        }
        private bool InitiateDrag(RadGridView gridView, DragDropQueryEventArgs dragDropArgs)
        {
            var itemsToDrag = this.GetItemsToReorder(gridView);
            var eventArgs = new BeginningDragEventArgs(gridView, itemsToDrag);
            this.OnBeginningDrag(eventArgs);
            if (!eventArgs.Cancel)
            {
                dragDropArgs.Options.Payload = itemsToDrag;
                this.OnDragStarted(new DragStartedEventArgs(gridView, itemsToDrag));
            }
            return !eventArgs.Cancel;
        }
        private IEnumerable<Object> GetItemsToReorder(RadGridView gridView)
        {
            if (this.PreserveSelectionOrderWhenReordering)
                return gridView.SelectedItems.ToList();
            else
            {
                List<object> itemsToReorder = new List<object>();
                for (int i = 0; i < gridView.Items.Count; i++)
                {
                    if (gridView.SelectedItems.Contains(gridView.Items[i]))
                        itemsToReorder.Add(gridView.Items[i]);
                }
                return itemsToReorder;
            }
        }
        protected virtual void OnBeginningDrag(BeginningDragEventArgs args)
        {
            if (this.BeginningDrag != null)
                this.BeginningDrag(this, args);
        }
        protected virtual void OnDragStarted(DragStartedEventArgs args)
        {
            if (this.DragStarted != null)
                this.DragStarted(this, args);
        }
        private TreeViewDragCue CreateDragCue()
        {
            var cue = new TreeViewDragCue();
            cue.Style = this.DragCueStyle;
            cue.DragTooltipContentTemplate = this.DragCueTooltipContentTemplate;
            cue.DragActionContentTemplate = this.DragCueActionContentTemplate;
            cue.ItemTemplate = this.DragCueItemTemplate;
            return cue;
        }
        private void UpdateDragCueContent(TreeViewDragCue cue, object tooltipContent, object actionContent)
        {
            cue.Style = this.DragCueStyle;
            cue.DragTooltipContentTemplate = this.DragCueTooltipContentTemplate;
            cue.DragActionContentTemplate = this.DragCueActionContentTemplate;
            cue.ItemTemplate = this.DragCueItemTemplate;
            cue.DragTooltipContent = tooltipContent;
            cue.DragActionContent = actionContent;
        }
        private void ClearDragCueContent(TreeViewDragCue cue)
        {
            cue.DragTooltipContentTemplate = null;
            cue.DragActionContentTemplate = null;
            cue.DragTooltipContent = null;
            cue.DragActionContent = null;
        }
    }
}
