set noexec off

--declare  current and required version
declare @RequiredDBVersion as nvarchar(100) = '00084'
declare @CurrentScriptVersion as nvarchar(100) = '00085'
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

IF OBJECT_ID ('[dbo].[SetICPMeetingPresentationStatus]') IS NOT NULL
	DROP PROCEDURE [dbo].[SetICPMeetingPresentationStatus]
GO

CREATE PROCEDURE [dbo].[SetICPMeetingPresentationStatus] 
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

--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00085'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())
