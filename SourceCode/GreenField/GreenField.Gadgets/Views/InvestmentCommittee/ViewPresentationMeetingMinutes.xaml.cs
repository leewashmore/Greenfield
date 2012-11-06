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
    /// Class behind for ViewPresentationMeetingMinutes
    /// </summary>
    public partial class ViewPresentationMeetingMinutes : ViewBaseUserControl
    {

        #region Properties
        /// <summary>
        /// property to set data context
        /// </summary>
        private ViewModelPresentationMeetingMinutes dataContextViewPresentationMeetingMinutes;
        public ViewModelPresentationMeetingMinutes DataContextViewPresentationMeetingMinutes
        {
            get { return dataContextViewPresentationMeetingMinutes; }
            set { dataContextViewPresentationMeetingMinutes = value; }
        }
        
        /// <summary>
        /// property to set IsActive variable of View Model
        /// </summary>
        private bool isActive;
        public override bool IsActive
        {
            get { return isActive; }
            set
            {
                isActive = value;
                if (DataContextViewPresentationMeetingMinutes != null)
                {
                    DataContextViewPresentationMeetingMinutes.IsActive = isActive;
                }
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataContextSource">ViewModelPresentationMeetingMinutes</param>
        public ViewPresentationMeetingMinutes(ViewModelPresentationMeetingMinutes dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextViewPresentationMeetingMinutes = dataContextSource;
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
            String filter = "All Files (*.*)|*.*";
            OpenFileDialog dialog = new OpenFileDialog() { Multiselect = false, Filter = filter };
            if (dialog.ShowDialog() == true)
            {
                DataContextViewPresentationMeetingMinutes.BusyIndicatorNotification(true, "Reading file...");

                Boolean uploadFileExists = DataContextViewPresentationMeetingMinutes.SelectedUploadDocumentInfo != UploadDocumentType.OTHER_DOCUMENT
                    && DataContextViewPresentationMeetingMinutes.SelectedMeetingDocumentationInfo
                        .Any(record => record.Category.ToUpper() == DataContextViewPresentationMeetingMinutes.SelectedUploadDocumentInfo.ToUpper());

                if (uploadFileExists)
                {
                    FileMaster uploadDocumentInfo = DataContextViewPresentationMeetingMinutes.SelectedMeetingDocumentationInfo
                    .Where(record => record.Category == DataContextViewPresentationMeetingMinutes.SelectedUploadDocumentInfo).FirstOrDefault();

                    if (uploadDocumentInfo != null)
                    {
                        DataContextViewPresentationMeetingMinutes.UploadFileData = uploadDocumentInfo;
                    }
                }
                else
                {
                    String securityNames = String.Empty;
                    String securityTickers = String.Empty;
                    String presenters = String.Empty;
                    foreach (MeetingMinuteData item in DataContextViewPresentationMeetingMinutes.ClosedForVotingMeetingMinuteDistinctPresentationInfo)
                    {
                        securityNames += item.SecurityName + ";";
                        securityTickers += item.SecurityTicker + ";";
                        presenters += item.Presenter + ";";
                    }
                    securityNames = securityNames != String.Empty ? securityNames.Substring(0, securityNames.Length - 1) : securityNames;
                    securityTickers = securityTickers != String.Empty ? securityTickers.Substring(0, securityTickers.Length - 1) : securityTickers;
                    presenters = presenters != String.Empty ? presenters.Substring(0, presenters.Length - 1) : presenters;

                    FileMaster presentationAttachedFileData = new FileMaster()
                    {
                        Name = dialog.File.Name,
                        SecurityName = securityNames,
                        SecurityTicker = securityTickers,
                        MetaTags = DataContextViewPresentationMeetingMinutes.SelectedClosedForVotingMeetingInfo
                            .MeetingDateTime.ToString("yyyy-MM-dd") + ";" + presenters,
                        Type = EnumUtils.GetDescriptionFromEnumValue<DocumentCategoryType>(DocumentCategoryType.IC_PRESENTATIONS),
                        Category = DataContextViewPresentationMeetingMinutes.SelectedUploadDocumentInfo
                    };
                    DataContextViewPresentationMeetingMinutes.UploadFileData = presentationAttachedFileData;
                }
                FileStream fileStream = dialog.File.OpenRead();
                DataContextViewPresentationMeetingMinutes.UploadFileStreamData = ReadFully(fileStream);
                DataContextViewPresentationMeetingMinutes.SelectedUploadFileName = dialog.File.Name;
                fileStream.Dispose();

                DataContextViewPresentationMeetingMinutes.BusyIndicatorNotification();
            }
        }

        /// <summary>
        /// btnPreview Click EventHandler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">RoutedEventArgs</param>
        private void btnPreview_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog() { Filter = "PDF (*.pdf) |*.pdf" };
            if (dialog.ShowDialog() == true)
            {
                DataContextViewPresentationMeetingMinutes.DownloadStream = dialog.OpenFile();
            }
        } 
        #endregion

        #region Helper Methods
        /// <summary>
        /// Converts Stream file to Byte array
        /// </summary>
        /// <param name="input">stream file</param>
        /// <returns>byte array</returns>
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
        #endregion

        #region Dispose Method
        /// <summary>
        /// method to dispose all running events
        /// </summary>
        public override void Dispose()
        {
            this.DataContextViewPresentationMeetingMinutes.Dispose();
            this.DataContextViewPresentationMeetingMinutes = null;
            this.DataContext = null;
        }
        #endregion        
    }
}