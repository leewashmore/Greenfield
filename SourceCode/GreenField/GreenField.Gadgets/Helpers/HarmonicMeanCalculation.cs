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
using System.Linq;

namespace GreenField.Gadgets.Helpers
{
    public class HarmonicMeanCalculation : EnumerableSelectorAggregateFunction
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
        public static double HarmonicMeanCalculationMethod<TSource, TResult>(IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            List<MyDataRow> sourceData = source.Cast<MyDataRow>().ToList();
            List<string> marketCapList = new List<string>();

            for (int i = 0; i < sourceData.Count; i++)
            {
                string marketCap = sourceData[0]["Market Capitalization"].ToString();
                marketCapList.Add(marketCap);
            }

            List<String> numericvalues = new List<string>();
            IEnumerable<string> values = from i in source 
                                         select Convert.ToString(selector(i)).ToLower();
            foreach (String strValue in values)
            {
                numericvalues.Add(strValue);
            }

            return 100;
        }
    }
}
