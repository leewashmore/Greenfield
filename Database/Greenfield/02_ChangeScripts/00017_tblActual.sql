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

CREATE TABLE [dbo].[tblActual](
	[XRef] [varchar](9) NOT NULL,
	[PeriodType] [varchar](2) NOT NULL,
	[EstimateType] [varchar](20) NOT NULL,
	[fYearEnd] [char](6) NOT NULL,
	[fPeriodEnd] [char](6) NOT NULL,
	[PeriodEndDate] [smalldatetime] NOT NULL,
	[Unit] [varchar](2) NULL,
	[ActualValue] [real] NULL,
	[AnnouncementDate] [datetime] NULL,
	[UpdateDate] [datetime] NULL,
	[Currency] [varchar](3) NULL
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00002'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())






