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
using System.Reflection;

namespace GreenField.Targeting.Controls
{
    /// <summary>
    /// There is no way to use command for passing event argumenets in the PRISM framework.
    /// So additional class is required.
    /// See more on this question at http://stackoverflow.com/questions/13565484/how-can-i-pass-the-event-argument-to-a-command-using-triggers
    /// </summary>
    public class InteractiveCommand : TriggerAction<DependencyObject>
    {
        public InteractiveCommand()
            : base()
        {
        }

        protected override void Invoke(object parameter)
        {
            if (base.AssociatedObject != null)
            {
                var command = this.ResolveCommand();
                if (command != null && command.CanExecute(parameter))
                {
                    command.Execute(parameter);
                }
            }
        }

        private ICommand ResolveCommand()
        {
            return this.Command;
        }

        public ICommand Command
        {
            get { return (ICommand)this.GetValue(CommandProperty); }
            set { this.SetValue(CommandProperty, value); }
        }

        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
            "Command", typeof(ICommand), typeof(InteractiveCommand), new PropertyMetadata(null)
        );
    }
}