using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Prism.Commands;
using System.Windows.Browser;
using Microsoft.Practices.Prism.Events;
using System.ComponentModel.Composition;
//using Ashmore.Emm.GreenField.Common;
using GreenField.Common;
//using Ashmore.Emm.GreenField.BusinessLogic.MeetingServiceReference;
using GreenField.ServiceCaller.MeetingServiceReference;

namespace GreenField.Gadgets.Models
{
    public class ICPAttachmentInfo : NotificationObject
    {

        private int _attachmentID;
        public int AttachmentID
        {
            get { return _attachmentID; }
            set
            {
                if (_attachmentID != value)
                {
                    _attachmentID = value;
                    RaisePropertyChanged(() => this.AttachmentID);
                }
            }
        }

        private long _presentationID;
        public long PresentationID
        {
            get { return _presentationID; }
            set
            {
                if (_presentationID != value)
                {
                    _presentationID = value;
                    RaisePropertyChanged(() => this.PresentationID);
                }
            }
        }

        private string _attachmentName;
        public string AttachmentName
        {
            get { return _attachmentName; }
            set
            {
                if (_attachmentName != value)
                {
                    _attachmentName = value;
                    RaisePropertyChanged(() => this.AttachmentName);
                }
            }
        }

        private string _attachmentSerializedData;
        public string AttachmentSerializedData
        {
            get { return _attachmentSerializedData; }
            set
            {
                if (_attachmentSerializedData != value)
                {
                    _attachmentSerializedData = value;
                    RaisePropertyChanged(() => this.AttachmentSerializedData);
                }
            }
        }

        private Visibility _attachmentVisibility = Visibility.Visible;
        public Visibility AttachmentVisibility
        {
            get { return _attachmentVisibility; }
            set
            {
                if (_attachmentVisibility != value)
                {
                    _attachmentVisibility = value;
                    RaisePropertyChanged(() => this.AttachmentVisibility);
                }
            }
        }
        
        
        public ICommand OpenAttachmentCommand
        {
            get { return new DelegateCommand<object>(ICPAttachmentOpenItem); }
        }

        public ICommand DeleteAttachmentCommand
        {
            get { return new DelegateCommand<object>(ICPAttachmentDeleteItem); }
        }

        private void ICPAttachmentOpenItem(object param)
        {
            
            HtmlPage.Window.Invoke("PassFileDataToChildWindow", new string[] { this.AttachmentSerializedData, this.AttachmentName });
        }

        private void ICPAttachmentDeleteItem(object param)
        {
            this.AttachmentVisibility = Visibility.Collapsed;
        }

        public AttachedFileInfo ConvertToDB()
        {
            AttachedFileInfo AttachmentFile = new AttachedFileInfo();
            AttachmentFile.PresentationID = this.PresentationID;
            AttachmentFile.FileName = this.AttachmentName;
            AttachmentFile.FileSerializedData = this.AttachmentSerializedData;
            AttachmentFile.CreatedBy = "rvig";
            AttachmentFile.CreatedOn = DateTime.Now;
            AttachmentFile.ModifiedBy = "rvig";
            AttachmentFile.ModifiedOn = DateTime.Now;
            return AttachmentFile;
        }

        public void ConvertFromDB(AttachedFileInfo fileInfo, int fileCount)
        {
            this.AttachmentID = fileCount;
            this.PresentationID = fileInfo.PresentationID;
            this.AttachmentName = fileInfo.FileName;
            this.AttachmentSerializedData = fileInfo.FileSerializedData;
            this.AttachmentVisibility = Visibility.Visible;
        }

    }
}
