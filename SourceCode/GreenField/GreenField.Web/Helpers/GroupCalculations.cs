using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GreenField.Web.Helpers
{
    public class GroupCalculations
    {
        public static decimal? SimpleAverage(decimal? numerator, decimal? denominator)
        {
            if (denominator == 0)
            {
                return 0;
            }
            if (numerator == null || denominator == null)
            {
                return null;
            }
            return numerator / denominator;
        }

        public static decimal? SimpleAverage(List<decimal?> list)
        {
            decimal? denominator = null;
            denominator = list.Where(x => !x.HasValue).Count() == list.Count() ? null : (decimal?)list.Where(x => x.HasValue).Count();
            decimal? numerator = null;
            numerator = list.Where(x => !x.HasValue).Count() == list.Count() ? null : (decimal?)list.Sum();
            if (denominator == 0)
            {
                return 0;
            }
            if (numerator == null || denominator == null)
            {
                return null;
            }
            return numerator / denominator;

        }

        public static decimal? PercentageOwned(List<decimal?> numerator,List<decimal?> denominator)
        {
            decimal? d;
            d = denominator.Where(x => !x.HasValue).Count() == denominator.Count() ? null : (decimal?)denominator.Sum();
            decimal? n;
            n = numerator.Where(x => !x.HasValue).Count() == numerator.Count() ? null : (decimal?)numerator.Sum();
            if (d == 0)
            {
                return 0;
            }
            if (d == null || n == null)
            {
                return null;
            }
            return n / d;

        }

        public static decimal? Median(List<decimal?> list)
        {
            list.Sort();

            int listSize = list.Count;
            decimal? medianValue = null;
            if (listSize > 0)
            {
                if (listSize % 2 == 0) //even
                {
                    int midIndex = listSize / 2;
                    medianValue = ((list.ElementAt(midIndex - 1) + list.ElementAt(midIndex)) / 2);

                }
                else
                {
                    if (listSize > 1)
                    {
                        double element = listSize / 2;
                        element = Math.Round(element, MidpointRounding.AwayFromZero);
                        medianValue = list.ElementAt((int)element);
                        
                    }
                    else
                    {
                        double element = listSize;
                        medianValue = list.ElementAt(0);
                    }
                }
            }
            return medianValue;
        }

        public static decimal? WeightedAverage(List<decimal?> numerator, List<decimal?> product, decimal? totalValue)
        {
            decimal? weightAve ; 

            if (totalValue != null && totalValue != 0)
            {
                weightAve = 0;
                for (int i = 0; i < numerator.Count; i++)
                {
                    weightAve = weightAve + (numerator[i] / totalValue) * product[i];
                }

            }
            else
            {
                weightAve = null;
            }


            return weightAve;
        }
    
    }
}