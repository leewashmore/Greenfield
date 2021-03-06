SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
alter procedure [dbo].[GetPresentationVotingDeadlineDetails] 	
@ScheduleMinutes INT
AS
BEGIN
	SET NOCOUNT ON;
	
	DECLARE @CURRENT_UTC DATETIME = GETUTCDATE()
	DECLARE @MARGINAL_UTC DATETIME = DATEADD(MI, -1 * @ScheduleMinutes, @CURRENT_UTC)
	
	SELECT _MI.MeetingID, _MI.MeetingDateTime, _MI.MeetingClosedDateTime, 
		_PI.PresentationID, _PI.Presenter, _PI.SecurityName, _PI.SecurityTicker, _PI.SecurityCountry, _PI.SecurityBuyRange, _PI.SecuritySellRange, _PI.SecurityPFVMeasure, _PI.SecurityRecommendation,
		_VI.VoterID, _VI.Name, _VI.VoteType, _VI.VoterBuyRange, _VI.VoterSellRange, _VI.DiscussionFlag, _VI.Notes,
		_FM.FileID, _FM.[Type], _FM.Category, _FM.Name as [FileName], _FM.Location
	FROM MeetingPresentationMappingInfo _MPMI
	LEFT JOIN MeetingInfo _MI ON _MI.MeetingID = _MPMI.MeetingID
	LEFT JOIN PresentationInfo _PI ON _PI.PresentationID = _MPMI.PresentationID
	LEFT JOIN PresentationAttachedFileInfo _PAFI ON _PAFI.PresentationID = _PI.PresentationID
	LEFT JOIN FileMaster _FM ON _FM.FileID = _PAFI.FileID
	LEFT JOIN VoterInfo _VI ON _VI.PresentationID = _PI.PresentationID
	WHERE _PI.StatusType = 'Ready for Voting'
	AND _MI.MeetingVotingClosedDateTime >= @MARGINAL_UTC 
	AND _MI.MeetingVotingClosedDateTime <= @CURRENT_UTC
	AND _vi.PostMeetingFlag = 'False'
	
END
GO
