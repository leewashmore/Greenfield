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

namespace GreenField.Gadgets.Views
{
    public partial class ViewScatterGraph : ViewBaseUserControl
    {
        public ViewScatterGraph(ViewModelScatterGraph dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
        }

        private void btnExportExcel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnFlip_Click(object sender, RoutedEventArgs e)
        {

        }

        private void dgScatterGraph_ElementExporting(object sender, Telerik.Windows.Controls.GridViewElementExportingEventArgs e)
        {

        }

        private void dgScatterGraph_RowLoaded(object sender, Telerik.Windows.Controls.GridView.RowLoadedEventArgs e)
        {

        }

        
    }
}
