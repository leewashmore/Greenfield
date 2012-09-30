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
using GreenField.Gadgets.Helpers;
using GreenField.Common;
using GreenField.Gadgets.ViewModels;
using System.IO;
using GreenField.ServiceCaller;

namespace GreenField.Gadgets.Views
{
    /// <summary>
    /// Code-Behind for ViewExcelModelUpload
    /// </summary>
    public partial class ViewExcelModelUpload : ViewBaseUserControl
    {
        private ViewModelExcelModelUpload dataContextSource;
        public ViewModelExcelModelUpload DataContextSource
        {
            get { return dataContextSource; }
            set { dataContextSource = value; }
        }


        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public ViewExcelModelUpload(ViewModelExcelModelUpload dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextSource = dataContextSource;
        }

        #endregion

        /// <summary>
        /// Button Click Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDownload_Click(object sender, RoutedEventArgs e)
        {
            byte[] fileBytes = this.DataContextSource.ModelWorkbook;

            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Execl files (*.xls)|*.xls";
            bool? dialogresult = dialog.ShowDialog();

            using (Stream fs = (Stream)dialog.OpenFile())
            {
                fs.Write(fileBytes, 0, fileBytes.Length);
                fs.Close();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bOpenFileDialog_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog1 = new OpenFileDialog();
                openFileDialog1.Filter = "Excel Files (.xls)|*.xls|All Files (*.*)|*.*";
                openFileDialog1.FilterIndex = 1;
                openFileDialog1.Multiselect = true;
                bool? userClickedOK = openFileDialog1.ShowDialog();
                if (userClickedOK == true)
                {
                    FileStream fileStream;
                    byte[] fileByte;
                    using (fileStream = openFileDialog1.File.OpenRead())
                    {
                        fileByte = new byte[fileStream.Length];
                        fileStream.Read(fileByte, 0, Convert.ToInt32(fileStream.Length));
                    }
                    this.DataContextSource.UploadWorkbook = fileByte;
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message, "Exception", MessageBoxButton.OK);
                Logging.LogException(this.DataContextSource.Logger, ex);
            }
        }

        /// <summary>
        /// Generate byte-Array from Excel
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private static byte[] GetBytsForFile(string filePath)
        {
            try
            {
                FileStream fileStream;
                byte[] fileByte;
                using (fileStream = File.OpenRead(filePath))
                {
                    fileByte = new byte[fileStream.Length];
                    fileStream.Read(fileByte, 0, Convert.ToInt32(fileStream.Length));
                }
                return fileByte;
            }
            catch (Exception ex)
            {
                //ExceptionTrace.LogException(ex);
                return null;
            }
        }

    }
}
