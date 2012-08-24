using System;
using System.Windows;
using System.Windows.Input;
using Microsoft.Practices.Prism.ViewModel;
using System.Collections.ObjectModel;
using Microsoft.Practices.Prism.Commands;
using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.Regions;
//using Ashmore.Emm.GreenField.BusinessLogic;
using GreenField.ServiceCaller;
//using Ashmore.Emm.GreenField.ICP.Meeting.Module.Model;
using GreenField.Gadgets.Models;
//using Ashmore.Emm.GreenField.Common;
using GreenField.Common;
using System.Collections.Generic;
//using Ashmore.Emm.GreenField.BusinessLogic.MeetingServiceReference;
using GreenField.ServiceCaller.MeetingDefinitions;
using Microsoft.Practices.Prism.ViewModel;
using GreenField.Gadgets.Views;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;
using GreenField.Gadgets.Helpers;
using Microsoft.Practices.Prism.Logging;


namespace GreenField.Gadgets.ViewModels
{
    public class ViewModelMemberVoting : NotificationObject
    {

        #region Fields

        public IRegionManager _regionManager { private get; set; }
       

        // private ManageMeetings _manageMeetings;
        /// <summary>
        /// Event Aggregator
        /// </summary>
        private IEventAggregator _eventAggregator;

        /// <summary>
        /// Instance of Service Caller Class
        /// </summary>
        private IDBInteractivity _dbInteractivity;

        /// <summary>
        /// Instance of LoggerFacade
        /// </summary>
        private ILoggerFacade _logger;

        /// <summary>
        /// IsActive is true when parent control is displayed on UI
        /// </summary>
        private bool _isActive;
        public bool IsActive
        {
            get
            {
                return _isActive;
            }
            set
            {
                _isActive = value;
            }
        }

        #endregion

        #region Constructor
        
        public ViewModelMemberVoting(DashboardGadgetParam param)
        {
            _dbInteractivity = param.DBInteractivity;
            _logger = param.LoggerFacade;
            _eventAggregator = param.EventAggregator;
            
            //Temp Code
            //ObservableCollection<string> myVar = new ObservableCollection<string> { "ABC", "YAN", "PER" };
            //blogObj.Add(new ICPBlogInfo
            //{
            //    BlogCreationDate = DateTime.Now,
            //    BlogCreatedBy = "mansi",
            //    BlogComment = "awesome work done"
            //});
            //BlogInfo = blogObj;
        }


        #endregion

        //#region Properties

        ////private ICPPresentationData _presentationInfo;
        ////public ICPPresentationData PresentationInfo
        ////{
        ////    get
        ////    {
        ////        if (_presentationInfo == null)
        ////            return new ICPPresentationData();
        ////        return _presentationInfo;
        ////    }
        ////    set
        ////    {
        ////        _presentationInfo = value;
        ////        RaisePropertyChanged(() => this.PresentationInfo);
        ////    }
        ////}

        //private ICPVoterInfo _voterInfo = new ICPVoterInfo();
        //public ICPVoterInfo VoterInfo
        //{
        //    get { return _voterInfo; }
        //    set
        //    {
        //        _voterInfo = value;
        //        RaisePropertyChanged(() => this.VoterInfo);
        //    }
        //}

        //private ObservableCollection<ICPAttachmentInfo> _attachmentInfo;
        //public ObservableCollection<ICPAttachmentInfo> AttachmentInfo
        //{
        //    get
        //    {
        //        if (_attachmentInfo == null)
        //            _attachmentInfo = new ObservableCollection<ICPAttachmentInfo>();
        //        return _attachmentInfo;
        //    }
        //    set
        //    {
        //        if (_attachmentInfo != value)
        //        {
        //            _attachmentInfo = value;
        //            RaisePropertyChanged(() => this.AttachmentInfo);
        //        }
        //    }
        //}

        //private ObservableCollection<ICPBlogInfo> _blogInfo;
        //public ObservableCollection<ICPBlogInfo> BlogInfo
        //{
        //    get { return _blogInfo; }
        //    set
        //    {
        //        if (_blogInfo != value)
        //        {
        //            _blogInfo = value;
        //            RaisePropertyChanged(() => BlogInfo);
        //        }
        //    }
        //}

        //ObservableCollection<ICPBlogInfo> blogObj = new ObservableCollection<ICPBlogInfo>();

        //private ViewPluginFlagEnumeration _viewPluginFlag;
        //public ViewPluginFlagEnumeration ViewPluginFlag
        //{
        //    get
        //    {
        //        return _viewPluginFlag;
        //    }
        //    set
        //    {
        //        _viewPluginFlag = value;
        //        RaisePropertyChanged(() => this.ViewPluginFlag);
        //        RaisePropertyChanged(() => this.FinalExecutionText);
        //        RaisePropertyChanged(() => this.VotingEnabled);
        //    }
        //}

        //private string _finalExecutionText;
        //public string FinalExecutionText
        //{
        //    get
        //    {
        //        switch (ViewPluginFlag)
        //        {
        //            case ViewPluginFlagEnumeration.Update:
        //                _finalExecutionText = "Update";
        //                break;
        //            case ViewPluginFlagEnumeration.View:
        //                _finalExecutionText = "Back";
        //                break;
        //        }
        //        return _finalExecutionText;
        //    }
        //}
        

        //private bool _votingEnabled;
        //public bool VotingEnabled
        //{
        //    get
        //    {
        //        switch (ViewPluginFlag)
        //        {
        //            case ViewPluginFlagEnumeration.Update:
        //                _votingEnabled = true;
        //                break;
        //            case ViewPluginFlagEnumeration.View:
        //                _votingEnabled = false;
        //                break;
        //        }
        //        return _votingEnabled;
        //    }
        //}
        

        //public ObservableCollection<string> PFVMeasureInfo
        //{
        //    get
        //    {
        //        return new ObservableCollection<string> { "PE2009", "PE2010" };
        //    }
        //}

        //private string _blogComment;
        //public string BlogComment
        //{
        //    get { return _blogComment; }
        //    set
        //    {
        //        if (_blogComment != value)
        //            _blogComment = value;
        //        RaisePropertyChanged(() => this.BlogComment);
        //    }
        //}

        //public ObservableCollection<string> NotificationInfo
        //{
        //    get
        //    {
        //        return new ObservableCollection<string> { "List1", "List2", "List3" };
        //    }
        //}

        //private string _selectedNotificationInfo;
        //public string SelectedNotificationInfo
        //{
        //    get { return _selectedNotificationInfo; }
        //    set
        //    {
        //        if (_selectedNotificationInfo != value)
        //        {
        //            _selectedNotificationInfo = value;
        //            RaisePropertyChanged(() => SelectedNotificationInfo);
        //        }
        //    }
        //}

        //public ICommand AddCommentButton
        //{
        //    get
        //    {
        //        return (new DelegateCommand<object>(ICPBlogInfoUpdateItem));
        //    }
        //}

        //public ICommand FinalExecutionCommand
        //{
        //    get
        //    {
        //        return (new DelegateCommand<object>(ICPVoterInfoFinalExecuteItem));
        //    }
        //}
        //#endregion

        //#region ICommand Methods

        //public void ICPVoterInfoFinalExecuteItem(object param)
        //{
        //    //if(ViewPluginFlag == ViewPluginFlagEnumeration.Update)
        //    //    _dbInteractivity.CreateVoterInfo(VoterInfo.GetVoterInfo(PresentationInfo.PresentationID), (msg) => { MessageBox.Show(msg); });
        //    _regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewPresentations", UriKind.Relative));
            
        //}

        //public void ICPBlogInfoUpdateItem(object param)
        //{ }

        //#endregion

        //#region INavigationAware methods

        //public bool IsNavigationTarget(NavigationContext navigationContext)
        //{
        //    return true;
        //}

        //public void OnNavigatedFrom(NavigationContext navigationContext)
        //{
        //    //throw new NotImplementedException();
        //}

        //public void OnNavigatedTo(NavigationContext navigationContext)
        //{
        //    //PresentationInfo = (navigationContext.NavigationService.Region.Context as ICNavigationInfo).PresentationInfoObject;
        //    //ViewPluginFlag = (navigationContext.NavigationService.Region.Context as ICNavigationInfo).ViewPluginFlagEnumerationObject;
        //    //_dbInteractivity.GetFileInfo(PresentationInfo.PresentationID, GetFileInfoCallBackMethod);
        //}

        //#endregion

        //#region Callback Method(s)

        //private void GetFileInfoCallBackMethod(List<AttachedFileInfo> fileInfo)
        //{
        //    int fileCount = 0;
        //    AttachmentInfo = new ObservableCollection<ICPAttachmentInfo>();
        //    foreach (AttachedFileInfo file in fileInfo)
        //    {
        //        ICPAttachmentInfo attachment = new ICPAttachmentInfo();
        //        attachment.ConvertFromDB(file, fileCount);
        //        AttachmentInfo.Add(attachment);
        //        fileCount++;
        //    }
        //}

        //#endregion

        #region EventUnSubscribe
        /// <summary>
        /// Method that disposes the events
        /// </summary>
        public void Dispose()
        {

        }

        #endregion

    }

}
