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

namespace GreenField.Gadgets.Views
{
    public partial class ViewUnrealizedGainLoss : UserControl
    {
        public ViewUnrealizedGainLoss(ViewModelUnrealizedGainLoss dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            priceDataGrid.Visibility = Visibility.Collapsed;
            //volumeDataGrid.Visibility = Visibility.Collapsed;
            //cbTime.SelectedItem = "1-Year";
            //cbFrequency.SelectedItem = "Daily";
        }

        /// <summary>
        /// Flipping between Grid & Chart
        /// Using the method FlipItem in class Flipper.cs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void btnFlipforGraph_Click(object sender, RoutedEventArgs e)
        {
            //if (volumeDataGrid.Visibility == Visibility.Collapsed)
            //    Flipper.FlipItem(volumeChart, volumeDataGrid);
            //else
            //    Flipper.FlipItem(volumeDataGrid, volumeChart);

        }

        private void btnFlipforChart_Click(object sender, RoutedEventArgs e)
        {
            if (priceDataGrid.Visibility == Visibility.Collapsed)
                Flipper.FlipItem(closingPriceChart, priceDataGrid);
            else
                Flipper.FlipItem(priceDataGrid, closingPriceChart);

        }

        private void cbTime_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

            //if (cbTime.SelectedItem != null)
            //{
            //    if (cbTime.SelectedItem.ToString().ToLower() == "custom")
            //    {
            //        lbStartDate.Visibility = Visibility.Visible;
            //        firstDatePicker.Visibility = Visibility.Visible;
            //        lbEndDate.Visibility = Visibility.Visible;
            //        secondDatePicker.Visibility = Visibility.Visible;
            //    }
            //    else
            //    {
            //        lbStartDate.Visibility = Visibility.Collapsed;
            //        firstDatePicker.Visibility = Visibility.Collapsed;
            //        lbEndDate.Visibility = Visibility.Collapsed;
            //        secondDatePicker.Visibility = Visibility.Collapsed;
            //    }
            //}
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
                SaveFileDialog dialog = new SaveFileDialog();
                dialog.DefaultExt = "XLSX";  //Default Format for saving file
                dialog.Filter = this.GetDefaulExt();

                if (!(bool)dialog.ShowDialog())
                    return;

                Stream fileStream = dialog.OpenFile();
                this.ExportTheFile(fileStream);
                fileStream.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private string GetDefaulExt()
        {
            return string.Format("{1} File (*.{0}) | *.{0}", "xlsx", "XLSX");
        }

        /// <summary>
        /// Method writing the stream of chart to an Excel File using ExportToExcelMl method.
        /// </summary>
        /// <param name="fileStream"></param>
        private void ExportTheFile(Stream fileStream)
        {
            try
            {
                closingPriceChart.ExportToExcelML(fileStream);
                //volumeChart.ExportToExcelML(fileStream);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
