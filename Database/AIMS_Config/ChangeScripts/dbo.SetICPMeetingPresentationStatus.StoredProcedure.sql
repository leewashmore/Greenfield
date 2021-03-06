SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
alter procedure [dbo].[SetICPMeetingPresentationStatus] 
	@UserName VARCHAR(50),
	@MeetingId BIGINT,	
	@Status VARCHAR(50)	
AS
BEGIN
	SET NOCOUNT ON;
	BEGIN TRANSACTION
	
		UPDATE PresentationInfo 
		SET StatusType = @Status,
			ModifiedBy = @UserName,
			ModifiedOn = GETUTCDATE()
		WHERE PresentationID IN
		(SELECT PresentationID FROM MeetingPresentationMappingInfo
			WHERE MeetingID = @MeetingId)
		
		
		IF @@ERROR <> 0
		BEGIN
			ROLLBACK TRANSACTION
			SELECT -1
			RETURN
		END	
		
	COMMIT TRANSACTION	
	SELECT 0    
END
GO
