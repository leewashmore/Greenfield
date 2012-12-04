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
using System.Diagnostics;
using System.Windows.Interactivity;

namespace GreenField.Targeting.Controls
{
    public class UpdateOnEnterBehavior : Behavior<TextBox>
    {
        public static readonly DependencyProperty EnterCommandProperty = DependencyProperty.Register("EnterCommand", typeof(ICommand), typeof(UpdateOnEnterBehavior), new PropertyMetadata(null));
        public ICommand EnterCommand
        {
            get { return (ICommand)this.GetValue(EnterCommandProperty); }
            set { this.SetValue(EnterCommandProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.KeyDown += KeyDown;
        }

        private void KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var binding = this.AssociatedObject.GetBindingExpression(TextBox.TextProperty);
                if (binding != null)
                {
                    binding.UpdateSource();
                }
                var command = this.EnterCommand;
                var param = new Object();
                if (command != null && command.CanExecute(param))
                {
                    command.Execute(param);
                }
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.KeyDown -= KeyDown;
        }
    }
}