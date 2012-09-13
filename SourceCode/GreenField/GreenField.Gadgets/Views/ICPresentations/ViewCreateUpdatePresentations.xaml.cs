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
using System.ComponentModel.Composition;
using GreenField.Gadgets.ViewModels;
using GreenField.Gadgets.Helpers;
using GreenField.ServiceCaller;
using System.IO;
using GreenField.ServiceCaller.MeetingDefinitions;
using GreenField.Common;
using GreenField.DataContracts;

namespace GreenField.Gadgets.Views
{
    public partial class ViewCreateUpdatePresentations : ViewBaseUserControl
    {        
        #region Properties
        /// <summary>
        /// property to set data context
        /// </summary>
        private ViewModelCreateUpdatePresentations _dataContextViewModelCreateUpdatePresentations;
        public ViewModelCreateUpdatePresentations DataContextViewModelCreateUpdatePresentations
        {
            get { return _dataContextViewModelCreateUpdatePresentations; }
            set { _dataContextViewModelCreateUpdatePresentations = value; }
        }



        /// <summary>
        /// property to set IsActive variable of View Model
        /// </summary>
        private bool _isActive;
        public override bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
                if (DataContextViewModelCreateUpdatePresentations != null) //DataContext instance
                    DataContextViewModelCreateUpdatePresentations.IsActive = _isActive;
            }
        }
        #endregion        

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataContextSource"></param>
        public ViewCreateUpdatePresentations(ViewModelCreateUpdatePresentations dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextViewModelCreateUpdatePresentations = dataContextSource;
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

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            String filter = "All Files (*.*)|*.*";
            if (DataContextViewModelCreateUpdatePresentations.SelectedUploadDocumentInfo == UploadDocumentType.POWERPOINT_PRESENTATION)
            {
                filter = "PowerPoint Presentation (*.pptx)|*.pptx";
            }
            else if (DataContextViewModelCreateUpdatePresentations.SelectedUploadDocumentInfo == UploadDocumentType.ADDITIONAL_ATTACHMENT)
            {
                filter = "PDF (*.pdf)|*.pdf|JPEG Picture (*.jpeg)|*.ppt";
            }
            else
            {
                filter = "PDF (*.pdf)|*.pdf";
            }

            
            OpenFileDialog dialog = new OpenFileDialog() { Multiselect = false, Filter = filter };            
            if (dialog.ShowDialog() == true)
            {
                if (DataContextViewModelCreateUpdatePresentations.SelectedUploadDocumentInfo == UploadDocumentType.POWERPOINT_PRESENTATION)
                {
                    if (dialog.File.Extension !=".pptx")
                        return;
                }
                else if (DataContextViewModelCreateUpdatePresentations.SelectedUploadDocumentInfo == UploadDocumentType.ADDITIONAL_ATTACHMENT)
                {
                    if (dialog.File.Extension != ".pdf" || dialog.File.Extension != ".jpeg")
                        return;                    
                }
                else
                {
                    if (dialog.File.Extension != ".pdf")
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

        private Byte[] ReadFully(Stream input)
        {
            Byte[] buffer = new byte[16 * 1024];

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

        
    }
}
