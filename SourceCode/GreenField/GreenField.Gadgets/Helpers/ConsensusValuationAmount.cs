using System;
using System.Linq;
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
using GreenField.Gadgets.Models;
using System.Collections.Generic;

namespace GreenField.Gadgets.Helpers
{
    public class ConsensusValuationAmount : EnumerableSelectorAggregateFunction
    {
        protected override string AggregateMethodName
        {
            get
            {
                return "ConsensusValuationAmountMethod";
            }
        }

        protected override Type ExtensionMethodsType
        {
            get
            {
                return typeof(ConsensusValuationData);
            }
        }
    }

    public class ConsensusValuationData
    {
        public static Decimal? ConsensusValuationAmountMethod<TSource, TResult>(IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            decimal? returnVal = null;
            Decimal amount = 0;
            List<string> values = (from i in source
                                         select Convert.ToString(selector(i)).ToLower()).ToList();
            
            if (values != null && values.Count > 0)
            {
                amount = decimal.TryParse(values[0], out amount) ? amount : 0;
            }

            returnVal = amount == 0 ? null : (decimal?)amount;

            return returnVal;
        }
    }
}
