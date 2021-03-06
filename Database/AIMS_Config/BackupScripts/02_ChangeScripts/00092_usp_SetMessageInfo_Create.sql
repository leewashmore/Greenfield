set noexec off

--declare  current and required version
declare @RequiredDBVersion as nvarchar(100) = '00091'
declare @CurrentScriptVersion as nvarchar(100) = '00092'
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

IF OBJECT_ID ('[dbo].[SetMessageInfo]') IS NOT NULL
	DROP PROCEDURE [dbo].[SetMessageInfo]
GO

CREATE PROCEDURE [dbo].[SetMessageInfo] 	
@EmailTo VARCHAR(MAX),
@EmailCc VARCHAR(MAX),
@EmailSubject VARCHAR(255),
@EmailMessageBody VARCHAR(MAX),
@EmailAttachment VARCHAR(MAX),
@UserName VARCHAR(50)
AS
BEGIN
	SET NOCOUNT ON;
	
	INSERT INTO MessageInfo ( [EmailTo], [EmailCc], [EmailSubject], [EmailMessageBody], [EmailAttachment], [EmailSent]
	, [CreatedBy], [CreatedOn], [ModifiedBy], [ModifiedOn] )
	VALUES ( @EmailTo, @EmailCc, @EmailSubject, @EmailMessageBody, @EmailAttachment, 'False'
	, @UserName, GETUTCDATE(), @UserName, GETUTCDATE())
	
	IF @@ERROR <> 0
	BEGIN
		SELECT -1
		RETURN
	END
	
	SELECT 0
END
GO

--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00092'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())
