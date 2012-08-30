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

namespace GreenField.Gadgets.Views.Documents
{
    public partial class ChildViewDocumentsUpload : ChildWindow
    {

        IDBInteractivity _dBInteractivity;
        ILoggerFacade _logger;

        public ChildViewDocumentsUpload(IDBInteractivity dBInteractivity, ILoggerFacade logger)
        {
            _dBInteractivity = dBInteractivity;
            _logger = logger;

            InitializeComponent();

            if (_dBInteractivity != null)
            {
                _dBInteractivity.RetrieveCompanyData(RetrieveCompanyDataCallbackMethod);
            }

            CategoryType.Remove(DocumentCategoryType.IC_PRESENTATIONS);
            CategoryType.Remove(DocumentCategoryType.MODELS);
            this.cbType.ItemsSource = CategoryType;
            this.cbType.SelectedIndex = 0;
            this.OKButton.IsEnabled = false;
        }

        public List<DocumentCategoryType> CategoryType
        {
            get { return EnumUtils.GetEnumDescriptions<DocumentCategoryType>(); }
        }

        public Byte[] UploadFileByteStream { get; set; }
        public String UploadFileName { get; set; }
        public String UploadFileNotes { get; set; }
        public String UploadFileTags { get; set; }
        public tblCompanyInfo UploadFileCompanyInfo { get; set; }
        public DocumentCategoryType UploadFileType { get; set; }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            UploadFileNotes = this.tbTags.Text;
            UploadFileTags = this.tbNotes.Text;
            
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

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

        private void cbCompany_SelectionChanged(object sender, Telerik.Windows.Controls.SelectionChangedEventArgs e)
        {
            UploadFileCompanyInfo = this.cbCompany.SelectedItem as tblCompanyInfo;
            ValidateSubmission();
        }

        private void RetrieveCompanyDataCallbackMethod(List<tblCompanyInfo> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    this.cbCompany.ItemsSource = result;
                    this.cbCompany.DisplayMemberPath = "Name";
                    this.cbCompany.SelectedIndex = 0;
                }
                else
                {
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
            }
            Logging.LogEndMethod(_logger, methodNamespace);
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

        private void ValidateSubmission()
        {
            this.OKButton.IsEnabled = UploadFileByteStream != null &&
                UploadFileName != null &&
                UploadFileCompanyInfo != null;
        }

        public void Initialize()
        {
            //UploadFileByteStream = null;
            //UploadFileName = null;            
        }

        private void cbType_SelectionChanged(object sender, Telerik.Windows.Controls.SelectionChangedEventArgs e)
        {
            UploadFileType = (DocumentCategoryType)this.cbType.SelectedItem;
            ValidateSubmission();
        }
    }
}

