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

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[NewPerformanceData](
	[INDE] [int] NULL,
	[TO_DATE] [datetime] NULL,
	[PORTYPE] [varchar](30) NULL,
	[PORTFOLIOGROUP] [varchar](30) NULL,
	[PORTFOLIO] [varchar](30) NULL,
	[RETURN_TYPE] [varchar](30) NULL,
	[CURRENCY] [varchar](30) NULL,
	[YEARS] [varchar](30) NULL,
	[BMID] [varchar](30) NULL,
	[BMNAME] [varchar](30) NULL,
	[POR_INCEPTION_DATE] [date] NULL,
	[RC_VOL] [decimal](10, 0) NULL,
	[RC_TRACKERROR] [decimal](10, 0) NULL,
	[RC_INFORMATION] [decimal](10, 0) NULL,
	[RC_ALPHA] [decimal](10, 0) NULL,
	[RC_BETA] [decimal](10, 0) NULL,
	[RC_R2] [decimal](10, 0) NULL,
	[RC_CORRELATION] [decimal](10, 0) NULL
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00002'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())






