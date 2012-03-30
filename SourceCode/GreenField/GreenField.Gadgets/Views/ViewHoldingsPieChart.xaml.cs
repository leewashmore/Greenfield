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
using GreenField.Gadgets.ViewModels;
using Telerik.Windows.Controls.Charting;

namespace GreenField.Gadgets.Views
{
    public partial class ViewHoldingsPieChart : UserControl
    {
        public ViewHoldingsPieChart(ViewModelHoldingsPieChart dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            RadialLabelSettings radialSettings = new RadialLabelSettings();
            radialSettings.SpiderModeEnabled = true;
            radialSettings.ShowConnectors = true;
            DoughnutSeriesDefinition doughtnutSeries = new DoughnutSeriesDefinition();
            doughtnutSeries.LabelSettings = radialSettings;
            this.crtHoldingsPercentageSector.DefaultSeriesDefinition = doughtnutSeries;
            this.crtHoldingsPercentageSector.DefaultView.ChartArea.SmartLabelsEnabled = true;
        }
    }
}
