set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00157'
declare @CurrentScriptVersion as nvarchar(100) = '00158'

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

IF OBJECT_ID ('[dbo].[Portfolio_Security_Targets_Union]') IS NOT NULL
	DROP View [dbo].[Portfolio_Security_Targets_Union]   
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE VIEW [dbo].[Portfolio_Security_Targets_Union]
AS
 SELECT
             [PORTFOLIO_SECURITY_TARGETS].[PORTFOLIO_ID] AS [PORTFOLIO_ID],
              [PORTFOLIO_SECURITY_TARGETS].[SECURITY_ID] AS [SECURITY_ID],
              [PORTFOLIO_SECURITY_TARGETS].[TARGET_PCT] AS [TARGET_PCT],
              [PORTFOLIO_SECURITY_TARGETS].[UPDATED] AS [UPDATED]
              FROM [dbo].[PORTFOLIO_SECURITY_TARGETS] AS [PORTFOLIO_SECURITY_TARGETS]
              UNION
              SELECT
              [BU_PORTFOLIO_SECURITY_TARGET].[PORTFOLIO_ID] AS [PORTFOLIO_ID],
              [BU_PORTFOLIO_SECURITY_TARGET].[SECURITY_ID] AS [SECURITY_ID],
              [BU_PORTFOLIO_SECURITY_TARGET].[TARGET] AS [TARGET_PCT],
              NULL AS [UPDATED]
              FROM [dbo].[BU_PORTFOLIO_SECURITY_TARGET] AS [BU_PORTFOLIO_SECURITY_TARGET]
              UNION
              SELECT
              [BGA_PORTFOLIO_SECURITY_FACTOR].[PORTFOLIO_ID] AS [PORTFOLIO_ID],
              [BGA_PORTFOLIO_SECURITY_FACTOR].[SECURITY_ID] AS [SECURITY_ID],
              [BGA_PORTFOLIO_SECURITY_FACTOR].FACTOR AS [TARGET_PCT],
              NULL AS [UPDATED]
              FROM [dbo].[BGA_PORTFOLIO_SECURITY_FACTOR] AS [BGA_PORTFOLIO_SECURITY_FACTOR]      

GO--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00158'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())


