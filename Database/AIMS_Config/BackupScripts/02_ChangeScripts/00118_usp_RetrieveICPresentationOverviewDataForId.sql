set noexec off

--declare  current and required version
declare @RequiredDBVersion as nvarchar(100) = '00117'
declare @CurrentScriptVersion as nvarchar(100) = '00118'
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

IF OBJECT_ID ('[dbo].[RetrieveICPresentationOverviewDataForId]') IS NOT NULL
	DROP PROCEDURE [dbo].[RetrieveICPresentationOverviewDataForId]
GO

CREATE PROCEDURE [dbo].[RetrieveICPresentationOverviewDataForId]
	@PresentationId BIGINT
AS
BEGIN
	
	SELECT 
	_PI.*,
	_MI.MeetingDateTime,
	_MI.MeetingClosedDateTime,
	_MI.MeetingVotingClosedDateTime
	
	FROM 
	PresentationInfo  _PI  
	LEFT JOIN MeetingPresentationMappingInfo _MPMI ON _MPMI.PresentationID = _PI.PresentationID
	LEFT JOIN MeetingInfo _MI ON _MPMI.MeetingID = _MI.MeetingID
	WHERE _PI.PresentationID = @PresentationId
	ORDER BY MeetingDateTime DESC, SecurityName ASC
END
GO

--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00118'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())
