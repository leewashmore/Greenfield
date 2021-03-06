SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
alter procedure [dbo].[RetrieveICPresentationOverviewDataForId]
	@PresentationId BIGINT
AS
BEGIN
	
	SELECT 
	_PI.*,
	_MI.MeetingDateTime,
	_MI.MeetingClosedDateTime,
	_MI.MeetingVotingClosedDateTime
	
	FROM 
	PresentationInfo  _PI  
	LEFT JOIN MeetingPresentationMappingInfo _MPMI ON _MPMI.PresentationID = _PI.PresentationID
	LEFT JOIN MeetingInfo _MI ON _MPMI.MeetingID = _MI.MeetingID
	WHERE _PI.PresentationID = @PresentationId
	ORDER BY MeetingDateTime DESC, SecurityName ASC
END
GO
