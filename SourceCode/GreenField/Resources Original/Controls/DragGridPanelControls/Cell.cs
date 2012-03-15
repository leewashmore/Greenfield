using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GreenField.Common.DragGridPanelControls
{
    /// <summary>
    /// Grid cell is responsible for the alignment of the UI elements bound to 
    ///the grid.
    /// </summary>
    public class Cell : FrameworkElement
    {
        #region Contructors
        /// <summary>
        /// Empty Constructor
        /// </summary>
        public Cell()
        {
        }
        /// <summary>
        /// Constructor with row and column number.
        /// </summary>
        /// <param name="row">row number</param>
        /// <param name="column">Column number</param>
        public Cell(int row, int column)
        {
            Grid.SetRow(this, row);
            Grid.SetColumn(this, column);
        }
        /// <summary>
        /// Constructor with row number, column number, row span, column span
        /// </summary>
        /// <param name="row">row number</param>
        /// <param name="column">Column number</param>
        /// <param name="rowSpan">rowspan</param>
        /// <param name="columnSpan">columnspan</param>
        public Cell(int row, int column, int rowSpan, int columnSpan)
            : this(row, column)
        {
            Grid.SetRowSpan(this, rowSpan);
            Grid.SetColumnSpan(this, columnSpan);
        }

        #endregion

        #region Internal Methods
        /// <summary>
        /// Align elements in the cell
        /// </summary>
        /// <param name="element">element</param>
        internal void ArrangeElement(UIElement element)
        {
            Point point = Position;
            element.Arrange(new Rect(point, RenderSize));
            //element.SetValue(FrameworkElement.WidthProperty, ActualWidth);
            //element.SetValue(FrameworkElement.HeightProperty, ActualHeight);
            Canvas.SetLeft(element, point.X);
            Canvas.SetTop(element, point.Y);
        }
        /// <summary>
        /// Get position of the cell.
        /// </summary>
        /// <returns>Point corresponding to the upper left corner of the cell</returns>
        internal Point Position
        {
            get
            {
                DependencyObject parent = VisualTreeHelper.GetParent(this);
                GeneralTransform gt = TransformToVisual(parent as UIElement);
                return gt.Transform(new Point());
            }
        }
        /// <summary>
        /// Get the layout of the cell.
        /// </summary>
        /// <returns>Rectangle describing the layout of the cell.</returns>
        internal Rect Rectangle
        {
            get { return new Rect(Position, RenderSize); }
        }
        /// <summary>
        /// Checks whether a point lies inside the cell
        /// </summary>
        /// <param name="p">Point to be checked</param>
        /// <returns>True if the point lies inside the cell.</returns>
        internal bool ContainsPoint(Point p)
        {
            return Rectangle.Contains(p);
        }
        #endregion
    }
}