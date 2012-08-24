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
//using Ashmore.Emm.GreenField.ICP.Meeting.Module.Model;
using GreenField.Gadgets.Models;
//using Ashmore.Emm.GreenField.BusinessLogic;
using GreenField.ServiceCaller;

using System.Collections.Generic;
//using Ashmore.Emm.GreenField.BusinessLogic.MeetingServiceReference;
using GreenField.ServiceCaller.MeetingDefinitions;
using Microsoft.Practices.Prism.Regions;
//using Ashmore.Emm.GreenField.Common;
using GreenField.Common;
using GreenField.Gadgets.Views;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Logging;



namespace GreenField.Gadgets.ViewModels
{
    public class ViewModelMeetings : NotificationObject
    {

        #region Fields

        public IRegionManager regionManager { private get; set; }

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

        #region Contructor

        public ViewModelMeetings(DashboardGadgetParam param)
        {
            _dbInteractivity = param.DBInteractivity;
            _logger = param.LoggerFacade;
            _eventAggregator = param.EventAggregator;


        }


        #endregion

        //#region Properties
              

        //private DateTime _searchDate = DateTime.Today;
        //public DateTime SearchDateProperty
        //{
        //    get { return _searchDate; }
        //    set
        //    {
        //        if (_searchDate != value)
        //        {
        //            _searchDate = value;
        //            RaisePropertyChanged(() => this.SearchDateProperty);
        //        }
        //    }
        //}

        //private object _gridActiveItem;
        //public object GridActiveItemProperty
        //{
        //    get { return _gridActiveItem; }
        //    set
        //    {
        //        if (_gridActiveItem != value)
        //        {
        //            _gridActiveItem = value;
        //            RaisePropertyChanged(() => this.GridActiveItemProperty);
        //            RaisePropertyChanged(() => this.EditCommand);
        //            RaisePropertyChanged(() => this.CancelCommand);
        //            RaisePropertyChanged(() => this.ViewMeetingAgendaCommand);
        //        }
        //    }
        //}

        //public ICommand SeachDateFilterCommand
        //{
        //    get { return new DelegateCommand<object>(ICPMeetingsListFilter); }
        //}

        //public ICommand ResetCommand
        //{
        //    get { return new DelegateCommand<object>(ICPMeetingsReset); }
        //}

        //public ICommand EditCommand
        //{
        //    get { return new DelegateCommand<object>(ICPMeetingsListEditItem, ItemSelectedValidation); }
        //}

        //public ICommand CancelCommand
        //{
        //    get { return new DelegateCommand<object>(ICPMeetingsListCancelItem, ItemSelectedValidation); }
        //}

        //public ICommand ViewMeetingAgendaCommand
        //{
        //    get { return new DelegateCommand<object>(ICPMeetingsListViewMeetingAgenda, ItemSelectedValidation); }
        //}

        //public ICommand NewMeetingCommand
        //{
        //    get { return new DelegateCommand<object>(ICPMeetingsListNewItem); }
        //}

        //private ICNavigationInfo _meetingNavigationObject = new ICNavigationInfo();
        //public ICNavigationInfo MeetingNavigationObjectProperty
        //{
        //    get { return _meetingNavigationObject; }
        //    set { _meetingNavigationObject = value; }
        //}
        
        //private ObservableCollection<ICPMeetingInfo> _meetingList;
        //public ObservableCollection<ICPMeetingInfo> MeetingListProperty
        //{
        //    get
        //    {
        //        if (_meetingList == null)
        //        {
        //            _dbInteractivity.GetMeetings(GetMeetingsCallBackMethod);
        //        }
        //        return _meetingList;
        //    }
        //    set
        //    {
        //        if (_meetingList != value)
        //        {
        //            _meetingList = value;
        //            RaisePropertyChanged(() => this.MeetingListProperty);
        //        }
        //    }
        //}

        //#endregion

        //#region Private members

        //private ObservableCollection<ICPMeetingInfo> dataContextObj = new ObservableCollection<ICPMeetingInfo>();

        //#endregion

        

        //#region ICommand Methods

        //private void ICPMeetingsListFilter(object param)
        //{
        //    //Run Server Method with filter as Meeting Date
        //    _dbInteractivity.GetMeetingsByDate(SearchDateProperty, GetMeetingsCallBackMethod);
        //}

        //private void ICPMeetingsReset(object param)
        //{
        //    _dbInteractivity.GetMeetings(GetMeetingsCallBackMethod);
        //}

        //private bool ItemSelectedValidation(object param)
        //{
        //    return GridActiveItemProperty == null ? false : true;
        //}

        //private void ICPMeetingsListEditItem(object param)
        //{
        //    //Navigate to Edit Page with Meeting ID as input
        //    MeetingNavigationObjectProperty.ViewPluginFlagEnumerationObject = ViewPluginFlagEnumeration.Update;
        //    MeetingNavigationObjectProperty.MeetingInfoObject = GridActiveItemProperty as ICPMeetingInfo;
        //    regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewCreateUpdateMeetings", UriKind.Relative));
        //}

        //private void ICPMeetingsListCancelItem(object param)
        //{
        //    //Run Server Method with parameter of Meeting ID
        //}

        //private void ICPMeetingsListViewMeetingAgenda(object param)
        //{
        //    //Navigate to Agenda Page with Meeting ID as input
        //    MeetingNavigationObjectProperty.MeetingInfoObject = GridActiveItemProperty as ICPMeetingInfo;
        //    regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewMeetingsAgenda", UriKind.Relative));
        //}

        //private void ICPMeetingsListNewItem(object param)
        //{
        //    //Navigate to Create Meeting Page
        //    MeetingNavigationObjectProperty.ViewPluginFlagEnumerationObject = ViewPluginFlagEnumeration.Create;
        //    MeetingNavigationObjectProperty.MeetingInfoObject = new ICPMeetingInfo();
        //    regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewCreateUpdateMeetings", UriKind.Relative));
            
        //}

        //#endregion

        //#region INavigationAware methods

        //public void OnNavigatedFrom(NavigationContext navigationContext)
        //{
        //    navigationContext.NavigationService.Region.Context = MeetingNavigationObjectProperty;
        //}

        //public void OnNavigatedTo(NavigationContext navigationContext)
        //{
        //}

        //public bool IsNavigationTarget(NavigationContext navigationContext)
        //{
        //    return true;
        //}

        //#endregion

        //private void GetMeetingsCallBackMethod(List<MeetingInfo> val)
        //{
        //    ObservableCollection<MeetingInfo> MeetingInfoCollObj = new ObservableCollection<MeetingInfo>(val);
        //    ObservableCollection<ICPMeetingInfo> ICPMeetingInfoCollObj = new ObservableCollection<ICPMeetingInfo>();
        //    foreach (MeetingInfo minfo in MeetingInfoCollObj)
        //    {
        //        ICPMeetingInfo ICPMeetingInfoObj = new ICPMeetingInfo
        //        {
        //            MeetingID = minfo.MeetingID,
        //            MeetingDateProperty = minfo.MeetingDateTime,
        //            MeetingTimeProperty = minfo.MeetingDateTime,
        //            MeetingClosedDateProperty = minfo.MeetingClosedDateTime,
        //            MeetingClosedTimeProperty = minfo.MeetingClosedDateTime,
        //            DescriptionProperty = minfo.MeetingDescription
        //        };

        //        ICPMeetingInfoCollObj.Add(ICPMeetingInfoObj);
        //    }

        //    MeetingListProperty = ICPMeetingInfoCollObj;
        //}


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
