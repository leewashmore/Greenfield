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
using GreenField.DataContracts;
using GreenField.Common;
using GreenField.ServiceCaller;
using GreenField.ServiceCaller.ExternalResearchDefinitions;
using Microsoft.Practices.Prism.Logging;
using System.IO;
using GreenField.Gadgets.ViewModels;

namespace GreenField.Gadgets.Views.Documents
{
    public partial class ChildViewDocumentsEditDelete : ChildWindow
    {
        public ChildViewModelDocumentsEditDelete ChildViewDocumentsEditDeleteDataContext { get; set; }

        public ChildViewDocumentsEditDelete(IDBInteractivity dBInteractivity, ILoggerFacade logger, List<String> companyInfo)
        {
            InitializeComponent();            
            ChildViewDocumentsEditDeleteDataContext = new ChildViewModelDocumentsEditDelete(dBInteractivity, logger, companyInfo);
            this.DataContext = ChildViewDocumentsEditDeleteDataContext;
            
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == true)
            {
                ChildViewDocumentsEditDeleteDataContext.UploadStream = FileToByteArray(dialog.File.Name, dialog.File.OpenRead());
                ChildViewDocumentsEditDeleteDataContext.UploadFileName = dialog.File.Name;
                this.tboxFileName.Text = dialog.File.Name;
            }
        }

        private Byte[] FileToByteArray(String fileName, FileStream fileStream)
        {
            Byte[] buffer = null;
            try
            {
                buffer = new Byte[fileStream.Length];
                Int32 bufferReadChar = fileStream.Read(buffer, 0, Convert.ToInt32(fileStream.Length));
                fileStream.Close();
                fileStream.Dispose();

            }
            catch (Exception)
            {
                throw;
            }
            return buffer;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.tboxFileName.Text = String.Empty;
            ChildViewDocumentsEditDeleteDataContext.UploadStream = null;
        }

        private void cbUserFile_SelectionChanged(object sender, Telerik.Windows.Controls.SelectionChangedEventArgs e)
        {
            this.tboxFileName.Text = String.Empty;
            ChildViewDocumentsEditDeleteDataContext.UploadStream = null;
        }
    }
}

