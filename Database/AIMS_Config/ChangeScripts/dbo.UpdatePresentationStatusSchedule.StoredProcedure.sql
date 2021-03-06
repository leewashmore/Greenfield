SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
alter procedure [dbo].[UpdatePresentationStatusSchedule] 	
AS
BEGIN
	SET NOCOUNT ON;
	
	DECLARE @CURRENT_UTC DATETIME = GETUTCDATE()
	DECLARE @MARGINAL_UTC DATETIME = DATEADD(MI, -15, @CURRENT_UTC)
	
	UPDATE PresentationInfo 
		SET StatusType = 'Ready for Voting',
			ModifiedBy = 'SYSTEM',
			ModifiedOn = GETUTCDATE()
		WHERE PresentationID IN
		(SELECT PresentationID FROM MeetingPresentationMappingInfo
			WHERE MeetingID IN
			(SELECT MeetingInfo.MeetingID from MeetingInfo
    WHERE MeetingClosedDateTime >= @MARGINAL_UTC 
    AND MeetingClosedDateTime <= @CURRENT_UTC))
    
    UPDATE PresentationInfo 
		SET StatusType = 'Closed for Voting',
			ModifiedBy = 'SYSTEM',
			ModifiedOn = GETUTCDATE()
		WHERE PresentationID IN
		(SELECT PresentationID FROM MeetingPresentationMappingInfo
			WHERE MeetingID IN
			(SELECT MeetingInfo.MeetingID from MeetingInfo
    WHERE MeetingVotingClosedDateTime >= @MARGINAL_UTC 
    AND MeetingVotingClosedDateTime <= @CURRENT_UTC))
END
GO
