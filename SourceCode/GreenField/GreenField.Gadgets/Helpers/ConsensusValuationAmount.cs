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
    /// <summary>
    /// Class to calculate Amount in Consensus Gadgets
    /// </summary>
    public class ConsensusValuationAmount : EnumerableSelectorAggregateFunction
    {
        /// <summary>
        /// Method name
        /// </summary>
        protected override string AggregateMethodName
        {
            get
            {
                return "ConsensusValuationAmountMethod";
            }
        }

        /// <summary>
        /// Extension Method type
        /// </summary>
        protected override Type ExtensionMethodsType
        {
            get
            {
                return typeof(ConsensusValuationData);
            }
        }
    }

    /// <summary>
    /// Consensus Valuation Amount
    /// </summary>
    public class ConsensusValuationData
    {
        /// <summary>
        /// Grouping Aggregate Calculator
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static string ConsensusValuationAmountMethod<TSource, TResult>(IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            string returnVal = null;
            Decimal amount = 0;
            List<string> values = (from i in source
                                   select Convert.ToString(selector(i)).ToLower()).ToList();

            if (values != null && values.Count > 0)
            {
                amount = decimal.TryParse(values[0], out amount) ? amount : 0;
            }

            returnVal = amount == 0 ? null : Convert.ToDecimal(amount).ToString("N2");

            return returnVal;
        }
    }
}
