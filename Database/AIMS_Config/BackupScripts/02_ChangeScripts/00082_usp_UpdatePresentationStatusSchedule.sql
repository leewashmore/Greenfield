set noexec off

--declare  current and required version
declare @RequiredDBVersion as nvarchar(100) = '00081'
declare @CurrentScriptVersion as nvarchar(100) = '00082'
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

CREATE PROCEDURE [dbo].[UpdatePresentationStatusSchedule] 	
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

--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00082'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())
