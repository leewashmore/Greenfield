using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows.Media.Animation;
using System.Windows.Media;
using System.Windows.Controls.Primitives;

namespace GreenField.Targeting.Controls
{
    /// <summary>
    /// A control to display information when the user hovers over an owner control
    /// </summary> 
    [TemplatePart(Name = CustomTooltip.NormalStateName, Type = typeof(Storyboard))]
    [TemplatePart(Name = CustomTooltip.RootElementName, Type = typeof(FrameworkElement))]
    [TemplatePart(Name = CustomTooltip.VisibleStateName, Type = typeof(Storyboard))]
    public partial class CustomTooltip : ContentControl
    {
        private const Double TOOLTIP_tolerance = 2.0;

        public Double HorizontalOffset
        {
            get { return (Double)GetValue(HorizontalOffsetProperty); }
            set { SetValue(HorizontalOffsetProperty, value); }
        }

        public static readonly DependencyProperty HorizontalOffsetProperty = DependencyProperty.Register("HorizontalOffset", typeof(Double), typeof(CustomTooltip), new PropertyMetadata(new PropertyChangedCallback(OnHorizontalOffsetPropertyChanged)));

        private static void OnHorizontalOffsetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // HorizontalOffset dependency property should be defined on a CustomTooltip 
            CustomTooltip toolTip = (CustomTooltip)d;

            var newOffset = (Double)e.NewValue;
            // Working around temporary limitations in Silverlight:
            // perform inequality test
            if (newOffset != (Double)e.OldValue)
            {
                toolTip.OnOffsetChanged(newOffset, 0);
            }
        }


        /// <summary> 
        /// Gets a value that determines whether tooltip is displayed or not. 
        /// </summary>
        public Boolean IsOpen
        {
            get { return (Boolean)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        /// <summary> 
        /// Identifies the IsOpen dependency property. 
        /// </summary>
        public static readonly DependencyProperty IsOpenProperty = DependencyProperty.Register("IsOpen", typeof(Boolean), typeof(CustomTooltip), new PropertyMetadata(new PropertyChangedCallback(OnIsOpenPropertyChanged)));

        private static void OnIsOpenPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // IsOpen dependency property should be defined on a CustomTooltip 
            CustomTooltip toolTip = (CustomTooltip)d;

            if (((Boolean)e.NewValue != (Boolean)e.OldValue))
            {
                toolTip.OnIsOpenChanged((Boolean)e.NewValue);
            }
        }


        /// <summary>
        /// Determines a vertical offset in pixels from the bottom of the
        /// mouse bounding rectangle to the top of the CustomTooltip. 
        /// </summary> 
        public Double VerticalOffset
        {
            get { return (Double)GetValue(VerticalOffsetProperty); }
            set { SetValue(VerticalOffsetProperty, value); }
        }

        public static readonly DependencyProperty VerticalOffsetProperty = DependencyProperty.Register("VerticalOffset", typeof(Double), typeof(CustomTooltip), new PropertyMetadata(new PropertyChangedCallback(OnVerticalOffsetPropertyChanged)));

        private static void OnVerticalOffsetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // VerticalOffset dependency property should be defined on a CustomTooltip
            CustomTooltip toolTip = (CustomTooltip)d;

            Double newOffset = (Double)e.NewValue;
            if (newOffset != (Double)e.OldValue)
            {
                toolTip.OnOffsetChanged(0, newOffset);
            }
        }


        /// <summary>
        /// This storyboard runs when the CustomTooltip closes.
        /// </summary> 
        private Storyboard NormalState;
        private const String NormalStateName = "Normal State";

        /// <summary>
        /// Part for the CustomTooltip.
        /// </summary> 
        internal FrameworkElement RootElement;
        internal const String RootElementName = "RootElement";

        /// <summary> 
        /// This storyboard runs when the CustomTooltip opens.
        /// </summary> 
        private Storyboard VisibleState;
        private const String VisibleStateName = "Visible State";


        /// <summary>
        /// Occurs when a CustomTooltip is closed and is no longer visible. 
        /// </summary>
        public event RoutedEventHandler Closed;

        /// <summary>
        /// Occurs when a CustomTooltip becomes visible.
        /// </summary> 
        public event RoutedEventHandler Opened;


        private Boolean _beginClosing;
        private Boolean _closingCompleted = true;
        private Size _lastSize;
        private Boolean _openingCompleted = true;
        private Boolean _openPopup;
        private Popup _parentPopup;

        private delegate void PerformOnNextTick();

        internal Popup ParentPopup
        {
            get { return this._parentPopup; }
            private set { this._parentPopup = value; }
        }

        /// <summary> 
        /// Creates a default CustomTooltip element
        /// </summary>
        public CustomTooltip()
            : base()
        {
            this.SizeChanged += new SizeChangedEventHandler(OnToolTipSizeChanged);
        }



        /// <summary>
        /// Apply a template to the CustomTooltip, invoked from ApplyTemplate
        /// </summary> 
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            // If the part is not present in the template,
            // don't display content, but don't throw either 

            // get the element
            RootElement = GetTemplateChild(CustomTooltip.RootElementName) as FrameworkElement;

            if (RootElement != null)
            {
                // get the states
                this.VisibleState = RootElement.Resources[VisibleStateName] as Storyboard;
                this.NormalState = RootElement.Resources[NormalStateName] as Storyboard;

                if (this.VisibleState != null)
                {
                    this.VisibleState.Completed += new EventHandler(OnOpeningCompleted);

                    // first time through when the opened event is fired, the storyboard is not 
                    // loaded from the template yet, because ApplyTemplate wasn't called yet,
                    // so I start the storyboard manually
                    // 

                    OnPopupOpened(null, EventArgs.Empty);
                }

                if (this.NormalState != null)
                {
                    this.NormalState.Completed += new EventHandler(OnClosingCompleted);
                }
            }
        }



        private void BeginClosing()
        {
            this._beginClosing = false;

            // close the popup after the animation is completed
            if (this.NormalState != null)
            {
                this._closingCompleted = false;
                this.NormalState.Begin();
            }
        }

        private void HookupParentPopup()
        {
            Debug.Assert(this._parentPopup == null, "this._parentPopup should be null, we want to set visual tree once");

            this._parentPopup = new Popup();

            this.IsTabStop = false;

            this._parentPopup.Child = this;

            // Working around temporary limitations in Silverlight:
            // set IsHitTestVisible on both the popup and the child 
            // 
            this._parentPopup.IsHitTestVisible = false;
            this.IsHitTestVisible = false;

        }

        private void OnClosed(RoutedEventArgs e)
        {
            RoutedEventHandler snapshot = this.Closed;
            if (snapshot != null)
            {
                snapshot(this, e);
            }
        }

        // called when the closing state transition is completed
        private void OnClosingCompleted(Object sender, EventArgs e)
        {
            this._closingCompleted = true;
            this._parentPopup.IsOpen = false;

            // Working around temporary limitations in Silverlight:
            // send the event manually 
            //

            this.Dispatcher.BeginInvoke(delegate() { OnPopupClosed(null, EventArgs.Empty); });

            // if the tooltip was forced to stop the closing animation, because it has to reopen,
            // proceed with open 
            if (this._openPopup)
            {
                this.Dispatcher.BeginInvoke(delegate() { OpenPopup(); });
            }
        }

        private void OnIsOpenChanged(Boolean isOpen)
        {
            if (isOpen)
            {
                if (this._parentPopup == null)
                {
                    HookupParentPopup();
                    OpenPopup();
                    return;
                }

                if (!this._closingCompleted)
                {
                    Debug.Assert(this.NormalState != null);

                    // Completed event for the closing storyboard will open the parent popup
                    // because _openPopup is set
                    this._openPopup = true;

                    this.NormalState.SkipToFill();
                    return;
                }

                PerformPlacement(this.HorizontalOffset, this.VerticalOffset);
                OpenPopup();
            }
            else
            {
                Debug.Assert(this._parentPopup != null);

                if (!this._openingCompleted)
                {
                    if (this.NormalState != null)
                    {
                        this._beginClosing = true;
                    }
                    this.VisibleState.SkipToFill();
                    // delay start of the closing storyboard until the opening one is completed
                    return;
                }

                if ((this.NormalState == null) || (this.NormalState.Children.Count != 0))
                {
                    // close immediatelly if no storyboard provided
                    this._parentPopup.IsOpen = false;
                    this.Dispatcher.BeginInvoke(delegate() { OnPopupClosed(null, EventArgs.Empty); });
                }
                else
                {
                    // close the popup after the animation is completed
                    this._closingCompleted = false;
                    this.NormalState.Begin();
                }
            }
        }

        private void OpenPopup()
        {
            this._parentPopup.IsOpen = true;

            // Working around temporary limitations in Silverlight:
            // send the Opened event manually
            // 
            this.Dispatcher.BeginInvoke(delegate() { OnPopupOpened(null, EventArgs.Empty); });

            this._openPopup = false;
        }

        private void OnOffsetChanged(Double horizontalOffset, Double verticalOffset)
        {
            if (this._parentPopup == null)
            {
                return;
            }

            if (IsOpen)
            {
                // update the current CustomTooltip position if needed 
                PerformPlacement(horizontalOffset, verticalOffset);
            }
        }

        private void OnOpened(RoutedEventArgs e)
        {
            RoutedEventHandler snapshot = this.Opened;
            if (snapshot != null)
            {
                snapshot(this, e);
            }
        }

        // called when the Visible state transition is completed
        private void OnOpeningCompleted(Object sender, EventArgs e)
        {
            this._openingCompleted = true;

            if (this._beginClosing)
            {
                this.Dispatcher.BeginInvoke(delegate() { BeginClosing(); });
            }
        }

        private void OnPopupClosed(Object source, EventArgs e)
        {
            OnClosed(new RoutedEventArgs { OriginalSource = this });
        }

        private void OnPopupOpened(Object source, EventArgs e)
        {
            //
            if (this.VisibleState != null)
            {
                this._openingCompleted = false;
                this.VisibleState.Begin();
            }
            OnOpened(new RoutedEventArgs { OriginalSource = this });
        }

        internal void OnRootVisualSizeChanged()
        {
            if (this._parentPopup != null)
            {
                PerformPlacement(this.HorizontalOffset, this.VerticalOffset);
            }
        }

        private void OnToolTipSizeChanged(Object sender, SizeChangedEventArgs e)
        {
            this._lastSize = e.NewSize;
            if (this._parentPopup != null)
            {
                PerformPlacement(this.HorizontalOffset, this.VerticalOffset);
            }
        }

        private void PerformClipping(Size size)
        {
            Point mouse = ToolTipService.MousePosition;
            RectangleGeometry rectangle = new RectangleGeometry();
            rectangle.Rect = new Rect(mouse.X, mouse.Y, size.Width, size.Height);
            ((UIElement)Content).Clip = rectangle;
        }

        private void PerformPlacement(Double horizontalOffset, Double verticalOffset)
        {
            Point mouse = ToolTipService.MousePosition;

            // align CustomTooltip with the bottom left corner of mouse bounding rectangle 
            //
            Double top = mouse.Y + new TextBlock().FontSize;
            Double left = mouse.X;

            top += verticalOffset;
            left += horizontalOffset;

            Double maxY = ToolTipService.RootVisual.ActualHeight;
            Double maxX = ToolTipService.RootVisual.ActualWidth;

            Rect toolTipRect = new Rect(left, top, this._lastSize.Width, this._lastSize.Height);
            Rect intersectionRect = new Rect(0, 0, maxX, maxY);

            intersectionRect.Intersect(toolTipRect);
            if ((Math.Abs(intersectionRect.Width - toolTipRect.Width) < TOOLTIP_tolerance) &&
                (Math.Abs(intersectionRect.Height - toolTipRect.Height) < TOOLTIP_tolerance))
            {
                // CustomTooltip is almost completely inside the plug-in 
                this._parentPopup.VerticalOffset = top;
                this._parentPopup.HorizontalOffset = left;
                return;
            }
            else if (intersectionRect.Equals(new Rect(0, 0, maxX, maxY)))
            {
                //CustomTooltip is bigger than the plug-in
                PerformClipping(new Size(maxX, maxY));
                this._parentPopup.VerticalOffset = 0;
                this._parentPopup.HorizontalOffset = 0;

                PerformClipping(new Size(maxX, maxY));
                return;
            }

            Double right = left + toolTipRect.Width;
            Double bottom = top + toolTipRect.Height;

            if (bottom > maxY)
            {
                // If the lower edge of the plug-in obscures the CustomTooltip,
                // it repositions itself to align with the upper edge of the bounding box of the mouse.
                bottom = top;
                top -= toolTipRect.Height;
            }

            if (top < 0)
            {
                // If the upper edge of Plug-in obscures the CustomTooltip, 
                // the control repositions itself to align with the upper edge.
                // align with the top of the plug-in
                top = 0;
            }
            else if (bottom > maxY)
            {
                // align with the bottom edge 
                top = Math.Max(0, maxY - toolTipRect.Height);
            }

            if (right > maxX)
            {
                // If the right edge obscures the CustomTooltip,
                // it opens in the opposite direction from the obscuring edge.
                right = left;
                left -= toolTipRect.Width;
            }

            if (left < 0)
            {
                // If the left edge obscures the CustomTooltip, 
                // it then aligns with the obscuring screen edge
                left = 0;
            }
            else if (right > maxX)
            {
                // align with the right edge 
                left = Math.Max(0, maxX - toolTipRect.Width);
            }

            // position the parent Popup
            this._parentPopup.VerticalOffset = top;
            this._parentPopup.HorizontalOffset = left;

            bottom = top + toolTipRect.Height;
            right = left + toolTipRect.Width;

            // if right/bottom doesn't fit into the plug-in bounds, clip the CustomTooltip
            Double dX = right - maxX;
            Double dY = bottom - maxY;
            if ((dX >= TOOLTIP_tolerance) || (dY >= TOOLTIP_tolerance))
            {
                PerformClipping(new Size(toolTipRect.Width - dX, toolTipRect.Height - dY));
            }
        }
    }
}