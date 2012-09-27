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
    }
}
