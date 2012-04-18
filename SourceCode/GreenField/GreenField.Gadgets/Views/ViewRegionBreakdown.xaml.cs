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
using GreenField.Gadgets.Helpers;
using Telerik.Windows.Data;
using System.ComponentModel;

namespace GreenField.Gadgets.Views
{
    public partial class ViewRegionBreakdown : ViewBaseUserControl
    {
        public ViewRegionBreakdown(ViewModelRegionBreakdown dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
        }

        /// <summary>
        /// Flipping between Grid & PieChart
        /// Using the method FlipItem in class Flipper.cs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFlip_Click(object sender, RoutedEventArgs e)
        {
            if (this.crtRegionBreakdown.Visibility == System.Windows.Visibility.Visible)
            {
                Flipper.FlipItem(this.crtRegionBreakdown, this.dgRegionBreakdown);
            }
            else
            {
                Flipper.FlipItem(this.dgRegionBreakdown, this.crtRegionBreakdown);
            }
        }

        public override void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
