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

/****** Object:  Table [dbo].[COMMODITY_FORECASTS]    Script Date: 06/18/2012 15:29:56 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[COMMODITY_FORECASTS](
	[COMMODITY_ID] [varchar](50) NOT NULL,
	[CURRENT_YEAR_END] [real] NULL,
	[NEXT_YEAR_END] [real] NULL,
	[LONG_TERM] [real] NULL,
	[LASTUPDATE] [datetime] NULL,
 CONSTRAINT [pk_Commodity] PRIMARY KEY CLUSTERED 
(
	[COMMODITY_ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00002'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())






