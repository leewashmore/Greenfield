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
using System.Collections.Generic;

namespace GreenField.IssuerShares.Controls
{
    public class Helper
    {
        public static TSomething As<TSomething>(TSomething something)
        {
            return something;
        }

        public static ObservableCollection<TValue> ToObservableCollection<TValue>(IEnumerable<TValue> values)
        {
            var result = new ObservableCollection<TValue>();
            foreach (var value in values)
            {
                result.Add(value);
            }
            return result;
        }

        
    }
}
