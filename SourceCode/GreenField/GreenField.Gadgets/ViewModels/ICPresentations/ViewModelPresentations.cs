using System;
using System.Net;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel.Composition;
//using Ashmore.Emm.GreenField.BusinessLogic;
using GreenField.ServiceCaller;
using Microsoft.Practices.Prism.ViewModel;
//using Ashmore.Emm.GreenField.BusinessLogic.MeetingServiceReference;
using GreenField.ServiceCaller.MeetingServiceReference;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Commands;
//using Ashmore.Emm.GreenField.ICP.Meeting.Module.Model;
using GreenField.Gadgets.Models;
using Microsoft.Practices.Prism.Regions;
//using Ashmore.Emm.GreenField.Common;
using GreenField.Common;
//using Ashmore.Emm.GreenField.ICP.Meeting.Module.Views;
using GreenField.Gadgets.Views;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;
using GreenField.Gadgets.Helpers;



namespace GreenField.Gadgets.ViewModels
{
    public class ViewModelPresentations : NotificationObject
    {

        #region Fields

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

        public ViewModelPresentations(DashboardGadgetParam param)
        {
            _dbInteractivity = param.DBInteractivity;
            _logger = param.LoggerFacade;
            _eventAggregator = param.EventAggregator;
            _regionManager = param.RegionManager;


            ManageMeetingsServiceCalls();
            
        }


        #endregion

        #region Properties

        #region Prism-Imported Properties

        public IRegionManager _regionManager { private get; set; }

        #endregion

        #region Binded Properties

        private ObservableCollection<string> _presenterInfo;
        public ObservableCollection<string> PresenterInfo
        {
            get { return _presenterInfo; }
            set
            {
                if (_presenterInfo != value)
                {
                    _presenterInfo = value;
                    RaisePropertyChanged(() => this.PresenterInfo);
                }
            }
        }

        private ObservableCollection<StatusType> _statusTypeInfo;
        public ObservableCollection<StatusType> StatusTypeInfo
        {
            get { return _statusTypeInfo; }
            set
            {
                if (_statusTypeInfo != value)
                {
                    _statusTypeInfo = value;
                    RaisePropertyChanged(() => this.StatusTypeInfo);
                }
            }
        }

        private ICPPresentationInfo _selectedPresentation;
        public ICPPresentationInfo SelectedPresentation
        {
            get { return _selectedPresentation; }
            set
            {
                if (_selectedPresentation != value)
                {
                    _selectedPresentation = value;
                    NavigationInfo.PresentationInfoObject = value;
                    RaisePropertyChanged(() => this.SelectedPresentation);
                    SelectionRaisePropertyChanged();
                }
            }
        }

        private ObservableCollection<ICPPresentationInfo> _presentationInfo;
        public ObservableCollection<ICPPresentationInfo> PresentationInfo
        {
            get { return _presentationInfo; }
            set
            {
                if (_presentationInfo != value)
                {
                    _presentationInfo = value;
                    RaisePropertyChanged(() => this.PresentationInfo);
                }
            }
        }


        private DateTime? _searchDateFilter = null;
        public DateTime? SearchDateFilter
        {
            get { return _searchDateFilter; }
            set
            {
                if (_searchDateFilter != value)
                {
                    _searchDateFilter = value;
                    RaisePropertyChanged(() => this.SearchDateFilter);
                    FilterRaisePropertyChanged();
                }
            }
        }




        private string _presenterFilter;
        public string PresenterFilter
        {
            get { return _presenterFilter; }
            set
            {
                if (_presenterFilter != value)
                {
                    _presenterFilter = value;
                    RaisePropertyChanged(() => this.PresenterFilter);
                    FilterRaisePropertyChanged();
                }
            }
        }



        private StatusType _statusTypeFilter;
        public StatusType StatusTypeFilter
        {
            get { return _statusTypeFilter; }
            set
            {
                if (_statusTypeFilter != value)
                {
                    _statusTypeFilter = value;
                    RaisePropertyChanged(() => this.StatusTypeFilter);
                    FilterRaisePropertyChanged();
                }
            }
        }
   
        #endregion

        #region ICommand Properties

     
        public ICommand UploadCommand
        {
            get { return new DelegateCommand<object>(ICPPresentationsUploadItem, ICPPresentationsUploadItemValidation); }
        }

        public ICommand EditCommand
        {
            get { return new DelegateCommand<object>(ICPPresentationsEditItem, ICPPresentationsEditItemValidation); }
        }

      
        public ICommand WithdrawCommand
        {
            get { return new DelegateCommand<object>(ICPPresentationsWithdrawItem, ICPPresentationsEditItemValidation); }
        }

        public ICommand ViewCommand
        {
            get { return new DelegateCommand<object>(ICPPresentationsViewItem, ICPPresentationsViewItemValidation); }
        }

    

        public ICommand NewPresentationCommand
        {
            get { return new DelegateCommand<object>(ICPPresentationsNewItem); }
        }

        public ICommand SearchCommand
        {
            get { return new DelegateCommand<object>(ICPPresentationsSearch, ICPPresentationsSearchResetValidation); }
        }

        public ICommand ResetCommand
        {
            get { return new DelegateCommand<object>(ICPPresentationsReset, ICPPresentationsSearchResetValidation); }
        }

        public ICommand AcceptCommand
        {
            get { return new DelegateCommand<object>(ICPPresentationsAcceptItem, ICPPresentationsAcceptItemValidation); }
        }

        public ICommand RequestCommand
        {
            get { return new DelegateCommand<object>(ICPPresentationsRequestItem, ICPPresentationsRequestItemValidation); }
        }


        #endregion

        #region Navigation Properties

        //Meeting Information not required - nulled
        private ICPNavigationInfo _navigationInfo = new ICPNavigationInfo { MeetingInfoObject = null };
        public ICPNavigationInfo NavigationInfo
        {
            get { return _navigationInfo; }
            set { _navigationInfo = value; }
        }

        #endregion

        #endregion

        #region ICommand Methods

      
        #region UploadCommand

        private bool ICPPresentationsUploadItemValidation(object param)
        {
            bool userRoleValidation = true; //Check if user is the author
            bool statusValidation = true;
            if (SelectedPresentation != null)
            {
                long status = SelectedPresentation.StatusTypeID;
                statusValidation = (status == StatusTypes.InProgress);
            }
            return SelectedPresentation != null ? (userRoleValidation && statusValidation ? true : false) : false;
        }

        private void ICPPresentationsUploadItem(object param)
        {
            if (NavigationInfo == null) NavigationInfo = new ICPNavigationInfo();

            NavigationInfo.MeetingInfoObject = null;
            NavigationInfo.ViewPluginFlagEnumerationObject = ViewPluginFlagEnumeration.Upload;
            NavigationInfo.PresentationInfoObject = SelectedPresentation;

            _regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardInvestmentCommitteeCreateEdit", UriKind.Relative));
        }

        #region EditCommand

        private bool ICPPresentationsEditItemValidation(object param)
        {
            bool userRoleValidation = true; //Check if user is the author
            bool statusValidation = true;
            if (SelectedPresentation != null)
            {
                long status = SelectedPresentation.StatusTypeID;
                statusValidation = (status == StatusTypes.InProgress) || (status == StatusTypes.Requested)
                    || (status == StatusTypes.PendingDocuments) || (status == StatusTypes.Withdrawn);
            }
            return SelectedPresentation != null ? (userRoleValidation && statusValidation ? true : false) : false;
        }

        private void ICPPresentationsEditItem(object param)
        {
            if (NavigationInfo == null) NavigationInfo = new ICPNavigationInfo();
                        
            NavigationInfo.MeetingInfoObject = null;
            NavigationInfo.ViewPluginFlagEnumerationObject = ViewPluginFlagEnumeration.Update;
            NavigationInfo.PresentationInfoObject = SelectedPresentation;

            _regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardInvestmentCommitteeCreateEdit", UriKind.Relative));
        }

        #endregion

        #region SearchCommand, ResetCommand

        private bool ICPPresentationsSearchResetValidation(object param)
        {
            return (SearchDateFilter != null || PresenterFilter != null || StatusTypeFilter != null) ? true : false;
        }

        private void ICPPresentationsSearch(object param)
        {
            DateTime? SearchDate = (SearchDateFilter != null) ? SearchDateFilter : null;
            string Presenter = (PresenterFilter != null) ? PresenterFilter : null;
            string Status = (StatusTypeFilter != null) ? StatusTypeFilter.StatusType1 : null;

            _dbInteractivity.GetPresentationsByMeetingDatePresenterStatus(SearchDate, Presenter, Status, GetPresentationsCallBackMethod);
        }

        private void ICPPresentationsReset(object param)
        {
            ManageMeetingsServiceCalls();
        }

        #endregion


        #region WithdrawCommand

        private bool ICPPresentationsWithdrawItemValidation(object param)
        {
            bool userRoleValidation = true; //Check if user is Author
            bool statusValidation = true; //Status checks might come later
            
            return SelectedPresentation != null ? (userRoleValidation && statusValidation ? true : false) : false;
        }

        private void ICPPresentationsWithdrawItem(object param)
        {
            //Mapping to be removed or not ?
            SelectedPresentation.StatusTypeID = StatusTypes.Withdrawn;
            _dbInteractivity.UpdatePresentation(SelectedPresentation.ConvertToDB(), (msg) =>
            {
                _dbInteractivity.GetPresentations(GetPresentationsCallBackMethod);
            });
        }

        #endregion

        #region ViewCommand

        private bool ICPPresentationsViewItemValidation(object param)
        {
            bool userRoleValidation = false; //Check if user is Author
            bool statusValidation = true;
            if (SelectedPresentation != null)
            {
                if (userRoleValidation)
                {
                    long status = SelectedPresentation.StatusTypeID;
                    statusValidation = (status == StatusTypes.InProgress || status == StatusTypes.Requested);
                }
            }

            return SelectedPresentation != null ? (statusValidation ? true : false) : false;
        }

        private void ICPPresentationsViewItem(object param)
        {
            bool userRoleValidation = true; //Check if user is voting member

            if (userRoleValidation && SelectedPresentation.StatusTypeID == StatusTypes.ReadyOpenforVoting)
                NavigationInfo.ViewPluginFlagEnumerationObject = ViewPluginFlagEnumeration.Update;
            else
                NavigationInfo.ViewPluginFlagEnumerationObject = ViewPluginFlagEnumeration.View;
            
            NavigationInfo.PresentationInfoObject = SelectedPresentation;
            _regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewMemberVoting", UriKind.Relative));
        }

        #endregion

       

        #region NewPresentationCommand

        private void ICPPresentationsNewItem(object param)
        {
            NavigationInfo.ViewPluginFlagEnumerationObject = ViewPluginFlagEnumeration.Create;
            _regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardInvestmentCommitteeCreateEdit", UriKind.Relative));
        }

        #endregion




        #region AcceptCommand

        private bool ICPPresentationsAcceptItemValidation(object param)
        {

            bool userRoleValidation = true; //Check if user is ICAdmin
            bool statusValidation = true;
            if (SelectedPresentation != null)
            {
                long status = SelectedPresentation.StatusTypeID;
                statusValidation = (status == StatusTypes.Requested);
            }

            return SelectedPresentation != null ? (userRoleValidation && statusValidation ? true : false) : false;
        }

        private void ICPPresentationsAcceptItem(object param)
        {

            ChildAcceptRequestPresentations acceptPresentationNotification =
            new ChildAcceptRequestPresentations(_dbInteractivity, _logger, Convert.ToDateTime(SelectedPresentation.PresentationDate)) { Title = "Accept Presentation" };

            acceptPresentationNotification.Show();
            acceptPresentationNotification.Unloaded += (se, e) =>
                {
                    if ((se as ChildAcceptRequestPresentations).DialogResult == true)
                    {
                        MeetingInfo meetingInfo = (se as ChildAcceptRequestPresentations).SelectedMeeting;

                        //Update Mapping

                        if (meetingInfo.MeetingDateTime != SelectedPresentation.PresentationDate)
                        {
                            MeetingPresentationMappingInfo mappingUpdateInfo = new MeetingPresentationMappingInfo
                            {
                                MeetingID = meetingInfo.MeetingID,
                                PresentationID = SelectedPresentation.PresentationID,
                                ModifedBy = "rvig",
                                ModifiedOn = DateTime.Now
                            };

                            _dbInteractivity.UpdateMeetingPresentationMapping(mappingUpdateInfo, (msg) => { });
                        }

                        //Update Status
                        SelectedPresentation.PresentationDate = meetingInfo.MeetingDateTime;
                        SelectedPresentation.StatusTypeID = StatusTypes.PendingDocuments;
                        _dbInteractivity.UpdatePresentation(SelectedPresentation.ConvertToDB(), (msg) =>
                        {
                            _dbInteractivity.GetPresentations(GetPresentationsCallBackMethod);
                        });
                    }
                };

        }

        #endregion

        #region RequestCommand

        private bool ICPPresentationsRequestItemValidation(object param)
        {
            bool userRoleValidation = true; //Check if user is Author
            bool statusValidation = true;
            if (SelectedPresentation != null)
            {
                long status = SelectedPresentation.StatusTypeID;
                statusValidation = (status == StatusTypes.InProgress);
            }

            return SelectedPresentation != null ? (userRoleValidation && statusValidation ? true : false) : false;
        }

        private void ICPPresentationsRequestItem(object param)
        {

            ChildAcceptRequestPresentations requestMeetingNotification =
                new ChildAcceptRequestPresentations(_dbInteractivity, _logger, Convert.ToDateTime(SelectedPresentation.PresentationDate)) { Title = "Request Meeting" };

            requestMeetingNotification.Show();
            requestMeetingNotification.Unloaded += (se, e) =>
            {
                MeetingInfo meetingInfo = (se as ChildAcceptRequestPresentations).SelectedMeeting;

                //Create Mapping

                MeetingPresentationMappingInfo mappingInfo = new MeetingPresentationMappingInfo
                {
                    MeetingID = meetingInfo.MeetingID,
                    PresentationID = SelectedPresentation.PresentationID,
                    CreatedBy = "rvig",
                    CreatedOn = DateTime.Now,
                    ModifedBy = "rvig",
                    ModifiedOn = DateTime.Now
                };

                _dbInteractivity.CreateMeetingPresentationMapping(mappingInfo, (msg) => { MessageBox.Show(msg); });

                //Update Status

                SelectedPresentation.PresentationDate = meetingInfo.MeetingDateTime;
                SelectedPresentation.StatusTypeID = StatusTypes.Requested;
                _dbInteractivity.UpdatePresentation(SelectedPresentation.ConvertToDB(), (msg) =>
                {
                    _dbInteractivity.GetPresentations(GetPresentationsCallBackMethod);
                });

            };
        }

        #endregion

        #endregion

        #endregion


        #region INavigationAware methods

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            navigationContext.NavigationService.Region.Context = NavigationInfo;
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            //Make Service Calls to DB everytime we navigate to this view
            ManageMeetingsServiceCalls();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        #endregion

        #region Helper Methods

        private void SelectionRaisePropertyChanged()
        {
            RaisePropertyChanged(() => this.EditCommand);
            RaisePropertyChanged(() => this.AcceptCommand);
            RaisePropertyChanged(() => this.WithdrawCommand);
            RaisePropertyChanged(() => this.ViewCommand);
            RaisePropertyChanged(() => this.RequestCommand);
        }

        private void FilterRaisePropertyChanged()
        {
            RaisePropertyChanged(() => this.SearchCommand);
            RaisePropertyChanged(() => this.ResetCommand);
        }

        private void ManageMeetingsServiceCalls()
        {
            _dbInteractivity.GetPresentations(GetPresentationsCallBackMethod);
            _dbInteractivity.GetDistinctPresenters(GetDistinctPresentersCallBackMethod);
            _dbInteractivity.GetStatusTypes(GetDistinctStatusTypesCallBackMethod);

            //Default Settings
            PresenterFilter = null;
            StatusTypeFilter = null;
            SearchDateFilter = null;

            SelectionRaisePropertyChanged();
        }

        #endregion

        #region CallBack Methods

        private void GetPresentationsCallBackMethod(List<PresentationInfoResult> val)
        {
            ObservableCollection<PresentationInfoResult> PresentationInfoCollObj = new ObservableCollection<PresentationInfoResult>(val);
            ObservableCollection<ICPPresentationInfo> ICPPresentationInfoCollObj = new ObservableCollection<ICPPresentationInfo>();
            foreach (PresentationInfoResult pinfo in PresentationInfoCollObj)
            {
                ICPPresentationInfo ICPPresentationInfoObj = new ICPPresentationInfo(pinfo);
                ICPPresentationInfoCollObj.Add(ICPPresentationInfoObj);
            }

            PresentationInfo = ICPPresentationInfoCollObj;

        }

        private void GetDistinctPresentersCallBackMethod(List<string> val)
        {
            PresenterInfo = new ObservableCollection<string>(val);
        }

        private void GetDistinctStatusTypesCallBackMethod(List<StatusType> val)
        {
            StatusTypeInfo = new ObservableCollection<StatusType>(val);
        }

        #endregion

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
