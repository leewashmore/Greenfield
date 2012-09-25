﻿using System;
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
using GreenField.ServiceCaller;
using Microsoft.Practices.Prism.ViewModel;
using GreenField.ServiceCaller.MeetingDefinitions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Commands;
using GreenField.Gadgets.Models;
using Microsoft.Practices.Prism.Regions;
using GreenField.Common;
using GreenField.Gadgets.Views;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.ServiceLocation;
using GreenField.Gadgets.Helpers;
using GreenField.DataContracts;

namespace GreenField.Gadgets.ViewModels
{
    public class ViewModelPresentations : NotificationObject
    {
        #region Fields
        private IRegionManager _regionManager;
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

        private Boolean _sendChangeDateNotification = false;
        private DateTime? _originalPresentationDate = null;
        private DateTime? _updatedPresentationDate = null;
        #endregion

        #region Constructor
        public ViewModelPresentations(DashboardGadgetParam param)
        {
            _dbInteractivity = param.DBInteractivity;
            _logger = param.LoggerFacade;
            _eventAggregator = param.EventAggregator;
            _regionManager = param.RegionManager;
        }
        #endregion

        #region Properties
        #region IsActive
        /// <summary>
        /// IsActive is true when parent control is displayed on UI
        /// </summary>
        private bool _isActive;
        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
                if (value)
                {
                    Initialize();
                }
            }
        } 
        #endregion

        #region Busy Indicator Notification
        /// <summary>
        /// Displays/Hides busy indicator to notify user of the on going process
        /// </summary>
        private bool _busyIndicatorIsBusy = false;
        public bool BusyIndicatorIsBusy
        {
            get { return _busyIndicatorIsBusy; }
            set
            {
                _busyIndicatorIsBusy = value;
                RaisePropertyChanged(() => this.BusyIndicatorIsBusy);
            }
        }

        /// <summary>
        /// Stores the message displayed over the busy indicator to notify user of the on going process
        /// </summary>
        private string _busyIndicatorContent;
        public string BusyIndicatorContent
        {
            get { return _busyIndicatorContent; }
            set
            {
                _busyIndicatorContent = value;
                RaisePropertyChanged(() => this.BusyIndicatorContent);
            }
        }
        #endregion        

        #region Binded
        private List<ICPresentationOverviewData> _iCPresentationOverviewInfo;
        public List<ICPresentationOverviewData> ICPresentationOverviewInfo
        {
            get { return _iCPresentationOverviewInfo; }
            set
            {
                _iCPresentationOverviewInfo = value;
                RaisePropertyChanged(() => this.ICPresentationOverviewInfo);
            }
        }

        private ICPresentationOverviewData _selectedPresentationOverviewInfo;
        public ICPresentationOverviewData SelectedPresentationOverviewInfo
        {
            get { return _selectedPresentationOverviewInfo; }
            set
            {
                _selectedPresentationOverviewInfo = value;
                RaisePropertyChanged(() => this.SelectedPresentationOverviewInfo);
                ICNavigation.Update(ICNavigationInfo.PresentationOverviewInfo, value);
                SelectionRaisePropertyChanged();
            }
        }

        private List<MeetingInfo> _meetingInfoDates;
        public List<MeetingInfo> MeetingInfoDates
        {
            get { return _meetingInfoDates; }
            set
            {
                if (value != null)
                {
                    _meetingInfoDates = value;
                    RaisePropertyChanged(() => this.MeetingInfoDates);
                    RaisePropertyChanged(() => this.NewCommand);
                }
            }
        }

        private MeetingInfo _selectedMeetingInfoDate;
        public MeetingInfo SelectedMeetingInfoDate
        {
            get { return _selectedMeetingInfoDate; }
            set
            {
                _selectedMeetingInfoDate = value;
                RaisePropertyChanged(() => this.SelectedMeetingInfoDate);
                RaisePropertyChanged(() => this.NewCommand);

                if (value != null)
                {                    
                    ICNavigation.Update(ICNavigationInfo.MeetingInfo, value);
                }
            }
        }         
        #endregion

        #region ICommand Properties
        public ICommand ChangeDateCommand
        {
            get { return new DelegateCommand<object>(ChangeDateCommandMethod, ChangeDateCommandValidationMethod); }
        }

        public ICommand DecisionEntryCommand
        {
            get { return new DelegateCommand<object>(DecisionEntryCommandMethod, DecisionEntryCommandValidationMethod); }
        }
     
        public ICommand UploadCommand
        {
            get { return new DelegateCommand<object>(UploadCommandMethod, UploadCommandValidationMethod); }
        }

        public ICommand EditCommand
        {
            get { return new DelegateCommand<object>(EditCommandMethod, EditCommandValidationMethod); }
        }
      
        public ICommand WithdrawCommand
        {
            get { return new DelegateCommand<object>(WithdrawCommandMethod, WithdrawCommandValidationMethod); }
        }

        public ICommand ViewCommand
        {
            get { return new DelegateCommand<object>(ViewCommandMethod, ViewCommandValidationMethod); }
        }    

        public ICommand NewCommand
        {
            get { return new DelegateCommand<object>(NewCommandMethod, NewCommandValidationMethod); }
        }
        #endregion        

        #endregion

        #region ICommand Methods
        private bool ChangeDateCommandValidationMethod(object param)
        {
            if (UserSession.SessionManager.SESSION == null
                || SelectedPresentationOverviewInfo == null)
                return false;

            return (UserSession.SessionManager.SESSION.Roles.Contains(MemberGroups.IC_ADMIN)
                || UserSession.SessionManager.SESSION.Roles.Contains(MemberGroups.IC_CHIEF_EXECUTIVE))
                && SelectedPresentationOverviewInfo.StatusType != StatusType.WITHDRAWN
                && SelectedPresentationOverviewInfo.StatusType != StatusType.FINAL;
        }

        private void ChangeDateCommandMethod(object param)
        {
            List<DateTime> proposedMeetingDates = MeetingInfoDates.Select(record => record.MeetingDateTime.ToLocalTime().Date).ToList();

            if (! UserSession.SessionManager.SESSION.Roles.Contains("CHIEF_EXECUTIVE"))
            {
                proposedMeetingDates = proposedMeetingDates.OrderBy(record => record).ToList();
                for (int index = 0; index < proposedMeetingDates.Count; index++)
                {
                    if (proposedMeetingDates[index] < Convert.ToDateTime(SelectedPresentationOverviewInfo.MeetingDateTime).Date)
                    {
                        proposedMeetingDates.RemoveAt(index);
                        index--;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            ChildViewPresentationDateChangeEdit childViewPresentationDateChangeEdit = new ChildViewPresentationDateChangeEdit(proposedMeetingDates
                , SelectedPresentationOverviewInfo.MeetingDateTime);
            childViewPresentationDateChangeEdit.Show();

            childViewPresentationDateChangeEdit.Unloaded += (se, e) =>
            {
                if (childViewPresentationDateChangeEdit.DialogResult == true)
                {
                    if (childViewPresentationDateChangeEdit.SelectedPresentationDateTime.Date
                        == Convert.ToDateTime(SelectedPresentationOverviewInfo.MeetingDateTime).Date)
                        return;

                    Prompt.ShowDialog("Confirm presentation DateTime from " +
                        Convert.ToDateTime(SelectedPresentationOverviewInfo.MeetingDateTime).ToLocalTime().ToString("MM-dd-yyyy h:mm tt") +
                        " to " + childViewPresentationDateChangeEdit.SelectedPresentationDateTime
                            .Add(Convert.ToDateTime(SelectedPresentationOverviewInfo.MeetingDateTime).TimeOfDay).ToLocalTime().ToString("MM-dd-yyyy h:mm tt"),
                        "Change Presentation Date", MessageBoxButton.OKCancel, (result) =>
                    {
                        if (result == MessageBoxResult.OK)
                        {
                            MeetingInfo meetingInfo = MeetingInfoDates
                                .Where(record => record.MeetingDateTime.ToLocalTime().Date == childViewPresentationDateChangeEdit.SelectedPresentationDateTime.Date)
                                .FirstOrDefault();

                            if (_dbInteractivity != null)
                            {
                                _sendChangeDateNotification = childViewPresentationDateChangeEdit.AlertNotification;
                                _originalPresentationDate = Convert.ToDateTime(SelectedPresentationOverviewInfo.MeetingDateTime);
                                _updatedPresentationDate = childViewPresentationDateChangeEdit.SelectedPresentationDateTime
                                    .Add(Convert.ToDateTime(SelectedPresentationOverviewInfo.MeetingDateTime).TimeOfDay);

                                BusyIndicatorNotification(true, "Updating Presentation date for the selected presentation...");
                                _dbInteractivity.UpdateMeetingPresentationDate(UserSession.SessionManager.SESSION.UserName, SelectedPresentationOverviewInfo.PresentationID
                                    , meetingInfo, UpdateMeetingPresentationDateCallbackMethod);
                            }                            
                        }
                    });


                }
            };
        }        

        private bool DecisionEntryCommandValidationMethod(object param)
        {
            if (UserSession.SessionManager.SESSION == null
                || SelectedPresentationOverviewInfo == null)
                return false;
            return UserSession.SessionManager.SESSION.Roles.Contains(MemberGroups.IC_ADMIN)
                && SelectedPresentationOverviewInfo.StatusType == StatusType.CLOSED_FOR_VOTING;
        }

        private void DecisionEntryCommandMethod(object param)
        {
            _regionManager.RequestNavigate(RegionNames.MAIN_REGION, "ViewDashboardInvestmentCommitteeDecisionEntry");
        }

        private bool UploadCommandValidationMethod(object param)
        {
            if (UserSession.SessionManager.SESSION == null 
                || SelectedPresentationOverviewInfo == null)
                return false;

            bool userRoleValidation = UserSession.SessionManager.SESSION.UserName == SelectedPresentationOverviewInfo.Presenter;
            bool statusValidation = SelectedPresentationOverviewInfo.StatusType == StatusType.IN_PROGRESS;
            return userRoleValidation && statusValidation;
        }

        private void UploadCommandMethod(object param)
        {
            ICNavigation.Update(ICNavigationInfo.ViewPluginFlagEnumerationInfo, ViewPluginFlagEnumeration.Upload);
            _eventAggregator.GetEvent<ToolboxUpdateEvent>().Publish(DashboardCategoryType.INVESTMENT_COMMITTEE_EDIT_PRESENTATION);
            _regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardInvestmentCommitteeEditPresentations", UriKind.Relative));
        }

        private bool EditCommandValidationMethod(object param)
        {
            if (UserSession.SessionManager.SESSION == null
                || SelectedPresentationOverviewInfo == null)
                return false;

            bool userRoleValidation = UserSession.SessionManager.SESSION.UserName == SelectedPresentationOverviewInfo.Presenter
                && !(UserSession.SessionManager.SESSION.Roles.Contains(MemberGroups.IC_ADMIN));
            bool statusValidation = SelectedPresentationOverviewInfo.StatusType == StatusType.IN_PROGRESS
                || SelectedPresentationOverviewInfo.StatusType == StatusType.READY_FOR_VOTING;

            return userRoleValidation && statusValidation;
        }

        private void EditCommandMethod(object param)
        {
            ICNavigation.Update(ICNavigationInfo.ViewPluginFlagEnumerationInfo, ViewPluginFlagEnumeration.Edit);
            _eventAggregator.GetEvent<ToolboxUpdateEvent>().Publish(DashboardCategoryType.INVESTMENT_COMMITTEE_EDIT_PRESENTATION);
            _regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardInvestmentCommitteeEditPresentations", UriKind.Relative));
        }

        private bool WithdrawCommandValidationMethod(object param)
        {
            if (UserSession.SessionManager.SESSION == null
                || SelectedPresentationOverviewInfo == null)
                return false;

            bool userRoleValidation = UserSession.SessionManager.SESSION.UserName == SelectedPresentationOverviewInfo.Presenter;
            bool statusValidation = SelectedPresentationOverviewInfo.StatusType == StatusType.IN_PROGRESS
                || SelectedPresentationOverviewInfo.StatusType == StatusType.READY_FOR_VOTING;

            return userRoleValidation && statusValidation;
        }

        private void WithdrawCommandMethod(object param)
        {
            Prompt.ShowDialog("Please confirm withdrawal of selected presentation", "Confirmation", MessageBoxButton.OKCancel, (result) =>
            {
                if (result == MessageBoxResult.OK)
                {
                    if (_dbInteractivity != null)
                    {
                        _dbInteractivity.SetICPPresentationStatus(UserSession.SessionManager.SESSION.UserName,
                                   SelectedPresentationOverviewInfo.PresentationID, StatusType.WITHDRAWN, SetICPPresentationStatusCallbackMethod);
                    } 
                }
            });
            
        }   

        private bool ViewCommandValidationMethod(object param)
        {
            if (UserSession.SessionManager.SESSION == null
                || SelectedPresentationOverviewInfo == null)
                return false;

            return SelectedPresentationOverviewInfo.StatusType != StatusType.IN_PROGRESS
                && SelectedPresentationOverviewInfo.StatusType != StatusType.WITHDRAWN;
        }

        private void ViewCommandMethod(object param)
        {
            bool userRoleValidation = UserSession.SessionManager.SESSION.Roles.Contains("IC_MEMBER_VOTING");

            if (userRoleValidation && SelectedPresentationOverviewInfo.StatusType == StatusType.READY_FOR_VOTING)
                ICNavigation.Update(ICNavigationInfo.ViewPluginFlagEnumerationInfo, ViewPluginFlagEnumeration.Vote);
            else
                ICNavigation.Update(ICNavigationInfo.ViewPluginFlagEnumerationInfo, ViewPluginFlagEnumeration.View);
                        
            _eventAggregator.GetEvent<ToolboxUpdateEvent>().Publish(DashboardCategoryType.INVESTMENT_COMMITTEE_VOTE);
            _regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardInvestmentCommitteeVote", UriKind.Relative));
        }

        private bool NewCommandValidationMethod(object param)
        {
            if (UserSession.SessionManager.SESSION == null)
                return false;

            return SelectedMeetingInfoDate != null && (!UserSession.SessionManager.SESSION.Roles.Contains(MemberGroups.IC_ADMIN));
        }

        private void NewCommandMethod(object param)
        {
            ICNavigation.Update(ICNavigationInfo.ViewPluginFlagEnumerationInfo, ViewPluginFlagEnumeration.Create);
            _eventAggregator.GetEvent<ToolboxUpdateEvent>().Publish(DashboardCategoryType.INVESTMENT_COMMITTEE_NEW_PRESENTATION);
            _regionManager.RequestNavigate(RegionNames.MAIN_REGION, new Uri("ViewDashboardInvestmentCommitteeNew", UriKind.Relative));
        }
        #endregion
        
        #region Helper Methods

        public void Initialize()
        {
            SelectedMeetingInfoDate = null;
            SelectionRaisePropertyChanged();
            if (_dbInteractivity != null && IsActive)
            {
                BusyIndicatorNotification(true, "Retrieving Presentation Overview Information...");
                _dbInteractivity.RetrievePresentationOverviewData(RetrievePresentationOverviewDataCallbackMethod);
            }
        }
        
        private void SelectionRaisePropertyChanged()
        {
            RaisePropertyChanged(() => this.EditCommand);
            RaisePropertyChanged(() => this.UploadCommand);
            RaisePropertyChanged(() => this.NewCommand);
            RaisePropertyChanged(() => this.WithdrawCommand);
            RaisePropertyChanged(() => this.ViewCommand);
            RaisePropertyChanged(() => this.ChangeDateCommand);
            RaisePropertyChanged(() => this.DecisionEntryCommand);
        }

        public void BusyIndicatorNotification(bool showBusyIndicator = false, String message = null)
        {
            if (message != null)
                BusyIndicatorContent = message;

            BusyIndicatorIsBusy = showBusyIndicator;
        }

        #endregion

        #region CallBack Methods
        private void RetrievePresentationOverviewDataCallbackMethod(List<ICPresentationOverviewData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    ICPresentationOverviewInfo = result;
                    if (_dbInteractivity != null)
                    {
                        BusyIndicatorNotification(true, "Retrieving available meeting dates...");
                        _dbInteractivity.GetAvailablePresentationDates(GetAvailablePresentationDatesCallbackMethod); 
                    }
                }
                else
                {
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                    BusyIndicatorNotification();
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
                BusyIndicatorNotification();
            }
            finally
            {
                Logging.LogEndMethod(_logger, methodNamespace);
                //BusyIndicatorNotification();
            }            
        }

        private void GetAvailablePresentationDatesCallbackMethod(List<MeetingInfo> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    MeetingInfoDates = result;                    
                }
                else
                {
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);                    
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);                
            }
            finally
            {
                Logging.LogEndMethod(_logger, methodNamespace);
                BusyIndicatorNotification();
            }
        }

        private void UpdateMeetingPresentationDateCallbackMethod(Boolean? result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result == true)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    if (SelectedPresentationOverviewInfo != null && _dbInteractivity != null && _sendChangeDateNotification)
                    {
                        BusyIndicatorNotification(true, "Retrieving presentation associated users...");
                        _dbInteractivity.RetrievePresentationVoterData(SelectedPresentationOverviewInfo.PresentationID, RetrievePresentationVoterDataCallbackMethod, true);
                    }
                    else
                    {
                        Initialize();
                    }
                }
                else
                {
                    Prompt.ShowDialog("An Error ocurred while updating presentation date");
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                    BusyIndicatorNotification();
                    Initialize();
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
                BusyIndicatorNotification();
                Initialize();
            }
            finally
            {
                Logging.LogEndMethod(_logger, methodNamespace);                
            }
        }

        private void SetICPPresentationStatusCallbackMethod(Boolean? result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    if (result == true)
                    {
                        Prompt.ShowDialog("Presentation was successfully withdrawn");
                        Initialize();
                    }
                }
                else
                {
                    Prompt.ShowDialog("An Error ocurred while updating presentation date");
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                    BusyIndicatorNotification();
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
                BusyIndicatorNotification();
            }
            finally
            {
                Logging.LogEndMethod(_logger, methodNamespace);
            }
        }

        private void RetrievePresentationVoterDataCallbackMethod(List<VoterInfo> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);

                    List<String> userNames = result.Where(record => record.PostMeetingFlag == false).Select(record => record.Name).ToList();
                    if (_dbInteractivity != null)
                    {
                        BusyIndicatorNotification(true, "Retrieving user credentials...");
                        _dbInteractivity.GetUsersByNames(userNames, GetUsersByNamesCallbackMethod);
                    }
                    else
                    {
                        Initialize();
                    }
                }
                else
                {
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                    BusyIndicatorNotification();
                    Initialize();
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
                BusyIndicatorNotification();
                Initialize();
            }
            finally
            {
                Logging.LogEndMethod(_logger, methodNamespace);                
            }
        }

        private void GetUsersByNamesCallbackMethod(List<MembershipUserInfo> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);

                    MembershipUserInfo presenterCredentials = result.Where(record => record.UserName.ToLower() 
                        == SelectedPresentationOverviewInfo.Presenter.ToLower()).FirstOrDefault();
                    if (presenterCredentials == null)
                        throw new Exception("Presenter credentials could not be retrieved. Alert notification was not successful.");

                    String emailtTo = presenterCredentials.Email;

                    String emailCc = String.Join("|", result.Where(record => record.UserName.ToLower() != SelectedPresentationOverviewInfo.Presenter.ToLower())
                        .Select(record => record.Email).ToArray());

                    String messageSubject = "Presentation Date Change Notification – " + SelectedPresentationOverviewInfo.SecurityName;
                    String messageBody = "The Investment Committee Admin has changed the presentation date for "
                        + SelectedPresentationOverviewInfo.SecurityName + " from "
                        + Convert.ToDateTime(_originalPresentationDate).ToString("MMMM dd, yyyy") + " UTC to "
                        + Convert.ToDateTime(_updatedPresentationDate).ToString("MMMM dd, yyyy") + " UTC*. Please contact the Investment Committee Admin with any questions or concerns.";

                    if (_dbInteractivity != null)
                    {
                        BusyIndicatorNotification(true, "Processing alert notification...");
                        _dbInteractivity.SetMessageInfo(emailtTo, emailCc, messageSubject, messageBody, null
                            , UserSession.SessionManager.SESSION.UserName, SetMessageInfoCallbackMethod);                        
                    }
                    else
                    {
                        Initialize();   
                    }
                }
                else
                {
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);
                    BusyIndicatorNotification();
                    Initialize();
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);
                BusyIndicatorNotification();
                Initialize();
            }
            finally
            {
                Logging.LogEndMethod(_logger, methodNamespace);
            }
        }

        private void SetMessageInfoCallbackMethod(Boolean? result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result == true)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);                    
                }
                else
                {
                    Logging.LogMethodParameterNull(_logger, methodNamespace, 1);                    
                }
            }
            catch (Exception ex)
            {
                Prompt.ShowDialog("Message: " + ex.Message + "\nStackTrace: " + Logging.StackTraceToString(ex), "Exception", MessageBoxButton.OK);
                Logging.LogException(_logger, ex);                
            }
            finally
            {
                Logging.LogEndMethod(_logger, methodNamespace);
                BusyIndicatorNotification();
                Initialize();
            }
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
