set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00005'
declare @CurrentScriptVersion as nvarchar(100) = '00006'

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

CREATE TABLE [dbo].[STATEMENT_CONSENSUS_MAPPING](
	[STATEMENT_TYPE] [char](3) NOT NULL,
	[ESTIMATE_ID] [int] NOT NULL,
	[DESCRIPTION] [varchar](50) NOT NULL,
	[SORT_ORDER] [int] NOT NULL,
	[EARNINGS] [varchar](50) NULL
) ON [PRIMARY]

GO

--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00006'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())



