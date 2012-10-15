using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Practices.Prism.Logging;
using GreenField.Gadgets.ViewModels;
using GreenField.ServiceCaller;

namespace GreenField.Gadgets.Views.Documents
{
    /// <summary>
    /// Code behind for ChildViewDocumentsEditDelete
    /// </summary>
    public partial class ChildViewDocumentsEditDelete : ChildWindow
    {
        #region Properties
        /// <summary>
        /// Stores datacontext instance
        /// </summary>
        public ChildViewModelDocumentsEditDelete ChildViewDocumentsEditDeleteDataContext { get; set; } 
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dBInteractivity">IDBInteractivity</param>
        /// <param name="logger">ILoggerFacade</param>
        /// <param name="companyInfo">Issuer information</param>
        public ChildViewDocumentsEditDelete(IDBInteractivity dBInteractivity, ILoggerFacade logger, List<String> companyInfo)
        {
            InitializeComponent();
            ChildViewDocumentsEditDeleteDataContext = new ChildViewModelDocumentsEditDelete(dBInteractivity, logger, companyInfo);
            this.DataContext = ChildViewDocumentsEditDeleteDataContext;

        } 
        #endregion

        #region Event Handlers
        /// <summary>
        /// btnBrowse Click EventHandler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">RoutedEventArgs</param>
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

        /// <summary>
        /// btnCancel Click EventHandler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">RoutedEventArgs</param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.tboxFileName.Text = String.Empty;
            ChildViewDocumentsEditDeleteDataContext.UploadStream = null;
        }

        /// <summary>
        /// cbUserFile SelectionChanged EventHandler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">SelectionChangedEventArgs</param>
        private void cbUserFile_SelectionChanged(object sender, Telerik.Windows.Controls.SelectionChangedEventArgs e)
        {
            this.tboxFileName.Text = String.Empty;
            ChildViewDocumentsEditDeleteDataContext.UploadStream = null;
        } 
        #endregion

        #region Helper Methods
        /// <summary>
        /// Reads stream and returns byte array
        /// </summary>
        /// <param name="fileName">fileName</param>
        /// <param name="fileStream">stream</param>
        /// <returns>byte array</returns>
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
        #endregion
    }
}