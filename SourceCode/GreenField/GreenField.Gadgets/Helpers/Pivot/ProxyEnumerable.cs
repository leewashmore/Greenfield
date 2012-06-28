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
using System.Collections;
using System.Collections.Generic;

namespace GreenField.Gadgets.Helpers
{
    public class ProxyEnumerable : IEnumerable
    {
        private readonly List<object> items = new List<object>();

        public ProxyEnumerable(IEnumerable source)
        {
            foreach (object item in source)
            {
                this.items.Add(item);
            }
        }

        public IEnumerator GetEnumerator()
        {
            foreach (object item in this.items)
            {
                yield return item;
            }
        }
    }
}
