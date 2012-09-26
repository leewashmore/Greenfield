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
using Telerik.Windows.Data;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace GreenField.Gadgets.Helpers
{
    public class HarmonicMeanCalculation : EnumerableAggregateFunction
    {
        protected override string AggregateMethodName
        {
            get
            {
                return "HarmonicMeanCalculationMethod";
            }
        }

        protected override Type ExtensionMethodsType
        {
            get
            {
                return typeof(HarmonicMean);
            }
        }
    }

    public class HarmonicMean
    {
        public static double HarmonicMeanCalculationMethod<T>(IEnumerable<MyDataRow> source)
        {

            //double sum1 = 0;
            //double sum2 = 0;
            //foreach (Item item in source)
            //{
            //    sum1 += item.Value1;
            //    sum2 += item.Value2;
            //}

            return 100;
        }
    }
}
