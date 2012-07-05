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
using GreenField.Common;
using GreenField.DataContracts;

namespace GreenField.Gadgets.ViewModels
{
    public class ViewModelScatterGraph
    {
        public ViewModelScatterGraph(DashboardGadgetParam param)
        {
            Int32? dataId = RatioPeriodMapping.GetEstimationId(ScatterGraphFinancialRatio.NET_DEBT_TO_EQUITY, ScatterGraphPeriod.TRAILING);

        }
    }
}
