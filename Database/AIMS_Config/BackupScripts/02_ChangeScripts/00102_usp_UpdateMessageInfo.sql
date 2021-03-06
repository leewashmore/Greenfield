set noexec off

--declare  current and required version
declare @RequiredDBVersion as nvarchar(100) = '00101'
declare @CurrentScriptVersion as nvarchar(100) = '00102'
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

IF OBJECT_ID ('[dbo].[UpdateMessageInfo]') IS NOT NULL
	DROP PROCEDURE [dbo].[UpdateMessageInfo]
GO

CREATE PROCEDURE [dbo].[UpdateMessageInfo] 	
@EmailId BIGINT,
@EmailSent BIT,
@UserName VARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON;
	
	UPDATE MessageInfo SET
	[EmailSent] = @EmailSent,
	[ModifiedBy] = @UserName,
	[ModifiedOn] = GETUTCDATE()
	WHERE [EmailId] = @EmailId
	
	IF @@ERROR <> 0
	BEGIN
		SELECT -1
		RETURN
	END
	
	SELECT 0
END
GO

--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00102'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())
