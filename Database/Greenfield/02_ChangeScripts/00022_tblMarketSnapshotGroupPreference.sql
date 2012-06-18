set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00020'
declare @CurrentScriptVersion as nvarchar(100) = '00022'

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

CREATE TABLE [dbo].[tblMarketSnapshotGroupPreference](
	[GroupPreferenceId] [int] IDENTITY(1,1) NOT NULL,
	[SnapshotPreferenceId] [int] NOT NULL,
	[GroupName] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_tblUserGroupPreference] PRIMARY KEY CLUSTERED 
(
	[GroupPreferenceId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[tblMarketSnapshotGroupPreference]  WITH CHECK ADD  CONSTRAINT [FK_tblMarketSnapshotGroupPreference_tblMarketSnapshotPreference] FOREIGN KEY([SnapshotPreferenceId])
REFERENCES [dbo].[tblMarketSnapshotPreference] ([SnapshotPreferenceId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[tblMarketSnapshotGroupPreference] CHECK CONSTRAINT [FK_tblMarketSnapshotGroupPreference_tblMarketSnapshotPreference]
GO
GO

SET ANSI_PADDING OFF
GO


--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00022'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())






