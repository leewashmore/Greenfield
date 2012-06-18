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

/****** Object:  Table [dbo].[Country_Master]    Script Date: 06/18/2012 15:33:41 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[Country_Master](
	[COUNTRY_CODE] [char](4) NOT NULL,
	[COUNTRY_NAME] [varchar](30) NOT NULL,
	[CURRENCY_CODE] [char](4) NOT NULL,
	[CURRENCY_NAME] [varchar](20) NOT NULL,
	[MACRO_ECON_DATA_CURRENT] [char](1) NOT NULL,
	[ASHEMM_PROPRIETARY_REGION_NAME] [varchar](20) NULL,
	[ASHEMM_PROPRIETARY_REGION_CODE] [varchar](20) NULL
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00002'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())






