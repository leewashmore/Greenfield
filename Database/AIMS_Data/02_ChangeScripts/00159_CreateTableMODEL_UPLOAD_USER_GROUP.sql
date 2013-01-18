set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00158'
declare @CurrentScriptVersion as nvarchar(100) = '00159'

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

IF OBJECT_ID ('[dbo].[MODEL_UPLOAD_USER_GROUP]') IS NOT NULL
	DROP TABLE [dbo].[MODEL_UPLOAD_USER_GROUP]
GO

/****** Object:  Table [dbo].[MODEL_UPLOAD_USER_GROUP]    Script Date: 01/18/2013 13:51:52 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[MODEL_UPLOAD_USER_GROUP](
	[MANAGER_NAME] [varchar](50) NOT NULL,
	[ANALYST_NAME] [varchar](50) NOT NULL,
 CONSTRAINT [PK_MODEL_UPLOAD_USER_GROUP] PRIMARY KEY CLUSTERED 
(
	[MANAGER_NAME] ASC,
	[ANALYST_NAME] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO



Go

--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00159'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())

