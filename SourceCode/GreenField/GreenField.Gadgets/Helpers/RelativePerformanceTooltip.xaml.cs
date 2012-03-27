using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using GreenField.ServiceCaller.ProxyDataDefinitions;

namespace GreenField.Gadgets.Helpers
{
    public partial class RelativePerformanceTooltip : UserControl
    {
        public RelativePerformanceTooltip(string CountryID, int sectorID)
        {
            InitializeComponent();
            
        }

        public class RelativePerformanceTooltipData
        {
            public List<string> SecurityNames { get; set; }
            public List<string> SecurityAlpha { get; set; }
            public List<string> SecurityActivePositionCountry { get; set; }
            public List<string> SecurityActivePositionSector { get; set; }
        }
    }
}
