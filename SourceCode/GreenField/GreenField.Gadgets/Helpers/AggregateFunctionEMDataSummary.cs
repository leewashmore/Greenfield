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
using GreenField.DataContracts.DataContracts;
using System.Collections.Generic;

namespace GreenField.Gadgets.Helpers
{
    public class AggregateFunctionEMDataSummary : EnumerableSelectorAggregateFunction
    {
        protected override string AggregateMethodName
        {
            get { return "GetAggregate"; }
        }

        protected override Type ExtensionMethodsType
        {
            get { return typeof(MyAggregates); }
        }

        public override string ResultFormatString
        {
            get { return "{0:n1}%"; }
        }
    }

    public class MyAggregates
    {
        public static decimal? GetAggregate<TSource>(IEnumerable<EMSummaryMarketData> source, Func<EMSummaryMarketData, decimal?> selector)
        {
            List<EMSummaryMarketData> dataSource = source.ToList();
            List<decimal?> dataSelector = source.Select(selector).ToList();

            decimal? sum = 0;
            decimal? regionalBenchmarkAggregate = 0;

            for (int i = 0; i < dataSource.Count(); i++)
            {
                if (dataSource[i].BenchmarkWeight != null)
                {
                    regionalBenchmarkAggregate += dataSource[i].BenchmarkWeight;
                    if (dataSelector[i] != null)
                    {
                        sum += dataSource[i].BenchmarkWeight * dataSelector[i];
                    }
                }
            }
            if (regionalBenchmarkAggregate != 0)
            {
                return sum / regionalBenchmarkAggregate;
            }
            else
            {
                return 0;
            }
        }
    }
}
