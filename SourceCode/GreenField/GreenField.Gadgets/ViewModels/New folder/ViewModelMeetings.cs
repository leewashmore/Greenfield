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
using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.ViewModel;
using Microsoft.Practices.Prism.Commands;
using System.Collections.ObjectModel;
using Ashmore.Emm.GreenField.ICP.Meeting.Module.Model;
using Ashmore.Emm.GreenField.BusinessLogic;
using System.Collections.Generic;
using Ashmore.Emm.GreenField.BusinessLogic.MeetingServiceReference;
using Microsoft.Practices.Prism.Regions;
using Ashmore.Emm.GreenField.Common;

namespace Ashmore.Emm.GreenField.ICP.Meeting.Module.ViewModels
{
    [Export]
    public class ViewModelMeetings : NotificationObject, INavigationAware
    {
        
        #region Properties

        [Import]
        public IRegionManager regionManager { private get; set; }

        ManageMeetings _manageMeetings;

        private DateTime _searchDate = DateTime.Today;
        public DateTime SearchDateProperty
        {
            get { return _searchDate; }
            set
            {
                if (_searchDate != value)
                {
                    _searchDate = value;
                    RaisePropertyChanged(() => this.SearchDateProperty);
                }
            }
        }

        private object _gridActiveItem;
        public object GridActiveItemProperty
        {
            get { return _gridActiveItem; }
            set
            {
                if (_gridActiveItem != value)
                {
                    _gridActiveItem = value;
                    RaisePropertyChanged(() => this.GridActiveItemProperty);
                    RaisePropertyChanged(() => this.EditCommand);
                    RaisePropertyChanged(() => this.CancelCommand);
                    RaisePropertyChanged(() => this.ViewMeetingAgendaCommand);
                }
            }
        }

        public ICommand SeachDateFilterCommand
        {
            get { return new DelegateCommand<object>(ICPMeetingsListFilter); }
        }

        public ICommand ResetCommand
        {
            get { return new DelegateCommand<object>(ICPMeetingsReset); }
        }

        public ICommand EditCommand
        {
            get { return new DelegateCommand<object>(ICPMeetingsListEditItem, ItemSelectedValidation); }
        }

        public ICommand CancelCommand
        {
            get { return new DelegateCommand<object>(ICPMeetingsListCancelItem, ItemSelectedValidation); }
        }

        public ICommand ViewMeetingAgendaCommand
        {
            get { return new DelegateCommand<object>(ICPMeetingsListViewMeetingAgenda, ItemSelectedValidation); }
        }

        public ICommand NewMeetingCommand
        {
            get { return new DelegateCommand<object>(ICPMeetingsListNewItem); }
        }

        private ICPNavigationInfo _meetingNavigationObject = new ICPNavigationInfo();
        public ICPNavigationInfo MeetingNavigationObjectProperty
        {
            get { return _meetingNavigationObject; }
            set { _meetingNavigationObject = value; }
        }
        
        private ObservableCollection<ICPMeetingInfo> _meetingList;
        public ObservableCollection<ICPMeetingInfo> MeetingListProperty
        {
            get
            {
                if (_meetingList == null)
                {
                    _manageMeetings.GetMeetings(GetMeetingsCallBackMethod);
                }
                return _meetingList;
            }
            set
            {
                if (_meetingList != value)
                {
                    _meetingList = value;
                    RaisePropertyChanged(() => this.MeetingListProperty);
                }
            }
        }

        #endregion

        #region Private members

        private ObservableCollection<ICPMeetingInfo> dataContextObj = new ObservableCollection<ICPMeetingInfo>();

        #endregion

        #region Contructor

 
        [ImportingConstructor]
        public ViewModelMeetings(ManageMeetings manageMeetings)
        {
            _manageMeetings = manageMeetings;
        }

        #endregion

        #region ICommand Methods

        private void ICPMeetingsListFilter(object param)
        {
            //Run Server Method with filter as Meeting Date
            _manageMeetings.GetMeetingsByDate(SearchDateProperty, GetMeetingsCallBackMethod);
        }

        private void ICPMeetingsReset(object param)
        {
            _manageMeetings.GetMeetings(GetMeetingsCallBackMethod);
        }

        private bool ItemSelectedValidation(object param)
        {
            return GridActiveItemProperty == null ? false : true;
        }

        private void ICPMeetingsListEditItem(object param)
        {
            //Navigate to Edit Page with Meeting ID as input
            MeetingNavigationObjectProperty.ViewPluginFlagEnumerationObject = ViewPluginFlagEnumeration.Update;
            MeetingNavigationObjectProperty.MeetingInfoObject = GridActiveItemProperty as ICPMeetingInfo;
            regionManager.RequestNavigate(RegionNames.MainRegion, new Uri("ViewCreateUpdateMeetings", UriKind.Relative));
        }

        private void ICPMeetingsListCancelItem(object param)
        {
            //Run Server Method with parameter of Meeting ID
        }

        private void ICPMeetingsListViewMeetingAgenda(object param)
        {
            //Navigate to Agenda Page with Meeting ID as input
            MeetingNavigationObjectProperty.MeetingInfoObject = GridActiveItemProperty as ICPMeetingInfo;
            regionManager.RequestNavigate(RegionNames.MainRegion, new Uri("ViewMeetingsAgenda", UriKind.Relative));
        }

        private void ICPMeetingsListNewItem(object param)
        {
            //Navigate to Create Meeting Page
            MeetingNavigationObjectProperty.ViewPluginFlagEnumerationObject = ViewPluginFlagEnumeration.Create;
            MeetingNavigationObjectProperty.MeetingInfoObject = new ICPMeetingInfo();
            regionManager.RequestNavigate(RegionNames.MainRegion, new Uri("ViewCreateUpdateMeetings", UriKind.Relative));
            
        }

        #endregion

        #region INavigationAware methods

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            navigationContext.NavigationService.Region.Context = MeetingNavigationObjectProperty;
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        #endregion

        private void GetMeetingsCallBackMethod(List<MeetingInfo> val)
        {
            ObservableCollection<MeetingInfo> MeetingInfoCollObj = new ObservableCollection<MeetingInfo>(val);
            ObservableCollection<ICPMeetingInfo> ICPMeetingInfoCollObj = new ObservableCollection<ICPMeetingInfo>();
            foreach (MeetingInfo minfo in MeetingInfoCollObj)
            {
                ICPMeetingInfo ICPMeetingInfoObj = new ICPMeetingInfo
                {
                    MeetingID = minfo.MeetingID,
                    MeetingDateProperty = minfo.MeetingDateTime,
                    MeetingTimeProperty = minfo.MeetingDateTime,
                    MeetingClosedDateProperty = minfo.MeetingClosedDateTime,
                    MeetingClosedTimeProperty = minfo.MeetingClosedDateTime,
                    DescriptionProperty = minfo.Description
                };

                ICPMeetingInfoCollObj.Add(ICPMeetingInfoObj);
            }

            MeetingListProperty = ICPMeetingInfoCollObj;
        }

        
    }
}
