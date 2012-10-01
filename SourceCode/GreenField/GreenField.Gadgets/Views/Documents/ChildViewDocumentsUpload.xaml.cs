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
using System.Text;
using System.Linq;
using System.Windows.Markup;

namespace GreenField.Gadgets.Views.Documents
{
    public partial class ChildViewDocumentsUpload : ChildWindow
    {

        IDBInteractivity _dBInteractivity;
        ILoggerFacade _logger;

        public ChildViewDocumentsUpload(IDBInteractivity dBInteractivity, ILoggerFacade logger, List<string> companyInfo, List<MembershipUserInfo> userInfo)
        {
            _dBInteractivity = dBInteractivity;
            _logger = logger;

            InitializeComponent();

            this.cbCompany.ItemsSource = companyInfo;
            this.cbCompany.SelectedIndex = 0;

            this.cbType.ItemsSource = CategoryType;
            this.cbType.SelectedIndex = 0;
            this.OKButton.IsEnabled = false;

            this.cbAlert.ItemsSource = userInfo;
            //StringBuilder itemTemplate = new StringBuilder();
            //itemTemplate.Append("<DataTemplate");
            //itemTemplate.Append(" xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'");
            //itemTemplate.Append(" xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'");
            //itemTemplate.Append(" xmlns:lanes='clr-namespace:GreenField.Gadgets.Helpers;assembly=GreenField.Gadgets'");
            //itemTemplate.Append(" xmlns:local='clr-namespace:GreenField.Gadgets.Views;assembly=GreenField.Gadgets'>");
            //itemTemplate.Append("<CheckBox>");
            //itemTemplate.Append("   <CheckBox.Content>");
            //itemTemplate.Append("       <Binding Path='UserName'/>");
            //itemTemplate.Append("   </CheckBox.Content>");
            //itemTemplate.Append("</CheckBox>");            
            //itemTemplate.Append("</DataTemplate>");
            //this.cbAlert.ItemTemplate = XamlReader.Load(itemTemplate.ToString()) as DataTemplate;

            this.cbAlert.DisplayMemberPath = "UserName";

        }

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

        public Byte[] UploadFileByteStream { get; set; }
        public String UploadFileName { get; set; }
        public String UploadFileNotes { get; set; }
        public String UploadFileTags { get; set; }
        public String UploadFileCompanyInfo { get; set; }
        public DocumentCategoryType UploadFileType { get; set; }
        public List<String> UserAlertEmails { get; set; }

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            UploadFileTags = this.tbTags.Text;
            UploadFileNotes = this.tbNotes.Text;

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
            UploadFileCompanyInfo = this.cbCompany.SelectedItem as String;
            ValidateSubmission();
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
            //this.tboxFileName.Text = null;
            //UploadFileByteStream = null;           
            //UploadFileName = null;       
        }

        private void cbType_SelectionChanged(object sender, Telerik.Windows.Controls.SelectionChangedEventArgs e)
        {
            UploadFileType = (DocumentCategoryType)this.cbType.SelectedItem;
            ValidateSubmission();
        }

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
    }
}

