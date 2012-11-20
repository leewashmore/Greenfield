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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections.Generic;

namespace GreenField.Targeting.Only
{
    public class PokableObservableCollection<TValue> : ObservableCollection<TValue>
    {
        public PokableObservableCollection(IEnumerable<TValue> values)
            : base(values)
        {
        }

        /// <summary>
        /// Triggers the firing of the CollectionChanged event from outside of the class.
        /// This is needed in order to make filtering work which can only be triggrered by this event.
        /// </summary>
        public void Poke()
        {
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }
}
