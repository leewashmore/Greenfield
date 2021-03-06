SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
alter procedure [dbo].[SetICPPresentationStatus] 
	@UserName VARCHAR(50),
	@PresentationId BIGINT,	
	@Status VARCHAR(50)	
AS
BEGIN
	SET NOCOUNT ON;
	Declare @Presenter varchar(50);
	Declare @meetingid int;
	BEGIN TRANSACTION

	
		
		IF	@Status = 'Withdrawn'  -- Setting it back to inprogress
		BEGIN
			UPDATE PresentationInfo 
			SET StatusType = 'In Progress',
				ModifiedBy = @UserName,
				ModifiedOn = GETUTCDATE()
			WHERE PresentationID = @PresentationId		
			
			select @presenter = Presenter from presentationinfo where presentationid = @presentationId
			
			Update Voterinfo set	-- clearing voting information
					notes=null,
					votetype=null,
					attendancetype = null,
					discussionflag = 0,
					VoterPFVMeasure=null,
					VoterBuyRange=null,
					voterSellRange=null,
					VoterRecommendation=null
			where presentationid=@presentationid and name <> @presenter and PostMeetingFlag =0
			
			Update Voterinfo set	-- clearing voting information
					notes=null,
					votetype=null,
					attendancetype = null,
					discussionflag = 0,
					VoterPFVMeasure=null,
					VoterBuyRange=null,
					voterSellRange=null,
					VoterRecommendation=null
			where presentationid=@presentationid and name <> @presenter and PostMeetingFlag =1
			
			select @meetingid=meetingid from MeetingPresentationMappingInfo where presentationid = @PresentationId
		
		-- this table will always update the most current day
			update meetinginfo set 
				MeetingDateTime = GETUTCDATE(),
				MeetingClosedDateTime = GETUTCDATE(),  
			    MeetingVotingClosedDateTime = GETUTCDATE() 
			where meetingid = @meetingid
				
				--DELETE FROM MeetingPresentationMappingInfo
				--WHERE [PresentationID] = @PresentationId
		END
		ELSE
		BEGIN
				UPDATE PresentationInfo 
				SET StatusType = @Status,
				ModifiedBy = @UserName,
				ModifiedOn = GETUTCDATE()
				WHERE PresentationID = @PresentationId		
				
							
			select @meetingid=meetingid from MeetingPresentationMappingInfo where presentationid = @PresentationId
		
		-- this table will always update the most current day
			update meetinginfo set 
				MeetingDateTime = GETUTCDATE(),
				MeetingClosedDateTime = GETUTCDATE(),  
			    MeetingVotingClosedDateTime = GETUTCDATE() 
			where meetingid = @meetingid
				
		END
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
