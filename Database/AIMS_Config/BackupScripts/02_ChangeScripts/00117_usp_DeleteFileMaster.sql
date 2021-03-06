set noexec off

--declare  current and required version
declare @RequiredDBVersion as nvarchar(100) = '00116'
declare @CurrentScriptVersion as nvarchar(100) = '00117'
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

IF OBJECT_ID ('[dbo].[DeleteFileMaster]') IS NOT NULL
	DROP PROCEDURE [dbo].[DeleteFileMaster]
GO

CREATE PROCEDURE [dbo].[DeleteFileMaster] 
	-- Add the parameters for the stored procedure here
	   @fileId BIGINT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	Delete from FileMaster where FileID = @fileId	
	
END
GO

--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00117'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())
