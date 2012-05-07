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
using System.ComponentModel.Composition;
using System.Collections.Generic;
using GreenField.ServiceCaller.PerformanceDefinitions;
using System.Linq;

namespace GreenField.ServiceCaller
{
    [Export(typeof(IPerformanceServiceCaller))]
    public class PerformanceServiceCaller : IPerformanceServiceCaller
    {
        

    }
}
