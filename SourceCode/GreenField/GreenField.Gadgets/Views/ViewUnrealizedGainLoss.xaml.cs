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
using System.IO;
using GreenField.Common;

namespace GreenField.Gadgets.Views
{
    public partial class ViewUnrealizedGainLoss : UserControl
    {
        private static class ExportTypes
        {
            public const string UNREALIZED_GAINLOSS_CHART = "Unrealized Gain/Loss Chart";
            public const string UNREALIZED_GAINLOSS_DATA = "Unrealized Gain/Loss Data";
        }

        public ViewUnrealizedGainLoss(ViewModelUnrealizedGainLoss dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            dataContextSource.unrealizedGainLossDataLoadedEvent +=
                new DataRetrievalProgressIndicator(dataContextSource_unrealizedGainLossDataLoadedEvent);
            dataContextSource.ChartArea = this.chUnrealizedGainLoss.DefaultView.ChartArea;
            this.chUnrealizedGainLoss.DataBound += dataContextSource.ChartDataBound;
            dgUnrealizedGainLoss.Visibility = Visibility.Collapsed;
        }

        void dataContextSource_unrealizedGainLossDataLoadedEvent(DataRetrievalProgressIndicatorEventArgs e)
        {
            if (e.ShowBusy)
                this.busyIndicator.IsBusy = true;
            else
                this.busyIndicator.IsBusy = false;
        }

        /// <summary>
        /// Flipping between Grid & Chart
        /// Using the method FlipItem in class Flipper.cs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFlip_Click(object sender, RoutedEventArgs e)
        {
            if (this.grdRadGridView.Visibility == Visibility.Visible)
                Flipper.FlipItem(this.grdRadGridView, this.grdRadChart);
            else
                Flipper.FlipItem(this.grdRadChart, this.grdRadGridView);

        }


        /// <summary>
        /// Method to catch Click Event of Export to Excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExportExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<RadExportOptions> RadExportOptionsInfo = new List<RadExportOptions>
                {
                    new RadExportOptions() { ElementName = ExportTypes.UNREALIZED_GAINLOSS_DATA, Element = this.dgUnrealizedGainLoss, ExportFilterOption = RadExportFilterOptions.RADGRIDVIEW_EXPORT_FILTER },
                    new RadExportOptions() { ElementName = ExportTypes.UNREALIZED_GAINLOSS_CHART, Element = this.chUnrealizedGainLoss, ExportFilterOption = RadExportFilterOptions.RADCHART_EXPORT_FILTER },                    
                    
                };
                ChildExportOptions childExportOptions = new ChildExportOptions(RadExportOptionsInfo, "Export Options: " + GadgetNames.UNREALIZED_GAINLOSS);
                childExportOptions.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


    }
}
