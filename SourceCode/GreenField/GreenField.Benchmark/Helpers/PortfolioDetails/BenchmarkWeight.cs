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

namespace GreenField.Benchmark.Helpers
{
    public class BenchmarkWeightFunction : EnumerableSelectorAggregateFunction
    {
        protected override string AggregateMethodName
        {
            get
            {
                return "StdDev";
            }
        }

        protected override Type ExtensionMethodsType
        {
            get
            {
                return typeof(Statistics);
            }
        }
    }
}
