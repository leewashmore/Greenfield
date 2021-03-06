SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
alter procedure [dbo].[RetrieveICMeetingInfoByStatusType] 
	@PresentationStatus VARCHAR(50)	
AS
BEGIN
	SET NOCOUNT ON;
	
	SELECT DISTINCT _PI.*,
	_MI.MeetingDateTime,
	_MI.MeetingClosedDateTime,
	_MI.MeetingVotingClosedDateTime FROM
	MeetingInfo _MI
	LEFT JOIN MeetingPresentationMappingInfo _MPMI ON _MPMI.MeetingID = _MI.MeetingID
	LEFT JOIN PresentationInfo _PI ON _MPMI.PresentationID = _PI.PresentationID
	WHERE _PI.StatusType = @PresentationStatus
	ORDER BY _MI.MeetingDateTime DESC
    
END
GO
