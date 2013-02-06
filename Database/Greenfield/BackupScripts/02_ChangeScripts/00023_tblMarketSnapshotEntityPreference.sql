set noexec off

--declare  current and required version
--also do it an the end of the script
declare @RequiredDBVersion as nvarchar(100) = '00022'
declare @CurrentScriptVersion as nvarchar(100) = '00023'

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

CREATE TABLE [dbo].[tblMarketSnapshotEntityPreference](
	[EntityPreferenceId] [int] IDENTITY(1,1) NOT NULL,
	[GroupPreferenceId] [int] NOT NULL,
	[EntityName] [nvarchar](max) NOT NULL,
	[EntityReturnType] [nvarchar](50) NULL,
	[EntityOrder] [int] NOT NULL,
	[EntityType] [nvarchar](50) NOT NULL,
	[EntityId] [nvarchar](50) NOT NULL,
	[EntityNodeType] [nvarchar](50) NULL,
	[EntityNodeValueCode] [nvarchar](50) NULL,
	[EntityNodeValueName] [nvarchar](50) NULL,
 CONSTRAINT [UNIQUE_PRIMARY] UNIQUE CLUSTERED 
(
	[GroupPreferenceId] ASC,
	[EntityOrder] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[tblMarketSnapshotEntityPreference]  WITH CHECK ADD  CONSTRAINT [FK_tblMarketSnapshotEntityPreference_tblMarketSnapshotGroupPreference] FOREIGN KEY([GroupPreferenceId])
REFERENCES [dbo].[tblMarketSnapshotGroupPreference] ([GroupPreferenceId])
ON UPDATE CASCADE
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[tblMarketSnapshotEntityPreference] CHECK CONSTRAINT [FK_tblMarketSnapshotEntityPreference_tblMarketSnapshotGroupPreference]
GO
GO

SET ANSI_PADDING OFF
GO


--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00023'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())






