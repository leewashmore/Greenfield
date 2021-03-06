SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO
alter procedure [dbo].[SetMeetingConfigSchedule]
	-- Add the parameters for the stored procedure here
	@userName VARCHAR(50),
	@PresentationDay VARCHAR(50),
	@PresentationTime DATETIME,
	@PresentationTimeZone VARCHAR(50),
	@PresentationDeadlineDay VARCHAR(50),
	@PresentationDeadlineTime DATETIME,
	@PreMeetingVotingDeadlineDay VARCHAR(50),
	@PreMeetingVotingDeadlineTime DATETIME    
AS
BEGIN		
	
	UPDATE MeetingConfigurationSchedule   
	SET [PresentationDay] = @PresentationDay,
		[PresentationTime] = @PresentationTime,
		[PresentationTimeZone] = @PresentationTimeZone,
		[PresentationDeadlineDay] = @PresentationDeadlineDay,
		[PresentationDeadlineTime] = @PresentationDeadlineTime,
		[PreMeetingVotingDeadlineDay] = @PreMeetingVotingDeadlineDay,
		[PreMeetingVotingDeadlineTime] = @PreMeetingVotingDeadlineTime,
		[ModifiedBy] = @userName,
		[ModifiedOn] = GETUTCDATE() 	
 	if @@ERROR <> 0
 	BEGIN 
 	 SELECT -1
 	END
 	
 	SELECT 0
END
GO
