USE [AIMS_Config]
GO
/****** Object:  StoredProcedure [dbo].[RetrieveICPresentationOverviewData]    Script Date: 09/22/2014 17:23:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
ALTER procedure [dbo].[RetrieveICPresentationOverviewData]
	@userid varchar(100),
	@statusType varchar(100)
	
	-- Add the parameters for the stored procedure here
AS
BEGIN
	if @statusType = '' and @userid <> ''
	begin
		SELECT 
		_PI.*,
		S.ISSUER_NAME,
		_MI.MeetingDateTime,
		_MI.MeetingClosedDateTime,
		_MI.MeetingVotingClosedDateTime
		
		FROM 
		PresentationInfo  _PI  
		LEFT JOIN MeetingPresentationMappingInfo _MPMI ON _MPMI.PresentationID = _PI.PresentationID
		LEFT JOIN MeetingInfo _MI ON _MPMI.MeetingID = _MI.MeetingID
		LEFT JOIN AIMS_MAIN.DBO.GF_SECURITY_BASEVIEW S ON S.SECURITY_ID = _PI.SECURITY_ID AND S.ISSUER_ID = _PI.ISSUER_ID
		where _PI.Presenter = @userid  --and statustype = @statusType
		and statustype  <> 'Decision Published'
		ORDER BY MeetingDateTime DESC, SecurityName ASC
	end
	else if @userid = '' and @statusType = 'VotingDecision'
	begin
		SELECT 
		_PI.*,
		S.ISSUER_NAME,
		_MI.MeetingDateTime,
		_MI.MeetingClosedDateTime,
		_MI.MeetingVotingClosedDateTime
		
		FROM 
		PresentationInfo  _PI  
		LEFT JOIN MeetingPresentationMappingInfo _MPMI ON _MPMI.PresentationID = _PI.PresentationID
		LEFT JOIN MeetingInfo _MI ON _MPMI.MeetingID = _MI.MeetingID
		LEFT JOIN AIMS_MAIN.DBO.GF_SECURITY_BASEVIEW S ON S.SECURITY_ID = _PI.SECURITY_ID AND S.ISSUER_ID = _PI.Issuer_ID
		Where  statustype  <> 'Decision Published'
		ORDER BY MeetingDateTime DESC, SecurityName ASC
	end
	else if @userid = '' and @statusType = 'Voting'
	begin
		SELECT 
		_PI.*,
		S.ISSUER_NAME,
		_MI.MeetingDateTime,
		_MI.MeetingClosedDateTime,
		_MI.MeetingVotingClosedDateTime
		
		FROM 
		PresentationInfo  _PI  
		LEFT JOIN MeetingPresentationMappingInfo _MPMI ON _MPMI.PresentationID = _PI.PresentationID
		LEFT JOIN MeetingInfo _MI ON _MPMI.MeetingID = _MI.MeetingID
		LEFT JOIN AIMS_MAIN.DBO.GF_SECURITY_BASEVIEW S ON S.SECURITY_ID = _PI.SECURITY_ID AND S.ISSUER_ID = _PI.ISSUER_ID
		where statustype in ('Ready for Voting','Voting Closed')
		ORDER BY MeetingDateTime DESC, SecurityName ASC
	end
	else if @userid = '' and @statusType = 'Decision Entered'
	begin
		SELECT 
		_PI.*,
		S.ISSUER_NAME,
		_MI.MeetingDateTime,
		_MI.MeetingClosedDateTime,
		_MI.MeetingVotingClosedDateTime
		
		FROM 
		PresentationInfo  _PI  
		LEFT JOIN MeetingPresentationMappingInfo _MPMI ON _MPMI.PresentationID = _PI.PresentationID
		LEFT JOIN MeetingInfo _MI ON _MPMI.MeetingID = _MI.MeetingID
		LEFT JOIN AIMS_MAIN.DBO.GF_SECURITY_BASEVIEW S ON S.SECURITY_ID = _PI.SECURITY_ID AND S.ISSUER_ID = _PI.ISSUER_ID
		where statustype in ('Decision Entered')
		ORDER BY MeetingDateTime DESC, SecurityName ASC
	end
END

-- exec [dbo].[RetrieveICPresentationOverviewData]