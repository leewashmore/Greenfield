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

namespace Aims.Controls
{
    public class AutoCompleteAsyncBehavior : Behavior<AutoCompleteBoxWithEnterWorking>
    {
        public static readonly DependencyProperty SearchCommandProperty = DependencyProperty.Register("SearchCommand", typeof(ICommand), typeof(AutoCompleteAsyncBehavior), new PropertyMetadata(null));
        public static readonly DependencyProperty PickCommandProperty = DependencyProperty.Register("PickCommand", typeof(ICommand), typeof(AutoCompleteAsyncBehavior), new PropertyMetadata(null));

        public ICommand SearchCommand
        {
            get { return (ICommand)this.GetValue(SearchCommandProperty); }
            set { this.SetValue(SearchCommandProperty, value); }
        }

        public ICommand PickCommand
        {
            get { return (ICommand)this.GetValue(PickCommandProperty); }
            set { this.SetValue(PickCommandProperty, value); }
        }

        protected override void OnAttached()
        {
            this.AssociatedObject.Enter += Enter;
            this.AssociatedObject.Populating += this.PopulatingHook;
        }

        protected override void OnDetaching()
        {
            this.AssociatedObject.Enter -= Enter;
            this.AssociatedObject.Populating -= this.PopulatingHook;
        }

        private void Enter(object sender, EventArgs e)
        {
            var command = this.PickCommand;
            var parameter = new Object();
            if (command != null && command.CanExecute(parameter))
            {
                command.Execute(parameter);
            }
        }

        private void PopulatingHook(object sender, PopulatingEventArgs e)
        {
            var command = this.SearchCommand;
            var parameter = new AutoCompleteRequest(e.Parameter, delegate
            {
                this.AssociatedObject.IsDropDownOpen = true;
                this.AssociatedObject.PopulateComplete();
                this.AssociatedObject.IsDropDownOpen = true;
            });

            if (command != null && command.CanExecute(parameter))
            {
                e.Cancel = true;
                command.Execute(parameter);
            }
        }
    }
}
