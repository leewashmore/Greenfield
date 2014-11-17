using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using GreenField.Common;
using GreenField.DataContracts;
using GreenField.Gadgets.Helpers;
using GreenField.Gadgets.ViewModels;
using GreenField.ServiceCaller.MeetingDefinitions;

namespace GreenField.Gadgets.Views
{
    /// <summary>
    /// Code behind for ViewCreateUpdatePresentations
    /// </summary>
    public partial class ViewCreateUpdatePresentations : ViewBaseUserControl
    {        
        #region Properties
        /// <summary>
        /// Property to set data context
        /// </summary>
        private ViewModelCreateUpdatePresentations dataContextViewModelCreateUpdatePresentations;
        public ViewModelCreateUpdatePresentations DataContextViewModelCreateUpdatePresentations
        {
            get { return dataContextViewModelCreateUpdatePresentations; }
            set { dataContextViewModelCreateUpdatePresentations = value; }
        }

        /// <summary>
        /// Property to set IsActive variable of View Model
        /// </summary>
        private bool isActive;
        public override bool IsActive
        {
            get { return isActive; }
            set
            {
                isActive = value;
                if (DataContextViewModelCreateUpdatePresentations != null)
                {
                    DataContextViewModelCreateUpdatePresentations.IsActive = isActive;
                }
            }
        }
        #endregion        

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataContextSource">ViewModelCreateUpdatePresentations</param>
        public ViewCreateUpdatePresentations(ViewModelCreateUpdatePresentations dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextViewModelCreateUpdatePresentations = dataContextSource;

            if (dataContextSource != null)
            {
                if (dataContextSource.SelectedPresentationOverviewInfo!= null && dataContextSource.SelectedPresentationOverviewInfo.StatusType != StatusType.IN_PROGRESS)
                {
                    HyperlinkButton pphyp = this.PPItemsControl.FindName("PowerPointHyperLink") as HyperlinkButton;
                    pphyp.IsEnabled = false;
                }

            }

          

           
        }
        #endregion

        #region Event Handlers
        /// <summary>
        /// btnBrowse Click event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            String filter = "All Files (*.*)|*.*";
            if (DataContextViewModelCreateUpdatePresentations.SelectedUploadDocumentInfo == UploadDocumentType.PRESENTATION)
            {
                filter = "PDF (*.pdf)|*.pdf";
            }
            else if (DataContextViewModelCreateUpdatePresentations.SelectedUploadDocumentInfo == UploadDocumentType.ADDITIONAL_ATTACHMENT)
            {
                filter = "PDF (*.pdf)|*.pdf|JPEG Picture (*.jpeg, *.jpg)|*.jpeg;*.jpg";
            }
            else
            {
                filter = "PDF (*.pdf)|*.pdf";
            }
            OpenFileDialog dialog = new OpenFileDialog() { Multiselect = false, Filter = filter };
            if (dialog.ShowDialog() == true)
            {
                if (DataContextViewModelCreateUpdatePresentations.SelectedUploadDocumentInfo == UploadDocumentType.PRESENTATION)
                {
                    if (dialog.File.Extension != null && dialog.File.Extension.ToLower()  != ".pdf"  )
                        return;
                }
                else if (DataContextViewModelCreateUpdatePresentations.SelectedUploadDocumentInfo == UploadDocumentType.ADDITIONAL_ATTACHMENT)
                {
                    if (dialog.File.Extension != null && dialog.File.Extension.ToLower() != ".pdf" && dialog.File.Extension.ToLower() != ".jpeg" && dialog.File.Extension.ToLower() != ".jpg")
                        return;
                }
                else
                {
                    if (dialog.File.Extension != null && dialog.File.Extension.ToLower() != ".pdf")
                        return;
                }
                DataContextViewModelCreateUpdatePresentations.BusyIndicatorNotification(true, "Reading file...");

                Boolean uploadFileExists = DataContextViewModelCreateUpdatePresentations.SelectedUploadDocumentInfo != UploadDocumentType.ADDITIONAL_ATTACHMENT
                    && DataContextViewModelCreateUpdatePresentations.SelectedPresentationDocumentationInfo
                        .Any(record => record.Category.ToUpper() == DataContextViewModelCreateUpdatePresentations.SelectedUploadDocumentInfo.ToUpper());

                if (uploadFileExists)
                {
                    FileMaster uploadDocumentInfo = DataContextViewModelCreateUpdatePresentations.SelectedPresentationDocumentationInfo
                    .Where(record => record.Category == DataContextViewModelCreateUpdatePresentations.SelectedUploadDocumentInfo).FirstOrDefault();

                    if (uploadDocumentInfo != null)
                    {
                        DataContextViewModelCreateUpdatePresentations.UploadFileData = uploadDocumentInfo;
                    }
                }
                else
                {
                    FileMaster presentationAttachedFileData = new FileMaster()
                    {
                        Name = dialog.File.Name,
                        SecurityName = DataContextViewModelCreateUpdatePresentations.SelectedPresentationOverviewInfo.SecurityName,
                        SecurityTicker = DataContextViewModelCreateUpdatePresentations.SelectedPresentationOverviewInfo.SecurityTicker,
                        Type = EnumUtils.GetDescriptionFromEnumValue<DocumentCategoryType>(DocumentCategoryType.IC_PRESENTATIONS),
                        Category = DataContextViewModelCreateUpdatePresentations.SelectedUploadDocumentInfo
                    };
                    DataContextViewModelCreateUpdatePresentations.UploadFileData = presentationAttachedFileData;
                }
                FileStream fileStream = dialog.File.OpenRead();
                DataContextViewModelCreateUpdatePresentations.UploadFileStreamData = ReadFully(fileStream);

                DataContextViewModelCreateUpdatePresentations.SelectedUploadFileName = dialog.File.Name;
                fileStream.Dispose();
                DataContextViewModelCreateUpdatePresentations.BusyIndicatorNotification();
            }
        }

        /// <summary>
        /// btnPreview Click event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPreview_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog() { Filter = "PDF (*.pdf) |*.pdf" };
            if (dialog.ShowDialog() == true)
            {
                DataContextViewModelCreateUpdatePresentations.DownloadStream = dialog.OpenFile();
            }
        } 
        #endregion

        #region Helper Methods
        /// <summary>
        /// Reads stream and returns byte array
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private Byte[] ReadFully(Stream input)
        {
            Byte[] buffer = new byte[input.Length];

            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        } 
        #endregion

        #region Dispose Method
        /// <summary>
        /// method to dispose all running events
        /// </summary>
        public override void Dispose()
        {
            this.DataContextViewModelCreateUpdatePresentations.Dispose();
            this.DataContextViewModelCreateUpdatePresentations = null;
            this.DataContext = null;
        }
        #endregion

        private void ICPacketHyperLink_Click(object sender, RoutedEventArgs e)
        {

        }

        private void TextBlock_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {

        }
    }
}
