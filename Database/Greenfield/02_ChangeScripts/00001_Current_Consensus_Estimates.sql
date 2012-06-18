set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00001'
declare @CurrentScriptVersion as nvarchar(100) = '00002'

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

/****** Object:  Table [dbo].[CURRENT_CONSENSUS_ESTIMATES]    Script Date: 06/18/2012 15:34:48 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[CURRENT_CONSENSUS_ESTIMATES](
	[ISSUER_ID] [varchar](20) NOT NULL,
	[SECURITY_ID] [varchar](20) NOT NULL,
	[DATA_SOURCE] [varchar](10) NOT NULL,
	[ROOT_SOURCE] [varchar](10) NOT NULL,
	[ROOT_SOURCE_DATE] [datetime] NOT NULL,
	[PERIOD_TYPE] [char](2) NOT NULL,
	[PERIOD_YEAR] [int] NOT NULL,
	[PERIOD_END_DATE] [datetime] NOT NULL,
	[FISCAL_TYPE] [char](8) NOT NULL,
	[CURRENCY] [char](3) NOT NULL,
	[DATA_ID] [int] NOT NULL,
	[AMOUNT] [decimal](32, 6) NOT NULL,
	[NUMBER_OF_ESTIMATES] [int] NOT NULL,
	[HIGH] [decimal](32, 6) NOT NULL,
	[LOW] [decimal](32, 6) NOT NULL,
	[SOURCE_CURRENCY] [char](3) NOT NULL,
	[STANDARD_DEVIATION] [decimal](32, 6) NOT NULL,
	[AMOUNT_TYPE] [char](10) NOT NULL
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00002'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())






