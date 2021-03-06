set noexec off

--declare  current and required version
declare @RequiredDBVersion as nvarchar(100) = '00086'
declare @CurrentScriptVersion as nvarchar(100) = '00087'
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

IF OBJECT_ID ('dbo.MeetingConfigurationSchedule') IS NOT NULL
	DROP TABLE dbo.MeetingConfigurationSchedule
GO

CREATE TABLE [dbo].[MeetingConfigurationSchedule](
	[PresentationDay] [varchar](50) NULL,
	[PresentationTime] [datetime] NOT NULL,
	[PresentationTimeZone] [varchar](50) NOT NULL,
	[PresentationDeadlineDay] [varchar](50) NULL,
	[PresentationDeadlineTime] [datetime] NOT NULL,
	[PreMeetingVotingDeadlineDay] [varchar](50) NULL,
	[PreMeetingVotingDeadlineTime] [datetime] NOT NULL,
	[ConfigurablePresentationDeadline] [decimal](18, 2) NULL,
	[ConfigurablePreMeetingVotingDeadline] [decimal](18, 2) NULL,
	[CreatedBy] [varchar](50) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[ModifiedBy] [varchar](50) NOT NULL,
	[ModifiedOn] [datetime] NOT NULL
) ON [PRIMARY]
GO

INSERT INTO [dbo].[MeetingConfigurationSchedule]
VALUES ( 'Tuesday', '2012-01-01 08:30:00', 'UTC', 'Friday', '2012-01-01 17:00:00', 'Tuesday', '2012-01-01 08:00:00', 39.5, 0.5, 'System', GETUTCDATE(), 'System', GETUTCDATE())

GO

--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00087'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())
