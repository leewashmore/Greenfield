using System;
using System.ComponentModel.Composition;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using GreenField.ServiceCaller.MeetingServiceReference;


namespace GreenField.ServiceCaller
{
    [Export]
    public class ManageMeetings
    {
        #region Meetings
        
        public void GetMeetings(Action<List<MeetingInfo>> callback)
        {
            MeetingServiceClient client = new MeetingServiceClient();

            client.GetMeetingsCompleted += (se, e) =>
                {
                    Action<List<MeetingInfo>> callBackMethod = e.UserState as Action<List<MeetingInfo>>;

                    if (callBackMethod != null)
                    {
                        callBackMethod(e.Result.ToList());
                    }
                };

            client.GetMeetingsAsync(callback);
        }

        public void GetMeetingDates(Action<List<DateTime?>> callback)
        {
            MeetingServiceClient client = new MeetingServiceClient();

            client.GetMeetingDatesCompleted += (se, e) =>
            {
                Action<List<DateTime?>> callBackMethod = e.UserState as Action<List<DateTime?>>;

                if (callBackMethod != null)
                {
                    callBackMethod(e.Result.ToList());
                }
            };

            client.GetMeetingDatesAsync(callback);
        }
        
        public void GetMeetingsByDate(DateTime date, Action<List<MeetingInfo>> callback)
        {
            MeetingServiceClient client = new MeetingServiceClient();

            client.GetMeetingsByDateCompleted += (se, e) =>
                {
                    Action<List<MeetingInfo>> callBackMethod = e.UserState as Action<List<MeetingInfo>>;

                    if (callBackMethod != null)
                    {
                        callBackMethod(e.Result.ToList());
                    }
                };

            client.GetMeetingsByDateAsync(date, callback);
        }
        #endregion

        #region CRUD Operations

        public void CreateMeeting(MeetingInfo meeting, Action<string> callback)
        {
             MeetingServiceClient client = new MeetingServiceClient();

            client.CreateMeetingCompleted += (se, e) =>
            {
                Action<string> callBackMethod = e.UserState as Action<string>;

                if (callBackMethod != null)
                {
                    callBackMethod("Created successfully!");
                }
            };

            client.CreateMeetingAsync(meeting, callback);
        }

        public void UpdateMeeting(MeetingInfo meeting, Action<string> callback)
        {
            MeetingServiceClient client = new MeetingServiceClient();

            client.UpdateMeetingCompleted += (se, e) =>
            {
                Action<string> callBackMethod = e.UserState as Action<string>;

                if (callBackMethod != null)
                {
                    callBackMethod("Updated successfully!");
                }
            };

            client.UpdateMeetingAsync(meeting, callback);
        }

        public void CreateMeetingPresentationMapping(MeetingPresentationMappingInfo meetingPresentationMappingInfo, Action<string> callback)
        {
            MeetingServiceClient client = new MeetingServiceClient();

            client.CreateMeetingPresentationMappingCompleted += (se, e) =>
            {
                Action<string> callBackMethod = e.UserState as Action<string>;

                if (callBackMethod != null)
                {
                    callBackMethod("Mapping inserted successfully!");
                }
            };

            client.CreateMeetingPresentationMappingAsync(meetingPresentationMappingInfo);

        }

        public void UpdateMeetingPresentationMapping(MeetingPresentationMappingInfo meetingPresentationMappingInfo, Action<string> callback)
        {
            MeetingServiceClient client = new MeetingServiceClient();

            client.UpdateMeetingPresentationMappingCompleted += (se, e) =>
            {
                Action<string> callBackMethod = e.UserState as Action<string>;

                if (callBackMethod != null)
                {
                    callBackMethod("Mapping updated successfully!");
                }
            };

            client.UpdateMeetingPresentationMappingAsync(meetingPresentationMappingInfo, callback);

        }

        public void CreatePresentation(PresentationInfo presentation, Action<long?> callback)
        {
            MeetingServiceClient client = new MeetingServiceClient();

            client.CreatePresentationCompleted += (se, e) =>
            {
                Action<long?> callBackMethod = e.UserState as Action<long?>;

                if (callBackMethod != null)
                {
                    callBackMethod(e.Result);
                }
            };

            client.CreatePresentationAsync(presentation, callback);
        }

        public void UpdatePresentation(PresentationInfo presentationInfo, Action<string> callback)
        {
            MeetingServiceClient client = new MeetingServiceClient();

            client.UpdatePresentationCompleted += (se, e) =>
            {
                Action<string> callBackMethod = e.UserState as Action<string>;

                if (callBackMethod != null)
                {
                    callBackMethod("Status updated successfully!");
                }
            };

            client.UpdatePresentationAsync(presentationInfo, callback);

        }

        public void CreateVoterInfo(VoterInfo voterInfo, Action<string> callback)
        {
            MeetingServiceClient client = new MeetingServiceClient();
            client.CreateVoterInfoCompleted += (se, e) =>
            {
                Action<string> callBackMethod = e.UserState as Action<string>;

                if (callBackMethod != null)
                {
                    callBackMethod("Your vote has been registered.");
                }
            };
            client.CreateVoterInfoAsync(voterInfo, callback);
        }

        public void CreateFileInfo(ObservableCollection<AttachedFileInfo> fileInfoColl, Action<string> callback)
        {
            MeetingServiceClient client = new MeetingServiceClient();
            client.CreateFileInfoCompleted += (se, e) =>
            {
                Action<string> callBackMethod = e.UserState as Action<string>;

                if (callBackMethod != null)
                {
                    callBackMethod("File(s) Saved");
                }
            };
            client.CreateFileInfoAsync(fileInfoColl, callback);
        }


        #endregion

        #region Presentations

        public void GetPresentations(Action<List<PresentationInfoResult>> callback)
        {
            MeetingServiceClient client = new MeetingServiceClient();

            client.GetPresentationsCompleted += (se, e) =>
            {
                Action<List<PresentationInfoResult>> callBackMethod = e.UserState as Action<List<PresentationInfoResult>>;

                if (callBackMethod != null)
                {
                    callBackMethod(e.Result.ToList());
                }
            };

            client.GetPresentationsAsync(callback);
        }

        public void GetPresentationsByMeetingID(long meetingID, Action<List<PresentationInfoResult>> callback)
        {
            MeetingServiceClient client = new MeetingServiceClient();

            client.GetPresentationsByMeetingIDCompleted += (se, e) =>
            {
                Action<List<PresentationInfoResult>> callBackMethod = e.UserState as Action<List<PresentationInfoResult>>;

                if (callBackMethod != null)
                {
                    callBackMethod(e.Result.ToList());
                }
            };

            client.GetPresentationsByMeetingIDAsync(meetingID, callback);
        }

        public void GetPresentationsByMeetingDatePresenterStatus(DateTime? meetingDate, string presenter, string status, Action<List<PresentationInfoResult>> callback)
        {
            MeetingServiceClient client = new MeetingServiceClient();

            client.GetPresentationsByMeetingDatePresenterStatusCompleted += (se, e) =>
            {
                Action<List<PresentationInfoResult>> callBackMethod = e.UserState as Action<List<PresentationInfoResult>>;

                if (callBackMethod != null)
                {
                    callBackMethod(e.Result.ToList());
                }
            };

            client.GetPresentationsByMeetingDatePresenterStatusAsync(meetingDate, presenter, status, callback);
        }

        public void GetDistinctPresenters(Action<List<string>> callback)
        {
            MeetingServiceClient client = new MeetingServiceClient();

            client.GetDistinctPresentersCompleted += (se, e) =>
            {
                Action<List<string>> callBackMethod = e.UserState as Action<List<string>>;

                if (callBackMethod != null)
                {
                    callBackMethod(e.Result.ToList());
                }
            };

            client.GetDistinctPresentersAsync(callback);
        }

        #endregion

        #region Status Type

        public void GetStatusTypes(Action<List<StatusType>> callback)
        {
            MeetingServiceClient client = new MeetingServiceClient();

            client.GetStatusTypesCompleted += (se, e) =>
            {
                Action<List<StatusType>> callBackMethod = e.UserState as Action<List<StatusType>>;

                if (callBackMethod != null)
                {
                    callBackMethod(e.Result.ToList());
                }
            };

            client.GetStatusTypesAsync(callback);
        }

        #endregion

        #region Attached Files

        public void GetFileInfo(long presentationID, Action<List<AttachedFileInfo>> callback)
        {
            MeetingServiceClient client = new MeetingServiceClient();

            client.GetFileInfoCompleted += (se, e) =>
            {
                Action<List<AttachedFileInfo>> callBackMethod = e.UserState as Action<List<AttachedFileInfo>>;

                if (callBackMethod != null)
                {
                    callBackMethod(e.Result.ToList());
                }
            };

            client.GetFileInfoAsync(presentationID, callback);
        }

        #endregion

    }
}
