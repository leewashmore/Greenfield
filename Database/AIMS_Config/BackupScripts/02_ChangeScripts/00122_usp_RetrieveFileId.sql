set noexec off

--declare  current and required version
declare @RequiredDBVersion as nvarchar(100) = '00121'
declare @CurrentScriptVersion as nvarchar(100) = '00122'
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

IF OBJECT_ID ('[dbo].[ModelRetrieveFileId]') IS NOT NULL
	DROP PROCEDURE [dbo].[ModelRetrieveFileId]
GO

CREATE PROCEDURE [dbo].[ModelRetrieveFileId]
	(
	@LOCATION VARCHAR(255)
	)
AS
BEGIN
	
	SELECT * 
	FROM FileMaster 
	WHERE Location=@LOCATION
	
END


GO

--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00122'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())
