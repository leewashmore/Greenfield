set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00153'
declare @CurrentScriptVersion as nvarchar(100) = '00154'

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

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

IF OBJECT_ID ('[dbo].[STATEMENT_CONSENSUS_MAPPING]') IS NOT NULL
	DELETE FROM [dbo].[STATEMENT_CONSENSUS_MAPPING] WHERE 
	STATEMENT_TYPE = 'INC' AND 
	(ESTIMATE_ID = 5 OR ESTIMATE_ID = 9 OR ESTIMATE_ID = 12
		OR ESTIMATE_ID = 13 OR ESTIMATE_ID = 15 OR ESTIMATE_ID = 16)	
		
	UPDATE [dbo].[STATEMENT_CONSENSUS_MAPPING] SET EARNINGS = NULL, DESCRIPTION = 'EPS' 
		WHERE STATEMENT_TYPE = 'INC' AND ESTIMATE_ID = 8
	UPDATE [dbo].[STATEMENT_CONSENSUS_MAPPING] SET EARNINGS = NULL 
		WHERE STATEMENT_TYPE = 'INC' AND ESTIMATE_ID = 11
	UPDATE [dbo].[STATEMENT_CONSENSUS_MAPPING] SET EARNINGS = NULL 
		WHERE STATEMENT_TYPE = 'INC' AND ESTIMATE_ID = 14
GO


--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00154'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())


