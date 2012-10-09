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
using System.Reflection;
using GreenField.Gadgets.Models;

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
        public static Decimal? HarmonicMeanCalculationMethod<TSource, TResult>(IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
                       
            Decimal totalweight = 0;
            Decimal totalharmonic = 0;
            Decimal amount = 0
           
            List<MyDataRow> sourceData = source.Cast<MyDataRow>().ToList();
            List<string> marketCapList = new List<string>();

            for (int i = 0; i < sourceData.Count; i++)
            {                
                string marketCap = sourceData[i]["Market Capitalization"].ToString();
                marketCapList.Add(marketCap);
            }

            List<String> numericvalues = new List<string>();
            IEnumerable<string> values = from i in source
                                         select Convert.ToString(selector(i)).ToLower();
            foreach (String strValue in values)
            {
                numericvalues.Add(strValue);
            }                        

            for (int i = 0; i < numericvalues.Count(); i++) 
            {
                if (marketCapList[i] != null && numericvalues[i] != null && marketCapList[i] != String.Empty && numericvalues[i] != String.Empty)
                {
                    decimal marketCap = 0;
                    marketCap = decimal.TryParse(marketCapList[i],out marketCap) ? marketCap : 0;
                    totalweight = totalweight + Convert.ToDecimal(marketCap);
                }
            }       

            for (int i = 0; i < numericvalues.Count(); i++)
            {
                if (marketCapList[i] != null && numericvalues[i] != null)
                {
                    if (totalweight != 0 && numericvalues[i] != String.Empty && marketCapList[i] != String.Empty)
                    {
                        decimal marketCap = 0;
                        decimal.TryParse(marketCapList[i], out marketCap);
                        Decimal weight = Convert.ToDecimal(marketCap) / totalweight;

                        Decimal measureValue = 0;
                        measureValue = decimal.TryParse(numericvalues[i], out measureValue) ? measureValue : 1;

                        if (measureValue != 0)
                        {
                            amount = 1 / Convert.ToDecimal(measureValue);
                        }
                        
                        Decimal inverseAmount = weight * amount;
                        totalharmonic = totalharmonic + inverseAmount;
                    }
                }
            }

            if (totalharmonic != 0)
            {
                return Math.Round((1 / totalharmonic),2);
            }
            else
            {
                return 0;
            }         

        }
    }
}
