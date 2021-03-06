set noexec off

--declare  current and required version
declare @RequiredDBVersion as nvarchar(100) = '00115'
declare @CurrentScriptVersion as nvarchar(100) = '00116'
--if current version already in DB, just skip
if exists(select 1 from ChangeScripts  where ScriptVersion = @CurrentScriptVersion)
 set noexec on 

--check that current DB version is Ok
declare @DBCurrentVersion as nvarchar(100) = (select top 1 ScriptVersion from ChangeScripts order by DateExecuted desc)
if (@DBCurrentVersion != @RequiredDBVersion)
begin
	RAISERROR(N'DB version is "%s", required "%s".', 16, 1, @DBCurrentVersion, @RequiredDBVersion)
	set noexec on
end

GO

IF OBJECT_ID ('[dbo].[SetICPMeetingPresentationDate]') IS NOT NULL
	DROP PROCEDURE [dbo].[SetICPMeetingPresentationDate]
GO

CREATE PROCEDURE [dbo].[SetICPMeetingPresentationDate] 
	@UserName VARCHAR(50),
	@PresentationId BIGINT,
	@MeetingId BIGINT,
	@MeetingDateTime DATETIME,
	@MeetingClosedDateTime DATETIME,
	@MeetingVotingClosedDateTime DATETIME
AS
BEGIN
	SET NOCOUNT ON;
	BEGIN TRANSACTION
		
		DECLARE @MeetingRecordCount INT
		SELECT @MeetingRecordCount = COUNT(*) FROM MeetingInfo 
		WHERE MeetingID = @MeetingId
		
		IF @@ERROR <> 0
		BEGIN
			ROLLBACK TRANSACTION
			SELECT -1		
			RETURN	
		END
			
		DECLARE @ProposedMeetingId BIGINT
		SET @ProposedMeetingId = @MeetingId		
		
		IF @MeetingRecordCount = 0
		BEGIN
			INSERT INTO MeetingInfo ( MeetingDateTime, MeetingClosedDateTime, MeetingVotingClosedDateTime, MeetingDescription
			, CreatedBy, CreatedOn, ModifiedBy, ModifiedOn)
			VALUES ( @MeetingDateTime, @MeetingClosedDateTime, @MeetingVotingClosedDateTime, NULL
			, @UserName, GETUTCDATE(), @UserName, GETUTCDATE())
			
			SET @ProposedMeetingId = @@IDENTITY
			
			IF @@ERROR <> 0
			BEGIN
				ROLLBACK TRANSACTION
				SELECT -2
				RETURN
			END
		END
		
		DECLARE @MeetingPresentationMappingRecordCount BIGINT
		SELECT @MeetingPresentationMappingRecordCount = COUNT(*) FROM MeetingPresentationMappingInfo
		WHERE [PresentationID] = @PresentationId
		
		IF @MeetingPresentationMappingRecordCount > 0
		BEGIN
			UPDATE MeetingPresentationMappingInfo 
			SET MeetingId = @ProposedMeetingId,
				ModifedBy = @UserName,
				ModifiedOn = GETUTCDATE()
			WHERE PresentationID = @PresentationId	
		END
		ELSE
		BEGIN
			INSERT INTO MeetingPresentationMappingInfo ([MeetingID], [PresentationID], [CreatedBy], [CreatedOn], [ModifedBy], [ModifiedOn])
			VALUES (@ProposedMeetingId, @PresentationId, @UserName, GETUTCDATE(), @UserName, GETUTCDATE())			
		END
		
		
		IF @@ERROR <> 0
		BEGIN
			ROLLBACK TRANSACTION
			SELECT -3
			RETURN
		END	
		
		DECLARE @ProposedMeetingDateTime DATETIME
		SELECT @ProposedMeetingDateTime = MeetingInfo.[MeetingDateTime] FROM MeetingInfo WHERE [MeetingID] = @ProposedMeetingId
		
		UPDATE PresentationInfo SET 
		CommitteeRangeEffectiveThrough = DATEADD(MI, 3, @ProposedMeetingDateTime)
		WHERE [PresentationID] = @PresentationId
		
		IF @@ERROR <> 0
		BEGIN
			ROLLBACK TRANSACTION
			SELECT -4
			RETURN
		END	
		
	COMMIT TRANSACTION	
	SELECT 0    
END
GO

--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00116'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())
