using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Practices.Prism.Logging;
using GreenField.Common;
using GreenField.DataContracts;
using GreenField.ServiceCaller;

namespace GreenField.Gadgets.Views.Documents
{
    /// <summary>
    /// Code behind for ChildViewDocumentsUpload
    /// </summary>
    public partial class ChildViewDocumentsUpload : ChildWindow
    {
        #region Fields
        /// <summary>
        /// Service caller
        /// </summary>
        IDBInteractivity dBInteractivity;

        /// <summary>
        /// Loggerfacade instance
        /// </summary>
        ILoggerFacade logger; 
        #endregion

        #region Properties
        /// <summary>
        /// upload file byte stream
        /// </summary>
        public Byte[] UploadFileByteStream { get; set; }

        /// <summary>
        /// upload file name
        /// </summary>
        public String UploadFileName { get; set; }

        /// <summary>
        /// upload file notes
        /// </summary>
        public String UploadFileNotes { get; set; }

        /// <summary>
        /// upload file metatags
        /// </summary>
        public String UploadFileTags { get; set; }

        /// <summary>
        /// upload file issuer name
        /// </summary>
        public String UploadFileCompanyInfo { get; set; }

        /// <summary>
        /// upload file category type
        /// </summary>
        public DocumentCategoryType UploadFileType { get; set; }

        /// <summary>
        /// user alert email addresses
        /// </summary>
        public List<String> UserAlertEmails { get; set; }

        /// <summary>
        /// Category reference type
        /// </summary>
        private List<DocumentCategoryType> _categoryType;
        public List<DocumentCategoryType> CategoryType
        {
            get
            {
                if (_categoryType == null)
                {
                    _categoryType = EnumUtils.GetEnumDescriptions<DocumentCategoryType>();
                    _categoryType.Remove(DocumentCategoryType.IC_PRESENTATIONS);
                    _categoryType.Remove(DocumentCategoryType.MODELS);
                    _categoryType.Remove(DocumentCategoryType.BLOG);
                }
                return _categoryType;
            }
        } 
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dBInteractivity">IDBInteractivity</param>
        /// <param name="logger">ILoggerFacade</param>
        /// <param name="companyInfo">Issuer names</param>
        /// <param name="userInfo">user information</param>
        public ChildViewDocumentsUpload(IDBInteractivity dBInteractivity, ILoggerFacade logger, List<string> companyInfo
            , List<MembershipUserInfo> userInfo)
        {
            this.dBInteractivity = dBInteractivity;
            this.logger = logger;

            InitializeComponent();

            this.cbCompany.ItemsSource = companyInfo;
            this.cbCompany.SelectedIndex = 0;

            this.cbType.ItemsSource = CategoryType;
            this.cbType.SelectedIndex = 0;
            this.OKButton.IsEnabled = false;

            this.cbAlert.ItemsSource = userInfo;
            this.cbAlert.DisplayMemberPath = "UserName";
        } 
        #endregion

        #region Event Handlers
        /// <summary>
        /// OKButton Click EventHandler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">RoutedEventArgs</param>
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            UploadFileTags = this.tbTags.Text;
            UploadFileNotes = this.tbNotes.Text;
            this.DialogResult = true;
        }

        /// <summary>
        /// CancelButton Click EventHandler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">RoutedEventArgs</param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        /// <summary>
        /// btnBrowse Click EventHandler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">RoutedEventArgs</param>
        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog() { Multiselect = false };
            if (dialog.ShowDialog() == true)
            {
                this.tboxFileName.Text = dialog.File.Name;
                UploadFileName = dialog.File.Name;
                UploadFileByteStream = FileToByteArray(UploadFileName, dialog.File.OpenRead());
                ValidateSubmission();
            }
        }

        /// <summary>
        /// cbCompany SelectionChanged EventHandler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">SelectionChangedEventArgs</param>
        private void cbCompany_SelectionChanged(object sender, Telerik.Windows.Controls.SelectionChangedEventArgs e)
        {
            UploadFileCompanyInfo = this.cbCompany.SelectedItem as String;
            ValidateSubmission();
        }
        
        /// <summary>
        /// cbType SelectionChanged EventHandler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">SelectionChangedEventArgs</param>
        private void cbType_SelectionChanged(object sender, Telerik.Windows.Controls.SelectionChangedEventArgs e)
        {
            UploadFileType = (DocumentCategoryType)this.cbType.SelectedItem;
            ValidateSubmission();
        }

        /// <summary>
        /// cbAlert SelectionChanged EventHandler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">SelectionChangedEventArgs</param>
        private void cbAlert_SelectionChanged(object sender, Telerik.Windows.Controls.SelectionChangedEventArgs e)
        {
            List<String> userArray = new List<String>();
            UserAlertEmails = new List<String>();
            foreach (MembershipUserInfo item in this.cbAlert.SelectedItems)
            {
                userArray.Add(item.UserName);
                UserAlertEmails.Add(item.Email);
            }
            this.txtAlertUsers.Text = String.Join(", ", userArray.ToArray());
        } 
        #endregion

        #region Helper Methods
        /// <summary>
        /// Validates input submission
        /// </summary>
        private void ValidateSubmission()
        {
            this.OKButton.IsEnabled = UploadFileByteStream != null &&
                UploadFileName != null &&
                UploadFileCompanyInfo != null;
        }
        
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

