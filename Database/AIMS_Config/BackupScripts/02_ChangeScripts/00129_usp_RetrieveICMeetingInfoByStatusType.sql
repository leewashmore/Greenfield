set noexec off

--declare  current and required version
declare @RequiredDBVersion as nvarchar(100) = '00128'
declare @CurrentScriptVersion as nvarchar(100) = '00129'
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

IF OBJECT_ID ('[dbo].[RetrieveICMeetingInfoByStatusType]') IS NOT NULL
	DROP PROCEDURE [dbo].[RetrieveICMeetingInfoByStatusType]
GO

/****** Object:  StoredProcedure [dbo].[ModelInsertInternalCommodityAssumptions]    Script Date: 08/07/2012 17:24:27 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[RetrieveICMeetingInfoByStatusType] 
	@PresentationStatus VARCHAR(50)	
AS
BEGIN
	SET NOCOUNT ON;
	
	SELECT DISTINCT _PI.*,
	_MI.MeetingDateTime,
	_MI.MeetingClosedDateTime,
	_MI.MeetingVotingClosedDateTime FROM
	MeetingInfo _MI
	LEFT JOIN MeetingPresentationMappingInfo _MPMI ON _MPMI.MeetingID = _MI.MeetingID
	LEFT JOIN PresentationInfo _PI ON _MPMI.PresentationID = _PI.PresentationID
	WHERE _PI.StatusType = @PresentationStatus
	ORDER BY _MI.MeetingDateTime DESC
    
END
Go

--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00129'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())
