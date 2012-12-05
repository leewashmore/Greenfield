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
using System.Windows.Data;
using System.Diagnostics;

namespace GreenField.Targeting.Controls
{
    public class UndoingCancelledSelectionBehavior : Behavior<ComboBox>
    {
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(Object), typeof(UndoingCancelledSelectionBehavior), new PropertyMetadata(null));
        public Object Source
        {
            get { return this.GetValue(SourceProperty); }
            set { this.SetValue(SourceProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            
            this.AssociatedObject.DropDownClosed += DropDownClosed;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.DropDownClosed -= DropDownClosed;
        }

        private void DropDownClosed(object sender, EventArgs e)
        {
            if (this.Source != null)
            {
                if (this.AssociatedObject.SelectedItem != this.Source)
                {
                    this.AssociatedObject.SelectedItem = this.Source;
                }
            }
        }
    }
}
