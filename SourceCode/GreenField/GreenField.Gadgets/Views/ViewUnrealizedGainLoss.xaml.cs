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
            dgUnrealizedGainLoss.Visibility = Visibility.Collapsed;            
        }

        /// <summary>
        /// Flipping between Grid & Chart
        /// Using the method FlipItem in class Flipper.cs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTFlip_Click(object sender, RoutedEventArgs e)
        {
            if (dgUnrealizedGainLoss.Visibility == Visibility.Collapsed)
                Flipper.FlipItem(chUnrealizedGainLoss, dgUnrealizedGainLoss);
            else
                Flipper.FlipItem(dgUnrealizedGainLoss, chUnrealizedGainLoss);

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
                chUnrealizedGainLoss.ExportToExcelML(fileStream);                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
