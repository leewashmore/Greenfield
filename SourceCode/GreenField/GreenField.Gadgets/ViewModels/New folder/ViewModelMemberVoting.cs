using System;
using System.Windows;
using System.Windows.Input;
using Microsoft.Practices.Prism.ViewModel;
using System.Collections.ObjectModel;
using Microsoft.Practices.Prism.Commands;
using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.Regions;
using Ashmore.Emm.GreenField.BusinessLogic;
using Ashmore.Emm.GreenField.ICP.Meeting.Module.Model;
using Ashmore.Emm.GreenField.Common;
using System.Collections.Generic;
using Ashmore.Emm.GreenField.BusinessLogic.MeetingServiceReference;

namespace Ashmore.Emm.GreenField.ICP.Meeting.Module.ViewModels
{
    [Export]
    public class ViewModelMemberVoting : NotificationObject, INavigationAware
    {

        #region Fields

        [Import]
        public IRegionManager _regionManager { private get; set; }
        ManageMeetings _manageMeetings;

        #endregion

        #region Constructor
        
        [ImportingConstructor]
        public ViewModelMemberVoting(ManageMeetings manageMeetings)
        {
            _manageMeetings = manageMeetings;

            //Temp Code
            ObservableCollection<string> myVar = new ObservableCollection<string> { "ABC", "YAN", "PER" };
            blogObj.Add(new ICPBlogInfo
            {
                BlogCreationDate = DateTime.Now,
                BlogCreatedBy = "mansi",
                BlogComment = "awesome work done"
            });
            BlogInfo = blogObj;
        }

        #endregion

        #region Properties

        private ICPPresentationInfo _presentationInfo;
        public ICPPresentationInfo PresentationInfo
        {
            get
            {
                if (_presentationInfo == null)
                    return new ICPPresentationInfo();
                return _presentationInfo;
            }
            set
            {
                _presentationInfo = value;
                RaisePropertyChanged(() => this.PresentationInfo);
            }
        }

        private ICPVoterInfo _voterInfo = new ICPVoterInfo();
        public ICPVoterInfo VoterInfo
        {
            get { return _voterInfo; }
            set
            {
                _voterInfo = value;
                RaisePropertyChanged(() => this.VoterInfo);
            }
        }

        private ObservableCollection<ICPAttachmentInfo> _attachmentInfo;
        public ObservableCollection<ICPAttachmentInfo> AttachmentInfo
        {
            get
            {
                if (_attachmentInfo == null)
                    _attachmentInfo = new ObservableCollection<ICPAttachmentInfo>();
                return _attachmentInfo;
            }
            set
            {
                if (_attachmentInfo != value)
                {
                    _attachmentInfo = value;
                    RaisePropertyChanged(() => this.AttachmentInfo);
                }
            }
        }

        private ObservableCollection<ICPBlogInfo> _blogInfo;
        public ObservableCollection<ICPBlogInfo> BlogInfo
        {
            get { return _blogInfo; }
            set
            {
                if (_blogInfo != value)
                {
                    _blogInfo = value;
                    RaisePropertyChanged(() => BlogInfo);
                }
            }
        }

        ObservableCollection<ICPBlogInfo> blogObj = new ObservableCollection<ICPBlogInfo>();

        private ViewPluginFlagEnumeration _viewPluginFlag;
        public ViewPluginFlagEnumeration ViewPluginFlag
        {
            get
            {
                return _viewPluginFlag;
            }
            set
            {
                _viewPluginFlag = value;
                RaisePropertyChanged(() => this.ViewPluginFlag);
                RaisePropertyChanged(() => this.FinalExecutionText);
                RaisePropertyChanged(() => this.VotingEnabled);
            }
        }

        private string _finalExecutionText;
        public string FinalExecutionText
        {
            get
            {
                switch (ViewPluginFlag)
                {
                    case ViewPluginFlagEnumeration.Update:
                        _finalExecutionText = "Update";
                        break;
                    case ViewPluginFlagEnumeration.View:
                        _finalExecutionText = "Back";
                        break;
                }
                return _finalExecutionText;
            }
        }
        

        private bool _votingEnabled;
        public bool VotingEnabled
        {
            get
            {
                switch (ViewPluginFlag)
                {
                    case ViewPluginFlagEnumeration.Update:
                        _votingEnabled = true;
                        break;
                    case ViewPluginFlagEnumeration.View:
                        _votingEnabled = false;
                        break;
                }
                return _votingEnabled;
            }
        }
        

        public ObservableCollection<string> PFVMeasureInfo
        {
            get
            {
                return new ObservableCollection<string> { "PE2009", "PE2010" };
            }
        }

        private string _blogComment;
        public string BlogComment
        {
            get { return _blogComment; }
            set
            {
                if (_blogComment != value)
                    _blogComment = value;
                RaisePropertyChanged(() => this.BlogComment);
            }
        }

        public ObservableCollection<string> NotificationInfo
        {
            get
            {
                return new ObservableCollection<string> { "List1", "List2", "List3" };
            }
        }

        private string _selectedNotificationInfo;
        public string SelectedNotificationInfo
        {
            get { return _selectedNotificationInfo; }
            set
            {
                if (_selectedNotificationInfo != value)
                {
                    _selectedNotificationInfo = value;
                    RaisePropertyChanged(() => SelectedNotificationInfo);
                }
            }
        }

        public ICommand AddCommentButton
        {
            get
            {
                return (new DelegateCommand<object>(ICPBlogInfoUpdateItem));
            }
        }

        public ICommand FinalExecutionCommand
        {
            get
            {
                return (new DelegateCommand<object>(ICPVoterInfoFinalExecuteItem));
            }
        }
        #endregion

        #region ICommand Methods

        public void ICPVoterInfoFinalExecuteItem(object param)
        {
            if(ViewPluginFlag == ViewPluginFlagEnumeration.Update)
                _manageMeetings.CreateVoterInfo(VoterInfo.GetVoterInfo(PresentationInfo.PresentationID), (msg) => { MessageBox.Show(msg); });
            _regionManager.RequestNavigate(RegionNames.MainRegion, new Uri("ViewPresentations", UriKind.Relative));
            
        }

        public void ICPBlogInfoUpdateItem(object param)
        { }

        #endregion

        #region INavigationAware methods

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            //throw new NotImplementedException();
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            PresentationInfo = (navigationContext.NavigationService.Region.Context as ICPNavigationInfo).PresentationInfoObject;
            ViewPluginFlag = (navigationContext.NavigationService.Region.Context as ICPNavigationInfo).ViewPluginFlagEnumerationObject;
            _manageMeetings.GetFileInfo(PresentationInfo.PresentationID, GetFileInfoCallBackMethod);
        }

        #endregion

        #region Callback Method(s)

        private void GetFileInfoCallBackMethod(List<AttachedFileInfo> fileInfo)
        {
            int fileCount = 0;
            AttachmentInfo = new ObservableCollection<ICPAttachmentInfo>();
            foreach (AttachedFileInfo file in fileInfo)
            {
                ICPAttachmentInfo attachment = new ICPAttachmentInfo();
                attachment.ConvertFromDB(file, fileCount);
                AttachmentInfo.Add(attachment);
                fileCount++;
            }
        }

        #endregion
    }

}
