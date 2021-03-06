set noexec off

--declare  current and required version
declare @RequiredDBVersion as nvarchar(100) = '00083'
declare @CurrentScriptVersion as nvarchar(100) = '00084'
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

IF OBJECT_ID ('[dbo].[SetICPPresentationStatus]') IS NOT NULL
	DROP PROCEDURE [dbo].[SetICPPresentationStatus]
GO

CREATE PROCEDURE [dbo].[SetICPPresentationStatus] 
	@UserName VARCHAR(50),
	@PresentationId BIGINT,	
	@Status VARCHAR(50)	
AS
BEGIN
	SET NOCOUNT ON;
	BEGIN TRANSACTION
	
		UPDATE PresentationInfo 
		SET StatusType = @Status,
			ModifiedBy = @UserName,
			ModifiedOn = GETUTCDATE()
		WHERE PresentationID = @PresentationId		
		
		IF	@Status = 'Withdrawn'
		BEGIN
			DELETE FROM MeetingPresentationMappingInfo
			WHERE [PresentationID] = @PresentationId
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

--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00084'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())
