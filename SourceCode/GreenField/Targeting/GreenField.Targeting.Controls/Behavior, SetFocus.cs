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
using System.Windows.Threading;

namespace GreenField.Targeting.Controls
{
    public class SetFocusBehavior : Behavior<ValueTextBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
        }

        public readonly static DependencyProperty WasLastEditedProperty = DependencyProperty.Register("WasLastEdited", typeof(Boolean), typeof(SetFocusBehavior), new PropertyMetadata(WhenPropertyChanged));

        private static void WhenPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var behavior = d as SetFocusBehavior;
            var wasLast = (Boolean)e.NewValue;
            if (wasLast)
            {
#warning HACK with timer!!!
                var timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromMilliseconds(50);
                timer.Stop();
                timer.Tick += (s1, e1) =>
                {
                    if (behavior.AssociatedObject.Focus())
                    {
                        behavior.WasLastEdited = false;
                    }
                    timer.Stop();
                    timer = null;
                };
                timer.Start();
            }
        }
        
        public Boolean WasLastEdited
        {
            get { return (Boolean)this.GetValue(WasLastEditedProperty); }
            set { this.SetValue(WasLastEditedProperty, value); }
        }
    }
}
