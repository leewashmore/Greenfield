using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using GreenField.Common.DragDockPanelControls;
using System.ComponentModel;

namespace GreenField.Common.DragGridPanelControls
{
    
    public class DragGridPanel : Canvas
    {
        #region Constants

        private const double _minimizedWidth = 250;

        #endregion

        #region Private Fields

        
        private readonly Grid LayoutGrid;
        private List<UIElement> _children = new List<UIElement>();
        private bool _isLoaded;
        private bool _isMaximized;
        private bool _isUpdated;
        private LayoutDefinition _restoredLayout;
        private int _restoredIndex;

        #endregion

        #region Layout Property
        
        public LayoutDefinition Layout
        {
            get { return (LayoutDefinition)GetValue(LayoutProperty); }
            set { SetValue(LayoutProperty, value); }
        }

        public static readonly DependencyProperty LayoutProperty =
            DependencyProperty.Register("Layout", typeof(LayoutDefinition), typeof(DragGridPanel),new PropertyMetadata(null, OnLayoutChanged));

        private static void OnLayoutChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            ((DragGridPanel)o).OnLayoutChanged((LayoutDefinition)e.NewValue, (LayoutDefinition)e.OldValue);
        }

        private void OnLayoutChanged(LayoutDefinition newValue, LayoutDefinition oldValue)
        {
            if (_isMaximized)
            {
                foreach (UIElement child in Children)
                {
                    if (child is DragDockPanel)
                        ((DragDockPanel)child).PanelState = PanelState.Restored;
                }
                _isMaximized = false;
            }

            OnCellsChanged(
                (newValue != null) ? newValue.Cells : null,
                (oldValue != null) ? oldValue.Cells : null);
            OnDefinitionsChanged(
                (newValue != null) ? newValue.Definitions : null,
                (oldValue != null) ? oldValue.Definitions : null);
        }

        #endregion

        #region Definitions Property
        
        public DependencyObjectCollection Definitions
        {
            get { return (Layout != null) ? Layout.Definitions : null; }
        }

        private void OnDefinitionsChanged(DependencyObjectCollection newValue, DependencyObjectCollection oldValue)
        {
            if (oldValue != null)
            {
                oldValue.CollectionChanged -= OnDefinitionsChanged;
                foreach (DependencyObject o in oldValue)
                {
                    RemoveDefinition(o);
                }
            }
            if (newValue != null)
            {
                newValue.CollectionChanged += OnDefinitionsChanged;
                foreach (DependencyObject o in newValue)
                {
                    AddDefinition(o);
                }
            }
            _isUpdated = false;
        }

        private void OnDefinitionsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (DependencyObject obj in e.NewItems)
                {
                    AddDefinition(obj);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (DependencyObject obj in e.OldItems)
                {
                    RemoveDefinition(obj);
                }
            }
            _isUpdated = false;
        }

        private void AddDefinition(DependencyObject obj)
        {
            if (obj is RowDefinition)
                LayoutGrid.RowDefinitions.Add((RowDefinition)obj);
            else if (obj is ColumnDefinition)
                LayoutGrid.ColumnDefinitions.Add((ColumnDefinition)obj);
            else throw new ArgumentException();
        }

        private void RemoveDefinition(DependencyObject obj)
        {
            if (obj is RowDefinition)
                LayoutGrid.RowDefinitions.Remove((RowDefinition)obj);
            else if (obj is ColumnDefinition)
                LayoutGrid.ColumnDefinitions.Remove((ColumnDefinition)obj);
        }

        #endregion

        #region Cells Property
        
        public CellCollection Cells
        {
            get { return (Layout != null) ? Layout.Cells : null; }
        }

        private void OnCellsChanged(CellCollection newValue, CellCollection oldValue)
        {
            if (oldValue != null)
            {
                oldValue.CollectionChanged -= OnCellsChanged;
                foreach (Cell cell in oldValue)
                {
                    RemoveCell(cell, -1);
                }
            }

            //Add new cells
            if (newValue != null)
            {
                newValue.CollectionChanged += OnCellsChanged;
                foreach (Cell cell in newValue)
                {
                    AddCell(cell, -1);
                }
            }
            _isUpdated = false;
        }

        private void OnCellsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                int i = 0;
                foreach (Cell cell in e.NewItems)
                {
                    AddCell(cell, e.NewStartingIndex + (i++));
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                int i = 0;
                foreach (Cell cell in e.OldItems)
                {
                    RemoveCell(cell, e.OldStartingIndex + (i++));
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Replace)
            {
                //TODO: Implement replace handler
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                foreach (Cell cell in Cells)
                {
                    RemoveCell(cell, -1);
                }
            }
            _isUpdated = false;
        }

        private void RemoveCell(Cell cell, int oldIndex)
        {
            LayoutGrid.Children.Remove(cell);
            if (oldIndex != -1)
            {
                UIElement element = Children.FirstOrDefault(c => GetIndex(c) == oldIndex);
                if (element != null)
                {
                    int index = FindIndex();
                    SetIndex(element, index);
                }
            }
        }

        private void AddCell(Cell cell, int newIndex)
        {
            int index = LayoutGrid.Children.Count;
            LayoutGrid.Children.Add(cell);
        
            UIElement element = Children.FirstOrDefault(c => GetIndex(c) == -1 && !LayoutGrid.Equals(c));
            if (element != null && newIndex != -1)
            {
                SetIndex(element, index);
                PreparePanel(element);
            }
        }

        #endregion

        #region Index Property

        public static readonly DependencyProperty IndexProperty = DependencyProperty.RegisterAttached("Index", typeof(int), typeof(DragGridPanel),new PropertyMetadata(-1, OnIndexChanged));

        public void SetIndex(UIElement element, int index)
        {
            element.SetValue(IndexProperty, index);
        }

        public int GetIndex(UIElement element)
        {
            return (int)element.GetValue(IndexProperty);
        }


        private static void OnIndexChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var panel = VisualTreeHelper.GetParent(o) as DragGridPanel;
            // If not already removed from the tree
            if (panel != null) panel.OnIndexChanged((UIElement)o, (int)e.NewValue, (int)e.OldValue);
        }

        private void OnIndexChanged(UIElement element, int newValue, int oldValue)
        {
            if (newValue > -1 && newValue < Cells.Count)
            {
                Cell cell = Cells[newValue];
                if (oldValue != -1)
                {
                    Animate(cell, element);
                    if (_isMaximized && element is DragDockPanel)
                    {
                        ((DragDockPanel)element).PanelState = newValue == 0 ? PanelState.Maximized : PanelState.Minimized;
                    }
                }
            }
        }

        private static void Animate(Cell cell, UIElement element)
        {
            var control = element as AnimatedHeaderedContentControl;//TBRemoved.......//DraggablePanel;
            if (control != null)
            {
                Rect rect = cell.Rectangle;
                //TBRemoved............
                //control.UpdateSize(rect.Width, rect.Height);
                //control.UpdatePosition(new Point(rect.Left, rect.Top));
                control.AnimateSize(rect.Width, rect.Height);
                control.AnimatePosition(rect.Left, rect.Top);
            }
            else
            {
                Rect p = cell.Rectangle;
                SetLeft(element, p.Left);
                SetTop(element, p.Top);
                element.Arrange(p);
            }
        }

        #endregion

        #region Constructor

        public DragGridPanel()
        {
            LayoutGrid = new Grid();
            Children.Add(LayoutGrid);

            Loaded += DragGridPanel_Loaded;
            SizeChanged += DragGridPanel_SizeChanged;
            LayoutUpdated += DragGridPanel_LayoutUpdated;
        }
        
        public DragGridPanel(LayoutDefinition layout) : this()
        {
            Layout = layout;
        }

        //TBRemoved.............
        //Size cellSize;
        private void DragGridPanel_Loaded(object sender, RoutedEventArgs e)
        {
            if (Layout == null)
                Layout = CreateUniformLayout();
         
            _isLoaded = true;
        }

        #endregion

        #region Layout and Resize

        private void DragGridPanel_LayoutUpdated(object sender, EventArgs e)
        {
            Update(RenderSize);
        }

        private void DragGridPanel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _isUpdated = false;
            Update(e.NewSize);
        }

        private void Update(Size size)
        {
            if (!_isLoaded || Children.Count == _children.Count && _isUpdated)
                return;

            EnsureGrid();
            LayoutGrid.Arrange(new Rect(new Point(), size));

            foreach (UIElement child in Children)
            {
                if (child == LayoutGrid)
                    continue;

                int index = GetIndex(child);
                if (index == -1)
                {
                    index = FindIndex();
                    SetIndex(child, index);
                    PreparePanel(child);
                }
                if (index != -1 && index<Cells.Count)
                {
                    if (!DesignerProperties.GetIsInDesignMode(this))
                        Animate(Cells[index], child);
                    else
                        Cells[index].ArrangeElement(child);
                }
                else
                    child.Arrange(new Rect());
            }
            foreach (UIElement element in _children)
            {
                if (!Children.Contains(element))
                {
                    UnbindPanel(element);
                    element.ClearValue(IndexProperty);
                }
            }
            _children = Children.ToList();
            _isUpdated = true;
        }

        #endregion

        #region Cell and definition creation

        private LayoutDefinition CreateUniformLayout()
        {
            return new LayoutDefinition
                   {
                       Definitions = CreateUniformGridDefinitions(),
                       Cells = CreateUniformCells()
                   };
        }

        private CellCollection CreateUniformCells()
        {
            int count = Children.Count - 1;
            int rows = (int)Math.Floor(Math.Sqrt(count));
            int columns = (int)Math.Ceiling(count / (double)rows);
            CellCollection cells = new CellCollection();
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    cells.Add(new Cell(i, j));
                }
            }
            return cells;
        }

        private DependencyObjectCollection CreateUniformGridDefinitions()
        {
            int count = Children.Count - 1;
            int rows = (int)Math.Floor(Math.Sqrt(count));
            int columns = (int)Math.Ceiling(count / (double)rows);
            DependencyObjectCollection definitions = new DependencyObjectCollection();
            for (int i = 0; i < rows; i++)
                definitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            for (int i = 0; i < columns; i++)
                definitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            return definitions;
        }

        private CellCollection CreateMaximizedCells()
        {
            CellCollection cells = new CellCollection();
            int count = Cells.Count;
            cells.Add(new Cell(0, 0, Math.Max(count - 1, 1), 1));
            for (int i = 0; i < count - 1; i++)
                cells.Add(new Cell(i, 1));
            return cells;
        }

        private DependencyObjectCollection CreateMaximizedDefinitions()
        {
            var defs = new DependencyObjectCollection();
            int count = Cells.Count;
            defs.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            if (count > 1)
                defs.Add(new ColumnDefinition { Width = new GridLength(_minimizedWidth, GridUnitType.Pixel) });
            for (int i = 0; i < count - 1; i++)
                defs.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            return defs;
        }

        #endregion

        #region Helper Methods

        private void EnsureGrid()
        {
            if (!Equals(VisualTreeHelper.GetParent(LayoutGrid)))
                Children.Add(LayoutGrid);
        }

        private void PreparePanel(UIElement element)
        {
            var panel = element as DragDockPanel;
            if (panel != null)
            {
                panel.DragFinished += panel_DragFinished;
                panel.Maximized += panel_Maximized;
                panel.Restored += panel_Restored;
                panel.Closed += panel_Closed;
                //SetPanelSize(panel);........TBRemoved
            }
        }

        /// <summary>
        /// Sets the events for persisting panels.
        /// </summary>
        /// <param name="persistedElement"></param>
        public void PreparePersistedPanel(UIElement persistedElement)
        {
            PreparePanel(persistedElement);
        }
        
        //........TBRemoved
        //Size _actualPanelSize,_reducedPanelSize;
        //private void SetPanelSize(DragDockPanel panel)
        //{
        //    if (!_isMaximized) _actualPanelSize = panel.RenderSize;
        //    Cell cellItem = Layout.Cells.First();//.FirstOrDefault(s => s.Height == 140);
        //    //panel.Measure(new Size(cellItem.ActualWidth, cellItem.ActualHeight));
        //    panel.Width = cellItem.ActualWidth;
        //    panel.Height = cellItem.ActualHeight;
        //    _reducedPanelSize = new Size(panel.Width, panel.Height);
        //}

        //private void RestorePanel(DragDockPanel panel)
        //{
        //    if (_isMaximized)
        //    {
        //        panel.Width = _reducedPanelSize.Width;
        //        panel.Height = _reducedPanelSize.Height;
        //    }
        //    else
        //    {
        //        panel.Width = _actualPanelSize.Width;
        //        panel.Height = _actualPanelSize.Height;
        //    }
        //}

        private void UnbindPanel(UIElement element)
        {
            var panel = element as DragDockPanel;
            if (panel != null)
            {
                panel.DragFinished -= panel_DragFinished;
                panel.Maximized -= panel_Maximized;
                panel.Restored -= panel_Restored;
                panel.Closed -= panel_Closed;
            }
        }

        private int FindIndex()
        {
            IEnumerable<int> free = Enumerable.Range(0, Cells.Count).Except(Children.Select(c => GetIndex(c)));

            if (free.Any())
                return free.First();

            return -1;
        }

        #endregion

        #region Maximize, Restore and Close Events

        private void panel_Restored(object sender, EventArgs e)
        {
            if (e.Equals(EventArgs.Empty))
                return;

            if (_isMaximized)
            {
                //RestorePanel(sender as DragDockPanel);........TBRemoved
                Layout = _restoredLayout;
                ExchangeIndexes((UIElement)sender, 0, _restoredIndex);
                
                foreach (UIElement child in Children)
                {
                    if (child != sender && child is DragDockPanel)
                        ((DragDockPanel)child).PanelState = PanelState.Restored;
                }
                UpdateLayout();
                _isMaximized = false;
            }
        }

        private void panel_Maximized(object sender, EventArgs e)
        {
            if (e.Equals(EventArgs.Empty))
                return;

            if (!_isMaximized)
            {
                _restoredLayout = Layout;
                Layout = CreateMaximizedLayout();
                //RestorePanel(sender as DragDockPanel);.......TBRemoved

                foreach (UIElement child in Children)
                {
                    if (child != sender && child is DragDockPanel)
                        ((DragDockPanel)child).PanelState = PanelState.Minimized;
                }
                UpdateLayout();
            }

            
            int index = GetIndex((UIElement)sender);
            _restoredIndex = index;

            ExchangeIndexes((UIElement)sender,index,0);

            _isMaximized = true;
        }

        private LayoutDefinition CreateMaximizedLayout()
        {
            return new LayoutDefinition
                   {
                       Cells = CreateMaximizedCells(),
                       Definitions = CreateMaximizedDefinitions()
                   };
        }

        /// <summary>
        /// Handles panel closed event.
        /// </summary>
        /// <param name="sender">DragDock panel</param>
        /// <param name="e"></param>
        void panel_Closed(object sender, EventArgs e)
        {
            this.Children.Remove(sender as UIElement);
        }

        #endregion

        #region Dragging

        private void panel_DragFinished(object sender, DragDockPanelControls.DragEventArgs args)
        {
            int index = GetIndex(sender as UIElement);
            Cell current = Cells[index];

            Point p = args.MouseEventArgs.GetPosition(this);
            Cell cell = Cells.FirstOrDefault(c => c.ContainsPoint(p));

            if (cell == null || cell == current)
            {
                Animate(current, sender as UIElement);
            }
            else
            {
                int newIndex = Cells.IndexOf(cell);
                ExchangeIndexes((UIElement)sender, index, newIndex);
            }
        }
       
        private void ExchangeIndexes(UIElement sender, int index, int newIndex)
        {
            UIElement oldElement = sender;
            UIElement newElement = Children.FirstOrDefault(c => GetIndex(c) == newIndex);
            SetIndex(oldElement, newIndex);

            if (newElement != null)
                SetIndex(newElement, index);
        }

        #endregion
    }
}