set noexec off

--declare  current and required version
declare @RequiredDBVersion as nvarchar(100) = '00096'
declare @CurrentScriptVersion as nvarchar(100) = '00097'
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
--Script for IC Presentation Create Table

IF OBJECT_ID ('dbo.[MessageInfo]') IS NOT NULL
	DROP TABLE dbo.[MessageInfo]
GO

CREATE TABLE [dbo].[MessageInfo](
	[EmailId] [bigint] IDENTITY(1,1) NOT NULL,
	[EmailTo] [varchar](max) NULL,
	[EmailCc] [varchar](max) NULL,
	[EmailSubject] [varchar](255) NULL,
	[EmailMessageBody] [varchar](max) NULL,
	[EmailAttachment] [varchar](max) NULL,
	[EmailSent] [bit] NULL,
	[CreatedBy] [varchar](50) NULL,
	[CreatedOn] [datetime] NULL,
	[ModifiedBy] [varchar](50) NULL,
	[ModifiedOn] [datetime] NULL,
 CONSTRAINT [PK_MessageInfo] PRIMARY KEY CLUSTERED 
(
	[EmailId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00097'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())

