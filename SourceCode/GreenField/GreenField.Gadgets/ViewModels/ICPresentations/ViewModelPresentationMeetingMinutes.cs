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
using GreenField.ServiceCaller;
using Microsoft.Practices.Prism.ViewModel;
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
using GreenField.ServiceCaller.MeetingDefinitions;
using System.IO;

namespace GreenField.Gadgets.ViewModels
{
    public class ViewModelPresentationMeetingMinutes : NotificationObject
    {
        #region Fields
        private IRegionManager _regionManager;
       
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
        public ViewModelPresentationMeetingMinutes(DashboardGadgetParam param)
        {
            _dbInteractivity = param.DBInteractivity;
            _logger = param.LoggerFacade;
            _eventAggregator = param.EventAggregator;
            _regionManager = param.RegionManager;

            if (_dbInteractivity != null)
            {
                BusyIndicatorNotification(true, "Retrieving Meetings with 'Closed for Voting' Investment Committee presentation status");
                _dbInteractivity.RetrieveMeetingInfoByPresentationStatus("Closed for Voting", RetrieveMeetingInfoByPresentationStatusCallbackMethod); 
            }
        }
        #endregion

        public List<String> AttendanceTypeInfo
        {
            get { return new List<string> { "Attended", "Video Conference", "Tele Conference", "Not Present" }; }
        }

        private List<MeetingInfo> _closedForVotingMeetingInfo;
        public List<MeetingInfo> ClosedForVotingMeetingInfo
        {
            get { return _closedForVotingMeetingInfo; }
            set 
            { 
                _closedForVotingMeetingInfo = value;
                RaisePropertyChanged(() => this.ClosedForVotingMeetingInfo);                
            }
        }

        private MeetingInfo _selectedClosedForVotingMeetingInfo;
        public MeetingInfo SelectedClosedForVotingMeetingInfo
        {
            get { return _selectedClosedForVotingMeetingInfo; }
            set
            {
                _selectedClosedForVotingMeetingInfo = value;
                RaisePropertyChanged(() => this.SelectedClosedForVotingMeetingInfo);
                if (value != null && _dbInteractivity != null)
                {
                    BusyIndicatorNotification(true, "Retrieving Presentation and Attendance details for the selected meeting");
                    _dbInteractivity.RetrieveMeetingMinuteDetails(value.MeetingID, RetrieveMeetingMinuteDetailsCallbackMethod); 
                }
            }
        }

        private List<MeetingMinuteData> _closedForVotingMeetingMinuteInfo;
        public List<MeetingMinuteData> ClosedForVotingMeetingMinuteInfo
        {
            get { return _closedForVotingMeetingMinuteInfo; }
            set
            {
                _closedForVotingMeetingMinuteInfo = value;
                RaisePropertyChanged(() => this.ClosedForVotingMeetingMinuteInfo);
                if (value == null)
                {
                    ClosedForVotingMeetingMinuteDistinctPresentationInfo = null;
                    ClosedForVotingMeetingMinuteDistinctAttendanceInfo = null;
                }
                else
                {
                    List<Int64> distinctPresentationIds = value.Select(record => record.PresentationID).ToList()
                        .Distinct().ToList();
                    List<MeetingMinuteData> distinctPresentationRecords = new List<MeetingMinuteData>();
                    foreach (Int64 presentationId in distinctPresentationIds)
                    {
                        distinctPresentationRecords.Add(
                            value.Where(record => record.PresentationID == presentationId).FirstOrDefault());
                    }

                    ClosedForVotingMeetingMinuteDistinctPresentationInfo = distinctPresentationRecords;

                    List<String> distinctVoterNames = value.Where(record => record.Name != null).Select(record => record.Name).ToList()
                        .Distinct().ToList();
                    List<MeetingMinuteData> distinctVoterRecords = new List<MeetingMinuteData>();
                    foreach (String name in distinctVoterNames)
                    {
                        distinctVoterRecords.Add(
                            value.Where(record => record.Name == name).FirstOrDefault());
                    }

                    ClosedForVotingMeetingMinuteDistinctAttendanceInfo = distinctVoterRecords;
                    
                }
            }
        }

        private List<MeetingMinuteData> _closedForVotingMeetingMinuteDistinctPresentationInfo;
        public List<MeetingMinuteData> ClosedForVotingMeetingMinuteDistinctPresentationInfo
        {
            get { return _closedForVotingMeetingMinuteDistinctPresentationInfo; }
            set
            {
                _closedForVotingMeetingMinuteDistinctPresentationInfo = value;
                RaisePropertyChanged(() => this.ClosedForVotingMeetingMinuteDistinctPresentationInfo);
            }
        }

        private List<MeetingMinuteData> _closedForVotingMeetingMinuteDistinctAttendanceInfo;
        public List<MeetingMinuteData> ClosedForVotingMeetingMinuteDistinctAttendanceInfo
        {
            get { return _closedForVotingMeetingMinuteDistinctAttendanceInfo; }
            set
            {
                _closedForVotingMeetingMinuteDistinctAttendanceInfo = value;
                RaisePropertyChanged(() => this.ClosedForVotingMeetingMinuteDistinctAttendanceInfo);
            }
        }

        public MeetingAttachedFileStreamData SelectedIndustryReportFileStreamData { get; set; }
        public MeetingAttachedFileStreamData SelectedOtherDocumentFileStreamData { get; set; }
        public MeetingAttachedFileStreamData SelectedDeleteFileStreamData { get; set; }

        private List<MeetingAttachedFileStreamData> _closedForVotingMeetingAttachedFileInfo;
        public List<MeetingAttachedFileStreamData> ClosedForVotingMeetingAttachedFileInfo
        {
            get { return _closedForVotingMeetingAttachedFileInfo; }
            set
            {
                _closedForVotingMeetingAttachedFileInfo = value;
                RaisePropertyChanged(() => this.ClosedForVotingMeetingAttachedFileInfo);                
            }
        }

        private String _selectedIndustryReports;
        public String SelectedIndustryReports
        {
            get { return _selectedIndustryReports; }
            set
            {
                _selectedIndustryReports = value;
                RaisePropertyChanged(() => this.SelectedIndustryReports);
            }
        }

        private String _selectedOtherReports;
        public String SelectedOtherReports
        {
            get { return _selectedOtherReports; }
            set
            {
                _selectedOtherReports = value;
                RaisePropertyChanged(() => this.SelectedOtherReports);
            }
        }        
        
        public ICommand AddAttendeeCommand
        {
            get { return new DelegateCommand<object>(AddAttendeeCommandMethod); }
        }

        public ICommand DeleteAttendeeCommand
        {
            get { return new DelegateCommand<object>(DeleteAttendeeCommandMethod); }
        }

        public ICommand DeleteAttachedFileCommand
        {
            get { return new DelegateCommand<object>(DeleteAttachedFileCommandMethod); }
        }

        public ICommand AddIndustryReportCommand
        {
            get { return new DelegateCommand<object>(AddIndustryReportCommandMethod); }
        }

        public ICommand AddOtherDocumentCommand
        {
            get { return new DelegateCommand<object>(AddOtherDocumentCommandMethod); }
        }

        public ICommand SaveCommand
        {
            get { return new DelegateCommand<object>(SaveCommandMethod); }
        }

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

        #region CallBack Methods

        private void RetrieveMeetingInfoByPresentationStatusCallbackMethod(List<MeetingInfo> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    ClosedForVotingMeetingInfo = result;
                    //if (result.Count > 0)
                    //{
                    //    SelectedClosedForVotingMeetingInfo = result[0];
                    //}
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
            Logging.LogEndMethod(_logger, methodNamespace);
            BusyIndicatorNotification();
        }

        private void RetrieveMeetingMinuteDetailsCallbackMethod(List<MeetingMinuteData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    ClosedForVotingMeetingMinuteInfo = result;
                    if (_dbInteractivity != null)
                    {
                        BusyIndicatorNotification(true, "Retrieving Attached document details for the selected meeting");
                        _dbInteractivity.RetrieveMeetingAttachedFileDetails(SelectedClosedForVotingMeetingInfo.MeetingID, RetrieveMeetingAttachedFileDetailsCallbackMethod);
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
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        private void RetrieveMeetingAttachedFileDetailsCallbackMethod(List<MeetingAttachedFileData> result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    List<MeetingAttachedFileStreamData> attachedFileStreamData = new List<MeetingAttachedFileStreamData>();
                    foreach (MeetingAttachedFileData attachedFileData in result)
                    {
                        attachedFileStreamData.Add(new MeetingAttachedFileStreamData() { MeetingAttachedFileData = attachedFileData });
                    }
                    ClosedForVotingMeetingAttachedFileInfo = attachedFileStreamData;
                    BusyIndicatorNotification();
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
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        private void UpdateMeetingMinuteDetailsCallbackMethod(Boolean? result)
        {
            string methodNamespace = String.Format("{0}.{1}", GetType().FullName, System.Reflection.MethodInfo.GetCurrentMethod().Name);
            Logging.LogBeginMethod(_logger, methodNamespace);
            try
            {
                if (result != null)
                {
                    Logging.LogMethodParameter(_logger, methodNamespace, result, 1);
                    
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
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        private void UpdateMeetingAttachedFileStreamDataCallbackMethod_DeleteAttachedFile(Boolean? result)
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
                        ClosedForVotingMeetingAttachedFileInfo = ClosedForVotingMeetingAttachedFileInfo
                            .Where(record => record != SelectedDeleteFileStreamData).ToList();
                        SelectedDeleteFileStreamData = null;
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
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        private void UpdateMeetingAttachedFileStreamDataCallbackMethod_AddIndustryReport(Boolean? result)
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
                        List<MeetingAttachedFileStreamData> meetingAttachedFileStreamData = new List<MeetingAttachedFileStreamData>
                            (ClosedForVotingMeetingAttachedFileInfo);
                        meetingAttachedFileStreamData.Add(SelectedIndustryReportFileStreamData);
                        ClosedForVotingMeetingAttachedFileInfo = meetingAttachedFileStreamData;

                        SelectedIndustryReports = null;
                        SelectedIndustryReportFileStreamData = null;
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
            Logging.LogEndMethod(_logger, methodNamespace);
        }

        private void UpdateMeetingAttachedFileStreamDataCallbackMethod_AddOtherDocument(Boolean? result)
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
                        List<MeetingAttachedFileStreamData> meetingAttachedFileStreamData = new List<MeetingAttachedFileStreamData>
                            (ClosedForVotingMeetingAttachedFileInfo);
                        meetingAttachedFileStreamData.Add(SelectedOtherDocumentFileStreamData);
                        ClosedForVotingMeetingAttachedFileInfo = meetingAttachedFileStreamData;

                        SelectedOtherReports = null;
                        SelectedOtherDocumentFileStreamData = null;
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
            Logging.LogEndMethod(_logger, methodNamespace);
        }
        
        #endregion      


        /// <summary>
        /// Display/Hide Busy Indicator
        /// </summary>
        /// <param name="showBusyIndicator">True to display indicator; default false</param>
        /// <param name="message">Content message for indicator; default null</param>
        public void BusyIndicatorNotification(bool showBusyIndicator = false, String message = null)
        {
            if (message != null)
                BusyIndicatorContent = message;

            BusyIndicatorIsBusy = showBusyIndicator;
        }

        private void AddAttendeeCommandMethod(object param)
        {
            ChildViewPresentationMeetingMinutesAddAttendee cVPresentationMeetingMinutesAddAttendee =
                new ChildViewPresentationMeetingMinutesAddAttendee();
            cVPresentationMeetingMinutesAddAttendee.Show();
            cVPresentationMeetingMinutesAddAttendee.Unloaded += (se, e) =>
            {
                if (cVPresentationMeetingMinutesAddAttendee.DialogResult.Equals(true))
                {
                    if (ClosedForVotingMeetingMinuteInfo.Any(record => record.Name == cVPresentationMeetingMinutesAddAttendee.SelectedUser))
                        return;

                    List<MeetingMinuteData> closedForVotingMeetingMinuteInfo = new List<MeetingMinuteData>(ClosedForVotingMeetingMinuteInfo);

                    foreach (MeetingMinuteData item in ClosedForVotingMeetingMinuteDistinctPresentationInfo)
                    {
                        closedForVotingMeetingMinuteInfo.Add(new MeetingMinuteData()
                        {
                            PresentationID = item.PresentationID,
                            Presenter = item.Presenter,
                            SecurityName = item.SecurityName,
                            SecurityTicker = item.SecurityTicker,
                            SecurityCountry = item.SecurityCountry,
                            SecurityIndustry = item.SecurityIndustry,
                            Name = cVPresentationMeetingMinutesAddAttendee.SelectedUser,
                            AttendanceType = cVPresentationMeetingMinutesAddAttendee.SelectedAttendanceType                            
                        });                         
                    }

                    ClosedForVotingMeetingMinuteInfo = closedForVotingMeetingMinuteInfo;
                }
            };
        }

        private void DeleteAttendeeCommandMethod(object param)
        {
            if (param is MeetingMinuteData)
            {
                MeetingMinuteData deletionAttendeeData = param as MeetingMinuteData;
                ClosedForVotingMeetingMinuteInfo = ClosedForVotingMeetingMinuteInfo
                    .Where(record => record.Name != deletionAttendeeData.Name).ToList();
            }
        }

        private void DeleteAttachedFileCommandMethod(object param)
        {
            if (param is MeetingAttachedFileStreamData)
            {
                SelectedDeleteFileStreamData = param as MeetingAttachedFileStreamData;
                Prompt.ShowDialog(messageText: "This action will permanently delete attachment from system. Do you wish to continue?", buttonType: MessageBoxButton.OKCancel, messageBoxResult: (result) =>
                {
                    if (result == MessageBoxResult.OK)
                    {
                        _dbInteractivity.UpdateMeetingAttachedFileStreamData(GreenField.UserSession.SessionManager.SESSION.UserName
                            , SelectedDeleteFileStreamData, UpdateMeetingAttachedFileStreamDataCallbackMethod_DeleteAttachedFile);                                                
                    }
                });                
            }
        }

        private void AddIndustryReportCommandMethod(object param)
        {
            _dbInteractivity.UpdateMeetingAttachedFileStreamData(GreenField.UserSession.SessionManager.SESSION.UserName
                , SelectedIndustryReportFileStreamData, UpdateMeetingAttachedFileStreamDataCallbackMethod_AddIndustryReport);            
        }

        private void AddOtherDocumentCommandMethod(object param)
        {
            _dbInteractivity.UpdateMeetingAttachedFileStreamData(GreenField.UserSession.SessionManager.SESSION.UserName
                , SelectedOtherDocumentFileStreamData, UpdateMeetingAttachedFileStreamDataCallbackMethod_AddOtherDocument);
        }

        private void SaveCommandMethod(object param)
        {
            foreach (MeetingMinuteData meetingMinuteData in ClosedForVotingMeetingMinuteDistinctAttendanceInfo)
            {
                List<MeetingMinuteData> voterSpecificMeetingMinuteData = ClosedForVotingMeetingMinuteInfo
                    .Where(record => record.Name == meetingMinuteData.Name).ToList();

                foreach (MeetingMinuteData voterMeetingMinuteData in voterSpecificMeetingMinuteData)
                {
                    voterMeetingMinuteData.AttendanceType = meetingMinuteData.AttendanceType;
                }
            }            

            _dbInteractivity.UpdateMeetingMinuteDetails(GreenField.UserSession.SessionManager.SESSION.UserName, SelectedClosedForVotingMeetingInfo, 
                ClosedForVotingMeetingMinuteInfo, UpdateMeetingMinuteDetailsCallbackMethod);
        }        

        public void Dispose()
        {
        }

        
    }
}
