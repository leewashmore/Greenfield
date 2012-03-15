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
    public partial class ViewClosingPriceChart : UserControl
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ViewClosingPriceChart(ViewModelClosingPriceChart DataContextSource)
        {
            InitializeComponent();
            this.DataContext = DataContextSource;
        }

        /// <summary>
        /// Flipping between Grid & Chart
        /// Using the method FlipItem in class Flipper.cs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTFlip_Click(object sender, RoutedEventArgs e)
        {
            UIElement elementOver = this.dgPricing.Visibility == System.Windows.Visibility.Visible ? (UIElement)this.dgPricing : (UIElement)this.chPricing;
            UIElement elementUnder = elementOver == (UIElement)this.chPricing ? (UIElement)this.dgPricing : (UIElement)this.chPricing;
            Flipper.FlipItem(elementOver, elementUnder);
        }

        private void btnBFlip_Click(object sender, RoutedEventArgs e)
        {
            UIElement elementOver = this.dgVolume.Visibility == System.Windows.Visibility.Visible ? (UIElement)this.dgVolume : (UIElement)this.chVolume;
            UIElement elementUnder = elementOver == (UIElement)this.chVolume ? (UIElement)this.dgVolume : (UIElement)this.chVolume;
            Flipper.FlipItem(elementOver, elementUnder);
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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
                //closingPriceChart.ExportToExcelML(fileStream);
                //volumeChart.ExportToExcelML(fileStream);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
