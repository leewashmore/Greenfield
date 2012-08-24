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
using GreenField.ServiceCaller.MeetingDefinitions;
using System.IO;

namespace GreenField.Gadgets.Views
{
    public partial class ViewPresentationMeetingMinutes : ViewBaseUserControl
    {

        #region Properties
        /// <summary>
        /// property to set data context
        /// </summary>
        private ViewModelPresentationMeetingMinutes _dataContextViewPresentationMeetingMinutes;
        public ViewModelPresentationMeetingMinutes DataContextViewPresentationMeetingMinutes
        {
            get { return _dataContextViewPresentationMeetingMinutes; }
            set { _dataContextViewPresentationMeetingMinutes = value; }
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
                if (DataContextViewPresentationMeetingMinutes != null) //DataContext instance
                    DataContextViewPresentationMeetingMinutes.IsActive = _isActive;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataContextSource"></param>
        public ViewPresentationMeetingMinutes(ViewModelPresentationMeetingMinutes dataContextSource)
        {
            InitializeComponent();
            this.DataContext = dataContextSource;
            this.DataContextViewPresentationMeetingMinutes = dataContextSource;
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

        private void btnBrowseIndustryReport_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog() { Multiselect = false };
            if (dialog.ShowDialog() == true)
            {
                DataContextViewPresentationMeetingMinutes.BusyIndicatorNotification(true, "Uploading file...");
                if (DataContextViewPresentationMeetingMinutes.ClosedForVotingMeetingAttachedFileInfo
                    .Any(record => record.MeetingAttachedFileData.FileName == dialog.File.Name))
                {
                    Prompt.ShowDialog("File '" + dialog.File.Name + "' already exists as an attachment. Please change the name of the file and upload again.");
                    DataContextViewPresentationMeetingMinutes.BusyIndicatorNotification();
                    return;
                }

                MeetingAttachedFileData meetingAttachedFileData
                    = new MeetingAttachedFileData() 
                    {
                        FileName = dialog.File.Name, 
                        FileType = "Industry Report",
                        MeetingID = DataContextViewPresentationMeetingMinutes.SelectedClosedForVotingMeetingInfo.MeetingID
                    };
                
                FileStream fileStream = dialog.File.OpenRead();
                
                DataContextViewPresentationMeetingMinutes.SelectedIndustryReportFileStreamData 
                    = new MeetingAttachedFileStreamData()
                    {
                        MeetingAttachedFileData = meetingAttachedFileData,
                        FileStream = ReadFully(fileStream)
                    };
                
                DataContextViewPresentationMeetingMinutes.SelectedIndustryReports = dialog.File.Name;
                DataContextViewPresentationMeetingMinutes.BusyIndicatorNotification();
            }
        }

        private void btnBrowseOtherDocuments_Click(object sender, RoutedEventArgs e)
        {
            DataContextViewPresentationMeetingMinutes.BusyIndicatorNotification(true, "Uploading file...");
            OpenFileDialog dialog = new OpenFileDialog() { Multiselect = false };
            if (dialog.ShowDialog() == true)
            {
                if (DataContextViewPresentationMeetingMinutes.ClosedForVotingMeetingAttachedFileInfo
                    .Any(record => record.MeetingAttachedFileData.FileName == dialog.File.Name))
                {
                    Prompt.ShowDialog("File '" + dialog.File.Name + "' already exists as an attachment. Please change the name of the file and upload again.");
                    DataContextViewPresentationMeetingMinutes.BusyIndicatorNotification();
                    return;
                }

                MeetingAttachedFileData meetingAttachedFileData
                    = new MeetingAttachedFileData()
                    {
                        FileName = dialog.File.Name,
                        FileType = "Other Document",
                        MeetingID = DataContextViewPresentationMeetingMinutes.SelectedClosedForVotingMeetingInfo.MeetingID
                    };
                FileStream fileStream = dialog.File.OpenRead();

                DataContextViewPresentationMeetingMinutes.SelectedIndustryReportFileStreamData
                    = new MeetingAttachedFileStreamData()
                    {
                        MeetingAttachedFileData = meetingAttachedFileData,
                        FileStream = ReadFully(fileStream)
                    };

                DataContextViewPresentationMeetingMinutes.SelectedIndustryReports = dialog.File.Name;
                DataContextViewPresentationMeetingMinutes.BusyIndicatorNotification();
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

