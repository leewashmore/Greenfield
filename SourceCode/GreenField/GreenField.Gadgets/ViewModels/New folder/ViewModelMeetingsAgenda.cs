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
using System.Collections.ObjectModel;
using Ashmore.Emm.GreenField.ICP.Meeting.Module.Model;
using Microsoft.Practices.Prism.Commands;
using System.ComponentModel.Composition;
using Microsoft.Practices.Prism.Regions;
using Ashmore.Emm.GreenField.Common;
using Ashmore.Emm.GreenField.BusinessLogic;
using System.Collections.Generic;
using Ashmore.Emm.GreenField.BusinessLogic.MeetingServiceReference;

namespace Ashmore.Emm.GreenField.ICP.Meeting.Module.ViewModels
{
    [Export]
    public class ViewModelMeetingsAgenda : NotificationObject, INavigationAware
    {
        #region Properties

        [Import]
        public IRegionManager regionManager { private get; set; }

        ManageMeetings _manageMeetings;

        private string _meetingDate;
        public string MeetingDateProperty
        {
            get { return _meetingDate; }
            set
            {
                if (_meetingDate != value)
                {
                    _meetingDate = value;
                    RaisePropertyChanged(() => this.MeetingDateProperty);
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
                    RaisePropertyChanged(() => this.AcceptCommand);
                    RaisePropertyChanged(() => this.WithdrawCommand);
                    RaisePropertyChanged(() => this.FinalizeCommand);

                }
            }
        }

        private ICPNavigationInfo _meetingNavigationObject = new ICPNavigationInfo();
        public ICPNavigationInfo MeetingNavigationObjectProperty
        {
            get { return _meetingNavigationObject; }
            set { _meetingNavigationObject = value; }
        }

        private ObservableCollection<ICPPresentationInfo> _presentationList;
        public ObservableCollection<ICPPresentationInfo> PresentationListProperty
        {
            get 
            {
                if (_presentationList == null)
                {
                    //_manageMeetings.GetMeetings(GetMeetingsCallBackMethod);
                   
                }            
                return _presentationList; 
            }
            set
            {
                if (_presentationList != value)
                {
                    _presentationList = value;
                    RaisePropertyChanged(() => this.PresentationListProperty);
                }
            }
        }

        public ICommand EditCommand
        {
            get { return new DelegateCommand<object>(ICPMeetingsAgendaEditItem, ItemSelectedValidation); }
        }

        public ICommand AcceptCommand
        {
            get { return new DelegateCommand<object>(ICPMeetingsAgendaAcceptItem, ItemSelectedValidation); }
        }

        public ICommand WithdrawCommand
        {
            get { return new DelegateCommand<object>(ICPMeetingsAgendaWithdrawItem, ItemSelectedValidation); }
        }

        public ICommand FinalizeCommand
        {
            get { return new DelegateCommand<object>(ICPMeetingsAgendaFinalizeItem, ItemSelectedValidation); }
        }

        public ICommand BackCommand
        {
            get { return new DelegateCommand<object>(ICPMeetingsAgendaNavigateBack); }
        }

        #endregion

        #region Private Members

        //ObservableCollection<ICPPresentationInfo> DataContextObj = new ObservableCollection<ICPPresentationInfo>();
        
        #endregion

        #region Constructor

        [ImportingConstructor]
        public ViewModelMeetingsAgenda(ManageMeetings manageMeetings)
        {
            _manageMeetings = manageMeetings;
            //DataContextObj.Add(new ICPPresentationInfo { PresentationID = 0, Presenter = "Bryan", Company = "Belle International", Ticker = "BIH", Status = "Pending Documents", Country = "China", Industry = "Coal" });
            //DataContextObj.Add(new ICPPresentationInfo { PresentationID = 1, Presenter = "Alejandro", Company = "Rozneft", Ticker = "YNDX US", Status = "Ready (Closed)", Country = "Brazil", Industry = "Plastics" });
            //DataContextObj.Add(new ICPPresentationInfo { PresentationID = 2, Presenter = "John", Company = "Lukoil Holdings", Ticker = "PTBR", Status = "Pending Documents", Country = "Russia", Industry = "Networking" });
            //DataContextObj.Add(new ICPPresentationInfo { PresentationID = 3, Presenter = "Bryan", Company = "Belon", Ticker = "BIHL", Status = "Ready (Closed)", Country = "Mexico", Industry = "Oil" });
            //PresentationListProperty = DataContextObj;
            

        }
        
        #endregion

        #region ICommand Methods

        private bool ItemSelectedValidation(object param)
        {
            return GridActiveItemProperty != null ? true : false;
        }

        private void ICPMeetingsAgendaEditItem(object param)
        {
            if (MeetingNavigationObjectProperty == null) MeetingNavigationObjectProperty = new ICPNavigationInfo();
            MeetingNavigationObjectProperty.ViewPluginFlagEnumerationObject = ViewPluginFlagEnumeration.Update;
            MeetingNavigationObjectProperty.PresentationInfoObject = GridActiveItemProperty as ICPPresentationInfo;
            MeetingNavigationObjectProperty.MeetingInfoObject = null;
            regionManager.RequestNavigate(RegionNames.MainRegion, new Uri("ViewCreateUpdatePresentations", UriKind.Relative));

        }

        private void ICPMeetingsAgendaAcceptItem(object param)
        {
            //Navigate to PopUp with alternate meeting date options, Presentation ID as input
        }
        
        private void ICPMeetingsAgendaWithdrawItem(object param)
        {
            //Navigate to PopUp confirming deletion of presentation request
        }
        
        private void ICPMeetingsAgendaFinalizeItem(object param)
        {
            //Navigate to FinalVote View, Presentation ID as input
        }

        private void ICPMeetingsAgendaNavigateBack(object param)
        {
            MeetingNavigationObjectProperty = null;
            regionManager.RequestNavigate(RegionNames.MainRegion, new Uri("ViewMeetings", UriKind.Relative));
        }

        #endregion

        #region INavigationAware methods

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            navigationContext.NavigationService.Region.Context = MeetingNavigationObjectProperty;
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            MeetingDateProperty = (navigationContext.NavigationService.Region.Context as ICPNavigationInfo).MeetingInfoObject.MeetingDateProperty.ToString("MMMM dd, yyyy");
            long MeetingID = (navigationContext.NavigationService.Region.Context as ICPNavigationInfo).MeetingInfoObject.MeetingID;
            _manageMeetings.GetPresentationsByMeetingID(MeetingID, GetPresentationsCallBackMethod);
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        #endregion

        private void GetPresentationsCallBackMethod(List<PresentationInfoResult> val)
        {
            ObservableCollection<PresentationInfoResult> PresentationInfoCollObj = new ObservableCollection<PresentationInfoResult>(val);
            ObservableCollection<ICPPresentationInfo> ICPPresentationInfoCollObj = new ObservableCollection<ICPPresentationInfo>();
            foreach (PresentationInfoResult pinfo in PresentationInfoCollObj)
            {
                ICPPresentationInfo ICPPresentationInfoObj = new ICPPresentationInfo(pinfo);
                ICPPresentationInfoCollObj.Add(ICPPresentationInfoObj);
            }

            PresentationListProperty = ICPPresentationInfoCollObj;
        }
    }
}
