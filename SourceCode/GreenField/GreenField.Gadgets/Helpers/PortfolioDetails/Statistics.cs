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
using System.Linq;

namespace GreenField.Gadgets.Helpers
{
    public class Statistics
    {
        public static decimal GroupPortfolioWeight<TSource>(IEnumerable<TSource> source, Func<TSource, decimal?> selector)
        {
            int itemCount = source.Count();
            if (itemCount > 1)
            {
                IEnumerable<decimal> values = from i in source select Convert.ToDecimal(selector(i));
                decimal sum = values.Sum();
                return sum;
            }
            return 0;
        }
    }
}
