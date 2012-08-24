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
using System.ComponentModel.Composition;
//using Ashmore.Emm.GreenField.ICP.Meeting.Module.Model;
using GreenField.Gadgets.Models;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
//using Ashmore.Emm.GreenField.BusinessLogic;
using GreenField.ServiceCaller;
//using Ashmore.Emm.GreenField.Common;
using GreenField.Common;
using Microsoft.Practices.Prism.Events;
using GreenField.Gadgets.Views;
using Microsoft.Practices.Prism.Logging;


namespace GreenField.Gadgets.ViewModels
{
    public class ViewModelCreateUpdateMeetings : NotificationObject
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


        private ViewPluginFlagEnumeration _viewPluginFlag;
        public ViewPluginFlagEnumeration ViewPluginFlagProperty
        {
            get
            {
                return _viewPluginFlag;
            }
            set
            {
                _viewPluginFlag = value;
                RaisePropertyChanged(() => this.HeaderProperty);
                RaisePropertyChanged(() => this.FinalizeButtonContentProperty);
            }
        }

        private string _Header;
        public string HeaderProperty
        {
            get
            {
                switch (ViewPluginFlagProperty)
                {
                    case ViewPluginFlagEnumeration.Create:
                        _Header = "Create Investment Commitee (IC) Meeting";
                        break;
                    case ViewPluginFlagEnumeration.Update:
                        _Header = "Update Investment Commitee (IC) Meeting";
                        break;
                    default:
                        break;
                }
                return _Header;
            }
        }

        private string _finalizeButtonContent;
        public string FinalizeButtonContentProperty
        {
            get
            {
                switch (ViewPluginFlagProperty)
                {
                    case ViewPluginFlagEnumeration.Create:
                        _finalizeButtonContent = "Create";
                        break;
                    case ViewPluginFlagEnumeration.Update:
                        _finalizeButtonContent = "Update";
                        break;
                    default:
                        break;
                }
                return _finalizeButtonContent;
            }
        }

        //private ICPMeetingInfo _meetingInfo;
        //public ICPMeetingInfo MeetingInfo
        //{
        //    get
        //    {
        //        if (_meetingInfo == null)
        //            return new ICPMeetingInfo();
        //        return _meetingInfo;
        //    }
        //    set
        //    {
        //        _meetingInfo = value;
        //        RaisePropertyChanged(() => this.MeetingInfo);
        //    }
        //}
        
        #region Constructor
        
        public ViewModelCreateUpdateMeetings(DashboardGadgetParam param)
        {
            _dbInteractivity = param.DBInteractivity;
            _logger = param.LoggerFacade;
            _eventAggregator = param.EventAggregator;
        }
        
        #endregion       


        public ICommand FinalizeCommand
        {
            get { return new DelegateCommand<object>(ICPMeetingsFinalizeItem); }
        }

        public ICommand BackCommand
        {
            get { return new DelegateCommand<object>(ICPMeetingsNavigateBack); }
        }

        private void ICPMeetingsFinalizeItem(object param)
        {
            switch (ViewPluginFlagProperty)
            {
                case ViewPluginFlagEnumeration.Create:
                    //_dbInteractivity.CreateMeeting(MeetingInfo.GetMeetingInfo(), (msg) => { MessageBox.Show(msg); });
                    break;
                case ViewPluginFlagEnumeration.Update:
                    //_dbInteractivity.UpdateMeeting(MeetingInfo.GetMeetingInfo(), (msg) => { MessageBox.Show(msg); });
                    break;
            }

            //Navigate to Meeting List View.
        }

        private void ICPMeetingsNavigateBack(object param)
        {
            regionManager.RequestNavigate("MainRegion", new Uri("ViewMeetings", UriKind.Relative));
        }

        #region INavigationAware methods

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            //ViewPluginFlagProperty = (navigationContext.NavigationService.Region.Context as ICNavigationInfo).ViewPluginFlagEnumerationObject;
            //MeetingInfo = (navigationContext.NavigationService.Region.Context as ICNavigationInfo).MeetingInfoObject;
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
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
