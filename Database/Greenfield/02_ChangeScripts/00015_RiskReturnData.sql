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

CREATE TABLE [dbo].[RiskReturnData](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[INDE] [int] NULL,
	[TO_DATE] [datetime] NULL,
	[PORTYPE] [varchar](100) NULL,
	[PORTFOLIOGROUP] [varchar](100) NULL,
	[PORTFOLIO] [varchar](100) NULL,
	[PORTFOLIOCODE] [varchar](100) NULL,
	[RETURN_TYPE] [varchar](100) NULL,
	[CURRENCY] [varchar](100) NULL,
	[YEAR] [varchar](100) NULL,
	[BMID] [varchar](100) NULL,
	[BMNAME] [varchar](100) NULL,
	[POR_INCEPTION_DATE] [datetime] NULL,
	[RC_VOL] [decimal](18, 0) NULL,
	[RC_TRACKERROR] [decimal](18, 0) NULL,
	[RC_INFORMATION] [decimal](18, 0) NULL,
	[RC_ALPHA] [decimal](18, 0) NULL,
	[RC_BETA] [decimal](18, 0) NULL,
	[RC_R2] [decimal](18, 0) NULL,
	[RC_CORRELATION] [decimal](18, 0) NULL,
	[RC_SHARPE] [decimal](18, 0) NULL
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00002'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())






