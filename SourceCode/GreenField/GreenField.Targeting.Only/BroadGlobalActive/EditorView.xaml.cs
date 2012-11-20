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
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Logging;
using System.Collections.Generic;
using GreenField.Targeting.Only.Backend.Targeting;


namespace GreenField.Targeting.Only.BroadGlobalActive
{
    public partial class EditorView : UserControl
    {
        public EditorView()
        {
            this.InitializeComponent();
        }

        private void Name_Clicked(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null) return;
            var expandable = button.DataContext as IExpandableModel;
            if (expandable == null) return;
            this.ToggleExpandable(expandable);
        }

        protected void ToggleExpandable(IExpandableModel expandable)
        {
            var before = expandable.IsExpanded;
            var after = !before;
            // this is what I though is enough to update the filter which is expected to show/hide the rows based on this property
            expandable.IsExpanded = after;

            // it turned out however that the filter is triggered by the CollectionChanged event
            // which we can only raise from the special method (see Poke below)
            // because the event can only be raised from within the class it belongs to
            // so we get to the view model (say goodbye to MVVM principles)
            var model = RuntimeHelper.DataContextAs<EditorViewModel>(this);
            // and trigger that damn event
            model.Residents.Poke();
        }

    }
}
