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
using System.Diagnostics;

namespace GreenField.Targeting.Controls
{
    public class WatermarkBehavior : Behavior<TextBox>
    {
        public String Watermark
        {
            get { return this.GetValue(WatermarkProperty) as String; }
            set { this.SetValue(WatermarkProperty, value); }
        }

        public static readonly DependencyProperty WatermarkProperty = DependencyProperty.Register("Watermark", typeof(String), typeof(WatermarkBehavior), new PropertyMetadata("", new PropertyChangedCallback(OnWatermarkChanged)));

        private static void OnWatermarkChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behavior = d as WatermarkBehavior;
            if (behavior.AssociatedObject != null)
            {
                if (!String.IsNullOrWhiteSpace(e.NewValue as String))
                {
                    behavior.SetWatermarkIfNeeded();
                }
            }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.GotFocus += GotFocus;
            this.AssociatedObject.LostFocus += LostFocus;
            this.AssociatedObject.TextChanged += TextChanged;
            this.SetWatermarkIfNeeded();
        }

        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            Debug.WriteLine("Text changed: " + this.AssociatedObject.Text);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.GotFocus -= GotFocus;
            this.AssociatedObject.LostFocus -= LostFocus;
            this.AssociatedObject.TextChanged -= TextChanged;
        }

        private void GotFocus(object sender, RoutedEventArgs e)
        {
            if (this.AssociatedObject.Text == this.Watermark)
            {
                if (this.IsSpecified)
                {
                    // do nothing, the value is the same as the watermark value on purpose
                }
                else
                {
                    this.AssociatedObject.Text = String.Empty;
                    this.AssociatedObject.Foreground = new SolidColorBrush(Colors.Black);
                }
            }
        }

        private void LostFocus(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Lost focus with " + this.AssociatedObject.Text + ".");
            this.SetWatermarkIfNeeded();
        }

        private void SetWatermarkIfNeeded()
        {
            if (String.IsNullOrWhiteSpace(this.AssociatedObject.Text))
            {
                this.SetEmpty();
            }
            else
            {
                this.IsSpecified = true;
            }
        }

        private Boolean isSpecified;
        public Boolean IsSpecified
        {
            get
            {
                return this.isSpecified;
            }
            set
            {
                this.isSpecified = value;
                Debug.WriteLine("Setting IsSpecified to " + value + ".");
            }
        }

        private void SetEmpty()
        {
            Debug.WriteLine("Setting as empty.");
            this.AssociatedObject.Text = this.Watermark;
            this.AssociatedObject.Foreground = new SolidColorBrush(Colors.Gray);
            this.IsSpecified = false;
        }

        
    }
}
