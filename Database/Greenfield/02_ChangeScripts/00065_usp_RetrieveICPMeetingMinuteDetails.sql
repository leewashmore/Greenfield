--declare  current and required version
declare @RequiredDBVersion as nvarchar(100) = '00064'
declare @CurrentScriptVersion as nvarchar(100) = '00065'
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

/****** Object:  StoredProcedure [dbo].[RetrieveICPMeetingMinuteDetails]    Script Date: 08/29/2012 14:47:48 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Rahul Vig
-- Create date: 18-08-2012
-- Description:	Retrieve Meeting Minute Details
-- =============================================
CREATE PROCEDURE [dbo].[RetrieveICPMeetingMinuteDetails] 	
	@MeetingId BIGINT = 0
AS
BEGIN
	SET NOCOUNT ON;
	
	SELECT 
	_PI.PresentationID,
	_PI.Presenter,	
	_PI.SecurityName,
	_PI.SecurityTicker,
	_PI.SecurityCountry,
	_PI.SecurityIndustry,
	_VI.VoterID,
	_VI.Name,
	_VI.AttendanceType
	FROM 
	PresentationInfo  _PI  
	LEFT JOIN MeetingPresentationMappingInfo _MPMI ON _MPMI.PresentationID = _PI.PresentationID
	LEFT JOIN MeetingInfo _MI ON _MPMI.MeetingID = _MI.MeetingID
	LEFT JOIN VoterInfo _VI ON _VI.PresentationID = _PI.PresentationID
	
	WHERE _MI.MeetingID = @MeetingId AND _PI.StatusType = 'Closed for Voting'    
END

GO

--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00065'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())
