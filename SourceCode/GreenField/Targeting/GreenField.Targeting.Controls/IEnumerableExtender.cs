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
using System.Collections.Generic;

namespace GreenField.Targeting.Controls
{
    public static class IEnumerableExtender
    {
        public static void ForEach<TValue>(this IEnumerable<TValue> values, Action<TValue> handler)
        {
            foreach (var value in values)
            {
                handler(value);
            }
        }
    }
}
