using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace GreenField.Targeting.Controls
{
    /// <summary> 
    /// Service class that provides the system implementation for displaying ToolTips.
    /// </summary>
    public static class CustomToolTipService
    {
        private static ToolTip current;

        private static Object lastSource;
        private static UIElement owner;
        private static FrameworkElement rootVisual;
        private static Dictionary<UIElement, ToolTip> all = new Dictionary<UIElement, ToolTip>();

        public static readonly DependencyProperty ToolTipProperty = DependencyProperty.RegisterAttached("ToolTip", typeof(Object), typeof(CustomToolTipService), new PropertyMetadata(OnToolTipPropertyChanged));

        /// <summary> 
        /// Gets the value of the ToolTip property on the specified Object.
        /// </summary>
        public static Object GetToolTip(DependencyObject element)
        {
            if (element == null) throw new ArgumentNullException("element");
            return element.GetValue(CustomToolTipService.ToolTipProperty);
        }

        /// <summary>
        /// Sets the ToolTip property on the specified Object. 
        /// </summary>
        public static void SetToolTip(DependencyObject element, Object value)
        {
            if (element == null) throw new ArgumentNullException("element");
            element.SetValue(CustomToolTipService.ToolTipProperty, value);
        }

        private static void OnToolTipPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UIElement owner = (UIElement)d;

            Object toolTip = e.NewValue;
            if (e.OldValue != null)
            {
                UnregisterTooltip(owner);
            }

            if (toolTip == null)
            {
                return;
            }

            RegisterToolTip(owner, toolTip);
            SetRootVisual();
        }

        /// <summary>
        /// VisualRoot - the main page
        /// </summary> 
        internal static FrameworkElement RootVisual
        {
            get
            {
                SetRootVisual();
                return CustomToolTipService.rootVisual;
            }
        }

        /// <summary> 
        /// Access the toolTip which is currenly open by mouse movements
        /// </summary> 
        internal static ToolTip CurrentToolTip
        {
            get { return CustomToolTipService.current; }
        }


        internal static void OnLostFocus(Object sender, RoutedEventArgs e)
        {
            if (CustomToolTipService.current == null)
            {
                CustomToolTipService.owner = null;
                CustomToolTipService.lastSource = null;
                return;
            }
            CloseTooltip(null, EventArgs.Empty);
        }

        private static void CloseTooltip(Object sender, EventArgs e)
        {
            Debug.Assert(CustomToolTipService.current != null, "no ToolTip to close");
            CustomToolTipService.current.IsOpen = false;
            CustomToolTipService.current = null;
            CustomToolTipService.owner = null;
            CustomToolTipService.lastSource = null;
        }

        private static ToolTip ConvertToToolTip(Object o)
        {
            ToolTip toolTip = o as ToolTip;
            if (toolTip == null)
            {
                toolTip = new ToolTip();
                toolTip.Content = o;
            }
            return toolTip;
        }

        private static void OnGotFocus(Object sender, RoutedEventArgs e)
        {
            var source = e.OriginalSource;

            // check if already processed then return
            if (CustomToolTipService.lastSource != null && Object.ReferenceEquals(CustomToolTipService.lastSource, source))
            {
                return;
            }

            var senderElement = (UIElement)sender;
            if (CustomToolTipService.current != null)
            {
                if (CustomToolTipService.all[senderElement] != CustomToolTipService.current)
                {
                    // first close the previous ToolTip if entering nested elements with tooltips 
                    CloseTooltip(null, EventArgs.Empty);
                }
                else
                {
                    // reentering the same element
                    return;
                }
            }

            CustomToolTipService.owner = senderElement;
            CustomToolTipService.lastSource = source;

            Debug.Assert(CustomToolTipService.current == null);

            SetRootVisual();
            OpenTooltip(null, EventArgs.Empty);

        }

        private static void OpenTooltip(Object sender, EventArgs e)
        {
            Debug.Assert(CustomToolTipService.owner != null, "ToolTip owner was not set prior to starting the open timer");
            Debug.Assert(CustomToolTipService.all[CustomToolTipService.owner] != null, "ToolTip must have been registered");

            CustomToolTipService.current = CustomToolTipService.all[CustomToolTipService.owner];
            CustomToolTipService.current.IsOpen = true;
        }
        
        private static void RegisterToolTip(UIElement owner, Object toolTip)
        {
            Debug.Assert(!CustomToolTipService.all.ContainsKey(owner), "duplicate tooltip for the same owner element");
            Debug.Assert(owner != null, "ToolTip must have an owner");
            Debug.Assert(toolTip != null, "ToolTip can not be null");

            owner.GotFocus += OnGotFocus;
            owner.LostFocus += OnLostFocus;
            CustomToolTipService.all[owner] = ConvertToToolTip(toolTip);
        }

        private static void SetRootVisual()
        {
            if (CustomToolTipService.rootVisual == null && Application.Current != null)
            {
                CustomToolTipService.rootVisual = Application.Current.RootVisual as FrameworkElement;
            }
        }

        private static void UnregisterTooltip(UIElement owner)
        {
            Debug.Assert(owner != null, "owner element is required");

            if (!CustomToolTipService.all.ContainsKey(owner)) return;

            var tooltip = CustomToolTipService.all[owner];
            if (tooltip.IsOpen)
            {
                if (tooltip == CustomToolTipService.current)
                {
                    CustomToolTipService.current = null;
                    CustomToolTipService.owner = null;
                    CustomToolTipService.lastSource = null;
                }

                tooltip.IsOpen = false;
            }

            CustomToolTipService.all[owner] = null;
            CustomToolTipService.all.Remove(owner);
        }
    }
}