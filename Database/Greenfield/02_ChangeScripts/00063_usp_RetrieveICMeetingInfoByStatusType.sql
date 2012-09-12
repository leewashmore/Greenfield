--declare  current and required version
declare @RequiredDBVersion as nvarchar(100) = '00061'
declare @CurrentScriptVersion as nvarchar(100) = '00063'
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

/****** Object:  StoredProcedure [dbo].[RetrieveICMeetingInfoByStatusType]    Script Date: 08/29/2012 14:46:47 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- ===================================================
-- Author:		Rahul Vig
-- Create date: 18-08-2012
-- Description:	Retrieve MeetingInfo By Presentation Status
-- ===================================================
CREATE PROCEDURE [dbo].[RetrieveICMeetingInfoByStatusType] 
	@PresentationStatus VARCHAR(50)	
AS
BEGIN
	SET NOCOUNT ON;
	
	SELECT DISTINCT _MI.* FROM
	MeetingInfo _MI
	LEFT JOIN MeetingPresentationMappingInfo _MPMI ON _MPMI.MeetingID = _MI.MeetingID
	LEFT JOIN PresentationInfo _PI ON _MPMI.PresentationID = _PI.PresentationID
	WHERE _PI.StatusType = @PresentationStatus
	ORDER BY _MI.MeetingDateTime DESC
    
END

GO

--indicate thet current script is executed
declare @CurrentScriptVersion as nvarchar(100) = '00063'
insert into ChangeScripts (ScriptVersion, DateExecuted ) values (@CurrentScriptVersion, GETDATE())
