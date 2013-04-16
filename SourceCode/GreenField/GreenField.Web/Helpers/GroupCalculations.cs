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
    }
}