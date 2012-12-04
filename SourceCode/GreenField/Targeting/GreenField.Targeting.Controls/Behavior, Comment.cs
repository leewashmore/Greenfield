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
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using TopDown.FacingServer.Backend.Targeting;
using Microsoft.Practices.Prism.Commands;

namespace GreenField.Targeting.Controls
{

    public class CommentContext
    {
        public Object Data { get; set; }
        public DelegateCommand CloseCommand { get; set; }
    }
    
    public class CommentBehavior : Behavior<ValueTextBox>
    {
        private readonly TimeSpan howLongWeWaitBeforePopupCloses = TimeSpan.FromMilliseconds(100);
        private DispatcherTimer popupClosingTimer;

        public static DependencyProperty ContentTemplateProperty = DependencyProperty.Register("ContentTemplate", typeof(DataTemplate), typeof(CommentBehavior), new PropertyMetadata(null));
        private Popup popup;
        
        private Popup Popup
        {
            get
            {
                if (this.popup == null)
                {
                    this.popup = new Popup();
                    this.popup.Child = new ContentControl {
                        ContentTemplate = this.ContentTemplate,
                        Content = new CommentContext {
                            Data = this.AssociatedObject.DataContext,
                            CloseCommand = new DelegateCommand(this.ClosePopup)
                        }
                    };
                    this.popup.Child.GotFocus += this.PopupChild_GotFocus;
                    this.popup.Child.LostFocus += this.PopupChild_LostFocus;
                }
                return this.popup;
            }
        }

        public DataTemplate ContentTemplate
        {
            get { return (DataTemplate)this.GetValue(ContentTemplateProperty); }
            set { this.SetValue(ContentTemplateProperty, value); }
        }

        // initializing / deinitializing
        
        protected override void OnAttached()
        {
            base.OnAttached();
            this.popupClosingTimer = new DispatcherTimer();
            this.popupClosingTimer.Stop();
            this.popupClosingTimer.Interval = howLongWeWaitBeforePopupCloses;
            this.popupClosingTimer.Tick += this.Tick;
            this.AssociatedObject.GotFocus += this.GotFocus;
            this.AssociatedObject.LostFocus += this.LostFocus;
        }


        protected override void OnDetaching()
        {
            this.OnDetaching();
            this.AssociatedObject.GotFocus -= this.GotFocus;
            this.AssociatedObject.LostFocus -= this.LostFocus;
            this.popupClosingTimer.Tick -= this.Tick;
            this.Popup.Child.GotFocus -= this.PopupChild_GotFocus;
            this.Popup.Child.LostFocus -= this.PopupChild_LostFocus;
            this.popupClosingTimer = null;
        }
        
        
        private void PopupChild_GotFocus(object sender, RoutedEventArgs e)
        {
            this.AssociatedObject.CancelFinishing();
            this.popupClosingTimer.Stop();
        }


        private void PopupChild_LostFocus(object sender, RoutedEventArgs e)
        {
            this.popupClosingTimer.Start();
        }


        private void Tick(object sender, EventArgs e)
        {
            this.ClosePopup();
        }

        public void ClosePopup()
        {
            this.Popup.IsOpen = false;
            this.popupClosingTimer.Stop();
            this.AssociatedObject.Finish();
        }

        protected void GotFocus(object sender, RoutedEventArgs e)
        {
            this.popupClosingTimer.Stop();
            this.Popup.Visibility = Visibility.Visible;
            this.Popup.IsOpen = true;
            var at = this.CalculatePopupPosition();
            this.Popup.HorizontalOffset = at.X;
            this.Popup.VerticalOffset = at.Y;
        }

        private Point CalculatePopupPosition()
        {
            var owner = this.AssociatedObject;
            var transformation = owner.TransformToVisual(Application.Current.RootVisual);
            var at = transformation.Transform(new Point(owner.ActualWidth, 0));
            return at;
        }

        protected void LostFocus(object sender, RoutedEventArgs e)
        {
            this.popupClosingTimer.Start();
        }
    }
}
