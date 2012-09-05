--declare  current and required version
declare @RequiredDBVersion as nvarchar(100) = '00078'
declare @CurrentScriptVersion as nvarchar(100) = '00079'
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

ALTER TABLE [dbo].[FileMaster]
ADD [Category] [varchar](50) NULL
	

GO

--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00079'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())


