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
        
        public SetFocusBehavior()
        {
        }


        private DispatcherTimer timer;
        protected override void OnAttached()
        {
            base.OnAttached();
            this.timer = new DispatcherTimer();
            this.timer.Stop();
            this.timer.Interval = TimeSpan.FromMilliseconds(500);
            this.timer.Tick += timer_Tick;
            this.timer.Start();
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.timer.Stop();
            this.timer.Tick -= timer_Tick;
            this.timer = null;
        }

        public readonly static DependencyProperty WasLastEditedProperty = DependencyProperty.Register("WasLastEdited", typeof(Boolean), typeof(SetFocusBehavior), new PropertyMetadata(null));
        
        public Boolean WasLastEdited
        {
            get { return (Boolean)this.GetValue(WasLastEditedProperty); }
            set { this.SetValue(WasLastEditedProperty, value); }
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if (this.WasLastEdited)
            {
                this.AssociatedObject.Focus();
                this.WasLastEdited = false;
            }
            this.timer.Stop();
        }
    }
}
