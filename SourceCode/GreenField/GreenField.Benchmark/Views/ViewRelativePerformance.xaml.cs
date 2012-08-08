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
using System.ComponentModel.Composition;
using GreenField.Benchmark.ViewModels;

namespace GreenField.Benchmark.Views
{
    [Export]
    public partial class ViewRelativePerformance : UserControl
    {
        public ViewRelativePerformance()
        {
            InitializeComponent();
        }

        [Import]
        public ViewModelRelativePerformance DataContextSource
        {
            set
            {
                this.DataContext = value;
            }
        }
    }
}
