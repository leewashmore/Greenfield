--declare  current and required version
declare @RequiredDBVersion as nvarchar(100) = '00061'
declare @CurrentScriptVersion as nvarchar(100) = '00062'
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

/****** Object:  StoredProcedure [dbo].[GetMeetingConfigSchedule]    Script Date: 08/29/2012 14:44:30 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Sneha Sharma
-- Create date: 14-08-2012
-- Description:	Fetch the System Configuration Settings
-- =============================================
CREATE PROCEDURE [dbo].[GetMeetingConfigSchedule]
	-- Add the parameters for the stored procedure here
AS
BEGIN
	
	SELECT 
	config.PresentationDateTime,
	config.PresentationTimeZone,
	config.PreMeetingVotingDeadline,
	config.PresentationDeadline,
	config.ConfigurablePresentationDeadline,
	config.ConfigurablePreMeetingVotingDeadline
	
	FROM 
	  MeetingConfigurationSchedule  config	
END

GO


--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00062'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())