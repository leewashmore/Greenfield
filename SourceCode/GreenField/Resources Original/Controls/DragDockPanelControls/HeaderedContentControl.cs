using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;

namespace GreenField.Common.DragDockPanelControls
{
    /// <summary>
    /// The base class for all controls that contain single content and have a header.
    /// HeaderedContentControl adds Header and HeaderTemplatefeatures to a ContentControl.
    /// </summary>
    public class HeaderedContentControl : ContentControl
    {
        #region public object Header

        /// <summary>
        /// Identifies the Header dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register(
                "Header",
                typeof (object),
                typeof (HeaderedContentControl),
                null);

        #endregion public object Header

        #region public DataTemplate HeaderTemplate

        /// <summary>
        /// Identifies the HeaderTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register(
                "HeaderTemplate",
                typeof (DataTemplate),
                typeof (HeaderedContentControl),
                null);

        #endregion public DataTemplate HeaderTemplate

        /// <summary>
        /// Default DependencyObject constructor.
        /// </summary>
        public HeaderedContentControl()
        {
            DefaultStyleKey = typeof (HeaderedContentControl);
        }

        /// <summary>
        /// Gets or sets an object that labels the HeaderedContentControl.  The
        /// default value is null.  The header can be a string, UIElement, or
        /// any other content.
        /// </summary>
        [Category("Common Properties"), Description("Gets or sets an object that labels the HeaderedContentControl.")]
        public object Header
        {
            get { return GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        /// <summary>
        /// Gets or sets the template used to display a control's header.
        /// The default is null.
        /// </summary>
        [Category("Common Properties"), Description("Gets or sets the template used to display a control's header.")]
        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate) GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }
       
    }
}