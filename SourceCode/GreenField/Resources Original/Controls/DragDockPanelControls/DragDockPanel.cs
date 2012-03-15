using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Runtime.Serialization;

namespace GreenField.Common.DragDockPanelControls
{
    /// <summary>
    /// A draggable, dockable, expandable panel class.
    /// </summary>
    public class DragDockPanel : DraggablePanel
    {
        /// <summary>
        /// The template part name for the maxmize toggle button.
        /// </summary>
        private const string ElementMaximizeToggleButton = "MaximizeToggleButton";

        /// <summary>
        /// The MaximizationEnabled Dependency Property.
        /// </summary>
        public static readonly DependencyProperty MaximizationEnabledProperty =
            DependencyProperty.Register("MaximizationEnabled", typeof(bool), typeof(DragDockPanel),new PropertyMetadata(true));
        /// <summary>
        /// Gets or sets a value indicating whether dragging is enabled.
        /// </summary>
        public bool MaximizationEnabled
        {
            get { return (bool)GetValue(MaximizationEnabledProperty); }
            set { SetValue(MaximizationEnabledProperty, value); }
        }

        /// <summary>
        /// The template part name for the Close Button image.
        /// </summary>
        private const string ElementCloseButton = "imageCloseButton";

        #region Private members

        /// <summary>
        /// Ignore the last uncheck event flag.
        /// </summary>
        private bool ignoreUnCheckedEvent;

        /// <summary>
        /// Panel maximised flag.
        /// </summary>
        private PanelState panelState = PanelState.Restored;

        #endregion

        /// <summary>
        /// Drag dock panel constructor.
        /// </summary>
        public DragDockPanel()
        {
            DefaultStyleKey = typeof (DragDockPanel);
        }

        #region Events

        /// <summary>
        /// The maxmised event.
        /// </summary>
        public event EventHandler Maximized;

        /// <summary>
        /// The restored event.
        /// </summary>
        public event EventHandler Restored;

        /// <summary>
        /// The minimized event.
        /// </summary>
        public event EventHandler Minimized;

        /// <summary>
        /// The minimised event.
        /// </summary>
        public event EventHandler Closed;

        #endregion

        #region Public members

        /// <summary>
        /// Gets or sets the calculated panel index.
        /// </summary>
        public int PanelIndex { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the panel is maximised (and updates the toggle button UI)
        /// </summary>
        [Category("Panel Properties"), Description("Sets whether the panel is maximised.")]
        public PanelState PanelState
        {
            get { return panelState; }

            set
            {
                panelState = value;
                var maximizeToggle = GetTemplateChild(ElementMaximizeToggleButton) as ToggleButton;
                
                if (panelState == PanelState.Restored)
                {
                    ignoreUnCheckedEvent = true;
                    if (maximizeToggle != null)
                    {
                        maximizeToggle.IsChecked = false;
                    }
                    
                    if (Restored != null)
                    {
                        Restored(this, EventArgs.Empty);
                    }
                }
                else if (panelState == PanelState.Maximized)
                {
                    if (maximizeToggle != null && MaximizationEnabled)
                    {
                        maximizeToggle.IsChecked = true;
                    }
                    if (Maximized != null)
                    {
                        Maximized(this, EventArgs.Empty);
                    }
                }
                else if (panelState == PanelState.Minimized)
                {
                    if (maximizeToggle!=null && MaximizationEnabled)
                    {
                        ignoreUnCheckedEvent = true;
                        maximizeToggle.IsChecked = false;
                    }

                    if (Minimized != null)
                    {
                        Minimized(this, EventArgs.Empty);
                    }
                }
                else if (panelState==PanelState.Closed)
                {
                    if (Closed != null)
                    {
                        Closed(this, EventArgs.Empty);
                    }
                }
            }
        }

        #endregion

        /// <summary>
        /// Gets called once the template is applied.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var maximizeToggle =
                GetTemplateChild(ElementMaximizeToggleButton) as ToggleButton;

            if (maximizeToggle != null)
            {
                maximizeToggle.Checked +=
                    MaximizeToggle_Checked;
                maximizeToggle.Unchecked +=
                    MaximizeToggle_Unchecked;

                if (PanelState == PanelState.Restored)
                {
                    ignoreUnCheckedEvent = true;
                    maximizeToggle.IsChecked = false;
                }
                else if (PanelState == PanelState.Maximized)
                {
                    maximizeToggle.IsChecked = true;
                }
            }

            var imgBtnClose = GetTemplateChild(ElementCloseButton) as Image;
            if (imgBtnClose != null)
            {
                imgBtnClose.MouseLeftButtonDown += imgBtnClose_MouseLeftButtonDown;
            }
        }

        

        /// <summary>
        /// Override for updating the panel position.
        /// </summary>
        /// <param name="pos">The new position.</param>
        public override void UpdatePosition(Point pos)
        {
            Canvas.SetLeft(this, pos.X);
            Canvas.SetTop(this, pos.Y);
        }

        #region Maximize events

        /// <summary>
        /// Fires the minimised event.
        /// </summary>
        /// <param name="sender">The maximised toggle.</param>
        /// <param name="e">Routed event args.</param>
        private void MaximizeToggle_Unchecked(object sender, RoutedEventArgs e)
        {
            if (!ignoreUnCheckedEvent)
            {
                PanelState = PanelState.Restored;

                // Fire the panel minimized event
                if (Restored != null)
                {
                    Restored(this, e);
                }
            }
            else
            {
                ignoreUnCheckedEvent = false;
            }
        }

        /// <summary>
        /// Fires the maximised event.
        /// </summary>
        /// <param name="sender">The maximised toggle.</param>
        /// <param name="e">Routed event args.</param>
        private void MaximizeToggle_Checked(object sender, RoutedEventArgs e)
        {
            // Bring the panel to the front
            Canvas.SetZIndex(this, CurrentZIndex++);

            ignoreUnCheckedEvent = false;
           
            panelState = PanelState.Maximized;
            // Fire the panel maximized event
            if (Maximized != null)
            {
                Maximized(this, e);
            }
        }

        #endregion


        #region Close Event

        /// <summary>
        /// Fires the close event.
        /// </summary>
        /// <param name="sender">Image button</param>
        /// <param name="e">MouseButtonEventArgs</param>
        void imgBtnClose_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //Animate size dwindling to zero
            this.AnimateSize(0, 0);

            panelState = PanelState.Closed;
            //Fire the Panel Closed event.
            if (Closed != null)
            {
                Closed(this, e);
            }

        }
        #endregion
    }
}