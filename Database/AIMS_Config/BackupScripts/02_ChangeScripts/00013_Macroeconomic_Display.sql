set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00012'
declare @CurrentScriptVersion as nvarchar(100) = '00013'

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

CREATE TABLE [dbo].[Macroeconomic_Display](
	[FIELD] [varchar](30) NOT NULL,
	[CATEGORY_NAME] [varchar](30) NOT NULL,
	[DESCRIPTION] [varchar](50) NOT NULL,
	[DATATYPE] [varchar](10) NOT NULL,
	[DECIMALS] [int] NULL,
	[SORT_ORDER] [int] NOT NULL,
	[HELP_TEXT] [varchar](255) NULL,
	[DISPLAY_TYPE] [varchar](30) NULL
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00013'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())






