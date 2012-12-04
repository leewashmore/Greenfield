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

namespace GreenField.Targeting.Controls
{
    public class ToggleBehavior : Behavior<ButtonBase>
    {
        public static readonly DependencyProperty SwitchProperty = DependencyProperty.Register("Switch", typeof(Boolean), typeof(ToggleBehavior), new PropertyMetadata(null));

        public Boolean Switch
        {
            get { return (Boolean)this.GetValue(SwitchProperty); }
            set { this.SetValue(SwitchProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.Click += Click;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.Click -= Click;
        }

        private void Click(object sender, RoutedEventArgs e)
        {
            this.Switch = !this.Switch;
        }
    }
}
